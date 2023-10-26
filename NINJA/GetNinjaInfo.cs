// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NINJA
{
    public class GetNinjaData
    {
        /// <summary>
        /// Gets the Data from Ninja Api
        /// </summary>
        /// <returns></returns>
        public static async Task<NinjaObject<NinjaEvent>> GetNinjaEvent(string serviceurl)
        {
            string eventselect = @"*/?limit=-1&offset=0&select=tmetadata&where=sactive.eq.true,tname.eq.1aJW6sEGo40hWeL_B2yK4N7CIGwRlmwVpAkwqxjF1ruA:1179155880&shownull=false&distinct=true";

            var requesturl = serviceurl + eventselect;

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(10);

                var myresponse = await client.GetAsync(requesturl);

                var myresponsejson = await myresponse.Content.ReadAsStringAsync();

                var btpresponseobject = JsonConvert.DeserializeObject<NinjaObject<NinjaEvent>>(myresponsejson);

                return btpresponseobject;
            }
        }


        /// <summary>
        /// Gets the Data from Ninja Api
        /// </summary>
        /// <returns></returns>
        public static async Task<NinjaObject<NinjaPlaceRoom>> GetNinjaPlaces(string serviceurl)
        {
            string placeselect = @"?limit=-1&offset=0&where=sactive.eq.true,sorigin.eq.1aJW6sEGo40hWeL_B2yK4N7CIGwRlmwVpAkwqxjF1ruA&shownull=false&distinct=true";

            var requesturl = serviceurl + placeselect;

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(20);

                var myresponse = await client.GetAsync(requesturl);

                var myresponsejson = await myresponse.Content.ReadAsStringAsync();

                var btpresponseobject = JsonConvert.DeserializeObject<NinjaObject<NinjaPlaceRoom>>(myresponsejson);

                return btpresponseobject;
            }
        }


    }
}
