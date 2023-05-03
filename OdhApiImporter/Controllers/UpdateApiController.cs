#nullable disable

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
using EBMS;
using NINJA;
using NINJA.Parser;
using System.Net.Http;
using RAVEN;
using Microsoft.Extensions.Hosting;
using OdhApiImporter.Helpers;
using OdhApiImporter.Helpers.DSS;
using OdhApiImporter.Helpers.LOOPTEC;
using OdhApiImporter.Helpers.SuedtirolWein;
using OdhNotifier;
using ServiceReferenceLCS;

namespace OdhApiImporter.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class UpdateApiController : Controller
    {
        private readonly ISettings settings;
        private readonly QueryFactory QueryFactory;
        private readonly ILogger<UpdateApiController> logger;
        private readonly IWebHostEnvironment env;
        private IOdhPushNotifier OdhPushnotifier;

        public UpdateApiController(IWebHostEnvironment env, ISettings settings, ILogger<UpdateApiController> logger, QueryFactory queryFactory, IOdhPushNotifier odhpushnotifier)
        {
            this.env = env;
            this.settings = settings;
            this.logger = logger;
            this.QueryFactory = queryFactory;
            this.OdhPushnotifier = odhpushnotifier;
        }

        #region UPDATE FROM RAVEN INSTANCE

        [HttpGet, Route("Raven/{datatype}/Update/{id}")]
        [Authorize(Roles = "DataPush")]
        public async Task<IActionResult> UpdateDataFromRaven(string id, string datatype, CancellationToken cancellationToken = default)
        {
            UpdateDetail updatedetail = default(UpdateDetail);
            string operation = "Update Raven";
            string updatetype = "single";
            string source = "api";
            string otherinfo = datatype;

            try
            {
                RavenImportHelper ravenimporthelper = new RavenImportHelper(settings, QueryFactory, UrlGeneratorStatic("Raven/" + datatype), OdhPushnotifier);
                var resulttuple = await ravenimporthelper.GetFromRavenAndTransformToPGObject(id, datatype, cancellationToken);
                updatedetail = resulttuple.Item2;

                var updateResult = GenericResultsHelper.GetSuccessUpdateResult(resulttuple.Item1, source, operation, updatetype, "Update Raven succeeded", otherinfo, updatedetail, true);

                return Ok(updateResult);
            }
            catch (Exception ex)
            {
                var errorResult = GenericResultsHelper.GetErrorUpdateResult(id, source, operation, updatetype, "Update Raven failed", otherinfo, updatedetail, ex, true);               

                return BadRequest(errorResult);
            }
        }

        [HttpGet, Route("Raven/{datatype}/Delete/{id}")]
        [Authorize(Roles = "DataPush")]
        public async Task<IActionResult> DeleteDataFromRaven(string id, string datatype, CancellationToken cancellationToken = default)
        {
            UpdateDetail updatedetail = default(UpdateDetail);
            string operation = "Delete Raven";
            string updatetype = "single";
            string source = "api";
            string otherinfo = datatype;

            try
            {
               
                RavenImportHelper ravenimporthelper = new RavenImportHelper(settings, QueryFactory, UrlGeneratorStatic("Raven/" + datatype), OdhPushnotifier);
                var resulttuple = await ravenimporthelper.DeletePGObject(id, datatype, cancellationToken);
                updatedetail = resulttuple.Item2;

                var deleteResult = GenericResultsHelper.GetSuccessUpdateResult(resulttuple.Item1, source, operation, updatetype, "Delete Raven succeeded", otherinfo, updatedetail, true);

                return Ok(deleteResult);
            }
            catch (Exception ex)
            {
                var errorResult = GenericResultsHelper.GetErrorUpdateResult(id, source, operation, updatetype, "Delete Raven failed", otherinfo, updatedetail, ex, true);

                return BadRequest(errorResult);
            }
        }

        #endregion

        #region REPROCESS PUSH FAILURE QUEUE
        [Authorize(Roles = "DataPush")]
        [HttpGet, Route("PushFailureQueue/Retry/{publishedon}")]
        public async Task<IActionResult> ElaborateFailureQueue(string publishedon, CancellationToken cancellationToken = default)
        {
            UpdateDetailFailureQueue updatedetail = default(UpdateDetailFailureQueue);
            string operation = "Update Failurequeue";
            string updatetype = "multiple";
            string source = "api";
            string otherinfo = "";

            try
            {
                var resulttuple = await OdhPushnotifier.PushFailureQueueToPublishedonService(new List<string>() { publishedon });

                updatedetail = new UpdateDetailFailureQueue()
                {                   
                    pushchannels = resulttuple.Keys,
                    pushed = resulttuple,                 
                };

                var updateResult = GenericResultsHelper.GetSuccessUpdateResult("", source, operation, updatetype, "Elaborate Failurequeue succeeded", otherinfo, updatedetail, true);

                return Ok(updateResult);
            }
            catch (Exception ex)
            {                
                var errorResult = GenericResultsHelper.GetErrorUpdateResult("", source, operation, updatetype, "Elaborate Failurequeue failed", otherinfo, updatedetail, ex, true);
     
                return BadRequest(errorResult);
            }
        }

        #endregion

        #region CUSTOM PUSH SEND

        [Authorize(Roles = "DataPush")]
        [HttpPost, Route("CustomDataPush/{odhtype}/{publishedon}")]
        public async Task<IActionResult> CustomDataPush(string odhtype, string publishedon, [FromBody] List<string> idlist)
        {
            UpdateDetailFailureQueue updatedetail = default(UpdateDetailFailureQueue);
            string operation = "Update Custom";
            string updatetype = "multiple";
            string source = "api";
            string otherinfo = odhtype;

            try
            {
                var resulttuple = await OdhPushnotifier.PushCustomObjectsToPublishedonService(new List<string>() { publishedon }, idlist, odhtype);

                updatedetail = new UpdateDetailFailureQueue()
                {
                    pushchannels = resulttuple.Keys,
                    pushed = resulttuple,
                };

                var updateResult = GenericResultsHelper.GetSuccessUpdateResult("", source, operation, updatetype, "Custom Update succeeded", otherinfo, updatedetail, true);

                return Ok(updateResult);
            }
            catch (Exception ex)
            {
                var errorResult = GenericResultsHelper.GetErrorUpdateResult("", source, operation, updatetype, "Custom Update failed", otherinfo, updatedetail, ex, true);

                return BadRequest(errorResult);
            }
        }


        #endregion

        #region EBMS DATA SYNC (EventShort)

        [HttpGet, Route("EBMS/EventShort/UpdateAll")]
        public async Task<IActionResult> UpdateAllEBMS(CancellationToken cancellationToken = default)
        {
            UpdateDetail updatedetail = default(UpdateDetail);
            string operation = "Update EBMS";
            string updatetype = "all";
            string source = "ebms";

            try
            {
                EbmsEventsImportHelper ebmsimporthelper = new EbmsEventsImportHelper(settings, QueryFactory, "eventeuracnoi", UrlGeneratorStatic("EBMS/EventShort"));

                updatedetail = await ebmsimporthelper.SaveDataToODH(null, null, cancellationToken);
                var updateResult = GenericResultsHelper.GetSuccessUpdateResult(null, source, operation, updatetype, "EBMS Eventshorts update succeeded", "", updatedetail, true);

                return Ok(updateResult);
            }
            catch (Exception ex)
            {
                var updateResult = GenericResultsHelper.GetErrorUpdateResult(null, source, operation, updatetype, "EBMS Eventshorts update failed", "", updatedetail, ex, true);
                return BadRequest(updateResult);
            }
        }

        [HttpGet, Route("EBMS/EventShort/UpdateSingle/{id}")]
        public IActionResult UpdateSingleEBMS(string id, CancellationToken cancellationToken = default)
        {
            return StatusCode(StatusCodes.Status501NotImplemented, new { error = "Not Implemented" });

            //try
            //{               
            //    return Ok(new UpdateResult
            //    {
            //        operation = "Update EBMS",
            //        id = id,
            //        updatetype = "single",
            //        otherinfo = "",
            //        message = "EBMS Eventshorts update succeeded",
            //        recordsmodified = 1,
            //        created = 0,
            //        updated = 0,
            //        deleted = 0,
            //        success = true
            //    });
            //}
            //catch (Exception ex)
            //{
            //    return BadRequest(new UpdateResult
            //    {
            //        operation = "Update EBMS",
            //        updatetype = "all",
            //        otherinfo = "",
            //        message = "EBMS Eventshorts update failed: " + ex.Message,
            //        recordsmodified = 0,
            //        created = 0,
            //        updated = 0,
            //        deleted = 0,
            //        success = false
            //    });
            //}
        }

        #endregion

        #region NINJA DATA SYNC (Events Centro Trevi and DRIN)

        [HttpGet, Route("NINJA/Events/UpdateAll")]
        public async Task<IActionResult> UpdateAllNinjaEvents(CancellationToken cancellationToken = default)
        {
            UpdateDetail updatedetail = default(UpdateDetail);
            string operation = "Update Ninja Events";
            string updatetype = "all";
            string source = "mobilityapi";

            try
            {
                MobilityEventsImportHelper ninjaimporthelper = new MobilityEventsImportHelper(settings, QueryFactory, "events", UrlGeneratorStatic("NINJA/Events"));
                updatedetail = await ninjaimporthelper.SaveDataToODH(null, null, cancellationToken);
                var updateResult = GenericResultsHelper.GetSuccessUpdateResult(null, source, operation, updatetype, "Ninja Events update succeeded", "", updatedetail, true);

                return Ok(updateResult);
            }
            catch (Exception ex)
            {
                var errorResult = GenericResultsHelper.GetErrorUpdateResult(null, source, operation, updatetype, "Ninja Events update failed", "", updatedetail, ex, true);
                return BadRequest(errorResult);
            }
        }

        [HttpGet, Route("NINJA/Events/UpdateSingle/{id}")]
        public IActionResult UpdateSingleNinjaEvents(string id, CancellationToken cancellationToken = default)
        {
            return StatusCode(StatusCodes.Status501NotImplemented, new { error = "Not Implemented" });
        }

        #endregion        

        #region SIAG DATA SYNC WEATHER 

        [HttpGet, Route("Siag/Weather/Import")]
        public async Task<IActionResult> ImportWeather(CancellationToken cancellationToken = default)
        {
            UpdateDetail updatedetail = default(UpdateDetail);
            string operation = "Import Weather data";
            string updatetype = "all";
            string source = "siag";
            string otherinfo = "actual";

            try
            {
                SiagWeatherImportHelper siagimporthelper = new SiagWeatherImportHelper(settings, QueryFactory, "weatherdatahistory", UrlGeneratorStatic("Siag/Weather"));
                updatedetail = await siagimporthelper.SaveWeatherToHistoryTable(cancellationToken);
                var updateResult = GenericResultsHelper.GetSuccessUpdateResult(null, source, operation, updatetype, "Import Weather data succeeded", otherinfo, updatedetail, true);

                return Ok(updateResult);
            }
            catch (Exception ex)
            {
                var errorResult = GenericResultsHelper.GetErrorUpdateResult(null, source, operation, updatetype, "Import Weather data failed", otherinfo, updatedetail, ex, true);
                return BadRequest(errorResult);
            }
        }

        [HttpGet, Route("Siag/Weather/Import/{id}")]
        public async Task<IActionResult> ImportWeatherByID(string id, CancellationToken cancellationToken = default)
        {
            UpdateDetail updatedetail = default(UpdateDetail);
            string operation = "Import Weather data";
            string updatetype = "single";
            string source = "siag";
            string otherinfo = "byid";

            try
            {
                SiagWeatherImportHelper siagimporthelper = new SiagWeatherImportHelper(settings, QueryFactory, "weatherdatahistory", UrlGeneratorStatic("Siag/Weather"));
                updatedetail = await siagimporthelper.SaveWeatherToHistoryTable(cancellationToken, id);
                var updateResult = GenericResultsHelper.GetSuccessUpdateResult(id, source, operation, updatetype, "Import Weather data succeeded id:" + id.ToString(), otherinfo, updatedetail, true);

                return Ok(updateResult);
            }
            catch (Exception ex)
            {
                var updateResult = GenericResultsHelper.GetErrorUpdateResult(null, source, operation, updatetype, "Import Weather data failed id:" + id.ToString(), otherinfo, updatedetail, ex, true);
                return BadRequest(updateResult);
            }
        }

        #endregion

        #region SIAG DATA SYNC MUSEUMS

        [HttpGet, Route("Siag/Museum/UpdateAll")]
        public async Task<IActionResult> ImportSiagMuseum(CancellationToken cancellationToken = default)
        {
            UpdateDetail updatedetail = default(UpdateDetail);
            string operation = "Import SIAG Museum data";
            string updatetype = "all";
            string source = "siag";
            string otherinfo = "actual";

            try
            {
                SiagMuseumImportHelper siagimporthelper = new SiagMuseumImportHelper(settings, QueryFactory, "smgpois", UrlGeneratorStatic("Siag/Museum"));
                updatedetail = await siagimporthelper.SaveDataToODH(null, null, cancellationToken);

                var updateResult = GenericResultsHelper.GetSuccessUpdateResult(null, source, operation, updatetype, "Import SIAG Museum data succeeded", otherinfo, updatedetail, true);

                return Ok(updateResult);
            }
            catch (Exception ex)
            {
                var updateResult = GenericResultsHelper.GetErrorUpdateResult(null, source, operation, updatetype, "Import SIAG Museum data failed", otherinfo, updatedetail, ex, true);
                return BadRequest(updateResult);
            }
        }

        #endregion

        #region SUEDTIROLWEIN DATA SYNC

        [HttpGet, Route("SuedtirolWein/Company/UpdateAll")]
        public async Task<IActionResult> ImportSuedtirolWineCompany(CancellationToken cancellationToken = default)
        {
            UpdateDetail updatedetail = default(UpdateDetail);
            string operation = "Import SuedtirolWein Company data";
            string updatetype = "all";
            string source = "suedtirolwein";
            string otherinfo = "actual";

            try
            {
                SuedtirolWeinCompanyImportHelper sweinimporthelper = new SuedtirolWeinCompanyImportHelper(settings, QueryFactory, "smgpois", UrlGeneratorStatic("SuedtirolWein/Company"));
                updatedetail = await sweinimporthelper.SaveDataToODH(null, null, cancellationToken);

                var updateResult = GenericResultsHelper.GetSuccessUpdateResult(null, source, operation, updatetype, "Import SuedtirolWein Company data succeeded", otherinfo, updatedetail, true);

                return Ok(updateResult);
            }
            catch (Exception ex)
            {
                var updateResult = GenericResultsHelper.GetErrorUpdateResult(null, source, operation, updatetype, "Import SuedtirolWein Company data failed", otherinfo, updatedetail, ex, true);
                return BadRequest(updateResult);
            }
        }

        [HttpGet, Route("SuedtirolWein/WineAward/UpdateAll")]
        public async Task<IActionResult> ImportSuedtirolWineAward(CancellationToken cancellationToken = default)
        {
            UpdateDetail updatedetail = default(UpdateDetail);
            string operation = "Import SuedtirolWein WineAward data";
            string updatetype = "all";
            string source = "suedtirolwein";
            string otherinfo = "actual";

            try
            {
                SuedtirolWeinAwardImportHelper sweinimporthelper = new SuedtirolWeinAwardImportHelper(settings, QueryFactory, "wines", UrlGeneratorStatic("SuedtirolWein/WineAward"));
                updatedetail = await sweinimporthelper.SaveDataToODH(null, null, cancellationToken);

                var updateResult = GenericResultsHelper.GetSuccessUpdateResult(null, source, operation, updatetype, "Import SuedtirolWein WineAward data succeeded", otherinfo, updatedetail, true);

                return Ok(updateResult);
            }
            catch (Exception ex)
            {
                var updateResult = GenericResultsHelper.GetErrorUpdateResult(null, source, operation, updatetype, "Import SuedtirolWein WineAward data failed", otherinfo, updatedetail, ex, true);
                return BadRequest(updateResult);
            }
        }

        #endregion

        #region LTS ACTIVITYDATA SYNC

        #endregion

        #region LTS POIDATA SYNC

        #endregion

        #region LTS EVENT DATA SYNC

        #endregion

        #region LTS GASTRONOMIC DATA SYNC

        [HttpGet, Route("LTS/GastronomicData/UpdateAll")]
        public async Task<IActionResult> ImportLTSGastronomies(bool onlychanged, CancellationToken cancellationToken = default)
        {
            UpdateDetail updatedetail = default(UpdateDetail);
            string operation = "Import LTS GastronomicData data";
            string updatetype = "all";
            string source = "lts";
            string otherinfo = "gastronomy";

            try
            {
                
                var updateResult = GenericResultsHelper.GetSuccessUpdateResult(null, source, operation, updatetype, "Import LTS GastronomicData data succeeded", otherinfo, updatedetail, true);

                return Ok(updateResult);
            }
            catch (Exception ex)
            {
                var updateResult = GenericResultsHelper.GetErrorUpdateResult(null, source, operation, updatetype, "Import LTS GastronomicData data failed", otherinfo, updatedetail, ex, true);
                return BadRequest(updateResult);
            }
        }


        #endregion

        #region LTS ACCOMMODATION DATA SYNC

        #endregion

        #region HGV ACCOMMODATION DATA SYNC

        #endregion

        #region LTS MEASURINGPOINTS DATA SYNC

        #endregion

        #region LTS WEBCAM DATA SYNC

        #endregion

        #region STA POI DATA SYNC

        //[Authorize(Roles = "DataWriter,STAPoiImport")]
        [HttpPost, Route("STA/VendingPoints/UpdateAll")]
        public async Task<IActionResult> SendVendingPointsFromSTA(CancellationToken cancellationToken)
        {
            UpdateDetail updatedetail = default(UpdateDetail);
            string operation = "Import Vendingpoints";
            string updatetype = "all";
            string source = "xls";
            string otherinfo = "STA";

            try
            {
                StaVendingpointsImportHelper staimporthelper = new StaVendingpointsImportHelper(settings, QueryFactory, UrlGeneratorStatic("STA/Vendingpoints"));

                updatedetail = await staimporthelper.PostVendingPointsFromSTA(Request, cancellationToken);

                var updateResult = GenericResultsHelper.GetSuccessUpdateResult("", source, operation, updatetype, "Import Vendingpoints succeeded", otherinfo, updatedetail, true);

                return Ok(updateResult);
            }
            catch (Exception ex)
            {
                var errorResult = GenericResultsHelper.GetErrorUpdateResult("", source, operation, updatetype, "Import Vendingpoints failed", otherinfo, updatedetail, ex, true);

                return BadRequest(errorResult);
            }
        }

        #endregion

        #region DSS DATA SYNC

        [HttpGet, Route("DSS/{dssentity}/UpdateAll")]
        public async Task<IActionResult> UpdateAllDSSData(string dssentity, CancellationToken cancellationToken = default)
        {
            UpdateDetail updatedetail = default(UpdateDetail);
            string operation = "Update DSS " + dssentity;
            string updatetype = "all";
            string source = "dss";
            string otherinfo = "actual";

            try
            {
                switch(dssentity)
                {
                    case "webcam":
                   
                        DSSWebcamImportHelper dsswebcamimporthelper = new DSSWebcamImportHelper(settings, QueryFactory, "webcams", UrlGeneratorStatic("DSS/Webcam"));                        

                        updatedetail = await dsswebcamimporthelper.SaveDataToODH(null, null, cancellationToken);
                        break;
                   
                    default:
                        DSSImportHelper dssimporthelper = new DSSImportHelper(settings, QueryFactory, "smgpois", UrlGeneratorStatic("DSS/" + dssentity));
                        dssimporthelper.entitytype = dssentity;

                       updatedetail = await dssimporthelper.SaveDataToODH(null, null, cancellationToken);
                        break;
                }

                var updateResult = GenericResultsHelper.GetSuccessUpdateResult(null, source, operation, updatetype, "DSS " + dssentity + " update succeeded", otherinfo, updatedetail, true);

                return Ok(updateResult);

            }
            catch (Exception ex)
            {
                var errorResult = GenericResultsHelper.GetErrorUpdateResult(null, source, operation, updatetype, "DSS " + dssentity + " update failed", "", updatedetail, ex, true);

                return BadRequest(errorResult);
            }
        }

        [HttpGet, Route("DSS/{dssentity}/UpdateSingle/{id}")]
        public IActionResult UpdateSingleDSS(string dssentity, string id, CancellationToken cancellationToken = default)
        {
            return StatusCode(StatusCodes.Status501NotImplemented, new { error = "Not Implemented" });

            //try
            //{               
            //    return Ok(new UpdateResult
            //    {
            //        operation = "Update EBMS",
            //        id = id,
            //        updatetype = "single",
            //        otherinfo = "",
            //        message = "EBMS Eventshorts update succeeded",
            //        recordsmodified = 1,
            //        created = 0,
            //        updated = 0,
            //        deleted = 0,
            //        success = true
            //    });
            //}
            //catch (Exception ex)
            //{
            //    return BadRequest(new UpdateResult
            //    {
            //        operation = "Update EBMS",
            //        updatetype = "all",
            //        otherinfo = "",
            //        message = "EBMS Eventshorts update failed: " + ex.Message,
            //        recordsmodified = 0,
            //        created = 0,
            //        updated = 0,
            //        deleted = 0,
            //        success = false
            //    });
            //}
        }

        #endregion

        #region EJOBS DATA SYNC

        [HttpGet, Route("LOOPTEC/Ejobs/UpdateAll")]
        public async Task<IActionResult> UpdateAllLooptecEjobs(CancellationToken cancellationToken = default)
        {
            UpdateDetail updatedetail = default(UpdateDetail);
            string operation = "Import Looptec Ejobs";
            string updatetype = "all";
            string source = "looptec";
            string otherinfo = "rawonly";

            try
            {
                LooptecEjobsImportHelper looptecejobsimporthelper = new LooptecEjobsImportHelper(settings, QueryFactory, "", UrlGeneratorStatic("LOOPTEC/Ejobs"));

                updatedetail = await looptecejobsimporthelper.SaveDataToODH(null, null, cancellationToken);
                var updateResult = GenericResultsHelper.GetSuccessUpdateResult(null, source, operation, updatetype, "Import Looptec Ejobs succeeded", otherinfo, updatedetail, true);

                return Ok(updateResult);
            }
            catch (Exception ex)
            {
                var updateResult = GenericResultsHelper.GetErrorUpdateResult(null, source, operation, updatetype, "Import Looptec Ejobs failed", otherinfo, updatedetail, ex, true);
                return BadRequest(updateResult);
            }
        }


        #endregion


        protected Func<string, string> UrlGeneratorStatic
        {
            get
            {
                return self =>
                {
                    var location = new Uri($"{Request.Scheme}://{Request.Host}/" + self);
                    return location.AbsoluteUri;
                };
            }
        }
    }
}
