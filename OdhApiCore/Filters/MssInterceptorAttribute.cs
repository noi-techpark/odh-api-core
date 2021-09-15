using DataModel;
using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MSS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OdhApiCore.Controllers;
using OdhApiCore.Controllers.helper;
using OdhApiCore.Responses;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Filters
{
    public class MssInterceptorAttribute : ActionFilterAttribute
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ISettings settings;
        protected QueryFactory QueryFactory { get; }

        public bool CheckAvailabilitySearch(System.Security.Claims.ClaimsPrincipal User)
        {
            List<string> roles = new List<string>() { "DataReader", "AccoReader" };

            foreach (var role in roles)
            {
                if (User.IsInRole(role))
                    return true;
            }

            return false;
        }

        public MssInterceptorAttribute(QueryFactory queryFactory, IHttpClientFactory httpClientFactory, ISettings settings)
        {
            this.httpClientFactory = httpClientFactory;
            this.settings = settings;
            this.QueryFactory = queryFactory;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var availabilitysearchavailable = CheckAvailabilitySearch(context.HttpContext.User);            

            //Getting Action name
            context.ActionDescriptor.RouteValues.TryGetValue("action", out string? actionid);

            //Only if Action ID is GetAccommodations perform the Availability Check before
            if (actionid == "GetAccommodations" && availabilitysearchavailable)
            {
                //Getting the Querystrings
                var actionarguments = context.ActionArguments;

                bool? availabilitycheck = ((LegacyBool)actionarguments["availabilitycheck"]).Value;
                
                if(availabilitycheck != null && availabilitycheck == true)
                {
                    string? categoryfilter = actionarguments.ContainsKey("categoryfilter") ? (string?)actionarguments["categoryfilter"] : null;
                    string? typefilter = actionarguments.ContainsKey("typefilter") ? (string?)actionarguments["typefilter"] : null;
                    string? featurefilter = actionarguments.ContainsKey("featurefilter") ? (string?)actionarguments["featurefilter"] : null;
                    string? featureidfilter = actionarguments.ContainsKey("featureidfilter") ? (string?)actionarguments["featureidfilter"] : null;
                    string? themefilter = actionarguments.ContainsKey("themefilter") ? (string?)actionarguments["themefilter"] : null;
                    string? badgefilter = actionarguments.ContainsKey("badgefilter") ? (string?)actionarguments["badgefilter"] : null;
                    string? idfilter = actionarguments.ContainsKey("idfilter") ? (string?)actionarguments["idfilter"] : null;
                    string? locfilter = actionarguments.ContainsKey("locfilter") ? (string?)actionarguments["locfilter"] : null;
                    string? altitudefilter = actionarguments.ContainsKey("altitudefilter") ? (string?)actionarguments["altitudefilter"] : null;
                    string? odhtagfilter = actionarguments.ContainsKey("odhtagfilter") ? (string?)actionarguments["odhtagfilter"] : null;
                    bool? active = actionarguments.ContainsKey("active") ? ((LegacyBool)actionarguments["active"]).Value : null;
                    bool? odhactive = actionarguments.ContainsKey("odhactive") ? ((LegacyBool)actionarguments["odhactive"]).Value : null;
                    bool? bookablefilter = actionarguments.ContainsKey("bookablefilter") ? ((LegacyBool)actionarguments["bookablefilter"]).Value : null;
                    string? updatefrom = actionarguments.ContainsKey("updatefrom") ? (string?)actionarguments["updatefrom"] : null;
                    string? seed = actionarguments.ContainsKey("seed") ? (string?)actionarguments["seed"] : null;
                    string? searchfilter = actionarguments.ContainsKey("searchfilter") ? (string?)actionarguments["searchfilter"] : null;
                    string? latitude = actionarguments.ContainsKey("latitude") ? (string?)actionarguments["latitude"] : null;
                    string? longitude = actionarguments.ContainsKey("longitude") ? (string?)actionarguments["longitude"] : null;
                    string? radius = actionarguments.ContainsKey("radius") ? (string?)actionarguments["radius"] : null;

                    string language = actionarguments.ContainsKey("language") ? (string)actionarguments["language"] : "de";
                    string? langfilter = actionarguments.ContainsKey("langfilter") ? (string)actionarguments["langfilter"] : null;

                    string boardfilter = actionarguments.ContainsKey("boardfilter") ? (string)actionarguments["boardfilter"] : "0";
                    string arrival = actionarguments.ContainsKey("arrival") ? (string)actionarguments["arrival"] : String.Format("{0:yyyy-MM-dd}", DateTime.Now);
                    string departure = actionarguments.ContainsKey("departure") ? (string)actionarguments["departure"] : String.Format("{0:yyyy-MM-dd}", DateTime.Now.AddDays(1));
                    string roominfo = actionarguments.ContainsKey("roominfo") ? (string)actionarguments["roominfo"] : "1-18,18";
                    string source = actionarguments.ContainsKey("source") ? (string)actionarguments["source"] : "sinfo";
                    string detail = actionarguments.ContainsKey("detail") ? (string)actionarguments["detail"] : "0";
                    string bokfilter = actionarguments.ContainsKey("bokfilter") ? (string)actionarguments["bokfilter"] : "hgv";
                    string idsource = actionarguments.ContainsKey("idsource") ? (string)actionarguments["idsource"] : "lts";

                    if(CheckArrivalAndDeparture(arrival, departure))
                    {
                        AccommodationHelper myhelper = await AccommodationHelper.CreateAsync(
                       QueryFactory, idfilter: idfilter, locfilter: locfilter, boardfilter: boardfilter, categoryfilter: categoryfilter, typefilter: typefilter,
                       featurefilter: featurefilter, featureidfilter: featureidfilter, badgefilter: badgefilter, themefilter: themefilter, altitudefilter: altitudefilter, smgtags: odhtagfilter, activefilter: active,
                       smgactivefilter: odhactive, bookablefilter: bookablefilter, lastchange: updatefrom, langfilter: langfilter, (CancellationToken)context.ActionArguments["cancellationToken"]);

                        var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

                        //Get Accommodations IDlist 
                        var idlist = await GetAccommodationBookList(myhelper, language, seed, searchfilter, geosearchresult);

                        ////Fix
                        //List<AccoBookList> myaccoboklist = new List<AccoBookList>();
                        //foreach(var item in idlist)
                        //{
                        //    var x = new AccoBookList();
                        //    x.Id = item.Id;
                        //    x.IsBookable = item.IsBookable;
                        //    x.AccoBookingChannel = JsonConvert.DeserializeObject<ICollection<AccoBookingChannel>?>(item.AccoBookingChannel.Value);                        
                        //}

                        //crashes because some AccobookingChannels are null TODO!
                        //var myaccobooklist = idlist.Select(x => new AccoBookList { Id = x.Id, IsBookable = x.IsBookable, AccoBookingChannel = JsonConvert.DeserializeObject<ICollection<AccoBookingChannel>?>(x.AccoBookingChannel.Value) }).ToList();

                        //TODO NOT WORKING, Availability Request is returned without paging


                        var booklist = idlist.Where(x => x.Id != null).Select(x => x.Id!.ToUpper()).ToList() ?? new List<string>();
                        var bokfilterlist = bokfilter.Split(',').ToList();

                        context.HttpContext.Items.Add("accobooklist", booklist);

                        if (bokfilterlist.Contains("hgv"))
                        {
                            MssResult mssresult = await GetMSSAvailability(
                                      language: language, arrival: arrival, departure: departure, boardfilter: boardfilter,
                                      roominfo: roominfo, bokfilter: bokfilter, detail: Convert.ToInt32(detail), bookableaccoIDs: booklist, idsofchannel: idsource, source: source);

                            if (mssresult != null)
                            {
                                context.HttpContext.Items.Add("mssavailablity", mssresult);
                            }
                        }
                        if (bokfilterlist.Contains("lts"))
                        {
                            MssResult lcsresult = await GetLCSAvailability(
                                            language: language, arrival: arrival, departure: departure, boardfilter: boardfilter,
                                            roominfo: roominfo, bookableaccoIDs: booklist, source: source);

                            if (lcsresult != null)
                            {
                                context.HttpContext.Items.Add("lcsavailablity", lcsresult);
                            }
                        }
                    }                                                          
                }

                await base.OnActionExecutionAsync(context, next);
            }
            else
            {
                await base.OnActionExecutionAsync(context, next);
            }
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            bool availabilitysearchavailable = CheckAvailabilitySearch(context.HttpContext.User);

            //Getting Action name
            context.ActionDescriptor.RouteValues.TryGetValue("action", out string? actionid);

            var query = context.HttpContext.Request.Query;

            string idsource = (string?)query["idsource"] ?? "lts";
            var availabilitychecklegacy = (string?)query["availabilitycheck"];
            bool.TryParse(availabilitychecklegacy, out bool availabilitycheck);

            string bokfilter = (string?)query["bokfilter"] ?? "hgv";
            var bokfilterlist = bokfilter.Split(',').ToList();

            if (availabilitycheck && availabilitysearchavailable)
            {
                string language = (string?)query["language"] ?? "de";
                //string availabilitychecklanguage = (string?)query["availabilitychecklanguage"] ?? "en";
                string boardfilter = (string?)query["boardfilter"] ?? "0";
                string arrival = (string?)query["arrival"] ?? String.Format("{0:yyyy-MM-dd}", DateTime.Now);
                string departure = (string?)query["departure"] ?? String.Format("{0:yyyy-MM-dd}", DateTime.Now.AddDays(1));
                string roominfo = (string?)query["roominfo"] ?? "1-18,18";
                string source = (string?)query["source"] ?? "sinfo";
                string detail = (string?)query["detail"] ?? "0";

                if (CheckArrivalAndDeparture(arrival, departure))
                {
                    var bookableAccoIds = new List<string>();
                    if (context.RouteData.Values.TryGetValue("id", out object? id) && id != null)
                        bookableAccoIds.Add((string)id);

                    if (context.Result is OkObjectResult okObject && okObject.Value is JsonRaw jRaw)
                    {
                        string json = jRaw.Value;
                        var jtoken = JToken.Parse(json);
                        if (jtoken is JObject jObject)
                        {
                            var mssResponseShort = jObject.Property("MssResponseShort");
                            if (mssResponseShort is JProperty mssResponseShortProperty)
                            {

                                List<MssResult> result = new List<MssResult>();

                                if (bokfilterlist.Contains("hgv"))
                                {
                                    MssResult? mssresult = default(MssResult);

                                    if (actionid == "GetAccommodations")
                                    {
                                        mssresult = (MssResult?)context.HttpContext.Items["mssavailablity"];
                                    }
                                    else if (actionid == "GetAccommodation")
                                    {
                                        mssresult = await GetMSSAvailability(
                                        language: language, arrival: arrival, departure: departure, boardfilter: boardfilter,
                                        roominfo: roominfo, bokfilter: bokfilter, detail: Convert.ToInt32(detail), bookableaccoIDs: bookableAccoIds, idsofchannel: idsource, source: source);
                                    }

                                    if (mssresult != null)
                                    {
                                        result.Add(mssresult);
                                    }
                                }
                                if (bokfilterlist.Contains("lts"))
                                {
                                    MssResult lcsresult = await GetLCSAvailability(
                                        language: language, arrival: arrival, departure: departure, boardfilter: boardfilter,
                                        roominfo: roominfo, bookableaccoIDs: bookableAccoIds, source: source);

                                    if (lcsresult != null)
                                    {
                                        result.Add(lcsresult);
                                    }
                                }

                                if (result.Count > 0)
                                {
                                    var resultJson = JsonConvert.SerializeObject(result.SelectMany(x => x.MssResponseShort));
                                    mssResponseShortProperty.Value = new JRaw(resultJson);
                                }
                            }
                        }
                        okObject.Value = jtoken;
                    }
                    else if (context.Result is OkObjectResult okObjectlist && okObjectlist.Value is JsonResult<JsonRaw> jRawList)
                    {
                        List<JToken> myRawList = new List<JToken>();

                        foreach (var myjRaw in jRawList.Items)
                        {
                            string json = myjRaw.Value;
                            var jtoken = JToken.Parse(json);
                            if (jtoken is JObject jObject)
                            {
                                var mssResponseShort = jObject.Property("MssResponseShort");
                                if (mssResponseShort is JProperty mssResponseShortProperty)
                                {

                                    List<MssResult> result = new List<MssResult>();

                                    if (bokfilterlist.Contains("hgv"))
                                    {
                                        MssResult? mssresult = null;

                                        if (actionid == "GetAccommodations")
                                        {
                                            mssresult = (MssResult?)context.HttpContext.Items["mssavailablity"];
                                        }
                                        else if (actionid == "GetAccommodation")
                                        {
                                            mssresult = await GetMSSAvailability(
                                            language: language, arrival: arrival, departure: departure, boardfilter: boardfilter,
                                            roominfo: roominfo, bokfilter: bokfilter, detail: Convert.ToInt32(detail), bookableaccoIDs: bookableAccoIds, idsofchannel: idsource, source: source);
                                        }

                                        if (mssresult != null)
                                        {
                                            result.Add(mssresult);
                                        }
                                    }
                                    if (bokfilterlist.Contains("lts"))
                                    {
                                        MssResult? lcsresult = null;

                                        if (actionid == "GetAccommodations")
                                        {
                                            lcsresult = (MssResult?)context.HttpContext.Items["lcsavailablity"];
                                        }
                                        else if (actionid == "GetAccommodation")
                                        {
                                            lcsresult = await GetLCSAvailability(
                                             language: language, arrival: arrival, departure: departure, boardfilter: boardfilter,
                                             roominfo: roominfo, bookableaccoIDs: bookableAccoIds, source: source);
                                        }

                                        if (lcsresult != null)
                                        {
                                            result.Add(lcsresult);
                                        }
                                    }

                                    if (result.Count > 0)
                                    {
                                        string? accid = jObject.Property("Id")?.Value.ToString();
                                        var data = result.SelectMany(x => x.MssResponseShort).Where(x => x.A0RID == accid);
                                        mssResponseShortProperty.Value = new JRaw(JsonConvert.SerializeObject(data));
                                    }
                                }
                            }
                            myRawList.Add(jtoken);
                        }

                        jRawList.Items = myRawList.Select(jtoken => new JsonRaw(jtoken.ToString()));
                    }
                }
            }

            //TODO REturn paginated result on list https://localhost:5001/api/Accommodation?pagenumber=1&pagesize=10&categoryfilter=30720&typefilter=1&arrival=01-02-2021&departure=05-02-2021&roominfo=1-18%2C18&bokfilter=hgv&source=sinfo&availabilitychecklanguage=en&availabilitycheck=true

            await base.OnResultExecutionAsync(context, next);
        }

        private async Task<MssResult> GetMSSAvailability(string language, string arrival, string departure, string boardfilter, string roominfo, string bokfilter, int? detail, List<string> bookableaccoIDs, string idsofchannel, string source, bool withoutmssids = false, string mssversion = "2")
        {            
            MssHelper myhelper = MssHelper.Create(bookableaccoIDs, idsofchannel, bokfilter, language, roominfo, boardfilter, arrival, departure, detail, source, mssversion);
                       
            //Achtung muassi no schaugn!
            if (bookableaccoIDs.Count > 0)
            {
                //0 MSS Method Olle channels affamol mit IDList
                var myparsedresponse = await GetMssData.GetMssResponse(
                    httpClientFactory.CreateClient("mss"),
                    lang: myhelper.mssrequestlanguage, idlist: myhelper.accoidlist, idsofchannel: myhelper.idsofchannel, mybookingchannels: myhelper.mybokchannels,
                    myroomdata: myhelper.myroomdata, arrival: myhelper.arrival, departure: myhelper.departure, service: myhelper.service,
                    hgvservicecode: myhelper.hgvservicecode, offerdetails: myhelper.xoffertype, hoteldetails: myhelper.xhoteldetails,
                    rooms: myhelper.rooms, source: myhelper.source, version: myhelper.mssversion, mssuser: settings.MssConfig.Username, msspswd: settings.MssConfig.Password, withoutmssids: withoutmssids
                    );
               
                if (myparsedresponse != null)
                    return myparsedresponse;
            }
            return new MssResult() { bookableHotels = 0, CheapestChannel = "", Cheapestprice = 0, ResultId = "", MssResponseShort = new List<MssResponseShort>() };
        }

        private async Task<MssResult> GetLCSAvailability(string language, string arrival, string departure, string boardfilter, string roominfo, List<string> bookableaccoIDs, string source)
        {
            LcsHelper myhelper = LcsHelper.Create(bookableaccoIDs, language, roominfo, boardfilter, arrival, departure, source);

            if (bookableaccoIDs.Count > 0)
            {
                var accosearchrequest = LCS.GetAccommodationDataLCS.GetAccommodationDataSearchRequest(resultRID: "", pageNr: "1", pageSize: "10000", language: myhelper.lcsrequestlanguage, 
                    sortingcriterion: "1", sortingorder: "", sortingpromotebookable: "", request: "0", filters: "0", timespanstart: myhelper.arrival, timespanend: myhelper.departure, 
                    checkavailabilitystatus: "1", onlybookableresults: "0", mealplans: myhelper.service, accommodationrids: myhelper.accoidlist, tourismorg: new List<string>(), 
                    districts: new List<string>(), marketinggroups: new List<string>(), lcsroomstay: myhelper.myroomdata, requestor: source, messagepswd: settings.LcsConfig.MessagePassword);

                var myaccosearchlcs = new LCS.GetAccommodationDataLCS(settings.LcsConfig.Username, settings.LcsConfig.Password);
                var response = await myaccosearchlcs.GetAccommodationDataSearchAsync(accosearchrequest);
                var myparsedresponse = LCS.ParseAccoSearchResult.ParsemyLCSResponse(language, response, myhelper.rooms);

                if (myparsedresponse != null)
                    return myparsedresponse;
            }
            return new MssResult() { bookableHotels = 0, CheapestChannel = "", Cheapestprice = 0, ResultId = "", MssResponseShort = new List<MssResponseShort>() };
        }

        private bool CheckArrivalAndDeparture(string arrival, string departure)
        {
            DateTime now = DateTime.Now;
            DateTime arrivaldt = DateTime.Parse(arrival);
            DateTime departuredt = DateTime.Parse(departure);

            if (arrivaldt.Date == departuredt.Date)
                return false;

            if (arrivaldt <= now.Date.AddDays(-1) || departuredt <= now.Date.AddDays(-1))
                return false;
            else
                return true;
        }

        private async Task<IEnumerable<AccoBookListRaw>> GetAccommodationBookList(AccommodationHelper myhelper, string language, string? seed, string? searchfilter, PGGeoSearchResult geosearchresult)
        {
            string select = $"data#>>'\\{{Id\\}}' as Id, data#>>'\\{{IsBookable\\}}' as IsBookable, data#>>'\\{{AccoBookingChannel\\}}' as AccoBookingChannel";
            //"data->'Id' as Id, data->'IsBookable' as IsBookable, data -> 'AccoBookingChannel' as AccoBookingChannel"

            var query =
                   QueryFactory.Query()
                       .SelectRaw(select)
                       .From("accommodations")
                       .AccommodationWhereExpression(
                           idlist: myhelper.idlist, accotypelist: myhelper.accotypelist,
                           categorylist: myhelper.categorylist, featurelist: myhelper.featurelist, featureidlist: myhelper.featureidlist,
                           badgelist: myhelper.badgelist, themelist: myhelper.themelist,
                           boardlist: myhelper.boardlist, smgtaglist: myhelper.smgtaglist,
                           districtlist: myhelper.districtlist, municipalitylist: myhelper.municipalitylist,
                           tourismvereinlist: myhelper.tourismvereinlist, regionlist: myhelper.regionlist,
                           apartmentfilter: myhelper.apartment, bookable: myhelper.bookable, altitude: myhelper.altitude,
                           altitudemin: myhelper.altitudemin, altitudemax: myhelper.altitudemax,
                           activefilter: myhelper.active, smgactivefilter: myhelper.smgactive,
                           searchfilter: searchfilter, language: language, lastchange: myhelper.lastchange, languagelist: new List<string>(),
                           filterClosedData: false)//FilterClosedData -->TODO)
                       .OrderBySeed(ref seed, "data #>>'\\{Shortname\\}' ASC")
                       .GeoSearchFilterAndOrderby(geosearchresult);

            return await query.GetAsync<AccoBookListRaw>();
        }

    }
}
