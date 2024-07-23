// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Amazon.S3.Model;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helper
{
    public class GeoSearchHelper
    {
        readonly double _eQuatorialEarthRadius = 6378.1370D;
        readonly double _d2r = (Math.PI / 180D);

        public double HaversineInKM(double lat1, double long1, double lat2, double long2)
        {
            double dlong = (long2 - long1) * _d2r;
            double dlat = (lat2 - lat1) * _d2r;
            double a = Math.Pow(Math.Sin(dlat / 2D), 2D) + Math.Cos(lat1 * _d2r) * Math.Cos(lat2 * _d2r) * Math.Pow(Math.Sin(dlong / 2D), 2D);
            double c = 2D * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1D - a));
            double d = _eQuatorialEarthRadius * c;

            return d;
        }

        public int HaversineInM(double lat1, double long1, double lat2, double long2)
        {
            return (int)(1000D * HaversineInKM(lat1, long1, lat2, long2));
        }

        public static bool CheckGeoSearch(string latitude, string longitude, string radius)
        {
            bool geosearch = false;
            //Check of Geosearch durchgeführt werden kann                    
            CultureInfo culture = CultureInfo.InvariantCulture;
            bool isLatDouble = Double.TryParse(latitude, NumberStyles.Any, culture, out double latitudecheck);
            bool isLongDouble = Double.TryParse(longitude, NumberStyles.Any, culture, out double longitudecheck);
            bool isRadiusInt = Int32.TryParse(radius, out int radiuscheck);

            if (isLatDouble && isLongDouble && isRadiusInt)
                geosearch = true;

            return geosearch;
        }

        public static PGGeoSearchResult GetPGGeoSearchResult(string? latitude, string? longitude, string? radius)
        {
            if (latitude == null && longitude == null)
                return new PGGeoSearchResult() { geosearch = false, latitude = 0, longitude = 0, radius = 0 };

            PGGeoSearchResult pggeosearchresult = new PGGeoSearchResult
            {
                geosearch = false
            };
            //Check of Geosearch durchgeführt werden kann                    
            CultureInfo culture = CultureInfo.InvariantCulture;
            bool isLatDouble = Double.TryParse(latitude, NumberStyles.Any, culture, out double latitudecheck);
            bool isLongDouble = Double.TryParse(longitude, NumberStyles.Any, culture, out double longitudecheck);
            bool isRadiusInt = Int32.TryParse(radius, out int radiuscheck);

            if (isLatDouble && isLongDouble)
            {
                pggeosearchresult.geosearch = true;
                pggeosearchresult.latitude = latitudecheck;
                pggeosearchresult.longitude = longitudecheck;

                if (isRadiusInt)
                    pggeosearchresult.radius = radiuscheck;
                else
                    pggeosearchresult.radius = 150000;

                //Check Distance, DistanceCalculator returns to high number
                //var actualdistance = DistanceCalculator.Distance(pggeosearchresult.latitude, pggeosearchresult.longitude, DistanceCalculator.suedtirolMitteLatitude, DistanceCalculator.suedtirolMitteLongitude, 'K');
                //if (actualdistance > 200)
                //    pggeosearchresult.geosearch = false;

            }
            else
            {
                pggeosearchresult.geosearch = false;
                pggeosearchresult.latitude = 0;
                pggeosearchresult.longitude = 0;
                pggeosearchresult.radius = 0;
            }

            return pggeosearchresult;
        }

        public static PGGeoSearchResult GetPGGeoSearchResult(double? latitude, double? longitude, int radius)
        {
            if (latitude == null && longitude == null)
                return new PGGeoSearchResult() { geosearch = false, latitude = 0, longitude = 0, radius = 0 };

            PGGeoSearchResult pggeosearchresult = new PGGeoSearchResult
            {
                geosearch = false
            };


            pggeosearchresult.geosearch = true;
            pggeosearchresult.latitude = latitude ?? 0;
            pggeosearchresult.longitude = longitude ?? 0;

            if (radius > 0)
                pggeosearchresult.radius = radius;
            else
                pggeosearchresult.radius = 150000;

            return pggeosearchresult;
        }

        public static RavenGeoSearchResult GetRavenGeoSearchResult(string latitude, string longitude, string radius)
        {
            if (latitude == null && longitude == null)
                return new RavenGeoSearchResult() { geosearch = false, latitude = 0, longitude = 0, radius = 0 };

            RavenGeoSearchResult pggeosearchresult = new RavenGeoSearchResult
            {
                geosearch = false
            };
            //Check of Geosearch durchgeführt werden kann                    
            CultureInfo culture = CultureInfo.InvariantCulture;
            bool isLatDouble = Double.TryParse(latitude, NumberStyles.Any, culture, out double latitudecheck);
            bool isLongDouble = Double.TryParse(longitude, NumberStyles.Any, culture, out double longitudecheck);
            bool isRadiusInt = Int32.TryParse(radius, out int radiuscheck);

            if (isLatDouble && isLongDouble)
            {
                pggeosearchresult.geosearch = true;
                pggeosearchresult.latitude = latitudecheck;
                pggeosearchresult.longitude = longitudecheck;

                if (isRadiusInt)
                    pggeosearchresult.radius = radiuscheck;
                else
                    pggeosearchresult.radius = 150;

                //Check ob das ganze sinn macht
                var actualdistance = DistanceCalculator.Distance(pggeosearchresult.latitude, pggeosearchresult.longitude, DistanceCalculator.suedtirolMitteLatitude, DistanceCalculator.suedtirolMitteLongitude, 'K');
                if (actualdistance > 150)
                    pggeosearchresult.geosearch = false;

            }
            else
            {
                pggeosearchresult.geosearch = false;
                pggeosearchresult.latitude = 0;
                pggeosearchresult.longitude = 0;
                pggeosearchresult.radius = 0;
            }

            return pggeosearchresult;
        }

        public static async Task<GeoPolygonSearchResult?> GetPolygon(string? polygon, QueryFactory queryfactory)
        {
            if(String.IsNullOrEmpty(polygon)) return null;            
            else
            {
                GeoPolygonSearchResult result = new GeoPolygonSearchResult();
                //setting standard operation to intersects
                result.operation = "intersects";                

                //Check for WKT String
                if (CheckWKTSyntax(polygon, queryfactory))
                {
                    var wktandsrid = GetWKTAndSRIDFromQuerystring(polygon);

                    if (wktandsrid != null)
                    {
                        result.wktstring = wktandsrid.Value.Item1;
                        result.srid = wktandsrid.Value.Item2;
                        return result;
                    }
                    else
                        return null;
                }

                else if (polygon.ToLower().StartsWith("bbc(") || polygon.ToLower().StartsWith("bbi("))
                {
                    result.polygon = new List<Tuple<double, double>>();

                    string coordstoprocess = "";
                    string polygonwithoutsrid = polygon;
                    
                    if (polygon.Split(";").Length > 1)
                    {
                        var splitted = polygon.Split(";");
                        var sridstr = splitted[1].ToUpper();
                        if (sridstr.StartsWith("SRID="))
                            result.srid = sridstr.Replace("SRID=", "");                        
                    }

                    if (polygon.ToLower().StartsWith("bbc"))
                    {
                        result.operation = "contains";
                        coordstoprocess = polygon.ToLower().Replace("bbc", "");
                    }
                    else if (polygon.ToLower().StartsWith("bbi"))
                    {
                        result.operation = "intersects";
                        coordstoprocess = polygon.ToLower().Replace("bbi", "");
                    }
                    else
                        coordstoprocess = polygon.ToLower();

                    coordstoprocess = Regex.Replace(coordstoprocess, @"\(+|\)+", "");
                    CultureInfo culture = CultureInfo.InvariantCulture;

                    foreach (var item in coordstoprocess.Split(','))
                    {
                        var coords = item.Trim().Split(" ");

                        if (coords.Count() == 2 && Double.TryParse(coords[0], NumberStyles.Any, culture, out double longitude) && Double.TryParse(coords[1], NumberStyles.Any, culture, out double latitude))
                        {
                            result.polygon.Add(Tuple.Create(longitude, latitude));
                        }
                    }

                    if (result.polygon.Count == 0) return null;
                    else return result;
                }
                else
                {
                    //Format = country.type.id or country.type.name
                    var inputquery = polygon.Split(".");
                    if (inputquery.Length != 3)
                        return null;

                    bool idtofilter = int.TryParse(inputquery[2], out int parsedid);

                    //Retrieve data from shape table

                    var geometry = await queryfactory.Query()
                        .SelectRaw("ST_AsText(geometry)")
                        .From("shapes")
                        .Where("country", inputquery[0].ToUpper())
                        .Where("type", inputquery[1].ToLower())
                        .When(idtofilter, x => x.Where("id", parsedid))
                        .When(!idtofilter, x => x.WhereLike("name", inputquery[2]))
                        //create a generated column which constructs a name by id,type and name
                        .FirstOrDefaultAsync<string>();

                    if (!String.IsNullOrEmpty(geometry))
                    {                        
                        result.wktstring = geometry.ToString();

                        return result;
                    }
                    else
                        return null;
                }
            }

        }

        public static bool CheckValidWKTString(string? polygon)
        {
            if (polygon.ToUpper().StartsWith("LINESTRING") ||
                polygon.ToUpper().StartsWith("POLYGON") ||
                polygon.ToUpper().StartsWith("MULTIPOINT") ||
                polygon.ToUpper().StartsWith("MULTILINESTRING") ||
                    polygon.ToUpper().StartsWith("MULTIPOLYGON") ||
                    polygon.ToUpper().StartsWith("GEOMETRYCOLLECTION") ||
                    polygon.ToUpper().StartsWith("POINT ZM") ||
                    polygon.ToUpper().StartsWith("POINT M"))
            {
                return true;
            }
            else
                return false;
        }

        public static bool CheckWKTSyntax(string? polygon, QueryFactory queryFactory)
        {
            try
            {
                var wktwithsrid = GetWKTAndSRIDFromQuerystring(polygon);

                if (wktwithsrid != null)
                {
                    var query = queryFactory.Query().SelectRaw($"Count(ST_GeometryFromText('{wktwithsrid.Value.Item1}',{wktwithsrid.Value.Item2}))").Get<int>();

                    if (query != null && query.FirstOrDefault() == 1)
                        return true;
                }

                return false;
            }
            catch { return false; }            
        }

        public static (string,string)? GetWKTAndSRIDFromQuerystring(string? polygon)
        {
            string srid = "4326";
            string wkt = "";

            //IF S7Rid is added
            if (polygon != null && polygon.Split(";").Length > 1)
            {
                var splitted = polygon.Split(";");
                var sridstr = splitted[1].ToUpper();
                if (sridstr.StartsWith("SRID="))
                {
                    sridstr = sridstr.Replace("SRID=", "");
                    
                    if (CheckValidWKTString(splitted[0]))
                    {
                        wkt = splitted[0];
                        return (wkt, sridstr);
                    }
                }
            }
            //IF only wkt is added
            else if (polygon != null && polygon.Split(';').Length == 1)
            {
                if (CheckValidWKTString(polygon))
                {
                    wkt = polygon;

                    return (wkt, srid);
                }
            }
            
            return null;            
        }


    }
    

    public class DistanceCalculator
    {
        public const double suedtirolMitteLatitude = 46.655781;
        public const double suedtirolMitteLongitude = 11.4296877;


        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //:::                                                                         :::
        //:::  This routine calculates the distance between two points (given the     :::
        //:::  latitude/longitude of those points). It is being used to calculate     :::
        //:::  the distance between two locations using GeoDataSource(TM) products    :::
        //:::                                                                         :::
        //:::  Definitions:                                                           :::
        //:::    South latitudes are negative, east longitudes are positive           :::
        //:::                                                                         :::
        //:::  Passed to function:                                                    :::
        //:::    lat1, lon1 = Latitude and Longitude of point 1 (in decimal degrees)  :::
        //:::    lat2, lon2 = Latitude and Longitude of point 2 (in decimal degrees)  :::
        //:::    unit = the unit you desire for results                               :::
        //:::           where: 'M' is statute miles (default)                         :::
        //:::                  'K' is kilometers                                      :::
        //:::                  'N' is nautical miles                                  :::
        //:::                                                                         :::
        //:::  Worldwide cities and other features databases with latitude longitude  :::
        //:::  are available at http://www.geodatasource.com                          :::
        //:::                                                                         :::
        //:::  For enquiries, please contact sales@geodatasource.com                  :::
        //:::                                                                         :::
        //:::  Official Web site: http://www.geodatasource.com                        :::
        //:::                                                                         :::
        //:::           GeoDataSource.com (C) All Rights Reserved 2015                :::
        //:::                                                                         :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public static double Distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(Deg2rad(lat1)) * Math.Sin(Deg2rad(lat2)) + Math.Cos(Deg2rad(lat1)) * Math.Cos(Deg2rad(lat2)) * Math.Cos(Deg2rad(theta));
            dist = Math.Acos(dist);
            dist = Rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist *= 1.609344;
            }
            else if (unit == 'N')
            {
                dist *= 0.8684;
            }
            return (dist);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double Deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double Rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

    }

    public class PGGeoSearchResult
    {
        public bool geosearch { get; set; } = false;
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int radius { get; set; }
    }

    public class GeoPolygonSearchResult
    {
        public string? operation { get; set; }
        public List<Tuple<double,double>>? polygon { get; set; }
        public string? wktstring { get; set; } = null;        
        public string srid { get; set; } = "4326";
    }

    public class RavenGeoSearchResult
    {
        public bool geosearch { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int radius { get; set; }
    }
}
