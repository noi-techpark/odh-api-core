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
        public static Metadata GetMetadata(string id, string type, string source, Nullable<DateTime> lastupdated = null, Nullable<bool> reduced = false)
        {
            return new Metadata() { Id = id, Type = type, LastUpdate = lastupdated, Source = source, Reduced = reduced };
        }

        public static Metadata GetMetadata<T>(T data, string source, Nullable<DateTime> lastupdated = null, bool reduced = false) where T : IIdentifiable, IMetaData
        {
            string type = ODHTypeHelper.TranslateType2TypeString<T>(data);

            //If source is already set use the old source
            if (data._Meta != null && !String.IsNullOrEmpty(data._Meta.Source))
                source = data._Meta.Source;

            //TODO add Metainfo when data is reduced

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
                Accommodation or AccommodationLinked => GetMetadataforAccommodation(myobject as AccommodationLinked),
                AccoRoom or AccommodationRoomLinked => GetMetadataforAccommodationRoom(myobject as AccommodationRoomLinked),
                GBLTSActivity or LTSActivityLinked => GetMetadataforActivity(myobject as LTSActivityLinked),
                GBLTSPoi or LTSPoiLinked => GetMetadataforPoi(myobject as LTSPoiLinked),
                Gastronomy or GastronomyLinked => GetMetadataforGastronomy(myobject as GastronomyLinked),
                Event or EventLinked => GetMetadataforEvent(myobject as EventLinked),
                ODHActivityPoi or ODHActivityPoiLinked => GetMetadataforOdhActivityPoi(myobject as ODHActivityPoiLinked),
                Package or PackageLinked => GetMetadataforPackage(myobject as PackageLinked),
                Measuringpoint or MeasuringpointLinked => GetMetadataforMeasuringpoint(myobject as MeasuringpointLinked),
                WebcamInfo or WebcamInfoLinked => GetMetadataforWebcam(myobject as WebcamInfoLinked),
                Article or ArticlesLinked => GetMetadataforArticle(myobject as ArticlesLinked),
                DDVenue => GetMetadataforVenue(myobject as DDVenue),
                EventShort or EventShortLinked => GetMetadataforEventShort(myobject as EventShortLinked),
                ExperienceArea or ExperienceAreaLinked => GetMetadataforExperienceArea(myobject as ExperienceAreaLinked),
                MetaRegion or MetaRegionLinked => GetMetadataforMetaRegion(myobject as MetaRegionLinked),
                Region or RegionLinked => GetMetadataforRegion(myobject as RegionLinked),
                Tourismverein or TourismvereinLinked => GetMetadataforTourismverein(myobject as TourismvereinLinked),
                Municipality or MunicipalityLinked => GetMetadataforMunicipality(myobject as MunicipalityLinked),
                District or DistrictLinked => GetMetadataforDistrict(myobject as DistrictLinked),
                SkiArea or SkiAreaLinked => GetMetadataforSkiArea(myobject as SkiAreaLinked),
                SkiRegion or SkiRegionLinked => GetMetadataforSkiRegion(myobject as SkiRegionLinked),
                Area or AreaLinked => GetMetadataforArea(myobject as AreaLinked),
                Wine or WineLinked => GetMetadataforWineAward(myobject as WineLinked),
                SmgTags or ODHTagLinked => GetMetadataforOdhTag(myobject as ODHTagLinked),
                _ => throw new Exception("not known odh type")
            };            
        }

        public static Metadata GetMetadataforAccommodation(AccommodationLinked data)
        {
            return GetMetadata(data, "lts", data.LastChange);
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

            return GetMetadata(data, datasource, data.LastChange);
        }

        public static Metadata GetMetadataforActivity(LTSActivityLinked data)
        { 
            return GetMetadata(data, "lts", data.LastChange);
        }

        public static Metadata GetMetadataforPoi(LTSPoiLinked data)
        {            
            return GetMetadata(data, "lts", data.LastChange);
        }

        public static Metadata GetMetadataforGastronomy(GastronomyLinked data)
        {
            return GetMetadata(data, "lts", data.LastChange); ;
        }

        public static Metadata GetMetadataforEvent(EventLinked data)
        {
            string sourcemeta = data.Source.ToLower();
            
            return GetMetadata(data, sourcemeta, data.LastChange);
        }

        public static Metadata GetMetadataforOdhActivityPoi(ODHActivityPoiLinked data)
        {
            string? sourcemeta = data.Source?.ToLower();

            if (sourcemeta == "common" || sourcemeta == "magnolia" || sourcemeta == "content")
                sourcemeta = "idm";

            return GetMetadata(data, sourcemeta ?? "", data.LastChange);
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
            return GetMetadata(data, "hgv", data.LastUpdate);
        }

        public static Metadata GetMetadataforMeasuringpoint(MeasuringpointLinked data)
        {
           return GetMetadata(data, "lts", data.LastChange);
        }

        public static Metadata GetMetadataforWebcam(WebcamInfoLinked data)
        {
            string sourcemeta = data.Source.ToLower();
            if (sourcemeta == "content")
                sourcemeta = "idm";

            return GetMetadata(data, sourcemeta, data.LastChange);
        }

        public static Metadata GetMetadataforArticle(ArticlesLinked data)
        {            
            return GetMetadata(data, "idm", data.LastChange);
        }

        public static Metadata GetMetadataforVenue(DDVenue data)
        {
            return data._Meta = GetMetadata(data, "lts", data.meta.lastUpdate);
        }

        public static Metadata GetMetadataforEventShort(EventShortLinked data)
        {
            string sourcestr = "";

            string sourcemeta = data.Source != null ? data.Source.ToLower() : "ebms";

            switch (sourcemeta)
            {
                case "content":
                    sourcestr = "noi";
                    break;
                case "ebms":
                    sourcestr = "eurac";
                    break;
                default:
                    sourcestr = sourcemeta;
                    break;                
            }                

            return GetMetadata(data, sourcestr, data.LastChange);
        }

        public static Metadata GetMetadataforExperienceArea(ExperienceAreaLinked data)
        {
            return GetMetadata(data, "idm", data.LastChange);
        }

        public static Metadata GetMetadataforMetaRegion(MetaRegionLinked data)
        {
            return GetMetadata(data, "idm", data.LastChange);
        }

        public static Metadata GetMetadataforRegion(RegionLinked data)
        {
            return GetMetadata(data, "idm", data.LastChange);
        }

        public static Metadata GetMetadataforTourismverein(TourismvereinLinked data)
        {
            return GetMetadata(data, "idm", data.LastChange);
        }

        public static Metadata GetMetadataforMunicipality(MunicipalityLinked data)
        {
            return GetMetadata(data, "idm", data.LastChange);
        }

        public static Metadata GetMetadataforDistrict(DistrictLinked data)
        {
            return GetMetadata(data, "idm", data.LastChange);
        }

        public static Metadata GetMetadataforSkiArea(SkiAreaLinked data)
        {
            return GetMetadata(data, "idm", data.LastChange);
        }

        public static Metadata GetMetadataforSkiRegion(SkiRegionLinked data)
        {
            return GetMetadata(data, "idm", data.LastChange);
        }

        public static Metadata GetMetadataforArea(AreaLinked data)
        {            
            return GetMetadata(data, "lts", data.LastChange);
        }

        public static Metadata GetMetadataforWineAward(WineLinked data)
        {
           return GetMetadata(data, "suedtirolwein", data.LastChange);
        }               
    }
}
