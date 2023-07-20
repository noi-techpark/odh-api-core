// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using SqlKata.Execution;
using System;
using System.Threading;
using System.Threading.Tasks;
using FERATEL;
using Newtonsoft.Json;
using Helper;
using System.Collections.Generic;

namespace OdhApiImporter.Helpers
{
    public class FeratelWebcamImportHelper : ImportHelper, IImportHelper
    {
        //TODO Make BaseUrl configurable in settings
        public const string serviceurl = @"http://wtvxmlp.feratel.com/xmlpan/x3/infoxml.jsp?pg=CDB9645D-E67B-44D2-9FC9-E1539FF9A6B7&lg=de&showKeywords=1&geoXY=1&xmlv3=1&nolg=1";

        public FeratelWebcamImportHelper(ISettings settings, QueryFactory queryfactory, string table, string importerURL) : base(settings, queryfactory, table, importerURL)
        {

        }

        public async Task<Tuple<int, int>> DeleteOrDisableData(string id, bool delete)
        {
            throw new NotImplementedException();
        }

        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, List<string>? idlist = null, CancellationToken cancellationToken = default)
        {
            //GET Data and Deserialize to Json
            var data = await GetFeratelData.GetWebcams(serviceurl);

            var newcounter = 0;

            if(data != null && data.Root.Element("content") != null && data.Root.Element("content").Element("portal") != null)
            {
                //Save to RAWTABLE
                foreach (var webcam in data.Element("content").Element("portal").Elements("link"))
                {
                    var rawdataid = await InsertInRawDataDB(webcam);
                    newcounter++;

                    //Because a dynamic is passed to the method a dynamic is returned also if int is defined!!! strange behavior of c#
                    string rawdataidstr = rawdataid.ToString();

                    WriteLog.LogToConsole(rawdataidstr, "dataimport", "single.feratel", new ImportLog() { sourceid = rawdataidstr, sourceinterface = "feratel.webcam", success = true, error = "" });
                }
            }            

            return new UpdateDetail() { created = newcounter, updated = 0, deleted = 0, error = 0 };
        }        

        private async Task<int> InsertInRawDataDB(dynamic webcam)
        {
            return await QueryFactory.InsertInRawtableAndGetIdAsync(
                        new RawDataStore()
                        {
                            datasource = "feratel",
                            rawformat = "json",
                            importdate = DateTime.Now,
                            license = "open",
                            sourceinterface = "webcams",
                            sourceurl = serviceurl,
                            type = "webcam",
                            sourceid = webcam.identifier,
                            raw = JsonConvert.SerializeObject(webcam),
                        });
        }
    }
}
