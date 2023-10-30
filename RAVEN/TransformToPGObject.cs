// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helper;
using Helper.Extensions;

namespace RAVEN
{
    public class TransformToPGObject
    {
        //HOF applies all the rules
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

            if (String.IsNullOrEmpty(data.Source))
                data.Source = "lts";

            data._Meta = MetadataHelper.GetMetadataobject<AccommodationLinked>(data, MetadataHelper.GetMetadataforAccommodation);  //GetMetadata(data.Id, "accommodation", "lts", data.LastChange);
            //data.PublishedOn = PublishedOnHelper.GetPublishenOnList("accommodation", data.SmgActive);


            return data;
        }

        public static AccommodationLinked GetAccommodationPGObject(AccommodationRaven data)
        {
            AccommodationLinked acco = new AccommodationLinked();

            acco.Id = data.Id.ToUpper();

            if (data.SmgTags != null && data.SmgTags.Count > 0)
                acco.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();

            //Shortdesc Longdesc fix
            foreach (var detail in data.AccoDetail)
            {
                var shortdesc = detail.Value.Longdesc;
                var longdesc = detail.Value.Shortdesc;

                detail.Value.Shortdesc = shortdesc;
                detail.Value.Longdesc = longdesc;
            }

       
            acco.AccoBookingChannel = data.AccoBookingChannel;
            acco.AccoDetail = data.AccoDetail;
            acco.AccoCategoryId = data.AccoCategoryId;
            acco.AccoRoomInfo = data.AccoRoomInfo;
            acco.AccoTypeId = data.AccoTypeId;
            acco.Active = data.Active;

            acco.GpsInfo = data.ConvertGpsInfoOnRootToGpsInfoArray();
            acco.BadgeIds = data.BadgeIds;
            acco.BoardIds = data.BoardIds;
            acco.DistanceInfo = data.DistanceInfo;
            acco.DistrictId = data.DistrictId;
            acco.Features = data.Features;
            acco.FirstImport = data.FirstImport;
            acco.GastronomyId = data.GastronomyId;
            acco.HasApartment = data.HasApartment;
            acco.HasLanguage = data.HasLanguage;
            acco.HasRoom      = data.HasRoom;
            acco.HgvId = data.HgvId;
            acco.ImageGallery = data.ImageGallery;
            acco.IndependentData = data.IndependentData;
            acco.IsAccommodation = data.IsAccommodation;
            acco.IsBookable = data.IsBookable;
            acco.IsCamping = data.IsCamping;
            acco.IsGastronomy = data.IsGastronomy;
            acco.LastChange = data.LastChange;
            acco.LicenseInfo = data.LicenseInfo;
            acco.LocationInfo = data.LocationInfo;
            acco.MainLanguage = data.MainLanguage;
            acco.Mapping = data.Mapping;
            acco.MarketingGroupIds = data.MarketingGroupIds;
            acco.MssResponseShort = data.MssResponseShort;
            acco.Representation = data.Representation;
            acco.Shortname = data.Shortname;
            acco.SmgActive = data.SmgActive;
            acco.SmgTags = data.SmgTags;
            acco.SpecialFeaturesIds = data.SpecialFeaturesIds;
            acco.Source = data.Source;

            if (String.IsNullOrEmpty(data.Source))
                acco.Source = "lts";

            acco.ThemeIds = data.ThemeIds;
            acco.TourismVereinId = data.TourismVereinId;
            acco.TrustYouActive = data.TrustYouActive;  
            acco.TrustYouID = data.TrustYouID;
            acco.TrustYouResults = data.TrustYouResults;
            acco.TrustYouScore = data.TrustYouScore;
            acco.TrustYouState = data.TrustYouState;
            acco.TVMember = data.TVMember;            

            acco._Meta = MetadataHelper.GetMetadataobject<AccommodationLinked>(acco, MetadataHelper.GetMetadataforAccommodation);  //GetMetadata(data.Id, "accommodation", "lts", data.LastChange);
            //data.PublishedOn = PublishedOnHelper.GetPublishenOnList("accommodation", data.SmgActive);

            return acco;
        }


        public static ODHActivityPoiLinked GetODHActivityPoiPGObject(ODHActivityPoiLinked data)
        {
            data.Id = data.Id.ToLower();

            if (data.Source != null)
                data.Source = data.Source.ToLower();

            if (data.SyncSourceInterface != null)
                data.SyncSourceInterface = data.SyncSourceInterface.ToLower();

            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();
            
            if (!String.IsNullOrEmpty(data.CustomId))
                data.CustomId = data.CustomId.ToUpper();

            //TODO
            //Write Webcam Objects to Related Content

            //Related Content
            if (data.RelatedContent != null)
            {
                List<RelatedContent> relcontentlist = new List<RelatedContent>();
                foreach (var relatedcontent in data.RelatedContent)
                {
                    RelatedContent relatedcontenttotransform = relatedcontent;

                    if (relatedcontent.Type == "acco" || relatedcontent.Type == "event" || relatedcontent.Type == "webcam")
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

            if (data.LTSTags != null)
            {
                foreach (var myltstag in data.LTSTags)
                {
                    myltstag.Id = myltstag.Id.ToLower();
                }
            }

            //Remove empty dictionary keys
            data.PoiProperty = data.PoiProperty == null ? new Dictionary<string, List<PoiProperty>>() : data.PoiProperty.Where(f => f.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value);            

            data._Meta = MetadataHelper.GetMetadataobject<ODHActivityPoiLinked>(data, MetadataHelper.GetMetadataforOdhActivityPoi); //GetMetadata(data.Id, "odhactivitypoi", sourcemeta, data.LastChange);

            ODHTagHelper.SetMainCategorizationForODHActivityPoi(data);

            return data;
        }

        public static AccommodationRoomLinked GetAccommodationRoomPGObject(AccommodationRoomLinked data)
        {
            data.Id = data.Id.ToUpper();
            data.A0RID = data.A0RID.ToUpper();
            if (!String.IsNullOrEmpty(data.LTSId))
                data.LTSId = data.LTSId.ToUpper();

            ////fix if source is null
            //string datasource = data.Source;

            //if (datasource == null)
            //{
            //    if (data.Id.Contains("hgv"))
            //        datasource = "hgv";
            //    else
            //        datasource = "lts";
            //}
            //else
            //{
            //    datasource = datasource.ToLower();
            //}

            data._Meta = MetadataHelper.GetMetadataobject<AccommodationRoomLinked>(data, MetadataHelper.GetMetadataforAccommodationRoom); //GetMetadata(data.Id, "accommodationroom", datasource, data.LastChange);
            //data.PublishedOn = PublishedOnHelper.GetPublishenOnList("accommodationroom", true);

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
            //data2.GpsPoints = new Dictionary<string, GpsInfo>();
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
            data2.LTSTags = data.LTSTags;
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
            data2.Source = data.Source;

            data2.Id = data2.Id.ToUpper();

            if (data2.SmgTags != null && data2.SmgTags.Count > 0)
                data2.SmgTags = data2.SmgTags.Select(x => x.ToLower()).ToList();

            if (data2.LTSTags != null)
            {
                foreach (var myltstag in data2.LTSTags)
                {
                    myltstag.Id = myltstag.Id.ToLower();
                }
            }
           
            if (String.IsNullOrEmpty(data2.Source))
                data2.Source = "lts";

            data2._Meta = MetadataHelper.GetMetadataobject<LTSActivityLinked>(data2, MetadataHelper.GetMetadataforActivity); //GetMetadata(data.Id, "ltsactivity", "lts", data.LastChange);
            data2.PublishedOn = new List<string>();

            return data2;
        }

        public static LTSPoiLinked GetPoiPGObject(LTSPoiLinked data)
        {

            data.Id = data.Id.ToUpper();

            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();

            if (data.LTSTags != null)
            {
                foreach (var myltstag in data.LTSTags)
                {
                    myltstag.Id = myltstag.Id.ToLower();
                }
            }

            if (String.IsNullOrEmpty(data.Source))
                data.Source = "lts";

            data.PublishedOn = new List<string>();

            data._Meta = MetadataHelper.GetMetadataobject<LTSPoiLinked>(data, MetadataHelper.GetMetadataforPoi); //GetMetadata(data.Id, "ltspoi", "lts", data.LastChange);

            return data;
        }

        public static ArticlesLinked GetArticlePGObject(ArticlesLinked data)
        {
            data.Id = data.Id.ToUpper();

            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();

            if (String.IsNullOrEmpty(data.Source))
                data.Source = "idm";

            data._Meta = MetadataHelper.GetMetadataobject<ArticlesLinked>(data, MetadataHelper.GetMetadataforArticle); //GetMetadata(data.Id, "article", "idm", data.LastChange);
            //data.PublishedOn = PublishedOnHelper.GetPublishenOnList("article", data.SmgActive);

            return data;
        }

        public static PackageLinked GetPackagePGObject(PackageLinked data)
        {
            data.Id = data.Id.ToUpper();
            data.HotelId = data.HotelId.Select(x => x.ToUpper()).ToList();

            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();

            if (String.IsNullOrEmpty(data.Source))
                data.Source = "hgv";

            data._Meta = MetadataHelper.GetMetadataobject<PackageLinked>(data, MetadataHelper.GetMetadataforPackage); //GetMetadata(data.Id, "package", "hgv", data.LastUpdate);
            //data.PublishedOn = PublishedOnHelper.GetPublishenOnList("package", data.SmgActive);

            return data;
        }

        public static EventLinked GetEventPGObject(EventLinked data)
        {
            data.Id = data.Id.ToUpper();

            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();

            data.Source = "lts";

            data._Meta = MetadataHelper.GetMetadataobject<EventLinked>(data, MetadataHelper.GetMetadataforEvent); //GetMetadata(data.Id, "event", "lts", data.LastChange);
            //data.PublishedOn = PublishedOnHelper.GetPublishenOnList("event", data.SmgActive);

            return data;
        }

        public static GastronomyLinked GetGastronomyPGObject(GastronomyLinked data)
        {
            data.Id = data.Id.ToUpper();
            if (!String.IsNullOrEmpty(data.AccommodationId))
                data.AccommodationId = data.AccommodationId.ToUpper();

            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();

            if (String.IsNullOrEmpty(data.Source))
                data.Source = "lts";

            data.PublishedOn = new List<string>();

            data._Meta = MetadataHelper.GetMetadataobject<GastronomyLinked>(data, MetadataHelper.GetMetadataforGastronomy); //GetMetadata(data.Id, "ltsgastronomy", "lts", data.LastChange);

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

            data.WebCamProperties = new WebcamProperties();
            data.WebCamProperties.WebcamUrl = data.Webcamurl;
            data.WebCamProperties.PreviewUrl = data.Previewurl;
            data.WebCamProperties.StreamUrl = data.Streamurl;            

            if (!String.IsNullOrEmpty(data.WebCamProperties.WebcamUrl))
            {
                //Hack add as ImageGallery
                data.ImageGallery = new List<ImageGallery>();
                ImageGallery image = new ImageGallery();
                image.ImageUrl = data.WebCamProperties.WebcamUrl;

                data.ImageGallery.Add(image);
            }


            if (String.IsNullOrEmpty(data.Source))
                data.Source = sourcemeta;
            else
                data.Source = data.Source.ToLower();

            data._Meta = MetadataHelper.GetMetadataobject<WebcamInfoLinked>(data, MetadataHelper.GetMetadataforWebcam); //GetMetadata(data.Id, "webcam", sourcemeta, data.LastChange);
            var webcampublished = data.WebcamAssignedOn != null && data.WebcamAssignedOn.Count > 0 ? true : false;

            //data.PublishedOn = PublishedOnHelper.GetPublishenOnList("webcam", webcampublished);

            return data;
        }

        public static WebcamInfoLinked GetWebcamInfoPGObject(WebcamInfoRaven data)
        {
            WebcamInfoLinked webcam = new WebcamInfoLinked();

            webcam.Id = data.Id.ToUpper();

            webcam.Active = data.Active;
            webcam.AreaIds = data.AreaIds;
            webcam.FirstImport = data.FirstImport;
            webcam.GpsInfo = data.GpsInfo != null ? new List<GpsInfo>() { data.GpsInfo } : new List<GpsInfo>();
            webcam.LastChange = data.LastChange;
            webcam.LicenseInfo = data.LicenseInfo;
            webcam.ListPosition = data.ListPosition;
            webcam.Mapping = data.Mapping;
            webcam.SmgActive = data.SmgActive;
            webcam.Shortname = data.Shortname;
            webcam.SmgTags = data.SmgTags;
            webcam.WebcamAssignedOn = data.WebcamAssignedOn;
            webcam.WebcamId = data.WebcamId;
            //Webcamproperties
            webcam.WebCamProperties = new WebcamProperties();
            webcam.WebCamProperties.WebcamUrl = data.Webcamurl;
            webcam.WebCamProperties.PreviewUrl = data.Previewurl; 
            webcam.WebCamProperties.StreamUrl = data.Streamurl;

            //ImageGallery
            if (!String.IsNullOrEmpty(data.Webcamurl))
            {
                //Hack add as ImageGallery
                webcam.ImageGallery = new List<ImageGallery>();
                ImageGallery image = new ImageGallery();
                image.ImageUrl = data.Webcamurl;

                webcam.ImageGallery.Add(image);
            }

            //Detail
            foreach (var kvp in data.Webcamname)
            {
                Detail detail = new Detail();
                detail.Language = kvp.Key;
                detail.Title = kvp.Value;

                webcam.Detail.TryAddOrUpdate(kvp.Key, detail);
            }

            webcam.HasLanguage = webcam.Detail.Keys;

            if (data.SmgTags != null && data.SmgTags.Count > 0)
                webcam.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();

            string sourcemeta = data.Source.ToLower();
            if (sourcemeta == "content")
                sourcemeta = "idm";

            if (String.IsNullOrEmpty(data.Source))
                webcam.Source = sourcemeta;
            else
                webcam.Source = data.Source.ToLower();

            webcam._Meta = MetadataHelper.GetMetadataobject<WebcamInfoLinked>(webcam, MetadataHelper.GetMetadataforWebcam); //GetMetadata(data.Id, "webcam", sourcemeta, data.LastChange);
            var webcampublished = data.WebcamAssignedOn != null && data.WebcamAssignedOn.Count > 0 ? true : false;

            //data.PublishedOn = PublishedOnHelper.GetPublishenOnList("webcam", webcampublished);

            return webcam;
        }


        public static MeasuringpointLinked GetMeasuringpointPGObject(MeasuringpointLinked data)
        {
            data.Id = data.Id.ToUpper();

            if (String.IsNullOrEmpty(data.Source))
                data.Source = "lts";
            else
                data.Source = data.Source.ToLower();

            data._Meta = MetadataHelper.GetMetadataobject<MeasuringpointLinked>(data, MetadataHelper.GetMetadataforMeasuringpoint); //GetMetadata(data.Id, "measuringpoint", "lts", data.LastChange);

            
            //data.PublishedOn = PublishedOnHelper.GetPublishenOnList("mesuringpoint", data.SmgActive);

            return data;
        }

        public static DDVenue GetVenuePGObject(DDVenue data)
        {
            data.Id = data.Id.ToUpper();

            data.LastChange = data.meta.lastUpdate;
            data.Shortname = data.attributes.name != null && data.attributes.name.Keys.Count > 0 ? data.attributes.name.FirstOrDefault().Value : "";
            data.LicenseInfo = data.odhdata.LicenseInfo;          

            if (data.odhdata.GpsInfo != null)
            {
                int i = 2;
                foreach (var gpsinfo in data.odhdata.GpsInfo)
                {
                    if (!data.odhdata.GpsPoints.ContainsKey(gpsinfo.Gpstype))
                    {
                        data.odhdata.GpsPoints.Add(gpsinfo.Gpstype, gpsinfo);
                    }
                    else
                    {
                        data.odhdata.GpsPoints.Add("position" + i, gpsinfo);
                        i++;
                    }
                }
            }

            data.odhdata.ODHActive = !data.attributes.categories.Contains("lts/visi_unpublishedOnODH") && data.attributes.categories.Contains("lts/visi_publishedOnODH") ? true : false;
            data.links.self = ODHConstant.ApplicationURL + "Venue/" + data.Id;

            data._Meta = MetadataHelper.GetMetadataobject<DDVenue>(data, MetadataHelper.GetMetadataforDDVenue);
            //data.odhdata.PublishedOn = PublishedOnHelper.GetPublishenOnList("venue", data.odhdata.ODHActive);

            //fixes
            data.odhdata.Source = data.odhdata.Source.ToLower();
            data.Source = data.odhdata.Source;

            data.LicenseInfo = data.odhdata.LicenseInfo;

            return data;
        }

        public static VenueLinked GetVenuePGObjectV2(DDVenue destinationdata)
        {
            //Transform to Venuelinked
            var venueLinked = new VenueLinked();
            venueLinked.Active = destinationdata.odhdata.Active;
            venueLinked.ContactInfos = destinationdata.odhdata.ContactInfos;
            venueLinked.Detail = destinationdata.odhdata.Detail;
            venueLinked.DistanceInfo = destinationdata.odhdata.DistanceInfo;
            venueLinked.FirstImport = destinationdata.FirstImport;
            venueLinked.GpsInfo = destinationdata.odhdata.GpsInfo;
            //venueLinked.GpsPoints = destinationdata.odhdata.GpsPoints;
            venueLinked.HasLanguage = destinationdata.odhdata.HasLanguage;
            venueLinked.Id = destinationdata.Id;
            venueLinked.ImageGallery = destinationdata.odhdata.ImageGallery;
            venueLinked.LastChange = destinationdata.LastChange;
            venueLinked.LicenseInfo = destinationdata.LicenseInfo;
            venueLinked.LocationInfo = destinationdata.odhdata.LocationInfo;
            venueLinked.Mapping = destinationdata.Mapping;
            venueLinked.SmgActive = destinationdata.odhdata.ODHActive;
            //venueLinked.ODHActive = destinationdata.odhdata.ODHActive;
            //venueLinked.ODHTags = destinationdata.odhdata.ODHTags;
            venueLinked.RoomCount = destinationdata.odhdata.RoomCount;
            venueLinked.RoomDetails = destinationdata.odhdata.RoomDetails;
            venueLinked.Beds = destinationdata.odhdata.Beds;
            venueLinked.OperationSchedule = destinationdata.odhdata.OperationSchedule;
            venueLinked.Shortname = destinationdata.odhdata.Shortname;
            venueLinked.SmgTags = destinationdata.odhdata.SmgTags;
            venueLinked.Source = destinationdata.odhdata.Source;
            venueLinked.SyncSourceInterface = destinationdata.odhdata.SyncSourceInterface;
            venueLinked.VenueCategory = destinationdata.odhdata.VenueCategory;
            venueLinked._Meta = destinationdata._Meta;
            venueLinked.PublishedOn = destinationdata.odhdata.PublishedOn;
            
            return venueLinked;
        }

        public static MetaRegionLinked GetMetaRegionPGObject(MetaRegionLinked data)
        {
            data.Id = data.Id.ToUpper();
            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();

            if (String.IsNullOrEmpty(data.Source))
                data.Source = "idm";

            data._Meta = MetadataHelper.GetMetadataobject<MetaRegionLinked>(data, MetadataHelper.GetMetadataforMetaRegion); //GetMetadata(data.Id, "metaregion", "idm", data.LastChange);
            //data.PublishedOn = PublishedOnHelper.GetPublishenOnList("metaregion", data.SmgActive);

            return data;
        }

        public static RegionLinked GetRegionPGObject(RegionLinked data)
        {
            data.Id = data.Id.ToUpper();
            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();
            
            if (String.IsNullOrEmpty(data.Source))
                data.Source = "idm";

            data._Meta = MetadataHelper.GetMetadataobject<RegionLinked>(data, MetadataHelper.GetMetadataforRegion); //GetMetadata(data.Id, "region", "idm", data.LastChange);
            //data.PublishedOn = PublishedOnHelper.GetPublishenOnList("region", data.SmgActive);

            return data;
        }

        public static TourismvereinLinked GetTourismAssociationPGObject(TourismvereinLinked data)
        {
            data.Id = data.Id.ToUpper();
            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();

            if (String.IsNullOrEmpty(data.Source))
                data.Source = "idm";

            data._Meta = MetadataHelper.GetMetadataobject<TourismvereinLinked>(data, MetadataHelper.GetMetadataforTourismverein);  //GetMetadata(data.Id, "tourismassociation", "idm", data.LastChange);
            //data.PublishedOn = PublishedOnHelper.GetPublishenOnList("tourismassociation", data.SmgActive);

            return data;
        }

        public static MunicipalityLinked GetMunicipalityPGObject(MunicipalityLinked data)
        {
            data.Id = data.Id.ToUpper();
            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();

            if (String.IsNullOrEmpty(data.Source))
                data.Source = "idm";

            data._Meta = MetadataHelper.GetMetadataobject<MunicipalityLinked>(data, MetadataHelper.GetMetadataforMunicipality); //GetMetadata(data.Id, "municipality", "idm", data.LastChange);
            //data.PublishedOn = PublishedOnHelper.GetPublishenOnList("municipality", data.SmgActive);

            return data;
        }

        public static DistrictLinked GetDistrictPGObject(DistrictLinked data)
        {
            data.Id = data.Id.ToUpper();
            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();

            if (String.IsNullOrEmpty(data.Source))
                data.Source = "idm";

            data._Meta = MetadataHelper.GetMetadataobject<DistrictLinked>(data, MetadataHelper.GetMetadataforDistrict); //GetMetadata(data.Id, "district", "idm", data.LastChange);
            //data.PublishedOn = PublishedOnHelper.GetPublishenOnList("district", data.SmgActive);

            return data;
        }

        public static ExperienceAreaLinked GetExperienceAreaPGObject(ExperienceAreaLinked data)
        {
            data.Id = data.Id.ToUpper();
            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();

            if (String.IsNullOrEmpty(data.Source))
                data.Source = "idm";

            data._Meta = MetadataHelper.GetMetadataobject<ExperienceAreaLinked>(data, MetadataHelper.GetMetadataforExperienceArea); //GetMetadata(data.Id, "experiencearea", "idm", data.LastChange);
            //data.PublishedOn = PublishedOnHelper.GetPublishenOnList("experiencearea", data.SmgActive);

            return data;
        }

        public static AreaLinked GetAreaPGObject(AreaLinked data)
        {
            data.Id = data.Id.ToUpper();            
            data._Meta = MetadataHelper.GetMetadataobject<AreaLinked>(data, MetadataHelper.GetMetadataforArea); //GetMetadata(data.Id, "area", "lts", data.LastChange);            

            return data;
        }

        public static SkiAreaLinked GetSkiAreaPGObject(SkiAreaLinked data)
        {
            data.Id = data.Id.ToUpper();
            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();

            if (String.IsNullOrEmpty(data.Source))
                data.Source = "idm";

            data._Meta = MetadataHelper.GetMetadataobject<SkiAreaLinked>(data, MetadataHelper.GetMetadataforSkiArea); //GetMetadata(data.Id, "skiarea", "idm", data.LastChange);
            //data.PublishedOn = PublishedOnHelper.GetPublishenOnList("skiarea", data.SmgActive);

            return data;
        }

        public static SkiRegionLinked GetSkiRegionPGObject(SkiRegionLinked data)
        {
            data.Id = data.Id.ToUpper();
            
            if (data.SmgTags != null && data.SmgTags.Count > 0)
                data.SmgTags = data.SmgTags.Select(x => x.ToLower()).ToList();

            if (String.IsNullOrEmpty(data.Source))
                data.Source = "idm";

            data._Meta = MetadataHelper.GetMetadataobject<SkiRegionLinked>(data, MetadataHelper.GetMetadataforSkiRegion); //GetMetadata(data.Id, "skiregion", "idm", data.LastChange);
            //data.PublishedOn = PublishedOnHelper.GetPublishenOnList("skiregion", data.SmgActive);

            return data;
        }

        public static WineLinked GetWinePGObject(WineLinked data)
        {
            data.Id = data.Id.ToUpper();

            if (String.IsNullOrEmpty(data.Source))
                data.Source = "suedtirolwein";

            data._Meta = MetadataHelper.GetMetadataobject<WineLinked>(data, MetadataHelper.GetMetadataforWineAward);  //GetMetadata(data.Id, "wineaward", "suedtirolwein", data.LastChange);
            //data.PublishedOn = PublishedOnHelper.GetPublishenOnList("wine", data.SmgActive);

            return data;
        }

        public static ODHTagLinked GetODHTagPGObject(ODHTagLinked data)
        {
            data.Id = data.Id.ToLower();

            data.MainEntity = data.MainEntity.ToLower();
            
            List<string> validforentitynew = new List<string>();
            foreach (var validforentity in data.ValidForEntity)
            {
                if (validforentity.ToLower() == "smgpoi")
                    validforentitynew.Add("odhactivitypoi");
                else
                    validforentitynew.Add(validforentity.ToLower());
            }

            data.ValidForEntity = validforentitynew;

            if (!data.ValidForEntity.Contains(data.MainEntity))
                data.ValidForEntity.Add(data.MainEntity);

            if (data.MainEntity == "smgpoi" && !data.ValidForEntity.Contains("odhactivitypoi"))
                validforentitynew.Add("odhactivitypoi");

            //Lowercase Mappings
            if (data.MappedTagIds != null && data.MappedTagIds.Count > 0)
            {
                data.MappedTagIds = data.MappedTagIds.ConverListToLowerCase().ToList();
            }

            //Lowercase MappedTagIds
            if (data.Mapping != null && data.Mapping.Count > 0)
            {
                IDictionary<string, IDictionary<string, string>> mappingdict = new Dictionary<string, IDictionary<string, string>>();

                foreach (var dictkvp in data.Mapping)
                {
                    mappingdict.Add(dictkvp.Key, dictkvp.Value.ConvertToLowercase(false, true));
                }

                data.Mapping = mappingdict;
            }


            data._Meta = MetadataHelper.GetMetadataobject<ODHTagLinked>(data, MetadataHelper.GetMetadataforOdhTag);  //GetMetadata(data.Id, "wineaward", "suedtirolwein", data.LastChange);

            if (data.PublishedOn == null)
                data.PublishedOn = new List<string>();

            //Adding LicenseInfo to ODHTag (not present on sinfo instance)                    
            data.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<ODHTagLinked>(data, Helper.LicenseHelper.GetLicenseforODHTag);

            //Change, Get the Publishedon directly from raven instance

            ////Hack Publishedon because ODHTag not implementing ISource
            //if (data.DisplayAsCategory != null && data.DisplayAsCategory.Value == true)
            //{
            //    //IF list is null instantiate it otherwise it will be empty
            //    data.PublishedOn = data.PublishedOn ?? new List<string>();

            //    data.PublishedOn.TryAddOrUpdateOnList("idm-marketplace");
            //}

            ////If Redactional Tag activate it
            //if (data.Source != null && data.Source.Contains("IDMRedactionalCategory"))
            //{
            //    //IF list is null instantiate it otherwise it will be empty
            //    data.PublishedOn = data.PublishedOn ?? new List<string>();

            //    data.PublishedOn.TryAddOrUpdateOnList("idm-marketplace");
            //}

            return data;
        }        
    }
}
