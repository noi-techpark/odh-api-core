// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OdhApiCore.Swagger
{
    public class CachingSwaggerProvider : ISwaggerProvider
    {
        private static readonly ConcurrentDictionary<string, OpenApiDocument> _cache =
            new ConcurrentDictionary<string, OpenApiDocument>();

        private readonly SwaggerGenerator _swaggerGenerator;

        public CachingSwaggerProvider(
            IOptions<SwaggerGeneratorOptions> optionsAccessor,
            IApiDescriptionGroupCollectionProvider apiDescriptionsProvider,
            ISchemaGenerator schemaGenerator
        )
        {
            _swaggerGenerator = new SwaggerGenerator(
                optionsAccessor.Value,
                apiDescriptionsProvider,
                schemaGenerator
            );
        }

        public OpenApiDocument GetSwagger(
            string documentName,
            string host = null,
            string basePath = null
        )
        {
            //bool isincache = false;
            //if (_cache.ContainsKey("v1"))
            //    isincache = true;

            var doc = _cache.GetOrAdd(
                documentName,
                (_) => _swaggerGenerator.GetSwagger(documentName, host, basePath)
            );

            //if (!isincache)
            //    this.SaveSwaggerJson("haslo");

            return doc;
        }
    }
}
