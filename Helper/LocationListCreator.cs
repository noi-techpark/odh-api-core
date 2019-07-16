using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Helper
{
    public class LocationListCreator
    {

        #region PostGres

        public static async Task<List<string>> CreateActivityAreaListPGAsync(string areafilter, NpgsqlConnection conn)
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
                            thearealist.AddRange(await GetAreaforRegionPGAsync(theareafilter.Replace("reg", ""), conn));

                            break;

                        case "tvs":

                            //Suche alle zugehörigen TVs für die Region
                            thearealist.AddRange(await GetAreaforTourismvereinPGAsync(theareafilter.Replace("tvs", ""), conn));

                            break;

                        case "skr":

                            //Suche alle zugehörigen TVs für die Region
                            thearealist.AddRange(await GetAreaforSkiRegionPGAsync(theareafilter.Replace("skr", ""), conn));

                            break;

                        case "ska":

                            //Suche alle zugehörigen TVs für die Region
                            thearealist.AddRange(await GetAreaforSkiAreaPGAsync(theareafilter.Replace("ska", ""), conn));

                            break;

                        case "are":
                            thearealist.Add(theareafilter.Replace("are", ""));
                            break;
                    }
                }
            }

            return thearealist;
        }

        public static async Task<List<string>> GetAreaforRegionPGAsync(string regionId, NpgsqlConnection conn)
        {
            List<string> arealist = new List<string>();

            string select = "Id,data ->> 'Id'";
            string where = "data ->> 'RegionId' = '" + regionId.ToUpper() + "'";

            var result = await PostgresSQLHelper.SelectFromTableDataAsStringAsync(conn, "areas", select, where, "", 0, null);

            foreach (var area in result)
            {
                var areacodes = area.Replace(" ", "").Replace("[", "").Replace("]", "").Replace("\"", "");

                List<string> areacodeslist = areacodes.Split(',').ToList();

                foreach (var areacode in areacodeslist)
                {
                    if (!arealist.Contains(areacode))
                        arealist.Add(areacode);
                }
            }

            return arealist;
        }

        public static async Task<List<string>> GetAreaforTourismvereinPGAsync(string tvId, NpgsqlConnection conn)
        {
            List<string> arealist = new List<string>();

            string select = "Id,data ->> 'Id'";
            string where = "data ->> 'TourismvereinId' = '" + tvId.ToUpper() + "'";

            var result = await PostgresSQLHelper.SelectFromTableDataAsStringAsync(conn, "areas", select, where, "", 0, null);

            foreach (var area in result)
            {
                var areacodes = area.Replace(" ", "").Replace("[", "").Replace("]", "").Replace("\"", "");

                List<string> areacodeslist = areacodes.Split(',').ToList();

                foreach (var areacode in areacodeslist)
                {
                    if (!arealist.Contains(areacode))
                        arealist.Add(areacode);
                }
            }

            return arealist;
        }

        public static async Task<List<string>> GetAreaforSkiRegionPGAsync(string skiregId, NpgsqlConnection conn)
        {
            List<string> arealist = new List<string>();

            string select = "Id,data ->> 'AreaId'";
            string where = "data ->> 'SkiRegionId' = '" + skiregId.ToUpper() + "'";

            var result = await PostgresSQLHelper.SelectFromTableDataAsStringAsync(conn, "skiareas", select, where, "", 0, null);

            foreach (var skiarea in result)
            {
                var areacodes = skiarea.Replace(" ", "").Replace("[", "").Replace("]", "").Replace("\"", "");

                List<string> areacodeslist = areacodes.Split(',').ToList();

                foreach (var areacode in areacodeslist)
                {
                    if (!arealist.Contains(areacode))
                        arealist.Add(areacode);
                }
            }

            return arealist;
        }

        public static async Task<List<string>> GetAreaforSkiAreaPGAsync(string skiareaId, NpgsqlConnection conn)
        {
            List<string> arealist = new List<string>();

            //string select = "Id,data ->> 'AreaId'";
            string select = "Id,data ->> 'AreaId'";
            string where = "Id = '" + skiareaId.ToUpper() + "'";

            var result = (await PostgresSQLHelper.SelectFromTableDataAsStringAsync(conn, "skiareas", select, where, "", 0, null)).FirstOrDefault();

            result = result.Replace(" ", "").Replace("[", "").Replace("]", "").Replace("\"", "");

            arealist = result.Split(',').ToList();

            return arealist;
        }



        #endregion


    }
}
