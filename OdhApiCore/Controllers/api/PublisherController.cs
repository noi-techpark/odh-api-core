// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using AspNetCore.CacheOutput;
using DataModel;
using Helper;
using Helper.Identity;
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
    public class PublisherController : OdhController
    {
        public PublisherController(IWebHostEnvironment env, ISettings settings, ILogger<PublisherController> logger, QueryFactory queryFactory, IOdhPushNotifier odhpushnotifier)
            : base(env, settings, logger, queryFactory, odhpushnotifier)
        {
        }

        #region SWAGGER Exposed API

        /// <summary>
        /// GET Publishers List
        /// </summary>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of IDs, 'null' = No Filter), (default:'null')</param>
        /// <param name="source">Source Filter (possible Values: 'lts','idm'), (default:'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of PublisherLinked Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<PublisherLinked>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Publisher")]
        public async Task<IActionResult> GetPublishersAsync(
            uint? pagenumber = 1,
            PageSize pagesize = null!, 
            string? language = null,
            string? idlist = null,
            string? source = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {         
            return await Get(pagenumber, pagesize, language, idlist, source, 
                fields: fields ?? Array.Empty<string>(), 
                  searchfilter, rawfilter, rawsort, removenullvalues: removenullvalues,
                    cancellationToken);           
        }

        /// <summary>
        /// GET Publisher Single
        /// </summary>
        /// <param name="id">ID of the Publisher</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>PublisherLinked Object</returns>
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(PublisherLinked), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Publisher/{id}", Name = "SinglePublisher")]
        public async Task<IActionResult> GetPublisherSingle(string id,
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
            uint? pagenumber, int? pagesize, string? language, string? idfilter, string? source, string[] fields,
            string? searchfilter, string? rawfilter, string? rawsort, bool removenullvalues,
            CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Read Filters to Add Check
                AdditionalFiltersToAdd.TryGetValue("Read", out var additionalfilter);

                var sourcelist = Helper.CommonListCreator.CreateIdList(source);
                var idlist = Helper.CommonListCreator.CreateIdList(idfilter);

                var query =
                    QueryFactory.Query()
                    .SelectRaw("data")
                    .From("publishers")
                    .PublishersWhereExpression(
                        languagelist: new List<string>(),
                        idlist: idlist,
                        sourcelist: sourcelist,
                        searchfilter: searchfilter,
                        language: language,
                        userroles: UserRolesToFilter
                        )
                    .ApplyRawFilter(rawfilter)
                    .ApplyOrdering(new PGGeoSearchResult() { geosearch = false }, rawsort, "data#>>'\\{Shortname\\}'");

               
                // Get paginated data
                var data =
                    await query
                        .PaginateAsync<JsonRaw>(
                            page: (int)pagenumber,
                            perPage: pagesize ?? 25);

                var dataTransformed =
                    data.List.Select(
                        raw => raw.TransformRawData(language, fields, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: FieldsToHide)
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
                //Additional Read Filters to Add Check
                AdditionalFiltersToAdd.TryGetValue("Read", out var additionalfilter);

                var data = await QueryFactory.Query("publishers")
                    .Select("data")
                    .Where("id", id.ToLower())
                    .FirstOrDefaultAsync<JsonRaw>();

                return data?.TransformRawData(language, fields, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: FieldsToHide);
            });
        }

        #endregion

        #region POST PUT DELETE

        /// <summary>
        /// POST Insert new Publisher
        /// </summary>
        /// <param name="publisher">PublisherLinked Object</param>
        /// <returns>Http Response</returns>
        //[Authorize(Roles = "DataWriter,DataCreate,PublisherManager,PublisherCreate")]
        [AuthorizeODH(PermissionAction.Create)]
        [HttpPost, Route("Publisher")]
        public Task<IActionResult> Post([FromBody] PublisherLinked publisher)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Read Filters to Add Check
                AdditionalFiltersToAdd.TryGetValue("Create", out var additionalfilter);

                publisher.Id = publisher.Key.ToLower().Replace(" ", "") ?? Helper.IdGenerator.GenerateIDFromType(publisher);

                if (publisher.LicenseInfo == null)
                    publisher.LicenseInfo = new LicenseInfo() { ClosedData = false };

                return await UpsertData<PublisherLinked>(publisher, "publishers", true, false, "api", additionalfilter);
            });
        }

        /// <summary>
        /// PUT Modify existing Publisher
        /// </summary>
        /// <param name="id">Publisher Id</param>
        /// <param name="publisher">Publisher Object</param>
        /// <returns>Http Response</returns>
        //[Authorize(Roles = "DataWriter,DataModify,PublisherManager,PublisherModify,PublisherUpdate")]
        [AuthorizeODH(PermissionAction.Update)]
        [HttpPut, Route("Publisher/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] PublisherLinked publisher)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Read Filters to Add Check
                AdditionalFiltersToAdd.TryGetValue("Update", out var additionalfilter);

                publisher.Id = Helper.IdGenerator.CheckIdFromType<PublisherLinked>(id);

                return await UpsertData<PublisherLinked>(publisher, "publishers", false, true, "api", additionalfilter);
            });
        }

        /// <summary>
        /// DELETE Publisher by Id
        /// </summary>
        /// <param name="id">Publisher Id</param>
        /// <returns>Http Response</returns>
        //[Authorize(Roles = "DataWriter,DataDelete,PublisherManager,PublisherDelete")]
        [AuthorizeODH(PermissionAction.Delete)]
        [HttpDelete, Route("Publisher/{id}")]
        public Task<IActionResult> Delete(string id)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Read Filters to Add Check
                AdditionalFiltersToAdd.TryGetValue("Delete", out var additionalfilter);

                id = Helper.IdGenerator.CheckIdFromType<PublisherLinked>(id);

                //return await DeleteData<PublisherLinked>(id, "publishers", additionalfilter); // Does not implement IPublishedOn

                return await DeleteData(id, "publishers", false, additionalfilter);
            });
        }


        #endregion
    }


}