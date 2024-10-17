// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper.Location;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.AdditionalProperties
{
    public static class AdditionalPropertiesHelper
    {
        /// <summary>
        /// Extension Method to check Additionalproperties Model
        /// </summary>
        /// <param name="oldlocationinfo"></param>
        /// <param name="queryFactory"></param>
        /// <returns></returns>
        public static async Task<IDictionary<string,string>> CheckAdditionalProperties<T>(this T data, QueryFactory queryFactory) where T : IHasAdditionalProperties
        {
            Dictionary<string, string> errorlist = new Dictionary<string, string>();

            foreach(var kvp in data.AdditionalProperties)
            {
                Type mytype = Type.GetType(kvp.Key);
                if(mytype != null)
                {
                    switch (kvp.Key)
                    {
                        case "EchargingProperties" :
                            if (kvp.Value is EchargingDataProperties)
                                return errorlist;
                            else
                                return errorlist.TryAddOrUpdate("typecast failed", "type cannot be casted to EchargingProperties");
                        default:
                            return errorlist;
                    }                    
                }
                else
                {
                    errorlist.Add("unknown type", "The Type " + kvp.Key + " is not known");
                }
            }

            return errorlist;
        }
        

    }
}
