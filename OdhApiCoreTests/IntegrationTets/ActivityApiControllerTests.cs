using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace OdhApiCoreTests.IntegrationTets
{
    public class ActivityApiControllerTests : IClassFixture<CustomWebApplicationFactory<OdhApiCore.Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<OdhApiCore.Startup> _factory;

        public ActivityApiControllerTests(CustomWebApplicationFactory<OdhApiCore.Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Get_ActivitiesWithoutQueryString()
        {
            var response = await _client.GetAsync("/api/Activity");
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", 
                response.Content.Headers.ContentType.ToString());
        }
    }
}
