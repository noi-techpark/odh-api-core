using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class Type2Table
    {
        public static string TranslateTypeToTable(string odhtype)
        {
            return odhtype switch
            {
                "accommodation" => "accommodations",
                "accommodationroom" => "accommodationrooms",
                "ltsactivity" => "activities",
                "ltspoi" => "pois",
                "ltsgastronomy" => "gastronomies",
                "event" => "events",
                "odhactivitypoi" => "smgpois",
                "package" => "packages",
                "measuringpoint" => "measuringpoints",
                "webcam" => "webcams",
                "article" => "articles",
                "venue" => "venues",
                "eventshort" => "eventeuracnoi",
                "experiencearea" => "experienceareas",
                "metaregion" => "metaregions",
                "region" => "regions",
                "tourismassociation" => "tvs",
                "municipality" => "municipalities",
                "district" => "districts",
                "skiarea" => "skiareas",
                "skiregion" => "skiregions",
                "area" => "areas",
                "wineaward" => "wines",
                _ => odhtype
            };
        }

    }
}
