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
        public CommonController(IWebHostEnvironment env, ISettings settings, ILogger<ActivityController> logger, QueryFactory queryFactory)
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
            CancellationToken cancellationToken = default)
        {                        
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, null, null, null, null, null, cancellationToken);

            return await CommonGetListHelper(tablename: "metaregions", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), language: language, commonhelper, geosearchresult:  geosearchresult, cancellationToken);
        }

        /// <summary>
        /// GET MetaRegion Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <returns>MetaRegion Object</returns>        
        /// <response code="200">List created</response>
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
            bool? visibleinsearch = false,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? seed = null,
            string? searchfilter = null,
            CancellationToken cancellationToken = default)
        {          
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, visibleinsearch, null, null, null, null, cancellationToken);

            return await CommonGetListHelper(tablename: "experienceareas", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(), language: language, commonhelper, geosearchresult: geosearchresult, cancellationToken);
        }

        /// <summary>
        /// GET ExperienceArea Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <returns>ExperienceArea Object</returns>        
        /// <response code="200">List created</response>
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

        ///// <summary>
        ///// GET Region List
        ///// </summary>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        ///// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        ///// <returns>Collection of Region Objects</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Array of Region Objects", typeof(IEnumerable<Region>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("Region")]
        //[HttpGet, Route("api/Region")]
        //public IHttpActionResult GetRegions(
        //    int elements = 0,
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null,
        //    string fields = null,
        //    string language = null)
        //{
        //    var table = CheckOpenData(User, "regions");
        //    var fieldselector = !String.IsNullOrEmpty(fields) ? fields.Split(',') : null;

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    return GetRegionList(elements, geosearchresult, fieldselector, table, language);
        //}

        ///// <summary>
        ///// GET Region Single
        ///// </summary>
        ///// <param name="id">ID of the requested data</param>
        ///// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        ///// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        ///// <returns>Region Object</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Region Object", typeof(Region))]
        //[OpenData("Region")]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[HttpGet, Route("api/Region/{id}")]
        //public IHttpActionResult GetRegionSingle(string id, string fields = null, string language = null)
        //{
        //    var table = CheckOpenData(User, "regions");
        //    var fieldselector = !String.IsNullOrEmpty(fields) ? fields.Split(',') : null;

        //    return GetRegion(id, fieldselector, table, language);
        //}

        ///// <summary>
        ///// GET TourismAssociation List
        ///// </summary>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        ///// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        ///// <returns>Collection of Tourismverein Objects</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Array of Tourismverein Objects", typeof(IEnumerable<Tourismverein>))]
        //[OpenData("TourismAssociation")]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[HttpGet, Route("api/TourismAssociation")]
        //public IHttpActionResult GetTourismverein(
        //    int elements = 0,
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null,
        //    string fields = null,
        //    string language = null)
        //{
        //    var table = CheckOpenData(User, "tvs");
        //    var fieldselector = !String.IsNullOrEmpty(fields) ? fields.Split(',') : null;

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    return GetTourismvereinList(elements, geosearchresult, fieldselector, table, language);
        //}

        ///// <summary>
        ///// GET TourismAssociation Single
        ///// </summary>
        ///// <param name="id">ID of the requested data</param>
        ///// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        ///// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        ///// <returns>Tourismverein Object</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Tourismverein Object", typeof(Tourismverein))]
        //[OpenData("TourismAssociation")]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[HttpGet, Route("api/TourismAssociation/{id}")]
        //public IHttpActionResult GetTourismvereinSingle(string id, string fields = null, string language = null)
        //{
        //    var table = CheckOpenData(User, "tvs");
        //    var fieldselector = !String.IsNullOrEmpty(fields) ? fields.Split(',') : null;

        //    return GetTourismverein(id, fieldselector, table, language);
        //}

        ///// <summary>
        ///// GET Municipality List
        ///// </summary>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="visibleinsearch">Filter only Elements flagged with visibleinsearch: (possible values: 'true','false'), (default:'false')</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        ///// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        ///// <returns>Collection of Municipality Objects</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Array of Municipality Objects", typeof(IEnumerable<Municipality>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("Municipality")]
        //[HttpGet, Route("api/Municipality")]
        //public IHttpActionResult GetMunicipality(
        //    int elements = 0,
        //    string visibleinsearch = "false",
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null,
        //    string fields = null,
        //    string language = null)
        //{
        //    var table = CheckOpenData(User, "municipalities");
        //    var fieldselector = !String.IsNullOrEmpty(fields) ? fields.Split(',') : null;

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    if (visibleinsearch.ToLower() == "true")
        //        return GetMunicipalityListVisibleinSearch(elements, geosearchresult, fieldselector, table, language);
        //    else
        //        return GetMunicipalityList(elements, geosearchresult, fieldselector, table, language);
        //}

        ///// <summary>
        ///// GET Municipality Single
        ///// </summary>
        ///// <param name="id">ID of the requested data</param>
        ///// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        ///// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        ///// <returns>Municipality Object</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Municipality Object", typeof(Municipality))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("Municipality")]
        //[HttpGet, Route("api/Municipality/{id}")]
        //public IHttpActionResult GetMunicipalitySingle(string id, string fields = null, string language = null)
        //{
        //    var table = CheckOpenData(User, "municipalities");
        //    var fieldselector = !String.IsNullOrEmpty(fields) ? fields.Split(',') : null;

        //    return GetMunicipality(id, fieldselector, table, language);
        //}

        ///// <summary>
        ///// GET District List
        ///// </summary>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="visibleinsearch">Filter only Elements flagged with visibleinsearch: (possible values: 'true','false'), (default:'false')</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        ///// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        ///// <returns>Collection of District Objects</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Array of District Objects", typeof(IEnumerable<District>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("District")]
        //[HttpGet, Route("api/District")]
        //public IHttpActionResult GetDistrict(
        //    int elements = 0,
        //    string visibleinsearch = "false",
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null,
        //    string fields = null,
        //    string language = null)
        //{
        //    var table = CheckOpenData(User, "districts");
        //    var fieldselector = !String.IsNullOrEmpty(fields) ? fields.Split(',') : null;

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    if (visibleinsearch.ToLower() == "true")
        //        return GetDistrictListVisibleinSearch(elements, geosearchresult, fieldselector, table, language);
        //    else
        //        return GetDistrictList(elements, geosearchresult, fieldselector, table, language);
        //}

        ///// <summary>
        ///// GET District Single
        ///// </summary>
        ///// <param name="id">ID of the requested data</param>
        ///// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        ///// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        ///// <returns>District Object</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "District Object", typeof(District))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("District")]
        //[HttpGet, Route("api/District/{id}")]
        //public IHttpActionResult GetDistrictSingle(string id, string fields = null, string language = null)
        //{
        //    var table = CheckOpenData(User, "districts");
        //    var fieldselector = !String.IsNullOrEmpty(fields) ? fields.Split(',') : null;

        //    return GetDistrict(id, fieldselector, table, language);
        //}


        ///// <summary>
        ///// GET Area List
        ///// </summary>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        ///// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        ///// <returns>Collection of Area Objects</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Array of Area Objects", typeof(IEnumerable<Area>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("Area")]
        //[HttpGet, Route("api/Area")]
        //public IHttpActionResult GetAreas(
        //    int elements = 0,
        //    string fields = null,
        //    string language = null)
        //{
        //    var fieldselector = !String.IsNullOrEmpty(fields) ? fields.Split(',') : null;

        //    return GetAreaList(elements, fieldselector, language);
        //}

        ///// <summary>
        ///// GET Area Single
        ///// </summary>
        ///// <param name="id">ID of the requested data</param>
        ///// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        ///// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        ///// <returns>Area Object</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Area Object", typeof(Area))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("Area")]
        //[HttpGet, Route("api/Area/{id}")]
        //public IHttpActionResult GetAreaSingle(string id, string fields = null, string language = null)
        //{
        //    var fieldselector = !String.IsNullOrEmpty(fields) ? fields.Split(',') : null;

        //    return GetArea(id, fieldselector, language);
        //}


        ///// <summary>
        ///// GET SkiRegion List
        ///// </summary>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        ///// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        ///// <returns>Collection of SkiRegion Objects</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Array of SkiRegion Objects", typeof(IEnumerable<SkiRegion>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("SkiRegion")]
        //[HttpGet, Route("api/SkiRegion")]
        //public IHttpActionResult GetSkiRegion(
        //    int elements = 0,
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null,
        //    string fields = null,
        //    string language = null)
        //{
        //    var table = CheckOpenData(User, "skiregions");
        //    var fieldselector = !String.IsNullOrEmpty(fields) ? fields.Split(',') : null;

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    return GetSkiregionList(elements, geosearchresult, fieldselector, table, language);
        //}

        ///// <summary>
        ///// GET SkiRegion Single
        ///// </summary>
        ///// <param name="id">ID of the requested data</param>
        ///// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        ///// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        ///// <returns>SkiRegion Object</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "SkiRegion Object", typeof(SkiRegion))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("SkiRegion")]
        //[HttpGet, Route("api/SkiRegion/{id}")]
        //public IHttpActionResult GetSkiRegionSingle(string id, string fields = null, string language = null)
        //{
        //    var table = CheckOpenData(User, "skiregions");
        //    var fieldselector = !String.IsNullOrEmpty(fields) ? fields.Split(',') : null;

        //    return GetSkiregion(id, fieldselector, table, language);
        //}

        ///// <summary>
        ///// GET SkiArea List
        ///// </summary>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        ///// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        ///// <returns>Collection of SkiArea Objects</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Array of SkiArea Objects", typeof(IEnumerable<SkiArea>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("SkiArea")]
        //[HttpGet, Route("api/SkiArea")]
        //public IHttpActionResult GetSkiArea(
        //    int elements = 0,
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null,
        //    string fields = null,
        //    string language = null)
        //{
        //    var table = CheckOpenData(User, "skiareas");
        //    var fieldselector = !String.IsNullOrEmpty(fields) ? fields.Split(',') : null;

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    return GetSkiareaList(elements, geosearchresult, fieldselector, table, language);
        //}

        ///// <summary>
        ///// GET SkiArea Single
        ///// </summary>
        ///// <param name="id">ID of the requested data</param>
        ///// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        ///// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        ///// <returns>SkiArea Object</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "SkiArea Object", typeof(SkiArea))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("SkiArea")]
        //[HttpGet, Route("api/SkiArea/{id}")]
        //public IHttpActionResult GetSkiAreaSingle(string id, string fields = null, string language = null)
        //{
        //    var table = CheckOpenData(User, "skiareas");
        //    var fieldselector = !String.IsNullOrEmpty(fields) ? fields.Split(',') : null;

        //    return GetSkiarea(id, fieldselector, table, language);
        //}

        ////Localized GETTER

        ///// <summary>
        ///// GET MetaRegion Localized List
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <returns>Collection of MetaRegionLocalized Objects</returns>        
        //[Obsolete("Deprecated, use api/MetaRegion")]
        //[SwaggerResponse(HttpStatusCode.OK, "Array of MetaRegionLocalized Objects", typeof(IEnumerable<MetaRegionLocalized>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("MetaRegion")]
        //[HttpGet, Route("api/MetaRegionLocalized")]
        //public IHttpActionResult GetMetaRegionsLocalized(
        //    string language = "en",
        //    int elements = 0,
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null)
        //{
        //    var table = CheckOpenData(User, "metaregions");

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    return GetLocalizedMetaRegionList(language, elements, geosearchresult, table);
        //}

        ///// <summary>
        ///// GET MetaRegion Localized Single
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="id">ID of the requested data</param>
        ///// <returns>MetaRegionLocalized Object</returns>        
        //[Obsolete("Deprecated, use api/MetaRegion")]
        //[SwaggerResponse(HttpStatusCode.OK, "MetaRegionLocalized Object", typeof(MetaRegionLocalized))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("MetaRegion")]
        //[HttpGet, Route("api/MetaRegionLocalized/{id}")]
        //public IHttpActionResult GetMetaRegionSingleLocalized(string id, string language = "en")
        //{
        //    var table = CheckOpenData(User, "metaregions");

        //    return GetMetaRegionLocalized(language, id, table);
        //}

        ///// <summary>
        ///// GET ExperienceArea Localized List
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="visibleinsearch">Filter only Elements flagged with visibleinsearch: (possible values: 'true','false'), (default:'false')</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <returns>Collection of ExperienceArea Localized Objects</returns>        
        //[Obsolete("Deprecated, use api/ExperienceArea")]
        //[SwaggerResponse(HttpStatusCode.OK, "Array of ExperienceArea Localized Objects", typeof(IEnumerable<ExperienceAreaLocalized>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("ExperianceArea")]
        //[HttpGet, Route("api/ExperienceAreaLocalized")]
        //public IHttpActionResult GetExperienceAreasLocalized(
        //    string language = "en",
        //    int elements = 0,
        //    string visibleinsearch = "false",
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null)
        //{
        //    var table = CheckOpenData(User, "experienceareas");

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    if (visibleinsearch.ToLower() == "true")
        //        return GetLocalizedExperienceAreasVisibleinSearch(language, elements, geosearchresult, table);
        //    else
        //        return GetLocalizedExperienceAreaList(language, elements, geosearchresult, table);
        //}

        ///// <summary>
        ///// GET ExperienceArea Localized Single
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="id">ID of the requested data</param>
        ///// <returns>ExperienceArea Localized Object</returns>        
        //[Obsolete("Deprecated, use api/ExperienceArea")]
        //[SwaggerResponse(HttpStatusCode.OK, "ExperienceArea Localized Object", typeof(BaseInfosLocalized))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("ExperienceArea")]
        //[HttpGet, Route("api/ExperienceAreaLocalized/{id}")]
        //public IHttpActionResult GetExperienceAreaSingleLocalized(string id, string language = "en")
        //{
        //    var table = CheckOpenData(User, "experienceareas");

        //    return GetExperienceAreaLocalized(language, id, table);
        //}

        ///// <summary>
        ///// GET Region Localized List
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <returns>Collection of RegionLocalized Objects</returns>        
        //[Obsolete("Deprecated, use api/Region")]
        //[SwaggerResponse(HttpStatusCode.OK, "Array of RegionLocalized Objects", typeof(IEnumerable<RegionLocalized>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("Region")]
        //[HttpGet, Route("api/RegionLocalized")]
        //public IHttpActionResult GetRegionsLocalized(
        //    string language = "en",
        //    int elements = 0,
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null)
        //{
        //    var table = CheckOpenData(User, "regions");

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    return GetLocalizedRegionList(language, elements, geosearchresult, table);
        //}

        ///// <summary>
        ///// GET Region Localized Single
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="id">ID of the requested data</param>
        ///// <returns>RegionLocalized Object</returns>        
        //[Obsolete("Deprecated, use api/Region")]
        //[SwaggerResponse(HttpStatusCode.OK, "RegionLocalized Object", typeof(RegionLocalized))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("Region")]
        //[HttpGet, Route("api/RegionLocalized/{id}")]
        //public IHttpActionResult GetRegionSingleLocalized(string id, string language = "en")
        //{
        //    var table = CheckOpenData(User, "regions");

        //    return GetRegionLocalized(language, id, table);
        //}

        ///// <summary>
        ///// GET TourismAssociation Localized List
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <returns>Collection of TourismvereinLocalized Objects</returns>        
        //[Obsolete("Deprecated, use api/TourismAssociation")]
        //[SwaggerResponse(HttpStatusCode.OK, "Array of TourismvereinLocalized Objects", typeof(IEnumerable<TourismvereinLocalized>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("TourismAssociation")]
        //[HttpGet, Route("api/TourismAssociationLocalized")]
        //public IHttpActionResult GetTourismvereinLocalized(
        //    string language = "en",
        //    int elements = 0,
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null)
        //{
        //    var table = CheckOpenData(User, "tvs");

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    return GetLocalizedTourismvereinList(language, elements, geosearchresult, table);
        //}

        ///// <summary>
        ///// GET TourismAssociation Localized Single
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="id">ID of the requested data</param>
        ///// <returns>TourismvereinLocalized Object</returns>        
        //[Obsolete("Deprecated, use api/TourismAssociation")]
        //[SwaggerResponse(HttpStatusCode.OK, "TourismvereinLocalized Object", typeof(TourismvereinLocalized))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("TourismAssociation")]
        //[HttpGet, Route("api/TourismAssociationLocalized/{id}")]
        //public IHttpActionResult GetTourismvereinSingleLocalized(string id, string language = "en")
        //{
        //    var table = CheckOpenData(User, "tvs");

        //    return GetTourismvereinLocalized(language, id, table);
        //}

        ///// <summary>
        ///// GET Municipality Localized List
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="visibleinsearch">Filter only Elements flagged with visibleinsearch: (possible values: 'true','false'), (default:'false')</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <returns>Collection of MunicipalityLocalized Objects</returns>        
        //[Obsolete("Deprecated, use api/Municipality")]
        //[SwaggerResponse(HttpStatusCode.OK, "Array of MunicipalityLocalized Objects", typeof(IEnumerable<MunicipalityLocalized>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("Municipality")]
        //[HttpGet, Route("api/MunicipalityLocalized")]
        //public IHttpActionResult GetMunicipalityLocalized(
        //    string language = "en",
        //    int elements = 0,
        //    string visibleinsearch = "false",
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null)
        //{
        //    var table = CheckOpenData(User, "municipalities");

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    if (visibleinsearch.ToLower() == "true")
        //        return GetLocalizedMunicipalityListVisibleinSearch(language, elements, geosearchresult, table);
        //    else
        //        return GetLocalizedMunicipalityList(language, elements, geosearchresult, table);

        //}

        ///// <summary>
        ///// GET Municipality Localized Single
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="id">ID of the requested data</param>
        ///// <returns>MunicipalityLocalized Object</returns>        
        //[Obsolete("Deprecated, use api/Municipality")]
        //[SwaggerResponse(HttpStatusCode.OK, "MunicipalityLocalized Object", typeof(MunicipalityLocalized))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("Municipality")]
        //[HttpGet, Route("api/MunicipalityLocalized/{id}")]
        //public IHttpActionResult GetMunicipalitySingleLocalized(string id, string language = "en")
        //{
        //    var table = CheckOpenData(User, "municipalities");

        //    return GetMunicipalityLocalized(language, id, table);
        //}

        ///// <summary>
        ///// GET District Localized List
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="visibleinsearch">Filter only Elements flagged with visibleinsearch: (possible values: 'true','false'), (default:'false')</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <returns>Collection of DistrictLocalized Objects</returns>        
        //[Obsolete("Deprecated, use api/District")]
        //[SwaggerResponse(HttpStatusCode.OK, "Array of DistrictLocalized Objects", typeof(IEnumerable<DistrictLocalized>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("District")]
        //[HttpGet, Route("api/DistrictLocalized")]
        //public IHttpActionResult GetDistrictLocalized(
        //    string language = "en",
        //    int elements = 0,
        //    string visibleinsearch = "false",
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null)
        //{
        //    var table = CheckOpenData(User, "districts");

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    if (visibleinsearch.ToLower() == "true")
        //        return GetLocalizedDistrictListVisibleinSearch(language, elements, geosearchresult, table);
        //    else
        //        return GetLocalizedDistrictList(language, elements, geosearchresult, table);

        //}

        ///// <summary>
        ///// GET District Localized Single
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="id">ID of the requested data</param>
        ///// <returns>DistrictLocalized Object</returns>        
        //[Obsolete("Deprecated, use api/District")]
        //[SwaggerResponse(HttpStatusCode.OK, "DistrictLocalized Object", typeof(DistrictLocalized))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("District")]
        //[HttpGet, Route("api/DistrictLocalized/{id}")]
        //public IHttpActionResult GetDistrictSingleLocalized(string id, string language = "en")
        //{
        //    var table = CheckOpenData(User, "districts");

        //    return GetDistrictLocalized(language, id, table);
        //}

        ///// <summary>
        ///// GET SkiRegion Localized List
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <returns>Collection of SkiRegionLocalized Objects</returns>        
        //[Obsolete("Deprecated, use api/SkiRegion")]
        //[SwaggerResponse(HttpStatusCode.OK, "Array of SkiRegionLocalized Objects", typeof(IEnumerable<SkiRegionLocalized>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("SkiRegion")]
        //[HttpGet, Route("api/SkiRegionLocalized")]
        //public IHttpActionResult GetSkiRegionLocalized(
        //    string language = "en",
        //    int elements = 0,
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null)
        //{
        //    var table = CheckOpenData(User, "skiregions");

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    return GetLocalizedSkiRegionList(language, elements, geosearchresult, table);
        //}

        ///// <summary>
        ///// GET SkiRegion Localized Single
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="id">ID of the requested data</param>
        ///// <returns>SkiRegionLocalized Object</returns>        
        //[Obsolete("Deprecated, use api/SkiRegion")]
        //[SwaggerResponse(HttpStatusCode.OK, "SkiRegionLocalized Object", typeof(SkiRegionLocalized))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("SkiRegion")]
        //[HttpGet, Route("api/SkiRegionLocalized/{id}")]
        //public IHttpActionResult GetSkiRegionSingleLocalized(string id, string language = "en")
        //{
        //    var table = CheckOpenData(User, "skiregions");

        //    return GetSkiregionLocalized(language, id, table);
        //}

        ///// <summary>
        ///// GET SkiArea Localized List
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <returns>Collection of SkiAreaLocalized Objects</returns>        
        //[Obsolete("Deprecated, use api/SkiArea")]
        //[SwaggerResponse(HttpStatusCode.OK, "Array of SkiAreaLocalized Objects", typeof(IEnumerable<SkiAreaLocalized>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("SkiArea")]
        //[HttpGet, Route("api/SkiAreaLocalized")]
        //public IHttpActionResult GetSkiAreaLocalized(
        //    string language = "en",
        //    int elements = 0,
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null)
        //{
        //    var table = CheckOpenData(User, "skiareas");

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    return GetLocalizedSkiAreaList(language, elements, geosearchresult, table);
        //}

        ///// <summary>
        ///// GET SkiArea Localized Single
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="id">ID of the requested data</param>
        ///// <returns>SkiAreaLocalized Object</returns>        
        //[Obsolete("Deprecated, use api/SkiArea")]
        //[SwaggerResponse(HttpStatusCode.OK, "SkiAreaLocalized Object", typeof(SkiAreaLocalized))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("SkiArea")]
        //[HttpGet, Route("api/SkiAreaLocalized/{id}")]
        //public IHttpActionResult GetSkiAreaSingleLocalized(string id, string language = "en")
        //{
        //    var table = CheckOpenData(User, "skiareas");

        //    return GetSkiareaLocalized(language, id, table);
        //}


        ////Reduced GETTER

        ///// <summary>
        ///// GET MetaRegion Reduced List
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <returns>Collection of CommonReduced Objects</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Array of CommonReduced Objects", typeof(IEnumerable<CommonReduced>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("MetaRegion")]
        //[HttpGet, Route("api/MetaRegionReduced")]
        //public IHttpActionResult GetMetaRegionsReduced(
        //    string language = "en",
        //    int elements = 0,
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null)
        //{
        //    var table = CheckOpenData(User, "metaregions");

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    return GetReducedMetaRegionList(language, elements, geosearchresult, table);
        //}

        ///// <summary>
        ///// GET ExperienceArea Reduced List
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="visibleinsearch">Filter only Elements flagged with visibleinsearch: (possible values: 'true','false'), (default:'false')</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <returns>Collection of ExperienceAreaName Objects</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Array of ExperienceAreaName Objects", typeof(IEnumerable<ExperienceAreaName>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("ExperienceArea")]
        //[HttpGet, Route("api/ExperienceAreaReduced")]
        //public IHttpActionResult GetExperienceAreasReduced(
        //    string language = "en",
        //    int elements = 0,
        //    string visibleinsearch = "false",
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null)
        //{
        //    var table = CheckOpenData(User, "experienceareas");

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    if (visibleinsearch.ToLower() == "true")
        //        return GetReducedExperienceAreaListVisibleinSearch(language, elements, geosearchresult, table);
        //    else
        //        return GetReducedExperienceAreaList(language, elements, geosearchresult, table);
        //}

        ///// <summary>
        ///// GET Region Reduced List
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <returns>Collection of CommonReduced Objects</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Array of CommonReduced Objects", typeof(IEnumerable<CommonReduced>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("Region")]
        //[HttpGet, Route("api/RegionReduced")]
        //public IHttpActionResult GetRegionsReduced(
        //    string language = "en",
        //    int elements = 0,
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null)
        //{
        //    var table = CheckOpenData(User, "regions");

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    return GetReducedRegionList(language, elements, geosearchresult, table);
        //}

        ///// <summary>
        ///// GET TourismAssociation Reduced List
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <returns>Collection of CommonReduced Objects</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Array of CommonReduced Objects", typeof(IEnumerable<CommonReduced>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("TourismAssociation")]
        //[HttpGet, Route("api/TourismAssociationReduced")]
        //public IHttpActionResult GetTourismvereinReduced(
        //    string language = "en",
        //    int elements = 0,
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null)
        //{
        //    var table = CheckOpenData(User, "tvs");

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    return GetReducedTourismvereinList(language, elements, geosearchresult, table);
        //}

        ///// <summary>
        ///// GET Municipality Reduced List
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="visibleinsearch">Filter only Elements flagged with visibleinsearch: (possible values: 'true','false'), (default:'false')</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <returns>Collection of CommonReduced Objects</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Array of CommonReduced Objects", typeof(IEnumerable<CommonReduced>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("Municipality")]
        //[HttpGet, Route("api/MunicipalityReduced")]
        //public IHttpActionResult GetMunicipalityReduced(
        //    string language = "en",
        //    int elements = 0,
        //    string visibleinsearch = "false",
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null)
        //{
        //    var table = CheckOpenData(User, "municipalities");

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    if (visibleinsearch.ToLower() == "true")
        //        return GetReducedMunicipalityListVisibleinSearch(language, elements, geosearchresult, table);
        //    else
        //        return GetReducedMunicipalityList(language, elements, geosearchresult, table);
        //}

        ///// <summary>
        ///// GET District Reduced List
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="visibleinsearch">Filter only Elements flagged with visibleinsearch: (possible values: 'true','false'), (default:'false')</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <returns>Collection of CommonReduced Objects</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Array of CommonReduced Objects", typeof(IEnumerable<CommonReduced>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("District")]
        //[HttpGet, Route("api/DistrictReduced")]
        //public IHttpActionResult GetDistrictReduced(
        //    string language = "en",
        //    int elements = 0,
        //    string visibleinsearch = "false",
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null)
        //{
        //    var table = CheckOpenData(User, "districts");

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    if (visibleinsearch.ToLower() == "true")
        //        return GetReducedDistrictListVisibleinSearch(language, elements, geosearchresult, table);
        //    else
        //        return GetReducedDistrictList(language, elements, geosearchresult, table);
        //}

        ///// <summary>
        ///// GET SkiRegion Reduced List
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <returns>Collection of CommonReduced Objects</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Array of CommonReduced Objects", typeof(IEnumerable<CommonReduced>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("SkiRegion")]
        //[HttpGet, Route("api/SkiRegionReduced")]
        //public IHttpActionResult GetSkiRegionReduced(
        //    string language = "en",
        //    int elements = 0,
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null)
        //{
        //    var table = CheckOpenData(User, "skiregions");

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    return GetReducedSkiRegionList(language, elements, geosearchresult, table);
        //}

        ///// <summary>
        ///// GET SkiArea Reduced List
        ///// </summary>
        ///// <param name="language">Localization Language, (default:'en')</param>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        ///// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        ///// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        ///// <returns>Collection of CommonReduced Objects</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Array of CommonReduced Objects", typeof(IEnumerable<CommonReduced>))]
        ////[Authorize(Roles = "DataReader,CommonReader")]
        //[OpenData("SkiArea")]
        //[HttpGet, Route("api/SkiAreaReduced")]
        //public IHttpActionResult GetSkiAreaReduced(
        //    string language = "en",
        //    int elements = 0,
        //    string latitude = null,
        //    string longitude = null,
        //    string radius = null)
        //{
        //    var table = CheckOpenData(User, "skiareas");

        //    var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

        //    return GetReducedSkiAreaList(language, elements, geosearchresult, table);
        //}


        ////Special GETTER

        ///// <summary>
        ///// GET Wine Awards List
        ///// </summary>
        ///// <param name="elements">Elements to retrieve (0 = Get All)</param>
        ///// <param name="companyid">Company Id</param>
        ///// <param name="wineid">WineId</param>
        ///// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        ///// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        ///// <returns>Collection of Wine Objects</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Array of Wine Objects", typeof(IEnumerable<Wine>))]
        ////[Authorize(Roles = "DataReader,CommonReader,SuedtirolWineReader")]
        //[OpenData("Wine")]
        //[HttpGet, Route("api/WineAward")]
        //public IHttpActionResult GetWineAwardsList(
        //    int elements = 0,
        //    string wineid = null,
        //    string companyid = null,
        //    string fields = null,
        //    string language = null
        //    )
        //{
        //    var fieldselector = !String.IsNullOrEmpty(fields) ? fields.Split(',') : null;

        //    if (wineid == null && companyid == null)
        //        return GetWineAwardList(0, fieldselector, language);
        //    else if (companyid != null || wineid != null)
        //        return GetWineListbyCompanyId(companyid, wineid, fieldselector, language);
        //    else
        //        return BadRequest("not supported");
        //}

        ///// <summary>
        ///// GET Wine Award Single
        ///// </summary>
        ///// <param name="id">ID of the requested data</param>
        ///// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        ///// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        ///// <returns>Wine Object</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Wine Object", typeof(Wine))]
        ////[Authorize(Roles = "DataReader,CommonReader,SuedtirolWineReader")]
        //[OpenData("Wine")]
        //[HttpGet, Route("api/WineAward/{id}")]
        //public IHttpActionResult GetWineAwardsSingle(string id, string fields = null, string language = null)
        //{
        //    var fieldselector = !String.IsNullOrEmpty(fields) ? fields.Split(',') : null;

        //    return GetWineAward(id, fieldselector, language);
        //}

        //MetaRegions

        //REGIONS

        //TVS

        //Municipality        

        //District        

        //Wine        



        #endregion


        #region GETTER

        private Task<IActionResult> CommonGetListHelper(string tablename, string? seed, string? searchfilter, string[] fields, string? language, CommonHelper commonhelper, PGGeoSearchResult geosearchresult, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {               
                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From(tablename)
                        .CommonWhereExpression(languagelist: new List<string>(), lastchange: commonhelper.lastchange, visibleinsearch: commonhelper.visibleinsearch,
                                               searchfilter: searchfilter, language: language, filterClosedData: FilterClosedData)
                        .OrderBySeed(ref seed, "data#>>'\\{Shortname\\}' ASC")
                        .GeoSearchFilterAndOrderby(geosearchresult);

                // Get paginated data
                var data =
                    await query
                        .GetAsync<JsonRaw>();

                var dataTransformed =
                    data.Select(
                        raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, urlGenerator: UrlGenerator)
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

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, urlGenerator: UrlGenerator);
            });
        }

        #endregion
    }
}
