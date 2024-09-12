// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Helper;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    public class WineHelper
    {
        public List<string> idlist;
        public List<string> companyidlist;
        public List<string> wineidlist;
        public List<string> languagelist;
        public List<string> sourcelist;
        public List<string> smgtaglist;
        public bool? visibleinsearch;
        public string? lastchange;
        public bool? active;
        public bool? smgactive;

        public static Task<WineHelper> CreateAsync(
            QueryFactory queryFactory, string? idfilter, string? companyidfilter, string? wineidfilter, string? languagefilter, bool? visibleinsearch, string? sourcefilter,
            bool? activefilter, bool? smgactivefilter, string? smgtags, string? lastchange,
            CancellationToken cancellationToken)
        {           
            return Task.FromResult(new WineHelper(
               idfilter: idfilter, languagefilter: languagefilter, companyidfilter: companyidfilter, wineidfilter: wineidfilter,
                activefilter: activefilter, smgactivefilter: smgactivefilter, visibleinsearch: visibleinsearch, sourcefilter: sourcefilter, smgtags: smgtags, lastchange: lastchange));
        }

        private WineHelper(
            string? idfilter, string? languagefilter, string? companyidfilter, string? wineidfilter, bool? activefilter, bool? smgactivefilter, bool? visibleinsearch, string? sourcefilter, string? smgtags, string? lastchange)
        {           
            idlist = Helper.CommonListCreator.CreateIdList(idfilter?.ToUpper());
            companyidlist = Helper.CommonListCreator.CreateIdList(companyidfilter?.ToLower());
            wineidlist = Helper.CommonListCreator.CreateIdList(wineidfilter?.ToLower());

            languagelist = Helper.CommonListCreator.CreateIdList(languagefilter?.ToLower());
            sourcelist = Helper.CommonListCreator.CreateIdList(sourcefilter);

            smgtaglist = CommonListCreator.CreateIdList(smgtags);
            //active
            active = activefilter;

            //smgactive
            smgactive = smgactivefilter;

            //smgactive
            this.visibleinsearch = visibleinsearch;

            this.lastchange = lastchange;
        }


    }
}
