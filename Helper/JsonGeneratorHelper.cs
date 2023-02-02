﻿using DataModel;
using Helper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Helper
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
                        activefilter: null, smgactivefilter: true, publishedonlist: new List<string>(), sourcelist: new List<string>(),
                        searchfilter: null, language: null, lastchange: null, languagelist: new List<string>(),
                        filterClosedData: false, reducedData: false)
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

        public static async Task GenerateJSONTaglist(QueryFactory queryFactory, string jsondir, string jsonName)
        {
            var serializer = new JsonSerializer();                      
            
            var query =
                queryFactory.Query()
                  .SelectRaw("data")
                  .From("tags");

            var data = await query.GetObjectListAsync<TagLinked>();

            //Save json
            string fileName = Path.Combine(jsondir, $"{jsonName}.json");
            using (var writer = File.CreateText(fileName))
            {
                serializer.Serialize(writer, data);
            }            
        }
    }

    public struct AccoBooklist
    {
        public string Id { get; init; }
    }
}
