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
using AspNetCore.CacheOutput;
using System.IO;
using OdhApiCore.GenericHelpers;

namespace OdhApiCore.Controllers.api
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class STAController : OdhController
    {
        private readonly IWebHostEnvironment env;
        private readonly ISettings settings;

        public STAController(IWebHostEnvironment env, ISettings settings, ILogger<STAController> logger, QueryFactory queryFactory)
         : base(env, settings, logger, queryFactory)
        {
            this.env = env;
            this.settings = settings;
        }

        #region GETTER

        [CacheOutput(ClientTimeSpan = 14400, ServerTimeSpan = 14400)]
        [HttpGet, Route("STA/ODHActivityPoi")]
        public async Task<IActionResult> GetODHActivityPoiListSTA(
            string language,
            CancellationToken cancellationToken)
        {
            try
            {
                var checkedlanguage = CheckLanguages(language);

                string fileName = Path.Combine(settings.JsonConfig.Jsondir, $"STAOdhActivitiesPois_{checkedlanguage}.json");

                using (StreamReader r = new StreamReader(fileName))
                {
                    string json = await r.ReadToEndAsync();

                    return new OkObjectResult(new JsonRaw(json));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [CacheOutput(ClientTimeSpan = 14400, ServerTimeSpan = 14400)]
        [HttpGet, Route("STA/Accommodation")]
        public async Task<IActionResult> GetAccommodationsSTA(
           string language,
           CancellationToken cancellationToken)
        {
            try
            {
                var checkedlanguage = CheckLanguages(language);

                //DOCKER ERROR Could not find a part of the path '/app/.\wwwroot\json\/STAAccommodations_de.json'.
                string fileName = Path.Combine(settings.JsonConfig.Jsondir, $"STAAccommodations_{checkedlanguage}.json");

                using (StreamReader r = new StreamReader(fileName))
                {
                    string json = await r.ReadToEndAsync();

                    return new OkObjectResult(new JsonRaw(json));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion       

        #region GENERATEJSON

        [InvalidateCacheOutput(typeof(STAController), nameof(GetODHActivityPoiListSTA))] // this will invalidate Get in a different controller
        [HttpGet, Route("STA/JsonPoi")]
        public async Task<IActionResult> ProducePoiSTAJson(CancellationToken cancellationToken)
        {
            try
            {
                await STARequestHelper.GenerateJSONODHActivityPoiForSTA(QueryFactory, settings.JsonConfig.Jsondir, settings.XmlConfig.Xmldir);
            
                return Ok(new
                {
                    operation = "Json Generation",
                    type = "ODHActivityPoi",
                    message = "Generate Json ODHActivityPoi for STA succeeded",
                    success = true
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new
                {
                    operation = "Json Generation",
                    type = "ODHActivityPoi",
                    message = "Generate Json ODHActivityPoi for STA failed error:" + ex.Message,
                    success = false
                });
            }
        }

        [InvalidateCacheOutput(typeof(STAController), nameof(GetAccommodationsSTA))] // this will invalidate Get in a different controller
        [HttpGet, Route("STA/JsonAccommodation")]
        public async Task<IActionResult> ProduceAccoSTAJson(CancellationToken cancellationToken)
        {
            try
            {
                await STARequestHelper.GenerateJSONAccommodationsForSTA(QueryFactory, settings.JsonConfig.Jsondir);

                return Ok(new
                {
                    operation = "Json Generation",
                    type = "Accommodation",
                    message = "Generate Json Accommodation for STA succeeded",
                    success = true
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new
                {
                    operation = "Json Generation",
                    type = "Accommodation",
                    message = "Generate Json Accommodation for STA failed error:" + ex.Message,
                    success = false
                });
            }
        }

        #endregion

        #region IMPORTER

        //[Authorize(Roles = "DataWriter,DataCreate,ODHTagManager,ODHTagCreate")]
        //[HttpGet, Route("STA/ImportVendingPoints")]
        //public async Task<IActionResult> ImportVendingPointsFromSTA(CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        return await ImportVendingPointsFromCSV(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [Authorize(Roles = "DataWriter,STAPoiImport")]
        [HttpPost, Route("STA/ImportVendingPoints")]
        public async Task<IActionResult> SendVendingPointsFromSTA(CancellationToken cancellationToken)
        {
            try
            {
                var result = await PostVendingPointsFromSTA(Request);

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

        #region HELPERS

        private string CheckLanguages(string lang)
        {
            if (lang == "de" || lang == "it" || lang == "en")
                return lang.ToLower();
            else
                return "en";
        }

        private static async Task<string> ReadStringDataManual(HttpRequest request)
        {
            using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private async Task<UpdateDetail> PostVendingPointsFromSTA(HttpRequest request)
        {
            string jsonContent = await ReadStringDataManual(request);

            if (!string.IsNullOrEmpty(jsonContent))
            {
                return await ImportVendingPointsFromCSV(jsonContent);
            }
            else
                throw new Exception("no Content");
        }

        private async Task<UpdateDetail> ImportVendingPointsFromCSV(string csvcontent)
        {
            var vendingpoints = await STA.GetDataFromSTA.ImportCSVFromSTA(csvcontent);

            if (vendingpoints.Success)
            {
                var updatecounter = 0;
                var newcounter = 0;
                var deletecounter = 0;

                //Import Each STA Vendingpoi to ODH
                foreach (var vendingpoint in vendingpoints.records)
                {
                    //Parse to ODHActivityPoi
                    var odhactivitypoi = STA.ParseSTAPois.ParseSTAVendingPointToODHActivityPoi(vendingpoint);

                    //MetaData
                    odhactivitypoi._Meta = MetadataHelper.GetMetadataobject<ODHActivityPoiLinked>(odhactivitypoi, MetadataHelper.GetMetadataforOdhActivityPoi); //GetMetadata(data.Id, "odhactivitypoi", sourcemeta, data.LastChange);
                    //LicenseInfo                                                                                                                                    //License
                    odhactivitypoi.LicenseInfo = LicenseHelper.GetLicenseforOdhActivityPoi(odhactivitypoi);

                    if(odhactivitypoi.GpsPoints.ContainsKey("position"))
                    {
                        //Get Nearest District
                        var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(odhactivitypoi.GpsPoints["position"].Latitude, odhactivitypoi.GpsPoints["position"].Longitude, 10000);
                        var nearestdistrict = await GetLocationInfo.GetNearestDistrict(QueryFactory, geosearchresult, 1);

                        if (nearestdistrict != null && nearestdistrict.Count() > 0)
                        {
                            //Get LocationInfo Object
                            var locationinfo = await GetLocationInfo.GetTheLocationInfoDistrict(QueryFactory, nearestdistrict.FirstOrDefault()?.Id);

                            if (locationinfo != null)
                                odhactivitypoi.LocationInfo = locationinfo;
                        }
                    }

                    //Adding TypeInfo Additional
                    odhactivitypoi.AdditionalPoiInfos = await GetAdditionalTypeInfo.GetAdditionalTypeInfoForPoi(QueryFactory, odhactivitypoi?.SubType, new List<string>() { "de","it","en" });

                    //Save to PG
                    //Check if data exists                    

                    var result = await QueryFactory.UpsertData<ODHActivityPoiLinked>(odhactivitypoi!, "smgpois");

                    if(result.updated != null)
                        updatecounter = updatecounter + result.updated.Value;
                    if (result.created != null)
                        newcounter = newcounter + result.created.Value;
                    if (result.deleted != null)
                        deletecounter = deletecounter + result.deleted.Value;
                }

                return new UpdateDetail() { created = newcounter, updated = updatecounter, deleted = deletecounter };
            }
            else if (vendingpoints.Error)
                throw new Exception(vendingpoints.ErrorMessage);
            else
                throw new Exception("no data to import");
        }

        #endregion
    }
}