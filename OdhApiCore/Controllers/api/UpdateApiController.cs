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
            var result = await ImportEbmsEventsToDB();

            return Ok(new UpdateResult
            {
                operation = "Update EBMS",
                updatetype = "all",
                message = "EBMS Eventshorts update succeeded",
                recordsupdated = result,
                success = true
            });                
        }


        [HttpGet, Route("EBMS/EventShort/UpdateSingle/{id}")]
        public async Task<IActionResult> UpdateSingleEBMS(string id, CancellationToken cancellationToken)
        {
            //TODO
            //await STARequestHelper.GenerateJSONODHActivityPoiForSTA(QueryFactory, settings.JsonConfig.Jsondir, settings.XmlConfig.Xmldir);

            return Ok(new UpdateResult
            {
                operation = "Update EBMS",
                id = id,
                updatetype = "single",
                message = "EBMS Eventshorts update succeeded",
                recordsupdated = "1",
                success = true
            });
        }

        #endregion

        #region NINJA exposed

        [HttpGet, Route("NINJA/Events/UpdateAll")]
        public async Task<IActionResult> UpdateAllNinjaEvents(CancellationToken cancellationToken)
        {
            var responseevents = await GetNinjaData.GetNinjaEvent();
            var responseplaces = await GetNinjaData.GetNinjaPlaces();

            var result = await SaveEventsToPG(responseevents.data, responseplaces.data);

            return Ok(new UpdateResult
            {
                operation = "Update Ninja Events",
                updatetype = "all",
                message = "Ninja Events update succeeded",
                recordsupdated = result,
                success = true
            });            
        }

        #endregion

        #region ODH RAVEN exposed

        [HttpGet, Route("Raven/{datatype}/Update/{id}")]
        [Authorize(Roles = "DataWriter,DataCreate,DataUpdate")]
        public async Task<IActionResult> UpdateFromRaven(string id, string datatype, CancellationToken cancellationToken)
        {
            return await GetFromRavenAndTransformToPGObject(id, datatype, cancellationToken);
        }        

        #endregion

        #region EBMS Helpers

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

                return String.Format("Events Updated {0} New {1} Deleted {2}", updatecounter.ToString(), newcounter.ToString(), deletecounter.ToString());
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
        private async Task<string> SaveEventsToPG(ICollection<NinjaData<NinjaEvent>> ninjadataarr, ICollection<NinjaData<NinjaPlaceRoom>> ninjaplaceroomarr)
        {
            try
            {
                var newimportcounter = 0;
                var updateimportcounter = 0;
                var errorimportcounter = 0;

                foreach (var ninjadata in ninjadataarr.Select(x => x.tmetadata))
                {
                    foreach (KeyValuePair<string, NinjaEvent> kvp in ninjadata)
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
                                                
                        if(result.Item1 == "insert")
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
                    }
                }

                return String.Format("Events Updated {0} New {1} Error {2}", updateimportcounter.ToString(), newimportcounter.ToString(), errorimportcounter.ToString());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }            
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
                    Id = locinfo.DistrictInfo.Id,
                    Name = locinfo.DistrictInfo.Name
                };
                locinfolinked.MunicipalityInfo = new MunicipalityInfoLinked()
                {
                    Id = locinfo.MunicipalityInfo.Id,
                    Name = locinfo.MunicipalityInfo.Name
                };
                locinfolinked.TvInfo = new TvInfoLinked()
                {
                    Id = locinfo.TvInfo.Id,
                    Name = locinfo.TvInfo.Name
                };
                locinfolinked.RegionInfo = new RegionInfoLinked()
                {
                    Id = locinfo.RegionInfo.Id,
                    Name = locinfo.RegionInfo.Name
                };

                myevent.LocationInfo = locinfolinked;
            }
        }

        private async Task<Tuple<string, string>> InsertEventInPG(EventLinked eventtosave, string idtocheck)
        {
            try
            {
                idtocheck = idtocheck.ToUpper();
                eventtosave.Id = eventtosave.Id.ToUpper();

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

        public async Task<LocationInfo> GetTheLocationInfoDistrict(string districtid)
        {
            try
            {
                LocationInfo mylocinfo = new LocationInfo();

                var district = await QueryFactory.Query("districts").Select("data").Where("id", districtid.ToUpper()).GetFirstOrDefaultAsObject<District>(); 
                var districtnames = (from x in district.Detail
                                     select x).ToDictionary(x => x.Key, x => x.Value.Title);

                var municipality = await QueryFactory.Query("municipalities").Select("data").Where("id", district.MunicipalityId.ToUpper()).GetFirstOrDefaultAsObject<Municipality>(); 
                var municipalitynames = (from x in municipality.Detail
                                         select x).ToDictionary(x => x.Key, x => x.Value.Title);

                var tourismverein = await QueryFactory.Query("tvs").Select("data").Where("id", district.TourismvereinId.ToUpper()).GetFirstOrDefaultAsObject<Tourismverein>();
                var tourismvereinnames = (from x in tourismverein.Detail
                                          select x).ToDictionary(x => x.Key, x => x.Value.Title);

                var region = await QueryFactory.Query("regions").Select("data").Where("id", district.RegionId.ToUpper()).GetFirstOrDefaultAsObject<Region>();
                var regionnames = (from x in region.Detail
                                   select x).ToDictionary(x => x.Key, x => x.Value.Title);

                //conn.Close();

                mylocinfo.DistrictInfo = new DistrictInfo() { Id = district.Id, Name = districtnames };
                mylocinfo.MunicipalityInfo = new MunicipalityInfo() { Id = municipality.Id, Name = municipalitynames };
                mylocinfo.TvInfo = new TvInfo() { Id = tourismverein.Id, Name = tourismvereinnames };
                mylocinfo.RegionInfo = new RegionInfo() { Id = region.Id, Name = regionnames };

                return mylocinfo;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        #endregion

        #region ODHRAVEN Helpers

        private async Task<IActionResult> GetFromRavenAndTransformToPGObject(string id, string datatype, CancellationToken cancellationToken)
        {
            try
            {
                var mydata = default(IIdentifiable);
                var mypgdata = default(IIdentifiable);

                switch (datatype.ToLower())
                {
                    case "accommodation":
                        mydata = await GetDataFromRaven.GetRavenData<AccommodationLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                        if (mydata != null)
                            mypgdata = TransformToPGObject.GetPGObject<AccommodationLinked, AccommodationLinked>((AccommodationLinked)mydata, TransformToPGObject.GetAccommodationPGObject);                                                    
                        //TODO CALL UPDATE METHOD ALSO FOR ROOMS
                        else
                            throw new Exception("No data found!");

                        return await SaveRavenObjectToPG<AccommodationLinked>((AccommodationLinked)mypgdata, "accommodations");

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
                        mydata = await GetDataFromRaven.GetRavenData<SmgPoiLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                        if (mydata != null)
                            mypgdata = TransformToPGObject.GetPGObject<SmgPoiLinked, SmgPoiLinked>((SmgPoiLinked)mydata, TransformToPGObject.GetODHActivityPoiPGObject);
                        else
                            throw new Exception("No data found!");

                        return await SaveRavenObjectToPG<SmgPoiLinked>((SmgPoiLinked)mypgdata, "smgpois");

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
                        mydata = await GetDataFromRaven.GetRavenData<TourismvereinLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
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

                        return await SaveRavenObjectToPG<ExperienceAreaLinked>((ExperienceAreaLinked)mypgdata, "skiareas");

                    case "skiregion":
                        mydata = await GetDataFromRaven.GetRavenData<SkiRegionLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                        if (mydata != null)
                            mypgdata = TransformToPGObject.GetPGObject<SkiRegionLinked, SkiRegionLinked>((SkiRegionLinked)mydata, TransformToPGObject.GetSkiRegionPGObject);
                        else
                            throw new Exception("No data found!");

                        return await SaveRavenObjectToPG<SkiRegionLinked>((SkiRegionLinked)mypgdata, "skiregions");

                    case "odhtag":
                        mydata = await GetDataFromRaven.GetRavenData<SmgTags>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                        if (mydata != null)
                            mypgdata = TransformToPGObject.GetPGObject<SmgTags, SmgTags>((SmgTags)mydata, TransformToPGObject.GetODHTagPGObject);
                        else
                            throw new Exception("No data found!");

                        return await SaveRavenObjectToPG<SmgTags>((SmgTags)mypgdata, "smgtags");

                    default:
                        return BadRequest(new { error = "no match found" });
                }
            }
            catch(Exception ex)
            {
                return BadRequest(new { error = env.IsDevelopment() ? ex.ToString() : ex.Message });
            }            
        }

        private async Task<IActionResult> SaveRavenObjectToPG<T>(T datatosave, string table) where T: IIdentifiable, IImportDateassigneable
        {
            return await UpsertData<T>(datatosave, table);
        }

        #endregion

    }

    public class UpdateResult
    {
        public string operation { get; set; }
        public string updatetype { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public string recordsupdated { get; set; }

        public string? id { get; set; }
    }

}