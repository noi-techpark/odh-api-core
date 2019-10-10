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

        public static JToken FilterByLanguage(this JToken token, string language)
        {
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

        public static JToken FilterImagesByCC0License(this JToken token)
        {
            static JObject? TransformLicenceProps(JObject obj)
            {
                // Get the License property of an object
                var licenseProp =
                    obj.Properties()
                       .SingleOrDefault(x => x.Name == "License");
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
            static JToken Walk(JToken token) =>
                token switch
                {
                    JObject obj =>
                        TransformLicenceProps(obj)!,
                    JProperty prop =>
                        TransformProp(prop)!,
                    JArray arr =>
                        new JArray(
                            arr.Select(x => Walk(x))
                               // Filter away empty content
                               .Where(x => x != null)),
                    _ => token
                };
            return Walk(token);
        }

        public static JToken FilterByFields(this JToken token, string[] fieldsFromQueryString, string language)
        {
            try
            {
                var fields = new List<(string name, string path)>
                {
                    ("Id", "Id"),
                    ("Name", $"Detail.{language}.Title")
                };
                fields.AddRange(fieldsFromQueryString.Select(field => (field, field)));
                if (token is JObject obj)
                {
                    return new JObject(
                        fields.Distinct().Select(x =>
                            new JProperty(x.name, token.SelectToken(x.path))
                        )
                    );
                }
                return token;
            }
            catch (JsonException ex)
            {
                // Very probably caused by an invalid JSONPath secification.
                throw new JsonPathException(ex.Message, ex);
            }
        }

        public static JsonRaw TransformRawData(this JsonRaw raw, string? language, string[] fields, bool checkCC0)
        {
            if (language != null || fields.Length != 0|| checkCC0)
            {
                var token = JToken.Parse(raw.Value);
                if (language != null && fields.Length == 0) token = FilterByLanguage(token, language);
                if (fields.Length > 0) token = FilterByFields(token, fields, language ?? "en");
                if (checkCC0) token = FilterImagesByCC0License(token);
                return new JsonRaw(token.ToString(Formatting.Indented));
            }
            else
            {
                return raw;
            }
        }
    }
}
