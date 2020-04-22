using Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using System;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    [ApiController]
    [Route("api")]
    [FormatFilter]
    public abstract class OdhController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly ISettings settings;

        public OdhController(IWebHostEnvironment env, ISettings settings, ILogger<OdhController> logger, QueryFactory queryFactory)
        {
            this.env = env;
            this.settings = settings;
            this.Logger = logger;
            this.QueryFactory = queryFactory;
        }

        protected ILogger<OdhController> Logger { get; }

        protected QueryFactory QueryFactory { get; }

        protected bool FilterCC0License => FilterClosedData;

        protected bool FilterClosedData
        {
            get
            {
                var roles = new[] {
                    "DataReader",
                    $"{this.ControllerContext.ActionDescriptor.ControllerName}Reader"
                };
                return !roles.Any(User.IsInRole);
            }
        }

        protected Func<string, string> UrlGenerator
        {
            get
            {
                return self =>
                {
                    var chunks = self.Split('/', 2);
                    if (chunks.Length < 2)
                        return self;
                    var controller = chunks[0];
                    var action = chunks[1];
                    return Url.Link($"Single{controller}", new { controller, id = action }); ;
                };
            }
        }

        protected async Task<IActionResult> DoAsync(Func<Task<IActionResult>> f)
        {
            try
            {
                return await f();
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
                    details = env.IsDevelopment() ? ex.ToString() : ex.Message
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Request Error")
                    return this.BadRequest(new { error = env.IsDevelopment() ? ex.ToString() : ex.Message });
                else
                    return this.StatusCode(StatusCodes.Status500InternalServerError, new { error = env.IsDevelopment() ? ex.ToString() : ex.Message });
            }
        }

        protected Task<IActionResult> DoAsyncReturn(Func<Task<object?>> f)
        {
            return DoAsync(async () =>
            {
                object? result = await f();
                if (result == null)
                    return this.NotFound();
                else
                    return this.Ok(result);
            });
        }
    }
}