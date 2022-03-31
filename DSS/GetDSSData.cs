using Newtonsoft.Json;
using System.Net;

namespace DSS
{
    public enum DSSRequestType
    {
        liftbase,
        liftstatus,
        slopebase,
        slopestatus
    };

    public class GetDSSData
    {
        public const string serviceurlliftbase = "liftbasis";
        public const string serviceurlliftstatus = "liftstatus";

        public const string serviceurlslopebase = "pistebasis";
        public const string serviceurlslopestatus = "pistenstatus";
                
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
        
        public static async Task<dynamic> GetDSSDataAsync(DSSRequestType dssRequestType, string dssuser, string dsspswd, string serviceurl)
        {
            //Request
            HttpResponseMessage response = await RequestDSSInfo(dssRequestType, dssuser, dsspswd, serviceurl);
            //Parse JSON Response to
            var responsetask = await response.Content.ReadAsStringAsync();
            dynamic responseobject = JsonConvert.DeserializeObject(responsetask);

            return responseobject;
        }        
    }
}