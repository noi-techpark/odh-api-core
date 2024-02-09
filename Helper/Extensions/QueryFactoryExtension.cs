// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper.Extensions;
using Helper.Generic;
using Helper.Identity;
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

        #region PG CRUD Helpers

      
        /// <summary>
        /// Inserts or Updates the Data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="QueryFactory"></param>
        /// <param name="data"></param>
        /// <param name="dataconfig"></param>
        /// <param name="editinfo"></param>
        /// <param name="constraints"></param>
        /// <param name="compareConfig"></param>
        /// <returns></returns>
        public static async Task<PGCRUDResult> UpsertData<T>(this QueryFactory QueryFactory, T data, DataInfo dataconfig, EditInfo editinfo, CRUDConstraints constraints, CompareConfig compareConfig) where T : IIdentifiable, IImportDateassigneable, IMetaData, new()
        {
            //TOCHECK: What if no id is passed? Generate ID?
            //TOCHECK: Id Uppercase or Lowercase depending on table
            //TOCHECK: Shortname population?

            List<string> channelstopublish = new List<string>();
            int? objectchangedcount = null;
            int? objectimagechangedcount = null;


            //If no data is passed return error
            if (data == null)
                return new PGCRUDResult() { id = "", odhtype = "", created = 0, updated = 0, deleted = 0, error = 1, errorreason = "No Data", operation = dataconfig.Operation.ToString(), changes = 0, compareobject = false, objectchanged = 0, objectimagechanged = 0, pushchannels = channelstopublish };


            //Check if data exists already
            var queryresult = await QueryFactory.Query(dataconfig.Table)
                      .Select("data")
                      .Where("id", data.Id)
                      .When(constraints.AccessRole.Count() > 0, q => q.FilterDataByAccessRoles(constraints.AccessRole))
                      .GetObjectSingleAsync<T>();

            int createresult = 0;
            int updateresult = 0;
            int errorresult = 0;
            string errorreason = "";

         
            bool imagesequal = false;
            EqualityResult equalityresult = new EqualityResult() { isequal = false, patch = null };

            //Setting LastChange
            data.LastChange = DateTime.Now;
            //Setting MetaInfo
            data._Meta = MetadataHelper.GetMetadataobject<T>(data);
            //Setting Editinfo
            data._Meta.UpdateInfo = new UpdateInfo() { UpdatedBy = editinfo.Editor, UpdateSource = editinfo.Source };
            //Setting Firstimport
            if (data.FirstImport == null)
                data.FirstImport = DateTime.Now;

            //Todo setting Shortname

            //Check data condition return not allowed if it fails
            if (!CheckCRUDCondition.CRUDOperationAllowed(data, constraints.Condition))
            {
                return new PGCRUDResult() { id = data.Id, odhtype = data._Meta.Type, created = 0, updated = 0, deleted = 0, error = 1, errorreason = "Not Allowed", operation = dataconfig.Operation.ToString(), changes = null, compareobject = false, objectchanged = objectchangedcount, objectimagechanged = objectimagechangedcount, pushchannels = channelstopublish };
            }

            if (queryresult == null)
            {
                if (dataconfig.ErrorWhendataIsNew)
                    return new PGCRUDResult() { id = data.Id, odhtype = data._Meta.Type, created = 0, updated = 0, deleted = 0, error = 1, errorreason = "Not Found", operation = dataconfig.Operation.ToString(), changes = null, compareobject = false, objectchanged = objectchangedcount, objectimagechanged = objectimagechangedcount, pushchannels = channelstopublish };

                createresult = await QueryFactory.Query(dataconfig.Table)
                   .InsertAsync(new JsonBData() { id = data.Id, data = new JsonRaw(data) });

                dataconfig.Operation = CRUDOperation.Create;

                if (data is IPublishedOn)
                    channelstopublish.AddRange((data as IPublishedOn).PublishedOn);  
                
                //On insert always set the object and image to changed
                objectchangedcount = 1;
                objectimagechangedcount = 1;
            }
            else
            {
                if (dataconfig.ErrorWhendataExists)
                    return new PGCRUDResult() { id = data.Id, odhtype = data._Meta.Type, created = 0, updated = 0, deleted = 0, error = 1, errorreason = "Bad Request", operation = dataconfig.Operation.ToString(), changes = null, compareobject = false, objectchanged = objectchangedcount, objectimagechanged = objectimagechangedcount, pushchannels = channelstopublish };

                //Compare the data
                if (compareConfig.CompareData && queryresult != null)
                {
                    equalityresult = EqualityHelper.CompareClassesTest<T>(queryresult, data, new List<string>() { "LastChange", "_Meta", "FirstImport" }, true);
                    if (equalityresult.isequal)
                        objectchangedcount = 0;
                    else
                        objectchangedcount = 1;
                }


                //Compare Image Gallery Check if this works with a cast to IImageGalleryAware
                if (compareConfig.CompareImages && queryresult != null && data is IImageGalleryAware && queryresult is IImageGallery)
                {
                    imagesequal = EqualityHelper.CompareImageGallery((data as IImageGalleryAware).ImageGallery, (queryresult as IImageGalleryAware).ImageGallery, new List<string>() { });
                    if (imagesequal)
                        objectimagechangedcount = 0;
                    else
                        objectimagechangedcount = 1;
                }

                //Add all Publishedonfields before and after change
                if (data is IPublishedOn && queryresult is IPublishedOn)
                    channelstopublish.AddRange((data as IPublishedOn).PublishedOn.UnionIfNotNull((queryresult as IPublishedOn).PublishedOn));

                updateresult = await QueryFactory
                    .Query(dataconfig.Table)
                    .Where("id", data.Id)                    
                    .UpdateAsync(new JsonBData() { id = data.Id, data = new JsonRaw(data) });

                dataconfig.Operation = CRUDOperation.Update;
            }

            if (createresult == 0 && updateresult == 0)
                return new PGCRUDResult() { id = data.Id, odhtype = data._Meta.Type, created = 0, updated = 0, deleted = 0, error = 1, errorreason = "Internal Error", operation = dataconfig.Operation.ToString(), changes = null, compareobject = false, objectchanged = objectchangedcount, objectimagechanged = objectimagechangedcount, pushchannels = channelstopublish };

            return new PGCRUDResult() { id = data.Id, odhtype = data._Meta.Type, created = createresult, updated = updateresult, deleted = 0, error = errorresult, errorreason = errorreason, operation = dataconfig.Operation.ToString(), compareobject = compareConfig.CompareData, objectchanged = objectchangedcount, objectimagechanged = objectimagechangedcount, pushchannels = channelstopublish, changes = equalityresult.patch };
        }


       /// <summary>
       /// Deletes the data
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="QueryFactory"></param>
       /// <param name="id"></param>
       /// <param name="dataconfig"></param>
       /// <param name="condition"></param>
       /// <returns></returns>
        public static async Task<PGCRUDResult> DeleteData<T>(this QueryFactory QueryFactory, string id, DataInfo dataconfig, CRUDConstraints constraints) where T : IIdentifiable, IImportDateassigneable, IMetaData
        {
            List<string> channelstopublish = new List<string>();

            if (string.IsNullOrEmpty(id))
                return new PGCRUDResult() { id = "", odhtype = "", created = 0, updated = 0, deleted = 0, error = 1, errorreason = "Bad Request", operation = dataconfig.Operation.ToString(), changes = null, compareobject = false, objectchanged = null, objectimagechanged = null, pushchannels = channelstopublish };

            var idtodelete = Helper.IdGenerator.CheckIdFromType<T>(id);

            //Check if data exists
            var queryresult = await QueryFactory.Query(dataconfig.Table)
                      .Select("data")
                      .Where("id", idtodelete)
                      .When(constraints.AccessRole.Count() > 0, q => q.FilterDataByAccessRoles(constraints.AccessRole))
                      .GetObjectSingleAsync<T>();

            var deleteresult = 0;
            var errorreason = "";

            if (queryresult == null)
            {
                return new PGCRUDResult() { id = idtodelete, odhtype = null, created = 0, updated = 0, deleted = 0, error = 1, errorreason = "Not Found", operation = dataconfig.Operation.ToString(), changes = null, compareobject = false, objectchanged = null, objectimagechanged = null, pushchannels = channelstopublish };
            }
            else
            {
                //Check data condition
                if (!CheckCRUDCondition.CRUDOperationAllowed(queryresult, constraints.Condition))
                {
                    return new PGCRUDResult() { id = idtodelete, odhtype = queryresult._Meta.Type, created = 0, updated = 0, deleted = 0, error = 1, errorreason = "Not Allowed", operation = dataconfig.Operation.ToString(), changes = null, compareobject = false, objectchanged = null, objectimagechanged = null, pushchannels = channelstopublish };
                }

                if (queryresult is IPublishedOn && ((IPublishedOn)queryresult).PublishedOn != null)
                    channelstopublish.AddRange(((IPublishedOn)queryresult).PublishedOn);

                deleteresult = await QueryFactory.Query(dataconfig.Table).Where("id", idtodelete)
                        .DeleteAsync();
            }

            if (deleteresult == 0)
                return new PGCRUDResult() { id = idtodelete, odhtype = queryresult._Meta.Type, created = 0, updated = 0, deleted = 0, error = 1, errorreason = "Internal Error", operation = dataconfig.Operation.ToString(), changes = null, compareobject = false, objectchanged = null, objectimagechanged = null, pushchannels = channelstopublish };

            return new PGCRUDResult() { id = idtodelete, odhtype = queryresult._Meta.Type, created = 0, updated = 0, deleted = deleteresult, error = 0, errorreason = errorreason, operation = dataconfig.Operation.ToString(), changes = null, compareobject = false, objectchanged = null, objectimagechanged = null, pushchannels = channelstopublish };
        }


        //TODO
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

            return new PGCRUDResult() { id = data.Id, created = createresult, updated = updateresult, deleted = 0, error = errorresult, operation = operation, compareobject = comparedata, objectchanged = equalityresult.isequal ? 0 : 1, objectimagechanged = imagecompareresult ? 0 : 1, pushchannels = channelstopublish, changes = equalityresult.patch };
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
