// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

                string valuenew = value.Replace("?", "");

                var splitted = valuenew.Split('&');

                if (splitted.Count() > 0)
                {
                    foreach (var splittedfield in splitted)
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

        public static string JoinStringListForPG(
            string separator,
            IEnumerable<string> list,
            string escapechar
        )
        {
            var newlist = list.Select(x => (escapechar + x + escapechar)).ToList();

            return String.Join(separator, newlist);
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
                throw new FormatException(
                    "The string is not a recognized as a valid boolean value."
                );
            }
        }

        public static string[] AddToStringArray(this string[] strarr, string value)
        {
            var strlist = strarr.ToList();
            strlist.Add(value);

            return strlist.ToArray();
        }

        public static string[] AddToStringArray(this string[] strarr, string[] values)
        {
            var strlist = strarr.ToList();

            foreach (var value in values)
            {
                strlist.Add(value);
            }

            return strlist.ToArray();
        }

        public static T TrimStringProperties<T>(this T input)
        {
            if (input is null)
            {
                return input;
            }

            var props = input
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(prop => prop.GetIndexParameters().Length == 0)
                .Where(prop => prop.CanWrite && prop.CanRead);

            foreach (PropertyInfo prop in props)
            {
                var value = prop.GetValue(input, null);

                if (value is string stringValue && stringValue != null)
                {
                    prop.SetValue(input, stringValue.Trim(), null);
                }
                else if (value is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        TrimStringProperties(item);
                    }
                }
            }

            return input;
        }
    }
}
