using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{

    [ApiController]
    [FormatFilter]
    public abstract class OdhController : ControllerBase
    {
        private readonly IPostGreSQLConnectionFactory connectionFactory;
        private readonly ILogger<OdhController> logger;
        private readonly ISettings settings;

        protected bool CheckCC0License => settings.CheckCC0License;

        public OdhController(ISettings settings, ILogger<OdhController> logger, IPostGreSQLConnectionFactory connectionFactory)
        {
            this.settings = settings;
            this.logger = logger;
            this.connectionFactory = connectionFactory;
        }

        protected ILogger<OdhController> Logger => logger;

        protected async Task<IActionResult> DoAsync(Func<IPostGreSQLConnectionFactory, Task<IActionResult>> f)
        {
            try
            {
                return await f(this.connectionFactory);
            }
            catch (PostGresSQLHelperException ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
            catch (JsonPathException ex)
            {
                return this.BadRequest(new {
                    error = "Invalid JSONPath selection",
                    path = ex.Path,
                    details = ex.Message
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Request Error")
                    return this.BadRequest(new { error = ex.Message });
                else
                    return this.StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        protected Task<IActionResult> DoAsyncReturn(Func<IPostGreSQLConnectionFactory, Task<object?>> f)
        {
            return DoAsync(async connectionFactory =>
            {
                object? result = await f(connectionFactory);
                if (result == null)
                    return this.NotFound();
                else
                    return this.Ok(result);
            });
        }
    }
}