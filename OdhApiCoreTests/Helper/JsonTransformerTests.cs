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
    }
}
