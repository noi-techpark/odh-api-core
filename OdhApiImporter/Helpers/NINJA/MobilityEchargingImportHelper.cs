// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper;
using Newtonsoft.Json;
using NINJA;
using NINJA.Parser;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiImporter.Helpers
{
    public class MobilityEchargingImportHelper : ImportHelper, IImportHelper
    {       
        public MobilityEchargingImportHelper(ISettings settings, QueryFactory queryfactory, string table, string importerURL) : base(settings, queryfactory, table, importerURL)
        {

        }


        #region NINJA Helpers

        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, List<string>? idlist = null, CancellationToken cancellationToken = default)
        {
            //Import the data from Mobility Api
            var echarginglist = await ImportList(cancellationToken);
            //Parse the data and save it to DB
            var result = await SaveEchargingstationsToPG(echarginglist);

            return result;
        }

        private async Task<NinjaObjectWithParent<NinjaEchargingStation, NinjaEchargingPlug>> ImportList(CancellationToken cancellationToken)
        {
            var response = await GetNinjaData.GetNinjaEchargingPlugs(settings.NinjaConfig.ServiceUrl);            

            WriteLog.LogToConsole("", "dataimport", "list.echarging", new ImportLog() { sourceid = "", sourceinterface = "mobility.echarging", success = true, error = "" });

            return response;
        }

        private async Task<UpdateDetail> SaveEchargingstationsToPG(NinjaObjectWithParent<NinjaEchargingStation, NinjaEchargingPlug> ninjadata)
        {
            var newimportcounter = 0;
            var updateimportcounter = 0;
            var errorimportcounter = 0;
            var deleteimportcounter = 0;            

            List<string> idlistspreadsheet = new List<string>();
        
            //Get all sources
            List<string> sourcelist = ninjadata.data.Select(x => x.porigin).ToList();


            foreach (var data in ninjadata.data)
            {
                string id = data.pcode;

                var objecttosave = ParseNinjaData.ParseNinjaEchargingToODHActivityPoi(id, data);

                if (objecttosave != null)
                {
                    ////Setting Location Info
                    ////Location Info (by GPS Point)
                    //if (objecttosave.Latitude != 0 && objecttosave.Longitude != 0)
                    //{
                    //    await SetLocationInfo(objecttosave);
                    //}

                    //objecttosave.Active = true;
                    //objecttosave.SmgActive = true;

                    //var idtocheck = kvp.Key;

                    //if (idtocheck.Length > 50)
                    //    idtocheck = idtocheck.Substring(0, 50);

                    //var result = await InsertDataToDB(objecttosave, kvp);

                    //newimportcounter = newimportcounter + result.created ?? 0;
                    //updateimportcounter = updateimportcounter + result.updated ?? 0;
                    //errorimportcounter = errorimportcounter + result.error ?? 0;

                    //idlistspreadsheet.Add(idtocheck.ToUpper());

                    //if (!sourcelist.Contains(objecttosave.Source))
                    //    sourcelist.Add(objecttosave.Source);

                    WriteLog.LogToConsole(id, "dataimport", "single.echarging", new ImportLog() { sourceid = id, sourceinterface = "mobility.echarging", success = true, error = "" });
                }
                else
                {
                    WriteLog.LogToConsole(id, "dataimport", "single.echarging", new ImportLog() { sourceid = id, sourceinterface = "mobility.echarging", success = false, error = "echarging could not be parsed" });
                }
            }

            //Begin SetDataNotinListToInactive
            var idlistdb = await GetAllDataBySource(sourcelist);

            var idstodelete = idlistdb.Where(p => !idlistspreadsheet.Any(p2 => p2 == p));

            foreach (var idtodelete in idstodelete)
            {
                var deletedisableresult = await DeleteOrDisableData(idtodelete, false);

                if(deletedisableresult.Item1 > 0)
                    WriteLog.LogToConsole(idtodelete, "dataimport", "single.echarging.deactivate", new ImportLog() { sourceid = idtodelete, sourceinterface = "mobility.echarging", success = true, error = "" });
                else if (deletedisableresult.Item2 > 0)
                    WriteLog.LogToConsole(idtodelete, "dataimport", "single.echarging.delete", new ImportLog() { sourceid = idtodelete, sourceinterface = "mobility.echarging", success = true, error = "" });


                deleteimportcounter = deleteimportcounter + deletedisableresult.Item1 + deletedisableresult.Item2;
            }

            return new UpdateDetail() { updated = updateimportcounter, created = newimportcounter, deleted = deleteimportcounter, error = errorimportcounter };
        }
   
        private async Task<PGCRUDResult> InsertDataToDB(ODHActivityPoiLinked objecttosave, KeyValuePair<string, NinjaDataWithParent<NinjaEchargingStation, NinjaEchargingPlug>> ninjadata)
        {
            try
            {
                objecttosave.Id = objecttosave.Id?.ToLower();

                //Set LicenseInfo
                objecttosave.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<ODHActivityPoi>(objecttosave, Helper.LicenseHelper.GetLicenseforOdhActivityPoi);

                //Set PublishedOn
                objecttosave.CreatePublishedOnList();

                var rawdataid = await InsertInRawDataDB(ninjadata);

                return await QueryFactory.UpsertData<ODHActivityPoiLinked>(objecttosave, "smgpois", rawdataid, "mobility.echarging.import", importerURL);                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<int> InsertInRawDataDB(KeyValuePair<string, NinjaDataWithParent<NinjaEchargingStation, NinjaEchargingPlug>> ninjadata)
        {
            return await QueryFactory.InsertInRawtableAndGetIdAsync(
                        new RawDataStore()
                        {
                            datasource = ninjadata.Value.porigin,
                            importdate = DateTime.Now,
                            raw = JsonConvert.SerializeObject(ninjadata.Value),
                            sourceinterface = "echarging",
                            sourceid = ninjadata.Key,
                            sourceurl = "https://mobility.api.opendatahub.com/v2/flat/EChargingPlug/",
                            type = "echarging",
                            license = "open",
                            rawformat = "json"
                        });
        }        
          
        private async Task<Tuple<int,int>> DeleteOrDisableData(string id, bool delete)
        {
            var deleteresult = 0;
            var updateresult = 0;

            if (delete)
            {
                deleteresult = await QueryFactory.Query("smgpois").Where("id", id)
                    .DeleteAsync();
            }
            else
            {
                var query =
               QueryFactory.Query("smgpois")
                   .Select("data")
                   .Where("id", id);

                var data = await query.GetObjectSingleAsync<ODHActivityPoiLinked>();

                if (data != null)                
                {
                    if (data.Active != false || data.SmgActive != false)
                    {
                        data.Active = false;
                        data.SmgActive = false;

                        updateresult = await QueryFactory.Query("smgpois").Where("id", id)
                                        .UpdateAsync(new JsonBData() { id = id, data = new JsonRaw(data) });                        
                    }
                }
            }

            return Tuple.Create(updateresult, deleteresult);
        }

        #endregion

        #region CUSTOM Ninja Import

        private async Task<List<string>> GetAllDataBySource(List<string> sourcelist)
        {

            var query =
               QueryFactory.Query("smgpois")
                   .Select("id")
                   .SourceFilter_GeneratedColumn(sourcelist);

            var eventids = await query.GetAsync<string>();

            return eventids.ToList();
        }

        //private async Task SetLocationInfo(ODHActivityPoiLinked myodhactivitypoi)
        //{
        //    var district = await GetLocationInfo.GetNearestDistrictbyGPS(QueryFactory, myevent.Latitude, myevent.Longitude, 30000);

        //    if (district == null)
        //        return;

        //    myevent.DistrictId = district.Id;
        //    myevent.DistrictIds = new List<string>() { district.Id };

        //    var locinfo = await GetLocationInfo.GetTheLocationInfoDistrict(QueryFactory, district.Id);
        //    if (locinfo != null)
        //    {
        //        LocationInfoLinked locinfolinked = new LocationInfoLinked
        //        {
        //            DistrictInfo = new DistrictInfoLinked
        //            {
        //                Id = locinfo.DistrictInfo?.Id,
        //                Name = locinfo.DistrictInfo?.Name
        //            },
        //            MunicipalityInfo = new MunicipalityInfoLinked
        //            {
        //                Id = locinfo.MunicipalityInfo?.Id,
        //                Name = locinfo.MunicipalityInfo?.Name
        //            },
        //            TvInfo = new TvInfoLinked
        //            {
        //                Id = locinfo.TvInfo?.Id,
        //                Name = locinfo.TvInfo?.Name
        //            },
        //            RegionInfo = new RegionInfoLinked
        //            {
        //                Id = locinfo.RegionInfo?.Id,
        //                Name = locinfo.RegionInfo?.Name
        //            }
        //        };

        //        myevent.LocationInfo = locinfolinked;
        //    }
        //}

        #endregion

    }
}
