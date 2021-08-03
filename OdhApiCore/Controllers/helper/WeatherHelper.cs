using Helper;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    public class WeatherHelper
    {
        public List<string> idlist;
        public List<string> languagelist;
        public string? lastchange;
        public DateTime? datefrom;
        public DateTime? dateto;

        public static Task<WeatherHelper> CreateAsync(
            QueryFactory queryFactory, string? idfilter, string? languagefilter, string? datefrom, string? dateto, string? lastchange,
            CancellationToken cancellationToken)
        {           
            return Task.FromResult(new WeatherHelper(
               idfilter: idfilter, languagefilter: languagefilter, datefromstr: datefrom, datetostr: dateto, lastchange: lastchange));
        }

        private WeatherHelper(
            string? idfilter, string? languagefilter, string? datefromstr, string? datetostr, string? lastchange)
        {           
            idlist = Helper.CommonListCreator.CreateIdList(idfilter?.ToUpper());
            languagelist = Helper.CommonListCreator.CreateIdList(languagefilter?.ToLower());

            datefrom = DateTime.MinValue;
            dateto = DateTime.MaxValue;

            if (!String.IsNullOrEmpty(datefromstr))
                if (datefromstr != "null")
                    datefrom = Convert.ToDateTime(datefromstr);

            if (!String.IsNullOrEmpty(datetostr))
                if (datetostr != "null")
                    dateto = Convert.ToDateTime(datetostr);
        }


    }
}
