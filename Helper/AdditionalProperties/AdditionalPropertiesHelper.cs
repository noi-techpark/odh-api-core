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
        public static async Task<IDictionary<string,string>> CheckAdditionalProperties<T>(this T data) where T : IHasAdditionalProperties
        {
            Dictionary<string, string> errorlist = new Dictionary<string, string>();

            bool success = false;

            foreach(var kvp in data.AdditionalProperties)
            {
                switch (kvp.Key)
                {
                    case "EchargingProperties":
                        if (kvp.Value is EchargingDataProperties)
                            success = true;
                        else
                        {
                            success = false;
                            errorlist.TryAddOrUpdate("typecast failed", "type cannot be casted to EchargingProperties");
                        }
                            
                        break;
                    default:
                        errorlist.Add("unknown type", "The Type " + kvp.Key + " is not known");
                        break;

                }
            }

            if(!success && errorlist.Count == 0)
                errorlist.Add("unknown error", "Some Type is not known or does not match");


            return errorlist;
        }
        

    }
}
