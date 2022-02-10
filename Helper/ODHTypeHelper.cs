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

        /// <summary>
        /// Translates a ODH Type to the Type as String
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="odhtype"></param>
        /// <returns></returns>
        public static string TranslateType2TypeString<T>(T odhtype)
        {
            return odhtype switch
            {
                Accommodation or AccommodationLinked => "accommodation",
                AccoRoom or AccommodationRoomLinked => "accommodationroom",
                GBLTSActivity or LTSActivityLinked => "ltsactivity",
                GBLTSPoi or LTSPoiLinked => "ltspoi",
                Gastronomy or GastronomyLinked => "ltsgastronomy",
                Event or EventLinked => "event",
                ODHActivityPoi or ODHActivityPoiLinked => "odhactivitypoi",
                Package or PackageLinked => "package",
                Measuringpoint or MeasuringpointLinked => "measuringpoint",
                WebcamInfo or WebcamInfoLinked => "webcam",
                Article or ArticlesLinked => "article",
                DDVenue => "venue",
                EventShort or EventShortLinked => "eventshort",
                ExperienceArea or ExperienceAreaLinked => "experiencearea",
                MetaRegion or MetaRegionLinked => "metaregion",
                Region or RegionLinked => "region",
                Tourismverein or TourismvereinLinked => "tourismassociation",
                Municipality or MunicipalityLinked => "municipality",
                District or DistrictLinked => "district",
                SkiArea or SkiAreaLinked => "skiarea",
                SkiRegion or SkiRegionLinked => "skiregion",
                Area or AreaLinked => "area",
                Wine or WineLinked => "wineaward",
                SmgTags or ODHTagLinked => "odhtag",
                _ => throw new Exception("not known odh type")
            };
        }

        /// <summary>
        /// Translates Type as String to PG Table Name
        /// </summary>
        /// <param name="odhtype"></param>
        /// <returns></returns>
        public static string TranslateTypeString2Table(string odhtype)
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

        /// <summary>
        /// Translates Type to PG Table Name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="odhtype"></param>
        /// <returns></returns>
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
                EventShort or EventShortLinked => "eventeuracnoi",
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

        /// <summary>
        /// Translates Table Name to Type as String
        /// </summary>
        /// <param name="odhtype"></param>
        /// <returns></returns>
        public static string TranslateTable2TypeString(string odhtype)
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

        //public static Func<string, string[]> TranslateTypeToSearchField(string odhtype, bool searchontext = false)
        //{
        //    return odhtype switch
        //    {
        //        "accommodation" => !searchontext ? PostgresSQLWhereBuilder.AccoTitleFieldsToSearchFor : AddToStringArray(PostgresSQLWhereBuilder.AccoTitleFieldsToSearchFor, "en"),
        //        "accommodationroom" => PostgresSQLWhereBuilder.AccoRoomNameFieldsToSearchFor,
        //        "ltsactivity" or "ltspoi" or "ltsgastronomy" or "event" or "odhactivitypoi" or "metaregion" or "region" or "tourismassociation" or "municipality"
        //        or "district" or "skiarea" or "skiregion" or "article" or "experiencearea"
        //        => PostgresSQLWhereBuilder.TitleFieldsToSearchFor,
        //        //"measuringpoint" => PostgresSQLWhereBuilder.,
        //        "webcam" => PostgresSQLWhereBuilder.WebcamnameFieldsToSearchFor,
        //        "venue" => PostgresSQLWhereBuilder.VenueTitleFieldsToSearchFor,
        //        //"eventshort" => "eventeuracnoi",           
        //        //"area" => "areas",
        //        //"wineaward" => "wines",
        //        _ => throw new Exception("not known odh type")
        //    };
        //}

        

        public static string TranslateTypeToTitleField(string odhtype, string language)
        {
            return odhtype switch
            {
                "accommodation" => $"AccoDetail.{language}.Name",
                "accommodationroom" => $"AccoRoomDetail.{language}.Name",
                "ltsactivity" or "ltspoi" or "ltsgastronomy" or "event" or "odhactivitypoi" or "metaregion" or "region" or "tourismassociation" or "municipality"
                or "district" or "skiarea" or "skiregion" or "article" or "experiencearea"
                => $"Detail.{language}.Title",
                "measuringpoint" => $"Shortname",
                "webcam" => $"Webcamname.{language}",
                "venue" => $"attributes.name.{PostgresSQLWhereBuilder.TransformLanguagetoDDStandard(language)}",
                //"eventshort" => "eventeuracnoi",           
                //"area" => "areas",
                //"wineaward" => "wines",
                _ => throw new Exception("not known odh type")
            };
        }

        public static string TranslateTypeToBaseTextField(string odhtype, string language)
        {
            return odhtype switch
            {
                "accommodation" => $"AccoDetail.{language}.Longdesc",
                "accommodationroom" => $"AccoRoomDetail.{language}.Longdesc",
                "ltsactivity" or "ltspoi" or "ltsgastronomy" or "event" or "odhactivitypoi" or "metaregion" or "region" or "tourismassociation" or "municipality"
                or "district" or "skiarea" or "skiregion" or "article" or "experiencearea"
                => $"Detail.{language}.BaseText",
                "measuringpoint" => "notextfield",
                "webcam" => "notextfield",
                "venue" => "notextfield",
                //"eventshort" => "eventeuracnoi",           
                //"area" => "areas",
                //"wineaward" => "wines",
                _ => throw new Exception("not known odh type")
            };
        }
    }
}
