using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RAVEN
{
    public class TransformToPGObject
    {
        //TODO Make a HOF and apply all the rules
        public static V GetPGObject<T, V>(T myobject, Func<T, V> pgmodelgenerator)
        {
            return pgmodelgenerator(myobject);
        }

        public static AccommodationLinked GetAccommodationPGObject(AccommodationLinked data)
        {
            data.Id = data.Id.ToUpper();

            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();

            //Shortdesc Longdesc fix TODO
            foreach (var detail in data.AccoDetail)
            {
                var shortdesc = detail.Value.Longdesc;
                var longdesc = detail.Value.Shortdesc;

                detail.Value.Shortdesc = shortdesc;
                detail.Value.Longdesc = longdesc;
            }

            data._Meta = GetMetadata(data.Id, "accommodation", "lts", data.LastChange);

            return data;
        }

        public static SmgPoiLinked GetODHActivityPoiPGObject(SmgPoiLinked data)
        {
            data.Id = data.Id.ToLower();

            if (data.SyncSourceInterface != null)
                data.SyncSourceInterface = data.SyncSourceInterface.ToLower();

            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();
            if (!String.IsNullOrEmpty(data.CustomId))
                data.CustomId = data.CustomId.ToUpper();

            //Related Content
            if (data.RelatedContent != null)
            {
                List<RelatedContent> relcontentlist = new List<RelatedContent>();
                foreach (var relatedcontent in data.RelatedContent)
                {
                    RelatedContent relatedcontenttotransform = relatedcontent;

                    if (relatedcontent.Type == "acco" || relatedcontent.Type == "event")
                    {
                        relatedcontenttotransform.Id = relatedcontenttotransform.Id.ToUpper();
                    }
                    else
                    {
                        relatedcontenttotransform.Id = relatedcontenttotransform.Id.ToLower();
                    }

                    relcontentlist.Add(relatedcontenttotransform);
                }

                data.RelatedContent = relcontentlist;
            }

            if (data.GpsInfo != null)
            {
                int i = 2;
                foreach (var gpsinfo in data.GpsInfo)
                {
                    if (!data.GpsPoints.ContainsKey(gpsinfo.Gpstype))
                    {
                        data.GpsPoints.Add(gpsinfo.Gpstype, gpsinfo);
                    }
                    else
                    {
                        data.GpsPoints.Add("position" + i, gpsinfo);
                        i++;
                    }                                            
                }
            }            

            string sourcemeta = data.Source.ToLower();

            if (sourcemeta == "common" || sourcemeta == "magnolia" || sourcemeta == "content")
                sourcemeta = "idm";

            data._Meta = GetMetadata(data.Id, "odhactivitypoi", sourcemeta, data.LastChange);


            return data;
        }

        public static AccommodationRoomLinked GetAccommodationRoomPGObject(AccommodationRoomLinked data)
        {


            return data;
        }

        public static LTSActivityLinked GetActivityPGObject(LTSActivityLinked data)
        {
           

            return data;
        }

        public static LTSPoiLinked GetPoiPGObject(LTSPoiLinked data)
        {


            return data;
        }

        public static ArticlesLinked GetArticlePGObject(ArticlesLinked data)
        {


            return data;
        }

        public static EventLinked GetEventPGObject(EventLinked data)
        {


            return data;
        }

        public static GastronomyLinked GetGastronomyPGObject(GastronomyLinked data)
        {


            return data;
        }

        public static WebcamInfoLinked GetWebcamInfoPGObject(WebcamInfoLinked data)
        {


            return data;
        }

        public static MeasuringpointLinked GetMeasuringpointPGObject(MeasuringpointLinked data)
        {


            return data;
        }

        public static DDVenue GetVenuePGObject(DDVenue data)
        {


            return data;
        }

        public static MetaRegionLinked GetMetaRegionPGObject(MetaRegionLinked data)
        {


            return data;
        }

        public static RegionLinked GetRegionPGObject(RegionLinked data)
        {


            return data;
        }

        public static TourismvereinLinked GetTourismAssociationPGObject(TourismvereinLinked data)
        {


            return data;
        }

        public static MunicipalityLinked GetMunicipalityPGObject(MunicipalityLinked data)
        {


            return data;
        }

        public static DistrictLinked GetDistrictPGObject(DistrictLinked data)
        {


            return data;
        }

        public static ExperienceAreaLinked GetExperienceAreaPGObject(ExperienceAreaLinked data)
        {


            return data;
        }

        public static AreaLinked GetAreaPGObject(AreaLinked data)
        {


            return data;
        }

        public static SkiAreaLinked GetSkiAreaPGObject(SkiAreaLinked data)
        {


            return data;
        }

        public static SkiRegion GetSkiRegionPGObject(SkiRegion data)
        {


            return data;
        }

        public static Wine GetWinePGObject(Wine data)
        {


            return data;
        }


        public static Metadata GetMetadata(string id, string type, string source, Nullable<DateTime> lastupdated = null)
        {
            return new Metadata() { Id = id, Type = type, LastUpdate = lastupdated, Source = source };
        }
    }
}
