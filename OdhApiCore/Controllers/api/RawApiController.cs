﻿using AspNetCore.CacheOutput;
using DataModel;
using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OdhApiCore.Filters;
using OdhApiCore.Responses;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.api
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RawdataApiController: OdhController
    {
        public RawdataApiController(IWebHostEnvironment env, ISettings settings, ILogger<TagController> logger, QueryFactory queryFactory)
           : base(env, settings, logger, queryFactory)
        {
        }

        /// <summary>
        /// GET Raw Data List
        /// </summary>
        /// <returns>Collection of Tag Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<TagLinked>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [OdhCacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 3600, CacheKeyGenerator = typeof(CustomCacheKeyGenerator))]
        [HttpGet, Route("Rawdata")]
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        public async Task<IActionResult> GetRawdataAsync(
            uint pagenumber = 1,
            PageSize pagesize = null!,           
            string? type = null,
            string? source = null,
            string? sourceinterface = null,
            string? sourceid = null,
            bool latest = true,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {            
            return await Get(pagenumber, pagesize, type, source, latest, fields: fields ?? Array.Empty<string>(),
                  searchfilter, rawfilter, rawsort, removenullvalues: removenullvalues, cancellationToken);
        }

        private Task<IActionResult> Get(
            uint pagenumber, int? pagesize,
            string type, string source, bool latest, 
            string[] fields, string? searchfilter, string? rawfilter, string? rawsort,
            bool removenullvalues,
            CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var typelist = !String.IsNullOrEmpty(type) ? type.Split(",").ToList() : null;
                var sourcelist = !String.IsNullOrEmpty(source) ? source.Split(",").ToList() : null;

                var fieldsTohide = FieldsToHide;

                //Example latest records
                //select*
                //from rawdata
                //where id in (SELECT max(id)  FROM rawdata where type = 'ejob' group by sourceid)

                var query =
                    QueryFactory.Query()
                    .When(latest, q => q.SelectRaw("max(id)"))
                    .From("rawdata")
                    .RawdataWhereExpression(null, null, typelist, sourcelist, latest, true)
                    //.ApplyRawFilter(rawfilter)
                    //.ApplyOrdering(new PGGeoSearchResult() { geosearch = false }, rawsort, "data #>>'\\{MainEntity\\}', data#>>'\\{Shortname\\}'")
                    .When(latest, q => q.GroupBy("sourceid"));

                //rawfilter and rawsort could be extended with a generated column "data" which only applies when rawtype is json

                if(latest)
                {
                    var query2 = 
                        QueryFactory.Query()
                        .From("rawdata")
                        .WhereIn("id", query);

                    // Get paginated data
                    var data =
                        await query2
                            .PaginateAsync<RawDataStoreWithId>(
                                page: (int)pagenumber,
                                perPage: pagesize ?? 25);


                    var jsonrawdata = data.List.Select(raw => new JsonRaw(raw.UseJsonRaw())).ToList();

                    var dataTransformed =
                            jsonrawdata.Select(
                                raw => raw.TransformRawData(null, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide)
                            );

                    uint totalpages = (uint)data.TotalPages;
                    uint totalcount = (uint)data.Count;

                    return ResponseHelpers.GetResult(
                        (uint)pagenumber,
                        totalpages,
                        totalcount,
                        null,
                        dataTransformed,
                        Url);
                }
                else
                {
                    // Get paginated data
                    var data =
                        await query
                            .PaginateAsync<RawDataStoreWithId>(
                                page: (int)pagenumber,
                                perPage: pagesize ?? 25);


                    var jsonrawdata = data.List.Select(raw => new JsonRaw(raw.UseJsonRaw())).ToList();

                    var dataTransformed =
                            jsonrawdata.Select(
                                raw => raw.TransformRawData(null, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide)
                            );

                    uint totalpages = (uint)data.TotalPages;
                    uint totalcount = (uint)data.Count;

                    return ResponseHelpers.GetResult(
                        (uint)pagenumber,
                        totalpages,
                        totalcount,
                        null,
                        dataTransformed,
                        Url);
                }

             
            });
        }


    }
}
