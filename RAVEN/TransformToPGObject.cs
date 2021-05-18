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
            data.Id = data.Id.ToUpper();
            data.A0RID = data.A0RID.ToUpper();
            if (!String.IsNullOrEmpty(data.LTSId))
                data.LTSId = data.LTSId.ToUpper();

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

            data._Meta = GetMetadata(data.Id, "accommodationroom", datasource, data.LastChange);

            return data;
        }

        public static LTSActivityLinked GetActivityPGObject(LTSActivityLinked data)
        {
            LTSActivityLinked data2 = new LTSActivityLinked();
            data2.Active = data.Active;
            data2.AdditionalPoiInfos = data.AdditionalPoiInfos;
            data2.AltitudeDifference = data.AltitudeDifference;
            data2.AltitudeHighestPoint = data.AltitudeHighestPoint;
            data2.AltitudeLowestPoint = data.AltitudeLowestPoint;
            data2.AltitudeSumDown = data.AltitudeSumDown;
            data2.AltitudeSumUp = data.AltitudeSumUp;
            data2.AreaId = data.AreaId;
            data2.BikeTransport = data.BikeTransport;
            data2.ContactInfos = data.ContactInfos;
            data2.Detail = data.Detail;
            data2.Difficulty = data.Difficulty;
            data2.DistanceDuration = data.DistanceDuration;
            data2.DistanceLength = data.DistanceLength;
            data2.Exposition = data.Exposition;
            data2.FeetClimb = data.FeetClimb;
            data2.FirstImport = data.FirstImport;
            data2.GpsInfo = data.GpsInfo;
            data2.GpsPoints = new Dictionary<string, GpsInfo>();
            data2.GpsTrack = data.GpsTrack;
            data2.HasFreeEntrance = data.HasFreeEntrance;
            data2.HasLanguage = data.HasLanguage;
            data2.HasRentals = data.HasRentals;
            data2.Highlight = data.Highlight;
            data2.Id = data.Id;
            data2.ImageGallery = data.ImageGallery;
            data2.IsOpen = data.IsOpen;
            data2.IsPrepared = data.IsPrepared;
            data2.IsWithLigth = data.IsWithLigth;
            data2.LastChange = data.LastChange;
            data2.LiftAvailable = data.LiftAvailable;
            data2.LocationInfo = data.LocationInfo;
            data2.LTSTags = new List<LTSTags>();
            data2.OperationSchedule = data.OperationSchedule;
            data2.OutdooractiveID = data.OutdooractiveID;
            data2.PoiType = data.PoiType;
            data2.Ratings = data.Ratings;
            data2.RunToValley = data.RunToValley;
            data2.Shortname = data.Shortname;
            data2.SmgActive = data.SmgActive;
            data2.SmgId = data.SmgId;
            data2.SmgTags = data.SmgTags;
            data2.SubType = data.SubType;
            data2.TourismorganizationId = data.TourismorganizationId;
            data2.Type = data.Type;
            data2.OwnerRid = data.OwnerRid;
            data2.ChildPoiIds = data.ChildPoiIds;
            data2.MasterPoiIds = data.MasterPoiIds;
            data2.CopyrightChecked = data.CopyrightChecked;
            data2.OutdooractiveElevationID = data.OutdooractiveElevationID;
            data2.WayNumber = data.WayNumber;
            data2.Number = data.Number;
            data2.LicenseInfo = data.LicenseInfo;


            data2.Id = data2.Id.ToUpper();

            if (data2.SmgTags != null && data2.SmgTags.Count > 0)
                data2.SmgTags = data2.SmgTags.Select(x => x.ToLower()).ToList();

            //Problem
            if (data2.GpsInfo != null)
            {
                //Es gibt hier
                //Startpunkt
                //Endpunkt
                //Start und Ziel
                //Bergstation
                //Talstation
                //Standpunkt

                foreach (var gpsinfo in data2.GpsInfo)
                {
                    if (gpsinfo.Gpstype == "Endpunkt")
                        data2.GpsPoints.Add("endposition", gpsinfo);
                    if (gpsinfo.Gpstype == "Bergstation")
                        data2.GpsPoints.Add("endposition", gpsinfo);

                    if (!data2.GpsPoints.ContainsKey("position"))
                    {
                        if (gpsinfo.Gpstype == "Standpunkt")
                            data2.GpsPoints.Add("position", gpsinfo);
                        if (gpsinfo.Gpstype == "Startpunkt")
                            data2.GpsPoints.Add("position", gpsinfo);
                        if (gpsinfo.Gpstype == "Start und Ziel")
                            data2.GpsPoints.Add("position", gpsinfo);
                        if (gpsinfo.Gpstype == "Talstation")
                            data2.GpsPoints.Add("position", gpsinfo);
                    }
                    else if (!data2.GpsPoints.ContainsKey("position1"))
                    {
                        if (gpsinfo.Gpstype == "Standpunkt")
                            data2.GpsPoints.Add("position1", gpsinfo);
                        if (gpsinfo.Gpstype == "Startpunkt")
                            data2.GpsPoints.Add("position1", gpsinfo);
                        if (gpsinfo.Gpstype == "Start und Ziel")
                            data2.GpsPoints.Add("position1", gpsinfo);
                        if (gpsinfo.Gpstype == "Talstation")
                            data2.GpsPoints.Add("position1", gpsinfo);
                    }
                    else
                    {
                        if (gpsinfo.Gpstype == "Standpunkt")
                            data2.GpsPoints.Add("position2", gpsinfo);
                        if (gpsinfo.Gpstype == "Startpunkt")
                            data2.GpsPoints.Add("position2", gpsinfo);
                        if (gpsinfo.Gpstype == "Start und Ziel")
                            data2.GpsPoints.Add("position2", gpsinfo);
                        if (gpsinfo.Gpstype == "Talstation")
                            data2.GpsPoints.Add("position2", gpsinfo);
                    }
                }
            }

            data2._Meta = GetMetadata(data.Id, "ltsactivity", "lts", data.LastChange);

            return data2;
        }

        public static LTSPoiLinked GetPoiPGObject(LTSPoiLinked data)
        {

            data.Id = data.Id.ToUpper();

            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();

            if (data.GpsInfo != null)
            {
                foreach (var gpsinfo in data.GpsInfo)
                {

                    if (gpsinfo.Gpstype == "Endpunkt")
                        data.GpsPoints.Add("endposition", gpsinfo);
                    if (gpsinfo.Gpstype == "Bergstation")
                        data.GpsPoints.Add("endposition", gpsinfo);

                    if (!data.GpsPoints.ContainsKey("position"))
                    {
                        if (gpsinfo.Gpstype == "Standpunkt")
                            data.GpsPoints.Add("position", gpsinfo);
                        if (gpsinfo.Gpstype == "Startpunkt")
                            data.GpsPoints.Add("position", gpsinfo);
                        if (gpsinfo.Gpstype == "Start und Ziel")
                            data.GpsPoints.Add("position", gpsinfo);
                        if (gpsinfo.Gpstype == "Talstation")
                            data.GpsPoints.Add("position", gpsinfo);
                    }
                    else if (!data.GpsPoints.ContainsKey("position1"))
                    {
                        if (gpsinfo.Gpstype == "Standpunkt")
                            data.GpsPoints.Add("position1", gpsinfo);
                        if (gpsinfo.Gpstype == "Startpunkt")
                            data.GpsPoints.Add("position1", gpsinfo);
                        if (gpsinfo.Gpstype == "Start und Ziel")
                            data.GpsPoints.Add("position1", gpsinfo);
                        if (gpsinfo.Gpstype == "Talstation")
                            data.GpsPoints.Add("position1", gpsinfo);
                    }
                    else
                    {
                        if (gpsinfo.Gpstype == "Standpunkt")
                            data.GpsPoints.Add("position2", gpsinfo);
                        if (gpsinfo.Gpstype == "Startpunkt")
                            data.GpsPoints.Add("position2", gpsinfo);
                        if (gpsinfo.Gpstype == "Start und Ziel")
                            data.GpsPoints.Add("position2", gpsinfo);
                        if (gpsinfo.Gpstype == "Talstation")
                            data.GpsPoints.Add("position2", gpsinfo);
                    }

                }
            }

            data._Meta = GetMetadata(data.Id, "ltspoi", "lts", data.LastChange);

            return data;
        }

        public static ArticlesLinked GetArticlePGObject(ArticlesLinked data)
        {
            data.Id = data.Id.ToUpper();

            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();

            data._Meta = GetMetadata(data.Id, "article", "idm", data.LastChange);

            return data;
        }

        public static PackageLinked GetArticlePGObject(PackageLinked data)
        {
            data.Id = data.Id.ToUpper();
            data.HotelId = data.HotelId.Select(x => x.ToUpper()).ToList();

            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();            

            data._Meta = GetMetadata(data.Id, "package", "hgv", data.LastUpdate);

            return data;
        }

        public static EventLinked GetEventPGObject(EventLinked data)
        {
            data.Id = data.Id.ToUpper();

            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();

            data._Meta = GetMetadata(data.Id, "event", "lts", data.LastChange);
            data.Source = "lts";

            return data;
        }

        public static GastronomyLinked GetGastronomyPGObject(GastronomyLinked data)
        {
            data.Id = data.Id.ToUpper();
            if (!String.IsNullOrEmpty(data.AccommodationId))
                data.AccommodationId = data.AccommodationId.ToUpper();

            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();
            
            data._Meta = GetMetadata(data.Id, "ltsgastronomy", "lts", data.LastChange);

            return data;
        }

        public static WebcamInfoLinked GetWebcamInfoPGObject(WebcamInfoLinked data)
        {
            data.Id = data.Id.ToUpper();
            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();

            if (data.Active == null)
                data.Active = false;

            if (data.SmgActive == null)
                data.SmgActive = false;

            string sourcemeta = data.Source.ToLower();
            if (sourcemeta == "content")
                sourcemeta = "idm";

            data._Meta = GetMetadata(data.Id, "webcam", sourcemeta, data.LastChange);

            return data;
        }

        public static MeasuringpointLinked GetMeasuringpointPGObject(MeasuringpointLinked data)
        {
            data.Id = data.Id.ToUpper();            
            data._Meta = GetMetadata(data.Id, "measuringpoint", "lts", data.LastChange);

            return data;
        }

        public static DDVenue GetVenuePGObject(DDVenue data)
        {
            data.Id = data.Id.ToUpper();

            data._Meta = GetMetadata(data.Id, "venue", "lts", data.meta.lastUpdate);
            data.odhdata.ODHActive = data.attributes.categories.Contains("lts/visi_unpublishedOnODH") ? false : true;

            data.links.self = ODHConstant.ApplicationURL + "Venue/" + data.Id;

            return data;
        }

        public static MetaRegionLinked GetMetaRegionPGObject(MetaRegionLinked data)
        {
            data.Id = data.Id.ToUpper();
            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList(); 
            data._Meta = GetMetadata(data.Id, "metaregion", "idm", data.LastChange);

            return data;
        }

        public static RegionLinked GetRegionPGObject(RegionLinked data)
        {
            data.Id = data.Id.ToUpper();
            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList(); 
            data._Meta = GetMetadata(data.Id, "region", "idm", data.LastChange);

            return data;
        }

        public static TourismvereinLinked GetTourismAssociationPGObject(TourismvereinLinked data)
        {
            data.Id = data.Id.ToUpper();
            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList(); 
            data._Meta = GetMetadata(data.Id, "tourismassociation", "idm", data.LastChange);

            return data;
        }

        public static MunicipalityLinked GetMunicipalityPGObject(MunicipalityLinked data)
        {
            data.Id = data.Id.ToUpper();
            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList(); 
            data._Meta = GetMetadata(data.Id, "municipality", "idm", data.LastChange);

            return data;
        }

        public static DistrictLinked GetDistrictPGObject(DistrictLinked data)
        {
            data.Id = data.Id.ToUpper();
            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();
            data._Meta = GetMetadata(data.Id, "district", "idm", data.LastChange);

            return data;
        }

        public static ExperienceAreaLinked GetExperienceAreaPGObject(ExperienceAreaLinked data)
        {
            data.Id = data.Id.ToUpper();
            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();
            data._Meta = GetMetadata(data.Id, "experiencearea", "idm", data.LastChange);

            return data;
        }

        public static AreaLinked GetAreaPGObject(AreaLinked data)
        {
            data.Id = data.Id.ToUpper();            
            data._Meta = GetMetadata(data.Id, "area", "lts", data.LastChange);

            return data;
        }

        public static SkiAreaLinked GetSkiAreaPGObject(SkiAreaLinked data)
        {
            data.Id = data.Id.ToUpper();
            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();
            
            data._Meta = GetMetadata(data.Id, "skiarea", "idm", data.LastChange);

            return data;
        }

        public static SkiRegionLinked GetSkiRegionPGObject(SkiRegionLinked data)
        {
            data.Id = data.Id.ToUpper();
            
            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();

            data._Meta = GetMetadata(data.Id, "skiregion", "idm", data.LastChange);

            return data;
        }

        public static WineLinked GetWinePGObject(WineLinked data)
        {
            data.Id = data.Id.ToUpper();
            data._Meta = GetMetadata(data.Id, "wineaward", "suedtirolwein", data.LastChange);

            return data;
        }

        public static SmgTags GetODHTagPGObject(SmgTags data)
        {
            data.Id = data.Id.ToLower();
            data.MainEntity = data.MainEntity.ToLower();
            List<string> validforentitynew = new List<string>();
            foreach (var validforentity in data.ValidForEntity)
            {
                validforentitynew.Add(validforentity.ToLower());
            }

            data.ValidForEntity = validforentitynew;

            return data;
        }

        public static Metadata GetMetadata(string id, string type, string source, Nullable<DateTime> lastupdated = null)
        {
            return new Metadata() { Id = id, Type = type, LastUpdate = lastupdated, Source = source };
        }
    }
}
