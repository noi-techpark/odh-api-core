using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SuedtirolWein
{
    public class GetSuedtirolWeinData
    {
        public const string serviceurlcompanies = @"https://suedtirolwein.secure.consisto.net/companies.ashx";
        public const string serviceurlawards = @"https://suedtirolwein.secure.consisto.net/awards.ashx";

        private static async Task<HttpResponseMessage> RequestCompaniesAsync(string lang)
        {
            try
            {
                string requesturl = serviceurlcompanies + "?lang=" + lang;

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

        private static async Task<HttpResponseMessage> RequestAwardsAsync(string lang)
        {
            try
            {
                string requesturl = serviceurlawards + "?lang=" + lang;

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

        public static async Task<XDocument> GetSueditrolWineCompaniesAsync(string lang)
        {
            //make Request
            HttpResponseMessage response = await RequestCompaniesAsync(lang);
            //Read Content and parse to XDocument
            var responsetask = await response.Content.ReadAsStringAsync();
            XDocument myweatherresponse = XDocument.Parse(responsetask);

            return myweatherresponse;
        }

        public static async Task<XDocument> GetSueditrolWineAwardsAsync(string lang)
        {
            //make Request
            HttpResponseMessage response = await RequestAwardsAsync(lang);
            //Read Content and parse to XDocument
            var responsetask = await response.Content.ReadAsStringAsync();
            XDocument myweatherresponse = XDocument.Parse(responsetask);

            return myweatherresponse;
        }

    }
}
