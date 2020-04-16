using Helper;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.api
{
    public class ODHActivityPoiHelper
    {
        public List<string> typelist;
        public List<string> poitypelist;
        public List<string> subtypelist;
        public List<string> idlist;
        public List<string> arealist;
        public List<string> smgtaglist;
        public List<string> sourcelist;
        public List<string> languagelist;
        public List<string> districtlist;
        public List<string> municipalitylist;
        public List<string> tourismvereinlist;
        public List<string> regionlist;
        public bool? highlight;
        public bool? active;
        public bool? smgactive;
        public string? lastchange;
    
        public static async Task<ODHActivityPoiHelper> CreateAsync(
        IPostGreSQLConnectionFactory connectionFactory, string? typefilter, string? subtypefilter, string? poitypefilter, string? idfilter, string? locfilter,
        string? areafilter, string? languagefilter, string? sourcefilter, bool? highlightfilter, bool? activefilter, bool? smgactivefilter,
        string? smgtags, string? lastchange, CancellationToken cancellationToken, QueryFactory queryFactory)
        {
            var arealist = await GenericHelper.RetrieveAreaFilterDataAsync(queryFactory, areafilter, cancellationToken);

            IEnumerable<string>? tourismusvereinids = null;
            if (locfilter != null && locfilter.Contains("mta"))
            {
                List<string> metaregionlist = CommonListCreator.CreateDistrictIdList(locfilter, "mta");
                tourismusvereinids = await GenericHelper.RetrieveLocFilterDataAsync(queryFactory, metaregionlist, cancellationToken);
            }

            return new ODHActivityPoiHelper(typefilter, subtypefilter, poitypefilter, idfilter, locfilter, arealist, languagefilter, sourcefilter, highlightfilter, activefilter, smgactivefilter, smgtags, lastchange, tourismusvereinids);
        }

        private ODHActivityPoiHelper(
            string? typefilter, string? subtypefilter, string? poitypefilter, string? idfilter, string? locfilter, IEnumerable<string> arealist, string? languagefilter, string? sourcefilter,
            bool? highlightfilter, bool? activefilter, bool? smgactivefilter, string? smgtags, string? lastchange, IEnumerable<string>? tourismusvereinids)
        {
            typelist = new List<string>();
            int typeinteger = 0;

            if (!String.IsNullOrEmpty(typefilter))
            {
                if (int.TryParse(typefilter, out typeinteger))
                {
                    //Sonderfall wenn alles abgefragt wird um keine unnötige Where zu erzeugen
                    if (typeinteger != 63)
                        typelist = Helper.ActivityPoiListCreator.CreateSmgPoiTypefromFlag(typefilter);
                }
                else
                {
                    if (typefilter.Contains(","))
                    {
                        typelist = typefilter.Split(',').ToList();
                    }
                    else if (typefilter != "null")
                        typelist.Add(typefilter);
                }
            }


            if (typelist.Count > 0)
                subtypelist = Helper.ActivityPoiListCreator.CreateSmgPoiSubTypefromFlag(typelist.FirstOrDefault(), subtypefilter);
            else
                subtypelist = new List<string>();

            if (subtypelist.Count > 0)
                poitypelist = Helper.ActivityPoiListCreator.CreateSmgPoiPoiTypefromFlag(subtypelist.FirstOrDefault(), poitypefilter);
            else
                poitypelist = new List<string>();


            if (poitypelist.Count > 0)
                subtypelist = Helper.ActivityPoiListCreator.CreatePoiSubTypefromFlag(poitypelist.FirstOrDefault(), subtypefilter);
            else
                subtypelist = new List<string>();


            idlist = Helper.CommonListCreator.CreateIdList(idfilter?.ToUpper());
            sourcelist = Helper.CommonListCreator.CreateSmgPoiSourceList(sourcefilter);
            languagelist = Helper.CommonListCreator.CreateIdList(languagefilter);

            this.arealist = arealist.ToList();

            smgtaglist = Helper.CommonListCreator.CreateIdList(smgtags);

            tourismvereinlist = new List<string>();
            regionlist = new List<string>();
            municipalitylist = new List<string>();
            districtlist = new List<string>();

            if (locfilter != null && locfilter.Contains("reg"))
                regionlist = Helper.CommonListCreator.CreateDistrictIdList(locfilter, "reg");
            if (locfilter != null && locfilter.Contains("tvs"))
                tourismvereinlist = Helper.CommonListCreator.CreateDistrictIdList(locfilter, "tvs");
            if (locfilter != null && locfilter.Contains("mun"))
                municipalitylist = Helper.CommonListCreator.CreateDistrictIdList(locfilter, "mun");
            if (locfilter != null && locfilter.Contains("fra"))
                districtlist = Helper.CommonListCreator.CreateDistrictIdList(locfilter, "fra");

            if (tourismusvereinids != null)
                tourismvereinlist.AddRange(tourismusvereinids);

            //highlight
            highlight = highlightfilter;
            //active
            active = activefilter;           
            //smgactive
            smgactive = smgactivefilter;

            this.lastchange = lastchange;
        }
    }
}
