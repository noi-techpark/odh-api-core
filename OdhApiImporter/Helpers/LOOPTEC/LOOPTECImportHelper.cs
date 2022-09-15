using DataModel;
using SqlKata.Execution;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiImporter.Helpers.LOOPTEC
{
    public class LOOPTECImportHelper : ImportHelper, IImportHelper
    {
        public LOOPTECImportHelper(ISettings settings, QueryFactory queryfactory, string table) : base(settings, queryfactory, table)
        {

        }

        public Task<Tuple<int, int>> DeleteOrDisableData(string id, bool delete)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, CancellationToken cancellationToken = default)
        {
            //GET Data

            //Save to RAWTABLE

            throw new NotImplementedException();
        }
    }
}
