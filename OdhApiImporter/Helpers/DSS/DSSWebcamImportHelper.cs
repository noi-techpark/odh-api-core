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
using System.Xml.Linq;
using ServiceReferenceLCS;
using System.Collections;

namespace OdhApiImporter.Helpers.DSS
{
    public class DSSWebcamImportHelper : ImportHelper, IImportHelper
    {        
        public DSSWebcamImportHelper(ISettings settings, QueryFactory queryfactory, string table) : base(settings, queryfactory, table)
        {
            entitytype = "webcam";
            rawonly = true;
            idlistdssinterface = new();
        }

        string entitytype;
        public bool rawonly { get; set; }
        public List<string> idlistdssinterface { get; set; }

        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, CancellationToken cancellationToken = default)
        {
            //GET DATA
            var dssdata = await GetData(cancellationToken);

            //Import Data to rawtable and odh TODO ADD THE Other types
            var updateresult = await ImportData(dssdata, cancellationToken);

            //Disable Data no
            var deleteresult = await SetDataNotinListToInactive(cancellationToken);

            return GenericResultsHelper.MergeUpdateDetail(new List<UpdateDetail>() { updateresult, deleteresult });            
        }

        //Imports DSS Data
        private async Task<List<dynamic>> GetData(CancellationToken cancellationToken)
        {
            return await GetDSSData.GetDSSDataAsync(DSSRequestType.webcams, settings.DSSConfig.User, settings.DSSConfig.Password, settings.DSSConfig.ServiceUrl);            
        }

        public async Task<UpdateDetail> ImportData(List<dynamic> dssinput, CancellationToken cancellationToken)
        {
            int updatecounter = 0;
            int newcounter = 0;
            int deletecounter = 0;
            int errorcounter = 0;

            List<string> idlistdssinterface = new List<string>();

            if (dssinput != null && dssinput.Count > 0)
            {
                string lastupdatestr = dssinput[0].lastUpdate;
                //interface lastupdate
                DateTime.TryParseExact(lastupdatestr, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime lastupdate);
                
                //loop trough dss items
                foreach (var item in dssinput[0].items)
                {
                    var importresult = await ImportDataSingle(item);

                    newcounter = newcounter + importresult.created ?? newcounter;
                    updatecounter = updatecounter + importresult.updated ?? updatecounter;
                    errorcounter = errorcounter + importresult.error ?? errorcounter;
                }
            }

            return new UpdateDetail() { created = newcounter, updated = updatecounter, deleted = deletecounter, error = errorcounter };
        }

        public async Task<UpdateDetail> ImportDataSingle(dynamic item)
        {
            int updatecounter = 0;
            int newcounter = 0;
            int deletecounter = 0;
            int errorcounter = 0;

            //id
            string returnid = "";

            try
            {
                WebcamInfoLinked parsedobject = default(WebcamInfoLinked);

                returnid = item.pid;

                if (!rawonly)
                {
                    //Parse DSS Data
                    parsedobject = await ParseDSSDataToWebcam(item);
                    if (parsedobject == null)
                        throw new Exception();                                              
                }

                var sourceid = (string)DSSImportUtil.GetSourceId(parsedobject, entitytype);

                //TODO GET ID based on item type

                //IF no id is provided timestamp generated WRONG i need a unique identifier to group on!
                if (String.IsNullOrEmpty(sourceid))
                    sourceid = DateTime.Now.ToString("yyyyMMddHHmmssfff");

                //Save parsedobject to DB + Save Rawdata to DB
                var pgcrudresult = await InsertDataToDB(parsedobject, new KeyValuePair<string, dynamic>(sourceid, item));

                newcounter = newcounter + pgcrudresult.created ?? 0;
                updatecounter = updatecounter + pgcrudresult.updated ?? 0;

                WriteLog.LogToConsole(parsedobject?.Id ?? returnid, "dataimport", "single.dss" + entitytype, new ImportLog() { sourceid = parsedobject?.Id ?? returnid, sourceinterface = "dss." + entitytype, success = true, error = "" });
            }
            catch
            {
                WriteLog.LogToConsole(returnid, "dataimport", "single.dss" + entitytype, new ImportLog() { sourceid = returnid, sourceinterface = "dss." + entitytype, success = false, error = entitytype + " could not be parsed" });

                errorcounter = errorcounter + 1;
            }

            return new UpdateDetail() { created = newcounter, updated = updatecounter, deleted = 0, error = errorcounter };
        }
        private async Task<UpdateDetail> SetDataNotinListToInactive(CancellationToken cancellationToken)
{
            int updateresult = 0;
            int deleteresult = 0;
            int errorresult = 0;

            try
            {
                //Begin SetDataNotinListToInactive
                var idlistdb = await GetAllDSSDataByInterface(new List<string>() { "dsswebcam" });

                var idstodelete = idlistdb.Where(p => !idlistdssinterface.Any(p2 => p2 == p));

                foreach (var idtodelete in idstodelete)
                {
                    var deletedisableresult = await DeleteOrDisableData(idtodelete, false);

                    if (deletedisableresult.Item1 > 0)
                        WriteLog.LogToConsole(idtodelete, "dataimport", "single.dss" + entitytype, new ImportLog() { sourceid = idtodelete, sourceinterface = "dss." + entitytype, success = true, error = "" });
                    else if (deletedisableresult.Item2 > 0)
                        WriteLog.LogToConsole(idtodelete, "dataimport", "single.dss" + entitytype, new ImportLog() { sourceid = idtodelete, sourceinterface = "dss." + entitytype, success = true, error = "" });


                    deleteresult = deleteresult + deletedisableresult.Item1 + deletedisableresult.Item2;
                }
            }
            catch (Exception ex)
            {
                WriteLog.LogToConsole("", "dataimport", "deactivate.dss" + entitytype, new ImportLog() { sourceid = "", sourceinterface = "dss." + entitytype, success = false, error = ex.Message });

                errorresult = errorresult + 1;
            }

            return new UpdateDetail() { created = 0, updated = updateresult, deleted = deleteresult, error = errorresult };
        }

        //Parse the dss interface content
        public async Task<WebcamInfoLinked?> ParseDSSDataToWebcam(dynamic dssinput)
        {
            //id
            string odhdssid = "dss_" + dssinput.pid;

            //Get the ODH Item
            var mydssquery = QueryFactory.Query(table)
              .Select("data")
              .Where("id", odhdssid);

            var webcamindb = await mydssquery.GetFirstOrDefaultAsObject<WebcamInfoLinked>();
            var webcam = default(WebcamInfoLinked);

            //TODO
            //webcam = ParseDSSToODHActivityPoi.ParseDSSLiftDataToODHActivityPoi(odhactivitypoiindb, dssinput);            

            return webcam;
        }

        private async Task<PGCRUDResult> InsertDataToDB(WebcamInfoLinked webcam, KeyValuePair<string, dynamic> dssdata)
        {
            var rawdataid = await InsertInRawDataDB(dssdata);

            if (!rawonly)
            {
                webcam.Id = webcam.Id?.ToLower();

                //Set LicenseInfo
                webcam.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<WebcamInfoLinked>(webcam, Helper.LicenseHelper.GetLicenseforWebcam);

                var pgcrudresult = await QueryFactory.UpsertData<WebcamInfoLinked>(webcam, table, rawdataid);

                return pgcrudresult;
            }
            else
                return new PGCRUDResult { created = 1, deleted = 0, updated = 0, error = 0, operation = "RAW_INSERT", id = rawdataid.ToString() };

        }

        private async Task<int> InsertInRawDataDB(KeyValuePair<string, dynamic> dssdata)
        {
            return await QueryFactory.InsertInRawtableAndGetIdAsync(
                        new RawDataStore()
                        {
                            datasource = "dss",
                            importdate = DateTime.Now,
                            raw = JsonConvert.SerializeObject(dssdata.Value),
                            sourceinterface = "webcam",
                            sourceid = dssdata.Key,
                            sourceurl = settings.DSSConfig.ServiceUrl,
                            type = "dss.webcam",
                            license = "open",
                            rawformat = "json"
                        });
        }

        private async Task<List<string>> GetAllDSSDataByInterface(List<string> syncsourceinterfacelist)
        {

            var query =
               QueryFactory.Query(table)
                   .Select("id")
                   .SourceFilter_GeneratedColumn(syncsourceinterfacelist);

            var idlist = await query.GetAsync<string>();

            return idlist.ToList();
        }        
    }
}
