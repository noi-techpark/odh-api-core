// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataModel;
using Helper;
using Helper.Generic;
using LTSAPI;
using LTSAPI.Parser;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NINJA.Parser;
using ServiceReferenceLCS;
using SqlKata.Execution;

namespace OdhApiImporter.Helpers.LTSAPI
{
    public class LTSApiEventImportHelper : ImportHelper, IImportHelper
    {
        public LTSApiEventImportHelper(
            ISettings settings,
            QueryFactory queryfactory,
            string table,
            string importerURL
        )
            : base(settings, queryfactory, table, importerURL) { }

        private async Task<List<JObject>> GetEventsFromLTSV2(List<string> eventids, DateTime? lastchanged)
        {
            try
            {
                LtsApi ltsapi = new LtsApi(
                    settings.LtsCredentials.serviceurl,
                    settings.LtsCredentials.username,
                    settings.LtsCredentials.password,
                    settings.LtsCredentials.ltsclientid,
                    false
                );
                
                if(eventids.Count == 1)
                {
                    var qs = new LTSQueryStrings() { page_size = 1, filter_endDate = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd") };
                    var dict = ltsapi.GetLTSQSDictionary(qs);

                    return await ltsapi.EventDetailRequest(eventids.FirstOrDefault(), dict);
                }
                else
                {
                    var qs = new LTSQueryStrings() { page_size = 100, filter_rids = String.Join(",", eventids), filter_endDate = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd") };
                    var dict = ltsapi.GetLTSQSDictionary(qs);

                    return await ltsapi.EventListRequest(dict, true);
                }
                //TODO Add the case no Ids are passed (lastchanged)

            }
            catch (Exception ex)
            {
                WriteLog.LogToConsole(
                    "",
                    "dataimport",
                    "list.events",
                    new ImportLog()
                    {
                        sourceid = "",
                        sourceinterface = "lts.events",
                        success = false,
                        error = ex.Message,
                    }
                );
                return null;
            }
        }

        public async Task<UpdateDetail> SaveDataToODH(
            DateTime? lastchanged = null,
            List<string>? idlist = null,
            CancellationToken cancellationToken = default
        )
        {
            //Import the List
            var eventlts = await GetEventsFromLTSV2(idlist, lastchanged);
            //Import Single Data & Deactivate Data
            var result = await SaveEventsToPG(eventlts);

            return result;
        }

        private async Task<UpdateDetail> SaveEventsToPG(List<JObject> ltsdata)
        {
            var newimportcounter = 0;
            var updateimportcounter = 0;
            var errorimportcounter = 0;
            var deleteimportcounter = 0;

            if (ltsdata != null)
            {
                List<string> idlistlts = new List<string>();

                List<LTSEvent> eventdata = new List<LTSEvent>();

                foreach (var ltsdatasingle in ltsdata)
                {
                    eventdata.Add(
                        ltsdatasingle.ToObject<LTSEvent>()
                    );
                }

                foreach (var data in eventdata)
                {
                    string id = data.data.rid;

                    var eventparsed = EventParser.ParseLTSEventV1(data.data, false);

                    //TODO Add the Code Here for 
                    //DistanceCalculation
                    //Tags not overwrite
                    //LocationInfo Creation
                    //EventDates not delete
                    //Event Start Begindate Logic
                    //ETC......

                    //GET OLD Event
                    var eventindb = await LoadDataFromDB<EventLinked>(id);

                    eventparsed.CreatePublishedOnList();

                    var result = await InsertDataToDB(eventparsed, data.data);

                    newimportcounter = newimportcounter + result.created ?? 0;
                    updateimportcounter = updateimportcounter + result.updated ?? 0;
                    errorimportcounter = errorimportcounter + result.error ?? 0;

                    idlistlts.Add(id);

                    WriteLog.LogToConsole(
                        id,
                        "dataimport",
                        "single.events",
                        new ImportLog()
                        {
                            sourceid = id,
                            sourceinterface = "lts.events",
                            success = true,
                            error = "",
                        }
                    );
                }

                //Deactivate this in the meantime
                //if (idlistlts.Count > 0)
                //{
                //    //Begin SetDataNotinListToInactive
                //    var idlistdb = await GetAllDataBySourceAndType(
                //        new List<string>() { "lts" },
                //        new List<string>() { "eventcategory" }
                //    );

                //    var idstodelete = idlistdb.Where(p => !idlistlts.Any(p2 => p2 == p));

                //    foreach (var idtodelete in idstodelete)
                //    {
                //        var deletedisableresult = await DeleteOrDisableData<TagLinked>(
                //            idtodelete,
                //            false
                //        );

                //        if (deletedisableresult.Item1 > 0)
                //            WriteLog.LogToConsole(
                //                idtodelete,
                //                "dataimport",
                //                "single.events.categories.deactivate",
                //                new ImportLog()
                //                {
                //                    sourceid = idtodelete,
                //                    sourceinterface = "lts.events.categories",
                //                    success = true,
                //                    error = "",
                //                }
                //            );
                //        else if (deletedisableresult.Item2 > 0)
                //            WriteLog.LogToConsole(
                //                idtodelete,
                //                "dataimport",
                //                "single.events.categories.delete",
                //                new ImportLog()
                //                {
                //                    sourceid = idtodelete,
                //                    sourceinterface = "lts.events.categories",
                //                    success = true,
                //                    error = "",
                //                }
                //            );

                //        deleteimportcounter =
                //            deleteimportcounter
                //            + deletedisableresult.Item1
                //            + deletedisableresult.Item2;
                //    }
                //}
            }
            else
                errorimportcounter = 1;

            return new UpdateDetail()
            {
                updated = updateimportcounter,
                created = newimportcounter,
                deleted = deleteimportcounter,
                error = errorimportcounter,
            };
        }

        private async Task<PGCRUDResult> InsertDataToDB(
            EventLinked objecttosave,
            LTSEventData eventlts
        )
        {
            try
            {
                //Set LicenseInfo
                objecttosave.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject(
                    objecttosave,
                    Helper.LicenseHelper.GetLicenseforEvent
                );

                //Setting MetaInfo (we need the MetaData Object in the PublishedOnList Creator)
                objecttosave._Meta = MetadataHelper.GetMetadataobject(objecttosave);

                //Set PublishedOn
                objecttosave.CreatePublishedOnList();

                var rawdataid = await InsertInRawDataDB(eventlts);

                return await QueryFactory.UpsertData<EventLinked>(
                    objecttosave,
                    new DataInfo("events", Helper.Generic.CRUDOperation.CreateAndUpdate),
                    new EditInfo("lts.events.import", importerURL),
                    new CRUDConstraints(),
                    new CompareConfig(true, false),
                    rawdataid
                );
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<int> InsertInRawDataDB(LTSEventData eventlts)
        {
            return await QueryFactory.InsertInRawtableAndGetIdAsync(
                new RawDataStore()
                {
                    datasource = "lts",
                    importdate = DateTime.Now,
                    raw = JsonConvert.SerializeObject(eventlts),
                    sourceinterface = "events",
                    sourceid = eventlts.rid,
                    sourceurl = "https://go.lts.it/api/v1/events",
                    type = "events",
                    license = "open",
                    rawformat = "json",
                }
            );
        }
    }
}
