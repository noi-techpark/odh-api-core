using Helper;
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
