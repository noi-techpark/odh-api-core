// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataModel.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Helper.JsonHelpers
{
    public class GetOnlyContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(
            MemberInfo member,
            MemberSerialization memberSerialization
        )
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property != null && property.Readable && property.AttributeProvider != null)
            {
                var attributes = property.AttributeProvider.GetAttributes(
                    typeof(GetOnlyJsonPropertyAttribute),
                    true
                );
                if (attributes != null && attributes.Count > 0)
                    property.Readable = false;
            }
            return property;
        }
    }
}
