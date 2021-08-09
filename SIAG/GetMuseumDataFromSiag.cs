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
        public const string serviceurl = @"https://musport.prov.bz.it/musport/services/MuseumsService/";

        public static async Task<HttpResponseMessage> MuseumList()
        {

            string requesturl = serviceurl + "getMuseums";
            using (var client = new HttpClient())
            {
                var myresponse = await client.GetAsync(requesturl);

                return myresponse;
            }

        }

        public static async Task<XDocument> GetMuseumList()
        {

            var myresponse = MuseumList();

            var myresponsecontent = await myresponse.Result.Content.ReadAsStringAsync();

            XDocument xresponse = XDocument.Parse(myresponsecontent);

            return xresponse;

        }

        public static async Task<HttpResponseMessage> MuseumDetail(string museumid)
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
                return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(ex.Message) };
            }
        }

        public static async Task<XDocument> GetMuseumDetail(string museumid)
        {
            try
            {
                var myresponse = MuseumDetail(museumid);

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
