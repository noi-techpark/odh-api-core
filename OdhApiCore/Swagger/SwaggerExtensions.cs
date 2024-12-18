// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace OdhApiCore.Swagger
{
    public static class SwaggerExtensions
    {
        //public static IApplicationBuilder UseSaveSwaggerJson(this IApplicationBuilder builder, IConfiguration configuration)
        //{
        //    return builder.Use(async (context, next) =>
        //    {
        //        builder.ApplicationServices.SaveSwaggerJson()
        //    });
        //}


        public static void SaveSwaggerJson(
            this IServiceProvider provider,
            string servername,
            string swaggerdir
        )
        {
            ISwaggerProvider sw = provider.GetRequiredService<ISwaggerProvider>();

            OpenApiDocument doc = sw.GetSwagger("v1", null, servername);
            string swaggerFile = doc.SerializeAsJson(
                Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0
            );

            string fileName = Path.Combine(swaggerdir, "swagger.json");
            File.WriteAllText(fileName, swaggerFile);
        }
    }
}
