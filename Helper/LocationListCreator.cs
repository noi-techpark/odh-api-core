using SqlKata;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    public static class LocationListCreator
    {
        #region PostGres
        [System.Obsolete]

        public static async Task<List<string>> CreateActivityAreaListPGAsync(string areafilter, IPostGreSQLConnectionFactory connectionFactory, CancellationToken cancellationToken)
        {
            List<string> thearealist = new List<string>();

            if (areafilter != null)
            {

                if (areafilter.Substring(areafilter.Length - 1, 1) == ",")
                    areafilter = areafilter.Substring(0, areafilter.Length - 1);

                //Klaub asanond
                var splittedlocfilter = areafilter.Split(',');

                foreach (string theareafilter in splittedlocfilter)
                {
                    string areatype = theareafilter.Substring(0, 3);

                    switch (areatype)
                    {
                        case "reg":

                            //Suche alle zugehörigen Areas für die Region
                            thearealist.AddRange(await GetAreaforRegionPGAsync(theareafilter.Replace("reg", ""), connectionFactory, cancellationToken));

                            break;

                        case "tvs":

                            //Suche alle zugehörigen TVs für die Region
                            thearealist.AddRange(await GetAreaforTourismvereinPGAsync(theareafilter.Replace("tvs", ""), connectionFactory, cancellationToken));

                            break;

                        case "skr":

                            //Suche alle zugehörigen TVs für die Region
                            thearealist.AddRange(await GetAreaforSkiRegionPGAsync(theareafilter.Replace("skr", ""), connectionFactory, cancellationToken));

                            break;

                        case "ska":

                            //Suche alle zugehörigen TVs für die Region
                            thearealist.AddRange(await GetAreaforSkiAreaPGAsync(theareafilter.Replace("ska", ""), connectionFactory, cancellationToken));

                            break;

                        case "are":
                            thearealist.Add(theareafilter.Replace("are", ""));
                            break;
                    }
                }
            }

            return thearealist;
        }

        public static async Task<IEnumerable<string>> CreateActivityAreaListPGAsync(QueryFactory queryFactory, string areafilter, CancellationToken cancellationToken)
        {
            if (areafilter == null)
                return Enumerable.Empty<string>();

            if (areafilter.Substring(areafilter.Length - 1, 1) == ",")
                areafilter = areafilter.Substring(0, areafilter.Length - 1);

            var thearealist = new List<string>();
            //Klaub asanond
            var splittedlocfilter = areafilter.Split(',');

            foreach (string theareafilter in splittedlocfilter)
            {
                string areatype = theareafilter.Substring(0, 3);

                switch (areatype)
                {
                    case "reg":
                        //Suche alle zugehörigen Areas für die Region
                        var areaIds = await
                            queryFactory.Query()
                                        .GetAreaforRegionPGAsync(theareafilter.Replace("reg", ""), cancellationToken);
                        thearealist.AddRange(areaIds);
                        break;

                    case "tvs":

                        //Suche alle zugehörigen TVs für die Region
                        var areaIds4 = await
                            queryFactory.Query()
                                        .GetAreaforTourismvereinPGAsync(theareafilter.Replace("tvs", ""), cancellationToken);
                        thearealist.AddRange(areaIds4);

                        break;

                    case "skr":

                        //Suche alle zugehörigen TVs für die Region
                        var areaIds2 = await
                            queryFactory.Query()
                                        .GetAreaforSkiRegionPGAsync(theareafilter.Replace("skr", ""), cancellationToken);
                        thearealist.AddRange(areaIds2);

                        break;

                    case "ska":

                        //Suche alle zugehörigen TVs für die Region
                        var areaIds3 = await
                            queryFactory.Query()
                                        .GetAreaforSkiAreaPGAsync(theareafilter.Replace("ska", ""), cancellationToken);
                        thearealist.AddRange(areaIds3);

                        break;

                    case "are":
                        thearealist.Add(theareafilter.Replace("are", ""));
                        break;
                }
            }

            return thearealist.Distinct();
        }

        [System.Obsolete]
        public static async Task<List<string>> GetAreaforRegionPGAsync(string regionId, IPostGreSQLConnectionFactory connectionFactory, CancellationToken cancellationToken)
        {
            List<string> arealist = new List<string>();

            string select = "Id,data ->> 'Id'";
            string where = "data ->> 'RegionId' = '" + regionId.ToUpper() + "'";

            var result = PostgresSQLHelper.SelectFromTableDataAsStringAsync(connectionFactory, "areas", select, where, "", 0, null, cancellationToken);

            await foreach (var area in result)
            {
                var areacodes = area.Value.Replace(" ", "").Replace("[", "").Replace("]", "").Replace("\"", "");

                List<string> areacodeslist = areacodes.Split(',').ToList();

                foreach (var areacode in areacodeslist)
                {
                    if (!arealist.Contains(areacode))
                        arealist.Add(areacode);
                }
            }

            return arealist;
        }

        public static async Task<IEnumerable<string>> GetAreaforRegionPGAsync(this Query query, string regionId, CancellationToken cancellationToken)
        {
            return await
                query.From("areas")
                     .SelectRaw("data->>'Id'")
                     .WhereRaw("data->>'RegionId' = ?", regionId.ToUpper())
                     .GetAsync<string>();
        }

        [System.Obsolete]
        public static async Task<List<string>> GetAreaforTourismvereinPGAsync(string tvId, IPostGreSQLConnectionFactory connectionFactory, CancellationToken cancellationToken)
        {
            List<string> arealist = new List<string>();

            string select = "Id,data ->> 'Id'";
            string where = "data ->> 'TourismvereinId' = '" + tvId.ToUpper() + "'";

            var result = PostgresSQLHelper.SelectFromTableDataAsStringAsync(connectionFactory, "areas", select, where, "", 0, null, cancellationToken);

            await foreach (var area in result)
            {
                var areacodes = area.Value.Replace(" ", "").Replace("[", "").Replace("]", "").Replace("\"", "");

                List<string> areacodeslist = areacodes.Split(',').ToList();

                foreach (var areacode in areacodeslist)
                {
                    if (!arealist.Contains(areacode))
                        arealist.Add(areacode);
                }
            }

            return arealist;
        }

        public static async Task<IEnumerable<string>> GetAreaforTourismvereinPGAsync(this Query query, string tvId, CancellationToken cancellationToken)
        {
            return await
                query.From("areas")
                     .SelectRaw("data->>'Id'")
                     .WhereRaw("data->>'TourismvereinId' = ?", tvId.ToUpper())
                     .GetAsync<string>();
        }

        [System.Obsolete]
        public static async Task<IEnumerable<string>> GetAreaforSkiRegionPGAsync(string skiregId, IPostGreSQLConnectionFactory connectionFactory, CancellationToken cancellationToken)
        {
            List<string> arealist = new List<string>();

            string select = "Id,data ->> 'AreaId'";
            string where = "data ->> 'SkiRegionId' = '" + skiregId.ToUpper() + "'";

            var result = PostgresSQLHelper.SelectFromTableDataAsStringAsync(connectionFactory, "skiareas", select, where, "", 0, null, cancellationToken);

            await foreach (var skiarea in result)
            {
                var areacodes = skiarea.Value.Replace(" ", "").Replace("[", "").Replace("]", "").Replace("\"", "");

                List<string> areacodeslist = areacodes.Split(',').ToList();

                foreach (var areacode in areacodeslist)
                {
                    if (!arealist.Contains(areacode))
                        arealist.Add(areacode);
                }
            }

            return arealist;
        }

        private static string[] FromJsonArray(string jsonArray) =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(jsonArray);

        public static async Task<IEnumerable<string>> GetAreaforSkiRegionPGAsync(this Query query, string skiregId, CancellationToken cancellationToken)
        {
            var areaIdsJson = await
                query.From("skiareas")
                     .SelectRaw("data->>'AreaId'")
                     .WhereRaw("data->>'SkiRegionId' = ?", skiregId.ToUpper())
                     .GetAsync<string>();
            return areaIdsJson.SelectMany(FromJsonArray)
                              .Distinct();
        }

        [System.Obsolete]
        public static async Task<IEnumerable<string>> GetAreaforSkiAreaPGAsync(string skiareaId, IPostGreSQLConnectionFactory connectionFactory, CancellationToken cancellationToken)
        {
            List<string> arealist = new List<string>();

            //string select = "Id,data ->> 'AreaId'";
            string select = "Id,data ->> 'AreaId'";
            string where = "Id = '" + skiareaId.ToUpper() + "'";

            var rawResult = await PostgresSQLHelper.SelectFromTableDataAsStringAsync(connectionFactory, "skiareas", select, where, "", 0, null, cancellationToken).SingleAsync();

            var result = rawResult.Value;

            result = result.Replace(" ", "").Replace("[", "").Replace("]", "").Replace("\"", "");

            arealist = result.Split(',').ToList();

            return arealist;
        }

        public static async Task<IEnumerable<string>> GetAreaforSkiAreaPGAsync(this Query query, string skiareaId, CancellationToken cancellationToken)
        {
            var areaIdsJson = await
                query.From("skiareas")
                     .SelectRaw("data->>'AreaId'")
                     .WhereRaw("data->>'Id' = ?", skiareaId.ToUpper())
                     .GetAsync<string>();
            return areaIdsJson.SelectMany(FromJsonArray)
                              .Distinct();
        }
        #endregion
    }
}
