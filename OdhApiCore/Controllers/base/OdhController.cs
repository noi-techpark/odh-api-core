using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public class CommaSeparatedArrayBinder : Microsoft.AspNetCore.Mvc.ModelBinding.IModelBinder
    {
        public Task BindModelAsync(Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var value = valueProviderResult.FirstValue; // get the value as string

            var model = value.Split(",");
            bindingContext.Result = Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }
    }

    [ApiController]
    public abstract class OdhController : ControllerBase
    {
        private readonly IPostGreSQLConnectionFactory connectionFactory;
        private readonly ISettings settings;

        protected bool CheckCC0License => settings.CheckCC0License;

        public OdhController(ISettings settings, IPostGreSQLConnectionFactory connectionFactory)
        {
            this.settings = settings;
            this.connectionFactory = connectionFactory;
        }

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
            catch (Exception ex)
            {
                if (ex.Message == "Request Error")
                    return this.BadRequest(new { error = ex.Message });
                else
                    return this.StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        protected Task<IActionResult> DoAsyncReturnString(Func<IPostGreSQLConnectionFactory, Task<string?>> f)
        {
            return DoAsync(async connectionFactory =>
            {
                string? result = await f(connectionFactory);
                if (result == null)
                    return this.NotFound();
                else
                    return this.Content(result, "application/json", Encoding.UTF8);
            });
        }

    }
}