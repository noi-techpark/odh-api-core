using DataModel.Annotations;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OdhApiCore.Swagger
{
    public class EnumAttributeSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if ((context.MemberInfo as ICustomAttributeProvider ?? context.ParameterInfo) is { } info)
            {
                var obsoleteMemberAttribute = info
                    .GetCustomAttributes(false)
                    .FirstOrDefault(attribute => attribute.GetType() == typeof(SwaggerEnumAttribute));
                if (obsoleteMemberAttribute is SwaggerEnumAttribute obsoleteMember)
                {
                    var enumValues = new List<IOpenApiAny>();
                    foreach (var enumValue in obsoleteMember.EnumValues)
                        enumValues.Add(new OpenApiString(enumValue));
                    schema.Enum = enumValues;
                }
            }
        }
    }
}
