using DataModel.Annotations;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Reflection;

namespace OdhApiCore.Swagger
{

    public class ObsoleteAttributeSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if ((context.ParameterInfo as ICustomAttributeProvider ?? context.MemberInfo) is { } info)
            {
                var obsoleteMemberAttribute = info
                    .GetCustomAttributes(false)
                    .FirstOrDefault(attribute => attribute.GetType() == typeof(SwaggerObsoleteAttribute));
                if (obsoleteMemberAttribute is SwaggerObsoleteAttribute obsoleteMember)
                {
                    schema.Deprecated = true;
                    if (!string.IsNullOrEmpty(obsoleteMember.Description))
                    {
                        if (string.IsNullOrEmpty(schema.Description))
                        {
                            schema.Description = obsoleteMember.Description;
                        }
                        else
                        {
                            schema.Description += $"\n{obsoleteMember.Description}";
                        }
                    }
                }
            }
        }
    }
}
