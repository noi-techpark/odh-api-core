using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.api
{
    public class TestController : Controller
    {
        private readonly IConfiguration configuration;

        public TestController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet, Route("api/test")]
        public string Get()
        {
            //try
            //{
            //    using (var conn = new NpgsqlConnection(configuration.GetConnectionString("PgConnection")))
            //    {
            //        conn.Open();

            //        return "connection exstabilished";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return ex.Message;
            //}

            //return configuration.GetConnectionString("PgConnection");

            return "hallo";
        }

        [HttpGet, Route("api/testdatabase")]
        public string GetTest()
        {
            try
            {
                using (var conn = new NpgsqlConnection(configuration.GetConnectionString("PgConnection")))
                {
                    conn.Open();

                    return "connection exstabilished";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
