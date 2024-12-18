// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DataModel;
using Helper;
using Newtonsoft.Json;
using SqlKata.Execution;

namespace OdhApiCore.Controllers
{
    public static class GenericHelper
    {
        public static async Task<IEnumerable<string>> RetrieveAreaFilterDataAsync(
            QueryFactory queryFactory,
            string? areafilter,
            CancellationToken cancellationToken
        )
        {
            if (areafilter != null)
            {
                return (
                    await LocationListCreator.CreateActivityAreaListPGAsync(
                        queryFactory,
                        areafilter,
                        cancellationToken
                    )
                ).ToList();
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }

        public static async Task<IEnumerable<string>> RetrieveLocFilterDataAsync(
            QueryFactory queryFactory,
            IReadOnlyCollection<string> metaregionlist,
            CancellationToken cancellationToken
        )
        {
            var data = await queryFactory
                .Query()
                .From("metaregions")
                .Select("data")
                .MetaRegionFilter(metaregionlist)
                .GetAsync<JsonRaw>();
            var mymetaregion = data.Select(raw =>
                JsonConvert.DeserializeObject<MetaRegion>(raw.Value)
            );
            return (
                from region in mymetaregion
                where region.TourismvereinIds != null
                from tid in region.TourismvereinIds ?? Enumerable.Empty<string>()
                select tid
            )
                .Distinct()
                .ToList();
        }

        #region Tag Filter

        public static IDictionary<string, List<string>> RetrieveTagFilter(string? tagfilter)
        {
            try
            {
                if (tagfilter == null)
                    return new Dictionary<string, List<string>>();

                var tagstofilter = new Dictionary<string, List<string>>();

                //Examples
                //tagfilter = and(idm.Winter,idm.Sommer)
                //tagfilter = or(lts.Winter,lts.Sommer)
                //tagfilter = or(lts.Winter,Sommer,idm.Wellness)

                //TODO
                //tagfilter = or(lts.Winter,idm.Sommer)and(lts.Winter,idm.Sommer) not working at the moment
                //tagfilter = or(winter) searches trough lts and

                //Get Tagoperator
                char[] splitParams = new char[] { '(', ')' };
                string[] tagoperators = tagfilter.ToLower().Split(splitParams);

                //TODO and or combination
                string currentoperator = "";

                foreach (string tagoperator in tagoperators)
                {
                    if (tagoperator.Equals("and") || tagoperator.Equals("or"))
                    {
                        currentoperator = tagoperator;
                    }
                    else if (!String.IsNullOrEmpty(tagoperator))
                    {
                        var splittedelements = tagoperator.Split(",");
                        tagstofilter.Add(currentoperator, splittedelements.ToList());
                    }
                }

                //int i = 0;

                //foreach(string tagoperator in tagoperators)
                //{
                //    if (tagoperator.Equals("and") || tagoperator.Equals("or"))
                //    {
                //        //Get data inside brackets
                //        var bracketdatalist = GetSubStrings(tagfilter, "(", ")");

                //        var bracketdataarr = bracketdatalist.ToArray();

                //        if (bracketdataarr[i] != null)
                //        {
                //            var splittedelements = bracketdataarr[i].Split(",");

                //            tagstofilter.Add(tagoperator, splittedelements.ToList());

                //            i++;
                //        }
                //    }
                //}

                return tagstofilter;
            }
            catch (Exception)
            {
                return new Dictionary<string, List<string>>();
            }
        }

        private static IEnumerable<string> GetSubStrings(string input, string start, string end)
        {
            Regex r = new Regex(Regex.Escape(start) + "(.*?)" + Regex.Escape(end));
            MatchCollection matches = r.Matches(input);
            foreach (Match match in matches)
                yield return match.Groups[1].Value;
        }

        private static IEnumerable<string> GetOperators(string input, string start, string end)
        {
            Regex r = new Regex(Regex.Escape(start) + "^(.*?)" + Regex.Escape(end));
            MatchCollection matches = r.Matches(input);
            foreach (Match match in matches)
                yield return match.Groups[1].Value;
        }

        #endregion
    }
}
