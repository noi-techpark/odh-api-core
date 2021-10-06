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
using NINJA;
using NINJA.Parser;
using System.Net.Http;
using RAVEN;
using Microsoft.Extensions.Hosting;

namespace OdhApiCore.Controllers.api
{
    [ApiExplorerSettings(IgnoreApi = true)]    
    [ApiController]
    public class UpdateApiController : OdhController
    {
        private readonly ISettings settings;
        private readonly IWebHostEnvironment env;

        public UpdateApiController(IWebHostEnvironment env, ISettings settings, ILogger<UpdateApiController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {
            this.env = env;
            this.settings = settings;
        }

        #region EBMS exposed

        [HttpGet, Route("EBMS/EventShort/UpdateAll")]
        public async Task<IActionResult> UpdateAllEBMS(CancellationToken cancellationToken)
        {
            try
            {
                var result = await ImportEbmsEventsToDB();

                return Ok(new UpdateResult
                {
                    operation = "Update EBMS",
                    updatetype = "all",
                    otherinfo = "",
                    message = "EBMS Eventshorts update succeeded",
                    recordsmodified = (result.created + result.updated + result.deleted).ToString(),
                    created = result.created,
                    updated = result.updated,
                    deleted = result.deleted,
                    success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new UpdateResult
                {
                    operation = "Update EBMS",
                    updatetype = "all",
                    otherinfo = "",
                    message = "EBMS Eventshorts update failed: " + ex.Message,
                    recordsmodified = "0",
                    created = 0,
                    updated = 0,
                    deleted = 0,
                    success = false
                });
            }
        }


        [HttpGet, Route("EBMS/EventShort/UpdateSingle/{id}")]
        public IActionResult UpdateSingleEBMS(string id, CancellationToken cancellationToken)
        {
            try
            {
                //TODO
                //await STARequestHelper.GenerateJSONODHActivityPoiForSTA(QueryFactory, settings.JsonConfig.Jsondir, settings.XmlConfig.Xmldir);

                return Ok(new UpdateResult
                {
                    operation = "Update EBMS",
                    id = id,
                    updatetype = "single",
                    otherinfo = "",
                    message = "EBMS Eventshorts update succeeded",
                    recordsmodified = "1",
                    created = 0,
                    updated = 0,
                    deleted = 0,
                    success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new UpdateResult
                {
                    operation = "Update EBMS",
                    updatetype = "all",
                    otherinfo = "",
                    message = "EBMS Eventshorts update failed: " + ex.Message,
                    recordsmodified = "0",
                    created = 0,
                    updated = 0,
                    deleted = 0,
                    success = false
                });
            }
        }

        #endregion

        #region NINJA exposed

        [HttpGet, Route("NINJA/Events/UpdateAll")]
        public async Task<IActionResult> UpdateAllNinjaEvents(CancellationToken cancellationToken)
        {
            try
            {
                var responseevents = await GetNinjaData.GetNinjaEvent();
                var responseplaces = await GetNinjaData.GetNinjaPlaces();

                var result = await SaveEventsToPG(responseevents.data, responseplaces.data);

                return Ok(new UpdateResult
                {
                    operation = "Update Ninja Events",
                    updatetype = "all",
                    otherinfo = "",
                    message = "Ninja Events update succeeded",
                    recordsmodified = (result.created + result.updated + result.deleted).ToString(),
                    created = result.created,
                    updated = result.updated,
                    deleted = result.deleted,
                    success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new UpdateResult
                {
                    operation = "Update Ninja Events",
                    updatetype = "all",
                    otherinfo = "",
                    message = "Update Ninja Events failed: " + ex.Message,
                    recordsmodified = "0",
                    created = 0,
                    updated = 0,
                    deleted = 0,
                    success = false
                });
            }
        }

        #endregion

        #region ODH RAVEN exposed

        [HttpGet, Route("Raven/{datatype}/Update/{id}")]
        //[Authorize(Roles = "DataWriter,DataCreate,DataUpdate")]
        public async Task<IActionResult> UpdateFromRaven(string id, string datatype, CancellationToken cancellationToken)
        {
            try
            {
                var result = await GetFromRavenAndTransformToPGObject(id, datatype, cancellationToken);
                
                return Ok(new UpdateResult
                {
                    operation = "Update Raven",
                    updatetype = "single",
                    otherinfo = datatype,
                    id = id,
                    message = "",
                    recordsmodified = (result.created + result.updated + result.deleted).ToString(),
                    created = result.created,
                    updated = result.updated,
                    deleted = result.deleted,
                    success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new UpdateResult
                {
                    operation = "Update Raven",
                    updatetype = "all",
                    otherinfo = "",
                    id = id,
                    message = "Update Raven failed: " + ex.Message,
                    recordsmodified = "0",
                    created = 0,
                    updated = 0,
                    deleted = 0,
                    success = false
                });
            }
        }

        #endregion

        #region EBMS Helpers

        private async Task<UpdateDetail> ImportEbmsEventsToDB()
        {
            var resulttuple = ImportEBMSData.GetEbmsEvents(settings.EbmsConfig.User, settings.EbmsConfig.Password);
            var resulttuplesorted = resulttuple.OrderBy(x => x.Item1.StartDate);

            var currenteventshort = await GetAllEventsShort(DateTime.Now);

            //var result = resulttuple.Select(x => x.Item1).ToList();
            //var resultsorted = result.OrderBy(x => x.StartDate);

            var updatecounter = 0;
            var newcounter = 0;
            var deletecounter = 0;

            foreach (var (eventshort, eventebms) in resulttuplesorted)
            {
                bool neweventshort = false;

                var query =
                   QueryFactory.Query("eventeuracnoi")
                       .Select("data")
                       .Where("id", eventshort.Id);

                var eventindb = await query.GetFirstOrDefaultAsObject<EventShort>();

                //currenteventshort.Where(x => x.EventId == eventshort.EventId).FirstOrDefault();

                var changedonDB = DateTime.Now;

                //Fields to not overwrite
                var imagegallery = new List<ImageGallery>();
                var eventTextDE = "";
                var eventTextIT = "";
                var eventTextEN = "";
                var videourl = "";
                Nullable<bool> activeweb = null;
                List<string>? technologyfields = null;
                List<string>? customtagging = null;
                var webadress = "";
                List<DocumentPDF>? eventdocument = new List<DocumentPDF>();
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

                    eventdocument = eventindb?.EventDocument;
                    soldout = eventindb?.SoldOut;
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
                    if (eventshort.CompanyName?.StartsWith("NOI - ") ?? false)
                    {
                        if (String.IsNullOrEmpty(eventshort.Display5))
                            eventshort.Display5 = "NOI";

                        eventshort.TechnologyFields = AssignTechnologyfieldsautomatically(eventshort.CompanyName, eventshort?.TechnologyFields ?? new List<string>());
                    }

                    int queryresult = 0;

                    //Setting LicenseInfo
                    eventshort!.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<EventShort>(eventshort, Helper.LicenseHelper.GetLicenseforEventShort);


                    if (neweventshort)
                    {
                        queryresult = await QueryFactory.Query("eventeuracnoi")
                            .InsertAsync(new JsonBData() { id = eventshort.Id?.ToLower(), data = new JsonRaw(eventshort) });
                        //.InsertAsync(new JsonBDataRaw() { id = eventshort.Id.ToLower(), data = new JsonRaw(eventshort), raw = JsonConvert.SerializeObject(eventebms) });

                        newcounter++;
                    }
                    else
                    {
                        //var query = await QueryFactory.Query("alpinebits").UpdateAsync(new JsonBData() { id = id, data = new JsonRaw(input) });

                        //TODO CHECK IF THIS WORKS     
                        queryresult = await QueryFactory.Query("eventeuracnoi").Where("id", eventshort.Id)
                            .UpdateAsync(new JsonBData() { id = eventshort?.Id?.ToLower(), data = eventshort != null ? new JsonRaw(eventshort) : null });
                        //.UpdateAsync(new JsonBDataRaw() { id = eventshort.Id.ToLower(), data = new JsonRaw(eventshort), raw = JsonConvert.SerializeObject(eventebms) });

                        updatecounter++;
                    }

                    //if (queryresult != "1")
                    //    Constants.tracesource.TraceEvent(TraceEventType.Error, 0, "EventShort save error: " + eventshort.Id);

                    //Console.ForegroundColor = ConsoleColor.Green;
                    //Console.WriteLine("EventShort imported: " + eventshort.Id);
                    //Constants.tracesource.TraceEvent(TraceEventType.Information, 0, "EventShort imported: " + eventshort.Id);                        
                }
            }

            if (resulttuple.Select(x => x.Item1).Count() > 0)
                deletecounter = await DeleteDeletedEvents(resulttuple.Select(x => x.Item1).ToList(), currenteventshort.ToList());

            return new UpdateDetail() { created = newcounter, updated = updatecounter, deleted = deletecounter };
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
                return new List<string>();
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
                        await QueryFactory.Query("eventeuracnoi").Where("id", eventshorttodeactivate.Id?.ToLower()).DeleteAsync();
                        deletecounter++;
                    }                        
                }
            }

            return deletecounter;
        }

        private async Task<IEnumerable<EventShort>> GetAllEventsShort(DateTime now)
        {
            var today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            
            var query =
                         QueryFactory.Query("eventeuracnoi")
                             .Select("data")
                             .WhereRaw("(((to_date(data->> 'EndDate', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", today) + "'))) AND(data#>>'\\{Source\\}' = ?)", "EBMS");

            return await query.GetAllAsObject<EventShort>();
        }

        #endregion

        #region NINJA Helpers

        //TODO CHECK IF THIS IS WORKING AND REFACTOR

        /// <summary>
        /// Save Events to Postgres
        /// </summary>
        /// <param name="ninjadataarr"></param>
        private async Task<UpdateDetail> SaveEventsToPG(ICollection<NinjaData<NinjaEvent>> ninjadataarr, ICollection<NinjaData<NinjaPlaceRoom>> ninjaplaceroomarr)
        {

            var newimportcounter = 0;
            var updateimportcounter = 0;
            var errorimportcounter = 0;
            var deleteimportcounter = 0;

            List<string> idlistspreadsheet = new List<string>();
            List<string> sourcelist = new List<string>();

            foreach (var ninjadata in ninjadataarr.Select(x => x.tmetadata))
            {
                foreach (KeyValuePair<string, NinjaEvent> kvp in ninjadata)
                {
                    if (!String.IsNullOrEmpty(kvp.Key))
                    {
                        var place = ninjaplaceroomarr.Where(x => x.sname == kvp.Value.place).FirstOrDefault();
                        var room = ninjaplaceroomarr.Where(x => x.sname == kvp.Value.room).FirstOrDefault();

                        var eventtosave = ParseNinjaData.ParseNinjaEventToODHEvent(kvp.Key, kvp.Value, place, room);

                        //Setting Location Info
                        //Location Info (by GPS Point)
                        if (eventtosave.Latitude != 0 && eventtosave.Longitude != 0)
                        {
                            await SetLocationInfo(eventtosave);
                        }

                        eventtosave.Active = true;
                        eventtosave.SmgActive = false;

                        var idtocheck = kvp.Key;

                        if (idtocheck.Length > 50)
                            idtocheck = idtocheck.Substring(0, 50);

                        var result = await InsertEventInPG(eventtosave, idtocheck);

                        if (result.Item1 == "insert")
                        {
                            if (result.Item2 == "1")
                                newimportcounter++;
                        }
                        else if (result.Item1 == "update")
                        {
                            if (result.Item2 == "1")
                                updateimportcounter++;
                        }
                        else
                        {
                            errorimportcounter++;
                        }

                        idlistspreadsheet.Add(idtocheck.ToUpper());
                        if (!sourcelist.Contains(eventtosave.Source))
                            sourcelist.Add(eventtosave.Source);
                    }
                }

                //TODO get all IDs in DB
                var idlistdb = await GetAllEventsBySource(sourcelist);

                var idstodelete = idlistdb.Where(p => !idlistspreadsheet.Any(p2 => p2 == p));

                foreach (var idtodelete in idstodelete)
                {
                    deleteimportcounter = deleteimportcounter + await DeleteOrDisableEvents(idtodelete, false);
                }

            }

            return new UpdateDetail() { updated = updateimportcounter, created = newimportcounter, deleted = deleteimportcounter };
        }

        private async Task SetLocationInfo(EventLinked myevent)
        {
            string wheregeo = PostgresSQLHelper.GetGeoWhereSimple(myevent.Latitude, myevent.Longitude, 30000);
            string orderbygeo = PostgresSQLHelper.GetGeoOrderBySimple(myevent.Latitude, myevent.Longitude);

            var query =
                     QueryFactory.Query("districts")
                         .Select("data")
                         .WhereRaw(wheregeo)
                         .OrderByRaw(orderbygeo);

            var districtlist = await query.GetFirstOrDefaultAsObject<District>();            

            if (districtlist != null)
            {
                myevent.DistrictId = districtlist.Id;
                myevent.DistrictIds = new List<string>() { districtlist.Id };

                //TODO MAYBE IN HELPER!
                var locinfo = await GetTheLocationInfoDistrict(districtlist.Id);

                LocationInfoLinked locinfolinked = new LocationInfoLinked();
                locinfolinked.DistrictInfo = new DistrictInfoLinked()
                {
                    Id = locinfo?.DistrictInfo?.Id,
                    Name = locinfo?.DistrictInfo?.Name
                };
                locinfolinked.MunicipalityInfo = new MunicipalityInfoLinked()
                {
                    Id = locinfo?.MunicipalityInfo?.Id,
                    Name = locinfo?.MunicipalityInfo?.Name
                };
                locinfolinked.TvInfo = new TvInfoLinked()
                {
                    Id = locinfo?.TvInfo?.Id,
                    Name = locinfo?.TvInfo?.Name
                };
                locinfolinked.RegionInfo = new RegionInfoLinked()
                {
                    Id = locinfo?.RegionInfo?.Id,
                    Name = locinfo?.RegionInfo?.Name
                };

                myevent.LocationInfo = locinfolinked;
            }
        }

        private async Task<Tuple<string, string>> InsertEventInPG(EventLinked eventtosave, string idtocheck)
        {
            try
            {
                idtocheck = idtocheck.ToUpper();
                eventtosave.Id = eventtosave.Id?.ToUpper();

                //Set LicenseInfo
                eventtosave.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<Event>(eventtosave, Helper.LicenseHelper.GetLicenseforEvent);

                return await InsertInDB(eventtosave, idtocheck, "events");
            }
            catch (Exception ex)
            {
                return Tuple.Create("error", ex.Message);
            }
        }

        private async Task<Tuple<string, string>> InsertInDB(EventLinked eventtosave, string idtocheck, string tablename)
        {
            //Check if data exists on PG

            var query =
               QueryFactory.Query("events")
                   .Select("data")
                   .Where("id", idtocheck);

            var eventindb = await query.GetAsync<JsonRaw>();
            
            if (eventindb.Count() == 0)
            {
                eventtosave.FirstImport = DateTime.Now;

                var queryresult = await QueryFactory.Query(tablename)
                               .InsertAsync(new JsonBData() { id = idtocheck, data = new JsonRaw(eventtosave) });

                return Tuple.Create("insert", queryresult.ToString());
            }
            else
            {
                var queryresult = await QueryFactory.Query(tablename).Where("id", idtocheck)
                                .UpdateAsync(new JsonBData() { id = idtocheck, data = new JsonRaw(eventtosave) });

                return Tuple.Create("update", queryresult.ToString());
            }                      
        }

        private async Task<List<string>> GetAllEventsBySource(List<string> sourcelist)
        {

            var query =
               QueryFactory.Query("events")
                   .Select("id")
                   .SourceFilter_GeneratedColumn(sourcelist);

            var eventids = await query.GetAsync<string>();

            return eventids.ToList();
        }

        private async Task<int> DeleteOrDisableEvents(string eventid, bool delete)
        {
            var result = 0;

            if (delete)
            {
                result = await QueryFactory.Query("events").Where("id", eventid)
                    .DeleteAsync();
            }
            else
            {
                var query =
               QueryFactory.Query("events")
                   .Select("data")
                   .Where("id", eventid);

                var data = await query.GetFirstOrDefaultAsObject<EventLinked>();

                if(data != null)
                {
                    data.Active = false;
                    data.SmgActive = false;

                    result = await QueryFactory.Query("events").Where("id", eventid)
                                    .UpdateAsync(new JsonBData() { id = eventid, data = new JsonRaw(data) });
                }               
            }

            return result;
        }
        
        public async Task<LocationInfo?> GetTheLocationInfoDistrict(string districtid)
        {
            try
            {
                LocationInfo mylocinfo = new LocationInfo();

                var district = await QueryFactory.Query("districts").Select("data").Where("id", districtid.ToUpper()).GetFirstOrDefaultAsObject<District>(); 
                var districtnames = (from x in district?.Detail
                                     select x).ToDictionary(x => x.Key, x => x.Value.Title);

                var municipality = await QueryFactory.Query("municipalities").Select("data").Where("id", district?.MunicipalityId?.ToUpper()).GetFirstOrDefaultAsObject<Municipality>(); 
                var municipalitynames = (from x in municipality?.Detail
                                         select x).ToDictionary(x => x.Key, x => x.Value.Title);

                var tourismverein = await QueryFactory.Query("tvs").Select("data").Where("id", district?.TourismvereinId?.ToUpper()).GetFirstOrDefaultAsObject<Tourismverein>();
                var tourismvereinnames = (from x in tourismverein?.Detail
                                          select x).ToDictionary(x => x.Key, x => x.Value.Title);

                var region = await QueryFactory.Query("regions").Select("data").Where("id", district?.RegionId?.ToUpper()).GetFirstOrDefaultAsObject<Region>();
                var regionnames = (from x in region?.Detail
                                   select x).ToDictionary(x => x.Key, x => x.Value.Title);

                //conn.Close();

                mylocinfo.DistrictInfo = new DistrictInfo() { Id = district?.Id, Name = districtnames };
                mylocinfo.MunicipalityInfo = new MunicipalityInfo() { Id = municipality?.Id, Name = municipalitynames };
                mylocinfo.TvInfo = new TvInfo() { Id = tourismverein?.Id, Name = tourismvereinnames };
                mylocinfo.RegionInfo = new RegionInfo() { Id = region?.Id, Name = regionnames };

                return mylocinfo;
            }
            catch(Exception)
            {
                return null;
            }
        }

        #endregion

        #region ODHRAVEN Helpers

        private async Task<UpdateDetail> GetFromRavenAndTransformToPGObject(string id, string datatype, CancellationToken cancellationToken)
        {
            var mydata = default(IIdentifiable);
            var mypgdata = default(IIdentifiable);

            switch (datatype.ToLower())
            {
                case "accommodation":
                    mydata = await GetDataFromRaven.GetRavenData<AccommodationLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<AccommodationLinked, AccommodationLinked>((AccommodationLinked)mydata, TransformToPGObject.GetAccommodationPGObject);
                    else
                        throw new Exception("No data found!");

                    var accoresult = await SaveRavenObjectToPG<AccommodationLinked>((AccommodationLinked)mypgdata, "accommodations");

                    //UPDATE ACCOMMODATIONROOMS
                    var myroomdatalist = await GetDataFromRaven.GetRavenData<IEnumerable<AccommodationRoomLinked>>("accommodationroom", id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken, "AccommodationRoom?accoid=");

                    if (myroomdatalist != null)
                    {
                        foreach (var myroomdata in myroomdatalist)
                        {
                            var mypgroomdata = TransformToPGObject.GetPGObject<AccommodationRoomLinked, AccommodationRoomLinked>((AccommodationRoomLinked)myroomdata, TransformToPGObject.GetAccommodationRoomPGObject);

                            var accoroomresult = await SaveRavenObjectToPG<AccommodationRoomLinked>((AccommodationRoomLinked)mypgroomdata, "accommodationrooms");
                        }
                    }
                    else
                        throw new Exception("No data found!");

                    //return Ok(new GenericResult() { Message = String.Format("{0} success: {1}", "accommodations", id) });
                    return accoresult;

                case "gastronomy":
                    mydata = await GetDataFromRaven.GetRavenData<GastronomyLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<GastronomyLinked, GastronomyLinked>((GastronomyLinked)mydata, TransformToPGObject.GetGastronomyPGObject);
                    else
                        throw new Exception("No data found!");

                    return await SaveRavenObjectToPG<GastronomyLinked>((GastronomyLinked)mypgdata, "gastronomies");

                case "activity":
                    mydata = await GetDataFromRaven.GetRavenData<LTSActivityLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<LTSActivityLinked, LTSActivityLinked>((LTSActivityLinked)mydata, TransformToPGObject.GetActivityPGObject);
                    else
                        throw new Exception("No data found!");

                    return await SaveRavenObjectToPG<LTSActivityLinked>((LTSActivityLinked)mypgdata, "activities");

                case "poi":
                    mydata = await GetDataFromRaven.GetRavenData<LTSPoiLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<LTSPoiLinked, LTSPoiLinked>((LTSPoiLinked)mydata, TransformToPGObject.GetPoiPGObject);
                    else
                        throw new Exception("No data found!");

                    return await SaveRavenObjectToPG<LTSPoiLinked>((LTSPoiLinked)mypgdata, "pois");

                case "odhactivitypoi":
                    mydata = await GetDataFromRaven.GetRavenData<ODHActivityPoiLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<ODHActivityPoiLinked, ODHActivityPoiLinked>((ODHActivityPoiLinked)mydata, TransformToPGObject.GetODHActivityPoiPGObject);
                    else
                        throw new Exception("No data found!");

                    return await SaveRavenObjectToPG<ODHActivityPoiLinked>((ODHActivityPoiLinked)mypgdata, "smgpois");

                case "event":
                    mydata = await GetDataFromRaven.GetRavenData<EventLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<EventLinked, EventLinked>((EventLinked)mydata, TransformToPGObject.GetEventPGObject);
                    else
                        throw new Exception("No data found!");

                    return await SaveRavenObjectToPG<EventLinked>((EventLinked)mypgdata, "events");

                case "webcam":
                    mydata = await GetDataFromRaven.GetRavenData<WebcamInfoLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<WebcamInfoLinked, WebcamInfoLinked>((WebcamInfoLinked)mydata, TransformToPGObject.GetWebcamInfoPGObject);
                    else
                        throw new Exception("No data found!");

                    return await SaveRavenObjectToPG<EventLinked>((EventLinked)mypgdata, "events");

                case "metaregion":
                    mydata = await GetDataFromRaven.GetRavenData<MetaRegionLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<MetaRegionLinked, MetaRegionLinked>((MetaRegionLinked)mydata, TransformToPGObject.GetMetaRegionPGObject);
                    else
                        throw new Exception("No data found!");

                    return await SaveRavenObjectToPG<MetaRegionLinked>((MetaRegionLinked)mypgdata, "metaregions");

                case "region":
                    mydata = await GetDataFromRaven.GetRavenData<RegionLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<RegionLinked, RegionLinked>((RegionLinked)mydata, TransformToPGObject.GetRegionPGObject);
                    else
                        throw new Exception("No data found!");

                    return await SaveRavenObjectToPG<RegionLinked>((RegionLinked)mypgdata, "regions");

                case "tv":
                    mydata = await GetDataFromRaven.GetRavenData<TourismvereinLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken, "TourismAssociation/");
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<TourismvereinLinked, TourismvereinLinked>((TourismvereinLinked)mydata, TransformToPGObject.GetTourismAssociationPGObject);
                    else
                        throw new Exception("No data found!");

                    return await SaveRavenObjectToPG<TourismvereinLinked>((TourismvereinLinked)mypgdata, "tvs");

                case "municipality":
                    mydata = await GetDataFromRaven.GetRavenData<MunicipalityLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<MunicipalityLinked, MunicipalityLinked>((MunicipalityLinked)mydata, TransformToPGObject.GetMunicipalityPGObject);
                    else
                        throw new Exception("No data found!");

                    return await SaveRavenObjectToPG<MunicipalityLinked>((MunicipalityLinked)mypgdata, "municipalities");

                case "district":
                    mydata = await GetDataFromRaven.GetRavenData<DistrictLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<DistrictLinked, DistrictLinked>((DistrictLinked)mydata, TransformToPGObject.GetDistrictPGObject);
                    else
                        throw new Exception("No data found!");

                    return await SaveRavenObjectToPG<DistrictLinked>((DistrictLinked)mypgdata, "districts");

                case "experiencearea":
                    mydata = await GetDataFromRaven.GetRavenData<ExperienceAreaLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<ExperienceAreaLinked, ExperienceAreaLinked>((ExperienceAreaLinked)mydata, TransformToPGObject.GetExperienceAreaPGObject);
                    else
                        throw new Exception("No data found!");

                    return await SaveRavenObjectToPG<ExperienceAreaLinked>((ExperienceAreaLinked)mypgdata, "experienceareas");

                case "skiarea":
                    mydata = await GetDataFromRaven.GetRavenData<SkiAreaLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<SkiAreaLinked, SkiAreaLinked>((SkiAreaLinked)mydata, TransformToPGObject.GetSkiAreaPGObject);
                    else
                        throw new Exception("No data found!");

                    return await SaveRavenObjectToPG<SkiAreaLinked>((SkiAreaLinked)mypgdata, "skiareas");

                case "skiregion":
                    mydata = await GetDataFromRaven.GetRavenData<SkiRegionLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<SkiRegionLinked, SkiRegionLinked>((SkiRegionLinked)mydata, TransformToPGObject.GetSkiRegionPGObject);
                    else
                        throw new Exception("No data found!");

                    return await SaveRavenObjectToPG<SkiRegionLinked>((SkiRegionLinked)mypgdata, "skiregions");

                case "article":
                    mydata = await GetDataFromRaven.GetRavenData<ArticlesLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<ArticlesLinked, ArticlesLinked>((ArticlesLinked)mydata, TransformToPGObject.GetArticlePGObject);
                    else
                        throw new Exception("No data found!");

                    return await SaveRavenObjectToPG<ArticlesLinked>((ArticlesLinked)mypgdata, "articles");

                case "odhtag":
                    mydata = await GetDataFromRaven.GetRavenData<ODHTagLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<ODHTagLinked, ODHTagLinked>((ODHTagLinked)mydata, TransformToPGObject.GetODHTagPGObject);
                    else
                        throw new Exception("No data found!");

                    return await SaveRavenObjectToPG<ODHTagLinked>((ODHTagLinked)mypgdata, "smgtags");

                case "measuringpoint":
                    mydata = await GetDataFromRaven.GetRavenData<MeasuringpointLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken, "Weather/Measuringpoint/");
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<MeasuringpointLinked, MeasuringpointLinked>((MeasuringpointLinked)mydata, TransformToPGObject.GetMeasuringpointPGObject);
                    else
                        throw new Exception("No data found!");

                    return await SaveRavenObjectToPG<MeasuringpointLinked>((MeasuringpointLinked)mypgdata, "measuringpoints");

                case "venue":
                    mydata = await GetDataFromRaven.GetRavenData<DDVenue>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<DDVenue, DDVenue>((DDVenue)mydata, TransformToPGObject.GetVenuePGObject);
                    else
                        throw new Exception("No data found!");

                    return await SaveRavenObjectToPG<DDVenue>((DDVenue)mypgdata, "venues");

                case "wine":
                    mydata = await GetDataFromRaven.GetRavenData<WineLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<WineLinked, WineLinked>((WineLinked)mydata, TransformToPGObject.GetWinePGObject);
                    else
                        throw new Exception("No data found!");

                    return await SaveRavenObjectToPG<WineLinked>((WineLinked)mypgdata, "wines");

                default:
                    throw new Exception("no match found");
            }
        }

        private async Task<UpdateDetail> SaveRavenObjectToPG<T>(T datatosave, string table) where T: IIdentifiable, IImportDateassigneable, IMetaData, ILicenseInfo
        {
            datatosave._Meta.LastUpdate = datatosave.LastChange;

            //Temporary Hack will be moved to the importer workerservice

            var result = await UpsertData<T>(datatosave, table);

            var insertcounter = 0;
            var updatecounter = 0;
            var deletecounter = 0;

            if(result is OkObjectResult)
            {
                var resultstr = ((OkObjectResult)result).Value.ToString();

                if (resultstr != null)
                {
                    if (resultstr.StartsWith("INSERT"))
                    {
                        if (resultstr.Contains("recordsmodified: 1"))
                            insertcounter++;
                    }
                    if (resultstr.StartsWith("UPDATE"))
                    {
                        if (resultstr.Contains("recordsmodified: 1"))
                            updatecounter++;
                    }
                }
            }

            return new UpdateDetail() { created = insertcounter, updated = updatecounter, deleted = deletecounter };
        }

        #endregion
    }
}