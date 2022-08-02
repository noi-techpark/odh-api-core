using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OdhApiCore.Swagger
{
    // Source: Swashbuckle documentation
    public class ExampleSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;
            if (type.IsEnum)
            {
                schema.Extensions.Add(
                  "x-ms-enum",
                  new OpenApiObject
                  {
                      ["name"] = new OpenApiString(type.Name),
                      ["modelAsString"] = new OpenApiBoolean(true)
                  }
                );
            };
        }
    }
}
