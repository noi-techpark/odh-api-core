// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public static class GpsConverter
    {

        public static IDictionary<string, GpsInfo> ToGpsPointsDictionary(this ICollection<GpsInfo>? gpsinfos, bool ltsactivitypoi = false)
        {
            //ODHActivityPoi should already pass
            if (!ltsactivitypoi)
            {
                if (gpsinfos != null && gpsinfos.Count > 0)
                {
                    //If GPSInfo has GpsType = null or Empty --->
                    if (gpsinfos.Any(x => String.IsNullOrEmpty(x.Gpstype)))
                    {
                        foreach(var gpsinfo in gpsinfos.Where(x => String.IsNullOrEmpty(x.Gpstype)))
                        {
                            gpsinfo.Gpstype = "position";
                        }
                    }

                    //If GPSInfo has more GpsType = position ---> Grouped count is inferior to totalcount GpsPoints returns only first element.....
                    if (gpsinfos.GroupBy(x => x.Gpstype).Count() < gpsinfos.Count)
                    {

                        return gpsinfos
                            .DistinctBy(x => x.Gpstype)
                            .Where(x => x.Gpstype != null)
                            .ToDictionary(x => x.Gpstype!, x => x);
                    }
                    else
                    {
                        return gpsinfos
                            .DistinctBy(x => x.Gpstype)
                            .Where(x => x.Gpstype != null)
                            .ToDictionary(x => x.Gpstype!, x => x);
                    }                      
                }                    
                else
                    return new Dictionary<string, GpsInfo>();
            }   
            //For LTS POI & LTS Activity
            else
            {
                var gpspoints = new Dictionary<string, GpsInfo>();
                var positioncount = 0;

                if (gpsinfos != null)
                {
                    foreach (var gpsinfo in gpsinfos)
                    {
                        string postionstr = "position";                        

                        if (positioncount > 0)
                            postionstr = postionstr + positioncount;

                        if (gpsinfo.Gpstype == "Endpunkt" || gpsinfo.Gpstype == "Bergstation")
                            gpspoints.Add("endposition", gpsinfo);

                        if (gpsinfo.Gpstype == "position" || gpsinfo.Gpstype == "Standpunkt" || gpsinfo.Gpstype == "Startpunkt" || gpsinfo.Gpstype == "Start und Ziel" || gpsinfo.Gpstype == "Talstation")
                            gpspoints.Add(postionstr, gpsinfo);

                        positioncount = gpspoints.Where(x => x.Key.StartsWith("position")).Count();
                    }
                }

                return gpspoints;
            }           
        }

        public static ICollection<GpsInfo> ConvertGpsInfoOnRootToGpsInfoArray(this IGpsInfo gpsinfo)
        {
            if (gpsinfo.Latitude != 0 && gpsinfo.Longitude != 0)
            {
                return new List<GpsInfo>
                    {
                        new GpsInfo(){ Gpstype = "position", Altitude = gpsinfo.Altitude, AltitudeUnitofMeasure = gpsinfo.AltitudeUnitofMeasure, Latitude = gpsinfo.Latitude, Longitude = gpsinfo.Longitude }
                    };
            }
            else
            {
                return null;
            }
        }
    }
}
