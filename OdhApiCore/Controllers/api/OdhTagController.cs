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
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    //[Route("ODHTag")]
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class ODHTagController : OdhController
    {
        public ODHTagController(IWebHostEnvironment env, ISettings settings, ILogger<ODHTagController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {
        }

        #region SWAGGER Exposed API

        /// <summary>
        /// GET ODHTag List
        /// </summary>
        /// <param name="validforentity">Filter on Tags valid on Entitys (accommodation, activity, poi, smgpoi, package, gastronomy, event, article, common .. etc..)</param>
        /// <param name="language"></param>
        /// <returns>Collection of ODHTag Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<SmgTags>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 3600, CacheKeyGenerator = typeof(CustomCacheKeyGenerator))]
        [HttpGet, Route("ODHTag")]
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        public async Task<IActionResult> GetODHTagsAsync(
            string? language = null,
            string? validforentity = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            string? localizationlanguage = null,  //TODO ignore this in swagger
            CancellationToken cancellationToken = default)
        {
            //Compatibility
            if (String.IsNullOrEmpty(language) && !String.IsNullOrEmpty(localizationlanguage))
                language = localizationlanguage;


            return await Get(language, validforentity, fields: fields ?? Array.Empty<string>(), 
                  searchfilter, rawfilter, rawsort,
                    cancellationToken);           
        }

        /// <summary>
        /// GET ODHTag Single
        /// </summary>
        /// <param name="id"></param>
        /// <param name="language"></param>
        /// <returns>ODHTag Object</returns>
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(SmgTags), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("ODHTag/{id}", Name = "SingleODHTag")]
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        public async Task<IActionResult> GetODHTagSingle(string id,
            string? language = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? localizationlanguage = null,   //TODO ignore this in swagger
            CancellationToken cancellationToken = default)
        {
            //Compatibility
            if (String.IsNullOrEmpty(language) && !String.IsNullOrEmpty(localizationlanguage))
                language = localizationlanguage;

            return await GetSingle(id, language, fields: fields ?? Array.Empty<string>(), cancellationToken);
        }

        #endregion

        #region GETTER

        private Task<IActionResult> Get(string? language, string? smgtagtype, string[] fields,
            string? searchfilter, string? rawfilter, string? rawsort,
            CancellationToken cancellationToken)
        {
            var mysmgtagtypelist = (smgtagtype ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries);

            return DoAsyncReturn(async () =>
            {
                //if (String.IsNullOrEmpty(rawsort))
                //    rawsort = "data #>>'\\{MainEntity\\}', data#>>'\\{Shortname\\}'";
                
                var query = 
                    QueryFactory.Query()
                    .SelectRaw("data")
                    .From("smgtags")
                    .ODHTagWhereExpression(
                        languagelist: new List<string>(), 
                        smgtagtypelist: mysmgtagtypelist,
                        searchfilter: searchfilter,
                        language: language,
                        filterClosedData: FilterClosedData
                        )
                    .ApplyRawFilter(rawfilter)
                    .ApplyOrdering(new PGGeoSearchResult() { geosearch = false }, rawsort, "data #>>'\\{MainEntity\\}', data#>>'\\{Shortname\\}'");


                var data = await query.GetAsync<JsonRaw>();

                return data.Select(raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, urlGenerator: UrlGenerator, userroles: UserRolesList));
            });
        }      

        private Task<IActionResult> GetSingle(string id, string? language, string[] fields, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var data = await QueryFactory.Query("smgtags")
                    .Select("data")
                    .Where("id", id.ToLower())
                    .When(FilterClosedData, q => q.FilterClosedData())
                    .FirstOrDefaultAsync<JsonRaw>();

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, urlGenerator: UrlGenerator, userroles: UserRolesList);
            });
        }

        #endregion

        #region POST PUT DELETE

        /// <summary>
        /// POST Insert new ODHTag
        /// </summary>
        /// <param name="odhtag">ODHTag Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,ODHTagManager,ODHTagCreate")]
        [HttpPost, Route("ODHTag")]
        public Task<IActionResult> Post([FromBody] SmgTags odhtag)
        {
            return DoAsyncReturn(async () =>
            {
                odhtag.Id = !String.IsNullOrEmpty(odhtag.Id) ? odhtag.Id.ToUpper() : "noId";
                return await UpsertData<SmgTags>(odhtag, "smgtags");
            });
        }

        /// <summary>
        /// PUT Modify existing ODHTag
        /// </summary>
        /// <param name="id">ODHTag Id</param>
        /// <param name="odhtag">ODHTag Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataModify,ODHTagManager,ODHTagModify")]
        [HttpPut, Route("ODHTag/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] SmgTags odhtag)
        {
            return DoAsyncReturn(async () =>
            {
                odhtag.Id = id.ToUpper();
                return await UpsertData<SmgTags>(odhtag, "smgtags");
            });
        }

        /// <summary>
        /// DELETE ODHTag by Id
        /// </summary>
        /// <param name="id">ODHTag Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataDelete,ODHTagManager,ODHTagDelete")]
        [HttpDelete, Route("ODHTag/{id}")]
        public Task<IActionResult> Delete(string id)
        {
            return DoAsyncReturn(async () =>
            {
                id = id.ToUpper();
                return await DeleteData(id, "smgtags");
            });
        }


        #endregion
    }
}