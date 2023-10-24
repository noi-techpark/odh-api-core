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
        public static void SaveSwaggerJson(this IServiceProvider provider, string servername)
        {
            ISwaggerProvider sw = provider.GetRequiredService<ISwaggerProvider>();
          
            OpenApiDocument doc = sw.GetSwagger("v1", null, servername);
            string swaggerFile = doc.SerializeAsJson(Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0);

            //Save json
            var swaggerdir = ".\\wwwroot\\json\\";

            string fileName = Path.Combine(swaggerdir, "swagger.json");
            File.WriteAllText(fileName, swaggerFile);
        }
    }
}
