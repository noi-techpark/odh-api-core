// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper;
using LTSAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NINJA.Parser;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiImporter.Helpers.LTSCDB
{
    public class LTSApiGuestCardImportHelper : ImportHelper, IImportHelper
    {
        public LTSApiGuestCardImportHelper(ISettings settings, QueryFactory queryfactory, string table, string importerURL) : base(settings, queryfactory, table, importerURL)
        {

        }

        private async Task<List<JObject>> GetGuestcardsFromLTSV2()
        {
            try
            {
                LtsApi ltsapi = new LtsApi(settings.LtsCredentials.serviceurl, settings.LtsCredentials.username, settings.LtsCredentials.password, settings.LtsCredentials.ltsclientid, false);
                var qs = new LTSQueryStrings() { page_size = 100 };
                var dict = ltsapi.GetLTSQSDictionary(qs);

                var ltsdata = await ltsapi.SuedtirolGuestPassCardTypesRequest(dict, true);

                return ltsdata;
            }
            catch (Exception ex)
            {
                return null;
            }
        }       

        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, List<string>? idlist = null, CancellationToken cancellationToken = default)
        {
            //Import the List
            var guestcards = await GetGuestcardsFromLTSV2();
            //Import Single Data


            //Deactivate Data

            throw new NotImplementedException();
        }

        private async Task<UpdateDetail> SaveSuedtirolGuestPassCardTypesToPG(NinjaObjectWithParent<NinjaEchargingPlug, NinjaEchargingStation> ninjadata)
        {
            var newimportcounter = 0;
            var updateimportcounter = 0;
            var errorimportcounter = 0;
            var deleteimportcounter = 0;

            //List<string> idlistspreadsheet = new List<string>();
          
            //var tagquery = QueryFactory.Query("smgtags")
            //        .Select("data")
            //        .Where("id", "e-auto ladestation");
            //var echargingtag = await tagquery.GetObjectSingleAsync<ODHTagLinked>();

            //foreach (var data in ninjadata.data)
            //{
            //    string id = "echarging_" + data.scode.Replace("/", "").ToLower();

            //    //See if data exists
            //    var query = QueryFactory.Query("smgpois")
            //        .Select("data")
            //        .Where("id", id);

            //    var objecttosave = await query.GetObjectSingleAsync<ODHActivityPoiLinked>();

            //    objecttosave = ParseNinjaData.ParseNinjaEchargingToODHActivityPoi(id, data, objecttosave, echargingtag);

            //    if (objecttosave != null)
            //    {
            //        //Setting Location Info                    
            //        if (objecttosave.GpsInfo != null)
            //        {
            //            await SetLocationInfo(objecttosave);
            //        }

            //        objecttosave.Active = true;
            //        //objecttosave.SmgActive = true;

            //        //var idtocheck = kvp.Key;

            //        //if (idtocheck.Length > 50)
            //        //    idtocheck = idtocheck.Substring(0, 50);

            //        var result = await InsertDataToDB(objecttosave, new KeyValuePair<string, NinjaDataWithParent<NinjaEchargingPlug, NinjaEchargingStation>>(id, data));

            //        newimportcounter = newimportcounter + result.created ?? 0;
            //        updateimportcounter = updateimportcounter + result.updated ?? 0;
            //        errorimportcounter = errorimportcounter + result.error ?? 0;

            //        idlistspreadsheet.Add(id);

            //        //if (!sourcelist.Contains(objecttosave.Source))
            //        //    sourcelist.Add(objecttosave.Source);

            //        WriteLog.LogToConsole(id, "dataimport", "single.echarging", new ImportLog() { sourceid = id, sourceinterface = "mobility.echarging", success = true, error = "" });
            //    }
            //    else
            //    {
            //        WriteLog.LogToConsole(id, "dataimport", "single.echarging", new ImportLog() { sourceid = id, sourceinterface = "mobility.echarging", success = false, error = "echarging could not be parsed" });
            //    }
            //}

            ////Begin SetDataNotinListToInactive
            //var idlistdb = await GetAllDataBySource(sourcelist.Select(x => x.Item1).Distinct().ToList());

            //var idstodelete = idlistdb.Where(p => !idlistspreadsheet.Any(p2 => p2 == p));

            //foreach (var idtodelete in idstodelete)
            //{
            //    var deletedisableresult = await DeleteOrDisableData(idtodelete, false);

            //    if (deletedisableresult.Item1 > 0)
            //        WriteLog.LogToConsole(idtodelete, "dataimport", "single.echarging.deactivate", new ImportLog() { sourceid = idtodelete, sourceinterface = "mobility.echarging", success = true, error = "" });
            //    else if (deletedisableresult.Item2 > 0)
            //        WriteLog.LogToConsole(idtodelete, "dataimport", "single.echarging.delete", new ImportLog() { sourceid = idtodelete, sourceinterface = "mobility.echarging", success = true, error = "" });


            //    deleteimportcounter = deleteimportcounter + deletedisableresult.Item1 + deletedisableresult.Item2;
            //}

            return new UpdateDetail() { updated = updateimportcounter, created = newimportcounter, deleted = deleteimportcounter, error = errorimportcounter };
        }

        //private async Task<PGCRUDResult> InsertDataToDB(ODHActivityPoiLinked objecttosave, KeyValuePair<string, NinjaDataWithParent<NinjaEchargingPlug, NinjaEchargingStation>> ninjadata)
        //{
        //    try
        //    {
        //        objecttosave.Id = objecttosave.Id?.ToLower();

        //        //Set LicenseInfo
        //        objecttosave.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject(objecttosave, Helper.LicenseHelper.GetLicenseforOdhActivityPoi);

        //        //Setting MetaInfo (we need the MetaData Object in the PublishedOnList Creator)
        //        objecttosave._Meta = MetadataHelper.GetMetadataobject(objecttosave);

        //        //Set PublishedOn
        //        objecttosave.CreatePublishedOnList();

        //        var rawdataid = await InsertInRawDataDB(ninjadata);

        //        return await QueryFactory.UpsertData<ODHActivityPoiLinked>(objecttosave, "tags", rawdataid, "lts.suedtirolguestpass.import", importerURL);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        //private async Task<int> InsertInRawDataDB(KeyValuePair<string, NinjaDataWithParent<NinjaEchargingPlug, NinjaEchargingStation>> ninjadata)
        //{
        //    return await QueryFactory.InsertInRawtableAndGetIdAsync(
        //                new RawDataStore()
        //                {
        //                    datasource = ninjadata.Value.porigin,
        //                    importdate = DateTime.Now,
        //                    raw = JsonConvert.SerializeObject(ninjadata.Value),
        //                    sourceinterface = "echarging",
        //                    sourceid = ninjadata.Key,
        //                    sourceurl = "https://mobility.api.opendatahub.com/v2/flat/EChargingPlug/",
        //                    type = "echarging",
        //                    license = "open",
        //                    rawformat = "json"
        //                });
        //}

        //private async Task<Tuple<int, int>> DeleteOrDisableData(string id, bool delete)
        //{
        //    var deleteresult = 0;
        //    var updateresult = 0;

        //    if (delete)
        //    {
        //        deleteresult = await QueryFactory.Query("smgpois").Where("id", id)
        //            .DeleteAsync();
        //    }
        //    else
        //    {
        //        var query =
        //       QueryFactory.Query("smgpois")
        //           .Select("data")
        //           .Where("id", id);

        //        var data = await query.GetObjectSingleAsync<ODHActivityPoiLinked>();

        //        if (data != null)
        //        {
        //            if (data.Active != false || data.SmgActive != false)
        //            {
        //                data.Active = false;
        //                data.SmgActive = false;

        //                updateresult = await QueryFactory.Query("smgpois").Where("id", id)
        //                                .UpdateAsync(new JsonBData() { id = id, data = new JsonRaw(data) });
        //            }
        //        }
        //    }

        //    return Tuple.Create(updateresult, deleteresult);
        //}


    }
}
