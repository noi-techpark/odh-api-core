// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

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
using Newtonsoft.Json.Converters;
using Npgsql;
using OdhApiCore.Responses;
using SqlKata;
using SqlKata.Execution;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.api
{
    /// <summary>
    /// EventShort Api (data provided by NOI/EURAC) ALL DATA Available as OPENDATA 
    /// </summary>
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class EventShortController : OdhController
    {        
        public EventShortController(IWebHostEnvironment env, ISettings settings, ILogger<EventShortController> logger, QueryFactory queryFactory)
           : base(env, settings, logger, queryFactory)
        {
        }

        #region SWAGGER Exposed API

        /// <summary>
        /// GET EventShort List
        /// </summary>
        /// <param name="pagenumber">Pagenumber (Integer)</param>
        /// <param name="pagesize">Pagesize (Integer), (default: 'null')</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="startdate">Format (yyyy-MM-dd HH:mm) default or Unix Timestamp</param>
        /// <param name="enddate">Format (yyyy-MM-dd HH:mm) default or Unix Timestamp</param>
        /// <param name="datetimeformat">not provided, use default format, for unix timestamp pass "uxtimestamp"</param>
        /// <param name="source">Source of the data, (possible values 'Content' or 'EBMS')</param>
        /// <param name="eventlocation">Event Location, (possible values, 'NOI' = Events at Noi Techpark, 'EC' = Eurac Events, 'OUT' = Events in other locatiosn)</param>
        /// <param name="onlyactive">'true' if only Events marked as Active should be returned</param>        
        /// <param name="websiteactive">'true' if only Events marked as Active for noi.bz.it should be returned</param>        
        /// <param name="communityactive">'true' if only Events marked as Active for Noi community should be returned</param>        
        /// <param name="eventids">comma separated list of event ids</param>
        /// <param name="sortorder">ASC or DESC by StartDate</param>
        /// <param name="webaddress">Searches the webaddress</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="publishedon">Published On Filter (Separator ',' List of publisher IDs), (default:'null')</param>       
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="optimizedates">Optimizes dates, cuts out all Rooms with Comment "x", revisits and corrects start + enddate</param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Result Object with EventShort List</returns>
        [ProducesResponseType(typeof(Result<EventShort>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet, Route("EventShort")]
        public async Task<IActionResult> Get(
            uint pagenumber = 1, 
            PageSize pagesize = null!, //1024 should be given as standard
            string? startdate = null, 
            string? enddate = null, 
            string? datetimeformat = null, 
            string? source = null,
            [SwaggerEnum(new[] { "NOI", "EC", "VV", "OUT" })]
            [SwaggerParameter("<p>Members:</p><ul><li><i>NOI</i> - NOI Techpark</li> <li><i>EC</i> - Eurac</li> <li><i>VV</i> - Virtual Village</li> <li><i>OUT</i> - Other Location</li> </ul>")]
            string? eventlocation = null, 
            LegacyBool onlyactive = null!,
            LegacyBool websiteactive = null!,
            LegacyBool communityactive = null!,
            string? eventids = null,
            string? webaddress = null,
            string? sortorder = "ASC",
            string? seed = null,
            string? language = null,
            string? langfilter = null,
            bool optimizedates = false,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? lastchange = null,
            string? publishedon = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,            
            bool removenullvalues = false,
            CancellationToken cancellationToken = default
            )
        {
            return await GetEventShortList(
               fields: fields ?? Array.Empty<string>(), language: language, pagenumber: pagenumber, pagesize: pagesize,
               startdate: startdate, enddate: enddate, datetimeformat: datetimeformat, idfilter: eventids,
                   searchfilter: searchfilter, sourcefilter: source, eventlocationfilter: eventlocation,
                   webaddressfilter: webaddress, active: onlyactive.Value, websiteactive: websiteactive.Value, communityactive: communityactive.Value, optimizedates: optimizedates,
                   sortorder: sortorder, seed: seed, lastchange: lastchange, publishedon: publishedon, 
                   rawfilter: rawfilter, rawsort: rawsort,  removenullvalues: removenullvalues, 
                   cancellationToken: cancellationToken);
        }

        /// <summary>
        /// GET EventShort Single
        /// </summary>
        /// <param name="id">Id of the Event</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="optimizedates">Optimizes dates, cuts out all Rooms with Comment "x", revisits and corrects start + enddate</param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>EventShort Object</returns>
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        /// //[Authorize(Roles = "DataReader,ActivityReader")]
        [ProducesResponseType(typeof(EventShort), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet, Route("EventShort/Detail/{id}")]
        [HttpGet, Route("EventShort/{id}", Name = "SingleEventShort")]
        public async Task<IActionResult> GetSingle(
            string id,
            string? language,
            bool optimizedates = false,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetEventShortSingle(id, language, optimizedates: optimizedates, fields: fields ?? Array.Empty<string>(), removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET EventShort List by Room Occupation
        /// </summary>
        /// <param name="startdate">Format (yyyy-MM-dd HH:mm) default or Unix Timestamp</param>
        /// <param name="enddate">Format (yyyy-MM-dd HH:mm) default or Unix Timestamp</param>
        /// <param name="datetimeformat">not provided, use default format, for unix timestamp pass "uxtimestamp"</param>
        /// <param name="source">Source of the data, (possible values 'Content' or 'EBMS')</param>
        /// <param name="eventlocation">Event Location, (possible values, 'NOI' = Events at Noi Techpark, 'EC' = Eurac Events, 'OUT' = Events in other locatiosn)</param>    
        /// <param name="onlyactive">'true' if only Events marked as Active should be returned</param>        
        /// <param name="websiteactive">'true' if only Events marked as Active for noi.bz.it should be returned</param>        
        /// <param name="communityactive">'true' if only Events marked as Active for Noi community should be returned</param>        
        /// <param name="eventids">comma separated list of event ids</param>
        /// <param name="webaddress">Filter by WebAddress Field</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="publishedon">Published On Filter (Separator ',' List of publisher IDs), (default:'null')</param>       
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>List of EventShortByRoom Objects</returns>
        [ProducesResponseType(typeof(IEnumerable<EventShortByRoom>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet, Route("EventShort/GetbyRoomBooked")]
        public async Task<IActionResult> GetbyRoomBooked(
            string? startdate = null, 
            string? enddate = null, 
            string? datetimeformat = null, 
            string? source = null,
            [SwaggerEnum(new[] { "NOI", "EC", "VV", "OUT" })]
            [SwaggerParameter("<p>Members:</p><ul><li><i>NOI</i> - NOI Techpark</li> <li><i>EC</i> - Eurac</li> <li><i>VV</i> - Virtual Village</li> <li><i>OUT</i> - Other Location</li> </ul>")]
            string? eventlocation = null,
            LegacyBool onlyactive = null!,
            LegacyBool websiteactive = null!,
            LegacyBool communityactive = null!,
            string? eventids = null, 
            string? webaddress = null,
            string? language = null,
            string? langfilter = null,
            string? lastchange = null,
            string? updatefrom = null,
            string? publishedon = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,            
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            if (updatefrom == null && lastchange != null)
                updatefrom = lastchange;

            return await GetEventShortListbyRoomBooked(startdate, enddate, datetimeformat, source, eventlocation, onlyactive.Value, 
                websiteactive.Value, communityactive.Value, eventids, webaddress, publishedon, updatefrom, language, null, 
                searchfilter, fields: fields ?? Array.Empty<string>(), rawfilter, rawsort, removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET NOI Room Mapping
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("EventShort/RoomMapping")]
        public async Task<IDictionary<string, string>> GetBDPNoiRoomsWithLinkDictionary(
            string? language = "en"
            )
        {
            return await GetBDPNoiRoomsWithLink(language);
        }

        /// <summary>
        /// GET EventShort Types
        /// </summary>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="type">Type to filter for ('TechnologyFields','CustomTagging')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of EventShortType Object</returns> 
        //[ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("EventShortTypes")]
        public async Task<IActionResult> GetEventShortTypes(
            string? language,
            string? type = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetEventShortTypesList(language, type, fields: fields ?? Array.Empty<string>(), searchfilter, rawfilter, rawsort, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET EventShort Type Single
        /// </summary>
        /// <param name="id">ID of the EventShort Type</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>ActivityPoiType Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(SmgPoiTypes), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("EventShortTypes/{*id}", Name = "SingleEventShortTypes")]
        public async Task<IActionResult> GetEventShortTypesSingle(
            string id,
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetEventShortType(id, language, fields: fields ?? Array.Empty<string>(), removenullvalues: removenullvalues, cancellationToken);
        }

        #endregion

        #region GETTER

        private Task<IActionResult> GetEventShortList(
            string[] fields, string? language, string? searchfilter, uint pagenumber, int? pagesize, string? startdate, string? enddate, string? datetimeformat,
            string? idfilter, string? sourcefilter, string? eventlocationfilter, string? webaddressfilter, bool? active, bool? websiteactive, bool? communityactive, bool optimizedates, string? sortorder, string? seed,
            string? lastchange, string? publishedon, string? rawfilter, string? rawsort, bool removenullvalues,  CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {

                EventShortHelper myeventshorthelper = EventShortHelper.Create(startdate, enddate, datetimeformat,
                    sourcefilter, eventlocationfilter, active, websiteactive, communityactive, idfilter, webaddressfilter, lastchange, sortorder, publishedon);

                var query =
                   QueryFactory.Query()
                       .SelectRaw("data")
                       .From("eventeuracnoi")
                       .EventShortWhereExpression(
                           idlist: myeventshorthelper.idlist, sourcelist: myeventshorthelper.sourcelist,
                           eventlocationlist: myeventshorthelper.eventlocationlist, webaddresslist: myeventshorthelper.webaddresslist,
                           start: myeventshorthelper.start, end: myeventshorthelper.end, activefilter: myeventshorthelper.activefilter,
                           websiteactivefilter: myeventshorthelper.websiteactivefilter, communityactivefilter: myeventshorthelper.communityactivefilter,
                           publishedonlist: myeventshorthelper.publishedonlist, searchfilter: searchfilter, language: language, lastchange: myeventshorthelper.lastchange,
                           filterClosedData: FilterClosedData, userroles: UserRolesToFilter, getbyrooms: false)
                       .ApplyRawFilter(rawfilter)
                       .ApplyOrdering(ref seed, new PGGeoSearchResult() { geosearch = false }, rawsort, "data #>>'\\{StartDate\\}' " + myeventshorthelper.sortorder);

                // Get paginated data
                var data =
                    (await query
                        .PaginateAsync<JsonRaw>(
                            page: (int)pagenumber,
                            perPage: pagesize ?? 25));

                if (optimizedates)
                    data.List = OptimizeRoomForAppList(data.List);

                var fieldsTohide = FieldsToHide;

                var dataTransformed =
                    data.List.Select(
                        raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide)
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
            string id, string? language, bool optimizedates, string[] fields, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("eventeuracnoi")
                        .Select("data")
                        .Where("id", id.ToLower())
                        .When(FilterClosedData, q => q.FilterClosedData());

                var fieldsTohide = FieldsToHide;

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                if (optimizedates && data is { })
                    data = OptimizeRoomForApp(data);

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide);
            });
        }

        #endregion

        #region CUSTOM METODS 

        private async Task<IActionResult> GetEventShortListbyRoomBooked(
            string? startdate, string? enddate, string? datetimeformat, string? sourcefilter, string? eventlocationfilter, 
            bool? active, bool? websiteactive, bool? communityactive, string? idfilter, string? webaddressfilter, string? publishedon, string? lastchange, string? language, string? sortorder, 
            string? searchfilter, string[] fields, string? rawfilter, string? rawsort, bool removenullvalues, CancellationToken cancellationToken)
        {

            EventShortHelper myeventshorthelper = EventShortHelper.Create(startdate, enddate, datetimeformat,
                  sourcefilter, eventlocationfilter, active, websiteactive, communityactive, idfilter, webaddressfilter, lastchange, sortorder, publishedon);

            var query =
               QueryFactory.Query()
                   .SelectRaw("data")
                   .From("eventeuracnoi")
                   .EventShortWhereExpression(
                       idlist: myeventshorthelper.idlist, sourcelist: myeventshorthelper.sourcelist,
                       eventlocationlist: myeventshorthelper.eventlocationlist, webaddresslist: myeventshorthelper.webaddresslist,
                       start: myeventshorthelper.start, end: myeventshorthelper.end, activefilter: myeventshorthelper.activefilter,
                       websiteactivefilter: myeventshorthelper.websiteactivefilter, communityactivefilter: myeventshorthelper.communityactivefilter,
                       publishedonlist: myeventshorthelper.publishedonlist, searchfilter: searchfilter, language: language, lastchange: myeventshorthelper.lastchange,
                       filterClosedData: FilterClosedData, userroles: UserRolesToFilter, getbyrooms: false)
                   .ApplyRawFilter(rawfilter);

            var data =
                    await query
                            .GetAsync<string>();

            var eventshortlist = data.Select(x => JsonConvert.DeserializeObject<EventShort>(x)!).ToList();

            var result = TransformEventShortToRoom(eventshortlist, myeventshorthelper.start, myeventshorthelper.end);

            IEnumerable<JsonRaw> resultraw = result.Select(x => new JsonRaw(x));

            var fieldsTohide = FieldsToHide;

            var dataTransformed =
                    resultraw.Select(
                        raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide)
                    );
            
            return Ok(dataTransformed);
        }

        private IEnumerable<EventShortByRoom> TransformEventShortToRoom(IEnumerable<EventShort> eventsshort, DateTime start, DateTime end)
        {
            List<EventShortByRoom> eventshortlistbyroom = new List<EventShortByRoom>();

            foreach (EventShort eventshort in eventsshort)
            {
                // 1 Event goes for more days
                //Hack, if Event Room is 

                foreach (RoomBooked room in eventshort.RoomBooked ?? new List<RoomBooked>())
                {
                    //Display room only if not comment x inserted
                    //if (room.Comment?.ToLower() != "x" && room.EndDate >= DateTime.Now)
                    if (room.Comment?.ToLower() != "x" && room.EndDate >= start)
                    {
                        if (room.StartDate <= end)
                        {

                            EventShortByRoom myeventshortbyroom = new EventShortByRoom();

                            myeventshortbyroom.EventAnchorVenue = eventshort.AnchorVenue;
                            myeventshortbyroom.EventAnchorVenueShort = eventshort.AnchorVenueShort;
                            myeventshortbyroom.EventDescriptionDE = eventshort.EventDescriptionDE;
                            myeventshortbyroom.EventDescriptionEN = eventshort.EventDescriptionEN;
                            myeventshortbyroom.EventDescriptionIT = eventshort.EventDescriptionIT;

                      
                            myeventshortbyroom.EventDescription.TryAddOrUpdate("de", eventshort.EventDescriptionDE != null ? eventshort.EventDescriptionDE.Trim() : "");
                            myeventshortbyroom.EventDescription.TryAddOrUpdate("it", eventshort.EventDescriptionIT != null ? eventshort.EventDescriptionIT.Trim() : "");
                            myeventshortbyroom.EventDescription.TryAddOrUpdate("en", eventshort.EventDescriptionEN != null ? eventshort.EventDescriptionEN.Trim() : "");

                            if (String.IsNullOrEmpty(myeventshortbyroom.EventDescription["it"]) && eventshort.EventDescriptionDE != null)
                                myeventshortbyroom.EventDescription?.TryAddOrUpdate("it", eventshort.EventDescriptionDE);
                            if (String.IsNullOrEmpty(myeventshortbyroom?.EventDescription?["en"]) && eventshort.EventDescriptionDE != null)
                                myeventshortbyroom!.EventDescription?.TryAddOrUpdate("en", eventshort.EventDescriptionDE);

                            myeventshortbyroom.EventTitle = myeventshortbyroom.EventDescription;

                         

                            myeventshortbyroom!.EventEndDate = eventshort.EndDate;
                            myeventshortbyroom.EventEndDateUTC = eventshort.EndDateUTC;
                            myeventshortbyroom.EventId = eventshort.EventId.Value;
                            myeventshortbyroom.EventLocation = eventshort.EventLocation;
                            myeventshortbyroom.EventSource = eventshort.Source;
                            myeventshortbyroom.EventStartDate = eventshort.StartDate;
                            myeventshortbyroom.EventStartDateUTC = eventshort.StartDateUTC;

                            myeventshortbyroom.EventWebAddress = eventshort.WebAddress;

                            myeventshortbyroom.Id = eventshort.Id;

                            myeventshortbyroom.RoomEndDate = room.EndDate;
                            myeventshortbyroom.RoomEndDateUTC = room.EndDateUTC;
                            myeventshortbyroom.RoomStartDate = room.StartDate;
                            myeventshortbyroom.RoomStartDateUTC = room.StartDateUTC;
                            myeventshortbyroom.SpaceDesc = room.SpaceDesc;
                            myeventshortbyroom.SpaceType = room.SpaceType;
                            myeventshortbyroom.Subtitle = room.Subtitle;

                            string roomname = room.SpaceDesc ?? "";
                            if (roomname.StartsWith("NOI ") || roomname.StartsWith("Noi ") || roomname.StartsWith("noi "))
                                roomname = roomname.Remove(0, 3).Trim();

                            myeventshortbyroom.SpaceDescList.Add(roomname);
                            myeventshortbyroom.CompanyName = eventshort.CompanyName;
                            if (myeventshortbyroom.CompanyName == null)
                                myeventshortbyroom.CompanyName = "";


                            // New Fields to display
                            myeventshortbyroom.ActiveWeb = eventshort.ActiveWeb;
                            myeventshortbyroom.ImageGallery = eventshort.ImageGallery != null ? eventshort.ImageGallery : new List<ImageGallery>();
                            myeventshortbyroom.VideoUrl = eventshort.VideoUrl != null ? eventshort.VideoUrl : "";
                            // Same Logic

                            string textde = eventshort.EventTextDE != null ? eventshort.EventTextDE : "";

                            myeventshortbyroom.EventTextDE = textde;
                            myeventshortbyroom.EventTextIT = eventshort.EventTextIT;
                            myeventshortbyroom.EventTextEN = eventshort.EventTextEN;


                            if (string.IsNullOrEmpty(myeventshortbyroom.EventTextIT))
                                myeventshortbyroom.EventTextIT = textde;
                            if (string.IsNullOrEmpty(myeventshortbyroom.EventTextEN))
                                myeventshortbyroom.EventTextEN = textde;

                            myeventshortbyroom.EventText.TryAddOrUpdate("de", eventshort.EventTextDE != null ? eventshort.EventTextDE.Trim() : "");
                            myeventshortbyroom.EventText.TryAddOrUpdate("it", eventshort.EventTextIT != null ? eventshort.EventTextIT.Trim() : "");
                            myeventshortbyroom.EventText.TryAddOrUpdate("en", eventshort.EventTextEN != null ? eventshort.EventTextEN.Trim() : "");

                            myeventshortbyroom.TechnologyFields = eventshort.TechnologyFields != null ? eventshort.TechnologyFields : new List<string>();
                            myeventshortbyroom.CustomTagging = eventshort.CustomTagging != null ? eventshort.CustomTagging : new List<string>();

                            if (eventshort.EventDocument != null)
                            {
                                if (eventshort.EventDocument.Count > 0)
                                {
                                    foreach (var eventdocument in eventshort.EventDocument)
                                    {
                                        if (!myeventshortbyroom.EventDocument?.ContainsKey(eventdocument.Language ?? "") ?? false)
                                            myeventshortbyroom!.EventDocument?.TryAddOrUpdate(eventdocument.Language ?? "", eventdocument.DocumentURL ?? "");
                                    }
                                }
                            }

                            if (eventshort.SoldOut != null)
                                myeventshortbyroom.SoldOut = (bool)eventshort.SoldOut;
                            else
                                myeventshortbyroom.SoldOut = false;


                            if (eventshort.ExternalOrganizer != null)
                                myeventshortbyroom.ExternalOrganizer = (bool)eventshort.ExternalOrganizer;
                            else
                                myeventshortbyroom.ExternalOrganizer = false;


                            if (!string.IsNullOrEmpty(eventshort.Display5))
                                myeventshortbyroom.CompanyName = eventshort.Display5;

                            AddToRoomList(eventshortlistbyroom, myeventshortbyroom);
                        }
                    }
                }
            }

            return eventshortlistbyroom.OrderBy(x => x.RoomStartDate);
        }

        private void AddToRoomList(List<EventShortByRoom> eventshortlistbyroom, EventShortByRoom roomtoadd)
        {
            // Sieh nach ob bereits ein Event mit demselben namen und 
            var sameevent = eventshortlistbyroom.Where(x => x.Id == roomtoadd.Id
                                                        && x.RoomStartDate == roomtoadd.RoomStartDate
                                                        && x.RoomEndDate == roomtoadd.RoomEndDate
                                                        && x.Subtitle == roomtoadd.Subtitle).FirstOrDefault();

            if (sameevent != null)
            {
                string roomname = roomtoadd.SpaceDesc ?? "";
                if (roomname.StartsWith("NOI ") || roomname.StartsWith("Noi ") || roomname.StartsWith("noi "))
                    roomname = roomname.Remove(0, 3).Trim();

                sameevent.SpaceDescList.Add(roomname);
            }
            else
                eventshortlistbyroom.Add(roomtoadd);

        }

        private IEnumerable<JsonRaw> OptimizeRoomForAppList(IEnumerable<JsonRaw> data)
        {
            List<JsonRaw> datanew = new List<JsonRaw>();
            foreach (var eventshortraw in data)
            {
                datanew.Add(OptimizeRoomForApp(eventshortraw));
            }

            return datanew;
            //data = eventshortlist.Select(x => new JsonRaw(x));
        }

        private JsonRaw OptimizeRoomForApp(JsonRaw eventshortraw)
        {
            //var eventshortlist = data.Select(x => JsonConvert.DeserializeObject<EventShort>(x.Value)!).ToList();
            var eventshort = JsonConvert.DeserializeObject<EventShort>(eventshortraw.Value) ?? new();

            // TODO: Remove all Rooms with x
            // Starting and ending date check with rooms remaining
            if (eventshort.RoomBooked != null && eventshort.RoomBooked.Count > 0)
            {
                eventshort.RoomBooked.RemoveAll(x => x.Comment == "x" || x.Comment == "X");
            }

            // Get the lowest room start date
            var firstbegindate = eventshort.RoomBooked?.OrderBy(x => x.StartDate).Select(x => x.StartDate).FirstOrDefault();
            var lastenddate = eventshort.RoomBooked?.OrderByDescending(x => x.EndDate).Select(x => x.EndDate).FirstOrDefault();

            if (firstbegindate.HasValue && firstbegindate != eventshort.StartDate)
            {
                eventshort.StartDate = firstbegindate.Value;
                eventshort.StartDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(firstbegindate.Value);
            }

            if (lastenddate.HasValue && lastenddate != eventshort.EndDate)
            {
                eventshort.EndDate = lastenddate.Value;
                eventshort.EndDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(lastenddate.Value);
            }

            return new JsonRaw(eventshort);
        }

        #endregion

        #region EVENTSHORT TYPES

        private Task<IActionResult> GetEventShortTypesList(string? language, string? typefilter, string[] fields, string? searchfilter, string? rawfilter, string? rawsort, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("eventshorttypes")
                        .SelectRaw("data")
                        .When(FilterClosedData, q => q.FilterClosedData())
                        .When(!String.IsNullOrEmpty(typefilter), q => q.WhereRaw("data->>'Type' ILIKE $$", typefilter))
                        .SearchFilter(PostgresSQLWhereBuilder.TypeDescFieldsToSearchFor(language), searchfilter)
                        .ApplyRawFilter(rawfilter)
                        .OrderOnlyByRawSortIfNotNull(rawsort);

                var data = await query.GetAsync<JsonRaw?>();

                var fieldsTohide = FieldsToHide;

                var dataTransformed =
                    data.Select(
                        raw => raw?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide)
                    );

                return dataTransformed;
            });
        }

        private Task<IActionResult> GetEventShortType(string id, string? language, string[] fields, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("eventshorttypes")
                        .Select("data")
                        //.WhereJsonb("Key", "ilike", id)
                        .Where("id", id.ToLower())
                        .When(FilterClosedData, q => q.FilterClosedData());
                //.Where("Key", "ILIKE", id);

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                var fieldsTohide = FieldsToHide;

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide);
            });
        }

        #endregion

        #region POST PUT DELETE

        // POST: api/EventShort
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[EnableCors("DataBrowserCorsPolicy")]
        [Authorize(Roles = "DataWriter,DataCreate,EventShortManager,EventShortCreate,VirtualVillageManager")]
        [ProducesResponseType(typeof(PGCRUDResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[OdhAuthorizeAttribute("DataWriter,DataCreate,EventShortManager,EventShortModify,VirtualVillageManager")]
        [HttpPost, Route("EventShort")]
        //[InvalidateCacheOutput("GetReducedAsync")]
        public async Task<IActionResult> Post([FromBody] EventShortLinked eventshort)
        {
            try
            {
                if (eventshort != null)
                {
                    //Adding Source content if not defined
                    if (String.IsNullOrEmpty(eventshort.Source))
                        eventshort.Source = "Content";

                    if (eventshort.EventLocation == null)
                        throw new Exception("EvenLocation needed");
                    
                    eventshort.EventLocation = eventshort.EventLocation.ToUpper();

                    if (CheckIfUserIsOnlyInRole("VirtualVillageManager", new List<string>() { "DataWriter", "DataCreate", "EventShortManager", "EventShortCreate" }) && eventshort.EventLocation != "VV")
                        throw new Exception("VirtualVillageManager can only insert Virtual Village Events");

                    if (CheckIfUserIsOnlyInRole("VirtualVillageManager", new List<string>() { "DataWriter", "DataCreate", "EventShortManager", "EventShortCreate" }))
                        eventshort.Source = User.Identity != null ? User.Identity.Name :"";

                    if (eventshort.StartDate == DateTime.MinValue || eventshort.EndDate == DateTime.MinValue)
                        throw new Exception("Start + End Time not set correctly");

                    eventshort.ChangedOn = DateTime.Now;
                    eventshort.LastChange = eventshort.ChangedOn;

                    eventshort.CheckMyInsertedLanguages();

                    eventshort.AnchorVenueShort = eventshort.AnchorVenue;

                    //Save DAtetime without Offset??
                    //eventshort.StartDate = DateTime.SpecifyKind(eventshort.StartDate, DateTimeKind.Utc);
                    //eventshort.EndDate = DateTime.SpecifyKind(eventshort.EndDate, DateTimeKind.Utc);

                    eventshort.EndDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(eventshort.EndDate);
                    eventshort.StartDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(eventshort.StartDate);

                    bool addroomautomatically = false;

                    if (eventshort.RoomBooked == null)
                    {
                        addroomautomatically = true;
                    }
                    else if (eventshort.RoomBooked.Count == 0)
                    {
                        addroomautomatically = true;
                    }

                    if (addroomautomatically)
                    {
                        //Default, Add Eventdate as Room
                        RoomBooked myroom = new RoomBooked();
                        myroom.StartDate = eventshort.StartDate;
                        myroom.EndDate = eventshort.EndDate;
                        myroom.EndDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(eventshort.EndDate);
                        myroom.StartDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(eventshort.StartDate);
                        myroom.Comment = "";
                        myroom.Space = eventshort.AnchorVenueShort;
                        myroom.SpaceAbbrev = eventshort.AnchorVenue;
                        myroom.SpaceDesc = eventshort.AnchorVenue;
                        //myroom.Space = eventshort.AnchorVenue;

                        if (eventshort.EventLocation == "NOI")
                            myroom.SpaceType = "NO";
                        else
                            myroom.SpaceType = eventshort.EventLocation;

                        myroom.Subtitle = "";

                        eventshort.RoomBooked = new List<RoomBooked>();
                        eventshort.RoomBooked.Add(myroom);
                    }
                    else
                    {
                        if(eventshort.RoomBooked != null)
                        {
                            foreach (var room in eventshort.RoomBooked)
                            {
                                //Save DAtetime without Offset??
                                //room.StartDate = DateTime.SpecifyKind(room.StartDate, DateTimeKind.Utc);
                                //room.EndDate = DateTime.SpecifyKind(room.EndDate, DateTimeKind.Utc);

                                room.EndDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(room.EndDate);
                                room.StartDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(room.StartDate);
                            }
                        }
                    }

                    //Event Title IT EN
                    if (string.IsNullOrEmpty(eventshort.EventDescriptionIT))
                        eventshort.EventTitle.TryAddOrUpdate("it",eventshort.EventDescriptionDE);

                    if (string.IsNullOrEmpty(eventshort.EventDescriptionEN))
                        eventshort.EventTitle.TryAddOrUpdate("en", eventshort.EventDescriptionDE);

                    //TraceSource tracesource = new TraceSource("CustomData");
                    //tracesource.TraceEvent(TraceEventType.Information, 0, "Event Start Date:" + String.Format("{0:dd/MM/yyyy hh:mm}", eventshort.StartDate));

                    string author = "unknown";
                    if (User.Identity != null && User.Identity.Name != null)
                        author = User.Identity.Name;

                    //LicenseInfo
                    eventshort.LicenseInfo = new LicenseInfo() { Author = author, ClosedData = false, LicenseHolder = "https://noi.bz.it/", License = "CC0" };
                    //eventshort._Meta = MetadataHelper.GetMetadataobject<EventShortLinked>(eventshort, MetadataHelper.GetMetadataforEventShort);
                    //eventshort._Meta.LastUpdate = eventshort.LastChange;

                    //PostgresSQLHelper.InsertDataIntoTable(conn, "eventeuracnoi", JsonConvert.SerializeObject(eventshort), eventshort.Id);
                    //tracesource.TraceEvent(TraceEventType.Information, 0, "Serialized object:" + JsonConvert.SerializeObject(eventshort));

                    //check if this works
                    //var query = await QueryFactory.Query("eventeuracnoi").InsertAsync(new JsonBData() { id = eventshort.Id, data = new JsonRaw(eventshort) });
                    //return Ok(new GenericResultExtended() { Message = "INSERT EventShort succeeded, Id:" + eventshort.Id, Id = eventshort.Id });

                    //GENERATE ID
                    eventshort.Id = Helper.IdGenerator.GenerateIDFromType(eventshort);

                    //Take Display1 value and set it to ActiveToday
                    if (eventshort.ActiveToday == true)
                        eventshort.Display1 = "Y";
                    if (eventshort.ActiveToday == false)
                        eventshort.Display1 = "N";

                    //Create PublishedonList
                    //PublishedOnHelper.CreatePublishedOnList<EventShortLinked>(eventshort);
                    //TODO, Sync publishedon with current values, remove SETTER from ActiveToday, ActiveWeb, ActiveCommunity and generate it from publishedon, remove 
                    //checkboxes from 

                    return await UpsertData<EventShortLinked>(eventshort, "eventeuracnoi", true);
                }
                else
                {
                    throw new Exception("No EventShort Data provided");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/EventShort/5
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,EventShortManager,EventShortModify,VirtualVillageManager")]
        [ProducesResponseType(typeof(PGCRUDResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[OdhAuthorizeAttribute("DataWriter,DataCreate,EventShortManager,EventShortModify,VirtualVillageManager")]
        [HttpPut, Route("EventShort/{id}")]
        //[InvalidateCacheOutput("GetReducedAsync")]
        public async Task<IActionResult> Put(string id, [FromBody] EventShortLinked eventshort)
        {
            try
            {
                if (eventshort != null && id != null)
                {
                    //Adding Source content if not defined
                    if (String.IsNullOrEmpty(eventshort.Source))
                        eventshort.Source = "Content";

                    if (eventshort.EventLocation == null)
                        throw new Exception("Eventlocation needed");

                    eventshort.EventLocation = eventshort.EventLocation.ToUpper();

                    if (CheckIfUserIsOnlyInRole("VirtualVillageManager", new List<string>() { "DataWriter", "DataCreate", "EventShortManager", "EventShortModify" }) && eventshort.EventLocation != "VV")
                        throw new Exception("VirtualVillageManager can only insert Virtual Village Events");

                    if(CheckIfUserIsOnlyInRole("VirtualVillageManager", new List<string>() { "DataWriter", "DataCreate", "EventShortManager", "EventShortModify" }))
                        eventshort.Source = User.Identity != null ? User.Identity.Name : "";

                    eventshort.ChangedOn = DateTime.Now;
                    eventshort.LastChange = eventshort.ChangedOn;
                    eventshort.CheckMyInsertedLanguages();
                    //eventshort._Meta.LastUpdate = eventshort.LastChange;

                    eventshort.AnchorVenueShort = eventshort.AnchorVenue;

                    eventshort.EndDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(eventshort.EndDate);
                    eventshort.StartDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(eventshort.StartDate);

                    //TODO on rooms
                    if (eventshort.RoomBooked != null)
                    {
                        foreach (var room in eventshort.RoomBooked)
                        {
                            room.EndDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(room.EndDate);
                            room.StartDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(room.StartDate);
                        }
                    }

                    //Event Title IT EN
                    if (string.IsNullOrEmpty(eventshort.EventDescriptionIT))
                        eventshort.EventTitle.TryAddOrUpdate("it", eventshort.EventDescriptionDE);

                    if (string.IsNullOrEmpty(eventshort.EventDescriptionEN))
                        eventshort.EventTitle.TryAddOrUpdate("en", eventshort.EventDescriptionDE);

                    //TODO CHECK IF THIS WORKS     
                    //var updatequery = await QueryFactory.Query("eventeuracnoi").Where("id", id)
                    //    .UpdateAsync(new JsonBData() { id = eventshort.Id ?? "", data = eventshort != null ? new JsonRaw(eventshort) : null });

                    //return Ok(new GenericResultExtended() { Message = String.Format("UPDATE eventshort succeeded, Id:{0}", eventshort?.Id), Id = eventshort?.Id });

                    //Take Display1 value and set it to ActiveToday
                    if (eventshort.ActiveToday == true)
                        eventshort.Display1 = "Y";
                    if (eventshort.ActiveToday == false)
                        eventshort.Display1 = "N";

                    //Check ID uppercase lowercase
                    eventshort.Id = Helper.IdGenerator.CheckIdFromType<EventShortLinked>(id);

                    //Create PublishedonList
                    //PublishedOnHelper.CreatePublishedOnList<EventShortLinked>(eventshort);

                    return await UpsertData<EventShortLinked>(eventshort, "eventeuracnoi", false, true);
                }
                else
                {
                    throw new Exception("No eventshort Data provided");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/EventShort/5
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,EventShortManager,EventShortDelete,VirtualVillageManager")]
        [ProducesResponseType(typeof(PGCRUDResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[OdhAuthorizeAttribute("DataWriter,DataCreate,EventShortManager,EventShortModify,VirtualVillageManager")]
        [HttpDelete, Route("EventShort/{id}")]
        //[InvalidateCacheOutput("GetReducedAsync")]        
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (id != null)
                {
                    var query =
                         QueryFactory.Query("eventeuracnoi")
                             .Select("data")
                             .Where("id", id.ToLower());

                    //var myeventraw = await query.FirstOrDefaultAsync<JsonRaw>();                    
                    //var myevent = JsonConvert.DeserializeObject<EventShort>(myeventraw.Value);

                    var myevent = await query.GetObjectSingleAsync<EventShort>();

                    if (myevent != null)
                    {
                        if (myevent.Source != "EBMS")
                        {
                            if (CheckIfUserIsOnlyInRole("VirtualVillageManager", new List<string>() { "DataWriter", "DataDelete", "EventShortManager", "EventShortDelete" }) && myevent.EventLocation != "VV")
                                throw new Exception("VirtualVillageManager can only delete Virtual Village Events");

                            //TODO CHECK IF THIS WORKS     
                            //var deletequery = await QueryFactory.Query("eventeuracnoi").Where("id", id).DeleteAsync();

                            //return Ok(new GenericResult() { Message = "DELETE EventShort succeeded, Id:" + id });

                            return await DeleteData(id, "eventeuracnoi");
                        }
                        else
                        {
                            if (User.IsInRole("VirtualVillageManager") && myevent.EventLocation == "VV")
                            {
                                //TODO CHECK IF THIS WORKS     
                                //var deletequery = await QueryFactory.Query("eventeuracnoi").Where("id", id).DeleteAsync();

                                //return Ok(new GenericResult() { Message = "DELETE EventShort succeeded, Id:" + id });

                                return await DeleteData(id, "eventeuracnoi");
                            }
                            else
                            {
                                throw new Exception("EventShort cannot be deleted");
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("EventShort not found");
                    }
                }
                else
                {
                    throw new Exception("No EventShort Id provided");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        //[KeyCloakAuthorizationFilter(Roles = "VirtualVillageManager")]
        //[HttpPost, Route("api/EventShortVirtualVillage")]
        //[InvalidateCacheOutput("GetReducedAsync")]
        //public HttpResponseMessage PostVirtualVillage([FromBody] EventShort eventshort)
        //{
        //    try
        //    {
        //        if (eventshort != null)
        //        {
        //            if (!User.IsInRole("VirtualVillageManager"))
        //                throw new Exception("Not Allowed");

        //            if (eventshort.EventLocation == null)
        //                throw new Exception("Eventlocation needed");

        //            eventshort.EventLocation = eventshort.EventLocation.ToUpper();
        //            eventshort.Source = User.Identity.Name;

        //            if (eventshort.EventLocation != "VV")
        //                throw new Exception("VirtualVillageManager can only insert Virtual Village Events");

        //            if (eventshort.StartDate == DateTime.MinValue || eventshort.EndDate == DateTime.MinValue)
        //                throw new Exception("Start + End Time not set correctly");

        //            using (var conn = new NpgsqlConnection(GlobalPGConnection.PGConnectionString))
        //            {
        //                conn.Open();

        //                eventshort.ChangedOn = DateTime.Now;

        //                eventshort.AnchorVenueShort = eventshort.AnchorVenue;

        //                //Save DAtetime without Offset??
        //                //eventshort.StartDate = DateTime.SpecifyKind(eventshort.StartDate, DateTimeKind.Utc);
        //                //eventshort.EndDate = DateTime.SpecifyKind(eventshort.EndDate, DateTimeKind.Utc);

        //                eventshort.EndDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(eventshort.EndDate);
        //                eventshort.StartDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(eventshort.StartDate);

        //                bool addroomautomatically = false;

        //                if (eventshort.RoomBooked == null)
        //                {
        //                    addroomautomatically = true;
        //                }
        //                else if (eventshort.RoomBooked.Count == 0)
        //                {
        //                    addroomautomatically = true;
        //                }

        //                if (addroomautomatically)
        //                {
        //                    //Default, Add Eventdate as Room
        //                    RoomBooked myroom = new RoomBooked();
        //                    myroom.StartDate = eventshort.StartDate;
        //                    myroom.EndDate = eventshort.EndDate;
        //                    myroom.EndDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(eventshort.EndDate);
        //                    myroom.StartDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(eventshort.StartDate);
        //                    myroom.Comment = "";
        //                    myroom.Space = eventshort.AnchorVenueShort;
        //                    myroom.SpaceAbbrev = eventshort.AnchorVenue;
        //                    myroom.SpaceDesc = eventshort.AnchorVenue;
        //                    //myroom.Space = eventshort.AnchorVenue;

        //                    if (eventshort.EventLocation == "NOI")
        //                        myroom.SpaceType = "NO";
        //                    else
        //                        myroom.SpaceType = eventshort.EventLocation;

        //                    myroom.Subtitle = "";

        //                    eventshort.RoomBooked = new List<RoomBooked>();
        //                    eventshort.RoomBooked.Add(myroom);
        //                }
        //                else
        //                {
        //                    //TODO on rooms
        //                    foreach (var room in eventshort.RoomBooked)
        //                    {
        //                        //Save DAtetime without Offset??
        //                        //room.StartDate = DateTime.SpecifyKind(room.StartDate, DateTimeKind.Utc);
        //                        //room.EndDate = DateTime.SpecifyKind(room.EndDate, DateTimeKind.Utc);

        //                        room.EndDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(room.EndDate);
        //                        room.StartDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(room.StartDate);
        //                    }
        //                }

        //                //Event Title IT EN
        //                if (string.IsNullOrEmpty(eventshort.EventDescriptionIT))
        //                    eventshort.EventDescriptionIT = eventshort.EventDescriptionDE;

        //                if (string.IsNullOrEmpty(eventshort.EventDescriptionEN))
        //                    eventshort.EventDescriptionEN = eventshort.EventDescriptionDE;



        //                //TraceSource tracesource = new TraceSource("CustomData");
        //                //tracesource.TraceEvent(TraceEventType.Information, 0, "Event Start Date:" + String.Format("{0:dd/MM/yyyy hh:mm}", eventshort.StartDate));
        //                eventshort.Id = System.Guid.NewGuid().ToString();

        //                //LicenseInfo
        //                eventshort.LicenseInfo = new LicenseInfo() { Author = User.Identity.Name, ClosedData = false, LicenseHolder = "https://noi.bz.it/", License = "CC0" };

        //                PostgresSQLHelper.InsertDataIntoTable(conn, "eventeuracnoi", JsonConvert.SerializeObject(eventshort), eventshort.Id);
        //                //tracesource.TraceEvent(TraceEventType.Information, 0, "Serialized object:" + JsonConvert.SerializeObject(eventshort));

        //                return Request.CreateResponse(HttpStatusCode.Created, new GenericResultExtended() { Message = "INSERT EventShort succeeded, Id:" + eventshort.Id, Id = eventshort.Id }, "application/json");
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception("No EventShort Data provided");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
        //    }
        //}

        //// PUT: api/EventShort/5
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[KeyCloakAuthorizationFilter(Roles = "VirtualVillageManager")]
        //[HttpPut, Route("api/EventShortVirtualVillage/{id}")]
        //[InvalidateCacheOutput("GetReducedAsync")]
        //public HttpResponseMessage PutVirtualVillage(string id, [FromBody] EventShort eventshort)
        //{
        //    try
        //    {
        //        if (eventshort != null && id != null)
        //        {
        //            if (eventshort.EventLocation == null)
        //                throw new Exception("Eventlocation needed");

        //            eventshort.EventLocation = eventshort.EventLocation.ToUpper();
        //            eventshort.Source = User.Identity.Name;

        //            if (eventshort.EventLocation != "VV")
        //                throw new Exception("VirtualVillageManager can only insert Virtual Village Events");

        //            using (var conn = new NpgsqlConnection(GlobalPGConnection.PGConnectionString))
        //            {

        //                conn.Open();

        //                eventshort.ChangedOn = DateTime.Now;

        //                eventshort.AnchorVenueShort = eventshort.AnchorVenue;

        //                eventshort.EndDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(eventshort.EndDate);
        //                eventshort.StartDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(eventshort.StartDate);

        //                //TODO on rooms
        //                foreach (var room in eventshort.RoomBooked)
        //                {
        //                    room.EndDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(room.EndDate);
        //                    room.StartDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(room.StartDate);
        //                }

        //                //Event Title IT EN
        //                if (string.IsNullOrEmpty(eventshort.EventDescriptionIT))
        //                    eventshort.EventDescriptionIT = eventshort.EventDescriptionDE;

        //                if (string.IsNullOrEmpty(eventshort.EventDescriptionEN))
        //                    eventshort.EventDescriptionEN = eventshort.EventDescriptionDE;


        //                PostgresSQLHelper.UpdateDataFromTable(conn, "eventeuracnoi", JsonConvert.SerializeObject(eventshort), eventshort.Id);

        //                return Request.CreateResponse(HttpStatusCode.OK, new GenericResultExtended() { Message = "UPDATE eventshort succeeded, Id:" + eventshort.Id, Id = eventshort.Id }, "application/json");
        //            }
        //            //}
        //            //else
        //            //{
        //            //    throw new Exception("EventShort cannot be updated");
        //            //}
        //        }
        //        else
        //        {
        //            throw new Exception("No eventshort Data provided");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
        //    }
        //}

        //// DELETE: api/EventShort/5
        //[KeyCloakAuthorizationFilter(Roles = "VirtualVillageManager")]
        //[HttpDelete, Route("api/EventShortVirtualVillage/{id}")]
        //[InvalidateCacheOutput("GetReducedAsync")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public HttpResponseMessage DeleteVirtualVillage(string id)
        //{
        //    try
        //    {
        //        if (id != null)
        //        {
        //            using (var conn = new NpgsqlConnection(GlobalPGConnection.PGConnectionString))
        //            {

        //                conn.Open();

        //                string selectexp = "*";
        //                string whereexp = "id ILIKE '" + id + "'";

        //                var myevent = PostgresSQLHelper.SelectFromTableDataAsObject<EventShort>(conn, "eventeuracnoi", selectexp, whereexp, "", 0, null).FirstOrDefault();

        //                if (myevent.EventLocation == "VV")
        //                {
        //                    PostgresSQLHelper.DeleteDataFromTable(conn, "eventeuracnoi", id);

        //                    return Request.CreateResponse(HttpStatusCode.OK, new GenericResult() { Message = "DELETE EventShort succeeded, Id:" + id }, "application/json");
        //                }
        //                else
        //                {
        //                    throw new Exception("EventShort cannot be deleted");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception("No EventShort Id provided");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
        //    }
        //}

        private bool CheckIfUserIsOnlyInRole(string role, List<string> rolestocheck)
        {
            bool returnbool = false;

            if (User.IsInRole(role))
                returnbool = true;

            foreach(string roletocheck in rolestocheck)
            {
                if(User.IsInRole(roletocheck))
                    returnbool = false;
            }

            return returnbool;
        }

        #endregion

        #region BDPRooms

        private const string bdpserviceurl = @"https://mobility.api.opendatahub.com/v2/flat/NOI-Place?select=smetadata&where=sactive.eq.true&limit=-1";

        private async Task<IDictionary<string, string>> GetBDPNoiRoomsWithLink(string? language)
        {
            try
            {
                var langtoinsert = "";

                //Temporary workaround
                if (!String.IsNullOrEmpty(language) && (language == "en" || language == "de" || language == "it"))
                    langtoinsert = language + "/";

                var bdprooms = await GetBDPNoiRooms();

                if (bdprooms == null)
                    return new Dictionary<string, string>();

                //var noimaproomlist = new List<NoiMapRooms>();
                var noimaproomlist = new Dictionary<string, string>();

                foreach (var room in bdprooms.data.Where(x => !String.IsNullOrEmpty(x.smetadata.todaynoibzit)))
                {
                    var maplink = "";

                    if (room.smetadata.floor >= 0)
                        maplink = room.smetadata.beacon_id.Replace(' ', '-').Replace('.', '-');
                    else
                        maplink = room.smetadata.beacon_id.Replace("-", "--").Replace('.', '-');

                    var nametodisplay = room.smetadata.todaynoibzit;

                    if (nametodisplay.StartsWith("NOI ") || nametodisplay.StartsWith("Noi ") || nametodisplay.StartsWith("noi "))
                        nametodisplay = nametodisplay.Remove(0, 3).Trim();

                    if (!noimaproomlist.ContainsKey(nametodisplay))
                        noimaproomlist.Add(nametodisplay, "https://maps.noi.bz.it/" + langtoinsert + "?shared=" + maplink);
                }

                return noimaproomlist;
            }
            catch (Exception)
            {
                return new Dictionary<string, string>();
            }
        }

        private async Task<BDPResponseObject?> GetBDPNoiRooms()
        {
            var requesturl = bdpserviceurl;

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(20);

                var myresponse = await client.GetAsync(requesturl);

                var myresponsejson = await myresponse.Content.ReadAsStringAsync();

                var btpresponseobject = JsonConvert.DeserializeObject<BDPResponseObject>(myresponsejson);

                return btpresponseobject;
            }
        }

        #endregion
    }

    public class EventShortType
    {
        public List<string> TechnologyFields { get; set; } = new();
        public List<string> CustomTaggings { get; set; } = new();
    }
}
