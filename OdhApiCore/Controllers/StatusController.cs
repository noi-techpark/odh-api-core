using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OdhApiCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        // GET api/status
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "api service started", "api service up!" };
        }

        // GET api/status/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "api service up";
        }

        // POST api/status
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/status/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/status/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

    }
}