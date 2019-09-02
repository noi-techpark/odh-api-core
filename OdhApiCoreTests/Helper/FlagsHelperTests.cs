using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Helper;

namespace OdhApiCoreTests.Helper
{
    public class FlagsHelperTests
    {
        [Fact]
        public void GetFlags_ValidEnum()
        {
            var @enum = ActivityTypeBerg.Bergtouren | ActivityTypeBerg.Schneeschuhtouren;
            var flags = @enum.GetFlags();
            Assert.Equal(2, flags.Count());
            Assert.Contains(ActivityTypeBerg.Bergtouren, flags);
            Assert.Contains(ActivityTypeBerg.Schneeschuhtouren, flags);
        }

        [Fact]
        public void GetFlags_InvalidEnumType()
        {
            var @enum = 12333;
            Assert.Throws<ArgumentException>("withFlags", () =>
            {
                var flags = @enum.GetFlags().ToList();
            });
        }

        [Fact]
        public void GetFlags_InvalidNonFlaggedEnum()
        {
            var @enum = DayOfWeek.Monday;
            Assert.Throws<ArgumentException>("withFlags", () =>
            {
                var flags = @enum.GetFlags().ToList();
            });
        }

        [Fact]
        public void SetFlags_Test()
        {
            var @enum = ActivityTypeBerg.Bergtouren | ActivityTypeBerg.Schneeschuhtouren;
            var newenum = @enum.SetFlags(ActivityTypeBerg.Hochtouren);
            var flags = newenum.GetFlags();
            Assert.Equal(3, flags.Count());
            Assert.True(newenum.HasFlag(ActivityTypeBerg.Hochtouren));

            // Functionally equivalent
            Assert.Equal(@enum | ActivityTypeBerg.Hochtouren, @enum.SetFlags(ActivityTypeBerg.Hochtouren));
        }

        [Fact]
        public void SetFlags_TestWithOnFalse()
        {
            var @enum = ActivityTypeBerg.Bergtouren | ActivityTypeBerg.Schneeschuhtouren | ActivityTypeBerg.Hochtouren;

            // Attention, not functionally equivalent!
            //Assert.Equal(@enum & ActivityTypeBerg.Bergtouren, @enum.SetFlags(ActivityTypeBerg.Bergtouren, false));

            var newenum2 = @enum.SetFlags(ActivityTypeBerg.Bergtouren, false);
            Assert.False(newenum2.HasFlag(ActivityTypeBerg.Bergtouren));
            Assert.True(newenum2.HasFlag(ActivityTypeBerg.Schneeschuhtouren));
            Assert.True(newenum2.HasFlag(ActivityTypeBerg.Hochtouren));
            Assert.Equal(2, newenum2.GetFlags().Count());
        }

        [Fact]
        public void IsFlagSet_Test()
        {
            var @enum = ActivityTypeBerg.Bergtouren | ActivityTypeBerg.Schneeschuhtouren;
            // Functionally equivalent
            Assert.Equal(@enum.HasFlag(ActivityTypeBerg.Alpinklettern), @enum.IsFlagSet(ActivityTypeBerg.Alpinklettern));
        }
    }
}
