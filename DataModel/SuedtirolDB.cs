// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Converters;
using DataModel.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics;

namespace DataModel
{
    #region Interfaces

    //Common Interfaces (shared between all Entities)

    public interface IIdentifiable
    {
        string Id { get; set; }
        string? Shortname { get; set; }
    }

    public interface IMetaData
    {
        Metadata _Meta { get; set; }
    }

    public interface ILicenseInfo
    {
        LicenseInfo LicenseInfo { get; set; }
    }

    public interface ISource
    {
        [SwaggerSchema("Source of the Data")]
        string? Source { get; set; }
    }

    public interface IImportDateassigneable
    {
        DateTime? FirstImport { get; set; }
        DateTime? LastChange { get; set; }
    }

    public interface IMappingAware
    {
        IDictionary<string, IDictionary<string, string>> Mapping { get; set; }
    }

    public interface IActivateable
    {
        bool Active { get; set; }
    }

    public interface ILanguage
    {
        string? Language { get; set; }
    }

    public interface IHasLanguage
    {
        ICollection<string>? HasLanguage { get; set; }
    }

    public interface IDetailInfosAware
    {
        IDictionary<string, Detail> Detail { get; set; }
    }

    public interface IDetailInfos
    {
        string? Header { get; set; }
        string? IntroText { get; set; }
        string? BaseText { get; set; }
        string? Title { get; set; }

        string? MetaTitle { get; set; }
        string? MetaDesc { get; set; }

        string? AdditionalText { get; set; }
        string? GetThereText { get; set; }
    }

    public interface IContactInfos
    {
        string? Address { get; set; }
        string? City { get; set; }
        string? ZipCode { get; set; }
        string? CountryCode { get; set; }
        string? CountryName { get; set; }
        string? Surname { get; set; }
        string? Givenname { get; set; }
        string? NamePrefix { get; set; }
        string? Email { get; set; }
        string? Phonenumber { get; set; }
        string? Faxnumber { get; set; }
        string? Url { get; set; }
        string? Vat { get; set; }
        string? Tax { get; set; }
    }

    public interface IContactInfosAware
    {
        IDictionary<string, ContactInfos> ContactInfos { get; set; }
    }

    public interface IImageGallery
    {
        string? ImageName { get; set; }
        string? ImageUrl { get; set; }
        int? Width { get; set; }
        int? Height { get; set; }
        string? ImageSource { get; set; }

        IDictionary<string, string> ImageTitle { get; set; }
        IDictionary<string, string> ImageDesc { get; set; }

        bool? IsInGallery { get; set; }
        int? ListPosition { get; set; }
        DateTime? ValidFrom { get; set; }
        DateTime? ValidTo { get; set; }
    }

    public interface IImageGalleryAware
    {
        ICollection<ImageGallery>? ImageGallery { get; set; }
    }

    public interface IVideoItems
    {
        string? Name { get; set; }
        string? Url { get; set; }
        string? VideoSource { get; set; }
        string? VideoType { get; set; }

        string? StreamingSource { get; set; }

        string VideoTitle { get; set; }
        string VideoDesc { get; set; }

        bool? Active { get; set; }
        string? CopyRight { get; set; }
        string? License { get; set; }
        string? LicenseHolder { get; set; }
    }

    public interface IVideoItemsAware
    {
        IDictionary<string, ICollection<VideoItems>>? VideoItems { get; set; }
    }

    public interface ILocationInfoAware
    {
        RegionInfo? RegionInfo { get; set; }
        TvInfo? TvInfo { get; set; }
        MunicipalityInfo? MunicipalityInfo { get; set; }
        DistrictInfo? DistrictInfo { get; set; }
    }

    public interface IGpsInfo
    {
        string? Gpstype { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
        double? Altitude { get; set; }
        string? AltitudeUnitofMeasure { get; set; }
    }

    public interface IGPSInfoAware
    {
        ICollection<GpsInfo> GpsInfo { get; set; }
    }

    public interface IGPSPointsAware
    {
        IDictionary<string, GpsInfo> GpsPoints { get; }
    }

    public interface IDistanceInfoAware
    {
        DistanceInfo? DistanceInfo { get; set; }
    }

    public interface IDistanceInfo
    {
        Nullable<double> DistanceToDistrict { get; set; }

        Nullable<double> DistanceToMunicipality { get; set; }
    }

    public interface IGpsTrack
    {
        string? Id { get; set; }
        IDictionary<string, string> GpxTrackDesc { get; set; }
        string? GpxTrackUrl { get; set; }
        string? Type { get; set; }

        string? Format { get; set; }
    }

    public interface IGpsPolygon
    {
        double Latitude { get; set; }
        double Longitude { get; set; }
    }

    public interface IGpsPolygonAware
    {
        ICollection<GpsPolygon>? GpsPolygon { get; set; }
    }

    public interface IGeoDataInfoAware
    {
        double? AltitudeDifference { get; set; }
        double? AltitudeHighestPoint { get; set; }
        double? DistanceDuration { get; set; }
        double? DistanceLength { get; set; }

        ICollection<GpsInfo>? GpsInfo { get; set; }
        ICollection<GpsTrack>? GpsTrack { get; set; }
    }

    public interface IWebcam
    {
        string? WebcamId { get; set; }
        IDictionary<string, string> Webcamname { get; set; }
        GpsInfo? GpsInfo { get; set; }

        string? Webcamurl { get; set; }
    }

    public interface IWebcamAware
    {
        ICollection<Webcam>? Webcam { get; set; }
    }

    public interface IOperationSchedules
    {
        IDictionary<string, string> OperationscheduleName { get; set; }
        //string OperationscheduleName { get; set; }
        DateTime Start { get; set; }
        DateTime Stop { get; set; }
        //bool? ClosedonPublicHolidays { get; set; }

        ICollection<OperationScheduleTime>? OperationScheduleTime { get; set; }
    }

    public interface IOperationScheduleTime
    {
        TimeSpan Start { get; set; }
        TimeSpan End { get; set; }
        bool Monday { get; set; }
        bool Tuesday { get; set; }
        bool Wednesday { get; set; }
        bool Thursday { get; set; }
        bool Friday { get; set; }
        bool Saturday { get; set; }
        bool Sunday { get; set; }
        int State { get; set; }
        int Timecode { get; set; }
    }

    public interface ISmgTags
    {
        ICollection<string>? SmgTags { get; set; }
    }

    public interface IRatings
    {
        string? Stamina { get; set; }
        string? Experience { get; set; }
        string? Landscape { get; set; }
        string? Difficulty { get; set; }
        string? Technique { get; set; }
    }

    public interface ISmgActive
    {
        bool SmgActive { get; set; }
    }

    public interface ISuedtirolType
    {
        string? Id { get; set; }
        string? Key { get; set; }
        string? Entity { get; set; }
        string? TypeParent { get; set; }
        int Level { get; set; }
        IDictionary<string, string> TypeNames { get; set; }
    }

    public interface ITags
    {
        IDictionary<string, List<Tags>> Tags { get; set; }
    }

    public interface IPublishedOn
    {
        ICollection<string>? PublishedOn { get; set; }
    }

    //ODHActivityPoi

    public interface IAdditionalPoiInfosAware
    {
        IDictionary<string, AdditionalPoiInfos> AdditionalPoiInfos { get; set; }
    }

    public interface IAdditionalPoiInfos
    {
        string? Novelty { get; set; }
        string? MainType { get; set; }
        string? SubType { get; set; }
        string? PoiType { get; set; }

        List<string> Categories { get; set; }
    }

    public interface IActivityStatus
    {
        bool? IsOpen { get; set; }
        bool? IsPrepared { get; set; }
        bool? RunToValley { get; set; }
        bool? IsWithLigth { get; set; }
        bool? HasRentals { get; set; }
    }

    //Article

    public interface IAdditionalArticleInfosAware
    {        
        IDictionary<string, AdditionalArticleInfos> AdditionalArticleInfos { get; set; }
    }

    //Event

    public interface IEventAdditionalInfos
    {
        string? Mplace { get; set; }
        string? Reg { get; set; }
        string? Location { get; set; }
    }


    public interface IEventPrice
    {
        double Price { get; set; }
        string? Type { get; set; }
        string? Pstd { get; set; }
        string? ShortDesc { get; set; }
        string? Description { get; set; }
    }

    public interface IEventDate
    {
        DateTime From { get; set; }
        DateTime To { get; set; }
        bool? SingleDays { get; set; }
        int? MinPersons { get; set; }
        int? MaxPersons { get; set; }
        bool? Ticket { get; set; }
        double? GpsNorth { get; set; }
        double? GpsEast { get; set; }
        TimeSpan? Begin { get; set; }
        TimeSpan? End { get; set; }
        TimeSpan? Entrance { get; set; }
    }

    #endregion

    #region District Municipality Region

    public class Region : BaseInfos, IImageGalleryAware, IWebcamAware, IGpsPolygon, IPublishedOn
    {
        public Region()
        {
            DetailThemed = new Dictionary<string, DetailThemed>();
        }
        public IDictionary<string, DetailThemed> DetailThemed { get; set; }

        public ICollection<GpsPolygon>? GpsPolygon { get; set; }
        public ICollection<Webcam>? Webcam { get; set; }
        public bool VisibleInSearch { get; set; }
        public ICollection<string>? SkiareaIds { get; set; }

        public ICollection<RelatedContent>? RelatedContent { get; set; }        
    }

    public class MetaRegion : BaseInfos, IImageGalleryAware, IWebcamAware, IGpsPolygon
    {
        public MetaRegion()
        {
            DetailThemed = new Dictionary<string, DetailThemed>();
        }

        public IDictionary<string, DetailThemed> DetailThemed { get; set; }
        public ICollection<string>? DistrictIds { get; set; }
        public ICollection<string>? TourismvereinIds { get; set; }
        public ICollection<string>? RegionIds { get; set; }
        public ICollection<GpsPolygon>? GpsPolygon { get; set; }
        public ICollection<Webcam>? Webcam { get; set; }
        public bool VisibleInSearch { get; set; }

        public ICollection<RelatedContent>? RelatedContent { get; set; }        
    }

    public class ExperienceArea : BaseInfos, IImageGalleryAware, IGpsPolygon
    {
        public ICollection<string>? DistrictIds { get; set; }
        public ICollection<string>? TourismvereinIds { get; set; }
        public ICollection<GpsPolygon>? GpsPolygon { get; set; }
        public bool VisibleInSearch { get; set; }
        public ICollection<RelatedContent>? RelatedContent { get; set; }
    }

    public class Tourismverein : BaseInfos, IImageGalleryAware, IWebcamAware, IGpsPolygon
    {
        public string? RegionId { get; set; }

        public ICollection<GpsPolygon>? GpsPolygon { get; set; }
        public ICollection<Webcam>? Webcam { get; set; }
        public bool VisibleInSearch { get; set; }
        public ICollection<string>? SkiareaIds { get; set; }

        public ICollection<RelatedContent>? RelatedContent { get; set; }        
    }

    public class Municipality : BaseInfos, IImageGalleryAware, IWebcamAware, IGpsPolygon
    {
        public string? Plz { get; set; }

        public string? RegionId { get; set; }
        public string? TourismvereinId { get; set; }
        public string? SiagId { get; set; }

        public ICollection<GpsPolygon>? GpsPolygon { get; set; }
        public ICollection<Webcam>? Webcam { get; set; }
        public bool VisibleInSearch { get; set; }

        public int Inhabitants { get; set; }
        public string? IstatNumber { get; set; }

        public ICollection<RelatedContent>? RelatedContent { get; set; }
    }

    public class District : BaseInfos, IImageGalleryAware, IWebcamAware, IGpsPolygon
    {
        public Nullable<bool> IsComune { get; set; }
        public string? RegionId { get; set; }
        public string? TourismvereinId { get; set; }
        public string? MunicipalityId { get; set; }
        public string? SiagId { get; set; }
        public ICollection<GpsPolygon>? GpsPolygon { get; set; }
        public ICollection<Webcam>? Webcam { get; set; }
        public bool VisibleInSearch { get; set; }
        public ICollection<RelatedContent>? RelatedContent { get; set; }        
    }

    public class Area : IIdentifiable, IActivateable, IImportDateassigneable, IMappingAware, ISource, ISmgActive, IPublishedOn
    {
        public Area()
        {
            Mapping = new Dictionary<string, IDictionary<string, string>>();
            Detail = new Dictionary<string, Detail>();
        }
        public LicenseInfo? LicenseInfo { get; set; }
        public string? Id { get; set; }
        public bool Active { get; set; }
        public bool SmgActive { get; set; }
        public string? Shortname { get; set; }
        public string? CustomId { get; set; }
        public string? RegionId { get; set; }
        public string? TourismvereinId { get; set; }
        public string? MunicipalityId { get; set; }
        public string? SkiAreaID { get; set; }
        public string? GID { get; set; }
        public string? LtsID { get; set; }
        public string? AreaType { get; set; }
        public DateTime? LastChange { get; set; }
        public DateTime? FirstImport { get; set; }
        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }
        public string Source { get; set; }
        public IDictionary<string, Detail> Detail { get; set; }

        public ICollection<string>? PublishedOn { get; set; }
    }

    public class GeneralGroup : BaseInfos
    {

    }

    public class SkiArea : BaseInfos, IImageGalleryAware, IWebcamAware, IContactInfosAware
    {
        public SkiArea()
        {
            SkiRegionName = new Dictionary<string, string>();
        }

        public string? SkiRegionId { get; set; }
        public string? SkiAreaMapURL { get; set; }
        public string? TotalSlopeKm { get; set; }

        public string? SlopeKmBlue { get; set; }
        public string? SlopeKmRed { get; set; }
        public string? SlopeKmBlack { get; set; }

        public string? LiftCount { get; set; }

        public string? AreaRadius { get; set; }


        public Nullable<int> AltitudeFrom { get; set; }
        public Nullable<int> AltitudeTo { get; set; }


        public IDictionary<string, string> SkiRegionName { get; set; }

        [SwaggerDeprecated("Deprecated use AreaIds")]
        public ICollection<string>? AreaId { get; set; }

        [GetOnlyJsonProperty]
        public ICollection<string>? AreaIds { get { return AreaId; } }

        public ICollection<Webcam>? Webcam { get; set; }

        public LocationInfo? LocationInfo { get; set; }

        public ICollection<OperationSchedule>? OperationSchedule { get; set; }

        public ICollection<string>? TourismvereinIds { get; set; }
        public ICollection<string>? RegionIds { get; set; }

        //New Municipality and District Ids
        public ICollection<string> MunicipalityIds { get; set; }
        public ICollection<string> DistrictIds { get; set; }

        public ICollection<GpsPolygon>? GpsPolygon { get; set; }

        public ICollection<RelatedContent>? RelatedContent { get; set; }        
    }

    public class SkiRegion : BaseInfos, IImageGalleryAware, IGpsPolygonAware, IWebcamAware
    {
        public ICollection<GpsPolygon>? GpsPolygon { get; set; }

        public ICollection<Webcam>? Webcam { get; set; }

        public ICollection<RelatedContent>? RelatedContent { get; set; }        
    }

    public class SmgTags : IIdentifiable, IImportDateassigneable, ILicenseInfo, IPublishedOn
    {
        public LicenseInfo? LicenseInfo { get; set; }

        public SmgTags()
        {
            TagName = new Dictionary<string, string>();
            ValidForEntity = new List<string>();

            Mapping = new Dictionary<string, IDictionary<string, string>>();
        }

        public string? Id { get; set; }

        [SwaggerDeprecated("Deprecated, refer to TagName")]
        public string? Shortname { get; set; }

        public IDictionary<string, string> TagName { get; set; }
        public ICollection<string> ValidForEntity { get; set; }


        public ICollection<string> Source { get; set; }

        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }

        public Nullable<bool> DisplayAsCategory { get; set; }

        [SwaggerDeprecated("Deprecated, use Mapping or MappedIds")]
        public IDictionary<string, string> IDMCategoryMapping { get; set; }
        [SwaggerDeprecated("Deprecated, use Mapping")]
        public LTSTaggingInfo LTSTaggingInfo { get; set; }

        [SwaggerDeprecated("Deprecated, use ValidForEntity")]
        public string? MainEntity { get; set; }

        //Generic Mapping Object
        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }

        public ICollection<string> MappedTagIds { get; set; }

        //obsolete replaced by PublishDataWithTagOn to be more generic
        //public ICollection<string>? AutoPublishOn { get; set; }

        //If this Tag is set whitelist for publisher true/false (Whitelist / Blacklist logic)
        public IDictionary<string, bool>? PublishDataWithTagOn { get; set; }

        public ICollection<string>? PublishedOn { get; set; }
    }

    public class Publisher : IIdentifiable, IImportDateassigneable, ILicenseInfo
    {
        public LicenseInfo? LicenseInfo { get; set; }

        public Publisher()
        {
            Name = new Dictionary<string, string>();
            
            //Mapping = new Dictionary<string, IDictionary<string, string>>();
        }

        public string? Id { get; set; }

        [SwaggerDeprecated("Deprecated, refer to Name")]
        public string? Shortname { get; set; }

        public IDictionary<string, string> Name { get; set; }        
        
        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }
        
        public string? PublisherUrl { get; set; }

        //Generic Mapping Object
        //public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }
    }


    public class LTSTaggingInfo
    {
        //NEW LTS RID
        public string LTSRID { get; set; }
        public string ParentLTSRID { get; set; }
    }

    public class LTSTin
    {
        public LTSTin()
        {
            Name = new Dictionary<string, string>();
            Description = new Dictionary<string, string>();
            LTSTinAssignment = new List<LTSTinAssignment>();
        }

        public string Id { get; set; }

        public IDictionary<string, string> Name { get; set; }
        public IDictionary<string, string> Description { get; set; }

        public ICollection<LTSTinAssignment> LTSTinAssignment { get; set; }
    }

    public class LTSTinAssignment
    {
        public string RID { get; set; }
        public string ODHTagId { get; set; }
        public string LTSTagRID { get; set; }
    }

    #endregion

    #region Marketinggroup

    public class Marketinggroup : IIdentifiable
    {
        public LicenseInfo? LicenseInfo { get; set; }

        public string? Id { get; set; }
        public string? Shortname { get; set; }
        public string? Beschreibung { get; set; }
    }

    #endregion

    #region Activities & POIs  

    public class LTSPoi : PoiBaseInfos, IGPSPointsAware
    {
        [SwaggerSchema(Description = "generated field", ReadOnly = true)]
        public IDictionary<string, GpsInfo> GpsPoints
        {
            get
            {
                return this.GpsInfo.ToGpsPointsDictionary(true);
            }
        }
    }

    public class LTSActivity : PoiBaseInfos, IGPSPointsAware
    {
        [SwaggerSchema(Description = "generated field", ReadOnly = true)]
        public IDictionary<string, GpsInfo> GpsPoints
        {
            get
            {
                return this.GpsInfo.ToGpsPointsDictionary(true);
            }
        }
    }

    public class ODHActivityPoi : PoiBaseInfos, IWebcamAware, ILicenseInfo, IGPSPointsAware
    {
        public ODHActivityPoi()
        {
            PoiProperty = new Dictionary<string, List<PoiProperty>>();
        }

        [SwaggerSchema("Id on the primary data Source")]
        public string? CustomId { get; set; }
        public ICollection<Webcam>? Webcam { get; set; }

        public IDictionary<string, List<PoiProperty>> PoiProperty { get; set; }
        public ICollection<string>? PoiServices { get; set; }

        public string? SyncSourceInterface { get; set; }
        public string? SyncUpdateMode { get; set; }

        public int? AgeFrom { get; set; }
        public int? AgeTo { get; set; }

        public int? MaxSeatingCapacity { get; set; }
        public ICollection<CategoryCodes>? CategoryCodes { get; set; }
        public ICollection<DishRates>? DishRates { get; set; }
        public ICollection<CapacityCeremony>? CapacityCeremony { get; set; }
        public ICollection<Facilities>? Facilities { get; set; }
        public ICollection<RelatedContent>? RelatedContent { get; set; }

        public IDictionary<string, List<AdditionalContact>>? AdditionalContact { get; set; }

        [SwaggerSchema(Description = "generated field", ReadOnly = true)]
        public IDictionary<string, GpsInfo> GpsPoints
        {
            get
            {
                return this.GpsInfo.ToGpsPointsDictionary();
            }
        }
    }

    public class PoiProperty
    {
        public string? Name { get; set; }
        public string? Value { get; set; }
    }

    public class AdditionalContact
    {
        public string Type { get; set; }
        public ContactInfos ContactInfos { get; set; }
        public string Description { get; set; }
    }

    public class RelatedContent
    {
        public string? Id { get; set; }

        [SwaggerDeprecated("Use the name of the referenced data")]
        public string? Name { get; set; }
        public string? Type { get; set; }

        public string? Link
        {
            get
            {
                if (!String.IsNullOrEmpty(this.Type))
                {
                    switch (this.Type.ToLower())
                    {
                        case "event":
                            return "Event/" + this.Id;
                        case "wineaward":
                            return "Common/WineAward/" + this.Id;
                        case "accommodation":
                            return "Accommodation/" + this.Id;
                        case "acco":
                            return "Accommodation/" + this.Id;
                        default:
                            return "ODHActivityPoi/" + this.Id;
                    }
                }
                else return "ODHActivityPoi/" + this.Id;
            }
        }

        public string Self {
            get
            {
                if (!String.IsNullOrEmpty(this.Type))
                {
                    switch (this.Type.ToLower())
                    {
                        case "event":
                            return "Event/" + this.Id;
                        case "wineaward":
                            return "Common/WineAward/" + this.Id;
                        case "accommodation":
                            return "Accommodation/" + this.Id;
                        case "acco":
                            return "Accommodation/" + this.Id;
                        default:
                            return "ODHActivityPoi/" + this.Id;
                    }
                }
                else return "ODHActivityPoi/" + this.Id;
            }
        }
    }

    public class RatingSources
    {
        public string? Source { get; set; }
        public string? Objectid { get; set; }
    }

    #endregion

    #region Accommodations

    public class Accommodation : TrustYouInfos, IIdentifiable, IActivateable, IGpsInfo, IImageGalleryAware, ISmgActive, IHasLanguage, IImportDateassigneable, ILicenseInfo, ISource, IMappingAware, IDistanceInfoAware, IPublishedOn, IGPSPointsAware
    {
        public LicenseInfo? LicenseInfo { get; set; }

        public Accommodation()
        {
            AccoDetail = new Dictionary<string, AccoDetail>();
            MssResponseShort = new List<MssResponseShort>();
            Mapping = new Dictionary<string, IDictionary<string, string>>();
        }

        public string? Id { get; set; }

        [SwaggerSchema("Data is marked as Active by the data provider")]
        public bool Active { get; set; }

        [SwaggerDeprecated("Accommodation ID on the HGV System, use Mapping instead")]
        public string? HgvId { get; set; }
        [SwaggerDeprecated("Use AccoDetail.lang.Name")]
        public string? Shortname { get; set; }

        //public int Units { get; set; }
        //public int Beds { get; set; }
        public int? Representation { get; set; }
        public bool HasApartment { get; set; }
        public bool? HasRoom { get; set; }
        public bool? IsCamping { get; set; }
        public bool? IsGastronomy { get; set; }
        public bool IsBookable { get; set; }
        public bool? IsAccommodation { get; set; }
        public bool SmgActive { get; set; }
        public bool? TVMember { get; set; }
        public string? TourismVereinId { get; set; }
        public string? MainLanguage { get; set; }
        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }
        public string? Gpstype { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Nullable<double> Altitude { get; set; }
        public string? AltitudeUnitofMeasure { get; set; }

        public DistanceInfo? DistanceInfo { get; set; }


        public string? AccoCategoryId { get; set; }
        public string? AccoTypeId { get; set; }
        public string? DistrictId { get; set; }

        public ICollection<string>? BoardIds { get; set; }
        public ICollection<string>? MarketingGroupIds { get; set; }
        public ICollection<AccoFeature>? Features { get; set; }

        public ICollection<string>? BadgeIds { get; set; }
        public ICollection<string>? ThemeIds { get; set; }
        public ICollection<string>? SpecialFeaturesIds { get; set; }

        public IDictionary<string, AccoDetail>? AccoDetail { get; set; }
        public ICollection<AccoBookingChannel>? AccoBookingChannel { get; set; }
        public ICollection<ImageGallery>? ImageGallery { get; set; }

        public LocationInfo? LocationInfo { get; set; }

        public string? GastronomyId { get; set; }
        public ICollection<string>? SmgTags { get; set; }

        public ICollection<string>? HasLanguage { get; set; }

        public ICollection<MssResponseShort>? MssResponseShort { get; set; }

        [SwaggerSchema("Independent Data <a href='https://www.independent.it/it/cooperativa-independent' target='_blank'>cooperative Independent</a>")]
        public IndependentData? IndependentData { get; set; }

        public ICollection<AccoRoomInfo>? AccoRoomInfo { get; set; }

        [SwaggerSchema(Description = "generated field", ReadOnly = true)]
        public IDictionary<string, GpsInfo> GpsPoints
        {
            get
            {
                if (this.Latitude != 0 && this.Longitude != 0)
                {
                    return new Dictionary<string, GpsInfo>
                    {
                        { "position", new GpsInfo(){ Gpstype = "position", Altitude = this.Altitude, AltitudeUnitofMeasure = this.AltitudeUnitofMeasure, Latitude = this.Latitude, Longitude = this.Longitude } }
                    };
                }
                else
                {
                    return new Dictionary<string, GpsInfo>
                    {
                    };
                }
            }
        }

        public ICollection<string>? PublishedOn { get; set; }

        public string? Source { get; set; }

        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }
    }

    public class AccoRoomInfo
    {
        public string Id { get; set; }

        public string Source { get; set; }
    }

    public class AccoDetail : ILanguage
    {
        public string? Language { get; set; }
        public string? Name { get; set; }
        public string? NameAddition { get; set; }
        public string? Street { get; set; }
        public string? Zip { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Fax { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? City { get; set; }
        public string? Shortdesc { get; set; }
        public string? Longdesc { get; set; }
        public string? Vat { get; set; }

        public string? CountryCode { get; set; }
    }

    public class AccoFeature
    {
        public string? Id { get; set; }
        public string? Name { get; set; }

        public string? HgvId { get; set; }
        public string? OtaCodes { get; set; }

        public List<int>? RoomAmenityCodes { get; set; }
    }

    public class AccoBookingChannel
    {
        public string? Id { get; set; }
        public string? Pos1ID { get; set; }
        public string? Portalname { get; set; }
        public string? BookingId { get; set; }
    }

    public abstract class TrustYouInfos
    {
        [SwaggerSchema("Accommodation Id on Trust You")]
        public string? TrustYouID { get; set; }

        [SwaggerSchema("Review Score on Trust You")]
        public double TrustYouScore { get; set; }

        [SwaggerSchema("Number of Ratings in Trust You")]
        public int TrustYouResults { get; set; }

        [SwaggerSchema("Active in Trust You")]
        public bool TrustYouActive { get; set; }

        [SwaggerSchema("Trust You State on LTS")]
        public int TrustYouState { get; set; }
    }

    public class AccoRoom : IIdentifiable, IImageGalleryAware, IHasLanguage, IImportDateassigneable, ILicenseInfo, ISource, IMappingAware, IPublishedOn
    {
        public LicenseInfo? LicenseInfo { get; set; }

        public AccoRoom()
        {
            AccoRoomDetail = new Dictionary<string, AccoRoomDetail>();
            Mapping = new Dictionary<string, IDictionary<string, string>>();
        }

        public string Id { get; set; }
        public string? Shortname { get; set; }

        public string? A0RID { get; set; }

        public string? Roomtype { get; set; }

        public ICollection<AccoFeature>? Features { get; set; }
        public IDictionary<string, AccoRoomDetail> AccoRoomDetail { get; set; }
        public ICollection<ImageGallery>? ImageGallery { get; set; }

        public ICollection<string>? HasLanguage { get; set; }

        public string? LTSId { get; set; }
        public string? HGVId { get; set; }
        public string? Source { get; set; }
        public string? RoomCode { get; set; }
        public Nullable<int> Roommax { get; set; }
        public Nullable<int> Roommin { get; set; }
        public Nullable<int> Roomstd { get; set; }
        public Nullable<double> PriceFrom { get; set; }
        public Nullable<int> RoomQuantity { get; set; }

        public List<string>? RoomNumbers { get; set; }
        public Nullable<int> RoomClassificationCodes { get; set; }
        public Nullable<int> RoomtypeInt { get; set; }

        public Nullable<DateTime> LastChange { get; set; }
        public Nullable<DateTime> FirstImport { get; set; }

        public ICollection<string>? PublishedOn { get; set; }

        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }
    }

    public class AccoRoomDetail : ILanguage
    {
        public string? Language { get; set; }

        public string? Name { get; set; }
        public string? Longdesc { get; set; }
        public string? Shortdesc { get; set; }
    }

    public class IndependentData
    {
        public IndependentData()
        {
            IndependentDescription = new Dictionary<string, IndependentDescription>();
        }

        public Dictionary<string, IndependentDescription> IndependentDescription { get; set; }
        public Int32? IndependentRating { get; set; }
        public bool? Enabled { get; set; }
    }

    public class IndependentDescription : ILanguage
    {
        public string? Language { get; set; }

        public string? Description { get; set; }
        public string? BacklinkUrl { get; set; }
    }

    #endregion

    #region Gastronomy

    public class Gastronomy : GastronomyBaseInfos, IGPSPointsAware
    {
        [SwaggerSchema(Description = "generated field", ReadOnly = true)]
        public IDictionary<string, GpsInfo> GpsPoints
        {
            get
            {
                if (this.Latitude != 0 && this.Longitude != 0)
                {
                    return new Dictionary<string, GpsInfo>
                    {
                        { "position", new GpsInfo(){ Gpstype = "position", Altitude = this.Altitude, AltitudeUnitofMeasure = this.AltitudeUnitofMeasure, Latitude = this.Latitude, Longitude = this.Longitude } }
                    };
                }
                else
                {
                    return new Dictionary<string, GpsInfo>
                    {
                    };
                }
            }
        }
    }

    #endregion

    #region Events

    public class Event : EventBaseInfos
    {

    }

    //TODO Migrate to new EventPricing class
    
    public class EventPricing
    {
        public EventPricing()
        {
            EventPricingDesc = new Dictionary<string, EventPricingDescription>();
        }

        public double Price { get; set; }
        public string? Type { get; set; }               
        public string PriceID { get; set; }
        
        public IDictionary<string, EventPricingDescription> EventPricingDesc { get; set; }
    }

    public class EventPricingDescription : ILanguage
    {
        public string? ShortDesc { get; set; }
        public string? LongDesc { get; set; }
        public string? Description { get; set; }
        public string? Language { get; set; }
    }

    #endregion

    #region Venues

    public class Venue : IIdentifiable, IActivateable, ISmgTags, IHasLanguage, IImportDateassigneable, ILicenseInfo, ISource, IMappingAware, IDistanceInfoAware, IGPSInfoAware, IPublishedOn, IImageGalleryAware, ISmgActive
    {
        public Venue()
        {
            //Mapping New
            Mapping = new Dictionary<string, IDictionary<string, string>>();
        }
             
        public LicenseInfo? LicenseInfo { get; set; }

        public string? Id { get; set; }

        public string? Shortname { get; set; }

        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }

        public bool Active { get; set; }
        public bool SmgActive { get; set; }
        
        public ICollection<string> SmgTags { get; set; }

        public LocationInfo LocationInfo { get; set; }
        public ICollection<string> HasLanguage { get; set; }

        public ICollection<VenueType> VenueCategory { get; set; }

        public ICollection<GpsInfo> GpsInfo { get; set; }

        public string Source { get; set; }
        public string SyncSourceInterface { get; set; }

        //New Details
        public Nullable<int> RoomCount { get; set; }
        public ICollection<VenueRoomDetails> RoomDetails { get; set; }

        [SwaggerSchema(Description = "generated field", ReadOnly = true)]

        public IDictionary<string, GpsInfo> GpsPoints
        {
            get
            {
                return this.GpsInfo.ToGpsPointsDictionary(true);
            }
        }

        public IDictionary<string, Detail> Detail { get; set; }

        public IDictionary<string, ContactInfos> ContactInfos { get; set; }

        public ICollection<ImageGallery> ImageGallery { get; set; }

        public ICollection<string>? PublishedOn { get; set; }

        //New Mapping
        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }

        public DistanceInfo DistanceInfo { get; set; }
    }


    #endregion

    #region Articles    

    public class Article : ArticleBaseInfos
    {

    }

    #endregion

    #region Weather

    public class Weather
    {
        public Weather()
        {
            this.Forecast = new HashSet<Forecast>();
            this.Mountain = new HashSet<Mountain>();
            this.Conditions = new HashSet<Conditions>();
            this.Stationdata = new HashSet<Stationdata>();
        }

        public int Id { get; set; }
        public DateTime date { get; set; }
        public string? evolutiontitle { get; set; }
        public string? evolution { get; set; }
        public string? language { get; set; }

        public ICollection<Conditions> Conditions { get; set; }
        public ICollection<Forecast> Forecast { get; set; }
        public ICollection<Mountain> Mountain { get; set; }
        public ICollection<Stationdata> Stationdata { get; set; }

        public LicenseInfo? LicenseInfo { get; set; }
    }

    public class Conditions
    {
        public DateTime date { get; set; }
        public string? Title { get; set; }
        public string? WeatherCondition { get; set; }
        public string? WeatherImgurl { get; set; }
        public string? Temperatures { get; set; }
        public string? Weatherdesc { get; set; }

        //NEW
        public string? Reliability { get; set; }
        public int TempMaxmax { get; set; }
        public int TempMaxmin { get; set; }
        public int TempMinmax { get; set; }
        public int TempMinmin { get; set; }
        public int bulletinStatus { get; set; }
    }

    public class Forecast
    {
        public DateTime date { get; set; }
        public int TempMaxmax { get; set; }
        public int TempMaxmin { get; set; }
        public int TempMinmax { get; set; }
        public int TempMinmin { get; set; }
        public string? Weatherdesc { get; set; }
        public string? Weathercode { get; set; }
        public string? WeatherImgurl { get; set; }
        public string? Reliability { get; set; }
    }

    public class Mountain
    {
        public DateTime date { get; set; }
        public string? Title { get; set; }
        public string? Conditions { get; set; }
        public string? Weatherdesc { get; set; }
        public string? Zerolimit { get; set; }
        public string? MountainImgurl { get; set; }
        public string? Reliability { get; set; }

        public string? Sunrise { get; set; }
        public string? Sunset { get; set; }
        public string? Moonrise { get; set; }
        public string? Moonset { get; set; }

        public string? Northcode { get; set; }
        public string? Northdesc { get; set; }
        public string? Northimgurl { get; set; }
        public string? Southcode { get; set; }
        public string? Southdesc { get; set; }
        public string? Southimgurl { get; set; }

        public int Temp1000 { get; set; }
        public int Temp2000 { get; set; }
        public int Temp3000 { get; set; }
        public int Temp4000 { get; set; }

        public string? Windcode { get; set; }
        public string? Winddesc { get; set; }
        public string? WindImgurl { get; set; }

        public List<string> Snowlimit { get; set; }
    }

    public class Stationdata
    {
        public DateTime date { get; set; }

        public int Id { get; set; }
        public string? CityName { get; set; }

        public string? WeatherCode { get; set; }
        public string? WeatherDesc { get; set; }
        public string? WeatherImgUrl { get; set; }

        //Compatibility
        public string? Weathercode { get { return this.WeatherCode; } }
        public string? Weatherdesc { get { return this.WeatherDesc; } }
        public string? Weatherimgurl { get { return this.WeatherImgUrl; } }

        public int MinTemp { get; set; }
        public int Maxtemp { get; set; }

        //Compatibility Reasons
        public int MaxTemp { get { return Maxtemp; } }
    }

    public class BezirksWeather
    {
        public BezirksWeather()
        {
            this.BezirksForecast = new HashSet<BezirksForecast>();
        }

        public int Id { get; set; }
        public string? DistrictName { get; set; }
        public DateTime date { get; set; }

        public List<string>? TourismVereinIds { get; set; }

        public ICollection<BezirksForecast> BezirksForecast { get; set; }

        public LicenseInfo? LicenseInfo { get; set; }
    }

    public class BezirksForecast
    {
        public DateTime date { get; set; }
        public string? WeatherCode { get; set; }
        public string? WeatherDesc { get; set; }
        public string? WeatherImgUrl { get; set; }

        //Compatibility
        public string? Weathercode { get { return this.WeatherCode; } }
        public string? Weatherdesc { get { return this.WeatherDesc; } }
        public string? Weatherimgurl { get { return this.WeatherImgUrl; } }


        public int MaxTemp { get; set; }
        public int MinTemp { get; set; }

        public int Freeze { get; set; }

        public int RainFrom { get; set; }
        public int RainTo { get; set; }

        public int Part1 { get; set; }
        public int Part2 { get; set; }
        public int Part3 { get; set; }
        public int Part4 { get; set; }

        public int Thunderstorm { get; set; }

        public int SymbolId { get; set; }
        
        public int? Reliability { get; set; }
    }

    public class WeatherRealTime
    {
        public double altitude { get; set; }
        [SwaggerSchema("Indicates whether the weather stations are: [1] in the valley, [2] gauge stations, [3] on the mountain")]
        public int categoryId { get; set; }
        public string? code { get; set; }
        public string? id { get; set; }
        [SwaggerSchema("Wind direction")]
        public string? dd { get; set; }
        [SwaggerSchema("Wind speed (km/h)")]
        public string? ff { get; set; }
        [SwaggerSchema("Snow depth (cm)")]
        public string? hs { get; set; }
        public DateTime lastUpdated { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        [SwaggerSchema("Only for weather stations on the mountain: [1] if the weather station is a snow measuring field, [2] if the weather station is a wind station, [null] otherwise")]
        public string? lwdType { get; set; }
        [SwaggerSchema("Precipitation from midnight (mm)")]
        public string? n { get; set; }
        public string? name { get; set; }
        [SwaggerSchema("Atmospheric pressure (hPa)")]
        public string? p { get; set; }
        [SwaggerSchema("Flow rate (m³/s)")]
        public string? q { get; set; }
        [SwaggerSchema("Relative humidity (rH)")]
        public string? rh { get; set; }
        [SwaggerSchema("Air temperature (°C)")]
        public string? t { get; set; }
        public string? vaxcode { get; set; }
        [SwaggerSchema("Water level (cm)")]
        public string? w { get; set; }
        [SwaggerSchema("Wind gust")]
        public string? wMax { get; set; }
        [SwaggerSchema("Sunshine duration (h)")]
        public string? sd { get; set; }
        [SwaggerSchema("Global radiation")]
        public string? gs { get; set; }
        [SwaggerSchema("Water temperature (°C)")]
        public string? wt { get; set; }
        public string? visibility { get; set; }
        public string? zoomLevel { get; set; }

        public ICollection<RealTimeMeasurements>? measurements { get; set; }
    }

    public class RealTimeMeasurements
    {
        public string? code { get; set; }
        public string? description { get; set; }
        public string? imageUrl { get; set; }
    }

    public class WeatherHistory : IIdentifiable, ILicenseInfo, IImportDateassigneable
    {
        public WeatherHistory()
        {
            Weather = new Dictionary<string, Weather>();
        }
        public IDictionary<string, Weather> Weather { get; set; }
        public LicenseInfo? LicenseInfo { get; set; }

        public List<string> HasLanguage { get; set; }
        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }

        public string? Shortname { get; set; }
        public string Id { get; set; }
    }

    #endregion   

    #region Packages

    public class Package : IIdentifiable, IActivateable, ISmgActive, ISmgTags, IImageGalleryAware, IHasLanguage, ISource
    {
        public LicenseInfo? LicenseInfo { get; set; }

        public Package()
        {
            PackageDetail = new Dictionary<string, PackageDetail>();
            Inclusive = new Dictionary<string, Inclusive>();
            ChannelInfo = new Dictionary<string, string>();
        }

        //IIdentifiable
        public string? Id { get; set; }

        //Infos zum Import
        public DateTime FirstImport { get; set; }
        public DateTime LastUpdate { get; set; }

        public bool Active { get; set; }
        public bool SmgActive { get; set; }
        public string? Shortname { get; set; }

        public ICollection<string>? SmgTags { get; set; }

        public DateTime ValidStart { get; set; }
        public DateTime ValidStop { get; set; }

        public string? OfferId { get; set; }

        public List<string>? HotelHgvId { get; set; }
        public List<string>? HotelId { get; set; }

        public int Offertyp { get; set; }
        public int Specialtyp { get; set; }

        public Nullable<int> Premiumtyp { get; set; }

        public int DaysArrival { get; set; }
        public int DaysDeparture { get; set; }

        public int DaysDurMin { get; set; }
        public int DaysDurMax { get; set; }

        public int DaysArrivalMin { get; set; }
        public int DaysArrivalMax { get; set; }

        public int ChildrenMin { get; set; }

        public bool ShortStay { get; set; }
        public bool LongStay { get; set; }

        public ICollection<ImageGallery>? ImageGallery { get; set; }
        public IDictionary<string, PackageDetail> PackageDetail { get; set; }

        public IDictionary<string, string> ChannelInfo { get; set; }

        public IDictionary<string, Inclusive> Inclusive { get; set; }
        public ICollection<PackageTheme>? PackageThemeDetail { get; set; }

        public ICollection<string>? PackageThemeList { get; set; }

        public ICollection<Season>? Season { get; set; }

        public List<string>? Services { get; set; }

        public ICollection<string>? HasLanguage { get; set; }

        //Location Geschichte
        public LocationInfo? LocationInfo { get; set; }
        public string? DistrictId { get; set; }

        //public string HGVLink
        public string? HgvLink { get; set; }

        //MSS Result
        public ICollection<MssResponseShort>? MssResponseShort { get; set; }

        public EvalancheMapping? EvalancheMapping { get; set; }

        public ICollection<string>? PublishedOn { get; set; }

        public string? Source { get; set; }
    }

    public class Season
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class PackageDetail : ILanguage
    {
        public string? Language { get; set; }
        public string? Title { get; set; }
        public string? Desc { get; set; }
    }

    public class Inclusive
    {
        public Inclusive()
        {
            PackageDetail = new Dictionary<string, PackageDetail>();
        }

        public int PriceId { get; set; }
        public int PriceTyp { get; set; }

        public IDictionary<string, PackageDetail> PackageDetail { get; set; }
        public ICollection<ImageGallery>? ImageGallery { get; set; }
    }    

    public class PackageTheme
    {
        public PackageTheme()
        {
            ThemeDetail = new Dictionary<string, ThemeDetail>();
        }

        public int ThemeId { get; set; }
        public IDictionary<string, ThemeDetail> ThemeDetail { get; set; }
    }    

    public class ThemeDetail : ILanguage
    {
        public string? Title { get; set; }
        public string? Language { get; set; }
    }

    #endregion

    #region Measuringpoints

    public class Measuringpoint : IIdentifiable, IActivateable, ISmgActive, IGpsInfo, ILicenseInfo, IImportDateassigneable, ISource, IMappingAware, IDistanceInfoAware, IPublishedOn, IGPSPointsAware
    {
        public Measuringpoint()
        {
            Mapping = new Dictionary<string, IDictionary<string, string>>();
        }

        public LicenseInfo? LicenseInfo { get; set; }

        public string? Id { get; set; }

        public DateTime? FirstImport { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime? LastChange { get; set; }

        public bool Active { get; set; }
        public bool SmgActive { get; set; }
        public string? Shortname { get; set; }

        //GPS
        public string? Gpstype { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Nullable<double> Altitude { get; set; }
        public string? AltitudeUnitofMeasure { get; set; }
        public DistanceInfo? DistanceInfo { get; set; }


        //Observation
        public string? SnowHeight { get; set; }
        public string? newSnowHeight { get; set; }
        public string? Temperature { get; set; }
        public DateTime LastSnowDate { get; set; }
        public List<WeatherObservation>? WeatherObservation { get; set; }

        //Location
        public LocationInfo? LocationInfo { get; set; }
        public string? OwnerId { get; set; }

        public List<string>? AreaIds { get; set; }

        [SwaggerSchema(Description = "generated field", ReadOnly = true)]
        public IDictionary<string, GpsInfo> GpsPoints
        {
            get
            {
                if (this.Latitude != 0 && this.Longitude != 0)
                {
                    return new Dictionary<string, GpsInfo>
                    {
                        { "position", new GpsInfo(){ Gpstype = "position", Altitude = this.Altitude, AltitudeUnitofMeasure = this.AltitudeUnitofMeasure, Latitude = this.Latitude, Longitude = this.Longitude } }
                    };
                }
                else
                {
                    return new Dictionary<string, GpsInfo>
                    {
                    };
                }
            }
        }

        public ICollection<string>? PublishedOn { get; set; }

        public string? Source { get; set; }

        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }


        public IEnumerable<string>? SkiAreaIds { get; set; }
    }

    public class WeatherObservation
    {
        public WeatherObservation()
        {
            WeatherStatus = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        public string? Level { get; set; }
        public string? LevelId { get; set; }
        public Dictionary<string, string> WeatherStatus { get; set; }
        //New
        public string? IconID { get; set; }
        public DateTime? Date { get; set; }
    }

    //SnowReport Base Data
    public class SnowReportBaseData
    {
        public SnowReportBaseData()
        {

        }

        public string? Id { get; set; }
        public string? RID { get; set; }
        public string? Skiregion { get; set; }
        public string? Areaname { get; set; }
        public DateTime LastUpdate { get; set; }
        public string? lang { get; set; }

        public string? SkiAreaSlopeKm { get; set; }
        public string? SkiMapUrl { get; set; }

        //Snow Data
        public ICollection<MeasuringpointReduced>? Measuringpoints { get; set; }
        public ICollection<string>? WebcamUrl { get; set; }

        //Webcam
        //public ICollection<Webcam> Webcams { get; set; }

        //public string snowheight { get; set; }
        //public string newsnowheigtht { get; set; }
        //public string lastsnow { get; set; }
        //public string weather { get; set; }
        //public string weatherimg { get; set; }
        //public string weathercode { get; set; }
        //public string observationtemperature { get; set; }

        //Slopes Lifts        
        public string? totalskilift { get; set; }
        public string? openskilift { get; set; }
        public string? totalskiliftkm { get; set; }
        public string? openskiliftkm { get; set; }
        public string? totalskislopes { get; set; }
        public string? openskislopes { get; set; }
        public string? totalskislopeskm { get; set; }
        public string? openskislopeskm { get; set; }
        public string? totaltracks { get; set; }
        public string? opentracks { get; set; }
        public string? totaltrackskm { get; set; }
        public string? opentrackskm { get; set; }
        public string? totalslides { get; set; }
        public string? opentslides { get; set; }
        public string? totalslideskm { get; set; }
        public string? opentslideskm { get; set; }
        public string? totaliceskating { get; set; }
        public string? openiceskating { get; set; }



        //Contact
        public string? contactadress { get; set; }
        public string? contacttel { get; set; }
        public string? contactcap { get; set; }
        public string? contactcity { get; set; }
        public string? contactfax { get; set; }
        public string? contactweburl { get; set; }
        public string? contactmail { get; set; }
        public string? contactlogo { get; set; }

        //Gps
        public string? contactgpsnorth { get; set; }
        public string? contactgpseast { get; set; }



        //public virtual ICollection<Slopes> Slopes { get; set; }
        //public virtual ICollection<Tracks> Tracks { get; set; }
        //public virtual ICollection<Lifts> Lifts { get; set; }
        //public virtual ICollection<Slides> Slides { get; set; }
        //public virtual ICollection<Iceskating> Iceskating { get; set; }
        //public virtual ICollection<Measuringpoints> Measuringpoints { get; set; }

        //public virtual ICollection<IActivity> IActivity { get; set; }
    }

    //Reduced Measuringpoint for Snow Report
    public class MeasuringpointReduced : ISource
    {
        public string? Id { get; set; }

        public DateTime LastUpdate { get; set; }
        public string? Shortname { get; set; }
        public string? SnowHeight { get; set; }
        public string? newSnowHeight { get; set; }
        public string? Temperature { get; set; }
        public DateTime LastSnowDate { get; set; }
        public List<WeatherObservation>? WeatherObservation { get; set; }

        public string? Source { get; set; }

        ////GPS
        //public string Gpstype { get; set; }
        //public double Latitude { get; set; }
        //public double Longitude { get; set; }
        //public Nullable<double> Altitude { get; set; }
        //public string AltitudeUnitofMeasure { get; set; }

        //Observation

        ////Location Geschichte
        //public LocationInfo LocationInfo { get; set; }
        //public string OwnerId { get; set; }

        //public List<string> AreaIds { get; set; }
    }

    #endregion

    #region EventShort

    public class EventShort : IIdentifiable, IImportDateassigneable, ISource, IMappingAware, ILicenseInfo, IPublishedOn, IGPSPointsAware
    {
        public EventShort()
        {
            Mapping = new Dictionary<string, IDictionary<string, string>>();
            EventText = new Dictionary<string, string>();
            EventTitle = new Dictionary<string, string>();
            Documents = new Dictionary<string, List<Document>?>();
        }

        public LicenseInfo? LicenseInfo { get; set; }

        public string? Id { get; set; }
        public string? Source { get; set; }

        [SwaggerEnum(new[] { "NOI", "EC" })]
        public string? EventLocation { get; set; }
        public int EventId { get; set; }

        //Dictionary with EventTextDE + EventDescriptionDE infos
        public IDictionary<string, string> EventText { get; set; }
        public IDictionary<string, string> EventTitle { get; set; }

        [SwaggerDeprecated("Deprecated, use EventText")]
        public string? EventTextDE {
            get
            {
                return EventText.ContainsKey("de") ? EventText["de"] : null;
            }
        }

        [SwaggerDeprecated("Deprecated, use EventText")]
        public string? EventTextIT {
            get
            {
                return EventText.ContainsKey("it") ? EventText["it"] : null;
            }
        }
        [SwaggerDeprecated("Deprecated, use EventText")]
        public string? EventTextEN {
            get
            {
                return EventText.ContainsKey("en") ? EventText["en"] : null;
            }
        }
     
        //Hauptbeschreibung
        [SwaggerDeprecated("Deprecated, use EventTitle")]
        public string? EventDescription { get; set; }
        //Beschreibung DE
        [SwaggerDeprecated("Deprecated, use EventTitle")] 
        public string? EventDescriptionDE {
            get
            {
                return EventTitle.ContainsKey("de") ? EventTitle["de"] : "";
            }
        }
        //Beschreibung IT
        [SwaggerDeprecated("Deprecated, use EventTitle")] 
        public string? EventDescriptionIT {
            get
            {
                return EventTitle.ContainsKey("it") ? EventTitle["it"] : "";
            }
        }
        //Beschreibung EN
        [SwaggerDeprecated("Deprecated, use EventTitle")] 
        public string? EventDescriptionEN {
            get
            {
                return EventTitle.ContainsKey("en") ? EventTitle["en"] : "";
            }
        }
        //Hauptsaal/ort
        public string? AnchorVenue { get; set; }
        //Hauptsaal/ort soll für die Ausgabe verwendet werden
        public string? AnchorVenueShort { get; set; }
        //letzte Änderung
        public DateTime ChangedOn { get; set; }
        //Beginndatum
        public DateTime StartDate { get; set; }
        //Beginnzeit
        //public string StartTime { get; set; }
        ////Ende Datum
        public DateTime EndDate { get; set; }
        //Endzeit
        //public string EndTime { get; set; }

        public double StartDateUTC { get; set; }
        public double EndDateUTC { get; set; }

        //URL für externe Webseite (noch nicht ausgefüllt)
        public string? WebAddress { get; set; }
        //Spezialfelder

        [SwaggerDeprecated("Deprecated")]
        [RegularExpression("Y|N", ErrorMessage = "Only Y and N allowed")]
        [SwaggerEnum(new[] { "Y", "N" })]
        [SwaggerSchema("Active")]
        public string? Display1 { get; set; }

        [SwaggerDeprecated("Deprecated")]
        [SwaggerEnum(new[] { "Y", "N" })]
        [SwaggerSchema("Intranet Eurac")]
        public string? Display2 { get; set; }

        [SwaggerDeprecated("Deprecated")]
        [SwaggerEnum(new[] { "Y", "N" })]
        [SwaggerSchema("Website Eurac")]
        public string? Display3 { get; set; }

        [SwaggerDeprecated("Deprecated")]
        [SwaggerEnum(new[] { "Y", "N" })]
        public string? Display4 { get; set; }

        [SwaggerDeprecated("Deprecated")]
        [SwaggerEnum(new[] { "Y", "N" })]
        public string? Display5 { get; set; }

        [SwaggerDeprecated("Deprecated")]
        [SwaggerEnum(new[] { "Y", "N" })]
        public string? Display6 { get; set; }

        [SwaggerDeprecated("Deprecated")]
        [SwaggerEnum(new[] { "Y", "N" })]
        public string? Display7 { get; set; }

        [SwaggerDeprecated("Deprecated")]
        [SwaggerEnum(new[] { "Y", "N" })]
        public string? Display8 { get; set; }

        [SwaggerDeprecated("Deprecated")]
        [SwaggerEnum(new[] { "Y", "N" })]
        public string? Display9 { get; set; }

        public string? CompanyName { get; set; }
        public string? CompanyId { get; set; }
        public string? CompanyAddressLine1 { get; set; }
        public string? CompanyAddressLine2 { get; set; }
        public string? CompanyAddressLine3 { get; set; }
        public string? CompanyPostalCode { get; set; }
        public string? CompanyCity { get; set; }
        public string? CompanyCountry { get; set; }
        public string? CompanyPhone { get; set; }
        public string? CompanyFax { get; set; }
        public string? CompanyMail { get; set; }
        public string? CompanyUrl { get; set; }

        //Person aus Modul CRM (interessiert uns nicht)
        public string? ContactCode { get; set; }
        public string? ContactFirstName { get; set; }
        public string? ContactLastName { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactCell { get; set; }
        public string? ContactFax { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactAddressLine1 { get; set; }
        public string? ContactAddressLine2 { get; set; }
        public string? ContactAddressLine3 { get; set; }
        public string? ContactPostalCode { get; set; }
        public string? ContactCity { get; set; }
        public string? ContactCountry { get; set; }

        //gebuchten Sääle von spezifischen Event
        //Space : Code für Raum von DB
        //SpaceDesc: Beschreibung --> zu nehmen
        //SpaceAbbrev: Abgekürzte Beschreibung 
        //SoaceType : EC = Eurac, NO = Noi
        //Comnment: entweder x oder leer --> x bedeutet bitte nicht anzeigen!!!!!!!
        //Subtitle: Untertitel vom Saal (anzeigen)
        //Zeiten (diese sind relevant, diese anzeigen)
        public List<RoomBooked>? RoomBooked { get; set; }

        public List<ImageGallery>? ImageGallery { get; set; }
        public string? VideoUrl { get; set; }
        public List<string>? TechnologyFields { get; set; }

        public List<string>? CustomTagging { get; set; }

        public List<DocumentPDF>? EventDocument {
            get
            {
                if (this.Documents != null && this.Documents.Count > 0)
                {
                    return this.Documents.SelectMany(x => x.Value).Select(y => new DocumentPDF() { DocumentURL = y.DocumentURL, Language = y.Language }).ToList();
                }
                else
                    return null;
            }
            //get; set; 
        }

        public IDictionary<string, List<Document>?> Documents { get; set; }


        public bool? ExternalOrganizer { get; set; }

        public string? Shortname { get; set; }

        public ICollection<string>? PublishedOn { get; set; }

        public string? AnchorVenueRoomMapping
        {
            get
            {
                return !String.IsNullOrEmpty(this.AnchorVenue) ? (this.AnchorVenue.StartsWith("NOI ") || this.AnchorVenue.StartsWith("Noi ") || this.AnchorVenue.StartsWith("noi ")) ? this.AnchorVenue.Remove(0, 3).Trim() : this.AnchorVenue : this.AnchorVenue;
            }
        }

        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }

        public bool? SoldOut { get; set; }

        [SwaggerDeprecated("Deprecated, use PublishedOn: today.noi.bz.it")]
        [SwaggerSchema(" ActiveToday Indicates if Event is shown on the today NOI Website")]
        public bool? ActiveToday {
            get
            {
                if (this.PublishedOn != null && this.PublishedOn.Count > 0)
                {
                    return this.PublishedOn.Contains("today.noi.bz.it");
                }
                else
                {
                    return false;
                }
            }
        }

        [SwaggerDeprecated("Deprecated, use PublishedOn: noi.bz.it")]
        [SwaggerSchema(" ActiveWeb Indicates if Event is shown on the Noi Website Section Events at NOI")]
        public bool? ActiveWeb {
            get
            {
                if (this.PublishedOn != null && this.PublishedOn.Count > 0)
                {
                    return this.PublishedOn.Contains("noi.bz.it");
                }
                else
                {
                    return false;
                }
            }
        }

        [SwaggerDeprecated("Deprecated, use PublishedOn: noi-communityapp")]
        [SwaggerSchema("ActiveCommunityApp Indicates if Event is shown on the Noi Community App")]
        public bool? ActiveCommunityApp {
            get
            {
                if (this.PublishedOn != null && this.PublishedOn.Count > 0)
                {
                    return this.PublishedOn.Contains("noi-communityapp");
                }
                else
                {
                    return false;
                }
            }
        }

        public ICollection<string>? HasLanguage { get; set; }

        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }

        public ICollection<GpsInfo>? GpsInfo { get; set; }

        [SwaggerSchema(Description = "generated field", ReadOnly = true)]
        public IDictionary<string, GpsInfo> GpsPoints
        {
            get
            {
                if (this.GpsInfo != null && this.GpsInfo.Count > 0)
                {
                    return this.GpsInfo.ToDictionary(x => x.Gpstype, x => x);
                }
                else
                {
                    return new Dictionary<string, GpsInfo>
                    {
                    };
                }
            }
        }

        public AgeRange? TypicalAgeRange { get; set; }
    }

    public class RoomBooked
    {
        public string? Space { get; set; }
        public string? SpaceDesc { get; set; }
        public string? SpaceAbbrev { get; set; }
        public string? SpaceType { get; set; }
        public string? Subtitle { get; set; }
        public string? Comment { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public double StartDateUTC { get; set; }
        public double EndDateUTC { get; set; }

        public string? SpaceDescRoomMapping
        {
            get
            {
                return !String.IsNullOrEmpty(this.SpaceDesc) ? (this.SpaceDesc.StartsWith("NOI ") || this.SpaceDesc.StartsWith("Noi ") || this.SpaceDesc.StartsWith("noi ")) ? this.SpaceDesc.Remove(0, 3).Trim() : this.SpaceDesc : this.SpaceDesc;
            }
        }
    }

    public class EventShortByRoom
    {
        public EventShortByRoom()
        {
            SpaceDescList = new List<string>();
            TechnologyFields = new List<string>();
            CustomTagging = new List<string>();
            EventTitle = new Dictionary<string, string>();
            EventText = new Dictionary<string, string>();
            EventDescription = new Dictionary<string, string>();
            EventDocument = new Dictionary<string, string>();
        }

        //Room Infos

        public List<string> SpaceDescList { get; set; }

        public string? SpaceDesc { get; set; }
        public string? SpaceType { get; set; }
        public string? Subtitle { get; set; }

        public DateTime RoomStartDate { get; set; }
        public DateTime RoomEndDate { get; set; }

        public double RoomStartDateUTC { get; set; }
        public double RoomEndDateUTC { get; set; }

        //Event Infos

        public int EventId { get; set; }

        public Dictionary<string, string> EventTitle { get; set; }

        public Dictionary<string, string> EventText { get; set; }

        [SwaggerDeprecated("Deprecated, use EventTitle")]
        public Dictionary<string, string> EventDescription { get; set; }

        [SwaggerDeprecated("Deprecated, use EventTitle")]
        public string? EventDescriptionDE { get; set; }
     
        [SwaggerDeprecated("Deprecated, use EventTitle")]
        public string? EventDescriptionIT { get; set; }
        
        [SwaggerDeprecated("Deprecated, use EventTitle")]
        public string? EventDescriptionEN { get; set; }

        public string? EventAnchorVenue { get; set; }
        public string? EventAnchorVenueShort { get; set; }

        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public double EventStartDateUTC { get; set; }
        public double EventEndDateUTC { get; set; }

        public string? EventWebAddress { get; set; }
        public string? Id { get; set; }
        public string? EventSource { get; set; }
        public string? EventLocation { get; set; }

        public string? CompanyName { get; set; }
        public List<ImageGallery>? ImageGallery { get; set; }
        public string? VideoUrl { get; set; }
        public Nullable<bool> ActiveWeb { get; set; }

        [SwaggerDeprecated("Deprecated, use EventText")]
        public string? EventTextDE { get; set; }
        [SwaggerDeprecated("Deprecated, use EventText")]
        public string? EventTextIT { get; set; }
        [SwaggerDeprecated("Deprecated, use EventText")]
        public string? EventTextEN { get; set; }

        public List<string>? TechnologyFields { get; set; }
        public List<string>? CustomTagging { get; set; }
        public bool? SoldOut { get; set; }

        public Dictionary<string, string> EventDocument { get; set; }

        public bool? ExternalOrganizer { get; set; }

        public ICollection<string>? PublishedOn { get; set; }
    }

    public class DocumentPDF
    {
        public string? DocumentURL { get; set; }
        public string? Language { get; set; }
    }

    public class Document
    {
        public string? DocumentName { get; set; }
        public string? DocumentURL { get; set; }
        public string? Language { get; set; }
    }

    public class AgeRange
    {
        public int AgeFrom { get; set; }
        public int AgeTo { get; set; }
    }

    #endregion

    #region Wine

    public class Wine : IIdentifiable, IImportDateassigneable, ILicenseInfo, ISource, IMappingAware, IPublishedOn, IActivateable, ISmgActive
    {
        public LicenseInfo? LicenseInfo { get; set; }

        public Wine()
        {
            Detail = new Dictionary<string, Detail>();
            Mapping = new Dictionary<string, IDictionary<string, string>>();
        }

        public string? Id { get; set; }

        public string? Shortname { get; set; }

        public IDictionary<string, Detail> Detail { get; set; }

        //public Dictionary<string, string> Title { get; set; }
        //public Dictionary<string, string> WineName { get; set; }        

        public int Vintage { get; set; }
        public int Awardyear { get; set; }

        [SwaggerSchema("Id on the primary data Source")]
        public string? CustomId { get; set; }
        public string? CompanyId { get; set; }

        public ICollection<ImageGallery>? ImageGallery { get; set; }
        public ICollection<string>? Awards { get; set; }

        public DateTime? LastChange { get; set; }
        public DateTime? FirstImport { get; set; }

        public bool Active { get; set; }
        public bool SmgActive { get; set; }

        public ICollection<string>? HasLanguage { get; set; }

        public string? Source { get; set; }

        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }

        public ICollection<string> PublishedOn { get; set; }
    }


    #endregion

    #region TypeInfos

    public class GastronomyTypes
    {
        public GastronomyTypes()
        {
            TypeDesc = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        public int Bitmask { get; set; }

        public string? Type { get; set; }

        public string? Key { get; set; }

        public Dictionary<string, string> TypeDesc { get; set; }
    }

    //For Types Api
    public class AccoTypes
    {
        public AccoTypes()
        {
            TypeDesc = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        public int Bitmask { get; set; }

        public string? Type { get; set; }

        public string? Key { get; set; }

        public Dictionary<string, string> TypeDesc { get; set; }

        public string? CustomId { get; set; }
    }

    //For Types Api
    public class AccoFeatures : AccoTypes
    {

    }

    //For Types Api 
    public class SmgPoiTypes
    {
        public SmgPoiTypes()
        {
            TypeDesc = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        public long Bitmask { get; set; }
        public string? Type { get; set; }
        public string? Parent { get; set; }
        public string Key { get; set; }

        public IDictionary<string, string>? TypeDesc { get; set; }
    }

    //For Types Api 
    public class EventTypes
    {
        public EventTypes()
        {
            TypeDesc = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        public int Bitmask { get; set; }
        public string? Type { get; set; }
        public Dictionary<string, string> TypeDesc { get; set; }
    }

    //For Types Api 
    public class ActivityTypes : SmgPoiTypes
    {

    }

    //For Types Api 
    public class ArticleTypes : SmgPoiTypes
    {

    }

    //For Types Api 
    public class PoiTypes : SmgPoiTypes
    {

    }

    //For Types Api 
    public class ODHActivityPoiTypes : SmgPoiTypes
    {
    }

    #endregion

    #region CommonInfos

    public class Metadata
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string? Source { get; set; }
        public bool Reduced { get; set; }

        public UpdateInfo? UpdateInfo { get; set; }
    }

    public class UpdateInfo
    {
        public string? UpdatedBy { get; set; }

        public string? UpdateSource { get; set; }
    }

    public class LicenseInfo
    {
        public string? License { get; set; }
        public string? LicenseHolder { get; set; }
        public string? Author { get; set; }
        public bool ClosedData { get; set; }
    }

    public class LTSTaggingType
    {
        public LTSTaggingType()
        {
            TypeNames = new Dictionary<string, string>();
            TypeDescriptions = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        public string? Key { get; set; }
        public string? Entity { get; set; }
        public string? TypeParent { get; set; }
        public int Level { get; set; }
        public IDictionary<string, string> TypeNames { get; set; }
        public IDictionary<string, string> TypeDescriptions { get; set; }
    }

    //BaseInfos for Districts / Regions / Municipalities ...
    public abstract class BaseInfos : IIdentifiable, IActivateable, IGpsInfo, ISmgTags, ISmgActive, IHasLanguage, IImportDateassigneable, ILicenseInfo, IDetailInfosAware, IContactInfosAware, ISource, IMappingAware, IDistanceInfoAware, IGPSPointsAware, IPublishedOn
    {
        public LicenseInfo? LicenseInfo { get; set; }

        public BaseInfos()
        {
            Detail = new Dictionary<string, Detail>();
            ContactInfos = new Dictionary<string, ContactInfos>();
            //Mapping New
            Mapping = new Dictionary<string, IDictionary<string, string>>();
        }

        public string Id { get; set; }
        public bool Active { get; set; }
        public string? CustomId { get; set; }
        public string? Shortname { get; set; }

        [SwaggerDeprecated("Deprecated, use GpsPoints")]
        public string? Gpstype { get; set; }
        [SwaggerDeprecated("Deprecated, use GpsPoints")]
        public double Latitude { get; set; }
        [SwaggerDeprecated("Deprecated, use GpsPoints")]
        public double Longitude { get; set; }
        [SwaggerDeprecated("Deprecated, use GpsPoints")]
        public Nullable<double> Altitude { get; set; }
        [SwaggerDeprecated("Deprecated, use GpsPoints")]
        public string? AltitudeUnitofMeasure { get; set; }

        public IDictionary<string, Detail> Detail { get; set; }
        public IDictionary<string, ContactInfos> ContactInfos { get; set; }
        public ICollection<ImageGallery>? ImageGallery { get; set; }

        public ICollection<string>? SmgTags { get; set; }

        //public DateTime FirstImport { get; set; }
        //public DateTime LastChange { get; set; }

        public bool SmgActive { get; set; }

        public ICollection<string>? HasLanguage { get; set; }

        public DateTime? LastChange { get; set; }
        public DateTime? FirstImport { get; set; }

        [SwaggerSchema(Description = "generated field", ReadOnly = true)]
        public IDictionary<string, GpsInfo> GpsPoints
        {
            get
            {
                if (this.Latitude != 0 && this.Longitude != 0)
                {
                    return new Dictionary<string, GpsInfo>
                    {
                        { "position", new GpsInfo(){ Gpstype = "position", Altitude = this.Altitude, AltitudeUnitofMeasure = this.AltitudeUnitofMeasure, Latitude = this.Latitude, Longitude = this.Longitude } }
                    };
                }
                else
                {
                    return new Dictionary<string, GpsInfo>
                    {
                    };
                }
            }
        }

        //New published on List
        public ICollection<string>? PublishedOn { get; set; }

        public string? Source { get; set; }

        //New Mapping
        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }

        public DistanceInfo? DistanceInfo { get; set; }

    }

    //BaseInfos for ODHActivityPois
    public class PoiBaseInfos : IIdentifiable, IActivateable, IGeoDataInfoAware, IActivityStatus, IImageGalleryAware, IContactInfosAware, IAdditionalPoiInfosAware, ISmgTags, ISmgActive, IHasLanguage, IImportDateassigneable, ILicenseInfo, IDetailInfosAware, ISource, IMappingAware, IDistanceInfoAware, IGPSInfoAware, IPublishedOn, IVideoItemsAware
    {
        public LicenseInfo? LicenseInfo { get; set; }

        public PoiBaseInfos()
        {
            Detail = new Dictionary<string, Detail>();
            ContactInfos = new Dictionary<string, ContactInfos>();
            AdditionalPoiInfos = new Dictionary<string, AdditionalPoiInfos>();
            Mapping = new Dictionary<string, IDictionary<string, string>>();
            Tags = new List<Tags>();
            VideoItems = new Dictionary<string, ICollection<VideoItems>>();
        }

        public string? Id { get; set; }

        public string? OutdooractiveID { get; set; }
        public string? OutdooractiveElevationID { get; set; }

        public bool? CopyrightChecked { get; set; }

        public bool Active { get; set; }
        public string? Shortname { get; set; }
        public string? SmgId { get; set; }
        public bool? Highlight { get; set; }

        [SwaggerDeprecated("Use Ratings.Difficulty")]
        public string? Difficulty { get; set; }

        [SwaggerDeprecated("Use AdditionalPoiInfos.Categories instead")]
        public string? Type { get; set; }
        [SwaggerDeprecated("Use AdditionalPoiInfos.Categories instead")]
        public string? SubType { get; set; }
        [SwaggerDeprecated("Use AdditionalPoiInfos.Categories instead")]
        public string? PoiType { get; set; }

        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }
        public bool SmgActive { get; set; }

        public LocationInfo? LocationInfo { get; set; }

        public string? TourismorganizationId { get; set; }

        [SwaggerDeprecated("Deprecated use AreaIds")]
        public ICollection<string>? AreaId { get; set; }
        
        [GetOnlyJsonProperty]
        public ICollection<string>? AreaIds { get { return AreaId; } }

        public double? AltitudeDifference { get; set; }
        public double? AltitudeHighestPoint { get; set; }
        public double? AltitudeLowestPoint { get; set; }
        public double? AltitudeSumUp { get; set; }
        public double? AltitudeSumDown { get; set; }

        public double? DistanceDuration { get; set; }
        public double? DistanceLength { get; set; }


        public bool? IsOpen { get; set; }
        public bool? IsPrepared { get; set; }
        public bool? RunToValley { get; set; }
        public bool? IsWithLigth { get; set; }
        public bool? HasRentals { get; set; }
        public bool? HasFreeEntrance { get; set; }
        public bool? LiftAvailable { get; set; }
        public bool? FeetClimb { get; set; }

        public bool? BikeTransport { get; set; }

        public ICollection<OperationSchedule>? OperationSchedule { get; set; }

        public ICollection<GpsInfo>? GpsInfo { get; set; }
        public ICollection<GpsTrack>? GpsTrack { get; set; }

        public ICollection<ImageGallery>? ImageGallery { get; set; }
        public IDictionary<string, Detail> Detail { get; set; }
        public IDictionary<string, ContactInfos> ContactInfos { get; set; }
        public IDictionary<string, AdditionalPoiInfos> AdditionalPoiInfos { get; set; }

        public ICollection<string>? SmgTags { get; set; }
        public ICollection<string>? HasLanguage { get; set; }

        public Ratings? Ratings { get; set; }
        public ICollection<string>? Exposition { get; set; }

        public string? OwnerRid { get; set; }

        public List<string>? ChildPoiIds { get; set; }
        public List<string>? MasterPoiIds { get; set; }

        public int? WayNumber { get; set; }

        public string? Number { get; set; }

        public List<LTSTags>? LTSTags { get; set; }

        public ICollection<string>? PublishedOn { get; set; }

        public string? Source { get; set; }

        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }

        public DistanceInfo? DistanceInfo { get; set; }

        public List<Tags> Tags { get; set; }

        public IDictionary<string, ICollection<VideoItems>>? VideoItems { get; set; }
    }

    //BaseInfo Article
    public abstract class ArticleBaseInfos : IIdentifiable, IActivateable, IImageGalleryAware, IContactInfosAware, IAdditionalArticleInfosAware, ISmgTags, ISmgActive, IImportDateassigneable, ILicenseInfo, IDetailInfosAware, ISource, IMappingAware, IGPSInfoAware, IDistanceInfoAware, IPublishedOn, IGPSPointsAware
    {
        public LicenseInfo? LicenseInfo { get; set; }

        public ArticleBaseInfos()
        {
            Detail = new Dictionary<string, Detail>();
            ContactInfos = new Dictionary<string, ContactInfos>();
            AdditionalArticleInfos = new Dictionary<string, AdditionalArticleInfos>();
            ArticleLinkInfo = new Dictionary<string, ArticleLinkInfo>();
            //Mapping New
            Mapping = new Dictionary<string, IDictionary<string, string>>();
        }

        public string? Id { get; set; }
        public bool Active { get; set; }
        public string? Shortname { get; set; }
        public bool Highlight { get; set; }

        //Activity SubType
        public string? Type { get; set; }
        public string? SubType { get; set; }
        //für BaseArticle
        //public string SubType2 { get; set; }

        //NEU SMG Infos
        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }
        public bool SmgActive { get; set; }

        public DateTime? ArticleDate { get; set; }

        //Mochmer des?
        public DateTime? ArticleDateTo { get; set; }

        //OperationSchedule
        //public string OperationscheduleName { get; set; }
        //public DateTime Start { get; set; }
        //public DateTime Stop { get; set; }
        //public bool? ClosedonPublicHolidays { get; set; }
        //public ICollection<OperationScheduleTime> OperationScheduleTime { get; set; }
        //Wenn mearere sein aso:
        public ICollection<OperationSchedule>? OperationSchedule { get; set; }

        public ICollection<GpsInfo>? GpsInfo { get; set; }
        public ICollection<GpsTrack>? GpsTrack { get; set; }

        public ICollection<ImageGallery>? ImageGallery { get; set; }
        public IDictionary<string, Detail> Detail { get; set; }
        public IDictionary<string, ContactInfos> ContactInfos { get; set; }
        public IDictionary<string, AdditionalArticleInfos> AdditionalArticleInfos { get; set; }

        //NEW Link Info
        public IDictionary<string, ArticleLinkInfo> ArticleLinkInfo { get; set; }

        public ICollection<string>? SmgTags { get; set; }

        public ICollection<string>? HasLanguage { get; set; }

        //public string CustomId { get; set; }

        //New Article Expiration date
        public DateTime? ExpirationDate { get; set; }

        //NEW Adding SpatialCoverage
        //public ICollection<SpatialCoverage> SpatialCoverage { get; set; }

        [SwaggerSchema(Description = "generated field", ReadOnly = true)]
        public IDictionary<string, GpsInfo> GpsPoints
        {
            get
            {
                if (this.GpsInfo != null && this.GpsInfo.Count > 0)
                {
                    return this.GpsInfo.ToDictionary(x => x.Gpstype, x => x);
                }
                else
                {
                    return new Dictionary<string, GpsInfo>
                    {
                    };
                }
            }
        }

        public ICollection<string>? PublishedOn { get; set; }

        public string? Source { get; set; }

        //New Mapping
        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }

        public DistanceInfo? DistanceInfo { get; set; }
    }

    //public class SpatialCoverage
    //{
    //    public IDictionary<string, string> Name { get; set; }
    //    public GpsInfo GpsInfo { get; set; }
    //}

    //BaseInfo Gastronomy
    public abstract class GastronomyBaseInfos : IIdentifiable, IActivateable, IGpsInfo, IImageGalleryAware, IContactInfosAware, ISmgTags, ISmgActive, IImportDateassigneable, IDetailInfosAware, ISource, IMappingAware, IDistanceInfoAware, ILicenseInfo
    {
        public LicenseInfo? LicenseInfo { get; set; }

        public GastronomyBaseInfos()
        {
            Detail = new Dictionary<string, Detail>();
            ContactInfos = new Dictionary<string, ContactInfos>();
            //Mapping New
            Mapping = new Dictionary<string, IDictionary<string, string>>();
        }

        public string? Id { get; set; }
        public bool Active { get; set; }
        public string? Shortname { get; set; }

        public string? Type { get; set; }

        //Region Fraktion 
        public string? DistrictId { get; set; }
        //public string MunicipalityId { get; set; }
        //public string RegionId { get; set; }       
        //public string TourismorganizationId { get; set; }        

        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }

        //GPS Info
        public string? Gpstype { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Nullable<double> Altitude { get; set; }
        public string? AltitudeUnitofMeasure { get; set; }

        //OperationSchedule
        //public string OperationscheduleName { get; set; }
        //public DateTime Start { get; set; }
        //public DateTime Stop { get; set; }
        //public bool? ClosedonPublicHolidays { get; set; }
        //public ICollection<OperationScheduleTime> OperationScheduleTime { get; set; }
        //Wenn mearere sein aso
        public ICollection<OperationSchedule>? OperationSchedule { get; set; }


        //CapacityCeremony
        public int? MaxSeatingCapacity { get; set; }

        //public ICollection<GpsInfo> GpsInfo { get; set; }
        public ICollection<ImageGallery>? ImageGallery { get; set; }
        public IDictionary<string, Detail> Detail { get; set; }
        public IDictionary<string, ContactInfos> ContactInfos { get; set; }

        public ICollection<CategoryCodes>? CategoryCodes { get; set; }
        public ICollection<DishRates>? DishRates { get; set; }
        public ICollection<CapacityCeremony>? CapacityCeremony { get; set; }
        public ICollection<Facilities>? Facilities { get; set; }

        public ICollection<string>? MarketinggroupId { get; set; }

        //NEU Region TV Municipality Fraktion NEU LocationInfo Classe
        public LocationInfo? LocationInfo { get; set; }

        public string? AccommodationId { get; set; }

        public ICollection<string>? SmgTags { get; set; }
        public bool SmgActive { get; set; }

        public ICollection<string>? HasLanguage { get; set; }

        //NEW
        public Nullable<int> RepresentationRestriction { get; set; }

        //New published on List
        public List<string>? PublishedOn { get; set; }

        public string? Source { get; set; }

        //New Mapping
        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }

        public DistanceInfo? DistanceInfo { get; set; }
    }

    //BaseInfo Events
    public abstract class EventBaseInfos : IIdentifiable, IActivateable, IImageGalleryAware, IGpsInfo, IContactInfosAware, ISmgTags, ISmgActive, IImportDateassigneable, IDetailInfosAware, ISource, IMappingAware, IDistanceInfoAware, ILicenseInfo, IPublishedOn, IGPSPointsAware
    {
        public LicenseInfo? LicenseInfo { get; set; }

        public EventBaseInfos()
        {
            Detail = new Dictionary<string, Detail>();
            ContactInfos = new Dictionary<string, ContactInfos>();
            OrganizerInfos = new Dictionary<string, ContactInfos>();
            EventAdditionalInfos = new Dictionary<string, EventAdditionalInfos>();
            EventPrice = new Dictionary<string, EventPrice>();
            EventPrices = new Dictionary<string, ICollection<EventPrice>>();
            //EventVariants = new Dictionary<string, ICollection<EventVariant>>();
            Hashtag = new Dictionary<string, ICollection<string>>();
            EventDescAdditional = new Dictionary<string, EventDescAdditional>();
            Mapping = new Dictionary<string, IDictionary<string, string>>();
        }

        public string? Id { get; set; }
        public bool Active { get; set; }
        public string? Shortname { get; set; }

        public Nullable<DateTime> DateBegin { get; set; }
        public Nullable<DateTime> DateEnd { get; set; }

        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }


        //GPS Info
        public string? Gpstype { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Nullable<double> Altitude { get; set; }
        public string? AltitudeUnitofMeasure { get; set; }

        public string? OrgRID { get; set; }

        [SwaggerDeprecated("Obsolete use EventPublisher List")]
        public int? Ranc { get; set; }
        public string? Ticket { get; set; }
        public string? SignOn { get; set; }
        public string? PayMet { get; set; }

        [SwaggerDeprecated("Obsolete")]
        public string? Type { get; set; }
        public string Pdf { get; set; }

        public string? DistrictId { get; set; }
        public ICollection<string>? DistrictIds { get; set; }
        public ICollection<ImageGallery>? ImageGallery { get; set; }

        public IDictionary<string, Detail> Detail { get; set; }

        public ICollection<string>? TopicRIDs { get; set; }
        public ICollection<Topic>? Topics { get; set; }


        public ICollection<EventPublisher>? EventPublisher { get; set; }

        public IDictionary<string, EventAdditionalInfos> EventAdditionalInfos { get; set; }
        public IDictionary<string, EventPrice> EventPrice { get; set; }

        public ICollection<EventDate>? EventDate { get; set; }

        public IDictionary<string, ContactInfos> ContactInfos { get; set; }
        public IDictionary<string, ContactInfos> OrganizerInfos { get; set; }

        public LocationInfo? LocationInfo { get; set; }

        public ICollection<string>? SmgTags { get; set; }
        public bool SmgActive { get; set; }

        public ICollection<string>? HasLanguage { get; set; }

        public Nullable<DateTime> NextBeginDate { get; set; }

        public string Source { get; set; }
        public bool? GrpEvent { get; set; }
        public bool? EventBenefit { get; set; }
        public EventBooking? EventBooking { get; set; }
        public ICollection<LTSTags> LTSTags { get; set; }

        public IDictionary<string, ICollection<EventPrice>> EventPrices { get; set; }

        //Only for LTS internal use
        //public IDictionary<string, ICollection<EventVariant>> EventVariants { get; set; }

        public IDictionary<string, ICollection<string>> Hashtag { get; set; }

        public EventOperationScheduleOverview EventOperationScheduleOverview { get; set; }

        [SwaggerSchema(Description = "generated field", ReadOnly = true)]
        public IDictionary<string, GpsInfo> GpsPoints
        {
            get
            {
                if (this.Latitude != 0 && this.Longitude != 0)
                {
                    return new Dictionary<string, GpsInfo>
                    {
                        { "position", new GpsInfo(){ Gpstype = "position", Altitude = this.Altitude, AltitudeUnitofMeasure = this.AltitudeUnitofMeasure, Latitude = this.Latitude, Longitude = this.Longitude } }
                    };
                }
                else
                {
                    return new Dictionary<string, GpsInfo>
                    {
                    };
                }
            }
        }

        public ICollection<string>? PublishedOn { get; set; }

        public string ClassificationRID { get; set; }

        public ICollection<EventCrossSelling> EventCrossSelling { get; set; }
        public IDictionary<string, EventDescAdditional> EventDescAdditional { get; set; }

        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }

        public DistanceInfo? DistanceInfo { get; set; }
    }

    public class Detail : IDetailInfos, ILanguage
    {
        public string? Header { get; set; }
        //public string SiteHeader { get; set; }  
        public string? SubHeader { get; set; }
        public string? IntroText { get; set; }
        public string? BaseText { get; set; }
        public string? Title { get; set; }
        //OLT
        //public string Alttext { get; set; }
        public string? AdditionalText { get; set; }
        //NEW
        public string? MetaTitle { get; set; }
        public string? MetaDesc { get; set; }

        public string? GetThereText { get; set; }
        public string? Language { get; set; }

        public ICollection<string>? Keywords { get; set; }

        //New LTS Fields        
        public string? ParkingInfo { get; set; }
        public string? PublicTransportationInfo { get; set; }
        public string? AuthorTip { get; set; }
        public string? SafetyInfo { get; set; }
        public string? EquipmentInfo { get; set; }
    }

    //Special Element for Themed Content 
    public class DetailThemed : ILanguage
    {
        public DetailThemed()
        {
            DetailsThemed = new Dictionary<string, DetailsThemed>();
        }

        public IDictionary<string, DetailsThemed> DetailsThemed { get; set; }
        public string? Language { get; set; }
    }

    public class DetailsThemed
    {
        public string? Title { get; set; }
        public string? Intro { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDesc { get; set; }
    }

    public class GpsInfo : IGpsInfo
    {
        public string? Gpstype { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Altitude { get; set; }
        public string? AltitudeUnitofMeasure { get; set; }
    }

    public class DistanceInfo : IDistanceInfo
    {
        public Nullable<double> DistanceToMunicipality { get; set; }
        public Nullable<double> DistanceToDistrict { get; set; }
    }

    public class GpsTrack : IGpsTrack
    {
        public GpsTrack()
        {
            GpxTrackDesc = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        public IDictionary<string, string> GpxTrackDesc { get; set; }
        public string? GpxTrackUrl { get; set; }
        public string? Type { get; set; }
        public string? Format { get; set; }
    }

    public class GpsPolygon : IGpsPolygon
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class Webcam : IWebcam
    {
        public Webcam()
        {
            Webcamname = new Dictionary<string, string>();
        }

        public string? WebcamId { get; set; }
        public IDictionary<string, string> Webcamname { get; set; }
        public string? Webcamurl { get; set; }

        [SwaggerDeprecated("Use GpsPoints instead")]
        public GpsInfo? GpsInfo { get; set; }
        public int? ListPosition { get; set; }
        public string? Streamurl { get; set; }
        public string? Previewurl { get; set; }

        public string? Source { get; set; }

        ////NEW Webcam Properties
        //public string Streamurl { get; set; }
        //public string Previewurl { get; set; }
        //public DateTime? LastChange { get; set; }
        //public DateTime? FirstImport { get; set; }

        //public bool? Active { get; set; }

        //public string Source { get; set; }
    }

    public class WebcamInfo : Webcam, IIdentifiable, IImportDateassigneable, ISource, ILicenseInfo, IMappingAware, IPublishedOn, IGPSPointsAware, IActivateable, ISmgActive
    {
        public WebcamInfo()
        {
            Mapping = new Dictionary<string, IDictionary<string, string>>();
        }

        public LicenseInfo? LicenseInfo { get; set; }

        //NEW Webcam Properties
        public string? Id { get; set; }
        public new string? Streamurl { get; set; }
        public new string? Previewurl { get; set; }
        public DateTime? LastChange { get; set; }
        public DateTime? FirstImport { get; set; }
        public string? Shortname { get; set; }
        public bool Active { get; set; }
        public bool SmgActive { get; set; }
        public new string? Source { get; set; }
        public ICollection<PublishedonObject>? WebcamAssignedOn { get; set; }

        public ICollection<string>? AreaIds { get; set; }

        public ICollection<string>? SmgTags { get; set; }

        [SwaggerSchema(Description = "generated field", ReadOnly = true)]
        public IDictionary<string, GpsInfo> GpsPoints
        {
            get
            {
                if (this.GpsInfo != null)
                {
                    return new Dictionary<string, GpsInfo>
                    {
                        { this.GpsInfo.Gpstype, this.GpsInfo }
                    };
                }
                else
                {
                    return new Dictionary<string, GpsInfo>
                    {
                    };
                }
            }
        }

        public ICollection<string>? PublishedOn { get; set; }

        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }

        //Temporary Hack because GpsInfo here is a object instead of object list
        public ICollection<GpsInfo> GpsInfos
        {
            get
            {
                return this.GpsInfo != null ? new List<GpsInfo> { this.GpsInfo } : new List<GpsInfo>();
            }
        }
    }

    public class PublishedonObject
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public DateTime LastChange { get; set; }
    }

    public class ImageGallery : IImageGallery
    {
        public ImageGallery()
        {
            ImageTitle = new Dictionary<string, string>();
            ImageDesc = new Dictionary<string, string>();
            ImageAltText = new Dictionary<string, string>();
        }

        public string? ImageName { get; set; }
        public string? ImageUrl { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string? ImageSource { get; set; }

        public IDictionary<string, string> ImageTitle { get; set; }
        public IDictionary<string, string> ImageDesc { get; set; }
        public IDictionary<string, string> ImageAltText { get; set; }

        public bool? IsInGallery { get; set; }
        public int? ListPosition { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        //NEU
        public string? CopyRight { get; set; }
        public string? License { get; set; }
        public string? LicenseHolder { get; set; }
        public ICollection<string>? ImageTags { get; set; }
    }

    public class VideoItems : IVideoItems
    {
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? VideoSource { get; set; }
        public string? VideoType { get; set; }
        public string? StreamingSource { get; set; }
        public string VideoTitle { get; set; }
        public string VideoDesc { get; set; }
        public bool? Active { get; set; }
        public string? CopyRight { get; set; }
        public string? License { get; set; }
        public string? LicenseHolder { get; set; }
        public string? Language { get; set; }
    }

    public class ContactInfos : IContactInfos, ILanguage
    {
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string? CountryCode { get; set; }
        public string? CountryName { get; set; }
        public string? Surname { get; set; }
        public string? Givenname { get; set; }
        public string? NamePrefix { get; set; }
        public string? Email { get; set; }
        public string? Phonenumber { get; set; }
        public string? Faxnumber { get; set; }
        public string? Url { get; set; }
        public string? Language { get; set; }
        public string? CompanyName { get; set; }
        public string? Vat { get; set; }
        public string? Tax { get; set; }
        public string? LogoUrl { get; set; }
    }

    public class AdditionalPoiInfos : IAdditionalPoiInfos, ILanguage
    {
        //public string Difficulty { get; set; }
        public string? Novelty { get; set; }

        [SwaggerDeprecated("Use Categories instead")]
        public string? MainType { get; set; }
        [SwaggerDeprecated("Use Categories instead")]
        public string? SubType { get; set; }
        [SwaggerDeprecated("Use Categories instead")]
        public string? PoiType { get; set; }

        public string? Language { get; set; }

        public List<string>? Categories { get; set; }
    }

    public class Ratings : IRatings
    {
        public string? Stamina { get; set; }
        public string? Experience { get; set; }
        public string? Landscape { get; set; }
        public string? Difficulty { get; set; }
        public string? Technique { get; set; }
    }

    public class LTSTags
    {
        public LTSTags()
        {
            TagName = new Dictionary<string, string>();
            LTSTins = new List<LTSTins>();
        }

        public string? Id { get; set; }
        public int Level { get; set; }
        public string? LTSRID { get; set; }
        public IDictionary<string, string> TagName { get; set; }

        public ICollection<LTSTins>? LTSTins { get; set; }
    }

    public class LTSTins
    {
        public LTSTins()
        {
            TinName = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        public string? LTSRID { get; set; }
        public IDictionary<string, string> TinName { get; set; }
    }

    public class AdditionalArticleInfos : ILanguage
    {
        public AdditionalArticleInfos()
        {
            Elements = new Dictionary<string, string>();
        }

        public string? Language { get; set; }
        public IDictionary<string, string> Elements { get; set; }
    }

    public class ArticleLinkInfo
    {
        public ArticleLinkInfo()
        {
            Elements = new Dictionary<string, string>();
        }

        public string? Language { get; set; }
        public IDictionary<string, string> Elements { get; set; }
    }

    public class CategoryCodes : IIdentifiable
    {
        public string? Id { get; set; }
        public string? Shortname { get; set; }
        //public string Language { get; set; }
    }

    public class Facilities : IIdentifiable
    {
        public string? Id { get; set; }
        public string? Shortname { get; set; }
        //public string Language { get; set; }
    }

    public class CapacityCeremony : IIdentifiable
    {
        public string? Id { get; set; }
        public string? Shortname { get; set; }
        public int MaxSeatingCapacity { get; set; }
        //public string Language { get; set; }
    }

    public class DishRates : IIdentifiable
    {
        public string? Id { get; set; }
        public string? Shortname { get; set; }
        public double MinAmount { get; set; }
        public double MaxAmount { get; set; }
        public string? CurrencyCode { get; set; }
        //public string Language { get; set; }
    }

    [SwaggerSchema("Wiki Article on <a href='https://github.com/noi-techpark/odh-docs/wiki/Operationschedule-Format' target='_blank'>Wiki Article</a>")]
    public class OperationSchedule : IOperationSchedules
    {
        public OperationSchedule()
        {
            OperationscheduleName = new Dictionary<string, string>();
        }
        public IDictionary<string, string> OperationscheduleName { get; set; }
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        /// <summary>
        /// Type: 1 - Standard, 2 - Only day + month recurring (year not to consider) 3 - only month recurring (season: year and day not to consider)
        /// </summary>        
        [SwaggerSchema("1 - Standard, 2 - Only day + month recurring (year not to consider) 3 - only month recurring (season: year and day not to consider), Wiki Article on <a href='https://github.com/noi-techpark/odh-docs/wiki/Operationschedule-Format' target='_blank'>Wiki Article</a>")]
        public string? Type { get; set; }

        public ICollection<OperationScheduleTime>? OperationScheduleTime { get; set; }
    }

    public class OperationScheduleTime : IOperationScheduleTime
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        // Here for compatibility reasons
        [SwaggerDeprecated("Will be removed within 2023-12-31")]
        public bool Thuresday { get { return Thursday; }  }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        /// <summary>
        /// //1 = closed, 2 = open, 0 = undefined
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 1 = General Opening Time, 2 = time range for warm meals, 3 = time range for pizza, 4 = time range for snack’s
        /// </summary>
        public int Timecode { get; set; }
    }

    //Event Data

    public class Topic
    {
        public string? TopicRID { get; set; }
        public string? TopicInfo { get; set; }
    }

    public class EventAdditionalInfos : IEventAdditionalInfos, ILanguage
    {
        public string? Mplace { get; set; }
        public string? Reg { get; set; }
        public string? Location { get; set; }
        public string? Language { get; set; }
    }

    //TODO Mark as deprecated
    public class EventPrice : IEventPrice, ILanguage
    {
        public double Price { get; set; }
        public string? Type { get; set; }
        public string? Pstd { get; set; }
        public string? ShortDesc { get; set; }
        public string? LongDesc { get; set; }
        public string? Description { get; set; }

        public string? Language { get; set; }
        public string PriceRID { get; set; }
        public string VarRID { get; set; }
    }

    public class EventPublisher
    {
        public string? PublisherRID { get; set; }
        public int Ranc { get; set; }
        public int Publish { get; set; }
    }

    public class EventDate : IEventDate
    {
        //Test automatic Generation
        //public DateTime DateBegin
        //{
        //    get
        //    {
        //        return new DateTime(From.Year, From.Month, From.Day, Begin.Value.Hours, Begin.Value.Minutes, Begin.Value.Days);
        //    }
        //}

        //public DateTime DateEnd
        //{
        //    get
        //    {
        //        return new DateTime(To.Year, To.Month, To.Day, End.Value.Hours, End.Value.Minutes, End.Value.Days);
        //    }
        //}

        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public bool? SingleDays { get; set; }
        public int? MinPersons { get; set; }
        public int? MaxPersons { get; set; }
        public bool? Ticket { get; set; }
        public double? GpsNorth { get; set; }
        public double? GpsEast { get; set; }
        public TimeSpan? Begin { get; set; }
        public TimeSpan? End { get; set; }
        public TimeSpan? Entrance { get; set; }

        //NEW Properties
        public double? InscriptionTill { get; set; }
        public bool? Active { get; set; }
        public string? DayRID { get; set; }

        public Dictionary<string, EventDateAdditionalInfo>? EventDateAdditionalInfo { get; set; }
        public ICollection<EventDateAdditionalTime>? EventDateAdditionalTime { get; set; }
        public EventDateCalculatedDay? EventCalculatedDay { get; set; }

        //New
        public string PriceFrom { get; set; }
        public string Cancelled { get; set; }
    }

    public class EventDateAdditionalInfo : ILanguage
    {
        public string Description { get; set; }
        public string Guide { get; set; }
        public string InscriptionLanguage { get; set; }
        public string Language { get; set; }

        public string Cancelled { get; set; }
    }

    //TODO GET MORE INFOS ABOUT THIS
    public class EventDateAdditionalTime
    {
        public string Days { get; set; }
        public TimeSpan Entrance1 { get; set; }
        public TimeSpan Begin1 { get; set; }
        public TimeSpan End1 { get; set; }
        public TimeSpan Entrance2 { get; set; }
        public TimeSpan Begin2 { get; set; }
        public TimeSpan End2 { get; set; }
    }

    public class EventDateCalculatedDay
    {
        public string CDayRID { get; set; }
        public DateTime Day { get; set; }
        public TimeSpan Begin { get; set; }
        //public int TicketsAvailable { get; set; }
        //public int MaxSellableTickets { get; set; }
        //public ICollection<EventDateCalculatedDayVariant> EventDateCalculatedDayVariant { get; set; }

        ////found in response
        //public Nullable<int> AvailabilityCalculatedValue { get; set; }
        //public Nullable<int> AvailabilityLow { get; set; }
        //public Nullable<double> PriceFrom { get; set; }
    }

    //public class EventDateCalculatedDayVariant
    //{
    //    public string VarRID { get; set; }
    //    public double Price { get; set; }
    //    public Nullable<bool> IsStandardVariant { get; set; }
    //    public Nullable<int> TotalSellable { get; set; }
    //}

    public class EventBooking
    {
        public EventBooking()
        {
            BookingUrl = new Dictionary<string, EventBookingDetail>();
        }

        public DateTime? BookableFrom { get; set; }
        public DateTime? BookableTo { get; set; }
        public int? AccommodationAssignment { get; set; }

        public Dictionary<string, EventBookingDetail> BookingUrl { get; set; }
    }

    public class EventBookingDetail
    {
        public string Url { get; set; }
    }

    //public class EventVariant
    //{
    //    public string VarRID { get; set; }
    //    public string ShortDescription { get; set; }
    //    public string LongDescription { get; set; }
    //    public string Description { get; set; }
    //    public string Language { get; set; }
    //}

    public class EventOperationScheduleOverview
    {
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
    }

    public class EventCrossSelling
    {
        public string EventRID { get; set; }
    }

    public class EventDescAdditional
    {
        public string Type { get; set; }
        public string Language { get; set; }
        public string Order { get; set; }
        public string RQPlain { get; set; }
        public string RQHtml { get; set; }
        public string RSPlain { get; set; }
        public string RSHtml { get; set; }
    }

    //end Event classes

    public class LocationInfo : ILocationInfoAware
    {
        public RegionInfo? RegionInfo { get; set; }
        public TvInfo? TvInfo { get; set; }
        public MunicipalityInfo? MunicipalityInfo { get; set; }
        public DistrictInfo? DistrictInfo { get; set; }
        public AreaInfo? AreaInfo { get; set; }
    }

    public class RegionInfo
    {
        public string? Id { get; set; }

        [SwaggerDeprecated("Deprecated use the Id/Selflink to retrieve correct names from the appropriate Endpoint")]
        public IDictionary<string, string?>? Name { get; set; }
    }

    public class TvInfo
    {
        public string? Id { get; set; }

        [SwaggerDeprecated("Deprecated use the Id/Selflink to retrieve correct names from the appropriate Endpoint")]
        public IDictionary<string, string?>? Name { get; set; }
    }

    public class MunicipalityInfo
    {
        public string? Id { get; set; }

        [SwaggerDeprecated("Deprecated use the Id/Selflink to retrieve correct names from the appropriate Endpoint")]
        public IDictionary<string, string?>? Name { get; set; }
    }

    public class DistrictInfo
    {
        public string? Id { get; set; }

        [SwaggerDeprecated("Deprecated use the Id/Selflink to retrieve correct names from the appropriate Endpoint")]
        public IDictionary<string, string?>? Name { get; set; }
    }

    public class AreaInfo
    {
        public string? Id { get; set; }

        [SwaggerDeprecated("Deprecated use the Id/Selflink to retrieve correct names from the appropriate Endpoint")]
        public IDictionary<string, string>? Name { get; set; }
    }

    #endregion    
    
}
