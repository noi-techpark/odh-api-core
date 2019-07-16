using System;
using System.Collections.Generic;
using System.Text;

namespace Helper
{
    public static class StringHelper
    {
        /// <summary>
        /// Extension Method, splits the String divided by Separator into a List
        /// </summary>
        /// <param name="thestring"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IList<string> ConvertToList(this string thestring, char separator)
        {
            List<string> activityIds = new List<string>();

            if (thestring != null)
            {
                if (thestring.Substring(thestring.Length - 1, 1) == ",")
                    thestring = thestring.Substring(0, thestring.Length - 1);

                var splittedfilter = thestring.Split(separator);

                foreach (var filter in splittedfilter)
                {
                    activityIds.Add(filter);
                }
            }

            return activityIds;
        }
    }
}
