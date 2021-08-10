using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
    public class JsonBData
    {
        public string? id { get; set; }
        public JsonRaw? data { get; set; }
    }

    public class JsonBDataRaw
    {
        public string id { get; set; }
        public JsonRaw data { get; set; }
        public Int32 rawdataid { get; set; }
    }

    public class RawDataStore
    {
        //public Int64? id { get; set; }
        public string type { get; set; }
        public string datasource { get; set; }
        public string sourceinterface { get; set; }
        public string sourceid { get; set; }
        public string sourceurl { get; set; }
        public DateTime importdate { get; set; }

        public string raw { get; set; }
    }
}

