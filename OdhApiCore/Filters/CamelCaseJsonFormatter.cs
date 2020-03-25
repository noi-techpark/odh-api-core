using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json.Serialization;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdhApiCore.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CamelCaseJsonFormatter : ActionFilterAttribute
    {
        public CamelCaseJsonFormatter()
        {
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context == null || context.Result == null)
                return;

            var settings = JsonSerializerSettingsProvider.CreateSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var formatter = new NewtonsoftJsonOutputFormatter(settings, ArrayPool<char>.Shared, new MvcOptions());

            switch (context.Result)
            {
                case OkObjectResult result:
                    result.Formatters.Add(formatter);
                    break;
            };
        }
    }
}
