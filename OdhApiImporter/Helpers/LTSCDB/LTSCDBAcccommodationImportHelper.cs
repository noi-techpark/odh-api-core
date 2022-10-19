using DataModel;
using SqlKata.Execution;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiImporter.Helpers.LTSCDB
{
    public class LTSCDBAcccommodationImportHelper : ImportHelper, IImportHelper
    {
        public LTSCDBAcccommodationImportHelper(ISettings settings, QueryFactory queryfactory, string table) : base(settings, queryfactory, table)
        {

        }

        public Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, CancellationToken cancellationToken = default)
        {
            //Import the List

            //Import Single Data

            //Deactivate Data

            throw new NotImplementedException();
        }
    }
}
