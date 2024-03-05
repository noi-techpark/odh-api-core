// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using AspNetCore.CacheOutput;
using DataModel;
using DataModel.Annotations;
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
using OdhNotifier;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.api
{
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class RawdataApiController : OdhController
    {
        public RawdataApiController(IWebHostEnvironment env, ISettings settings, ILogger<TagController> logger, QueryFactory queryFactory, IOdhPushNotifier odhpushnotifier)
           : base(env, settings, logger, queryFactory, odhpushnotifier)
        {
        }

        /// <summary>
        /// GET Raw Data List
        /// </summary>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="type">Type Filter (Separator ',' List of rawdata types, 'null' = No Filter), (default:'null')</param>
        /// <param name="source">Source Filter (Separator ',' List of rawdata sources, 'null' = No Filter), (default:'null')</param>
        /// <param name="sourceinterface">SourceInterface Filter (Separator ',' List of Sourceinterfaces, 'null' = No Filter), (default:'null')</param>
        /// <param name="sourceid">SourceIDFilter (Separator ',' List of Rawdata SourceIDs, 'null' = No Filter), (default:'null')</param>
        /// <param name="idlist">IDFilter (Separator ',' List of Rawdata IDs, 'null' = No Filter), (default:'null')</param>
        /// <param name="latest">Get only latest data</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="rawfilter">Currently not working on rawdata<a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort">Currently not working on rawdata<a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of Raw Data Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [OdhCacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 3600, CacheKeyGenerator = typeof(CustomCacheKeyGenerator))]
        [HttpGet, Route("Rawdata")]
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        public async Task<IActionResult> GetRawdataAsync(
            uint pagenumber = 1,
            PageSize pagesize = null!,
            [SwaggerEnum(new[] { "event_euracnoi", "odhactivitypoi-lift", "odhactivitypoi-slope", "odhactivitypoi-museum", "weather", "event_centrotrevi-drin", "ejob" })]
            string? type = null,
            [SwaggerEnum(new[] { "dss", "eurac", "siag", "centrotrevi-drin", "looptec" })]
            string? source = null,
            string? sourceinterface = null,
            string? sourceid = null,
            string? idlist = null,
            bool latest = true,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {            
            return await Get(pagenumber, pagesize, type, source, idlist, sourceid, latest, fields: fields ?? Array.Empty<string>(),
                  searchfilter, rawfilter, rawsort, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Rawdata Single
        /// </summary>
        /// <param name="id">ID of rawdata</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Rawdata Object</returns>
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(RawDataStoreWithId), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,PoiReader")]
        [HttpGet, Route("Rawdata/{id}", Name = "SingleRawdata")]
        public async Task<IActionResult> GetRawdataSingle(
            string id,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetSingle(id, fields: fields ?? Array.Empty<string>(), removenullvalues: removenullvalues, cancellationToken);
        }

        private Task<IActionResult> Get(
            uint pagenumber, int? pagesize,
            string type, string source, string ids, string sourceids, bool latest, 
            string[] fields, string? searchfilter, string? rawfilter, string? rawsort,
            bool removenullvalues,
            CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var typelist = !String.IsNullOrEmpty(type) ? type.Split(",").ToList() : null;
                var sourcelist = !String.IsNullOrEmpty(source) ? source.Split(",").ToList() : null;
                var idlist = !String.IsNullOrEmpty(ids) ? ids.Split(",").ToList() : null;
                var sourceidlist = !String.IsNullOrEmpty(sourceids) ? sourceids.Split(",").ToList() : null;
                
                //Example latest records
                //select*
                //from rawdata
                //where id in (SELECT max(id)  FROM rawdata where type = 'ejob' group by sourceid)

                //TO CHECK additionalfilter???

                var query =
                    QueryFactory.Query()
                    .When(latest, q => q.SelectRaw("max(id)"))
                    .From("rawdata")
                    .RawdataWhereExpression(idlist, 
                    sourceidlist, typelist, sourcelist, 
                    additionalfilter: null,
                    userroles: UserRolesToFilter)
                    .ApplyRawFilter(rawfilter)
                    .OrderOnlyByRawSortIfNotNull(rawsort)
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
                                raw => raw.TransformRawData(null, fields, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: null)
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
                                raw => raw.TransformRawData(null, fields, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: null)
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

        private Task<IActionResult> GetSingle(string id,
          string[] fields,
          bool removenullvalues,
          CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                if (Int32.TryParse(id, out var numericid))
                {
                    var query =
                    QueryFactory.Query("rawdata")
                        .Where("id", numericid);
                        //.When(FilterClosedData, q => q.FilterClosedData_Raw());

                    var data = await query.FirstOrDefaultAsync<RawDataStoreWithId?>();
                    
                    //return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: null);
                    return data != null ? data.UseJsonRaw() : new RawDataStoreWithId();
                }
                else
                    return BadRequest("Id could not be found");
                
            });
        }

    }
}
