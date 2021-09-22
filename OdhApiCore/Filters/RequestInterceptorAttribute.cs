using DataModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using OdhApiCore.Controllers;
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
            context.ActionDescriptor.RouteValues.TryGetValue("controller", out string? controllerid);

            var actiontointercept = GetActionsToIntercept(actionid, controllerid);

            if (actiontointercept != null)
            {
                //Configure here the Actions where Interceptor should do something and configure it globally

                //Getting the Querystrings
                var match = GetQueryStringsToIntercept(actiontointercept, context.ActionArguments);
                //bool? availabilitycheck = ((LegacyBool)actionarguments["availabilitycheck"]).Value;  //EX

                ////Getting Referer
                //var httpheaders = context.HttpContext.Request.Headers;
                //var referer = httpheaders["Referer"].ToString();

                //actionarguments["availabilitycheck"]).Value;

                //await GetReturnObject(context, actionid ?? "", actionarguments, httpheaders);

                if (match)
                {
                    RouteValueDictionary redirectTargetDictionary = new RouteValueDictionary();
                    
                    redirectTargetDictionary.Add("action", actiontointercept.RedirectAction);
                    redirectTargetDictionary.Add("controller", actiontointercept.RedirectController);
                    
                    if(actiontointercept.RedirectQueryStrings != null)
                    {
                        foreach (var redirectqs in actiontointercept.RedirectQueryStrings)
                        {
                            redirectTargetDictionary.Add("language", context.ActionArguments["language"]);
                        }
                    }

                    context.Result = new RedirectToRouteResult(redirectTargetDictionary);
                    await context.Result.ExecuteResultAsync(context);
                }                
            }
            
            await base.OnActionExecutionAsync(context, next);                                  
        }

        public RequestInterceptorConfig? GetActionsToIntercept(string actionid, string controller)
        {
            if (settings.RequestInterceptorConfig != null && settings.RequestInterceptorConfig.Where(x => x.Action == actionid && x.Controller == controller).Count() > 0)
                return settings.RequestInterceptorConfig.Where(x => x.Action == actionid && x.Controller == controller).FirstOrDefault();
            else
                return null;
        }

        public bool GetQueryStringsToIntercept(RequestInterceptorConfig config, IDictionary<string,object> querystrings)
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

            List<string> toexclude = new List<string> { "cancellationToken" };

            foreach(var item in querystrings)
            {
                if (!toexclude.Contains(item.Key))
                {
                    if (item.Key == "pagesize")
                    {
                        actualdict.TryAdd(item.Key, ((PageSize)item.Value).Value.ToString());
                    }
                    else if (item.Key == "highlight" || item.Key == "active" || item.Key == "odhactive")
                    {
                        if (((LegacyBool)item.Value).Value != null)
                            actualdict.TryAdd(item.Key, ((LegacyBool)item.Value).Value.ToString());
                    }
                    else if (item.Key == "fields")
                    {
                        if (((string[])item.Value).Count() > 0)
                            actualdict.TryAdd(item.Key, (String.Join(",", (string[])item.Value)));
                    }
                    else
                    {
                        actualdict.TryAdd(item.Key, (string?)item.Value);
                    }                        
                }
            }

            //Matching the two Dictionaries
            return MatchDictionaries(configdict, actualdict);               
        }

        private static bool MatchDictionaries(IDictionary<string,string> dict1, IDictionary<string,string> dict2)
        {
            //return only if there is a 1:1 match
            foreach(var item in dict1)
            {
                if (!dict2.ContainsKey(item.Key))
                    return false;

                if (dict2[item.Key] != item.Value)
                    return false;
            }

            return true;
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
