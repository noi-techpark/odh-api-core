using DataModel;
using Helper;
using LCS;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OdhApiImporter.Helpers.LTSLCS
{
    public enum LCSEntities
    {
        GastronomicData,
        ActivityData,
        PoiData,
        BeaconData
    }

    public class LCSImportHelper : ImportHelper, IImportHelper
    {
        public LCSEntities EntityType { get; set; }

        public LCSImportHelper(ISettings settings, QueryFactory queryfactory, string table, string importerURL) : base(settings, queryfactory, table, importerURL)
        {

        }

        private async Task<List<string>> ImportList(DateTime? lastchanged, CancellationToken cancellationToken)
        {
            var lcs = new LCS.GetGastronomicDataLCS(settings.LcsConfig.Username, settings.LcsConfig.Password);

            
            if(lastchanged != null)
            {               
                var myrequest = GetLCSRequests.GetGastronomicDataChangedRequestAsync(String.Format("{0:yyyy-MM-dd}", lastchanged), "", "NOI", settings.LcsConfig.MessagePassword);

                var result =  lcs.GetGastronomicDataChanged(myrequest);
            }
            else
            {
                var result2 = GetGastronomicDataListPaged.GetGastronomyListLTS(lcs, 25, "de", settings.LcsConfig.MessagePassword);
            }

            
            WriteLog.LogToConsole("", "dataimport", "list.ltsgastronomy", new ImportLog() { sourceid = "", sourceinterface = "lts.gastronomicdata", success = true, error = "" });

            return new List<string>();
        }


        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, List<string>? idlist = null, CancellationToken cancellationToken = default)
        {
            await ImportList(lastchanged, cancellationToken);

            throw new NotImplementedException();
        }
    }
}
