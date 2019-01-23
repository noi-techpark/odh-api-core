using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Helper
{
    public class LocationListCreator
    {

        #region PostGres

        public static List<string> CreateActivityAreaListPG(string areafilter, NpgsqlConnection conn)
        {
            List<string> thearealist = new List<string>();

            if (areafilter != "null")
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
                            thearealist.AddRange(GetAreaforRegionPG(theareafilter.Replace("reg", ""), conn));

                            break;

                        case "tvs":

                            //Suche alle zugehörigen TVs für die Region
                            thearealist.AddRange(GetAreaforTourismvereinPG(theareafilter.Replace("tvs", ""), conn));

                            break;

                        case "skr":

                            //Suche alle zugehörigen TVs für die Region
                            thearealist.AddRange(GetAreaforSkiRegionPG(theareafilter.Replace("skr", ""), conn));

                            break;

                        case "ska":

                            //Suche alle zugehörigen TVs für die Region
                            thearealist.AddRange(GetAreaforSkiAreaPG(theareafilter.Replace("ska", ""), conn));

                            break;

                        case "are":
                            thearealist.Add(theareafilter.Replace("are", ""));
                            break;
                    }
                }
            }

            return thearealist;
        }

        public static List<string> GetAreaforRegionPG(string regionId, NpgsqlConnection conn)
        {
            List<string> arealist = new List<string>();

            string select = "Id,data ->> 'Id'";
            string where = "data ->> 'RegionId' = '" + regionId.ToUpper() + "'";

            var result = PostgresSQLHelper.SelectFromTableDataAsString(conn, "areas", select, where, "", 0, null);

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

        public static List<string> GetAreaforTourismvereinPG(string tvId, NpgsqlConnection conn)
        {
            List<string> arealist = new List<string>();

            string select = "Id,data ->> 'Id'";
            string where = "data ->> 'TourismvereinId' = '" + tvId.ToUpper() + "'";

            var result = PostgresSQLHelper.SelectFromTableDataAsString(conn, "areas", select, where, "", 0, null);

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

        public static List<string> GetAreaforSkiRegionPG(string skiregId, NpgsqlConnection conn)
        {
            List<string> arealist = new List<string>();

            string select = "Id,data ->> 'AreaId'";
            string where = "data ->> 'SkiRegionId' = '" + skiregId.ToUpper() + "'";

            var result = PostgresSQLHelper.SelectFromTableDataAsString(conn, "skiareas", select, where, "", 0, null);

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

        public static List<string> GetAreaforSkiAreaPG(string skiareaId, NpgsqlConnection conn)
        {
            List<string> arealist = new List<string>();

            //string select = "Id,data ->> 'AreaId'";
            string select = "Id,data ->> 'AreaId'";
            string where = "Id = '" + skiareaId.ToUpper() + "'";

            var result = PostgresSQLHelper.SelectFromTableDataAsString(conn, "skiareas", select, where, "", 0, null).FirstOrDefault();

            result = result.Replace(" ", "").Replace("[", "").Replace("]", "").Replace("\"", "");

            arealist = result.Split(',').ToList();

            return arealist;
        }



        #endregion


    }
}
