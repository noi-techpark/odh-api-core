using Helper;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    public class EventHelper
    {
        public List<string> idlist;
        public List<string> orgidlist;
        public List<string> rancidlist;
        public List<string> typeidlist;
        public List<string> topicrids;
        public List<string> smgtaglist;
        public List<string> districtlist;
        public List<string> municipalitylist;
        public List<string> tourismvereinlist;
        public List<string> regionlist;
        public List<string> languagelist;
        public List<string> sourcelist;
        public bool? active;
        public bool? smgactive;
        public DateTime? begin;
        public DateTime? end;
        public string? lastchange;

        public static async Task<EventHelper> CreateAsync(
            QueryFactory queryFactory, string? idfilter, string? locfilter, string? rancfilter,
            string? typefilter, string? topicfilter, string? orgfilter, string? begindate, string? enddate,
            bool? activefilter, bool? smgactivefilter, string? smgtags, string? lastchange, string? langfilter, string? source,
            CancellationToken cancellationToken)
        {
            IEnumerable<string>? tourismusvereinids = null;
            if (locfilter != null && locfilter.Contains("mta"))
            {
                List<string> metaregionlist = CommonListCreator.CreateDistrictIdList(locfilter, "mta");
                tourismusvereinids = await GenericHelper.RetrieveLocFilterDataAsync(queryFactory, metaregionlist, cancellationToken);
            }

            return new EventHelper(
                idfilter: idfilter, locfilter: locfilter, rancfilter: rancfilter, typefilter: typefilter,
                topicfilter: topicfilter, orgfilter: orgfilter, begindate: begindate, enddate: enddate,
                activefilter: activefilter, smgactivefilter: smgactivefilter, smgtags: smgtags, lastchange: lastchange, sourcefilter: source, languagefilter: langfilter,
                tourismusvereinids: tourismusvereinids);
        }

        private EventHelper(
            string? idfilter, string? locfilter, string? rancfilter,
            string? typefilter, string? topicfilter, string? orgfilter,
            string? begindate, string? enddate, bool? activefilter, bool? smgactivefilter,
            string? smgtags, string? lastchange, string? languagefilter, string? sourcefilter, IEnumerable<string>? tourismusvereinids)
        {
            idlist = CommonListCreator.CreateIdList(idfilter?.ToUpper());

            smgtaglist = CommonListCreator.CreateIdList(smgtags);
            sourcelist = Helper.CommonListCreator.CreateSmgPoiSourceList(sourcefilter);
            languagelist = Helper.CommonListCreator.CreateIdList(languagefilter);

            orgidlist = CommonListCreator.CreateIdList(orgfilter);
            rancidlist = CommonListCreator.CreateIdList(rancfilter);
            typeidlist = CommonListCreator.CreateIdList(typefilter);
            topicrids = EventListCreator.CreateEventTopicRidListfromFlag(topicfilter);

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

            //smgactive
            smgactive = smgactivefilter;

            this.lastchange = lastchange;

            begin = DateTime.MinValue;
            end = DateTime.MaxValue;

            if (!String.IsNullOrEmpty(begindate))
                if (begindate != "null")
                    begin = Convert.ToDateTime(begindate);

            if (!String.IsNullOrEmpty(enddate))
                if (enddate != "null")
                    end = Convert.ToDateTime(enddate);

        }


    }
}
