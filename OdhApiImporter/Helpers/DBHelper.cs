// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModel;
using SqlKata.Execution;

namespace OdhApiImporter.Helpers
{
    //public static class PGQueryExtensions
    //{
    //    #region PG Helpers

    //    public static async Task<PGCRUDResult> UpsertData<T>(this QueryFactory QueryFactory, T data, string table) where T : IIdentifiable, IImportDateassigneable
    //    {
    //        if (data == null)
    //            throw new ArgumentNullException(nameof(data), "no data");

    //        //Check if data exists
    //        var query = QueryFactory.Query(table)
    //                  .Select("data")
    //                  .Where("id", data.Id);

    //        var queryresult = await query.GetAsync<T>();

    //        string operation = "";

    //        int createresult = 0;
    //        int updateresult = 0;

    //        if (queryresult == null || queryresult.Count() == 0)
    //        {
    //            data.FirstImport = DateTime.Now;
    //            data.LastChange = DateTime.Now;

    //            createresult = await QueryFactory.Query(table)
    //               .InsertAsync(new JsonBData() { id = data.Id, data = new JsonRaw(data) });
    //            operation = "INSERT";
    //        }
    //        else
    //        {
    //            data.LastChange = DateTime.Now;

    //            updateresult = await QueryFactory.Query(table).Where("id", data.Id)
    //                    .UpdateAsync(new JsonBData() { id = data.Id, data = new JsonRaw(data) });
    //            operation = "UPDATE";
    //        }

    //        return new PGCRUDResult() { id = data.Id, created = createresult, updated = updateresult, deleted = 0, operation = operation };
    //    }

    //    private static async Task<PGCRUDResult> DeleteData(this QueryFactory QueryFactory, string id, string table)
    //    {
    //        if (string.IsNullOrEmpty(id))
    //            throw new ArgumentException(nameof(id), "No data");

    //        //Check if data exists
    //        var query =
    //              QueryFactory.Query(table)
    //                  .Select("data")
    //                  .Where("id", id);

    //        var deleteresult = 0;

    //        if (query == null)
    //        {
    //            throw new ArgumentNullException(nameof(query), "No data");
    //        }
    //        else
    //        {
    //            deleteresult = await QueryFactory.Query(table).Where("id", id)
    //                    .DeleteAsync();
    //        }

    //        return new PGCRUDResult() { id = id, created = 0, updated = 0, deleted = deleteresult, operation = "DELETE" };
    //    }

    //    #endregion

    //    #region RawDataStore



    //    #endregion
    //}

    //public struct PGCRUDResult
    //{
    //    public string id { get; init; }
    //    public string operation { get; init; }
    //    public int? updated { get; init; }
    //    public int? created { get; init; }
    //    public int? deleted { get; init; }
    //}
}
