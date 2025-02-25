// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helper.Extensions;
using Helper;
using System.Globalization;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.IO;
using NetTopologySuite.Algorithm;

namespace DIGIWAY
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
            odhactivitypoi.Detail = new Dictionary<string, Detail>();
            odhactivitypoi.Detail.TryAddOrUpdate<string, Detail>("de", new Detail()
            {
                Title = digiwaydata.properties.ROUTE_NAME,
                BaseText = digiwaydata.properties.ROUTE_DESC,
                Header = digiwaydata.properties.ROUTE_TYPE,
                AdditionalText = digiwaydata.properties.ROUTE_NUMBER
            });
            odhactivitypoi.ContactInfos = new Dictionary<string, ContactInfos>();
            odhactivitypoi.ContactInfos.TryAddOrUpdate<string, ContactInfos>("de", new ContactInfos()
            {
                City = digiwaydata.properties.MUNICIPALITY,
                Region = digiwaydata.properties.REGION                
            });
            odhactivitypoi.DistanceLength = digiwaydata.properties.LENGTH;
            odhactivitypoi.Difficulty = digiwaydata.properties.DIFFICULTY;
            odhactivitypoi.AltitudeSumDown = digiwaydata.properties.DOWNHILL_METERS;
            odhactivitypoi.AltitudeSumUp = digiwaydata.properties.UPHILL_METERS;
            odhactivitypoi.Source = "digiway";
            //Add Tags


            GeoShapeJson geoshape = new GeoShapeJson();
            geoshape.Name = digiwaydata.id;
            geoshape.Type = "cycleway";
            geoshape.Type_Uts = digiwaydata.properties.OBJECT;
            geoshape.Name_Alternative = digiwaydata.properties.ROUTE_NAME;
            geoshape.Shape_area = 0;
            geoshape.Shape_length = 0;

            //Check
            //List<LineString> linestringlist = new List<LineString>();

            List<Coordinate> coordinates = new List<Coordinate>();

            string coordinatesstr = "MULTILINESTRING((";
            foreach(var coordinate1 in digiwaydata.geometry.coordinates)
            {
                foreach (var coordinate2 in coordinate1)
                {
                    //List<Coordinate> coordinates = new List<Coordinate>();

                    List<double> coords = new List<double>();

                    foreach (var coordinate in coordinate2)
                    {
                        coords.Add(coordinate);
                        coordinatesstr = coordinatesstr + coordinate.ToString(CultureInfo.InvariantCulture) + " ";
                    }

                    if(coords.Count == 2)
                        coordinates.Add(new Coordinate(coords[0], coords[1]));

                    coordinatesstr = coordinatesstr.Remove(coordinatesstr.Length - 1);
                    coordinatesstr = coordinatesstr + ",";                                       
                }
                coordinatesstr = coordinatesstr.Remove(coordinatesstr.Length - 1);
            }
            coordinatesstr = coordinatesstr + "))";

            WKTReader reader = new WKTReader();
            Geometry geom = reader.Read(coordinatesstr);


            //var geometryfactory = new GeometryFactory();
            //List<LineString> lines = new List<LineString>();
            //foreach(var coord in geom.Coordinates)
            //{
            //    lines.Add()
            //}
            //geometryfactory.CreateMultiLineString(geometryfactory.CreateLineString(geom.Coordinates))

            //geoshape.Geometry = geom;


            var geomfactory = new GeometryFactory();
            var linestring = geomfactory.WithSRID(32632).CreateLineString(coordinates.ToArray());

            geoshape.Geometry = linestring;

            //To check if it can be done with linq
            //var test = "MULTILINESTRING((" + String.Join(",", digiwaydata.geometry.coordinates.SelectMany(x => x)) +"))";

            //geoshape.geom = digiwaydata.geometry


            return (odhactivitypoi, geoshape);
        }

    }
}
