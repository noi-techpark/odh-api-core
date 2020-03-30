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
        [Obsolete]
        public static async Task<IEnumerable<string>> RetrieveAreaFilterDataAsync(
           IPostGreSQLConnectionFactory connectionFactory, string? areafilter, CancellationToken cancellationToken)
        {
            if (areafilter != null)
            {
                return (await LocationListCreator.CreateActivityAreaListPGAsync(
                    areafilter, connectionFactory, cancellationToken)).ToList();
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }

        public static async Task<IEnumerable<string>> RetrieveLocFilterDataAsync(
            this Query query, IReadOnlyCollection<string> metaregionlist, CancellationToken cancellationToken)
        {
            var data = await query.From("metaregions")
                    .Select("data")
                    .MetaRegionFilter(metaregionlist)
                    .GetAsync<JsonRaw>();
            var mymetaregion = data.Select(raw => JsonConvert.DeserializeObject<MetaRegion>(raw.Value));
            return (from region in mymetaregion
                    where region.TourismvereinIds != null
                    from tid in region.TourismvereinIds
                    select tid).Distinct().ToList();
        }

        [Obsolete]
        public static async IAsyncEnumerable<string> RetrieveLocFilterDataAsync(
            IPostGreSQLConnectionFactory connectionFactory, List<string> metaregionlist, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var mtapgwhere = PostgresSQLWhereBuilder.CreateMetaRegionWhereExpression(metaregionlist);
            var mymetaregion = PostgresSQLHelper.SelectFromTableDataAsObjectParametrizedAsync<MetaRegion>(
                connectionFactory, "metaregions", "*", mtapgwhere,
                "", 0, null, cancellationToken);

            await foreach (var region in mymetaregion)
                if (region.TourismvereinIds != null)
                    foreach (var tids in region.TourismvereinIds)
                        yield return tids;
        }
    }
}
