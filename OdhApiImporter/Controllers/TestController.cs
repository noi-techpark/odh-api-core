// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

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
        private readonly ILogger<TestController> logger;
        private readonly IWebHostEnvironment env;
        private IOdhPushNotifier OdhPushnotifier;
        private readonly IMongoDBFactory MongoDBFactory;

        public TestController(IWebHostEnvironment env, ISettings settings, ILogger<TestController> logger, QueryFactory queryFactory, IMongoDBFactory mongoDBFactory, IOdhPushNotifier odhpushnotifier)
        {
            this.env = env;
            this.settings = settings;
            this.logger = logger;
            this.QueryFactory = queryFactory;
            this.MongoDBFactory = mongoDBFactory;
            this.OdhPushnotifier = odhpushnotifier;
        }

        [HttpGet, Route("Test")]
        public IActionResult Get()
        {
            return Ok("importer alive");
        }

       

        //[HttpGet, Route("TestMongoDB")]
        //public async Task<IActionResult> TestMongoDB()
        //{
        //    var test = MongoDBFactory.GetDocumentById<BsonDocument>("TestDB", "TestDB", "63cfa30278b2fc0eda271a28");
            
        //    return Ok(test.ToString());
        //}
    }


}
