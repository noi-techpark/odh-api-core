using Newtonsoft.Json;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helper
{
    public static class PostgresSQLQueryExtensions
    {
        /// <summary>
        /// Convert a (simple) JsonPath path to a Postgres array,
        /// which can be used in the #>> operator.<br />
        /// E.g. Detail.de.Title => Detail,de,Title
        /// </summary>
        public static string JsonPathToPostgresArray(string field) =>
            field.Replace('.', ',');

        public static Query WhereJsonb(
            this Query query,
            string path,
            string value) =>
                query.WhereRaw(
                    $"data#>>'\\{{{path}\\}}' = ?",
                    value
                );

        public static Query WhereJsonb(
            this Query query,
            string path,
            int value) =>
                query.WhereRaw(
                    $"data#>>'\\{{{path}\\}}' = ?",
                    value.ToString()
                );

        public static Query WhereJsonb(
            this Query query,
            string path,
            bool value) =>
                query.WhereRaw(
                    $"data#>>'\\{{{path}\\}}' = ?",
                    value ? "true" : "false"
                );

        public static Query OrWhereJsonb(
            this Query query,
            string path,
            string value) =>
                query.OrWhereRaw(
                    $"data#>>'\\{{{path}\\}}' = ?",
                    value
                );

        public static Query OrWhereJsonb(
            this Query query,
            string path,
            int value) =>
                query.OrWhereRaw(
                    $"data#>>'\\{{{path}\\}}' = ?",
                    value.ToString()
                );

        public static Query OrWhereJsonb(
            this Query query,
            string path,
            bool value) =>
                query.OrWhereRaw(
                    $"data#>>'\\{{{path}\\}}' = ?",
                    value ? "true" : "false"
                );

        [Obsolete]
        public static Query WhereJsonb<T>(
            this Query query,
            T value,
            Func<T, object> jsonObjectConstructor) =>
                query.WhereRaw(
                    "data @> jsonb(?)",
                    JsonConvert.SerializeObject(
                        jsonObjectConstructor(value)
                    )
                );

        [Obsolete]
        public static Query OrWhereJsonb<T>(
            this Query query,
            T value,
            Func<T, object> jsonObjectConstructor) =>
                query.OrWhereRaw(
                    "data @> jsonb(?)",
                    JsonConvert.SerializeObject(
                        jsonObjectConstructor(value)
                    )
                );

        [Obsolete]
        public static Query WhereInJsonb<T>(
            this Query query,
            IReadOnlyCollection<T> list,
            Func<T, object> jsonObjectConstructor) =>
                query.Where(q =>
                {
                    foreach (var item in list)
                    {
                        q = q.OrWhereJsonb(
                            value: item,
                            jsonObjectConstructor
                        );
                    }
                    return q;
                });

        public static Query WhereInJsonb<T>(
            this Query query,
            IReadOnlyCollection<T> list,
            string path,
            Func<T, string> jsonObjectConstructor) =>
                query.Where(q =>
                {
                    foreach (var item in list)
                    {
                        q = q.OrWhereJsonb(
                            path,
                            jsonObjectConstructor(item)
                        );
                    }
                    return q;
                });

        public static Query WhereInJsonb<T>(
            this Query query,
            IReadOnlyCollection<T> list,
            string jsonPath)
        {
            if (list.Count == 0)
            {
                return query;
            }
            else
            {
                return query.WhereRaw(
                    $"data#>>'\\{{{JsonPathToPostgresArray(jsonPath)}\\}}' = ANY(?)",
                    new [] { new[] { list } }
                );
            }
        }

        public static Query DistrictFilter(this Query query, IReadOnlyCollection<string> districtlist) =>
            query.WhereInJsonb(
                list: districtlist,
                "DistrictId",
                id => id.ToUpper()
            );

        public static Query IdUpperFilter(this Query query, IReadOnlyCollection<string> idlist) =>
            query.Where(q =>
            {
                foreach (var id in idlist)
                {
                    q = q.OrWhere("id", "=", id.ToUpper());
                }
                return q;
            });

        public static Query IdLowerFilter(this Query query, IReadOnlyCollection<string> idlist) =>
            query.Where(q =>
            {
                foreach (var id in idlist)
                {
                    q = q.OrWhere("id", "=", id.ToLower());
                }
                return q;
            });

        public static Query LastChangedFilter(this Query query, string? updatefrom) =>
            query.When(
                updatefrom != null,
                query => query.WhereRaw(
                    "to_date(data->>'LastChange', 'YYYY-MM-DD') > date(?)",
                    updatefrom
                )
            );

        public static Query LocFilterDistrictFilter(this Query query, IReadOnlyCollection<string> districtlist) =>
            query.WhereInJsonb(
                list: districtlist,
                jsonPath: "LocationInfo.DistrictInfo.Id"
            );

        public static Query LocFilterMunicipalityFilter(this Query query, IReadOnlyCollection<string> municipalitylist) =>
            query.WhereInJsonb(
                list: municipalitylist,
                jsonPath: "LocationInfo.MunicipalityInfo.Id"
            );

        public static Query LocFilterTvsFilter(this Query query, IReadOnlyCollection<string> tourismvereinlist) =>
            query.WhereInJsonb(
                list: tourismvereinlist,
                jsonPath: "LocationInfo.TvInfo.Id"
            );

        public static Query LocFilterRegionFilter(this Query query, IReadOnlyCollection<string> regionlist) =>
            query.WhereInJsonb(
                list: regionlist,
                jsonPath: "LocationInfo.RegionInfo.Id"
            );

        public static Query AreaFilter(this Query query, IReadOnlyCollection<string> arealist) =>
            query.WhereInJsonb(
                arealist,
                areaid => new { AreaId = new[] { areaid } }
            );

        public static Query HighlightFilter(this Query query, bool? highlight) =>
            query.When(
                highlight != null,
                query => query.WhereJsonb(
                    "Highlight",
                    highlight ?? false
                )
            );

        public static Query ActiveFilter(this Query query, bool? active) =>
            query.When(
                active != null,
                query => query.WhereJsonb(
                    "Active",
                    active ?? false
                )
            );

        public static Query SmgActiveFilter(this Query query, bool? smgactive) =>
            query.When(
                smgactive != null,
                query => query.WhereJsonb(
                    "SmgActive",
                    smgactive ?? false
                )
            );

        public static Query DistanceFilter(this Query query, bool distance, int distancemin, int distancemax) =>
            query.When(
                distance,
                query => query.WhereRaw(
                    "(data->>'DistanceLength')::numeric > ? AND (data->>'DistanceLength')::numeric < ?",
                    distancemin,
                    distancemax
                )
            );

        public static Query DurationFilter(this Query query, bool duration, double durationmin, double durationmax) =>
            query.When(
                duration,
                query => query.WhereRaw(
                    "(data->>'DistanceDuration')::numeric > ? AND (data->>'DistanceDuration')::numeric < ?",
                    durationmin,
                    durationmax
                )
            );

        public static Query AltitudeFilter(this Query query, bool altitude, int altitudemin, int altitudemax) =>
            query.When(
                altitude,
                query => query.WhereRaw(
                    "(data->>'AltitudeDifference')::numeric > ? AND (data->>'AltitudeDifference')::numeric < ?",
                    altitudemin,
                    altitudemax
                )
            );

        public static Query SmgTagFilter(this Query query, IReadOnlyCollection<string> smgtaglist) =>
            query.WhereInJsonb(
                smgtaglist,
                tag => new { SmgTags = new[] { tag.ToLower() } }
            );

        public static Query SearchFilter(this Query query, string[] fields, string? searchfilter) =>
            query.When(
                searchfilter != null && fields.Length > 0,
                query => query.Where(q =>
                {
                    foreach (var field in fields)
                    {
                        q = q.OrWhereRaw(
                                $"data#>>'\\{{{JsonPathToPostgresArray(field)}\\}}' ILIKE ?",
                                $"%{searchfilter}%");
                    }
                    return q;
                })
            );

        public static Query ActivityTypeFilter(this Query query, IReadOnlyCollection<string> activitytypelist) =>
            query.WhereInJsonb(
                list: activitytypelist,
                "Type",
                type => type
            );

        public static Query ActivitySubTypeFilter(this Query query, IReadOnlyCollection<string> subtypelist) =>
            query.WhereInJsonb(
                list: subtypelist,
                tag => new { SmgTags = new[] { tag.ToLower() } }
            );

        public static Query DifficultyFilter(this Query query, IReadOnlyCollection<string> difficultylist) =>
            query.WhereInJsonb(
                list: difficultylist,
                jsonPath: "Difficulty"
            );

        public static Query PoiTypeFilter(this Query query, IReadOnlyCollection<string> poitypelist) =>
            query.WhereInJsonb(
                poitypelist,
                poitype => new { SmgTags = new[] { poitype.ToLower() } }
            );

        public static Query PoiSubTypeFilter(this Query query, IReadOnlyCollection<string> subtypelist) =>
            query.WhereInJsonb(
                subtypelist,
                poitype => new { SmgTags = new[] { poitype.ToLower() } }
            );

        public static Query MetaRegionFilter(this Query query, IReadOnlyCollection<string> metaregionlist) =>
            query.WhereInJsonb(
                metaregionlist,
                "Id",
                id => id.ToUpper()
            );

        // TODO Add correct filters
        public static Query CuisineCodeFilter(this Query query, IReadOnlyCollection<string> cuisinecodelist) =>
            query.WhereInJsonb(
                cuisinecodelist,
                tag => new { Facilities = new[] { new { Id = tag.ToUpper() } } }
            );

        public static Query CeremonyCodeFilter(this Query query, IReadOnlyCollection<string> ceremonycodelist) =>
           query.WhereInJsonb(
               ceremonycodelist,
               tag => new { CapacityCeremony = new[] { new { Id = tag.ToUpper() } } }
           );

        public static Query CategoryCodeFilter(this Query query, IReadOnlyCollection<string> categorycodelist) =>
           query.WhereInJsonb(
               categorycodelist,
               tag => new { CategoryCodes = new[] { new { Id = tag.ToUpper() } } }
           );

        public static Query DishCodeFilter(this Query query, IReadOnlyCollection<string> dishcodelist) =>
           query.WhereInJsonb(
               dishcodelist,
               tag => new { DishRates = new[] { new { Id = tag.ToUpper() } } }
           );

        public static Query SourceFilter(this Query query, IReadOnlyCollection<string> sourcelist) =>
            query.WhereInJsonb(
                list: sourcelist,
                "Source",
                id => id.ToUpper()
            );
    }
}
