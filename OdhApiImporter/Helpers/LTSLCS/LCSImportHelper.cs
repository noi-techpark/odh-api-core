using DataModel;
using Helper;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

        public Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, List<string>? idlist = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
