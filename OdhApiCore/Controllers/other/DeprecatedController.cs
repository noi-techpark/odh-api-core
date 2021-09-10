using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.other
{
    /// <summary>
    /// Returns all info about Deprecated Methods, fields etc...
    /// </summary>
    [Route("v1/[controller]")]
    [ApiController]
    public class DeprecatedController : ControllerBase
    {
        //TODO Get openapi file and parse trough an render to output

        public async Task<IActionResult> Deprecated()
        {
            var requesturl = string.Format("{0}://{1}{2}{3}", HttpContext.Request.Scheme, HttpContext.Request.Host, HttpContext.Request.Path, "swagger/v1/swagger.json");

            using (var client = new HttpClient())
            {
                var myresponse = await client.GetAsync(requesturl);

                JObject obj = JsonConvert.DeserializeObject(result);

                //obj["dialog"]["prompt"]

                return Ok(obj);
            }                              
        }
    }

    public class DeprecatedInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }
}
