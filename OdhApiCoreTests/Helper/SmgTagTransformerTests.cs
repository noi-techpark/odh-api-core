using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Helper;
using DataModel;

namespace OdhApiCoreTests.Helper
{
    public class SmgTagTransformerTests
    {
        public static TheoryData<string> GetLanguages()
        {
            return new TheoryData<string> { "de", "it", "en", "ru" };
        }

        [Theory]
        [MemberData(nameof(GetLanguages))]
        public void TransformToLocalizedSmgTag_EmptySmgTags(string language)
        {
            var smgtagslist = Enumerable.Empty<SmgTags>();
            IEnumerable<SmgTags> result = SmgTagTransformer.TransformToLocalizedSmgTag(
                smgtagslist,
                language
            );
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Theory]
        [MemberData(nameof(GetLanguages))]
        public void TransformToLocalizedSmgTag_SingleSmgTag(string language)
        {
            var expectedSmgTag = new SmgTags
            {
                Id = Guid.NewGuid().ToString(),
                Shortname = "shortname",
                TagName = new Dictionary<string, string>
                {
                    { "de", "tagname" },
                    { "it", "tagname" }
                },
                ValidForEntity = Array.Empty<string>()
            };
            var smgtagslist = new SmgTags[] { expectedSmgTag };
            IEnumerable<SmgTags> result = SmgTagTransformer.TransformToLocalizedSmgTag(
                smgtagslist,
                language
            );
            Assert.NotNull(result);
            var smgtag = Assert.Single(result);
            Assert.Equal(expectedSmgTag.Id, smgtag.Id);
            Assert.Equal(expectedSmgTag.Shortname, smgtag.Shortname);
            Assert.Equal(expectedSmgTag.ValidForEntity, smgtag.ValidForEntity);
            if (expectedSmgTag.TagName.ContainsKey(language))
                Assert.Single(smgtag.TagName);
            else
                Assert.Empty(smgtag.TagName);
        }
    }
}
