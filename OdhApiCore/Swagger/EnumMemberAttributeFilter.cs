using DataModel.Annotations;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace OdhApiCore.Swagger
{
    public class EnumMemberAttributeFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.MemberInfo is { } memberInfo)
            {
                var obsoleteMemberAttribute = memberInfo
                    .GetCustomAttributes(false)
                    .FirstOrDefault(attribute => attribute.GetType() == typeof(SwaggerEnumMemberAttribute));
                if (obsoleteMemberAttribute is SwaggerEnumMemberAttribute obsoleteMember)
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
