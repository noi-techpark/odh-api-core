// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Helper;
using Xunit;

namespace OdhApiCoreTests.Helper
{
    public class PostgresSQLOrderByBuilderTests
    {
        [Theory]
        [InlineData("100", "10")]
        [InlineData("42", "42")]
        [InlineData("83", "8")]
        public void BuildSeedOrderBy_WithSeed(string? seed, string resultSeed)
        {
            string orderby = "";
            PostgresSQLOrderByBuilder.BuildSeedOrderBy(ref orderby, ref seed, "");
            Assert.Equal($"md5(id || '{resultSeed}')", orderby);
        }

        [Fact]
        public void BuildSeedOrderBy_WithoutSeed()
        {
            string orderby = "";
            string? nullseed = null;

            PostgresSQLOrderByBuilder.BuildSeedOrderBy(ref orderby, ref nullseed, "orderbyclause");
            Assert.Equal("orderbyclause", orderby);
        }

        [Fact]
        public void BuildSeedOrderBy_WithInvalidSeed()
        {
            string orderby = "";
            string? invalidseed = "invalidnumber";

            PostgresSQLOrderByBuilder.BuildSeedOrderBy(
                ref orderby,
                ref invalidseed,
                "orderbyclause"
            );
            // CHECK: Is this the correct behavior?
            Assert.Equal("md5(id || '')", orderby);
        }
    }
}
