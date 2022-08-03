using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public static class GpsExtensions
    {
        public static Dictionary<string, GpsInfo> ConvertGpsInfoToGpsPoints(this List<GpsInfo> gpsinfolist)
        {
            Dictionary<string, GpsInfo> gpspoints = new Dictionary<string, GpsInfo>();
            foreach (var gpsinfo in gpsinfolist)
            {
                if (gpsinfo.Gpstype != null)
                    gpspoints.Add(gpsinfo.Gpstype, gpsinfo);
            }

            //TODO LINQ SELECT

            return gpspoints;
        }

        public static Dictionary<string, GpsInfo> ConvertGpsInfoToGpsPointsLinq(this List<GpsInfo> gpsinfolist)
        {
            if (gpsinfolist != null && gpsinfolist.Count > 0)
                return gpsinfolist
                    .Distinct()
                    .Where(x => x.Gpstype != null)
                    .ToDictionary(x => x.Gpstype!, x => x);
            else
                return new Dictionary<string, GpsInfo>();
        }
    }
}
