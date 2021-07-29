using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdhApiImporter.Models
{
    public class UpdateResult
    {
        public string operation { get; set; }
        public string updatetype { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public string recordsupdated { get; set; }

        public string id { get; set; }
    }
}
