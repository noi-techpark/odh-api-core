using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

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
                    return gpsinfos
                        .Distinct()
                        .Where(x => x.Gpstype != null)
                        .ToDictionary(x => x.Gpstype!, x => x);
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

                        positioncount = gpspoints.Where(x => x.Key == "position").Count();
                    }
                }

                return gpspoints;
            }           
        }


        //public IDictionary<string, GpsInfo> GpsPoints
        //{
        //    get
        //    {
        //        if (this.GpsInfo != null && this.GpsInfo.Count > 0)
        //        {
        //            return this.GpsInfo.ToDictionary(x => x.Gpstype, x => x);
        //        }
        //        else
        //        {
        //            return new Dictionary<string, GpsInfo>
        //            {
        //            };
        //        }
        //    }
        //}

    }
}
