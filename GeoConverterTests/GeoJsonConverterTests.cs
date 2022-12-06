using GeoConverter;
using System.Reflection;

namespace GeoConverterTests
{
    public class GeoJsonConverterTests
    {
        [Theory]
        [InlineData("216051")]
        [InlineData("216052")]
        [InlineData("216053")]
        public void TestConversion(string input)
        {
            string kml = GetContent(input, "kml");
            var actual = GeoJsonConverter.ConvertFromKml(kml);
            string expected = GetContent(input ,"json");
            Assert.Equal(expected, actual);
        }

        private static string GetContent(string input, string extension)
        {
            return File.ReadAllText(Path.Combine("Files", Path.ChangeExtension(input, extension)));
        }
    }
}