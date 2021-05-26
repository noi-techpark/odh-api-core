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
using AspNetCore.CacheOutput;
using System.IO;
using OdhApiCore.GenericHelpers;

namespace OdhApiCore.Controllers.api
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class STAController : OdhController
    {
        private readonly IWebHostEnvironment env;
        private readonly ISettings settings;

        public STAController(IWebHostEnvironment env, ISettings settings, ILogger<STAController> logger, QueryFactory queryFactory)
         : base(env, settings, logger, queryFactory)
        {
            this.env = env;
            this.settings = settings;
        }

        #region GETTER

        [CacheOutput(ClientTimeSpan = 14400, ServerTimeSpan = 14400)]
        [HttpGet, Route("STA/ODHActivityPoi")]
        public async Task<IActionResult> GetODHActivityPoiListSTA(
            string language,
            CancellationToken cancellationToken)
        {
            try
            {
                string fileName = Path.Combine(settings.JsonConfig.Jsondir, $"STAOdhActivitiesPois_{language}.json");

                using (StreamReader r = new StreamReader(fileName))
                {
                    string json = await r.ReadToEndAsync();

                    return new OkObjectResult(new JsonRaw(json));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [CacheOutput(ClientTimeSpan = 14400, ServerTimeSpan = 14400)]
        [HttpGet, Route("STA/Accommodation")]
        public async Task<IActionResult> GetAccommodationsSTA(
           string language,
           CancellationToken cancellationToken)
        {
            try
            {
                //DOCKER ERROR Could not find a part of the path '/app/.\wwwroot\json\/STAAccommodations_de.json'.
                string fileName = Path.Combine(settings.JsonConfig.Jsondir, $"STAAccommodations_{language}.json");

                using (StreamReader r = new StreamReader(fileName))
                {
                    string json = await r.ReadToEndAsync();

                    return new OkObjectResult(new JsonRaw(json));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion       

        #region GENERATEJSON

        [InvalidateCacheOutput(nameof(GetODHActivityPoiListSTA), typeof(STAController))] // this will invalidate Get in a different controller
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

        [InvalidateCacheOutput(nameof(GetAccommodationsSTA), typeof(STAController))] // this will invalidate Get in a different controller
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

        #endregion

        #region IMPORTER

        [HttpGet, Route("STA/ImportVendingPoints")]
        public async Task<IActionResult> ImportVendingPointsFromSTA(CancellationToken cancellationToken)
        {
            try
            {
                return await ImportVendingPointsFromCSV(null);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route("STA/ImportVendingPoints")]
        public async Task<IActionResult> SendVendingPointsFromSTA(CancellationToken cancellationToken)
        {
            try
            {
                return await PostVendingPointsFromSTA(Request);
            }
            catch (Exception ex)
            {
                return BadRequest(new GenericResult() { Message = ex.Message });
            }
        }

        #endregion

        #region HELPERS

        private static async Task<string> ReadStringDataManual(HttpRequest request)
        {
            using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private async Task<IActionResult> PostVendingPointsFromSTA(HttpRequest request)
        {
            string jsonContent = await ReadStringDataManual(request);

            if (!string.IsNullOrEmpty(jsonContent))
            {
                return await ImportVendingPointsFromCSV(jsonContent);
            }
            else
                throw new Exception("no Content");
        }

        private async Task<IActionResult> ImportVendingPointsFromCSV(string csvcontent)
        {
            var vendingpoints = await STA.GetDataFromSTA.ImportCSVFromSTA(csvcontent);

            if (vendingpoints.Success)
            {
                //Import Each STA Vendingpoi to ODH
                foreach (var vendingpoint in vendingpoints.records)
                {
                    //Parse to ODHActivityPoi
                    var odhactivitypoi = STA.ParseSTAPois.ParseSTAVendingPointToODHActivityPoi(vendingpoint);

                    //TODO SET ATTRIBUTES
                    //MetaData
                    odhactivitypoi._Meta = MetadataHelper.GetMetadataobject<SmgPoiLinked>(odhactivitypoi, MetadataHelper.GetMetadataforOdhActivityPoi); //GetMetadata(data.Id, "odhactivitypoi", sourcemeta, data.LastChange);
                                                                                                                                                        //License
                    odhactivitypoi.LicenseInfo = LicenseHelper.GetLicenseforOdhActivityPoi(odhactivitypoi);

                    //Save to PG
                    //Check if data exists
                    await UpsertData<ODHActivityPoi>(odhactivitypoi, "smgpois");
                }

                return new OkObjectResult("import from posted csv succeeded");
            }
            else if (vendingpoints.Error)
                throw new Exception(vendingpoints.ErrorMessage);
            else
                throw new Exception("no data to import");
        }

        #endregion
    }
}