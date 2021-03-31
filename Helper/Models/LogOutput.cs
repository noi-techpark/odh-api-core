using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class LogOutput<T>
    {
        public string type { get; set; }
        public string id { get; set; }
        public T json { get; set; }
    }
}
