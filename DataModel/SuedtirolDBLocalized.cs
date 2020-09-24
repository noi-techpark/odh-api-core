using System;
using System.Collections.Generic;

namespace DataModel
{
    public class AccommodationLocalized
    {
        public string? Id { get; set; }
        public string? HgvId { get; set; }
        public string? Shortname { get; set; }
        public int Units { get; set; }
        public int Beds { get; set; }
        public bool HasApartment { get; set; }
        public bool HasRoom { get; set; }
        public bool IsBookable { get; set; }
        public bool SmgActive { get; set; }
        public bool TVMember { get; set; }
        public string? TourismVereinId { get; set; }
        public string? MainLanguage { get; set; }
        public DateTime FirstImport { get; set; }
        public DateTime LastChange { get; set; }
        public string? Gpstype { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Nullable<double> Altitude { get; set; }
        public string? AltitudeUnitofMeasure { get; set; }

        public string? AccoCategoryId { get; set; }
        public string? AccoTypeId { get; set; }
        public string? DistrictId { get; set; }

        public ICollection<string>? BoardIds { get; set; }
        //public ICollection<string> MarketingGroupIds { get; set; }
        //public ICollection<AccoFeature> Features { get; set; }

        //Custom
        public ICollection<string>? BadgeIds { get; set; }
        public ICollection<string>? ThemeIds { get; set; }
        public ICollection<string>? SpecialFeaturesIds { get; set; }

        public AccoDetail? AccoDetail { get; set; }
        public ICollection<AccoBookingChannel>? AccoBookingChannel { get; set; }
        public ICollection<ImageGalleryLocalized>? ImageGallery { get; set; }

        //NEU Region TV Municipality Fraktion NEU LocationInfo Classe
        public LocationInfoLocalized? LocationInfo { get; set; }

        //Gastronomy 
        public string? GastronomyId { get; set; }
        public ICollection<string>? SmgTags { get; set; }

        public string? TrustYouID { get; set; }
        public double TrustYouScore { get; set; }
        public int TrustYouResults { get; set; }
        public bool TrustYouActive { get; set; }
        public int TrustYouState { get; set; }

        public ICollection<MssResponseShort>? MssResponseShort { get; set; }
    }

    public class ArticleBaseInfosLocalized
    {
        public string? Id { get; set; }
        public bool Active { get; set; }
        public string? Shortname { get; set; }
        public bool Highlight { get; set; }

        //Activity SubType
        public string? Type { get; set; }
        public string? SubType { get; set; }

        //NEU SMG Infos
        public DateTime FirstImport { get; set; }
        public DateTime LastChange { get; set; }
        public bool SmgActive { get; set; }

        //OperationSchedule
        public ICollection<OperationSchedule>? OperationSchedule { get; set; }

        public ICollection<GpsInfo>? GpsInfo { get; set; }
        public ICollection<GpsTrack>? GpsTrack { get; set; }

        public ICollection<ImageGalleryLocalized>? ImageGallery { get; set; }
        public Detail? Detail { get; set; }
        public ContactInfos? ContactInfos { get; set; }
        public AdditionalArticleInfos? AdditionalArticleInfos { get; set; }

        public ICollection<string>? SmgTags { get; set; }
    }

    public class BaseInfosLocalized
    {
        public string? Id { get; set; }
        public bool Active { get; set; }
        public string? CustomId { get; set; }
        public string? Shortname { get; set; }
        public string? Gpstype { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Nullable<double> Altitude { get; set; }
        public string? AltitudeUnitofMeasure { get; set; }

        public Detail? Detail { get; set; }
        public ContactInfos? ContactInfos { get; set; }
        public ICollection<ImageGalleryLocalized>? ImageGallery { get; set; }

        public ICollection<string>? SmgTags { get; set; }

        public bool SmgActive { get; set; }
    }

    public class RegionLocalized : BaseInfosLocalized
    {
        public DetailThemed? DetailThemed { get; set; }

        public ICollection<GpsPolygon>? GpsPolygon { get; set; }
        public ICollection<WebcamLocalized>? Webcam { get; set; }
        public bool VisibleInSearch { get; set; }
        public ICollection<string>? SkiareaIds { get; set; }
    }

    public class TourismvereinLocalized : BaseInfosLocalized
    {
        public string? RegionId { get; set; }

        public ICollection<GpsPolygon>? GpsPolygon { get; set; }
        public ICollection<WebcamLocalized>? Webcam { get; set; }
        public bool VisibleInSearch { get; set; }
        public ICollection<string>? SkiareaIds { get; set; }
    }

    public class MunicipalityLocalized : BaseInfosLocalized
    {
        public string? Plz { get; set; }

        public string? RegionId { get; set; }
        public string? TourismvereinId { get; set; }
        public string? SiagId { get; set; }

        public ICollection<GpsPolygon>? GpsPolygon { get; set; }
        public ICollection<WebcamLocalized>? Webcam { get; set; }
        public bool VisibleInSearch { get; set; }

        public int Inhabitants { get; set; }
        public string? IstatNumber { get; set; }
    }

    public class DistrictLocalized : BaseInfosLocalized
    {
        public Nullable<bool> IsComune { get; set; }
        public string? RegionId { get; set; }
        public string? TourismvereinId { get; set; }
        public string? MunicipalityId { get; set; }

        public string? SiagId { get; set; }

        public ICollection<GpsPolygon>? GpsPolygon { get; set; }
        public ICollection<WebcamLocalized>? Webcam { get; set; }
        public bool VisibleInSearch { get; set; }
    }

    public class MetaRegionLocalized : BaseInfosLocalized
    {
        public DetailThemed? DetailThemed { get; set; }
        public ICollection<string>? DistrictIds { get; set; }
        public ICollection<string>? TourismvereinIds { get; set; }
        public ICollection<string>? RegionIds { get; set; }

        public ICollection<GpsPolygon>? GpsPolygon { get; set; }
        public ICollection<WebcamLocalized>? Webcam { get; set; }
        public bool VisibleInSearch { get; set; }
    }

    public class ExperienceAreaLocalized : BaseInfosLocalized
    {
        public ICollection<string>? DistrictIds { get; set; }
        public ICollection<string>? TourismvereinIds { get; set; }

        public ICollection<GpsPolygon>? GpsPolygon { get; set; }
        //public ICollection<WebcamLocalized> Webcam { get; set; }
        public bool VisibleInSearch { get; set; }
    }

    public class SkiRegionLocalized : BaseInfosLocalized
    {
        public ICollection<GpsPolygon>? GpsPolygon { get; set; }

        public ICollection<WebcamLocalized>? Webcam { get; set; }
    }

    public class SkiAreaLocalized : BaseInfosLocalized
    {
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


        public string? SkiRegionName { get; set; }

        public ICollection<string>? AreaId { get; set; }
        public ICollection<WebcamLocalized>? Webcam { get; set; }
        //NEU Region TV Municipality Fraktion NEU LocationInfo Classe
        public LocationInfoLocalized? LocationInfo { get; set; }

        //Folsch
        //public OperationSchedule OperationSchedule { get; set; }
        public ICollection<OperationSchedule>? OperationSchedule { get; set; }

        public ICollection<string>? TourismvereinIds { get; set; }
        public ICollection<string>? RegionIds { get; set; }

        public ICollection<GpsPolygon>? GpsPolygon { get; set; }
    }

    public class DataForList
    {
        public string? Name { get; set; }
        public string? TV { get; set; }
        public string? Region { get; set; }
        public string? Type { get; set; }
        public string? Subtype { get; set; }
        public List<string>? Category { get; set; }
    }

    public class EventLocalized
    {
        //IIdentifiable
        public string? Id { get; set; }
        public bool Active { get; set; }
        public string? Shortname { get; set; }

        public Nullable<DateTime> DateBegin { get; set; }
        public Nullable<DateTime> DateEnd { get; set; }

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

        public string? DistrictId { get; set; }
        //???????
        public ICollection<string>? DistrictIds { get; set; }

        //ImageGallery
        public ICollection<ImageGalleryLocalized>? ImageGallery { get; set; }

        //Detail
        public Detail? Detail { get; set; }

        public ICollection<string>? TopicRIDs { get; set; }

        public ICollection<EventPublisher>? EventPublisher { get; set; }

        public EventAdditionalInfos? EventAdditionalInfos { get; set; }
        public EventPrice? EventPrice { get; set; }

        public ICollection<EventDate>? EventDate { get; set; }

        public ContactInfos? ContactInfos { get; set; }
        public ContactInfos? OrganizerInfos { get; set; }

        //NEU Region TV Municipality Fraktion NEU LocationInfo Classe
        public LocationInfoLocalized? LocationInfo { get; set; }

        public ICollection<string>? SmgTags { get; set; }
        public bool SmgActive { get; set; }

        //public ICollection<string> HasLanguage { get; set; }
    }

    public class GastronomyLocalized
    {
        public string? Id { get; set; }
        public bool Active { get; set; }
        public string? Shortname { get; set; }

        public string? Type { get; set; }

        // Fraktion 
        public string? DistrictId { get; set; }

        public DateTime FirstImport { get; set; }
        public DateTime LastChange { get; set; }

        //GPS Info
        public string? Gpstype { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Nullable<double> Altitude { get; set; }
        public string? AltitudeUnitofMeasure { get; set; }

        //OperationSchedule
        public ICollection<OperationSchedule>? OperationSchedule { get; set; }

        //CapacityCeremony
        public int MaxSeatingCapacity { get; set; }

        //public ICollection<GpsInfo> GpsInfo { get; set; }
        public ICollection<ImageGalleryLocalized>? ImageGallery { get; set; }
        public Detail? Detail { get; set; }
        public ContactInfos? ContactInfos { get; set; }

        public ICollection<CategoryCodes>? CategoryCodes { get; set; }
        public ICollection<DishRates>? DishRates { get; set; }
        public ICollection<CapacityCeremony>? CapacityCeremony { get; set; }
        public ICollection<Facilities>? Facilities { get; set; }

        public ICollection<string>? MarketinggroupId { get; set; }

        //NEU Region TV Municipality Fraktion NEU LocationInfo Classe
        public LocationInfoLocalized? LocationInfo { get; set; }

        public string? AccommodationId { get; set; }

        public ICollection<string>? SmgTags { get; set; }
        public bool SmgActive { get; set; }

        //public ICollection<string> HasLanguage { get; set; }
    }

    public class MobileData
    {
        public string? Id { get; set; }                              //ID of the Data
        public string? Name { get; set; }                            //Name of the Data in the requested language
        public string? Image { get; set; }                           //First Image (for the size of the Image i can append the imageresizer functionality, is something like &width=150
        public string? Region { get; set; }                          //Name of the Region in the requested language
        public string? Tourismverein { get; set; }                   //Name of the Tourismverein in the requested language
        public string? Type { get; set; }                            //Type of the data (acco, gastro, event, smgpoi)
        public string? Gpstype { get; set; } //?brauchi des
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Dictionary<string, string>? Category { get; set; }    //The first Question what to do here ;)
        public Dictionary<string, string>? Additional { get; set; }  //All Props needed to display as an Associative Array
    }

    public class MobileDataV2 : MobileData
    {
        public string? TypeTripplaner { get; set; }
        //NEU
        public Nullable<DateTime> LastChanged { get; set; }
    }

    public class MobileDetail
    {
        //Common Info VALID for all Datatypes
        public string? Id { get; set; }
        public string? MainType { get; set; }

        public ICollection<GpsInfo>? GpsInfo { get; set; }
        public ICollection<GpsTrack>? GpsTrack { get; set; }
        public ICollection<ImageGalleryLocalized>? ImageGallery { get; set; }
        public LocationInfoLocalized? LocationInfo { get; set; }
        public ICollection<OperationSchedule>? OperationSchedule { get; set; }
        public Detail? Detail { get; set; }
        public ContactInfos? ContactInfos { get; set; }
        public ICollection<string>? SmgTags { get; set; }

        //public ICollection<PoiProperty> PoiProperty { get; set; }
        public IDictionary<string, string>? PoiProperty { get; set; }
        public ICollection<string>? PoiServices { get; set; }


        //Specific on SMGPois (Activities Pois Gastronomies)        
        public string? Type { get; set; }
        public string? SubType { get; set; }
        public string? PoiType { get; set; }

        public bool Highlight { get; set; }
        public string? Difficulty { get; set; }              //Only valid on a Activity else 0
        public double AltitudeDifference { get; set; }      //Only valid on a Activity else 0
        public double AltitudeHighestPoint { get; set; }    //Only valid on a Activity else 0    
        public double AltitudeLowestPoint { get; set; }     //Only valid on a Activity else 0
        public double AltitudeSumUp { get; set; }           //Only valid on a Activity else 0
        public double AltitudeSumDown { get; set; }         //Only valid on a Activity else 0
        public double DistanceDuration { get; set; }        //Only valid on a Activity else 0
        public double DistanceLength { get; set; }          //Only valid on a Activity else 0

        public bool IsOpen { get; set; }                    //Only valid on a Activity Poi Gastronomy else false
        public bool IsPrepared { get; set; }                //Only valid on a Activity else false
        public bool RunToValley { get; set; }               //Only valid on a Activity else false
        public bool IsWithLigth { get; set; }               //Only valid on a Activity else false
        public bool HasRentals { get; set; }                //Only valid on a Activity else false
        public bool HasFreeEntrance { get; set; }           //Only valid on a Activity & Poi else false
        public bool LiftAvailable { get; set; }             //Only valid on a Activity else false
        public bool FeetClimb { get; set; }                 //Only valid on a Activity else false

        public int MaxSeatingCapacity { get; set; }         //Only valid on a Gastronomy else 0

        public Ratings? Ratings { get; set; }                //Only valid on a Activity else empty
        public ICollection<string>? Exposition { get; set; } //Only valid on a Activity else 0

        public string? Novelty { get; set; }                 //Only valid on a Activity & Poi


        //Specific on Events        
        public Nullable<DateTime> DateBegin { get; set; }
        public Nullable<DateTime> DateEnd { get; set; }
        public int Ranc { get; set; }
        public string? Ticket { get; set; }
        public string? SignOn { get; set; }
        public string? PayMet { get; set; }
        public ICollection<EventDate>? EventDate { get; set; }
        public ContactInfos? OrganizerInfos { get; set; }

    }

    public class MobileDetailV2 : MobileDetail
    {
        public string? TypeTripplaner { get; set; }
        //NEU
        public Nullable<DateTime> LastChanged { get; set; }

        public List<Suggestion>? Suggestion_android { get; set; }
        public List<Suggestion>? Suggestion_ios { get; set; }

        public string? Web_detail_url { get; set; }

        public string? OutdoorActiveID { get; set; }
        public string? OutdoorActiveElevationID { get; set; }

        //NEU
        public IDictionary<string, string>? LinkedAppSuggestions { get; set; }
        public bool? overWriteAppSuggestions { get; set; }
    }

    public class MobileAccoDetailV2 : AccommodationLocalized
    {
        public string? TypeTripplaner { get; set; }
        public string? MainType { get; set; }
        //NEU
        public Nullable<DateTime> LastChanged { get; set; }

        public List<Suggestion>? Suggestion_android { get; set; }
        public List<Suggestion>? Suggestion_ios { get; set; }

        public string? Web_detail_url { get; set; }

        //NEU
        //public IDictionary<string, string> LinkedAppSuggestions { get; set; }
        //public bool? overWriteAppSuggestions { get; set; }

    }

    public class MobileDataExtended : MobileData
    {
        public string? ShortDesc { get; set; }                              //ShortDesc of the Data (for Tips)

    }

    public class MobileDataExtendedV2 : MobileDataV2
    {
        public string? ShortDesc { get; set; }                              //ShortDesc of the Data (for Tips)
    }

    public class BookingReservation
    {
        public string? Id { get; set; }

        public DateTime from { get; set; }
        public DateTime to { get; set; }

        public string? UserEmail { get; set; }
        public string? AccommodationId { get; set; }

        public double BookingPrice { get; set; }
        public DateTime BookingDate { get; set; }

        public int Rooms { get; set; }
        public string? Persons { get; set; }
        //Roominfo?
    }

    public class PackageLocalized
    {
        //IIdentifiable
        public string? Id { get; set; }
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

        public int DaysArrival { get; set; }
        public int DaysDeparture { get; set; }

        public int DaysDurMin { get; set; }
        public int DaysDurMax { get; set; }

        public int DaysArrivalMin { get; set; }
        public int DaysArrivalMax { get; set; }

        public int ChildrenMin { get; set; }

        public ICollection<ImageGalleryLocalized>? ImageGallery { get; set; }
        public PackageDetail? PackageDetail { get; set; }

        public IDictionary<string, string>? ChannelInfo { get; set; }

        public ICollection<InclusiveLocalized>? Inclusive { get; set; }
        public ICollection<PackageThemeLocalized>? PackageThemeDetail { get; set; }

        public ICollection<Season>? Season { get; set; }

        public List<string>? Services { get; set; }

        //NEU Region TV Municipality Fraktion NEU LocationInfo Classe
        public LocationInfoLocalized? LocationInfo { get; set; }

        public ICollection<MssResponseShort>? MssResponseShort { get; set; }
    }

    public class PoiBaseInfosLocalized
    {
        public string? Id { get; set; }
        public bool Active { get; set; }
        public string? Shortname { get; set; }
        public string? SmgId { get; set; }
        public bool Highlight { get; set; }
        public string? Difficulty { get; set; }
        //Activity SubType
        public string? Type { get; set; }
        public string? SubType { get; set; }

        //NEU SMG Infos
        public DateTime FirstImport { get; set; }
        public DateTime LastChange { get; set; }
        public bool SmgActive { get; set; }

        //NEU Region TV Municipality Fraktion NEU LocationInfo Classe
        public LocationInfoLocalized? LocationInfo { get; set; }

        public string? TourismorganizationId { get; set; }
        public ICollection<string>? AreaId { get; set; }

        //Distance & Altitude Informationen
        public double AltitudeDifference { get; set; }
        public double AltitudeHighestPoint { get; set; }
        public double AltitudeLowestPoint { get; set; }
        public double AltitudeSumUp { get; set; }
        public double AltitudeSumDown { get; set; }
        public double DistanceDuration { get; set; }
        public double DistanceLength { get; set; }



        //Status & Features
        public bool IsOpen { get; set; }
        public bool IsPrepared { get; set; }
        public bool RunToValley { get; set; }
        public bool IsWithLigth { get; set; }
        public bool HasRentals { get; set; }
        public bool HasFreeEntrance { get; set; }
        public bool LiftAvailable { get; set; }
        public bool FeetClimb { get; set; }
        public Nullable<bool> BikeTransport { get; set; }


        //Für mearere aso
        public ICollection<OperationSchedule>? OperationSchedule { get; set; }

        public ICollection<GpsInfo>? GpsInfo { get; set; }
        public ICollection<GpsTrack>? GpsTrack { get; set; }

        public ICollection<ImageGalleryLocalized>? ImageGallery { get; set; }
        public Detail? Detail { get; set; }
        public ContactInfos? ContactInfos { get; set; }
        public AdditionalPoiInfos? AdditionalPoiInfos { get; set; }

        public Ratings? Ratings { get; set; }
        public ICollection<string>? Exposition { get; set; }

        public ICollection<string>? SmgTags { get; set; }
    }

    public class LTSPoiLocalized : PoiBaseInfosLocalized
    {
        public List<LTSTagsLocalized>? LTSTags { get; set; }
    }

    public class LTSTagsLocalized
    {
        public string? Id { get; set; }
        public int Level { get; set; }
        public string? TagName { get; set; }
    }

    public class GBLTSActivityPoiLocalized : PoiBaseInfosLocalized
    {
        public IDictionary<string, GpsInfo>? GpsPoints { get; set; }
        public List<LTSTagsLocalized>? LTSTags { get; set; }
    }

    public class SmgPoiLocalized : PoiBaseInfosLocalized
    {
        //Des isch no ordentlich zu mochn

        public string? PoiType { get; set; }

        public ICollection<PoiProperty>? PoiProperty { get; set; }
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
    }

    public class ODHActivityPoiLocalized : SmgPoiLocalized
    {
        public ODHActivityPoiLocalized()
        {
            GpsPoints = new Dictionary<string, GpsInfo>();
        }

        public IDictionary<string, GpsInfo> GpsPoints { get; set; }
    }
}
