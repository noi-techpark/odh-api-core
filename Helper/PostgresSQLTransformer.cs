using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helper
{
    public class PostgresSQLTransformer
    {
        #region Transformers

        public static AccommodationLocalized TransformToAccommodationLocalized(Accommodation acco, string language)
        {
            AccommodationLocalized data = new AccommodationLocalized();
            data.AccoBookingChannel = acco.AccoBookingChannel;
            data.AccoCategoryId = acco.AccoCategoryId;
            data.AccoDetail = acco.AccoDetail != null ? acco.AccoDetail.ContainsKey(language) ? acco.AccoDetail[language] : null : null;
            data.AccoTypeId = acco.AccoTypeId;
            data.Altitude = acco.Altitude;
            data.AltitudeUnitofMeasure = acco.AltitudeUnitofMeasure;
            data.BadgeIds = acco.BadgeIds;
            data.Beds = acco.Beds;
            data.BoardIds = acco.BoardIds;
            //Features = acco.Features,
            data.FirstImport = acco.FirstImport;
            data.GastronomyId = acco.GastronomyId;
            data.Gpstype = acco.Gpstype;
            data.HgvId = acco.HgvId;
            data.HasApartment = acco.HasApartment;
            data.HasRoom = acco.HasRoom;
            data.Id = acco.Id;
            data.IsBookable = acco.IsBookable;
            data.DistrictId = acco.DistrictId;
            data.LastChange = acco.LastChange;
            data.Latitude = acco.Latitude;
            data.Longitude = acco.Longitude;
            //data.LocationInfo = new LocationInfoLocalized()
            //{
            //    DistrictInfo = new DistrictInfoLocalized() { Id = acco.LocationInfo.DistrictInfo.Id, Name = acco.LocationInfo.DistrictInfo.Name[language] },
            //    MunicipalityInfo = new MunicipalityInfoLocalized() { Id = acco.LocationInfo.MunicipalityInfo.Id, Name = acco.LocationInfo.MunicipalityInfo.Name[language] },
            //    TvInfo = new TvInfoLocalized() { Id = acco.LocationInfo.TvInfo.Id, Name = acco.LocationInfo.TvInfo.Name[language] },
            //    RegionInfo = new RegionInfoLocalized() { Id = acco.LocationInfo.RegionInfo.Id, Name = acco.LocationInfo.RegionInfo.Name[language] }
            //};
            var distinfolocalized = new DistrictInfoLocalized() { };
            if (acco.LocationInfo != null)
            {
                if (acco.LocationInfo.DistrictInfo != null)
                {
                    distinfolocalized.Id = acco.LocationInfo.DistrictInfo.Id;
                    distinfolocalized.Name = acco.LocationInfo?.DistrictInfo?.Name?.ContainsKey(language) ?? false ? acco.LocationInfo?.DistrictInfo?.Name?[language] : "";
                }
            }

            var muninfolocalized = new MunicipalityInfoLocalized() { };
            if (acco.LocationInfo != null)
            {
                if (acco.LocationInfo.MunicipalityInfo != null)
                {
                    muninfolocalized.Id = acco.LocationInfo.MunicipalityInfo.Id;
                    muninfolocalized.Name = acco.LocationInfo?.MunicipalityInfo?.Name?.ContainsKey(language) ?? false ? acco.LocationInfo?.MunicipalityInfo?.Name?[language] : "";
                }
            }

            var reginfolocalized = new RegionInfoLocalized() { };
            if (acco.LocationInfo != null)
            {
                if (acco.LocationInfo.RegionInfo != null)
                {
                    reginfolocalized.Id = acco.LocationInfo.RegionInfo.Id;
                    reginfolocalized.Name = acco.LocationInfo?.RegionInfo?.Name?.ContainsKey(language) ?? false ? acco.LocationInfo?.RegionInfo?.Name?[language] : "";
                }
            }

            var tvinfolocalized = new TvInfoLocalized() { };
            if (acco.LocationInfo != null)
            {
                if (acco.LocationInfo.TvInfo != null)
                {
                    tvinfolocalized.Id = acco.LocationInfo.TvInfo.Id;
                    tvinfolocalized.Name = acco.LocationInfo?.TvInfo?.Name?.ContainsKey(language) ?? false ? acco.LocationInfo?.TvInfo?.Name?[language] : "";
                }
            }

            data.LocationInfo = new LocationInfoLocalized()
            {
                DistrictInfo = distinfolocalized,
                MunicipalityInfo = muninfolocalized,
                TvInfo = tvinfolocalized,
                RegionInfo = reginfolocalized
            };

            data.ImageGallery = acco.ImageGallery != null ? acco.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
            data.MainLanguage = acco.MainLanguage;
            //MarketingGroupIds = acco.MarketingGroupIds,
            data.Shortname = acco.Shortname;
            data.SmgActive = acco.SmgActive;
            data.SmgTags = acco.SmgTags;
            data.SpecialFeaturesIds = acco.SpecialFeaturesIds;
            data.ThemeIds = acco.ThemeIds;
            data.TourismVereinId = acco.TourismVereinId;
            data.TrustYouID = acco.TrustYouID;
            data.TrustYouResults = acco.TrustYouResults;
            data.TrustYouScore = acco.TrustYouScore;

            data.TrustYouActive = acco.TrustYouActive;
            data.TrustYouState = acco.TrustYouState;

            data.TVMember = acco.TVMember;
            data.Units = acco.Units;


            return data;
        }

        public static AccoListObject TransformToAccommodationListObject(Accommodation acco, string language)
        {
            AccoListObject data = new AccoListObject();
            data.Id = acco.Id;
            data.Name = acco.AccoDetail?.ContainsKey(language) ?? false ? acco.AccoDetail?[language].Name : "";
            data.Type = acco.AccoTypeId;
            data.Category = acco.AccoCategoryId;
            data.District = acco.LocationInfo?.DistrictInfo != null ? acco.LocationInfo?.DistrictInfo?.Name?[language] : null;
            data.Municipality = acco.LocationInfo?.MunicipalityInfo != null ? acco.LocationInfo?.MunicipalityInfo?.Name?[language] : null;
            data.Tourismverein = acco.LocationInfo?.TvInfo != null ? acco.LocationInfo?.TvInfo?.Name?[language] : null;
            data.Region = acco.LocationInfo?.RegionInfo != null ? acco.LocationInfo?.RegionInfo?.Name?[language] : null;
            data.TrustYouID = acco.TrustYouID;
            data.TrustYouResults = acco.TrustYouResults;
            data.TrustYouScore = acco.TrustYouScore;
            data.SuedtirolinfoLink = $"https://www.suedtirol.info/{language}/tripmapping/acco/{acco.Id?.ToUpper()}";
            data.ImageGallery = acco.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ? acco.ImageGallery.Where(x => x.ListPosition == 0).ToList() : null;

            return data;
        }

        public static GBLTSActivityPoiLocalized TransformToGBLTSActivityPoiLocalized(GBLTSPoi poibaseinfo, string language)
        {
            GBLTSActivityPoiLocalized data = new GBLTSActivityPoiLocalized();

            data.Id = poibaseinfo.Id;
            data.LastChange = poibaseinfo.LastChange;
            data.FirstImport = poibaseinfo.FirstImport;
            data.Active = poibaseinfo.Active;
            data.AdditionalPoiInfos = poibaseinfo.AdditionalPoiInfos != null ? poibaseinfo.AdditionalPoiInfos.ContainsKey(language) ? poibaseinfo.AdditionalPoiInfos[language] : null : null;
            data.AltitudeDifference = poibaseinfo.AltitudeDifference;
            data.AltitudeHighestPoint = poibaseinfo.AltitudeHighestPoint;
            data.AltitudeLowestPoint = poibaseinfo.AltitudeLowestPoint;
            data.AltitudeSumDown = poibaseinfo.AltitudeSumDown;
            data.AltitudeSumUp = poibaseinfo.AltitudeSumUp;
            data.AreaId = poibaseinfo.AreaId;
            data.ContactInfos = poibaseinfo.ContactInfos != null ? poibaseinfo.ContactInfos.ContainsKey(language) ? poibaseinfo.ContactInfos[language] : null : null;
            data.Detail = poibaseinfo.Detail != null ? poibaseinfo.Detail.ContainsKey(language) ? poibaseinfo.Detail[language] : null : null;
            data.Difficulty = poibaseinfo.Difficulty;
            data.DistanceDuration = poibaseinfo.DistanceDuration;
            data.DistanceLength = poibaseinfo.DistanceLength;
            data.Exposition = poibaseinfo.Exposition;
            data.FeetClimb = poibaseinfo.FeetClimb;
            data.GpsInfo = poibaseinfo.GpsInfo;
            data.GpsTrack = poibaseinfo.GpsTrack;
            data.HasFreeEntrance = poibaseinfo.HasFreeEntrance;
            data.HasRentals = poibaseinfo.HasRentals;
            data.Highlight = poibaseinfo.Highlight;
            data.IsOpen = poibaseinfo.IsOpen;
            data.IsPrepared = poibaseinfo.IsPrepared;
            data.IsWithLigth = poibaseinfo.IsWithLigth;
            data.LiftAvailable = poibaseinfo.LiftAvailable;
            data.OperationSchedule = poibaseinfo.OperationSchedule;
            data.Ratings = poibaseinfo.Ratings;
            data.RunToValley = poibaseinfo.RunToValley;
            data.Shortname = poibaseinfo.Shortname;
            data.SmgActive = poibaseinfo.SmgActive;
            data.SmgId = poibaseinfo.SmgId;
            data.SubType = poibaseinfo.SubType;
            data.TourismorganizationId = poibaseinfo.TourismorganizationId;
            data.Type = poibaseinfo.Type;
            data.SmgTags = poibaseinfo.SmgTags;

            var distinfolocalized = new DistrictInfoLocalized() { };
            if (poibaseinfo.LocationInfo != null)
            {
                if (poibaseinfo.LocationInfo.DistrictInfo != null)
                {
                    distinfolocalized.Id = poibaseinfo.LocationInfo.DistrictInfo.Id;
                    distinfolocalized.Name = poibaseinfo.LocationInfo?.DistrictInfo?.Name?.ContainsKey(language) ?? false ? poibaseinfo.LocationInfo?.DistrictInfo?.Name?[language] : "";
                }
            }

            var muninfolocalized = new MunicipalityInfoLocalized() { };
            if (poibaseinfo.LocationInfo != null)
            {
                if (poibaseinfo.LocationInfo.MunicipalityInfo != null)
                {
                    muninfolocalized.Id = poibaseinfo.LocationInfo.MunicipalityInfo.Id;
                    muninfolocalized.Name = poibaseinfo.LocationInfo?.MunicipalityInfo?.Name?.ContainsKey(language) ?? false ? poibaseinfo.LocationInfo?.MunicipalityInfo?.Name?[language] : "";
                }
            }

            var reginfolocalized = new RegionInfoLocalized() { };
            if (poibaseinfo.LocationInfo != null)
            {
                if (poibaseinfo.LocationInfo.RegionInfo != null)
                {
                    reginfolocalized.Id = poibaseinfo.LocationInfo.RegionInfo.Id;
                    reginfolocalized.Name = poibaseinfo.LocationInfo?.RegionInfo?.Name?.ContainsKey(language) ?? false ? poibaseinfo.LocationInfo?.RegionInfo?.Name?[language] : "";
                }
            }

            var tvinfolocalized = new TvInfoLocalized() { };
            if (poibaseinfo.LocationInfo != null)
            {
                if (poibaseinfo.LocationInfo.TvInfo != null)
                {
                    tvinfolocalized.Id = poibaseinfo.LocationInfo.TvInfo.Id;
                    tvinfolocalized.Name = poibaseinfo.LocationInfo?.TvInfo?.Name?.ContainsKey(language) ?? false ? poibaseinfo.LocationInfo?.TvInfo?.Name?[language] : "";
                }
            }

            data.LocationInfo = new LocationInfoLocalized()
            {
                DistrictInfo = distinfolocalized,
                MunicipalityInfo = muninfolocalized,
                TvInfo = tvinfolocalized,
                RegionInfo = reginfolocalized
            };
            data.ImageGallery = poibaseinfo.ImageGallery != null ? poibaseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;


            List<LTSTagsLocalized> ltstagslocalized = new List<LTSTagsLocalized>();

            if (poibaseinfo.LTSTags != null)
            {
                foreach (var ltstag in poibaseinfo.LTSTags)
                {
                    ltstagslocalized.Add(new LTSTagsLocalized() { Id = ltstag.Id, Level = ltstag.Level, TagName = ltstag.TagName.ContainsKey(language) ? ltstag.TagName[language] : "" });
                }
            }

            data.LTSTags = ltstagslocalized;
            data.GpsPoints = poibaseinfo.GpsPoints;

            return data;
        }

        public static GastronomyLocalized TransformToGastronomyLocalized(Gastronomy pgdata, string language)
        {
            GastronomyLocalized data = new GastronomyLocalized();

            data.Id = pgdata.Id;
            data.AccommodationId = pgdata.AccommodationId;
            data.Active = pgdata.Active;
            data.Altitude = pgdata.Altitude;
            data.AltitudeUnitofMeasure = pgdata.AltitudeUnitofMeasure;
            data.CapacityCeremony = pgdata.CapacityCeremony;
            data.CategoryCodes = pgdata.CategoryCodes;
            data.ContactInfos = pgdata.ContactInfos != null ? pgdata.ContactInfos.ContainsKey(language) ? pgdata.ContactInfos[language] : null : null;
            data.Detail = pgdata.Detail != null ? pgdata.Detail.ContainsKey(language) ? pgdata.Detail[language] : null : null;
            data.DishRates = pgdata.DishRates;
            data.DistrictId = pgdata.DistrictId;
            data.Facilities = pgdata.Facilities;
            data.FirstImport = pgdata.FirstImport;
            data.Gpstype = pgdata.Gpstype;
            data.ImageGallery = pgdata.ImageGallery != null ? pgdata.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
            data.LastChange = pgdata.LastChange;
            data.Latitude = pgdata.Latitude;
            data.Longitude = pgdata.Longitude;
            data.MarketinggroupId = pgdata.MarketinggroupId;
            data.MaxSeatingCapacity = pgdata.MaxSeatingCapacity;
            data.OperationSchedule = data.OperationSchedule;
            data.Shortname = pgdata.Shortname;
            data.SmgActive = pgdata.SmgActive;
            data.SmgTags = pgdata.SmgTags;
            data.Type = pgdata.Type;

            var distinfolocalized = new DistrictInfoLocalized() { };
            if (pgdata.LocationInfo != null)
            {
                if (pgdata.LocationInfo.DistrictInfo != null)
                {
                    distinfolocalized.Id = pgdata.LocationInfo.DistrictInfo.Id;
                    distinfolocalized.Name = pgdata.LocationInfo?.DistrictInfo?.Name?.ContainsKey(language) ?? false ? pgdata.LocationInfo?.DistrictInfo?.Name?[language] : "";
                }
            }

            var muninfolocalized = new MunicipalityInfoLocalized() { };
            if (pgdata.LocationInfo != null)
            {
                if (pgdata.LocationInfo.MunicipalityInfo != null)
                {
                    muninfolocalized.Id = pgdata.LocationInfo.MunicipalityInfo.Id;
                    muninfolocalized.Name = pgdata.LocationInfo.MunicipalityInfo?.Name?.ContainsKey(language) ?? false ? pgdata.LocationInfo?.MunicipalityInfo?.Name?[language] : "";
                }
            }

            var reginfolocalized = new RegionInfoLocalized() { };
            if (pgdata.LocationInfo != null)
            {
                if (pgdata.LocationInfo.RegionInfo != null)
                {
                    reginfolocalized.Id = pgdata.LocationInfo.RegionInfo.Id;
                    reginfolocalized.Name = pgdata.LocationInfo?.RegionInfo?.Name?.ContainsKey(language) ?? false ? pgdata.LocationInfo?.RegionInfo?.Name?[language] : "";
                }
            }

            var tvinfolocalized = new TvInfoLocalized() { };
            if (pgdata.LocationInfo != null)
            {
                if (pgdata.LocationInfo.TvInfo != null)
                {
                    tvinfolocalized.Id = pgdata.LocationInfo.TvInfo.Id;
                    tvinfolocalized.Name = pgdata.LocationInfo?.TvInfo?.Name?.ContainsKey(language) ?? false ? pgdata.LocationInfo?.TvInfo?.Name?[language] : "";
                }
            }

            data.LocationInfo = new LocationInfoLocalized()
            {
                DistrictInfo = distinfolocalized,
                MunicipalityInfo = muninfolocalized,
                TvInfo = tvinfolocalized,
                RegionInfo = reginfolocalized
            };

            return data;
        }

        public static EventLocalized TransformToEventLocalized(Event psdata, string language)
        {
            EventLocalized data = new EventLocalized();

            data.Id = psdata.Id;
            data.Active = psdata.Active;
            data.Altitude = psdata.Altitude;
            data.AltitudeUnitofMeasure = psdata.AltitudeUnitofMeasure;
            data.ContactInfos = psdata.ContactInfos != null ? psdata.ContactInfos.ContainsKey(language) ? psdata.ContactInfos[language] : null : null;
            data.DateBegin = psdata.DateBegin;
            data.DateEnd = psdata.DateEnd;
            data.Detail = psdata.Detail != null ? psdata.Detail.ContainsKey(language) ? psdata.Detail[language] : null : null;
            data.DistrictId = psdata.DistrictId;
            data.DistrictIds = psdata.DistrictIds;
            data.EventAdditionalInfos = psdata.EventAdditionalInfos != null ? psdata.EventAdditionalInfos.ContainsKey(language) ? psdata.EventAdditionalInfos[language] : null : null;
            data.EventDate = psdata.EventDate;
            data.EventPrice = psdata.EventPrice != null ? psdata.EventPrice.ContainsKey(language) ? psdata.EventPrice[language] : null : null;
            data.EventPublisher = psdata.EventPublisher;
            data.Gpstype = psdata.Gpstype;
            data.ImageGallery = psdata.ImageGallery != null ? psdata.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
            data.Latitude = psdata.Latitude;
            data.Longitude = psdata.Longitude;
            data.OrganizerInfos = psdata.OrganizerInfos != null ? psdata.OrganizerInfos.ContainsKey(language) ? psdata.OrganizerInfos[language] : null : null;
            data.OrgRID = psdata.OrgRID;
            data.PayMet = psdata.PayMet;
            data.Ranc = psdata.Ranc;
            data.Shortname = psdata.Shortname;
            data.SignOn = psdata.SignOn;
            data.SmgActive = psdata.SmgActive;
            data.SmgTags = psdata.SmgTags;
            data.Ticket = psdata.Ticket;
            data.TopicRIDs = psdata.TopicRIDs;
            data.Type = psdata.Type;

            var distinfolocalized = new DistrictInfoLocalized() { };
            if (psdata.LocationInfo != null)
            {
                if (psdata.LocationInfo.DistrictInfo != null)
                {
                    distinfolocalized.Id = psdata.LocationInfo.DistrictInfo.Id;
                    distinfolocalized.Name = psdata.LocationInfo?.DistrictInfo?.Name?.ContainsKey(language) ?? false ? psdata.LocationInfo?.DistrictInfo?.Name?[language] : "";
                }
            }

            var muninfolocalized = new MunicipalityInfoLocalized() { };
            if (psdata.LocationInfo != null)
            {
                if (psdata.LocationInfo.MunicipalityInfo != null)
                {
                    muninfolocalized.Id = psdata.LocationInfo.MunicipalityInfo.Id;
                    muninfolocalized.Name = psdata.LocationInfo?.MunicipalityInfo?.Name?.ContainsKey(language) ?? false ? psdata.LocationInfo?.MunicipalityInfo?.Name?[language] : "";
                }
            }

            var reginfolocalized = new RegionInfoLocalized() { };
            if (psdata.LocationInfo != null)
            {
                if (psdata.LocationInfo.RegionInfo != null)
                {
                    reginfolocalized.Id = psdata.LocationInfo.RegionInfo.Id;
                    reginfolocalized.Name = psdata.LocationInfo.RegionInfo?.Name?.ContainsKey(language) ?? false ? psdata.LocationInfo?.RegionInfo?.Name?[language] : "";
                }
            }

            var tvinfolocalized = new TvInfoLocalized() { };
            if (psdata.LocationInfo != null)
            {
                if (psdata.LocationInfo.TvInfo != null)
                {
                    tvinfolocalized.Id = psdata.LocationInfo.TvInfo.Id;
                    tvinfolocalized.Name = psdata.LocationInfo?.TvInfo?.Name?.ContainsKey(language) ?? false ? psdata.LocationInfo?.TvInfo?.Name?[language] : "";
                }
            }

            data.LocationInfo = new LocationInfoLocalized()
            {
                DistrictInfo = distinfolocalized,
                MunicipalityInfo = muninfolocalized,
                TvInfo = tvinfolocalized,
                RegionInfo = reginfolocalized
            };

            return data;
        }

        public static ArticleBaseInfosLocalized TransformToArticleBaseInfosLocalized(Article poibaseinfo, string language)
        {
            ArticleBaseInfosLocalized data = new ArticleBaseInfosLocalized();

            data.Id = poibaseinfo.Id;
            data.LastChange = poibaseinfo.LastChange;
            data.FirstImport = poibaseinfo.FirstImport;
            data.Active = poibaseinfo.Active;
            data.AdditionalArticleInfos = poibaseinfo.AdditionalArticleInfos != null ? poibaseinfo.AdditionalArticleInfos.ContainsKey(language) ? poibaseinfo.AdditionalArticleInfos[language] : null : null;
            data.ContactInfos = poibaseinfo.ContactInfos != null ? poibaseinfo.ContactInfos.ContainsKey(language) ? poibaseinfo.ContactInfos[language] : null : null;
            data.Detail = poibaseinfo.Detail != null ? poibaseinfo.Detail.ContainsKey(language) ? poibaseinfo.Detail[language] : null : null;
            data.GpsInfo = poibaseinfo.GpsInfo;
            data.GpsTrack = poibaseinfo.GpsTrack;
            data.Highlight = poibaseinfo.Highlight;
            data.OperationSchedule = poibaseinfo.OperationSchedule;
            data.Shortname = poibaseinfo.Shortname;
            data.SmgActive = poibaseinfo.SmgActive;
            data.SubType = poibaseinfo.SubType;
            data.Type = poibaseinfo.Type;
            data.SmgTags = poibaseinfo.SmgTags;

            data.ImageGallery = poibaseinfo.ImageGallery != null ? poibaseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;

            //MarketingGroupIds = acco.MarketingGroupIds,                    

            return data;
        }

        public static ODHActivityPoiLocalized TransformToODHActivityPoiLocalized(ODHActivityPoi poibaseinfo, string language)
        {
            ODHActivityPoiLocalized data = new ODHActivityPoiLocalized();

            data.Id = poibaseinfo.Id;
            data.LastChange = poibaseinfo.LastChange;
            data.FirstImport = poibaseinfo.FirstImport;
            data.Active = poibaseinfo.Active;
            data.AdditionalPoiInfos = poibaseinfo.AdditionalPoiInfos != null ? poibaseinfo.AdditionalPoiInfos.Count > 0 ? poibaseinfo.AdditionalPoiInfos.ContainsKey(language) ? poibaseinfo.AdditionalPoiInfos[language] : null : null : null;
            data.ContactInfos = poibaseinfo.ContactInfos != null ? poibaseinfo.ContactInfos.ContainsKey(language) ? poibaseinfo.ContactInfos[language] : null : null;
            data.Detail = poibaseinfo.Detail != null ? poibaseinfo.Detail.ContainsKey(language) ? poibaseinfo.Detail[language] : null : null;
            data.GpsInfo = poibaseinfo.GpsInfo;
            data.GpsTrack = poibaseinfo.GpsTrack;
            data.GpsPoints = poibaseinfo.GpsPoints;
            data.Highlight = poibaseinfo.Highlight;
            data.OperationSchedule = poibaseinfo.OperationSchedule;
            data.Shortname = poibaseinfo.Shortname;
            data.SmgActive = poibaseinfo.SmgActive;
            data.SubType = poibaseinfo.SubType;
            data.Type = poibaseinfo.Type;
            data.SmgTags = poibaseinfo.SmgTags;
            data.ImageGallery = poibaseinfo.ImageGallery != null ? poibaseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
            data.AltitudeDifference = poibaseinfo.AltitudeDifference;
            data.AltitudeHighestPoint = poibaseinfo.AltitudeHighestPoint;
            data.AltitudeLowestPoint = poibaseinfo.AltitudeLowestPoint;
            data.AltitudeSumDown = poibaseinfo.AltitudeSumDown;
            data.AltitudeSumUp = poibaseinfo.AltitudeSumUp;
            data.AreaId = poibaseinfo.AreaId;
            data.Difficulty = poibaseinfo.Difficulty;
            data.DistanceDuration = poibaseinfo.DistanceDuration;
            data.DistanceLength = poibaseinfo.DistanceLength;
            data.Exposition = poibaseinfo.Exposition;
            data.FeetClimb = poibaseinfo.FeetClimb;
            data.HasFreeEntrance = poibaseinfo.HasFreeEntrance;
            data.HasRentals = poibaseinfo.HasRentals;
            data.IsOpen = poibaseinfo.IsOpen;
            data.IsPrepared = poibaseinfo.IsPrepared;
            data.IsWithLigth = poibaseinfo.IsWithLigth;
            data.LiftAvailable = poibaseinfo.LiftAvailable;
            data.BikeTransport = poibaseinfo.BikeTransport;
            data.PoiProperty = poibaseinfo.PoiProperty != null ? poibaseinfo.PoiProperty.ContainsKey(language) ? poibaseinfo.PoiProperty[language] : null : null;
            data.PoiServices = poibaseinfo.PoiServices;
            data.PoiType = poibaseinfo.PoiType;
            data.Ratings = poibaseinfo.Ratings;
            data.RunToValley = poibaseinfo.RunToValley;
            data.SmgId = poibaseinfo.SmgId;
            data.SubType = poibaseinfo.SubType;
            data.TourismorganizationId = poibaseinfo.TourismorganizationId;
            data.AgeFrom = poibaseinfo.AgeFrom;
            data.AgeTo = poibaseinfo.AgeTo;
            data.SyncSourceInterface = poibaseinfo.SyncSourceInterface;
            data.SyncUpdateMode = poibaseinfo.SyncUpdateMode;
            data.Source = poibaseinfo.Source;
            //data.MaxSeatingCapacity = poibaseinfo.MaxSeatingCapacity weiter

            var distinfolocalized = new DistrictInfoLocalized() { };
            if (poibaseinfo.LocationInfo != null)
            {
                if (poibaseinfo.LocationInfo.DistrictInfo != null)
                {
                    distinfolocalized.Id = poibaseinfo.LocationInfo.DistrictInfo.Id;
                    distinfolocalized.Name = poibaseinfo.LocationInfo?.DistrictInfo?.Name?.ContainsKey(language) ?? false ? poibaseinfo.LocationInfo?.DistrictInfo?.Name?[language] : "";
                }
            }

            var muninfolocalized = new MunicipalityInfoLocalized() { };
            if (poibaseinfo.LocationInfo != null)
            {
                if (poibaseinfo.LocationInfo.MunicipalityInfo != null)
                {
                    muninfolocalized.Id = poibaseinfo.LocationInfo.MunicipalityInfo.Id;
                    muninfolocalized.Name = poibaseinfo.LocationInfo?.MunicipalityInfo?.Name?.ContainsKey(language) ?? false ? poibaseinfo.LocationInfo?.MunicipalityInfo?.Name?[language] : "";
                }
            }

            var reginfolocalized = new RegionInfoLocalized() { };
            if (poibaseinfo.LocationInfo != null)
            {
                if (poibaseinfo.LocationInfo.RegionInfo != null)
                {
                    reginfolocalized.Id = poibaseinfo.LocationInfo.RegionInfo.Id;
                    reginfolocalized.Name = poibaseinfo.LocationInfo?.RegionInfo?.Name?.ContainsKey(language) ?? false ? poibaseinfo.LocationInfo?.RegionInfo?.Name?[language] : "";
                }
            }

            var tvinfolocalized = new TvInfoLocalized() { };
            if (poibaseinfo.LocationInfo != null)
            {
                if (poibaseinfo.LocationInfo.TvInfo != null)
                {
                    tvinfolocalized.Id = poibaseinfo.LocationInfo.TvInfo.Id;
                    tvinfolocalized.Name = poibaseinfo.LocationInfo?.TvInfo?.Name?.ContainsKey(language) ?? false ? poibaseinfo.LocationInfo?.TvInfo?.Name?[language] : "";
                }
            }

            data.LocationInfo = new LocationInfoLocalized()
            {
                DistrictInfo = distinfolocalized,
                MunicipalityInfo = muninfolocalized,
                TvInfo = tvinfolocalized,
                RegionInfo = reginfolocalized
            };
            //MarketingGroupIds = acco.MarketingGroupIds,                    

            return data;
        }

        public static PackageLocalized TransformToPackageLocalized(Package package, string language)
        {
            PackageLocalized data = new PackageLocalized();
            data.Active = package.Active;

            data.ChannelInfo = package.ChannelInfo;
            data.ChildrenMin = package.ChildrenMin;
            data.DaysArrival = package.DaysArrival;
            data.DaysArrivalMax = package.DaysArrivalMax;
            data.DaysArrivalMin = package.DaysArrivalMin;
            data.DaysDeparture = package.DaysDeparture;
            data.DaysDurMax = package.DaysDurMax;
            data.DaysDurMin = package.DaysDurMin;
            data.HotelHgvId = package.HotelHgvId;
            data.HotelId = package.HotelId;
            data.Id = package.Id;
            data.ImageGallery = package.ImageGallery != null ? package.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
            data.Inclusive = package.Inclusive != null ? package.Inclusive.Select(x => new InclusiveLocalized()
            {
                ImageGallery = x.Value.ImageGallery != null ? x.Value.ImageGallery.Select(y => new ImageGalleryLocalized() { Height = y.Height, ImageDesc = y.ImageDesc.ContainsKey(language) ? y.ImageDesc[language] : "", ImageName = y.ImageName, ImageSource = y.ImageSource, ImageTitle = y.ImageTitle.ContainsKey(language) ? y.ImageTitle[language] : "", ImageUrl = y.ImageUrl, IsInGallery = y.IsInGallery, ListPosition = y.ListPosition, ValidFrom = y.ValidFrom, ValidTo = y.ValidTo, Width = y.Width, CopyRight = y.CopyRight }).ToList() : null,
                PackageDetail = x.Value.PackageDetail != null ? x.Value.PackageDetail.ContainsKey(language) ? x.Value.PackageDetail[language] : null : null,
                PriceId = x.Value.PriceId,
                PriceTyp = x.Value.PriceTyp
            }).ToList() : null;
            data.LocationInfo = new LocationInfoLocalized()
            {
                DistrictInfo = package.LocationInfo != null ? package.LocationInfo.DistrictInfo != null ? new DistrictInfoLocalized() { Id = package.LocationInfo.DistrictInfo.Id, Name = package.LocationInfo?.DistrictInfo?.Name?[language] } : new DistrictInfoLocalized() : new DistrictInfoLocalized(),
                MunicipalityInfo = package.LocationInfo != null ? package.LocationInfo.MunicipalityInfo != null ? new MunicipalityInfoLocalized() { Id = package.LocationInfo.MunicipalityInfo.Id, Name = package.LocationInfo?.MunicipalityInfo?.Name?[language] } : new MunicipalityInfoLocalized() : new MunicipalityInfoLocalized(),
                TvInfo = package.LocationInfo != null ? package.LocationInfo.TvInfo != null ? new TvInfoLocalized() { Id = package.LocationInfo.TvInfo.Id, Name = package.LocationInfo?.TvInfo?.Name?[language] } : new TvInfoLocalized() : new TvInfoLocalized(),
                RegionInfo = package.LocationInfo != null ? package.LocationInfo.RegionInfo != null ? new RegionInfoLocalized() { Id = package.LocationInfo.RegionInfo.Id, Name = package.LocationInfo?.RegionInfo?.Name?[language] } : new RegionInfoLocalized() : new RegionInfoLocalized()
            };
            data.OfferId = package.OfferId;
            data.Offertyp = package.Offertyp;
            data.PackageDetail = package.PackageDetail != null ? package.PackageDetail.ContainsKey(language) ? package.PackageDetail[language] : null : null;
            data.Season = package.Season;
            data.Shortname = package.Shortname;
            data.SmgActive = package.SmgActive;
            data.SmgTags = package.SmgTags;
            data.Specialtyp = package.Specialtyp;
            data.ValidStart = package.ValidStart;
            data.ValidStop = package.ValidStop;
            data.Services = package.Services;
            data.PackageThemeDetail = package.PackageThemeDetail != null ? package.PackageThemeDetail.Select(x => new PackageThemeLocalized()
            {
                ThemeId = x.ThemeId,
                ThemeDetail = x.ThemeDetail.ContainsKey(language) ? x.ThemeDetail[language] : null
            }).ToList() : null;



            //var distinfolocalized = new DistrictInfoLocalized() { };
            //if (acco.LocationInfo.DistrictInfo != null)
            //{
            //    distinfolocalized.Id = acco.LocationInfo.DistrictInfo.Id;
            //    distinfolocalized.Name = acco.LocationInfo.DistrictInfo.Name.ContainsKey(language) ? acco.LocationInfo.DistrictInfo.Name[language] : "";
            //}

            //var muninfolocalized = new MunicipalityInfoLocalized() { };
            //if (acco.LocationInfo.MunicipalityInfo != null)
            //{
            //    muninfolocalized.Id = acco.LocationInfo.MunicipalityInfo.Id;
            //    muninfolocalized.Name = acco.LocationInfo.MunicipalityInfo.Name.ContainsKey(language) ? acco.LocationInfo.MunicipalityInfo.Name[language] : "";
            //}

            //var reginfolocalized = new RegionInfoLocalized() { };
            //if (acco.LocationInfo.RegionInfo != null)
            //{
            //    reginfolocalized.Id = acco.LocationInfo.RegionInfo.Id;
            //    reginfolocalized.Name = acco.LocationInfo.RegionInfo.Name.ContainsKey(language) ? acco.LocationInfo.RegionInfo.Name[language] : "";
            //}

            //var tvinfolocalized = new TvInfoLocalized() { };
            //if (acco.LocationInfo.TvInfo != null)
            //{
            //    tvinfolocalized.Id = acco.LocationInfo.TvInfo.Id;
            //    tvinfolocalized.Name = acco.LocationInfo.TvInfo.Name.ContainsKey(language) ? acco.LocationInfo.TvInfo.Name[language] : "";
            //}

            //data.LocationInfo = new LocationInfoLocalized()
            //{
            //    DistrictInfo = distinfolocalized,
            //    MunicipalityInfo = muninfolocalized,
            //    TvInfo = tvinfolocalized,
            //    RegionInfo = reginfolocalized
            //};

            return data;

        }

        public static PackageBookList TransformToPackageBooklist(Package package, string language)
        {
            PackageBookList data = new PackageBookList();
            data.Id = package.Id;
            data.OfferId = package.OfferId;
            data.HotelId = package.HotelId;
            data.HotelHgvId = package.HotelHgvId;

            return data;
        }

        public static BaseInfosLocalized TransformToBaseInfosLocalized(BaseInfos baseinfo, string language)
        {
            BaseInfosLocalized data = new BaseInfosLocalized();

            data.Id = baseinfo.Id;
            data.Active = baseinfo.Active;
            data.ContactInfos = baseinfo.ContactInfos != null ? baseinfo.ContactInfos.ContainsKey(language) ? baseinfo.ContactInfos[language] : null : null;
            data.Detail = baseinfo.Detail != null ? baseinfo.Detail.ContainsKey(language) ? baseinfo.Detail[language] : null : null;
            data.Gpstype = baseinfo.Gpstype;
            data.Latitude = baseinfo.Latitude;
            data.Longitude = baseinfo.Longitude;
            data.Altitude = baseinfo.Altitude;
            data.AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure;
            data.ImageGallery = baseinfo.ImageGallery != null ? baseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
            data.CustomId = baseinfo.CustomId;
            data.Shortname = baseinfo.Shortname;
            data.SmgActive = baseinfo.SmgActive;
            data.SmgTags = baseinfo.SmgTags;

            return data;
        }

        public static RegionLocalized TransformToRegionLocalized(Region baseinfo, string language)
        {
            RegionLocalized data = new RegionLocalized();

            data.Id = baseinfo.Id;
            data.Active = baseinfo.Active;
            data.ContactInfos = baseinfo.ContactInfos != null ? baseinfo.ContactInfos.ContainsKey(language) ? baseinfo.ContactInfos[language] : null : null;
            data.Detail = baseinfo.Detail != null ? baseinfo.Detail.ContainsKey(language) ? baseinfo.Detail[language] : null : null;
            data.Gpstype = baseinfo.Gpstype;
            data.Latitude = baseinfo.Latitude;
            data.Longitude = baseinfo.Longitude;
            data.Altitude = baseinfo.Altitude;
            data.AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure;
            data.ImageGallery = baseinfo.ImageGallery != null ? baseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
            data.CustomId = baseinfo.CustomId;
            data.Shortname = baseinfo.Shortname;
            data.SmgActive = baseinfo.SmgActive;
            data.SmgTags = baseinfo.SmgTags;
            data.DetailThemed = baseinfo.DetailThemed != null ? baseinfo.DetailThemed.ContainsKey(language) ? baseinfo.DetailThemed[language] : null : null;
            data.GpsPolygon = baseinfo.GpsPolygon;
            data.Webcam = baseinfo.Webcam != null ? baseinfo.Webcam.Select(x => new WebcamLocalized() { GpsInfo = x.GpsInfo, WebcamId = x.WebcamId, Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "", Webcamurl = x.Webcamurl }).ToList() : null;
            data.VisibleInSearch = baseinfo.VisibleInSearch;
            data.SkiareaIds = baseinfo.SkiareaIds;

            return data;
        }

        public static TourismvereinLocalized TransformToTourismvereinLocalized(Tourismverein baseinfo, string language)
        {
            TourismvereinLocalized data = new TourismvereinLocalized();

            data.Id = baseinfo.Id;
            data.Active = baseinfo.Active;
            data.ContactInfos = baseinfo.ContactInfos != null ? baseinfo.ContactInfos.ContainsKey(language) ? baseinfo.ContactInfos[language] : null : null;
            data.Detail = baseinfo.Detail != null ? baseinfo.Detail.ContainsKey(language) ? baseinfo.Detail[language] : null : null;
            data.Gpstype = baseinfo.Gpstype;
            data.Latitude = baseinfo.Latitude;
            data.Longitude = baseinfo.Longitude;
            data.Altitude = baseinfo.Altitude;
            data.AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure;
            data.ImageGallery = baseinfo.ImageGallery != null ? baseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
            data.CustomId = baseinfo.CustomId;
            data.Shortname = baseinfo.Shortname;
            data.SmgActive = baseinfo.SmgActive;
            data.SmgTags = baseinfo.SmgTags;
            data.GpsPolygon = baseinfo.GpsPolygon;
            data.Webcam = baseinfo.Webcam != null ? baseinfo.Webcam.Select(x => new WebcamLocalized() { GpsInfo = x.GpsInfo, WebcamId = x.WebcamId, Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "", Webcamurl = x.Webcamurl }).ToList() : null;
            data.VisibleInSearch = baseinfo.VisibleInSearch;
            data.SkiareaIds = baseinfo.SkiareaIds;
            data.RegionId = baseinfo.RegionId;

            return data;
        }

        public static MunicipalityLocalized TransformToMunicipalityLocalized(Municipality baseinfo, string language)
        {
            MunicipalityLocalized data = new MunicipalityLocalized();

            data.Id = baseinfo.Id;
            data.Active = baseinfo.Active;
            data.ContactInfos = baseinfo.ContactInfos != null ? baseinfo.ContactInfos.ContainsKey(language) ? baseinfo.ContactInfos[language] : null : null;
            data.Detail = baseinfo.Detail != null ? baseinfo.Detail.ContainsKey(language) ? baseinfo.Detail[language] : null : null;
            data.Gpstype = baseinfo.Gpstype;
            data.Latitude = baseinfo.Latitude;
            data.Longitude = baseinfo.Longitude;
            data.Altitude = baseinfo.Altitude;
            data.AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure;
            data.ImageGallery = baseinfo.ImageGallery != null ? baseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
            data.CustomId = baseinfo.CustomId;
            data.Shortname = baseinfo.Shortname;
            data.SmgActive = baseinfo.SmgActive;
            data.SmgTags = baseinfo.SmgTags;
            data.GpsPolygon = baseinfo.GpsPolygon;
            data.Webcam = baseinfo.Webcam != null ? baseinfo.Webcam.Select(x => new WebcamLocalized() { GpsInfo = x.GpsInfo, WebcamId = x.WebcamId, Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "", Webcamurl = x.Webcamurl }).ToList() : null;
            data.VisibleInSearch = baseinfo.VisibleInSearch;
            data.RegionId = baseinfo.RegionId;
            data.Plz = baseinfo.Plz;
            data.TourismvereinId = baseinfo.TourismvereinId;
            data.SiagId = baseinfo.SiagId;
            data.Inhabitants = baseinfo.Inhabitants;
            data.IstatNumber = baseinfo.IstatNumber;

            return data;
        }

        public static DistrictLocalized TransformToDistrictLocalized(District baseinfo, string language)
        {
            DistrictLocalized data = new DistrictLocalized();

            data.Id = baseinfo.Id;
            data.Active = baseinfo.Active;
            data.ContactInfos = baseinfo.ContactInfos != null ? baseinfo.ContactInfos.ContainsKey(language) ? baseinfo.ContactInfos[language] : null : null;
            data.Detail = baseinfo.Detail != null ? baseinfo.Detail.ContainsKey(language) ? baseinfo.Detail[language] : null : null;
            data.Gpstype = baseinfo.Gpstype;
            data.Latitude = baseinfo.Latitude;
            data.Longitude = baseinfo.Longitude;
            data.Altitude = baseinfo.Altitude;
            data.AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure;
            data.ImageGallery = baseinfo.ImageGallery != null ? baseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
            data.CustomId = baseinfo.CustomId;
            data.Shortname = baseinfo.Shortname;
            data.SmgActive = baseinfo.SmgActive;
            data.SmgTags = baseinfo.SmgTags;
            data.GpsPolygon = baseinfo.GpsPolygon;
            data.Webcam = baseinfo.Webcam != null ? baseinfo.Webcam.Select(x => new WebcamLocalized() { GpsInfo = x.GpsInfo, WebcamId = x.WebcamId, Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "", Webcamurl = x.Webcamurl }).ToList() : null;
            data.VisibleInSearch = baseinfo.VisibleInSearch;
            data.RegionId = baseinfo.RegionId;
            data.IsComune = baseinfo.IsComune;
            data.TourismvereinId = baseinfo.TourismvereinId;
            data.SiagId = baseinfo.SiagId;
            data.MunicipalityId = baseinfo.MunicipalityId;

            return data;
        }

        public static MetaRegionLocalized TransformToMetaRegionLocalized(MetaRegion baseinfo, string language)
        {
            MetaRegionLocalized data = new MetaRegionLocalized();

            data.Id = baseinfo.Id;
            data.Active = baseinfo.Active;
            data.ContactInfos = baseinfo.ContactInfos != null ? baseinfo.ContactInfos.ContainsKey(language) ? baseinfo.ContactInfos[language] : null : null;
            data.Detail = baseinfo.Detail != null ? baseinfo.Detail.ContainsKey(language) ? baseinfo.Detail[language] : null : null;
            data.Gpstype = baseinfo.Gpstype;
            data.Latitude = baseinfo.Latitude;
            data.Longitude = baseinfo.Longitude;
            data.Altitude = baseinfo.Altitude;
            data.AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure;
            data.ImageGallery = baseinfo.ImageGallery != null ? baseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
            data.CustomId = baseinfo.CustomId;
            data.Shortname = baseinfo.Shortname;
            data.SmgActive = baseinfo.SmgActive;
            data.SmgTags = baseinfo.SmgTags;
            data.GpsPolygon = baseinfo.GpsPolygon;
            data.Webcam = baseinfo.Webcam != null ? baseinfo.Webcam.Select(x => new WebcamLocalized() { GpsInfo = x.GpsInfo, WebcamId = x.WebcamId, Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "", Webcamurl = x.Webcamurl }).ToList() : null;
            data.VisibleInSearch = baseinfo.VisibleInSearch;
            data.DistrictIds = baseinfo.DistrictIds;
            data.TourismvereinIds = baseinfo.TourismvereinIds;
            data.RegionIds = baseinfo.RegionIds;
            data.DetailThemed = baseinfo.DetailThemed != null ? baseinfo.DetailThemed.ContainsKey(language) ? baseinfo.DetailThemed[language] : null : null;

            return data;
        }

        public static ExperienceAreaLocalized TransformToExperienceAreaLocalized(ExperienceArea baseinfo, string language)
        {
            ExperienceAreaLocalized data = new ExperienceAreaLocalized();

            data.Id = baseinfo.Id;
            data.Active = baseinfo.Active;
            data.ContactInfos = baseinfo.ContactInfos != null ? baseinfo.ContactInfos.ContainsKey(language) ? baseinfo.ContactInfos[language] : null : null;
            data.Detail = baseinfo.Detail != null ? baseinfo.Detail.ContainsKey(language) ? baseinfo.Detail[language] : null : null;
            data.Gpstype = baseinfo.Gpstype;
            data.Latitude = baseinfo.Latitude;
            data.Longitude = baseinfo.Longitude;
            data.Altitude = baseinfo.Altitude;
            data.AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure;
            data.ImageGallery = baseinfo.ImageGallery != null ? baseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
            data.CustomId = baseinfo.CustomId;
            data.Shortname = baseinfo.Shortname;
            data.SmgActive = baseinfo.SmgActive;
            data.SmgTags = baseinfo.SmgTags;
            data.GpsPolygon = baseinfo.GpsPolygon;
            //data.Webcam = baseinfo.Webcam != null ? baseinfo.Webcam.Select(x => new WebcamLocalized() { GpsInfo = x.GpsInfo, WebcamId = x.WebcamId, Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "", Webcamurl = x.Webcamurl }).ToList() : null;
            data.VisibleInSearch = baseinfo.VisibleInSearch;
            data.DistrictIds = baseinfo.DistrictIds;
            data.TourismvereinIds = baseinfo.TourismvereinIds;
            //data.RegionIds = baseinfo.RegionIds;
            //data.DetailThemed = baseinfo.DetailThemed != null ? baseinfo.DetailThemed.ContainsKey(language) ? baseinfo.DetailThemed[language] : null : null;

            return data;
        }

        public static SkiRegionLocalized TransformToSkiRegionLocalized(SkiRegion baseinfo, string language)
        {
            SkiRegionLocalized data = new SkiRegionLocalized();

            data.Id = baseinfo.Id;
            data.Active = baseinfo.Active;
            data.ContactInfos = baseinfo.ContactInfos != null ? baseinfo.ContactInfos.ContainsKey(language) ? baseinfo.ContactInfos[language] : null : null;
            data.Detail = baseinfo.Detail != null ? baseinfo.Detail.ContainsKey(language) ? baseinfo.Detail[language] : null : null;
            data.Gpstype = baseinfo.Gpstype;
            data.Latitude = baseinfo.Latitude;
            data.Longitude = baseinfo.Longitude;
            data.Altitude = baseinfo.Altitude;
            data.AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure;
            data.ImageGallery = baseinfo.ImageGallery != null ? baseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
            data.CustomId = baseinfo.CustomId;
            data.Shortname = baseinfo.Shortname;
            data.SmgActive = baseinfo.SmgActive;
            data.SmgTags = baseinfo.SmgTags;
            data.GpsPolygon = baseinfo.GpsPolygon;
            data.Webcam = baseinfo.Webcam != null ? baseinfo.Webcam.Select(x => new WebcamLocalized() { GpsInfo = x.GpsInfo, WebcamId = x.WebcamId, Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "", Webcamurl = x.Webcamurl }).ToList() : null;

            return data;
        }

        public static SkiAreaLocalized TransformToSkiAreaLocalized(SkiArea baseinfo, string language)
        {
            SkiAreaLocalized data = new SkiAreaLocalized();

            data.Id = baseinfo.Id;
            data.Active = baseinfo.Active;
            data.ContactInfos = baseinfo.ContactInfos != null ? baseinfo.ContactInfos.ContainsKey(language) ? baseinfo.ContactInfos[language] : null : null;
            data.Detail = baseinfo.Detail != null ? baseinfo.Detail.ContainsKey(language) ? baseinfo.Detail[language] : null : null;
            data.Gpstype = baseinfo.Gpstype;
            data.Latitude = baseinfo.Latitude;
            data.Longitude = baseinfo.Longitude;
            data.Altitude = baseinfo.Altitude;
            data.AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure;
            data.ImageGallery = baseinfo.ImageGallery != null ? baseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
            data.CustomId = baseinfo.CustomId;
            data.Shortname = baseinfo.Shortname;
            data.SmgActive = baseinfo.SmgActive;
            data.SmgTags = baseinfo.SmgTags;
            data.GpsPolygon = baseinfo.GpsPolygon;
            data.Webcam = baseinfo.Webcam != null ? baseinfo.Webcam.Select(x => new WebcamLocalized() { GpsInfo = x.GpsInfo, WebcamId = x.WebcamId, Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "", Webcamurl = x.Webcamurl }).ToList() : null;

            data.SkiRegionId = baseinfo.SkiRegionId;
            data.SkiAreaMapURL = baseinfo.SkiAreaMapURL;
            data.TotalSlopeKm = baseinfo.TotalSlopeKm;
            data.SlopeKmBlue = baseinfo.SlopeKmBlue;
            data.SlopeKmRed = baseinfo.SlopeKmRed;
            data.SlopeKmBlack = baseinfo.SlopeKmBlack;
            data.LiftCount = baseinfo.LiftCount;
            data.AltitudeFrom = baseinfo.AltitudeFrom;
            data.AltitudeTo = baseinfo.AltitudeTo;
            data.SkiRegionName = baseinfo.SkiRegionName != null ? baseinfo.SkiRegionName.ContainsKey(language) ? baseinfo.SkiRegionName[language] : "" : "";
            data.AreaId = baseinfo.AreaId;
            data.OperationSchedule = baseinfo.OperationSchedule;
            data.TourismvereinIds = baseinfo.TourismvereinIds;
            data.RegionIds = baseinfo.RegionIds;
            data.LocationInfo = new LocationInfoLocalized()
            {
                DistrictInfo = baseinfo.LocationInfo != null ? baseinfo.LocationInfo.DistrictInfo != null ? new DistrictInfoLocalized() { Id = baseinfo.LocationInfo.DistrictInfo.Id, Name = baseinfo.LocationInfo?.DistrictInfo?.Name?[language] } : new DistrictInfoLocalized() : new DistrictInfoLocalized(),
                MunicipalityInfo = baseinfo.LocationInfo != null ? baseinfo.LocationInfo.MunicipalityInfo != null ? new MunicipalityInfoLocalized() { Id = baseinfo.LocationInfo.MunicipalityInfo.Id, Name = baseinfo.LocationInfo?.MunicipalityInfo?.Name?[language] } : new MunicipalityInfoLocalized() : new MunicipalityInfoLocalized(),
                TvInfo = baseinfo.LocationInfo != null ? baseinfo.LocationInfo.TvInfo != null ? new TvInfoLocalized() { Id = baseinfo.LocationInfo.TvInfo.Id, Name = baseinfo.LocationInfo?.TvInfo?.Name?[language] } : new TvInfoLocalized() : new TvInfoLocalized(),
                RegionInfo = baseinfo.LocationInfo != null ? baseinfo.LocationInfo.RegionInfo != null ? new RegionInfoLocalized() { Id = baseinfo.LocationInfo.RegionInfo.Id, Name = baseinfo.LocationInfo?.RegionInfo?.Name?[language] } : new RegionInfoLocalized() : new RegionInfoLocalized()

            };


            return data;
        }

        public static MobileData TransformAccommodationToMobileDataObject(Accommodation acommodation, string language)
        {
            var nulldouble = 0;

            MobileData data = new MobileData();

            data.Id = acommodation.Id;
            data.Name = acommodation.AccoDetail?[language] != null ? acommodation.AccoDetail[language].Name != null ? acommodation.AccoDetail[language].Name : "" : "";
            data.Image = acommodation.ImageGallery?.Count > 0 ? acommodation.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ? acommodation.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl : acommodation.ImageGallery.FirstOrDefault().ImageUrl : "noimage";
            data.Region = acommodation.LocationInfo?.RegionInfo?.Name?[language];
            data.Tourismverein = acommodation.LocationInfo?.TvInfo?.Name?[language];
            data.Latitude = acommodation.Latitude;
            data.Longitude = acommodation.Longitude;
            data.Type = "acco";
            data.Category = new Dictionary<string, string>()
                                                 {
                                                     //Smgpoi
                                                     { "maintype", "" },
                                                     { "subtype", "" },
                                                     { "poitype", "" },
                                                     //Accommodation
                                                     { "board", String.Join(",", acommodation.BoardIds ?? Enumerable.Empty<string>()) },
                                                     { "type", acommodation.AccoTypeId ?? "" },
                                                     { "category", acommodation.AccoCategoryId ?? "" },
                                                     { "theme", String.Join(",", acommodation.ThemeIds ?? Enumerable.Empty<string>()) },
                                                     { "badge", String.Join(",", acommodation.BadgeIds ?? Enumerable.Empty<string>()) },
                                                     { "specialfeature", String.Join(",", acommodation.SpecialFeaturesIds ?? Enumerable.Empty<string>()) },
                                                     // //Gastronomy
                                                     //{ "categorycodes", "" },                                                     
                                                     //{ "ceremonycodes", "" },
                                                     //{ "dishcodes", "" },
                                                     ////EventTopic
                                                     //{ "EventTopic ", "" }
                                                 };
            data.Additional = new Dictionary<string, string>()
                                                 {
                                                     //SmgPoi
                                                     { "altitudedifference", nulldouble.ToString() },
                                                     { "distanceduration", nulldouble.ToString() },
                                                     { "difficulty", nulldouble.ToString() },
                                                     { "distancelength", nulldouble.ToString() },
                                                     //Accommodation
                                                     { "trustyouid", acommodation.TrustYouID != null ? acommodation.TrustYouID.ToString() : "" },
                                                     { "trustyouscore", acommodation.TrustYouScore.ToString() },
                                                     { "trustyouratings", acommodation.TrustYouResults.ToString() },        
                                                     //Gastronomy (no property)
                                                     //Event
                                                     { "topevent", "false" },
                                                     { "begindate", "" },
                                                     { "enddate", "" }
                                                 };

            return data;
        }

        public static MobileData TransformODHActivityPoiToMobileDataObject(ODHActivityPoi smgpoi, string language)
        {
            var nulldouble = 0;

            MobileData data = new MobileData();

            data.Id = smgpoi.Id;
            data.Name = smgpoi.Detail[language] != null ? smgpoi.Detail[language].Title != null ? smgpoi.Detail[language].Title : "" : "";
            //Wenn Image leer "noimage", wenn ImageListpos == 0 
            //Image = activity.ImageGallery != null ? activity.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ? activity.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageSource == "LTS" ? activity.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl + "&W=200" : activity.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl + "&width=200" : activity.ImageGallery.FirstOrDefault().ImageUrl + "&width=200" : "noimage",
            data.Image = smgpoi.ImageGallery?.Count > 0 ? smgpoi.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ? smgpoi.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl : smgpoi.ImageGallery.FirstOrDefault().ImageUrl : "noimage";
            data.Region = smgpoi.LocationInfo?.RegionInfo != null ? smgpoi.LocationInfo?.RegionInfo?.Name?[language] : "";
            data.Tourismverein = smgpoi.LocationInfo?.TvInfo != null ? smgpoi.LocationInfo?.TvInfo?.Name?[language] : "";
            data.Latitude = smgpoi.GpsInfo != null ? smgpoi.GpsInfo.Count > 0 ? smgpoi.GpsInfo.Where(x => x.Gpstype == "position").Count() > 0 ? smgpoi.GpsInfo.Where(x => x.Gpstype == "position").FirstOrDefault().Latitude : smgpoi.GpsInfo.FirstOrDefault().Latitude : 0 : 0;
            data.Longitude = smgpoi.GpsInfo != null ? smgpoi.GpsInfo.Count > 0 ? smgpoi.GpsInfo.Where(x => x.Gpstype == "position").Count() > 0 ? smgpoi.GpsInfo.Where(x => x.Gpstype == "position").FirstOrDefault().Longitude : smgpoi.GpsInfo.FirstOrDefault().Longitude : 0 : 0;
            data.Type = "smgpoi";
            data.Category = new Dictionary<string, string>()
                                                 {
                                                     //Smgpoi
                                                     { "maintype", smgpoi.AdditionalPoiInfos?[language].MainType ?? "" },
                                                     { "subtype", smgpoi.AdditionalPoiInfos?[language].SubType ?? "" },
                                                     { "poitype", smgpoi.AdditionalPoiInfos?[language].PoiType ?? "" },
                                                     //Accommodation
                                                     { "board", "" },
                                                     { "type", "" },
                                                     { "category", "" },
                                                     { "theme", "" },
                                                     { "badge", "" },
                                                     { "specialfeature", "" },
                                                     ////Gastronomy
                                                     //{ "categorycodes", "" },                                                     
                                                     //{ "ceremonycodes", "" },
                                                     //{ "dishcodes", "" },
                                                     ////Events
                                                     //{ "eventtopic ", "" }
                                                 };
            data.Additional = new Dictionary<string, string>()
                                                 {
                                                     //SmgPoi
                                                     { "altitudedifference", smgpoi.AltitudeDifference.ToString() },
                                                     { "distanceduration", smgpoi.DistanceDuration.ToString() },
                                                     { "difficulty", smgpoi.Difficulty ?? nulldouble.ToString() },
                                                     { "distancelength", smgpoi.DistanceLength.ToString() },
                                                     //Accommodation
                                                     { "trustyouid", "" },
                                                     { "trustyouscore", nulldouble.ToString() },
                                                     { "trustyouratings", nulldouble.ToString() },   
                                                     //Gastronomy
                                                     //Event
                                                     { "topevent", "false" },
                                                     { "begindate", "" },
                                                     { "enddate", "" }
                                                 };

            return data;
        }

        public static MobileData TransformEventToMobileDataObject(Event eventinfo, string language)
        {
            var nulldouble = 0;

            MobileData data = new MobileData();

            data.Id = eventinfo.Id;
            data.Name = eventinfo.Detail[language] != null ? eventinfo.Detail[language].Title != null ? eventinfo.Detail[language].Title : "" : "";
            //Image = eventinfo.ImageGallery != null ? eventinfo.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ? eventinfo.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageSource == "LTS" ? eventinfo.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl + "&W=200" : eventinfo.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl + "&width=200" : eventinfo.ImageGallery.FirstOrDefault().ImageUrl + "&width=200" : "noimage",
            data.Image = eventinfo.ImageGallery?.Count > 0 ? eventinfo.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ? eventinfo.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl : eventinfo.ImageGallery.FirstOrDefault().ImageUrl : "noimage";
            data.Region = eventinfo.LocationInfo?.RegionInfo?.Name?[language];
            data.Tourismverein = eventinfo.LocationInfo?.TvInfo?.Name?[language];
            data.Latitude = eventinfo.Latitude;
            data.Longitude = eventinfo.Longitude;
            data.Type = "event";
            data.Category = new Dictionary<string, string>()
                                                 {
                                                     //Smgpoi
                                                     { "maintype", "" },
                                                     { "subtype", "" },
                                                     { "poitype", "" },
                                                     //Accommodation
                                                     { "board", "" },
                                                     { "type", "" },
                                                     { "category", "" },
                                                     { "theme", "" },
                                                     { "badge", "" },
                                                     { "specialfeature", "" },
                                                     // //Gastronomy
                                                     //{ "categorycodes", "" },                                                     
                                                     //{ "ceremonycodes", "" },
                                                     //{ "dishcodes", "" },
                                                     ////Event
                                                     //{ "eventtopic", String.Join(",", eventinfo.TopicRIDs) }
                                                 };
            data.Additional = new Dictionary<string, string>()
                                                 {
                                                     //SmgPoi
                                                     { "altitudedifference", nulldouble.ToString() },
                                                     { "distanceduration", nulldouble.ToString() },
                                                     { "difficulty", nulldouble.ToString() },
                                                     { "distancelength", nulldouble.ToString() },
                                                     //Accommodation
                                                     { "trustyouscore", nulldouble.ToString() },
                                                     { "trustyouratings", nulldouble.ToString() },  
                                                     //Gastronomy
                                                     //Event                                                     
                                                     { "topevent", eventinfo.SmgTags != null ? eventinfo.SmgTags.Contains("TopEvent") ? "true" : "false" : "false" },
                                                     { "begindate", String.Format("{0:MM/dd/yyyy}", eventinfo.DateBegin) },
                                                     { "enddate", String.Format("{0:MM/dd/yyyy}", eventinfo.DateEnd) }
                                                     //{ "begindate", ((DateTime)eventinfo.DateBegin).ToShortDateString() },
                                                     //{ "enddate", ((DateTime)eventinfo.DateEnd).ToShortDateString() }

                                                 };
            return data;

        }

        public static MobileDataExtended TransformODHActivityPoiToMobileDataExtendedObject(ODHActivityPoi activity, string language)
        {
            var nulldouble = 0;

            MobileDataExtended data = new MobileDataExtended();

            data.Id = activity.Id;
            data.Name = activity.Detail[language] != null ? activity.Detail[language].Title != null ? activity.Detail[language].Title : "" : "";
            //Wenn Image leer "noimage", wenn ImageListpos == 0 
            data.ShortDesc = activity.Detail[language] != null ? activity.Detail[language].MetaDesc != null ? activity.Detail[language].MetaDesc : "" : "";
            data.Image = activity.ImageGallery?.Count > 0 ? activity.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ? activity.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl : activity.ImageGallery.FirstOrDefault().ImageUrl : "noimage";
            data.Region = activity.LocationInfo?.RegionInfo?.Name?[language];
            data.Tourismverein = activity.LocationInfo?.TvInfo?.Name?[language];
            data.Latitude = activity.GpsInfo != null ? activity.GpsInfo.Count > 0 ? activity.GpsInfo.Where(x => x.Gpstype == "position").Count() > 0 ? activity.GpsInfo.Where(x => x.Gpstype == "position").FirstOrDefault().Latitude : activity.GpsInfo.FirstOrDefault().Latitude : 0 : 0;
            data.Longitude = activity.GpsInfo != null ? activity.GpsInfo.Count > 0 ? activity.GpsInfo.Where(x => x.Gpstype == "position").Count() > 0 ? activity.GpsInfo.Where(x => x.Gpstype == "position").FirstOrDefault().Longitude : activity.GpsInfo.FirstOrDefault().Longitude : 0 : 0;
            data.Type = "smgpoi";
            data.Category = new Dictionary<string, string>()
                                                 {
                                                     //Smgpoi
                                                     { "maintype", activity.AdditionalPoiInfos?[language]?.MainType ?? "" },
                                                     { "subtype", activity.AdditionalPoiInfos?[language]?.SubType ?? "" },
                                                     { "poitype", activity.AdditionalPoiInfos?[language]?.PoiType ?? "" },
                                                     //Accommodation
                                                     { "board", "" },
                                                     { "type", "" },
                                                     { "category", "" },
                                                     { "theme", "" },
                                                     { "badge", "" },
                                                     { "specialfeature", "" },
                                                     ////Gastronomy
                                                     //{ "categorycodes", "" },                                                     
                                                     //{ "ceremonycodes", "" },
                                                     //{ "dishcodes", "" },
                                                     ////Events
                                                     //{ "eventtopic ", "" }
                                                 };
            data.Additional = new Dictionary<string, string>()
                                                 {
                                                     //SmgPoi
                                                     { "altitudedifference", activity.AltitudeDifference.ToString() },
                                                     { "distanceduration", activity.DistanceDuration.ToString() },
                                                     { "difficulty", activity.Difficulty != null ? activity.Difficulty : nulldouble.ToString() },
                                                     { "distancelength", activity.DistanceLength.ToString() },
                                                     //Accommodation
                                                     { "trustyouid", "" },
                                                     { "trustyouscore", nulldouble.ToString() },
                                                     { "trustyouratings", nulldouble.ToString() },   
                                                     //Gastronomy
                                                     //Event
                                                     { "topevent", "false" },
                                                     { "begindate", "" },
                                                     { "enddate", "" }
                                                 };

            return data;
        }

        public static MobileDetail TransformEventToMobileDetail(Event myevent, string language)
        {
            MobileDetail data = new MobileDetail();
            data.Id = myevent.Id;
            data.AltitudeDifference = 0;
            data.AltitudeHighestPoint = 0;
            data.AltitudeLowestPoint = 0;
            data.AltitudeSumDown = 0;
            data.AltitudeSumUp = 0;
            //CapacityCeremony = null,
            //CategoryCodes = null,
            data.ContactInfos = myevent.ContactInfos[language];
            data.DateBegin = myevent.DateBegin;
            data.DateEnd = myevent.DateEnd;
            data.Detail = myevent.Detail[language];
            data.Difficulty = "";
            //DishRates = null,
            data.DistanceDuration = 0;
            data.DistanceLength = 0;
            data.EventDate = myevent.EventDate;
            data.Exposition = null;
            //Facilities = null,
            data.FeetClimb = false;
            data.GpsInfo = Enumerable.Repeat<GpsInfo>(new GpsInfo() { Altitude = myevent.Altitude, Latitude = myevent.Latitude, Longitude = myevent.Longitude, Gpstype = "position" }, 1).ToList();
            data.GpsTrack = null;
            data.HasFreeEntrance = false;
            data.HasRentals = false;
            data.Highlight = myevent.SmgTags != null ? myevent.SmgTags.Contains("TopEvent") ? true : false : false;
            //data.ImageGallery = myevent.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc[language], ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle[language], ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList();
            data.ImageGallery = myevent.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc[language] : null, ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle[language] : null, ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList();
            data.IsOpen = false;
            data.IsPrepared = false;
            data.IsWithLigth = false;
            data.LiftAvailable = false;
            data.LocationInfo = new LocationInfoLocalized()
            {
                DistrictInfo = new DistrictInfoLocalized() { Id = myevent.LocationInfo?.DistrictInfo?.Id, Name = myevent.LocationInfo?.DistrictInfo?.Name?[language] },
                MunicipalityInfo = new MunicipalityInfoLocalized() { Id = myevent.LocationInfo?.MunicipalityInfo?.Id, Name = myevent.LocationInfo?.MunicipalityInfo?.Name?[language] },
                TvInfo = new TvInfoLocalized() { Id = myevent.LocationInfo?.TvInfo?.Id, Name = myevent.LocationInfo?.TvInfo?.Name?[language] },
                RegionInfo = new RegionInfoLocalized() { Id = myevent.LocationInfo?.RegionInfo?.Id, Name = myevent.LocationInfo?.RegionInfo?.Name?[language] }
            };
            data.MainType = "event";
            data.MaxSeatingCapacity = 0;
            data.Novelty = "";
            data.OperationSchedule = null;
            data.OrganizerInfos = myevent.OrganizerInfos[language];
            data.PayMet = myevent.PayMet;
            data.PoiType = "";
            //PoiProperty = myevent.TopicRIDs.Select(x => new PoiProperty(){ Name = "TopicRID", Value = x }).ToList(),
            data.PoiProperty = null;
            data.PoiServices = null; //myevent.TopicRIDs,
            data.Ranc = myevent.Ranc;
            data.Ratings = null;
            data.RunToValley = false;
            data.SignOn = myevent.SignOn;
            data.SmgTags = myevent.SmgTags;
            data.SubType = language != "de" ? language != "it" ? "Event" : "Evento" : "Veranstaltung";
            data.Ticket = myevent.Ticket;
            //TopicRIDs = myevent.TopicRIDs,
            data.Type = language != "de" ? language != "it" ? "Event" : "Evento" : "Veranstaltung";

            return data;
        }

        public static MobileDetail TransformODHActivityPoiToMobileDetail(ODHActivityPoi activity, string language)
        {
            MobileDetail data = new MobileDetail();
            data.Id = activity.Id;
            //AdditionalPoiInfos = activity.AdditionalPoiInfos[language],
            data.AltitudeDifference = activity.AltitudeDifference;
            data.AltitudeHighestPoint = activity.AltitudeHighestPoint;
            data.AltitudeLowestPoint = activity.AltitudeLowestPoint;
            data.AltitudeSumDown = activity.AltitudeSumDown;
            data.AltitudeSumUp = activity.AltitudeSumUp;
            //CapacityCeremony = null,
            //CategoryCodes = null,                                             
            data.ContactInfos = activity.ContactInfos[language];
            data.DateBegin = null;
            data.DateEnd = null;
            data.Detail = activity.Detail[language];
            data.Difficulty = activity.Difficulty;
            //DishRates = null,                                             
            data.DistanceDuration = activity.DistanceDuration;
            data.DistanceLength = activity.DistanceLength;
            data.EventDate = null;
            data.Exposition = activity.Exposition;
            //Facilities = null,
            data.FeetClimb = activity.FeetClimb;
            data.GpsInfo = activity.GpsInfo;
            data.GpsTrack = activity.GpsTrack;
            data.HasFreeEntrance = activity.HasFreeEntrance;
            data.HasRentals = activity.HasRentals;
            data.Highlight = activity.Highlight;
            //data.ImageGallery = activity.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc[language], ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle[language], ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList();
            data.ImageGallery = activity.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc[language] : null, ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle[language] : null, ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList();
            data.IsOpen = activity.IsOpen;
            data.IsPrepared = activity.IsPrepared;
            data.IsWithLigth = activity.IsWithLigth;
            data.LiftAvailable = activity.LiftAvailable;
            data.LocationInfo = new LocationInfoLocalized()
            {
                DistrictInfo = new DistrictInfoLocalized() { Id = activity.LocationInfo?.DistrictInfo?.Id, Name = activity.LocationInfo?.DistrictInfo?.Name?[language] },
                MunicipalityInfo = new MunicipalityInfoLocalized() { Id = activity.LocationInfo?.MunicipalityInfo?.Id, Name = activity.LocationInfo?.MunicipalityInfo?.Name?[language] },
                TvInfo = new TvInfoLocalized() { Id = activity.LocationInfo?.TvInfo?.Id, Name = activity.LocationInfo?.TvInfo?.Name?[language] },
                RegionInfo = new RegionInfoLocalized() { Id = activity.LocationInfo?.RegionInfo?.Id, Name = activity.LocationInfo?.RegionInfo?.Name?[language] }
            };
            data.MainType = "smgpoi";
            data.MaxSeatingCapacity = 0;
            data.Novelty = activity.AdditionalPoiInfos[language].Novelty;
            data.OperationSchedule = activity.OperationSchedule;
            data.OrganizerInfos = null;
            data.PayMet = "";
            data.PoiType = activity.AdditionalPoiInfos[language].PoiType;
            //PoiProperty = activity.SyncSourceInterface == "GastronomicData" ? activity.CategoryCodes.Select(x => new PoiProperty() { Name = x.Id, Value = x.Shortname }).ToList() : activity.PoiProperty[language],
            data.PoiProperty = activity.PoiProperty != null ? activity.PoiProperty.Count > 0 ? activity.PoiProperty[language]?.Where(e => e.Name != null).ToDictionary(e => e.Name ?? "", v => v.Value ?? "") : null : null;
            data.PoiServices = activity.SyncSourceInterface == "GastronomicData" ? activity.Facilities?.Where(x => x.Id != null).Select(x => x.Id ?? "").ToList() : activity.PoiServices;
            data.Ranc = 0;
            data.Ratings = activity.Ratings;
            data.RunToValley = activity.RunToValley;
            data.SignOn = "";
            data.SmgTags = activity.SmgTags;
            data.SubType = activity.AdditionalPoiInfos[language].SubType;
            data.Ticket = "";
            //TopicRIDs = null, 
            data.Type = activity.AdditionalPoiInfos[language].MainType;

            return data;
        }

        public static MobileHtmlLocalized TransformToHtmlLocalized(MobileHtml data, string language)
        {
            MobileHtmlLocalized mobilehtml = new MobileHtmlLocalized();

            mobilehtml.Id = data.Id;
            mobilehtml.HtmlText = data.HtmlText[language];

            return mobilehtml;
        }


        #endregion

        #region Transformer Helpers



        #endregion
    }
}
