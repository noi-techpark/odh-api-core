using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace OdhApiCore.Middleware
{
    class OdhRateLimitConfiguration : RateLimitConfiguration
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OdhRateLimitConfiguration(
            IHttpContextAccessor httpContextAccessor,
            IOptions<IpRateLimitOptions> ipOptions,
            IOptions<ClientRateLimitOptions> clientOptions)
            : base(ipOptions, clientOptions)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override void RegisterResolvers()
        {
            ClientResolvers.Add(new OdhResolveContributor(_httpContextAccessor));
        }
    }

    class OdhResolveContributor : IClientResolveContributor
    {
        private IHttpContextAccessor httpContextAccessor;

        public OdhResolveContributor(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task<string> ResolveClientAsync(HttpContext httpContext)
        {
            var request = httpContextAccessor.HttpContext?.Request;
            string clientId = request?.Headers.Authorization.ToString() != "" ? "Authenticated" : "Anonymous";
            return Task.FromResult(clientId);
        }
    }
}
