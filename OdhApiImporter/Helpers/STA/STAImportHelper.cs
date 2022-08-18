using DataModel;
using Helper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiImporter.Helpers
{
    public class STAImportHelper //: ImportHelper, IImportHelper
    {
        private readonly QueryFactory QueryFactory;
        private readonly ISettings settings;

        public STAImportHelper(ISettings settings, QueryFactory queryfactory)
        {
            this.QueryFactory = queryfactory;
            this.settings = settings;
        }

        private static async Task<string> ReadStringDataManual(HttpRequest request)
        {
            using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task<UpdateDetail> PostVendingPointsFromSTA(
            HttpRequest request,
            CancellationToken cancellationToken
        )
        {
            string jsonContent = await ReadStringDataManual(request);

            if (!string.IsNullOrEmpty(jsonContent))
            {
                return await ImportVendingPointsFromCSV(jsonContent, cancellationToken);
            }
            else
                throw new Exception("no Content");
        }

        private async Task<UpdateDetail> ImportVendingPointsFromCSV(
            string csvcontent,
            CancellationToken cancellationToken
        )
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
                    var odhactivitypoi = STA.ParseSTAPois.ParseSTAVendingPointToODHActivityPoi(
                        vendingpoint
                    );

                    if (odhactivitypoi != null)
                    {
                        //MetaData
                        //odhactivitypoi._Meta = MetadataHelper.GetMetadataobject<ODHActivityPoiLinked>(odhactivitypoi, MetadataHelper.GetMetadataforOdhActivityPoi); //GetMetadata(data.Id, "odhactivitypoi", sourcemeta, data.LastChange);
                        //LicenseInfo                                                                                                                                    //License
                        odhactivitypoi.LicenseInfo = LicenseHelper.GetLicenseforOdhActivityPoi(
                            odhactivitypoi
                        );

                        if (odhactivitypoi.GpsPoints.ContainsKey("position"))
                        {
                            //Get Nearest District
                            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(
                                odhactivitypoi.GpsPoints["position"].Latitude,
                                odhactivitypoi.GpsPoints["position"].Longitude,
                                10000
                            );
                            var nearestdistrict = await GetLocationInfo.GetNearestDistrict(
                                QueryFactory,
                                geosearchresult,
                                1
                            );

                            if (nearestdistrict != null && nearestdistrict.Count() > 0)
                            {
                                //Get LocationInfo Object
                                var locationinfo = await GetLocationInfo.GetTheLocationInfoDistrict(
                                    QueryFactory,
                                    nearestdistrict.FirstOrDefault()?.Id
                                );

                                if (locationinfo != null)
                                    odhactivitypoi.LocationInfo = locationinfo;
                            }
                        }

                        //Adding TypeInfo Additional
                        odhactivitypoi.AdditionalPoiInfos =
                            await GetAdditionalTypeInfo.GetAdditionalTypeInfoForPoi(
                                QueryFactory,
                                odhactivitypoi?.SubType,
                                new List<string>() { "de", "it", "en" }
                            );

                        if (odhactivitypoi is { })
                        {
                            ODHTagHelper.SetMainCategorizationForODHActivityPoi(odhactivitypoi);

                            //Special get all Taglist and traduce it on import
                            await GenericTaggingHelper.AddMappingToODHActivityPoi(
                                odhactivitypoi,
                                settings.JsonConfig.Jsondir
                            );

                            //Save to Rawdatatable
                            var rawdataid = await InsertInRawDataDB(vendingpoint);

                            //Save to PG
                            //Check if data exists
                            var result = await QueryFactory.UpsertData(
                                odhactivitypoi,
                                "smgpois",
                                rawdataid
                            );

                            if (result.updated != null)
                                updatecounter = updatecounter + result.updated.Value;
                            if (result.created != null)
                                newcounter = newcounter + result.created.Value;
                            if (result.deleted != null)
                                deletecounter = deletecounter + result.deleted.Value;
                        }
                    }
                }

                return new UpdateDetail()
                {
                    created = newcounter,
                    updated = updatecounter,
                    deleted = deletecounter
                };
            }
            else if (vendingpoints.Error)
                throw new Exception(vendingpoints.ErrorMessage);
            else
                throw new Exception("no data to import");
        }

        private async Task<int> InsertInRawDataDB(STA.STAVendingPoint stavendingpoint)
        {
            return await QueryFactory.InsertInRawtableAndGetIdAsync(
                new RawDataStore()
                {
                    datasource = "sta",
                    importdate = DateTime.Now,
                    raw = JsonConvert.SerializeObject(stavendingpoint),
                    sourceinterface = "csv",
                    sourceid = stavendingpoint?.STA_ID ?? "",
                    sourceurl = "csvfile",
                    type = "poi_sta_vendingpoint"
                }
            );
        }
    }
}
