// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Geo.Measure;
using Helper;
using SqlKata;
using SqlKata.Execution;

namespace OdhApiCore.Controllers
{
    public class EventV2Helper
    {
        public List<string> idlist;
        public List<string> venueidlist;
        public List<string> districtlist;
        public List<string> municipalitylist;
        public List<string> tourismvereinlist;
        public List<string> regionlist;
        public List<string> languagelist;
        public List<string> sourcelist;
        public bool? active;
        public DateTime? begin;
        public DateTime? end;
        public string? lastchange;

        //New Publishedonlist
        public List<string> publishedonlist;
        public IDictionary<string, List<string>> tagdict;

        public static async Task<EventV2Helper> CreateAsync(
            QueryFactory queryFactory,
            string? idfilter,
            string? venueidfilter,
            string? locfilter,
            string? tagfilter,
            string? begindate,
            string? enddate,
            bool? activefilter,
            string? lastchange,
            string? langfilter,
            string? source,
            string? publishedonfilter,
            CancellationToken cancellationToken
        )
        {
            IEnumerable<string>? tourismusvereinids = null;
            if (locfilter != null && locfilter.Contains("mta"))
            {
                List<string> metaregionlist = CommonListCreator.CreateDistrictIdList(
                    locfilter,
                    "mta"
                );
                tourismusvereinids = await GenericHelper.RetrieveLocFilterDataAsync(
                    queryFactory,
                    metaregionlist,
                    cancellationToken
                );
            }

            return new EventV2Helper(
                idfilter: idfilter,
                venueidfilter: venueidfilter,
                locfilter: locfilter,
                tagfilter: tagfilter,
                begindate: begindate,
                enddate: enddate,
                activefilter: activefilter,
                lastchange: lastchange,
                sourcefilter: source,
                languagefilter: langfilter,
                publishedonfilter: publishedonfilter,
                tourismusvereinids: tourismusvereinids
            );
        }

        private EventV2Helper(
            string? idfilter,
            string? venueidfilter,
            string? locfilter,
            string? tagfilter,
            string? begindate,
            string? enddate,
            bool? activefilter,
            string? lastchange,
            string? languagefilter,
            string? sourcefilter,
            string? publishedonfilter,
            IEnumerable<string>? tourismusvereinids
        )
        {
            idlist = CommonListCreator.CreateIdList(idfilter?.ToUpper());
            venueidlist = CommonListCreator.CreateIdList(venueidfilter?.ToUpper());

            sourcelist = Helper.CommonListCreator.CreateSmgPoiSourceList(sourcefilter);
            languagelist = Helper.CommonListCreator.CreateIdList(languagefilter);

            tourismvereinlist = new List<string>();
            regionlist = new List<string>();
            municipalitylist = new List<string>();
            districtlist = new List<string>();

            if (locfilter != null && locfilter.Contains("reg"))
                regionlist = CommonListCreator.CreateDistrictIdList(locfilter, "reg");
            if (locfilter != null && locfilter.Contains("tvs"))
                tourismvereinlist = CommonListCreator.CreateDistrictIdList(locfilter, "tvs");
            if (locfilter != null && locfilter.Contains("mun"))
                municipalitylist = CommonListCreator.CreateDistrictIdList(locfilter, "mun");
            if (locfilter != null && locfilter.Contains("fra"))
                districtlist = CommonListCreator.CreateDistrictIdList(locfilter, "fra");

            if (tourismusvereinids != null)
                tourismvereinlist.AddRange(tourismusvereinids);

            //active
            active = activefilter;

            //tagfilter
            tagdict = GenericHelper.RetrieveTagFilter(tagfilter);

            this.lastchange = lastchange;

            begin = DateTime.MinValue;
            end = DateTime.MaxValue;

            if (!String.IsNullOrEmpty(begindate))
                if (begindate != "null")
                    begin = Convert.ToDateTime(begindate);

            if (!String.IsNullOrEmpty(enddate))
                if (enddate != "null")
                    end = Convert.ToDateTime(enddate);

            publishedonlist = Helper.CommonListCreator.CreateIdList(publishedonfilter?.ToLower());
        }
    }
}
