using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class PublishedOnHelper
    {
        public static List<string> GetPublishenOnList(string type, bool smgactive)
        {
            List<string> publishedonlist = new List<string>();

            if (type == "eventshort")
            {
                if (smgactive)
                    publishedonlist.Add("https://noi.bz.it");
            }
            else if(type != "package")
            {
                if (smgactive)
                    publishedonlist.Add("https://www.suedtirol.info");
            }

            //TODO ADD some ifs Create better logic

            return publishedonlist;
        }
    }
}
