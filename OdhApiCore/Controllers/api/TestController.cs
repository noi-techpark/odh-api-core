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

namespace OdhApiCore.Controllers.api
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly ISettings settings;

        public TestController(IWebHostEnvironment env, ISettings settings)           
        {
            this.env = env;
            this.settings = settings;
        }

        [TypeFilter(typeof(Filters.RequestInterceptorAttribute))]
        [HttpGet, Route("TestObject")]
        public IActionResult GetTagObject(string language = null, CancellationToken cancellationToken = default)
        {
            var tag = new SmgTags() { Id = "test", MainEntity = "Blah", Shortname = "hallo", TagName = null, ValidForEntity = new List<string>() { "hallo" } };

            return Ok(tag);
        }
     

        [HttpGet, Route("Anonymous")]
        public IActionResult GetAnonymous(CancellationToken cancellationToken)
        {
            return this.Content(User.Identity?.Name + " Anonymous working", "application/json", Encoding.UTF8);
        }

        [Authorize]
        [HttpGet, Route("Restricted")]
        public IActionResult GetRestricted(CancellationToken cancellationToken)
        {
            return this.Content(User.Identity?.Name + " Restricted working", "application/json", Encoding.UTF8);
        }

        [Authorize(Roles = "DataReader")]
        [HttpGet, Route("WithRole")]
        public IActionResult GetWithRole(CancellationToken cancellationToken)
        {
            return this.Content(User.Identity?.Name + " WithRole working", "application/json", Encoding.UTF8);
        }

        [Authorize(Roles = "Hallihallo")]
        [HttpGet, Route("WithRole2")]
        public IActionResult GetWithRole2(CancellationToken cancellationToken)
        {
            return this.Content(User.Identity?.Name + " WithRole2 working", "application/json", Encoding.UTF8);
        }

        [HttpGet, Route("Environment")]
        public IActionResult GetEnvironmentV(CancellationToken cancellationToken)
        {
            var siaguser = settings.SiagConfig.Username;
            var mssuser = settings.MssConfig.Username;
            var xmldir = settings.XmlConfig.Xmldir;
            var xmldir2 = settings.XmlConfig.XmldirWeather;
            var pgconnection = settings.PostgresConnectionString;


            return this.Content(" mss user: " + mssuser + " siag user " + siaguser + " xmldir " + xmldir + " " + xmldir2  + " pgconnection " + pgconnection.Substring(0,50), "application/json", Encoding.UTF8);
        }

        #region CacheTestController

        // Cache for 100 seconds on the server, inform the client that response is valid for 100 seconds
        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100)]
        [HttpGet, Route("Cached100")]
        public IEnumerable<string> GetCached100()
        {
            return new string[] { "value1", "value2", DateTime.Now.ToLongTimeString() };
        }

        // Cache for 100 seconds on the server, inform the client that response is valid for 100 seconds. Cache for anonymous users only.
        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100, AnonymousOnly = true)]
        [HttpGet, Route("Cached100Anonymous")]
        public IEnumerable<string> GetCached100Anonymous()
        {
            return new string[] { "value1", "value2", DateTime.Now.ToLongTimeString() };
        }

        // Inform the client that response is valid for 50 seconds. Force client to revalidate.
        [CacheOutput(ClientTimeSpan = 50, MustRevalidate = true)]
        [HttpGet, Route("Cached50Revalidate")]
        public IEnumerable<string> GetCached50Revalidate(string hello)
        {
            return new string[] { "value1", "value2", DateTime.Now.ToLongTimeString() };
        }

        // Cache for 50 seconds on the server. Ignore querystring parameters when serving cached content.
        [CacheOutput(ServerTimeSpan = 50, ExcludeQueryStringFromCacheKey = true)]
        [HttpGet, Route("Cached50WithoutQS")]
        public IEnumerable<string> GetCached50WithoutQS(string hello)
        {
            return new string[] { "value1", "value2", DateTime.Now.ToLongTimeString() };
        }

        //TODO SEE if Cache distinguish between Authenticated and not authenticated user

        #endregion
    }
}