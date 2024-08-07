//// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
////
//// SPDX-License-Identifier: AGPL-3.0-or-later

//using AspNetCore.CacheOutput;
//using DataModel;
//using Helper;
//using Microsoft.AspNetCore.Cors;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using OdhApiCore.Responses;
//using SqlKata.Execution;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Serilog.Context;
//using Microsoft.AspNetCore.Authorization;
//using Swashbuckle.AspNetCore.Annotations;
//using OdhApiCore.Filters;
//using ServiceReferenceLCS;
//using OdhApiCore.Controllers.api;
//using Helper.Identity;
//using OdhNotifier;
//using Helper.Generic;
//using Geo.Geometries;

//namespace OdhApiCore.Controllers
//{
//    /// <summary>
//    /// GastroItem Api (data provided by LTS GastroItemData) Deprecated! Please use Endpoint ODHGastroItemPoi
//    /// </summary>
//    [EnableCors("CorsPolicy")]
//    [NullStringParameterActionFilter]
//    public class GastroItemController : OdhController
//    {     
//        public GastroItemController(IWebHostEnvironment env, ISettings settings, ILogger<GastroItemController> logger, QueryFactory queryFactory, IOdhPushNotifier odhpushnotifier)
//            : base(env, settings, logger, queryFactory, odhpushnotifier)
//        {
//        }

//        #region SWAGGER Exposed API

//        /// <summary>
//        /// GET GastroItem List
//        /// </summary>
//        /// <param name="pagenumber">Pagenumber</param>
//        /// <param name="pagesize">Elements per Page, (default:10)</param>
//        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
//        /// <param name="GastroItemtype">Type of the GastroItem ('null' = Filter disabled, possible values: BITMASK: 'Mountains = 1','Cycling = 2','Local tours = 4','Horses = 8','Hiking = 16','Running and fitness = 32','Cross-country ski-track = 64','Tobbogan run = 128','Slopes = 256','Lifts = 512'), (default:'1023' == ALL), REFERENCE TO: GET /api/GastroItemTypes </param>
//        /// <param name="subtype">Subtype of the GastroItem (BITMASK Filter = available SubTypes depends on the selected GastroItem Type), (default:'null')</param>
//        /// <param name="idlist">IDFilter (Separator ',' List of GastroItem IDs), (default:'null')</param>
//        /// <param name="locfilter">Locfilter SPECIAL Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction), 'null' = (No Filter), (default:'null') <a href="https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#location-filter-locfilter" target="_blank">Wiki locfilter</a></param>        
//        /// <param name="areafilter">AreaFilter (Separator ',' IDList of AreaIDs separated by ','), (default:'null')</param>
//        /// <param name="distancefilter">Distance Range Filter (Separator ',' example Value: 15,40 Distance from 15 up to 40 Km), (default:'null')</param>
//        /// <param name="altitudefilter">Altitude Range Filter (Separator ',' example Value: 500,1000 Altitude from 500 up to 1000 metres), (default:'null')</param>
//        /// <param name="durationfilter">Duration Range Filter (Separator ',' example Value: 1,3 Duration from 1 to 3 hours), (default:'null')</param>
//        /// <param name="highlight">Hightlight Filter (possible values: 'false' = only Activities with Highlight false, 'true' = only Activities with Highlight true), (default:'null')</param>
//        /// <param name="difficultyfilter">Difficulty Filter (possible values: '1' = easy, '2' = medium, '3' = difficult), (default:'null')</param>
//        /// <param name="odhtagfilter">Taglist Filter (String, Separator ',' more Tags possible, available Tags reference to 'v1/ODHTag?validforentity=GastroItem'), (default:'null')</param>
//        /// <param name="active">Active Activities Filter (possible Values: 'true' only active Activities, 'false' only disabled Activities), (default:'null')</param>
//        /// <param name="odhactive">Odhactive (Published) Activities Filter (possible Values: 'true' only published Activities, 'false' only not published Activities), (default:'null')</param>
//        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
//        /// <param name="latitude">GeoFilter FLOAT Latitude Format: '46.624975', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
//        /// <param name="longitude">GeoFilter FLOAT Longitude Format: '11.369909', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
//        /// <param name="radius">Radius INTEGER to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
//        /// <param name="polygon">valid WKT (Well-known text representation of geometry) Format, Examples (POLYGON ((30 10, 40 40, 20 40, 10 20, 30 10))) / By Using the GeoShapes Api (v1/GeoShapes) and passing Country.Type.Id OR Country.Type.Name Example (it.municipality.3066) / Bounding Box Filter bbc: 'Bounding Box Contains', 'bbi': 'Bounding Box Intersects', followed by a List of Comma Separated Longitude Latitude Tuples, 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#polygon-filter-functionality' target="_blank">Wiki geosort</a></param>
//        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
//        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
//        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
//        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
//        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
//        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
//        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues" target="_blank">Opendatahub Wiki</a></param>        
//        /// <returns>Collection of GastroItem Objects</returns>
//        /// <response code="200">List created</response>
//        /// <response code="400">Request Error</response>
//        /// <response code="500">Internal Server Error</response>
//        [ProducesResponseType(typeof(JsonResult<GastroItem>), StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        //[Authorize(Roles = "DataReader,GastroItemReader")]
//        //[OdhCacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 3600, CacheKeyGenerator = typeof(CustomCacheKeyGenerator), MustRevalidate = true)]
//        [HttpGet, Route("GastroItem")]
//        public async Task<IActionResult> GetGastroItemList(
//            uint pagenumber = 1,
//            PageSize pagesize = null!,
//            string? idlist = null,
//            LegacyBool active = null!,
//            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
//            string[]? fields = null,
//            string? searchfilter = null,
//            string? rawfilter = null,
//            string? rawsort = null,
//            bool removenullvalues = false,
//            CancellationToken cancellationToken = default)
//        {
//            return await GetFiltered(
//                    fields: fields ?? Array.Empty<string>(), pagenumber: pagenumber,
//                    pagesize: pagesize,
//                    rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues,
//                    cancellationToken: cancellationToken);
//        }

//        /// <summary>
//        /// GET GastroItem Single
//        /// </summary>
//        /// <param name="id">ID of the GastroItem</param>
//        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
//        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
//        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
//        /// <returns>GBLTSGastroItem Object</returns>
//        /// <response code="200">Object created</response>
//        /// <response code="400">Request Error</response>
//        /// <response code="500">Internal Server Error</response>
//        /// //[Authorize(Roles = "DataReader,GastroItemReader")]
//        [ProducesResponseType(typeof(GastroItem), StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        [HttpGet, Route("GastroItem/{id}", Name = "SingleGastroItem")]
//        public async Task<IActionResult> GetGastroItemSingle(
//            string id,
//            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
//            string[]? fields = null,
//            bool removenullvalues = false,
//            CancellationToken cancellationToken = default)
//        {
//            return await GetSingle(id, fields: fields ?? Array.Empty<string>(), removenullvalues, cancellationToken);
//        }

//        #endregion

//        #region GETTER

//        private Task<IActionResult> GetFiltered(
//            string[] fields, uint pagenumber, int? pagesize, 
//            string? rawfilter, string? rawsort,
//            bool removenullvalues, CancellationToken cancellationToken)
//        {
//            return DoAsyncReturn(async () =>
//            {
//                //Additional Read Filters to Add Check
//                AdditionalFiltersToAdd.TryGetValue("Read", out var additionalfilter);

//                var query =
//                    QueryFactory.Query()
//                        .SelectRaw("data")
//                        .From("gastroitems")
//                        .FilterDataByAccessRoles(UserRolesToFilter)
//                        .ApplyRawFilter(rawfilter)
//                        .ApplyOrdering_GeneratedColumns(new PGGeoSearchResult(){ geosearch = false }, rawsort);

//                // Get paginated data
//                var data =
//                    await query
//                        .PaginateAsync<JsonRaw>(
//                            page: (int)pagenumber,
//                            perPage: pagesize ?? 25);
                
//                var dataTransformed =
//                    data.List.Select(
//                        raw => raw.TransformRawData(null, fields, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: null)
//                    );

//                uint totalpages = (uint)data.TotalPages;
//                uint totalcount = (uint)data.Count;

//                return ResponseHelpers.GetResult(
//                    pagenumber,
//                    totalpages,
//                    totalcount,
//                    null ,
//                    dataTransformed,
//                    Url);
//            });
//        }

//        /// <summary>
//        /// GET Single GastroItem
//        /// </summary>
//        /// <param name="id">ID of the GastroItem</param>
//        /// <returns>GastroItem Object</returns>
//        private Task<IActionResult> GetSingle(string id, string[] fields, bool removenullvalues, CancellationToken cancellationToken)
//        {
//            return DoAsyncReturn(async () =>
//            {
//                //Additional Read Filters to Add Check
//                AdditionalFiltersToAdd.TryGetValue("Read", out var additionalfilter);

//                var query =
//                    QueryFactory.Query("gastroitems")
//                        .Select("data")
//                        .Where("id", id.ToUpper())
//                        .When(!String.IsNullOrEmpty(additionalfilter), q => q.FilterAdditionalDataByCondition(additionalfilter))
//                        .FilterDataByAccessRoles(UserRolesToFilter);

//                var data = await query.FirstOrDefaultAsync<JsonRaw?>();
                
//                return data?.TransformRawData(null, fields, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: null);
//            });
//        }

//        #endregion

        

//        #region POST PUT DELETE

//        /// <summary>
//        /// POST Insert new GastroItem
//        /// </summary>
//        /// <param name="GastroItem">GastroItem Object</param>
//        /// <returns>Http Response</returns>
//        //[ApiExplorerSettings(IgnoreApi = true)]
//        //[InvalidateCacheOutput(typeof(GastroItemApiController), nameof(GetGastroItemList))]
//        //[AuthorizeODH(PermissionAction.Create)]
//        [HttpPost, Route("GastroItem")]
//        public Task<IActionResult> Post([FromBody] GastroItem gastroitem)
//        {
//            return DoAsyncReturn(async () =>
//            {
//                //Additional Filters on the Action Create
//                AdditionalFiltersToAdd.TryGetValue("Create", out var additionalfilter);

//                gastroitem.Id = Helper.IdGenerator.GenerateIDFromType(gastroitem);
                
//                return await UpsertData<GastroItem>(gastroitem, new DataInfo("gastroitems", CRUDOperation.Create), new CompareConfig(true, true), new CRUDConstraints(additionalfilter, UserRolesToFilter));
//            });
//        }

//        /// <summary>
//        /// PUT Modify existing GastroItem
//        /// </summary>
//        /// <param name="id">GastroItem Id</param>
//        /// <param name="GastroItem">GastroItem Object</param>
//        /// <returns>Http Response</returns>
//        //[ApiExplorerSettings(IgnoreApi = true)]
//        //[InvalidateCacheOutput(typeof(GastroItemApiController), nameof(GetGastroItemList))]
//        //[Authorize(Roles = "DataWriter,DataModify,GastroItemManager,GastroItemModify,GastroItemUpdate")]
//        //[AuthorizeODH(PermissionAction.Update)]
//        [HttpPut, Route("GastroItem/{id}")]
//        public Task<IActionResult> Put(string id, [FromBody] GastroItem gastroitem)
//        {
//            return DoAsyncReturn(async () =>
//            {
//                //Additional Filters on the Action Create
//                AdditionalFiltersToAdd.TryGetValue("Update", out var additionalfilter);

//                gastroitem.Id = Helper.IdGenerator.CheckIdFromType<GastroItem>(id);
             
//                return await UpsertData<GastroItem>(gastroitem, new DataInfo("gastroitems", CRUDOperation.Update), new CompareConfig(true, true), new CRUDConstraints(additionalfilter, UserRolesToFilter));
//            });
//        }

//        /// <summary>
//        /// DELETE GastroItem by Id
//        /// </summary>
//        /// <param name="id">GastroItem Id</param>
//        /// <returns>Http Response</returns>
//        //[ApiExplorerSettings(IgnoreApi = true)]
//        //[InvalidateCacheOutput(typeof(GastroItemApiController), nameof(GetGastroItemList))]
//        //[Authorize(Roles = "DataWriter,DataDelete,GastroItemManager,GastroItemDelete")]
//        //[AuthorizeODH(PermissionAction.Delete)]
//        [HttpDelete, Route("GastroItem/{id}")]
//        public Task<IActionResult> Delete(string id)
//        {
//            return DoAsyncReturn(async () =>
//            {
//                //Additional Filters on the Action Create
//                AdditionalFiltersToAdd.TryGetValue("Delete", out var additionalfilter);

//                id = Helper.IdGenerator.CheckIdFromType<GastroItem>(id);

//                return await DeleteData<GastroItem>(id, new DataInfo("gastroitems", CRUDOperation.Delete), new CRUDConstraints(additionalfilter, UserRolesToFilter));
//            });
//        }


//        #endregion
//    }
//}