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
//using OdhApiCore.Filters;
//using OdhApiCore.GenericHelpers;
using EBMS;
using NINJA;
using NINJA.Parser;
using System.Net.Http;
using RAVEN;
using Microsoft.Extensions.Hosting;
using OdhApiImporter.Helpers.EBMS;
using OdhApiImporter.Helpers.NINJA;
using OdhApiImporter.Helpers.RAVEN;
using OdhApiImporter.Models;

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

        #region EBMS exposed

        [HttpGet, Route("EBMS/EventShort/UpdateAll")]
        public async Task<IActionResult> UpdateAllEBMS(CancellationToken cancellationToken)
        {
            try
            {
                EBMSImportHelper ebmsimporthelper = new EBMSImportHelper(settings, QueryFactory);

                var result = await ebmsimporthelper.ImportEbmsEventsToDB();

                return Ok(new UpdateResult
                {
                    operation = "Update EBMS",
                    updatetype = "all",
                    message = "EBMS Eventshorts update succeeded",
                    recordsupdated = result,
                    success = true
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new UpdateResult
                {
                    operation = "Update EBMS",
                    updatetype = "all",
                    message = "EBMS Eventshorts update failed: " + ex.Message,
                    recordsupdated = "0",
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
                    message = "EBMS Eventshorts update succeeded",
                    recordsupdated = "1",
                    success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new UpdateResult
                {
                    operation = "Update EBMS",
                    updatetype = "all",
                    message = "EBMS Eventshorts update failed: " + ex.Message,
                    recordsupdated = "0",
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

                NINJAImportHelper ninjaimporthelper = new NINJAImportHelper(settings, QueryFactory);
                var result = await ninjaimporthelper.SaveEventsToPG(responseevents.data, responseplaces.data);

                return Ok(new UpdateResult
                {
                    operation = "Update Ninja Events",
                    updatetype = "all",
                    message = "Ninja Events update succeeded",
                    recordsupdated = result,
                    success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new UpdateResult
                {
                    operation = "Update Ninja Events",
                    updatetype = "all",
                    message = "Update Ninja Events failed: " + ex.Message,
                    recordsupdated = "0",
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
                RAVENImportHelper ravenimporthelper = new RAVENImportHelper(settings, QueryFactory);
                var result = await ravenimporthelper.GetFromRavenAndTransformToPGObject(id, datatype, cancellationToken);

                return Ok(new UpdateResult
                {
                    operation = "Update Raven",
                    updatetype = "single",
                    message = "",
                    recordsupdated = "1",
                    success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new UpdateResult
                {
                    operation = "Update Raven",
                    updatetype = "all",
                    message = "Update Raven failed: " + ex.Message,
                    recordsupdated = "0",
                    success = false
                });
            } 
        }

        #endregion
    }
}
