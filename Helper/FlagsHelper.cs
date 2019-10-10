using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Helper
{
    // The casts to object in the below code are an unfortunate necessity due to
    // C#'s restriction against a where T : Enum constraint. (There are ways around
    // this, but they're outside the scope of this simple illustration.)
    public static class FlagsHelper
    {
        private static void CheckIsEnum<T>(bool withFlags)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException($"Type '{typeof(T).FullName}' is not an enum", nameof(withFlags));
            if (withFlags && !Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
                throw new ArgumentException($"Type '{typeof(T).FullName}' doesn't have the 'Flags' attribute", nameof(withFlags));
        }

        public static bool IsFlagSet<T>(this T value, T flag) where T : struct
        {
            CheckIsEnum<T>(true);
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flag);
            return (lValue & lFlag) != 0;
        }

        public static IEnumerable<T> GetFlags<T>(this T value) where T : struct
        {
            CheckIsEnum<T>(true);
            foreach (T flag in Enum.GetValues(typeof(T)).Cast<T>())
            {
                if (value.IsFlagSet(flag))
                    yield return flag;
            }
        }

        public static T SetFlags<T>(this T value, T flags, bool on) where T : struct
        {
            CheckIsEnum<T>(true);
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flags);
            if (on)
            {
                lValue |= lFlag;
            }
            else
            {
                lValue &= (~lFlag);
            }
            return (T)Enum.ToObject(typeof(T), lValue);
        }

        public static T SetFlags<T>(this T value, T flags) where T : struct
        {
            return value.SetFlags(flags, true);
        }

        public static T ClearFlags<T>(this T value, T flags) where T : struct
        {
            return value.SetFlags(flags, false);
        }

        public static T CombineFlags<T>(this IEnumerable<T> flags) where T : struct
        {
            CheckIsEnum<T>(true);
            long lValue = 0;
            foreach (T flag in flags)
            {
                long lFlag = Convert.ToInt64(flag);
                lValue |= lFlag;
            }
            return (T)Enum.ToObject(typeof(T), lValue);
        }

        public static string? GetDescription<T>(this T value) where T : struct
        {
            CheckIsEnum<T>(false);
            string? name = Enum.GetName(typeof(T), value);
            if (name != null)
            {
                FieldInfo? field = typeof(T).GetField(name);
                if (field != null)
                {
                    if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }

        public static int GetFlagofType<T>(string? id)
        {
            foreach (object? smgpoitypeflag in Enum.GetValues(typeof(T)))
            {
                if (smgpoitypeflag != null)
                {
                    string description = "";

                    string? name = Enum.GetName(typeof(T), smgpoitypeflag);
                    if (name != null)
                    {
                        FieldInfo? field = typeof(T).GetField(name);
                        if (field != null)
                        {
                            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                            {
                                description = attr.Description;
                            }
                        }
                    }

                    if (description == id)
                    {
                        return ((int)smgpoitypeflag);
                    }
                }
            }

            return -1;
        }

        public static long GetFlagofTypeLong<T>(string? id)
        {
            foreach (object? smgpoitypeflag in Enum.GetValues(typeof(T)))
            {
                if (smgpoitypeflag != null)
                {
                    string description = "";

                    string? name = Enum.GetName(typeof(T), smgpoitypeflag);
                    if (name != null)
                    {
                        FieldInfo? field = typeof(T).GetField(name);
                        if (field != null)
                        {
                            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                            {
                                description = attr.Description;
                            }
                        }
                    }

                    if (description == id)
                    {
                        return ((long)smgpoitypeflag);
                    }
                }
            }

            return -1;
        }


        //public static IEnumerable<T> GetFlagByDescription<T>(this T value) where T : struct
        //{
        //    CheckIsEnum<T>(true);
        //    foreach (T flag in Enum.GetValues(typeof(T)).Cast<T>())
        //    {
        //        if (value.IsFlagSet(flag))
        //            yield return flag;
        //    }
        //}

        //Des gibmer die Liste zrugg!
        public static List<string> GetDescriptionList<T>(this IEnumerable<T> enumlist) where T : struct
        {
            List<string> descriptionList = new List<string>();
            foreach (var value in enumlist)
            {
                CheckIsEnum<T>(false);
                string? name = Enum.GetName(typeof(T), value);
                if (name != null)
                {
                    FieldInfo? field = typeof(T).GetField(name);
                    if (field != null)
                    {
                        if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                        {
                            descriptionList.Add(attr.Description);
                        }
                    }
                }
            }
            return descriptionList;
        }

        //hmm geat net
        //public static List<string> GetListOfDescription<T>() where T : struct
        //{
        //    Type t = typeof(T);
        //    return !t.IsEnum ? null : Enum.GetValues(t).Cast<Enum>().Select(x => x.GetDescription()).ToList();
        //}

        //Bruachts nix
        //public static string DescriptionAttr<T>(this T source)
        //{
        //    FieldInfo fi = source.GetType().GetField(source.ToString());

        //    DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
        //        typeof(DescriptionAttribute), false);

        //    if (attributes != null && attributes.Length > 0) return attributes[0].Description;
        //    else return source.ToString();
        //}
    }
}
