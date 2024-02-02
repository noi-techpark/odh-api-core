// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Identity
{
    public class CheckCRUDCondition
    {
        public static bool CheckCondition<T>(T data, string? condition) where T : IIdentifiable, IImportDateassigneable, IMetaData
        {
            bool checkresult = true;

            if (condition == null)
                return checkresult;

            var splittedcondition = condition.Split("=");

            //Add here all allowed conditions
            if (splittedcondition.Length > 1)
            {
                switch (splittedcondition[0].ToLower())
                {
                    //Hardcoded 
                    case "source":
                        if (data._Meta.Source != splittedcondition[1])
                            checkresult = false;
                        break;
                    //Using reflection to get the Type
                    default:
                        var property = data.GetType().GetProperty(splittedcondition[0]);
                        if (property != null)
                        {
                            var propvalue = property.GetValue(data, null);
                            if (propvalue.ToString() != splittedcondition[1])
                                checkresult = false;
                        }

                        break;
                }
            }

            return checkresult;
        }
    }
}
