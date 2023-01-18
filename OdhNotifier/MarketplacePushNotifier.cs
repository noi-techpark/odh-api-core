using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdhNotifier
{
    public class MarketplacePushNotifierMeta : NotifyMeta
    {
        public MarketplacePushNotifierMeta()
        {
            this.Destination = "marketplace";
            this.Mode = "post";

            this.ValidTypes = new List<string>()
            {
                "ACCOMMODATION",
                "REGION",
                "MUNICIPALITY",
                "DISTRICT",
                "TOURISM_ASSOCIATION",
                "ODH_ACTIVITY_POI",
                "EVENT",
                "ODH_TAG"
            };

        }
    }
}
