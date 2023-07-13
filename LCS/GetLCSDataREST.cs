// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace LCS
{
    public class GetLCSDataREST
    {
        public static async Task<XDocument> GetDataAsXML(string user, string pswd, string method, XElement request)
        {
            string requesturl = @"https://lcs.lts.it/api/data.svc/xml/" + method;

            HttpClient myclient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });

            byte[] mypass = Encoding.UTF8.GetBytes(user + ":" + pswd);
            string encoding = Convert.ToBase64String(mypass);

            myclient.DefaultRequestHeaders.Add("Authorization", "Basic " + encoding);
            myclient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate");

            var myresponse = await myclient.PostAsync(requesturl, new StringContent(request.ToString(), Encoding.UTF8, "text/xml"));

            var responsecontent = await myresponse.Content.ReadAsStringAsync();

            return XDocument.Parse(responsecontent);
        }

        public static async Task<JObject> GetDataAsJSON(string user, string pswd, string method, XElement request)
        {
            string requesturl = @"https://lcs.lts.it/api/data.svc/xml/" + method;

            HttpClient myclient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });

            byte[] mypass = Encoding.UTF8.GetBytes(user + ":" + pswd);
            string encoding = Convert.ToBase64String(mypass);

            myclient.DefaultRequestHeaders.Add("Authorization", "Basic " + encoding);
            myclient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate");

            var myresponse = await myclient.PostAsync(requesturl, new StringContent(request.ToString(), Encoding.UTF8, "application/json"));

            var responsecontent = await myresponse.Content.ReadAsStringAsync();

            return JObject.Parse(responsecontent);
        }
    }
}
