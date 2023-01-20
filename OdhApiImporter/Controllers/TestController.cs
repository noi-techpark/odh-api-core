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

        [HttpGet, Route("TestNotifyMP")]
        public async Task<HttpResponseMessage> TestNotify()
        {
            var marketplaceconfig = settings.NotifierConfig.Where(x => x.ServiceName == "Marketplace").FirstOrDefault();

            NotifyMetaGenerated meta = new NotifyMetaGenerated(marketplaceconfig, "2657B7CBCB85380B253D2FBE28AF100E", "ACCOMMODATION", "forced", "api");
            
            return await OdhPushNotifier.SendNotify(meta);
        }

        [HttpGet, Route("TestNotifySinfo")]
        public async Task<HttpResponseMessage> TestNotifySinfo()
        {
            var sinfoconfig = settings.NotifierConfig.Where(x => x.ServiceName == "Sinfo").FirstOrDefault();

            NotifyMetaGenerated meta = new NotifyMetaGenerated(sinfoconfig, "2657B7CBCB85380B253D2FBE28AF100E", "accommodation", "forced", "api");
            
            return await OdhPushNotifier.SendNotify(meta);
        }
    }
}
