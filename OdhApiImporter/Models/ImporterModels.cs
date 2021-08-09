using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdhApiImporter.Models
{
    public struct UpdateResult
    {
        public string operation { get; init; }
        public string updatetype { get; init; }
        public string message { get; init; }
        public bool success { get; init; }
        public string recordsupdated { get; init; }

        public string id { get; init; }
    }
}
