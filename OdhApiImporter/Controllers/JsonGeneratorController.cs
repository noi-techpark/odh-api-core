using Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiImporter.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class JsonGeneratorController : Controller
    {
        private readonly ISettings settings;
        private readonly QueryFactory QueryFactory;
        private readonly ILogger<JsonGeneratorController> logger;
        private readonly IWebHostEnvironment env;

        public JsonGeneratorController(IWebHostEnvironment env, ISettings settings, ILogger<JsonGeneratorController> logger, QueryFactory queryFactory)
        {
            this.env = env;
            this.settings = settings;
            this.logger = logger;
            this.QueryFactory = queryFactory;
        }

        #region Tags

        [HttpGet, Route("ODH/Taglist")]
        public async Task<IActionResult> ProduceTagJson(CancellationToken cancellationToken)
        {
            try
            {
                await JsonGeneratorHelper.GenerateJSONTaglist(QueryFactory, settings.JsonConfig.Jsondir, "GenericTags");

                return Ok(new
                {
                    operation = "Json Generation",
                    type = "Taglist",
                    message = "Generate Json Taglist succeeded",
                    success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    operation = "Json Generation",
                    type = "Taglist",
                    message = "Generate Json Taglist failed error: " + ex.Message,
                    success = false
                });
            }
        }

        #endregion
    }
}
