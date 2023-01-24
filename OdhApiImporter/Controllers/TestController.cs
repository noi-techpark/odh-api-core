using Helper;
using Helper.Factories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using OdhNotifier;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
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
        private IOdhPushNotifier odhpushnotifier;
        private readonly MongoDBFactory MongoDBFactory;

        public TestController(IWebHostEnvironment env, ISettings settings, ILogger<JsonGeneratorController> logger, QueryFactory queryFactory, MongoDBFactory mongoDBFactory, IOdhPushNotifier odhpushnotifier)
        {
            this.env = env;
            this.settings = settings;
            this.logger = logger;
            this.QueryFactory = queryFactory;
            this.MongoDBFactory = mongoDBFactory;
            this.odhpushnotifier = odhpushnotifier;
        }

        [HttpGet, Route("Test")]
        public IActionResult Get()
        {

            return Ok("importer alive");
        }

        [HttpGet, Route("TestNotify")]
        public async Task<IActionResult> TestNotify()
        {            
            var responses = await odhpushnotifier.PushToAllRegisteredServices("2657B7CBCB85380B253D2FBE28AF100E", "ACCOMMODATION", "forced", "api");

            return Ok();
        }

        [HttpGet, Route("TestMongoDB")]
        public async Task<IActionResult> TestMongoDB()
        {            
            return Ok(MongoDBFactory.GetDocumentById<BsonDocument>("TestDB", "TestDB", "63cfa30278b2fc0eda271a28"));
        }
    }


}
