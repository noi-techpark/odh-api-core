using Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace OdhApiCore
{
    public static class HttpRequestExtensions
    {
        public static IApplicationBuilder UseODHCustomHttpRequestConfig(
            this IApplicationBuilder builder,
            IConfiguration configuration
        )
        {
            return builder.Use(
                async (context, next) =>
                {
                    //If Root is requested forward to Databrowser (Compatibility reason)
                    if (
                        String.IsNullOrEmpty(context.Request.Path.Value)
                        || context.Request.Path.Value == "/"
                    )
                    {
                        if (context.Request.Host.ToString().Equals("tourism.opendatahub.bz.it"))
                        {
                            context.Response.Redirect(
                                configuration
                                    .GetSection("DataBrowserConfig")
                                    .GetValue<string>("Url")
                            );
                            return;
                        }
                        else
                        {
                            context.Response.Redirect("/swagger");
                            return;
                        }
                    }
                    else if (context.Request.Path.Value == "/api")
                    {
                        context.Response.Redirect("/v1");
                        return;
                    }
                    else if (context.Request.Path.Value.StartsWith("/swagger/ui/index"))
                    {
                        context.Response.Redirect("/swagger");
                        return;
                    }
                    Stopwatch requesttime = new Stopwatch();
                    requesttime.Start();

                    await next();

                    //Log only if api is requested! including HTTP Statuscode therefore after await next();
                    //if(context.Request.Path.StartsWithSegments("/v1/", StringComparison.OrdinalIgnoreCase))
                    if (
                        !String.IsNullOrEmpty(context.Request.Path.Value)
                        && context.Request.Path.Value.StartsWith(
                            "/v1",
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
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
                        if (
                            referer == "http://localhost/"
                            && context.Request.Query.ContainsKey("Referer")
                        )
                            referer = context.Request.Query["Referer"].ToString();

                        //Origin
                        var origin = "not provided";
                        if (context.Request.Query.ContainsKey("Origin"))
                            origin = context.Request.Query["Origin"].ToString();

                        //User Agent
                        var useragent = "not provided";
                        if (context.Request.Headers.ContainsKey("User-Agent"))
                            useragent = context.Request.Headers["User-Agent"].ToString();

                        var urlparameters =
                            context.Request.QueryString.Value != null
                                ? context.Request.QueryString.HasValue
                                    ? context.Request.QueryString.Value.Replace("?", "")
                                    : ""
                                : "";

                        //To check
                        var remoteip = RemoteIpHelper.GetRequestIP(context, true);

                        //Request Length
                        requesttime.Stop();

                        //TODO Add Response Size?

                        HttpRequestLog httplog = new HttpRequestLog()
                        {
                            host = context.Request.Host.ToString(),
                            path = context.Request.Path.ToString(),
                            urlparams = urlparameters, //.Replace("&", "-"),  //Helper.StringHelpers.GenerateDictionaryFromQuerystring(context.Request.QueryString.ToString()),
                            referer = referer,
                            schema = context.Request.Scheme,
                            useragent = useragent,
                            username =
                                context.User.Identity != null
                                    ? context.User.Identity.Name != null
                                        ? context.User.Identity.Name.ToString()
                                        : "anonymous"
                                    : "anonymous",
                            ipaddress = remoteip,
                            statuscode = context.Response.StatusCode,
                            origin = origin,
                            elapsedtime = requesttime.ElapsedMilliseconds,
                            appliedquota = "",
                            ratelimitkey = ""
                        };
                        LogOutput<HttpRequestLog> logoutput = new LogOutput<HttpRequestLog>()
                        {
                            id = "",
                            type = "HttpRequest",
                            log = "apiaccess",
                            output = httplog
                        };

                        Console.WriteLine(JsonConvert.SerializeObject(logoutput));
                    }
                }
            );
        }

        public static void GenerateLogResponse(
            Microsoft.AspNetCore.Http.HttpContext context,
            int? elapsedtime = 0,
            string? quotaapplied = "",
            string? cachekey = ""
        )
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

            var urlparameters =
                context.Request.QueryString.Value != null
                    ? context.Request.QueryString.HasValue
                        ? context.Request.QueryString.Value.Replace("?", "")
                        : ""
                    : "";

            //To check
            var remoteip = RemoteIpHelper.GetRequestIP(context, true);

            //TODO Add Response Size?

            HttpRequestLog httplog = new HttpRequestLog()
            {
                host = context.Request.Host.ToString(),
                path = context.Request.Path.ToString(),
                urlparams = urlparameters, //.Replace("&", "-"),  //Helper.StringHelpers.GenerateDictionaryFromQuerystring(context.Request.QueryString.ToString()),
                referer = referer,
                schema = context.Request.Scheme,
                useragent = useragent,
                username =
                    context.User.Identity != null
                        ? context.User.Identity.Name != null
                            ? context.User.Identity.Name.ToString()
                            : "anonymous"
                        : "anonymous",
                ipaddress = remoteip,
                statuscode = context.Response.StatusCode,
                origin = origin,
                elapsedtime = elapsedtime,
                appliedquota = quotaapplied,
                ratelimitkey = cachekey
            };
            LogOutput<HttpRequestLog> logoutput = new LogOutput<HttpRequestLog>()
            {
                id = "",
                type = "HttpRequest",
                log = "apiaccess",
                output = httplog
            };

            Console.WriteLine(JsonConvert.SerializeObject(logoutput));
        }
    }
}
