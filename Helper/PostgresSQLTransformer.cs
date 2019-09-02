using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helper
{
    public class PostgresSQLTransformer
    {
        #region Transformers

        public static AccommodationLocalized TransformToAccommodationLocalized(
            Accommodation acco, string language)
        {
            AccommodationLocalized data = new AccommodationLocalized
            {
                AccoBookingChannel = acco.AccoBookingChannel,
                AccoCategoryId = acco.AccoCategoryId,
                AccoDetail =
                    acco.AccoDetail != null ?
                        acco.AccoDetail.ContainsKey(language) ?
                            acco.AccoDetail[language] :
                            null :
                        null,
                AccoTypeId = acco.AccoTypeId,
                Altitude = acco.Altitude,
                AltitudeUnitofMeasure = acco.AltitudeUnitofMeasure,
                BadgeIds = acco.BadgeIds,
                Beds = acco.Beds,
                BoardIds = acco.BoardIds,
                //Features = acco.Features,
                FirstImport = acco.FirstImport,
                GastronomyId = acco.GastronomyId,
                Gpstype = acco.Gpstype,
                HgvId = acco.HgvId,
                HasApartment = acco.HasApartment,
                HasRoom = acco.HasRoom,
                Id = acco.Id,
                IsBookable = acco.IsBookable,
                DistrictId = acco.DistrictId,
                LastChange = acco.LastChange,
                Latitude = acco.Latitude,
                Longitude = acco.Longitude
            };

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
                    distinfolocalized.Name =
                        acco.LocationInfo?.DistrictInfo?.Name?.ContainsKey(language) ?? false ?
                            acco.LocationInfo?.DistrictInfo?.Name?[language] :
                            "";
                }
            }

            var muninfolocalized = new MunicipalityInfoLocalized() { };
            if (acco.LocationInfo != null)
            {
                if (acco.LocationInfo.MunicipalityInfo != null)
                {
                    muninfolocalized.Id = acco.LocationInfo.MunicipalityInfo.Id;
                    muninfolocalized.Name =
                        acco.LocationInfo?.MunicipalityInfo?.Name?.ContainsKey(language) ?? false ?
                            acco.LocationInfo?.MunicipalityInfo?.Name?[language] :
                            "";
                }
            }

            var reginfolocalized = new RegionInfoLocalized() { };
            if (acco.LocationInfo != null)
            {
                if (acco.LocationInfo.RegionInfo != null)
                {
                    reginfolocalized.Id = acco.LocationInfo.RegionInfo.Id;
                    reginfolocalized.Name =
                        acco.LocationInfo?.RegionInfo?.Name?.ContainsKey(language) ?? false ?
                            acco.LocationInfo?.RegionInfo?.Name?[language] :
                            "";
                }
            }

            var tvinfolocalized = new TvInfoLocalized() { };
            if (acco.LocationInfo != null)
            {
                if (acco.LocationInfo.TvInfo != null)
                {
                    tvinfolocalized.Id = acco.LocationInfo.TvInfo.Id;
                    tvinfolocalized.Name =
                        acco.LocationInfo?.TvInfo?.Name?.ContainsKey(language) ?? false ?
                            acco.LocationInfo?.TvInfo?.Name?[language] :
                            "";
                }
            }

            data.LocationInfo = new LocationInfoLocalized()
            {
                DistrictInfo = distinfolocalized,
                MunicipalityInfo = muninfolocalized,
                TvInfo = tvinfolocalized,
                RegionInfo = reginfolocalized
            };

            data.ImageGallery =
                    acco.ImageGallery?.Select(x =>
                        new ImageGalleryLocalized()
                        {
                            Height = x.Height,
                            ImageDesc =
                                x.ImageDesc.Count > 0 ?
                                    x.ImageDesc.ContainsKey(language) ?
                                        x.ImageDesc[language] :
                                        "" :
                                    "",
                            ImageName = x.ImageName,
                            ImageSource = x.ImageSource,
                            ImageTitle =
                                x.ImageTitle.Count > 0 ?
                                    x.ImageTitle.ContainsKey(language) ?
                                        x.ImageTitle[language] :
                                        "" :
                                    "",
                            ImageUrl = x.ImageUrl,
                            IsInGallery = x.IsInGallery,
                            ListPosition = x.ListPosition,
                            ValidFrom = x.ValidFrom,
                            ValidTo = x.ValidTo,
                            Width = x.Width,
                            CopyRight = x.CopyRight
                        })?.ToList();
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

        public static AccoListObject TransformToAccommodationListObject(
            Accommodation acco, string language)
        {
            return new AccoListObject
            {
                Id = acco.Id,
                Name =
                    acco.AccoDetail?.ContainsKey(language) ?? false ?
                        acco.AccoDetail?[language].Name :
                        "",
                Type = acco.AccoTypeId,
                Category = acco.AccoCategoryId,
                District =
                    acco.LocationInfo?.DistrictInfo != null ?
                        acco.LocationInfo?.DistrictInfo?.Name?[language] :
                        null,
                Municipality =
                    acco.LocationInfo?.MunicipalityInfo != null ?
                        acco.LocationInfo?.MunicipalityInfo?.Name?[language] :
                        null,
                Tourismverein =
                    acco.LocationInfo?.TvInfo != null ?
                        acco.LocationInfo?.TvInfo?.Name?[language] :
                        null,
                Region =
                    acco.LocationInfo?.RegionInfo != null ?
                        acco.LocationInfo?.RegionInfo?.Name?[language] :
                        null,
                TrustYouID = acco.TrustYouID,
                TrustYouResults = acco.TrustYouResults,
                TrustYouScore = acco.TrustYouScore,
                SuedtirolinfoLink =
                    $"https://www.suedtirol.info/{language}/tripmapping/acco/{acco.Id?.ToUpper()}",
                ImageGallery =
                    acco.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ?
                        acco.ImageGallery.Where(x => x.ListPosition == 0).ToList() :
                        null
            };
        }

        public static GBLTSActivityPoiLocalized TransformToGBLTSActivityPoiLocalized(GBLTSPoi poibaseinfo, string language)
        {
            GBLTSActivityPoiLocalized data = new GBLTSActivityPoiLocalized
            {
                Id = poibaseinfo.Id,
                LastChange = poibaseinfo.LastChange,
                FirstImport = poibaseinfo.FirstImport,
                Active = poibaseinfo.Active,
                AdditionalPoiInfos =
                    poibaseinfo.AdditionalPoiInfos != null ?
                        poibaseinfo.AdditionalPoiInfos.ContainsKey(language) ?
                            poibaseinfo.AdditionalPoiInfos[language] :
                            null :
                        null,
                AltitudeDifference = poibaseinfo.AltitudeDifference,
                AltitudeHighestPoint = poibaseinfo.AltitudeHighestPoint,
                AltitudeLowestPoint = poibaseinfo.AltitudeLowestPoint,
                AltitudeSumDown = poibaseinfo.AltitudeSumDown,
                AltitudeSumUp = poibaseinfo.AltitudeSumUp,
                AreaId = poibaseinfo.AreaId,
                ContactInfos =
                    poibaseinfo.ContactInfos != null ?
                        poibaseinfo.ContactInfos.ContainsKey(language) ?
                            poibaseinfo.ContactInfos[language] :
                            null :
                        null,
                Detail =
                    poibaseinfo.Detail != null ?
                        poibaseinfo.Detail.ContainsKey(language) ?
                            poibaseinfo.Detail[language] :
                            null :
                        null,
                Difficulty = poibaseinfo.Difficulty,
                DistanceDuration = poibaseinfo.DistanceDuration,
                DistanceLength = poibaseinfo.DistanceLength,
                Exposition = poibaseinfo.Exposition,
                FeetClimb = poibaseinfo.FeetClimb,
                GpsInfo = poibaseinfo.GpsInfo,
                GpsTrack = poibaseinfo.GpsTrack,
                HasFreeEntrance = poibaseinfo.HasFreeEntrance,
                HasRentals = poibaseinfo.HasRentals,
                Highlight = poibaseinfo.Highlight,
                IsOpen = poibaseinfo.IsOpen,
                IsPrepared = poibaseinfo.IsPrepared,
                IsWithLigth = poibaseinfo.IsWithLigth,
                LiftAvailable = poibaseinfo.LiftAvailable,
                OperationSchedule = poibaseinfo.OperationSchedule,
                Ratings = poibaseinfo.Ratings,
                RunToValley = poibaseinfo.RunToValley,
                Shortname = poibaseinfo.Shortname,
                SmgActive = poibaseinfo.SmgActive,
                SmgId = poibaseinfo.SmgId,
                SubType = poibaseinfo.SubType,
                TourismorganizationId = poibaseinfo.TourismorganizationId,
                Type = poibaseinfo.Type,
                SmgTags = poibaseinfo.SmgTags
            };

            var distinfolocalized = new DistrictInfoLocalized() { };
            if (poibaseinfo.LocationInfo != null)
            {
                if (poibaseinfo.LocationInfo.DistrictInfo != null)
                {
                    distinfolocalized.Id = poibaseinfo.LocationInfo.DistrictInfo.Id;
                    distinfolocalized.Name =
                        poibaseinfo.LocationInfo?.DistrictInfo?.Name?.ContainsKey(language) ?? false ?
                            poibaseinfo.LocationInfo?.DistrictInfo?.Name?[language] :
                            "";
                }
            }

            var muninfolocalized = new MunicipalityInfoLocalized() { };
            if (poibaseinfo.LocationInfo != null)
            {
                if (poibaseinfo.LocationInfo.MunicipalityInfo != null)
                {
                    muninfolocalized.Id = poibaseinfo.LocationInfo.MunicipalityInfo.Id;
                    muninfolocalized.Name =
                        poibaseinfo.LocationInfo?.MunicipalityInfo?.Name?.ContainsKey(language) ?? false ?
                            poibaseinfo.LocationInfo?.MunicipalityInfo?.Name?[language] :
                            "";
                }
            }

            var reginfolocalized = new RegionInfoLocalized() { };
            if (poibaseinfo.LocationInfo != null)
            {
                if (poibaseinfo.LocationInfo.RegionInfo != null)
                {
                    reginfolocalized.Id = poibaseinfo.LocationInfo.RegionInfo.Id;
                    reginfolocalized.Name =
                        poibaseinfo.LocationInfo?.RegionInfo?.Name?.ContainsKey(language) ?? false ?
                            poibaseinfo.LocationInfo?.RegionInfo?.Name?[language] :
                            "";
                }
            }

            var tvinfolocalized = new TvInfoLocalized() { };
            if (poibaseinfo.LocationInfo != null)
            {
                if (poibaseinfo.LocationInfo.TvInfo != null)
                {
                    tvinfolocalized.Id = poibaseinfo.LocationInfo.TvInfo.Id;
                    tvinfolocalized.Name =
                        poibaseinfo.LocationInfo?.TvInfo?.Name?.ContainsKey(language) ?? false ?
                            poibaseinfo.LocationInfo?.TvInfo?.Name?[language] :
                            "";
                }
            }

            data.LocationInfo = new LocationInfoLocalized()
            {
                DistrictInfo = distinfolocalized,
                MunicipalityInfo = muninfolocalized,
                TvInfo = tvinfolocalized,
                RegionInfo = reginfolocalized
            };
            data.ImageGallery =
                poibaseinfo.ImageGallery?.Select(x => new ImageGalleryLocalized() {
                    Height = x.Height,
                    ImageDesc =
                        x.ImageDesc.Count > 0 ?
                            x.ImageDesc.ContainsKey(language) ?
                                x.ImageDesc[language] :
                                "" :
                            "",
                    ImageName = x.ImageName,
                    ImageSource = x.ImageSource,
                    ImageTitle =
                        x.ImageTitle.Count > 0 ?
                            x.ImageTitle.ContainsKey(language) ?
                                x.ImageTitle[language] :
                                "" :
                            "",
                    ImageUrl = x.ImageUrl,
                    IsInGallery = x.IsInGallery,
                    ListPosition = x.ListPosition,
                    ValidFrom = x.ValidFrom,
                    ValidTo = x.ValidTo,
                    Width = x.Width,
                    CopyRight = x.CopyRight
                })?.ToList();


            List<LTSTagsLocalized> ltstagslocalized = new List<LTSTagsLocalized>();

            if (poibaseinfo.LTSTags != null)
            {
                foreach (var ltstag in poibaseinfo.LTSTags)
                {
                    ltstagslocalized.Add(new LTSTagsLocalized() {
                        Id = ltstag.Id,
                        Level = ltstag.Level,
                        TagName =
                            ltstag.TagName.ContainsKey(language) ?
                                ltstag.TagName[language] :
                                ""
                    });
                }
            }

            data.LTSTags = ltstagslocalized;
            data.GpsPoints = poibaseinfo.GpsPoints;

            return data;
        }

        public static GastronomyLocalized TransformToGastronomyLocalized(
            Gastronomy pgdata, string language)
        {
            GastronomyLocalized data = new GastronomyLocalized
            {
                Id = pgdata.Id,
                AccommodationId = pgdata.AccommodationId,
                Active = pgdata.Active,
                Altitude = pgdata.Altitude,
                AltitudeUnitofMeasure = pgdata.AltitudeUnitofMeasure,
                CapacityCeremony = pgdata.CapacityCeremony,
                CategoryCodes = pgdata.CategoryCodes,
                ContactInfos =
                    pgdata.ContactInfos != null ?
                        pgdata.ContactInfos.ContainsKey(language) ?
                            pgdata.ContactInfos[language] :
                            null :
                        null,
                Detail =
                    pgdata.Detail != null ?
                        pgdata.Detail.ContainsKey(language) ?
                            pgdata.Detail[language] :
                            null :
                        null,
                DishRates = pgdata.DishRates,
                DistrictId = pgdata.DistrictId,
                Facilities = pgdata.Facilities,
                FirstImport = pgdata.FirstImport,
                Gpstype = pgdata.Gpstype,
                ImageGallery =
                    pgdata.ImageGallery?.Select(x => new ImageGalleryLocalized() {
                        Height = x.Height,
                        ImageDesc =
                            x.ImageDesc.Count > 0 ?
                                x.ImageDesc.ContainsKey(language) ?
                                    x.ImageDesc[language] :
                                    "" :
                                "",
                        ImageName = x.ImageName,
                        ImageSource = x.ImageSource,
                        ImageTitle =
                            x.ImageTitle.Count > 0 ?
                                x.ImageTitle.ContainsKey(language) ?
                                    x.ImageTitle[language] :
                                    "" :
                                "",
                        ImageUrl = x.ImageUrl,
                        IsInGallery = x.IsInGallery,
                        ListPosition = x.ListPosition,
                        ValidFrom = x.ValidFrom,
                        ValidTo = x.ValidTo,
                        Width = x.Width,
                        CopyRight = x.CopyRight
                    })?.ToList(),
                LastChange = pgdata.LastChange,
                Latitude = pgdata.Latitude,
                Longitude = pgdata.Longitude,
                MarketinggroupId = pgdata.MarketinggroupId,
                MaxSeatingCapacity = pgdata.MaxSeatingCapacity,
                OperationSchedule = pgdata.OperationSchedule,
                Shortname = pgdata.Shortname,
                SmgActive = pgdata.SmgActive,
                SmgTags = pgdata.SmgTags,
                Type = pgdata.Type
            };

            var distinfolocalized = new DistrictInfoLocalized() { };
            if (pgdata.LocationInfo != null)
            {
                if (pgdata.LocationInfo.DistrictInfo != null)
                {
                    distinfolocalized.Id = pgdata.LocationInfo.DistrictInfo.Id;
                    distinfolocalized.Name =
                        pgdata.LocationInfo?.DistrictInfo?.Name?.ContainsKey(language) ?? false ?
                            pgdata.LocationInfo?.DistrictInfo?.Name?[language] :
                            "";
                }
            }

            var muninfolocalized = new MunicipalityInfoLocalized() { };
            if (pgdata.LocationInfo != null)
            {
                if (pgdata.LocationInfo.MunicipalityInfo != null)
                {
                    muninfolocalized.Id = pgdata.LocationInfo.MunicipalityInfo.Id;
                    muninfolocalized.Name =
                        pgdata.LocationInfo.MunicipalityInfo?.Name?.ContainsKey(language) ?? false ?
                            pgdata.LocationInfo?.MunicipalityInfo?.Name?[language] :
                            "";
                }
            }

            var reginfolocalized = new RegionInfoLocalized() { };
            if (pgdata.LocationInfo != null)
            {
                if (pgdata.LocationInfo.RegionInfo != null)
                {
                    reginfolocalized.Id = pgdata.LocationInfo.RegionInfo.Id;
                    reginfolocalized.Name =
                        pgdata.LocationInfo?.RegionInfo?.Name?.ContainsKey(language) ?? false ?
                            pgdata.LocationInfo?.RegionInfo?.Name?[language] :
                            "";
                }
            }

            var tvinfolocalized = new TvInfoLocalized() { };
            if (pgdata.LocationInfo != null)
            {
                if (pgdata.LocationInfo.TvInfo != null)
                {
                    tvinfolocalized.Id = pgdata.LocationInfo.TvInfo.Id;
                    tvinfolocalized.Name =
                        pgdata.LocationInfo?.TvInfo?.Name?.ContainsKey(language) ?? false ?
                            pgdata.LocationInfo?.TvInfo?.Name?[language] :
                            "";
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
            EventLocalized data = new EventLocalized
            {
                Id = psdata.Id,
                Active = psdata.Active,
                Altitude = psdata.Altitude,
                AltitudeUnitofMeasure = psdata.AltitudeUnitofMeasure,
                ContactInfos =
                    psdata.ContactInfos != null ?
                        psdata.ContactInfos.ContainsKey(language) ?
                            psdata.ContactInfos[language] :
                            null :
                        null,
                DateBegin = psdata.DateBegin,
                DateEnd = psdata.DateEnd,
                Detail =
                    psdata.Detail != null ?
                        psdata.Detail.ContainsKey(language) ?
                            psdata.Detail[language] :
                            null :
                        null,
                DistrictId = psdata.DistrictId,
                DistrictIds = psdata.DistrictIds,
                EventAdditionalInfos =
                    psdata.EventAdditionalInfos != null ?
                        psdata.EventAdditionalInfos.ContainsKey(language) ?
                            psdata.EventAdditionalInfos[language] :
                            null :
                        null,
                EventDate = psdata.EventDate,
                EventPrice =
                psdata.EventPrice != null ?
                    psdata.EventPrice.ContainsKey(language) ?
                        psdata.EventPrice[language] :
                        null :
                    null,
                EventPublisher = psdata.EventPublisher,
                Gpstype = psdata.Gpstype,
                ImageGallery =
                    psdata.ImageGallery?.Select(x => new ImageGalleryLocalized()
                    {
                        Height = x.Height,
                        ImageDesc =
                            x.ImageDesc.Count > 0 ?
                                x.ImageDesc.ContainsKey(language) ?
                                    x.ImageDesc[language] :
                                    "" :
                                "",
                        ImageName = x.ImageName,
                        ImageSource = x.ImageSource,
                        ImageTitle =
                            x.ImageTitle.Count > 0 ?
                                x.ImageTitle.ContainsKey(language) ?
                                    x.ImageTitle[language] :
                                    "" :
                                "",
                        ImageUrl = x.ImageUrl,
                        IsInGallery = x.IsInGallery,
                        ListPosition = x.ListPosition,
                        ValidFrom = x.ValidFrom,
                        ValidTo = x.ValidTo,
                        Width = x.Width,
                        CopyRight = x.CopyRight
                    })?.ToList(),
                Latitude = psdata.Latitude,
                Longitude = psdata.Longitude,
                OrganizerInfos =
                    psdata.OrganizerInfos != null ?
                        psdata.OrganizerInfos.ContainsKey(language) ?
                            psdata.OrganizerInfos[language] :
                            null :
                        null,
                OrgRID = psdata.OrgRID,
                PayMet = psdata.PayMet,
                Ranc = psdata.Ranc,
                Shortname = psdata.Shortname,
                SignOn = psdata.SignOn,
                SmgActive = psdata.SmgActive,
                SmgTags = psdata.SmgTags,
                Ticket = psdata.Ticket,
                TopicRIDs = psdata.TopicRIDs,
                Type = psdata.Type
            };

            var distinfolocalized = new DistrictInfoLocalized() { };
            if (psdata.LocationInfo != null)
            {
                if (psdata.LocationInfo.DistrictInfo != null)
                {
                    distinfolocalized.Id = psdata.LocationInfo.DistrictInfo.Id;
                    distinfolocalized.Name =
                        psdata.LocationInfo?.DistrictInfo?.Name?.ContainsKey(language) ?? false ?
                            psdata.LocationInfo?.DistrictInfo?.Name?[language] :
                            "";
                }
            }

            var muninfolocalized = new MunicipalityInfoLocalized() { };
            if (psdata.LocationInfo != null)
            {
                if (psdata.LocationInfo.MunicipalityInfo != null)
                {
                    muninfolocalized.Id = psdata.LocationInfo.MunicipalityInfo.Id;
                    muninfolocalized.Name =
                        psdata.LocationInfo?.MunicipalityInfo?.Name?.ContainsKey(language) ?? false ?
                            psdata.LocationInfo?.MunicipalityInfo?.Name?[language] :
                            "";
                }
            }

            var reginfolocalized = new RegionInfoLocalized() { };
            if (psdata.LocationInfo != null)
            {
                if (psdata.LocationInfo.RegionInfo != null)
                {
                    reginfolocalized.Id = psdata.LocationInfo.RegionInfo.Id;
                    reginfolocalized.Name =
                        psdata.LocationInfo.RegionInfo?.Name?.ContainsKey(language) ?? false ?
                            psdata.LocationInfo?.RegionInfo?.Name?[language] :
                            "";
                }
            }

            var tvinfolocalized = new TvInfoLocalized() { };
            if (psdata.LocationInfo != null)
            {
                if (psdata.LocationInfo.TvInfo != null)
                {
                    tvinfolocalized.Id = psdata.LocationInfo.TvInfo.Id;
                    tvinfolocalized.Name =
                        psdata.LocationInfo?.TvInfo?.Name?.ContainsKey(language) ?? false ?
                            psdata.LocationInfo?.TvInfo?.Name?[language] :
                            "";
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

        public static ArticleBaseInfosLocalized TransformToArticleBaseInfosLocalized(
            Article poibaseinfo, string language)
        {
            ArticleBaseInfosLocalized data = new ArticleBaseInfosLocalized
            {
                Id = poibaseinfo.Id,
                LastChange = poibaseinfo.LastChange,
                FirstImport = poibaseinfo.FirstImport,
                Active = poibaseinfo.Active,
                AdditionalArticleInfos =
                    poibaseinfo.AdditionalArticleInfos != null ?
                        poibaseinfo.AdditionalArticleInfos.ContainsKey(language) ?
                            poibaseinfo.AdditionalArticleInfos[language] :
                            null :
                        null,
                ContactInfos =
                    poibaseinfo.ContactInfos != null ?
                        poibaseinfo.ContactInfos.ContainsKey(language) ?
                            poibaseinfo.ContactInfos[language] :
                            null :
                            null,
                Detail =
                    poibaseinfo.Detail != null ?
                        poibaseinfo.Detail.ContainsKey(language) ?
                            poibaseinfo.Detail[language] :
                            null :
                        null,
                GpsInfo = poibaseinfo.GpsInfo,
                GpsTrack = poibaseinfo.GpsTrack,
                Highlight = poibaseinfo.Highlight,
                OperationSchedule = poibaseinfo.OperationSchedule,
                Shortname = poibaseinfo.Shortname,
                SmgActive = poibaseinfo.SmgActive,
                SubType = poibaseinfo.SubType,
                Type = poibaseinfo.Type,
                SmgTags = poibaseinfo.SmgTags,

                ImageGallery =
                    poibaseinfo.ImageGallery?.Select(x => new ImageGalleryLocalized() {
                        Height = x.Height,
                        ImageDesc =
                            x.ImageDesc.Count > 0 ?
                                x.ImageDesc.ContainsKey(language) ?
                                    x.ImageDesc[language] :
                                    "" :
                                "",
                        ImageName = x.ImageName,
                        ImageSource = x.ImageSource,
                        ImageTitle =
                            x.ImageTitle.Count > 0 ?
                                x.ImageTitle.ContainsKey(language) ?
                                    x.ImageTitle[language] :
                                    "" :
                                "",
                        ImageUrl = x.ImageUrl,
                        IsInGallery = x.IsInGallery,
                        ListPosition = x.ListPosition,
                        ValidFrom = x.ValidFrom,
                        ValidTo = x.ValidTo,
                        Width = x.Width,
                        CopyRight = x.CopyRight
                    })?.ToList()
            };

            //MarketingGroupIds = acco.MarketingGroupIds,

            return data;
        }

        public static ODHActivityPoiLocalized TransformToODHActivityPoiLocalized(
            ODHActivityPoi poibaseinfo, string language)
        {
            ODHActivityPoiLocalized data = new ODHActivityPoiLocalized
            {
                Id = poibaseinfo.Id,
                LastChange = poibaseinfo.LastChange,
                FirstImport = poibaseinfo.FirstImport,
                Active = poibaseinfo.Active,
                AdditionalPoiInfos =
                    poibaseinfo.AdditionalPoiInfos != null ?
                        poibaseinfo.AdditionalPoiInfos.Count > 0 ?
                            poibaseinfo.AdditionalPoiInfos.ContainsKey(language) ?
                                poibaseinfo.AdditionalPoiInfos[language] :
                                null :
                            null :
                        null,
                ContactInfos =
                    poibaseinfo.ContactInfos != null ?
                        poibaseinfo.ContactInfos.ContainsKey(language) ?
                            poibaseinfo.ContactInfos[language] :
                            null :
                        null,
                Detail =
                    poibaseinfo.Detail != null ?
                        poibaseinfo.Detail.ContainsKey(language) ?
                            poibaseinfo.Detail[language] :
                            null :
                        null,
                GpsInfo = poibaseinfo.GpsInfo,
                GpsTrack = poibaseinfo.GpsTrack,
                GpsPoints = poibaseinfo.GpsPoints,
                Highlight = poibaseinfo.Highlight,
                OperationSchedule = poibaseinfo.OperationSchedule,
                Shortname = poibaseinfo.Shortname,
                SmgActive = poibaseinfo.SmgActive,
                SubType = poibaseinfo.SubType,
                Type = poibaseinfo.Type,
                SmgTags = poibaseinfo.SmgTags,
                ImageGallery =
                    poibaseinfo.ImageGallery?.Select(x => new ImageGalleryLocalized() {
                        Height = x.Height,
                        ImageDesc =
                            x.ImageDesc.Count > 0 ?
                                x.ImageDesc.ContainsKey(language) ?
                                    x.ImageDesc[language] :
                                    "" :
                                "",
                        ImageName = x.ImageName,
                        ImageSource = x.ImageSource,
                        ImageTitle =
                            x.ImageTitle.Count > 0 ?
                                x.ImageTitle.ContainsKey(language) ?
                                    x.ImageTitle[language] :
                                    "" :
                                "",
                        ImageUrl = x.ImageUrl,
                        IsInGallery = x.IsInGallery,
                        ListPosition = x.ListPosition,
                        ValidFrom = x.ValidFrom,
                        ValidTo = x.ValidTo,
                        Width = x.Width,
                        CopyRight = x.CopyRight
                    }).ToList(),
                AltitudeDifference = poibaseinfo.AltitudeDifference,
                AltitudeHighestPoint = poibaseinfo.AltitudeHighestPoint,
                AltitudeLowestPoint = poibaseinfo.AltitudeLowestPoint,
                AltitudeSumDown = poibaseinfo.AltitudeSumDown,
                AltitudeSumUp = poibaseinfo.AltitudeSumUp,
                AreaId = poibaseinfo.AreaId,
                Difficulty = poibaseinfo.Difficulty,
                DistanceDuration = poibaseinfo.DistanceDuration,
                DistanceLength = poibaseinfo.DistanceLength,
                Exposition = poibaseinfo.Exposition,
                FeetClimb = poibaseinfo.FeetClimb,
                HasFreeEntrance = poibaseinfo.HasFreeEntrance,
                HasRentals = poibaseinfo.HasRentals,
                IsOpen = poibaseinfo.IsOpen,
                IsPrepared = poibaseinfo.IsPrepared,
                IsWithLigth = poibaseinfo.IsWithLigth,
                LiftAvailable = poibaseinfo.LiftAvailable,
                BikeTransport = poibaseinfo.BikeTransport,
                PoiProperty =
                    poibaseinfo.PoiProperty != null ?
                        poibaseinfo.PoiProperty.ContainsKey(language) ?
                            poibaseinfo.PoiProperty[language] :
                            null :
                        null,
                PoiServices = poibaseinfo.PoiServices,
                PoiType = poibaseinfo.PoiType,
                Ratings = poibaseinfo.Ratings,
                RunToValley = poibaseinfo.RunToValley,
                SmgId = poibaseinfo.SmgId,
                TourismorganizationId = poibaseinfo.TourismorganizationId,
                AgeFrom = poibaseinfo.AgeFrom,
                AgeTo = poibaseinfo.AgeTo,
                SyncSourceInterface = poibaseinfo.SyncSourceInterface,
                SyncUpdateMode = poibaseinfo.SyncUpdateMode,
                Source = poibaseinfo.Source
            };
            //data.MaxSeatingCapacity = poibaseinfo.MaxSeatingCapacity weiter

            var distinfolocalized = new DistrictInfoLocalized() { };
            if (poibaseinfo.LocationInfo != null)
            {
                if (poibaseinfo.LocationInfo.DistrictInfo != null)
                {
                    distinfolocalized.Id = poibaseinfo.LocationInfo.DistrictInfo.Id;
                    distinfolocalized.Name =
                        poibaseinfo.LocationInfo?.DistrictInfo?.Name?.ContainsKey(language) ?? false ?
                            poibaseinfo.LocationInfo?.DistrictInfo?.Name?[language] :
                            "";
                }
            }

            var muninfolocalized = new MunicipalityInfoLocalized() { };
            if (poibaseinfo.LocationInfo != null)
            {
                if (poibaseinfo.LocationInfo.MunicipalityInfo != null)
                {
                    muninfolocalized.Id = poibaseinfo.LocationInfo.MunicipalityInfo.Id;
                    muninfolocalized.Name =
                        poibaseinfo.LocationInfo?.MunicipalityInfo?.Name?.ContainsKey(language) ?? false ?
                            poibaseinfo.LocationInfo?.MunicipalityInfo?.Name?[language] :
                            "";
                }
            }

            var reginfolocalized = new RegionInfoLocalized() { };
            if (poibaseinfo.LocationInfo != null)
            {
                if (poibaseinfo.LocationInfo.RegionInfo != null)
                {
                    reginfolocalized.Id = poibaseinfo.LocationInfo.RegionInfo.Id;
                    reginfolocalized.Name =
                        poibaseinfo.LocationInfo?.RegionInfo?.Name?.ContainsKey(language) ?? false ?
                            poibaseinfo.LocationInfo?.RegionInfo?.Name?[language] :
                            "";
                }
            }

            var tvinfolocalized = new TvInfoLocalized() { };
            if (poibaseinfo.LocationInfo != null)
            {
                if (poibaseinfo.LocationInfo.TvInfo != null)
                {
                    tvinfolocalized.Id = poibaseinfo.LocationInfo.TvInfo.Id;
                    tvinfolocalized.Name =
                        poibaseinfo.LocationInfo?.TvInfo?.Name?.ContainsKey(language) ?? false ?
                            poibaseinfo.LocationInfo?.TvInfo?.Name?[language] :
                            "";
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
            PackageLocalized data = new PackageLocalized
            {
                Active = package.Active,

                ChannelInfo = package.ChannelInfo,
                ChildrenMin = package.ChildrenMin,
                DaysArrival = package.DaysArrival,
                DaysArrivalMax = package.DaysArrivalMax,
                DaysArrivalMin = package.DaysArrivalMin,
                DaysDeparture = package.DaysDeparture,
                DaysDurMax = package.DaysDurMax,
                DaysDurMin = package.DaysDurMin,
                HotelHgvId = package.HotelHgvId,
                HotelId = package.HotelId,
                Id = package.Id,
                ImageGallery =
                    package.ImageGallery?.Select(x => new ImageGalleryLocalized()
                    {
                        Height = x.Height,
                        ImageDesc =
                            x.ImageDesc.Count > 0 ?
                                x.ImageDesc.ContainsKey(language) ?
                                    x.ImageDesc[language] :
                                    "" :
                                "",
                        ImageName = x.ImageName,
                        ImageSource = x.ImageSource,
                        ImageTitle =
                            x.ImageTitle.Count > 0 ?
                                x.ImageTitle.ContainsKey(language) ?
                                    x.ImageTitle[language] :
                                    "" :
                                "",
                        ImageUrl = x.ImageUrl,
                        IsInGallery = x.IsInGallery,
                        ListPosition = x.ListPosition,
                        ValidFrom = x.ValidFrom,
                        ValidTo = x.ValidTo,
                        Width = x.Width,
                        CopyRight = x.CopyRight
                    })?.ToList(),
                Inclusive =
                    package.Inclusive?.Select(x => new InclusiveLocalized()
                    {
                        ImageGallery =
                            x.Value.ImageGallery?.Select(y => new ImageGalleryLocalized()
                            {
                                Height = y.Height,
                                ImageDesc = y.ImageDesc.ContainsKey(language) ? y.ImageDesc[language] : "",
                                ImageName = y.ImageName,
                                ImageSource = y.ImageSource,
                                ImageTitle = y.ImageTitle.ContainsKey(language) ? y.ImageTitle[language] : "",
                                ImageUrl = y.ImageUrl,
                                IsInGallery = y.IsInGallery,
                                ListPosition = y.ListPosition,
                                ValidFrom = y.ValidFrom,
                                ValidTo = y.ValidTo,
                                Width = y.Width,
                                CopyRight = y.CopyRight
                            })?.ToList(),
                        PackageDetail =
                            x.Value.PackageDetail != null ?
                                x.Value.PackageDetail.ContainsKey(language) ?
                                    x.Value.PackageDetail[language] :
                                    null :
                                null,
                        PriceId = x.Value.PriceId,
                        PriceTyp = x.Value.PriceTyp
                    })?.ToList(),
                LocationInfo = new LocationInfoLocalized()
                {
                    DistrictInfo =
                        package.LocationInfo != null ?
                            package.LocationInfo.DistrictInfo != null ?
                                new DistrictInfoLocalized()
                                {
                                    Id = package.LocationInfo.DistrictInfo.Id,
                                    Name = package.LocationInfo?.DistrictInfo?.Name?[language]
                                } :
                                new DistrictInfoLocalized() :
                            new DistrictInfoLocalized(),
                    MunicipalityInfo =
                        package.LocationInfo != null ?
                            package.LocationInfo.MunicipalityInfo != null ?
                                new MunicipalityInfoLocalized()
                                {
                                    Id = package.LocationInfo.MunicipalityInfo.Id,
                                    Name = package.LocationInfo?.MunicipalityInfo?.Name?[language]
                                } :
                                new MunicipalityInfoLocalized() :
                            new MunicipalityInfoLocalized(),
                    TvInfo =
                        package.LocationInfo != null ?
                            package.LocationInfo.TvInfo != null ?
                                new TvInfoLocalized()
                                {
                                    Id = package.LocationInfo.TvInfo.Id,
                                    Name = package.LocationInfo?.TvInfo?.Name?[language]
                                } :
                                new TvInfoLocalized() :
                            new TvInfoLocalized(),
                    RegionInfo =
                        package.LocationInfo != null ?
                            package.LocationInfo.RegionInfo != null ?
                                new RegionInfoLocalized()
                                {
                                    Id = package.LocationInfo.RegionInfo.Id,
                                    Name = package.LocationInfo?.RegionInfo?.Name?[language]
                                } :
                                new RegionInfoLocalized() :
                            new RegionInfoLocalized()
                },
                OfferId = package.OfferId,
                Offertyp = package.Offertyp,
                PackageDetail =
                    package.PackageDetail != null ?
                        package.PackageDetail.ContainsKey(language) ?
                            package.PackageDetail[language] :
                            null :
                        null,
                Season = package.Season,
                Shortname = package.Shortname,
                SmgActive = package.SmgActive,
                SmgTags = package.SmgTags,
                Specialtyp = package.Specialtyp,
                ValidStart = package.ValidStart,
                ValidStop = package.ValidStop,
                Services = package.Services,
                PackageThemeDetail =
                    package.PackageThemeDetail?.Select(x => new PackageThemeLocalized()
                    {
                        ThemeId = x.ThemeId,
                        ThemeDetail = x.ThemeDetail.ContainsKey(language) ? x.ThemeDetail[language] : null
                    })?.ToList()
            };



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
            BaseInfosLocalized data = new BaseInfosLocalized
            {
                Id = baseinfo.Id,
                Active = baseinfo.Active,
                ContactInfos =
                    baseinfo.ContactInfos != null ?
                        baseinfo.ContactInfos.ContainsKey(language) ?
                            baseinfo.ContactInfos[language] :
                            null :
                        null,
                Detail =
                    baseinfo.Detail != null ?
                        baseinfo.Detail.ContainsKey(language) ?
                            baseinfo.Detail[language] :
                            null :
                        null,
                Gpstype = baseinfo.Gpstype,
                Latitude = baseinfo.Latitude,
                Longitude = baseinfo.Longitude,
                Altitude = baseinfo.Altitude,
                AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure,
                ImageGallery =
                    baseinfo.ImageGallery?.Select(x => new ImageGalleryLocalized()
                    {
                        Height = x.Height,
                        ImageDesc =
                            x.ImageDesc.Count > 0 ?
                                x.ImageDesc.ContainsKey(language) ?
                                    x.ImageDesc[language] :
                                    "" :
                                "",
                        ImageName = x.ImageName,
                        ImageSource = x.ImageSource,
                        ImageTitle =
                            x.ImageTitle.Count > 0 ?
                                x.ImageTitle.ContainsKey(language) ?
                                    x.ImageTitle[language] :
                                    "" :
                                "",
                        ImageUrl = x.ImageUrl,
                        IsInGallery = x.IsInGallery,
                        ListPosition = x.ListPosition,
                        ValidFrom = x.ValidFrom,
                        ValidTo = x.ValidTo,
                        Width = x.Width,
                        CopyRight = x.CopyRight
                    })?.ToList(),
                CustomId = baseinfo.CustomId,
                Shortname = baseinfo.Shortname,
                SmgActive = baseinfo.SmgActive,
                SmgTags = baseinfo.SmgTags
            };

            return data;
        }

        public static RegionLocalized TransformToRegionLocalized(Region baseinfo, string language)
        {
            RegionLocalized data = new RegionLocalized
            {
                Id = baseinfo.Id,
                Active = baseinfo.Active,
                ContactInfos =
                    baseinfo.ContactInfos != null ?
                        baseinfo.ContactInfos.ContainsKey(language) ?
                            baseinfo.ContactInfos[language] :
                            null :
                        null,
                Detail =
                    baseinfo.Detail != null ?
                        baseinfo.Detail.ContainsKey(language) ?
                            baseinfo.Detail[language] :
                            null :
                        null,
                Gpstype = baseinfo.Gpstype,
                Latitude = baseinfo.Latitude,
                Longitude = baseinfo.Longitude,
                Altitude = baseinfo.Altitude,
                AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure,
                ImageGallery =
                    baseinfo.ImageGallery?.Select(x => new ImageGalleryLocalized()
                    {
                        Height = x.Height,
                        ImageDesc =
                            x.ImageDesc.Count > 0 ?
                                x.ImageDesc.ContainsKey(language) ?
                                    x.ImageDesc[language] :
                                    "" :
                                "",
                        ImageName = x.ImageName,
                        ImageSource = x.ImageSource,
                        ImageTitle =
                            x.ImageTitle.Count > 0 ?
                                x.ImageTitle.ContainsKey(language) ?
                                    x.ImageTitle[language] :
                                    "" :
                                "",
                        ImageUrl = x.ImageUrl,
                        IsInGallery = x.IsInGallery,
                        ListPosition = x.ListPosition,
                        ValidFrom = x.ValidFrom,
                        ValidTo = x.ValidTo,
                        Width = x.Width,
                        CopyRight = x.CopyRight
                    })?.ToList(),
                CustomId = baseinfo.CustomId,
                Shortname = baseinfo.Shortname,
                SmgActive = baseinfo.SmgActive,
                SmgTags = baseinfo.SmgTags,
                DetailThemed =
                    baseinfo.DetailThemed != null ?
                        baseinfo.DetailThemed.ContainsKey(language) ?
                            baseinfo.DetailThemed[language] :
                                null :
                            null,
                GpsPolygon = baseinfo.GpsPolygon,
                Webcam =
                    baseinfo.Webcam?.Select(x => new WebcamLocalized()
                    {
                        GpsInfo = x.GpsInfo,
                        WebcamId = x.WebcamId,
                        Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "",
                        Webcamurl = x.Webcamurl
                    })?.ToList(),
                VisibleInSearch = baseinfo.VisibleInSearch,
                SkiareaIds = baseinfo.SkiareaIds
            };

            return data;
        }

        public static TourismvereinLocalized TransformToTourismvereinLocalized(
            Tourismverein baseinfo, string language)
        {
            TourismvereinLocalized data = new TourismvereinLocalized
            {
                Id = baseinfo.Id,
                Active = baseinfo.Active,
                ContactInfos =
                    baseinfo.ContactInfos != null ?
                        baseinfo.ContactInfos.ContainsKey(language) ?
                            baseinfo.ContactInfos[language] :
                            null :
                        null,
                Detail =
                    baseinfo.Detail != null ?
                        baseinfo.Detail.ContainsKey(language) ?
                            baseinfo.Detail[language] :
                            null :
                        null,
                Gpstype = baseinfo.Gpstype,
                Latitude = baseinfo.Latitude,
                Longitude = baseinfo.Longitude,
                Altitude = baseinfo.Altitude,
                AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure,
                ImageGallery =
                    baseinfo.ImageGallery?.Select(x => new ImageGalleryLocalized()
                    {
                        Height = x.Height,
                        ImageDesc =
                            x.ImageDesc.Count > 0 ?
                                x.ImageDesc.ContainsKey(language) ?
                                    x.ImageDesc[language] :
                                    "" :
                                "",
                        ImageName = x.ImageName,
                        ImageSource = x.ImageSource,
                        ImageTitle =
                            x.ImageTitle.Count > 0 ?
                                x.ImageTitle.ContainsKey(language) ?
                                    x.ImageTitle[language] :
                                    "" :
                                "",
                        ImageUrl = x.ImageUrl,
                        IsInGallery = x.IsInGallery,
                        ListPosition = x.ListPosition,
                        ValidFrom = x.ValidFrom,
                        ValidTo = x.ValidTo,
                        Width = x.Width,
                        CopyRight = x.CopyRight
                    })?.ToList(),
                CustomId = baseinfo.CustomId,
                Shortname = baseinfo.Shortname,
                SmgActive = baseinfo.SmgActive,
                SmgTags = baseinfo.SmgTags,
                GpsPolygon = baseinfo.GpsPolygon,
                Webcam =
                    baseinfo.Webcam?.Select(x => new WebcamLocalized()
                    {
                        GpsInfo = x.GpsInfo,
                        WebcamId = x.WebcamId,
                        Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "",
                        Webcamurl = x.Webcamurl
                    })?.ToList(),
                VisibleInSearch = baseinfo.VisibleInSearch,
                SkiareaIds = baseinfo.SkiareaIds,
                RegionId = baseinfo.RegionId
            };

            return data;
        }

        public static MunicipalityLocalized TransformToMunicipalityLocalized(
            Municipality baseinfo, string language)
        {
            MunicipalityLocalized data = new MunicipalityLocalized
            {
                Id = baseinfo.Id,
                Active = baseinfo.Active,
                ContactInfos =
                    baseinfo.ContactInfos != null ?
                        baseinfo.ContactInfos.ContainsKey(language) ?
                            baseinfo.ContactInfos[language] :
                            null :
                        null,
                Detail =
                    baseinfo.Detail != null ?
                        baseinfo.Detail.ContainsKey(language) ?
                            baseinfo.Detail[language] :
                            null :
                        null,
                Gpstype = baseinfo.Gpstype,
                Latitude = baseinfo.Latitude,
                Longitude = baseinfo.Longitude,
                Altitude = baseinfo.Altitude,
                AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure,
                ImageGallery =
                    baseinfo.ImageGallery?.Select(x => new ImageGalleryLocalized()
                    {
                        Height = x.Height,
                        ImageDesc =
                            x.ImageDesc.Count > 0 ?
                                x.ImageDesc.ContainsKey(language) ?
                                    x.ImageDesc[language] :
                                    "" :
                                "",
                        ImageName = x.ImageName,
                        ImageSource = x.ImageSource,
                        ImageTitle =
                            x.ImageTitle.Count > 0 ?
                                x.ImageTitle.ContainsKey(language) ?
                                    x.ImageTitle[language] :
                                    "" :
                                "",
                        ImageUrl = x.ImageUrl,
                        IsInGallery = x.IsInGallery,
                        ListPosition = x.ListPosition,
                        ValidFrom = x.ValidFrom,
                        ValidTo = x.ValidTo,
                        Width = x.Width,
                        CopyRight = x.CopyRight
                    })?.ToList(),
                CustomId = baseinfo.CustomId,
                Shortname = baseinfo.Shortname,
                SmgActive = baseinfo.SmgActive,
                SmgTags = baseinfo.SmgTags,
                GpsPolygon = baseinfo.GpsPolygon,
                Webcam =
                    baseinfo.Webcam?.Select(x => new WebcamLocalized()
                    {
                        GpsInfo = x.GpsInfo,
                        WebcamId = x.WebcamId,
                        Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "",
                        Webcamurl = x.Webcamurl
                    })?.ToList(),
                VisibleInSearch = baseinfo.VisibleInSearch,
                RegionId = baseinfo.RegionId,
                Plz = baseinfo.Plz,
                TourismvereinId = baseinfo.TourismvereinId,
                SiagId = baseinfo.SiagId,
                Inhabitants = baseinfo.Inhabitants,
                IstatNumber = baseinfo.IstatNumber
            };

            return data;
        }

        public static DistrictLocalized TransformToDistrictLocalized(District baseinfo, string language)
        {
            DistrictLocalized data = new DistrictLocalized
            {
                Id = baseinfo.Id,
                Active = baseinfo.Active,
                ContactInfos =
                    baseinfo.ContactInfos != null ?
                        baseinfo.ContactInfos.ContainsKey(language) ?
                            baseinfo.ContactInfos[language] :
                            null :
                        null,
                Detail =
                    baseinfo.Detail != null ?
                        baseinfo.Detail.ContainsKey(language) ?
                            baseinfo.Detail[language] :
                            null :
                        null,
                Gpstype = baseinfo.Gpstype,
                Latitude = baseinfo.Latitude,
                Longitude = baseinfo.Longitude,
                Altitude = baseinfo.Altitude,
                AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure,
                ImageGallery =
                    baseinfo.ImageGallery?.Select(x => new ImageGalleryLocalized()
                    {
                        Height = x.Height,
                        ImageDesc =
                            x.ImageDesc.Count > 0 ?
                                x.ImageDesc.ContainsKey(language) ?
                                    x.ImageDesc[language] :
                                    "" :
                                "",
                        ImageName = x.ImageName,
                        ImageSource = x.ImageSource,
                        ImageTitle =
                            x.ImageTitle.Count > 0 ?
                                x.ImageTitle.ContainsKey(language) ?
                                    x.ImageTitle[language] :
                                    "" :
                                "",
                        ImageUrl = x.ImageUrl,
                        IsInGallery = x.IsInGallery,
                        ListPosition = x.ListPosition,
                        ValidFrom = x.ValidFrom,
                        ValidTo = x.ValidTo,
                        Width = x.Width,
                        CopyRight = x.CopyRight
                    })?.ToList(),
                CustomId = baseinfo.CustomId,
                Shortname = baseinfo.Shortname,
                SmgActive = baseinfo.SmgActive,
                SmgTags = baseinfo.SmgTags,
                GpsPolygon = baseinfo.GpsPolygon,
                Webcam =
                    baseinfo.Webcam?.Select(x => new WebcamLocalized()
                    {
                        GpsInfo = x.GpsInfo,
                        WebcamId = x.WebcamId,
                        Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "",
                        Webcamurl = x.Webcamurl
                    })?.ToList(),
                VisibleInSearch = baseinfo.VisibleInSearch,
                RegionId = baseinfo.RegionId,
                IsComune = baseinfo.IsComune,
                TourismvereinId = baseinfo.TourismvereinId,
                SiagId = baseinfo.SiagId,
                MunicipalityId = baseinfo.MunicipalityId
            };

            return data;
        }

        public static MetaRegionLocalized TransformToMetaRegionLocalized(MetaRegion baseinfo, string language)
        {
            return new MetaRegionLocalized
            {
                Id = baseinfo.Id,
                Active = baseinfo.Active,
                ContactInfos =
                    baseinfo.ContactInfos != null ?
                        baseinfo.ContactInfos.ContainsKey(language) ?
                            baseinfo.ContactInfos[language] :
                            null :
                        null,
                Detail =
                    baseinfo.Detail != null ?
                        baseinfo.Detail.ContainsKey(language) ?
                            baseinfo.Detail[language] :
                            null :
                        null,
                Gpstype = baseinfo.Gpstype,
                Latitude = baseinfo.Latitude,
                Longitude = baseinfo.Longitude,
                Altitude = baseinfo.Altitude,
                AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure,
                ImageGallery =
                    baseinfo.ImageGallery?.Select(x => new ImageGalleryLocalized()
                    {
                        Height = x.Height,
                        ImageDesc =
                            x.ImageDesc.Count > 0 ?
                                x.ImageDesc.ContainsKey(language) ?
                                    x.ImageDesc[language] :
                                    "" :
                                "",
                        ImageName = x.ImageName,
                        ImageSource = x.ImageSource,
                        ImageTitle =
                            x.ImageTitle.Count > 0 ?
                                x.ImageTitle.ContainsKey(language) ?
                                    x.ImageTitle[language] :
                                    "" :
                                "",
                        ImageUrl = x.ImageUrl,
                        IsInGallery = x.IsInGallery,
                        ListPosition = x.ListPosition,
                        ValidFrom = x.ValidFrom,
                        ValidTo = x.ValidTo,
                        Width = x.Width,
                        CopyRight = x.CopyRight
                    })?.ToList(),
                CustomId = baseinfo.CustomId,
                Shortname = baseinfo.Shortname,
                SmgActive = baseinfo.SmgActive,
                SmgTags = baseinfo.SmgTags,
                GpsPolygon = baseinfo.GpsPolygon,
                Webcam =
                    baseinfo.Webcam?.Select(x => new WebcamLocalized()
                    {
                        GpsInfo = x.GpsInfo,
                        WebcamId = x.WebcamId,
                        Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "",
                        Webcamurl = x.Webcamurl
                    })?.ToList(),
                VisibleInSearch = baseinfo.VisibleInSearch,
                DistrictIds = baseinfo.DistrictIds,
                TourismvereinIds = baseinfo.TourismvereinIds,
                RegionIds = baseinfo.RegionIds,
                DetailThemed =
                    baseinfo.DetailThemed != null ?
                        baseinfo.DetailThemed.ContainsKey(language) ?
                            baseinfo.DetailThemed[language] :
                            null :
                        null
            };
        }

        public static ExperienceAreaLocalized TransformToExperienceAreaLocalized(
            ExperienceArea baseinfo, string language)
        {
            return new ExperienceAreaLocalized
            {
                Id = baseinfo.Id,
                Active = baseinfo.Active,
                ContactInfos =
                    baseinfo.ContactInfos != null ?
                        baseinfo.ContactInfos.ContainsKey(language) ?
                            baseinfo.ContactInfos[language] :
                            null :
                        null,
                Detail =
                    baseinfo.Detail != null ?
                        baseinfo.Detail.ContainsKey(language) ?
                            baseinfo.Detail[language] :
                            null :
                        null,
                Gpstype = baseinfo.Gpstype,
                Latitude = baseinfo.Latitude,
                Longitude = baseinfo.Longitude,
                Altitude = baseinfo.Altitude,
                AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure,
                ImageGallery =
                    baseinfo.ImageGallery?.Select(x => new ImageGalleryLocalized()
                    {
                        Height = x.Height,
                        ImageDesc =
                            x.ImageDesc.Count > 0 ?
                                x.ImageDesc.ContainsKey(language) ?
                                    x.ImageDesc[language] :
                                    "" :
                                "",
                        ImageName = x.ImageName,
                        ImageSource = x.ImageSource,
                        ImageTitle =
                            x.ImageTitle.Count > 0 ?
                                x.ImageTitle.ContainsKey(language) ?
                                    x.ImageTitle[language] :
                                    "" :
                                "",
                        ImageUrl = x.ImageUrl,
                        IsInGallery = x.IsInGallery,
                        ListPosition = x.ListPosition,
                        ValidFrom = x.ValidFrom,
                        ValidTo = x.ValidTo,
                        Width = x.Width,
                        CopyRight = x.CopyRight
                    }).ToList(),
                CustomId = baseinfo.CustomId,
                Shortname = baseinfo.Shortname,
                SmgActive = baseinfo.SmgActive,
                SmgTags = baseinfo.SmgTags,
                GpsPolygon = baseinfo.GpsPolygon,
                //data.Webcam = baseinfo.Webcam != null ? baseinfo.Webcam.Select(x => new WebcamLocalized() { GpsInfo = x.GpsInfo, WebcamId = x.WebcamId, Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "", Webcamurl = x.Webcamurl }).ToList() : null;
                VisibleInSearch = baseinfo.VisibleInSearch,
                DistrictIds = baseinfo.DistrictIds,
                TourismvereinIds = baseinfo.TourismvereinIds
            };
            //data.RegionIds = baseinfo.RegionIds;
            //data.DetailThemed = baseinfo.DetailThemed != null ? baseinfo.DetailThemed.ContainsKey(language) ? baseinfo.DetailThemed[language] : null : null;
        }

        public static SkiRegionLocalized TransformToSkiRegionLocalized(SkiRegion baseinfo, string language)
        {
            return new SkiRegionLocalized
            {
                Id = baseinfo.Id,
                Active = baseinfo.Active,
                ContactInfos =
                    baseinfo.ContactInfos != null ?
                        baseinfo.ContactInfos.ContainsKey(language) ?
                            baseinfo.ContactInfos[language] :
                            null :
                        null,
                Detail =
                    baseinfo.Detail != null ?
                        baseinfo.Detail.ContainsKey(language) ?
                            baseinfo.Detail[language] :
                            null :
                        null,
                Gpstype = baseinfo.Gpstype,
                Latitude = baseinfo.Latitude,
                Longitude = baseinfo.Longitude,
                Altitude = baseinfo.Altitude,
                AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure,
                ImageGallery =
                    baseinfo.ImageGallery?.Select(x => new ImageGalleryLocalized()
                    {
                        Height = x.Height,
                        ImageDesc =
                            x.ImageDesc.Count > 0 ?
                                x.ImageDesc.ContainsKey(language) ?
                                    x.ImageDesc[language] :
                                    "" :
                                "",
                        ImageName = x.ImageName,
                        ImageSource = x.ImageSource,
                        ImageTitle =
                            x.ImageTitle.Count > 0 ?
                                x.ImageTitle.ContainsKey(language) ?
                                    x.ImageTitle[language] :
                                    "" :
                                "",
                        ImageUrl = x.ImageUrl,
                        IsInGallery = x.IsInGallery,
                        ListPosition = x.ListPosition,
                        ValidFrom = x.ValidFrom,
                        ValidTo = x.ValidTo,
                        Width = x.Width,
                        CopyRight = x.CopyRight
                    })?.ToList(),
                CustomId = baseinfo.CustomId,
                Shortname = baseinfo.Shortname,
                SmgActive = baseinfo.SmgActive,
                SmgTags = baseinfo.SmgTags,
                GpsPolygon = baseinfo.GpsPolygon,
                Webcam =
                    baseinfo.Webcam?.Select(x => new WebcamLocalized()
                    {
                        GpsInfo = x.GpsInfo,
                        WebcamId = x.WebcamId,
                        Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "",
                        Webcamurl = x.Webcamurl
                    })?.ToList()
            };
        }

        public static SkiAreaLocalized TransformToSkiAreaLocalized(SkiArea baseinfo, string language)
        {
            return new SkiAreaLocalized
            {
                Id = baseinfo.Id,
                Active = baseinfo.Active,
                ContactInfos =
                    baseinfo.ContactInfos != null ?
                        baseinfo.ContactInfos.ContainsKey(language) ?
                            baseinfo.ContactInfos[language] :
                            null :
                        null,
                Detail =
                    baseinfo.Detail != null ?
                        baseinfo.Detail.ContainsKey(language) ?
                            baseinfo.Detail[language] :
                            null :
                        null,
                Gpstype = baseinfo.Gpstype,
                Latitude = baseinfo.Latitude,
                Longitude = baseinfo.Longitude,
                Altitude = baseinfo.Altitude,
                AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure,
                ImageGallery =
                    baseinfo.ImageGallery?.Select(x => new ImageGalleryLocalized()
                    {
                        Height = x.Height,
                        ImageDesc =
                            x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ?
                                x.ImageDesc[language] :
                                "" :
                            "",
                        ImageName = x.ImageName,
                        ImageSource = x.ImageSource,
                        ImageTitle =
                            x.ImageTitle.Count > 0 ?
                                x.ImageTitle.ContainsKey(language) ?
                                    x.ImageTitle[language] :
                                    "" :
                                "",
                        ImageUrl = x.ImageUrl,
                        IsInGallery = x.IsInGallery,
                        ListPosition = x.ListPosition,
                        ValidFrom = x.ValidFrom,
                        ValidTo = x.ValidTo,
                        Width = x.Width,
                        CopyRight = x.CopyRight
                    })?.ToList(),
                CustomId = baseinfo.CustomId,
                Shortname = baseinfo.Shortname,
                SmgActive = baseinfo.SmgActive,
                SmgTags = baseinfo.SmgTags,
                GpsPolygon = baseinfo.GpsPolygon,
                Webcam =
                    baseinfo.Webcam?.Select(x => new WebcamLocalized()
                    {
                        GpsInfo = x.GpsInfo,
                        WebcamId = x.WebcamId,
                        Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "",
                        Webcamurl = x.Webcamurl
                    })?.ToList(),

                SkiRegionId = baseinfo.SkiRegionId,
                SkiAreaMapURL = baseinfo.SkiAreaMapURL,
                TotalSlopeKm = baseinfo.TotalSlopeKm,
                SlopeKmBlue = baseinfo.SlopeKmBlue,
                SlopeKmRed = baseinfo.SlopeKmRed,
                SlopeKmBlack = baseinfo.SlopeKmBlack,
                LiftCount = baseinfo.LiftCount,
                AltitudeFrom = baseinfo.AltitudeFrom,
                AltitudeTo = baseinfo.AltitudeTo,
                SkiRegionName =
                    baseinfo.SkiRegionName != null ?
                        baseinfo.SkiRegionName.ContainsKey(language) ?
                            baseinfo.SkiRegionName[language] :
                            "" :
                        "",
                AreaId = baseinfo.AreaId,
                OperationSchedule = baseinfo.OperationSchedule,
                TourismvereinIds = baseinfo.TourismvereinIds,
                RegionIds = baseinfo.RegionIds,
                LocationInfo = new LocationInfoLocalized()
                {
                    DistrictInfo =
                        baseinfo.LocationInfo != null ?
                            baseinfo.LocationInfo.DistrictInfo != null ?
                                new DistrictInfoLocalized()
                                {
                                    Id = baseinfo.LocationInfo.DistrictInfo.Id,
                                    Name = baseinfo.LocationInfo?.DistrictInfo?.Name?[language]
                                } :
                                new DistrictInfoLocalized() :
                            new DistrictInfoLocalized(),
                    MunicipalityInfo =
                        baseinfo.LocationInfo != null ?
                            baseinfo.LocationInfo.MunicipalityInfo != null ?
                                new MunicipalityInfoLocalized()
                                {
                                    Id = baseinfo.LocationInfo.MunicipalityInfo.Id,
                                    Name = baseinfo.LocationInfo?.MunicipalityInfo?.Name?[language]
                                } :
                                new MunicipalityInfoLocalized() :
                            new MunicipalityInfoLocalized(),
                    TvInfo =
                        baseinfo.LocationInfo != null ?
                            baseinfo.LocationInfo.TvInfo != null ?
                                new TvInfoLocalized()
                                {
                                    Id = baseinfo.LocationInfo.TvInfo.Id,
                                    Name = baseinfo.LocationInfo?.TvInfo?.Name?[language]
                                } :
                                new TvInfoLocalized() :
                            new TvInfoLocalized(),
                    RegionInfo =
                        baseinfo.LocationInfo != null ?
                            baseinfo.LocationInfo.RegionInfo != null ?
                                new RegionInfoLocalized()
                                {
                                    Id = baseinfo.LocationInfo.RegionInfo.Id,
                                    Name = baseinfo.LocationInfo?.RegionInfo?.Name?[language]
                                } :
                                new RegionInfoLocalized() :
                            new RegionInfoLocalized()
                }
            };
        }

        public static MobileData TransformAccommodationToMobileDataObject(
            Accommodation acommodation, string language)
        {
            var nulldouble = 0;

            return new MobileData
            {
                Id = acommodation.Id,
                Name =
                    acommodation.AccoDetail?[language] != null ?
                        acommodation.AccoDetail[language].Name ?? "" :
                        "",
                Image =
                    acommodation.ImageGallery?.Count > 0 ?
                        acommodation.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ?
                            acommodation.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl :
                            acommodation.ImageGallery.FirstOrDefault().ImageUrl :
                        "noimage",
                Region = acommodation.LocationInfo?.RegionInfo?.Name?[language],
                Tourismverein = acommodation.LocationInfo?.TvInfo?.Name?[language],
                Latitude = acommodation.Latitude,
                Longitude = acommodation.Longitude,
                Type = "acco",
                Category = new Dictionary<string, string>()
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
                },
                Additional = new Dictionary<string, string>()
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
                }
            };
        }

        public static MobileData TransformODHActivityPoiToMobileDataObject(
            ODHActivityPoi smgpoi,
            string language)
        {
            var nulldouble = 0;

            return new MobileData
            {
                Id = smgpoi.Id,
                Name =
                    smgpoi.Detail[language] != null ?
                        smgpoi.Detail[language].Title ?? "" :
                        "",
                //Wenn Image leer "noimage", wenn ImageListpos == 0 
                //Image = activity.ImageGallery != null ? activity.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ? activity.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageSource == "LTS" ? activity.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl + "&W=200" : activity.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl + "&width=200" : activity.ImageGallery.FirstOrDefault().ImageUrl + "&width=200" : "noimage",
                Image =
                    smgpoi.ImageGallery?.Count > 0 ?
                        smgpoi.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ?
                            smgpoi.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl :
                            smgpoi.ImageGallery.FirstOrDefault().ImageUrl :
                        "noimage",
                Region =
                    smgpoi.LocationInfo?.RegionInfo != null ?
                        smgpoi.LocationInfo?.RegionInfo?.Name?[language] :
                        "",
                Tourismverein =
                    smgpoi.LocationInfo?.TvInfo != null ?
                        smgpoi.LocationInfo?.TvInfo?.Name?[language] :
                        "",
                Latitude =
                    smgpoi.GpsInfo != null ?
                        smgpoi.GpsInfo.Count > 0 ?
                            smgpoi.GpsInfo.Where(x => x.Gpstype == "position").Count() > 0 ?
                                smgpoi.GpsInfo.Where(x => x.Gpstype == "position").FirstOrDefault().Latitude :
                                smgpoi.GpsInfo.FirstOrDefault().Latitude :
                            0 :
                        0,
                Longitude =
                    smgpoi.GpsInfo != null ?
                        smgpoi.GpsInfo.Count > 0 ?
                            smgpoi.GpsInfo.Where(x => x.Gpstype == "position").Count() > 0 ?
                                smgpoi.GpsInfo.Where(x => x.Gpstype == "position").FirstOrDefault().Longitude :
                                smgpoi.GpsInfo.FirstOrDefault().Longitude :
                            0 :
                        0,
                Type = "smgpoi",
                Category =
                new Dictionary<string, string>()
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
                },
                Additional = new Dictionary<string, string>()
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
                }
            };
        }

        public static MobileData TransformEventToMobileDataObject(Event eventinfo, string language)
        {
            var nulldouble = 0;

            MobileData data = new MobileData
            {
                Id = eventinfo.Id,
                Name =
                    eventinfo.Detail[language] != null ?
                        eventinfo.Detail[language].Title ?? "" :
                        "",
                //Image = eventinfo.ImageGallery != null ? eventinfo.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ? eventinfo.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageSource == "LTS" ? eventinfo.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl + "&W=200" : eventinfo.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl + "&width=200" : eventinfo.ImageGallery.FirstOrDefault().ImageUrl + "&width=200" : "noimage",
                Image =
                    eventinfo.ImageGallery?.Count > 0 ?
                        eventinfo.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ?
                            eventinfo.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl :
                            eventinfo.ImageGallery.FirstOrDefault().ImageUrl :
                        "noimage",
                Region = eventinfo.LocationInfo?.RegionInfo?.Name?[language],
                Tourismverein = eventinfo.LocationInfo?.TvInfo?.Name?[language],
                Latitude = eventinfo.Latitude,
                Longitude = eventinfo.Longitude,
                Type = "event",
                Category = new Dictionary<string, string>()
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
                },
                Additional = new Dictionary<string, string>()
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
                    { "topevent",
                            eventinfo.SmgTags != null ?
                                eventinfo.SmgTags.Contains("TopEvent") ?
                                    "true" :
                                    "false" :
                                "false" },
                    { "begindate", String.Format("{0:MM/dd/yyyy}", eventinfo.DateBegin) },
                    { "enddate", String.Format("{0:MM/dd/yyyy}", eventinfo.DateEnd) }
                    //{ "begindate", ((DateTime)eventinfo.DateBegin).ToShortDateString() },
                    //{ "enddate", ((DateTime)eventinfo.DateEnd).ToShortDateString() }
                }
            };
            return data;

        }

        public static MobileDataExtended TransformODHActivityPoiToMobileDataExtendedObject(
            ODHActivityPoi activity, string language)
        {
            var nulldouble = 0;

            MobileDataExtended data = new MobileDataExtended
            {
                Id = activity.Id,
                Name =
                    activity.Detail[language] != null ?
                        activity.Detail[language].Title ?? "" :
                        "",
                //Wenn Image leer "noimage", wenn ImageListpos == 0 
                ShortDesc =
                    activity.Detail[language] != null ?
                        activity.Detail[language].MetaDesc ?? "" :
                        "",
                Image =
                    activity.ImageGallery?.Count > 0 ?
                        activity.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ?
                            activity.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl :
                            activity.ImageGallery.FirstOrDefault().ImageUrl :
                        "noimage",
                Region = activity.LocationInfo?.RegionInfo?.Name?[language],
                Tourismverein = activity.LocationInfo?.TvInfo?.Name?[language],
                Latitude =
                    activity.GpsInfo != null ?
                        activity.GpsInfo.Count > 0 ?
                            activity.GpsInfo.Where(x => x.Gpstype == "position").Count() > 0 ?
                                activity.GpsInfo.Where(x => x.Gpstype == "position").FirstOrDefault().Latitude :
                                activity.GpsInfo.FirstOrDefault().Latitude :
                            0 :
                        0,
                Longitude = activity.GpsInfo != null ? activity.GpsInfo.Count > 0 ? activity.GpsInfo.Where(x => x.Gpstype == "position").Count() > 0 ? activity.GpsInfo.Where(x => x.Gpstype == "position").FirstOrDefault().Longitude : activity.GpsInfo.FirstOrDefault().Longitude : 0 : 0,
                Type = "smgpoi",
                Category = new Dictionary<string, string>()
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
                },
                Additional = new Dictionary<string, string>()
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
                }
            };

            return data;
        }

        public static MobileDetail TransformEventToMobileDetail(Event myevent, string language)
        {
            return new MobileDetail
            {
                Id = myevent.Id,
                AltitudeDifference = 0,
                AltitudeHighestPoint = 0,
                AltitudeLowestPoint = 0,
                AltitudeSumDown = 0,
                AltitudeSumUp = 0,
                //CapacityCeremony = null,
                //CategoryCodes = null,
                ContactInfos = myevent.ContactInfos[language],
                DateBegin = myevent.DateBegin,
                DateEnd = myevent.DateEnd,
                Detail = myevent.Detail[language],
                Difficulty = "",
                //DishRates = null,
                DistanceDuration = 0,
                DistanceLength = 0,
                EventDate = myevent.EventDate,
                Exposition = null,
                //Facilities = null,
                FeetClimb = false,
                GpsInfo =
                    Enumerable.Repeat<GpsInfo>(new GpsInfo()
                    {
                        Altitude = myevent.Altitude,
                        Latitude = myevent.Latitude,
                        Longitude = myevent.Longitude,
                        Gpstype = "position"
                    }, 1).ToList(),
                GpsTrack = null,
                HasFreeEntrance = false,
                HasRentals = false,
                Highlight =
                    myevent.SmgTags != null ?
                        myevent.SmgTags.Contains("TopEvent") ?
                            true :
                            false :
                        false,
                //data.ImageGallery = myevent.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc[language], ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle[language], ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList();
                ImageGallery =
                    myevent.ImageGallery.Select(x => new ImageGalleryLocalized()
                    {
                        Height = x.Height,
                        ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc[language] : null,
                        ImageName = x.ImageName,
                        ImageSource = x.ImageSource,
                        ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle[language] : null,
                        ImageUrl = x.ImageUrl,
                        IsInGallery = x.IsInGallery,
                        ListPosition = x.ListPosition,
                        ValidFrom = x.ValidFrom,
                        ValidTo = x.ValidTo,
                        Width = x.Width,
                        CopyRight = x.CopyRight
                    }).ToList(),
                IsOpen = false,
                IsPrepared = false,
                IsWithLigth = false,
                LiftAvailable = false,
                LocationInfo = new LocationInfoLocalized()
                {
                    DistrictInfo = new DistrictInfoLocalized()
                    {
                        Id = myevent.LocationInfo?.DistrictInfo?.Id,
                        Name = myevent.LocationInfo?.DistrictInfo?.Name?[language]
                    },
                    MunicipalityInfo = new MunicipalityInfoLocalized()
                    {
                        Id = myevent.LocationInfo?.MunicipalityInfo?.Id,
                        Name = myevent.LocationInfo?.MunicipalityInfo?.Name?[language]
                    },
                    TvInfo = new TvInfoLocalized()
                    {
                        Id = myevent.LocationInfo?.TvInfo?.Id,
                        Name = myevent.LocationInfo?.TvInfo?.Name?[language]
                    },
                    RegionInfo = new RegionInfoLocalized()
                    {
                        Id = myevent.LocationInfo?.RegionInfo?.Id,
                        Name = myevent.LocationInfo?.RegionInfo?.Name?[language]
                    }
                },
                MainType = "event",
                MaxSeatingCapacity = 0,
                Novelty = "",
                OperationSchedule = null,
                OrganizerInfos = myevent.OrganizerInfos[language],
                PayMet = myevent.PayMet,
                PoiType = "",
                //PoiProperty = myevent.TopicRIDs.Select(x => new PoiProperty(){ Name = "TopicRID", Value = x }).ToList(),
                PoiProperty = null,
                PoiServices = null, //myevent.TopicRIDs,
                Ranc = myevent.Ranc,
                Ratings = null,
                RunToValley = false,
                SignOn = myevent.SignOn,
                SmgTags = myevent.SmgTags,
                SubType =
                    language != "de" ?
                        language != "it" ?
                            "Event" :
                            "Evento" :
                        "Veranstaltung",
                Ticket = myevent.Ticket,
                //TopicRIDs = myevent.TopicRIDs,
                Type =
                    language != "de" ?
                        language != "it" ?
                            "Event" :
                            "Evento" :
                        "Veranstaltung"
            };
        }

        public static MobileDetail TransformODHActivityPoiToMobileDetail(
            ODHActivityPoi activity, string language) => new MobileDetail
            {
                Id = activity.Id,
                //AdditionalPoiInfos = activity.AdditionalPoiInfos[language],
                AltitudeDifference = activity.AltitudeDifference,
                AltitudeHighestPoint = activity.AltitudeHighestPoint,
                AltitudeLowestPoint = activity.AltitudeLowestPoint,
                AltitudeSumDown = activity.AltitudeSumDown,
                AltitudeSumUp = activity.AltitudeSumUp,
                //CapacityCeremony = null,
                //CategoryCodes = null,
                ContactInfos = activity.ContactInfos[language],
                DateBegin = null,
                DateEnd = null,
                Detail = activity.Detail[language],
                Difficulty = activity.Difficulty,
                //DishRates = null,
                DistanceDuration = activity.DistanceDuration,
                DistanceLength = activity.DistanceLength,
                EventDate = null,
                Exposition = activity.Exposition,
                //Facilities = null,
                FeetClimb = activity.FeetClimb,
                GpsInfo = activity.GpsInfo,
                GpsTrack = activity.GpsTrack,
                HasFreeEntrance = activity.HasFreeEntrance,
                HasRentals = activity.HasRentals,
                Highlight = activity.Highlight,
                //data.ImageGallery = activity.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc[language], ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle[language], ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList();
                ImageGallery =
                    activity.ImageGallery.Select(x => new ImageGalleryLocalized()
                    {
                        Height = x.Height,
                        ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc[language] : null,
                        ImageName = x.ImageName,
                        ImageSource = x.ImageSource,
                        ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle[language] : null,
                        ImageUrl = x.ImageUrl,
                        IsInGallery = x.IsInGallery,
                        ListPosition = x.ListPosition,
                        ValidFrom = x.ValidFrom,
                        ValidTo = x.ValidTo,
                        Width = x.Width,
                        CopyRight = x.CopyRight
                    }).ToList(),
                IsOpen = activity.IsOpen,
                IsPrepared = activity.IsPrepared,
                IsWithLigth = activity.IsWithLigth,
                LiftAvailable = activity.LiftAvailable,
                LocationInfo = new LocationInfoLocalized()
                {
                    DistrictInfo = new DistrictInfoLocalized()
                    {
                        Id = activity.LocationInfo?.DistrictInfo?.Id,
                        Name = activity.LocationInfo?.DistrictInfo?.Name?[language]
                    },
                    MunicipalityInfo = new MunicipalityInfoLocalized()
                    {
                        Id = activity.LocationInfo?.MunicipalityInfo?.Id,
                        Name = activity.LocationInfo?.MunicipalityInfo?.Name?[language]
                    },
                    TvInfo = new TvInfoLocalized()
                    {
                        Id = activity.LocationInfo?.TvInfo?.Id,
                        Name = activity.LocationInfo?.TvInfo?.Name?[language]
                    },
                    RegionInfo = new RegionInfoLocalized()
                    {
                        Id = activity.LocationInfo?.RegionInfo?.Id,
                        Name = activity.LocationInfo?.RegionInfo?.Name?[language]
                    }
                },
                MainType = "smgpoi",
                MaxSeatingCapacity = 0,
                Novelty = activity.AdditionalPoiInfos[language].Novelty,
                OperationSchedule = activity.OperationSchedule,
                OrganizerInfos = null,
                PayMet = "",
                PoiType = activity.AdditionalPoiInfos[language].PoiType,
                //PoiProperty = activity.SyncSourceInterface == "GastronomicData" ? activity.CategoryCodes.Select(x => new PoiProperty() { Name = x.Id, Value = x.Shortname }).ToList() : activity.PoiProperty[language],
                PoiProperty =
                    activity.PoiProperty != null ?
                        activity.PoiProperty.Count > 0 ?
                            activity.PoiProperty[language]?.Where(e => e.Name != null)
                                .ToDictionary(e => e.Name ?? "", v => v.Value ?? "") :
                            null :
                        null,
                PoiServices =
                    activity.SyncSourceInterface == "GastronomicData" ?
                        activity.Facilities?.Where(x => x.Id != null).Select(x => x.Id ?? "").ToList() :
                        activity.PoiServices,
                Ranc = 0,
                Ratings = activity.Ratings,
                RunToValley = activity.RunToValley,
                SignOn = "",
                SmgTags = activity.SmgTags,
                SubType = activity.AdditionalPoiInfos[language].SubType,
                Ticket = "",
                //TopicRIDs = null, 
                Type = activity.AdditionalPoiInfos[language].MainType
            };

        public static MobileHtmlLocalized TransformToHtmlLocalized(MobileHtml data, string language)
        {
            return new MobileHtmlLocalized
            {
                Id = data.Id,
                HtmlText = data.HtmlText[language]
            };
        }


        #endregion

        #region Transformer Helpers



        #endregion
    }
}
