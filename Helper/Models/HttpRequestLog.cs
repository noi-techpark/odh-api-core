using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class HttpRequestLog
    {
        public string Username { get; set; }
        public string Referer { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }
        public string Schema { get; set; }
        public string Querystring { get; set; }
    }
}
