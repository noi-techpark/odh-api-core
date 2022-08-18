using Microsoft.AspNetCore.Mvc;

namespace OdhApiImporter.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class TestController : Controller
    {
        [HttpGet, Route("Test")]
        public IActionResult Get()
        {
            return Ok("importer alive");
        }
    }
}
