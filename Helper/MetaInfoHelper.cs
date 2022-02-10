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
        public static Metadata GetMetadata<T>(T data, string source, Nullable<DateTime> lastupdated = null) where T : IIdentifiable
        {
            string type = ODHTypeHelper.TranslateType2TypeString<T>(data);
            return new Metadata() { Id = data.Id, Type = type, LastUpdate = lastupdated, Source = source };
        }

        //TODO Make a HOF and apply all the rules
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
                ExperienceArea or ExperienceAreaLinked => GetMetadataforExperienceArea(myobject as ExperienceArea),
                MetaRegion or MetaRegionLinked => GetMetadataforMetaRegion(myobject as MetaRegionLinked),
                Region or RegionLinked => GetMetadataforRegion(myobject as RegionLinked),
                Tourismverein or TourismvereinLinked => GetMetadataforTourismverein(myobject as TourismvereinLinked),
                Municipality or MunicipalityLinked => GetMetadataforMunicipality(myobject as MunicipalityLinked),
                District or DistrictLinked => GetMetadataforDistrict(myobject as DistrictLinked),
                SkiArea or SkiAreaLinked => GetMetadataforSkiArea(myobject as SkiAreaLinked),
                SkiRegion or SkiRegionLinked => GetMetadataforSkiRegion(myobject as SkiRegionLinked),
                Area or AreaLinked => GetMetadataforArea(myobject as AreaLinked),
                Wine or WineLinked => GetMetadataforWineAward(myobject as WineLinked),
                SmgTags or ODHTagLinked => GetMetadataforSmgTag(myobject as ODHTagLinked),
                _ => throw new Exception("not known odh type")
            };            
        }

        public static Metadata GetMetadataforAccommodation(Accommodation data)
        {
            return GetMetadata(data, "lts", data.LastChange);
        }

        public static Metadata GetMetadataforAccommodationRoom(AccoRoom data)
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

        public static Metadata GetMetadataforActivity(PoiBaseInfos data)
        { 
            return GetMetadata(data, "lts", data.LastChange);
        }

        public static Metadata GetMetadataforPoi(LTSPoi data)
        {            
            return GetMetadata(data, "lts", data.LastChange);
        }

        public static Metadata GetMetadataforGastronomy(Gastronomy data)
        {
            return GetMetadata(data, "lts", data.LastChange); ;
        }

        public static Metadata GetMetadataforEvent(Event data)
        {
            string sourcemeta = data.Source.ToLower();
            
            return GetMetadata(data, sourcemeta, data.LastChange);
        }

        public static Metadata GetMetadataforOdhActivityPoi(SmgPoi data)
        {
            string? sourcemeta = data.Source?.ToLower();

            if (sourcemeta == "common" || sourcemeta == "magnolia" || sourcemeta == "content")
                sourcemeta = "idm";

            return GetMetadata(data, sourcemeta ?? "", data.LastChange);
        }

        public static Metadata GetMetadataforOdhTag(SmgTags data)
        {
            string sourcemeta = "idm";

            if (data.Source.Contains("LTSCategory"))
                sourcemeta = "lts";

            return GetMetadata(data, sourcemeta, data.LastChange);
        }

        public static Metadata GetMetadataforPackage(Package data)
        {
            return GetMetadata(data, "hgv", data.LastUpdate);
        }

        public static Metadata GetMetadataforMeasuringpoint(Measuringpoint data)
        {
           return GetMetadata(data, "lts", data.LastChange);
        }

        public static Metadata GetMetadataforWebcam(WebcamInfo data)
        {
            string sourcemeta = data.Source.ToLower();
            if (sourcemeta == "content")
                sourcemeta = "idm";

            return GetMetadata(data, sourcemeta, data.LastChange);
        }

        public static Metadata GetMetadataforArticle(ArticleBaseInfos data)
        {
            //

            return GetMetadata(data, "idm", data.LastChange);
        }

        public static Metadata GetMetadataforVenue(DDVenue data)
        {
            return data._Meta = GetMetadata(data, "lts", data.meta.lastUpdate);
        }

        public static Metadata GetMetadataforEventShort(EventShort data)
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

        public static Metadata GetMetadataforExperienceArea(ExperienceArea data)
        {
            return GetMetadata(data, "idm", data.LastChange);
        }

        public static Metadata GetMetadataforMetaRegion(MetaRegion data)
        {
            return GetMetadata(data, "idm", data.LastChange);
        }

        public static Metadata GetMetadataforRegion(Region data)
        {
            return GetMetadata(data, "idm", data.LastChange);
        }

        public static Metadata GetMetadataforTourismverein(Tourismverein data)
        {
            return GetMetadata(data, "idm", data.LastChange);
        }

        public static Metadata GetMetadataforMunicipality(Municipality data)
        {
            return GetMetadata(data, "idm", data.LastChange);
        }

        public static Metadata GetMetadataforDistrict(District data)
        {
            return GetMetadata(data, "idm", data.LastChange);
        }

        public static Metadata GetMetadataforSkiArea(SkiArea data)
        {
            return GetMetadata(data, "idm", data.LastChange);
        }

        public static Metadata GetMetadataforSkiRegion(SkiRegion data)
        {
            return GetMetadata(data, "idm", data.LastChange);
        }

        public static Metadata GetMetadataforArea(Area data)
        {            
            return GetMetadata(data, "lts", data.LastChange);
        }

        public static Metadata GetMetadataforWineAward(Wine data)
        {
           return GetMetadata(data, "suedtirolwein", data.LastChange);
        }
        
        public static Metadata GetMetadataforSmgTag(SmgTags data)
        {
            var source = data.Source != null && data.Source.Count > 0  ? data.Source.FirstOrDefault().ToLower() : "";
            string sourcemeta = "idm";

            if (data.Source.Contains("LTSCategory"))
                sourcemeta = "lts";

            return GetMetadata(data, sourcemeta, data.LastChange);
        }
    }
}
