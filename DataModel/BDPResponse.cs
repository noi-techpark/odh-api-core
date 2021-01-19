using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
    public class BDPResponseObject
    {
        public int offset { get; set; }
        public ICollection<BDPData> data { get; set; }
        public int limit { get; set; }
    }

    public class BDPData
    {
        //public string scode { get; set; }
        //public bool sactive { get; set; }
        //public string stype { get; set; }
        //public bool savailable { get; set; }
        //public string sname { get; set; }
        //public string sorigin { get; set; }
        public BDPMetaData smetadata { get; set; }
        //public BDPCoordinate scoordinate { get; set; }        
    }

    public class BDPMetaData
    {
        public double gps { get; set; }
        public string type { get; set; }
        public int floor { get; set; }
        public string company { get; set; }
        public string beacon_id { get; set; }
        public string beacons_color { get; set; }

        public string poi_name_de { get; set; }
        public string poi_name_it { get; set; }
        public string poi_name_en { get; set; }
        public int beacons_amount { get; set; }

        public string todaynoibzit { get; set; }

        public string notes { get; set; }

        public string link { get; set; }
    }

    public class BDPCoordinate
    {
        public double x { get; set; }
        public double y { get; set; }
        public int srid { get; set; }
    }
}
