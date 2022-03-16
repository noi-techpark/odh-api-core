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
            myodhactivitypoilinked.SyncSourceInterface = "liftbase";

            myodhactivitypoilinked.Id = "dss_" + dssitem.rid;
            myodhactivitypoilinked.CustomId = dssitem.pid;

            myodhactivitypoilinked.HasLanguage = new List<string>() { "de", "it", "en" };

            if (myodhactivitypoilinked.FirstImport == null)
                myodhactivitypoilinked.FirstImport = DateTime.Now;

            var lastchangeobj = (string)dssitem["update-date"];

            if(double.TryParse(lastchangeobj, out double updatedate))
                myodhactivitypoilinked.LastChange = Helper.DateTimeHelper.UnixTimeStampToDateTime(updatedate);

            //name

            //description

            //info-text

            //info-text-summer

            //lifttype TODO Mapping

            //skiresort TODO Mapping 

            //Operationschedule (opening-times, opening-times-summer, season-summer, season-winter)

            //Properties (length, capacity, capacity-per-hour, altitude-start, altitude-end, height-difference, summercard-points, bike-transport, duration)

            //Other (regionId, duration, state-winter, state-summer, datacenterId, number, winterOperation, sorter, summerOperation, sorterSummer)

            //Location (location, locationMountain, gePositionFile)

            return myodhactivitypoilinked;
        }

    }
}


//liftbase
