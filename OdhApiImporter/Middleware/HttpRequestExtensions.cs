using Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace OdhApiImporter
{
    public static class HttpRequestExtensions
    {
        public static IApplicationBuilder UseODHCustomHttpRequestConfig(this IApplicationBuilder builder, IConfiguration configuration)
        {
            return builder.Use(async (context, next) =>
            {
                Stopwatch requesttime = new Stopwatch();
                requesttime.Start();

                await next();

                if (!String.IsNullOrEmpty(context.Request.Path.Value))
                {
                    requesttime.Stop();

                    GenerateLogResponse(context, requesttime.ElapsedMilliseconds, "", "");
                }
            });
        }

        public static void GenerateLogResponse(Microsoft.AspNetCore.Http.HttpContext context, long? elapsedtime = 0, string? quotaapplied = "", string? cachekey = "")
        {
            //TODO Make a Referer Class/Method for the logic
            var referer = "not provided";

            if (context.Request.Headers.ContainsKey("Referer"))
                referer = context.Request.Headers["Referer"].ToString();
            else
            {
                //Search the QS for Referer
                if (context.Request.Query.ContainsKey("Referer"))
                    referer = context.Request.Query["Referer"].ToString();
            }

            //Quick Fix, Android is passing http://localhost/ as referer
            if (referer == "http://localhost/" && context.Request.Query.ContainsKey("Referer"))
                referer = context.Request.Query["Referer"].ToString();

            //Origin
            var origin = "not provided";
            if (context.Request.Query.ContainsKey("Origin"))
                origin = context.Request.Query["Origin"].ToString();

            //User Agent
            var useragent = "not provided";
            if (context.Request.Headers.ContainsKey("User-Agent"))
                useragent = context.Request.Headers["User-Agent"].ToString();

            var urlparameters = context.Request.QueryString.Value != null ? context.Request.QueryString.HasValue ? context.Request.QueryString.Value.Replace("?", "") : "" : "";

            //To check
            var remoteip = RemoteIpHelper.GetRequestIP(context, true);

            //Rate Limit Policy
            var ratelimitpolicy = quotaapplied;
            if (context.Response.Headers.ContainsKey("X-Rate-Limit-Policy"))
                ratelimitpolicy = context.Response.Headers["X-Rate-Limit-Policy"].ToString();

            //TODO Add Response Size
            //var responsesize = context.Response.ContentLength;  always null

            //TODO Add Roles
            //var roles = ((ClaimsIdentity)context.User.Identity).Claims
            //    .Where(c => c.Type == ClaimTypes.Role)
            //    .Select(c => c.Value);

            HttpRequestLog httplog = new HttpRequestLog()
            {
                host = context.Request.Host.ToString(),
                path = context.Request.Path.ToString(),
                urlparams = urlparameters, //.Replace("&", "-"),  //Helper.StringHelpers.GenerateDictionaryFromQuerystring(context.Request.QueryString.ToString()),
                referer = referer,
                schema = context.Request.Scheme,
                useragent = useragent,
                username = context.User.Identity != null ? context.User.Identity.Name != null ? context.User.Identity.Name.ToString() : "anonymous" : "anonymous",
                ipaddress = remoteip,
                statuscode = context.Response.StatusCode,
                origin = origin,
                elapsedtime = elapsedtime,
                appliedquota = ratelimitpolicy,
                ratelimitkey = cachekey
            };
            LogOutput<HttpRequestLog> logoutput = new LogOutput<HttpRequestLog>() { id = "", type = "HttpRequest", log = "apiaccess", output = httplog };

            Console.WriteLine(JsonConvert.SerializeObject(logoutput));
        }

    }

}
