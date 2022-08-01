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

        public UpdateApiController(IWebHostEnvironment env, ISettings settings, ILogger<UpdateApiController> logger, QueryFactory queryFactory)
        {
            this.env = env;
            this.settings = settings;
            this.logger = logger;
            this.QueryFactory = queryFactory;
        }        

        #region UPDATE FROM RAVEN INSTANCE

        [HttpGet, Route("Raven/{datatype}/Update/{id}")]
        //[Authorize(Roles = "DataWriter,DataCreate,DataUpdate")]
        public async Task<IActionResult> UpdateFromRaven(string id, string datatype, CancellationToken cancellationToken = default)
        {
            try
            {
                RAVENImportHelper ravenimporthelper = new RAVENImportHelper(settings, QueryFactory);
                var resulttuple = await ravenimporthelper.GetFromRavenAndTransformToPGObject(id, datatype, cancellationToken);
                var result = resulttuple.Item2;


                var updateResult = GenericResultsHelper.GetSuccessUpdateResult(resulttuple.Item1, "service.suedtirol.info", "Update Raven", "single", "", datatype, result, true);

                return Ok(updateResult);
            }
            catch (Exception ex)
            {
                var errorResult = GenericResultsHelper.GetErrorUpdateResult(id, "service.suedtirol.info", "Update Raven", "single", "Update Raven failed: ", datatype, new UpdateDetail(), ex, true);

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
                EBMSImportHelper ebmsimporthelper = new EBMSImportHelper(settings, QueryFactory, "eventeuracnoi");

                updatedetail = await ebmsimporthelper.SaveDataToODH(null, cancellationToken);
                var updateResult = GenericResultsHelper.GetSuccessUpdateResult(null, source, operation, updatetype, "EBMS Eventshorts update succeeded", "", updatedetail, true);

                return Ok(updateResult);
            }
            catch(Exception ex)
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
                NINJAImportHelper ninjaimporthelper = new NINJAImportHelper(settings, QueryFactory);
                updatedetail = await ninjaimporthelper.SaveDataToODH(null, cancellationToken);
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
        public async Task<IActionResult> UpdateSingleNinjaEvents(string id, CancellationToken cancellationToken = default)
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

            try
            {
                SIAGImportHelper siagimporthelper = new SIAGImportHelper(settings, QueryFactory, "weatherdatahistory");
                updatedetail = await siagimporthelper.SaveWeatherToHistoryTable(cancellationToken);
                var updateResult = GenericResultsHelper.GetSuccessUpdateResult(null, source, operation, updatetype, "Import Weather data succeeded", "actual", updatedetail, true);

                return Ok(updateResult);     
            }
            catch (Exception ex)
            {
                var errorResult = GenericResultsHelper.GetErrorUpdateResult(null, source, operation, updatetype, "Import Weather data failed", "actual", updatedetail, ex, true);
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

            try
            {
                SIAGImportHelper siagimporthelper = new SIAGImportHelper(settings, QueryFactory, "weatherdatahistory");
                updatedetail = await siagimporthelper.SaveWeatherToHistoryTable(cancellationToken, id);
                var updateResult = GenericResultsHelper.GetSuccessUpdateResult(id, source, operation, updatetype, "Import Weather data succeeded id:" + id.ToString(), "byid", updatedetail, true);

                return Ok(updateResult);                
            }
            catch (Exception ex)
            {
                var updateResult = GenericResultsHelper.GetErrorUpdateResult(null, source, operation, updatetype, "Import Weather data failed id:" + id.ToString(), "byid", updatedetail, ex, true);
                return BadRequest(updateResult);
            }
        }

        #endregion

        #region SIAG DATA SYNC MUSEUMS

        [HttpGet, Route("Siag/Museum/UpdateAll")]
        public async Task<IActionResult> ImportMuseum(CancellationToken cancellationToken = default)
        {
            UpdateDetail updatedetail = default(UpdateDetail);
            string operation = "Import SIAG Museum data";
            string updatetype = "all";
            string source = "siag";

            try
            {
                SIAGImportHelper siagimporthelper = new SIAGImportHelper(settings, QueryFactory, "smgpois");
                updatedetail = await siagimporthelper.SaveDataToODH(null, cancellationToken);

                var updateResult = GenericResultsHelper.GetSuccessUpdateResult(null, source, operation, updatetype, "Import SIAG Museum data succeeded", "actual", updatedetail, true);

                return Ok(updateResult);
            }
            catch (Exception ex)
            {
                var updateResult = GenericResultsHelper.GetErrorUpdateResult(null, source, operation, updatetype, "Import SIAG Museum data failed", "actual", updatedetail, ex, true);
                return BadRequest(updateResult);                
            }
        }

        #endregion

        #region SUEDTIROLWEIN DATA SYNC

        #endregion

        #region LTS ACTIVITYDATA SYNC

        #endregion

        #region LTS POIDATA SYNC

        #endregion

        #region LTS EVENT DATA SYNC

        #endregion

        #region LTS GASTRONOMIC DATA SYNC

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
            try
            {
                STAImportHelper staimporthelper = new STAImportHelper(settings, QueryFactory);

                var result = await staimporthelper.PostVendingPointsFromSTA(Request, cancellationToken);

                return Ok(new
                {
                    operation = "Import Vendingpoints",
                    updatetype = "all",
                    otherinfo = "STA",
                    id = "",
                    message = "Import Vendingpoints succeeded",
                    recordsmodified = (result.created + result.updated + result.deleted),
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
                    operation = "Import Vendingpoints",
                    updatetype = "all",
                    otherinfo = "STA",
                    id = "",
                    message = "Import Vendingpoints failed: " + ex.Message,
                    recordsmodified = 0,
                    created = 0,
                    updated = 0,
                    deleted = 0,
                    success = false
                });
            }
        }

        #endregion

        #region DSS DATA SYNC

        [HttpGet, Route("DSS/{dssentity}/UpdateAll")]
        public async Task<IActionResult> UpdateAllDSSLifts(string dssentity, CancellationToken cancellationToken = default)
        {
            UpdateDetail updatedetail = default(UpdateDetail);
            string operation = "Update DSS " + dssentity;
            string updatetype = "all";
            string source = "dss";            

            try
            {
                DSSImportHelper dssimporthelper = new DSSImportHelper(settings, QueryFactory, "smgpois");                
                dssimporthelper.entitytype = dssentity;
                
                updatedetail = await dssimporthelper.SaveDataToODH(null, cancellationToken);

                var updateResult = GenericResultsHelper.GetSuccessUpdateResult(null, source, operation, updatetype, "DSS " + dssentity + " update succeeded", "", updatedetail, true);

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

    }
}
