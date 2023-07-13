// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class SwaggerDeprecatedAttribute : Attribute
    {
        public SwaggerDeprecatedAttribute(string? description = null)
        {
            Description = description;
        }

        public string Description { get; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class SwaggerEnumAttribute : Attribute
    {
        public SwaggerEnumAttribute(string[] enumValues)
        {
            EnumValues = enumValues;
        }

        public IEnumerable<string> EnumValues { get; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GetOnlyJsonPropertyAttribute : Attribute
    {
    }
}
