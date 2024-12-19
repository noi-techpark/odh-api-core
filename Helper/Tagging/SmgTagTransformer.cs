// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using DataModel;

namespace Helper
{
    public static class SmgTagTransformer
    {
        public static IEnumerable<SmgTags> TransformToLocalizedSmgTag(
            this IEnumerable<SmgTags> smgtaglist,
            string language
        )
        {
            return (
                from smgtag in smgtaglist
                select new SmgTags
                {
                    Id = smgtag.Id,
                    Shortname = smgtag.Shortname,
                    MainEntity = smgtag.MainEntity,
                    ValidForEntity = smgtag.ValidForEntity,
                    TagName = smgtag
                        .TagName.Where(x => x.Key == language)
                        .ToDictionary(x => x.Key, x => x.Value),
                }
            );
        }

        public static SmgTags TransformToLocalizedSmgTag(this SmgTags smgtag, string language)
        {
            return new SmgTags
            {
                Id = smgtag.Id,
                Shortname = smgtag.Shortname,
                MainEntity = smgtag.MainEntity,
                ValidForEntity = smgtag.ValidForEntity,
                TagName = smgtag
                    .TagName.Where(x => x.Key == language)
                    .ToDictionary(x => x.Key, x => x.Value),
            };
        }

        public static IEnumerable<SmgTagReduced> TransformToReducedSmgTag(
            this IEnumerable<SmgTags> smgtaglist,
            string language
        )
        {
            return (
                from smgtag in smgtaglist
                select new SmgTagReduced { Id = smgtag.Id, Name = smgtag.TagName[language] }
            );
        }
    }
}
