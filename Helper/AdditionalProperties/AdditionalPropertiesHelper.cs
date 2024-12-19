// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Helper.Location;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlKata.Execution;

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
        public static async Task<IDictionary<string, string>> CheckAdditionalProperties<T>(
            this T data
        )
            where T : IHasAdditionalProperties
        {
            Dictionary<string, string> errorlist = new Dictionary<string, string>();

            bool success = false;

            foreach (var kvp in data.AdditionalProperties)
            {
                switch (kvp.Key)
                {
                    case "EchargingDataProperties":

                        var result = CastAs<EchargingDataProperties>(kvp.Value);
                        success = result.Item1;
                        if (!success)
                            errorlist.TryAddOrUpdate("error", (string)result.Item2);
                        else
                        {
                            //Assign the Casted model
                            if (result.Item3 != null)
                            {
                                data.AdditionalProperties.TryAddOrUpdate(
                                    "EchargingDataProperties",
                                    (EchargingDataProperties)result.Item3
                                );
                            }
                        }

                        break;
                    default:
                        errorlist.Add("unknown error", "The Type " + kvp.Key + " is not known");
                        break;
                }
            }

            return errorlist;
        }

        public static (bool, string, T) CastAs<T>(dynamic data)
        {
            try
            {
                T info = ((JObject)data).ToObject<T>();

                return (true, "", info);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, default(T));
            }
        }
    }
}
