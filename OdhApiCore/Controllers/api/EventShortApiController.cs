using DataModel;
using Helper;
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
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet, Route("EventShort/Detail/{id}")]
        public async Task<IActionResult> GetDetail(
            string id,
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            CancellationToken cancellationToken = default)
        {
            return await GetEventShortSingle(id, language, fields, cancellationToken);
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
                           filterClosedData: FilterClosedData, getbyrooms: false)
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

        private IEnumerable<EventShortByRoom> TransformEventShortToRoom(IEnumerable<EventShort> eventsshort, DateTime start, DateTime end, string onlyactive)
        {
            List<EventShortByRoom> eventshortlistbyroom = new List<EventShortByRoom>();

            foreach (var eventshort in eventsshort)
            {
                // 1 Event goes for more days
                //Hack, if Event Room is 

                foreach (var room in eventshort.RoomBooked)
                {
                    //Display room only if not comment x inserted
                    if (room.Comment.ToLower() != "x" && room.EndDate >= DateTime.Now)
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

                            if (String.IsNullOrEmpty(myeventshortbyroom.EventDescription["it"]))
                                myeventshortbyroom.EventDescription.TryAddOrUpdate("it", eventshort.EventDescriptionDE);
                            if (String.IsNullOrEmpty(myeventshortbyroom.EventDescription["en"]))
                                myeventshortbyroom.EventDescription.TryAddOrUpdate("en", eventshort.EventDescriptionDE);

                            myeventshortbyroom.EventEndDate = eventshort.EndDate;
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

                            string roomname = room.SpaceDesc;
                            if (roomname.StartsWith("NOI ") || roomname.StartsWith("Noi ") || roomname.StartsWith("noi "))
                                roomname = room.SpaceDesc.Remove(0, 3).Trim();

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
                                        if (!myeventshortbyroom.EventDocument.ContainsKey(eventdocument.Language))
                                            myeventshortbyroom.EventDocument.TryAddOrUpdate(eventdocument.Language, eventdocument.DocumentURL);
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
                string roomname = roomtoadd.SpaceDesc;
                if (roomname.StartsWith("NOI ") || roomname.StartsWith("Noi ") || roomname.StartsWith("noi "))
                    roomname = roomtoadd.SpaceDesc.Remove(0, 3).Trim();

                sameevent.SpaceDescList.Add(roomname);
            }
            else
                eventshortlistbyroom.Add(roomtoadd);

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
            catch (Exception ex)
            {
                return null;
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
