using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
    public class JsonBData
    {
        public string id { get; set; }
        public JsonRaw data { get; set; }
    }

    public class JsonBDataRaw
    {
        public string id { get; set; }
        public JsonRaw data { get; set; }
        public string raw { get; set; }
    }

    public class RawData
    {
        public int id { get; set; }
        public string type { get; set; }
        public string source { get; set; }
        public string sourceid { get; set; }
        public DateTime timestamp {get;set;}
    }
}