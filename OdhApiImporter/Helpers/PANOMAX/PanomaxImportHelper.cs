// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using SqlKata.Execution;
using System;
using System.Threading;
using System.Threading.Tasks;
using PANOMAX;
using Newtonsoft.Json;
using Helper;
using System.Collections.Generic;
using DSS;
using OdhApiImporter.Helpers.DSS;
using System.Globalization;
using System.Linq;
using DSS.Parser;
using static Microsoft.FSharp.Core.ByRefKinds;

namespace OdhApiImporter.Helpers
{
    public class PanomaxImportHelper : ImportHelper, IImportHelper
    {
        //TODO Make BaseUrl configurable in settings
        public const string serviceurl = @"https://api.panomax.com/1.0/instances/lists/public";
        public const string serviceurlvideos = @"https://api.panomax.com/1.0/cams/videos/public";

        public List<string> idlistinterface { get; set; }

        public PanomaxImportHelper(ISettings settings, QueryFactory queryfactory, string table, string importerURL) : base(settings, queryfactory, table, importerURL)
        {
            idlistinterface = new List<string>();
        }
     
        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, List<string>? idlist = null, CancellationToken cancellationToken = default)
        {
            //GET Data List Webcams and Videos
            var data = await GetData(cancellationToken);

            //UPDATE all data
            var updateresult = await ImportData(data, cancellationToken);

            //Disable Data not in panomax list
            var deleteresult = await SetDataNotinListToInactive(cancellationToken);

            return GenericResultsHelper.MergeUpdateDetail(new List<UpdateDetail>() { updateresult, deleteresult });
        }

        private async Task<List<dynamic>> GetData(CancellationToken cancellationToken)
        {
            List<dynamic> panomaxdata = new List<dynamic>();

            //Get Panomax webcam data
            panomaxdata.Add(await GetPanomaxData.GetWebcams(serviceurl));
            //Get Panomax video data
            panomaxdata.Add(await GetPanomaxData.GetVideos(serviceurlvideos));

            return panomaxdata;
        }

        public async Task<UpdateDetail> ImportData(List<dynamic> panomaxinput, CancellationToken cancellationToken)
        {
            int updatecounter = 0;
            int newcounter = 0;
            int deletecounter = 0;
            int errorcounter = 0;

            if (panomaxinput != null && panomaxinput.Count > 0)
            {                
                //loop trough dss items
                foreach (var webcam in panomaxinput[0])
                {
                    var importresult = await ImportDataSingle(webcam, panomaxinput[1]);

                    newcounter = newcounter + importresult.created ?? newcounter;
                    updatecounter = updatecounter + importresult.updated ?? updatecounter;
                    errorcounter = errorcounter + importresult.error ?? errorcounter;
                }
            }

            return new UpdateDetail() { created = newcounter, updated = updatecounter, deleted = deletecounter, error = errorcounter };
        }

        public async Task<UpdateDetail> ImportDataSingle(dynamic webcam, dynamic videolist)
        {
            int updatecounter = 0;
            int newcounter = 0;
            int deletecounter = 0;
            int errorcounter = 0;

            //id
            string returnid = "";

            try
            {
                //id
                returnid = webcam.id;

                idlistinterface.Add("panomax_" + returnid);

                //Parse Panomax Webcam Data
                WebcamInfoLinked parsedobject = await ParsePanomaxDataToWebcam("panomax_" + returnid, webcam);
                if (parsedobject == null)
                    throw new Exception();

                //Get Areas to Assign (Areas is a LTS only concept and will be removed in future)

                //Set Shortname
                parsedobject.Shortname = parsedobject.Detail.Select(x => x.Value.Title).FirstOrDefault();

                //assign Videos
                dynamic videostoassign = null;
                foreach(var video in videolist.cams)
                {
                    if (video.id == webcam.camId)
                    {
                        videostoassign = video;
                    }
                }

                var hasvideos = ((IEnumerable<dynamic>)videolist.cams).Where(x => x.id == webcam.camId).FirstOrDefault();

                parsedobject.VideoItems = ParsePanomaxToODH.ParseVideosToVideoItems(parsedobject.VideoItems, videostoassign);

                //Save parsedobject to DB + Save Rawdata to DB
                var pgcrudresult = await InsertDataToDB(parsedobject, new KeyValuePair<string, dynamic>(returnid, webcam));

                newcounter = newcounter + pgcrudresult.created ?? 0;
                updatecounter = updatecounter + pgcrudresult.updated ?? 0;

                WriteLog.LogToConsole(parsedobject.Id, "dataimport", "single.panomax", new ImportLog() { sourceid = parsedobject.Id, sourceinterface = "panomax.webcam", success = true, error = "" });
            }
            catch (Exception ex)
            {
                WriteLog.LogToConsole(returnid, "dataimport", "single.panomax", new ImportLog() { sourceid = returnid, sourceinterface = "panomax.webcam", success = false, error = "panomax webcam could not be parsed" });

                errorcounter = errorcounter + 1;
            }

            return new UpdateDetail() { created = newcounter, updated = updatecounter, deleted = 0, error = errorcounter };
        }

        private async Task<PGCRUDResult> InsertDataToDB(WebcamInfoLinked webcam, KeyValuePair<string, dynamic> panomaxdata)
        {
            var rawdataid = await InsertInRawDataDB(panomaxdata);

            webcam.Id = webcam.Id?.ToLower();

            //Set LicenseInfo
            webcam.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<WebcamInfoLinked>(webcam, Helper.LicenseHelper.GetLicenseforWebcam);

            var pgcrudresult = await QueryFactory.UpsertData<WebcamInfoLinked>(webcam, table, rawdataid, "panomax.webcam.import", importerURL);

            return pgcrudresult;
        }

        private async Task<int> InsertInRawDataDB(KeyValuePair<string, dynamic> panomaxdata)
        {
            return await QueryFactory.InsertInRawtableAndGetIdAsync(
                        new RawDataStore()
                        {
                            datasource = "panomax",
                            rawformat = "json",
                            importdate = DateTime.Now,
                            license = "open",
                            sourceinterface = "webcams",
                            sourceurl = serviceurl,
                            type = "webcam",
                            sourceid = panomaxdata.Key,
                            raw = JsonConvert.SerializeObject(panomaxdata.Value),
                        });
        }
      
        //Parse the panomax interface content
        public async Task<WebcamInfoLinked?> ParsePanomaxDataToWebcam(string odhid, dynamic input)
        {         
            //Get the ODH Item
            var mydssquery = QueryFactory.Query(table)
              .Select("data")
              .Where("id", odhid);

            var webcamindb = await mydssquery.GetObjectSingleAsync<WebcamInfoLinked>();
            var webcam = default(WebcamInfoLinked);

            webcam = ParsePanomaxToODH.ParseWebcamToWebcamInfo(webcamindb, input);

            return webcam;
        }

        private async Task<UpdateDetail> SetDataNotinListToInactive(CancellationToken cancellationToken)
        {
            int updateresult = 0;
            int deleteresult = 0;
            int errorresult = 0;

            try
            {
                //Begin SetDataNotinListToInactive
                var idlistdb = await GetAllPanomaxDataByInterface(new List<string>() { "panomax" });

                var idstodelete = idlistdb.Where(p => !idlistinterface.Any(p2 => p2 == p));

                foreach (var idtodelete in idstodelete)
                {
                    var result = await DeleteOrDisableData<WebcamInfoLinked>(idtodelete, false);

                    updateresult = updateresult + result.Item1;
                    deleteresult = deleteresult + result.Item2;                    
                }
            }
            catch (Exception ex)
            {
                WriteLog.LogToConsole("", "dataimport", "deactivate.panomax", new ImportLog() { sourceid = "", sourceinterface = "panomax.webcam", success = false, error = ex.Message });

                errorresult = errorresult + 1;
            }

            return new UpdateDetail() { created = 0, updated = updateresult, deleted = deleteresult, error = errorresult };
        }

        private async Task<List<string>> GetAllPanomaxDataByInterface(List<string> syncsourceinterfacelist)
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
