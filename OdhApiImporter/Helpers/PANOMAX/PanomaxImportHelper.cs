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

namespace OdhApiImporter.Helpers
{
    public class PanomaxImportHelper : ImportHelper, IImportHelper
    {
        //TODO Make BaseUrl configurable in settings
        public const string serviceurl = @"https://api.panomax.com/1.0/instances/lists/public";
        public const string serviceurlvideos = @"https://api.panomax.com/1.0/cams/videos/public";

        public PanomaxImportHelper(ISettings settings, QueryFactory queryfactory, string table, string importerURL) : base(settings, queryfactory, table, importerURL)
        {

        }

        public async Task<Tuple<int, int>> DeleteOrDisableData(string id, bool delete)
        {
            throw new NotImplementedException();
        }

        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, List<string>? idlist = null, CancellationToken cancellationToken = default)
        {
            //GET Data List
            var data = await GetPanomaxData.GetWebcams(serviceurl);

            var newcounter = 0;

            if(data != null)
            {

                //Save to RAWTABLE
                foreach (var webcam in data)
                {
                    var rawdataid = await InsertInRawDataDB(webcam);
                    newcounter++;

                    //Because a dynamic is passed to the method a dynamic is returned also if int is defined!!! strange behavior of c#
                    string rawdataidstr = rawdataid.ToString();

                    WriteLog.LogToConsole(rawdataidstr, "dataimport", "single.panomax", new ImportLog() { sourceid = rawdataidstr, sourceinterface = "panomax.webcam", success = true, error = "" });
                }
            }            

            return new UpdateDetail() { created = newcounter, updated = 0, deleted = 0, error = 0 };
        }        

        private async Task<int> InsertInRawDataDB(dynamic webcam)
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
                            sourceid = webcam.id,
                            raw = JsonConvert.SerializeObject(webcam),
                        });
        }
    }
}
