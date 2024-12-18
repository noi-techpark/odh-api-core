// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using System.Reflection;
using DataModel.Annotations;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OdhApiCore.Swagger
{
    public class ReferencedAttributeSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (
                (context.ParameterInfo as ICustomAttributeProvider ?? context.MemberInfo) is
                { } info
            )
            {
                var referencedAttribute = info.GetCustomAttributes(false)
                    .FirstOrDefault(attribute =>
                        attribute.GetType() == typeof(SwaggerReferenceAttribute)
                    );
                if (referencedAttribute is SwaggerReferenceAttribute referenced)
                {
                    //OpenApiReference reference = new OpenApiReference();
                    OpenApiObject oobj = new OpenApiObject();

                    schema.Extensions.Add(
                        "isReferencedTo",
                        new OpenApiString(referenced.Description)
                    );
                }
            }
        }
    }
}
