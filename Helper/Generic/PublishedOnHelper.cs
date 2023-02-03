using DataModel;
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

            //TODO ADD PublishedOn Marketplace Logic

            return publishedonlist;
        }
    }

    public static class PublishOnHelperV2
    {
        public static List<string> GetPublishenOnList<T>(this T mydata, bool smgactive, ICollection<AllowedTags> allowedtags = null) where T : IIdentifiable, IMetaData, IActivateable, ISource, ISmgTags
        {
            Dictionary<string, List<string>> allowedsources = new Dictionary<string, List<string>>()
            {
                { "event", new List<string>(){ "lts" } },
                { "accommodation", new List<string>(){ "lts" } },
                { "odhactivitypoi", new List<string>(){ "lts","suedtirolwein", "archapp" } }
            };


            List<string> publishedonlist = new List<string>();

            switch (mydata._Meta.Type)
            {
                //Accommodations smgactive (Source LTS IDMActive)
                case "accommodation":
                    if (smgactive && allowedsources[mydata._Meta.Type].Contains(mydata._Meta.Source))
                    {
                        publishedonlist.Add("https://www.suedtirol.info");
                        publishedonlist.Add("idm-marketplace");
                    }

                    break;
                //Event Add all Active Events from now
                case "event":
                    if (mydata.Active && allowedsources[mydata._Meta.Type].Contains(mydata._Meta.Source))
                    {
                        publishedonlist.Add("https://www.suedtirol.info");
                        publishedonlist.Add("idm-marketplace");
                    }

                    break;

                //ODHActivityPoi 
                case "odhactivitypoi":

                    if (mydata.Active && allowedsources[mydata._Meta.Type].Contains(mydata._Meta.Source))
                    {
                        //IF category is whitelisted
                        if (allowedtags.Select(x => x.Id).ToList().Intersect(mydata.SmgTags).Count() > 0)
                        {

                            publishedonlist.Add("https://www.suedtirol.info");
                            publishedonlist.Add("idm-marketplace");
                        }
                    }

                    break;

                default:
                    if (smgactive)
                    {
                        publishedonlist.Add("https://www.suedtirol.info");
                        publishedonlist.Add("idm-marketplace");
                    }

                    break;
            }

            return publishedonlist;
        }

    }
}
