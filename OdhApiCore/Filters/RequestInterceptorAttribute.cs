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

            var matchedactiontointercept = GetActionsToIntercept(actionid, controllerid, context.ActionArguments);

            if (matchedactiontointercept != null)
            {
                RouteValueDictionary redirectTargetDictionary = new RouteValueDictionary();

                redirectTargetDictionary.Add("action", matchedactiontointercept.RedirectAction);
                redirectTargetDictionary.Add("controller", matchedactiontointercept.RedirectController);

                if (matchedactiontointercept.RedirectQueryStrings != null)
                {
                    foreach (var redirectqs in matchedactiontointercept.RedirectQueryStrings)
                    {
                        if (context.ActionArguments.ContainsKey(redirectqs))
                            redirectTargetDictionary.Add(redirectqs, context.ActionArguments[redirectqs]);
                    }
                }

                context.Result = new RedirectToRouteResult(redirectTargetDictionary);
                await context.Result.ExecuteResultAsync(context);
            }
            
            await base.OnActionExecutionAsync(context, next);                                  
        }

        public RequestInterceptorConfig? GetActionsToIntercept(string actionid, string controller, IDictionary<string,object> actionarguments)
        {
            if (settings.RequestInterceptorConfig != null && settings.RequestInterceptorConfig.Where(x => x.Action == actionid && x.Controller == controller).Count() > 0)
            {
                foreach(var validconfig in settings.RequestInterceptorConfig.Where(x => x.Action == actionid && x.Controller == controller))
                {
                    var match = GetQueryStringsToInterceptAndMatch(validconfig, actionarguments);

                    if (match)
                        return validconfig;
                }                
            }                
            
            return null;
        }

        public bool GetQueryStringsToInterceptAndMatch(RequestInterceptorConfig config, IDictionary<string,object> querystrings)
        {
            //Forget about cancellationtoken and other generated
            Dictionary<string, string> configdict = new Dictionary<string, string>();

            if (config.QueryStrings != null)
            {
                foreach (var item in config.QueryStrings)
                {
                    var configqssplitted = item.Split("=");

                    if (configqssplitted.Count() >= 2)
                    {
                        configdict.TryAdd(configqssplitted[0], configqssplitted[1].ToLower());
                    }
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
                        actualdict.TryAdd(item.Key, ((string)item.Value));
                    }                        
                }
            }

            //Matching the two Dictionaries
            return MatchDictionaries(configdict, actualdict);               
        }

        private static bool MatchDictionaries(IDictionary<string,string> dict1, IDictionary<string,string> dict2)
        {
            List<string> validlanguages = new List<string>() { "de", "it", "en", "nl", "cs", "pl", "fr", "ru" };

            //return only if there is a 1:1 match
            foreach(var item in dict1)
            {
                //if the Request does not contain the configured QS Key exit immediately
                if (!dict2.ContainsKey(item.Key))
                    return false;


                //if the config does not contains a * go on
                if (!item.Value.Contains("*"))
                {
                    //if the values are different exit immediately
                    if (dict2[item.Key].ToLower() != item.Value.ToLower())
                        return false;
                }
                else
                {
                    //Specialcase Language + fields with *
                    //check if one of the validlanguages matches
                    int matchcount = 0;

                    foreach(var lang in validlanguages)
                    {
                        var newconfigvalue = item.Value.ToLower().Replace("*", lang);

                        if(dict2[item.Key].ToLower() == newconfigvalue.ToLower())
                             matchcount++;
                    }

                    //if no matches exit immediately
                    if (matchcount == 0)
                        return false;
                }
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
