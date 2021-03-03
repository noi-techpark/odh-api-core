using DataModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OdhApiCore.Responses;
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
            context.ActionDescriptor.RouteValues.TryGetValue("action", out string? actionid);

            if (GetActionsToIntercept(actionid))
            {
                //Configure here the Actions where Interceptor should do something and configure it globally

                //Getting the Querystrings
                var actionarguments = context.ActionArguments;
                //bool? availabilitycheck = ((LegacyBool)actionarguments["availabilitycheck"]).Value;  //EX

                //Getting Referer
                var httpheaders = context.HttpContext.Request.Headers;
                var referer = httpheaders["Referer"].ToString();

                //actionarguments["availabilitycheck"]).Value;

                await GetReturnObject(context, actionid, actionarguments, httpheaders);                
            }
            else
            {
                await base.OnActionExecutionAsync(context, next);
            }
           

        }

        //public override async Task OnActionExecuting(HttpActionContext context, ActionExecutionDelegate next)
        //{

        //}

        public bool GetActionsToIntercept(string? actionid)
        {
            if (actionid == "GetTagObject")
                return true;
            else
                return false;
        }

        public async Task GetReturnObject(ActionExecutingContext context, string action, IDictionary<string, object> actionarguments, IHeaderDictionary headerDictionary)
        {
            context.Result = new OkObjectResult(new JsonRaw("hallo " + action));

            return;
        }
    }
}
