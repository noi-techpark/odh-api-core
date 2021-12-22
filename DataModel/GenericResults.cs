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

        public string id { get; init; }
    }

    public struct UpdateDetail
    {
        public int? updated { get; init; }
        public int? created { get; init; }
        public int? deleted { get; init; }
    }

    public struct PGCRUDResult
    {
        public string id { get; init; }
        public string operation { get; init; }
        public int? updated { get; init; }
        public int? created { get; init; }
        public int? deleted { get; init; }

        public int? error { get; init; }
    }

    public class GenericResultsHelper
    {
        public static UpdateDetail MergeUpdateDetail(IEnumerable<UpdateDetail> updatedetails)
        {
            int? updated = 0;
            int? created = 0;
            int? deleted = 0;

            foreach(var updatedetail in updatedetails)
            {
                created = updatedetail.created + created;
                updated = updatedetail.updated + updated;
                deleted = updatedetail.deleted + deleted;
            }

            return new UpdateDetail() { created = created, updated = updated, deleted = deleted };
        }
    }
}
