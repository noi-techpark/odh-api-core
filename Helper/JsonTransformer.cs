using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Helper
{
    public static class JsonTransformer
    {
        private static readonly HashSet<string> Languages = new HashSet<string> {
            "de", "it", "en", "cs", "fr", "nl", "pl", "ru"
        };

        private static bool IsLanguageKey(string key) =>
            Languages.Contains(key);

        public static JToken? FilterByLanguage(this JToken? token, string language)
        {
            if (token == null)
                return null;
            JToken Walk(JToken token) =>
                token switch
                {
                    JObject obj =>
                        new JObject(
                            obj.Properties()
                               .Where(x =>
                                    // Check if property name is a language identifier
                                    // and if it is the same as the provided language argument
                                    !(IsLanguageKey(x.Name) && x.Name != language))
                               .Select(Walk)),
                    JProperty prop =>
                        new JProperty(prop.Name, Walk(prop.Value)),
                    JArray arr =>
                        new JArray(arr.Select(Walk)),
                    _ => token
                };
            return Walk(token);
        }

        public static JToken? FilterImagesByCC0License(this JToken? token)
        {
            if (token == null)
                return null;
            static JObject? TransformObj(JObject obj)
            {
                // Get the License property of an object
                var licenseProp = obj.Property("License");
                // If License property exists and it's value isn't CC0 return null,
                // which filters away the whole object
                return licenseProp != null && !licenseProp.Value.Equals(new JValue("CC0")) ?
                    null :
                    new JObject(obj.Properties().Select(x => Walk(x)));
            };
            static JProperty? TransformProp(JProperty prop)
            {
                var value = Walk(prop.Value);
                return value == null ? null : new JProperty(prop.Name, value);
            }
            static JToken? Walk(JToken token) =>
                token switch
                {
                    JObject obj =>
                        TransformObj(obj),
                    JProperty prop =>
                        TransformProp(prop),
                    JArray arr =>
                        new JArray(
                            arr.Select(x => Walk(x))
                               // Filter away empty content
                               .Where(x => x != null)),
                    _ => token
                };
            return Walk(token);
        }

        public static JToken? FilterClosedData(this JToken? token)
        {
            if (token == null)
                return null;
            static JObject? TransformObj(JObject obj)
            {
                // Get the Metadata property
                var metaProp = obj.Property("_Meta");
                if (metaProp != null && metaProp.Value is JObject metaObj)
                {
                    // Get the ClosedData property of an object
                    var closedDataProp = metaObj.Property("ClosedData");
                    // If ClosedData property exists and it's value is true,
                    // which filters away the whole object
                    if (closedDataProp != null && closedDataProp.Value.Equals(new JValue(true)))
                    {
                        return null;
                    }
                }
                return new JObject(obj.Properties().Select(x => Walk(x)));
            };
            static JProperty? TransformProp(JProperty prop)
            {
                var value = Walk(prop.Value);
                return value == null ? null : new JProperty(prop.Name, value);
            }
            static JToken? Walk(JToken token) =>
                token switch
                {
                    JObject obj =>
                        TransformObj(obj),
                    JProperty prop =>
                        TransformProp(prop),
                    JArray arr =>
                        new JArray(
                            arr.Select(x => Walk(x))
                               // Filter away empty content
                               .Where(x => x != null)),
                    _ => token
                };
            return Walk(token);
        }

        public static JToken? FilterMetaInformations(this JToken? token)
        {
            if (token == null)
                return null;
            static JObject TransformMetaObj(JObject obj) =>
                new JObject(obj.Properties().Where(x => x.Name != "_Meta"));
            static JToken Walk(JToken token) =>
                token switch
                {
                    JObject obj => TransformMetaObj(obj),
                    _ => token
                };
            return Walk(token);
        }

        sealed class DistinctComparer
            : IEqualityComparer<(string name, string path)>
        {
            public bool Equals([AllowNull] (string name, string path) x, [AllowNull] (string name, string path) y) =>
                x.name == y.name;

            public int GetHashCode([DisallowNull] (string name, string path) obj) =>
                obj.name.GetHashCode();
        }

        public static JToken? FilterByFields(this JToken? token, string[] fieldsFromQueryString, string? languageParam)
        {
            if (token == null)
                return null;
            var language = languageParam ?? "en";
            var fields = new List<(string name, string path)>
            {
                ("Id", "Id"),
                ("Name", $"Detail.{language}.Title")
            };
            fields.AddRange(fieldsFromQueryString.Select(field => (field, field)));
            if (token is JObject obj)
            {
                return new JObject(
                    fields.Distinct(new DistinctComparer()).Select(x =>
                    {
                        try
                        {
                            return new JProperty(x.name, token.SelectToken(x.path));
                        }
                        catch (JsonException ex)
                        {
                            throw new JsonPathException(ex.Message, x.path, ex);
                        }
                    })
                );
            }
            return token;
        }

        public static JsonRaw? TransformRawData(this JsonRaw raw, string? language, string[] fields, bool checkCC0, bool filterClosedData)
        {
            JToken? token = JToken.Parse(raw.Value);
            if (language != null) token = FilterByLanguage(token, language);
            if (fields.Length > 0) token = FilterByFields(token, fields, language);
            if (checkCC0) token = FilterImagesByCC0License(token);
            if (filterClosedData) token = token.FilterClosedData();
            token = token.FilterMetaInformations();
            return (token == null) ?
                null :
                new JsonRaw(token.ToString(Formatting.Indented));
        }
    }
}
