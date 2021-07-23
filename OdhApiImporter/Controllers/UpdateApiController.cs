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


        [HttpGet, Route("EBMS/EventShort/UpdateSingle/{id}")]
        public IActionResult UpdateSingleEBMS(string id, CancellationToken cancellationToken)
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

        #endregion

        #region ODH RAVEN exposed

        [HttpGet, Route("Raven/{datatype}/Update/{id}")]
        //[Authorize(Roles = "DataWriter,DataCreate,DataUpdate")]
        public async Task<IActionResult> UpdateFromRaven(string id, string datatype, CancellationToken cancellationToken)
        {
            return await GetFromRavenAndTransformToPGObject(id, datatype, cancellationToken);
        }

        #endregion


        #region ODHRAVEN Helpers

        public async Task<IActionResult> GetFromRavenAndTransformToPGObject(string id, string datatype, CancellationToken cancellationToken)
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
                        else
                            throw new Exception("No data found!");

                        await SaveRavenObjectToPG<AccommodationLinked>((AccommodationLinked)mypgdata, "accommodations");

                        //UPDATE ACCOMMODATIONROOMS
                        var myroomdatalist = await GetDataFromRaven.GetRavenData<IEnumerable<AccommodationRoomLinked>>("accommodationroom", id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken, "?accoid=");

                        if (myroomdatalist != null)
                        {
                            foreach (var myroomdata in myroomdatalist)
                            {
                                var mypgroomdata = TransformToPGObject.GetPGObject<AccommodationRoomLinked, AccommodationRoomLinked>((AccommodationRoomLinked)myroomdata, TransformToPGObject.GetAccommodationRoomPGObject);

                                await SaveRavenObjectToPG<AccommodationRoomLinked>((AccommodationRoomLinked)mypgroomdata, "accommodationrooms");
                            }
                        }
                        else
                            throw new Exception("No data found!");

                        return Ok(new GenericResult() { Message = String.Format("{0} success: {1}", "accommodations", id) });

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
                        mydata = await GetDataFromRaven.GetRavenData<MeasuringpointLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
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

                        return await SaveRavenObjectToPG<DDVenue>((DDVenue)mypgdata, "venue");

                    default:
                        return BadRequest(new { error = "no match found" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = env.IsDevelopment() ? ex.ToString() : ex.Message });
            }
        }

        private async Task<IActionResult> SaveRavenObjectToPG<T>(T datatosave, string table) where T : IIdentifiable, IImportDateassigneable, IMetaData, ILicenseInfo
        {
            datatosave._Meta.LastUpdate = datatosave.LastChange;

            return await UpsertData<T>(datatosave, table);
        }

        #endregion

        #region PG Helpers

        private async Task<IActionResult> UpsertData<T>(T data, string table) where T : IIdentifiable, IImportDateassigneable
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "no data");

            //Check if data exists
            var query = QueryFactory.Query(table)
                      .Select("data")
                      .Where("id", data.Id);

            var queryresult = await query.GetAsync<T>();

            string operation = "";

            if (queryresult == null || queryresult.Count() == 0)
            {
                data.FirstImport = DateTime.Now;
                data.LastChange = DateTime.Now;

                var insertresult = await QueryFactory.Query(table)
                   .InsertAsync(new JsonBData() { id = data.Id, data = new JsonRaw(data) });
                operation = "INSERT";
            }
            else
            {
                data.LastChange = DateTime.Now;

                var updateresult = await QueryFactory.Query(table).Where("id", data.Id)
                        .UpdateAsync(new JsonBData() { id = data.Id, data = new JsonRaw(data) });
                operation = "UPDATE";
            }

            return Ok(new GenericResult() { Message = String.Format("{0} success: {1}", operation, data.Id) });
        }

        private async Task<IActionResult> DeleteData(string id, string table)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException(nameof(id), "No data");

            //Check if data exists
            var query =
                  QueryFactory.Query(table)
                      .Select("data")
                      .Where("id", id);

            if (query == null)
            {
                throw new ArgumentNullException(nameof(query), "No data");
            }
            else
            {
                await QueryFactory.Query(table).Where("id", id)
                        .DeleteAsync();
            }

            return Ok(new GenericResult() { Message = String.Format("DELETE success: {0}", id) });
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

        public string id { get; set; }

    }
}
