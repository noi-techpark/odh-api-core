using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.other
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class GpxController : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataReader,ActivityReader,ODHPoiReader")]
        [HttpGet, Route("v1/Activity/Gpx/{tvid}/{gpxid}")]
        public async Task<IActionResult> GetActivityGpx(string tvid, string gpxid)

        {
            HttpClient myclient = new HttpClient();

            Uri requesturi = new Uri("https://lcs.lts.it/downloads/gpx/" + tvid + "/" + gpxid);

            var result = await myclient.GetAsync(requesturi);

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataReader,ActivityReader,ODHPoiReader")]
        [HttpGet, Route("v1/Activity/GpxByUrl/{gpxurl}")]
        public async Task<IActionResult> GetActivityGpxURL(string gpxurl)
        {
            HttpClient myclient = new HttpClient();

            Uri requesturi = new Uri(gpxurl);

            var response = await myclient.GetAsync(requesturi);

            return Ok(response);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataReader,ActivityReader,ODHPoiReader")]
        [HttpGet, Route("v1/Poi/Gpx/{tvid}/{gpxid}")]
        public async Task<IActionResult> GetPoiGpx(string gpxid, string tvid)
        {
            HttpClient myclient = new HttpClient();

            Uri requesturi = new Uri("https://lcs.lts.it/downloads/gpx/" + tvid + "/" + gpxid + ".Gpx");

            var response = await myclient.GetAsync(requesturi);

            return Ok(response);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataReader,ActivityReader,ODHPoiReader")]
        [HttpGet, Route("v1/SmgPoiGpx/{gpxid}")]
        public async Task<IActionResult> GetSmgPoiGpx(string gpxid)
        {
            if (String.IsNullOrEmpty(gpxid))
                return BadRequest();

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/Gpx/" + gpxid + ".gpx", FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = gpxid + ".gpx";

            return Ok(response);
        }

    }
}
