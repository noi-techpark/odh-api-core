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
        public static Metadata GetMetadata(string id, string type, string source, Nullable<DateTime> lastupdated = null)
        {
            return new Metadata() { Id = id, Type = type, LastUpdate = lastupdated, Source = source };
        }

        //TODO Make a HOF and apply all the rules
        public static Metadata GetMetadataobject<T>(T myobject, Func<T, Metadata> metadataganerator)
        {
            return metadataganerator(myobject);
        }

        public static Metadata GetMetadataforAccommodation(Accommodation data)
        {
            return GetMetadata(data.Id, "accommodation", "lts", data.LastChange);
        }

        public static Metadata GetMetadataforAccommodationRoom(AccoRoom data)
        {
            //fix if source is null
            string datasource = data.Source;

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

            return GetMetadata(data.Id, "accommodationroom", datasource, data.LastChange);
        }

        public static Metadata GetMetadataforActivity(PoiBaseInfos data)
        { 
            return GetMetadata(data.Id, "ltsactivity", "lts", data.LastChange);
        }

        public static Metadata GetMetadataforPoi(LTSPoi data)
        {            
            return GetMetadata(data.Id, "ltspoi", "lts", data.LastChange);
        }

        public static Metadata GetMetadataforGastronomy(Gastronomy data)
        {
            return GetMetadata(data.Id, "ltsgastronomy", "lts", data.LastChange); ;
        }

        public static Metadata GetMetadataforEvent(Event data)
        {
            string sourcemeta = data.Source.ToLower();
            
            return GetMetadata(data.Id, "event", sourcemeta, data.LastChange);
        }

        public static Metadata GetMetadataforOdhActivityPoi(SmgPoi data)
        {
            string sourcemeta = data.Source.ToLower();

            if (sourcemeta == "common" || sourcemeta == "magnolia" || sourcemeta == "content")
                sourcemeta = "idm";

            return GetMetadata(data.Id, "odhactivitypoi", sourcemeta, data.LastChange);
        }

        public static Metadata GetMetadataforPackage(Package data)
        {
            return GetMetadata(data.Id, "package", "hgv", data.LastUpdate);
        }

        public static Metadata GetMetadataforMeasuringpoint(Measuringpoint data)
        {
           return GetMetadata(data.Id, "measuringpoint", "lts", data.LastChange);
        }

        public static Metadata GetMetadataforWebcam(WebcamInfo data)
        {
            string sourcemeta = data.Source.ToLower();
            if (sourcemeta == "content")
                sourcemeta = "idm";

            return GetMetadata(data.Id, "webcam", sourcemeta, data.LastChange);
        }

        public static Metadata GetMetadataforArticle(ArticleBaseInfos data)
        {
            return GetMetadata(data.Id, "article", "idm", data.LastChange);
        }

        public static Metadata GetMetadataforVenue(DDVenue data)
        {
            return data._Meta = GetMetadata(data.Id, "venue", "lts", data.meta.lastUpdate);
        }

        public static Metadata GetMetadataforEventShort(EventShort data)
        {
            return GetMetadata(data.Id, "eventshort", "eurac");
        }

        public static Metadata GetMetadataforExperienceArea(ExperienceArea data)
        {
            return GetMetadata(data.Id, "experiencearea", "idm", data.LastChange);
        }

        public static Metadata GetMetadataforMetaRegion(MetaRegion data)
        {
            return GetMetadata(data.Id, "metaregion", "idm", data.LastChange);
        }

        public static Metadata GetMetadataforRegion(Region data)
        {
            return GetMetadata(data.Id, "region", "idm", data.LastChange);
        }

        public static Metadata GetMetadataforTourismverein(Tourismverein data)
        {
            return GetMetadata(data.Id, "tourismassociation", "idm", data.LastChange);
        }

        public static Metadata GetMetadataforMunicipality(Municipality data)
        {
            return GetMetadata(data.Id, "municipality", "idm", data.LastChange);
        }

        public static Metadata GetMetadataforDistrict(District data)
        {
            return GetMetadata(data.Id, "district", "idm", data.LastChange);
        }

        public static Metadata GetMetadataforSkiArea(SkiArea data)
        {
            return GetMetadata(data.Id, "skiarea", "idm", data.LastChange);
        }

        public static Metadata GetMetadataforSkiRegion(SkiRegion data)
        {
            return GetMetadata(data.Id, "skiregion", "idm", data.LastChange);
        }

        public static Metadata GetMetadataforArea(Area data)
        {            
            return GetMetadata(data.Id, "area", "lts", data.LastChange);
        }

        public static Metadata GetMetadataforWineAward(Wine data)
        {
           return GetMetadata(data.Id, "wineaward", "suedtirolwein", data.LastChange);
        }
    }
}
