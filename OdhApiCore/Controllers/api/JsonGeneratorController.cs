﻿using DataModel;
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
using OdhApiCore.GenericHelpers;

namespace OdhApiCore.Controllers.api
{
    [ApiExplorerSettings(IgnoreApi = true)]    
    [ApiController]
    public class JsonGeneratorController : OdhController
    {        
        private readonly ISettings settings;

        public JsonGeneratorController(IWebHostEnvironment env, ISettings settings, ILogger<AlpineBitsController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {
            this.settings = settings;
        }

        [HttpGet, Route("STA/JsonPoi")]
        public async Task<IActionResult> ProducePoiSTAJson(CancellationToken cancellationToken)
        {
            await STARequestHelper.GenerateJSONODHActivityPoiForSTA(QueryFactory, settings.JsonConfig.Jsondir, settings.XmlConfig.Xmldir);

            return Ok("json odhactivitypoi generated");
        }

        [HttpGet, Route("STA/JsonAccommodation")]
        public async Task<IActionResult> ProduceAccoSTAJson(CancellationToken cancellationToken)
        {
            await STARequestHelper.GenerateJSONAccommodationsForSTA(QueryFactory, settings.JsonConfig.Jsondir);

            return Ok("json accommodations generated");
        }

        //TODO ADD the Json Generation for
        //Accobooklist
        //Locationlists

    }
}