using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class HttpRequestLog
    {
        public string username { get; set; }
        public string referer { get; set; }
        public string host { get; set; }
        public string path { get; set; }
        public string schema { get; set; }
        //public Dictionary<string,string>? querystring { get; set; }
        public string urlparams { get; set; }
        public string useragent { get; set; }
        public string ipaddress { get; set; }
    }
}
