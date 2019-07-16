using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Helper
{
    public class GeoSearchHelper
    {
        double _eQuatorialEarthRadius = 6378.1370D;
        double _d2r = (Math.PI / 180D);

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
            double latitudecheck;
            bool isLatDouble = Double.TryParse(latitude, NumberStyles.Any, culture, out latitudecheck);
            double longitudecheck;
            bool isLongDouble = Double.TryParse(longitude, NumberStyles.Any, culture, out longitudecheck);
            int radiuscheck;
            bool isRadiusInt = Int32.TryParse(radius, out radiuscheck);

            if (isLatDouble && isLongDouble && isRadiusInt)
                geosearch = true;

            return geosearch;
        }

        public static PGGeoSearchResult GetPGGeoSearchResult(string? latitude, string? longitude, string? radius)
        {
            if (latitude ==  null && longitude == null)
                return new PGGeoSearchResult() { geosearch = false, latitude = 0, longitude = 0, radius = 0 };

            PGGeoSearchResult pggeosearchresult = new PGGeoSearchResult();

            pggeosearchresult.geosearch = false;
            //Check of Geosearch durchgeführt werden kann                    
            CultureInfo culture = CultureInfo.InvariantCulture;
            double latitudecheck;
            bool isLatDouble = Double.TryParse(latitude, NumberStyles.Any, culture, out latitudecheck);
            double longitudecheck;
            bool isLongDouble = Double.TryParse(longitude, NumberStyles.Any, culture, out longitudecheck);
            int radiuscheck;
            bool isRadiusInt = Int32.TryParse(radius, out radiuscheck);

            if (isLatDouble && isLongDouble)
            {
                pggeosearchresult.geosearch = true;
                pggeosearchresult.latitude = latitudecheck;
                pggeosearchresult.longitude = longitudecheck;

                if (isRadiusInt)
                    pggeosearchresult.radius = radiuscheck;
                else
                    pggeosearchresult.radius = 150000;

                //Check ob das ganze sinn macht
                var actualdistance = DistanceCalculator.distance(pggeosearchresult.latitude, pggeosearchresult.longitude, DistanceCalculator.suedtirolMitteLatitude, DistanceCalculator.suedtirolMitteLongitude, 'K');
                if (actualdistance > 200)
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

        public static RavenGeoSearchResult GetRavenGeoSearchResult(string latitude, string longitude, string radius)
        {
            if (latitude ==  null && longitude == null)
                return new RavenGeoSearchResult() { geosearch = false, latitude = 0, longitude = 0, radius = 0 };

            RavenGeoSearchResult pggeosearchresult = new RavenGeoSearchResult();

            pggeosearchresult.geosearch = false;
            //Check of Geosearch durchgeführt werden kann                    
            CultureInfo culture = CultureInfo.InvariantCulture;
            double latitudecheck;
            bool isLatDouble = Double.TryParse(latitude, NumberStyles.Any, culture, out latitudecheck);
            double longitudecheck;
            bool isLongDouble = Double.TryParse(longitude, NumberStyles.Any, culture, out longitudecheck);
            int radiuscheck;
            bool isRadiusInt = Int32.TryParse(radius, out radiuscheck);

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
                var actualdistance = DistanceCalculator.distance(pggeosearchresult.latitude, pggeosearchresult.longitude, DistanceCalculator.suedtirolMitteLatitude, DistanceCalculator.suedtirolMitteLongitude, 'K');
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

        public static double distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }
            return (dist);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

    }

    public class PGGeoSearchResult
    {
        public bool geosearch { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int radius { get; set; }
    }

    public class RavenGeoSearchResult
    {
        public bool geosearch { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int radius { get; set; }
    }
}
