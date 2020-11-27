using SqlKata;
using System.Runtime.CompilerServices;

namespace Helper
{
    public static class PostgresSQLOrderByBuilder
    {
        /// <summary>
        /// Build Orderby with Seed, if a seed "null" is passed an orderby query with sortfield and direction will be created
        /// </summary>
        /// <param name="orderby"></param>
        /// <param name="seed"></param>
        /// <param name="sortfield"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static string? BuildSeedOrderBy(ref string orderby, string? seed, string sortifseednull)
        {
            string? myseed = seed;

            if (seed != null)
            {
                myseed = Helper.CreateSeed.GetSeed(seed);
                orderby = $"md5(id || '{myseed}')";
            }
            else
            {
                orderby = sortifseednull;
            }

            return myseed;
        }

        public static Query OrderBySeed(this Query query, ref string? seed, string sortifseednull)
        {
            string orderby = "";
            BuildSeedOrderBy(ref orderby, seed, sortifseednull);
            return query.OrderByRaw(orderby);
        }

        public static Query ApplyOrdering(this Query query, ref string? seed, PGGeoSearchResult geosearchresult, string? rawsort)
        {
            switch (geosearchresult, rawsort)
            {
                case (PGGeoSearchResult geosr, _) when geosr.geosearch:
                    return query.GeoSearchFilterAndOrderby(geosr);
                case (_, string raw):
                    string rawOrderBy = RawQueryParser.Transformer.TransformSort(raw);
                    return query.OrderByRaw(rawOrderBy);
                default:
                    return query.OrderBySeed(ref seed, "data#>>'\\{Shortname\\}' ASC");
            }
        }
    }
}
