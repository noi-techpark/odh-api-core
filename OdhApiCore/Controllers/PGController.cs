using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace OdhApiCore.Controllers
{    
    public abstract class PGController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        protected string connectionString = "";

        public PGController(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("PGConnection");
        }

    }
}