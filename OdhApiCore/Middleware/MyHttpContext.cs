// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace OdhApiCore
//{
//    public class MyHttpContext
//    {
//        private static IHttpContextAccessor m_httpContextAccessor;

//        public static HttpContext Current => m_httpContextAccessor.HttpContext;

//        public static string AppBaseUrl => $"{Current.Request.Scheme}://{Current.Request.Host}{Current.Request.PathBase}";

//        internal static void Configure(IHttpContextAccessor contextAccessor)
//        {
//            m_httpContextAccessor = contextAccessor;
//        }
//    }

//    public static class HttpContextExtensions
//    {
//        public static void AddHttpContextAccessor(this IServiceCollection services)
//        {
//            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//        }

//        public static IApplicationBuilder UseHttpContext(this IApplicationBuilder app)
//        {
//            MyHttpContext.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
//            return app;
//        }
//    }
//}
