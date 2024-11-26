// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using Helper;

namespace OdhApiCore
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache;

        public RateLimitingMiddleware(RequestDelegate next, IDistributedCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context, ISettings settings)
        {
            var ratelimitconfig = settings.RateLimitConfig;
            var endpoint = context.GetEndpoint();

            //If already a 401 Response is waiting do nothing
            if(context.Response.HasStarted)
            {
                return;
            }

            // If no config present do nothing
            if (ratelimitconfig is null)
            {
                await _next(context);
                return;
            }

            // If route is listed in NoRateLimitRoutesConfig do nothing
            if (CheckNoRestrictionRoutes(context.Request.Path, settings))
            {
                await _next(context);
                return;
            }

            // If Referer is listed in NoRateLimitRefererConfig do nothing
            if (CheckNoRestricionReferer(context, settings))
            {
                await _next(context);
                return;
            }

            var (rlConfig, key) = GenerateClientKeyExtended(context, settings.RateLimitConfig);

            if (rlConfig is not null && rlConfig.Type != "Admin")
            {
                var clientStatistics = await GetClientStatisticsByKey(key);

                context.AddRateLimitHeaders(rlConfig.MaxRequests, clientStatistics == null ? 0 : clientStatistics.LastSuccessfulResponseTimeList.Count, rlConfig.TimeWindow, rlConfig.Type);

                if (clientStatistics != null && clientStatistics.LastSuccessfulResponseTimeList.Count >= rlConfig.MaxRequests)
                {
                    // Done by WriteasJson
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;

                    await context.Response.WriteAsJsonAsync(new QuotaExceededMessage { Message = "You have exhausted your API Request Quota", Policy = rlConfig.Type, RetryAfter = rlConfig.TimeWindow, RequestsDone = clientStatistics.LastSuccessfulResponseTimeList.Count });

                    HttpRequestExtensions.GenerateLogResponse(context, clientStatistics.LastSuccessfulResponseTimeList.Count, rlConfig.Type, key);

                    return;
                }

                await UpdateClientStatisticsStorage(key, rlConfig.MaxRequests, TimeSpan.FromSeconds(rlConfig.TimeWindow));                
            }

            await _next(context);
        }

        private static string GenerateClientKey(HttpContext context) => $"{context.Request.Path}_{context.Connection.RemoteIpAddress}";

        private static (RateLimitConfig? rlConfig, string key) GenerateClientKeyExtended(HttpContext context, List<RateLimitConfig> rlsettings)
        {
            RateLimitConfig? ratelimitconfig = default;
            string ratelimitcachekey = "";

            var referer = "";

            // Check Referer
            if (context.Request.Headers.ContainsKey("Referer"))
                referer = context.Request.Headers["Referer"].ToString();
            else
            {
                // Search the QS for Referer
                if (context.Request.Query.ContainsKey("Referer"))
                    referer = context.Request.Query["Referer"].ToString();
            }

            var bearertoken = "";
            var loggeduser = "";
            List<string> userrole = new List<string>();

            // Check Referer
            if (context.Request.Headers.ContainsKey("Authorization"))
                bearertoken = context.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(bearertoken) && bearertoken.StartsWith("Bearer"))
            {
                var handler = new JwtSecurityTokenHandler();
                var token = bearertoken.Replace("Bearer", "").Trim();

                var jwttoken = ReadMyJWTSecurityToken(token, handler);


                if (jwttoken != null)
                {
                    // Gets name from claims. Generally it's an email address.
                    var usernameClaim = jwttoken.Claims
                        .Where(x => x.Type == ClaimTypes.Name || x.Type == "name")
                        .FirstOrDefault();

                    //HACK if no username (Serviceaccount) lets get the preferred_username
                    if(usernameClaim == null)
                    {
                        usernameClaim = jwttoken.Claims
                        .Where(x => x.Type == ClaimTypes.Name || x.Type == "preferred_username")
                        .FirstOrDefault();
                    }

                    if (usernameClaim != null)
                        loggeduser = usernameClaim.Value;

                    var roleClaim = jwttoken.Claims
                        .Where(x => x.Type == ClaimTypes.Name || x.Type == "role");
                    
                    if(roleClaim != null)
                    {
                        foreach(var role in roleClaim)
                        {
                            userrole.Add(role.Value);
                        }
                    }
                }
            }

            // Check Loggeduser

            // TODO: Check if User has Referer, isLogged isAnonymous


            //Get the Remote IP            
            //var remoteip = RemoteIpHelper.GetRequestIP(context, true);


            // Case 1 Anonymous, Go to IP Restriction (Maybe on Path?)
            if (String.IsNullOrEmpty(referer) && String.IsNullOrEmpty(loggeduser))
            {
                ratelimitcachekey = $"{context.Request.Path}_{context.Connection.RemoteIpAddress}";
                ratelimitconfig = rlsettings.FirstOrDefault(x => x.Type == "Anonymous");
            }
            // Case 2 Referer passed generate key with Referer
            else if (!String.IsNullOrEmpty(referer) && String.IsNullOrEmpty(loggeduser))
            {
                ratelimitcachekey = $"{context.Request.Path}_{context.Connection.RemoteIpAddress}_{referer}";
                ratelimitconfig = rlsettings.Where(x => x.Type == "Referer").FirstOrDefault();
            }

            // Case 3 Logged user, decode token and use username as key
            else if (!String.IsNullOrEmpty(loggeduser))
            {
                ratelimitcachekey = $"{context.Request.Path}_{context.Connection.RemoteIpAddress}_{loggeduser}";
                ratelimitconfig = rlsettings.Where(x => x.Type == "Basic").FirstOrDefault();

                // If user is in Role 
                if(userrole.Count > 0)
                {
                    if(userrole.Contains("ODH_ROLE_ADVANCED"))
                        ratelimitconfig = rlsettings.Where(x => x.Type == "Advanced").FirstOrDefault();
                    if (userrole.Contains("ODH_ROLE_PREMIUM"))
                        ratelimitconfig = rlsettings.Where(x => x.Type == "Premium").FirstOrDefault();
                    if (userrole.Contains("ODH_ROLE_ADMIN"))
                        ratelimitconfig = rlsettings.Where(x => x.Type == "Admin").FirstOrDefault();
                }

                // Fallback if ratelimitconfig by Role is null
                if(ratelimitconfig == null)
                    ratelimitconfig = rlsettings.Where(x => x.Type == "Basic").FirstOrDefault();
            }
            // No rate limit
            else
            {
                return (null, "");
            }

            return (ratelimitconfig, ratelimitcachekey);
        }

        private static JwtSecurityToken? ReadMyJWTSecurityToken(string token, JwtSecurityTokenHandler handler)
        {
            try
            {
                var jwttoken = handler.ReadJwtToken(token);

                return jwttoken;
            }
            catch
            {
                return null;
            }
        }

        private async Task<ClientStatistics?> GetClientStatisticsByKey(string key) => await _cache.GetCacheValueAsync<ClientStatistics>(key);

        private async Task UpdateClientStatisticsStorage(string key, int maxRequests, TimeSpan timeWindow)
        {
            var clientStat = await _cache.GetCacheValueAsync<ClientStatistics>(key);

            if (clientStat != null)
            {
                var now = DateTime.UtcNow;

                clientStat.LastSuccessfulResponseTimeList.Add(now);
                clientStat.LastSuccessfulResponseTimeList = RemoveAllExpiredResponseDateTimes(clientStat.LastSuccessfulResponseTimeList, timeWindow, now);

                await _cache.SetCacheValueAsync<ClientStatistics>(key, timeWindow, clientStat);
            }
            else
            {
                var clientStatistics = new ClientStatistics
                {
                    LastSuccessfulResponseTimeList = new List<DateTime>() { DateTime.UtcNow }
                };

                await _cache.SetCacheValueAsync(key, timeWindow, clientStatistics);
            }

        }

        private static List<DateTime> RemoveAllExpiredResponseDateTimes(List<DateTime> list, TimeSpan timeWindow, DateTime dateto)
        {
            var validfrom = dateto.Subtract(timeWindow);

            // Remove all no more valid Requests                      
            return list.Where(x => x >= validfrom).ToList();
        }

        public static MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }

        private static bool CheckNoRestrictionRoutes(PathString currentpath, ISettings settings)
        {
            bool toreturn = false;
            
            if (settings.NoRateLimitConfig.NoRateLimitRoutes != null && currentpath.Value != null && settings.NoRateLimitConfig.NoRateLimitRoutes.Contains(currentpath.Value))
                return true;


            return toreturn;
        }

        private static bool CheckNoRestricionReferer(HttpContext context, ISettings settings)
        {
            bool toreturn = false;

            string currentreferer = "";
            // Check Referer
            if (context.Request.Headers.ContainsKey("Referer"))
                currentreferer = context.Request.Headers["Referer"].ToString();
            else
            {
                // Search the QS for Referer
                if (context.Request.Query.ContainsKey("Referer"))
                    currentreferer = context.Request.Query["Referer"].ToString();
            }

            if (settings.NoRateLimitConfig.NoRateLimitReferer != null && settings.NoRateLimitConfig.NoRateLimitReferer.Contains(currentreferer))
                return true;


            return toreturn;
        }
    }

    public class ClientStatistics
    {
        public ClientStatistics()
        {
            LastSuccessfulResponseTimeList = new List<DateTime>();
        }

        public List<DateTime> LastSuccessfulResponseTimeList { get; set; }
    }

    public class QuotaExceededMessage
    {
        public string? Message { get; set; }
        public string? Hint
        {
            get
            {
                return "https://github.com/noi-techpark/odh-docs/wiki/Api-Quota";
            }
        }
        public string? Policy { get; set; }

        public int RetryAfter { get; set; }

        public int RequestsDone { get; set; }
    }
}
