// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Collections.Generic;

namespace Helper
{
    public class LocHelperclass
    {
        public string? typ { get; set; }
        public string? name { get; set; }
        public string? id { get; set; }
    }

    public class LocHelperclassExtended
    {
        public string? typ { get; set; }
        public string? name { get; set; }
        public string? parent { get; set; }
        public string? id { get; set; }
    }

    public class LocHelperclassCategories : LocHelperclass
    {
        public string? categoryid { get; set; }
    }

    public class LocHelperclassCategoriesSubCategories : LocHelperclassCategories
    {
        public string? subcategoryid { get; set; }
    }

    public class AccoHelperClassExtended : LocHelperclass
    {
        public string? themeIds { get; set; }
    }

    public class SkiAreaNameHelperclass
    {
        public string? typ { get; set; }
        public string? name { get; set; }
        public string? id { get; set; }
        //public List<string> areaids { get; set; }
    }

    public class CustomHelperclass
    {
        public string? Key { get; set; }
        public IDictionary<string, string> Name => new Dictionary<string, string>();
    }

    public class LocHelperclassDynamic : LocHelperclass
    {        
        public new dynamic? name { get; set; }     
    }
}
