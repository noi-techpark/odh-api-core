using DataModel;
using Helper;
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
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
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
            string? seed = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            CancellationToken cancellationToken = default)
        {                        
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, null, null, null, null, null, cancellationToken);

            return await CommonGetListHelper(tablename: "metaregions", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), language: language, commonhelper, geosearchresult:  geosearchresult, rawfilter: rawfilter, rawsort: rawsort, cancellationToken);
        }

        /// <summary>
        /// GET MetaRegion Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
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
            CancellationToken cancellationToken = default)
        {            
            return await CommonGetSingleHelper(id: id, tablename: "metaregions", fields: fields ?? Array.Empty<string>(), language: language, cancellationToken);
        }

        /// <summary>
        /// GET Experiencearea List
        /// </summary>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="visibleinsearch">Filter only Elements flagged with visibleinsearch: (possible values: 'true','false'), (default:'false')</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
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
            string? seed = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null, 
            CancellationToken cancellationToken = default)
        {          
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, visibleinsearch, null, null, null, null, cancellationToken);

            return await CommonGetListHelper(tablename: "experienceareas", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, cancellationToken);
        }

        /// <summary>
        /// GET ExperienceArea Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
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
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "experienceareas", fields: fields ?? Array.Empty<string>(), language: language, cancellationToken);
        }

        /// <summary>
        /// GET Region List
        /// </summary>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
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
            string? seed = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, null, null, null, null, null, cancellationToken);

            return await CommonGetListHelper(tablename: "regions", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, cancellationToken);
        }

        /// <summary>
        /// GET Region Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
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
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "regions", fields: fields ?? Array.Empty<string>(), language: language, cancellationToken);
        }

        /// <summary>
        /// GET TourismAssociation List
        /// </summary>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
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
            string? seed = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, null, null, null, null, null, cancellationToken);

            return await CommonGetListHelper(tablename: "tvs", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, cancellationToken);

        }

        /// <summary>
        /// GET TourismAssociation Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
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
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "tvs", fields: fields ?? Array.Empty<string>(), language: language, cancellationToken);
        }

        /// <summary>
        /// GET Municipality List
        /// </summary>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="visibleinsearch">Filter only Elements flagged with visibleinsearch: (possible values: 'true','false'), (default:'false')</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
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
            string? seed = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, visibleinsearch, null, null, null, null, cancellationToken);

            return await CommonGetListHelper(tablename: "municipalities", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, cancellationToken);

        }

        /// <summary>
        /// GET Municipality Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
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
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "municipalities", fields: fields ?? Array.Empty<string>(), language: language, cancellationToken);
        }

        /// <summary>
        /// GET District List
        /// </summary>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="visibleinsearch">Filter only Elements flagged with visibleinsearch: (possible values: 'true','false'), (default:'false')</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
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
            string? seed = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, visibleinsearch, null, null, null, null, cancellationToken);

            return await CommonGetListHelper(tablename: "districts", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, cancellationToken);
        }

        /// <summary>
        /// GET District Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
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
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "districts", fields: fields ?? Array.Empty<string>(), language: language, cancellationToken);
        }


        /// <summary>
        /// GET Area List
        /// </summary>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
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
            string? seed = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            CancellationToken cancellationToken = default)
        {
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, null, null, null, null, null, cancellationToken);

            return await CommonGetListHelper(tablename: "areas", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), language: language, commonhelper, geosearchresult: null, rawfilter: rawfilter, rawsort: rawsort, cancellationToken);
        }

        /// <summary>
        /// GET Area Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
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
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "areas", fields: fields ?? Array.Empty<string>(), language: language, cancellationToken);
        }


        /// <summary>
        /// GET SkiRegion List
        /// </summary>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
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
            string? seed = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            CancellationToken cancellationToken = default)
        {
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, null, null, null, null, null, cancellationToken);

            return await CommonGetListHelper(tablename: "skiregions", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), language: language, commonhelper, geosearchresult: null, rawfilter: rawfilter, rawsort: rawsort, cancellationToken);
        }

        /// <summary>
        /// GET SkiRegion Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
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
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "skiregions", fields: fields ?? Array.Empty<string>(), language: language, cancellationToken);
        }

        /// <summary>
        /// GET SkiArea List
        /// </summary>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
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
            string? seed = null,
            string? searchfilter = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            string? rawfilter = null,
            string? rawsort = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, null, null, null, null, null, cancellationToken);

            return await CommonGetListHelper(tablename: "skiareas", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, cancellationToken);
        }

        /// <summary>
        /// GET SkiArea Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
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
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "skiareas", fields: fields ?? Array.Empty<string>(), language: language, cancellationToken);
        }


        ////Special GETTER

        /// <summary>
        /// GET Wine Awards List
        /// </summary>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="companyid">Company Id</param>
        /// <param name="wineid">WineId</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
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
            string? seed = null,
            string? rawfilter = null,
            string? rawsort = null,
            string? searchfilter = null,
            CancellationToken cancellationToken = default
            )
        {
            WineHelper commonhelper = await WineHelper.CreateAsync(QueryFactory, null, companyid, wineid, null, null, null, null, null, null, cancellationToken);

            return await WineGetListHelper(tablename: "wines", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), language: language, commonhelper, rawfilter: rawfilter, rawsort: rawsort, cancellationToken);
        }

        /// <summary>
        /// GET Wine Award Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <returns>Wine Object</returns>        
        [ProducesResponseType(typeof(SkiArea), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("WineAward/{id}", Name ="SingleWineAward")]
        public async Task<IActionResult> GetWineAwardsSingle(string id, 
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "wines", fields: fields ?? Array.Empty<string>(), language: language, cancellationToken);
        }

        #endregion

        #region GETTER

        private Task<IActionResult> CommonGetListHelper(string tablename, string? seed, string? searchfilter, string[] fields, string? language, CommonHelper commonhelper, PGGeoSearchResult geosearchresult, string? rawfilter, string? rawsort, CancellationToken cancellationToken)
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
                        raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, urlGenerator: UrlGenerator, userroles: UserRolesList)
                    );

                return dataTransformed;
            });
        }

        private Task<IActionResult> WineGetListHelper(string tablename, string? seed, string? searchfilter, string[] fields, string? language, WineHelper winehelper, string? rawfilter, string? rawsort, CancellationToken cancellationToken)
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
                        raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, urlGenerator: UrlGenerator, userroles: UserRolesList)
                    );

                return dataTransformed;
            });
        }

        private Task<IActionResult> CommonGetSingleHelper(string id, string tablename, string[] fields, string? language, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query(tablename)
                        .Select("data")
                        .Where("id", id)
                        .When(FilterClosedData, q => q.FilterClosedData());

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, urlGenerator: UrlGenerator, userroles: UserRolesList);
            });
        }

        #endregion

        #region POST PUT DELETE

        #endregion
    }
}
