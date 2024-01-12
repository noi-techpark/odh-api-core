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
        public static IEnumerable<string> GetDeprecatedFields(Type type)
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var deprecatedprops = props.Where(p => p.GetCustomAttribute(typeof(SwaggerDeprecatedAttribute), true) != null);

            return deprecatedprops.Select(x => x.Name).ToList();
        }
    }
}
