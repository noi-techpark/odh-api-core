using DataModel;
using SqlKata.Execution;
using System;
using System.Threading;
using System.Threading.Tasks;
using DSS;
using System.Collections.Generic;

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

        public List<DSSRequestType> requesttypelist { get; set; }

        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, CancellationToken cancellationToken = default)
        {
            List<dynamic> dssdata = new List<dynamic>();

            foreach (var requesttype in requesttypelist)
            {
                //Get DSS data
                dssdata.Add(await GetDSSData.GetDSSDataAsync(requesttype, settings.DSSConfig.User, settings.DSSConfig.Password, settings.DSSConfig.ServiceUrl));
            }

            //TODO Parse DSS Data


            throw new NotImplementedException();
        }
    }
}
