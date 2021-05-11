using AspNetCore.CacheOutput;
using DataModel;
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

namespace OdhApiCore.Controllers
{
    /// <summary>
    /// WebcamInfo Api (data provided by LTS ActivityData) SOME DATA Available as OPENDATA
    /// </summary>
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class WebcamInfoController : OdhController
    {
        // Only for test purposes

        public WebcamInfoController(IWebHostEnvironment env, ISettings settings, ILogger<WebcamInfoController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {
        }

        #region SWAGGER Exposed API

        /// <summary>
        /// GET Webcam List
        /// </summary>
        /// <param name="pagenumber">Pagenumber, (default:1)</param>
        /// <param name="pagesize">Elements per Page (max 1024), (default:10)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, not provided disables Random Sorting, (default:'null') </param>
        /// <param name="idlist">IDFilter (Separator ',' List of Gastronomy IDs), (default:'null')</param>
        /// <param name="source">Source Filter(String, ), (default:'null')</param>        
        /// <param name="active">Active Webcam Filter (possible Values: 'true' only Active Gastronomies, 'false' only Disabled Gastronomies</param>
        /// <param name="odhactive">ODH Active (refers to field SmgActive) (Published) Webcam Filter (possible Values: 'true' only published Webcam, 'false' only not published Webcam, (default:'null')</param>        
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in KM. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <returns>Collection of WebcamInfo Objects</returns>    
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(JsonResult<WebcamInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 3600, CacheKeyGenerator = typeof(CustomCacheKeyGenerator))]
        [HttpGet, Route("WebcamInfo")]
        public async Task<IActionResult> Get(
            string? language = null,
            uint pagenumber = 1,
            PageSize pagesize = null!,
            string? source = null,
            string? idlist = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? seed = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            string? updatefrom = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            return await GetFilteredAsync(
                fields: fields ?? Array.Empty<string>(), language, pagenumber, pagesize,
                source, idlist, searchfilter, active?.Value, odhactive?.Value, 
                seed, updatefrom, geosearchresult, rawfilter: rawfilter, rawsort: rawsort, 
                cancellationToken);
        }

        /// <summary>
        /// GET Webcam Single 
        /// </summary>
        /// <param name="id">ID of the Webcam</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <returns>WebcamInfo Object</returns>
        [ProducesResponseType(typeof(WebcamInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("WebcamInfo/{id}", Name = "SingleWebcamInfo")]
        public Task<IActionResult> Get(
            string id,
            string? language = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            CancellationToken cancellationToken = default)
        {
            return GetSingleAsync(id, language, fields: fields ?? Array.Empty<string>(), cancellationToken);
        }

        #endregion

        #region GETTER

        private Task<IActionResult> GetFilteredAsync(
            string[] fields, string? language, uint pagenumber, int? pagesize, string? source,
            string? idfilter, string? searchfilter, bool? active, bool? smgactive,
            string? seed, string? lastchange, PGGeoSearchResult geosearchresult,
            string? rawfilter, string? rawsort, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                WebcamInfoHelper mywebcaminfohelper = WebcamInfoHelper.Create(
                    source, idfilter, active, smgactive, lastchange);

                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From("webcams")
                        .WebCamInfoWhereExpression(
                            idlist: mywebcaminfohelper.idlist, sourcelist: mywebcaminfohelper.sourcelist,
                            activefilter: mywebcaminfohelper.active, smgactivefilter: mywebcaminfohelper.smgactive,
                            searchfilter: searchfilter, language: language, lastchange: mywebcaminfohelper.lastchange,
                            languagelist: new List<string>(), filterClosedData: FilterClosedData)
                        .ApplyRawFilter(rawfilter)
                        .ApplyOrdering_GeneratedColumns(ref seed, geosearchresult, rawsort); //.ApplyOrdering(ref seed, geosearchresult, rawsort);


                // Get paginated data
                var data =
                    await query
                        .PaginateAsync<JsonRaw>(
                            page: (int)pagenumber,
                            perPage: (int)pagesize);

                var dataTransformed =
                    data.List.Select(
                        raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, urlGenerator: UrlGenerator, userroles: UserRolesList)
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

        private Task<IActionResult> GetSingleAsync(string id, string? language, string[] fields, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("webcams")
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