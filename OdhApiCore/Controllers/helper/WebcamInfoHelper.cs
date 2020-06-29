using Helper;
using SqlKata;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    public class WebcamInfoHelper
    {
        public List<string> idlist;
        public List<string> sourcelist;
        public bool? active;
        public bool? smgactive;
        public string? lastchange;

        public static WebcamInfoHelper Create(
            string? sourcefilter,
            string? idfilter, bool? activefilter, bool? smgactivefilter, string? lastchange)
        {
            return new WebcamInfoHelper(
                idfilter: idfilter, sourcefilter: sourcefilter,
                activefilter: activefilter, smgactivefilter: smgactivefilter,
                lastchange: lastchange);
        }

        private WebcamInfoHelper(
            string? sourcefilter, string? idfilter, bool? activefilter, bool? smgactivefilter, string? lastchange)
        {
            idlist = Helper.CommonListCreator.CreateIdList(idfilter?.ToUpper());
            sourcelist = Helper.CommonListCreator.CreateIdList(sourcefilter?.ToLower());

            //active
            active = activefilter;

            //smgactive
            smgactive = smgactivefilter;

            this.lastchange = lastchange;
        }


    }
}
