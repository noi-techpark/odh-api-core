using Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OdhApiCoreTests.Helper
{
    public class JsonTransformerTests
    {
        [Fact]
        public void FilterByLanguageTest()
        {
            var actual = @"{""languages"":{""de"":""hallo"",""it"":""ciao"",""en"":""hello""}}";
            var expected = @"{""languages"":{""de"":""hallo""}}";
            var token = JToken.Parse(actual);
            var transformedToken = token.FilterByLanguage("de");
            var transformed = transformedToken.ToString(Formatting.None);
            Assert.Equal(expected, transformed);
        }

        [Fact]
        public void FilterImagesByCC0LicenseTest()
        {
            var actual = @"[{""License"":""CC0"",""Name"":""Image1""},{""License"":""LTS"",""Name"":""Image2""}]";
            var expected = @"[{""License"":""CC0"",""Name"":""Image1""}]";
            var token = JToken.Parse(actual);
            var transformedToken = token.FilterImagesByCC0License();
            var transformed = transformedToken.ToString(Formatting.None);
            Assert.Equal(expected, transformed);
        }

        [Fact]
        public void FilterByFieldsTests()
        {
            var actual = @"{
                ""Id"": 1,
                ""IsOpen"": true,
                ""Detail"": {
                    ""de"": {
                        ""Title"": ""Hallo"",
                        ""Body"": ""Welt""
                    },
                    ""en"": {
                        ""Title"": ""Hello"",
                        ""Body"": ""World""
                    }
                }
            }";
            var expected = @"{
                ""Id"": 1,
                ""Name"": ""Hallo"",
                ""IsOpen"": true,
                ""Detail.de.Title"": ""Hallo"",
                ""Detail['en']['Body']"": ""World"",
                ""FooBar"": null
            }";
            var fields = new string[] {
                "IsOpen", "Detail.de.Title", "Detail['en']['Body']", "FooBar"
            };
            var language = "de";
            var token = JToken.Parse(actual);
            var transformedToken = token.FilterByFields(fields, language);
            var expectedToken = JToken.Parse(expected);
            Assert.Equal(
                expectedToken.ToString(Formatting.None),
                transformedToken.ToString(Formatting.None)
            );
        }
    }
}
