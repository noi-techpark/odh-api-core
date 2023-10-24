using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Concurrent;

namespace OdhApiCore.Swagger
{
    public class CachingSwaggerProvider : ISwaggerProvider
    {
        private static readonly ConcurrentDictionary<string, OpenApiDocument> _cache = new ConcurrentDictionary<string, OpenApiDocument>();

        private readonly SwaggerGenerator _swaggerGenerator;

        public CachingSwaggerProvider(
            IOptions<SwaggerGeneratorOptions> optionsAccessor,
            IApiDescriptionGroupCollectionProvider apiDescriptionsProvider,
            ISchemaGenerator schemaGenerator)
        {
            _swaggerGenerator = new SwaggerGenerator(optionsAccessor.Value, apiDescriptionsProvider, schemaGenerator);
        }

        public OpenApiDocument GetSwagger(string documentName, string host = null, string basePath = null)
        {
            return _cache.GetOrAdd(documentName, (_) => _swaggerGenerator.GetSwagger(documentName, host, basePath));
        }
    }
}
