﻿using DataModel;
using LCS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LCS
{
    public class GetSnowReport
    {
        //Get Simple Snowreport for one Area
        public static SnowReportBaseData GetLiveSnowReport(
            string lang,
            SkiArea myskiarea,
            string requestor,
            string ltsuser,
            string ltspswd,
            string ltsmsgpswd
        )
        {
            List<string> myareas = myskiarea.AreaId.ToList();

            List<string> prefilter2 = new List<string>();
            prefilter2.Add("1330A749F05A441EA702B64A8F82CD3C"); //Lift
            prefilter2.Add("CFF9182A21DC4C0B88030EBE98D73A67"); //Pischtn
            prefilter2.Add("C84ACF76753D48BCA2F3E5ACC89FF45E"); //Longlafn
            prefilter2.Add("077EED61EE2F49AE95E472EA7B5C93AB"); //Roudlan

            List<string> activitytype = new List<string>();
            activitytype.Add("LIFT");
            activitytype.Add("SLOPE");
            activitytype.Add("SKITRACK");
            activitytype.Add("SLIDE");

            //INFOS für LIFT SLOPE SKITRACK SLIDE
            var myactivityrequest = LCS.GetActivityDataLCS.GetActivitySearchRequestAsync(
                "",
                "1",
                "25",
                lang,
                "1",
                "1",
                "",
                "0",
                "1",
                "0",
                "0",
                "1",
                "0",
                "0",
                "0",
                "0",
                "0",
                "0",
                "0",
                "1",
                "",
                "",
                "",
                "",
                new List<string>(),
                activitytype,
                new List<string>(),
                myareas,
                new List<string>(),
                prefilter2,
                requestor,
                ltsmsgpswd
            );

            //INFOS für MEASURINGPOINTS ???
            var mysnowsearchrequest = GetActivityDataLCS.GetWeatherSnowSearchRequest(
                "",
                "1",
                "25",
                lang,
                "0",
                "0",
                "1",
                "1",
                "0",
                "0",
                "1",
                "",
                "1",
                "1",
                "0",
                "1",
                new List<string>(),
                myareas,
                new List<string>(),
                requestor,
                ltsmsgpswd
            );

            //Detailinfos für Measuringpoints


            try
            {
                GetActivityDataLCS myactivitysearch = new GetActivityDataLCS(ltsuser, ltspswd);

                var myactivityresponse = myactivitysearch.GetActivitySearch(myactivityrequest);
                var mysnowobservationresponse = myactivitysearch.GetWeatherSnowSearch(
                    mysnowsearchrequest
                );

                //Sowolla iatz muassi aus de 2 Responsen richtig ausserparsen
                var measuringpointlist = ParseSnowDataLive.ParseMyMeasuringPoint(
                    mysnowobservationresponse
                );

                var mysnowreport = ParseSnowDataLive.ParseMySnowReportData(
                    lang,
                    myskiarea,
                    myactivityresponse,
                    measuringpointlist
                );

                return mysnowreport;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
