using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OdhApiCoreTests.IntegrationTets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace OdhApiCoreTests.IntegrationTests
{
    [Trait("Category", "Integration")]
    public class FilterTests : IClassFixture<CustomWebApplicationFactory<OdhApiCore.Startup>>

    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<OdhApiCore.Startup> _factory;

        public FilterTests(CustomWebApplicationFactory<OdhApiCore.Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task TestFields()
        {
            var url = "/v1/Accommodation?pagesize=20&pagenumber=1&fields=AccoDetail.de.Name,Features[*].Name,HgvId,Test";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType?.ToString());
            string json = await response.Content.ReadAsStringAsync();
            dynamic? data = JsonConvert.DeserializeObject(json);
            Assert.NotNull(data);
            if (data != null)
            {
                Assert.IsType<JObject>(data);
                Helpers.JsonIsType<long>(data.TotalResults);
                Assert.NotEqual(0, (long)data.TotalResults);
                Helpers.JsonIsType<long>(data.TotalPages);
                Assert.NotEqual(0, (long)data.TotalPages);
                Helpers.JsonIsType<long>(data.CurrentPage);
                Assert.Equal(1, (long)data.CurrentPage);
                Helpers.JsonIsType<string>(data.Seed);
                Assert.Empty(data.Seed);
                Assert.IsType<JArray>(data.Items);
                Assert.NotEmpty(data.Items);

                Assert.Equal(20, data.Items.Count);

                var firstItem = data.Items[0];
                Helpers.JsonIsType<string>(firstItem.Id);
                Assert.Equal("118939DAC0CD11D2AE71004095429799", (string)firstItem.Id);
                Helpers.JsonIsType<string>(firstItem["AccoDetail.de.Name"]);
                Assert.Equal("\"Bamguat\"", (string)firstItem["AccoDetail.de.Name"]);
                Helpers.JsonIsType<JArray>(firstItem["Features[*].Name"]);
                Assert.Equal(ToJArray(new[] {
                           "Families",
                           "Seniors",
                           "Hiking",
                           "Bread delivery service",
                           "Pick-up service",
                           "Washing machine",
                           "Barbeque area",
                           "Garden ",
                           "Playground",
                           "Open car park",
                           "Fruit growing farm",
                           "WLAN",
                           "Free Wi-Fi",
                           "No pets or domestic animals",
                           "Outdoor  pool",
                           "Partner Therme Meran",
                           "Quiet location",
                           "MeranCard (15.10.-30.06.)"
                    }), (JArray)firstItem["Features[*].Name"]);
                Helpers.JsonIsType<string?>(firstItem.HgvId);
                Assert.Null((string?)firstItem.HgvId);
                Helpers.JsonIsType<object?>(firstItem.Test);
                Assert.Equal((object?)firstItem.Test, new JValue((object?)null));
            }
        }

        private IEnumerable<JToken> ToJArray(object[] objects)
        {
            return new JArray(objects.Select(@object => new JValue(@object)));
        }
    }
}
