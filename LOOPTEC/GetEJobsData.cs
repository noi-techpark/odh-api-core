// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Newtonsoft.Json;

namespace LOOPTEC
{
    public class GetEJobsData
    {
        private static async Task<HttpResponseMessage> GetEjobsDataFromService(
            string user,
            string pass,
            string serviceurl
        )
        {
            using (var client = new HttpClient())
            {
                var myresponse = await client.GetAsync(serviceurl);

                return myresponse;
            }
        }

        public static async Task<dynamic?> GetEjobsDataAsync(
            string user,
            string pass,
            string serviceurl
        )
        {
            //Request
            HttpResponseMessage response = await GetEjobsDataFromService(user, pass, serviceurl);
            //Parse JSON Response to
            var responsetask = await response.Content.ReadAsStringAsync();
            dynamic? responseobject = JsonConvert.DeserializeObject(responsetask);

            return responseobject;
        }
    }
}
