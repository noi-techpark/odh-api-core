using DataModel;
using SqlKata.Execution;
using System;
using System.Threading;
using System.Threading.Tasks;
using DSS;
using System.Collections.Generic;
using DSS.Parser;

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

            //Parse DSS Data
            var parsedresuld = await ParseDSSDataToODHActivityPoi(dssdata[0], dssdata[1]);

            //Insert in DB

            throw new NotImplementedException();
        }

        //Parse the dss interface content
        public async Task<IEnumerable<Tuple<ODHActivityPoiLinked, dynamic>>> ParseDSSDataToODHActivityPoi(dynamic dssinputbase, dynamic dssinputstatus)
        {
            List<Tuple<ODHActivityPoiLinked, dynamic>> myparseddatalist = new List<Tuple<ODHActivityPoiLinked, dynamic>>();

            //interface lastupdate
            var lastupdate = Convert.ToDateTime(dssinputbase.lastUpdate);

            //loop trough items
            foreach (var item in dssinputbase.items)
            {
                //Get the ODH Item
                var mydssquery = QueryFactory.Query("smgpois")
                  .Select("data")
                  .Where("id", "dss_" + item.rid.ToLower());

                var odhactivitypoiindb = await mydssquery.GetFirstOrDefaultAsObject<ODHActivityPoiLinked>();

                var odhactivitypoi = ParseDSSToODHActivityPoi.ParseDSSDataToODHActivityPoi(item, odhactivitypoiindb);
                myparseddatalist.Add(Tuple.Create(odhactivitypoi, item));
            }

            return myparseddatalist;
        }
    }
}
