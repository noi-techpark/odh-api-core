using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using OdhApiCore.Filters;
using OdhApiCore.GenericHelpers;
using EBMS;

namespace OdhApiCore.Controllers.api
{
    [ApiExplorerSettings(IgnoreApi = true)]    
    [ApiController]
    public class UpdateApiController : OdhController
    {        
        private readonly ISettings settings;

        public UpdateApiController(IWebHostEnvironment env, ISettings settings, ILogger<AlpineBitsController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {
            this.settings = settings;
        }

        [HttpGet, Route("EBMS/UpdateAll")]
        public async Task<IActionResult> UpdateAllEBMS(CancellationToken cancellationToken)
        {
            var result = await ImportEbmsEventsToDB();

            return Ok("EBMS Eventshorts \" updated");
        }


        [HttpGet, Route("EBMS/UpdateSingle/{id}")]
        public async Task<IActionResult> UpdateSingleEBMS(string id, CancellationToken cancellationToken)
        {
            //await STARequestHelper.GenerateJSONODHActivityPoiForSTA(QueryFactory, settings.JsonConfig.Jsondir, settings.XmlConfig.Xmldir);

            return Ok("EBMS Eventshorts \" updated");
        }

        private async Task<string> ImportEbmsEventsToDB()
        {
            try
            {                
                var result = ImportEBMSData.GetEbmsEvents(settings.EbmsConfig.User, settings.EbmsConfig.Password);

                var currenteventshort = await GetAllEventsShort(DateTime.Now);

                var resultsorted = result.OrderBy(x => x.StartDate);

                var updatecounter = 0;
                var newcounter = 0;
                var deletecounter = 0;

                foreach (var eventshort in resultsorted)
                {
                    bool neweventshort = false;

                    var query =
                       QueryFactory.Query("eventeuracnoi")
                           .Select("data")
                           .Where("id", eventshort.Id);

                    var eventindbraw = await query.FirstOrDefaultAsync<JsonRaw?>();
                    var eventindb = JsonConvert.DeserializeObject<EventShort>(eventindbraw.Value);

                    //currenteventshort.Where(x => x.EventId == eventshort.EventId).FirstOrDefault();

                    var changedonDB = DateTime.Now;

                    //Fields to not overwrite
                    var imagegallery = new List<ImageGallery>();
                    var eventTextDE = "";
                    var eventTextIT = "";
                    var eventTextEN = "";
                    var videourl = "";
                    Nullable<bool> activeweb = null;
                    List<string> technologyfields = null;
                    List<string> customtagging = null;
                    var webadress = "";
                    List<DocumentPDF> eventdocument = new List<DocumentPDF>();
                    bool? soldout = false;
                    bool? externalorganizer = false;

                    if (eventindb != null)
                    {
                        changedonDB = eventindb.ChangedOn;
                        imagegallery = eventindb.ImageGallery;
                        eventTextDE = eventindb.EventTextDE;
                        eventTextIT = eventindb.EventTextIT;
                        eventTextEN = eventindb.EventTextEN;
                        activeweb = eventindb.ActiveWeb;
                        videourl = eventindb.VideoUrl;
                        technologyfields = eventindb.TechnologyFields;
                        customtagging = eventindb.CustomTagging;
                        webadress = eventindb.WebAddress;
                        externalorganizer = eventindb.ExternalOrganizer;

                        eventdocument = eventindb.EventDocument;
                        soldout = eventindb.SoldOut;
                    }
                    else
                    {
                        neweventshort = true;
                    }


                    if (changedonDB != eventshort.ChangedOn)
                    {

                        eventshort.ImageGallery = imagegallery;
                        eventshort.EventTextDE = eventTextDE;
                        eventshort.EventTextIT = eventTextIT;
                        eventshort.EventTextEN = eventTextEN;
                        eventshort.ActiveWeb = activeweb;
                        eventshort.VideoUrl = videourl;
                        eventshort.TechnologyFields = technologyfields;
                        eventshort.CustomTagging = customtagging;
                        if (!String.IsNullOrEmpty(webadress))
                            eventshort.WebAddress = webadress;

                        eventshort.SoldOut = soldout;
                        eventshort.EventDocument = eventdocument;
                        eventshort.ExternalOrganizer = externalorganizer;

                        //New If CompanyName is Noi - blablabla assign TechnologyField automatically and Write to Display5 if not empty "NOI"
                        if (eventshort.CompanyName.StartsWith("NOI - "))
                        {
                            if (String.IsNullOrEmpty(eventshort.Display5))
                                eventshort.Display5 = "NOI";

                            eventshort.TechnologyFields = AssignTechnologyfieldsautomatically(eventshort.CompanyName, eventshort.TechnologyFields);
                        }

                        int queryresult = 0;

                        //Setting LicenseInfo
                        eventshort.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<EventShort>(eventshort, Helper.LicenseHelper.GetLicenseforEventShort);


                        if (neweventshort)
                        {
                            queryresult = await QueryFactory.Query("eventeuracnoi")
                                .InsertAsync(new JsonBData() { id = eventshort.Id.ToLower(), data = new JsonRaw(eventshort) });

                            newcounter++;
                        }
                        else
                        {
                            //var query = await QueryFactory.Query("alpinebits").UpdateAsync(new JsonBData() { id = id, data = new JsonRaw(input) });

                            //TODO CHECK IF THIS WORKS     
                            queryresult = await QueryFactory.Query("eventeuracnoi").Where("id", eventshort.Id)
                                .UpdateAsync(new JsonBData() { id = eventshort.Id.ToLower(), data = new JsonRaw(eventshort) });

                            updatecounter++;
                        }

                        //if (queryresult != "1")
                        //    Constants.tracesource.TraceEvent(TraceEventType.Error, 0, "EventShort save error: " + eventshort.Id);

                        //Console.ForegroundColor = ConsoleColor.Green;
                        //Console.WriteLine("EventShort imported: " + eventshort.Id);
                        //Constants.tracesource.TraceEvent(TraceEventType.Information, 0, "EventShort imported: " + eventshort.Id);                        
                    }
                }

                if (result.Count > 0)
                    deletecounter = await DeleteDeletedEvents(result, currenteventshort.ToList());

                return String.Format("Events Updated {0} New {1} Deleted {2]", updatecounter, newcounter, deletecounter );
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static List<string> AssignTechnologyfieldsautomatically(string companyname, List<string> technologyfields)
        {
            if (technologyfields == null)
                technologyfields = new List<string>();

            //Digital, Alpine, Automotive/Automation, Food, Green

            AssignTechnologyFields(companyname, "Digital", "Digital", technologyfields);
            AssignTechnologyFields(companyname, "Alpine", "Alpine", technologyfields);
            AssignTechnologyFields(companyname, "Automotive", "Automotive/Automation", technologyfields);
            AssignTechnologyFields(companyname, "Food", "Food", technologyfields);
            AssignTechnologyFields(companyname, "Green", "Green", technologyfields);

            if (technologyfields.Count == 0)
                return null;
            else
                return technologyfields;
        }

        private static void AssignTechnologyFields(string companyname, string tocheck, string toassign, List<string> automatictechnologyfields)
        {
            if (companyname.Contains(tocheck))
                if (!automatictechnologyfields.Contains(toassign))
                    automatictechnologyfields.Add(toassign);
        }

        private async Task<int> DeleteDeletedEvents(List<EventShort> eventshortfromnow, List<EventShort> eventshortinDB)
        {
            //TODO CHECK if Event is in list, if not, DELETE!
            //TODO CHECK IF THIS IS WORKING CORRECTLY


            var idsonListinDB = eventshortinDB.Select(x => x.EventId).ToList();
            var idsonService = eventshortfromnow.Select(x => x.EventId).ToList();

            var deletecounter = 0;

            var idstodelete = idsonListinDB.Where(p => !idsonService.Any(p2 => p2 == p));

            if (idstodelete.Count() > 0)           
            {
                foreach (var idtodelete in idstodelete)
                {
                    //Set to inactive or delete?

                    var eventshorttodeactivate = eventshortinDB.Where(x => x.EventId == idtodelete).FirstOrDefault();

                    //TODO CHECK IF IT WORKS
                    if (eventshorttodeactivate != null)
                    {
                        await QueryFactory.Query("eventeuracnoi").Where("id", eventshorttodeactivate.Id.ToLower()).DeleteAsync();
                        deletecounter++;
                    }                        
                }
            }

            return deletecounter;
        }

        private async Task<IEnumerable<EventShort>> GetAllEventsShort(DateTime now)
        {
            var today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            string where = "(((to_date(data->> 'EndDate', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", today) + "'))) AND (data @> '{ \"Source\" : \"EBMS\" }')";

            var query =
                         QueryFactory.Query("eventeuracnoi")
                             .Select("data")
                             .WhereRaw(where);

            var myevents = await query.GetAsync<JsonRaw>();

            List<EventShort> myeventshortlist = new List<EventShort>();

            foreach(var myevent in myevents)
            {
                myeventshortlist.Add(JsonConvert.DeserializeObject<EventShort>(myevent.Value));
            }
            
            return myeventshortlist;            
        }

    }
}