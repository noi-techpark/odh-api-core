using Xunit;
using Helper;

namespace OdhApiCoreTests.Helper
{
    public class PostgresSQLOrderByBuilderTests
    {
        [Theory]
        [InlineData("100", "10")]
        [InlineData("42", "42")]
        [InlineData("83", "8")]
        public void BuildSeedOrderBy_WithSeed(string seed, string resultSeed)
        {
            string orderby = "";
            PostgresSQLOrderByBuilder.BuildSeedOrderBy(ref orderby, seed, "");
            Assert.Equal($"md5(id || '{resultSeed}')", orderby);
        }

        [Fact]
        public void BuildSeedOrderBy_WithoutSeed()
        {
            string orderby = "";
            PostgresSQLOrderByBuilder.BuildSeedOrderBy(ref orderby, null, "orderbyclause");
            Assert.Equal("orderbyclause", orderby);
        }

        [Fact]
        public void BuildSeedOrderBy_WithInvalidSeed()
        {
            string orderby = "";
            PostgresSQLOrderByBuilder.BuildSeedOrderBy(ref orderby, "invalidnumber", "orderbyclause");
            // CHECK: Is this the correct behavior?
            Assert.Equal("md5(id || '')", orderby);
        }
    }
}
