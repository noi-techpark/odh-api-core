using AspNetCore.Proxy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.other
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ODHProxyController : ControllerBase
    {
        
        [ApiExplorerSettings(IgnoreApi = true)]                
        [HttpGet, Route("v1/ODHProxy/{*url}")]        
        public Task GetODHProxy(string url)
        {
            try
            {
                var parameter = "?";

                foreach (var paramdict in HttpContext.Request.Query)
                {
                    parameter = parameter + paramdict.Key + "=" + paramdict.Value;
                }


                var fullurl = url + parameter;

                return this.HttpProxyAsync(fullurl);
            }
            catch(Exception ex)
            {
                ex.HelpLink = url;

                return Task.FromException(ex);
            }
        }
    }
}
