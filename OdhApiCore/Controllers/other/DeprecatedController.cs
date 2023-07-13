// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

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
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class DeprecatedController : ControllerBase
    {
        //TODO Get openapi file and parse trough an render to output
        [HttpGet, Route("v1/Deprecated")]
        public async Task<IActionResult> Deprecated()
        {
            var requesturl = string.Format("{0}://{1}{2}{3}", HttpContext.Request.Scheme, HttpContext.Request.Host, HttpContext.Request.Path, "swagger/v1/swagger.json");

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(requesturl);
                var responsecontent = await response.Content.ReadAsStringAsync();                

                JObject? obj = JsonConvert.DeserializeObject<JObject>(responsecontent);

                //obj["dialog"]["prompt"]

                return Ok(obj);
            }                              
        }
    }

    public class DeprecatedInfo
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
    }
}
