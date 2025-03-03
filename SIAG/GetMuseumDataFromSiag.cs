// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SIAG
{
    public class GetMuseumFromSIAG
    {
        public static async Task<HttpResponseMessage> MuseumList(string serviceurl)
        {
            string requesturl = serviceurl + "getMuseums";
            using (var client = new HttpClient())
            {
                var myresponse = await client.GetAsync(requesturl);

                return myresponse;
            }
        }

        public static async Task<XDocument> GetMuseumList(string serviceurl)
        {
            var myresponse = MuseumList(serviceurl);

            var myresponsecontent = await myresponse.Result.Content.ReadAsStringAsync();

            XDocument xresponse = XDocument.Parse(myresponsecontent);

            return xresponse;
        }

        public static async Task<HttpResponseMessage> MuseumDetail(
            string serviceurl,
            string museumid
        )
        {
            try
            {
                string requesturl = serviceurl + "getMuseumDetail?param0=" + museumid;
                using (var client = new HttpClient())
                {
                    var myresponse = await client.GetAsync(requesturl);

                    return myresponse;
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(ex.Message),
                };
            }
        }

        public static async Task<XDocument?> GetMuseumDetail(string serviceurl, string museumid)
        {
            try
            {
                var myresponse = MuseumDetail(serviceurl, museumid);

                var myresponsecontent = await myresponse.Result.Content.ReadAsStringAsync();

                XDocument xresponse = XDocument.Parse(myresponsecontent);

                return xresponse;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
