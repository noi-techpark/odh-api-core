using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdhApiCore.Filters
{
    public class RequestInterceptorAttribute : ActionFilterAttribute
    {
        private readonly ISettings settings;

        public RequestInterceptorAttribute(ISettings settings)
        {
              this.settings = settings;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //Getting Action name example actionid == "GetAccommodations"
            context.ActionDescriptor.RouteValues.TryGetValue("action", out string? actionid);

            //Configure here the Actions where Interceptor should do something and configure it globally

            //Getting the Querystrings
            var actionarguments = context.ActionArguments;

            //actionarguments["availabilitycheck"]).Value;

            await base.OnActionExecutionAsync(context, next);
        }

        public bool GetActionsToIntercept()
        {
            return false;
        }

        public void GetReturnObject(string action)
        {

        }
    }
}
