using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace OdhApiCoreTests.IntegrationTets
{
    [Trait("Category", "Integration")]
    public class PoiApiControllerTests : IClassFixture<CustomWebApplicationFactory<OdhApiCore.Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<OdhApiCore.Startup> _factory;

        public PoiApiControllerTests(CustomWebApplicationFactory<OdhApiCore.Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        private static T JsonIsType<T>(JToken token)
        {
            switch (token)
            {
                case JValue value when (value.Value == null):
                    return default!;
                case JValue value:
                    return Assert.IsType<T>(value.Value);
                default:
                    return Assert.IsType<T>(token);
            }
        }

        [Theory]
        [InlineData("/api/Poi")]
        [InlineData("/api/Poi?pagesize=1")]
        [InlineData("/api/Poi?poitype=12")]
        [InlineData("/api/Poi?language=de")]
        [InlineData("/api/Poi?language=en")]
        [InlineData("/api/Poi?pagenumber=1&pagesize=100&poitype=511&areafilter=skaSKIC57DA31F859141A1802E86B410FEBD70&active=true&seed=null")]
        [InlineData("/api/Poi?pagenumber=1&pagesize=100&poitype=511&areafilter=skaSKIEC3B49365C47477B83D124D9AE6C3259&active=true&seed=null")]
        [InlineData("/api/Poi?pagenumber=1&pagesize=10&poitype=11&locfilter=tvs5228229651CA11D18F1400A02427D15E&odhactive=true&active=true&seed=null")]
        [InlineData("/api/Poi?pagenumber=1&pagesize=10&poitype=511&highlight=true&seed=null")]
        [InlineData("/api/Poi?pagenumber=1&pagesize=20&poitype=Sport%20und%20Freizeit&subtype=null&idlist=null&locfilter=null&areafilter=null&highlight=&active=&odhactive=&odhtagfilter=null&seed=null")]
        //Fails because boolean don't accept null string, to verifiy
        //[InlineData("/api/Activity?pagenumber=1&pagesize=20&activitytype=Berg&subtype=null&idlist=null&locfilter=null&areafilter=null&distancefilter=null&altitudefilter=null&durationfilter=null&highlight=null&difficultyfilter=null&active=null&odhactive=null&odhtagfilter=null&seed=null")]
        public async Task Get_Pois(string url)
        {
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            string json = await response.Content.ReadAsStringAsync();
            dynamic? data = JsonConvert.DeserializeObject(json);
            Assert.NotNull(data);
            if (data != null)
            {
                Assert.IsType<JObject>(data);
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

        [Theory]
        [InlineData("/api/Poi/EF22956102A175B15EA27EEBC03EB10D")]
        [InlineData("/api/Poi/D1AFE8FAB27A00518DEA1576119E03DE")]
        public async Task Get_SinglePoi(string url)
        {
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            string json = await response.Content.ReadAsStringAsync();
            dynamic? data = JsonConvert.DeserializeObject(json);
            Assert.NotNull(data);
            if (data != null)
            {
                Assert.IsType<JObject>(data);
                JsonIsType<string>(data.Id);
                JsonIsType<string>(data.Type);
                JsonIsType<string?>(data.SmgId);
                JsonIsType<bool>(data.Active);
                Assert.IsType<JArray>(data.AreaId);
                Assert.NotEmpty(data.AreaId);
                Assert.IsType<JObject>(data.Detail);
                Assert.IsType<JObject>(data.Detail.de);
                JsonIsType<string>(data.Detail.de.Title);
                JsonIsType<bool>(data.IsOpen);
                Assert.IsType<JArray>(data.GpsInfo);
                JsonIsType<bool>(data.Highlight);
                Assert.IsType<JArray>(data.HasLanguage);
                Assert.IsType<JObject>(data.ContactInfos);
                Assert.IsType<JObject>(data.ContactInfos.de);
                JsonIsType<string>(data.TourismorganizationId);
            }
        }
    }
}
