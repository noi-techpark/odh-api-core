using Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OdhApiCore.Controllers;
using OdhApiCore.Controllers.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OdhApiCore.Filters
{
    public class MssInterceptorAttribute : ActionFilterAttribute
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ISettings settings;

        public MssInterceptorAttribute(IHttpClientFactory httpClientFactory, ISettings settings)
        {
            this.httpClientFactory = httpClientFactory;
            this.settings = settings;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //Getting Action name
            context.ActionDescriptor.RouteValues.TryGetValue("action", out string? actionid);

            if (actionid == "GetAccommodations")
            {

            }
            else
            {
                await base.OnActionExecutionAsync(context, next);
            }
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var query = context.HttpContext.Request.Query;

            string idsource = (string?)query["idsource"] ?? "lts";
            var availabilitychecklegacy = (string?)query["availabilitycheck"];
            bool.TryParse(availabilitychecklegacy, out bool availabilitycheck);

            string bokfilter = (string?)query["bokfilter"] ?? "hgv";
            var bokfilterlist = bokfilter.Split(',').ToList(); ;

            if (availabilitycheck)
            {
                string language = (string?)query["language"] ?? "de";
                //string availabilitychecklanguage = (string?)query["availabilitychecklanguage"] ?? "en";
                string boardfilter = (string?)query["boardfilter"] ?? "0";
                string arrival = (string?)query["arrival"] ?? String.Format("{0:yyyy-MM-dd}", DateTime.Now);
                string departure = (string?)query["departure"] ?? String.Format("{0:yyyy-MM-dd}", DateTime.Now.AddDays(1));
                string roominfo = (string?)query["roominfo"] ?? "1-18,18";
                string source = (string?)query["source"] ?? "sinfo";
                string detail = (string?)query["detail"] ?? "0";                

                context.RouteData.Values.TryGetValue("id", out object id);
                var bookableAccoIds = new List<string>() { (string)id };

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
                                MssResult mssresult = await GetMSSAvailability(
                                    language: language, arrival: arrival, departure: departure, boardfilter: boardfilter,
                                    roominfo: roominfo, bokfilter: bokfilter, detail: Convert.ToInt32(detail), bookableaccoIDs: bookableAccoIds, idsofchannel: idsource, source: source);

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

                            if(result.Count > 0)
                            {
                                var resultJson = JsonConvert.SerializeObject(result.SelectMany(x => x.MssResponseShort));
                                mssResponseShortProperty.Value = new JRaw(resultJson);
                            }
                        }
                    }
                    okObject.Value = jtoken;
                }
            }
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
                var accosearchrequest = Helper.LCS.GetAccommodationDataLCS.GetAccommodationDataSearchRequest(resultRID: "", pageNr: "1", pageSize: "10000", language: myhelper.lcsrequestlanguage, 
                    sortingcriterion: "1", sortingorder: "", sortingpromotebookable: "", request: "0", filters: "0", timespanstart: myhelper.arrival, timespanend: myhelper.departure, 
                    checkavailabilitystatus: "1", onlybookableresults: "0", mealplans: myhelper.service, accommodationrids: myhelper.accoidlist, tourismorg: new List<string>(), 
                    districts: new List<string>(), marketinggroups: new List<string>(), lcsroomstay: myhelper.myroomdata, requestor: source, messagepswd: settings.LcsConfig.MessagePassword);

                var myaccosearchlcs = new Helper.LCS.GetAccommodationDataLCS(settings.LcsConfig.Username, settings.LcsConfig.Password);
                var response = await myaccosearchlcs.GetAccommodationDataSearchAsync(accosearchrequest);
                var myparsedresponse = Helper.LCS.ParseAccoSearchResult.ParsemyLCSResponse(language, response, myhelper.rooms);

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

    }
}
