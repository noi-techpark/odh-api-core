// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper.Extensions;
using Helper.Generic;
using Helper.JsonHelpers;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;


namespace Helper
{
    public static class QueryFactoryExtension
    {
        #region Query Extension Methods Common used

        //Duplicates?

        //public static async Task<T?> GetFirstOrDefaultAsObject<T>(this Query query) {

        //    var rawdata = await query.FirstOrDefaultAsync<JsonRaw>();
        //    return rawdata != null ? JsonConvert.DeserializeObject<T>(rawdata.Value) : default(T);            
        //}

        //public static async Task<IEnumerable<T>> GetAllAsObject<T>(this Query query)
        //{
        //    var rawdatalist = await query.GetAsync<JsonRaw>();
        //    List<T> datalist = new List<T>();

        //    foreach (var rawdata in rawdatalist)
        //    {
        //        var value = JsonConvert.DeserializeObject<T>(rawdata.Value);
        //        if (value != null)
        //            datalist.Add(value);
        //    }
        //    return datalist;
        //}

        #endregion


        //Using Newtonsoft
        public static async Task<T?> GetObjectSingleAsync<T>(this Query query, CancellationToken cancellationToken = default) where T : notnull
        {
            //using this ContractResolver avoids duplicate Lists
            var settings = new JsonSerializerSettings { ContractResolver = new GetOnlyContractResolver() };

            var result = await query.FirstOrDefaultAsync<JsonRaw>();
            return result != null ? JsonConvert.DeserializeObject<T>(result.Value, settings) : default!;
            //return JsonConvert.DeserializeObject<T>(result.Value, settings) ?? default(T);
        }

        //Using System.Text.Json --> producing Exception when single object is not found!
        //public static async Task<T> GetObjectSingleAsyncV2<T>(this Query query, CancellationToken cancellationToken = default) where T : notnull
        //{
        //    var result = await query.FirstOrDefaultAsync<JsonRaw>();
        //    return System.Text.Json.JsonSerializer.Deserialize<T>(result.Value) ?? default!;
        //}

        //Using Newtonsoft
        public static async Task<IEnumerable<T>> GetObjectListAsync<T>(this Query query, CancellationToken cancellationToken = default) where T : notnull
        {
            //using this ContractResolver avoids duplicate Lists
            var settings = new JsonSerializerSettings { ContractResolver = new GetOnlyContractResolver() };

            var result = await query.GetAsync<JsonRaw>();
            return result.Select(x => JsonConvert.DeserializeObject<T>(x.Value, settings)!) ?? default!;
        }

        //Using System.Text.Json
        //public static async Task<IEnumerable<T>> GetObjectListAsyncV2<T>(this Query query, CancellationToken cancellationToken = default) where T : notnull
        //{
        //    var result = await query.GetAsync<JsonRaw>();
        //    return result.Select(x => System.Text.Json.JsonSerializer.Deserialize<T>(x.Value)!) ?? default!;
        //}

        //Insert also data in Raw table
        public static async Task<int> InsertInRawtableAndGetIdAsync(this QueryFactory queryfactory, RawDataStore rawData, CancellationToken cancellationToken = default)
        {
            return await queryfactory.Query("rawdata")
                 .InsertGetIdAsync<int>(rawData);
        }

        #region PG Helpers

        public static async Task<PGCRUDResult> UpsertData<T>(this QueryFactory QueryFactory, T data, string table, string editor, string editsource, bool errorwhendataexists = false, bool errorwhendataisnew = false) where T : IIdentifiable, IImportDateassigneable, IMetaData
        {
            //TODO: What if no id is passed? Generate ID
            //TODO: Id Uppercase or Lowercase depending on table
            //TODO: Shortname population?

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
            int errorresult = 0;

            data.LastChange = DateTime.Now;
            //Setting MetaInfo
            data._Meta = MetadataHelper.GetMetadataobject<T>(data);

            //Setting Editinfo
            data._Meta.UpdateInfo = new UpdateInfo() { UpdatedBy = editor, UpdateSource = editsource };

            if (data.FirstImport == null)
                data.FirstImport = DateTime.Now;

            if (queryresult == null || queryresult.Count() == 0)
            {
                if (errorwhendataisnew)
                    throw new ArgumentNullException(nameof(data.Id), "Id does not exist");

                createresult = await QueryFactory.Query(table)
                   .InsertAsync(new JsonBData() { id = data.Id, data = new JsonRaw(data) });
                operation = "INSERT";
            }
            else
            {   
                if(errorwhendataexists)
                    throw new ArgumentNullException(nameof(data.Id), "Id exists already");

                updateresult = await QueryFactory.Query(table).Where("id", data.Id)
                        .UpdateAsync(new JsonBData() { id = data.Id, data = new JsonRaw(data) });
                operation = "UPDATE";
            }

            if (createresult == 0 && updateresult == 0)
                errorresult = 1;

            return new PGCRUDResult() { id = data.Id, created = createresult, updated = updateresult, deleted = 0, error = errorresult, operation = operation, compareobject = false, objectchanged = null, objectimageschanged = null, pushchannels = null, changes = null };
        }

        public static async Task<PGCRUDResult> UpsertDataDestinationData<T,V>(this QueryFactory QueryFactory, T data, V destinationdata, string table, bool errorwhendataexists = false, bool errorwhendataisnew = false, bool comparedata = false, bool compareimagedata = false) 
            where T : IIdentifiable, IImportDateassigneable, IMetaData, IPublishedOn, IImageGalleryAware, new()
            where V : IIdentifiable, IImportDateassigneable, IMetaData
        {
         
            if (data == null)
                throw new ArgumentNullException(nameof(data), "no data");

            //Check if data exists
            var query = QueryFactory.Query(table)
                      .Select("data")
                      .Where("id", data.Id);

            var queryresult = await query.GetObjectSingleAsync<T>();

            string operation = "";

            int createresult = 0;
            int updateresult = 0;
            int errorresult = 0;
            //bool compareresult = false;
            EqualityResult equalityresult = new EqualityResult() { isequal = false, patch = null };

            bool imagecompareresult = false;
            List<string> channelstopublish = new List<string>();


            data.LastChange = DateTime.Now;
            destinationdata.LastChange = DateTime.Now;
            //Setting MetaInfo
            data._Meta = MetadataHelper.GetMetadataobject<T>(data);
            destinationdata._Meta = MetadataHelper.GetMetadataobject<V>(destinationdata);

            if (data.FirstImport == null)
            {
                data.FirstImport = DateTime.Now;
                destinationdata.FirstImport = DateTime.Now;
            }

            if (queryresult == null)
            {
                if (errorwhendataisnew)
                    throw new ArgumentNullException(nameof(data.Id), "Id does not exist");

                createresult = await QueryFactory.Query(table)
                   .InsertAsync(new JsonBDataDestinationData() { id = data.Id, data = new JsonRaw(data), destinationdata = new JsonRaw(destinationdata) });
                operation = "INSERT";
            }
            else
            {
                //Compare the data
                if (comparedata && queryresult != null)
                    equalityresult = EqualityHelper.CompareClassesTest<T>(queryresult, data, new List<string>() { "LastChange", "_Meta", "FirstImport" }, true);

                //Compare Image Gallery
                if (compareimagedata && queryresult != null)
                    imagecompareresult = EqualityHelper.CompareImageGallery(data.ImageGallery, queryresult.ImageGallery, new List<string>() { });

                //Check if Publishedon List changed and populate channels to publish information
                channelstopublish.AddRange(data.PublishedOn.UnionIfNotNull(queryresult.PublishedOn));                

                if (errorwhendataexists)
                    throw new ArgumentNullException(nameof(data.Id), "Id exists already");

                updateresult = await QueryFactory.Query(table).Where("id", data.Id)
                        .UpdateAsync(new JsonBDataDestinationData() { id = data.Id, data = new JsonRaw(data), destinationdata = new JsonRaw(destinationdata) });
                operation = "UPDATE";
            }

            if (createresult == 0 && updateresult == 0)
                errorresult = 1;

            return new PGCRUDResult() { id = data.Id, created = createresult, updated = updateresult, deleted = 0, error = errorresult, operation = operation, compareobject = comparedata, objectchanged = equalityresult.isequal ? 0 : 1, objectimageschanged = imagecompareresult ? 0 : 1, pushchannels = channelstopublish, changes = equalityresult.patch };
        }
      
        /// <summary>
        /// Deletes the data if it exists by passing the Type the IdStyle (lowercase, uppercase) is derterminated and a check if the data can be converted to this entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="QueryFactory"></param>
        /// <param name="id"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<PGCRUDResult> DeleteData<T>(this QueryFactory QueryFactory, string id, string table) where T : IIdentifiable, IImportDateassigneable, IMetaData, IPublishedOn, new()
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException(nameof(id), "No data");

            var idtodelete = Helper.IdGenerator.CheckIdFromType<T>(id);

            //Check if data exists
            var query =
                  QueryFactory.Query(table)
                      .Select("data")
                      .Where("id", idtodelete);

            var queryresult = await query.GetObjectSingleAsync<T>();

            var deleteresult = 0;
            var errorresult = 0;
            List<string> channelstopublish = new List<string>();

            if (queryresult == null)
            {
                //throw new ArgumentNullException(nameof(query), "No data");
                return new PGCRUDResult() { id = idtodelete, created = 0, updated = 0, deleted = 0, error = 1, operation = "DELETE", changes = 0, compareobject = false, objectchanged = 0, objectimageschanged = 0, pushchannels = channelstopublish };
            }
            else
            {
                if (queryresult.PublishedOn != null)
                    channelstopublish.AddRange(queryresult.PublishedOn);

                deleteresult = await QueryFactory.Query(table).Where("id", idtodelete)
                        .DeleteAsync();
            }

            if (deleteresult == 0)
                errorresult = 1;

            return new PGCRUDResult() { id = idtodelete, created = 0, updated = 0, deleted = deleteresult, error = errorresult, operation = "DELETE", changes = 0, compareobject = false, objectchanged = 0, objectimageschanged = 0, pushchannels = channelstopublish };
        }

        /// <summary>
        /// Deletes the data if it exists, by passing id and table
        /// </summary>
        /// <param name="QueryFactory"></param>
        /// <param name="id"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task<PGCRUDResult> DeleteData(this QueryFactory QueryFactory, string id, string table, bool casesensitive = true)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException(nameof(id), "No data");

            //Check if data exists
            var query =
                  QueryFactory.Query(table)
                      .Select("data")
                      .When(casesensitive, x => x.Where("id", id))
                      .When(!casesensitive, x => x.WhereLike("id", id));

            var deleteresult = 0;
            var errorresult = 0;

            if (query == null)
            {
                throw new ArgumentNullException(nameof(query), "No data");
            }
            else
            {
                deleteresult = await QueryFactory.Query(table)
                        .When(casesensitive, x => x.Where("id", id))
                        .When(!casesensitive, x => x.WhereLike("id", id))
                        .DeleteAsync();
            }

            if (deleteresult == 0)
                errorresult = 1;

            return new PGCRUDResult() { id = id, created = 0, updated = 0, deleted = deleteresult, error = errorresult, operation = "DELETE" };
        }


        /// <summary>
        /// Upsert Data and check if Object has changed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="QueryFactory"></param>
        /// <param name="data"></param>
        /// <param name="table"></param>
        /// <param name="editor"></param>
        /// <param name="editsource"></param>
        /// <param name="errorwhendataexists"></param>
        /// <param name="errorwhendataisnew"></param>
        /// <param name="comparedata"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task<PGCRUDResult> UpsertDataAndCompare<T>(this QueryFactory QueryFactory, T data, string table, string editor, string editsource, bool errorwhendataexists = false, bool errorwhendataisnew = false, bool comparedata = false) where T : IIdentifiable, IImportDateassigneable, IMetaData, IPublishedOn, new()
        {
            //TODO: What if no id is passed? Generate ID
            //TODO: Id Uppercase or Lowercase depending on table
            //TODO: Shortname population?

            if (data == null)
                throw new ArgumentNullException(nameof(data), "no data");

            //Check if data exists
            var query = QueryFactory.Query(table)
                      .Select("data")
                      .Where("id", data.Id);

            var queryresult = await query.GetObjectSingleAsync<T>();       

            string operation = "";

            int createresult = 0;
            int updateresult = 0;
            int errorresult = 0;
            //bool compareresult = false;            
            EqualityResult equalityresult = new EqualityResult() { isequal = false, patch = null };

            List<string> channelstopublish = new List<string>();

            data.LastChange = DateTime.Now;
            //Setting MetaInfo
            data._Meta = MetadataHelper.GetMetadataobject<T>(data);

            //Setting Editinfo
            data._Meta.UpdateInfo = new UpdateInfo() { UpdatedBy = editor, UpdateSource = editsource };

            if (data.FirstImport == null)
                data.FirstImport = DateTime.Now;

            //

            if (queryresult == null)
            {
                if (errorwhendataisnew)
                    throw new ArgumentNullException(nameof(data.Id), "Id does not exist");

                createresult = await QueryFactory.Query(table)
                   .InsertAsync(new JsonBData() { id = data.Id, data = new JsonRaw(data) });
                operation = "INSERT";

                //If data is null let equalityresult.isequal = false, add all the publish channels                
                if (data.PublishedOn != null)
                    channelstopublish.AddRange(data.PublishedOn);
            }
            else
            {
                //Compare the data
                if(comparedata)
                    equalityresult = EqualityHelper.CompareClassesTest<T>(queryresult, data, new List<string>() { "LastChange", "_Meta", "FirstImport" }, true);

                //Add all Publishedonfields before and after change
                channelstopublish.AddRange(data.PublishedOn.UnionIfNotNull(queryresult.PublishedOn));
            
                if (errorwhendataexists)
                    throw new ArgumentNullException(nameof(data.Id), "Id exists already");

                updateresult = await QueryFactory.Query(table).Where("id", data.Id)
                        .UpdateAsync(new JsonBData() { id = data.Id, data = new JsonRaw(data) });
                operation = "UPDATE";
            }

            if (createresult == 0 && updateresult == 0)
                errorresult = 1;

            return new PGCRUDResult() { id = data.Id, created = createresult, updated = updateresult, deleted = 0, error = errorresult, operation = operation, compareobject = comparedata, objectchanged = equalityresult.isequal ? 0 : 1, objectimageschanged = null, pushchannels = channelstopublish, changes = equalityresult.patch };
        }

        /// <summary>
        /// Upsert Data and check if Object and ImageGallery has changed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="QueryFactory"></param>
        /// <param name="data"></param>
        /// <param name="table"></param>
        /// <param name="editor"></param>
        /// <param name="editsource"></param>
        /// <param name="errorwhendataexists"></param>
        /// <param name="errorwhendataisnew"></param>
        /// <param name="comparedata"></param>
        /// <param name="compareimagedata"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task<PGCRUDResult> UpsertDataAndFullCompare<T>(this QueryFactory QueryFactory, T data, string table, string editor, string editsource, bool errorwhendataexists = false, bool errorwhendataisnew = false, bool comparedata = false, bool compareimagedata = false) where T : IIdentifiable, IImportDateassigneable, IMetaData, IImageGalleryAware, IPublishedOn, new()
        {
            //TODO: What if no id is passed? Generate ID
            //TODO: Id Uppercase or Lowercase depending on table
            //TODO: Shortname population?

            if (data == null)
                throw new ArgumentNullException(nameof(data), "no data");

            //Check if data exists
            var query = QueryFactory.Query(table)
                      .Select("data")
                      .Where("id", data.Id);

            //Json.net Deserializer bug, AreaIds,AreaId has duplicate IDs
            var queryresult = await query.GetObjectSingleAsync<T>();            

            string operation = "";

            int createresult = 0;
            int updateresult = 0;
            int errorresult = 0;
            //bool compareresult = false;
            bool imagecompareresult = false;
            EqualityResult equalityresult = new EqualityResult() { isequal = false, patch = null };
            List<string> channelstopublish = new List<string>();

            data.LastChange = DateTime.Now;
            //Setting MetaInfo
            data._Meta = MetadataHelper.GetMetadataobject<T>(data);

            //Setting Editinfo
            data._Meta.UpdateInfo = new UpdateInfo() { UpdatedBy = editor, UpdateSource = editsource };

            if (data.FirstImport == null)
                data.FirstImport = DateTime.Now;

            if (queryresult == null)
            {
                if (errorwhendataisnew)
                    throw new ArgumentNullException(nameof(data.Id), "Id does not exist");

                createresult = await QueryFactory.Query(table)
                   .InsertAsync(new JsonBData() { id = data.Id, data = new JsonRaw(data) });
                operation = "INSERT";

                //If data is null let equalityresult.isequal = false and imagecompareresult = false, and the publish channels
                if (data.PublishedOn != null)
                    channelstopublish.AddRange(data.PublishedOn);
            }
            else
            {
                //Compare the data
                if (comparedata && queryresult != null)
                    equalityresult = EqualityHelper.CompareClassesTest<T>(queryresult, data, new List<string>() { "LastChange", "_Meta", "FirstImport" }, true);
              
                //Compare Image Gallery
                if (compareimagedata && queryresult != null)
                    imagecompareresult = EqualityHelper.CompareImageGallery(data.ImageGallery, queryresult.ImageGallery, new List<string>() { });

                //Add all Publishedonfields before and after change
                channelstopublish.AddRange(data.PublishedOn.UnionIfNotNull(queryresult.PublishedOn));
                
                if (errorwhendataexists)
                    throw new ArgumentNullException(nameof(data.Id), "Id exists already");

                updateresult = await QueryFactory.Query(table).Where("id", data.Id)
                        .UpdateAsync(new JsonBData() { id = data.Id, data = new JsonRaw(data) });

                operation = "UPDATE";
            }

            if (createresult == 0 && updateresult == 0)
                errorresult = 1;

            return new PGCRUDResult() { id = data.Id, created = createresult, updated = updateresult, deleted = 0, error = errorresult, operation = operation, compareobject = comparedata, objectchanged = equalityresult.isequal ? 0 : 1, objectimageschanged = imagecompareresult ? 0 : 1, pushchannels = channelstopublish, changes = equalityresult.patch };
        }

        #endregion

        #region RawDataStore

        public static async Task<PGCRUDResult> UpsertData<T>(this QueryFactory QueryFactory, T data, string table, int rawdataid, string editor, string editsource, bool errorwhendataexists = false) where T : IIdentifiable, IImportDateassigneable, IMetaData
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
            int errorresult = 0;

            data.LastChange = DateTime.Now;
            //Setting MetaInfo
            data._Meta = MetadataHelper.GetMetadataobject<T>(data);

            if (data.FirstImport == null)
                data.FirstImport = DateTime.Now;

            //Setting Editinfo
            data._Meta.UpdateInfo = new UpdateInfo() { UpdatedBy = editor, UpdateSource = editsource };

            if (queryresult == null || queryresult.Count() == 0)
            {
                createresult = await QueryFactory.Query(table)
                   .InsertAsync(new JsonBDataRaw() { id = data.Id, data = new JsonRaw(data), rawdataid = rawdataid });
                operation = "INSERT";
            }
            else
            {
                if (errorwhendataexists)
                    throw new ArgumentNullException(nameof(data), "Id exists already");
                
                updateresult = await QueryFactory.Query(table).Where("id", data.Id)
                        .UpdateAsync(new JsonBDataRaw() { id = data.Id, data = new JsonRaw(data), rawdataid = rawdataid });
                operation = "UPDATE";
            }

            if (createresult == 0 && updateresult == 0)
                errorresult = 1;

            return new PGCRUDResult() { id = data.Id, created = createresult, updated = updateresult, deleted = 0, error = errorresult, operation = operation };                    
        }

        #endregion

    }


}
