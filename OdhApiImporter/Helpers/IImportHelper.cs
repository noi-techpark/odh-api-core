using DataModel;
using Helper;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiImporter.Helpers
{
    public interface IImportHelper
    {
        Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, CancellationToken cancellationToken = default);

        Task<Tuple<int, int>> DeleteOrDisableData(string id, bool delete);
    }

    public class ImportHelper : IImportHelper
    {
        protected readonly QueryFactory QueryFactory;
        protected readonly ISettings settings;
        protected readonly string table;        

        public ImportHelper(ISettings settings, QueryFactory queryfactory, string table)
        {
            this.QueryFactory = queryfactory;
            this.settings = settings;
            this.table = table;
        }

        //GETs the data from the various sources and saves it to ODH
        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Tuple<int, int>> DeleteOrDisableData(string id, bool delete)
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

                var data = await query.GetFirstOrDefaultAsObject<ODHActivityPoiLinked>();

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

    }
}
