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
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class SearchController : OdhController
    {
        public SearchController(IWebHostEnvironment env, ISettings settings, ILogger<ODHTagController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {
        }

        #region SWAGGER Exposed API

        /// <summary>
        /// GET Search over all Entities
        /// </summary>
        /// <param name="validforentity">Restrict search to Entities (accommodation, activity, poi, smgpoi, package, gastronomy, event, article, common .. etc..)</param>
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
            CancellationToken cancellationToken = default)
        {            
            return await Get(language, validforentity, fields: fields ?? Array.Empty<string>(), 
                  searchfilter, cancellationToken);           
        }

        #endregion

        #region GETTER

        private Task<IActionResult> Get(string? language, string? validforentity, string[] fields,
            string? searchfilter, CancellationToken cancellationToken)
        {
            var myentitytypelist = (validforentity ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries);

            //return DoAsyncReturn(async () =>
            //{
            //    //if (String.IsNullOrEmpty(rawsort))
            //    //    rawsort = "data #>>'\\{MainEntity\\}', data#>>'\\{Shortname\\}'";

            //    var query =
            //        QueryFactory.Query()
            //        .SelectRaw("data")
            //        .From("smgtags")
            //        .ODHTagWhereExpression(
            //            languagelist: new List<string>(),
            //            smgtagtypelist: mysmgtagtypelist,
            //            searchfilter: searchfilter,
            //            language: language,
            //            filterClosedData: FilterClosedData
            //            )
            //        .ApplyRawFilter(rawfilter)
            //        .ApplyOrdering(new PGGeoSearchResult() { geosearch = false }, rawsort, "data #>>'\\{MainEntity\\}', data#>>'\\{Shortname\\}'");


            //    var data = await query.GetAsync<JsonRaw>();

            //    return data.Select(raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, urlGenerator: UrlGenerator, userroles: UserRolesList));
            //});

            return null;
        }

        #endregion
    }
}