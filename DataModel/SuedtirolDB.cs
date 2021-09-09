using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Annotations;

namespace DataModel
{
    /// <summary>
    /// Interfaces welche von den anderen Klassen implementiert werden
    /// </summary>
    #region Interfaces

    //Common Interfaces

    //Id (LTSRID) und Shortname
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

    public interface IImportDateassigneable
    {
        DateTime? FirstImport { get; set; }
        DateTime? LastChange { get; set; }
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
        //brauchts nix?
        IDictionary<string, ContactInfos> ContactInfos { get; set; }
    }

    public interface IImageGallery
    {
        //ImageDESC nur als Icollection??? zuerts LTS Schnittstellen anschauen

        string? ImageName { get; set; }
        string? ImageUrl { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        string? ImageSource { get; set; }

        IDictionary<string, string> ImageTitle { get; set; }
        IDictionary<string, string> ImageDesc { get; set; }

        Nullable<bool> IsInGallery { get; set; }
        Nullable<int> ListPosition { get; set; }
        Nullable<DateTime> ValidFrom { get; set; }
        Nullable<DateTime> ValidTo { get; set; }
    }

    public interface IImageGalleryAware
    {
        //brauchts nix?
        ICollection<ImageGallery>? ImageGallery { get; set; }
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
        Nullable<double> Altitude { get; set; }
        string? AltitudeUnitofMeasure { get; set; }
    }

    public interface IGpsTrack
    {
        string? Id { get; set; }
        IDictionary<string, string> GpxTrackDesc { get; set; }
        string? GpxTrackUrl { get; set; }
        string? Type { get; set; }
    }

    public interface IGpsPolygon
    {
        double Latitude { get; set; }
        double Longitude { get; set; }
    }

    public interface IGpsPolygonAware
    {
        //brauchts nix?
        ICollection<GpsPolygon>? GpsPolygon { get; set; }
    }

    public interface IGeoDataInfoAware
    {
        double AltitudeDifference { get; set; }
        double AltitudeHighestPoint { get; set; }
        double DistanceDuration { get; set; }
        double DistanceLength { get; set; }

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
        bool Thuresday { get; set; }
        bool Friday { get; set; }
        bool Saturday { get; set; }
        bool Sunday { get; set; }
        int State { get; set; }
        int Timecode { get; set; }
    }

    public interface ISmgTags
    {
        ICollection<string>? SmgTags { get; set; }
        //string Id { get; set; }
        //IDictionary<string, string> TagDescription { get; set; }
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

    //End Common Interfaces

    //Activity & Poi Data Interfaces

    public interface IAdditionalPoiInfosAware
    {
        //brauchts nix?
        IDictionary<string, AdditionalPoiInfos> AdditionalPoiInfos { get; set; }
    }

    public interface IAdditionalPoiInfos
    {
        //string Difficulty { get; set; }
        string? Novelty { get; set; }
        string? MainType { get; set; }
        string? SubType { get; set; }
        string? PoiType { get; set; }

        List<string> Categories { get; set; }
    }

    public interface IActivityStatus
    {
        bool IsOpen { get; set; }
        bool IsPrepared { get; set; }
        bool RunToValley { get; set; }
        bool IsWithLigth { get; set; }
        bool HasRentals { get; set; }
    }

    //End Activity & Poi Data Interfaces 

    //Article Interfaces

    public interface IAdditionalArticleInfosAware
    {
        //brauchts nix?
        IDictionary<string, AdditionalArticleInfos> AdditionalArticleInfos { get; set; }
    }

    //End Article Interfaces

    //Event Interfaces

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
        bool SingleDays { get; set; }
        int MinPersons { get; set; }
        int MaxPersons { get; set; }
        bool Ticket { get; set; }
        double GpsNorth { get; set; }
        double GpsEast { get; set; }
        TimeSpan Begin { get; set; }
        TimeSpan End { get; set; }
        TimeSpan Entrance { get; set; }
    }

    //End Event Interfaces

    #endregion

    /// <summary>
    /// Fraktionen, Gemeinden und Regionen mit Ihren Detailinformationen
    /// </summary>
    #region District Municipality Region

    public class Region : BaseInfos, IImageGalleryAware, IWebcamAware
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

    public class MetaRegion : BaseInfos, IImageGalleryAware, IWebcamAware
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

    //NEW Erlebnisräume
    public class ExperienceArea : BaseInfos, IImageGalleryAware
    {
        public ICollection<string>? DistrictIds { get; set; }
        public ICollection<string>? TourismvereinIds { get; set; }

        public ICollection<GpsPolygon>? GpsPolygon { get; set; }
        public bool VisibleInSearch { get; set; }

        public ICollection<RelatedContent>? RelatedContent { get; set; }
    }

    public class Tourismverein : BaseInfos, IImageGalleryAware, IWebcamAware
    {
        public string? RegionId { get; set; }

        public ICollection<GpsPolygon>? GpsPolygon { get; set; }
        public ICollection<Webcam>? Webcam { get; set; }
        public bool VisibleInSearch { get; set; }
        public ICollection<string>? SkiareaIds { get; set; }

        public ICollection<RelatedContent>? RelatedContent { get; set; }
    }

    public class Municipality : BaseInfos, IImageGalleryAware, IWebcamAware
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

    public class District : BaseInfos, IImageGalleryAware, IWebcamAware
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

    public class Area : IIdentifiable, IActivateable, IImportDateassigneable
    {
        public LicenseInfo LicenseInfo { get; set; }

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

        //Neu
        public string? SlopeKmBlue { get; set; }
        public string? SlopeKmRed { get; set; }
        public string? SlopeKmBlack { get; set; }

        public string? LiftCount { get; set; }

        //Neu Altimeter von bis
        public Nullable<int> AltitudeFrom { get; set; }
        public Nullable<int> AltitudeTo { get; set; }


        public IDictionary<string, string> SkiRegionName { get; set; }

        public ICollection<string>? AreaId { get; set; }
        //Compatibility
        public ICollection<string>? AreaIds { get { return AreaId; } }

        public ICollection<Webcam>? Webcam { get; set; }
        //NEU Region TV Municipality Fraktion NEU LocationInfo Classe
        public LocationInfo? LocationInfo { get; set; }

        //Folsch
        //public OperationSchedule OperationSchedule { get; set; }
        public ICollection<OperationSchedule>? OperationSchedule { get; set; }

        public ICollection<string>? TourismvereinIds { get; set; }
        public ICollection<string>? RegionIds { get; set; }

        public ICollection<GpsPolygon>? GpsPolygon { get; set; }

        public ICollection<RelatedContent>? RelatedContent { get; set; }
    }

    public class SkiRegion : BaseInfos, IImageGalleryAware, IGpsPolygonAware, IWebcamAware
    {
        public ICollection<GpsPolygon>? GpsPolygon { get; set; }

        public ICollection<Webcam>? Webcam { get; set; }

        public ICollection<RelatedContent>? RelatedContent { get; set; }
    }

    public class Naturepark : BaseInfos, IImageGalleryAware, IContactInfosAware, IWebcamAware
    {
        //NEU Region TV Municipality Fraktion NEU LocationInfo Classe
        public LocationInfo? LocationInfo { get; set; }

        public ICollection<Webcam>? Webcam { get; set; }
        public ICollection<GpsPolygon>? GpsPolygon { get; set; }

        public ICollection<RelatedContent>? RelatedContent { get; set; }
    }

    public class Christkindlmarkt : BaseInfos, IImageGalleryAware, IContactInfosAware, IWebcamAware
    {
        //NEU Region TV Municipality Fraktion NEU LocationInfo Classe        
        public LocationInfo? LocationInfo { get; set; }

        public ICollection<Webcam>? Webcam { get; set; }
    }

    public class SmgTags : IIdentifiable, IImportDateassigneable, ILicenseInfo
    {
        public LicenseInfo LicenseInfo { get; set; }

        public SmgTags()
        {
            TagName = new Dictionary<string, string>();
            ValidForEntity = new List<string>();
        }

        public string? Id { get; set; }
        public string? Shortname { get; set; }

        public IDictionary<string, string> TagName { get; set; }
        public ICollection<string> ValidForEntity { get; set; }

        public string? MainEntity { get; set; }

        public ICollection<string> Source { get; set; }

        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }

        //IDM Mapping
        public IDictionary<string, string> IDMCategoryMapping { get; set; }
        public Nullable<bool> DisplayAsCategory { get; set; }

        public LTSTaggingInfo LTSTaggingInfo { get; set; }
    }

    public class LTSTaggingInfo
    {
        //NEW LTS RID
        public string LTSRID { get; set; }
        public string ParentLTSRID { get; set; }
    }

#endregion


/// <summary>
/// Informationen zu G0RIDs Marketinggroups Muassi no iberprüfen
/// </summary>
#region Marketinggroup

public class Marketinggroup : IIdentifiable
    {
        public LicenseInfo LicenseInfo { get; set; }

        public string? Id { get; set; }
        public string? Shortname { get; set; }
        public string? Beschreibung { get; set; }
    }

    #endregion

    /// <summary>
    /// Alle Aktivitäten welche eine Route haben
    /// </summary>
    #region Activities & POIs

    //Activities

    //public class Wanderung : PoiBaseInfos
    //{
    //    //public List<string> LTSTagList { get; set; }
    //}

    public class Hike : PoiBaseInfos
    {

    }

    public class RunningFitness : PoiBaseInfos
    {

    }

    public class Bike : PoiBaseInfos
    {

    }

    public class Alpine : PoiBaseInfos
    {

    }

    public class Skitrack : PoiBaseInfos
    {

    }

    public class Slide : PoiBaseInfos
    {

    }

    public class Slope : PoiBaseInfos
    {

    }

    public class Lift : PoiBaseInfos
    {

    }

    public class Equestrianism : PoiBaseInfos
    {

    }

    public class CityTour : PoiBaseInfos
    {

    }

    /// <summary>
    /// LTS Point of Interest
    /// </summary>
    public class LTSPoi : PoiBaseInfos, ILicenseInfo
    {
        
    }

    //For PG Activities & Pois

    public class GBLTSPoi : LTSPoi
    {
        public GBLTSPoi()
        {
            GpsPoints = new Dictionary<string, GpsInfo>();
        }

        public IDictionary<string, GpsInfo> GpsPoints { get; set; }
    }

    public class GBLTSActivity : PoiBaseInfos
    {
        public GBLTSActivity()
        {
            GpsPoints = new Dictionary<string, GpsInfo>();
        }

        public IDictionary<string, GpsInfo> GpsPoints { get; set; }        
    }

    //End for PG

    public class Mobility : PoiBaseInfos
    {

    }

    public class Active : PoiBaseInfos
    {

    }

    public class Nightlife : PoiBaseInfos
    {

    }

    public class Service : PoiBaseInfos
    {

    }

    public class Shop : PoiBaseInfos
    {

    }

    public class Sightseen : PoiBaseInfos
    {

    }

    public class Artisan : PoiBaseInfos
    {

    }

    public class Health : PoiBaseInfos
    {

    }

    public class SmgPoi : PoiBaseInfos, IWebcamAware, ILicenseInfo
    {
        public SmgPoi()
        {
            PoiProperty = new Dictionary<string, List<PoiProperty>>();
            //LinkedAppSuggestions = new Dictionary<string, string>();
        }

        public string? CustomId { get; set; }


        public ICollection<Webcam>? Webcam { get; set; }

        public IDictionary<string, List<PoiProperty>> PoiProperty { get; set; }
        public ICollection<string>? PoiServices { get; set; }

        //Neu        
        public string? Source { get; set; }
        public string? SyncSourceInterface { get; set; }
        public string? SyncUpdateMode { get; set; }

        public int AgeFrom { get; set; }
        public int AgeTo { get; set; }

        //NEW Gastronomy
        public int MaxSeatingCapacity { get; set; }
        public ICollection<CategoryCodes>? CategoryCodes { get; set; }
        public ICollection<DishRates>? DishRates { get; set; }
        public ICollection<CapacityCeremony>? CapacityCeremony { get; set; }
        public ICollection<Facilities>? Facilities { get; set; }

        public ICollection<RelatedContent>? RelatedContent { get; set; }

        //new for Wine Importers
        public IDictionary<string, List<AdditionalContact>> AdditionalContact { get; set; }

        //NEU LISTE Suggestions
        //public IDictionary<string, string> LinkedAppSuggestions { get; set; }
        //public bool? overWriteAppSuggestions { get; set; }

        ////NEU LISTE mit z.B Google Places ID
        //public ICollection<RatingSources> RatingSources { get; set; }
    }

    public class ODHActivityPoi : SmgPoi
    {
        public ODHActivityPoi()
        {
            GpsPoints = new Dictionary<string, GpsInfo>();
        }

        public IDictionary<string, GpsInfo> GpsPoints { get; set; }        
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

        public string Self { get { return this.Link; } }
    }

    public class AppSuggestion
    {
        public AppSuggestion()
        {
            Suggestion = new Dictionary<string, Suggestion>();
        }

        public string? Id { get; set; }
        public string? Platform { get; set; }

        public List<AppSuggestionValidFor>? Validfor { get; set; }

        public IDictionary<string, Suggestion> Suggestion { get; set; }

    }

    public class AppSuggestionValidFor
    {
        public string? MainEntity { get; set; }
        public string? Type { get; set; }
        public string? Value { get; set; }
    }

    public class Suggestion
    {
        public string? Title { get; set; }
        public string? Icon { get; set; }
        public string? Package { get; set; }
        public string? Developer { get; set; }
        public string? Description { get; set; }
    }

    public class RatingSources
    {
        public string? Source { get; set; }
        public string? Objectid { get; set; }
    }



    #endregion

    #region Accommodations

    public class Accommodation : TrustYouInfos, IIdentifiable, IActivateable, IGpsInfo, IImageGalleryAware, ISmgActive, IHasLanguage, IImportDateassigneable, ILicenseInfo
    {
        public LicenseInfo LicenseInfo { get; set; }

        public Accommodation()
        {
            AccoDetail = new Dictionary<string, AccoDetail>();
            MssResponseShort = new List<MssResponseShort>();
        }

        public string? Id { get; set; }
        public bool Active { get; set; }
        public string? HgvId { get; set; }
        public string? Shortname { get; set; }

        //public int Units { get; set; }
        //public int Beds { get; set; }
        public int? Representation { get; set; }
        public bool HasApartment { get; set; }
        public bool HasRoom { get; set; }
        public bool IsCamping { get; set; }
        public bool IsGastronomy { get; set; }
        public bool IsBookable { get; set; }
        public bool IsAccommodation { get; set; }
        public bool SmgActive { get; set; }
        public bool TVMember { get; set; }
        public string? TourismVereinId { get; set; }
        public string? MainLanguage { get; set; }
        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }
        public string? Gpstype { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Nullable<double> Altitude { get; set; }
        public string? AltitudeUnitofMeasure { get; set; }

        public string? AccoCategoryId { get; set; }
        public string? AccoTypeId { get; set; }
        public string? DistrictId { get; set; }

        public ICollection<string>? BoardIds { get; set; }
        public ICollection<string>? MarketingGroupIds { get; set; }
        public ICollection<AccoFeature>? Features { get; set; }

        //Custom
        public ICollection<string>? BadgeIds { get; set; }
        public ICollection<string>? ThemeIds { get; set; }
        public ICollection<string>? SpecialFeaturesIds { get; set; }

        public IDictionary<string, AccoDetail>? AccoDetail { get; set; }
        public ICollection<AccoBookingChannel>? AccoBookingChannel { get; set; }
        public ICollection<ImageGallery>? ImageGallery { get; set; }

        //NEU Region TV Municipality Fraktion NEU LocationInfo Classe
        public LocationInfo? LocationInfo { get; set; }

        //Gastronomy 
        public string? GastronomyId { get; set; }
        public ICollection<string>? SmgTags { get; set; }

        public ICollection<string>? HasLanguage { get; set; }

        //MSS Result
        public ICollection<MssResponseShort>? MssResponseShort { get; set; }

        //Independent Data
        public IndependentData IndependentData { get; set; }

        public ICollection<AccoRoomInfo> AccoRoomInfo { get; set; }
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
        public string? TrustYouID { get; set; }
        public double TrustYouScore { get; set; }
        public int TrustYouResults { get; set; }
        public bool TrustYouActive { get; set; }
        public int TrustYouState { get; set; }
    }

    public class AccoRoom : IIdentifiable, IImageGalleryAware, IHasLanguage, IImportDateassigneable, ILicenseInfo
    {
        public LicenseInfo LicenseInfo { get; set; }

        public AccoRoom()
        {
            AccoRoomDetail = new Dictionary<string, AccoRoomDetail>();
        }

        public string Id { get; set; }
        public string? Shortname { get; set; }

        public string? A0RID { get; set; }

        public string? Roomtype { get; set; }

        public ICollection<AccoFeature>? Features { get; set; }
        public IDictionary<string, AccoRoomDetail> AccoRoomDetail { get; set; }
        public ICollection<ImageGallery>? ImageGallery { get; set; }

        public ICollection<string>? HasLanguage { get; set; }

        //NEU
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

    public class Gastronomy : GastronomyBaseInfos, ILicenseInfo
    {

    }

    //Für Types Api


    #endregion

    #region Events

    public class Event : EventBaseInfos, ILicenseInfo
    {
        //Neu        
        //public string CustomId { get; set; }
        //public string Source { get; set; }
        //public string SyncSourceInterface { get; set; }
        //public string SyncUpdateMode { get; set; }
        //public bool? GrpEvent { get; set; }
        //public string Pdf { get; set; }

        //public string Source { get; set; }
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

        public LicenseInfo LicenseInfo { get; set; }
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

        public LicenseInfo LicenseInfo { get; set; }
    }

    public class BezirksForecast
    {
        public DateTime date { get; set; }
        public string? WeatherCode { get; set; }
        public string? WeatherDesc { get; set; }
        public string? WeatherImgUrl { get; set; }

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
    }

    public class WeatherRealTime
    {
        public double altitude { get; set; }
        public int categoryId { get; set; }
        public string? code { get; set; }
        public string? id { get; set; }
        public string? dd { get; set; }
        public string? ff { get; set; }
        public string? hs { get; set; }
        public DateTime lastUpdated { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string? lwdType { get; set; }
        public string? n { get; set; }
        public string? name { get; set; }
        public string? p { get; set; }
        public string? q { get; set; }
        public string? rh { get; set; }
        public string? t { get; set; }
        public string? vaxcode { get; set; }
        public string? w { get; set; }
        public string? wmax { get; set; }
        public string? sd { get; set; }
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

    public class WeatherHistory
    {
        public WeatherHistory()
        {
            Weather = new Dictionary<string, Weather>();
        }
        public IDictionary<string, Weather> Weather { get; set; }
        public LicenseInfo LicenseInfo { get; set; }

        public List<string> HasLanguage { get; set; }
        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }

        public string? Shortname { get; set; }
    }

    #endregion

    #region Articles    

    public class Article : ArticleBaseInfos  //, IArticleBaseInfos
    {

    }

    public class RecipeArticle : ArticleBaseInfos
    {

    }

    public class PressArticle : ArticleBaseInfos
    {

    }

    public class BaseArticle : ArticleBaseInfos
    {

    }

    public class EventArticle : ArticleBaseInfos
    {

    }

    public class BookArticle : ArticleBaseInfos
    {

    }

    public class CatalogArticle : ArticleBaseInfos
    {

    }

    public class TouroperatorArticle : ArticleBaseInfos
    {

    }

    public class B2BArticle : ArticleBaseInfos
    {

    }

    public class ContentArticle : ArticleBaseInfos
    {

    }



    #endregion

    #region Packages

    public class Package : IIdentifiable, IActivateable, ISmgActive, ISmgTags, IImageGalleryAware, IHasLanguage
    {
        public LicenseInfo LicenseInfo { get; set; }

        public Package()
        {
            PackageDetail = new Dictionary<string, PackageDetail>();
            Inclusive = new Dictionary<string, Inclusive>();
            ChannelInfo = new Dictionary<string, string>();
            //PackageTheme = new Dictionary<string, PackageTheme>();
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

    public class InclusiveLocalized
    {
        public int PriceId { get; set; }
        public int PriceTyp { get; set; }

        public PackageDetail? PackageDetail { get; set; }
        public ICollection<ImageGalleryLocalized>? ImageGallery { get; set; }
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

    public class PackageThemeLocalized
    {
        public int ThemeId { get; set; }
        public ThemeDetail? ThemeDetail { get; set; }
    }

    public class ThemeDetail : ILanguage
    {
        public string? Title { get; set; }
        public string? Language { get; set; }
    }

    #endregion

    #region Measuringpoints

    public class Measuringpoint : IIdentifiable, IActivateable, ISmgActive, IGpsInfo, ILicenseInfo, IImportDateassigneable
    {
        public LicenseInfo LicenseInfo { get; set; }

        //IIdentifiable
        public string? Id { get; set; }

        //Infos zum Import
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

        //Observation
        public string? SnowHeight { get; set; }
        public string? newSnowHeight { get; set; }
        public string? Temperature { get; set; }
        public DateTime LastSnowDate { get; set; }
        public List<WeatherObservation>? WeatherObservation { get; set; }

        //Location Geschichte
        public LocationInfo? LocationInfo { get; set; }
        public string? OwnerId { get; set; }

        public List<string>? AreaIds { get; set; }
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

        public int Id { get; set; }
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

    public class MeasuringpointReduced
    {
        //IIdentifiable
        public string? Id { get; set; }

        public DateTime LastUpdate { get; set; }
        public string? Shortname { get; set; }
        public string? SnowHeight { get; set; }
        public string? newSnowHeight { get; set; }
        public string? Temperature { get; set; }
        public DateTime LastSnowDate { get; set; }
        public List<WeatherObservation>? WeatherObservation { get; set; }


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

    //Für Types Api
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
    }

    //Für Types Api
    public class AccoFeatures : AccoTypes
    {

    }

    //Für Types Api 
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

    //Für Types Api 
    public class ActivityTypes : SmgPoiTypes
    {

    }

    //Für Types Api 
    public class ArticleTypes : SmgPoiTypes
    {

    }

    //Für Types Api 
    public class PoiTypes : SmgPoiTypes
    {

    }

    public class ODHActivityPoiTypes : SmgPoiTypes
    {
    }

    #endregion

    #region Mobile

    public class SmgPoisMobileTypes
    {
        public SmgPoisMobileTypes()
        {
            TypeDesc = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        //public int Bitmask { get; set; }
        public string? Key { get; set; }
        public string? Type { get; set; }
        public string? IconURL { get; set; }
        public int SortOrder { get; set; }
        public bool active { get; set; }

        public Dictionary<string, string>? TypeDesc { get; set; }
    }

    public class SmgPoisMobileTypesExtended : SmgPoisMobileTypes
    {
        public ICollection<SmgPoisMobileFilters>? SubTypes { get; set; }
    }

    public class SmgPoisMobileFilters
    {
        public SmgPoisMobileFilters()
        {
            FilterText = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        public string? MainTypeId { get; set; }
        public int SortOrder { get; set; }

        public int Bitmask { get; set; }
        public IDictionary<string, string> FilterText { get; set; }
        public string? FilterReference { get; set; }

        public ICollection<SmgPoisMobileFilterDetail>? FilterDetails { get; set; }
    }

    public class SmgPoisMobileFilterDetail
    {
        public SmgPoisMobileFilterDetail()
        {
            FilterText = new Dictionary<string, string>();
            StartingDesc = new Dictionary<string, string>();
            EndDesc = new Dictionary<string, string>();
        }

        public string? Id { get; set; }                                  //Unique Id
        //public string MainTypeId { get; set; }                          //Reference to Maintype
        public int SortOrder { get; set; }                              //Sort Order of the Filter
        public int Bitmask { get; set; }

        public string? Filtertype { get; set; }                          //Type of the Filter (checkbox, scroller, rating)
        public IDictionary<string, string> FilterText { get; set; }    //Values of the Filter, Key is intended to use on the Filter Api, Value is what we want to display.  

        public string? FilterReference { get; set; }
        public string? FilterString { get; set; }

        public IDictionary<string, string> StartingDesc { get; set; }    //For a scroller Filter, there can be provided a Starting Description (like 1 km)
        public string? StartingValue { get; set; }                       //For a scroller Filter a startingvalue can be defined
        public IDictionary<string, string> EndDesc { get; set; }         //For a scroller Filter, there can be provided a Ending Description (like >20 km)
        public string? EndValue { get; set; }                            //For a scroller Filter a endingvalue can be defined

        public int RatingItems { get; set; }                            //For a rating Filter there can be set a Rating Items (this well be 6)
        public string? SelectedValue { get; set; }                       //For a rating Filter the Initially selected Value can be defined

    }

    public class MobileHtml
    {
        public MobileHtml()
        {
            HtmlText = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        public Dictionary<string, string> HtmlText { get; set; }
    }

    public class Tutorial
    {
        public Tutorial()
        {
            image_url = new Dictionary<string, string>();
            title = new Dictionary<string, string>();
            description = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        public Dictionary<string, string> image_url { get; set; }
        public Dictionary<string, string> title { get; set; }
        public Dictionary<string, string> description { get; set; }

        public string? category { get; set; }
        public int sortorder { get; set; }

    }

    public class AppMessage
    {
        public AppMessage()
        {
            Text = new Dictionary<string, string>();
            Title = new Dictionary<string, string>();
            VideoUrl = new Dictionary<string, string>();
            Images = new Dictionary<string, List<AppMessageImage>>();
        }


        public string? Id { get; set; }
        public string? Type { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public Dictionary<string, string> Title { get; set; }
        public Dictionary<string, string> Text { get; set; }

        public Dictionary<string, List<AppMessageImage>> Images { get; set; }
        public Dictionary<string, string> VideoUrl { get; set; }
    }

    public class AppMessageImage
    {
        public string? ImageUrl { get; set; }
        public int SortOrder { get; set; }
    }

    //public class FiltersByMainType
    //{
    //    public Dictionary<string, >

    //}

    //public class SmgPoisMobileFilterLocalized
    //{
    //    public string Id { get; set; }                                  //Unique ID
    //    public string MainTypeId { get; set; }                          //Reference to Maintype

    //    public string Filtername { get; set; }                          //Filtername to display
    //    public string Filterkey { get; set; }                           //Effective Value you use on the Filter Api
    //    public string FilterReference { get; set; }

    //    public string language { get; set; }                            //Current Language
    //    public int SortOrder { get; set; }                              //Sort Order

    //    public ICollection<SmgPoisMobileFilterDetailLocalized> FilterDetails { get; set; }     //List with Detailed Filters
    //}

    //public class SmgPoisMobileFilterDetailLocalized
    //{       
    //    public string Id { get; set; }                                  //Unique Id
    //    public string SubTypeId { get; set; }                           //Reference to SmgPoisMobileFilterListLocalized ID

    //    public string Filtername { get; set; }                          //Filtername to display
    //    public string Filterkey { get; set; }                           //Effective Value you use on the Filter Api
    //    public string FilterReference { get; set; }

    //    public string Filtertype { get; set; }                          //Type of the Filter (checkbox, scroller, rating)

    //    public string language { get; set; }                            //Current Language
    //    public int SortOrder { get; set; }                              //Sort Order of the Filter

    //    public string StartingDesc { get; set; }                        //For a scroller Filter, there can be provided a Starting Description (like 1 km)
    //    public string StartingValue { get; set; }                       //For a scroller Filter a startingvalue can be defined
    //    public string EndDesc { get; set; }                             //For a scroller Filter, there can be provided a Ending Description (like >20 km)
    //    public string EndValue { get; set; }                            //For a scroller Filter a endingvalue can be defined

    //    public int RatingItems { get; set; }                            //For a rating Filter there can be set a Rating Items (this well be 6)
    //    public string SelectedValue { get; set; }                       //For a rating Filter the Initially selected Value can be defined
    //}

    public class AccoThemesMobile
    {
        public AccoThemesMobile()
        {
            Name = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        public int Bitmask { get; set; }
        public string? Type { get; set; }
        public string? Key { get; set; }
        public Dictionary<string, string> Name { get; set; }
        public string? ImageURL { get; set; }
        //public int AccoCount { get; set; }
        public int SortOrder { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<int> AccoCount { get; set; }
    }

    public class AccoThemesFull
    {
        public string? Id { get; set; }
        public int Bitmask { get; set; }
        public string? Type { get; set; }
        public string? Name { get; set; }
        public string? ImageURL { get; set; }
        public int AccoCount { get; set; }
        public int SortOrder { get; set; }
    }

    public class AppCustomTips
    {
        public AppCustomTips()
        {
            Title = new Dictionary<string, string>();
            Description = new Dictionary<string, string>();
            Region = new Dictionary<string, string>();
            Tv = new Dictionary<string, string>();
            LinkText = new Dictionary<string, string>();
            Category = new Dictionary<string, string>();
            ValidForLanguage = new Dictionary<string, bool>();
        }

        public string? Id { get; set; }
        public string? ImageUrl { get; set; }
        public IDictionary<string, string> Title { get; set; }
        public IDictionary<string, string> Description { get; set; }
        public IDictionary<string, string> Region { get; set; }
        public IDictionary<string, string> Tv { get; set; }
        public IDictionary<string, string> LinkText { get; set; }
        public string? Link { get; set; }
        public bool Active { get; set; }
        public DateTime LastChanged { get; set; }
        public string? Type { get; set; }

        public string? TvId { get; set; }

        //additional
        public IDictionary<string, string> Category { get; set; }
        public string? Difficulty { get; set; }
        public string? Duration { get; set; }
        public string? Length { get; set; }

        //Settings
        public ICollection<AppCustomTipsSettings>? AppCustomTipsSettings { get; set; }

        public IDictionary<string, bool> ValidForLanguage { get; set; }
    }

    public class AppCustomTipsSettings
    {
        public int Fixedposition { get; set; }  //position 0 is random
        public bool Randomposition { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }

    #endregion


    /// <summary>
    /// Gemeinsam Benutzte Klassen / Entitäten
    /// </summary>
    #region CommonInfos

    public class Wine : IIdentifiable, IImportDateassigneable,ILicenseInfo
    {
        public LicenseInfo LicenseInfo { get; set; }

        public Wine()
        {
            Detail = new Dictionary<string, Detail>();
        }

        public string? Id { get; set; }

        public string? Shortname { get; set; }

        public IDictionary<string, Detail> Detail { get; set; }

        //public Dictionary<string, string> Title { get; set; }
        //public Dictionary<string, string> WineName { get; set; }        

        public int Vintage { get; set; }
        public int Awardyear { get; set; }

        public string? CustomId { get; set; }
        public string? CompanyId { get; set; }



        public ICollection<ImageGallery>? ImageGallery { get; set; }
        public ICollection<string>? Awards { get; set; }

        public DateTime? LastChange { get; set; }
        public DateTime? FirstImport { get; set; }

        public bool Active { get; set; }
        public bool SmgActive { get; set; }

        public ICollection<string>? HasLanguage { get; set; }
    }

    public class SuedtirolType : ISuedtirolType
    {
        public SuedtirolType()
        {
            TypeNames = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        public string? Key { get; set; }

        public int? Bitmask { get; set; } 

        public string? Entity { get; set; }
        public string? TypeParent { get; set; }
        public int Level { get; set; }
        public IDictionary<string, string> TypeNames { get; set; }
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

    //BaseInfos für Districts / Regions / Municipalities
    public abstract class BaseInfos : IIdentifiable, IActivateable, IGpsInfo, ISmgTags, ISmgActive, IHasLanguage, IImportDateassigneable, ILicenseInfo
    {
        public LicenseInfo LicenseInfo { get; set; }

        public BaseInfos()
        {
            Detail = new Dictionary<string, Detail>();
            ContactInfos = new Dictionary<string, ContactInfos>();
        }

        public string Id { get; set; }
        public bool Active { get; set; }
        public string? CustomId { get; set; }
        public string? Shortname { get; set; }
        public string? Gpstype { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Nullable<double> Altitude { get; set; }
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
    }

    //Erweiterte Baseinfos für Activities //abstract wegen Index mol ogscholten
    public class PoiBaseInfos : IIdentifiable, IActivateable, IGeoDataInfoAware, IActivityStatus, IImageGalleryAware, IContactInfosAware, IAdditionalPoiInfosAware, ISmgTags, ISmgActive, IHasLanguage, IImportDateassigneable, ILicenseInfo
    {
        public LicenseInfo LicenseInfo { get; set; }

        public PoiBaseInfos()
        {
            Detail = new Dictionary<string, Detail>();
            ContactInfos = new Dictionary<string, ContactInfos>();
            AdditionalPoiInfos = new Dictionary<string, AdditionalPoiInfos>();
        }

        public string? Id { get; set; }

        public string? OutdooractiveID { get; set; }
        public string? OutdooractiveElevationID { get; set; }

        //new
        public Nullable<bool> CopyrightChecked { get; set; }

        public bool Active { get; set; }
        public string? Shortname { get; set; }
        public string? SmgId { get; set; }
        public bool Highlight { get; set; }

        //obsolete ??
        public string? Difficulty { get; set; }

        //Activity SubType
        public string? Type { get; set; }
        public string? SubType { get; set; }
        public string? PoiType { get; set; }

        //NEU SMG Infos
        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }
        public bool SmgActive { get; set; }

        //NEU Region TV Municipality Fraktion NEU LocationInfo Classe
        public LocationInfo? LocationInfo { get; set; }

        public string? TourismorganizationId { get; set; }
        public ICollection<string>? AreaId { get; set; }
        //Compatibility
        public ICollection<string>? AreaIds { get { return AreaId; } }


        //Distance & Altitude Informationen
        public double AltitudeDifference { get; set; }
        //neu LTSUpdate 11.16
        public double AltitudeHighestPoint { get; set; }
        public double AltitudeLowestPoint { get; set; }
        public double AltitudeSumUp { get; set; }
        public double AltitudeSumDown { get; set; }


        public double DistanceDuration { get; set; }
        public double DistanceLength { get; set; }
        //neu LTSUpdate 11.16

        //Status & Features
        public bool IsOpen { get; set; }
        public bool IsPrepared { get; set; }
        public bool RunToValley { get; set; }
        public bool IsWithLigth { get; set; }
        public bool HasRentals { get; set; }
        public bool HasFreeEntrance { get; set; }
        public bool LiftAvailable { get; set; }
        public bool FeetClimb { get; set; }

        //neu
        public Nullable<bool> BikeTransport { get; set; }

        //OperationSchedule
        //public string OperationscheduleName { get; set; }
        //public DateTime Start { get; set; }
        //public DateTime Stop { get; set; }
        //public bool? ClosedonPublicHolidays { get; set; }
        //public ICollection<OperationScheduleTime> OperationScheduleTime { get; set; }
        //Für mearere aso
        public ICollection<OperationSchedule>? OperationSchedule { get; set; }

        public ICollection<GpsInfo>? GpsInfo { get; set; }
        public ICollection<GpsTrack>? GpsTrack { get; set; }

        public ICollection<ImageGallery>? ImageGallery { get; set; }
        public IDictionary<string, Detail> Detail { get; set; }
        public IDictionary<string, ContactInfos> ContactInfos { get; set; }
        public IDictionary<string, AdditionalPoiInfos> AdditionalPoiInfos { get; set; }

        public ICollection<string>? SmgTags { get; set; }
        public ICollection<string>? HasLanguage { get; set; }

        //neu LTSUpdate 11.16
        public Ratings? Ratings { get; set; }
        public ICollection<string>? Exposition { get; set; }

        public string? OwnerRid { get; set; }

        public List<string>? ChildPoiIds { get; set; }
        public List<string>? MasterPoiIds { get; set; }
        
        //New
        public Nullable<int> WayNumber { get; set; }

        public string? Number { get; set; }

        public List<LTSTags>? LTSTags { get; set; }
    }

    //Erweiterte Baseinfos für ARticles
    public abstract class ArticleBaseInfos : IIdentifiable, IActivateable, IImageGalleryAware, IContactInfosAware, IAdditionalArticleInfosAware, ISmgTags, ISmgActive, IImportDateassigneable, ILicenseInfo
    {
        public LicenseInfo LicenseInfo { get; set; }

        public ArticleBaseInfos()
        {
            Detail = new Dictionary<string, Detail>();
            ContactInfos = new Dictionary<string, ContactInfos>();
            AdditionalArticleInfos = new Dictionary<string, AdditionalArticleInfos>();
            ArticleLinkInfo = new Dictionary<string, ArticleLinkInfo>();
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

        public Nullable<DateTime> ArticleDate { get; set; }

        //Mochmer des?
        public Nullable<DateTime> ArticleDateTo { get; set; }

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
    }

    //Erweiterte Baseinfos für Gastronomy
    public abstract class GastronomyBaseInfos : IIdentifiable, IActivateable, IGpsInfo, IImageGalleryAware, IContactInfosAware, ISmgTags, ISmgActive, IImportDateassigneable
    {
        public LicenseInfo LicenseInfo { get; set; }

        public GastronomyBaseInfos()
        {
            Detail = new Dictionary<string, Detail>();
            ContactInfos = new Dictionary<string, ContactInfos>();
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
        public int MaxSeatingCapacity { get; set; }

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
    }

    //Erweiterte BaseInfo für Events
    public abstract class EventBaseInfos : IIdentifiable, IActivateable, IImageGalleryAware, IGpsInfo, IContactInfosAware, ISmgTags, ISmgActive, IImportDateassigneable
    {
        public LicenseInfo LicenseInfo { get; set; }

        public EventBaseInfos()
        {
            Detail = new Dictionary<string, Detail>();
            ContactInfos = new Dictionary<string, ContactInfos>();
            OrganizerInfos = new Dictionary<string, ContactInfos>();
            EventAdditionalInfos = new Dictionary<string, EventAdditionalInfos>();
            EventPrice = new Dictionary<string, EventPrice>();
            EventPrices = new Dictionary<string, ICollection<EventPrice>>();
            EventVariants = new Dictionary<string, ICollection<EventVariant>>();
            Hashtag = new Dictionary<string, ICollection<string>>();
        }

        //IIdentifiable
        public string? Id { get; set; }
        public bool Active { get; set; }
        public string? Shortname { get; set; }

        public Nullable<DateTime> DateBegin { get; set; }
        public Nullable<DateTime> DateEnd { get; set; }

        //Infos on the Data
        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }


        //GPS Info
        public string? Gpstype { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Nullable<double> Altitude { get; set; }
        public string? AltitudeUnitofMeasure { get; set; }

        //Eventspezifische Infos
        public string? OrgRID { get; set; }
        public int Ranc { get; set; }
        public string? Ticket { get; set; }
        public string? SignOn { get; set; }
        public string? PayMet { get; set; }
        public string? Type { get; set; }
        public string Pdf { get; set; }

        public string? DistrictId { get; set; }
        //???????
        public ICollection<string>? DistrictIds { get; set; }

        //ImageGallery
        public ICollection<ImageGallery>? ImageGallery { get; set; }

        //Detail
        public IDictionary<string, Detail> Detail { get; set; }

        public ICollection<string>? TopicRIDs { get; set; }
        public ICollection<Topic>? Topics { get; set; }


        public ICollection<EventPublisher>? EventPublisher { get; set; }

        public IDictionary<string, EventAdditionalInfos> EventAdditionalInfos { get; set; }
        public IDictionary<string, EventPrice> EventPrice { get; set; }

        public ICollection<EventDate>? EventDate { get; set; }

        public IDictionary<string, ContactInfos> ContactInfos { get; set; }
        public IDictionary<string, ContactInfos> OrganizerInfos { get; set; }

        //NEU Region TV Municipality Fraktion NEU LocationInfo Classe
        public LocationInfo? LocationInfo { get; set; }

        public ICollection<string>? SmgTags { get; set; }
        public bool SmgActive { get; set; }

        public ICollection<string>? HasLanguage { get; set; }

        public Nullable<DateTime> NextBeginDate { get; set; }

        //NEW Fields 
        public string Source { get; set; }
        public bool? GrpEvent { get; set; }
        public bool? EventBenefit { get; set; }
        public EventBooking EventBooking { get; set; }
        public ICollection<LTSTags> LTSTags { get; set; }

        public IDictionary<string, ICollection<EventPrice>> EventPrices { get; set; }
        public IDictionary<string, ICollection<EventVariant>> EventVariants { get; set; }

        public IDictionary<string, ICollection<string>> Hashtag { get; set; }

        public EventOperationScheduleOverview EventOperationScheduleOverview { get; set; }
    }

    public class Topic
    {
        public string? TopicRID { get; set; }
        public string? TopicInfo { get; set; }
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

        public ICollection<string> Keywords { get; set; }

        //New LTS Fields        
        public string ParkingInfo { get; set; }
        public string PublicTransportationInfo { get; set; }
        public string AuthorTip { get; set; }
        public string SafetyInfo { get; set; }
        public string EquipmentInfo { get; set; }
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
        public Nullable<double> Altitude { get; set; }
        public string? AltitudeUnitofMeasure { get; set; }
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
        public GpsInfo? GpsInfo { get; set; }
        //Neu
        public Nullable<int> ListPosition { get; set; }

        ////NEW Webcam Properties
        public string Streamurl { get; set; }
        public string Previewurl { get; set; }

        public string Source { get; set; }

        ////NEW Webcam Properties
        //public string Streamurl { get; set; }
        //public string Previewurl { get; set; }
        //public DateTime? LastChange { get; set; }
        //public DateTime? FirstImport { get; set; }

        //public bool? Active { get; set; }

        //public string Source { get; set; }
    }

    public class WebcamLocalized
    {
        public string? WebcamId { get; set; }
        public string? Webcamname { get; set; }
        public string? Webcamurl { get; set; }
        public GpsInfo? GpsInfo { get; set; }
        public Nullable<int> ListPosition { get; set; }
    }

    public class WebcamInfo : Webcam, IIdentifiable, IImportDateassigneable
    {
        public LicenseInfo? LicenseInfo { get; set; }

        //NEW Webcam Properties
        public string? Id { get; set; }
        public new string? Streamurl { get; set; }
        public new string? Previewurl { get; set; }
        public DateTime? LastChange { get; set; }
        public DateTime? FirstImport { get; set; }
        public string? Shortname { get; set; }
        public bool? Active { get; set; }
        public bool? SmgActive { get; set; }
        public new string? Source { get; set; }
        public ICollection<PublishedonObject>? WebcamAssignedOn { get; set; }

        public ICollection<string>? AreaIds { get; set; }

        public ICollection<string>? SmgTags { get; set; }
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
        public int Width { get; set; }
        public int Height { get; set; }
        public string? ImageSource { get; set; }

        public IDictionary<string, string> ImageTitle { get; set; }
        public IDictionary<string, string> ImageDesc { get; set; }
        public IDictionary<string, string> ImageAltText { get; set; }

        public Nullable<bool> IsInGallery { get; set; }
        public Nullable<int> ListPosition { get; set; }
        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }

        //NEU
        public string? CopyRight { get; set; }
        public string? License { get; set; }
        public string? LicenseHolder { get; set; }
        public ICollection<string>? ImageTags { get; set; }
    }

    public class ImageGalleryLocalized
    {
        public string? ImageName { get; set; }
        public string? ImageUrl { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string? ImageSource { get; set; }

        public string? ImageTitle { get; set; }
        public string? ImageDesc { get; set; }
        //public string Language { get; set; }
        public Nullable<bool> IsInGallery { get; set; }
        public Nullable<int> ListPosition { get; set; }
        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }

        public string? CopyRight { get; set; }
        public string? License { get; set; }
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
        public string? MainType { get; set; }
        public string? SubType { get; set; }
        public string? PoiType { get; set; }
        public string? Language { get; set; }

        public List<string> Categories { get; set; }
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

        public ICollection<LTSTins> LTSTins { get; set; }
    }

    public class LTSTins
    {
        public LTSTins()
        {
            TinName = new Dictionary<string, string>();
        }

        public string Id { get; set; }
        public string LTSRID { get; set; }
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
        [SwaggerSchema("Will be removed withhin 2021-12-31")]
        [Obsolete]
        public bool Thuresday { get; set; }
        public bool Thursday { get { return Thuresday; } }
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

    public class EventAdditionalInfos : IEventAdditionalInfos, ILanguage
    {
        public string? Mplace { get; set; }
        public string? Reg { get; set; }
        public string? Location { get; set; }
        public string? Language { get; set; }
    }

    public class EventPrice : IEventPrice, ILanguage
    {
        public double Price { get; set; }
        public string? Type { get; set; }
        public string? Pstd { get; set; }
        public string? ShortDesc { get; set; }
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
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public bool SingleDays { get; set; }
        public int MinPersons { get; set; }
        public int MaxPersons { get; set; }
        public bool Ticket { get; set; }
        public double GpsNorth { get; set; }
        public double GpsEast { get; set; }
        public TimeSpan Begin { get; set; }
        public TimeSpan End { get; set; }
        public TimeSpan Entrance { get; set; }

        //NEW Properties
        public Nullable<double> InscriptionTill { get; set; }
        public Nullable<bool> Active { get; set; }
        public string DayRID { get; set; }

        public Dictionary<string, EventDateAdditionalInfo> EventDateAdditionalInfo { get; set; }
        public ICollection<EventDateAdditionalTime> EventDateAdditionalTime { get; set; }
        public EventDateCalculatedDay EventCalculatedDay { get; set; }
    }

    public class EventDateAdditionalInfo : ILanguage
    {
        public string Description { get; set; }
        public string Guide { get; set; }
        public string InscriptionLanguage { get; set; }
        public string Language { get; set; }
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
        public int TicketsAvailable { get; set; }
        public int MaxSellableTickets { get; set; }
        public ICollection<EventDateCalculatedDayVariant> EventDateCalculatedDayVariant { get; set; }

        //found in response
        public Nullable<int> AvailabilityCalculatedValue { get; set; }
        public Nullable<int> AvailabilityLow { get; set; }
        public Nullable<double> PriceFrom { get; set; }
    }

    public class EventDateCalculatedDayVariant
    {
        public string VarRID { get; set; }
        public double Price { get; set; }
        public Nullable<bool> IsStandardVariant { get; set; }
        public Nullable<int> TotalSellable { get; set; }
    }

    public class EventBooking
    {
        public EventBooking()
        {
            BookingUrl = new Dictionary<string, EventBookingDetail>();
        }

        public DateTime BookableFrom { get; set; }
        public DateTime BookableTo { get; set; }
        public int? AccommodationAssignment { get; set; }

        public Dictionary<string, EventBookingDetail> BookingUrl { get; set; }
    }

    public class EventBookingDetail
    {
        public string Url { get; set; }
    }

    public class EventVariant
    {
        public string VarRID { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
    }

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

    //Evalanche Spezial

    public class EvalancheMapping
    {
        public EvalancheMapping()
        {
            EvalancheArticleID = new Dictionary<string, int>();
        }

        public IDictionary<string, int> EvalancheArticleID { get; set; }

    }

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
        public IDictionary<string, string?>? Name { get; set; }
    }

    public class TvInfo
    {
        public string? Id { get; set; }
        public IDictionary<string, string?>? Name { get; set; }
    }

    public class MunicipalityInfo
    {
        public string? Id { get; set; }
        public IDictionary<string, string?>? Name { get; set; }
    }

    public class DistrictInfo
    {
        public string? Id { get; set; }
        public IDictionary<string, string?>? Name { get; set; }
    }

    public class AreaInfo
    {
        public string? Id { get; set; }
        public IDictionary<string, string>? Name { get; set; }
    }

    #endregion

    #region Evalanche

    public class EvalancheMailing
    {
        public EvalancheMailing()
        {
            ArticleIDs = new Dictionary<string, string>();
        }
        public string? Name { get; set; }
        public DateTime CreationDate { get; set; }
        public string? Type { get; set; }

        //Slot and ID of the Article
        public IDictionary<string, string> ArticleIDs { get; set; }
    }

    #endregion

    #region Resultset

    //Generic Result für Paging
    public class Result<T>
    {
        public int TotalResults { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public Nullable<int> OnlineResults { get; set; }
        public string? ResultId { get; set; }
        public string? Seed { get; set; }

        public ICollection<T>? Items { get; set; }
    }

    public class ResultAsync<T>
    {
        public int TotalResults { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public Nullable<int> OnlineResults { get; set; }

        public string? Seed { get; set; }

        public IList<T>? Items { get; set; }
    }

    #endregion

    #region Specials

    public class PackageAccos : Package
    {
        public Accommodation? Accommodation { get; set; }
    }

    #endregion

    #region EBMS

    public class EventShort : IIdentifiable
    {
        public LicenseInfo? LicenseInfo { get; set; }

        public string? Id { get; set; }
        public string? Source { get; set; }
        public string? EventLocation { get; set; }

        public int EventId { get; set; }
        //Hauptbeschreibung
        public string? EventDescription { get; set; }
        //Beschreibung DE
        public string? EventDescriptionDE { get; set; }
        //Beschreibung IT
        public string? EventDescriptionIT { get; set; }
        //Beschreibung EN
        public string? EventDescriptionEN { get; set; }
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

        //Eurac Videowall (Y / N) Wenn hier N wird ganzes Event nicht angezeigt
        public string? Display1 { get; set; }
        //Intranet Eurac (Y / N)
        public string? Display2 { get; set; }
        //Webseite Eurac ( Y /N)
        public string? Display3 { get; set; }
        //diese sind nicht belegt, könnten verwendet werden
        public string? Display4 { get; set; }
        public string? Display5 { get; set; }
        public string? Display6 { get; set; }
        public string? Display7 { get; set; }
        public string? Display8 { get; set; }
        public string? Display9 { get; set; }

        //CRM Modul Account (Firma) interessiert uns nicht
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

        //all das interessiert nicht
        //public string AbstractsEN { get; set; }
        //public string AbstractsGER { get; set; }
        //public string AbstractsIT { get; set; }
        ////gehört zu Abstract
        //public string Documents { get; set; }
        public List<ImageGallery>? ImageGallery { get; set; }
        public string? VideoUrl { get; set; }

        /// <summary>
        /// ActiveWeb Indicates if Event is shown on the Noi Website Section Events at NOI
        /// </summary>
        public Nullable<bool> ActiveWeb { get; set; }

        public string? EventTextDE { get; set; }
        public string? EventTextIT { get; set; }
        public string? EventTextEN { get; set; }

        public List<string>? TechnologyFields { get; set; }

        public List<string>? CustomTagging { get; set; }

        public bool? SoldOut { get; set; }
        public List<DocumentPDF>? EventDocument { get; set; }

        public bool? ExternalOrganizer { get; set; }

        public string? Shortname { get; set; }
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

    }

    public class EventShortByRoom
    {
        public EventShortByRoom()
        {
            SpaceDescList = new List<string>();
            TechnologyFields = new List<string>();
            CustomTagging = new List<string>();
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

        public Dictionary<string, string> EventDescription { get; set; }

        public string? EventDescriptionDE { get; set; }
        public string? EventDescriptionIT { get; set; }
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

        public string? EventTextDE { get; set; }
        public string? EventTextIT { get; set; }
        public string? EventTextEN { get; set; }

        public List<string>? TechnologyFields { get; set; }
        public List<string>? CustomTagging { get; set; }
        public bool? SoldOut { get; set; }

        public Dictionary<string, string> EventDocument { get; set; }

        public bool? ExternalOrganizer { get; set; }
        //public string MapsNoiUrl { get; set; }
    }

    public class DocumentPDF
    {
        public string? DocumentURL { get; set; }
        public string? Language { get; set; }
    }

    #endregion

    public class MetaInfosOdhActivityPoi
    {
        public MetaInfosOdhActivityPoi()
        {
            Metainfos = new Dictionary<string, List<Dictionary<string, object>>>();
        }
        public string? Id { get; set; }

        public Dictionary<string, List<Dictionary<string, object>>> Metainfos { get; set; }
    }

    #region MetaInfo

    public class Metadata
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public Nullable<DateTime> LastUpdate { get; set; }

        //New
        public string Source { get; set; }
    }

    #endregion

    #region LicenseInfo

    public class LicenseInfo
    {
        //public string DataType { get; set; }
        public string? License { get; set; }
        public string? LicenseHolder { get; set; }
        public string? Author { get; set; }
        public bool ClosedData { get; set; }
    }

    #endregion
}
