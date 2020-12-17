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

        public static Query ApplyOrdering(this Query query, ref string? seed, PGGeoSearchResult geosearchresult, string? rawsort, string? overwritestandardorder = null) =>
            (geosearchresult, rawsort) switch
            {
                (PGGeoSearchResult geosr, _) when geosr.geosearch =>
                    query.GeoSearchFilterAndOrderby(geosr),
                (_, string raw) =>
                    query.OrderByRaw(RawQueryParser.Transformer.TransformSort(raw)),
                _ =>
                    query.OrderBySeed(ref seed, overwritestandardorder != null ? overwritestandardorder : "data#>>'\\{Shortname\\}' ASC")
            };

        public static Query ApplyOrdering(this Query query, PGGeoSearchResult geosearchresult, string? rawsort, string? overwritestandardorder = null) =>
            (geosearchresult, rawsort) switch
            {
                (PGGeoSearchResult geosr, _) when geosr.geosearch =>
                    query.GeoSearchFilterAndOrderby(geosr),
                (_, string raw) =>
                    query.OrderByRaw(RawQueryParser.Transformer.TransformSort(raw)),
                _ =>
                    query.OrderByRaw(overwritestandardorder != null ? overwritestandardorder : "data#>>'\\{Shortname\\}' ASC")
            };        

        public static Query ApplyRawFilter(this Query query, string? rawFilter) =>
            rawFilter != null ? query.WhereRaw(RawQueryParser.Transformer.TransformFilter(rawFilter)) : query;
    }
}
