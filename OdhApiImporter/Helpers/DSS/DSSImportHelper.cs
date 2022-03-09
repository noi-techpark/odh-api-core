using DataModel;
using SqlKata.Execution;
using System;
using System.Threading;
using System.Threading.Tasks;
using DSS;

namespace OdhApiImporter.Helpers.DSS
{
    public class DSSImportHelper : ImportHelper, IImportHelper
    {
        //private readonly QueryFactory QueryFactory;
        //private readonly ISettings settings;

        //public DSSImportHelper(ISettings settings, QueryFactory queryfactory)
        //{
        //    this.QueryFactory = queryfactory;
        //    this.settings = settings;
        //}

        public DSSImportHelper(ISettings settings, QueryFactory queryfactory, string table) : base(settings, queryfactory, table)
        {

        }

        public Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, CancellationToken cancellationToken = default)
        {
            //Get DSS data
            var result = GetDSSData.GetDSSDataAsync(DSSRequestType.slopebase, settings.DSSConfig.User, settings.DSSConfig.Password, settings.DSSConfig.ServiceUrl);



            throw new NotImplementedException();
        }
    }
}
