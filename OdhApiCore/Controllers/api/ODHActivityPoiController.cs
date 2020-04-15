using Helper;
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
        public ODHActivityPoiController(IWebHostEnvironment env, ISettings settings, ILogger<ActivityController> logger, IPostGreSQLConnectionFactory connectionFactory, Factories.PostgresQueryFactory queryFactory)
           : base(env, settings, logger, connectionFactory, queryFactory)
        {
        }

        #region SWAGGER Exposed API

        //Standard GETTER

        /// <summary>
        /// GET ODHActivityPoi List
        /// </summary>
        /// <param name="pagenumber">Pagenumber, (default:1)</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, not provided disables Random Sorting, (default:'null') </param>
        /// <param name="type">Type of the ODHActivityPoi ('null' = Filter disabled, possible values: BITMASK: 1 = Wellness, 2 = Winter, 4 = Summer, 8 = Culture, 16 = Other, 32 = Gastronomy), (default: 63 == ALL), REFERENCE TO: GET /api/ODHActivityPoiTypes </param>
        /// <param name="subtype">Subtype of the ODHActivityPoi ('null' = Filter disabled, BITMASK Filter, available SubTypes depends on the selected Maintype reference to ODHActivityPoiTypes)</param>
        /// <param name="poitype">Additional Type of the ODHActivityPoi ('null' = Filter disabled, BITMASK Filter, available SubTypes depends on the selected Maintype, SubType reference to ODHActivityPoiTypes)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of ODHActivityPoi IDs), (default:'null')</param>
        /// <param name="locfilter">Locfilter (Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMASSOCIATIONID = (Filter by Tourismassociation), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction), 'null' = No Filter), (default:'null')</param>
        /// <param name="areafilter">AreaFilter (Alternate Locfilter, can be combined with locfilter) (Separator ',' possible values: reg + REGIONID = (Filter by Region), tvs + TOURISMASSOCIATIONID = (Filter by Tourismassociation), skr + SKIREGIONID = (Filter by Skiregion), ska + SKIAREAID = (Filter by Skiarea), are + AREAID = (Filter by LTS Area), 'null' = No Filter), (default:'null')</param>
        /// <param name="langfilter">ODHActivityPoi Langfilter (returns only SmgPois available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="highlight">Hightlight Filter (possible values: 'false' = only ODHActivityPoi with Highlight false, 'true' = only ODHActivityPoi with Highlight true), (default:'null')</param>
        /// <param name="source">Source Filter (possible Values: 'null' Displays all ODHActivityPoi, 'None', 'ActivityData', 'PoiData', 'GastronomicData', 'MuseumData', 'Magnolia', 'Content', 'SuedtirolWein', 'ArchApp' (default:'null')</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="odhtagfilter">ODH Taglist Filter (refers to Array SmgTags) (String, Separator ',' more Tags possible, available Tags reference to 'api/ODHTag?validforentity=smgpoi'), (default:'null')</param>        
        /// <param name="active">Active ODHActivityPoi Filter (possible Values: 'true' only active ODHActivityPoi, 'false' only not active ODHActivityPoi, (default:'null')</param>        
        /// <param name="odhactive">ODH Active (Published) ODHActivityPoi Filter (Refers to field SmgActive) (possible Values: 'true' only published ODHActivityPoi, 'false' only not published ODHActivityPoi, (default:'null')</param>        
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="updatefrom">Date from Format (yyyy-MM-dd) (all GBActivityPoi with LastChange >= datefrom are passed), (default: null)</param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null)</param>
        /// <returns>Collection of ODH Activity Poi Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<ODHActivityPoi>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("api/ODHActivityPoi")]
        public async Task<IActionResult> GetODHActivityPoiList(
            string? language = null,
            uint pagenumber = 1,
            uint pagesize = 10,
            string? type = "63",
            string? subtype = null,
            string? poitype = null,
            string? idlist = null,
            string? locfilter = null,
            string? langfilter = null,
            string? areafilter = null,
            LegacyBool highlight = null,
            string? source = null,
            string? odhtagfilter = null,
            LegacyBool odhactive = null,
            LegacyBool active = null,
            string? lastchange = null,
            string? seed = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            return await GetFiltered(
                fields: fields ?? Array.Empty<string>(), language: language, pagenumber: pagenumber, pagesize: pagesize,
                type: type, subtypefilter: subtype, poitypefilter: poitype, searchfilter: searchfilter, idfilter: idlist, languagefilter: langfilter, 
                sourcefilter: source, locfilter: locfilter, areafilter: areafilter, highlightfilter: highlight?.Value, active: active?.Value,
                smgactive: odhactive?.Value, smgtags: odhtagfilter, seed: seed, lastchange: lastchange, geosearchresult, cancellationToken);
        }

        /// <summary>
        /// GET ODHActivityPoi Single 
        /// </summary>
        /// <param name="id">ID of the Poi</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <returns>ODHActivityPoi Object</returns>
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ODHActivityPoi), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("api/ODHActivityPoi/{id}")]
        public async Task<IActionResult> GetODHActivityPoiSingle(
            string id, 
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            CancellationToken cancellationToken = default)
        {
            return await GetSingle(id, language, fields: fields ?? Array.Empty<string>(), cancellationToken);
        }

        //Special GETTER

        /// <summary>
        /// GET ODHActivityPoi Types List
        /// </summary>
        /// <returns>Collection of ActivityPoiType Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<SmgPoiTypes>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("api/ODHActivityPoiTypes")]
        public async Task<IActionResult> GetAllODHActivityPoiTypesList()
        {
            return await GetSmgPoiTypesList();
        }

        /// <summary>
        /// GET ODHActivityPoi Types Single
        /// </summary>
        /// <returns>ActivityPoiType Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(SmgPoiTypes), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("api/ODHActivityPoiTypes/{*id}")]
        public async Task<IActionResult> GetAllODHActivityPoiTypesSingle(string id)
        {
            return await GetSmgPoiTypesSingle(id);
        }


        #endregion

        #region GETTER

        private Task<IActionResult> GetFiltered(string[] fields, string? language, uint pagenumber, uint pagesize,
            string? type, string? subtypefilter, string? poitypefilter, string? searchfilter, string? idfilter, string? languagefilter, string? sourcefilter, string? locfilter, 
            string? areafilter, bool? highlightfilter, bool? active, bool? smgactive, string? smgtags, string? seed, string? lastchange, PGGeoSearchResult geosearchresult, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async connectionFactory =>
            {
                ODHActivityPoiHelper myodhactivitypoihelper = await ODHActivityPoiHelper.CreateAsync(
                    connectionFactory, type, subtypefilter, poitypefilter, idfilter, locfilter, areafilter, languagefilter, sourcefilter, highlightfilter, active, smgactive, smgtags, lastchange,
                    cancellationToken, QueryFactory);

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
                            searchfilter: searchfilter, language: language, lastchange: myodhactivitypoihelper.lastchange)
                        .OrderBySeed(ref seed, "data ->>'Shortname' ASC")
                        .GeoSearchFilterAndOrderby(geosearchresult);

                // Get paginated data
                var data =
                    await query
                        .PaginateAsync<JsonRaw>(
                            page: (int)pagenumber,
                            perPage: (int)pagesize);

                var dataTransformed =
                    data.List.Select(
                        raw => raw.TransformRawData(language, fields, checkCC0: CheckCC0License)
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

        private Task<IActionResult> GetSingle(string id, string? language, string[] fields, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async connectionFactory =>
            {
                var query =
                    QueryFactory.Query("smgpois")
                        .Select("data")
                        .Where("id", id);

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data?.TransformRawData(language, fields, checkCC0: CheckCC0License);
            });
        }

        #endregion

        #region CUSTOM METODS

        private Task<IActionResult> GetSmgPoiTypesList()
        {
            return DoAsyncReturn(async connectionFactory =>
            {
                var query =
                    QueryFactory.Query("smgpoitypes")
                        .SelectRaw("data");

                var data = await query.GetAsync<JsonRaw?>();

                return data;
            });
        }

        private Task<IActionResult> GetSmgPoiTypesSingle(string id)
        {
            return DoAsyncReturn(async connectionFactory =>
            {
                var query =
                    QueryFactory.Query("smgpoitypes")
                        .Select("data")
                        .WhereJsonb("Key", id);
                //.Where("Key", "ILIKE", id);

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data;
            });
        }


        #endregion
    }
}
