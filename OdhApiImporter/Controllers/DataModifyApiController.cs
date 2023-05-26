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

        [Authorize(Roles = "DataPush")]
        [HttpGet, Route("ModifyEventShort")]
        public async Task<IActionResult> ModifyEventShort(CancellationToken cancellationToken)
        {
            var objectscount = 0;

            CustomDataOperation customdataoperation = new CustomDataOperation(settings, QueryFactory);
            //objectscount = await customdataoperation.UpdateAllEventShortstActiveTodayField();
            //objectscount = await customdataoperation.UpdateAllEventShortBrokenLinks();

            //objectscount = await customdataoperation.UpdateAllEventShortPublisherInfo();                        

            objectscount = await customdataoperation.UpdateAllEventShortstEventDocumentField();

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
                updated = 0,
                success = true
            });
        }

        [Authorize(Roles = "DataPush")]
        [HttpGet, Route("CleanEventShort")]
        public async Task<IActionResult> CleanEventShort(CancellationToken cancellationToken)
        {
            var objectscount = 0;

            CustomDataOperation customdataoperation = new CustomDataOperation(settings, QueryFactory);
            //objectscount = await customdataoperation.UpdateAllEventShortstActiveTodayField();
            //objectscount = await customdataoperation.UpdateAllEventShortBrokenLinks();

            //objectscount = await customdataoperation.UpdateAllEventShortPublisherInfo();                        

            objectscount = await customdataoperation.CleanEventShortstEventDocumentField();

            return Ok(new UpdateResult
            {
                operation = "Clean EventShort",
                updatetype = "custom",
                otherinfo = "",
                message = "Done",
                recordsmodified = objectscount,
                created = 0,
                deleted = 0,
                id = "",
                updated = 0,
                success = true
            });
        }

        #endregion

        #region Accommodations

        [Authorize(Roles = "DataPush")]
        [HttpPost, Route("ModifyAccommodations")]
        public async Task<IActionResult> ModifyAccommodations(bool trim, [FromBody] List<string> idlist)
        {
            var objectscount = 0;

            CustomDataOperation customdataoperation = new CustomDataOperation(settings, QueryFactory);
          
            objectscount = await customdataoperation.AccommodationModify(idlist, trim);

            return Ok(new UpdateResult
            {
                operation = "Modify Accommodation",
                updatetype = "custom",
                otherinfo = "",
                message = "Done",
                recordsmodified = objectscount,
                created = 0,
                deleted = 0,
                id = "",
                updated = 0,
                success = true
            });
        }

        #endregion

        #region Articles

        [Authorize(Roles = "DataPush")]
        [HttpGet, Route("FillDummyNews")]
        public async Task<IActionResult> FillDBWithDummynews(CancellationToken cancellationToken)
        {
            //CustomDataOperation customdataoperation = new CustomDataOperation(settings, QueryFactory);
            //var objectscount = await customdataoperation.FillDBWithDummyNews();

            return Ok(new UpdateResult
            {
                operation = "Modify Articles",
                updatetype = "custom",
                otherinfo = "",
                message = "Done",
                recordsmodified = 0,
                created = 0,
                deleted = 0,
                id = "",
                updated = 0,
                success = true
            });
        }

        #endregion

        #region Weather

        [Authorize(Roles = "DataPush")]
        [HttpGet, Route("ModifyWeatherHistory")]
        public async Task<IActionResult> ModifyWeatherHistory(CancellationToken cancellationToken)
        {
            //CustomDataOperation customdataoperation = new CustomDataOperation(settings, QueryFactory);
            //var objectscount = await customdataoperation.UpdateAllWeatherHistoryWithMetainfo();

            return Ok(new UpdateResult
            {
                operation = "Modify WeatherHistory",
                updatetype = "custom",
                otherinfo = "",
                message = "Done",
                recordsmodified = 0,
                created = 0,
                deleted = 0,
                id = "",
                updated = 0,
                success = true
            });
        }

        #endregion

        #region ODHActivityPoi

        [Authorize(Roles = "DataPush")]
        [HttpGet, Route("ModifyOldODHActivityPoi")]
        public async Task<IActionResult> ModifyODHActivityPoiTags(CancellationToken cancellationToken)
        {
            //CustomDataOperation customdataoperation = new CustomDataOperation(settings, QueryFactory);
            //var objectscount = await customdataoperation.UpdateAllODHActivityPoiOldTags("sta");
            //var objectscount2 = await customdataoperation.UpdateAllODHActivityPoiOldTags("dss");


            return Ok(new UpdateResult
            {
                operation = "Modify ODHActivityPoi",
                updatetype = "custom",
                otherinfo = "",
                message = "Done",
                recordsmodified = 0,
                created = 0,
                deleted = 0,
                id = "",
                updated = 0,
                success = true
            });
        }

        [Authorize(Roles = "DataPush")]
        [HttpGet, Route("ModifySTAVendingpoint")]
        public async Task<IActionResult> ModifySTAVendingpoint(CancellationToken cancellationToken)
        {
            //CustomDataOperation customdataoperation = new CustomDataOperation(settings, QueryFactory);
            //var objectscount = await customdataoperation.UpdateAllSTAVendingpoints();

            return Ok(new UpdateResult
            {
                operation = "Modify STA Vendingpoint",
                updatetype = "custom",
                otherinfo = "",
                message = "Done",
                recordsmodified = 0,
                created = 0,
                deleted = 0,
                id = "",
                updated = 0,
                success = true
            });
        }

        #endregion

        #region MetaData

        [Authorize(Roles = "DataPush")]
        [HttpGet, Route("UpdateMetaDataRecordCount")]
        public async Task<IActionResult> UpdateMetaDataRecordCount(CancellationToken cancellationToken)
        {
            CustomDataOperation customdataoperation = new CustomDataOperation(settings, QueryFactory);
            var objectscount = await customdataoperation.UpdateMetaDataApiRecordCount();            

            return Ok(new UpdateResult
            {
                operation = "Modify Metadata Record Count",
                updatetype = "custom",
                otherinfo = "",
                message = "Done",
                recordsmodified = 0,
                created = 0,
                deleted = 0,
                id = "",
                updated = 0,
                success = true
            });
        }

        //[Authorize(Roles = "DataPush")]
        [HttpGet, Route("UpdateMetaDataApiId")]
        public async Task<IActionResult> UpdateMetaDataApiId(CancellationToken cancellationToken)
        {
            CustomDataOperation customdataoperation = new CustomDataOperation(settings, QueryFactory);
            var objectscount = await customdataoperation.UpdateMetaDataApiId();

            return Ok(new UpdateResult
            {
                operation = "Modify Metadata ApiId",
                updatetype = "custom",
                otherinfo = "",
                message = "Done",
                recordsmodified = 0,
                created = objectscount["created"],
                deleted = objectscount["deleted"],
                id = "",
                updated = 0,
                success = true
            });
        }

        #endregion
    }
}