using CDB;
using DataModel;
using LCS;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OdhApiImporter.Helpers.LTSCDB
{
    public class LTSCDBAcccommodationImportHelper : ImportHelper, IImportHelper
    {
        public LTSCDBAcccommodationImportHelper(ISettings settings, QueryFactory queryfactory, string table) : base(settings, queryfactory, table)
        {

        }

        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, List<string>? idlist = null, CancellationToken cancellationToken = default)
        {
            //Import the List


            //Import Single Data

            //Deactivate Data

            throw new NotImplementedException();
        }

        private async Task<XDocument> ImportList(DateTime? lastchanged)
        {
            return GetAccommodationDataCDB.GetHotelChangedfromCDB(lastchanged, "1", settings.CDBConfig.Username, settings.CDBConfig.Password, settings.CDBConfig.Url);
        }
    }
}
