using DataModel;
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
            //var jsondeserialized = JsonConvert.DeserializeObject(dssitem);


            bool newobject = false;

            if (myodhactivitypoilinked == null)
                myodhactivitypoilinked = new ODHActivityPoiLinked();

            myodhactivitypoilinked.Source = "dss";
            myodhactivitypoilinked.SyncSourceInterface = "dssliftbase";

            myodhactivitypoilinked.Id = "dss_" + dssitem.rid;
            myodhactivitypoilinked.CustomId = dssitem.pid;

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

            //info-text

            //info-text-summer

            //lifttype TODO Mapping

            //skiresort TODO Mapping 

            //Operationschedule (opening-times, opening-times-summer, season-summer, season-winter)
            myodhactivitypoilinked.OperationSchedule = new List<OperationSchedule>();
            myodhactivitypoilinked.OperationSchedule.Add(ParseToODHOperationScheduleFormat("winter", dssitem.data));
            myodhactivitypoilinked.OperationSchedule.Add(ParseToODHOperationScheduleFormat("summer", dssitem.data));

            //Properties (length, capacity, capacity-per-hour, altitude-start, altitude-end, height-difference, summercard-points, bike-transport, duration)
            var length = (double)dssitem["data"]["length"];
            var altitudestart = (int)dssitem["data"]["altitude-start"];
            var altitudeend = (int)dssitem["data"]["altitude-end"];
            var heightdifference = (int)dssitem["data"]["height-difference"];

            var biketransport = (bool)dssitem["data"]["bike-transport"];

            //Other (regionId, duration, state-winter, state-summer, datacenterId, number, winterOperation, sorter, summerOperation, sorterSummer)

            //Location (location, locationMountain, gePositionFile)

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

        

    }
}


//liftbase
