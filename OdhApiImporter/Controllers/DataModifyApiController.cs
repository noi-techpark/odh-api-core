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

        public DataModifyApiController(IWebHostEnvironment env, ISettings settings, ILogger<DataModifyApiController> logger, QueryFactory queryFactory)
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

            //objectscount = await customdataoperation.UpdateAllEventShortstEventDocumentField();

            //objectscount = await customdataoperation.UpdateAllEventShortstActiveFieldToTrue();

            objectscount = await customdataoperation.UpdateAllEventShortstHasLanguage();

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

        [Authorize(Roles = "DataPush")]
        [HttpGet, Route("ModifyAccommodationRooms")]
        public async Task<IActionResult> ModifyAccommodationRooms()
        {
            var objectscount = 0;

            CustomDataOperation customdataoperation = new CustomDataOperation(settings, QueryFactory);

            objectscount = await customdataoperation.AccommodationRoomModify();

            return Ok(new UpdateResult
            {
                operation = "Modify Accommodation Array",
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
                recordsmodified = objectscount,
                created = 0,
                deleted = 0,
                id = "",
                updated = 0,
                success = true
            });
        }

        [Authorize(Roles = "DataPush")]
        [HttpGet, Route("ResaveMetaData")]
        public async Task<IActionResult> ResaveMetaData(bool correcturls = false)
        {
            CustomDataOperation customdataoperation = new CustomDataOperation(settings, QueryFactory);
            var objectscount = await customdataoperation.ResaveMetaData(Request.Host.ToString(), correcturls);

            return Ok(new UpdateResult
            {
                operation = "Resave Metadata",
                updatetype = "custom",
                otherinfo = Request.Host.ToString(),
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
        [HttpGet, Route("ResaveTags")]
        public async Task<IActionResult> ResaveTags(bool correcturls = false)
        {
            CustomDataOperation customdataoperation = new CustomDataOperation(settings, QueryFactory);
            var objectscount = await customdataoperation.ResaveTags();

            return Ok(new UpdateResult
            {
                operation = "Resave Tags",
                updatetype = "custom",
                otherinfo = Request.Host.ToString(),
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

        #region Generic

        [Authorize(Roles = "DataPush")]
        [HttpGet, Route("ResaveSourcefield")]
        public async Task<IActionResult> ResaveSource(string odhtype, string sourcetofilter, string sourcetochange)
        {
            CustomDataOperation customdataoperation = new CustomDataOperation(settings, QueryFactory);

            var objectscount = 0;
            
            switch(odhtype)
            {
                case "accommodation": 
                    objectscount = await customdataoperation.ResaveSourcesOnType<AccommodationLinked>(odhtype, sourcetofilter, sourcetochange); ; 
                    break;
                case "accommodationroom":
                    objectscount = await customdataoperation.ResaveSourcesOnType<AccommodationRoomLinked>(odhtype, sourcetofilter, sourcetochange); ;
                    break;
                case "package":
                    objectscount = await customdataoperation.ResaveSourcesOnType<PackageLinked>(odhtype, sourcetofilter, sourcetochange); ;
                    break;
                case "odhactivitypoi":
                    objectscount = await customdataoperation.ResaveSourcesOnType<ODHActivityPoiLinked>(odhtype, sourcetofilter, sourcetochange); ;
                    break;
                case "event":
                    objectscount = await customdataoperation.ResaveSourcesOnType<EventLinked>(odhtype, sourcetofilter, sourcetochange); ;
                    break;
                case "article":
                    objectscount = await customdataoperation.ResaveSourcesOnType<ArticlesLinked>(odhtype, sourcetofilter, sourcetochange); ;
                    break;
                case "ltsactivity":
                    objectscount = await customdataoperation.ResaveSourcesOnType<LTSActivityLinked>(odhtype, sourcetofilter, sourcetochange); ;
                    break;
                case "ltspoi":
                    objectscount = await customdataoperation.ResaveSourcesOnType<LTSPoiLinked>(odhtype, sourcetofilter, sourcetochange); ;
                    break;
                case "ltsgastronomy":
                    objectscount = await customdataoperation.ResaveSourcesOnType<GastronomyLinked>(odhtype, sourcetofilter, sourcetochange); ;
                    break;
                case "webcam":
                    objectscount = await customdataoperation.ResaveSourcesOnType<WebcamInfoLinked>(odhtype, sourcetofilter, sourcetochange); ;
                    break;
                case "wineaward":
                    objectscount = await customdataoperation.ResaveSourcesOnType<WineLinked>(odhtype, sourcetofilter, sourcetochange); ;
                    break;
                case "venue":
                    objectscount = await customdataoperation.ResaveSourcesOnType<VenueLinked>(odhtype, sourcetofilter, sourcetochange); ;
                    break;
                case "eventshort":
                    objectscount = await customdataoperation.ResaveSourcesOnType<EventShortLinked>(odhtype, sourcetofilter, sourcetochange); ;
                    break;
                case "weatherhistory":
                    objectscount = await customdataoperation.ResaveSourcesOnType<WeatherHistoryLinked>(odhtype, sourcetofilter, sourcetochange); ;
                    break;
                case "area":
                    objectscount = await customdataoperation.ResaveSourcesOnType<AreaLinked>(odhtype, sourcetofilter, sourcetochange); ;
                    break;
            }

            return Ok(new UpdateResult
            {
                operation = "Resave Source",
                updatetype = "custom",
                otherinfo = odhtype,
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

        #region ODHTag

        [Authorize(Roles = "DataPush")]
        [HttpGet, Route("ModifyODHTags")]
        public async Task<IActionResult> ModifyODHTags(CancellationToken cancellationToken)
        {
            CustomDataOperation customdataoperation = new CustomDataOperation(settings, QueryFactory);
            var objectscount = await customdataoperation.UpdateAllODHTags();

            return Ok(new UpdateResult
            {
                operation = "Modify ODHTags",
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

        #region Haslanguage

        [Authorize(Roles = "DataPush")]
        [HttpGet, Route("ModifyWines")]
        public async Task<IActionResult> ModifyWines(CancellationToken cancellationToken)
        {
            var objectscount = 0;

            CustomDataOperation customdataoperation = new CustomDataOperation(settings, QueryFactory);
            //objectscount = await customdataoperation.UpdateAllEventShortstActiveTodayField();
            //objectscount = await customdataoperation.UpdateAllEventShortBrokenLinks();

            //objectscount = await customdataoperation.UpdateAllEventShortPublisherInfo();                        

            //objectscount = await customdataoperation.UpdateAllEventShortstEventDocumentField();

            //objectscount = await customdataoperation.UpdateAllEventShortstActiveFieldToTrue();

            objectscount = await customdataoperation.UpdateAllWineHasLanguage();

            return Ok(new UpdateResult
            {
                operation = "Modify wines",
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
    }
}