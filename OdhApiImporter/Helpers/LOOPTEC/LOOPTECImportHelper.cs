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

        public async Task<Tuple<int, int>> DeleteOrDisableData(string id, bool delete)
        {
            throw new NotImplementedException();
        }

        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, CancellationToken cancellationToken = default)
        {
            //GET Data and Deserialize to Json
            var data = await GetEJobsData.GetEjobsDataAsync("", "", serviceurl);

            var newcounter = 0;

            if(data != null)
            {
                //Save to RAWTABLE
                foreach (var ejob in data.jobs)
                {
                    var rawdataid = await InsertInRawDataDB(ejob);
                    newcounter++;

                    //Because a dynamic is passed to the method a dynamic is returned also if int is defined!!! strange behavior of c#
                    string rawdataidstr = rawdataid.ToString();

                    WriteLog.LogToConsole(rawdataidstr, "dataimport", "single.ejob", new ImportLog() { sourceid = rawdataidstr, sourceinterface = "looptec.ejob", success = true, error = "" });
                }
            }            

            return new UpdateDetail() { created = newcounter, updated = 0, deleted = 0, error = 0 };
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
                            raw = JsonConvert.SerializeObject(ejob),
                        });
        }
    }
}
