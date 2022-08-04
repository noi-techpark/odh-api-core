using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore
{
    public static class HttpContextExtensions
    {
        public static void AddRateLimitHeaders(this HttpContext context, int maxrequests, int requestsdone, int timewindow, string policy, CancellationToken token = default)
        {
            var remainingrequests = maxrequests - requestsdone;

            context.Response.Headers.Add("X-Rate-Limit-Policy", policy);
            context.Response.Headers.Add("X-Rate-Limit-Limit", maxrequests.ToString());
            context.Response.Headers.Add("X-Rate-Limit-Remaining", remainingrequests.ToString());
            context.Response.Headers.Add("X-Rate-Limit-Reset", timewindow.ToString());            
        }
    }
}
