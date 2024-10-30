// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper;
using LTSAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NINJA.Parser;
using ServiceReferenceLCS;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiImporter.Helpers.LTSAPI
{
    public class LTSApiVenueCategoriesImportHelper : ImportHelper, IImportHelper
    {
        public LTSApiVenueCategoriesImportHelper(ISettings settings, QueryFactory queryfactory, string table, string importerURL) : base(settings, queryfactory, table, importerURL)
        {

        }

        private async Task<List<JObject>> GetVenueCategoriesFromLTSV2()
        {
            try
            {
                LtsApi ltsapi = new LtsApi(settings.LtsCredentials.serviceurl, settings.LtsCredentials.username, settings.LtsCredentials.password, settings.LtsCredentials.ltsclientid, false);
                var qs = new LTSQueryStrings() { page_size = 100 };
                var dict = ltsapi.GetLTSQSDictionary(qs);

                var ltsdata = await ltsapi.VenueCategoryRequest(dict, true);

                return ltsdata;
            }
            catch (Exception ex)
            {
                WriteLog.LogToConsole("", "dataimport", "single.venues.categories", new ImportLog() { sourceid = "", sourceinterface = "lts.venues.categories", success = false, error = ex.Message });

                return null;
            }
        }

        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, List<string>? idlist = null, CancellationToken cancellationToken = default)
        {
            //Import the List
            var eventtags = await GetVenueCategoriesFromLTSV2();
            //Import Single Data & Deactivate Data
            var result = await SaveGastronomyCategoriesToPG(eventtags);            

            return result;
        }

        private async Task<UpdateDetail> SaveGastronomyCategoriesToPG(List<JObject> ltsdata)
        {
            var newimportcounter = 0;
            var updateimportcounter = 0;
            var errorimportcounter = 0;
            var deleteimportcounter = 0;

            if (ltsdata != null)
            {

                List<string> idlistlts = new List<string>();

                List<LTSVenueCategory> eventtagdata = new List<LTSVenueCategory>();

                foreach (var ltsdatasingle in ltsdata)
                {
                    eventtagdata.AddRange(ltsdatasingle["data"].ToObject<IList<LTSVenueCategory>>());
                }

                foreach (var data in eventtagdata)
                {
                    string id = data.rid;

                    //See if data exists
                    var query = QueryFactory.Query("tags")
                        .Select("data")
                        .Where("id", id);

                    var objecttosave = await query.GetObjectSingleAsync<TagLinked>();

                    if (objecttosave == null)
                        objecttosave = new TagLinked();

                    objecttosave.Id = data.rid;
                    objecttosave.Active = true;
                    objecttosave.DisplayAsCategory = false;
                    objecttosave.FirstImport = objecttosave.FirstImport == null ? DateTime.Now : objecttosave.FirstImport;
                    objecttosave.LastChange = data.lastUpdate;

                    objecttosave.Source = "lts";
                    objecttosave.TagName = data.name;
                    objecttosave.Description = data.description;

                    objecttosave.MainEntity = "odhactivitypoi";
                    objecttosave.ValidForEntity = new List<string>() { "odhactivitypoi", "gastronomy" };
                    objecttosave.Shortname = objecttosave.TagName.ContainsKey("en") ? objecttosave.TagName["en"] : objecttosave.TagName.FirstOrDefault().Value;
                    objecttosave.Types = new List<string>() { "venuecategory" }; //TODO Clean venue categories

                    //objecttosave.IDMCategoryMapping = null;
                    objecttosave.PublishDataWithTagOn = null;
                    objecttosave.Mapping = new Dictionary<string, IDictionary<string, string>>() { { "lts", new Dictionary<string, string>() { { "rid", data.rid }, { "code", data.code } } } };
                    objecttosave.LTSTaggingInfo = null;
                    objecttosave.PublishedOn = null;

                    //Do not set this because we have mapped tag ids assigned
                    //objecttosave.MappedTagIds = null;


                    var result = await InsertDataToDB(objecttosave, data);

                    newimportcounter = newimportcounter + result.created ?? 0;
                    updateimportcounter = updateimportcounter + result.updated ?? 0;
                    errorimportcounter = errorimportcounter + result.error ?? 0;

                    idlistlts.Add(id);

                    WriteLog.LogToConsole(id, "dataimport", "single.venues.categories", new ImportLog() { sourceid = id, sourceinterface = "lts.venues.categories", success = true, error = "" });
                }

                if (idlistlts.Count > 0)
                {
                    //Begin SetDataNotinListToInactive
                    var idlistdb = await GetAllDataBySourceAndType(new List<string>() { "lts" }, new List<string>() { "venuecategory" });

                    var idstodelete = idlistdb.Where(p => !idlistlts.Any(p2 => p2 == p));

                    foreach (var idtodelete in idstodelete)
                    {
                        var deletedisableresult = await DeleteOrDisableData<TagLinked>(idtodelete, false);

                        if (deletedisableresult.Item1 > 0)
                            WriteLog.LogToConsole(idtodelete, "dataimport", "single.venues.categories.deactivate", new ImportLog() { sourceid = idtodelete, sourceinterface = "lts.venues.categories", success = true, error = "" });
                        else if (deletedisableresult.Item2 > 0)
                            WriteLog.LogToConsole(idtodelete, "dataimport", "single.venues.categories.delete", new ImportLog() { sourceid = idtodelete, sourceinterface = "lts.venues.categories", success = true, error = "" });


                        deleteimportcounter = deleteimportcounter + deletedisableresult.Item1 + deletedisableresult.Item2;
                    }
                }
            }
            else
                errorimportcounter = 1;

            return new UpdateDetail() { updated = updateimportcounter, created = newimportcounter, deleted = deleteimportcounter, error = errorimportcounter };
        }

        private async Task<PGCRUDResult> InsertDataToDB(TagLinked objecttosave, LTSVenueCategory data)
        {
            try
            {                
                //Set LicenseInfo
                objecttosave.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject(objecttosave, Helper.LicenseHelper.GetLicenseforTag);

                //Setting MetaInfo (we need the MetaData Object in the PublishedOnList Creator)
                objecttosave._Meta = MetadataHelper.GetMetadataobject(objecttosave);

                //Set PublishedOn
                objecttosave.CreatePublishedOnList();

                var rawdataid = await InsertInRawDataDB(data);

                return await QueryFactory.UpsertData<TagLinked>(objecttosave, "tags", rawdataid, "lts.venues.categories.import", importerURL);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<int> InsertInRawDataDB(LTSVenueCategory data)
        {
            return await QueryFactory.InsertInRawtableAndGetIdAsync(
                        new RawDataStore()
                        {
                            datasource = "lts",
                            importdate = DateTime.Now,
                            raw = JsonConvert.SerializeObject(data),
                            sourceinterface = "venues",
                            sourceid = data.rid,
                            sourceurl = "https://go.lts.it/api/v1/venues/categories",
                            type = "venues.categories",
                            license = "open",
                            rawformat = "json"
                        });
        }        
    }

    public class LTSVenueCategory
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }

        public bool? isHotel { get; set; }
        public int? minimalHallNumber { get; set; }
        public int? minimalSurfaceInSquareMeters { get; set; }

        public IDictionary<string,string> name { get; set; }
        public IDictionary<string, string> description { get; set; }
        //public bool isActive { get; set; }
        public string code { get; set; }        
    }    
}
