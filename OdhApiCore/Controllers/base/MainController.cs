using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataModel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OdhApiCore.Controllers.api;
using Schema.NET;
using ServiceReferenceLCS;
using SqlKata.Execution;

namespace OdhApiCore.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [EnableCors("CorsPolicy")]
    public class MainController : OdhController
    {
        private static string absoluteUri = "";
        private readonly ISettings settings;

        public MainController(IWebHostEnvironment env, ISettings settings, ILogger<ODHActivityPoiController> logger, QueryFactory queryFactory) : base(env, settings, logger, queryFactory)
        {
            this.settings = settings;
        }

       

        [HttpGet, Route("", Name = "TourismApi")]
        [HttpGet, Route("Metadata", Name = "TourismApiMetaData")]
        public async Task<IActionResult> Get()
        {
            //var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
            var location = new Uri($"{Request.Scheme}://{Request.Host}");
            absoluteUri = location.AbsoluteUri;

            var result = await GetMainApi();

            return Ok(result);
        }



        #region POST PUT DELETE



        #endregion

        #region HELPERS

        private async Task<IEnumerable<TourismData>> GetMainApi()
        {
            List<TourismData> tourismdatalist = new List<TourismData>();

            string fileName = Path.Combine(settings.JsonConfig.Jsondir, $"v1.json");

            using (StreamReader r = new StreamReader(fileName))
            {
                string json = await r.ReadToEndAsync();

                tourismdatalist = JsonConvert.DeserializeObject<List<TourismData>>(json);
            }

            return tourismdatalist;
        }

        public static string GetAbsoluteUri()
        {
            return absoluteUri;
        }

        private async Task<ActionResult> WriteJsonToTable()
        {
            var jsondata = await GetMainApi();
            string table = "metadata";
            int result = 0;

            foreach(var json in jsondata)
            {
                //Check if data exists
                var query = QueryFactory.Query(table)
                          .Select("data")
                          .Where("id", json.Id);

                var queryresult = await query.GetAsync<TourismData>();

                if (queryresult == null || queryresult.Count() == 0)
                {                    
                    result = await QueryFactory.Query(table)
                       .InsertAsync(new JsonBData() { id = json.Id, data = new JsonRaw(json) });                    
                }
                else
                {
                    result = await QueryFactory.Query(table).Where("id", json.Id)
                            .UpdateAsync(new JsonBData() { id = json.Id, data = new JsonRaw(json) });                    
                }

            }

            return Ok("Result: " + result);
        }


        #endregion
    }

    public class TourismData
    {
        //public TourismData(string url)
        //{
        //    ApplicationURL = url;
        //}

        //private string ApplicationURL { get; set; }

        public string ApiIdentifier { get; set; } = default!;
        
        public string ApiFilter { get; set; }

        public string Id { get; set; } = default!;
        public string OdhType { get; set; } = default!;
        public string Description { get; set; } = default!;

        private string swaggerUrl = default!;
        public string SwaggerUrl
        {
            get { return Uri.EscapeUriString(MainController.GetAbsoluteUri() + "swagger/index.html#/" + swaggerUrl); }
            set { swaggerUrl = value; }
        }

        public string Self
        {
            get
            {
                return Uri.EscapeUriString(MainController.GetAbsoluteUri() + "v1/") + this.ApiIdentifier + this.ApiFilter;                
            }
        }

        public string Source { get; set; }

        public string License { get; set; } = default!;

        public string LicenseType { get; set; }

        public string LicenseInfo { get; set; }

        public bool Deprecated { get; set; }

        public bool SingleDataset { get; set; }        
    }
}
