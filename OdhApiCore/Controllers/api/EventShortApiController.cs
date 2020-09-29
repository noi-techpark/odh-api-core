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

namespace OdhApiCore.Controllers.api
{
    /// <summary>
    /// Articles Api (data provided by IDM) SOME DATA Available as OPENDATA 
    /// </summary>
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class EventShortApiController : OdhController
    {
        public EventShortApiController(IWebHostEnvironment env, ISettings settings, ILogger<ActivityController> logger, QueryFactory queryFactory)
           : base(env, settings, logger, queryFactory)
        {
        }

        #region SWAGGER Exposed API

        /// <summary>
        /// GET EventShort List
        /// </summary>
        /// <param name="pagenumber">Pagenumber (Integer)</param>
        /// <param name="pagesize">Pagesize (Integer)</param>
        /// <param name="startdate">Format (yyyy-MM-dd HH:mm) default or Unix Timestamp</param>
        /// <param name="enddate">Format (yyyy-MM-dd HH:mm) default or Unix Timestamp</param>
        /// <param name="datetimeformat">not provided, use default format, for unix timestamp pass "uxtimestamp"</param>
        /// <param name="source">Source of the data, (possible values 'Content' or 'EBMS')</param>
        /// <param name="eventlocation">Event Location, (possible values, 'NOI' or 'EC')</param>
        /// <param name="onlyactive">'true' if only Events marked as Active by Eurac should be displayed</param>
        /// <param name="eventids">comma separated list of event ids</param>
        /// <param name="sortorder">ASC or DESC by StartDate</param>
        /// <param name="webaddress">Searches the webaddress</param>
        /// <returns>Result Object with EventShort List</returns>
        [ProducesResponseType(typeof(Result<EventShort>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet, Route("EventShort")]
        public async Task<IActionResult> Get(
            uint pagenumber = 1, 
            uint pagesize = 1024, 
            string? startdate = null, 
            string? enddate = null, 
            string? datetimeformat = null, 
            string? source = null, 
            string? eventlocation = null, 
            LegacyBool onlyactive = null!, 
            string? eventids = null,
            string? webaddress = null,
            string? sortorder = "ASC",
            string? seed = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? lastchange = null,
            CancellationToken cancellationToken = default
            )
        {
            return await GetEventShortList(
               fields: fields ?? Array.Empty<string>(), language: "", pagenumber: pagenumber, pagesize: pagesize,
               startdate: startdate, enddate: enddate, datetimeformat: datetimeformat, idfilter: eventids,
                   searchfilter: "", sourcefilter: source, eventlocationfilter: eventlocation,
                   webaddressfilter: webaddress, active: onlyactive.Value,
                   sortorder: sortorder, seed: seed, lastchange: lastchange, cancellationToken: cancellationToken);
        }


        /// <summary>
        /// GET EventShort Single
        /// </summary>
        /// <param name="id">Id of the Event</param>
        /// <returns>EventShort Object</returns>
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        /// //[Authorize(Roles = "DataReader,ActivityReader")]
        [ProducesResponseType(typeof(EventShort), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet, Route("EventShort/{id}")]
        public async Task<IActionResult> GetSingle(
            string id,
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            CancellationToken cancellationToken = default)
        {
            return await GetEventShortSingle(id, language, fields: fields ?? Array.Empty<string>(), cancellationToken);
        }

        //Compatibility Route
        [HttpGet, Route("api/EventShort/Detail/{id}")]
        public async Task<IActionResult> GetDetail(
            string id,
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            CancellationToken cancellationToken = default)
        {
            return await GetEventShortSingle(id, language, fields, cancellationToken);
        }

        #endregion

        #region GETTER

        private Task<IActionResult> GetEventShortList(
            string[] fields, string? language, string? searchfilter, uint pagenumber, uint pagesize, string? startdate, string? enddate, string? datetimeformat,
            string? idfilter, string? sourcefilter, string? eventlocationfilter, string? webaddressfilter, bool? active, string? sortorder, string? seed,
            string? lastchange, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {

                EventShortHelper myeventshorthelper = EventShortHelper.Create(startdate, enddate, datetimeformat,
                    sourcefilter, eventlocationfilter, active, idfilter, webaddressfilter, lastchange, sortorder);

                var query =
                   QueryFactory.Query()
                       .SelectRaw("data")
                       .From("eventeuracnoi")
                       .EventShortWhereExpression(
                           idlist: myeventshorthelper.idlist, sourcelist: myeventshorthelper.sourcelist,
                           eventlocationlist: myeventshorthelper.eventlocationlist, webaddresslist: myeventshorthelper.webaddresslist,
                           start: myeventshorthelper.start, end: myeventshorthelper.end, activefilter: myeventshorthelper.activefilter,
                           searchfilter: searchfilter, language: language, lastchange: myeventshorthelper.lastchange,
                           filterClosedData: FilterClosedData)
                      .OrderBySeed(ref seed, "data #>>'\\{StartDate\\}' " + myeventshorthelper.sortorder);
                       
                //.OrderBy("data #>>'\\{StartDate\\}' " + myeventshorthelper.sortorder);                       

                // Get paginated data
                var data =
                    await query
                        .PaginateAsync<JsonRaw>(
                            page: (int)pagenumber,
                            perPage: (int)pagesize);

                var dataTransformed =
                    data.List.Select(
                        raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, urlGenerator: UrlGenerator)
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

        private Task<IActionResult> GetEventShortSingle(
            string id, string? language, string[] fields, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("eventeuracnoi")
                        .Select("data")
                        .Where("id", id.ToLower())
                        .When(FilterClosedData, q => q.FilterClosedData());

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, urlGenerator: UrlGenerator);
            });
        }

        #endregion

        #region CUSTOM METODS 

        #endregion
    }
}
