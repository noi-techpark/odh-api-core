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

        public JsonGeneratorController(
            IWebHostEnvironment env,
            ISettings settings,
            ILogger<JsonGeneratorController> logger,
            QueryFactory queryFactory
        ) : base(env, settings, logger, queryFactory)
        {
            this.settings = settings;
        }

        #region Accommodation

        [HttpGet, Route("ODH/AccommodationBooklist")]
        public async Task<IActionResult> ProduceAccoBooklistJson(
            CancellationToken cancellationToken
        )
        {
            try
            {
                await JsonGeneratorHelper.GenerateJSONAccommodationsForBooklist(
                    QueryFactory,
                    settings.JsonConfig.Jsondir,
                    true,
                    "AccosBookable"
                );

                return Ok(
                    new
                    {
                        operation = "Json Generation",
                        type = "AccommodationBooklist",
                        message = "Generate Json AccommodationBooklist succeeded",
                        success = true
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        operation = "Json Generation",
                        type = "AccommodationBooklist",
                        message = "Generate Json AccommodationBooklist failed error:" + ex.Message,
                        success = false
                    }
                );
            }
        }

        [HttpGet, Route("ODH/AccommodationFulllist")]
        public async Task<IActionResult> ProduceAccoFulllistJson(
            CancellationToken cancellationToken
        )
        {
            try
            {
                await JsonGeneratorHelper.GenerateJSONAccommodationsForBooklist(
                    QueryFactory,
                    settings.JsonConfig.Jsondir,
                    true,
                    "AccosAll"
                );

                return Ok(
                    new
                    {
                        operation = "Json Generation",
                        type = "AccommodationFullist",
                        message = "Generate Json AccommodationFullist succeeded",
                        success = true
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        operation = "Json Generation",
                        type = "AccommodationFullist",
                        message = "Generate Json AccommodationBooklist failed error: " + ex.Message,
                        success = false
                    }
                );
            }
        }

        #endregion

        #region Tags

        [HttpGet, Route("ODH/Taglist")]
        public async Task<IActionResult> ProduceTagJson(CancellationToken cancellationToken)
        {
            try
            {
                await JsonGeneratorHelper.GenerateJSONTaglist(
                    QueryFactory,
                    settings.JsonConfig.Jsondir,
                    "GenericTags"
                );

                return Ok(
                    new
                    {
                        operation = "Json Generation",
                        type = "Taglist",
                        message = "Generate Json Taglist succeeded",
                        success = true
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        operation = "Json Generation",
                        type = "Taglist",
                        message = "Generate Json Taglist failed error: " + ex.Message,
                        success = false
                    }
                );
            }
        }

        #endregion

        #region STA

        [InvalidateCacheOutput(
            typeof(OdhApiCore.Controllers.sta.STAController),
            nameof(OdhApiCore.Controllers.sta.STAController.GetODHActivityPoiListSTA)
        )] // this will invalidate Get in a different controller
        [HttpGet, Route("STA/JsonPoi")]
        public async Task<IActionResult> ProducePoiSTAJson(CancellationToken cancellationToken)
        {
            try
            {
                await STARequestHelper.GenerateJSONODHActivityPoiForSTA(
                    QueryFactory,
                    settings.JsonConfig.Jsondir,
                    settings.XmlConfig.Xmldir
                );

                return Ok(
                    new
                    {
                        operation = "Json Generation",
                        type = "ODHActivityPoi",
                        message = "Generate Json ODHActivityPoi for STA succeeded",
                        success = true
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        operation = "Json Generation",
                        type = "ODHActivityPoi",
                        message = "Generate Json ODHActivityPoi for STA failed error:" + ex.Message,
                        success = false
                    }
                );
            }
        }

        [InvalidateCacheOutput(
            typeof(OdhApiCore.Controllers.sta.STAController),
            nameof(OdhApiCore.Controllers.sta.STAController.GetAccommodationsSTA)
        )] // this will invalidate Get in a different controller
        [HttpGet, Route("STA/JsonAccommodation")]
        public async Task<IActionResult> ProduceAccoSTAJson(CancellationToken cancellationToken)
        {
            try
            {
                await STARequestHelper.GenerateJSONAccommodationsForSTA(
                    QueryFactory,
                    settings.JsonConfig.Jsondir
                );

                return Ok(
                    new
                    {
                        operation = "Json Generation",
                        type = "Accommodation",
                        message = "Generate Json Accommodation for STA succeeded",
                        success = true
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        operation = "Json Generation",
                        type = "Accommodation",
                        message = "Generate Json Accommodation for STA failed error:" + ex.Message,
                        success = false
                    }
                );
            }
        }

        #endregion

        //TODO ADD the Json Generation for
        //Locationlists
    }
}
