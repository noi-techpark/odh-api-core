// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OdhApiImporter.Helpers
{
    public interface IImportHelper
    {
        Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, List<string>? idlist = null,  CancellationToken cancellationToken = default);

        Task<Tuple<int, int>> DeleteOrDisableData<T>(string id, bool delete) where T : IActivateable,ISmgActive;

        //Task<UpdateDetail> ImportData(ImportObject importobject, CancellationToken cancellationToken);
    }

    public class ImportHelper
    {
        protected readonly QueryFactory QueryFactory;
        protected readonly ISettings settings;
        protected readonly string table;
        protected readonly string importerURL;

        public ImportHelper(ISettings settings, QueryFactory queryfactory, string table, string importerURL)
        {
            this.QueryFactory = queryfactory;
            this.settings = settings;
            this.table = table;
            this.importerURL = importerURL;
        }

        public async Task<Tuple<int, int>> DeleteOrDisableData<T>(string id, bool delete) where T: IActivateable, ISmgActive
        {
            var deleteresult = 0;
            var updateresult = 0;

            if (delete)
            {
                deleteresult = await QueryFactory.Query(table).Where("id", id)
                    .DeleteAsync();
            }
            else
            {
                var query =
               QueryFactory.Query(table)
                   .Select("data")
                   .Where("id", id);

                var data = await query.GetObjectSingleAsync<T>();

                if (data != null)
                {
                    if (data.Active != false || data.SmgActive != false)
                    {
                        data.Active = false;
                        data.SmgActive = false;

                        updateresult = await QueryFactory.Query(table).Where("id", id)
                                        .UpdateAsync(new JsonBData() { id = id, data = new JsonRaw(data) });
                    }
                }
            }

            return Tuple.Create(updateresult, deleteresult);
        }

        //Helper get all data from source
        public async Task<List<string>> GetAllDataBySource(List<string> syncsourceinterfacelist)
        {
            var query =
               QueryFactory.Query(table)
                   .Select("id")                   
                   .SourceFilter_GeneratedColumn(syncsourceinterfacelist);

            var idlist = await query.GetAsync<string>();

            return idlist.ToList();
        }
    }

    //public class ImportObject
    //{
    //    public XDocument XdocumentList { get; set; }
    //    public Dictionary<string, XDocument> XdocumentDictionary { get; set; }
    //}
}
