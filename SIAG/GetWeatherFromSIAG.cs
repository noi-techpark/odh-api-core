// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SIAG
{
    public class GetWeatherFromSIAG
    {
        //old deprecated endpoints
        //public const string serviceurlsiag = @"https://wetter.ws.siag.it/Weather_V1.svc/web/getLastProvBulletin";
        //public const string serviceurlbezirksiag = @"https://wetter.ws.siag.it/Agriculture_V1.svc/web/getLastBulletin";

        public const string serviceurlsiag = @"https://weather.services.siag.it/api/v2/bulletinHD";
        public const string serviceurlbezirksiag = @"https://weather.services.siag.it/api/v2/district/";

        public const string serviceurlrealtime = @"http://weather.services.siag.it/api/v2/station";

        public const string serviceurl = @"http://daten.buergernetz.bz.it/services/weather/bulletin";
        public const string serviceurlbezirk = @"http://daten.buergernetz.bz.it/services/weather/district/";

        public static async Task<HttpResponseMessage> RequestAsync(string lang, string siaguser, string siagpswd, string source, bool usejson = false, string? weatherid = null)
        {
            try
            {
                string format = "xml";
                if (usejson)
                {
                    format = "json";
                }

                string requesturl = serviceurl + "?lang=" + lang + "&format=" + format;

                if (!String.IsNullOrEmpty(weatherid))
                    requesturl = serviceurl + "/" + weatherid + "?lang=" + lang + "&format=" + format;

                if (source == "siag")
                {
                    requesturl = serviceurlsiag + "?lang=" + lang + "&format=" + format;

                    if (!String.IsNullOrEmpty(weatherid))
                        requesturl = serviceurlsiag + "/" + weatherid + "?lang=" + lang + "&format=" + format;
                }

                using (var client = new HttpClient())
                {
                    var myresponse = await client.GetAsync(requesturl);

                    return myresponse;
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(ex.Message) };
            }

        }

        public static async Task<HttpResponseMessage> RequestBezirksWeatherAsync(string lang, string distid, string siaguser, string siagpswd, string source, bool usejson = false)
        {
            try
            {
                string format = "xml";
                if (usejson)
                {
                    format = "json";
                }

                string requesturl = serviceurlbezirk + distid + "/bulletin?lang=" + lang + "&format=" + format;

                if (source == "siag")
                    requesturl = serviceurlbezirksiag + distid + "/bulletin?lang=" + lang + "&format=" + format;

                //Hack adding nocache parameter because of server side caching problem with 2 domains from siag   
                requesturl = requesturl + "&nocache=" + source;

                using (var client = new HttpClient())
                {
                    var myresponse = await client.GetAsync(requesturl);

                    return myresponse;
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(ex.Message) };
            }

        }

        public static async Task<HttpResponseMessage> RequestRealtimeWeatherAsync(string lang)
        {
            try
            {
                string requesturl = serviceurlrealtime + "?lang=" + lang;

                using (var handler = new HttpClientHandler { })
                {
                    using (var client = new HttpClient(handler))
                    {
                        var myresponse = await client.GetAsync(requesturl);

                        return myresponse;
                    }
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(ex.Message) };
            }
        }

    }
}
