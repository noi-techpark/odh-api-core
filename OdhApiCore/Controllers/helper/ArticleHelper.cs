using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.api
{
    public class ArticleHelper
    {
        public List<string> typelist;
        public List<string> subtypelist;
        public List<string> idlist;
        public List<string> smgtaglist;
        public List<string> languagelist;
        public bool? highlight;
        public bool? active;
        public bool? smgactive;
        public string? lastchange;

        public static ArticleHelper Create(
            string? typefilter, string? subtypefilter, string? idfilter,
            string? languagefilter, bool? highlightfilter, bool? activefilter, bool? smgactivefilter,
            string? smgtags, string? lastchange)
        {
            return new ArticleHelper(typefilter, subtypefilter, idfilter, languagefilter, highlightfilter, activefilter, smgactivefilter, smgtags, lastchange);
        }

        private ArticleHelper(
            string? typefilter, string? subtypefilter, string? idfilter, string? languagefilter,
            bool? highlightfilter, bool? activefilter, bool? smgactivefilter, string? smgtags, string? lastchange)
        {
            typelist = new List<string>();
            int typeinteger = 0;

            if (!String.IsNullOrEmpty(typefilter))
            {
                if (int.TryParse(typefilter, out typeinteger))
                {
                    //Sonderfall wenn alles abgefragt wird um keine unnötige Where zu erzeugen
                    if (typeinteger != 1023)
                        typelist = Helper.ArticleListCreator.CreateArticleTypefromFlag(typefilter);
                }
                else
                {
                    typelist.AddRange(Helper.CommonListCreator.CreateIdList(typefilter));
                }
            }

            int subtypeinteger = 0;

            if (!String.IsNullOrEmpty(subtypefilter))
            {
                if (subtypefilter != "null")
                {
                    if (int.TryParse(subtypefilter, out subtypeinteger))
                    {
                        subtypelist = Helper.ArticleListCreator.CreateArticleSubTypefromFlag(typelist.FirstOrDefault(), subtypefilter);
                    }
                    else
                    {
                        subtypelist = Helper.ArticleListCreator.CreateArticleSubTypeList(typelist.FirstOrDefault(), subtypefilter);
                    }
                }
                else
                    subtypelist = new List<string>();
            }
            else
                subtypelist = new List<string>();


            idlist = Helper.CommonListCreator.CreateIdList(idfilter?.ToUpper());
            languagelist = Helper.CommonListCreator.CreateIdList(languagefilter);

            smgtaglist = Helper.CommonListCreator.CreateIdList(smgtags);

            //highlight
            highlight = highlightfilter;
            //active
            active = activefilter;
            //smgactive
            smgactive = smgactivefilter;

            this.lastchange = lastchange;
        }
    }
}
