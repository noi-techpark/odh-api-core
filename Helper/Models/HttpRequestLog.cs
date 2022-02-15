using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public struct HttpRequestLog
    {
        public string username { get; init; }
        public string referer { get; init; }
        public string origin { get; init; }
        public string host { get; init; }
        public string path { get; init; }
        public string schema { get; init; }
        //public Dictionary<string,string>? querystring { get; set; }
        public string urlparams { get; init; }
        public string useragent { get; init; }
        public string? ipaddress { get; init; }
        public int? statuscode { get; init; }
    }
}
