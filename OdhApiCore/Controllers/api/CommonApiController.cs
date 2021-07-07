using DataModel;
using Helper;
using Microsoft.AspNetCore.Authorization;
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
    public class CommonController : OdhController
    {
        public CommonController(IWebHostEnvironment env, ISettings settings, ILogger<CommonController> logger, QueryFactory queryFactory)
       : base(env, settings, logger, queryFactory)
        {
        }

        #region SWAGGER Exposed API        

        //Standard GETTER

        /// <summary>
        /// GET MetaRegion List
        /// </summary>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null)<a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false</param>
        /// <returns>Collection of MetaRegion Objects</returns>     
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<MetaRegion>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,CommonReader")]
        [HttpGet, Route("MetaRegion")]
        public async Task<IActionResult> GetMetaRegions(
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null,
            string? updatefrom = null,
            string? seed = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {                        
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, null, null, null, null, null, cancellationToken);

            return await CommonGetListHelper(tablename: "metaregions", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), 
                language: language, commonhelper, geosearchresult:  geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET MetaRegion Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false</param>
        /// <returns>MetaRegion Object</returns>        
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(MetaRegion), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("MetaRegion/{id}", Name = "SingleMetaRegion")]
        public async Task<IActionResult> GetMetaRegionSingle(
            string id,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {            
            return await CommonGetSingleHelper(id: id, tablename: "metaregions", fields: fields ?? Array.Empty<string>(), language: language, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Experiencearea List
        /// </summary>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="visibleinsearch">Filter only Elements flagged with visibleinsearch: (possible values: 'true','false'), (default:'false')</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null)<a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false</param>
        /// <returns>Collection of ExperienceArea Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<ExperienceArea>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("ExperienceArea")]
        public async Task<IActionResult> GetExperienceAreas(
            bool? visibleinsearch = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null, 
            string? seed = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {          
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, visibleinsearch, null, null, null, null, cancellationToken);

            return await CommonGetListHelper(tablename: "experienceareas", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), 
                language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET ExperienceArea Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false</param>
        /// <returns>ExperienceArea Object</returns>        
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ExperienceArea), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("ExperienceArea/{id}", Name ="SingleExperienceArea")]
        public async Task<IActionResult> GetExperienceAreaSingle(string id,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "experienceareas", fields: fields ?? Array.Empty<string>(), language: language, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Region List
        /// </summary>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null)<a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false</param>
        /// <returns>Collection of Region Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<Region>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Region")]
        public async Task<IActionResult> GetRegions(
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null,
            string? seed = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, null, null, null, null, null, cancellationToken);

            return await CommonGetListHelper(tablename: "regions", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), 
                language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Region Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false</param>
        /// <returns>Region Object</returns>        
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(Region), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Region/{id}", Name ="SingleRegion")]
        public async Task<IActionResult> GetRegionSingle(string id,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "regions", fields: fields ?? Array.Empty<string>(), language: language, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET TourismAssociation List
        /// </summary>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null)<a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false</param>
        /// <returns>Collection of Tourismverein Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<Tourismverein>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("TourismAssociation")]
        public async Task<IActionResult> GetTourismverein(
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null,
            string? seed = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, null, null, null, null, null, cancellationToken);

            return await CommonGetListHelper(tablename: "tvs", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), 
                language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);

        }

        /// <summary>
        /// GET TourismAssociation Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false</param>
        /// <returns>Tourismverein Object</returns>        
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(Tourismverein), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("TourismAssociation/{id}", Name = "SingleTourismAssociation")]
        public async Task<IActionResult> GetTourismvereinSingle(string id,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "tvs", fields: fields ?? Array.Empty<string>(), language: language, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Municipality List
        /// </summary>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="visibleinsearch">Filter only Elements flagged with visibleinsearch: (possible values: 'true','false'), (default:'false')</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null)<a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false</param>
        /// <returns>Collection of Municipality Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<Municipality>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Municipality")]
        public async Task<IActionResult> GetMunicipality(
            bool? visibleinsearch,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null,
            string? seed = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, visibleinsearch, null, null, null, null, cancellationToken);

            return await CommonGetListHelper(tablename: "municipalities", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), 
                language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);

        }

        /// <summary>
        /// GET Municipality Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false</param>
        /// <returns>Municipality Object</returns>        
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(Municipality), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Municipality/{id}", Name = "SingleMunicipality")]
        public async Task<IActionResult> GetMunicipalitySingle(string id, 
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "municipalities", fields: fields ?? Array.Empty<string>(), language: language, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET District List
        /// </summary>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="visibleinsearch">Filter only Elements flagged with visibleinsearch: (possible values: 'true','false'), (default:'false')</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null)<a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false</param>
        /// <returns>Collection of District Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<District>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("District")]
        public async Task<IActionResult> GetDistrict(
            bool? visibleinsearch,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null,
            string? seed = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, visibleinsearch, null, null, null, null, cancellationToken);

            return await CommonGetListHelper(tablename: "districts", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), 
                language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET District Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false</param>
        /// <returns>District Object</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(District), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("District/{id}", Name = "SingleDistrict")]
        public async Task<IActionResult> GetDistrictSingle(string id,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "districts", fields: fields ?? Array.Empty<string>(), language: language, removenullvalues: removenullvalues, cancellationToken);
        }


        /// <summary>
        /// GET Area List
        /// </summary>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null)<a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false</param>
        /// <returns>Collection of Area Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<Area>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Area")]
        public async Task<IActionResult> GetAreas(
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null,
            string? seed = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, null, null, null, null, null, cancellationToken);

            return await CommonGetListHelper(tablename: "areas", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), 
                language: language, commonhelper, geosearchresult: null, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Area Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false</param>
        /// <returns>Area Object</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(Area), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Area/{id}", Name = "SingleArea")]
        public async Task<IActionResult> GetAreaSingle(string id, 
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "areas", fields: fields ?? Array.Empty<string>(), language: language, removenullvalues : removenullvalues, cancellationToken);
        }


        /// <summary>
        /// GET SkiRegion List
        /// </summary>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null)<a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false</param>
        /// <returns>Collection of SkiRegion Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<SkiRegion>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("SkiRegion")]
        public async Task<IActionResult> GetSkiRegion(
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null,
            string? seed = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, null, null, null, null, null, cancellationToken);

            return await CommonGetListHelper(tablename: "skiregions", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), 
                language: language, commonhelper, geosearchresult: null, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET SkiRegion Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false</param>
        /// <returns>SkiRegion Object</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(SkiRegion), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("SkiRegion/{id}", Name ="SingleSkiRegion")]
        public async Task<IActionResult> GetSkiRegionSingle(string id, 
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "skiregions", fields: fields ?? Array.Empty<string>(), language: language, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET SkiArea List
        /// </summary>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null)<a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false</param>
        /// <returns>Collection of SkiArea Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<SkiArea>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("SkiArea")]
        public async Task<IActionResult> GetSkiArea(
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null,
            string? seed = null,
            string? searchfilter = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, null, null, null, null, null, cancellationToken);

            return await CommonGetListHelper(tablename: "skiareas", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), 
                language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET SkiArea Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false</param>
        /// <returns>SkiArea Object</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(SkiArea), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("SkiArea/{id}", Name ="SingleSkiArea")]
        public async Task<IActionResult> GetSkiAreaSingle(string id,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "skiareas", fields: fields ?? Array.Empty<string>(), language: language, removenullvalues: removenullvalues, cancellationToken);
        }


        ////Special GETTER

        /// <summary>
        /// GET Wine Awards List
        /// </summary>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="companyid">Company Id</param>
        /// <param name="wineid">WineId</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null)<a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false</param>
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<Wine>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("WineAward")]
        public async Task<IActionResult> GetWineAwardsList(
            string? wineid = null,
            string? companyid = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null,
            string? seed = null,
            string? rawfilter = null,
            string? rawsort = null,
            string? searchfilter = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default
            )
        {
            WineHelper commonhelper = await WineHelper.CreateAsync(QueryFactory, null, companyid, wineid, null, null, null, null, null, null, cancellationToken);

            return await WineGetListHelper(tablename: "wines", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), 
                language: language, commonhelper, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Wine Award Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false</param>
        /// <returns>Wine Object</returns>        
        [ProducesResponseType(typeof(Wine), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("WineAward/{id}", Name ="SingleWineAward")]
        public async Task<IActionResult> GetWineAwardsSingle(string id, 
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "wines", fields: fields ?? Array.Empty<string>(), language: language, removenullvalues: removenullvalues, cancellationToken);
        }

        #endregion

        #region GETTER

        private Task<IActionResult> CommonGetListHelper(string tablename, string? seed, string? searchfilter, string[] fields, string? language, CommonHelper commonhelper, PGGeoSearchResult geosearchresult, string? rawfilter, string? rawsort, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {               
                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From(tablename)
                        .CommonWhereExpression(languagelist: new List<string>(), lastchange: commonhelper.lastchange, visibleinsearch: commonhelper.visibleinsearch, activefilter: commonhelper.active, odhactivefilter: commonhelper.smgactive,
                                               searchfilter: searchfilter, language: language, filterClosedData: FilterClosedData)
                        .ApplyRawFilter(rawfilter)
                        .ApplyOrdering_GeneratedColumns(ref seed, geosearchresult, rawsort); //.ApplyOrdering(ref seed, new PGGeoSearchResult() { geosearch = false }, rawsort);

                // Get paginated data
                var data =
                    await query
                        .GetAsync<JsonRaw>();

                var dataTransformed =
                    data.Select(
                        raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, userroles: UserRolesList)
                    );

                return dataTransformed;
            });
        }

        private Task<IActionResult> WineGetListHelper(string tablename, string? seed, string? searchfilter, string[] fields, string? language, WineHelper winehelper, string? rawfilter, string? rawsort, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From(tablename)
                        .WineWhereExpression(languagelist: new List<string>(), lastchange: winehelper.lastchange, wineid: winehelper.wineidlist, companyid: winehelper.companyidlist,
                                               searchfilter: searchfilter, language: language, filterClosedData: FilterClosedData)
                        .ApplyRawFilter(rawfilter)
                        .ApplyOrdering(ref seed, new PGGeoSearchResult() { geosearch = false }, rawsort);

                // Get paginated data
                var data =
                    await query
                        .GetAsync<JsonRaw>();

                var dataTransformed =
                    data.Select(
                        raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, userroles: UserRolesList)
                    );

                return dataTransformed;
            });
        }

        private Task<IActionResult> CommonGetSingleHelper(string id, string tablename, string[] fields, string? language, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query(tablename)
                        .Select("data")
                        .Where("id", id)
                        .When(FilterClosedData, q => q.FilterClosedData());

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, userroles: UserRolesList);
            });
        }

        #endregion

        #region POST PUT DELETE

        /// <summary>
        /// POST Insert new MetaRegion
        /// </summary>
        /// <param name="data">MetaRegion Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,CommonManager,CommonCreate")]
        [HttpPost, Route("MetaRegion")]
        public Task<IActionResult> Post([FromBody] MetaRegionLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                data.Id = !String.IsNullOrEmpty(data.Id) ? data.Id.ToUpper() : "noId";
                return await UpsertData<MetaRegionLinked>(data, "metaregions");
            });
        }

        /// <summary>
        /// POST Insert new Region
        /// </summary>
        /// <param name="data">Region Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,CommonManager,CommonCreate")]
        [HttpPost, Route("Region")]
        public Task<IActionResult> Post([FromBody] RegionLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                data.Id = !String.IsNullOrEmpty(data.Id) ? data.Id.ToUpper() : "noId";
                return await UpsertData<RegionLinked>(data, "regions");
            });
        }

        /// <summary>
        /// POST Insert new ExperienceArea
        /// </summary>
        /// <param name="data">ExperienceArea Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,CommonManager,CommonCreate")]
        [HttpPost, Route("ExperienceArea")]
        public Task<IActionResult> Post([FromBody] ExperienceAreaLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                data.Id = !String.IsNullOrEmpty(data.Id) ? data.Id.ToUpper() : "noId";
                return await UpsertData<ExperienceAreaLinked>(data, "experienceareas");
            });
        }

        /// <summary>
        /// POST Insert new TourismAssociation
        /// </summary>
        /// <param name="data">TourismAssociation Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,CommonManager,CommonCreate")]
        [HttpPost, Route("TourismAssociation")]
        public Task<IActionResult> Post([FromBody] TourismvereinLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                data.Id = !String.IsNullOrEmpty(data.Id) ? data.Id.ToUpper() : "noId";
                return await UpsertData<TourismvereinLinked>(data, "tvs");
            });
        }

        /// <summary>
        /// POST Insert new Municipality
        /// </summary>
        /// <param name="data">Municipality Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,CommonManager,CommonCreate")]
        [HttpPost, Route("Municipality")]
        public Task<IActionResult> Post([FromBody] MunicipalityLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                data.Id = !String.IsNullOrEmpty(data.Id) ? data.Id.ToUpper() : "noId";
                return await UpsertData<MunicipalityLinked>(data, "municipalities");
            });
        }

        /// <summary>
        /// POST Insert new District
        /// </summary>
        /// <param name="data">District Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,CommonManager,CommonCreate")]
        [HttpPost, Route("District")]
        public Task<IActionResult> Post([FromBody] DistrictLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                data.Id = !String.IsNullOrEmpty(data.Id) ? data.Id.ToUpper() : "noId";
                return await UpsertData<DistrictLinked>(data, "districts");
            });
        }

        /// <summary>
        /// POST Insert new Area
        /// </summary>
        /// <param name="data">Area Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,CommonManager,CommonCreate")]
        [HttpPost, Route("Area")]
        public Task<IActionResult> Post([FromBody] AreaLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                data.Id = !String.IsNullOrEmpty(data.Id) ? data.Id.ToUpper() : "noId";
                return await UpsertData<AreaLinked>(data, "areas");
            });
        }

        /// <summary>
        /// POST Insert new SkiRegion
        /// </summary>
        /// <param name="data">SkiRegion Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,CommonManager,CommonCreate")]
        [HttpPost, Route("SkiRegion")]
        public Task<IActionResult> Post([FromBody] SkiRegionLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                data.Id = !String.IsNullOrEmpty(data.Id) ? data.Id.ToUpper() : "noId";
                return await UpsertData<SkiRegionLinked>(data, "skiregions");
            });
        }

        /// <summary>
        /// POST Insert new SkiArea
        /// </summary>
        /// <param name="data">SkiArea Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,CommonManager,CommonCreate")]
        [HttpPost, Route("SkiArea")]
        public Task<IActionResult> Post([FromBody] SkiAreaLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                data.Id = !String.IsNullOrEmpty(data.Id) ? data.Id.ToUpper() : "noId";
                return await UpsertData<SkiAreaLinked>(data, "skiareas");
            });
        }

        /// <summary>
        /// POST Insert new Wine
        /// </summary>
        /// <param name="data">Wine Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,CommonManager,CommonCreate")]
        [HttpPost, Route("WineAward")]
        public Task<IActionResult> Post([FromBody] Wine data)
        {
            return DoAsyncReturn(async () =>
            {
                data.Id = !String.IsNullOrEmpty(data.Id) ? data.Id.ToUpper() : "noId";
                return await UpsertData<Wine>(data, "wines");
            });
        }

        /// <summary>
        /// PUT Modify existing MetaRegion
        /// </summary>
        /// <param name="id">MetaRegion Id</param>
        /// <param name="data">MetaRegion Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataModify,CommonManager,CommonModify")]
        [HttpPut, Route("MetaRegion/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] MetaRegionLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                data.Id = id.ToUpper();
                return await UpsertData<MetaRegionLinked>(data, "metaregions");
            });
        }

        /// <summary>
        /// PUT Modify existing Region
        /// </summary>
        /// <param name="id">Region Id</param>
        /// <param name="data">Region Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataModify,CommonManager,CommonModify")]
        [HttpPut, Route("Region/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] RegionLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                data.Id = id.ToUpper();
                return await UpsertData<RegionLinked>(data, "regions");
            });
        }

        /// <summary>
        /// PUT Modify existing ExperienceArea
        /// </summary>
        /// <param name="id">ExperienceArea Id</param>
        /// <param name="data">ExperienceArea Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataModify,CommonManager,CommonModify")]
        [HttpPut, Route("ExperienceArea/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] ExperienceAreaLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                data.Id = id.ToUpper();
                return await UpsertData<ExperienceAreaLinked>(data, "experienceareas");
            });
        }

        /// <summary>
        /// PUT Modify existing TourismAssociation
        /// </summary>
        /// <param name="id">TourismAssociation Id</param>
        /// <param name="data">TourismAssociation Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataModify,CommonManager,CommonModify")]
        [HttpPut, Route("TourismAssociation/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] TourismvereinLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                data.Id = id.ToUpper();
                return await UpsertData<TourismvereinLinked>(data, "tvs");
            });
        }

        /// <summary>
        /// PUT Modify existing Municipality
        /// </summary>
        /// <param name="id">Municipality Id</param>
        /// <param name="data">Municipality Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataModify,CommonManager,CommonModify")]
        [HttpPut, Route("Municipality/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] MunicipalityLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                data.Id = id.ToUpper();
                return await UpsertData<MunicipalityLinked>(data, "municipalities");
            });
        }

        /// <summary>
        /// PUT Modify existing District
        /// </summary>
        /// <param name="id">District Id</param>
        /// <param name="data">District Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataModify,CommonManager,CommonModify")]
        [HttpPut, Route("District/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] DistrictLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                data.Id = id.ToUpper();
                return await UpsertData<DistrictLinked>(data, "districts");
            });
        }

        /// <summary>
        /// PUT Modify existing Area
        /// </summary>
        /// <param name="id">Area Id</param>
        /// <param name="data">Area Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataModify,CommonManager,CommonModify")]
        [HttpPut, Route("Area/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] AreaLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                data.Id = id.ToUpper();
                return await UpsertData<AreaLinked>(data, "areas");
            });
        }

        /// <summary>
        /// PUT Modify existing SkiRegion
        /// </summary>
        /// <param name="id">SkiRegion Id</param>
        /// <param name="data">SkiRegion Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataModify,CommonManager,CommonModify")]
        [HttpPut, Route("SkiRegion/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] SkiRegionLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                data.Id = id.ToUpper();
                return await UpsertData<SkiRegionLinked>(data, "skiregions");
            });
        }

        /// <summary>
        /// PUT Modify existing SkiArea
        /// </summary>
        /// <param name="id">SkiArea Id</param>
        /// <param name="data">SkiArea Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataModify,CommonManager,CommonModify")]
        [HttpPut, Route("SkiArea/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] SkiAreaLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                data.Id = id.ToUpper();
                return await UpsertData<SkiAreaLinked>(data, "skiareas");
            });
        }

        /// <summary>
        /// PUT Modify existing WineAward
        /// </summary>
        /// <param name="id">WineAward Id</param>
        /// <param name="data">WineAward Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataModify,CommonManager,CommonModify")]
        [HttpPut, Route("WineAward/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] Wine data)
        {
            return DoAsyncReturn(async () =>
            {
                data.Id = id.ToUpper();
                return await UpsertData<Wine>(data, "wines");
            });
        }

        /// <summary>
        /// DELETE MetaRegion by Id
        /// </summary>
        /// <param name="id">MetaRegion Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataDelete,CommonManager,CommonDelete")]
        [HttpDelete, Route("MetaRegion/{id}")]
        public Task<IActionResult> DeleteMetaRegion(string id)
        {
            return DoAsyncReturn(async () =>
            {
                id = id.ToUpper();
                return await DeleteData(id, "metaregions");
            });
        }

        /// <summary>
        /// DELETE Region by Id
        /// </summary>
        /// <param name="id">Region Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataDelete,CommonManager,CommonDelete")]
        [HttpDelete, Route("Region/{id}")]
        public Task<IActionResult> DeleteRegion(string id)
        {
            return DoAsyncReturn(async () =>
            {
                id = id.ToUpper();
                return await DeleteData(id, "regions");
            });
        }

        /// <summary>
        /// DELETE ExperienceArea by Id
        /// </summary>
        /// <param name="id">ExperienceArea Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataDelete,CommonManager,CommonDelete")]
        [HttpDelete, Route("ExperienceArea/{id}")]
        public Task<IActionResult> DeleteExperienceArea(string id)
        {
            return DoAsyncReturn(async () =>
            {
                id = id.ToUpper();
                return await DeleteData(id, "experienceareas");
            });
        }

        /// <summary>
        /// DELETE TourismAssociation by Id
        /// </summary>
        /// <param name="id">TourismAssociation Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataDelete,CommonManager,CommonDelete")]
        [HttpDelete, Route("TourismAssociation/{id}")]
        public Task<IActionResult> DeleteTourismAssociation(string id)
        {
            return DoAsyncReturn(async () =>
            {
                id = id.ToUpper();
                return await DeleteData(id, "tvs");
            });
        }

        /// <summary>
        /// DELETE Municipality by Id
        /// </summary>
        /// <param name="id">Municipality Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataDelete,CommonManager,CommonDelete")]
        [HttpDelete, Route("Municipality/{id}")]
        public Task<IActionResult> DeleteMunicipality(string id)
        {
            return DoAsyncReturn(async () =>
            {
                id = id.ToUpper();
                return await DeleteData(id, "municipalities");
            });
        }

        /// <summary>
        /// DELETE District by Id
        /// </summary>
        /// <param name="id">District Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataDelete,CommonManager,CommonDelete")]
        [HttpDelete, Route("District/{id}")]
        public Task<IActionResult> DeleteDistrict(string id)
        {
            return DoAsyncReturn(async () =>
            {
                id = id.ToUpper();
                return await DeleteData(id, "districts");
            });
        }

        /// <summary>
        /// DELETE Area by Id
        /// </summary>
        /// <param name="id">Area Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataDelete,CommonManager,CommonDelete")]
        [HttpDelete, Route("Area/{id}")]
        public Task<IActionResult> DeleteArea(string id)
        {
            return DoAsyncReturn(async () =>
            {
                id = id.ToUpper();
                return await DeleteData(id, "areas");
            });
        }

        /// <summary>
        /// DELETE SkiRegion by Id
        /// </summary>
        /// <param name="id">SkiRegion Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataDelete,CommonManager,CommonDelete")]
        [HttpDelete, Route("SkiRegion/{id}")]
        public Task<IActionResult> DeleteSkiRegion(string id)
        {
            return DoAsyncReturn(async () =>
            {
                id = id.ToUpper();
                return await DeleteData(id, "skiregions");
            });
        }

        /// <summary>
        /// DELETE SkiArea by Id
        /// </summary>
        /// <param name="id">SkiArea Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataDelete,CommonManager,CommonDelete")]
        [HttpDelete, Route("SkiArea/{id}")]
        public Task<IActionResult> DeleteSkiArea(string id)
        {
            return DoAsyncReturn(async () =>
            {
                id = id.ToUpper();
                return await DeleteData(id, "skiareas");
            });
        }

        /// <summary>
        /// DELETE WineAward by Id
        /// </summary>
        /// <param name="id">WineAward Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataDelete,CommonManager,CommonDelete")]
        [HttpDelete, Route("WineAward/{id}")]
        public Task<IActionResult> DeleteWineAward(string id)
        {
            return DoAsyncReturn(async () =>
            {
                id = id.ToUpper();
                return await DeleteData(id, "wines");
            });
        }

        #endregion
    }
}
