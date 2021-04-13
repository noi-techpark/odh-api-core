using DataModel;
using Helper;
using Microsoft.AspNetCore.Mvc;
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
    public class JsonGeneratorHelper
    {
        public static async Task GenerateJSONAccommodationsForBooklist(QueryFactory queryFactory, string jsondir, bool isbookable, string jsonName)
        {
            //List<string> languagelist = new List<string>() { "de", "it", "en" };

            var serializer = new JsonSerializer();
            //foreach (var language in languagelist)
            //{
            string select = "id as Id";
            var seed = Helper.CreateSeed.GetSeed(0);
            string orderby = $"md5(id || '{seed}')";

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
                        apartmentfilter: null, bookable: isbookable, altitude: false,
                        altitudemin: 0, altitudemax: 0,
                        activefilter: null, smgactivefilter: true,
                        searchfilter: null, language: null, lastchange: null, languagelist: new List<string>(),
                        filterClosedData: false)
                  .OrderByRaw(orderby);

            var data = await query.GetAsync<string>();
            
            //Save json
            string fileName = Path.Combine(jsondir, $"{jsonName}.json");
            using (var writer = File.CreateText(fileName))
            {
                serializer.Serialize(writer, data);
            }

            //}
        }
    }

    public class AccoBooklist
    {
        public string Id { get; set; }
    }
}
