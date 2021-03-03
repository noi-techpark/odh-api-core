using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using OdhApiCore.Filters;

namespace OdhApiCore.Controllers.api
{
    [ApiExplorerSettings(IgnoreApi = true)]    
    [ApiController]
    public class JsonGeneratorController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly ISettings settings;

        public JsonGeneratorController(IWebHostEnvironment env, ISettings settings)           
        {
            this.env = env;
            this.settings = settings;
        }

        [HttpGet, Route("STA/JsonPoi")]
        public IActionResult ProducePoiSTAJson(CancellationToken cancellationToken)
        {
            //TODO
            
            return Ok("json generated");
        }

        [HttpGet, Route("STA/JsonAccommodation")]
        public IActionResult ProduceAccoSTAJson(CancellationToken cancellationToken)
        {
            //TODO

            return Ok("json generated");
        }
    }
}