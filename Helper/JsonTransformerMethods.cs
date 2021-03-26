using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Helper
{
    public static class JsonTransformerMethods
    {
        private static readonly HashSet<string> Languages = new HashSet<string> {
            "de", "it", "en", "cs", "fr", "nl", "pl", "ru"
        };

        private static bool IsLanguageKey(string key) =>
            Languages.Contains(key);

        //Cutting out all not requested Language Dictionary Objects
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

        //Cutting out all images where the License is not CC0
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
                return licenseProp != null && (licenseProp.Value == null || !licenseProp.Value.Equals(new JValue("CC0"))) ?
                    null :
                    new JObject(obj.Properties().Select(x => Walk(x)).Where(x => x != null));
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

        //Cutting out all Rooms where Source is not HGV
        public static JToken? FilterAccoRoomInfoByHGVSource(this JToken? token)
        {
            if (token == null)
                return null;
            static JObject? TransformObj(JObject obj)
            {
                // Get the AccoRoomInfo property of an object which has to be an array
                var accoRoomInfo = obj.Property("AccoRoomInfo");
                if (accoRoomInfo is not null && accoRoomInfo.Value is JArray)
                {
                    var props = accoRoomInfo.Value.ToArray().Select(prop =>
                    {
                        // The prop needs to be an object
                        if (prop is JObject roomInfo)
                        {
                            // Get the Source property of an object
                            var sourceProp = roomInfo.Property("Source");
                            // If Source property exists and it's value is hgv return null,
                            // which filters away the whole object
                            return sourceProp is not null && (sourceProp.Value is null || sourceProp.Value.Equals(new JValue("hgv"))) ?
                                null :
                                new JObject(roomInfo.Properties().Select(x => Walk(x)).Where(x => x != null));
                        }
                        else
                        {
                            return prop;
                        }
                    }).Where(prop => prop is not null);
                    obj.TryAddOrUpdate("AccoRoomInfo", new JArray(props));
                    return obj;
                }
                else
                {
                    return obj;
                }
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

        //Cutting out Property TVMember on Accommodations
        public static JToken? FilterTVMemberInformations(this JToken? token)
        {
            if (token == null)
                return null;
            

            static JObject TransformTVMemberProp(JObject obj)
            {
                // Get the TVMember property of an object which has to be an property
                var accoTVMember = obj.Property("TVMember");
                if (accoTVMember is not null && accoTVMember.Value is JProperty)
                {
                    //Cut out this property

                    return obj;
                }
                else
                {
                    return obj;
                }
            }                
            static JProperty? TransformProp(JProperty prop)
            {
                var value = Walk(prop.Value);
                return value == null ? null : new JProperty(prop.Name, value);
            }
            static JToken Walk(JToken token) =>
                token switch
                {
                    JObject obj => TransformTVMemberProp(obj),
                    JProperty prop => TransformProp(prop),
                    _ => token
                };
            return Walk(token);
        }


        //Cutting out Property _Meta
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

        //Cutting out all Properties where Meta Contains the information Closeddata is true
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

        //Cuts out all fields that are not requested only Id is active by default
        public static JToken? FilterByFields(this JToken? token, string[] fieldsFromQueryString, string? languageParam)
        {
            if (token == null)
                return null;
            var language = languageParam ?? "en";
            var fields = new List<(string name, string path)>
            {
                ("Id", "Id")
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

        //Transforms URL of the self link to the right domain
        public static JToken? TransformSelfLink(this JToken? token, Func<string, string> urlGenerator)
        {
            if (token == null)
                return null;
            static JObject? TransformObj(JObject obj, Func<string, string> urlGenerator) =>
                new JObject(obj.Properties().Select(x => Walk(x, urlGenerator)));
            static string? TransformSelf(string? self, Func<string, string> urlGenerator)
            {
                if (self == null)
                    return null;
                // FIXME: Temporary workaround
                if (self.StartsWith("https://tourism.opendatahub.bz.it/api/"))
                    self = self.Substring(38);
                return urlGenerator(self);
            }
            static JProperty? TransformProp(JProperty prop, Func<string, string> urlGenerator)
            {
                if (prop.Name == "Self")
                {
                    string? value = TransformSelf(prop.Value.Value<string?>(), urlGenerator);
                    return new JProperty(prop.Name, value);
                }
                else
                {
                    var value = Walk(prop.Value, urlGenerator);
                    return value == null ? null : new JProperty(prop.Name, value);
                }
            }
            static JToken? Walk(JToken token, Func<string, string> urlGenerator) =>
                token switch
                {
                    JObject obj =>
                        TransformObj(obj, urlGenerator),
                    JProperty prop =>
                        TransformProp(prop, urlGenerator),
                    JArray arr =>
                        new JArray(
                            arr.Select(x => Walk(x, urlGenerator))
                               // Filter away empty content
                               .Where(x => x != null)),
                    _ => token
                };
            return Walk(token, urlGenerator);
        }

    }
}
