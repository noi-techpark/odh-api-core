using SqlKata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helper
{
    public static class PostgresSQLWhereBuilder
    {
        private static readonly string[] _languagesToSearchFor =
            new[] { "de", "it", "en" };

        /// <summary>
        /// Provide title fields as JsonPath
        /// </summary>
        /// <param name="language">
        /// If provided only the fields with the
        /// specified language get returned
        /// </param>
        private static string[] TitleFieldsToSearchFor(string? language) =>
            _languagesToSearchFor.Where(lang =>
                language != null ? lang == language : true
            ).Select(lang =>
                $"Detail.{lang}.Title"
            ).ToArray();

        public static void CheckPassedLanguage(ref string language, IEnumerable<string> availablelanguages)
        {
            language = language.ToLower();

            if (!availablelanguages.Contains(language))
                throw new Exception("passed language not available or passed incorrect string");
        }

        //Return where and Parameters
        [System.Diagnostics.Conditional("TRACE")]
        private static void LogMethodInfo(System.Reflection.MethodBase m, params object?[] parameters)
        {
            var parameterInfo =
                m.GetParameters()
                    .Zip(parameters)
                    .Select((x, _) => (x.First.Name, x.Second));
            Serilog.Log.Debug("{method}({@parameters})", m.Name, parameterInfo);
        }

        //Return Where and Parameters for Activity
        public static Query ActivityWhereExpression(
            this Query query, IReadOnlyCollection<string> languagelist,
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> activitytypelist,
            IReadOnlyCollection<string> subtypelist, IReadOnlyCollection<string> difficultylist,
            IReadOnlyCollection<string> smgtaglist, IReadOnlyCollection<string> districtlist,
            IReadOnlyCollection<string> municipalitylist, IReadOnlyCollection<string> tourismvereinlist,
            IReadOnlyCollection<string> regionlist, IReadOnlyCollection<string> arealist, bool distance, int distancemin,
            int distancemax, bool duration, int durationmin, int durationmax, bool altitude, int altitudemin,
            int altitudemax, bool? highlight, bool? activefilter, bool? smgactivefilter, string? searchfilter,
            string? language, string? lastchange, bool filterClosedData)
        {
            LogMethodInfo(
                System.Reflection.MethodBase.GetCurrentMethod()!,
                 "<query>", // not interested in query
                idlist, activitytypelist,
                subtypelist, difficultylist,
                smgtaglist, districtlist,
                municipalitylist, tourismvereinlist,
                regionlist, arealist, distance, distancemin,
                distancemax, duration, durationmin,
                durationmax, altitude, altitudemin,
                altitudemax, highlight, activefilter,
                smgactivefilter, searchfilter,
                language, lastchange
            );

            return query
                .IdUpperFilter(idlist)
                .DistrictFilter(districtlist)
                .LocFilterMunicipalityFilter(municipalitylist)
                .LocFilterTvsFilter(tourismvereinlist)
                .LocFilterRegionFilter(regionlist)
                .AreaFilter(arealist)
                .ActivityTypeFilter(activitytypelist)
                .ActivitySubTypeFilter(subtypelist)
                .DifficultyFilter(difficultylist)
                .DistanceFilter(distance, distancemin, distancemax)
                .DurationFilter(duration, durationmin, durationmax)
                .AltitudeFilter(altitude, altitudemin, altitudemax)
                .HighlightFilter(highlight)
                .ActiveFilter(activefilter)
                .SmgActiveFilter(smgactivefilter)
                .SmgTagFilter(smgtaglist)
                .SearchFilter(TitleFieldsToSearchFor(language), searchfilter)
                .LastChangedFilter(lastchange)
                .When(filterClosedData, q => q.FilterClosedData());
        }

        //Return Where and Parameters for Poi
        public static Query PoiWhereExpression(
            this Query query, IReadOnlyCollection<string> languagelist,
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> poitypelist,
            IReadOnlyCollection<string> subtypelist, IReadOnlyCollection<string> smgtaglist,
            IReadOnlyCollection<string> districtlist, IReadOnlyCollection<string> municipalitylist,
            IReadOnlyCollection<string> tourismvereinlist, IReadOnlyCollection<string> regionlist,
            IReadOnlyCollection<string> arealist, bool? highlight, bool? activefilter,
            bool? smgactivefilter, string? searchfilter, string? language, string? lastchange,
            bool filterClosedData)
        {
            LogMethodInfo(
                System.Reflection.MethodBase.GetCurrentMethod()!,
                 "<query>", 
                idlist, poitypelist,
                subtypelist, smgtaglist,
                districtlist, municipalitylist,
                tourismvereinlist, regionlist,
                arealist, highlight, activefilter,
                smgactivefilter, searchfilter,
                language, lastchange
            );

            return query
                .IdUpperFilter(idlist)
                .DistrictFilter(districtlist)
                .LocFilterMunicipalityFilter(municipalitylist)
                .LocFilterTvsFilter(tourismvereinlist)
                .LocFilterRegionFilter(regionlist)
                .AreaFilter(arealist)
                .PoiTypeFilter(poitypelist)
                .PoiSubTypeFilter(subtypelist)
                .SmgTagFilter(smgtaglist)
                .HighlightFilter(highlight)
                .ActiveFilter(activefilter)
                .SmgActiveFilter(smgactivefilter)
                .SearchFilter(TitleFieldsToSearchFor(language), searchfilter)
                .LastChangedFilter(lastchange)
                .When(filterClosedData, q => q.FilterClosedData());
        }

        //Return Where and Parameters for Gastronomy
        public static Query GastronomyWhereExpression(
            this Query query, IReadOnlyCollection<string> languagelist,
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> dishcodeslist,
            IReadOnlyCollection<string> ceremonycodeslist, IReadOnlyCollection<string> categorycodeslist,
            IReadOnlyCollection<string> facilitycodeslist, IReadOnlyCollection<string> smgtaglist,
            IReadOnlyCollection<string> districtlist, IReadOnlyCollection<string> municipalitylist,
            IReadOnlyCollection<string> tourismvereinlist, IReadOnlyCollection<string> regionlist, bool? activefilter,
            bool? smgactivefilter, string? searchfilter, string? language, string? lastchange,
            bool filterClosedData)
        {
            LogMethodInfo(
                System.Reflection.MethodBase.GetCurrentMethod()!,
                idlist, dishcodeslist,
                ceremonycodeslist, categorycodeslist,
                facilitycodeslist, smgtaglist,
                districtlist, municipalitylist,
                tourismvereinlist, regionlist,
                activefilter,
                smgactivefilter, searchfilter,
                language, lastchange
            );

            return query
                .IdUpperFilter(idlist)
                .DistrictFilter(districtlist)                
                .LocFilterMunicipalityFilter(municipalitylist)
                .LocFilterTvsFilter(tourismvereinlist)
                .LocFilterRegionFilter(regionlist)
                .CeremonyCodeFilter(ceremonycodeslist)
                .CategoryCodeFilter(categorycodeslist)
                .CuisineCodeFilter(facilitycodeslist)
                .DishCodeFilter(dishcodeslist)
                .SmgTagFilter(smgtaglist)                
                .ActiveFilter(activefilter)
                .SmgActiveFilter(smgactivefilter)
                .SearchFilter(TitleFieldsToSearchFor(language), searchfilter)
                .LastChangedFilter(lastchange)
                .When(filterClosedData, q => q.FilterClosedData());
        }

        //Return Where and Parameters for Activity
        public static Query ODHActivityPoiWhereExpression(
            this Query query, IReadOnlyCollection<string> languagelist,
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> typelist, IReadOnlyCollection<string> subtypelist,
            IReadOnlyCollection<string> poitypelist, IReadOnlyCollection<string> sourcelist,
            IReadOnlyCollection<string> smgtaglist, IReadOnlyCollection<string> districtlist,
            IReadOnlyCollection<string> municipalitylist, IReadOnlyCollection<string> tourismvereinlist,
            IReadOnlyCollection<string> regionlist, IReadOnlyCollection<string> arealist, bool? highlight, bool? activefilter, bool? smgactivefilter, 
            string? searchfilter, string? language, string? lastchange, bool filterClosedData)
        {
            LogMethodInfo(
                System.Reflection.MethodBase.GetCurrentMethod()!,
                 "<query>", // not interested in query
                idlist, typelist,
                subtypelist, poitypelist, languagelist, sourcelist,
                smgtaglist, districtlist,
                municipalitylist, tourismvereinlist,
                regionlist, arealist, highlight, activefilter,
                smgactivefilter, searchfilter,
                language, lastchange
            );

            return query
                .IdLowerFilter(idlist)
                .DistrictFilter(districtlist)
                .LocFilterMunicipalityFilter(municipalitylist)
                .LocFilterTvsFilter(tourismvereinlist)
                .LocFilterRegionFilter(regionlist)
                .AreaFilter(arealist)
                .ODHActivityPoiTypeFilter(typelist)
                .ODHActivityPoiSubTypeFilter(subtypelist)
                .ODHActivityPoiPoiTypeFilter(subtypelist)
                .SourceFilter(sourcelist)
                .HasLanguageFilter(languagelist)
                .HighlightFilter(highlight)
                .ActiveFilter(activefilter)
                .SmgActiveFilter(smgactivefilter)
                .SmgTagFilter(smgtaglist)
                .SearchFilter(TitleFieldsToSearchFor(language), searchfilter)
                .LastChangedFilter(lastchange)
                .When(filterClosedData, q => q.FilterClosedData());
        }

        //Return Where and Parameters for Article
        public static Query ArticleWhereExpression(
            this Query query, IReadOnlyCollection<string> languagelist,
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> typelist, IReadOnlyCollection<string> subtypelist,            
            IReadOnlyCollection<string> smgtaglist, bool? highlight, bool? activefilter, bool? smgactivefilter,
            string? searchfilter, string? language, string? lastchange, bool filterClosedData)
        {
            LogMethodInfo(
                System.Reflection.MethodBase.GetCurrentMethod()!,
                 "<query>", // not interested in query
                idlist, typelist,
                subtypelist, languagelist, smgtaglist, 
                highlight, activefilter,
                smgactivefilter, searchfilter,
                language, lastchange
            );

            return query
                .IdLowerFilter(idlist)
                .ODHActivityPoiTypeFilter(typelist)
                .ODHActivityPoiSubTypeFilter(subtypelist)
                .ODHActivityPoiPoiTypeFilter(subtypelist)
                .HasLanguageFilter(languagelist)
                .HighlightFilter(highlight)
                .ActiveFilter(activefilter)
                .SmgActiveFilter(smgactivefilter)
                .SmgTagFilter(smgtaglist)
                .SearchFilter(TitleFieldsToSearchFor(language), searchfilter)
                .LastChangedFilter(lastchange)
                .When(filterClosedData, q => q.FilterClosedData());
        }

        //Return Where and Parameters for Event
        public static Query EventWhereExpression(
          this Query query, IReadOnlyCollection<string> languagelist,
          IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> topiclist,
          IReadOnlyCollection<string> typelist, IReadOnlyCollection<string> ranclist,
          IReadOnlyCollection<string> smgtaglist, IReadOnlyCollection<string> districtlist,
          IReadOnlyCollection<string> municipalitylist, IReadOnlyCollection<string> tourismvereinlist,
          IReadOnlyCollection<string> regionlist, IReadOnlyCollection<string> orglist, DateTime? begindate, DateTime? enddate,
          bool? activefilter, bool? smgactivefilter, string? searchfilter,
          string? language, string? lastchange, bool filterClosedData)
        {
            LogMethodInfo(
                System.Reflection.MethodBase.GetCurrentMethod()!,
                 "<query>", // not interested in query
                idlist, topiclist,
                typelist, ranclist,
                smgtaglist, districtlist,
                municipalitylist, tourismvereinlist,
                regionlist, orglist, begindate, enddate,
                activefilter, smgactivefilter, searchfilter,
                language, lastchange
            );

            return query
                .IdUpperFilter(idlist)
                .DistrictFilter(districtlist)
                .LocFilterMunicipalityFilter(municipalitylist)
                .LocFilterTvsFilter(tourismvereinlist)
                .LocFilterRegionFilter(regionlist)
                .EventTopicFilter(topiclist)
                //.ActivityTypeFilter(activitytypelist)
                //.ActivitySubTypeFilter(subtypelist)
                //.DifficultyFilter(difficultylist)
                //.DistanceFilter(distance, distancemin, distancemax)
                //.DurationFilter(duration, durationmin, durationmax)
                //.AltitudeFilter(altitude, altitudemin, altitudemax)
                //.HighlightFilter(highlight)
                .ActiveFilter(activefilter)
                .SmgActiveFilter(smgactivefilter)
                .SmgTagFilter(smgtaglist)
                .SearchFilter(TitleFieldsToSearchFor(language), searchfilter)
                .LastChangedFilter(lastchange)
                .When(filterClosedData, q => q.FilterClosedData());
        }


        //Return Where and Parameters for WebCamInfo
        public static Query WebCamInfoWhereExpression(
            this Query query, IReadOnlyCollection<string> languagelist,
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> sourcelist,
            bool? activefilter, bool? smgactivefilter, string? searchfilter,
            string? language, string? lastchange, bool filterClosedData)
        {
            LogMethodInfo(
                System.Reflection.MethodBase.GetCurrentMethod()!,
                 "<query>", // not interested in query
                idlist, sourcelist,
                activefilter,
                smgactivefilter, searchfilter,
                language, lastchange
            );

            return query
                .IdUpperFilter(idlist)
                .SourceFilter(sourcelist)
                .ActiveFilter(activefilter)
                .SmgActiveFilter(smgactivefilter)
                .SearchFilter(TitleFieldsToSearchFor(language), searchfilter) //TODO here the title is in another field
                .LastChangedFilter(lastchange)
                .When(filterClosedData, q => q.FilterClosedData());
        }
    }
}
