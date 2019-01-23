using Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/Test")]
    public class TestController : Controller
    {
        private readonly IConfiguration configuration;
        private string connectionString = "";

        public TestController(IConfiguration config)
        {
            configuration = config;
            connectionString = configuration.GetConnectionString("PGConnection");
        }

        // GET: api/Test
        [HttpGet]
        public IActionResult Get()
        {
            //return Content("hallo", "application/json");

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {

                    conn.Open();

                    string select = "*";
                    string orderby = "data ->>'MainEntity', data ->>'Shortname'";
                    string where = "";

                    var myresult = PostgresSQLHelper.SelectFromTableDataAsString(conn, "smgtags", select, where, orderby, 0, null);

                    conn.Close();

                    //return new HttpResponseMessage()
                    //{
                    //    StatusCode = HttpStatusCode.OK,
                    //    Content = new StringContent("[" + String.Join(",", myresult) + "]", Encoding.UTF8, "application/json"),
                    //};

                    JsonResult myjson = Json(myresult);


                    return Content("[" + String.Join(",", myresult) + "]", "application/json");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Test/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            //string strHostName = Dns.GetHostName();
            //IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            //IPAddress[] addr = ipEntry.AddressList;

            //string ipadresses = "";

            //for (int i = 0; i < addr.Length; i++)
            //{
            //    ipadresses = ipadresses + addr[i].ToString() + ";";
            //}

            //return "HOST " + strHostName + " IP: " + ipadresses;

            return "hallo";
        }

        //// GET: api/Test/5
        //[Route("[action]")]
        //[HttpGet]
        //public bool GetTest()
        //{
        //    bool pingable = false;
        //    Ping pinger = null;

        //    try
        //    {
        //        pinger = new Ping();
        //        PingReply reply = pinger.Send("172.19.0.2");
        //        pingable = reply.Status == IPStatus.Success;
        //    }
        //    catch (PingException)
        //    {
        //        // Discard PingExceptions and return false;
        //    }
        //    finally
        //    {
        //        if (pinger != null)
        //        {
        //            pinger.Dispose();
        //        }
        //    }

        //    return pingable;
        //}

        // POST: api/Test
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Test/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
