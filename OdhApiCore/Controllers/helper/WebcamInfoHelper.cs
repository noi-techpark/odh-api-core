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
        //New Publishedonlist
        public List<string> publishedonlist;

        public static WebcamInfoHelper Create(
            string? sourcefilter,
            string? idfilter, bool? activefilter, bool? smgactivefilter, string? lastchange, string? publishedonfilter)
        {
            return new WebcamInfoHelper(
                idfilter: idfilter, sourcefilter: sourcefilter,
                activefilter: activefilter, smgactivefilter: smgactivefilter,
                lastchange: lastchange, publishedonfilter: publishedonfilter);
        }

        private WebcamInfoHelper(
            string? sourcefilter, string? idfilter, bool? activefilter, bool? smgactivefilter, string? lastchange, string? publishedonfilter)
        {
            idlist = Helper.CommonListCreator.CreateIdList(idfilter?.ToUpper());
            sourcelist = Helper.CommonListCreator.CreateIdList(sourcefilter?.ToLower());

            //active
            active = activefilter;

            //smgactive
            smgactive = smgactivefilter;

            this.lastchange = lastchange;

            publishedonlist = Helper.CommonListCreator.CreateIdList(publishedonfilter?.ToLower());
        }


    }
}
