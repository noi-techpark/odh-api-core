using AspNetCore.CacheOutput;
using DataModel;
using Helper;
using Microsoft.AspNetCore.Authorization;
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
    /// Events Api (data provided by LTS) SOME DATA Available as OPENDATA 
    /// </summary>
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class EventController : OdhController
    {      
        public EventController(IWebHostEnvironment env, ISettings settings, ILogger<EventController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {
        }

        #region SWAGGER Exposed API

        /// <summary>
        /// GET Event List
        /// </summary>
        /// <param name="pagenumber">Pagenumber, (default:1)</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of Event IDs, 'null' = No Filter), (default:'null')</param>
        /// <param name="locfilter">Locfilter SPECIAL Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction), 'null' = (No Filter), (default:'null') <a href="https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#location-filter-locfilter" target="_blank">Wiki locfilter</a></param>        
        /// <param name="rancfilter">Rancfilter (Ranc 0-5 possible)</param>
        /// <param name="typefilter">Typefilter (Type of Event: not used yet)</param>
        /// <param name="topicfilter">Topic ID Filter (Filter by Topic ID) BITMASK</param>
        /// <param name="orgfilter">Organization Filter (Filter by Organizer RID)</param>
        /// <param name="odhtagfilter">ODH Taglist Filter (refers to Array SmgTags) (String, Separator ',' more Tags possible, available Tags reference to 'v1/ODHTag?validforentity=event'), (default:'null')</param>        
        /// <param name="begindate">BeginDate of Events (Format: yyyy-MM-dd)</param>
        /// <param name="sort">Sorting of Events by Next Begindate ('desc': Descending, 'asc': Ascending)</param>
        /// <param name="enddate">EndDate of Events (Format: yyyy-MM-dd)</param>
        /// <param name="active">Active Events Filter (possible Values: 'true' only Active Events, 'false' only Disabled Events, (default:'null')</param>
        /// <param name="odhactive">ODH Active (Published) Events Filter (Refers to field SmgActive) Events Filter (possible Values: 'true' only published Events, 'false' only not published Events, (default:'null')</param>                
        /// <param name="latitude">GeoFilter FLOAT Latitude Format: '46.624975', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter' target="_blank">Wiki geosort</a></param>
        /// <param name="longitude">GeoFilter FLOAT Longitude Format: '11.369909', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter' target="_blank">Wiki geosort</a></param>
        /// <param name="radius">Radius INTEGER to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter' target="_blank">Wiki geosort</a></param>
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null)<a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of Event Objects</returns>         /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(JsonResult<Event>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 3600, CacheKeyGenerator = typeof(CustomCacheKeyGenerator))]
        //[Authorize]
        [HttpGet, Route("Event")]
        public async Task<IActionResult> GetEventList(
            string? language = null,
            uint pagenumber = 1,
            PageSize pagesize = null!,
            string? idlist = null,
            string? locfilter = null,
            string? rancfilter = null,
            string? typefilter = null,
            string? topicfilter = null,
            string? orgfilter = null,
            string? odhtagfilter = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? begindate = null,
            string? enddate = null,
            string? sort = null,
            string? updatefrom = null,
            string? seed = null,
            string? langfilter = null,
            string? source = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            return await GetFiltered(
                    fields: fields ?? Array.Empty<string>(), language: language, pagenumber: pagenumber,
                    pagesize: pagesize, typefilter: typefilter, idfilter: idlist, rancfilter: rancfilter,
                    searchfilter: searchfilter, locfilter: locfilter, topicfilter: topicfilter, orgfilter: orgfilter,
                    begindate: begindate, enddate: enddate, sort: sort, active: active,
                    smgactive: odhactive, smgtags: odhtagfilter, seed: seed, lastchange: updatefrom,
                    langfilter: langfilter, source: source,
                    geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues,
                    cancellationToken: cancellationToken);
        }

        /// <summary>
        /// GET Event Single 
        /// </summary>
        /// <param name="id">ID of the Event</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Event Object</returns>
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>        
        [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Event/{id}", Name = "SingleEvent")]
        public async Task<IActionResult> GetEventSingle(
            string id,
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null, 
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetSingle(id, language, fields: fields ?? Array.Empty<string>(), removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Event Topic List
        /// </summary>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null)<a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of EventTypes Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        //[CacheOutputUntilToday(23, 59)]
        [ProducesResponseType(typeof(IEnumerable<EventTypes>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        
        [HttpGet, Route("EventTopics")]
        public async Task<IActionResult> GetAllEventTopicListAsync(
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetEventTopicListAsync(language, fields: fields ?? Array.Empty<string>(), searchfilter, rawfilter, rawsort, removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Event Topic Single
        /// </summary>
        /// <param name="id">ID of the Event</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>EventTypes Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        //[CacheOutputUntilToday(23, 59)]
        [ProducesResponseType(typeof(EventTypes), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        
        [HttpGet, Route("EventTopics/{id}", Name = "SingleEventTopics")]
        public async Task<IActionResult> GetAllEventTopicSingleAsync(
            string id,
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetEventTopicSingleAsync(id, language, fields: fields ?? Array.Empty<string>(), removenullvalues: removenullvalues, cancellationToken);
        }

        #endregion

        #region GETTER

        private Task<IActionResult> GetFiltered(
        string[] fields, string? language, uint pagenumber, int? pagesize, string? typefilter, string? idfilter,
        string? rancfilter, string? searchfilter, string? locfilter, string? orgfilter, string? topicfilter, string? begindate, string? enddate,
        string? sort, bool? active, bool? smgactive, string? smgtags, string? seed, string? lastchange, string? langfilter, string? source, 
        PGGeoSearchResult geosearchresult, string? rawfilter, string? rawsort, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                EventHelper myeventhelper = await EventHelper.CreateAsync(
                    QueryFactory, idfilter, locfilter, rancfilter, typefilter, topicfilter, orgfilter, begindate, enddate,
                    active, smgactive, smgtags, lastchange, langfilter, source,
                    cancellationToken);

                string sortifseednull = null;

                if (sort != null)
                {                  
                    if (sort.ToLower() == "asc")
                        sortifseednull = "gen_nextbegindate ASC";
                    else
                        sortifseednull = "gen_nextbegindate DESC";

                    //Set seed to null
                    seed = null;
                    
                    //Set Rawfilter to this RTHOENI rawfilter can overwrite this ;)
                    //rawfilter = sortifseednull;
                }

                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From("events")
                        .EventWhereExpression(
                            idlist: myeventhelper.idlist, typelist: myeventhelper.typeidlist,
                            ranclist: myeventhelper.rancidlist, orglist: myeventhelper.orgidlist,
                            smgtaglist: myeventhelper.smgtaglist, districtlist: myeventhelper.districtlist,
                            municipalitylist: myeventhelper.municipalitylist, tourismvereinlist: myeventhelper.tourismvereinlist,
                            regionlist: myeventhelper.regionlist, topiclist: myeventhelper.topicrids, sourcelist: myeventhelper.sourcelist,
                            languagelist: myeventhelper.languagelist, begindate: myeventhelper.begin, enddate: myeventhelper.end,
                            activefilter: myeventhelper.active, smgactivefilter: myeventhelper.smgactive,
                            searchfilter: searchfilter, language: language, lastchange: myeventhelper.lastchange,
                            filterClosedData: FilterClosedData)
                         .ApplyRawFilter(rawfilter)
                         .OrderByRawIfNotNull(sortifseednull)
                         .ApplyOrdering_GeneratedColumns(ref seed, geosearchresult, rawsort, sortifseednull);
                         //.ApplyOrdering(ref seed, geosearchresult, rawsort, sortifseednull);

                //.OrderBySeed(ref seed, sortifseednull)
                //.GeoSearchFilterAndOrderby(geosearchresult);
                //TODO Use sorting

                // Get paginated data
                var data =
                    await query
                        .PaginateAsync<JsonRaw>(
                            page: (int)pagenumber,
                            perPage: (int)pagesize);

                var dataTransformed =
                    data.List.Select(
                        raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, userroles: UserRolesList)
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

        /// <summary>
        /// GET Single Event
        /// </summary>
        /// <param name="id">ID of the Event</param>
        /// <returns>Event Object</returns>
        private Task<IActionResult> GetSingle(string id, string? language, string[] fields, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("events")
                        .Select("data")
                        .Where("id", id)
                        .When(FilterClosedData, q => q.FilterClosedData());

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, userroles: UserRolesList);
            });
        }

        #endregion

        #region CATEGORIES

        /// <summary>
        /// GET Event Topics List
        /// </summary>
        /// <returns>Collection of EventTypes Object</returns>
        private Task<IActionResult> GetEventTopicListAsync(string? language, string[] fields, string? searchfilter, string? rawfilter, string? rawsort, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("eventtypes")
                        .SelectRaw("data")
                        .When(FilterClosedData, q => q.FilterClosedData())
                        .SearchFilter(PostgresSQLWhereBuilder.TypeDescFieldsToSearchFor(language), searchfilter)
                        .ApplyRawFilter(rawfilter)
                        .OrderOnlyByRawSortIfNotNull(rawsort);

                var data = await query.GetAsync<JsonRaw?>();

                var dataTransformed =
                    data.Select(
                        raw => raw?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, userroles: UserRolesList)
                    );

                return dataTransformed;
            });
        }

        /// <summary>
        /// GET Event Topic Single
        /// </summary>
        /// <returns>EventTypes Object</returns>
        private Task<IActionResult> GetEventTopicSingleAsync(string id, string? language, string[] fields, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("eventtypes")
                        .Select("data")
                        .Where("id", id.ToUpper())
                        .When(FilterClosedData, q => q.FilterClosedData());

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, userroles: UserRolesList);
            });
        }

        #endregion

        #region POST PUT DELETE

        /// <summary>
        /// POST Insert new Event
        /// </summary>
        /// <param name="odhevent">Event Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,EventManager,EventCreate")]
        [HttpPost, Route("Event")]
        public Task<IActionResult> Post([FromBody] EventLinked odhevent)
        {
            return DoAsyncReturn(async () =>
            {
                odhevent.Id = !String.IsNullOrEmpty(odhevent.Id) ? odhevent.Id.ToUpper() : "noId";
                return await UpsertData<EventLinked>(odhevent, "events");
            });
        }

        /// <summary>
        /// PUT Modify existing Event
        /// </summary>
        /// <param name="id">Event Id</param>
        /// <param name="odhevent">Event Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataModify,EventManager,EventModify")]
        [HttpPut, Route("Event/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] EventLinked odhevent)
        {
            return DoAsyncReturn(async () =>
            {
                odhevent.Id = id.ToUpper();
                return await UpsertData<EventLinked>(odhevent, "events");
            });
        }

        /// <summary>
        /// DELETE Event by Id
        /// </summary>
        /// <param name="id">Event Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataDelete,EventManager,EventDelete")]
        [HttpDelete, Route("Event/{id}")]
        public Task<IActionResult> Delete(string id)
        {
            return DoAsyncReturn(async () =>
            {
                id = id.ToUpper();
                return await DeleteData(id, "events");
            });
        }


        #endregion
    }
}