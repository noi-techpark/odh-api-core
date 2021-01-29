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
            string jsonPath,
            string comparisonOperator,
            string value) =>
                query.WhereRaw(
                    $"data#>>'\\{{{JsonPathToPostgresArray(jsonPath)}\\}}' {comparisonOperator} ?",
                    value
                );

        public static Query WhereJsonb(
            this Query query,
            string jsonPath,
            string value) =>
            query.WhereJsonb(jsonPath, "=", value);

        public static Query WhereJsonb(
            this Query query,
            string jsonPath,
            int value) =>
                query.WhereRaw(
                    $"data#>>'\\{{{JsonPathToPostgresArray(jsonPath)}\\}}' = ?",
                    value.ToString()
                );

        public static Query WhereJsonb(
            this Query query,
            string jsonPath,
            bool value) =>
                query.WhereRaw(
                    $"data#>>'\\{{{JsonPathToPostgresArray(jsonPath)}\\}}' = ?",
                    value ? "true" : "false"
                );

        public static Query OrWhereJsonb(
            this Query query,
            string jsonPath,
            string value) =>
                query.OrWhereRaw(
                    $"data#>>'\\{{{JsonPathToPostgresArray(jsonPath)}\\}}' = ?",
                    value
                );

        public static Query OrWhereJsonb(
            this Query query,
            string jsonPath,
            int value) =>
                query.OrWhereRaw(
                    $"data#>>'\\{{{JsonPathToPostgresArray(jsonPath)}\\}}' = ?",
                    value.ToString()
                );

        public static Query OrWhereJsonb(
            this Query query,
            string jsonPath,
            bool value) =>
                query.OrWhereRaw(
                    $"data#>>'\\{{{JsonPathToPostgresArray(jsonPath)}\\}}' = ?",
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
            string jsonPath,
            Func<T, string> jsonObjectConstructor) =>
                query.Where(q =>
                {
                    foreach (var item in list)
                    {
                        q = q.OrWhereJsonb(
                            jsonPath,
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
                    new[] { new[] { list } }
                );
            }
        }

        [Obsolete]
        public static Query WhereAllInJsonb<T>(
           this Query query,
           IReadOnlyCollection<T> list,
           Func<T, object> jsonObjectConstructor) =>
               query.Where(q =>
               {
                   foreach (var item in list)
                   {
                       q = q.WhereJsonb(
                           value: item,
                           jsonObjectConstructor
                       );
                   }
                   return q;
               });

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

        public static Query IdIlikeFilter(this Query query, IReadOnlyCollection<string> idlist) =>
           query.Where(q =>
           {
               foreach (var id in idlist)
               {
                   q = q.OrWhere("id", "ILIKE", id);
               }
               return q;
           });

        public static Query LastChangedFilter(this Query query, string? updatefrom) =>
            query.When(
                updatefrom != null,
                query => query.WhereRaw(
                    "to_date(data#>>'\\{LastChange\\}', 'YYYY-MM-DD') > date(?)",
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

        public static Query AreaFilterMeasuringpoints(this Query query, IReadOnlyCollection<string> arealist) =>
            query.WhereInJsonb(
                arealist,
                areaid => new { AreaIds = new[] { areaid } }
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
                    "(data#>>'\\{DistanceLength\\}')::numeric > ? AND (data#>>'\\{DistanceLength\\}')::numeric < ?",
                    distancemin,
                    distancemax
                )
            );

        public static Query DurationFilter(this Query query, bool duration, double durationmin, double durationmax) =>
            query.When(
                duration,
                query => query.WhereRaw(
                    "(data#>>'\\{DistanceDuration\\}')::numeric > ? AND (data#>>'\\{DistanceDuration\\}')::numeric < ?",
                    durationmin,
                    durationmax
                )
            );

        public static Query AltitudeFilter(this Query query, bool altitude, int altitudemin, int altitudemax) =>
            query.When(
                altitude,
                query => query.WhereRaw(
                    "(data#>>'\\{AltitudeDifference\\}')::numeric > ? AND (data#>>'\\{AltitudeDifference\\}')::numeric < ?",
                    altitudemin,
                    altitudemax
                )
            );

        public static Query SmgTagFilter(this Query query, IReadOnlyCollection<string> smgtaglist) =>
            query.WhereInJsonb(
                smgtaglist,
                tag => new { SmgTags = new[] { tag.ToLower() } }
            );

        //NOT WORKING
        //public static Query SmgTagFilter(this Query query, IReadOnlyCollection<string> smgtaglist) =>
        //    query.WhereInJsonb(
        //        smgtaglist,
        //        "SmgTags",
        //        tag => tag.ToLower()
        //);

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

        public static Query ActivityTypeFilterOnTags(this Query query, IReadOnlyCollection<string> activitytypelist) =>
            query.WhereInJsonb(
                list: activitytypelist,
                tag => new { SmgTags = new[] { tag.ToLower() } }
            );

        public static Query ActivitySubTypeFilterOnTags(this Query query, IReadOnlyCollection<string> subtypelist) =>
            query.WhereInJsonb(
                list: subtypelist,
                tag => new { SmgTags = new[] { tag.ToLower() } }
            );

        public static Query DifficultyFilter(this Query query, IReadOnlyCollection<string> difficultylist) =>
            query.WhereInJsonb(
                list: difficultylist,
                jsonPath: "Difficulty"
            );

        public static Query PoiTypeFilterOnTags(this Query query, IReadOnlyCollection<string> poitypelist) =>
            query.WhereInJsonb(
                poitypelist,
                poitype => new { SmgTags = new[] { poitype.ToLower() } }
            );

        public static Query PoiSubTypeFilterOnTags(this Query query, IReadOnlyCollection<string> subtypelist) =>
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

        public static Query SyncSourceInterfaceFilter(this Query query, IReadOnlyCollection<string> sourcelist) =>
            query.WhereInJsonb(
                list: sourcelist,
                "SyncSourceInterface",
                id => id.ToUpper()
            );

        public static Query SourceFilterMeta(this Query query, IReadOnlyCollection<string> sourcelist) =>
            query.WhereInJsonb(
                list: sourcelist,
                "Meta.Source",
                id => id.ToUpper()
            );

        //not working
        //public static Query HasLanguageFilter(this Query query, IReadOnlyCollection<string> languagelist) =>
        //    query.WhereInJsonb(
        //        list: languagelist,
        //        "HasLanguage",
        //        id => id.ToUpper()
        //    );

        public static Query HasLanguageFilter(this Query query, IReadOnlyCollection<string> languagelist) =>
        query.WhereInJsonb(
                languagelist,
                lang => new { HasLanguage = new[] { lang.ToLower() } }
            );


        public static Query ODHActivityPoiTypeFilter(this Query query, IReadOnlyCollection<string> typelist) =>
            query.WhereInJsonb(
                list: typelist,
                "Type",
                type => type
            );

        public static Query ODHActivityPoiSubTypeFilter(this Query query, IReadOnlyCollection<string> subtypelist) =>
            query.WhereInJsonb(
                list: subtypelist,
                "SubType",
                type => type
            );

        public static Query ODHActivityPoiPoiTypeFilter(this Query query, IReadOnlyCollection<string> poitypelist) =>
             query.WhereInJsonb(
                list: poitypelist,
                "PoiType",
                type => type
            );

        //To change, ODH Type Filtering based on 
        public static Query ODHActivityPoiTypeFilterOnTags(this Query query, IReadOnlyCollection<string> typelist) =>
            query.WhereInJsonb(
                typelist,
                type => new { SmgTags = new[] { type.ToLower() } }
            );

        public static Query ODHActivityPoiSubTypeFilterOnTags(this Query query, IReadOnlyCollection<string> subtypelist) =>
           query.WhereInJsonb(
               subtypelist,
               subtype => new { SmgTags = new[] { subtype.ToLower() } }
           );

        public static Query ODHActivityPoiPoiTypeFilterOnTags(this Query query, IReadOnlyCollection<string> poitypelist) =>
           query.WhereInJsonb(
               poitypelist,
               poitype => new { SmgTags = new[] { poitype.ToLower() } }
           );


        public static Query EventTopicFilter(this Query query, IReadOnlyCollection<string> eventtopiclist) =>
           query.WhereInJsonb(
               eventtopiclist,
               topic => new { TopicRIDs = new[] { topic.ToUpper() } }
           );

        public static Query EventTypeFilter(this Query query, IReadOnlyCollection<string> eventtypelist) =>
           query.WhereInJsonb(
                list: eventtypelist,
                "Type",
                type => type
            );

        public static Query EventRancFilter(this Query query, IReadOnlyCollection<string> eventrancfilterlist) =>
           query.WhereInJsonb(
                list: eventrancfilterlist,
                "Ranc",
                ranc => ranc
            );

        public static Query EventOrgFilter(this Query query, IReadOnlyCollection<string> eventorgfilter) =>
           query.WhereInJsonb(
                list: eventorgfilter,
                "OrgRID",
                org => org
            );


        //Only Begindate given
        public static Query EventDateFilterBegin(this Query query, DateTime? begin, DateTime? end) =>
            query.When(
                begin != DateTime.MinValue && end == DateTime.MaxValue,
                query => query.WhereRaw(
                    "((begindate >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "' AND begindate < '" + String.Format("{0:yyyy-MM-dd}", end) + "') OR(enddate >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "' AND enddate < '" + String.Format("{0:yyyy-MM-dd}", end) + "'))"
                )
            );

        //Only Enddate given
        public static Query EventDateFilterEnd(this Query query, DateTime? begin, DateTime? end) =>
            query.When(
                begin == DateTime.MinValue && end != DateTime.MaxValue,
                query => query.WhereRaw(
                    "((begindate > '" + String.Format("{0:yyyy-MM-dd}", begin) + "' AND begindate < '" + String.Format("{0:yyyy-MM-dd}", end?.AddDays(1)) + "') OR (enddate > '" + String.Format("{0:yyyy-MM-dd}", begin) + "' AND enddate < '" + String.Format("{0:yyyy-MM-dd}", end?.AddDays(1)) + "'))"
                )
            );

        //Both Begin and Enddate given
        public static Query EventDateFilterBeginEnd(this Query query, DateTime? begin, DateTime? end) =>
            query.When(
                begin != DateTime.MinValue && end != DateTime.MaxValue,
                query => query.WhereRaw(
                    "((begindate >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "' AND begindate < '" + String.Format("{0:yyyy-MM-dd}", end?.AddDays(1)) + "') OR (enddate >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "' AND enddate < '" + String.Format("{0:yyyy-MM-dd}", end?.AddDays(1)) + "'))"
                )
            );
      
        public static Query VisibleInSearchFilter(this Query query, bool? visibleinsearch) =>
            query.When(
                visibleinsearch != null,
                query => query.WhereJsonb(
                    "VisibleInSearch",
                    visibleinsearch ?? false
                )
            );

        public static Query CompanyIdFilter(this Query query, IReadOnlyCollection<string> companyidfilter) =>
           query.WhereInJsonb(
                list: companyidfilter,
                "CompanyId",
                compid => compid
            );

        public static Query WineIdFilter(this Query query, IReadOnlyCollection<string> wineidfilter) =>
           query.WhereInJsonb(
                list: wineidfilter,
                "CustomId",
                wineid => wineid
            );

        public static Query AccoAltitudeFilter(this Query query, bool altitude, int altitudemin, int altitudemax) =>
            query.When(
                altitude,
                query => query.WhereRaw(
                    "(data#>>'\\{Altitude\\}')::numeric > ? AND (data#>>'\\{Altitude\\}')::numeric < ?",
                    altitudemin,
                    altitudemax
                )
            );

        public static Query AccoTypeFilter(this Query query, IReadOnlyCollection<string> accotypefilter) =>
          query.WhereInJsonb(
               list: accotypefilter,
               "AccoTypeId",
               wineid => wineid
           );
        
        public static Query AccoBoardFilter(this Query query, IReadOnlyCollection<string> boardlist) =>
         query.WhereInJsonb(
             boardlist,
             board => new { BoardIds = new[] { board.ToLower() } }
         );

        public static Query AccoBadgeFilter(this Query query, IReadOnlyCollection<string> badgelist) =>
         query.WhereInJsonb(
             badgelist,
             badge => new { BadgeIds = new[] { badge.ToLower() } }
         );
        
        public static Query AccoLTSFeatureFilter(this Query query, IReadOnlyCollection<string> ltsfeaturelist) =>
         query.WhereInJsonb(
             ltsfeaturelist,
             feature => new { Features = new[] { new { Id = feature.ToUpper() } } }
         );

        public static Query AccoMarketinggroupFilter(this Query query, IReadOnlyCollection<string> marketinggrouplist) =>
         query.WhereInJsonb(
             marketinggrouplist,
             marketinggroup => new { MarketingGroupIds = new[] { marketinggroup } }
         );

        public static Query AccoBookingPortalFeatureFilter(this Query query, IReadOnlyCollection<string> bookingportallist) =>
         query.WhereInJsonb(
             bookingportallist,
             bookingportal => new { AccoBookingChannel = new[] { new { Id = bookingportal } } }
         );

        //THEMEFILTER is AND connected
        public static Query AccoThemeFilter(this Query query, IReadOnlyCollection<string> themelist) =>                                  
            query.WhereAllInJsonb(
             themelist,
             theme => new { ThemeIds = new[] { theme } }
         );

        //TODO THEMEFILTER is AND connected
        public static Query AccoFeatureFilter(this Query query, IReadOnlyCollection<string> featurelist) =>
            query.WhereAllInJsonb(
             featurelist,
             feature => new { SpecialFeaturesIds = new[] { feature } }
         );

        public static Query AccoCategoryFilter(this Query query, IReadOnlyCollection<string> categorylist) =>
           query.WhereInJsonb(
                list: categorylist,
                "AccoCategoryId",
                category => category
            );

        public static Query AccoFeatureIdFilter(this Query query, IReadOnlyCollection<string> featureidlist) =>
        query.WhereInJsonb(
                featureidlist,
                tag => new { Features = new[] { new { Id = tag.ToUpper() } } }
            );


        public static Query AccoApartmentFilter(this Query query, bool? apartment) =>
            query.When(
                apartment != null,
                query => query.WhereJsonb(
                    "HasApartment",
                    apartment ?? false
                )
            );

        public static Query AccoBookableFilter(this Query query, bool? bookable) =>
            query.When(
                bookable != null,
                query => query.WhereJsonb(
                    "IsBookable",
                    bookable ?? false
                )
            );

        public static Query EventShortLocationFilter(this Query query, IReadOnlyCollection<string> eventlocationlist) =>
           query.WhereInJsonb(
               list: eventlocationlist,
               "EventLocation",
               id => id.ToUpper()
           );

        public static Query EventShortWebaddressFilter(this Query query, IReadOnlyCollection<string> webaddresslist) =>
           query.WhereInJsonb(
               list: webaddresslist,
               "WebAddress",
               id => id
           );

        public static Query EventShortActiveFilter(this Query query, string? active) =>
            query.When(
                active != null,
                query => query.WhereJsonb(
                    "Display1",
                    active ?? ""
                )
            );

        //Only Begindate given
        public static Query EventShortDateFilterBegin(this Query query, DateTime? start, DateTime? end, bool active) =>
            query.When(
                start != DateTime.MinValue && end == DateTime.MaxValue && active,
                query => query.WhereRaw(
                    "((to_timestamp(data ->> 'EndDate', 'YYYY-MM-DD T HH24:MI:SS') >= '" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", start) + "'))"
                )
            );

        //Only Enddate given
        public static Query EventShortDateFilterEnd(this Query query, DateTime? start, DateTime? end, bool active) =>
            query.When(
                start == DateTime.MinValue && end != DateTime.MaxValue && active,
                query => query.WhereRaw(
                    "((to_timestamp(data->> 'EndDate', 'YYYY-MM-DD T HH24:MI:SS') <= '" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", end) + "'))"
                )
            );

        //Both Begin and Enddate given
        public static Query EventShortDateFilterBeginEnd(this Query query, DateTime? start, DateTime? end, bool active) =>
            query.When(
                start != DateTime.MinValue && end != DateTime.MaxValue && active,
                query => query.WhereRaw(
                    "(((to_timestamp(data ->> 'StartDate', 'YYYY-MM-DD T HH24:MI:SS') >= '" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", start) + "') AND (to_timestamp(data->> 'EndDate', 'YYYY-MM-DD T HH24:MI:SS') <= '" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", end) + "')))"
                )
            );

        //Only Begindate given
        public static Query EventShortDateFilterBeginByRoom(this Query query, DateTime? start, DateTime? end, bool active) =>
            query.When(
                start != DateTime.MinValue && end == DateTime.MaxValue && active,
                query => query.WhereRaw(
                    "((to_date(data ->> 'EndDate', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", start) + "'))"
                )
            );

        //Only Enddate given
        public static Query EventShortDateFilterEndByRoom(this Query query, DateTime? start, DateTime? end, bool active) =>
            query.When(
                start == DateTime.MinValue && end != DateTime.MaxValue && active,
                query => query.WhereRaw(
                    "((to_date(data->> 'EndDate', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "'))"
                )
            );

        //Both Begin and Enddate given
        public static Query EventShortDateFilterBeginEndByRoom(this Query query, DateTime? start, DateTime? end, bool active) =>
            query.When(
                start != DateTime.MinValue && end != DateTime.MaxValue && active,
                query => query.WhereRaw(
                    "(((to_date(data ->> 'EndDate', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", start) + "') AND (to_date(data->> 'StartDate', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')))"
                )
            );

        public static Query ODHTagValidForEntityFilter(this Query query, IReadOnlyCollection<string> smgtagtypelist) =>           
           query.WhereInJsonb(
               smgtagtypelist,
               smgtagtype => new { ValidForEntity = new[] { smgtagtype.ToLower() } }
           );         

        //AlpineBits
        public static Query AlpineBitsAccommodationIdFilter(this Query query, IReadOnlyCollection<string> accommodationids) =>
            query.WhereInJsonb(
                list: accommodationids,
                "AccommodationId",
                id => id
            );

        //AlpineBits
        public static Query AlpineBitsMessageFilter(this Query query, IReadOnlyCollection<string> messagetypelist) =>
            query.WhereInJsonb(
                list: messagetypelist,
                "MessageType",
                id => id
            );

        //Venue Filters (Special case)

        public static Query VenueLocFilterDistrictFilter(this Query query, IReadOnlyCollection<string> districtlist) =>
          query.WhereInJsonb(
              list: districtlist,
              jsonPath: "odhdata.LocationInfo.DistrictInfo.Id"
          );

        public static Query VenueLocFilterMunicipalityFilter(this Query query, IReadOnlyCollection<string> municipalitylist) =>
            query.WhereInJsonb(
                list: municipalitylist,
                jsonPath: "odhdata.LocationInfo.MunicipalityInfo.Id"
            );

        public static Query VenueLocFilterTvsFilter(this Query query, IReadOnlyCollection<string> tourismvereinlist) =>
            query.WhereInJsonb(
                list: tourismvereinlist,
                jsonPath: "odhdata.LocationInfo.TvInfo.Id"
            );

        public static Query VenueLocFilterRegionFilter(this Query query, IReadOnlyCollection<string> regionlist) =>
            query.WhereInJsonb(
                list: regionlist,
                jsonPath: "odhdata.LocationInfo.RegionInfo.Id"
            );

        public static Query VenueActiveFilter(this Query query, bool? active) =>
            query.When(
                active != null,
                query => query.WhereJsonb(
                    "odhdata.Active",
                    active ?? false
                )
            );

        public static Query VenueODHActiveFilter(this Query query, bool? odhactive) =>
            query.When(
                odhactive != null,
                query => query.WhereJsonb(
                    "odhdata.ODHActive",
                    odhactive ?? false
                )
            );

        public static Query VenueCategoryFilter(this Query query, IReadOnlyCollection<string> categorylist) =>
           query.WhereInJsonb(
                   categorylist,
                   tag => new { odhdata = new { VenueCategory = new[] { new { Id = tag.ToUpper() } } } }
               );
        
        public static Query VenueFeatureFilter(this Query query, IReadOnlyCollection<string> featurelist) =>
           query.WhereInJsonb(
                   featurelist,
                   tag => new { odhdata = new { RoomDetails = new[] { new { VenueFeatures = new[] { new { Id = tag.ToUpper() } } } } } }
               );

        public static Query VenueSetupTypeFilter(this Query query, IReadOnlyCollection<string> setuptypelist) =>
          query.WhereInJsonb(
                  setuptypelist,
                  tag => new { odhdata = new { RoomDetails = new[] { new { VenueSetup = new[] { new { Id = tag.ToUpper() } } } } } }
              );

        public static Query VenueRoomCountFilter(this Query query, bool roomcount, int roomcountmin, int roomcountmax) =>
            query.When(
                roomcount,
                query => query.WhereRaw(
                    "(data#>>'\\{odhdata,RoomCount\\}')::numeric > ? AND (data#>>'\\{odhdata,RoomCount\\}')::numeric < ?",
                    roomcountmin,
                    roomcountmax
                )
            );

        public static Query VenueODHTagFilter(this Query query, IReadOnlyCollection<string> smgtaglist) =>
          query.WhereInJsonb(
              smgtaglist,
              tag => new { odhdata = new { ODHTags = new[] { tag.ToLower() } } }
          );

        public static Query VenueLastChangedFilter(this Query query, string? updatefrom) =>
           query.When(
               updatefrom != null,
               query => query.WhereRaw(
                   "to_date(data#>>'\\{meta,lastUpdate\\}', 'YYYY-MM-DD') > date(?)",
                   updatefrom
               )
           );

        public static Query VenueSourceFilter(this Query query, IReadOnlyCollection<string> sourcelist) =>
            query.WhereInJsonb(
                list: sourcelist,
                "odhdata.SyncSourceInterface",
                id => id.ToUpper()
            );

        public static Query VenueHasLanguageFilter(this Query query, IReadOnlyCollection<string> languagelist) =>
            query.WhereInJsonb(
               languagelist,
               lang => new { odhdata = new { HasLanguage = new[] { lang.ToLower() } } }
           );

        //private static Query VenueCapacityFilterWhere(bool capacity, int capacitymin, int capacitymax)
        //{
        //    //TODO!!!
        //    if (capacity)
        //    {
        //        //if (!String.IsNullOrEmpty(whereexpression))
        //        //    whereexpression = whereexpression + " AND ";

        //        //whereexpression = whereexpression + "(data ->'odhdata' ->> 'RoomCount')::numeric > @roomcountmin AND (data ->'odhdata' ->> 'RoomCount')::numeric < @roomcountmax";
        //        //parameters.Add(new PGParameters() { Name = "roomcountmin", Type = NpgsqlTypes.NpgsqlDbType.Numeric, Value = roomcountmin.ToString() });
        //        //parameters.Add(new PGParameters() { Name = "roomcountmax", Type = NpgsqlTypes.NpgsqlDbType.Numeric, Value = roomcountmax.ToString() });
        //    }

        //}

        //Standard JSON Filter
        public static Query FilterClosedData(this Query query) =>
            query.Where(q =>
                q.WhereRaw(
                    "data#>>'\\{LicenseInfo,ClosedData\\}' IS NULL"
                ).OrWhereRaw(
                    "data#>>'\\{LicenseInfo,ClosedData\\}' = 'false'"
                )
            );

        public static Query FilterClosedDataVenues(this Query query) =>
            query.Where(q =>
                q.WhereRaw(
                    "data#>>'\\{odhdata,LicenseInfo,ClosedData\\}' IS NULL"
                ).OrWhereRaw(
                    "data#>>'\\{odhdata,LicenseInfo,ClosedData\\}' = 'false'"
                )
            );

        #region Generated Columns Basic

        public static Query WhereArrayInListOr(this Query query, IReadOnlyCollection<string> list, string generatedcolumn) =>
                query.Where(q =>
                {
                    foreach (var item in list)
                    {
                        q = q.OrWhereRaw(
                            generatedcolumn + " @> array\\[?\\]", item.ToLower()
                        );
                    }
                    return q;
                });

        public static Query WhereArrayInListAnd(this Query query, IReadOnlyCollection<string> list, string generatedcolumn) =>
            query.Where(q => 
            q.WhereRaw(
                generatedcolumn + " @> array\\[?\\]", list.Select(x => x.ToLower())
                )
            );


        #endregion

        #region Generated Clolumns Where Expressions

        //Filter on Generated Field gen_licenseinfo_closeddata
        public static Query FilterClosedData_GeneratedColumn(this Query query) =>
            query.Where(q =>
                q.WhereRaw(
                    "gen_licenseinfo_closeddata IS NULL"
                ).OrWhereRaw(
                    "gen_licenseinfo_closeddata = false"
                )
            );
        
        //Filter on Generated Field gen_haslanguage AND
        public static Query HasLanguageFilterAnd_GeneratedColumn(this Query query, IReadOnlyCollection<string> languagelist) =>
         query.Where(q => q.WhereRaw(
             "gen_haslanguage @> array\\[?\\]", 
             languagelist.Select(x => x.ToLower())
             )
         );

        //Filter on Generated Field gen_haslanguage AND
        public static Query HasLanguageFilterOr_GeneratedColumn(this Query query, IReadOnlyCollection<string> languagelist) =>
         query.Where(q =>
         {
             foreach (var item in languagelist)
             {
                 q = q.OrWhereRaw(
                     "gen_haslanguage @> array\\[?\\]", item.ToLower()
                 );
             }
             return q;
         });

        //Filter on Generated Field gen_smgtags AND
        public static Query SmgTagFilterAnd_GeneratedColumn(this Query query, IReadOnlyCollection<string> list) =>
         query.Where(q => q.WhereRaw(
             "gen_smgtags @> array\\[?\\]", 
             list.Select(x => x.ToLower())
             )
         );

        //Filter on Generated Field gen_smgtags OR
        public static Query SmgTagFilterOr_GeneratedColumn(this Query query, IReadOnlyCollection<string> list) =>
        query.Where(q =>
        {
            foreach (var item in list)
            {
                q = q.OrWhereRaw(
                    "gen_smgtags @> array\\[?\\]", item.ToLower()
                );
            }
            return q;
        });
        
        //Filter on Generated Field gen_active 
        public static Query ActiveFilter_GeneratedColumn(this Query query, bool? active) =>
            query.When(
                active != null,
                query => query.WhereRaw(
                    "gen_active", 
                    active ?? false
                )
            );

        //Filter on Generated Field gen_odhactive 
        public static Query OdhActiveFilter_GeneratedColumn(this Query query, bool? odhactive) =>
            query.When(
                odhactive != null,
                query => query.WhereRaw(
                    "gen_odhactive",
                    odhactive ?? false
                )
            );

    }

    #endregion
}
