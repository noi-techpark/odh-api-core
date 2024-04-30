// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

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

        public static string GetGeoWhereExtendedGpsInfo(double latitude, double longitude, int radius)
        {
            return $"earth_distance(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}),ll_to_earth((data#>>'\\{{GpsInfo,0,Latitude\\}}')::double precision, (data#>>'\\{{GpsInfo,0,Longitude\\}}')::double precision)) < {radius.ToString()}";
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
            PGGeoSearchResult? geosearchresult)
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

        #region Geo Helpers Generated Columns

        //For Activities Pois and Smgpois

        public static string GetGeoWhereSimple_GeneratedColumns(double latitude, double longitude, int radius)
        {
            return $"earth_distance(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}),ll_to_earth((gen_latitude)::double precision, (gen_longitude)::double precision)) < {radius.ToString()}";
        }

        public static string GetGeoWhereInPolygon_GeneratedColumns(List<Tuple<double,double>> polygon, string? operation = null)
        {
            return $"{GetPolygonOperator(operation)}(ST_Polygon('LINESTRING({ String.Join(",", polygon.Select(t => string.Format("{0} {1}", t.Item1.ToString(CultureInfo.InvariantCulture), t.Item2.ToString(CultureInfo.InvariantCulture))))})'::geometry, 4326), gen_position)";
        }

        public static string GetPolygonOperator(string operation) => operation switch
        {
            "contains" => "ST_Contains",
            "intersects" => "ST_Intersects",
            _ => "ST_Contains"
        };          

        
        public static string GetGeoOrderBySimple_GeneratedColumns(double latitude, double longitude)
        {
            return $"earth_distance(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}),ll_to_earth((gen_latitude)::double precision, (gen_longitude)::double precision))";
        }      
        
        public static string GetGeoWhereExtended_GeneratedColumns(double latitude, double longitude, int radius)
        {
            return $"earth_distance(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}),ll_to_earth((gen_latitude)::double precision, (gen_longitude)::double precision)) < {radius.ToString()}";
        }

        public static string GetGeoOrderByExtended_GeneratedColumns(double latitude, double longitude)
        {
            return $"earth_distance(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}),ll_to_earth((gen_latitude)::double precision, (gen_longitude)::double precision))";
        }

        public static string GetGeoOrderByExtended_GeneratedColumns(string latitude, string longitude)
        {
            return $"earth_distance(ll_to_earth({latitude}, {longitude}),ll_to_earth((gen_latitude)::double precision, (gen_longitude)::double precision))";
        }

        public static string GetGeoWhereBoundingBoxes_GeneratedColumns(string latitude, string longitude, string radius)
        {
            return $"earth_box(ll_to_earth({latitude}, {longitude}), {radius}) @> ll_to_earth((gen_latitude)::double precision, (gen_longitude)::double precision) and earth_distance(ll_to_earth({latitude}, {longitude}), ll_to_earth((gen_latitude)::double precision, (gen_longitude)::double precision)) < {radius}";
        }

        public static string GetGeoWhereBoundingBoxes_GeneratedColumns(double latitude, double longitude, int radius)
        {
            return $"earth_box(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}), {radius.ToString()}) @> ll_to_earth((gen_latitude)::double precision, (gen_longitude)::double precision) and earth_distance(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}), ll_to_earth((gen_latitude)::double precision, (gen_longitude)::double precision)) < {radius.ToString()}";
        }

        public static string GetGeoWhereBoundingBoxesExtended_GeneratedColumns(string latitude, string longitude, string radius)
        {
            return $"earth_box(ll_to_earth({latitude}, {longitude}), {radius}) @> ll_to_earth((gen_latitude)::double precision, (gen_longitude)::double precision) and earth_distance(ll_to_earth({latitude}, {longitude}), ll_to_earth((gen_latitude)::double precision, (gen_longitude)::double precision)) < {radius}";
        }

        public static string GetGeoWhereBoundingBoxesExtended_GeneratedColumns(double latitude, double longitude, int radius)
        {
            return $"earth_box(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}), {radius.ToString()}) @> ll_to_earth((gen_latitude)::double precision, (gen_longitude)::double precision) and earth_distance(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}), ll_to_earth((gen_latitude)::double precision, (gen_longitude)::double precision)) < {radius.ToString()}";
        }

        //For Accommodations
        public static void ApplyGeoSearchWhereOrderbySimple_GeneratedColumns(
            ref string where, ref string orderby, PGGeoSearchResult geosearchresult)
        {
            if (geosearchresult != null)
            {
                if (geosearchresult.geosearch)
                {
                    if (!String.IsNullOrEmpty(where))
                        where += " AND ";

                    where += PostgresSQLHelper.GetGeoWhereSimple_GeneratedColumns(
                        geosearchresult.latitude,
                        geosearchresult.longitude,
                        geosearchresult.radius);
                    orderby = PostgresSQLHelper.GetGeoOrderBySimple_GeneratedColumns(
                        geosearchresult.latitude,
                        geosearchresult.longitude);
                }
            }
        }

        public static Query GeoSearchFilterAndOrderby_GeneratedColumns(
            this Query query,
            PGGeoSearchResult? geosearchresult)
        {
            if (geosearchresult == null || !geosearchresult.geosearch)
                return query;

            return
                query.WhereRaw(
                        GetGeoWhereExtended_GeneratedColumns(
                            geosearchresult.latitude,
                            geosearchresult.longitude,
                            geosearchresult.radius)
                    ).OrderByRaw(
                        GetGeoOrderByExtended_GeneratedColumns(
                            geosearchresult.latitude,
                            geosearchresult.longitude)
                    );
        }

        //For Activities Pois and GBActivityPoi
        public static void ApplyGeoSearchWhereOrderby_GeneratedColumns(
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

                    where += PostgresSQLHelper.GetGeoWhereExtended_GeneratedColumns(
                        geosearchresult.latitude,
                        geosearchresult.longitude,
                        geosearchresult.radius);
                    orderby = PostgresSQLHelper.GetGeoOrderByExtended_GeneratedColumns(
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
