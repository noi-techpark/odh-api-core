using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public struct UpdateResult
    {
        public string operation { get; init; }
        public string updatetype { get; init; }
        public string otherinfo { get; init; }
        public string message { get; init; }
        public bool success { get; init; }
        public int? recordsmodified { get; init; }

        public int? updated { get; init; }
        public int? created { get; init; }
        public int? deleted { get; init; }

        public int? objectcompared { get; init; }
        public int? objectchanged { get; init; }

        public IDictionary<string,string>? pushed { get; init; }

        public int? error { get; init; }

        public string id { get; init; }

        public string exception { get; init; }

        public string stacktrace { get; init; }

        public string source { get; init; }
    }

    public struct UpdateDetail
    {
        //Crud
        public int? updated { get; init; }
        public int? created { get; init; }
        public int? deleted { get; init; }

        //Error
        public int? error { get; init; }

        //Comparision
        public bool? compareobject { get; init; }
        public int? objectchanged { get; init; }
        public int? objectimagechanged { get; init; }

        //Push Info
        public IDictionary<string,string> pushed { get; init; }
    }

    public struct PGCRUDResult
    {
        public string id { get; init; }
        public string operation { get; init; }
        public int? updated { get; init; }
        public int? created { get; init; }
        public int? deleted { get; init; }

        public int? error { get; init; }

        public bool? compareobject { get; init; }
        public int? objectchanged { get; init; }
        public int? objectimageschanged { get; init; }
    }

    public class GenericResultsHelper
    {
        public static UpdateDetail MergeUpdateDetail(IEnumerable<UpdateDetail> updatedetails)
        {
            int? updated = 0;
            int? created = 0;
            int? deleted = 0;
            int? error = 0;
            bool? compareobject = false;
            int? objectchanged = 0;
            int? objectimagechanged = 0;

            IDictionary<string,string> pushed = new Dictionary<string,string>();

            if(updatedetails.Any(x => x.compareobject != null && x.compareobject == true))
                compareobject = true;

            foreach (var updatedetail in updatedetails)
            {
                created = updatedetail.created + created;
                updated = updatedetail.updated + updated;
                deleted = updatedetail.deleted + deleted;
                error = updatedetail.error + error;
                objectchanged = updatedetail.objectchanged + objectchanged;
                objectimagechanged = updatedetail.objectimagechanged + objectimagechanged;

                if (updatedetail.pushed != null)
                {
                    foreach (var updatedetailpushed in updatedetail.pushed)
                        pushed.TryAdd(updatedetailpushed.Key, updatedetailpushed.Value);
                }
            }

            return new UpdateDetail() { created = created, updated = updated, deleted = deleted, error= error, compareobject = compareobject, objectchanged = objectchanged, objectimagechanged = objectimagechanged, pushed = pushed };
        }

        public static UpdateResult GetSuccessUpdateResult(string id, string source, string operation, string updatetype, string message, string otherinfo, UpdateDetail detail, bool createlog)
        {
            var result = new UpdateResult()
            {
                id = id,
                source = source,
                operation = operation,
                updatetype = updatetype,
                otherinfo = otherinfo,
                message = message,
                recordsmodified = (detail.created + detail.updated + detail.deleted),
                created = detail.created,
                updated = detail.updated,
                deleted = detail.deleted,
                objectchanged = detail.objectchanged ,
                objectcompared = detail.compareobject == null ? 0 : detail.compareobject.Value ? 1 : 0,
                pushed = detail.pushed,
                error = detail.error,
                success = true,
                exception = null,
                stacktrace = null
            };

            if(createlog)
                Console.WriteLine(JsonConvert.SerializeObject(result));

            return result;
        }

        public static UpdateResult GetErrorUpdateResult(string id, string source, string operation, string updatetype, string message, string otherinfo, UpdateDetail detail, Exception ex, bool createlog)
        {    
            var result = new UpdateResult()
            {
                id = id,
                source = source,
                operation = operation,
                updatetype = updatetype,
                otherinfo = "",
                message = message,
                recordsmodified = (detail.created + detail.updated + detail.deleted),
                created = detail.created,
                updated = detail.updated,
                deleted = detail.deleted,
                error = detail.error,
                success = false,
                exception = ex.Message,
                stacktrace = ex.StackTrace
            };

            if (createlog)
                Console.WriteLine(JsonConvert.SerializeObject(result));

            return result;
        }
    }
}
