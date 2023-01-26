using DataModel;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                    if (property1 != null)
                        property1.SetValue(compareclass1, null, null);

                    //Set the fields null 
                    var property2 = compareclass2.GetType().GetProperty(s);
                    if (property2 != null)
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
                    if (property1 != null)
                        property1.SetValue(compareclass1, null, null);

                    //Set the fields null 
                    var property2 = compareclass2.GetType().GetProperty(s);
                    if (property2 != null)
                        property2.SetValue(compareclass2, null, null);
                }
            }

            return (JsonConvert.SerializeObject(compareclass1) == JsonConvert.SerializeObject(compareclass2));
        }

        public static bool CompareImageGallery(ICollection<ImageGallery> compareclass1, ICollection<ImageGallery> compareclass2, List<string> propertiestonotcheck)
        {
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

            return (JsonConvert.SerializeObject(compareclass1) == JsonConvert.SerializeObject(compareclass2));
        }

    }

  
}
