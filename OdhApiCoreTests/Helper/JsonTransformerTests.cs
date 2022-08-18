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
            var actual =
                @"{
                ""languages"": {
                    ""de"": ""hallo"",
                    ""it"": ""ciao"",
                    ""en"": ""hello""
                }
            }";
            var expected =
                @"{
                ""languages"": {
                    ""de"": ""hallo""
                }
            }";
            var token = JToken.Parse(actual);
            var transformedToken = token.FilterByLanguage("de");
            var expectedToken = JToken.Parse(expected);
            Assert.Equal(expectedToken, transformedToken);
        }

        [Fact]
        public void FilterImagesByCC0LicenseTest()
        {
            var actual =
                @"[{
                ""License"": ""CC0"",
                ""Name"": ""Image1""
            },
            {
                ""License"": ""LTS"",
                ""Name"": ""Image2""
            }]";
            var expected =
                @"[{
                ""License"": ""CC0"",
                ""Name"": ""Image1""
            }]";
            var token = JToken.Parse(actual);
            var transformedToken = token.FilterImagesByCC0License();
            var expectedToken = JToken.Parse(expected);
            Assert.Equal(expectedToken, transformedToken);
        }

        //[Theory]
        //[InlineData("de", "Hallo")]
        //[InlineData("en", "Hello")]
        //[InlineData(null, "Hello")]
        //[InlineData("it", null)]
        //public void FilterByFieldsTests(string? language, string? expectedName)
        //{
        //    var actual = @"{
        //        ""Id"": 1,
        //        ""IsOpen"": true,
        //        ""Detail"": {
        //            ""de"": {
        //                ""Title"": ""Hallo"",
        //                ""Body"": ""Welt""
        //            },
        //            ""en"": {
        //                ""Title"": ""Hello"",
        //                ""Body"": ""World""
        //            }
        //        },
        //        ""Values"": [""A"", ""B"", ""C""]
        //    }";
        //    var expected = $@"{{
        //        ""Id"": 1,
        //        ""Name"": {(expectedName == null ? "null" : $@"""{expectedName}""")},
        //        ""IsOpen"": true,
        //        ""Detail.de.Title"": ""Hallo"",
        //        ""Detail['en']['Body']"": ""World"",
        //        ""Values[1]"": ""B"",
        //        ""FooBar"": null
        //    }}";
        //    // Documentation for JSONPath: https://goessner.net/articles/JsonPath/
        //    var fields = new string[] {
        //        "IsOpen",
        //        "Detail.de.Title",
        //        "Detail['en']['Body']",
        //        "Values[1]",
        //        "FooBar"
        //    };
        //    var token = JToken.Parse(actual);
        //    var transformedToken = token.FilterByFields(fields, language);
        //    var expectedToken = JToken.Parse(expected);
        //    Assert.Equal(expectedToken, transformedToken);
        //}

        [Fact]
        public void FilterNullPropertiesTest()
        {
            var actual = @"{ field1: null, field2: 42, field3: { field4: null } }";
            var expected = @"{ field2: 42, field3: {} }";
            var token = JToken.Parse(actual);
            var transformedToken = token.FilterOutNullProperties();
            var expectedToken = JToken.Parse(expected);
            Assert.Equal(expectedToken.ToString(), transformedToken?.ToString());
        }
    }
}
