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
        public const string serviceurlliftbase = @"http://dss.dev.tinext.net/.rest/json-export/export/liftbasis";
        public const string serviceurlliftstatus = @"http://dss.dev.tinext.net/.rest/json-export/export/liftstatus";

        public const string serviceurlslopebase = @"http://dss.dev.tinext.net/.rest/json-export/export/pistebasis";
        public const string serviceurlslopestatus = @"http://dss.dev.tinext.net/.rest/json-export/export/pistenstatus";
        


        public const string serviceurlawards = @"https://suedtirolwein.secure.consisto.net/awards.ashx";

        private static async Task<HttpResponseMessage> RequestDSSInfo(DSSRequestType dssRequestType,  string dssuser, string dsspswd)
        {
            try
            {
                var serviceurl = "";

                switch(dssRequestType)
                {
                    case DSSRequestType.liftbase: 
                        serviceurl = serviceurlliftbase;
                        break;
                    case DSSRequestType.liftstatus:
                        serviceurl = serviceurlliftstatus;
                        break;
                    case DSSRequestType.slopebase:
                        serviceurl = serviceurlslopebase;
                        break;
                    case DSSRequestType.slopestatus:
                        serviceurl = serviceurlslopestatus;
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
        
        public static async Task<dynamic> GetDSSDataAsync(DSSRequestType dssRequestType, string dssuser, string dsspswd)
        {
            //Request
            HttpResponseMessage response = await RequestDSSInfo(dssRequestType, dssuser, dsspswd);
            //Parse JSON Response to
            var responsetask = await response.Content.ReadAsStringAsync();
            dynamic responseobject = JsonConvert.DeserializeObject(responsetask);

            return responseobject;
        }        
    }
}