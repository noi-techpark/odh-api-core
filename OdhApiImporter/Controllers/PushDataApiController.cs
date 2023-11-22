// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

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
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using EBMS;
using NINJA;
using NINJA.Parser;
using System.Net.Http;
using RAVEN;
using Microsoft.Extensions.Hosting;
using OdhApiImporter.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using OdhNotifier;

namespace OdhApiImporter.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class PushDataApiController : Controller
    {
        private readonly ISettings settings;
        private readonly QueryFactory QueryFactory;
        private readonly ILogger<UpdateApiController> logger;
        private readonly IWebHostEnvironment env;
        private IOdhPushNotifier OdhPushnotifier;

        public PushDataApiController(IWebHostEnvironment env, ISettings settings, ILogger<UpdateApiController> logger, QueryFactory queryFactory, IOdhPushNotifier odhpushnotifier)
        {
            this.env = env;
            this.settings = settings;
            this.logger = logger;
            this.QueryFactory = queryFactory;
            this.OdhPushnotifier = odhpushnotifier;
        }

        #region CustomPush

        [Authorize(Roles = "DataPush")]
        [HttpGet, Route("PushODHActivityPoisByTag/{tags}")]
        public async Task<IActionResult> PushODHActivityPoisByTag(string tags, CancellationToken cancellationToken)
        {
            List<string> taglist = tags.Split(',').ToList();

            PushDataOperation customdataoperation = new PushDataOperation(settings, QueryFactory, OdhPushnotifier);
            var results = await customdataoperation.PushAllODHActivityPoiwithTags(taglist);

            List<UpdateResult> updates = new List<UpdateResult>();
            foreach (var result in results) {
                updates.Add(
                    new UpdateResult
                    {
                        operation = "Push ODHActivityPoi by Tag " + String.Join(",", taglist),
                        updatetype = "custom",
                        otherinfo = "",
                        message = "Done",
                        recordsmodified = 0,
                        created = 0,
                        deleted = 0,
                        id = "",
                        updated = 0,
                        success = true,
                        pushed = result
                    });
            }

            return Ok(updates);
        }

        #endregion
    }
}