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
using OdhApiCore.GenericHelpers;
using AspNetCore.CacheOutput;

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

        [InvalidateCacheOutput(nameof(STAController.GetODHActivityPoiListSTA), typeof(STAController))] // this will invalidate Get in a different controller
        [HttpGet, Route("STA/JsonPoi")]
        public async Task<IActionResult> ProducePoiSTAJson(CancellationToken cancellationToken)
        {
            await STARequestHelper.GenerateJSONODHActivityPoiForSTA(QueryFactory, settings.JsonConfig.Jsondir, settings.XmlConfig.Xmldir);

            return Ok(new
            {
                operation = "Json Generation",
                type = "ODHActivityPoi",
                message = "Generate Json ODHActivityPoi for STA succeeded",                
                success = true
            });
        }

        [InvalidateCacheOutput(nameof(STAController.GetAccommodationsSTA), typeof(STAController))] // this will invalidate Get in a different controller
        [HttpGet, Route("STA/JsonAccommodation")]
        public async Task<IActionResult> ProduceAccoSTAJson(CancellationToken cancellationToken)
        {
            await STARequestHelper.GenerateJSONAccommodationsForSTA(QueryFactory, settings.JsonConfig.Jsondir);

            return Ok(new
            {
                operation = "Json Generation",
                type = "Accommodation",
                message = "Generate Json Accommodation for STA succeeded",
                success = true
            });
        }

        [HttpGet, Route("ODH/AccommodationBooklist")]
        public async Task<IActionResult> ProduceAccoBooklistJson(CancellationToken cancellationToken)
        {
            await JsonGeneratorHelper.GenerateJSONAccommodationsForBooklist(QueryFactory, settings.JsonConfig.Jsondir, true, "AccosBookable");

            return Ok(new
            {
                operation = "Json Generation",
                type = "AccommodationBooklist",
                message = "Generate Json AccommodationBooklist succeeded",
                success = true
            });
        }

        [HttpGet, Route("ODH/AccommodationFulllist")]
        public async Task<IActionResult> ProduceAccoFulllistJson(CancellationToken cancellationToken)
        {
            await JsonGeneratorHelper.GenerateJSONAccommodationsForBooklist(QueryFactory, settings.JsonConfig.Jsondir, true, "AccosAll");

            return Ok(new
            {
                operation = "Json Generation",
                type = "AccommodationFullist",
                message = "Generate Json AccommodationFullist succeeded",
                success = true
            });
        }

        //TODO ADD the Json Generation for        
        //Locationlists

    }
}