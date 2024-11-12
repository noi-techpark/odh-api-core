// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper;
using LTSAPI;
using LTSAPI.Parser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Helper.Tagging;

namespace OdhApiImporter.Helpers.LTSAPI
{
    public class LTSApiAccommodationImportHelper : ImportHelper, IImportHelper
    {
        public LTSApiAccommodationImportHelper(ISettings settings, QueryFactory queryfactory, string table, string importerURL) : base(settings, queryfactory, table, importerURL)
        {

        }

        private async Task<List<JObject>> GetAccommodationListFromLTSV2(DateTime? lastchanged, List<string>? idlist)
        {
            try
            {
                LtsApi ltsapi = new LtsApi(settings.LtsCredentials.serviceurl, settings.LtsCredentials.username, settings.LtsCredentials.password, settings.LtsCredentials.ltsclientid, false);
                var qs = new LTSQueryStrings() { page_size = 20 };

                if(lastchanged != null)
                    qs.filter_lastUpdate = lastchanged;

                if (idlist != null && idlist.Count > 0)
                    qs.filter_rids = String.Join(",", idlist);

                var dict = ltsapi.GetLTSQSDictionary(qs);
                var ltsdata = await ltsapi.AccommodationListRequest(dict, true);

                return ltsdata;
            }
            catch (Exception ex)
            {
                WriteLog.LogToConsole("", "dataimport", "list.accommodations", new ImportLog() { sourceid = "", sourceinterface = "lts.accommodations", success = false, error = ex.Message });

                return null;
            }
        }

        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, List<string>? idlist = null, CancellationToken cancellationToken = default)
        {
            //Import the List & Data
            var accommodationids = await GetAccommodationListFromLTSV2(lastchanged, idlist);
            //Import Single Data & Deactivate Data
            var result = await SaveAccommodationsToPG(accommodationids);            

            return result;
        }

        private async Task<UpdateDetail> SaveAccommodationsToPG(List<JObject> ltsdata)
        {
            var newimportcounter = 0;
            var updateimportcounter = 0;
            var errorimportcounter = 0;
            var deleteimportcounter = 0;

            if (ltsdata != null)
            {
                List<string> idlistlts = new List<string>();                
                List<LTSAccoData> ltsaccos = new List<LTSAccoData>();

                var xmlfiles = LoadXmlFiles(Path.Combine(".\\xml\\"));

                foreach (var ltsdatasingle in ltsdata)
                {
                    //To check if this works also for the paginated
                    ltsaccos.AddRange(ltsdatasingle["data"].ToObject<IList<LTSAccoData>>());
                }

                foreach (var data in ltsaccos)
                {
                    string id = data.rid;

                    //See if data exists
                    var query = QueryFactory.Query("accommodations")
                        .Select("data")
                        .Where("id", id);

                    var objecttosave = await query.GetObjectSingleAsync<AccommodationV2>();

                    //Parse Accommodation 
                    AccommodationV2 accommodationV2 = AccommodationParser.ParseLTSAccommodation(data, false, xmlfiles);

                    //TODO Take everything from Loaded Accommodation that should remain as it is
                    if(objecttosave != null)
                    {

                    }

                    var result = await InsertDataToDB(accommodationV2, data);

                    newimportcounter = newimportcounter + result.created ?? 0;
                    updateimportcounter = updateimportcounter + result.updated ?? 0;
                    errorimportcounter = errorimportcounter + result.error ?? 0;

                    idlistlts.Add(id);

                    WriteLog.LogToConsole(id, "dataimport", "single.accommodations", new ImportLog() { sourceid = id, sourceinterface = "lts.accommodations", success = true, error = "" });
                }
            }
            else
                errorimportcounter = 1;

            return new UpdateDetail() { updated = updateimportcounter, created = newimportcounter, deleted = deleteimportcounter, error = errorimportcounter };
        }

        public IDictionary<string, XDocument> LoadXmlFiles(string directory)
        {
            //TODO move this files to Database

            IDictionary<string, XDocument> myxmlfiles = new Dictionary<string, XDocument>();
            myxmlfiles.Add("AccoCategories", XDocument.Load(directory + "AccoCategories.xml"));
            myxmlfiles.Add("AccoTypes", XDocument.Load(directory + "AccoTypes.xml"));
            myxmlfiles.Add("Alpine", XDocument.Load(directory + "Alpine.xml"));
            myxmlfiles.Add("Boards", XDocument.Load(directory + "Boards.xml"));
            myxmlfiles.Add("City", XDocument.Load(directory + "City.xml"));
            myxmlfiles.Add("Dolomites", XDocument.Load(directory + "Dolomites.xml"));
            myxmlfiles.Add("Features", XDocument.Load(directory + "Features.xml"));
            myxmlfiles.Add("Mediterranean", XDocument.Load(directory + "Mediterranean.xml"));
            myxmlfiles.Add("NearSkiArea", XDocument.Load(directory + "NearSkiArea.xml"));
            myxmlfiles.Add("RoomAmenities", XDocument.Load(directory + "RoomAmenities.xml"));
            myxmlfiles.Add("Vinum", XDocument.Load(directory + "Vinum.xml"));
            myxmlfiles.Add("Wine", XDocument.Load(directory + "Wine.xml"));

            return myxmlfiles;
        }

        private async Task<PGCRUDResult> InsertDataToDB(AccommodationV2 objecttosave, LTSAccoData data)
        {
            try
            {                
                //Set LicenseInfo
                objecttosave.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject(objecttosave, Helper.LicenseHelper.GetLicenseforAccommodation);

                //Setting MetaInfo (we need the MetaData Object in the PublishedOnList Creator)
                objecttosave._Meta = MetadataHelper.GetMetadataobject(objecttosave);

                //Set PublishedOn
                objecttosave.CreatePublishedOnList();

                //Populate Tags (Id/Source/Type)
                await objecttosave.UpdateTagsExtension(QueryFactory);

                var rawdataid = await InsertInRawDataDB(data);

                return await QueryFactory.UpsertData<AccommodationV2>(objecttosave, "accommodations", rawdataid, "lts.accommodations.import", importerURL);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<int> InsertInRawDataDB(LTSAccoData data)
        {
            return await QueryFactory.InsertInRawtableAndGetIdAsync(
                        new RawDataStore()
                        {
                            datasource = "lts",
                            importdate = DateTime.Now,
                            raw = JsonConvert.SerializeObject(data),
                            sourceinterface = "accommodations",
                            sourceid = data.rid,
                            sourceurl = "https://go.lts.it/api/v1/accommodations",
                            type = "accommodations",
                            license = "open",
                            rawformat = "json"
                        });
        }        
    }    
}
