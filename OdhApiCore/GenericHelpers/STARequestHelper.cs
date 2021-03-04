using DataModel;
using Helper;
using Newtonsoft.Json;
using OdhApiCore.Controllers;
using OdhApiCore.Controllers.api;
using OdhApiCore.Responses;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OdhApiCore.GenericHelpers
{
    public class STARequestHelper
    {      
        public static List<string> GetUrlstoCheck(string domain)
        {
            List<string> urltorequest = new List<string>();
            urltorequest.Add(domain + "/v1/Poi?language=de&poitype=447&active=true&fields=Id,Detail.de.Title,ContactInfos.de.City&pagesize=20000");
            urltorequest.Add(domain + "/v1/Accommodation?language=de&poitype=447&active=true&fields=Id,AccoDetail.de.Name,AccoDetail.de.City&pagesize=10000");
            urltorequest.Add(domain + "/v1/ODHActivityPoi?language=de&poitype=447&active=true&fields=Id,Detail.de.Title,ContactInfos.de.City&pagesize=20000");
            urltorequest.Add(domain + "/v1/Poi?language=it&poitype=447&active=true&fields=Id,Detail.it.Title,ContactInfos.it.City&pagesize=20000");
            urltorequest.Add(domain + "/v1/Accommodation?language=it&poitype=447&active=true&fields=Id,AccoDetail.it.Name,AccoDetail.it.City&pagesize=10000");
            urltorequest.Add(domain + "/v1/ODHActivityPoi?language=it&poitype=447&active=true&fields=Id,Detail.it.Title,ContactInfos.it.City&pagesize=20000");
            urltorequest.Add(domain + "/v1/Poi?language=en&poitype=447&active=true&fields=Id,Detail.en.Title,ContactInfos.en.City&pagesize=20000");
            urltorequest.Add(domain + "/v1/Accommodation?language=en&poitype=447&active=true&fields=Id,AccoDetail.en.Name,AccoDetail.en.City&pagesize=10000");
            urltorequest.Add(domain + "/v1/ODHActivityPoi?language=en&poitype=447&active=true&fields=Id,Detail.en.Title,ContactInfos.en.City&pagesize=20000");
            urltorequest.Add(domain + "/v1/Poi?language=de&poitype=447&active=true&fields=Id,Detail.de.Title,ContactInfos.de.City&pagesize=20000");
            urltorequest.Add(domain + "/v1/Accommodation?language=de&poitype=447&active=true&fields=Id,AccoDetail.de.Name,AccoDetail.de.City&pagesize=10000&key=");
            urltorequest.Add(domain + "/v1/ODHActivityPoi?language=de&poitype=447&active=true&fields=Id,Detail.de.Title,ContactInfos.de.City&pagesize=20000&key=");
            urltorequest.Add(domain + "/v1/Poi?language=it&poitype=447&active=true&fields=Id,Detail.it.Title,ContactInfos.it.City&pagesize=20000&key=");
            urltorequest.Add(domain + "/v1/Accommodation?language=it&poitype=447&active=true&fields=Id,AccoDetail.it.Name,AccoDetail.it.City&pagesize=10000&key=");
            urltorequest.Add(domain + "/v1/ODHActivityPoi?language=it&poitype=447&active=true&fields=Id,Detail.it.Title,ContactInfos.it.City&pagesize=20000&key=");
            urltorequest.Add(domain + "/v1/Poi?language=en&poitype=447&active=true&fields=Id,Detail.en.Title,ContactInfos.en.City&pagesize=20000&key=");
            urltorequest.Add(domain + "/v1/Accommodation?language=en&poitype=447&active=true&fields=Id,AccoDetail.en.Name,AccoDetail.en.City&pagesize=10000&key=");
            urltorequest.Add(domain + "/v1/ODHActivityPoi?language=en&poitype=447&active=true&fields=Id,Detail.en.Title,ContactInfos.en.City&pagesize=20000&key=");

            return urltorequest;
        }

        public static async Task GenerateJSONAccommodationsForSTA(QueryFactory queryFactory, string jsondir)
        {
            List<string> languagelist = new List<string>() { "de", "it", "en" };

            foreach (var language in languagelist)
            {
                string select = $"data->>'Id' as Id, data->'AccoDetail'->'{language}'->>'Name' AS \"AccoDetail.{language}.Name\", data->'AccoDetail'->'{language}'->>'City' AS \"AccoDetail.{language}.City\"";
                string orderby = "data ->>'Shortname' ASC";
                //List<string> fieldselectorlist = new List<string>() { "Id", "AccoDetail." + language + ".Name", "AccoDetail." + language + ".City" };
         
                   var query =
                  queryFactory.Query()
                      .SelectRaw(select)
                      .From("accommodations")
                      .AccommodationWhereExpression(
                            idlist: new List<string>(), accotypelist: new List<string>(),
                            categorylist: new List<string>(), featurelist: new Dictionary<string, bool>(), featureidlist: new List<string>(),
                            badgelist: new List<string>(), themelist: new Dictionary<string, bool>(),
                            boardlist: new List<string>(), smgtaglist: new List<string>(),
                            districtlist: new List<string>(), municipalitylist: new List<string>(),
                            tourismvereinlist: new List<string>(), regionlist: new List<string>(),
                            apartmentfilter: null, bookable: null, altitude: false,
                            altitudemin: 0, altitudemax: 0,
                            activefilter: true, smgactivefilter: null,
                            searchfilter: null, language: language, lastchange: null, languagelist: new List<string>() { language },
                            filterClosedData: true)
                      .OrderByRaw(orderby);

                var data = await query.GetAsync<object>();

                //Json has to much Escapes! TO RESOLVE
                var datatransformed = new List<JsonRaw>();

                foreach(var myobject in data)
                {
                    datatransformed.Add(new JsonRaw(myobject));
                }

                //Save json

                string fileName = jsondir + "\\STAAccommodations_" + language + ".json";

                // Check if file already exists. If yes, delete it. 
                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                }

                // Create a new file 
                using (FileStream fs = System.IO.File.Create(fileName))
                {
                    //var sz = JsonConvert.SerializeObject(data);

                    var sz = JsonConvert.SerializeObject(
                        ResponseHelpers.GetResult(
                            1,                    
                            2,                    
                            (uint)data.Count(),
                            null,
                            datatransformed,
                            null));

                    Byte[] mybyte = new UTF8Encoding(true).GetBytes(sz);
                    fs.Write(mybyte, 0, mybyte.Length);

                    Console.WriteLine("ODH Accommodations for STA created " + language);
                }
            }
        }

        public static async Task GenerateJSONODHActivityPoiForSTA(QueryFactory queryFactory, string jsondir, string xmlconfig)
        {
            List<string> languagelist = new List<string>() { "de", "it", "en" };

            foreach (var language in languagelist)
            {
                string select = "data->'Id' as Id, data->'Detail'->'" + language + "'->'Title' AS \"Detail." + language + ".Title\", data->'ContactInfos'->'" + language + "'->'City' AS \"ContactInfos." + language + ".City\"";

                string orderby = "data ->>'Shortname' ASC";
                //List<string> fieldselectorlist = new List<string>() { "Id", "Detail." + language + ".Title", "ContactInfos." + language + ".City" };

                var categoriestoretrieve = GetSTACategoriesToFilter(xmlconfig);

                var query =
                queryFactory.Query()
                    .SelectRaw(select)
                    .From("smgpois")
                        .ODHActivityPoiWhereExpression(
                            idlist: new List<string>(), typelist: new List<string>(),
                            subtypelist: new List<string>(), poitypelist: new List<string>(),
                            smgtaglist: categoriestoretrieve, districtlist: new List<string>(),
                            municipalitylist: new List<string>(), tourismvereinlist: new List<string>(),
                            regionlist: new List<string>(), arealist: new List<string>(),
                            sourcelist: new List<string>(), languagelist: new List<string>() { language },
                            highlight: null,
                            activefilter: true, smgactivefilter: null,
                            searchfilter: null, language: language, lastchange: null,
                            filterClosedData: true)
                    .OrderByRaw(orderby);

                var data = await query.GetAsync<object>();

                //Save json

                string fileName = jsondir + "\\STAOdhActivitiesPois_" + language + ".json";

                // Check if file already exists. If yes, delete it. 
                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                }

                // Create a new file 
                using (FileStream fs = System.IO.File.Create(fileName))
                {
                    //var sz = JsonConvert.SerializeObject(data);

                    var sz = JsonConvert.SerializeObject(
                         ResponseHelpers.GetResult(
                            1,
                            2,
                            (uint)data.Count(),
                            null,
                            data,
                            null));

                    Byte[] mybyte = new UTF8Encoding(true).GetBytes(sz);
                    fs.Write(mybyte, 0, mybyte.Length);

                    Console.WriteLine("ODH Activities & Pois for STA created " + language);
                }
            }
        }

        public static List<string> GetSTACategoriesToFilter(string xmldir)
        {
            List<string> categories = new List<string>();

            var staconfig = XDocument.Load(xmldir + "STACategories.xml");

            categories.AddRange(staconfig.Root.Element("ODHActivityPois").Element("Categories").Elements("Item").Select(x => x.Value).ToList());
            categories.AddRange(staconfig.Root.Element("ODHActivityPois").Element("SubCategories").Elements("Item").Select(x => x.Value).ToList());
            categories.AddRange(staconfig.Root.Element("ODHActivityPois").Element("PoiCategories").Elements("Item").Select(x => x.Value).ToList());

            return categories;
        }
    }
}
