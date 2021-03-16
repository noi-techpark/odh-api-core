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
            //await STARequestHelper.GenerateJSONODHActivityPoiForSTA(QueryFactory, settings.JsonConfig.Jsondir, settings.XmlConfig.Xmldir);

            return Ok("EBMS Eventshorts \" updated");
        }


        [HttpGet, Route("EBMS/UpdateSingle/{id}")]
        public async Task<IActionResult> UpdateSingleEBMS(string id, CancellationToken cancellationToken)
        {
            //await STARequestHelper.GenerateJSONODHActivityPoiForSTA(QueryFactory, settings.JsonConfig.Jsondir, settings.XmlConfig.Xmldir);

            return Ok("EBMS Eventshorts \" updated");
        }

        private async Task ImportEvmsEventsToDB()
        {
            try
            {                
                var result = ImportEBMSData.GetEbmsEvents(settings.EbmsConfig.User, settings.EbmsConfig.Password);

                IEnumerable<EventShort> currenteventshort = GetAllEventsShort(DateTime.Now).ToList();

                var resultsorted = result.OrderBy(x => x.StartDate);

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
                                .InsertAsync(new JsonBData() { id = eventshort.Id, data = new JsonRaw(eventshort) });
                        }
                        else
                        {
                            //var query = await QueryFactory.Query("alpinebits").UpdateAsync(new JsonBData() { id = id, data = new JsonRaw(input) });

                            //TODO CHECK IF THIS WORKS     
                            queryresult = await QueryFactory.Query("eventeuracnoi").Where("id", eventshort.Id)
                                .UpdateAsync(new JsonBData() { id = eventshort.Id, data = new JsonRaw(eventshort) });

                            //queryresult = PostgresSQLHelper.UpdateDataFromTable(conn, "eventeuracnoi", JsonConvert.SerializeObject(eventshort), eventshort.Id);
                        }

                        //if (queryresult != "1")
                        //    Constants.tracesource.TraceEvent(TraceEventType.Error, 0, "EventShort save error: " + eventshort.Id);

                        //Console.ForegroundColor = ConsoleColor.Green;
                        //Console.WriteLine("EventShort imported: " + eventshort.Id);
                        //Constants.tracesource.TraceEvent(TraceEventType.Information, 0, "EventShort imported: " + eventshort.Id);                        
                    }
                }

                //if (result.Count > 0)
                //    DeleteDeletedEvents(result, currenteventshort.ToList());

                //Constants.tracesource.TraceEvent(TraceEventType.Information, 0, "EventShort Import succeeded");
            }
            catch (Exception ex)
            {
                //Console.WriteLine("ERROR on EventShort import:" + ex.Message);
                //Constants.tracesource.TraceEvent(TraceEventType.Error, 0, "ERROR on EventShort import:" + ex.Message);

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

        //private static void DeleteDeletedEvents(List<EventShort> eventshortfromnow, List<EventShort> eventshortinDB)
        //{
        //    //TODO CHECK if Event is in list, if not, DELETE!
        //    //TODO CHECK IF THIS IS WORKING CORRECTLY


        //    var idsonListinDB = eventshortinDB.Select(x => x.EventId).ToList();
        //    var idsonService = eventshortfromnow.Select(x => x.EventId).ToList();


        //    var idstodelete = idsonListinDB.Where(p => !idsonService.Any(p2 => p2 == p));

        //    if (idstodelete.Count() == 0)
        //    {
        //        Console.ForegroundColor = ConsoleColor.DarkRed;
        //        Console.WriteLine("Nothing to delete");
        //        Constants.tracesource.TraceEvent(TraceEventType.Information, 0, "Nothing to delete");
        //    }
        //    else
        //    {
        //        foreach (var idtodelete in idstodelete)
        //        {
        //            //Set to inactive or delete?

        //            var eventshorttodeactivate = eventshortinDB.Where(x => x.EventId == idtodelete).FirstOrDefault();

        //            if (eventshorttodeactivate != null)
        //            {

        //                using (var conn = new NpgsqlConnection(GlobalPGConnection.PGConnectionString))
        //                {
        //                    conn.Open();

        //                    PostgresSQLHelper.DeleteDataFromTable(conn, "eventeuracnoi", eventshorttodeactivate.Id.ToLower());

        //                    Console.ForegroundColor = ConsoleColor.DarkGreen;

        //                    Console.WriteLine("EventShort DELETED not more on service: " + eventshorttodeactivate.Id);
        //                    Constants.tracesource.TraceEvent(TraceEventType.Information, 0, "EventShort DELETED not more on service: " + eventshorttodeactivate.Id);

        //                }
        //            }
        //            else
        //            {
        //                Console.ForegroundColor = ConsoleColor.DarkRed;

        //                Console.WriteLine("EventShort already deleted: " + eventshorttodeactivate.Id);
        //                //Constants.tracesource.TraceEvent(TraceEventType.Information, 0, "EventShort already set to inactive: " + eventshorttodeactivate.Id);
        //            }
        //        }
        //    }
        //}

        //private static IEnumerable<EventShort> GetAllEventsShort(DateTime now)
        //{
        //    var today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);            

        //    using (var conn = new NpgsqlConnection(GlobalPGConnection.PGConnectionString))
        //    {
        //        conn.Open();

        //        string select = "*";
        //        string orderby = "";

        //        string where = "(((to_date(data->> 'EndDate', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", today) + "'))) AND (data @> '{ \"Source\" : \"EBMS\" }')";

        //        var data = PostgresSQLHelper.SelectFromTableDataAsObject<EventShort>(conn, "eventeuracnoi", select, where, orderby, 0, null);

        //        return data;
        //    }
        //}

    }
}