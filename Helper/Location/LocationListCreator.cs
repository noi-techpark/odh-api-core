using DataModel;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    public static class LocationListCreator
    {
        #region Locfilter Helper

        public static async Task<IEnumerable<string>> CreateActivityAreaListPGAsync(QueryFactory queryFactory, string areafilter, CancellationToken cancellationToken)
        {
            //Klaub asanond
            var splittedlocfilter =
                areafilter
                    .Split(',', System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(areafilter => (type: areafilter.Substring(0, 3), id: areafilter.Substring(3)));

            var thearealist = new List<string>();
            foreach ((string type, string id) in splittedlocfilter)
            {
                switch (type)
                {
                    case "reg":
                        //Suche alle zugehörigen Areas für die Region
                        thearealist.AddRange(await queryFactory.Query().GetAreaforRegionPGAsync(id, cancellationToken));
                        break;

                    case "tvs":
                        //Suche alle zugehörigen TVs für die Region
                        thearealist.AddRange(await queryFactory.Query().GetAreaforTourismvereinPGAsync(id, cancellationToken));
                        break;

                    case "skr":
                        //Suche alle zugehörigen TVs für die Region
                        thearealist.AddRange(await queryFactory.Query().GetAreaforSkiRegionPGAsync(id, cancellationToken));
                        break;

                    case "ska":
                        //Suche alle zugehörigen TVs für die Region
                        thearealist.AddRange(await queryFactory.Query().GetAreaforSkiAreaPGAsync(id, cancellationToken));
                        break;

                    case "are":
                        thearealist.Add(id);
                        break;
                }
            }
            return thearealist.Distinct();
        }

        public static async Task<IEnumerable<string>> GetAreaforRegionPGAsync(this Query query, string regionId, CancellationToken cancellationToken)
        {
            return await
                query.From("areas")
                     .SelectRaw("data#>>'\\{Id\\}'")
                     .WhereRaw("data#>>'\\{RegionId\\}' = $$", regionId.ToUpper())
                     .GetAsync<string>();
        }

        public static async Task<IEnumerable<string>> GetAreaforTourismvereinPGAsync(this Query query, string tvId, CancellationToken cancellationToken)
        {
            return await
                query.From("areas")
                     .SelectRaw("data#>>'\\{Id\\}'")
                     .WhereRaw("data#>>'\\{TourismvereinId\\}' = $$", tvId.ToUpper())
                     .GetAsync<string>();
        }

        private static string[] FromJsonArray(string jsonArray) =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(jsonArray) ?? new string[] { };

        public static async Task<IEnumerable<string>> GetAreaforSkiRegionPGAsync(this Query query, string skiregId, CancellationToken cancellationToken)
        {
            var areaIdsJson = await
                query.From("skiareas")
                     .SelectRaw("data#>>'\\{AreaId\\}'")
                     .WhereRaw("data#>>'\\{SkiRegionId\\}' = $$", skiregId.ToUpper())
                     .GetAsync<string>();
            return areaIdsJson.SelectMany(FromJsonArray)
                              .Distinct();
        }

        public static async Task<IEnumerable<string>> GetAreaforSkiAreaPGAsync(this Query query, string skiareaId, CancellationToken cancellationToken)
        {
            var areaIdsJson = await
                query.From("skiareas")
                     .SelectRaw("data#>>'\\{AreaId\\}'")
                     .WhereRaw("data#>>'\\{Id\\}' = $$", skiareaId.ToUpper())
                     .GetAsync<string>();
            return areaIdsJson.SelectMany(FromJsonArray)
                              .Distinct();
        }

        #endregion

        #region LocationApi Helper
        
        public static async Task<IEnumerable<T>> GetLocationFromDB<T>(QueryFactory queryFactory, string table, string whereraw) where T : notnull
        {
            return await queryFactory.Query()
                   .Select("data")
                   .From(table)
                   .WhereRaw(whereraw)
                   .GetObjectListAsync<T>();
        }
        public static async Task<IEnumerable<T>> GetLocationFromDB<T>(QueryFactory queryFactory, string table, Tuple<string, string> where) where T : notnull
        {
            return await queryFactory.Query()
                   .Select("data")
                   .From(table)
                   .Where(where.Item1, where.Item2)
                   .GetObjectListAsync<T>();
        }

        public static IEnumerable<LocHelperclassDynamic> CreateLocHelperClassDynamic<T>(string typ, IEnumerable<T> locationlist, string? lang) where T : IIdentifiable, IDetailInfosAware
        {
            var locationlistreduced = default(IEnumerable<LocHelperclassDynamic>);

            if (lang != null)
                locationlistreduced = locationlist.Select(x => new LocHelperclassDynamic { typ = typ, name = x.Detail[lang].Title, id = x.Id });
            else
                locationlistreduced = locationlist.Select(x => new LocHelperclassDynamic { typ = typ, name = x.Detail.ToDictionary(y => y.Key, y => y.Value.Title), id = x.Id });

            return locationlistreduced;
        }

        #endregion        
    }
}
