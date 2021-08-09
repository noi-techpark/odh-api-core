using AspNetCore.Proxy;
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
        public Task GetActivityGpx(string tvid, string gpxid)
        {
            var url = "https://lcs.lts.it/downloads/gpx/" + tvid + "/" + gpxid;

            return this.HttpProxyAsync(url);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataReader,ActivityReader,ODHPoiReader")]
        [HttpGet, Route("v1/Activity/GpxByUrl/{gpxurl}")]
        public Task GetActivityGpxURL(string gpxurl)
        {            
            return this.HttpProxyAsync(gpxurl);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataReader,ActivityReader,ODHPoiReader")]
        [HttpGet, Route("v1/Poi/Gpx/{tvid}/{gpxid}")]
        public Task GetPoiGpx(string gpxid, string tvid)
        {
            var url = "https://lcs.lts.it/downloads/gpx/" + tvid + "/" + gpxid + ".Gpx";

            return this.HttpProxyAsync(url);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataReader,ActivityReader,ODHPoiReader")]
        [HttpGet, Route("v1/SmgPoiGpx/{gpxid}")]
        public IActionResult GetSmgPoiGpx(string gpxid)
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
