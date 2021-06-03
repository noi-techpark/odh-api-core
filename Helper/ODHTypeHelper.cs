using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class ODHTypeHelper
    {
    
        public static string[] GetAllSearchableODHTypes(bool getall)
        {
            var odhtypes = new List<string>();

            odhtypes.Add("accommodation");
            odhtypes.Add("odhactivitypoi");
            odhtypes.Add("event");
            odhtypes.Add("region");
            odhtypes.Add("skiarea");
            odhtypes.Add("tourismassociation");
            odhtypes.Add("webcam");
            odhtypes.Add("venue");

            if (getall)
            {
                odhtypes.Add("accommodationroom");
                odhtypes.Add("package");
                odhtypes.Add("ltsactivity");
                odhtypes.Add("ltspoi");
                odhtypes.Add("ltsgastronomy");
                odhtypes.Add("measuringpoint");
                odhtypes.Add("article");
                odhtypes.Add("municipality");
                odhtypes.Add("district");
                odhtypes.Add("skiregion");
                odhtypes.Add("eventshort");
                odhtypes.Add("experiencearea");
                odhtypes.Add("metaregion");
                odhtypes.Add("area");
                odhtypes.Add("wineaward");
            }

            return odhtypes.ToArray();
        }

        public static string TranslateType2Table(string odhtype)
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

        public static string TranslateType2Table<T>(T odhtype)
        {
            return odhtype switch
            {
                Accommodation or AccommodationLinked => "accommodations",
                AccoRoom or AccommodationRoomLinked => "accommodationrooms",
                GBLTSActivity or LTSActivityLinked => "activities",
                GBLTSPoi or LTSPoiLinked => "pois",
                Gastronomy or GastronomyLinked => "gastronomies",
                Event or EventLinked => "events",
                ODHActivityPoi or ODHActivityPoiLinked => "smgpois",
                Package or PackageLinked => "packages",
                Measuringpoint or MeasuringpointLinked => "measuringpoints",
                WebcamInfo or WebcamInfoLinked => "webcams",
                Article or ArticlesLinked => "articles",
                DDVenue => "venues",
                EventShort => "eventeuracnoi",
                ExperienceArea or ExperienceAreaLinked => "experienceareas",
                MetaRegion or MetaRegionLinked => "metaregions",
                Region or RegionLinked => "regions",
                Tourismverein or TourismvereinLinked => "tvs",
                Municipality or MunicipalityLinked => "municipalities",
                District or DistrictLinked => "districts",
                SkiArea or SkiAreaLinked => "skiareas",
                SkiRegion or SkiRegionLinked => "skiregions",
                Area or AreaLinked => "areas",
                Wine or WineLinked => "wines",
                _ => throw new Exception("not known odh type")
            };
        }

        public static string TranslateTable2Type(string odhtype)
        {
            return odhtype switch
            {
                "accommodations" => "accommodation",
                "accommodationrooms" => "accommodationroom",
                "activities" => "ltsactivity",
                "pois" => "ltspoi",
                "gastronomies" => "ltsgastronomy",
                "events" => "event",
                "smgpois" => "odhactivitypoi",
                "packages" => "package",
                "measuringpoints" => "measuringpoint",
                "webcams" => "webcam",
                "articles" => "article",
                "venues" => "venue",
                "eventeuracnoi" => "eventshort",
                "experienceareas" => "experiencearea",
                "metaregions" => "metaregion",
                "regions" => "region",
                "tvs" => "tourismassociation",
                "municipalities" => "municipality",
                "districts" => "district",
                "skiareas" => "skiarea",
                "skiregions" => "skiregion",
                "areas" => "area",
                "wines" => "wineaward",
                _ => throw new Exception("not known odh type")
            };
        }


        public static Func<string, string[]> TranslateTypeToSearchField(string odhtype)
        {
            return odhtype switch
            {
                "accommodation" => PostgresSQLWhereBuilder.AccoTitleFieldsToSearchFor,
                "accommodationroom" => PostgresSQLWhereBuilder.AccoRoomNameFieldsToSearchFor,
                "ltsactivity" or "ltspoi" or "ltsgastronomy" or "event" or "odhactivitypoi" or "metaregion" or "region" or "tourismassociation" or "municipality"
                or "district" or "skiarea" or "skiregion" or "article" or "experiencearea"
                => PostgresSQLWhereBuilder.TitleFieldsToSearchFor,
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
