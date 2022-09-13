using AspNetCore.CacheOutput;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OdhApiCore.Filters
{
    public class OdhCacheOutputAttribute : CacheOutputAttribute
    {
        protected override bool IsCachingAllowed(FilterContext actionContext, bool anonymousOnly)
        {
            var environment = actionContext.HttpContext.RequestServices.GetService<IWebHostEnvironment>();
            if (environment?.IsDevelopment() ?? false)
            {
                return false;
            }
            return base.IsCachingAllowed(actionContext, anonymousOnly);
        }
    }
}
