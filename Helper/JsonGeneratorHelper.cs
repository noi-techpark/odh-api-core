// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
                        userroles: new List<string>(){ "IDM" })
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

        public static async Task GenerateJSONODHTagAutoPublishlist(QueryFactory queryFactory, string jsondir, string jsonName)
        {
            var serializer = new JsonSerializer();

            var query =
                queryFactory.Query()
                  .SelectRaw("data")
                  .From("smgtags")
                  .WhereRaw("data->>'PublishDataWithTagOn' is not null and data->'PublishDataWithTagOn' != '{}'");

            var datafirst = await query.GetObjectListAsync<ODHTagLinked>();

            var data = datafirst.Select(x => new AllowedTags() { Id = x.Id,  PublishDataWithTagOn = x.PublishDataWithTagOn }).ToList();

            //Save json
            string fileName = Path.Combine(jsondir, $"{jsonName}.json");
            using (var writer = File.CreateText(fileName))
            {
                serializer.Serialize(writer, data);
            }
        }

        public static async Task GenerateJSONODHTagCategoriesList(QueryFactory queryFactory, string jsondir, string jsonName)
        {
            var serializer = new JsonSerializer();

            var query =
                queryFactory.Query()
                  .SelectRaw("data")
                  .From("smgtags")
                  .WhereRaw("data->'ValidForEntity' ? $$", "odhactivitypoi")
                  .WhereRaw("data->>'DisplayAsCategory' = $$", "true");

            var datafirst = await query.GetObjectListAsync<ODHTagLinked>();

            var data = datafirst.Select(x => new CategoriesTags() { Id = x.Id, TagName = x.TagName }).ToList();

            //Save json
            string fileName = Path.Combine(jsondir, $"{jsonName}.json");
            using (var writer = File.CreateText(fileName))
            {
                serializer.Serialize(writer, data);
            }
        }
        
        public static async Task GenerateJSONLocationlist(QueryFactory queryFactory, string jsondir, string jsonName)
        {
            var serializer = new JsonSerializer();

            var regionlist = await LocationListCreator.GetLocationFromDB<Region>(queryFactory, "regions", "");
            var tvlist = await LocationListCreator.GetLocationFromDB<Tourismverein>(queryFactory, "tvs", "");
            var municipalitylist = await LocationListCreator.GetLocationFromDB<Municipality>(queryFactory, "municipalities", "");
            var districtlist = await LocationListCreator.GetLocationFromDB<District>(queryFactory, "districts", "");
            var arealist = await LocationListCreator.GetLocationFromDB<Area>(queryFactory, "areas", "");

            var regionlistreduced = LocationListCreator.CreateLocHelperClassDynamic<Region>("reg", regionlist, null);
            var tvlistreduced = LocationListCreator.CreateLocHelperClassDynamic<Tourismverein>("tvs", tvlist, null);
            var municipalitylistreduced = LocationListCreator.CreateLocHelperClassDynamic<Municipality>("mun", municipalitylist, null);
            var districtlistreduced = LocationListCreator.CreateLocHelperClassDynamic<District>("fra", districtlist, null);
            var arealistreduced = arealist.Select(x => new LocHelperclassDynamic { typ = "are", name = x.Detail.ToDictionary(y => y.Key, y => y.Value.Title), id = x.Id });

            var locationlist = new List<LocHelperclassDynamic>();
            locationlist.AddRange(regionlistreduced);
            locationlist.AddRange(tvlistreduced);
            locationlist.AddRange(municipalitylistreduced);
            locationlist.AddRange(districtlistreduced);
            locationlist.AddRange(arealistreduced);

            //Save json
            string fileName = Path.Combine(jsondir, $"{jsonName}.json");
            using (var writer = File.CreateText(fileName))
            {
                serializer.Serialize(writer, locationlist);
            }
        }

    }

    public struct AccoBooklist
    {
        public string Id { get; init; }
    }

    public class AllowedTags
    {
        public string Id { get; set; }
        public IDictionary<string, bool> PublishDataWithTagOn { get; set; }
    }

    public class CategoriesTags
    {
        public string Id { get; set; }
        public IDictionary<string, string> TagName { get; set; }
    }

    public class LocationList
    {
        public string Id { get; set; }
        public string Type { get; set; }

        public IDictionary<string, string> Name { get; set; }
    }
}
