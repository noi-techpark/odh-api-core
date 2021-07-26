using DataModel;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdhApiImporter.Helpers
{
    public static class PGQueryExtensions
    {
        #region PG Helpers

        public static async Task<string> UpsertData<T>(this QueryFactory QueryFactory, T data, string table) where T : IIdentifiable, IImportDateassigneable
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "no data");

            //Check if data exists
            var query = QueryFactory.Query(table)
                      .Select("data")
                      .Where("id", data.Id);

            var queryresult = await query.GetAsync<T>();

            string operation = "";

            if (queryresult == null || queryresult.Count() == 0)
            {
                data.FirstImport = DateTime.Now;
                data.LastChange = DateTime.Now;

                var insertresult = await QueryFactory.Query(table)
                   .InsertAsync(new JsonBData() { id = data.Id, data = new JsonRaw(data) });
                operation = "INSERT";
            }
            else
            {
                data.LastChange = DateTime.Now;

                var updateresult = await QueryFactory.Query(table).Where("id", data.Id)
                        .UpdateAsync(new JsonBData() { id = data.Id, data = new JsonRaw(data) });
                operation = "UPDATE";
            }

            return String.Format("{0} success: {1}", operation, data.Id);
        }

        private static async Task<string> DeleteData(this QueryFactory QueryFactory, string id, string table)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException(nameof(id), "No data");

            //Check if data exists
            var query =
                  QueryFactory.Query(table)
                      .Select("data")
                      .Where("id", id);

            if (query == null)
            {
                throw new ArgumentNullException(nameof(query), "No data");
            }
            else
            {
                await QueryFactory.Query(table).Where("id", id)
                        .DeleteAsync();
            }

            return String.Format("DELETE success: {0}", id);
        }

        #endregion
    }
}
