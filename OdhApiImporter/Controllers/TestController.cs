using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OdhNotifier;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OdhApiImporter.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class TestController : Controller
    {
        private readonly ISettings settings;
        private readonly QueryFactory QueryFactory;
        private readonly ILogger<JsonGeneratorController> logger;
        private readonly IWebHostEnvironment env;

        public TestController(IWebHostEnvironment env, ISettings settings, ILogger<JsonGeneratorController> logger, QueryFactory queryFactory)
        {
            this.env = env;
            this.settings = settings;
            this.logger = logger;
            this.QueryFactory = queryFactory;
        }

        [HttpGet, Route("Test")]
        public IActionResult Get()
        {

            return Ok("importer alive");
        }

        [HttpGet, Route("TestNotify")]
        public async Task<HttpResponseMessage> TestNotify()
        {
            var marketplaceconfig = settings.NotifierConfig.Where(x => x.ServiceName == "Marketplace").FirstOrDefault();

            MarketplacePushNotifierMeta mymeta = new MarketplacePushNotifierMeta()
            {
                Id = "2657B7CBCB85380B253D2FBE28AF100E",
                Url = marketplaceconfig.Url,
                Origin = "api",
                UdateMode = "forced",
                Headers = new Dictionary<string, string>() {
                    { "client_id", marketplaceconfig.User },
                    { "client_secret", marketplaceconfig.Password }
                },
                Type = "ACCOMMODATION"
            };

            return await OdhPushNotifier.SendNotify(mymeta);
        }
    }
}
