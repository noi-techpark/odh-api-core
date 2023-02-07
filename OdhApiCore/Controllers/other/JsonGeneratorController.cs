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

namespace OdhApiCore.Controllers.other
{
    [ApiExplorerSettings(IgnoreApi = true)]    
    [ApiController]
    public class JsonGeneratorController : OdhController
    {        
        private readonly ISettings settings;

        public JsonGeneratorController(IWebHostEnvironment env, ISettings settings, ILogger<JsonGeneratorController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {
            this.settings = settings;
        }

        #region Accommodation

        [HttpGet, Route("ODH/AccommodationBooklist")]
        public async Task<IActionResult> ProduceAccoBooklistJson(CancellationToken cancellationToken)
        {
            try
            {
                await JsonGeneratorHelper.GenerateJSONAccommodationsForBooklist(QueryFactory, settings.JsonConfig.Jsondir, true, "AccosBookable");

                var result = GenericResultsHelper.GetSuccessJsonGenerateResult("Json Generation", "AccommodationBooklist", "Generate Json AccommodationBooklist succeeded", true);

                return Ok(result);
            }
            catch(Exception ex)
            {
                var result = GenericResultsHelper.GetErrorJsonGenerateResult("Json Generation", "AccommodationBooklist", "Generate Json AccommodationBooklist succeeded", ex, true);

                return BadRequest(result);
            }
        }

        [HttpGet, Route("ODH/AccommodationFulllist")]
        public async Task<IActionResult> ProduceAccoFulllistJson(CancellationToken cancellationToken)
        {
            try
            {
                await JsonGeneratorHelper.GenerateJSONAccommodationsForBooklist(QueryFactory, settings.JsonConfig.Jsondir, true, "AccosAll");
                
                var result = GenericResultsHelper.GetSuccessJsonGenerateResult("Json Generation", "AccommodationFullist", "Generate Json AccommodationFullist succeeded", true);

                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = GenericResultsHelper.GetErrorJsonGenerateResult("Json Generation", "AccommodationFullist", "Generate Json AccommodationBooklist failed", ex, true);

                return BadRequest(result);
            }
        }

        #endregion

        #region Tags

        [HttpGet, Route("ODH/Taglist")]
        public async Task<IActionResult> ProduceTagJson(CancellationToken cancellationToken)
        {
            try
            {
                await JsonGeneratorHelper.GenerateJSONTaglist(QueryFactory, settings.JsonConfig.Jsondir, "GenericTags");

                var result = GenericResultsHelper.GetSuccessJsonGenerateResult("Json Generation", "Taglist", "Generate Json Taglist succeeded", true);

                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = GenericResultsHelper.GetErrorJsonGenerateResult("Json Generation", "Taglist", "Generate Json Taglist failed", ex, true);

                return BadRequest(result);
            }
        }

        #endregion

        #region STA

        [InvalidateCacheOutput(typeof(OdhApiCore.Controllers.sta.STAController), nameof(OdhApiCore.Controllers.sta.STAController.GetODHActivityPoiListSTA))] // this will invalidate Get in a different controller
        [HttpGet, Route("STA/JsonPoi")]
        public async Task<IActionResult> ProducePoiSTAJson(CancellationToken cancellationToken)
        {
            try
            {
                await STARequestHelper.GenerateJSONODHActivityPoiForSTA(QueryFactory, settings.JsonConfig.Jsondir, settings.XmlConfig.Xmldir);

                var result = GenericResultsHelper.GetSuccessJsonGenerateResult("Json Generation", "ODHActivityPoiSTA", "Generate Json ODHActivityPoi for STA succeeded", true);

                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = GenericResultsHelper.GetErrorJsonGenerateResult("Json Generation", "ODHActivityPoiSTA", "Generate Json ODHActivityPoi for STA failed", ex, true);

                return BadRequest(result);
            }
        }

        [InvalidateCacheOutput(typeof(OdhApiCore.Controllers.sta.STAController), nameof(OdhApiCore.Controllers.sta.STAController.GetAccommodationsSTA))] // this will invalidate Get in a different controller
        [HttpGet, Route("STA/JsonAccommodation")]
        public async Task<IActionResult> ProduceAccoSTAJson(CancellationToken cancellationToken)
        {
            try
            {
                await STARequestHelper.GenerateJSONAccommodationsForSTA(QueryFactory, settings.JsonConfig.Jsondir);

                var result = GenericResultsHelper.GetSuccessJsonGenerateResult("Json Generation", "AccommodationSTA", "Generate Json Accommodation for STA succeeded", true);

                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = GenericResultsHelper.GetErrorJsonGenerateResult("Json Generation", "AccommodationSTA", "Generate Json Accommodation for STA failed", ex, true);

                return BadRequest(result);
            }
        }

        #endregion
        
    }
}