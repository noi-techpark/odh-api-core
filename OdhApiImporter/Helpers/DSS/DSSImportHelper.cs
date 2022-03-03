using DataModel;
using SqlKata.Execution;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiImporter.Helpers.DSS
{
    public class DSSImportHelper : IImportHelper
    {
        private readonly QueryFactory QueryFactory;
        private readonly ISettings settings;

        public DSSImportHelper(ISettings settings, QueryFactory queryfactory)
        {
            this.QueryFactory = queryfactory;
            this.settings = settings;
        }

        public Task<Tuple<int, int>> DeleteOrDisableData(string id, bool delete)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
