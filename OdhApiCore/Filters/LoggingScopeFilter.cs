using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using OdhApiCore.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdhApiCore.Filters
{
    public class LoggingScopeFilter : IActionFilter
    {
        private readonly ILoggerFactory loggerFactory;

        public LoggingScopeFilter(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var logger = loggerFactory.CreateLogger(context.Controller.GetType().FullName);
            // The scope will automatically appended to every log event
            using var scope = logger.BeginScope(new Dictionary<string, object> {
                { "operation_id", context.HttpContext.TraceIdentifier }
            });
        }

        public void OnActionExecuted(ActionExecutedContext context)
        { }
    }
}
