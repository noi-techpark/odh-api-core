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
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using Helper;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Helper.Identity
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            var endpoint = context.GetEndpoint();

            //If already a 401 Response is waiting do nothing
            if (context.Response.HasStarted)
            {
                return;
            }
            
            //GET BEARER TOKEN from Authorization Header
            var bearertoken = "";
           
            if (context.Request.Headers.ContainsKey("Authorization"))
                bearertoken = context.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(bearertoken) && bearertoken.StartsWith("Bearer"))
            {
                var handler = new JwtSecurityTokenHandler();
                var token = bearertoken.Replace("Bearer", "").Trim();

                var permissions = await RequestAuthorizationEndpoint(token, configuration.GetSection("OauthServerConfig").GetValue<string>("Authority"));
                
                //Store the permissions as claims
                foreach(var permission in permissions)
                {
                    foreach(var scope in permission.scopes)
                    {
                        var resourceendpointsplitted = permission.rsname.Split('?');
                        var additionalfilter = resourceendpointsplitted.Length > 1 ? "_" + resourceendpointsplitted[1] : "";

                        if (context.User.Identities.FirstOrDefault() != null)
                            context.User.Identities.FirstOrDefault().AddClaim(new Claim(ClaimTypes.Role, resourceendpointsplitted[0] + "_" + scope + additionalfilter));
                    }
                }
                
            }

            await _next(context);
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

        private static async Task<IEnumerable<KeyCloakPermissions>> RequestAuthorizationEndpoint(string token, string authorizationendpoint)
        {
            //TODO REQUEST AUTHORIZATION ON KEYCLOAK with grant_type=urn:ietf:params:oauth:grant-type:uma-ticket AND audience=odh-tourism-api
            //and pass the bearer token

            var body = new Dictionary<string, string>();
            body.Add("grant_type", "urn:ietf:params:oauth:grant-type:uma-ticket");
            body.Add("audience", "odh-tourism-api");
            body.Add("response_mode", "permissions");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using var request = new HttpRequestMessage(HttpMethod.Post, authorizationendpoint + "protocol/openid-connect/token") { Content = new FormUrlEncodedContent(body) };
                using var response = await client.SendAsync(request);

                //if (response.StatusCode != HttpStatusCode.OK)
                //{
                    //Return directly?? Handle Access Denied error when no Role defined in Permissions is found

                    //throw new Exception("Error on getting data " + response.StatusCode.ToString());
                //}

                var responseobject = new List<KeyCloakPermissions>();

                if (response.StatusCode != HttpStatusCode.OK)
                {

                    //Parse JSON Response to
                    var responsecontent = await response.Content.ReadAsStringAsync();
                    responseobject = JsonConvert.DeserializeObject<List<KeyCloakPermissions>>(responsecontent);
                }

                return responseobject;
            }            
        }
    }

    public class KeyCloakPermissions
    {
        public List<string> scopes { get; set; }
        public string rsname { get; set; }
    }
}

