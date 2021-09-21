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

            var actiontointercept = GetActionsToIntercept(actionid);

            if (actiontointercept != null)
            {
                //Configure here the Actions where Interceptor should do something and configure it globally

                //Getting the Querystrings
                GetQueryStringsToIntercept(actiontointercept, context.ActionArguments);
                //bool? availabilitycheck = ((LegacyBool)actionarguments["availabilitycheck"]).Value;  //EX

                ////Getting Referer
                //var httpheaders = context.HttpContext.Request.Headers;
                //var referer = httpheaders["Referer"].ToString();

                //actionarguments["availabilitycheck"]).Value;

                //await GetReturnObject(context, actionid ?? "", actionarguments, httpheaders);
                
                await base.OnActionExecutionAsync(context, next);
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

        public RequestInterceptorConfig? GetActionsToIntercept(string? actionid)
        {
            if (settings.RequestInterceptorConfig != null && settings.RequestInterceptorConfig.Where(x => x.Route == actionid).Count() > 0)
                return settings.RequestInterceptorConfig.Where(x => x.Route == actionid).FirstOrDefault();
            else
                return null;
        }

        public void GetQueryStringsToIntercept(RequestInterceptorConfig config, IDictionary<string,object> querystrings)
        {
            //Forget about cancellationtoken and other generated
            Dictionary<string, string> configdict = new Dictionary<string, string>();

            foreach(var item in config.QueryStrings)
            {
                var configqssplitted = item.Split("=");

                if(configqssplitted.Count() >= 2)
                {
                    configdict.TryAdd(configqssplitted[0], configqssplitted[1]);
                }
                
            }

            var actualdict = new Dictionary<string, string>(); 

            foreach(var item in querystrings)
            {
                actualdict.TryAdd(item.Key, item.Value.ToString());
            }

            var resultDict =
                    configdict.Where(x => actualdict.ContainsKey(x.Key))
                               .ToDictionary(x => x.Key, x => x.Value + actualdict[x.Key]);
        }

        //public Task GetReturnObject(ActionExecutingContext context, string action, IDictionary<string, object> actionarguments, IHeaderDictionary headerDictionary)
        //{     
        //    var language = (string?)actionarguments["language"];

        //    string fileName = Path.Combine(settings.JsonConfig.Jsondir, $"STAAccommodations_{language}.json");

        //    using (StreamReader r = new StreamReader(fileName))
        //    {
        //        string json = r.ReadToEndAsync().Result;

        //        //var response = this.Request.CreateResponse(HttpStatusCode.OK);
        //        //response.Content = new StringContent(json, Encoding.UTF8, "application/json");
        //        //return ResponseMessage(response);

        //        context.Result = new OkObjectResult(new JsonRaw(json));
        //    }

        //    return Task.CompletedTask;
        //}
    }
}
