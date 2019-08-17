﻿using Helper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    public class ActivityHelper
    {
        public List<string> activitytypelist;
        public List<string> subtypelist;
        public List<string> idlist;
        public List<string> arealist;
        public List<string> smgtaglist;
        public List<string> difficultylist;
        public List<string> tourismvereinlist;
        public List<string> regionlist;
        public bool distance;
        public int distancemin;
        public int distancemax;
        public bool altitude;
        public int altitudemin;
        public int altitudemax;
        public bool duration;
        public int durationmin;
        public int durationmax;
        public bool? highlight;
        public bool? active;
        public bool? smgactive;

        public static async Task<ActivityHelper> CreateAsync(string connectionString, string? activitytype, string? subtypefilter, string? idfilter, string? locfilter, string? areafilter, string? distancefilter, string? altitudefilter, string? durationfilter, string? highlightfilter, string? difficultyfilter, string? activefilter, string? smgactivefilter, string? smgtags, CancellationToken cancellationToken)
        {
            var arealist = await RetrieveAreaFilterDataAsync(connectionString, areafilter, cancellationToken);

            IEnumerable<string>? tourismusvereinids = null;
            if (locfilter != null && locfilter.Contains("mta"))
            {
                List<string> metaregionlist = CommonListCreator.CreateDistrictIdList(locfilter, "mta");
                tourismusvereinids = await RetrieveLocFilterDataAsync(connectionString, metaregionlist, cancellationToken);
            }

            return new ActivityHelper(activitytype, subtypefilter, idfilter, locfilter, arealist, distancefilter, altitudefilter, durationfilter, highlightfilter, difficultyfilter, activefilter, smgactivefilter, smgtags, tourismusvereinids);
        }

        private ActivityHelper(string? activitytype, string? subtypefilter, string? idfilter, string? locfilter, IEnumerable<string> arealist, string? distancefilter, string? altitudefilter, string? durationfilter, string? highlightfilter, string? difficultyfilter, string? activefilter, string? smgactivefilter, string? smgtags, IEnumerable<string>? tourismusvereinids)
        {
            activitytypelist = new List<string>();
            int typeinteger = 0;

            if (activitytype != null)
            {
                if (int.TryParse(activitytype, out typeinteger))
                {
                    if (typeinteger != 1023)
                        activitytypelist = Helper.ActivityPoiListCreator.CreateActivityTypefromFlag(activitytype);
                }
                else
                {
                    activitytypelist.Add(activitytype);
                }
            }
            if (activitytypelist.Count > 0)
                subtypelist = Helper.ActivityPoiListCreator.CreateActivitySubTypefromFlag(activitytypelist.FirstOrDefault(), subtypefilter);
            else
                subtypelist = new List<string>();

            idlist = Helper.CommonListCreator.CreateIdList(idfilter?.ToUpper());

            this.arealist = arealist.ToList();

            smgtaglist = CommonListCreator.CreateIdList(smgtags);
            difficultylist = CommonListCreator.CreateDifficultyList(difficultyfilter, activitytype);

            tourismvereinlist = new List<string>();
            regionlist = new List<string>();
            if (locfilter != null && locfilter.Contains("reg"))
                regionlist = CommonListCreator.CreateDistrictIdList(locfilter, "reg");
            if (locfilter != null && locfilter.Contains("tvs"))
                tourismvereinlist = CommonListCreator.CreateDistrictIdList(locfilter, "tvs");

            if (tourismusvereinids != null)
                tourismvereinlist.AddRange(tourismusvereinids);

            //            //Sonderfall für MetaRegion hole mir alle DistrictIds dieser MetaRegion
            //            if (locfilter != null && locfilter.Contains("mta"))
            //            {
            //                List<string> metaregionlist = CommonListCreator.CreateDistrictIdList(locfilter, "mta");
            //
            //                // FIXME: do not call in constructor
            //                //var tourismusvereinids = RetrieveLocFilterDataAsync(metaregionlist, connectionString).Result;
            //                tourismvereinlist.AddRange(tourismusvereinids);
            //            }

            //Distance
            distance = false;
            distancemin = 0;
            distancemax = 0;
            var distancefilterresult = CommonListCreator.CreateRangeString(distancefilter);
            if (distancefilterresult != null)
            {
                distance = true;
                distancemin = distancefilterresult.Item1 * 1000;
                distancemax = distancefilterresult.Item2 * 1000;
            }

            //Altitude
            altitude = false;
            altitudemin = 0;
            altitudemax = 0;
            var altitudefilterresult = CommonListCreator.CreateRangeString(altitudefilter);
            if (altitudefilterresult != null)
            {
                altitude = true;
                altitudemin = altitudefilterresult.Item1;
                altitudemax = altitudefilterresult.Item2;
            }

            //Duration
            duration = false;
            durationmin = 0;
            durationmax = 0;
            var durationfilterresult = CommonListCreator.CreateRangeString(durationfilter);
            if (durationfilterresult != null)
            {
                duration = true;
                durationmin = durationfilterresult.Item1;
                durationmax = durationfilterresult.Item2;
            }

            //highlight
            highlight = null;
            if (highlightfilter == "true")
                highlight = true;
            if (highlightfilter == "false")
                highlight = false;

            //active
            active = null;
            if (activefilter == "true")
                active = true;
            if (activefilter == "false")
                active = false;

            //smgactive
            smgactive = null;
            if (smgactivefilter == "true")
                smgactive = true;
            if (smgactivefilter == "false")
                smgactive = false;
        }

        private static async Task<IEnumerable<string>> RetrieveAreaFilterDataAsync(string connectionString, string? areafilter, CancellationToken cancellationToken)
        {
            if (areafilter != null)
            {
                return (await LocationListCreator.CreateActivityAreaListPGAsync(areafilter, connectionString, cancellationToken)).ToList();
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }

        private static async Task<IEnumerable<string>> RetrieveLocFilterDataAsync(string connectionString, List<string> metaregionlist, CancellationToken cancellationToken)
        {
            var mtapgwhere = PostgresSQLWhereBuilder.CreateMetaRegionWhereExpression(metaregionlist);
            var mymetaregion = await PostgresSQLHelper.SelectFromTableDataAsObjectParametrizedAsync<MetaRegion>(connectionString, "metaregions", "*", mtapgwhere.Item1, mtapgwhere.Item2, "", 0, null, cancellationToken);

            return mymetaregion.SelectMany(x => x.TourismvereinIds).ToList();
        }
    }
}
