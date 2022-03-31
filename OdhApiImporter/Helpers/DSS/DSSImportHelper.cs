using DataModel;
using SqlKata.Execution;
using System;
using System.Threading;
using System.Threading.Tasks;
using DSS;
using System.Collections.Generic;
using DSS.Parser;
using System.Globalization;
using Helper;
using Newtonsoft.Json;
using System.Linq;

namespace OdhApiImporter.Helpers.DSS
{
    public class DSSImportHelper : ImportHelper, IImportHelper
    {
        //private readonly QueryFactory QueryFactory;
        //private readonly ISettings settings;

        //public DSSImportHelper(ISettings settings, QueryFactory queryfactory)
        //{
        //    this.QueryFactory = queryfactory;
        //    this.settings = settings;
        //}

        public DSSImportHelper(ISettings settings, QueryFactory queryfactory, string table) : base(settings, queryfactory, table)
        {

        }

        public List<DSSRequestType> requesttypelist { get; set; }
        public string entitytype { get; set; }

        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, CancellationToken cancellationToken = default)
        {
            requesttypelist = new List<DSSRequestType>();

            if (entitytype.ToLower() == "lift")
            {
                requesttypelist.Add(DSSRequestType.liftbase);
                requesttypelist.Add(DSSRequestType.liftstatus);
            }
            else if (entitytype.ToLower() == "slope")
            {
                requesttypelist.Add(DSSRequestType.slopebase);
                requesttypelist.Add(DSSRequestType.slopestatus);
            }

            List<dynamic> dssdata = new List<dynamic>();

            foreach (var requesttype in requesttypelist)
            {
                //Get DSS data
                dssdata.Add(await GetDSSData.GetDSSDataAsync(requesttype, settings.DSSConfig.User, settings.DSSConfig.Password, settings.DSSConfig.ServiceUrl));
            }

            var updateresult = await ImportData(dssdata, cancellationToken);

            return updateresult;
        }

        public async Task<UpdateDetail> ImportData(List<dynamic> dssinput, CancellationToken cancellationToken)
        {
            int updatecounter = 0;
            int newcounter = 0;

            if(dssinput != null && dssinput.Count > 0)
            {
                string lastupdatestr = dssinput[0].lastUpdate;
                //interface lastupdate
                DateTime.TryParseExact(lastupdatestr, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime lastupdate);

                var areaquery = QueryFactory.Query()
                            .SelectRaw("data")
                            .From("areas");

                // Get all Areas
                var arealist =
                    await areaquery
                        .GetAllAsObject<AreaLinked>();

                //loop trough dss items
                foreach (var item in dssinput[0].items)
                {
                    //Parse DSS Data
                    ODHActivityPoiLinked parsedobject = await ParseDSSDataToODHActivityPoi(item);

                    if (parsedobject != null)
                    {
                        //Add the LocationInfo
                        //TODO if Area can be mapped return locationinfo
                        if (parsedobject.GpsInfo != null && parsedobject.GpsInfo.Count > 0)
                        {
                            if (parsedobject.GpsInfo.FirstOrDefault().Latitude != 0 && parsedobject.GpsInfo.FirstOrDefault().Longitude != 0)
                            {
                                var district = await GetLocationInfo.GetNearestDistrictbyGPS(QueryFactory, parsedobject.GpsInfo.FirstOrDefault().Latitude, parsedobject.GpsInfo.FirstOrDefault().Longitude, 30000);

                                if (district != null)
                                {
                                    var locinfo = await GetLocationInfo.GetTheLocationInfoDistrict(QueryFactory, district.Id);

                                    parsedobject.LocationInfo = locinfo;
                                    parsedobject.TourismorganizationId = locinfo.TvInfo.Id;
                                }
                            }
                        }

                        //Add AreaInfo from DSS skiarea
                        var dssskiarearid = (int?)item["skiresort"]["pid"];
                        if (dssskiarearid != null)
                        {
                            //TODO Select Area which has the mapping to dss/rid and fill AreaId Array and LocationInfo.Area
                            var area = arealist.Where(x => x.Mapping.ContainsKey("dss") && x.Mapping["dss"].ContainsKey("pid") && x.Mapping["dss"]["pid"] == dssskiarearid.ToString()).FirstOrDefault();

                            if (area != null)
                            {
                                parsedobject.AreaId = new List<string>() { area.Id };
                                if (parsedobject.LocationInfo == null)
                                    parsedobject.LocationInfo = new LocationInfoLinked();

                                Dictionary<string, string> areanames = new Dictionary<string, string>();
                                areanames.Add("de", area.Shortname);
                                areanames.Add("it", area.Shortname);
                                areanames.Add("en", area.Shortname);
                                areanames.Add("nl", area.Shortname);
                                areanames.Add("cs", area.Shortname);
                                areanames.Add("pl", area.Shortname);
                                areanames.Add("fr", area.Shortname);
                                areanames.Add("ru", area.Shortname);

                                parsedobject.LocationInfo.AreaInfo = new AreaInfoLinked() { Id = area.Id, Name = areanames };
                            }
                        }

                        //Save parsedobject to DB + Save Rawdata to DB
                        var pgcrudresult = await InsertDataToDB(parsedobject, new KeyValuePair<string, dynamic>((string)item.pid, item));

                        newcounter = newcounter + pgcrudresult.created.Value;
                        updatecounter = updatecounter + pgcrudresult.updated.Value;

                        WriteLog.LogToConsole(parsedobject.Id, "dataimport", "single.dss" + entitytype, new ImportLog() { sourceid = parsedobject.Id, sourceinterface = "dss." + entitytype + "base", success = true, error = "" });
                    }
                    else
                    {
                        WriteLog.LogToConsole(parsedobject.Id, "dataimport", "single.dss" + entitytype, new ImportLog() { sourceid = parsedobject.Id, sourceinterface = "dss." + entitytype + "base", success = false, error = entitytype + " could not be parsed" });
                    }
                }
            }            

            return new UpdateDetail() { created = newcounter, updated = updatecounter, deleted = 0 };
        }

        //Parse the dss interface content
        public async Task<ODHActivityPoiLinked> ParseDSSDataToODHActivityPoi(dynamic dssinput)
        {
            //id
            string odhdssid = "dss_" + dssinput.pid;

            //Get the ODH Item
            var mydssquery = QueryFactory.Query("smgpois")
              .Select("data")
              .Where("id", odhdssid);

            var odhactivitypoiindb = await mydssquery.GetFirstOrDefaultAsObject<ODHActivityPoiLinked>();
            var odhactivitypoi = default(ODHActivityPoiLinked);

            if (entitytype.ToLower() == "lift")
            {
                odhactivitypoi = ParseDSSToODHActivityPoi.ParseDSSLiftDataToODHActivityPoi(odhactivitypoiindb, dssinput);
            }
            else if (entitytype.ToLower() == "slope")
            {
                odhactivitypoi = ParseDSSToODHActivityPoi.ParseDSSSlopeDataToODHActivityPoi(odhactivitypoiindb, dssinput);
            }

            //TODOS all of this stuff, Tags, Categories etc....

            return odhactivitypoi;
        }

        private async Task<PGCRUDResult> InsertDataToDB(ODHActivityPoiLinked odhactivitypoi, KeyValuePair<string, dynamic> dssdata)
        {
            try
            {
                odhactivitypoi.Id = odhactivitypoi.Id?.ToLower();

                //Set LicenseInfo
                odhactivitypoi.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<ODHActivityPoi>(odhactivitypoi, Helper.LicenseHelper.GetLicenseforOdhActivityPoi);

                var rawdataid = await InsertInRawDataDB(dssdata);

                return await QueryFactory.UpsertData<ODHActivityPoiLinked>(odhactivitypoi, "smgpois", rawdataid);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<int> InsertInRawDataDB(KeyValuePair<string, dynamic> dssdata)
        {
            return await QueryFactory.InsertInRawtableAndGetIdAsync(
                        new RawDataStore()
                        {
                            datasource = "dss",
                            importdate = DateTime.Now,
                            raw = JsonConvert.SerializeObject(dssdata.Value),
                            sourceinterface = entitytype + "base",
                            sourceid = dssdata.Key,
                            sourceurl = "http://dss.dev.tinext.net/.rest/json-export/export/",
                            type = "odhactivitypoi-museum"
                        });
        }
    }
}
