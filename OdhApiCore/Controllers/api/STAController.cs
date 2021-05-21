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

namespace OdhApiCore.Controllers.api
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class STAController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly ISettings settings;

        public STAController(IWebHostEnvironment env, ISettings settings)           
        {
            this.env = env;
            this.settings = settings;
        }

        [CacheOutput(ClientTimeSpan = 14400, ServerTimeSpan = 14400)]
        [HttpGet, Route("v1/STA/ODHActivityPoi")]
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
        [HttpGet, Route("v1/STA/Accommodation")]
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

        [HttpGet, Route("v1/STA/ImportVendingPoints")]
        public async Task<IActionResult> ImportVendingPointsFromSTA(           
           CancellationToken cancellationToken)
        {
            try
            {
                await STA.GetDataFromSTA.ImportCSVFromSTA();
                
                return new OkObjectResult("test succeeded");                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route("v1/STA/ImportVendingPoints")]
        public async Task<IActionResult> SendVendingPointsFromSTA(
           CancellationToken cancellationToken)
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
                await STA.GetDataFromSTA.ImportCSVFromSTA();

                return new OkObjectResult("import from posted csv succeeded");
            }
            else
                throw new Exception("no Content");
        }


    }
}