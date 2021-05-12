using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RAVEN
{
    public class GetDataFromRaven
    {        
        public static async Task<T> GetRavenData<T>(string type, string id, string odhravenserviceurl, string username, string password)
        {
            try
            {
                var requesturl = odhravenserviceurl + type + "/" + id;

                CredentialCache wrCache = new CredentialCache();
                wrCache.Add(new Uri(requesturl), "Basic", new NetworkCredential(username, password));

                using (var handler = new HttpClientHandler { Credentials = wrCache })
                {
                    using (var client = new HttpClient())
                    {

                        client.Timeout = TimeSpan.FromSeconds(20);

                        var myresponse = await client.GetAsync(requesturl);

                        var myresponsejson = await myresponse.Content.ReadAsStringAsync();

                        var responseobject = JsonConvert.DeserializeObject<T>(myresponsejson);

                        return responseobject;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }                        
        }
    }
}
