// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using SqlKata.Execution;
using System;
using System.Threading;
using System.Threading.Tasks;
using PANOCLOUD;
using Newtonsoft.Json;
using Helper;
using System.Collections.Generic;

namespace OdhApiImporter.Helpers
{
    public class PanocloudImportHelper : ImportHelper, IImportHelper
    {
        //TODO Make BaseUrl configurable in settings
        public const string serviceurl = @"https://backend.panocloud.com/partner-api/api.php?key=be39ad8114f6e9d99414151c62bc9a7fb83dbb69";

        public PanocloudImportHelper(ISettings settings, QueryFactory queryfactory, string table, string importerURL) : base(settings, queryfactory, table, importerURL)
        {

        }

        public async Task<Tuple<int, int>> DeleteOrDisableData(string id, bool delete)
        {
            throw new NotImplementedException();
        }

        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, List<string>? idlist = null, CancellationToken cancellationToken = default)
        {
            //GET Data and Deserialize to Json
            var data = await GetPanocloudData.GetWebcams(serviceurl);

            var newcounter = 0;

            if(data != null)
            {
                //Save to RAWTABLE
                foreach (var webcam in data.LiveCam)
                {
                    var rawdataid = await InsertInRawDataDB(webcam);
                    newcounter++;

                    //Because a dynamic is passed to the method a dynamic is returned also if int is defined!!! strange behavior of c#
                    string rawdataidstr = rawdataid.ToString();

                    WriteLog.LogToConsole(rawdataidstr, "dataimport", "single.panocloud", new ImportLog() { sourceid = rawdataidstr, sourceinterface = "panocloud.webcam", success = true, error = "" });
                }
            }            

            return new UpdateDetail() { created = newcounter, updated = 0, deleted = 0, error = 0 };
        }        

        private async Task<int> InsertInRawDataDB(dynamic webcam)
        {
            return await QueryFactory.InsertInRawtableAndGetIdAsync(
                        new RawDataStore()
                        {
                            datasource = "panocloud",
                            rawformat = "json",
                            importdate = DateTime.Now,
                            license = "open",
                            sourceinterface = "webcams",
                            sourceurl = serviceurl,
                            type = "webcam",
                            sourceid = "panomax" + webcam.lastModifiedUnix,
                            raw = JsonConvert.SerializeObject(webcam),
                        });
        }
    }
}
