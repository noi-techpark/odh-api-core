using DataModel;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiImporter.Helpers.LTSCDB
{
    public class LTSCDBEventImportHelper : ImportHelper, IImportHelper
    {
        public LTSCDBEventImportHelper(ISettings settings, QueryFactory queryfactory, string table) : base(settings, queryfactory, table)
        {

        }

        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, List<string>? idlist = null, CancellationToken cancellationToken = default)
        {
            //Import the List

            //Import Single Data

            //Deactivate Data

            throw new NotImplementedException();
        }
    }
}
