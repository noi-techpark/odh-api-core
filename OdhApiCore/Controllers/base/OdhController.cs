using Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using System;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{

    [ApiController]
    [FormatFilter]
    public abstract class OdhController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly ISettings settings;
        private readonly ILogger<OdhController> logger;
        private readonly IPostGreSQLConnectionFactory connectionFactory;
        private readonly PostgresQueryFactory queryFactory;

        protected bool CheckCC0License => settings.CheckCC0License;

        public OdhController(IWebHostEnvironment env, ISettings settings, ILogger<OdhController> logger, IPostGreSQLConnectionFactory connectionFactory, PostgresQueryFactory queryFactory)
        {
            this.env = env;
            this.settings = settings;
            this.logger = logger;
            this.connectionFactory = connectionFactory;
            this.queryFactory = queryFactory;
        }

        protected ILogger<OdhController> Logger => logger;
        protected PostgresQueryFactory QueryFactory => queryFactory;

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