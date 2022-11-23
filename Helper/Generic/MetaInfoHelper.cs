using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class MetadataHelper
    {
        //Simple Method to reset the Metainfo
        public static Metadata GetMetadata(string id, string type, string? source, DateTime? lastupdated = null, bool reduced = false)
        {
            return new Metadata() { Id = id, Type = type, LastUpdate = lastupdated, Source = source, Reduced = reduced };
        }

        public static Metadata GetMetadata<T>(T data, string source, DateTime? lastupdated = null, bool reduced = false) where T : IIdentifiable, IMetaData
        {
            string type = ODHTypeHelper.TranslateType2TypeString<T>(data);

            //If source is already set use the old source
            if (data._Meta != null && !string.IsNullOrEmpty(data._Meta.Source))
                source = data._Meta.Source;            

            return new Metadata() { Id = data.Id, Type = type, LastUpdate = lastupdated, Source = source, Reduced = reduced };
        }
               
        public static Metadata GetMetadataobject<T>(T myobject, Func<T, Metadata> metadataganerator)
        {
            return metadataganerator(myobject);
        }

        //Discriminator if the function is not passed
        public static Metadata GetMetadataobject<T>(T myobject)
        {
            return myobject switch
            {
                AccommodationLinked al => GetMetadataforAccommodation(al),
                AccommodationRoomLinked al => GetMetadataforAccommodationRoom(al),
                LTSActivityLinked ltsal => GetMetadataforActivity(ltsal),
                LTSPoiLinked ltspl => GetMetadataforPoi(ltspl),
                GastronomyLinked gl => GetMetadataforGastronomy(gl),
                EventLinked el => GetMetadataforEvent(el),
                ODHActivityPoiLinked odhapl => GetMetadataforOdhActivityPoi(odhapl),
                PackageLinked pl => GetMetadataforPackage(pl),
                MeasuringpointLinked ml => GetMetadataforMeasuringpoint(ml),
                WebcamInfoLinked wil => GetMetadataforWebcam(wil),
                ArticlesLinked al => GetMetadataforArticle(al),
                DDVenue ddv => GetMetadataforDDVenue(ddv),
                VenueLinked vl => GetMetadataforVenue(vl),
                EventShortLinked esl => GetMetadataforEventShort(esl),
                ExperienceAreaLinked eal => GetMetadataforExperienceArea(eal),
                MetaRegionLinked mrl => GetMetadataforMetaRegion(mrl),
                RegionLinked rl => GetMetadataforRegion(rl),
                TourismvereinLinked tvl => GetMetadataforTourismverein(tvl),
                MunicipalityLinked ml => GetMetadataforMunicipality(ml),
                DistrictLinked dl => GetMetadataforDistrict(dl),
                SkiAreaLinked sal => GetMetadataforSkiArea(sal),
                SkiRegionLinked srl => GetMetadataforSkiRegion(srl),
                AreaLinked al => GetMetadataforArea(al),
                WineLinked wl => GetMetadataforWineAward(wl),
                ODHTagLinked odhtl => GetMetadataforOdhTag(odhtl),
                WeatherHistoryLinked wh => GetMetaDataForWeatherHistory(wh),
                WeatherLinked we => GetMetaDataForWeather(we),
                TourismMetaData tm => GetMetaDataForMetaData(tm),
                _ => throw new Exception("not known odh type")
            };            
        }

        public static Metadata GetMetadataforAccommodation(AccommodationLinked data)
        {
            bool reduced = false;
            if (data._Meta != null)
                reduced = (bool)data._Meta.Reduced;

            return GetMetadata(data, "lts", data.LastChange, reduced);
        }

        public static Metadata GetMetadataforAccommodationRoom(AccommodationRoomLinked data)
        {
            //fix if source is null
            string? datasource = data.Source;

            if (datasource == null)
            {
                if (data.Id.Contains("hgv"))
                    datasource = "hgv";
                else
                    datasource = "lts";
            }
            else
            {
                datasource = datasource.ToLower();
            }

            return GetMetadata(data, datasource, data.LastChange, false);
        }

        public static Metadata GetMetadataforActivity(LTSActivityLinked data)
        {
            bool reduced = false;
            if (data._Meta != null)
                reduced = (bool)data._Meta.Reduced;

            var sourcemeta = "lts";
            if(data.Source != null) 
                sourcemeta = data.Source.ToLower();

            return GetMetadata(data, sourcemeta, data.LastChange, reduced);
        }

        public static Metadata GetMetadataforPoi(LTSPoiLinked data)
        {
            bool reduced = false;
            if (data._Meta != null)
                reduced = (bool)data._Meta.Reduced;

            return GetMetadata(data, "lts", data.LastChange, reduced);
        }

        public static Metadata GetMetadataforGastronomy(GastronomyLinked data)
        {
            bool reduced = false;
            if (data._Meta != null)
                reduced = (bool)data._Meta.Reduced;

            return GetMetadata(data, "lts", data.LastChange, reduced);
        }

        public static Metadata GetMetadataforEvent(EventLinked data)
        {
            string sourcemeta = data.Source.ToLower();
            bool reduced = false;
            if (data._Meta != null)
                reduced = (bool)data._Meta.Reduced;

            return GetMetadata(data, sourcemeta, data.LastChange, reduced);
        }

        public static Metadata GetMetadataforOdhActivityPoi(ODHActivityPoiLinked data)
        {
            string? sourcemeta = data.Source?.ToLower();

            if (sourcemeta == "common" || sourcemeta == "magnolia" || sourcemeta == "content")
                sourcemeta = "idm";

            bool reduced = false;
            if (data._Meta != null)
                reduced = (bool)data._Meta.Reduced;

            return GetMetadata(data, sourcemeta ?? "", data.LastChange, reduced);
        }

        public static Metadata GetMetadataforOdhTag(ODHTagLinked data)
        {
            string sourcemeta = "idm";

            if (data.Source != null && data.Source.Contains("LTSCategory"))
                sourcemeta = "lts";

            return GetMetadata(data, sourcemeta, data.LastChange);
        }

        public static Metadata GetMetadataforPackage(PackageLinked data)
        {
            return GetMetadata(data, "hgv", data.LastUpdate, false);
        }

        public static Metadata GetMetadataforMeasuringpoint(MeasuringpointLinked data)
        {
            bool reduced = false;
            if (data._Meta != null)
                reduced = (bool)data._Meta.Reduced;

            return GetMetadata(data, "lts", data.LastChange, reduced);
        }

        public static Metadata GetMetadataforWebcam(WebcamInfoLinked data)
        {
            string sourcemeta = data.Source?.ToLower() ?? "";
            if (sourcemeta == "content")
                sourcemeta = "idm";

            bool reduced = false;
            if (data._Meta != null)
                reduced = (bool)data._Meta.Reduced;

            return GetMetadata(data, sourcemeta, data.LastChange, reduced);
        }

        public static Metadata GetMetadataforArticle(ArticlesLinked data)
        {
            string sourcemeta = "noi";
            
            if(!String.IsNullOrEmpty(data.Source))
                sourcemeta = data.Source.ToLower();

            if (sourcemeta == "content")
                sourcemeta = "idm";

            return GetMetadata(data, sourcemeta, data.LastChange, false);
        }

        public static Metadata GetMetadataforDDVenue(DDVenue data)
        {
            bool reduced = false;
            if (data._Meta != null)
                reduced = (bool)data._Meta.Reduced;

            return data._Meta = GetMetadata(data, "lts", data.meta?.lastUpdate, reduced);
        }

        public static Metadata GetMetadataforVenue(VenueLinked data)
        {
            bool reduced = false;
            if (data._Meta != null)
                reduced = (bool)data._Meta.Reduced;

            return data._Meta = GetMetadata(data, "lts", data.LastChange, reduced);
        }

        public static Metadata GetMetadataforEventShort(EventShortLinked data)
        {
            string sourcemeta = data.Source != null ? data.Source.ToLower() : "ebms";

            string sourcestr = sourcemeta switch
            {
                "content" => "noi",
                "ebms" => "eurac",
                _ => sourcemeta,
            };
            return GetMetadata(data, sourcestr, data.LastChange, false);
        }

        public static Metadata GetMetadataforExperienceArea(ExperienceAreaLinked data)
        {
            return GetMetadata(data, "idm", data.LastChange, false);
        }

        public static Metadata GetMetadataforMetaRegion(MetaRegionLinked data)
        {
            return GetMetadata(data, "idm", data.LastChange, false);
        }

        public static Metadata GetMetadataforRegion(RegionLinked data)
        {
            return GetMetadata(data, "idm", data.LastChange, false);
        }

        public static Metadata GetMetadataforTourismverein(TourismvereinLinked data)
        {
            return GetMetadata(data, "idm", data.LastChange, false);
        }

        public static Metadata GetMetadataforMunicipality(MunicipalityLinked data)
        {
            return GetMetadata(data, "idm", data.LastChange, false);
        }

        public static Metadata GetMetadataforDistrict(DistrictLinked data)
        {
            return GetMetadata(data, "idm", data.LastChange, false);
        }

        public static Metadata GetMetadataforSkiArea(SkiAreaLinked data)
        {
            return GetMetadata(data, "idm", data.LastChange, false);
        }

        public static Metadata GetMetadataforSkiRegion(SkiRegionLinked data)
        {
            return GetMetadata(data, "idm", data.LastChange, false);
        }

        public static Metadata GetMetadataforArea(AreaLinked data)
        {            
            return GetMetadata(data, "lts", data.LastChange, false);
        }

        public static Metadata GetMetadataforWineAward(WineLinked data)
        {
           return GetMetadata(data, "suedtirolwein", data.LastChange, false);
        }

        public static Metadata GetMetaDataForWeatherHistory(WeatherHistoryLinked data)
        {
            return GetMetadata(data, "siag", data.LastChange, false);
        }

        //Hack because WeatherLinked is not IIdentifiable so return directly
        public static Metadata GetMetaDataForWeather(WeatherLinked data)
        {
            string type = ODHTypeHelper.TranslateType2TypeString<WeatherLinked>(data);

            return new Metadata() { Id = data.Id.ToString(), Type = type, LastUpdate = data.date, Source = "siag", Reduced = false };            
        }

        public static Metadata GetMetaDataForMetaData(TourismMetaData data)
        {
            return GetMetadata(data, "noi", data.LastChange, false);
        }
    }
}
