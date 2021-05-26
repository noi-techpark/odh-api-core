using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class StringHelpers
    {
        public static string RemoveSpecialCharacters(string value, char[] specialCharacters)
        {
            return new String(value.Except(specialCharacters).ToArray());
        }

        public static Dictionary<string, string>? GenerateDictionaryFromQuerystring(string value)
        {
            if (String.IsNullOrEmpty(value))
                return null;
            else
            {
                Dictionary<string, string>? myvaluedict = new Dictionary<string, string>();

                string valuenew = value.Replace("?","");

                var splitted = valuenew.Split('&');

                if (splitted.Count() > 0)
                {
                    foreach(var splittedfield in  splitted)
                    {
                        var splittedobj = splittedfield.Split('=');
                        if (splittedobj.Count() == 2)
                            myvaluedict.Add(splittedobj[0], splittedobj[1]);
                    }

                    return myvaluedict;
                }
                else
                    return null;
            }
        }        
    }

    public static class StringExtensions
    {
        public static bool ConvertStringToBoolean(this string returnValue)
        {
            if (returnValue == "1")
            {
                return true;
            }
            else if (returnValue == "0")
            {
                return false;
            }
            else
            {
                throw new FormatException("The string is not a recognized as a valid boolean value.");
            }
        }
    }
}
