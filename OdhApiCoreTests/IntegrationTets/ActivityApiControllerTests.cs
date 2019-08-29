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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        private static T JsonIsType<T>(object obj)
        {
            switch (obj)
            {
                case JValue value:
                    return Assert.IsType<T>(value.Value);
                default:
                    return Assert.IsType<T>(obj);
            }
        }

        [Theory]
        [InlineData("/api/Activity")]
        [InlineData("/api/Activity?pagesize=1")]
        [InlineData("/api/Activity?activitytype=12")]
        [InlineData("/api/ActivityLocalized?language=de")]
        [InlineData("/api/ActivityLocalized?language=en")]
        public async Task Get_Activities(string url)
        {
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", 
                response.Content.Headers.ContentType.ToString());
            string json = await response.Content.ReadAsStringAsync();
            dynamic? data = JsonConvert.DeserializeObject(json);
            Assert.NotNull(data);
            if (data != null) {
                JsonIsType<long>(data.TotalResults);
                Assert.NotEqual(0, (long)data.TotalResults);
                JsonIsType<long>(data.TotalPages);
                Assert.NotEqual(0, (long)data.TotalPages);
                JsonIsType<long>(data.CurrentPage);
                Assert.Equal(1, (long)data.CurrentPage);
                JsonIsType<string>(data.Seed);
                Assert.Empty(data.Seed);
                Assert.IsType<JArray>(data.Items);
                Assert.NotEmpty(data.Items);
            }
        }
    }
}
