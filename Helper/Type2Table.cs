using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class Type2Table
    {
        public static string[] GetAllSearchableODHTypes()
        {
            return new string[]
            {
                "accommodation",
                "accommodationroom",
                "ltsactivity",
                "ltspoi",
                "ltsgastronomy",
                "event",
                "odhactivitypoi",
                "package",
                "measuringpoint",
                "webcam",
                "article",
                "venue",
                "eventshort",
                "experiencearea",
                "metaregion",
                "region",
                "tourismassociation",
                "municipality",
                "district",
                "skiarea",
                "skiregion",
                "area",
                "wineaward"
            };
        }

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
                _ => throw new Exception("not known odh type")
            };
        }

    }

    public class Type2SearchFunction
    {

        public static Func<string, string[]> TranslateTypeToSearchField(string odhtype)
        {
            return odhtype switch
            {
                "accommodation" => PostgresSQLWhereBuilder.AccoTitleFieldsToSearchFor,
                "accommodationroom" => PostgresSQLWhereBuilder.AccoRoomNameFieldsToSearchFor,
                "ltsactivity" or "ltspoi" or "ltsgastronomy" or "event" or "odhactivitypoi" or "metaregion" or "region" or "tourismassociation" or "municipality"
                or "district" or "skiarea" or "skiregion" or "article" or "experiencearea"
                => PostgresSQLWhereBuilder.AccoTitleFieldsToSearchFor,
                //"measuringpoint" => PostgresSQLWhereBuilder.,
                "webcam" => PostgresSQLWhereBuilder.WebcamnameFieldsToSearchFor,
                "venue" => PostgresSQLWhereBuilder.VenueTitleFieldsToSearchFor,
                //"eventshort" => "eventeuracnoi",           
                //"area" => "areas",
                //"wineaward" => "wines",
                _ => throw new Exception("not known odh type")
            };
        }

        public static string TranslateTypeToTitleField(string odhtype, string language)
        {
            return odhtype switch
            {
                "accommodation" => $"AccoDetail.{language}.Name",
                "accommodationroom" => $"AccoRoomDetail.{language}.Name",
                "ltsactivity" or "ltspoi" or "ltsgastronomy" or "event" or "odhactivitypoi" or "metaregion" or "region" or "tourismassociation" or "municipality"
                or "district" or "skiarea" or "skiregion" or "article" or "experiencearea"
                => $"Detail.{language}.Title",
                //"measuringpoint" => PostgresSQLWhereBuilder.,
                "webcam" => $"Webcamname.{language}",
                "venue" => $"attributes.name.{PostgresSQLWhereBuilder.TransformLanguagetoDDStandard(language)}",
                //"eventshort" => "eventeuracnoi",           
                //"area" => "areas",
                //"wineaward" => "wines",
                _ => throw new Exception("not known odh type")
            };
        }
    }
}
