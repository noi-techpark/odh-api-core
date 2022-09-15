using DataModel;
using SqlKata.Execution;
using System;
using System.Threading;
using System.Threading.Tasks;
using LOOPTEC;
using Newtonsoft.Json;
using Helper;

namespace OdhApiImporter.Helpers.LOOPTEC
{
    public class LOOPTECImportHelper : ImportHelper, IImportHelper
    {
        public const string serviceurl = @"https://app.onboard-staging.org/exports/v1/jobs/open_data_hub.json";

        public LOOPTECImportHelper(ISettings settings, QueryFactory queryfactory, string table) : base(settings, queryfactory, table)
        {

        }

        public Task<Tuple<int, int>> DeleteOrDisableData(string id, bool delete)
        {
            throw new NotImplementedException();
        }

        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, CancellationToken cancellationToken = default)
        {
            //GET Data and Deserialize to Json
            var data = await GetEJobsData.GetEjobsDataAsync("", "", serviceurl);

            var newcounter = 0;

            //Save to RAWTABLE
            foreach(var ejob in data)
            {
                var rawdataid = await InsertInRawDataDB(ejob);
                newcounter++;
                WriteLog.LogToConsole(rawdataid, "dataimport", "single.ejob", new ImportLog() { sourceid = rawdataid, sourceinterface = "looptec.ejob", success = true, error = "" });
            }

            return new UpdateDetail() { created = newcounter, updated = 0, deleted = 0 };
        }

        private async Task<int> InsertInRawDataDB(dynamic ejob)
        {
            return await QueryFactory.InsertInRawtableAndGetIdAsync(
                        new RawDataStore()
                        {
                            datasource = "looptec",
                            rawformat = "json",
                            importdate = DateTime.Now,
                            license = "open",
                            sourceinterface = "ejobs-onboard",
                            sourceurl = serviceurl,
                            type = "ejob",
                            sourceid = ejob.identifier,
                            raw = JsonConvert.SerializeObject(ejob.Value),
                        });
        }
    }
}
