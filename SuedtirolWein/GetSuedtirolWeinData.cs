// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SuedtirolWein
{
    public class GetSuedtirolWeinData
    {
        private static async Task<HttpResponseMessage> RequestCompaniesAsync(string serviceurl, string lang)
        {
            try
            {
                string requesturl = serviceurl + "companies.ashx?lang=" + lang;

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

        private static async Task<HttpResponseMessage> RequestAwardsAsync(string serviceurl, string lang)
        {
            try
            {
                string requesturl = serviceurl + "awards.ashx?lang=" + lang;

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

        public static async Task<XDocument> GetSueditrolWineCompaniesAsync(string serviceurl, string lang)
        {
            //make Request
            HttpResponseMessage response = await RequestCompaniesAsync(serviceurl, lang);
            //Read Content and parse to XDocument
            var responsetask = await response.Content.ReadAsStringAsync();
            XDocument myweatherresponse = XDocument.Parse(responsetask);

            return myweatherresponse;
        }

        public static async Task<XDocument> GetSueditrolWineAwardsAsync(string serviceurl, string lang)
        {
            //make Request
            HttpResponseMessage response = await RequestAwardsAsync(serviceurl, lang);
            //Read Content and parse to XDocument
            var responsetask = await response.Content.ReadAsStringAsync();
            XDocument myweatherresponse = XDocument.Parse(responsetask);

            return myweatherresponse;
        }

    }
}
