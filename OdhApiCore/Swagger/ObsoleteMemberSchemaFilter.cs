using DataModel.Annotations;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace OdhApiCore.Swagger
{

    public class ObsoleteMemberSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.MemberInfo is { })
            {
                var memberInfo = context.MemberInfo;
                var obsoleteMemberAttribute = memberInfo
                    .GetCustomAttributes(false)
                    .FirstOrDefault(attribute => attribute.GetType() == typeof(SwaggerObsoleteMemberAttribute));
                if (obsoleteMemberAttribute is SwaggerObsoleteMemberAttribute obsoleteMember)
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
