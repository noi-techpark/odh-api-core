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
        /// <param name="odhtype">Restrict search to Entities (accommodation, odhactivitypoi, event, webcam, measuringpoint, ltsactivity, ltspoi, ltsgastronomy, article .. etc..)</param>
        /// <param name="language"></param>
        /// <returns>Collection of ODHTag Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<SmgTags>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 3600, CacheKeyGenerator = typeof(CustomCacheKeyGenerator))]
        [HttpGet, Route("SearchOverAll")]
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        public async Task<IActionResult> GetSearchAsync(
            string searchfilter,
            string? language = "en",
            string? odhtype = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? rawfilter = null,
            string? rawsort = null,
            int? limitto = 5,
            CancellationToken cancellationToken = default)
        {
            return await Get(language, odhtype, fields: fields ?? Array.Empty<string>(),
                  searchfilter, rawfilter, rawsort, limitto, cancellationToken);
        }

        #endregion

        #region GETTER

        private Task<IActionResult> Get(string? language, string? validforentity, string[] fields,
            string? searchfilter, string? rawfilter, string? rawsort, int? limitto, CancellationToken cancellationToken)
        {
            var myentitytypelist = (validforentity ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries);

            if (myentitytypelist.Count() == 0)
                myentitytypelist = new string[] { "accommodation", "odhactivitypoi", "event" };


            var searchresult = new List<JsonRaw>();
            var searchresultpertype = new Dictionary<string, uint>();

            return DoAsyncReturn(async () =>
            {                 
                foreach (var entitytype in myentitytypelist)
                {
                    var customfields = fields;

                    if (fields == Array.Empty<string>())
                        customfields = new string[] { "Id", Type2SearchFunction.TranslateTypeToTitleField(entitytype, language), "_Meta.Type", "Self" };

                    var result = await SearchTroughEntity(Type2SearchFunction.TranslateTypeToSearchField(entitytype), Type2Table.TranslateTypeToTable(entitytype), language, customfields, searchfilter, rawfilter, rawsort, limitto, cancellationToken);

                    if (result != null)
                    {
                        searchresult.AddRange(result);
                        searchresultpertype.Add(entitytype, (uint)result.Count());
                    }                        
                }

                return new SearchResult<JsonRaw>
                {
                    Items = searchresult,
                    totalResults = (uint)searchresult.Count,
                    detailedResults = searchresultpertype
                };                    
            });
        }

        private async Task<IEnumerable<JsonRaw?>> SearchTroughEntity(Func<string, string[]> fieldsearchfunc, string table, string? language, string[] fields,
            string? searchfilter, string? rawfilter, string? rawsort, int? limitto, CancellationToken cancellationToken)
        {
            var query =
                QueryFactory.Query()
                .SelectRaw("data")
                .From(table)
                .SearchFilter(fieldsearchfunc(language), searchfilter)
                .When(FilterClosedData, q => q.FilterClosedData_GeneratedColumn())
                .ApplyRawFilter(rawfilter)
                .ApplyOrdering(new PGGeoSearchResult() { geosearch = false }, rawsort, "data#>>'\\{Shortname\\}'")
                .Limit(limitto.Value);


            var data = await query.GetAsync<JsonRaw>();

            return data.Select(raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, urlGenerator: UrlGenerator, userroles: UserRolesList));
        }

        #endregion
    }
}