using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace OdhApiCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class OdhController : ControllerBase
    {
        protected readonly string connectionString;

        public OdhController(ISettings settings)
        {
            connectionString = settings.PostgresConnectionString;
        }

        protected IActionResult Do(Func<NpgsqlConnection, string> f)
        {
            try
            {
                using (var conn = new NpgsqlConnection(this.connectionString))
                {
                    conn.Open();

                    return this.Content(f(conn), "application/json", Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                if(ex.Message == "Request Error")
                    return this.BadRequest(new { error = ex.Message });
                else
                    return this.StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

    }
}