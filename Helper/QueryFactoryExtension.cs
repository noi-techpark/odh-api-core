using DataModel;
using Newtonsoft.Json;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    public static class QueryFactoryExtension
    {
        public static async Task<T> GetObjectSingleAsync<T>(this Query query, CancellationToken cancellationToken = default) where T : notnull
        {
            var result = await query.FirstOrDefaultAsync<JsonRaw>();
            return JsonConvert.DeserializeObject<T>(result.Value) ?? default!;
        }

        public static async Task<IEnumerable<T>> GetObjectListAsync<T>(this Query query, CancellationToken cancellationToken = default) where T : notnull
        {
            var result = await query.GetAsync<JsonRaw>();
            return result.Select(x => JsonConvert.DeserializeObject<T>(x.Value)!) ?? default!;
        }

        //Insert also data in Raw table
        public static async Task<int> InsertInRawtableAndGetIdAsync(this QueryFactory queryfactory, RawDataStore rawData, CancellationToken cancellationToken = default)
        {
            return await queryfactory.Query("rawdata")
                 .InsertGetIdAsync<int>(rawData);
        }

        #region PG Helpers

        public static async Task<PGCRUDResult> UpsertData<T>(this QueryFactory QueryFactory, T data, string table) where T : IIdentifiable, IImportDateassigneable
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "no data");

            //Check if data exists
            var query = QueryFactory.Query(table)
                      .Select("data")
                      .Where("id", data.Id);

            var queryresult = await query.GetAsync<T>();

            string operation = "";

            int createresult = 0;
            int updateresult = 0;

            if (queryresult == null || queryresult.Count() == 0)
            {
                data.FirstImport = DateTime.Now;
                data.LastChange = DateTime.Now;

                createresult = await QueryFactory.Query(table)
                   .InsertAsync(new JsonBData() { id = data.Id, data = new JsonRaw(data) });
                operation = "INSERT";
            }
            else
            {
                data.LastChange = DateTime.Now;

                updateresult = await QueryFactory.Query(table).Where("id", data.Id)
                        .UpdateAsync(new JsonBData() { id = data.Id, data = new JsonRaw(data) });
                operation = "UPDATE";
            }

            return new PGCRUDResult() { id = data.Id, created = createresult, updated = updateresult, deleted = 0, operation = operation };
        }

        private static async Task<PGCRUDResult> DeleteData(this QueryFactory QueryFactory, string id, string table)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException(nameof(id), "No data");

            //Check if data exists
            var query =
                  QueryFactory.Query(table)
                      .Select("data")
                      .Where("id", id);

            var deleteresult = 0;

            if (query == null)
            {
                throw new ArgumentNullException(nameof(query), "No data");
            }
            else
            {
                deleteresult = await QueryFactory.Query(table).Where("id", id)
                        .DeleteAsync();
            }

            return new PGCRUDResult() { id = id, created = 0, updated = 0, deleted = deleteresult, operation = "DELETE" };
        }

        #endregion

        #region RawDataStore

        public static async Task<PGCRUDResult> UpsertData<T>(this QueryFactory QueryFactory, T data, string table, int rawdataid) where T : IIdentifiable, IImportDateassigneable
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "no data");

            //Check if data exists
            var query = QueryFactory.Query(table)
                      .Select("data")
                      .Where("id", data.Id);

            var queryresult = await query.GetAsync<T>();

            string operation = "";

            int createresult = 0;
            int updateresult = 0;

            if (queryresult == null || queryresult.Count() == 0)
            {
                data.FirstImport = DateTime.Now;
                data.LastChange = DateTime.Now;

                createresult = await QueryFactory.Query(table)
                   .InsertAsync(new JsonBDataRaw() { id = data.Id, data = new JsonRaw(data), rawdataid = rawdataid });
                operation = "INSERT";
            }
            else
            {
                data.LastChange = DateTime.Now;

                updateresult = await QueryFactory.Query(table).Where("id", data.Id)
                        .UpdateAsync(new JsonBDataRaw() { id = data.Id, data = new JsonRaw(data), rawdataid = rawdataid });
                operation = "UPDATE";
            }

            return new PGCRUDResult() { id = data.Id, created = createresult, updated = updateresult, deleted = 0, operation = operation };                    
        }


        #endregion

    }


}
