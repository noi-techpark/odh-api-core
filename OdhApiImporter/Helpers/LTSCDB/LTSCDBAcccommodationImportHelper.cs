// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using CDB;
using DataModel;
using Helper;
using LCS;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OdhApiImporter.Helpers.LTSCDB
{
    public class LTSCDBAcccommodationImportHelper : ImportHelper, IImportHelper
    {
        public LTSCDBAcccommodationImportHelper(ISettings settings, QueryFactory queryfactory, string table, string importerURL) : base(settings, queryfactory, table, importerURL)
        {

        }

        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, List<string>? idlist = null, CancellationToken cancellationToken = default)
        {
            //Import the List
            if(idlist == null)
                idlist = await ImportList(lastchanged);

            //Import Single Data
            foreach(var data in idlist)
            {

            }

            //Deactivate Data

            throw new NotImplementedException();
        }

        private async Task<List<string>> ImportList(DateTime? lastchanged)
        {
            var importlist = GetAccommodationDataCDB.GetHotelChangedfromCDB(lastchanged, "1", settings.CDBConfig.Username, settings.CDBConfig.Password, settings.CDBConfig.Url);

            var idlist = new List<string>();

            if (lastchanged != null)
            {
                idlist = (from x in importlist.Root.Element("Head").Elements("Data")
                             where Convert.ToDateTime(x.Attribute("A0CoC").Value) >= DateTime.Now.AddDays(-1).Date
                             select x.Attribute("A0RID").Value).ToList();
            }
            else
            {
                idlist = (from x in importlist.Root.Element("Head").Elements("Data")
                             select x.Attribute("A0RID").Value).ToList();
            }

            return idlist;
        }
    }
}
