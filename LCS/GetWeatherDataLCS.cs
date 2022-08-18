using System;
using System.Collections.Generic;
using System.Text;
using ServiceReferenceLCS;

namespace LCS
{
    public class GetWeatherDataLCS
    {
        //Methode wos Live die Wetterdaten holt zu an Messpunkt!
        public static ServiceReferenceLCS.WeatherSnowObservationDetailRS GetMeasuringpointData(
            string lang,
            List<string> measuringpointList,
            string ltsuser,
            string ltspswd,
            string ltsmsgpswd
        )
        {
            List<string> areas = new List<string>();

            var myweathersnowrequest = GetActivityDataLCS.GetWeatherSnowDetailRequest(
                "",
                "1",
                "25",
                lang,
                "0",
                "0",
                "1",
                "1",
                "0",
                "1",
                "1",
                "1",
                "1",
                "1",
                "",
                "",
                "",
                measuringpointList,
                "SMG",
                ltsmsgpswd
            );

            GetActivityDataLCS myactivitysearch = new GetActivityDataLCS(ltsuser, ltspswd);

            var myweatherresponse = myactivitysearch.GetWeatherSnowDetail(myweathersnowrequest);

            return myweatherresponse;
        }

        //Methode wos Live die Wetterdaten holt zu an Messpunkt!
        public static ServiceReferenceLCS.WeatherSnowObservationDetailRS GetMeasuringpointDataReduced(
            string lang,
            List<string> measuringpointList,
            string ltsuser,
            string ltspswd,
            string ltsmsgpswd
        )
        {
            List<string> areas = new List<string>();

            var myweathersnowrequest = GetActivityDataLCS.GetWeatherSnowDetailRequest(
                "",
                "1",
                "25",
                lang,
                "0",
                "0",
                "0",
                "1",
                "0",
                "0",
                "1",
                "1",
                "0",
                "1",
                "",
                "",
                "",
                measuringpointList,
                "SMG",
                ltsmsgpswd
            );

            GetActivityDataLCS myactivitysearch = new GetActivityDataLCS(ltsuser, ltspswd);

            var myweatherresponse = myactivitysearch.GetWeatherSnowDetail(myweathersnowrequest);

            return myweatherresponse;
        }

        //Muass erst schaugn wia des am besten zu mochn isch
        //Schneabericht muass a doher
    }
}
