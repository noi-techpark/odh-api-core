using DataModel;
using Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSS.Parser
{
    public class ParseDSSToODHActivityPoi
    {        
        public static ODHActivityPoiLinked ParseDSSDataToODHActivityPoi(ODHActivityPoiLinked? myodhactivitypoilinked, dynamic dssitem)
        {            
            if (myodhactivitypoilinked == null)
                myodhactivitypoilinked = new ODHActivityPoiLinked();

            myodhactivitypoilinked.Active = true;
            myodhactivitypoilinked.SmgActive = true;

            myodhactivitypoilinked.Source = "dss";
            myodhactivitypoilinked.SyncSourceInterface = "dssliftbase";
            myodhactivitypoilinked.SyncUpdateMode = "full";

            myodhactivitypoilinked.Id = "dss_" + dssitem.rid;
            myodhactivitypoilinked.CustomId = dssitem.pid;

            myodhactivitypoilinked.Type = "Anderes";
            myodhactivitypoilinked.SubType = "Aufstiegsanlagen";

            myodhactivitypoilinked.SmgTags = new List<string>();
            myodhactivitypoilinked.SmgTags.Add(myodhactivitypoilinked.Type);
            myodhactivitypoilinked.SmgTags.Add(myodhactivitypoilinked.SubType);

            myodhactivitypoilinked.HasLanguage = new List<string>() { "de", "it", "en" };

            if (myodhactivitypoilinked.FirstImport == null)
                myodhactivitypoilinked.FirstImport = DateTime.Now;

            var lastchangeobj = (string)dssitem["update-date"];

            if(double.TryParse(lastchangeobj, out double updatedate))
                myodhactivitypoilinked.LastChange = Helper.DateTimeHelper.UnixTimeStampToDateTime(updatedate);

            //name
            var namede = (string)dssitem["name"]["de"];
            var nameit = (string)dssitem["name"]["it"];
            var nameen = (string)dssitem["name"]["en"];

            //description
            var descde = (string)dssitem["description"]["de"];
            var descit = (string)dssitem["description"]["it"];
            var descen = (string)dssitem["description"]["en"];

            //info-text TODO

            //info-text-summer TODO

            myodhactivitypoilinked.Detail.Add("de", new Detail() { Language = "de", Title= namede, BaseText = descde });
            myodhactivitypoilinked.Detail.Add("it", new Detail() { Language = "it", Title = nameit, BaseText = descit });
            myodhactivitypoilinked.Detail.Add("en", new Detail() { Language = "en", Title = nameen, BaseText = descen });

            //lifttype TODO Mapping
            List<string> lifftype = ParseDSSTypeToODHType(dssitem["lifttype"]);
            foreach(var tag in lifftype)
            {
                myodhactivitypoilinked.SmgTags.Add(tag);
            }
            

            //skiresort TODO Mapping 

            //Operationschedule (opening-times, opening-times-summer, season-summer, season-winter)
            myodhactivitypoilinked.OperationSchedule = new List<OperationSchedule>();
            myodhactivitypoilinked.OperationSchedule.Add(ParseToODHOperationScheduleFormat("winter", dssitem.data));
            myodhactivitypoilinked.OperationSchedule.Add(ParseToODHOperationScheduleFormat("summer", dssitem.data));

            //Properties (length, capacity, capacity-per-hour, altitude-start, altitude-end, height-difference, summercard-points, bike-transport, duration)
            var length = (double)dssitem["data"]["length"];
            myodhactivitypoilinked.DistanceLength = length;

            var altitudestart = (int)dssitem["data"]["altitude-start"];
            myodhactivitypoilinked.AltitudeLowestPoint = altitudestart;

            var altitudeend = (int)dssitem["data"]["altitude-end"];
            myodhactivitypoilinked.AltitudeHighestPoint = altitudeend;

            var heightdifference = (int)dssitem["data"]["height-difference"];
            myodhactivitypoilinked.AltitudeDifference = heightdifference;

            var biketransport = (bool)dssitem["data"]["bike-transport"];
            myodhactivitypoilinked.BikeTransport = biketransport;


            //Other (regionId, duration, state-winter, state-summer, datacenterId, number, winterOperation, sorter, summerOperation, sorterSummer)

            var duration = (string)dssitem["data"]["duration"];            
            myodhactivitypoilinked.DistanceDuration = ConvertToDistanceDuration(duration);

            var number = (string)dssitem["data"]["number"];
            myodhactivitypoilinked.Number = number;


            var regionid = (int)dssitem["data"]["regionId"];
            //isopen?
            var statewinter = (int)dssitem["data"]["state-winter"];
            //isopen?
            var statesummer = (int)dssitem["data"]["state-summer"];
            //?
            var datacenterId = (int)dssitem["data"]["datacenterId"];

            var winterOperation = (bool)dssitem["data"]["winterOperation"];
            var summerOperation = (bool)dssitem["data"]["summerOperation"];

            var sorterSummer = (bool)dssitem["data"]["sorterSummer"];


            //TODO LOCATIONINFO


            //TODO SKIAREAINFO

            
            myodhactivitypoilinked.GpsTrack = ParseToODHGpsTrack((string)dssitem["geoPositionFile"]);

            List<GpsInfo> gpsinfolist = ParseToODHGpsInfo(dssitem["location"], dssitem["locationMountain"], altitudestart, altitudeend);

            myodhactivitypoilinked.GpsInfo = gpsinfolist;
            myodhactivitypoilinked.GpsPoints = gpsinfolist.ConvertGpsInfoToGpsPointsLinq();

            return myodhactivitypoilinked;
        }

        private static OperationSchedule ParseToODHOperationScheduleFormat(string season, dynamic data)
        {
            string summer = "";

            Dictionary<string, string> seasonname = new Dictionary<string, string>();    
            if(season == "summer")
            {
                summer = "-summer";
                seasonname.Add("de", "Sommersaison");
                seasonname.Add("it", "stagioneestiva");
                seasonname.Add("en", "summerseason");
            }
            else if (season == "winter")
            {
                seasonname.Add("de", "Wintersaison");
                seasonname.Add("it", "stagioneinvernale");
                seasonname.Add("en", "winterseason");
            }


            var seasonstart = (double)data["season-" + season]["start"];
            var seasonend = (double)data["season-" + season]["end"];

            var openingtimestart = (string)data["opening-times" + summer]["start"];
            var openingtimeend = (string)data["opening-times" + summer]["end"];
            var openingtimestartafternoon = (string)data["opening-times" + summer]["startAfternoon"];
            var openingtimeendafternoon = (string)data["opening-times" + summer]["endAfternoon"];

            //Season
            OperationSchedule operationSchedule = new OperationSchedule();
            operationSchedule.Type = "1";
            operationSchedule.OperationscheduleName = seasonname;
            operationSchedule.Start = Helper.DateTimeHelper.UnixTimeStampToDateTime(seasonstart);
            operationSchedule.Stop = Helper.DateTimeHelper.UnixTimeStampToDateTime(seasonend);

            operationSchedule.OperationScheduleTime = new List<OperationScheduleTime>();

            OperationScheduleTime operationScheduleTime = new OperationScheduleTime();
            operationScheduleTime.Timecode = 1;
            operationScheduleTime.Monday = true;
            operationScheduleTime.Tuesday = true;
            operationScheduleTime.Wednesday = true;
            operationScheduleTime.Friday = true;
            operationScheduleTime.Saturday = true;
            operationScheduleTime.Sunday = true;

            operationScheduleTime.Start = TimeSpan.Parse(openingtimestart);
            operationScheduleTime.End = TimeSpan.Parse(openingtimeend);

            operationSchedule.OperationScheduleTime.Add(operationScheduleTime);

            //Check if there is one or two openingtimes
            if (openingtimestartafternoon != "" && openingtimestartafternoon != "00:00")
            {
                OperationScheduleTime operationScheduleTimeafternoon = new OperationScheduleTime();
                operationScheduleTimeafternoon.Timecode = 1;
                operationScheduleTimeafternoon.Monday = true;
                operationScheduleTimeafternoon.Tuesday = true;
                operationScheduleTimeafternoon.Wednesday = true;
                operationScheduleTimeafternoon.Friday = true;
                operationScheduleTimeafternoon.Saturday = true;
                operationScheduleTimeafternoon.Sunday = true;

                operationScheduleTimeafternoon.Start = TimeSpan.Parse(openingtimestartafternoon);
                operationScheduleTimeafternoon.End = TimeSpan.Parse(openingtimeendafternoon);

                operationSchedule.OperationScheduleTime.Add(operationScheduleTime);
            }            

            return operationSchedule;
        }

        private static List<GpsInfo> ParseToODHGpsInfo(dynamic location, dynamic locationMountain, int altitudestart = 0, int altitudeend = 0)
        {
            List<GpsInfo> gpsinfolist = new List<GpsInfo>();

            if(location != null)
            {
                var lat = (double?)location["lat"];
                var lon = (double?)location["lon"];

                if(lat != null && lon != null)
                {
                    gpsinfolist.Add(new GpsInfo() { AltitudeUnitofMeasure = "m", Altitude = altitudestart, Gpstype = "position", Latitude = lat.Value, Longitude = lon.Value });
                    gpsinfolist.Add(new GpsInfo() { AltitudeUnitofMeasure = "m", Altitude = altitudestart, Gpstype = "valleystationpoint", Latitude = lat.Value, Longitude = lon.Value });
                }
            }

            if (locationMountain != null)
            {
                var lat = (double?)location["lat"];
                var lon = (double?)location["lon"];

                if (lat != null && lon != null)
                {
                    gpsinfolist.Add(new GpsInfo() { AltitudeUnitofMeasure = "m", Altitude = altitudeend, Gpstype = "mountainstationpoint", Latitude = lat.Value, Longitude = lon.Value });
                }
            }

            return gpsinfolist;
        }

        private static List<GpsTrack> ParseToODHGpsTrack(dynamic geoPositionFile)
        {
            List<GpsTrack> gpstracklist = new List<GpsTrack>();
            
            if(geoPositionFile != null && String.IsNullOrEmpty(geoPositionFile))
            {
                GpsTrack track = new GpsTrack();
                track.GpxTrackUrl = geoPositionFile;
                gpstracklist.Add(track);
            }

            return gpstracklist;
        }

        private static List<string> ParseDSSTypeToODHType(dynamic lifttype) 
        {
            List<string> odhtagstoadd = new List<string>();

            int lifttyperid = (int)lifttype["rid"];

            DSSTypeAufstiegsanlagen flag = (DSSTypeAufstiegsanlagen)lifttyperid;
            var flagstring = flag.GetDescription<DSSTypeAufstiegsanlagen>();

            if(flagstring != null)
                odhtagstoadd.Add(flagstring);

            //TODO Add Sessellift if it is of type 

            return odhtagstoadd;
        }

        private static double ConvertToDistanceDuration(string distanceduration)
        {
            if (double.TryParse(distanceduration, out double distancedurationresult))
            {
                //Convert the result from seconds to a double value

                TimeSpan durationts = TimeSpan.FromSeconds(distancedurationresult);

                //int tshours = Convert.ToInt32(durationsplittet[0]);
                //int tsminutes = Convert.ToInt32(durationsplittet[1]);

                //hike.DistanceDuration = Math.Round(tshours + tsminutes / 60.0, 2);
                return Math.Round(durationts.TotalHours, 2);
            }
            else
                return 0;
        }
    }
}

//lifttype rid: 1, Seilbahn --> Seilbahn
//lifttype rid: 2, 
//lifttype rid: 3, Kabinenbahn --> Kabinenbahn
//lifttype rid: 4, Unterirdische Bahn --> Unterirdische Bahn
//lifttype rid: 5, 
//lifttype rid: 6, 
//lifttype rid: 7, Sessellift 2 --> Sessellift
//lifttype rid: 8, Sessellift 3 --> Sessellift
//lifttype rid: 9, Skilift --> Skilift
//lifttype rid: 10, Aufzug --> Schrägaufzug
//lifttype rid: 11, Kleinskilift --> Klein-Skilift
//lifttype rid: 12, Telemix --> Telemix
//lifttype rid: 13, Standseilbahn --> Standseilbahn/Zahnradbahn
//lifttype rid: 14, Skibus --> Skibus
//lifttype rid: 15, Zug --> Zug
//lifttype rid: 16, Sessellift 4 --> Sessellift
//lifttype rid: 17, Sessellift 6 --> Sessellift
//lifttype rid: 18, Sessellift 8 --> Sessellift
//lifttype rid: 19, Skiförderband --> Förderband
//lifttype rid: 20, 
//lifttype rid: 21, Sessellift 4 mit Kuppel --> 4er Sessellift kuppelbar
//lifttype rid: 22, Sessellift 6 mit Kuppel --> 6er Sessellift kuppelbar
//lifttype rid: 22, Sessellift 8 mit Kuppel --> 8er Sessellift kuppelbar


//Skiresort rid 1 Falzarego
//Skiresort rid 8 Auronzo 
//Skiresort rid 7 San Vito di Cadore
//Skiresort rid 4 Faloria 
//Skiresort rid 2 Tofana 
//Skiresort rid 5 Cristallo 
//Skiresort rid 6 Misurina
//Skiresort rid 76 Fedare 
//Skiresort rid 9 Kronplatz
//Skiresort rid 10 Corvara 
//Skiresort rid 11 Colfosco 
//Skiresort rid 12 Stern
//Skiresort rid 13 San Cassiano 
//Skiresort rid 14 Badia 
//Skiresort rid 19 Wolkenstein - Dantercepies 
//Skiresort rid 21 Wolkenstein - Plan de Gralba 
//Skiresort rid 22 Wolkenstein - Sellajoch
//Skiresort rid 20 St. Cristina - Ciampinoi - Wolkenstein 
//Skiresort rid 18 St. Cristina - Monte Pana 
//Skiresort rid 16 St. Ulrich - Resciesa - Seceda - Col Raiser - St. Cristina 
//Skiresort rid 17 St. Ulrich - Palmer - Furdenan
//Skiresort rid 23 Seiser Alm 
//Skiresort rid 26 Canazei - Belvedere
//Skiresort rid 27 Campitello Col Rodella
//Skiresort rid 25 Alba - Ciampac 
//Skiresort rid 28 Pozza - Buffaure 
//Skiresort rid 29 Vigo - Catinaccio
//Skiresort rid 31 Passo Fedaia - Marmolada 
//Skiresort rid 30 Karerpass 
//Skiresort rid 55 Arabba
//Skiresort rid 65 Marmolada
//Skiresort rid 32 Helm - Stiergarten - Rotwand
//Skiresort rid 33 Monte Baranci
//Skiresort rid 35 Toblach - Rienz
//Skiresort rid 36 Waldheim
//Skiresort rid 37 Kreuzbergpass
//Skiresort rid 38 Prags
//Skiresort rid 77 Padola
//Skiresort rid 39 Alpe Cermis
//Skiresort rid 43 Bellamonte  
//Skiresort rid 79 Alpe Lusia
//Skiresort rid 28624570 Jochgrimm
//Skiresort rid 44 Passo Lavazé
//Skiresort rid 78 Passo Rolle
//Skiresort rid 46 Passo Rolle  ??
//Skiresort rid 40 Pampeago
//Skiresort rid 42 Predazzo
//Skiresort rid 41 Obereggen
//Skiresort rid 45 Deutschnofen - Petersberg
//Skiresort rid 47 San Martino di Castrozza - Carosello delle Malghe
//Skiresort rid 48 San Martino di Castrozza - Colverde
//Skiresort rid 49 San Martino di Castrozza - Pra delle Nasse
//Skiresort rid 70 Gitschberg - Jochtal 
//Skiresort rid 69 Plose - Brixen 
//Skiresort rid 72 Feldthurns
//Skiresort rid 73 Lüsen
//Skiresort rid 74 Vilnösstal
//Skiresort rid 62 San Pellegrino
//Skiresort rid 67 Lusia
//Skiresort rid 68 Bellamonte
//Skiresort rid 27 Campitello Col Rodella
//Skiresort rid 51 Alleghe
//Skiresort rid 52 Zoldo
//Skiresort rid 53 Palafavera
//Skiresort rid 54 Selva di Cadore

