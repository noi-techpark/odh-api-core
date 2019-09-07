using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Helper;

namespace OdhApiCoreTests.Helper
{
    public class PostgresSQLOrderByBuilderTests
    {
        [Fact]
        public void BuildSeedOrderBy_WithSeed()
        {
            string orderby = "";
            PostgresSQLOrderByBuilder.BuildSeedOrderBy(ref orderby, "100", "");
            Assert.Equal("md5(id || '10')", orderby);

            orderby = "";
            PostgresSQLOrderByBuilder.BuildSeedOrderBy(ref orderby, "42", "");
            Assert.Equal("md5(id || '42')", orderby);

            orderby = "";
            PostgresSQLOrderByBuilder.BuildSeedOrderBy(ref orderby, "83", "");
            Assert.Equal("md5(id || '8')", orderby);
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
