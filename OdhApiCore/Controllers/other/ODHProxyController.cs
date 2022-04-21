using AspNetCore.Proxy;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.other
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ODHProxyController : Controller
    {
        [ApiExplorerSettings(IgnoreApi = true)]        
        [HttpGet, Route("v1/ODHProxy/{url}")]
        public Task GetProxied(string url)
        {            
            return this.HttpProxyAsync(url);
        }
    }
}
