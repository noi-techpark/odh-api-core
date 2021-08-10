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
            return GetMetadata(data, "idm", data.LastChange);
        }

        public static Metadata GetMetadataforVenue(DDVenue data)
        {
            return data._Meta = GetMetadata(data, "lts", data.meta.lastUpdate);
        }

        public static Metadata GetMetadataforEventShort(EventShort data)
        {
            return GetMetadata(data, "eurac");
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

        //TODO
        //public static Metadata GetMetadataforSmgTag(SmgTags data)
        //{
        //    var source = data.Source.FirstOrDefault().ToLower();

        //    return GetMetadata(data.Id, "odhtag", source, data.LastChange);
        //}
    }
}
