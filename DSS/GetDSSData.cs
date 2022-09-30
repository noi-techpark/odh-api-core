using Newtonsoft.Json;
using System.Net;

namespace DSS
{
    public enum DSSRequestType
    {
        liftbase,
        liftstatus,
        slopebase,
        slopestatus,
        overview,
        skiresorts,
        skicircuits,
        webcams,
        sellingpoints,
        snowparks,
        alpinehuts,
        taxi,
        healthcare,
        weather
    };

    public class GetDSSData
    {
        public const string serviceurlliftbase = "liftbasis";
        public const string serviceurlliftstatus = "liftstatus";

        public const string serviceurlslopebase = "pistebasis";
        public const string serviceurlslopestatus = "pistenstatus";

        public const string serviceurloverview = "overview";
        public const string serviceurlskiresorts = "talschaften";

        public const string serviceurlskicircuits = "ski-circuits";
        public const string serviceurlwebcams = "webcams";

        public const string serviceurlsellingpoints = "selling-points";
        public const string serviceurlsnowpark = "snowpark";

        public const string serviceurlmountainhuts = "mountain-huts";
        public const string serviceurltaxi = "taxi";

        public const string serviceurlhealthcare = "healthcare";
        public const string serviceurlweather = "wetter";

        private static async Task<HttpResponseMessage> RequestDSSInfo(DSSRequestType dssRequestType,  string dssuser, string dsspswd, string serviceurl)
        {
            try
            {             
                switch(dssRequestType)
                {
                    case DSSRequestType.liftbase: 
                        serviceurl = serviceurl + serviceurlliftbase;
                        break;
                    case DSSRequestType.liftstatus:
                        serviceurl = serviceurl + serviceurlliftstatus;
                        break;
                    case DSSRequestType.slopebase:
                        serviceurl = serviceurl + serviceurlslopebase;
                        break;
                    case DSSRequestType.slopestatus:
                        serviceurl = serviceurl + serviceurlslopestatus;
                        break;
                    case DSSRequestType.overview:
                        serviceurl = serviceurl + serviceurloverview;
                        break;
                    case DSSRequestType.skiresorts:
                        serviceurl = serviceurl + serviceurlskiresorts;
                        break;
                    case DSSRequestType.skicircuits:
                        serviceurl = serviceurl + serviceurlskicircuits;
                        break;
                    case DSSRequestType.snowparks:
                        serviceurl = serviceurl + serviceurlsnowpark;
                        break;
                    case DSSRequestType.webcams:
                        serviceurl = serviceurl + serviceurlwebcams;
                        break;
                    case DSSRequestType.healthcare:
                        serviceurl = serviceurl + serviceurlhealthcare;
                        break;
                    case DSSRequestType.weather:
                        serviceurl = serviceurl + serviceurlweather;
                        break;
                    case DSSRequestType.taxi:
                        serviceurl = serviceurl + serviceurltaxi;
                        break;
                    case DSSRequestType.alpinehuts:
                        serviceurl = serviceurl + serviceurlmountainhuts;
                        break;
                    case DSSRequestType.sellingpoints:
                        serviceurl = serviceurl + serviceurlsellingpoints;
                        break;
                }

                if (String.IsNullOrEmpty(serviceurl))
                    throw new Exception("no service url set");

                string requesturl = serviceurl + "?username=" + dssuser + "&password=" + dsspswd;

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
        
        public static async Task<dynamic?> GetDSSDataAsync(DSSRequestType dssRequestType, string dssuser, string dsspswd, string serviceurl)
        {
            //Request
            HttpResponseMessage response = await RequestDSSInfo(dssRequestType, dssuser, dsspswd, serviceurl);
            //Parse JSON Response to
            var responsetask = await response.Content.ReadAsStringAsync();
            dynamic? responseobject = JsonConvert.DeserializeObject(responsetask);

            return responseobject;
        }        
    }
}