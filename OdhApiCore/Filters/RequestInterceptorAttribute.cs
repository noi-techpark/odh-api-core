using DataModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OdhApiCore.Responses;
using System;
using System.Collections.Generic;
using System.IO;
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

                await GetReturnObject(context, actionid ?? "", actionarguments, httpheaders);                
            }
            else
            {
                await base.OnActionExecutionAsync(context, next);
            }
           
            //Maybe with config like "action", match (parameter:blah) (referer:blah) return "json", withlanguage true/false

        }

        //public override async Task OnActionExecuting(HttpActionContext context, ActionExecutionDelegate next)
        //{

        //}

        public bool GetActionsToIntercept(string? actionid)
        {
            if (settings.RequestInterceptorConfig != null && settings.RequestInterceptorConfig.Where(x => x.Route == actionid).Count() > 0)
                return true;
            else
                return false;
        }

        public Task GetReturnObject(ActionExecutingContext context, string action, IDictionary<string, object> actionarguments, IHeaderDictionary headerDictionary)
        {     
            var language = (string?)actionarguments["language"];

            string fileName = Path.Combine(settings.JsonConfig.Jsondir, $"STAAccommodations_{language}.json");

            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEndAsync().Result;

                //var response = this.Request.CreateResponse(HttpStatusCode.OK);
                //response.Content = new StringContent(json, Encoding.UTF8, "application/json");
                //return ResponseMessage(response);

                context.Result = new OkObjectResult(new JsonRaw(json));
            }

            return Task.CompletedTask;
        }
    }
}
