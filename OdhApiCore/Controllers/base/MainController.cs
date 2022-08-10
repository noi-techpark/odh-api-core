using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceReferenceLCS;

namespace OdhApiCore.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly ISettings settings;
        private static string absoluteUri = ""; 

        public MainController(IWebHostEnvironment env, ISettings settings)
        {
            this.env = env;
            this.settings = settings;            
        }

        public static string GetAbsoluteUri()
        {
            return absoluteUri;
        }

        //Solved with Redirect
        //[HttpGet, Route("api")]
        [HttpGet, Route("v1", Name = "TourismApi")]
        public async Task<IActionResult> Get()
        {
            //var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
            var location = new Uri($"{Request.Scheme}://{Request.Host}");
            absoluteUri = location.AbsoluteUri;

            var result = await GetMainApi();

            return Ok(result);
        }

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
                return Uri.EscapeUriString(MainController.GetAbsoluteUri() + "v1/" + Uri.EscapeDataString(this.ApiIdentifier + this.ApiFilter));                
            }
        }

        public string Source { get; set; }

        public string License { get; set; } = default!;

        public string LicenseType { get; set; }        

        public bool Deprecated { get; set; }
    }
}
