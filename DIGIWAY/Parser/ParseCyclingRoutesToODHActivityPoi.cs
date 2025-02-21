// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIGIWAY.Parser
{
    public class ParseCyclingRoutesToODHActivityPoi
    {

        public static (ODHActivityPoiLinked, GeoShapeJson) ParseDigiWayCyclingRoutesToODHActivityPoi(
            ODHActivityPoiLinked? odhactivitypoi,
            DigiWayRoutesCycleWays digiwaydata
        )
        {
            if(odhactivitypoi == null)
                odhactivitypoi = new ODHActivityPoiLinked();

            odhactivitypoi.Id = digiwaydata.id;
            odhactivitypoi.FirstImport = Convert.ToDateTime(digiwaydata.properties.CREATE_DATE);
            odhactivitypoi.LastChange = Convert.ToDateTime(digiwaydata.properties.UPDATE_DATE);
            odhactivitypoi.HasLanguage = new List<string>() { "de" };
            odhactivitypoi.Shortname = digiwaydata.properties.ROUTE_NAME;

            GeoShapeJson geoshape = new GeoShapeJson();
            //geoshape.name = digiwaydata.id;
            //geoshape.geom = digiwaydata.geometry


            return (odhactivitypoi, geoshape);
        }

    }
}
