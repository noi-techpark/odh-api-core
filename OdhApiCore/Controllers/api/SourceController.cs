// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

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
using OdhApiCore.Filters;
using OdhApiCore.Responses;
using ServiceReferenceLCS;
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
    public class SourceController : OdhController
    {
        public SourceController(IWebHostEnvironment env, ISettings settings, ILogger<SourceController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {
        }

        #region SWAGGER Exposed API

        /// <summary>
        /// GET Sources List
        /// </summary>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of IDs, 'null' = No Filter), (default:'null')</param>/// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of SourceLinked Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<SourceLinked>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Source")]
        public async Task<IActionResult> GetSourcesAsync(
            uint? pagenumber = 1,
            PageSize pagesize = null!, 
            string? language = null,
            string? idlist = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {         
            return await Get(pagenumber, pagesize, language, idlist,
                fields: fields ?? Array.Empty<string>(), 
                  searchfilter, rawfilter, rawsort, removenullvalues: removenullvalues,
                    cancellationToken);           
        }

        /// <summary>
        /// GET Source Single
        /// </summary>
        /// <param name="id">ID of the Source</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>SourceLinked Object</returns>
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(SourceLinked), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Source/{id}", Name = "SingleSource")]
        public async Task<IActionResult> GetSourceSingle(string id,
            string? language = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? localizationlanguage = null,   //TODO ignore this in swagger
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            //Compatibility
            if (String.IsNullOrEmpty(language) && !String.IsNullOrEmpty(localizationlanguage))
                language = localizationlanguage;

            return await GetSingle(id, language, fields: fields ?? Array.Empty<string>(), removenullvalues: removenullvalues, cancellationToken);
        }

        #endregion

        #region GETTER

        private Task<IActionResult> Get(
            uint? pagenumber, int? pagesize, string? idfilter, string? language, string[] fields,
            string? searchfilter, string? rawfilter, string? rawsort, bool removenullvalues,
            CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var idlist = Helper.CommonListCreator.CreateIdList(idfilter);

                var query =
                    QueryFactory.Query()
                    .SelectRaw("data")
                    .From("sources")
                    .SourcesWhereExpression(
                        languagelist: new List<string>(),
                        idlist: idlist,
                        searchfilter: searchfilter,
                        language: language,
                        filterClosedData: FilterClosedData, userroles: UserRolesToFilter
                        )
                    .ApplyRawFilter(rawfilter)
                    .ApplyOrdering(new PGGeoSearchResult() { geosearch = false }, rawsort, "data#>>'\\{Shortname\\}'");

                var fieldsTohide = FieldsToHide;


                // Get paginated data
                var data =
                    await query
                        .PaginateAsync<JsonRaw>(
                            page: (int)pagenumber,
                            perPage: pagesize ?? 25);

                var dataTransformed =
                    data.List.Select(
                        raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide)
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
            });
        }      

        private Task<IActionResult> GetSingle(string id, string? language, string[] fields, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var data = await QueryFactory.Query("sources")
                    .Select("data")
                    .Where("id", id.ToLower())
                    .When(FilterClosedData, q => q.FilterClosedData())
                    .FirstOrDefaultAsync<JsonRaw>();

                var fieldsTohide = FieldsToHide;

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide);
            });
        }

        #endregion

        #region POST PUT DELETE

        /// <summary>
        /// POST Insert new Source
        /// </summary>
        /// <param name="Source">SourceLinked Object</param>
        /// <returns>Http Response</returns>
        [Authorize(Roles = "DataWriter,DataCreate,SourceManager,SourceCreate")]
        [HttpPost, Route("Source")]
        public Task<IActionResult> Post([FromBody] SourceLinked source)
        {
            return DoAsyncReturn(async () =>
            {
                source.Id = source.Key.ToLower().Replace(" ", "") ?? Helper.IdGenerator.GenerateIDFromType(source);

                if (source.LicenseInfo == null)
                    source.LicenseInfo = new LicenseInfo() { ClosedData = false };

                return await UpsertData<SourceLinked>(source, "sources", true);
            });
        }

        /// <summary>
        /// PUT Modify existing Source
        /// </summary>
        /// <param name="id">Source Id</param>
        /// <param name="source">Source Object</param>
        /// <returns>Http Response</returns>
        [Authorize(Roles = "DataWriter,DataModify,SourceManager,SourceModify,SourceUpdate")]
        [HttpPut, Route("Source/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] SourceLinked source)
        {
            return DoAsyncReturn(async () =>
            {
                source.Id = Helper.IdGenerator.CheckIdFromType<SourceLinked>(id);

                return await UpsertData<SourceLinked>(source, "sources", false, true);
            });
        }

        /// <summary>
        /// DELETE Source by Id
        /// </summary>
        /// <param name="id">Source Id</param>
        /// <returns>Http Response</returns>
        [Authorize(Roles = "DataWriter,DataDelete,SourceManager,SourceDelete")]
        [HttpDelete, Route("Source/{id}")]
        public Task<IActionResult> Delete(string id)
        {
            return DoAsyncReturn(async () =>
            {
                id = Helper.IdGenerator.CheckIdFromType<SourceLinked>(id);

                return await DeleteData(id, "sources");
            });
        }


        #endregion
    }


}