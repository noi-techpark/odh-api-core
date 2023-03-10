using DataModel;
using System;
using System.Threading;
using System.Threading.Tasks;
using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using Newtonsoft.Json;
using OdhApiImporter.Helpers;
using OdhApiImporter.Helpers.DSS;
using OdhApiImporter.Helpers.LOOPTEC;
using OdhApiImporter.Helpers.SuedtirolWein;
using OdhNotifier;

namespace OdhApiImporter.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class NotificationFailureController : Controller
    {
        private readonly ISettings settings;
        private readonly QueryFactory QueryFactory;
        private readonly ILogger<UpdateApiController> logger;
        private readonly IWebHostEnvironment env;
        private IOdhPushNotifier OdhPushnotifier;

        public NotificationFailureController(IWebHostEnvironment env, ISettings settings, ILogger<UpdateApiController> logger, QueryFactory queryFactory, IOdhPushNotifier odhpushnotifier)
        {
            this.env = env;
            this.settings = settings;
            this.logger = logger;
            this.QueryFactory = queryFactory;
            this.OdhPushnotifier = odhpushnotifier;
        }

        [HttpGet, Route("NotificationFailure/Resend/{id}")]
        //[Authorize(Roles = "DataWriter,DataCreate,DataUpdate")]
        public async Task<IActionResult> ResendNotificationFailure(string id, CancellationToken cancellationToken = default)
        {
            UpdateDetail updatedetail = default(UpdateDetail);
            string operation = "Update Raven";
            string updatetype = "single";
            string source = "api";
            string otherinfo = "";

            try
            {
                //LOAD from DB
                //RESEND TO all publishers

                //var updateResult = GenericResultsHelper.GetSuccessUpdateResult(resulttuple.Item1, source, operation, updatetype, "Update Raven succeeded", otherinfo, updatedetail, true);

                //return Ok(updateResult);

                return Ok();
            }
            catch (Exception ex)
            {
                //var errorResult = GenericResultsHelper.GetErrorUpdateResult(id, source, operation, updatetype, "Update Raven failed", otherinfo, updatedetail, ex, true);

                //return BadRequest(errorResult);

                return BadRequest();
            }
        }

    }
}
