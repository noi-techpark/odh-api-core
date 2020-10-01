using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace DataModel
{
    public class JsonBData
    {
        public string id { get; set; }
        public JToken data { get; set; }
    }
}
