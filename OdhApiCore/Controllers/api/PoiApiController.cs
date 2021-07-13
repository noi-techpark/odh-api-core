using AspNetCore.CacheOutput;
using DataModel;
using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
    /// Poi Api (data provided by LTS PoiData) SOME DATA Available as OPENDATA
    /// </summary>
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class PoiController : OdhController
    {
        public PoiController(IWebHostEnvironment env, ISettings settings, ILogger<PoiController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {
        }

        #region SWAGGER Exposed API

        /// <summary>
        /// GET Poi List
        /// </summary>
        /// <param name="pagenumber">Pagenumber, (default:1)</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="poitype">Type of the Poi ('null' = Filter disabled, possible values: BITMASK 'Doctors, Pharmacies = 1','Shops = 2','Culture and sights= 4','Nightlife and entertainment = 8','Public institutions = 16','Sports and leisure = 32','Traffic and transport = 64', 'Service providers' = 128, 'Craft' = 256), (default:'511' == ALL), REFERENCE TO: GET /api/PoiTypes </param>
        /// <param name="subtype">Subtype of the Poi ('null' = Filter disabled, available Subtypes depends on the poitype BITMASK), (default:'null')</param>
        /// <param name="idlist">IDFilter (Separator ',' List of Activity IDs, 'null' = No Filter), (default:'null')</param>
        /// <param name="locfilter">Locfilter SPECIAL Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction), 'null' = (No Filter), (default:'null') <a href="https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#location-filter-locfilter" target="_blank">Wiki locfilter</a></param>        
        /// <param name="areafilter">AreaFilter (Alternate Locfilter, can be combined with locfilter) (Separator ',' possible values: reg + REGIONID = (Filter by Region), tvs + TOURISMASSOCIATIONID = (Filter by Tourismassociation), skr + SKIREGIONID = (Filter by Skiregion), ska + SKIAREAID = (Filter by Skiarea), are + AREAID = (Filter by LTS Area), 'null' = No Filter), (default:'null')</param>
        /// <param name="highlight">Highlight Filter (Show only Highlights possible values: 'true' : show only Highlight Pois, 'null' Filter disabled), (default:'null')</param>
        /// <param name="odhtagfilter">ODH Taglist Filter (refers to Array SmgTags) (String, Separator ',' more Tags possible, available Tags reference to 'v1/ODHTag?validforentity=poi'), (default:'null')</param>
        /// <param name="active">Active Pois Filter (possible Values: 'true' only Active Pois, 'false' only Disabled Pois, (default:'null')</param>
        /// <param name="odhactive">ODH Active (Published) Pois Filter (Refers to field SmgActive) Pois Filter (possible Values: 'true' only published Pois, 'false' only not published Pois, (default:'null')</param>
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="latitude">GeoFilter FLOAT Latitude Format: '46.624975', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter' target="_blank">Wiki geosort</a></param>
        /// <param name="longitude">GeoFilter FLOAT Longitude Format: '11.369909', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter' target="_blank">Wiki geosort</a></param>
        /// <param name="radius">Radius INTEGER to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter' target="_blank">Wiki geosort</a></param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null)<a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of LTSPoi Objects</returns>
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(JsonResult<GBLTSPoi>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 3600, CacheKeyGenerator = typeof(CustomCacheKeyGenerator))]
        [HttpGet, Route("Poi")]
        public async Task<IActionResult> GetPoiList(
            string? language = null,
            uint pagenumber = 1,
            PageSize pagesize = null!,
            string? poitype = "511",
            string? subtype = null,
            string? idlist = null,
            string? areafilter = null,
            LegacyBool highlight = null!,
            string? locfilter = null,
            string? odhtagfilter = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? updatefrom = null,
            string? langfilter = null,
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
            //TODO
            //CheckOpenData(User);

            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            return await GetFiltered(
                fields: fields ?? Array.Empty<string>(), language: language, pagenumber: pagenumber, pagesize: pagesize,
                activitytype: poitype, subtypefilter: subtype, idfilter: idlist, searchfilter: searchfilter,
                locfilter: locfilter, areafilter: areafilter, highlightfilter: highlight, active: active, smgactive: odhactive,
                smgtags: odhtagfilter, seed: seed, lastchange: updatefrom, langfilter: langfilter, geosearchresult: geosearchresult,
                rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// GET Poi Single
        /// </summary>
        /// <param name="id">ID of the Poi</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>GBLTSPoi Object</returns>
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(GBLTSPoi), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,PoiReader")]
        [HttpGet, Route("Poi/{id}", Name = "SinglePoi")]
        public async Task<IActionResult> GetPoiSingle(
            string id,
            string? language = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetSingle(id, language, fields: fields ?? Array.Empty<string>(), removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Poi Types List
        /// </summary>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null)<a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of PoiType Object</returns>
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        //[CacheOutputUntilToday(23, 59)]
        [ProducesResponseType(typeof(IEnumerable<PoiTypes>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,PoiReader")]
        [HttpGet, Route("PoiTypes")]
        public async Task<IActionResult> GetAllPoiTypesList(
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetPoiTypesList(language, fields: fields ?? Array.Empty<string>(), searchfilter, rawfilter, rawsort, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Poi Types Single
        /// </summary>
        /// <param name="id">ID of the Poi Type</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>PoiType Object</returns>
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(PoiTypes), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,PoiReader")]
        [HttpGet, Route("PoiTypes/{id}", Name = "SinglePoiTypes")]
        public async Task<IActionResult> GetAllPoiTypesSingle(
            string id,
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetPoiTypesSingleAsync(id, language, fields: fields ?? Array.Empty<string>(), removenullvalues: removenullvalues, cancellationToken);
        }

        #endregion

        #region GETTER

        /// <summary>
        /// GET Pois List
        /// </summary>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page</param>
        /// <param name="activitytype">Type of the Poi (possible values: STRINGS: 'Ärzte, Apotheken','Geschäfte und Dienstleister','Kultur und Sehenswürdigkeiten','Nachtleben und Unterhaltung','Öffentliche Einrichtungen','Sport und Freizeit','Verkehr und Transport' : BITMASK also possible: 'Ärtze, Apotheken = 1','Geschäfte und Dienstleister = 2','Kultur und Sehenswürdigkeiten = 4','Nachtleben und Unterhaltung = 8','Öffentliche Einrichtungen = 16','Sport und Freizeit = 32','Verkehr und Transport = 64')</param>
        /// <param name="subtypefilter">Subtype of the Poi ('null' = Filter disabled, available Subtypes depends on the activitytype BITMASK)</param>
        /// <param name="idfilter">IDFilter (Separator ',' List of Activity IDs, 'null' = No Filter)</param>
        /// <param name="locfilter">Locfilter (Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction), 'null' = No Filter)</param>
        /// <param name="areafilter">AreaFilter (Separator ',' IDList of AreaIDs separated by ',', 'null' : Filter disabled)</param>
        /// <param name="highlightfilter">Highlight Filter (Show only Highlights possible values: 'true' : show only Highlight Pois, 'null' Filter disabled)</param>
        /// <param name="active">Active Filter (possible Values: 'null' Displays all Pois, 'true' only Active Pois, 'false' only Disabled Pois</param>
        /// <param name="smgactive">SMGActive Filter (possible Values: 'null' Displays all Pois, 'true' only SMG Active Pois, 'false' only SMG Disabled Pois</param>
        /// <param name="smgtags">SMGTag Filter (String, Separator ',' more SMGTags possible, 'null' = No Filter)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting</param>
        /// <returns>Result Object with Collection of Pois</returns>
        private Task<IActionResult> GetFiltered(
            string[] fields, string? language, uint pagenumber, int? pagesize, string? activitytype, string? subtypefilter,
            string? idfilter, string? searchfilter, string? locfilter, string? areafilter, bool? highlightfilter, bool? active,
            bool? smgactive, string? smgtags, string? seed, string? lastchange, string? langfilter, PGGeoSearchResult geosearchresult, string? rawfilter, string? rawsort, bool removenullvalues,
            CancellationToken cancellationToken)
        {

            return DoAsyncReturn(async () =>
            {
                PoiHelper myactivityhelper = await PoiHelper.CreateAsync(
                    QueryFactory, poitype: activitytype, subtypefilter: subtypefilter, idfilter: idfilter,
                    locfilter: locfilter, areafilter: areafilter, highlightfilter: highlightfilter, activefilter: active,
                    smgactivefilter: smgactive, smgtags: smgtags, lastchange: lastchange, langfilter: langfilter, cancellationToken: cancellationToken);

                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From("pois")
                        .PoiWhereExpression(
                            idlist: myactivityhelper.idlist, poitypelist: myactivityhelper.poitypelist,
                            subtypelist: myactivityhelper.subtypelist, smgtaglist: myactivityhelper.smgtaglist,
                            districtlist: new List<string>(), municipalitylist: new List<string>(),
                            tourismvereinlist: myactivityhelper.tourismvereinlist, regionlist: myactivityhelper.regionlist,
                            arealist: myactivityhelper.arealist, highlight: myactivityhelper.highlight,
                            activefilter: myactivityhelper.active, smgactivefilter: myactivityhelper.smgactive,
                            searchfilter: searchfilter, language: language, lastchange: myactivityhelper.lastchange, languagelist: myactivityhelper.languagelist,
                            filterClosedData: FilterClosedData
                        )
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

        /// <summary>
        /// GET Single Poi
        /// </summary>
        /// <param name="id">ID of Poi</param>
        /// <returns>Poi Object</returns>
        private Task<IActionResult> GetSingle(string id, 
            string? language, 
            string[] fields,
            bool removenullvalues,
            CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("pois")
                        .Select("data")
                        .Where("id", id)
                        .When(FilterClosedData, q => q.FilterClosedData());

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, userroles: UserRolesList);
            });
        }

        #endregion

        #region CATEGORIES

        /// <summary>
        /// GET Poi Types List
        /// </summary>
        /// <returns>Collection of PoiTypes Object</returns>
        private Task<IActionResult> GetPoiTypesList(string? language, string[] fields, string? searchfilter, string? rawfilter, string? rawsort, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                     QueryFactory.Query("poitypes")
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

        /// <summary>
        /// GET Poi Types Single
        /// </summary>
        /// <returns>PoiTypes Object</returns>
        private Task<IActionResult> GetPoiTypesSingleAsync(string id, string? language, string[] fields, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("poitypes")
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
        /// POST Insert new Poi
        /// </summary>
        /// <param name="poi">Poi Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,PoiManager,PoiCreate")]
        [HttpPost, Route("Poi")]
        public Task<IActionResult> Post([FromBody] LTSPoiLinked poi)
        {
            return DoAsyncReturn(async () =>
            {
                poi.Id = !String.IsNullOrEmpty(poi.Id) ? poi.Id.ToUpper() : "noId";
                return await UpsertData<LTSPoiLinked>(poi, "pois");
            });
        }

        /// <summary>
        /// PUT Modify existing Poi
        /// </summary>
        /// <param name="id">Poi Id</param>
        /// <param name="poi">Poi Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataModify,PoiManager,PoiModify")]
        [HttpPut, Route("Poi/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] LTSPoiLinked poi)
        {
            return DoAsyncReturn(async () =>
            {
                poi.Id = id.ToUpper();
                return await UpsertData<LTSPoiLinked>(poi, "pois");
            });
        }

        /// <summary>
        /// DELETE Poi by Id
        /// </summary>
        /// <param name="id">Poi Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataDelete,PoiManager,PoiDelete")]
        [HttpDelete, Route("Poi/{id}")]
        public Task<IActionResult> Delete(string id)
        {
            return DoAsyncReturn(async () =>
            {
                id = id.ToUpper();
                return await DeleteData(id, "pois");
            });
        }

        #endregion

    }

}
