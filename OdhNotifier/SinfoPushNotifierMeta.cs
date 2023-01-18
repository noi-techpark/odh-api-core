using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdhNotifier
{
    public class SinfoPushNotifierMeta : NotifyMeta
    {
        public SinfoPushNotifierMeta()
        {
            this.Destination = "sinfo";

            this.ValidTypes = new List<string>()
            {
                "accommodation",
                "activity",
                "article",
                "event",
                "metaregion",
                "region",
                "experiencearea",
                "municipality",
                "tvs",
                "district",
                "skiregion",
                "skiarea",
                "gastronomy",
                "odhactivitypoi",
                "smgtags"
            };

        }
    }
}
