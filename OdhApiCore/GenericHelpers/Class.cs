using DataModel;
using Helper;
using Newtonsoft.Json;
using OdhApiCore.Controllers;
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
      
        private static List<string> GetUrlstoCheck(string domain)
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
                string select = "data->'Id' as Id, data->'AccoDetail'->'" + language + "'->'Name', data->'AccoDetail'->'" + language + "'->'City'";
                string orderby = "data ->>'Shortname' ASC";
                List<string> fieldselectorlist = new List<string>() { "Id", "AccoDetail." + language + ".Name", "AccoDetail." + language + ".City" };
         
                AccommodationHelper myhelper = await AccommodationHelper.CreateAsync(
                   queryFactory, idfilter: null, locfilter: null, boardfilter: null, categoryfilter: null, typefilter: null,
                   featurefilter: null, featureidfilter: null, badgefilter: null, themefilter: null, altitudefilter: null, smgtags: null, activefilter: true,
                   smgactivefilter: null, bookablefilter: null, lastchange: null, default);

                var query =
                  queryFactory.Query()
                      .SelectRaw(select)
                      .From("accommodations")
                      .AccommodationWhereExpression(
                            idlist: myhelper.idlist, accotypelist: myhelper.accotypelist,
                            categorylist: myhelper.categorylist, featurelist: myhelper.featurelist, featureidlist: myhelper.featureidlist,
                            badgelist: myhelper.badgelist, themelist: myhelper.themelist,
                            boardlist: myhelper.boardlist, smgtaglist: myhelper.smgtaglist,
                            districtlist: myhelper.districtlist, municipalitylist: myhelper.municipalitylist,
                            tourismvereinlist: myhelper.tourismvereinlist, regionlist: myhelper.regionlist,
                            apartmentfilter: myhelper.apartment, bookable: myhelper.bookable, altitude: myhelper.altitude,
                            altitudemin: myhelper.altitudemin, altitudemax: myhelper.altitudemax,
                            activefilter: myhelper.active, smgactivefilter: myhelper.smgactive,
                            searchfilter: null, language: language, lastchange: myhelper.lastchange, languagelist: new List<string>(),
                            filterClosedData: true)
                      .OrderByRaw(orderby)
                 ;

                var data = await query.GetAsync<JsonRaw?>();

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

                    var sz = JsonConvert.SerializeObject(ResponseHelpers.GetResult(
                    1,
                    2,
                    (uint)data.Count(),
                    null,
                    data,
                    null));

                    Byte[] mybyte = new UTF8Encoding(true).GetBytes(sz);
                    fs.Write(mybyte, 0, mybyte.Length);

                    Console.WriteLine("ODH Accommodations for STA created " + language);
                }
            }
        }

        //public static void GenerateJSONODHActivityPoiForSTA(QueryFactory queryFactory, string jsondir, string xmlconfig)
        //{           
        //        List<string> languagelist = new List<string>() { "de", "it", "en" };

        //    foreach (var language in languagelist)
        //    {               
        //        string select = "data->'Id' as Id, data->'Detail'->'" + language + "'->'Title', data->'ContactInfos'->'" + language + "'->'City'";

        //        string orderby = "data ->>'Shortname' ASC";
        //        List<string> fieldselectorlist = new List<string>() { "Id", "Detail." + language + ".Title", "ContactInfos." + language + ".City" };


        //        var categoriestoretrieve = GetSTACategoriesToFilter(xmlconfig);

        //        var where = PostgresSQLWhereBuilder.CreateSmgPoiWhereExpression(new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>() { language },
        //            categoriestoretrieve, new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), null, true, null, null, null);
        //        string whereexpression = where.Item1;

        //        var data = PostgresSQLHelper.SelectFromTableDataAsJsonParametrizedJsonRaw(conn, "smgpoisopen", select, whereexpression, where.Item2, orderby, 0, null, fieldselectorlist);

        //        //Save json

        //        string fileName = jsondir + "\\STAOdhActivitiesPois_" + language + ".json";

        //        // Check if file already exists. If yes, delete it. 
        //        if (System.IO.File.Exists(fileName))
        //        {
        //            System.IO.File.Delete(fileName);
        //        }

        //        // Create a new file 
        //        using (FileStream fs = System.IO.File.Create(fileName))
        //        {
        //            //var sz = JsonConvert.SerializeObject(data);

        //            var sz = JsonConvert.SerializeObject(PostgresSQLHelper.GetResult(1, 1, data.Count(), "", data));

        //            Byte[] mybyte = new UTF8Encoding(true).GetBytes(sz);
        //            fs.Write(mybyte, 0, mybyte.Length);

        //            Console.WriteLine("ODH Activities & Pois for STA created " + language);
        //        }
        //    }
        //}

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
