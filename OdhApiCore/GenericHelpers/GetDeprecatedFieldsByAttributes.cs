// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Reflection;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using DataModel.Annotations;

namespace OdhApiCore.GenericHelpers
{
    public class GetDeprecatedFieldsByAttributes
    {
        public static IEnumerable<DeprecationInfo> GetDeprecatedFields(Type type)
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var deprecatedprops = props.Where(p => p.GetCustomAttribute(typeof(SwaggerDeprecatedAttribute), true) != null);

            //foreach(var deprecatedprop in deprecatedprops)
            //{
                
            //}

            return deprecatedprops.Select(x => new DeprecationInfo() { 
                Name = x.Name, 
                Description = x.GetCustomAttribute<SwaggerDeprecatedAttribute>().Description, 
                Type = x.PropertyType.ToString() }).ToList();
        }
    }

    public class DeprecationInfo
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public DateTime? DeprecationDate { get; set; }
        public DateTime? RemovedAfter { get; }
    }
}
