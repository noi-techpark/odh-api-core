// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIGIWAY
{
    public class DigiWayRoutesCycleWaysResult
    {
        public string type { get; set; }

	    public ICollection<DigiWayRoutesCycleWays> features { get; set; }
    }

    public class DigiWayRoutesCycleWays
    {
        public string type { get; set; }
        public string id { get; set; }
        public Geometry geometry { get; set; }
        public string geometry_name { get; set; }
        public Properties properties { get; set; }
        public float[] bbox { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public float[][][] coordinates { get; set; }
    }

    public class Properties
    {
        public int ID { get; set; }
        public string OBJECT { get; set; }
        public string ROUTE_NUMBER { get; set; }
        public string ROUTE_NAME { get; set; }
        public string ROUTE_TYPE { get; set; }
        public string ROUTE_DESC { get; set; }
        public string ROUTE_START { get; set; }
        public string ROUTE_END { get; set; }
        public string MUNICIPALITY { get; set; }
        public string REGION { get; set; }
        public string RUNNING_TIME { get; set; }
        public string DIFFICULTY { get; set; }
        public string STATUS { get; set; }
        public string STATUS_DATE { get; set; }
        public int DOWNHILL_METERS { get; set; }
        public int UPHILL_METERS { get; set; }
        public int START_HEIGHT { get; set; }
        public int END_HEIGHT { get; set; }
        public float LENGTH { get; set; }
        public string CREATE_DATE { get; set; }
        public string UPDATE_DATE { get; set; }
    }

}
