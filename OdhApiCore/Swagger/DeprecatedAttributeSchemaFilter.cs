using DataModel.Annotations;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Reflection;

namespace OdhApiCore.Swagger
{
    public class DeprecatedAttributeSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if ((context.ParameterInfo as ICustomAttributeProvider ?? context.MemberInfo) is { } info)
            {
                var deprecatedAttribute = info
                    .GetCustomAttributes(false)
                    .FirstOrDefault(attribute => attribute.GetType() == typeof(SwaggerDeprecatedAttribute));
                if (deprecatedAttribute is SwaggerDeprecatedAttribute deprecated)
                {
                    schema.Deprecated = true;
                    if (!string.IsNullOrEmpty(deprecated.Description))
                    {
                        if (string.IsNullOrEmpty(schema.Description))
                            schema.Description = deprecated.Description;
                        else
                            schema.Description += $"\n{deprecated.Description}";
                    }
                }
            }
        }
    }
}
