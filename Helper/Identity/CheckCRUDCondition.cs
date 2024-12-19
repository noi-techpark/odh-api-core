// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using SqlKata;

namespace Helper.Identity
{
    public class CheckCRUDCondition
    {
        public static bool CRUDOperationAllowed<T>(T data, string? condition)
            where T : IIdentifiable, IImportDateassigneable, IMetaData
        {
            //TODO what if condition has more variables NOT working at the moment because

            bool checkresult = true;

            if (condition == null)
                return checkresult;

            List<bool> checks = new List<bool>();

            var splittedcondition = condition
                .Split("&")
                .Select(x => x.Split("="))
                .Select(x => new ReadCondition() { Column = x[0], Value = x[1] });

            var itemstoadd = splittedcondition
                .GroupBy(x => x.Column)
                .Where(g => g.Count() == 1)
                .ToList();
            foreach (var item in itemstoadd)
            {
                checks.Add(CheckTheFields(data, item.Key, item.Select(x => x.Value).ToList()));
            }

            var itemstomerge = splittedcondition
                .GroupBy(x => x.Column)
                .Where(g => g.Count() > 1)
                .ToList();
            foreach (var item in itemstomerge)
            {
                checks.Add(CheckTheFields(data, item.Key, item.Select(x => x.Value).ToList()));
            }

            return checks.All(x => x == true);
        }

        public static bool CheckTheFields<T>(T data, string input, List<string> values)
            where T : IIdentifiable, IImportDateassigneable, IMetaData
        {
            var checkresult = true;

            switch (input.ToLower())
            {
                //Hardcoded
                case "source":
                    if (!values.Any(x => x == data._Meta.Source))
                        checkresult = false;
                    break;
                //Using reflection to get the Type
                default:
                    var property = data.GetType().GetProperty(input);
                    if (property != null)
                    {
                        var propvalue = property.GetValue(data, null);
                        if (!values.Any(x => x == propvalue.ToString()))
                            checkresult = false;
                    }

                    break;
            }

            return checkresult;
        }

        public static bool CRUDOperationAllowedWithoutConstraint<T>(T data, string? condition)
        {
            bool checkresult = true;

            if (condition == null)
                return checkresult;

            var splitmultipleconditions = condition.Split("&");

            foreach (var singlecondition in splitmultipleconditions)
            {
                var splittedcondition = condition.Split("=");

                //Add here all allowed conditions
                if (splittedcondition.Length > 1)
                {
                    switch (splittedcondition[0].ToLower())
                    {
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
            }

            return checkresult;
        }
    }
}
