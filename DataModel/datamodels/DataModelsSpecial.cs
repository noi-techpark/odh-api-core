// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;

namespace DataModel
{
    #region Specials

    public class PackageAccos : Package
    {
        public Accommodation? Accommodation { get; set; }
    }

    public class MetaInfosOdhActivityPoi
    {
        public MetaInfosOdhActivityPoi()
        {
            Metainfos = new Dictionary<string, List<Dictionary<string, object>>>();
        }
        public string? Id { get; set; }

        public Dictionary<string, List<Dictionary<string, object>>> Metainfos { get; set; }
    }

    #endregion
    public class AccoBookList
    {        
        public string? Id { get; set; }
        public bool IsBookable { get; set; }
        public ICollection<AccoBookingChannel>? AccoBookingChannel { get; set; }
    }

    public class AccoBookListRaw
    {        
        public string? Id { get; set; }
        public bool IsBookable { get; set; }
        public JsonRaw AccoBookingChannel { get; set; }
    }

    public class AccoCustom
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? District { get; set; }
        public string? Municipality { get; set; }
        public string? Region { get; set; }
    }

    public class AccoListObject
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Type { get; set; }
        public string? SuedtirolinfoLink { get; set; }

        public string? District { get; set; }
        public string? Municipality { get; set; }
        public string? Tourismverein { get; set; }
        public string? Region { get; set; }

        public ICollection<ImageGallery>? ImageGallery { get; set; }

        public string? TrustYouID { get; set; }
        public double TrustYouScore { get; set; }
        public int TrustYouResults { get; set; }

        public ICollection<MssResponseShort>? MssResponseShort { get; set; }

        public ICollection<string>? Tags { get; set; }
    }

    public class AccommodationReducedExtended
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? ThemeIds { get; set; }
    }

    public class AccoReducedForMap
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? ThemeIds { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public AccoDetail? AccoDetail { get; set; }
    }

    public class AccommodationReduced
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class WebcamInfoReduced
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class ActivityPoiReduced
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class ActivityPoiSmgId
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? SubType { get; set; }
        public string? SmgId { get; set; }
        public string? SmgText { get; set; }
        public string? BaseText { get; set; }
    }

    public class AreaLocationInfo
    {
        public string? Id { get; set; }
        public string? AreaGID { get; set; }
        public string? AreaName { get; set; }
        public string? AreaType { get; set; }
        public string? RegionId { get; set; }
        public string? RegionName { get; set; }
        public string? TourismvereinId { get; set; }
        public string? TourismvereinName { get; set; }
    }

    public class ArticleReduced
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class EventReduced
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class EventShortReduced
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class ExperienceAreaName
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public ICollection<string>? DistrictIds { get; set; }
        public ICollection<string>? TourismvereinIds { get; set; }
        public bool VisibleInSearch { get; set; }
    }

    public class GastronomyReduced
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class DistrictName
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? RegionId { get; set; }
        public string? TourismvereinId { get; set; }
        public string? MunicipalityId { get; set; }
        public bool VisibleInSearch { get; set; }
    }

    public class GastronomyRelatedContentReduced
    {
        public string? Id { get; set; }
        public string? Type { get; set; }
        public string? Name { get; set; }
    }

    public class ImageResult
    {
        public string? Id { get; set; }
        public ICollection<ImageGallery>? Images { get; set; }
    }

    public class MobileHtmlLocalized
    {
        public string? Id { get; set; }
        public string? HtmlText { get; set; }
    }

    public class MunicipalityName
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? RegionId { get; set; }
        public string? TourismvereinId { get; set; }
        public bool VisibleInSearch { get; set; }
    }

    public class PackageBookList
    {
        public string? Id { get; set; }
        public string? OfferId { get; set; }
        public List<string>? HotelHgvId { get; set; }
        public List<string>? HotelId { get; set; }
    }

    public class PackageReduced
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class RegionName
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class SkiAreaName
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? SkiRegionId { get; set; }
    }

    public class SkiRegionName
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class SmgPoiReducedForMap
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public ContactInfos? ContactInfo { get; set; }
    }

    public class SmgPoiReduced
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class SmgPoiRelatedContentReduced
    {
        public string? Id { get; set; }
        public string? Type { get; set; }
        public string? Name { get; set; }
    }

    public class SmgTagReduced
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class TourismvereinName
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? RegionId { get; set; }
    }

    public class TutorialLocalized
    {
        public string? Id { get; set; }
        public string? image_url { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public string? language { get; set; }
    }

    public class CommonReduced
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }



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

    //Evalanche Spezial
    public class EvalancheMapping
    {
        public EvalancheMapping()
        {
            EvalancheArticleID = new Dictionary<string, int>();
        }

        public IDictionary<string, int> EvalancheArticleID { get; set; }

    }

    #endregion
}
