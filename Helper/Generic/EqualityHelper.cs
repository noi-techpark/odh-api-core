using DataModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Generic
{
    public class EqualityHelper
    {
        public static bool CompareClasses<T>(object class1, object class2, List<string> propertiestonotcheck) where T : new()
        {
            T compareclass1 = new T();
            T compareclass2 = new T();

            CopyClassHelper.CopyPropertyValues(class1, compareclass1);
            CopyClassHelper.CopyPropertyValues(class2, compareclass2);

            if (propertiestonotcheck != null)
            {
                foreach (string s in propertiestonotcheck)
                {
                    //Set the fields null (DateTime sets DateTime min
                    var property1 = compareclass1.GetType().GetProperty(s);
                    if (property1 != null && property1.GetSetMethod() != null)
                        property1.SetValue(compareclass1, null, null);

                    //Set the fields null 
                    var property2 = compareclass2.GetType().GetProperty(s);
                    if (property2 != null && property2.GetSetMethod() != null)
                        property2.SetValue(compareclass2, null, null);
                }
            }

            return (JsonConvert.SerializeObject(compareclass1) == JsonConvert.SerializeObject(compareclass2));
        }

        public static bool CompareClassesTest<T>(object class1, object class2, List<string> propertiestonotcheck) where T : IIdentifiable, new()
        {            
            T compareclass1 = new T();
            T compareclass2 = new T();

            CopyClassHelper.CopyPropertyValues(class1, compareclass1);
            CopyClassHelper.CopyPropertyValues(class2, compareclass2);

            if (propertiestonotcheck != null)
            {
                foreach (string s in propertiestonotcheck)
                {
                    //Set the fields null (DateTime sets DateTime min
                    var property1 = compareclass1.GetType().GetProperty(s);
                    if (property1 != null && property1.GetSetMethod() != null)
                        property1.SetValue(compareclass1, null, null);

                    //Set the fields null 
                    var property2 = compareclass2.GetType().GetProperty(s);
                    if (property2 != null && property2.GetSetMethod() != null)
                        property2.SetValue(compareclass2, null, null);
                }
            }

            //Postgres JsonB does a Dictionary Key sorting automatically. So the retrieved Json has the Dictionary Keys ordered by Keyname alphabetically, therefore a resort is needed to compare
            //both Serialized Objects

            //var jsonSerializerSettings = new JsonSerializerSettings();
            //jsonSerializerSettings.Converters.Add(new DictionaryOrderConverter());

            //To Test for Performance, Deep Equals vs String Comparision?
            //var result1 = JsonConvert.SerializeObject(compareclass1, jsonSerializerSettings);
            //var result2 = JsonConvert.SerializeObject(compareclass2, jsonSerializerSettings);
            //return (result1 == result2);

            return JToken.DeepEquals(JToken.FromObject(compareclass1), JToken.FromObject(compareclass2));            
        }

        public static bool CompareImageGallery(ICollection<ImageGallery> compareclass1, ICollection<ImageGallery> compareclass2, List<string> propertiestonotcheck)
        {
            if (compareclass1 == null && compareclass2 == null)
                return true;

            //CopyClassHelper.CopyPropertyValues(class1, compareclass1);
            //CopyClassHelper.CopyPropertyValues(class2, compareclass2);

            if (propertiestonotcheck != null)
            {
                foreach (string s in propertiestonotcheck)
                {
                    //Set the fields null (DateTime sets DateTime min
                    var property1 = compareclass1.GetType().GetProperty(s);
                    if (property1 != null)
                        property1.SetValue(compareclass1, null, null);

                    //Set the fields null 
                    var property2 = compareclass2.GetType().GetProperty(s);
                    if (property2 != null)
                        property2.SetValue(compareclass2, null, null);
                }
            }

            //var jsonSerializerSettings = new JsonSerializerSettings();
            //jsonSerializerSettings.Converters.Add(new DictionaryOrderConverter());

            //var result1 = JsonConvert.SerializeObject(compareclass1, jsonSerializerSettings);
            //var result2 = JsonConvert.SerializeObject(compareclass2, jsonSerializerSettings);

            //return (result1 == result2);

            return JToken.DeepEquals(JToken.FromObject(compareclass1), JToken.FromObject(compareclass2));
        }

        public static bool ComparePublishedOn(ICollection<string> compareclass1, ICollection<string> compareclass2)
        {
            return JToken.DeepEquals(JToken.FromObject(compareclass1), JToken.FromObject(compareclass2));
        }
    }

    public class OrderedContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization).OrderBy(p => p.PropertyName).ToList();
        }    
    }

    public class DictionaryOrderConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        public override bool CanRead
        {
            get { return false; }
        }


        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject obj = new JObject();

            if (typeof(IDictionary<string, string>).IsAssignableFrom(value.GetType()))
            {
                var dict = (IDictionary<string, string>)value;

                if (dict != null && dict.Count() > 0)
                {
                    foreach (var kvp in dict.OrderBy(x => x.Key).ToDictionary(obj => obj.Key, obj => obj.Value))
                    {
                        obj.Add(kvp.Key, kvp.Value);
                    }
                }
            }
            else if (typeof(IDictionary).IsAssignableFrom(value.GetType()))
            {
                var dict = (IDictionary)value;                

                if (dict != null && dict.Keys.Count > 0)
                {
                    var mykeysordered = ((ICollection<string>)dict.Keys).ToList();

                    foreach (var key in mykeysordered.OrderBy(x => x))
                    {
                        obj.Add(key, JToken.FromObject(dict[key]));
                    }
                }
            }            

            obj.WriteTo(writer);
        }
    }


}
