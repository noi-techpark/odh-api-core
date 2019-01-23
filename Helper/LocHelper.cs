using System;
using System.Collections.Generic;
using System.Text;

namespace Helper
{
    public class LocHelperclass
    {
        public string typ { get; set; }
        public string name { get; set; }
        public string id { get; set; }
    }

    public class LocHelperclassExtended
    {
        public string typ { get; set; }
        public string name { get; set; }
        public string parent { get; set; }
        public string id { get; set; }
    }

    public class LocHelperclassCategories : LocHelperclass
    {
        public string categoryid { get; set; }
    }

    public class LocHelperclassCategoriesSubCategories : LocHelperclassCategories
    {
        public string subcategoryid { get; set; }
    }

    public class AccoHelperClassExtended : LocHelperclass
    {
        public string themeIds { get; set; }
    }

    public class SkiAreaNameHelperclass
    {
        public string typ { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        //public List<string> areaids { get; set; }
    }

    public class CustomHelperclass
    {
        public CustomHelperclass()
        {
            Name = new Dictionary<string, string>();
        }

        public string Key { get; set; }
        public IDictionary<string, string> Name { get; set; }
    }


}
