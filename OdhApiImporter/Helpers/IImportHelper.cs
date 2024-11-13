// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OdhApiImporter.Helpers
{
    public interface IImportHelper
    {
        Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, List<string>? idlist = null,  CancellationToken cancellationToken = default);

        Task<Tuple<int, int>> DeleteOrDisableData<T>(string id, bool delete) where T : IActivateable;

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

        public async Task<Tuple<int, int>> DeleteOrDisableData<T>(string id, bool delete) where T: IActivateable
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
                    if (data.Active != false || (data is ISmgActive &&  ((ISmgActive)data).SmgActive != false))
                    {
                        data.Active = false;
                        if(data is ISmgActive)
                            ((ISmgActive)data).SmgActive = false;

                        updateresult = await QueryFactory.Query(table).Where("id", id)
                                        .UpdateAsync(new JsonBData() { id = id, data = new JsonRaw(data) });
                    }
                }
            }

            return Tuple.Create(updateresult, deleteresult);
        }

   
        //Helper get all data from source
        public async Task<List<string>> GetAllDataBySource(List<string> syncsourcelist, List<string>? syncsourceinterfacelist = null)
        {
            var query =
               QueryFactory.Query(table)
                   .Select("id")                   
                   .SourceFilter_GeneratedColumn(syncsourcelist)
                   .When(syncsourceinterfacelist != null, x => x.SyncSourceInterfaceFilter_GeneratedColumn(syncsourceinterfacelist));

            var idlist = await query.GetAsync<string>();

            return idlist.ToList();
        }

        public async Task<List<string>> GetAllDataBySourceAndType(List<string> sourcelist, List<string> typelist)
        {

            var query =
               QueryFactory.Query(table)
                   .Select("id")
                   .SourceFilter_GeneratedColumn(sourcelist)
                   .WhereArrayInListOr(typelist, "gen_types");

            var ids = await query.GetAsync<string>();

            return ids.ToList();
        }
    }

    public class ImportUtils
    {
        public static void SaveDataAsJson<T>(T data, string filename, string path)
        {
            //Save to to xml folder Features
            var serializer = new JsonSerializer();
            //Save json
            string fileName = Path.Combine(path, filename + ".json");
            using (var writer = File.CreateText(fileName))
            {
                serializer.Serialize(writer, data);
            }
        }

        public static async Task<T> LoadFromJsonAndDeSerialize<T>(string filename, string path)
        {
            using (StreamReader r = new StreamReader(Path.Combine(path, filename + ".json")))
            {
                string json = await r.ReadToEndAsync();

                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        public static async Task<JArray> LoadFromJsonAndDeSerialize(string filename, string path)
        {
            using (StreamReader r = new StreamReader(Path.Combine(path, filename + ".json")))
            {
                string json = await r.ReadToEndAsync();

                return JArray.Parse(json) ?? new JArray();
            }
        }

        public static async Task<IDictionary<string, JArray>> LoadJsonFiles(string directory, List<string> filenames)
        {
            IDictionary<string, JArray> myjsonfiles = new Dictionary<string, JArray>();
            foreach (string filename in filenames)
                myjsonfiles.Add(filename, await LoadFromJsonAndDeSerialize(filename, directory));

            return myjsonfiles;
        }

        public static IDictionary<string, XDocument> LoadXmlFiles(string directory, List<string> filenames)
        {
            //TODO move this files to Database

            IDictionary<string, XDocument> myxmlfiles = new Dictionary<string, XDocument>();

            foreach(var filename in filenames)
            {
                myxmlfiles.Add(filename, XDocument.Load(directory + filename + ".xml"));

            }            

            return myxmlfiles;
        }
    }
}
