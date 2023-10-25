// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Text.Json.Nodes;

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


        public static void SaveSwaggerJson(this IServiceProvider provider, string servername, string swaggerdir)
        {
            ISwaggerProvider sw = provider.GetRequiredService<ISwaggerProvider>();
          
            OpenApiDocument doc = sw.GetSwagger("v1", null, servername);
            string swaggerFile = doc.SerializeAsJson(Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0);
            
            string fileName = Path.Combine(swaggerdir, "swagger.json");
            File.WriteAllText(fileName, swaggerFile);
        }
    }
}
