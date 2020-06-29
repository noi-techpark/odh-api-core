using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    public static class PostgresSQLHelper
    {
        #region Geo Helpers

        //For Activities Pois and Smgpois

        public static string GetGeoWhereSimple(double latitude, double longitude, int radius)
        {
            return $"earth_distance(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}),ll_to_earth((data#>>'\\{{Latitude\\}}')::double precision, (data#>>'\\{{Longitude\\}}')::double precision)) < {radius.ToString()}";
        }

        //public static string GetGeoWhereSimple(string latitude, string longitude, string radius)
        //{
        //    return "earth_distance(ll_to_earth(" + latitude + ", " + longitude + "),ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision)) < " + radius;
        //}

        public static string GetGeoOrderBySimple(double latitude, double longitude)
        {
            return $"earth_distance(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}),ll_to_earth((data#>>'\\{{Latitude\\}}')::double precision, (data#>>'\\{{Longitude\\}}')::double precision))";
        }

        //public static string GetGeoOrderBySimple(string latitude, string longitude)
        //{
        //    return "earth_distance(ll_to_earth(" + latitude + ", " + longitude + "),ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision))";
        //}

        public static string GetGeoWhereExtended(double latitude, double longitude, int radius)
        {
            return $"earth_distance(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}),ll_to_earth((data#>>'\\{{GpsPoints,position,Latitude\\}}')::double precision, (data#>>'\\{{GpsPoints,position,Longitude\\}}')::double precision)) < {radius.ToString()}";
        }

        //public static string GetGeoWhereExtended(string latitude, string longitude, string radius)
        //{
        //    return "earth_distance(ll_to_earth(" + latitude + ", " + longitude + "),ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision)) < " + radius;
        //}

        public static string GetGeoOrderByExtended(double latitude, double longitude)
        {
            return $"earth_distance(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}),ll_to_earth((data#>>'\\{{GpsPoints,position,Latitude\\}}')::double precision, (data#>>'\\{{GpsPoints,position,Longitude\\}}')::double precision))";
        }

        public static string GetGeoOrderByExtended(string latitude, string longitude)
        {
            return $"earth_distance(ll_to_earth({latitude}, {longitude}),ll_to_earth((data->'GpsPoints'->'position'#>>'\\{{Latitude\\}}')::double precision, (data#>>'\\{{GpsPoints,position,Longitude\\}}')::double precision))";
        }

        public static string GetGeoWhereBoundingBoxes(string latitude, string longitude, string radius)
        {
            return $"earth_box(ll_to_earth({latitude}, {longitude}), {radius}) @> ll_to_earth((data#>>'\\{{Latitude\\}}')::double precision, (data#>>'\\{{Longitude\\}}')::double precision) and earth_distance(ll_to_earth({latitude}, {longitude}), ll_to_earth((data#>>'\\{{Latitude\\}}')::double precision, (data#>>'\\{{Longitude\\}}')::double precision)) < {radius}";
        }

        public static string GetGeoWhereBoundingBoxes(double latitude, double longitude, int radius)
        {
            return $"earth_box(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}), {radius.ToString()}) @> ll_to_earth((data#>>'\\{{Latitude\\}}')::double precision, (data#>>'\\{{Longitude\\}}')::double precision) and earth_distance(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}), ll_to_earth((data#>>'\\{{Latitude\\}}')::double precision, (data#>>'\\{{Longitude\\}}')::double precision)) < {radius.ToString()}";
        }

        public static string GetGeoWhereBoundingBoxesExtended(string latitude, string longitude, string radius)
        {
            return $"earth_box(ll_to_earth({latitude}, {longitude}), {radius}) @> ll_to_earth((data#>>'\\{{GpsPoints,position,Latitude\\}}')::double precision, (data#>>'\\{{GpsPoints,position,Longitude\\}}')::double precision) and earth_distance(ll_to_earth({latitude}, {longitude}), ll_to_earth((data#>>'\\{{GpsPoints,position,Latitude\\}}')::double precision, (data#>>'\\{{GpsPoints,position,Longitude\\}}')::double precision)) < {radius}";
        }

        public static string GetGeoWhereBoundingBoxesExtended(double latitude, double longitude, int radius)
        {
            return $"earth_box(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}), {radius.ToString()}) @> ll_to_earth((data#>>'\\{{GpsPoints,position,Latitude\\}}')::double precision, (data#>>'\\{{GpsPoints,position,Longitude\\}}')::double precision) and earth_distance(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}), ll_to_earth((data#>>'\\{{GpsPoints,position,Latitude\\}}')::double precision, (data#>>'\\{{GpsPoints,position,Longitude\\}}')::double precision)) < {radius.ToString()}";
        }

        //For Accommodations
        public static void ApplyGeoSearchWhereOrderbySimple(
            ref string where, ref string orderby, PGGeoSearchResult geosearchresult)
        {
            if (geosearchresult != null)
            {
                if (geosearchresult.geosearch)
                {
                    if (!String.IsNullOrEmpty(where))
                        where += " AND ";

                    where += PostgresSQLHelper.GetGeoWhereSimple(
                        geosearchresult.latitude,
                        geosearchresult.longitude,
                        geosearchresult.radius);
                    orderby = PostgresSQLHelper.GetGeoOrderBySimple(
                        geosearchresult.latitude,
                        geosearchresult.longitude);
                }
            }
        }

        public static Query GeoSearchFilterAndOrderby(
            this Query query,
            PGGeoSearchResult geosearchresult)
        {
            if (geosearchresult == null || !geosearchresult.geosearch)
                return query;

            return
                query.WhereRaw(
                        GetGeoWhereExtended(
                            geosearchresult.latitude,
                            geosearchresult.longitude,
                            geosearchresult.radius)
                    ).OrderByRaw(
                        GetGeoOrderByExtended(
                            geosearchresult.latitude,
                            geosearchresult.longitude)
                    );
        }

        //For Activities Pois and GBActivityPoi
        public static void ApplyGeoSearchWhereOrderby(
            ref string where,
            ref string orderby,
            PGGeoSearchResult geosearchresult)
        {
            if (geosearchresult != null)
            {
                if (geosearchresult.geosearch)
                {
                    if (!String.IsNullOrEmpty(where))
                        where += " AND ";

                    where += PostgresSQLHelper.GetGeoWhereExtended(
                        geosearchresult.latitude,
                        geosearchresult.longitude,
                        geosearchresult.radius);
                    orderby = PostgresSQLHelper.GetGeoOrderByExtended(
                        geosearchresult.latitude,
                        geosearchresult.longitude);
                }
            }
        }

        #endregion

        public static uint PGPagingHelper(uint totalcount, uint pagesize)
        {
            uint totalpages;
            if (totalcount % pagesize == 0)
                totalpages = totalcount / pagesize;
            else
                totalpages = (totalcount / pagesize) + 1;

            return totalpages;
        }

    }

    public class PGParameters
    {
        public string? Name { get; set; }
        public NpgsqlTypes.NpgsqlDbType Type { get; set; }
        public string? Value { get; set; }
    }
}
