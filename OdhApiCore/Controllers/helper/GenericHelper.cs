using DataModel;
using Helper;
using Newtonsoft.Json;
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
                    from tid in region.TourismvereinIds
                    select tid).Distinct().ToList();
        }
    }
}
