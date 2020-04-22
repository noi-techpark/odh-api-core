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

namespace OdhApiCore.Controllers.api
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet, Route("Anonymous")]
        public IActionResult GetAnonymous(CancellationToken cancellationToken)
        {
            return this.Content(User.Identity.Name  + " Anonymous working", "application/json", Encoding.UTF8);
        }

        [Authorize]
        [HttpGet, Route("Restricted")]
        public IActionResult GetRestricted(CancellationToken cancellationToken)
        {
            return this.Content(User.Identity.Name + " Restricted working", "application/json", Encoding.UTF8);
        }

        [Authorize(Roles = "DataReader")]
        [HttpGet, Route("WithRole")]
        public IActionResult GetWithRole(CancellationToken cancellationToken)
        {
            return this.Content(User.Identity.Name + " WithRole working", "application/json", Encoding.UTF8);
        }

        [Authorize(Roles = "Hallihallo")]
        [HttpGet, Route("WithRole2")]
        public IActionResult GetWithRole2(CancellationToken cancellationToken)
        {
            return this.Content(User.Identity.Name + " WithRole2 working", "application/json", Encoding.UTF8);
        }
    }
}