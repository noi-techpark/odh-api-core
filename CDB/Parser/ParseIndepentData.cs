using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Helper;

namespace CDB.Parser
{
    public class ParseIndepentData
    {
        public static IndependentData ParseIndependentData(XDocument independentdata, string a0rid)
        {
            try
            {               
                var data = independentdata.Root.Element("Head").Element("Data");

                if (data != null)
                {
                    IndependentData mydata = new IndependentData();

                    mydata.Enabled = !String.IsNullOrEmpty(data.Attribute("A22Ene").Value) ? Convert.ToBoolean(Convert.ToInt16(data.Attribute("A22Ene").Value)) : false;
                    mydata.IndependentRating = !String.IsNullOrEmpty(data.Attribute("A22Rat").Value) ? Convert.ToInt32(data.Attribute("A22Rat").Value) : 0;

                    var mydatalnglist = data.Elements("DataLng");

                    foreach (var mydatalanguage in mydatalnglist)
                    {
                        IndependentDescription mydesc = new IndependentDescription();

                        string language = mydatalanguage.Attribute("LngID").Value;

                        mydesc.Language = language;
                        mydesc.BacklinkUrl = mydatalanguage.Attribute("A23Url").Value;
                        mydesc.Description = mydatalanguage.Attribute("A23Des").Value;

                        mydata.IndependentDescription.TryAddOrUpdate(language, mydesc);
                    }

                    return mydata;
                }
                else
                {
                    Console.WriteLine("Independent Data is null, not importing A0RID " + a0rid);

                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on Import Independent DAta A0RID " + a0rid + " " + ex.Message);

                return null;
            }
        }
    }
}
