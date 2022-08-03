using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SwaggerObsoleteMemberAttribute : Attribute
    {
        public SwaggerObsoleteMemberAttribute(string? description = null)
        {
            Description = description;
        }

        public string Description { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SwaggerEnumMemberAttribute : Attribute
    {
        public SwaggerEnumMemberAttribute(string[] enumValues)
        {
            EnumValues = enumValues;
        }

        public IEnumerable<string> EnumValues { get; }
    }
}
