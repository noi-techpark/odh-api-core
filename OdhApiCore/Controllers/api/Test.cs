using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.api
{
    public class Test : Controller
    {
        private readonly IConfiguration configuration;

        public Test(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet, Route("api/test")]
        public string Get()
        {
            var x = configuration.GetConnectionString("PGConnection");

            return x;
        }
    }
}
