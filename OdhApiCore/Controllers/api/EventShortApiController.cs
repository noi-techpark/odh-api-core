using DataModel;
using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
using OdhApiCore.Responses;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.api
{
    /// <summary>
    /// Articles Api (data provided by IDM) SOME DATA Available as OPENDATA 
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
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            CancellationToken cancellationToken = default
            )
        {
            return await GetEventShortList(
               fields: fields ?? Array.Empty<string>(), language: "", pagenumber: pagenumber, pagesize: pagesize,
               startdate: startdate, enddate: enddate, datetimeformat: datetimeformat, idfilter: eventids,
                   searchfilter: searchfilter, sourcefilter: source, eventlocationfilter: eventlocation,
                   webaddressfilter: webaddress, active: onlyactive.Value,
                   sortorder: sortorder, seed: seed, lastchange: lastchange,
                   rawfilter: rawfilter, rawsort: rawsort, cancellationToken: cancellationToken);
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
        [HttpGet, Route("EventShort/Detail/{id}")]
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

        /// <summary>
        /// GET EventShort List by Room Occupation
        /// </summary>
        /// <param name="startdate">Format (yyyy-MM-dd HH:mm) default or Unix Timestamp</param>
        /// <param name="enddate">Format (yyyy-MM-dd HH:mm) default or Unix Timestamp</param>
        /// <param name="datetimeformat">not provided, use default format, for unix timestamp pass "uxtimestamp"</param>
        /// <param name="source">Source of the data, (possible values 'Content' or 'EBMS')</param>
        /// <param name="eventlocation">Event Location, (possible values, 'NOI' or 'EC')</param>
        /// <param name="onlyactive">'true' if only Events marked as Active by Eurac should be displayed</param>
        /// <param name="eventids">comma separated list of event ids</param>
        /// <returns>List of EventShortByRoom Objects</returns>
        [ProducesResponseType(typeof(IEnumerable<EventShortByRoom>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet, Route("EventShort/GetbyRoomBooked")]
        public async Task<ActionResult<IEnumerable<EventShortByRoom>>> GetbyRoomBooked(
            string? startdate = null, 
            string? enddate = null, 
            string? datetimeformat = null, 
            string? source = null, 
            string? eventlocation = null,
            LegacyBool onlyactive = null!, 
            string? eventids = null, 
            string? webaddress = null)
        {
            return await GetEventShortListbyRoomBooked(startdate, enddate, datetimeformat, source, eventlocation, onlyactive.Value, eventids, webaddress, null, null, null, null);
        }

        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("EventShort/RoomMapping")]
        //[CacheOutput(ClientTimeSpan = 3600, ServerTimeSpan = 3600, AnonymousOnly = true)]
        public async Task<IDictionary<string, string>> GetBDPNoiRoomsWithLinkDictionary()
        {
            return await GetBDPNoiRoomsWithLink();
        }

        #endregion

        #region GETTER

        private Task<IActionResult> GetEventShortList(
            string[] fields, string? language, string? searchfilter, uint pagenumber, uint pagesize, string? startdate, string? enddate, string? datetimeformat,
            string? idfilter, string? sourcefilter, string? eventlocationfilter, string? webaddressfilter, bool? active, string? sortorder, string? seed,
            string? lastchange, string? rawfilter, string? rawsort, CancellationToken cancellationToken)
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
                           filterClosedData: FilterClosedData, getbyrooms: false)
                       .ApplyRawFilter(rawfilter)
                       .ApplyOrdering(ref seed, new PGGeoSearchResult() { geosearch = false }, rawsort, "data #>>'\\{StartDate\\}' " + myeventshorthelper.sortorder);

                //.OrderBySeed(ref seed, );

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

        private async Task<ActionResult<IEnumerable<EventShortByRoom>>> GetEventShortListbyRoomBooked(
            string? startdate, string? enddate, string? datetimeformat, string? sourcefilter, string? eventlocationfilter, 
            bool? active, string? idfilter, string? webaddressfilter, string? lastchange, string? language, string? sortorder, string? searchfilter)
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
                       filterClosedData: FilterClosedData, getbyrooms: false);

            var data =
                    await query
                            .GetAsync<string>();

            var eventshortlist = data.Select(x => JsonConvert.DeserializeObject<EventShort>(x)).ToList();


            return Ok(TransformEventShortToRoom(eventshortlist, myeventshorthelper.start, myeventshorthelper.end, myeventshorthelper.activefilter));
        }

        private IEnumerable<EventShortByRoom> TransformEventShortToRoom(IEnumerable<EventShort> eventsshort, DateTime start, DateTime end, string? onlyactive)
        {
            List<EventShortByRoom> eventshortlistbyroom = new List<EventShortByRoom>();

            foreach (EventShort eventshort in eventsshort)
            {
                // 1 Event goes for more days
                //Hack, if Event Room is 

                foreach (RoomBooked room in eventshort.RoomBooked ?? new List<RoomBooked>())
                {
                    //Display room only if not comment x inserted
                    if (room.Comment?.ToLower() != "x" && room.EndDate >= DateTime.Now)
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

                            myeventshortbyroom!.EventEndDate = eventshort.EndDate;
                            myeventshortbyroom.EventEndDateUTC = eventshort.EndDateUTC;
                            myeventshortbyroom.EventId = eventshort.EventId;
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


                            //var maplink = noiroomslist.Where(x => x.name == room.SpaceDesc.ToLower()).FirstOrDefault();
                            ////var maplink = noiroomslist.Where(x => x.name == eventshort.AnchorVenue.ToLower()).FirstOrDefault();

                            //if (maplink != null)
                            //    myeventshortbyroom.MapsNoiUrl = maplink.maplink;
                            //else
                            //    myeventshortbyroom.MapsNoiUrl = "";

                            string roomname = room.SpaceDesc ?? "";
                            if (roomname.StartsWith("NOI ") || roomname.StartsWith("Noi ") || roomname.StartsWith("noi "))
                                roomname = roomname.Remove(0, 3).Trim();

                            myeventshortbyroom.SpaceDescList.Add(roomname);
                            myeventshortbyroom.CompanyName = eventshort.CompanyName;
                            if (myeventshortbyroom.CompanyName == null)
                                myeventshortbyroom.CompanyName = "";


                            //New Fields to display
                            myeventshortbyroom.ActiveWeb = eventshort.ActiveWeb;
                            myeventshortbyroom.ImageGallery = eventshort.ImageGallery != null ? eventshort.ImageGallery : new List<ImageGallery>();
                            myeventshortbyroom.VideoUrl = eventshort.VideoUrl != null ? eventshort.VideoUrl : "";
                            //Same Logic

                            string textde = eventshort.EventTextDE != null ? eventshort.EventTextDE : "";

                            myeventshortbyroom.EventTextDE = textde;
                            myeventshortbyroom.EventTextIT = eventshort.EventTextIT;
                            myeventshortbyroom.EventTextEN = eventshort.EventTextEN;


                            if (String.IsNullOrEmpty(myeventshortbyroom.EventTextIT))
                                myeventshortbyroom.EventTextIT = textde;
                            if (String.IsNullOrEmpty(myeventshortbyroom.EventTextEN))
                                myeventshortbyroom.EventTextEN = textde;

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


                            if (!String.IsNullOrEmpty(eventshort.Display5))
                                myeventshortbyroom.CompanyName = eventshort.Display5;

                            if (onlyactive == null)
                                onlyactive = "";

                            if (onlyactive.ToLower() == "true")
                            {
                                if (eventshort.Display1 == "Y")
                                    AddToRoomList(eventshortlistbyroom, myeventshortbyroom);
                                //eventshortlistbyroom.Add(myeventshortbyroom);
                            }
                            else
                            {
                                //eventshortlistbyroom.Add(myeventshortbyroom);
                                AddToRoomList(eventshortlistbyroom, myeventshortbyroom);
                            }
                        }
                    }
                }
            }

            return eventshortlistbyroom.OrderBy(x => x.RoomStartDate);
        }

        private void AddToRoomList(List<EventShortByRoom> eventshortlistbyroom, EventShortByRoom roomtoadd)
        {
            //Sieh nach ob bereits ein Event mit demselben namen und 
            var sameevent = eventshortlistbyroom.Where(x => x.Id == roomtoadd.Id
                                                        && x.RoomStartDate == roomtoadd.RoomStartDate
                                                        && x.RoomEndDate == roomtoadd.RoomEndDate).FirstOrDefault();




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

        #endregion

        #region POST PUT DELETE

        // POST: api/EventShort
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,EventShortManager,EventShortCreate,VirtualVillageManager")]
        [HttpPost, Route("EventShort")]
        //[InvalidateCacheOutput("GetReducedAsync")]
        public async Task<IActionResult> Post([FromBody] EventShort eventshort)
        {
            try
            {
                if (eventshort != null)
                {
                    if (eventshort.EventLocation == null)
                        throw new Exception("Eventlocation needed");

                    eventshort.EventLocation = eventshort.EventLocation.ToUpper();

                    if (CheckIfUserIsOnlyInRole("VirtualVillageManager", new List<string>() { "DataWriter", "DataCreate", "EventShortManager", "EventShortCreate" }) && eventshort.EventLocation != "VV")
                        throw new Exception("VirtualVillageManager can only insert Virtual Village Events");

                    if (CheckIfUserIsOnlyInRole("VirtualVillageManager", new List<string>() { "DataWriter", "DataCreate", "EventShortManager", "EventShortCreate" }))
                        eventshort.Source = User.Identity != null ? User.Identity.Name :"";

                    if (eventshort.StartDate == DateTime.MinValue || eventshort.EndDate == DateTime.MinValue)
                        throw new Exception("Start + End Time not set correctly");

                    eventshort.ChangedOn = DateTime.Now;
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
                        eventshort.EventDescriptionIT = eventshort.EventDescriptionDE;

                    if (string.IsNullOrEmpty(eventshort.EventDescriptionEN))
                        eventshort.EventDescriptionEN = eventshort.EventDescriptionDE;

                    //TraceSource tracesource = new TraceSource("CustomData");
                    //tracesource.TraceEvent(TraceEventType.Information, 0, "Event Start Date:" + String.Format("{0:dd/MM/yyyy hh:mm}", eventshort.StartDate));
                    eventshort.Id = System.Guid.NewGuid().ToString();

                    string author = "unknown";
                    if (User.Identity != null && User.Identity.Name != null)
                        author = User.Identity.Name;

                    //LicenseInfo
                    eventshort.LicenseInfo = new LicenseInfo() { Author = author, ClosedData = false, LicenseHolder = "https://noi.bz.it/", License = "CC0" };

                    //PostgresSQLHelper.InsertDataIntoTable(conn, "eventeuracnoi", JsonConvert.SerializeObject(eventshort), eventshort.Id);
                    //tracesource.TraceEvent(TraceEventType.Information, 0, "Serialized object:" + JsonConvert.SerializeObject(eventshort));

                    //check if this works
                    var query = await QueryFactory.Query("eventeuracnoi").InsertAsync(new JsonBData() { id = eventshort.Id, data = new JsonRaw(eventshort) });

                    return Ok(new GenericResultExtended() { Message = "INSERT EventShort succeeded, Id:" + eventshort.Id, Id = eventshort.Id });
                        //new CreatedAtActionResult(nameof(GetById), "Products", new { id = product.Id }, product); ; //Request.CreateResponse(HttpStatusCode.Created, new GenericResultExtended() { Message = "INSERT EventShort succeeded, Id:" + eventshort.Id, Id = eventshort.Id }, "application/json");
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
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,EventShortManager,EventShortModify,VirtualVillageManager")]
        [HttpPut, Route("EventShort/{id}")]
        //[InvalidateCacheOutput("GetReducedAsync")]
        public async Task<IActionResult> Put(string id, [FromBody] EventShort eventshort)
        {
            try
            {
                if (eventshort != null && id != null)
                {
                    if (eventshort.EventLocation == null)
                        throw new Exception("Eventlocation needed");

                    eventshort.EventLocation = eventshort.EventLocation.ToUpper();

                    if (CheckIfUserIsOnlyInRole("VirtualVillageManager", new List<string>() { "DataWriter", "DataCreate", "EventShortManager", "EventShortModify" }) && eventshort.EventLocation != "VV")
                        throw new Exception("VirtualVillageManager can only insert Virtual Village Events");

                    if(CheckIfUserIsOnlyInRole("VirtualVillageManager", new List<string>() { "DataWriter", "DataCreate", "EventShortManager", "EventShortModify" }))
                        eventshort.Source = User.Identity != null ? User.Identity.Name : "";

                    eventshort.ChangedOn = DateTime.Now;

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
                        eventshort.EventDescriptionIT = eventshort.EventDescriptionDE;

                    if (string.IsNullOrEmpty(eventshort.EventDescriptionEN))
                        eventshort.EventDescriptionEN = eventshort.EventDescriptionDE;

                    //TODO CHECK IF THIS WORKS     
                    var updatequery = await QueryFactory.Query("eventeuracnoi").Where("id", id)
                        .UpdateAsync(new JsonBData() { id = eventshort.Id, data = new JsonRaw(eventshort) });

                    return Ok(new GenericResultExtended() { Message = "UPDATE eventshort succeeded, Id:" + eventshort.Id, Id = eventshort.Id });
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
        [Authorize(Roles = "DataWriter,DataCreate,EventShortManager,EventShortDelete,VirtualVillageManager")]
        [HttpDelete, Route("EventShort/{id}")]
        //[InvalidateCacheOutput("GetReducedAsync")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (id != null)
                {
                    var query =
                         QueryFactory.Query("eventeuracnoi")
                             .Select("data")
                             .Where("id", id.ToLower())
                             .When(FilterClosedData, q => q.FilterClosedData());

                    //TO CHECK First select as JsonRaw then convert to eventshort????
                    var myevent = await query.FirstOrDefaultAsync<EventShort?>();

                    if (myevent != null)
                    {
                        if (myevent.Source != "EBMS")
                        {
                            if (CheckIfUserIsOnlyInRole("VirtualVillageManager", new List<string>() { "DataWriter", "DataCreate", "EventShortManager", "EventShortDelete" }) && myevent.EventLocation != "VV")
                                throw new Exception("VirtualVillageManager can only delete Virtual Village Events");

                            //TODO CHECK IF THIS WORKS     
                            var deletequery = await QueryFactory.Query("eventeuracnoi").Where("id", id).DeleteAsync();

                            return Ok(new GenericResult() { Message = "DELETE EventShort succeeded, Id:" + id });
                        }
                        else
                        {
                            if (User.IsInRole("VirtualVillageManager") && myevent.EventLocation == "VV")
                            {
                                //TODO CHECK IF THIS WORKS     
                                var deletequery = await QueryFactory.Query("eventeuracnoi").Where("id", id).DeleteAsync();

                                return Ok(new GenericResult() { Message = "DELETE EventShort succeeded, Id:" + id });
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

        private const string bdpserviceurl = @"https://mobility.api.opendatahub.bz.it/v2/flat/NOI-Place?select=smetadata&where=sactive.eq.true&limit=-1";

        private async Task<IDictionary<string, string>> GetBDPNoiRoomsWithLink()
        {
            try
            {
                var bdprooms = await GetBDPNoiRooms();

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
                        noimaproomlist.Add(nametodisplay, "https://maps.noi.bz.it/?shared=" + maplink);
                }

                return noimaproomlist;
            }
            catch (Exception)
            {
                return new Dictionary<string, string>();
            }
        }

        private async Task<BDPResponseObject> GetBDPNoiRooms()
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
}
