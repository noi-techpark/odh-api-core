using AspNetCore.CacheOutput;
using DataModel;
using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        /// <param name="type">Type of the ODHActivityPoi ('null' = Filter disabled, possible values: BITMASK: 1 = Wellness, 2 = Winter, 4 = Summer, 8 = Culture, 16 = Other, 32 = Gastronomy), (default: 63 == ALL), REFERENCE TO: GET /api/ODHActivityPoiTypes </param>
        /// <param name="subtype">Subtype of the ODHActivityPoi ('null' = Filter disabled, BITMASK Filter, available SubTypes depends on the selected Maintype reference to ODHActivityPoiTypes)</param>
        /// <param name="poitype">Additional Type of the ODHActivityPoi ('null' = Filter disabled, BITMASK Filter, available SubTypes depends on the selected Maintype, SubType reference to ODHActivityPoiTypes)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of ODHActivityPoi IDs), (default:'null')</param>
        /// <param name="locfilter">Locfilter SPECIAL Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction), 'null' = (No Filter), (default:'null') <a href="https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#location-filter-locfilter" target="_blank">Wiki locfilter</a></param>        
        /// <param name="areafilter">AreaFilter (Alternate Locfilter, can be combined with locfilter) (Separator ',' possible values: reg + REGIONID = (Filter by Region), tvs + TOURISMASSOCIATIONID = (Filter by Tourismassociation), skr + SKIREGIONID = (Filter by Skiregion), ska + SKIAREAID = (Filter by Skiarea), are + AREAID = (Filter by LTS Area), 'null' = No Filter), (default:'null')</param>
        /// <param name="langfilter">ODHActivityPoi Langfilter (returns only SmgPois available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="highlight">Hightlight Filter (possible values: 'false' = only ODHActivityPoi with Highlight false, 'true' = only ODHActivityPoi with Highlight true), (default:'null')</param>
        /// <param name="source">Source Filter (possible Values: 'null' Displays all ODHActivityPoi, 'None', 'ActivityData', 'PoiData', 'GastronomicData', 'MuseumData', 'Magnolia', 'Content', 'SuedtirolWein', 'ArchApp'), (default:'null')</param>
        /// <param name="latitude">GeoFilter FLOAT Latitude Format: '46.624975', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="longitude">GeoFilter FLOAT Longitude Format: '11.369909', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="radius">Radius INTEGER to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="odhtagfilter">ODH Taglist Filter (refers to Array SmgTags) (String, Separator ',' more Tags possible, available Tags reference to 'v1/ODHTag?validforentity=smgpoi'), (default:'null')</param>        
        /// <param name="active">Active ODHActivityPoi Filter (possible Values: 'true' only active ODHActivityPoi, 'false' only not active ODHActivityPoi), (default:'null')</param>        
        /// <param name="odhactive">ODH Active (Published) ODHActivityPoi Filter (Refers to field OdhActive) (possible Values: 'true' only published ODHActivityPoi, 'false' only not published ODHActivityPoi), (default:'null')</param>        
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
        [ProducesResponseType(typeof(JsonResult<ODHActivityPoi>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 3600, CacheKeyGenerator = typeof(CustomCacheKeyGenerator))]
        [HttpGet, Route("ODHActivityPoi")]
        public async Task<IActionResult> GetODHActivityPoiList(
            string? language = null,
            uint pagenumber = 1,
            PageSize pagesize = null!,
            string? type = "255",
            string? subtype = null,
            string? poitype = null,
            string? idlist = null,
            string? locfilter = null,
            string? langfilter = null,
            string? areafilter = null,
            LegacyBool highlight = null!,
            string? source = null,
            string? odhtagfilter = null,
            LegacyBool odhactive = null!,
            LegacyBool active = null!,
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
                type: type, subtypefilter: subtype, poitypefilter: poitype, searchfilter: searchfilter, idfilter: idlist, languagefilter: langfilter,
                sourcefilter: source, locfilter: locfilter, areafilter: areafilter, highlightfilter: highlight?.Value, active: active?.Value,
                smgactive: odhactive?.Value, smgtags: odhtagfilter, seed: seed, lastchange: updatefrom, geosearchresult, rawfilter: rawfilter, rawsort: rawsort,
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
        [ProducesResponseType(typeof(ODHActivityPoi), StatusCodes.Status200OK)]
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
            string? type, string? subtypefilter, string? poitypefilter, string? searchfilter, string? idfilter, string? languagefilter, string? sourcefilter, string? locfilter,
            string? areafilter, bool? highlightfilter, bool? active, bool? smgactive, string? smgtags, string? seed, string? lastchange, PGGeoSearchResult geosearchresult,
            string? rawfilter, string? rawsort, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                ODHActivityPoiHelper myodhactivitypoihelper = await ODHActivityPoiHelper.CreateAsync(
                    QueryFactory, type, subtypefilter, poitypefilter, idfilter, locfilter,
                    areafilter, languagefilter, sourcefilter, highlightfilter, active, smgactive, smgtags,
                    lastchange, cancellationToken);

                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From("smgpois")
                        .ODHActivityPoiWhereExpression(
                            idlist: myodhactivitypoihelper.idlist, typelist: myodhactivitypoihelper.typelist,
                            subtypelist: myodhactivitypoihelper.subtypelist, poitypelist: myodhactivitypoihelper.poitypelist,
                            smgtaglist: myodhactivitypoihelper.smgtaglist, districtlist: myodhactivitypoihelper.districtlist,
                            municipalitylist: myodhactivitypoihelper.municipalitylist, tourismvereinlist: myodhactivitypoihelper.tourismvereinlist,
                            regionlist: myodhactivitypoihelper.regionlist, arealist: myodhactivitypoihelper.arealist,
                            sourcelist: myodhactivitypoihelper.sourcelist, languagelist: myodhactivitypoihelper.languagelist,
                            highlight: myodhactivitypoihelper.highlight,
                            activefilter: myodhactivitypoihelper.active, smgactivefilter: myodhactivitypoihelper.smgactive,
                            searchfilter: searchfilter, language: language, lastchange: myodhactivitypoihelper.lastchange,
                            filterClosedData: FilterClosedData)
                        .ApplyRawFilter(rawfilter)
                        .ApplyOrdering_GeneratedColumns(ref seed, geosearchresult, rawsort);

                // Get paginated data
                var data =
                    await query
                        .PaginateAsync<JsonRaw>(
                            page: (int)pagenumber,
                            perPage: (int)pagesize);

                var dataTransformed =
                    data.List.Select(
                        raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, userroles: UserRolesList)
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
                        .Where("id", id);

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, userroles: UserRolesList);
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

                var dataTransformed =
                    data.Select(
                        raw => raw?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, userroles: UserRolesList)
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

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, userroles: UserRolesList);
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
        [Authorize(Roles = "DataWriter,DataCreate,ODHActivityPoiManager,ODHActivityPoiCreate,SmgPoiManager,SmgPoiCreate")]
        [HttpPost, Route("ODHActivityPoi")]
        public Task<IActionResult> Post([FromBody] ODHActivityPoi odhactivitypoi)
        {
            return DoAsyncReturn(async () =>
            {
                odhactivitypoi.Id = !String.IsNullOrEmpty(odhactivitypoi.Id) ? odhactivitypoi.Id.ToUpper() : "noId";
                return await UpsertData<ODHActivityPoi>(odhactivitypoi, "smgpois");
            });
        }

        /// <summary>
        /// PUT Modify existing ODHActivityPoi
        /// </summary>
        /// <param name="id">ODHActivityPoi Id</param>
        /// <param name="odhactivitypoi">ODHActivityPoi Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataModify,ODHActivityPoiManager,ODHActivityPoiModify,SmgPoiManager,SmgPoiModify")]
        [HttpPut, Route("ODHActivityPoi/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] ODHActivityPoi odhactivitypoi)
        {
            return DoAsyncReturn(async () =>
            {
                odhactivitypoi.Id = id.ToUpper();
                return await UpsertData<ODHActivityPoi>(odhactivitypoi, "smgpois");
            });
        }

        /// <summary>
        /// DELETE ODHActivityPoi by Id
        /// </summary>
        /// <param name="id">ODHActivityPoi Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataDelete,ODHActivityPoiManager,ODHActivityPoiDelete,SmgPoiManager,SmgPoiDelete")]
        [HttpDelete, Route("ODHActivityPoi/{id}")]
        public Task<IActionResult> Delete(string id)
        {
            return DoAsyncReturn(async () =>
            {
                id = id.ToUpper();
                return await DeleteData(id, "smgpois");
            });
        }


        #endregion
    }
}
