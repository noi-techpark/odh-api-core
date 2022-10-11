using AspNetCore.CacheOutput;
using DataModel;
using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OdhApiCore.Filters;
using OdhApiCore.Responses;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.api
{
    /// <summary>
    /// ODH Activity Poi Api (data provided by various data providers) SOME DATA Available as OPENDATA
    /// </summary>
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class ODHActivityPoiController : OdhController
    {        
        public ODHActivityPoiController(IWebHostEnvironment env, ISettings settings, ILogger<ODHActivityPoiController> logger, QueryFactory queryFactory)
           : base(env, settings, logger, queryFactory)
        {
        }

        #region SWAGGER Exposed API

        //Standard GETTER

        /// <summary>
        /// GET ODHActivityPoi List
        /// </summary>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, not provided disables Random Sorting, (default:'null') </param>
        /// <param name="type">Type of the ODHActivityPoi ('null' = Filter disabled, possible values: BITMASK: 1 = Wellness, 2 = Winter, 4 = Summer, 8 = Culture, 16 = Other, 32 = Gastronomy, 64 = Mobility, 128 = Shops and services), (default: 255 == ALL), refers to <a href='https://tourism.opendatahub.bz.it/v1/ODHActivityPoiTypes?rawfilter=eq(Type,%27Type%27)' target="_blank">ODHActivityPoi Types</a>, Type: Type</param>
        /// <param name="activitytype">Filtering by Activity Type defined by LTS ('null' = Filter disabled, possible values: BITMASK: 'Mountains = 1','Cycling = 2','Local tours = 4','Horses = 8','Hiking = 16','Running and fitness = 32','Cross-country ski-track = 64','Tobbogan run = 128','Slopes = 256','Lifts = 512'), (default:'1023' == ALL), , refers to <a href='https://tourism.opendatahub.bz.it/v1/ActivityTypes?rawfilter=eq(Type,%27Type%27)' target="_blank">ActivityTypes</a>, Type: Type</param>
        /// <param name="poitype">Filtering by Poi Type defined by LTS ('null' = Filter disabled, possible values: BITMASK 'Doctors, Pharmacies = 1','Shops = 2','Culture and sights= 4','Nightlife and entertainment = 8','Public institutions = 16','Sports and leisure = 32','Traffic and transport = 64', 'Service providers' = 128, 'Craft' = 256), (default:'511' == ALL), , refers to <a href='https://tourism.opendatahub.bz.it/v1/PoiTypes?rawfilter=eq(Type,%27Type%27)' target="_blank">PoiTypes</a>, Type: Type</param>
        /// <param name="subtype">Subtype of the ODHActivityPoi ('null' = Filter disabled, BITMASK Filter, available SubTypes depends on the selected Maintype) <a href='https://tourism.opendatahub.bz.it/v1/ODHActivityPoiTypes?rawfilter=eq(Type,%27SubType%27)' target="_blank">ODHActivityPoi SubTypes</a>, or <a href='https://tourism.opendatahub.bz.it/v1/ActivityTypes?rawfilter=eq(Type,%27SubType%27)' target="_blank">Activity SubTypes</a>, or <a href='https://tourism.opendatahub.bz.it/v1/PoiTypes?rawfilter=eq(Type,%27SubType%27)' target="_blank">Poi SubTypes</a>, Type: SubType</param>
        /// <param name="level3type">Additional Type of Level 3 the ODHActivityPoi ('null' = Filter disabled, BITMASK Filter, available SubTypes depends on the selected Maintype, SubType reference to ODHActivityPoiTypes)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of ODHActivityPoi IDs), (default:'null')</param>
        /// <param name="locfilter">Locfilter SPECIAL Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction), 'null' = (No Filter), (default:'null') <a href="https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#location-filter-locfilter" target="_blank">Wiki locfilter</a></param>        
        /// <param name="areafilter">AreaFilter (Alternate Locfilter, can be combined with locfilter) (Separator ',' possible values: reg + REGIONID = (Filter by Region), tvs + TOURISMASSOCIATIONID = (Filter by Tourismassociation), skr + SKIREGIONID = (Filter by Skiregion), ska + SKIAREAID = (Filter by Skiarea), are + AREAID = (Filter by LTS Area), 'null' = No Filter), (default:'null')</param>
        /// <param name="langfilter">ODHActivityPoi Langfilter (returns only SmgPois available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="highlight">Hightlight Filter (possible values: 'false' = only ODHActivityPoi with Highlight false, 'true' = only ODHActivityPoi with Highlight true), (default:'null')</param>
        /// <param name="source">Source Filter (possible Values: 'null' Displays all ODHActivityPoi, 'None', 'ActivityData', 'PoiData', 'GastronomicData', 'MuseumData', 'Magnolia', 'Content', 'SuedtirolWein', 'ArchApp'), (default:'null')</param>
        /// <param name="latitude">GeoFilter FLOAT Latitude Format: '46.624975', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="longitude">GeoFilter FLOAT Longitude Format: '11.369909', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="radius">Radius INTEGER to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="odhtagfilter">ODH Taglist Filter (refers to Array SmgTags) (String, Separator ',' more Tags possible (OR FILTER), available Tags reference to 'v1/ODHTag?validforentity=odhactivitypoi'), (default:'null')</param>        
        /// <param name="odhtagfilter_and">ODH Taglist Filter (refers to Array SmgTags) (String, Separator ',' more Tags possible (AND FILTER), available Tags reference to 'v1/ODHTag?validforentity=odhactivitypoi'), (default:'null')</param>        
        /// <param name="active">Active ODHActivityPoi Filter (possible Values: 'true' only active ODHActivityPoi, 'false' only not active ODHActivityPoi), (default:'null')</param>        
        /// <param name="odhactive">ODH Active (Published) ODHActivityPoi Filter (Refers to field OdhActive) (possible Values: 'true' only published ODHActivityPoi, 'false' only not published ODHActivityPoi), (default:'null')</param>        
        /// <param name="categorycodefilter">CategoryCode Filter (Only for ODHActivityTypes of type Gastronomy) (BITMASK) refers to <a href='https://tourism.opendatahub.bz.it/v1/GastronomyTypes?rawfilter=eq(Type,\"CategoryCodes\")' target="_blank">GastronomyTypes</a>, Type: CategoryCodes</param>
        /// <param name="dishcodefilter">DishCode Filter (Only for ODHActivityTypes of type Gastronomy) (BITMASK) refers to <a href='https://tourism.opendatahub.bz.it/v1/GastronomyTypes' target="_blank">GastronomyTypes</a>, Type: DishCodes</param>
        /// <param name="ceremonycodefilter">CeremonyCode Filter (Only for ODHActivityTypes of type Gastronomy) (BITMASK) refers to <a href='https://tourism.opendatahub.bz.it/v1/GastronomyTypes' target="_blank">GastronomyTypes</a>, Type: CeremonyCodes</param>
        /// <param name="facilitycodefilter">FacilityCode Filter (Only for ODHActivityTypes of type Gastronomy) (BITMASK) refers to <a href='https://tourism.opendatahub.bz.it/v1/GastronomyTypes' target="_blank">GastronomyTypes</a>, Type: with FacilityCodes_ prefix</param>       
        /// <param name="cuisinecodefilter">CuisineCode Filter (Only for ODHActivityTypes of type Gastronomy) (BITMASK) refers to <a href='https://tourism.opendatahub.bz.it/v1/GastronomyTypes' target="_blank">GastronomyTypes</a>, Type: CuisineCodes</param>       
        /// <param name="distancefilter">Distance Range Filter (Separator ',' example Value: 15,40 Distance from 15 up to 40 Km), (default:'null')</param>
        /// <param name="altitudefilter">Altitude Range Filter (Separator ',' example Value: 500,1000 Altitude from 500 up to 1000 metres), (default:'null')</param>
        /// <param name="durationfilter">Duration Range Filter (Separator ',' example Value: 1,3 Duration from 1 to 3 hours), (default:'null')</param>
        /// <param name="difficultyfilter">Difficulty Filter (possible values: '1' = easy, '2' = medium, '3' = difficult), (default:'null')</param>
        /// <param name="tagfilter">Filter on Tags. Syntax =and/or(TagSource.TagId,TagSource.TagId) example or(idm.summer,lts.hiking) and(idm.themed hikes,lts.family hikings), default: 'null')</param>
        /// <param name="publishedon">Published On Filter (Separator ',' List of publisher IDs), (default:'null')</param>       
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of ODH Activity Poi Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(JsonResult<ODHActivityPoiLinked>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [TypeFilter(typeof(Filters.RequestInterceptorAttribute))]
        [OdhCacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 3600, CacheKeyGenerator = typeof(CustomCacheKeyGenerator), MustRevalidate = true)]
        [HttpGet, Route("ODHActivityPoi")]
        public async Task<IActionResult> GetODHActivityPoiList(
            string? language = null,
            uint pagenumber = 1,
            PageSize pagesize = null!,
            string? type = "255",
            string? activitytype = null,
            string? poitype = null,
            string? subtype = null,
            string? level3type = null,
            string? idlist = null,
            string? locfilter = null,
            string? langfilter = null,
            string? areafilter = null,
            LegacyBool highlight = null!,
            string? source = null,
            string? odhtagfilter = null,
            string? odhtagfilter_and = null,
            LegacyBool odhactive = null!,
            LegacyBool active = null!,
            string? categorycodefilter = null,
            string? dishcodefilter = null,
            string? ceremonycodefilter = null,            
            string? facilitycodefilter = null,
            string? cuisinecodefilter = null,
            string? difficultyfilter = null,
            string? distancefilter = null,
            string? altitudefilter = null,
            string? durationfilter = null,
            string? tagfilter = null,
            string? publishedon = null,            
            string? updatefrom = null,
            string? seed = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            return await GetFiltered(
                fields: fields ?? Array.Empty<string>(), language: language, pagenumber: pagenumber, pagesize: pagesize,
                type: type, subtypefilter: subtype, level3typefilter: level3type, searchfilter: searchfilter, idfilter: idlist, languagefilter: langfilter,
                sourcefilter: source, locfilter: locfilter, areafilter: areafilter, highlightfilter: highlight?.Value, active: active?.Value,
                smgactive: odhactive?.Value, smgtags: odhtagfilter, smgtagsand: odhtagfilter_and,
                categorycodefilter: categorycodefilter, dishcodefilter: dishcodefilter, ceremonycodefilter: ceremonycodefilter, facilitycodefilter: facilitycodefilter, cuisinecodefilter: cuisinecodefilter,
                activitytypefilter: activitytype, poitypefilter: poitype, difficultyfilter: difficultyfilter, distancefilter: distancefilter, altitudefilter: altitudefilter, durationfilter: durationfilter,
                tagfilter: tagfilter, publishedon: publishedon, seed: seed, lastchange: updatefrom, geosearchresult, rawfilter: rawfilter, rawsort: rawsort,
                removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET ODHActivityPoi Single 
        /// </summary>
        /// <param name="id">ID of the Poi</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>ODHActivityPoi Object</returns>
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ODHActivityPoiLinked), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("ODHActivityPoi/{id}", Name = "SingleODHActivityPoi")]
        public async Task<IActionResult> GetODHActivityPoiSingle(
            string id,
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetSingle(id, language, fields: fields ?? Array.Empty<string>(), removenullvalues: removenullvalues, cancellationToken);
        }

        //Special GETTER

        /// <summary>
        /// GET ODHActivityPoi Types List
        /// </summary>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of ActivityPoiType Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<SmgPoiTypes>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("ODHActivityPoiTypes")]
        public async Task<IActionResult> GetAllODHActivityPoiTypesList(
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetSmgPoiTypesList(language, fields: fields ?? Array.Empty<string>(), searchfilter, rawfilter, rawsort, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET ODHActivityPoi Types Single
        /// </summary>
        /// <param name="id">ID of the ODHActivityPoi Type</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>ActivityPoiType Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(SmgPoiTypes), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("ODHActivityPoiTypes/{*id}", Name = "SingleODHActivityPoiTypes")]
        public async Task<IActionResult> GetAllODHActivityPoiTypesSingle(
            string id,
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetSmgPoiTypesSingle(id, language, fields: fields ?? Array.Empty<string>(), removenullvalues: removenullvalues, cancellationToken);
        }


        #endregion

        #region GETTER

        private Task<IActionResult> GetFiltered(string[] fields, string? language, uint pagenumber, int? pagesize,
            string? type, string? subtypefilter, string? level3typefilter, string? searchfilter, string? idfilter, string? languagefilter, string? sourcefilter, string? locfilter,
            string? areafilter, bool? highlightfilter, bool? active, bool? smgactive, string? smgtags, string? smgtagsand,
            string? categorycodefilter, string? dishcodefilter, string? ceremonycodefilter, string? facilitycodefilter, string? cuisinecodefilter,
            string? activitytypefilter, string? poitypefilter, string? difficultyfilter, string? distancefilter, string? altitudefilter, string? durationfilter,
            string? tagfilter, string? publishedon,
            string? seed, string? lastchange, PGGeoSearchResult geosearchresult,
            string? rawfilter, string? rawsort, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                ODHActivityPoiHelper myodhactivitypoihelper = await ODHActivityPoiHelper.CreateAsync(
                    queryFactory: QueryFactory,typefilter: type, subtypefilter: subtypefilter, level3typefilter: level3typefilter, 
                    idfilter: idfilter, locfilter: locfilter, areafilter: areafilter,  languagefilter: languagefilter, sourcefilter: sourcefilter, 
                    highlightfilter: highlightfilter, activefilter: active, smgactivefilter: smgactive, smgtags: smgtags, smgtagsand: smgtagsand, lastchange: lastchange, 
                    categorycodefilter : categorycodefilter, dishcodefilter: dishcodefilter, ceremonycodefilter: ceremonycodefilter, facilitycodefilter: facilitycodefilter, cuisinecodefilter: cuisinecodefilter,
                    activitytypefilter: activitytypefilter, poitypefilter: poitypefilter, distancefilter: distancefilter, altitudefilter: altitudefilter, durationfilter: durationfilter, difficultyfilter: difficultyfilter,
                    tagfilter: tagfilter, publishedonfilter: publishedon,
                    cancellationToken);

                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From("smgpois")
                        .ODHActivityPoiWhereExpression(
                            idlist: myodhactivitypoihelper.idlist, typelist: myodhactivitypoihelper.typelist,
                            subtypelist: myodhactivitypoihelper.subtypelist, level3typelist: myodhactivitypoihelper.level3typelist,
                            smgtaglist: myodhactivitypoihelper.smgtaglist, smgtaglistand: myodhactivitypoihelper.smgtaglistand, districtlist: myodhactivitypoihelper.districtlist,
                            municipalitylist: myodhactivitypoihelper.municipalitylist, tourismvereinlist: myodhactivitypoihelper.tourismvereinlist,
                            regionlist: myodhactivitypoihelper.regionlist, arealist: myodhactivitypoihelper.arealist,
                            sourcelist: myodhactivitypoihelper.sourcelist, languagelist: myodhactivitypoihelper.languagelist,
                            highlight: myodhactivitypoihelper.highlight,
                            activefilter: myodhactivitypoihelper.active, smgactivefilter: myodhactivitypoihelper.smgactive,
                            categorycodeslist: myodhactivitypoihelper.categorycodesids, dishcodeslist: myodhactivitypoihelper.dishcodesids, ceremonycodeslist: myodhactivitypoihelper.ceremonycodesids,
                            facilitycodeslist: myodhactivitypoihelper.facilitycodesids, 
                            activitytypelist: myodhactivitypoihelper.activitytypelist, poitypelist: myodhactivitypoihelper.poitypelist,
                            difficultylist: myodhactivitypoihelper.difficultylist, distance: myodhactivitypoihelper.distance,
                            distancemin: myodhactivitypoihelper.distancemin, distancemax: myodhactivitypoihelper.distancemax, duration: myodhactivitypoihelper.duration, durationmin: myodhactivitypoihelper.durationmin, 
                            durationmax: myodhactivitypoihelper.durationmax, altitude: myodhactivitypoihelper.altitude, altitudemin: myodhactivitypoihelper.altitudemin, altitudemax: myodhactivitypoihelper.altitudemax,
                            tagdict: myodhactivitypoihelper.tagdict, publishedonlist: myodhactivitypoihelper.publishedonlist,
                            searchfilter: searchfilter, language: language, lastchange: myodhactivitypoihelper.lastchange,
                            filterClosedData: FilterClosedData, reducedData: ReducedData)
                        .ApplyRawFilter(rawfilter)
                        .ApplyOrdering_GeneratedColumns(ref seed, geosearchresult, rawsort);

                // Get paginated data
                var data =
                    await query
                        .PaginateAsync<JsonRaw>(
                            page: (int)pagenumber,
                            perPage: pagesize ?? 25);

                var fieldsTohide = FieldsToHide;

                var dataTransformed =
                    data.List.Select(
                        raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide)
                    );

                uint totalpages = (uint)data.TotalPages;
                uint totalcount = (uint)data.Count;

                return ResponseHelpers.GetResult(
                    pagenumber,
                    totalpages,
                    totalcount,
                    seed,
                    dataTransformed,
                    Url);
            });
        }

        private Task<IActionResult> GetSingle(string id, string? language, string[] fields, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("smgpois")
                        .Select("data")
                        .Where("id", id.ToLower())
                        .Anonymous_Logged_UserRule_GeneratedColumn(FilterClosedData, !ReducedData); ;

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                var fieldsTohide = FieldsToHide;

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide);
            });
        }

        #endregion

        #region CATEGORIES

        private Task<IActionResult> GetSmgPoiTypesList(string? language, string[] fields, string? searchfilter, string? rawfilter, string? rawsort, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("smgpoitypes")
                        .SelectRaw("data")
                        .When(FilterClosedData, q => q.FilterClosedData())
                        .SearchFilter(PostgresSQLWhereBuilder.TypeDescFieldsToSearchFor(language), searchfilter)
                        .ApplyRawFilter(rawfilter)
                        .OrderOnlyByRawSortIfNotNull(rawsort);

                var data = await query.GetAsync<JsonRaw?>();

                var fieldsTohide = FieldsToHide;

                var dataTransformed =
                    data.Select(
                        raw => raw?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide)
                    );

                return dataTransformed;
            });
        }

        private Task<IActionResult> GetSmgPoiTypesSingle(string id, string? language, string[] fields, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("smgpoitypes")
                        .Select("data")
                        //.WhereJsonb("Key", "ilike", id)
                        .Where("id", id.ToLower())
                        .When(FilterClosedData, q => q.FilterClosedData());
                //.Where("Key", "ILIKE", id);

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                var fieldsTohide = FieldsToHide;

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide);
            });
        }


        #endregion

        #region POST PUT DELETE

        /// <summary>
        /// POST Insert new ODHActivityPoi
        /// </summary>
        /// <param name="odhactivitypoi">ODHActivityPoi Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,ODHActivityPoiWriter,ODHActivityPoiManager,ODHActivityPoiCreate,SmgPoiManager,SmgPoiCreate")]
        [HttpPost, Route("ODHActivityPoi")]
        public Task<IActionResult> Post([FromBody] ODHActivityPoiLinked odhactivitypoi)
        {
            return DoAsyncReturn(async () =>
            {
                //GENERATE ID
                odhactivitypoi.Id = Helper.IdGenerator.GenerateIDFromType(odhactivitypoi);

                odhactivitypoi.CheckMyInsertedLanguages(new List<string> { "de", "en", "it","nl","cs","pl","ru","fr" });


                //odhactivitypoi.Id = !String.IsNullOrEmpty(odhactivitypoi.Id) ? odhactivitypoi.Id.ToLower() : "noid";
                return await UpsertData<ODHActivityPoiLinked>(odhactivitypoi, "smgpois", true);
            });
        }

        /// <summary>
        /// PUT Modify existing ODHActivityPoi
        /// </summary>
        /// <param name="id">ODHActivityPoi Id</param>
        /// <param name="odhactivitypoi">ODHActivityPoi Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataModify,ODHActivityPoiWriter,ODHActivityPoiManager,ODHActivityPoiModify,SmgPoiManager,SmgPoiModify")]
        [HttpPut, Route("ODHActivityPoi/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] ODHActivityPoiLinked odhactivitypoi)
        {
            return DoAsyncReturn(async () =>
            {
                //Check ID uppercase lowercase
                Helper.IdGenerator.CheckIdFromType(odhactivitypoi);

                odhactivitypoi.CheckMyInsertedLanguages(new List<string> { "de", "en", "it", "nl", "cs", "pl", "ru", "fr" });

                //odhactivitypoi.Id = id.ToLower();
                return await UpsertData<ODHActivityPoiLinked>(odhactivitypoi, "smgpois", false, true);
            });
        }

        /// <summary>
        /// DELETE ODHActivityPoi by Id
        /// </summary>
        /// <param name="id">ODHActivityPoi Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataDelete,ODHActivityPoiWriter,ODHActivityPoiManager,ODHActivityPoiDelete,SmgPoiManager,SmgPoiDelete")]
        [HttpDelete, Route("ODHActivityPoi/{id}")]
        public Task<IActionResult> Delete(string id)
        {
            return DoAsyncReturn(async () =>
            {
                //Check ID uppercase lowercase
                id = Helper.IdGenerator.CheckIdFromType<ODHActivityPoiLinked>(id);

                return await DeleteData(id, "smgpois");
            });
        }


        #endregion
    }
}
