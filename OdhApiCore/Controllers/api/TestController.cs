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
        public IActionResult GetTagObject(CancellationToken cancellationToken)
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
    }
}