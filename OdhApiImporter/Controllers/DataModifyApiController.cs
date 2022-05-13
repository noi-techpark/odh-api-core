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

namespace OdhApiImporter.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class DataModifyApiController : Controller
    {
        private readonly ISettings settings;
        private readonly QueryFactory QueryFactory;
        private readonly ILogger<UpdateApiController> logger;
        private readonly IWebHostEnvironment env;

        public DataModifyApiController(IWebHostEnvironment env, ISettings settings, ILogger<UpdateApiController> logger, QueryFactory queryFactory)
        {
            this.env = env;
            this.settings = settings;
            this.logger = logger;
            this.QueryFactory = queryFactory;
        }

        #region EventShort

        [HttpGet, Route("ModifyEventShort")]
        public async Task<IActionResult> ModifyEventShort(CancellationToken cancellationToken)
        {
            CustomDataOperation customdataoperation = new CustomDataOperation(settings, QueryFactory);
            var objectscount = await customdataoperation.UpdateAllEventShortstonewDataModel();

            return Ok(new UpdateResult
            {
                operation = "Modify EventShort",
                updatetype = "custom",
                otherinfo = "",
                message = "Done",
                recordsmodified = objectscount,
                created = 0,
                deleted = 0,
                id = "",
                updated = objectscount,
                success = true
            });

            return Ok();
        }

        [HttpGet, Route("ModifySTAVendingpoint")]
        public async Task<IActionResult> ModifySTAVendingpoint(CancellationToken cancellationToken)
        {
            CustomDataOperation customdataoperation = new CustomDataOperation(settings, QueryFactory);
            var objectscount = await customdataoperation.UpdateAllSTAVendingpoints();

            return Ok(new UpdateResult
            {
                operation = "Modify STA Vendingpoint",
                updatetype = "custom",
                otherinfo = "",
                message = "Done",
                recordsmodified = objectscount,
                created = 0,
                deleted = 0,
                id = "",
                updated = objectscount,
                success = true
            });

            return Ok();
        }

        #endregion

        #region Articles

        [HttpGet, Route("FillDummyNews")]
        public async Task<IActionResult> FillDBWithDummynews(CancellationToken cancellationToken)
        {
            CustomDataOperation customdataoperation = new CustomDataOperation(settings, QueryFactory);
            var objectscount = await customdataoperation.FillDBWithDummyNews();

            return Ok(new UpdateResult
            {
                operation = "Modify Articles",
                updatetype = "custom",
                otherinfo = "",
                message = "Done",
                recordsmodified = objectscount,
                created = 0,
                deleted = 0,
                id = "",
                updated = objectscount,
                success = true
            });

            return Ok();
        }

        #endregion


    }
}
