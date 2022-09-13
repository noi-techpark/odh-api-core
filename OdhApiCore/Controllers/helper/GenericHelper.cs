using DataModel;
using Helper;
using Newtonsoft.Json;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    public static class GenericHelper
    {
        public static async Task<IEnumerable<string>> RetrieveAreaFilterDataAsync(
           QueryFactory queryFactory, string? areafilter, CancellationToken cancellationToken)
        {
            if (areafilter != null)
            {
                return (await LocationListCreator.CreateActivityAreaListPGAsync(
                    queryFactory, areafilter, cancellationToken)).ToList();
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }

        public static async Task<IEnumerable<string>> RetrieveLocFilterDataAsync(
            QueryFactory queryFactory, IReadOnlyCollection<string> metaregionlist, CancellationToken cancellationToken)
        {
            var data = await queryFactory.Query()
                    .From("metaregions")
                    .Select("data")
                    .MetaRegionFilter(metaregionlist)
                    .GetAsync<JsonRaw>();
            var mymetaregion = data.Select(raw => JsonConvert.DeserializeObject<MetaRegion>(raw.Value));
            return (from region in mymetaregion
                    where region.TourismvereinIds != null
                    from tid in region.TourismvereinIds ?? Enumerable.Empty<string>()
                    select tid).Distinct().ToList();
        }

        #region Tag Filter

        public static IDictionary<string,IDictionary<string,string>> RetrieveTagFilter(string? tagfilter)
        {            
            try
            {
                if (tagfilter == null)
                    return new Dictionary<string, IDictionary<string, string>>();

                var tagstofilter = new Dictionary<string, IDictionary<string, string>>();

                //Examples
                //tagfilter = and(idm.Winter,idm.Sommer)
                //tagfilter = or(lts.Winter,lts.Sommer)
                //tagfilter = or(lts.Winter,idm.Sommer)and(lts.Winter,idm.Sommer) not wokring at the moment

                //Get Tagoperator
                string tagoperator = tagfilter.Split('(').First();

                //Get data inside brackets
                var bracketdatalist = GetSubStrings(tagfilter, "(", ")");

                foreach(var bracketdata in bracketdatalist)
                {
                    var splittedelements = bracketdata.Split(",");

                    var splitdict = new Dictionary<string, string>();

                    foreach (var splittedelement in splittedelements)
                    {
                        var splittedtag = splittedelement.Split(".");
                        if (splittedtag.Length > 1)
                        {
                            splitdict.Add(splittedtag[1], splittedtag[0]);                         
                        }
                    }

                    tagstofilter.Add(tagoperator, splitdict);
                }



                return tagstofilter;
            }
            catch (Exception)
            {
                return new Dictionary<string, IDictionary<string, string>>();
            }
        }

        private static IEnumerable<string> GetSubStrings(string input, string start, string end)
        {
            Regex r = new Regex(Regex.Escape(start) + "(.*?)" + Regex.Escape(end));
            MatchCollection matches = r.Matches(input);
            foreach (Match match in matches)
                yield return match.Groups[1].Value;
        }

        #endregion
    }
}
