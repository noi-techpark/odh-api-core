using DataModel;
using Helper;
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
                RouteValueDictionary redirectTargetDictionary = new();

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

        public RequestInterceptorConfig? GetActionsToIntercept(string? actionid, string? controller, IDictionary<string, object?> actionarguments)
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

        public bool GetQueryStringsToInterceptAndMatch(RequestInterceptorConfig config, IDictionary<string, object?> querystrings)
        {
            // Forget about cancellationtoken and other generated
            Dictionary<string, string> configdict = new();

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

            var actualdict = new Dictionary<string, string?>();

            List<string> toexclude = new() { "cancellationToken" };

            foreach(var item in querystrings)
            {
                if (!toexclude.Contains(item.Key))
                {
                    if (item.Key == "pagesize")
                    {
                        actualdict.TryAdd(item.Key, ((PageSize?)item.Value)?.Value.ToString());
                    }
                    else if (item.Key == "highlight" || item.Key == "active" || item.Key == "odhactive" || item.Key == "hascc0image" || item.Key == "hasimage")
                    {
                        if (((LegacyBool?)item.Value)?.Value != null)
                            actualdict.TryAdd(item.Key, ((LegacyBool)item.Value).Value.ToString());
                    }
                    else if (item.Key == "fields")
                    {
                        if (((string[]?)item.Value)?.Count() > 0)
                            actualdict.TryAdd(item.Key, (String.Join(",", (string[])item.Value)));
                    }
                    else if (item.Key == "pagenumber")
                    {
                        actualdict.TryAdd(item.Key, ((uint?)item.Value).ToString());
                    }
                    else if (item.Key == "removenullvalues")
                    {
                        actualdict.TryAdd(item.Key, ((bool?)item.Value).ToString());
                    }
                    else
                    {
                        actualdict.TryAdd(item.Key, ((string?)item.Value));
                    }                        
                }
            }

            // Matching the two Dictionaries
            return MatchDictionaries(configdict, actualdict);               
        }

        private static bool MatchDictionaries(IDictionary<string, string> dict1, IDictionary<string, string?> dict2)
        {
             List<string> validlanguages = new() { "de", "it", "en", "nl", "cs", "pl", "fr", "ru" };

            // Return only if there is a 1:1 match
            foreach(var item in dict1)
            {
                // If the Request does not contain the configured QS Key exit immediately
                if (!dict2.ContainsKey(item.Key))
                    return false;


                // If the config does not contains a * go on
                if (!item.Value.Contains("*"))
                {
                    // If the values are different exit immediately
                    if (dict2?[item.Key]?.ToLower() != item.Value.ToLower())
                        return false;
                }
                else
                {
                    // Specialcase Language + fields with *
                    // check if one of the validlanguages matches
                    int matchcount = 0;

                    foreach(var lang in validlanguages)
                    {
                        var newconfigvalue = item.Value.ToLower().Replace("*", lang);

                        if(dict2?[item.Key]?.ToLower() == newconfigvalue.ToLower())
                             matchcount++;
                    }

                    // If no matches exit immediately
                    if (matchcount == 0)
                        return false;
                }
            }

            return true;
        }
    }
}
