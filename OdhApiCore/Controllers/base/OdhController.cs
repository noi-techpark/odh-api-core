using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
        private readonly IPostGreSQLConnectionFactory connectionFactory;

        public OdhController(IPostGreSQLConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        protected async Task<IActionResult> DoAsync(Func<IPostGreSQLConnectionFactory, Task<string>> f)
        {
            try
            {
                return this.Content(await f(this.connectionFactory), "application/json", Encoding.UTF8);
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