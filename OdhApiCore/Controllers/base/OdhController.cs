using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Npgsql;

namespace OdhApiCore.Controllers
{
    public class NullStringParameterActionFilterAttribute : ActionFilterAttribute
    {
        public NullStringParameterActionFilterAttribute()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var key in context.ActionArguments.Keys.ToArray())
            {
                var value = context.ActionArguments[key];
                if (value as string == "null")
                {
                    context.ActionArguments[key] = null;
                }
            }
            base.OnActionExecuting(context);
        }
    }

    [ApiController]
    public abstract class OdhController : ControllerBase
    {
        protected readonly string connectionString;

        public OdhController(ISettings settings)
        {
            this.connectionString = settings.PostgresConnectionString;
        }

        private static async Task<NpgsqlConnection> CreateConnection(
            string connectionString, CancellationToken cancellationToken)
        {
            // TODO: additional initialization logic goes here
            var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync(cancellationToken);
            return conn;
        }

        protected async Task<IActionResult> DoAsync(Func<Func<CancellationToken, Task<NpgsqlConnection>>, Task<string>> f)
        {
            try
            {
                Task<NpgsqlConnection> connectionFactory(CancellationToken cancellationToken) =>
                    CreateConnection(this.connectionString, cancellationToken);
                return this.Content(await f(connectionFactory), "application/json", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Request Error")
                    return this.BadRequest(new { error = ex.Message });
                else
                    return this.StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

    }
}