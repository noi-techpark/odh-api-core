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

        public int? error { get; init }
    }
}
