using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataModel
{
    /// <summary>
    /// This class contains the classes used by Open Data Hub PG Instance with linked data
    /// </summary>
    #region Linked Classes

    public class AccoFeatureLinked : AccoFeature
    {
        public string Self
        {
            get
            {
                return "AccommodationFeatures/" + Uri.EscapeDataString(this.Id);
            }
        }
    }

    public class AccoRoomInfoLinked : AccoRoomInfo
    {
        public string Self
        {
            get
            {
                return "AccommodationRoom/" + Uri.EscapeDataString(this.Id);
            }
        }
    }

    public class TopicLinked : Topic
    {
        public string Self
        {
            get
            {
                return "EventTopics/" + Uri.EscapeDataString(this.TopicRID);
            }
        }
    }

    public class CategoryCodesLinked : CategoryCodes
    {
        public string Self
        {
            get
            {
                return String.IsNullOrEmpty(this.Id) ? null : "GastronomyTypes/" + Uri.EscapeDataString(this.Id);
            }
        }
    }

    public class FacilitiesLinked : Facilities
    {
        public string Self
        {
            get
            {
                return String.IsNullOrEmpty(this.Id) ? null : "GastronomyTypes/" + Uri.EscapeDataString(this.Id);
            }
        }
    }

    public class CapacityCeremonyLinked : CapacityCeremony
    {
        public string Self
        {
            get
            {
                return String.IsNullOrEmpty(this.Id) ? null : "GastronomyTypes/" + Uri.EscapeDataString(this.Id);
            }
        }
    }

    public class DishRatesLinked : DishRates
    {
        public string Self
        {
            get
            {
                return String.IsNullOrEmpty(this.Id) ? null : "GastronomyTypes/" + Uri.EscapeDataString(this.Id);
            }
        }
    }

    public class RegionInfoLinked : RegionInfo
    {
        public string Self
        {
            get
            {
                return String.IsNullOrEmpty(this.Id) ? null : "Region/" + Uri.EscapeDataString(this.Id);
            }
        }
    }

    public class TvInfoLinked : TvInfo
    {
        public string Self
        {
            get
            {
                return String.IsNullOrEmpty(this.Id) ? null : "TourismAssociation/" + Uri.EscapeDataString(this.Id);
            }
        }
    }

    public class MunicipalityInfoLinked : MunicipalityInfo
    {
        public string Self
        {
            get
            {
                return String.IsNullOrEmpty(this.Id) ? null : "Municipality/" + Uri.EscapeDataString(this.Id);
            }
        }
    }

    public class DistrictInfoLinked : DistrictInfo
    {
        public string Self
        {
            get
            {
                return String.IsNullOrEmpty(this.Id) ? null : "District/" + Uri.EscapeDataString(this.Id);
            }
        }
    }

    public class AreaInfoLinked : AreaInfo
    {
        public string Self
        {
            get
            {
                return String.IsNullOrEmpty(this.Id) ? null : "Area/" + Uri.EscapeDataString(this.Id);
            }
        }
    }

    public class LocationInfoLinked : LocationInfo
    {
        public new RegionInfoLinked? RegionInfo { get; set; }
        public new TvInfoLinked? TvInfo { get; set; }
        public new MunicipalityInfoLinked? MunicipalityInfo { get; set; }
        public new DistrictInfoLinked? DistrictInfo { get; set; }
        public new AreaInfoLinked? AreaInfo { get; set; }
    }

    public class LTSTagsLinked : LTSTags
    {
        public string Self
        {
            get
            {
                return String.IsNullOrEmpty(this.Id) ? null : ODHConstant.ApplicationURL + "ODHTag/" + this.Id;
            }
        }
    }

    public class ODHTags
    {
        public string Id { get; set; }
        public string Self { get; set; }
    }

    //NEW Tags GENERIC
    public class Tags
    {
        public string Id { get; set; }

        public string Source { get; set; }
        public string Self
        {
            get
            {
                return ODHConstant.ApplicationURL + "Tag/" + this.Id;
            }
        }
    }


    public class ODHActivityPoiTypesLink
    {
        public string Id { get; set; }
        public string Self { get; set; }
        public string Type { get; set; }
    }

    public class AccoCategory
    {
        public string Id { get; set; }
        public string Self { get; set; }
    }

    public class AccoType
    {
        public string Id { get; set; }
        public string Self { get; set; }
    }

    public class AccoBoards
    {
        public string Id { get; set; }
        public string Self { get; set; }
    }

    public class AccoBadges
    {
        public string Id { get; set; }
        public string Self { get; set; }
    }

    public class AccoThemes
    {
        public string Id { get; set; }
        public string Self { get; set; }
    }

    public class AccoSpecialFeatures
    {
        public string Id { get; set; }
        public string Self { get; set; }
    }

    public class DistrictLink
    {
        public string Id { get; set; }
        public string Self { get; set; }
    }

    public class TourismAssociationLink
    {
        public string Id { get; set; }
        public string Self { get; set; }
    }

    public class RegionLink
    {
        public string Id { get; set; }
        public string Self { get; set; }
    }

    public class MunicipalityLink
    {
        public string Id { get; set; }
        public string Self { get; set; }
    }

    public class AreaLink
    {
        public string Id { get; set; }
        public string Self { get; set; }
    }

    public class SkiAreaLink
    {
        public string Id { get; set; }
        public string Self { get; set; }
    }

    public class SkiRegionLink
    {
        public string Id { get; set; }
        public string Self { get; set; }
    }

    public class CompanyLink
    {
        public string Id { get; set; }
        public string Self { get; set; }
    }

    public class GastronomyLinked : Gastronomy, IMetaData
    {
        public Metadata _Meta { get; set; }

        public string Self
        {
            get
            {
                return "Gastronomy/" + Uri.EscapeDataString(this.Id);
            }
        }

        public bool OdhActive
        {
            get
            {
                return this.SmgActive;
            }
        }

        //Taglist
        public ICollection<ODHTags> ODHTags
        {
            get
            {
                return this.SmgTags != null ? this.SmgTags.Select(x => new ODHTags() { Id = x, Self = ODHConstant.ApplicationURL + "ODHTag/" + x }).ToList() : new List<ODHTags>();
            }
        }

        //Overwriting Categorycodes etc...
        public new ICollection<CategoryCodesLinked>? CategoryCodes { get; set; }
        public new ICollection<DishRatesLinked>? DishRates { get; set; }
        public new ICollection<CapacityCeremonyLinked>? CapacityCeremony { get; set; }
        public new ICollection<FacilitiesLinked>? Facilities { get; set; }

        //Overwrites The LocationInfo
        public new LocationInfoLinked? LocationInfo { get; set; }
    }

    public class AccommodationLinked : Accommodation, IMetaData
    {
        public Metadata _Meta { get; set; }

        public string Self
        {
            get
            {
                return "Accommodation/" + Uri.EscapeDataString(this.Id);
            }
        }

        public bool OdhActive
        {
            get
            {
                return this.SmgActive;
            }
        }

        //Taglist
        public ICollection<ODHTags> ODHTags
        {
            get
            {
                return this.SmgTags != null ? this.SmgTags.Select(x => new ODHTags() { Id = x, Self = ODHConstant.ApplicationURL + "ODHTag/" + x }).ToList() : new List<ODHTags>();
            }
        }

        public AccoType AccoType
        {
            get
            {
                return String.IsNullOrEmpty(this.AccoTypeId) ? null : new AccoType() { Id = this.AccoTypeId, Self = ODHConstant.ApplicationURL + "AccommodationTypes/" + Uri.EscapeDataString(this.AccoTypeId) };
            }
        }

        public AccoCategory AccoCategory
        {
            get
            {
                return String.IsNullOrEmpty(this.AccoCategoryId) ? null : new AccoCategory() { Id = this.AccoCategoryId, Self = ODHConstant.ApplicationURL + "AccommodationTypes/" + Uri.EscapeDataString(this.AccoCategoryId) };
            }
        }

        public ICollection<AccoBoards> AccoBoards
        {
            get
            {
                return this.BoardIds != null ? this.BoardIds.Select(x => new AccoBoards() { Id = x, Self = ODHConstant.ApplicationURL + "AccommodationTypes/" + x }).ToList() : new List<AccoBoards>();
            }
        }

        public ICollection<AccoBadges> AccoBadges
        {
            get
            {
                return this.BadgeIds != null ? this.BadgeIds.Select(x => new AccoBadges() { Id = x, Self = ODHConstant.ApplicationURL + "AccommodationTypes/" + x }).ToList() : new List<AccoBadges>();
            }
        }

        public ICollection<AccoThemes> AccoThemes
        {
            get
            {
                return this.ThemeIds != null ? this.ThemeIds.Select(x => new AccoThemes() { Id = x, Self = ODHConstant.ApplicationURL + "AccommodationTypes/" + x }).ToList() : new List<AccoThemes>();
            }
        }

        public ICollection<AccoSpecialFeatures> AccoSpecialFeatures
        {
            get
            {
                return this.SpecialFeaturesIds != null ? this.SpecialFeaturesIds.Select(x => new AccoSpecialFeatures() { Id = x, Self = ODHConstant.ApplicationURL + "AccommodationTypes/" + x }).ToList() : new List<AccoSpecialFeatures>();
            }
        }

        //Overwrites The Features
        public new ICollection<AccoFeatureLinked>? Features { get; set; }

        //Overwrites The Features
        public new ICollection<AccoRoomInfoLinked>? AccoRoomInfo { get; set; }

        //Overwrites The LocationInfo
        public new LocationInfoLinked? LocationInfo { get; set; }
    }

    public class AccommodationRoomLinked : AccoRoom, IMetaData
    {
        public Metadata _Meta { get; set; }

        public string Self
        {
            get
            {
                return "AccommodationRoom/" + Uri.EscapeDataString(this.Id);
            }
        }

        //Overwrites The Features
        public new ICollection<AccoFeatureLinked>? Features { get; set; }       
    }

    public class EventPG : Event
    {
        public List<DateTime> EventDatesBegin { get; set; }
        public List<DateTime> EventDatesEnd { get; set; }

        public int EventDateCounter { get; set; }
    }

    public class EventLinked : Event, IMetaData
    {
        public Metadata _Meta { get; set; }

        public List<DateTime> EventDatesBegin
        {
            get
            {
                return this.EventDate != null ? this.EventDate.Select(x => x.From).ToList() : null;
            }
        }

        public List<DateTime> EventDatesEnd
        {
            get
            {
                return this.EventDate != null ? this.EventDate.Select(x => x.To).ToList() : null;
            }
        }

        public int EventDateCounter
        {
            get
            {
                return this.EventDate != null ? this.EventDate.Count : 0;
            }
        }

        public string Self
        {
            get
            {
                return "Event/" + Uri.EscapeDataString(this.Id);
            }
        }

        public ICollection<DistrictLink> Districts
        {
            get
            {
                return this.DistrictIds != null ? this.DistrictIds.Select(x => new DistrictLink() { Id = x, Self = ODHConstant.ApplicationURL + "District/" + x }).ToList() : new List<DistrictLink>();
            }
        }

        //Taglist
        public ICollection<ODHTags> ODHTags
        {
            get
            {
                return this.SmgTags != null ? this.SmgTags.Select(x => new ODHTags() { Id = x, Self = ODHConstant.ApplicationURL + "ODHTag/" + x }).ToList() : new List<ODHTags>();
            }
        }

        public bool OdhActive
        {
            get
            {
                return this.SmgActive;
            }
        }

        //Overwrites The Features
        public new ICollection<TopicLinked> Topics { get; set; }

        //Overwrites The LocationInfo
        public new LocationInfoLinked? LocationInfo { get; set; }

        //Overwrites LTSTags
        public new List<LTSTagsLinked>? LTSTags { get; set; }
    }

    public class PackageLinked : Package, IMetaData
    {
        public Metadata _Meta { get; set; }

        public string Self
        {
            get
            {
                return "Package/" + Uri.EscapeDataString(this.Id);
            }
        }

        //Taglist
        public ICollection<ODHTags> ODHTags
        {
            get
            {
                return this.SmgTags != null ? this.SmgTags.Select(x => new ODHTags() { Id = x, Self = ODHConstant.ApplicationURL + "ODHTag/" + x }).ToList() : new List<ODHTags>();
            }
        }

        public bool OdhActive
        {
            get
            {
                return this.SmgActive;
            }
        }

        //Overwrites The LocationInfo
        public new LocationInfoLinked? LocationInfo { get; set; }
    }

    public class ODHActivityPoiLinked : ODHActivityPoi, IMetaData
    {
        public Metadata? _Meta { get; set; }

        public string? Self
        {
            get
            {
                return this.Id != null ? "ODHActivityPoi/" + Uri.EscapeDataString(this.Id) : null;
            }
        }

        public bool OdhActive
        {
            get
            {
                return this.SmgActive;
            }
        }

        public ICollection<ODHTags> ODHTags
        {
            get
            {
                return this.SmgTags != null ? this.SmgTags.Select(x => new ODHTags() { Id = x, Self = ODHConstant.ApplicationURL + "ODHTag/" + x }).ToList() : new List<ODHTags>();
            }
        }

        public ICollection<ODHActivityPoiTypesLink>? ODHActivityPoiTypes
        {
            get
            {
                var returnlist = new List<ODHActivityPoiTypesLink>();
                returnlist.Add(new ODHActivityPoiTypesLink() { Id = this.Type, Self = ODHConstant.ApplicationURL + "OdhActivityPoiTypes/" + this.Type, Type = "Type" });
                returnlist.Add(new ODHActivityPoiTypesLink() { Id = this.SubType, Self = ODHConstant.ApplicationURL + "OdhActivityPoiTypes/" + this.SubType, Type = "SubType" });
                if (!String.IsNullOrEmpty(this.PoiType))
                    returnlist.Add(new ODHActivityPoiTypesLink() { Id = this.PoiType, Self =ODHConstant.ApplicationURL + "OdhActivityPoiTypes/" + this.PoiType, Type = "PoiType" });

                return returnlist;
            }
        }

        public ICollection<AreaLink>? Areas
        {
            get
            {
                return this.AreaId != null ? this.AreaId.Select(x => new AreaLink() { Id = x, Self = ODHConstant.ApplicationURL + "Area/" + x }).ToList() : new List<AreaLink>();
            }
        }

        public new ICollection<CategoryCodesLinked>? CategoryCodes { get; set; }
        public new ICollection<DishRatesLinked>? DishRates { get; set; }
        public new ICollection<CapacityCeremonyLinked>? CapacityCeremony { get; set; }
        public new ICollection<FacilitiesLinked>? Facilities { get; set; }

        //Overwrites The LocationInfo
        public new LocationInfoLinked? LocationInfo { get; set; }

        //Overwrites LTSTags
        public new List<LTSTagsLinked>? LTSTags { get; set; }        
    }

    public class LTSPoiLinked : LTSPoi, IMetaData
    {
        public Metadata _Meta { get; set; }

        public string Self
        {
            get
            {
                return "Poi/" + Uri.EscapeDataString(this.Id);
            }
        }

        public bool OdhActive
        {
            get
            {
                return this.SmgActive;
            }
        }

        public ICollection<ODHTags> ODHTags
        {
            get
            {
                return this.SmgTags != null ? this.SmgTags.Select(x => new ODHTags() { Id = x, Self = ODHConstant.ApplicationURL + "PoiTypes/" + x }).ToList() : new List<ODHTags>();
            }
        }

        public ICollection<ODHActivityPoiTypesLink> PoiTypes
        {
            get
            {
                var returnlist = new List<ODHActivityPoiTypesLink>();
                returnlist.Add(new ODHActivityPoiTypesLink() { Id = this.Type, Self = ODHConstant.ApplicationURL + "PoiTypes/" + Uri.EscapeDataString(this.Type), Type = "Type" });
                if (!String.IsNullOrEmpty(this.SubType) && this.SubType != "no Subtype" && this.SubType != "Essen Trinken")
                    returnlist.Add(new ODHActivityPoiTypesLink() { Id = this.SubType, Self = ODHConstant.ApplicationURL + "PoiTypes/" + Uri.EscapeDataString(this.SubType), Type = "SubType" });
                if (!String.IsNullOrEmpty(this.PoiType))
                    returnlist.Add(new ODHActivityPoiTypesLink() { Id = this.PoiType, Self = ODHConstant.ApplicationURL + "PoiTypes/" + Uri.EscapeDataString(this.PoiType), Type = "PoiType" });

                return returnlist;
            }
        }

        public ICollection<AreaLink> Areas
        {
            get
            {
                return this.AreaId != null ? this.AreaId.Select(x => new AreaLink() { Id = x, Self = ODHConstant.ApplicationURL + "Area/" + x }).ToList() : new List<AreaLink>();
            }
        }

        //Overwrites The LocationInfo
        public new LocationInfoLinked? LocationInfo { get; set; }

        //Overwrites LTSTags
        public new List<LTSTagsLinked>? LTSTags { get; set; }        
    }

    public class LTSActivityLinked : LTSActivity, IMetaData
    {
        public Metadata _Meta { get; set; }

        public string Self
        {
            get
            {
                return ODHConstant.ApplicationURL + "Activity/" + Uri.EscapeDataString(this.Id);
            }
        }

        public bool OdhActive
        {
            get
            {
                return this.SmgActive;
            }
        }

        public ICollection<ODHTags> ODHTags
        {
            get
            {
                return this.SmgTags != null ? this.SmgTags.Select(x => new ODHTags() { Id = x, Self = ODHConstant.ApplicationURL + "ActivityTypes/" + x }).ToList() : new List<ODHTags>();
            }
        }

        public ICollection<ODHActivityPoiTypesLink> ActivityTypes
        {
            get
            {
                var returnlist = new List<ODHActivityPoiTypesLink>();
                returnlist.Add(new ODHActivityPoiTypesLink() { Id = this.Type, Self = ODHConstant.ApplicationURL + "ActivityTypes/" + Uri.EscapeDataString(this.Type), Type = "Type" });
                if (!String.IsNullOrEmpty(this.SubType) && this.SubType != "no Subtype" && this.SubType != "Essen Trinken")
                    returnlist.Add(new ODHActivityPoiTypesLink() { Id = this.SubType, Self = ODHConstant.ApplicationURL + "ActivityTypes/" + Uri.EscapeDataString(this.SubType), Type = "SubType" });
                if (!String.IsNullOrEmpty(this.PoiType))
                    returnlist.Add(new ODHActivityPoiTypesLink() { Id = this.PoiType, Self = ODHConstant.ApplicationURL + "ActivityTypes/" + Uri.EscapeDataString(this.PoiType), Type = "PoiType" });

                return returnlist;
            }
        }

        public ICollection<AreaLink> Areas
        {
            get
            {
                return this.AreaId != null ? this.AreaId.Select(x => new AreaLink() { Id = x, Self = ODHConstant.ApplicationURL + "Area/" + x }).ToList() : new List<AreaLink>();
            }
        }

        //Overwrites The LocationInfo
        public new LocationInfoLinked? LocationInfo { get; set; }

        //Overwrites LTSTags
        public new List<LTSTagsLinked>? LTSTags { get; set; }        
    }

    public class ArticlesLinked : ArticleBaseInfos, IMetaData
    {
        public Metadata? _Meta { get; set; }

        public string? Self
        {
            get
            {
                return this.Id != null ? "Article/" + Uri.EscapeDataString(this.Id) : null;
            }
        }

        public bool OdhActive
        {
            get
            {
                return this.SmgActive;
            }
        }

        public ICollection<ODHActivityPoiTypesLink>? ArticleTypes
        {
            get
            {
                var returnlist = new List<ODHActivityPoiTypesLink>();
                returnlist.Add(new ODHActivityPoiTypesLink() { Id = this.Type, Self = ODHConstant.ApplicationURL + "ArticleTypes/" + Uri.EscapeDataString(this.Type), Type = "ArticleType" });
                if (!String.IsNullOrEmpty(this.SubType) && this.SubType != "no Subtype" && this.SubType != "Essen Trinken")
                    returnlist.Add(new ODHActivityPoiTypesLink() { Id = this.SubType, Self = ODHConstant.ApplicationURL + "ArticleTypes/" + Uri.EscapeDataString(this.SubType), Type = "ArticleSubType" });

                return returnlist;
            }
        }

        public ICollection<string>? ArticleTypeList
        {
            get
            {
                var returnlist = new List<string>();
                returnlist.Add(this.Type);
                if (!String.IsNullOrEmpty(this.SubType) && this.SubType != "no Subtype" && this.SubType != "Essen Trinken")
                    returnlist.Add(this.SubType);

                return returnlist;
            }
        }

        public ICollection<ODHTags> ODHTags
        {
            get
            {
                return this.SmgTags != null ? this.SmgTags.Select(x => new ODHTags() { Id = x, Self = ODHConstant.ApplicationURL + "ODHTag/" + x }).ToList() : new List<ODHTags>();
            }
        }
    }

    public class DistrictLinked : District, IMetaData
    {
        public Metadata _Meta { get; set; }

        public string Self
        {
            get
            {
                return "District/" + Uri.EscapeDataString(this.Id);
            }
        }

        public ICollection<ODHTags> ODHTags
        {
            get
            {
                return this.SmgTags != null ? this.SmgTags.Select(x => new ODHTags() { Id = x, Self = ODHConstant.ApplicationURL + "ODHTag/" + x }).ToList() : new List<ODHTags>();
            }
        }

        public bool OdhActive
        {
            get
            {
                return this.SmgActive;
            }
        }

        public RegionLink Region
        {
            get
            {
                return String.IsNullOrEmpty(this.RegionId) ? null : new RegionLink() { Id = this.RegionId, Self = ODHConstant.ApplicationURL + "Region/" + Uri.EscapeDataString(this.RegionId) };
            }
        }

        public MunicipalityLink Municipality
        {
            get
            {
                return String.IsNullOrEmpty(this.MunicipalityId) ? null : new MunicipalityLink() { Id = this.MunicipalityId, Self = ODHConstant.ApplicationURL + "Municipality/" + Uri.EscapeDataString(this.MunicipalityId) };
            }
        }

        public TourismAssociationLink Tourismassociation
        {
            get
            {
                return String.IsNullOrEmpty(this.TourismvereinId) ? null : new TourismAssociationLink() { Id = this.TourismvereinId, Self = ODHConstant.ApplicationURL + "TourismAssociation/" + Uri.EscapeDataString(this.TourismvereinId) };
            }
        }
    }

    public class MunicipalityLinked : Municipality, IMetaData
    {
        public Metadata _Meta { get; set; }

        public string Self
        {
            get
            {
                return "Municipality/" + Uri.EscapeDataString(this.Id);
            }
        }

        public ICollection<ODHTags> ODHTags
        {
            get
            {
                return this.SmgTags != null ? this.SmgTags.Select(x => new ODHTags() { Id = x, Self = ODHConstant.ApplicationURL + "ODHTag/" + x }).ToList() : new List<ODHTags>();
            }
        }

        public bool OdhActive
        {
            get
            {
                return this.SmgActive;
            }
        }

        public RegionLink Region
        {
            get
            {
                return String.IsNullOrEmpty(this.RegionId) ? null : new RegionLink() { Id = this.RegionId, Self = ODHConstant.ApplicationURL + "Region/" + Uri.EscapeDataString(this.RegionId) };
            }
        }

        public TourismAssociationLink Tourismassociation
        {
            get
            {
                return String.IsNullOrEmpty(this.TourismvereinId) ? null : new TourismAssociationLink() { Id = this.TourismvereinId, Self = ODHConstant.ApplicationURL + "TourismAssociation/" + Uri.EscapeDataString(this.TourismvereinId) };
            }
        }
    }

    public class TourismvereinLinked : Tourismverein, IMetaData
    {
        public Metadata _Meta { get; set; }

        public string Self
        {
            get
            {
                return "TourismAssociation/" + Uri.EscapeDataString(this.Id);
            }
        }

        public ICollection<ODHTags> ODHTags
        {
            get
            {
                return this.SmgTags != null ? this.SmgTags.Select(x => new ODHTags() { Id = x, Self = ODHConstant.ApplicationURL + "ODHTag/" + x }).ToList() : new List<ODHTags>();
            }
        }

        public bool OdhActive
        {
            get
            {
                return this.SmgActive;
            }
        }

        public RegionLink Region
        {
            get
            {
                return String.IsNullOrEmpty(this.RegionId) ? null : new RegionLink() { Id = this.RegionId, Self = ODHConstant.ApplicationURL + "Region/" + Uri.EscapeDataString(this.RegionId) };
            }
        }

        public ICollection<SkiAreaLink> SkiAreas
        {
            get
            {
                return this.SkiareaIds != null ? this.SkiareaIds.Select(x => new SkiAreaLink() { Id = x, Self = ODHConstant.ApplicationURL + "SkiArea/" + x }).ToList() : new List<SkiAreaLink>();
            }
        }
    }

    public class RegionLinked : Region, IMetaData
    {
        public Metadata _Meta { get; set; }

        public string Self
        {
            get
            {
                return "Region/" + Uri.EscapeDataString(this.Id);
            }
        }

        public ICollection<ODHTags> ODHTags
        {
            get
            {
                return this.SmgTags != null ? this.SmgTags.Select(x => new ODHTags() { Id = x, Self = ODHConstant.ApplicationURL + "ODHTag/" + x }).ToList() : new List<ODHTags>();
            }
        }

        public bool OdhActive
        {
            get
            {
                return this.SmgActive;
            }
        }

        public ICollection<SkiAreaLink> SkiAreas
        {
            get
            {
                return this.SkiareaIds != null ? this.SkiareaIds.Select(x => new SkiAreaLink() { Id = x, Self = ODHConstant.ApplicationURL + "SkiArea/" + x }).ToList() : new List<SkiAreaLink>();
            }
        }
    }

    public class MetaRegionLinked : MetaRegion, IMetaData
    {
        public Metadata _Meta { get; set; }

        public string Self
        {
            get
            {
                return "MetaRegion/" + Uri.EscapeDataString(this.Id);
            }
        }

        public ICollection<ODHTags> ODHTags
        {
            get
            {
                return this.SmgTags != null ? this.SmgTags.Select(x => new ODHTags() { Id = x, Self = ODHConstant.ApplicationURL + "ODHTag/" + x }).ToList() : new List<ODHTags>();
            }
        }

        public bool OdhActive
        {
            get
            {
                return this.SmgActive;
            }
        }

        public ICollection<DistrictLink> Districts
        {
            get
            {
                return this.DistrictIds != null ? this.DistrictIds.Select(x => new DistrictLink() { Id = x, Self = ODHConstant.ApplicationURL + "District/" + x }).ToList() : new List<DistrictLink>();
            }
        }

        public ICollection<TourismAssociationLink> TourismAssociations
        {
            get
            {
                return this.TourismvereinIds != null ? this.TourismvereinIds.Select(x => new TourismAssociationLink() { Id = x, Self = ODHConstant.ApplicationURL + "TourismAssociation/" + x }).ToList() : new List<TourismAssociationLink>();
            }
        }

        public ICollection<RegionLink> Regions
        {
            get
            {
                return this.RegionIds != null ? this.RegionIds.Select(x => new RegionLink() { Id = x, Self = ODHConstant.ApplicationURL + "Region/" + x }).ToList() : new List<RegionLink>();
            }
        }
    }

    public class ExperienceAreaLinked : ExperienceArea, IMetaData
    {
        public Metadata _Meta { get; set; }

        public string Self
        {
            get
            {
                return "ExperienceArea/" + Uri.EscapeDataString(this.Id);
            }
        }

        public ICollection<ODHTags> ODHTags
        {
            get
            {
                return this.SmgTags != null ? this.SmgTags.Select(x => new ODHTags() { Id = x, Self = ODHConstant.ApplicationURL + "ODHTag/" + x }).ToList() : new List<ODHTags>();
            }
        }

        public bool OdhActive
        {
            get
            {
                return this.SmgActive;
            }
        }

        public ICollection<TourismAssociationLink> TourismAssociations
        {
            get
            {
                return this.TourismvereinIds != null ? this.TourismvereinIds.Select(x => new TourismAssociationLink() { Id = x, Self = ODHConstant.ApplicationURL + "TourismAssociation/" + x }).ToList() : new List<TourismAssociationLink>();
            }
        }

        public ICollection<DistrictLink> Districts
        {
            get
            {
                return this.DistrictIds != null ? this.DistrictIds.Select(x => new DistrictLink() { Id = x, Self = ODHConstant.ApplicationURL + "District/" + x }).ToList() : new List<DistrictLink>();
            }
        }
    }

    public class AreaLinked : Area, IMetaData
    {
        public Metadata _Meta { get; set; }

        public string Self
        {
            get
            {
                return "Area/" + Uri.EscapeDataString(this.Id);
            }
        }

        public bool OdhActive
        {
            get
            {
                return this.SmgActive;
            }
        }

        public RegionLink Region
        {
            get
            {
                return String.IsNullOrEmpty(this.RegionId) ? null : new RegionLink() { Id = this.RegionId, Self = ODHConstant.ApplicationURL + "Region/" + Uri.EscapeDataString(this.RegionId) };
            }
        }

        public MunicipalityLink Municipality
        {
            get
            {
                return String.IsNullOrEmpty(this.MunicipalityId) ? null : new MunicipalityLink() { Id = this.MunicipalityId, Self = ODHConstant.ApplicationURL + "Municipality/" + Uri.EscapeDataString(this.MunicipalityId) };
            }
        }

        public TourismAssociationLink Tourismassociation
        {
            get
            {
                return String.IsNullOrEmpty(this.TourismvereinId) ? null : new TourismAssociationLink() { Id = this.TourismvereinId, Self = ODHConstant.ApplicationURL + "TourismAssociation/" + Uri.EscapeDataString(this.TourismvereinId) };
            }
        }

        public SkiAreaLink SkiArea
        {
            get
            {
                return String.IsNullOrEmpty(this.SkiAreaID) ? null : new SkiAreaLink() { Id = this.SkiAreaID, Self = ODHConstant.ApplicationURL + "SkiArea/" + Uri.EscapeDataString(this.SkiAreaID) };
            }
        }
    }

    public class SkiAreaLinked : SkiArea, IMetaData
    {
        public Metadata _Meta { get; set; }

        public string Self
        {
            get
            {
                return "SkiArea/" + Uri.EscapeDataString(this.Id);
            }
        }

        public ICollection<ODHTags> ODHTags
        {
            get
            {
                return this.SmgTags != null ? this.SmgTags.Select(x => new ODHTags() { Id = x, Self = ODHConstant.ApplicationURL + "ODHTag/" + x }).ToList() : new List<ODHTags>();
            }
        }

        public bool OdhActive
        {
            get
            {
                return this.SmgActive;
            }
        }

        public SkiRegionLink SkiRegion
        {
            get
            {
                return String.IsNullOrEmpty(this.SkiRegionId) ? null : new SkiRegionLink() { Id = this.SkiRegionId, Self = ODHConstant.ApplicationURL + "SkiRegion/" + Uri.EscapeDataString(this.SkiRegionId) };
            }
        }

        public ICollection<AreaLink> Areas
        {
            get
            {
                return this.AreaId != null ? this.AreaId.Select(x => new AreaLink() { Id = x, Self = ODHConstant.ApplicationURL + "Area/" + x }).ToList() : new List<AreaLink>();
            }
        }

        public ICollection<TourismAssociationLink> TourismAssociations
        {
            get
            {
                return this.TourismvereinIds != null ? this.TourismvereinIds.Select(x => new TourismAssociationLink() { Id = x, Self = ODHConstant.ApplicationURL + "TourismAssociation/" + x }).ToList() : new List<TourismAssociationLink>();
            }
        }

        public ICollection<RegionLink> Regions
        {
            get
            {
                return this.RegionIds != null ? this.RegionIds.Select(x => new RegionLink() { Id = x, Self = ODHConstant.ApplicationURL + "Region/" + x }).ToList() : new List<RegionLink>();
            }
        }

        //Overwrites The LocationInfo
        public new LocationInfoLinked? LocationInfo { get; set; }
    }

    public class SkiRegionLinked : SkiRegion, IMetaData
    {
        public Metadata _Meta { get; set; }

        public string Self
        {
            get
            {
                return "SkiRegion/" + Uri.EscapeDataString(this.Id);
            }
        }

        public ICollection<ODHTags> ODHTags
        {
            get
            {
                return this.SmgTags != null ? this.SmgTags.Select(x => new ODHTags() { Id = x, Self = ODHConstant.ApplicationURL + "ODHTag/" + x }).ToList() : new List<ODHTags>();
            }
        }

        public bool OdhActive
        {
            get
            {
                return this.SmgActive;
            }
        }
    }

    public class WebcamInfoLinked : WebcamInfo, IMetaData
    {
        public Metadata? _Meta { get; set; }

        public string Self
        {
            get
            {
                return "WebcamInfo/" + Uri.EscapeDataString(this.Id);
            }
        }

        public bool OdhActive
        {
            get
            {
                return (bool)this.SmgActive;
            }
        }

        public ICollection<AreaLink> Areas
        {
            get
            {
                return this.AreaIds != null ? this.AreaIds.Select(x => new AreaLink() { Id = x, Self = ODHConstant.ApplicationURL + "Area/" + x }).ToList() : new List<AreaLink>();
            }
        }

        public ICollection<ODHTags> ODHTags
        {
            get
            {
                return this.SmgTags != null ? this.SmgTags.Select(x => new ODHTags() { Id = x, Self = ODHConstant.ApplicationURL + "ODHTag/" + x }).ToList() : new List<ODHTags>();
            }
        }       
    }

    public class MeasuringpointLinked : Measuringpoint, IMetaData
    {
        public Metadata _Meta { get; set; }

        public string Self
        {
            get
            {
                return "Weather/Measuringpoint/" + Uri.EscapeDataString(this.Id);
            }
        }

        public bool OdhActive
        {
            get
            {
                return (bool)this.SmgActive;
            }
        }

        public ICollection<AreaLink> Areas
        {
            get
            {
                return this.AreaIds != null ? this.AreaIds.Select(x => new AreaLink() { Id = x, Self = ODHConstant.ApplicationURL + "Area/" + x }).ToList() : new List<AreaLink>();
            }
        }

        //Overwrites The LocationInfo
        public new LocationInfoLinked? LocationInfo { get; set; }       
    }

    public class WineLinked : Wine, IMetaData
    {
        public Metadata _Meta { get; set; }

        public string Self
        {
            get
            {
                return ODHConstant.ApplicationURL + "WineAward/" + this.Id;
            }
        }

        public bool OdhActive
        {
            get
            {
                return (bool)this.SmgActive;
            }
        }

        public CompanyLink Company
        {
            get
            {
                return String.IsNullOrEmpty(this.CompanyId) ? null : new CompanyLink() { Id = this.CompanyId, Self = ODHConstant.ApplicationURL + "ODHActivityPoi/" + this.CompanyId };
            }
        }
    }

    public class EventShortLinked : EventShort, IMetaData
    {
        public Metadata? _Meta { get; set; }

        public string Self
        {
            get
            {
                return ODHConstant.ApplicationURL + "EventShort/" + this.Id;
            }
        }
    }

    public class WeatherLinked : Weather, IMetaData
    {
        public Metadata? _Meta { get; set; }

        public string Self
        {
            get
            {
                return ODHConstant.ApplicationURL + "Weather/" + this.Id;
            }
        }
    }

    public class WeatherHistoryLinked : WeatherHistory, IMetaData
    {
        public Metadata? _Meta { get; set; }

        public string Self
        {
            get
            {
                return ODHConstant.ApplicationURL + "WeatherHistory/" + this.Id;
            }
        }
    }

    public class WeatherDistrictLinked : BezirksWeather, IMetaData
    {
        public Metadata? _Meta { get; set; }

        public string Self
        {
            get
            {
                return ODHConstant.ApplicationURL + "Weather/District" + this.Id;
            }
        }
    }

    public class ODHTagLinked : SmgTags, IMetaData
    {
        public Metadata _Meta { get; set; }        

        public string Self
        {
            get
            {
                return ODHConstant.ApplicationURL + "ODHTag/" + this.Id;
            }
        }
    }

    public class TagLinked : SmgTags, IMetaData
    {
        public Metadata _Meta { get; set; }

        public string Self
        {
            get
            {
                return ODHConstant.ApplicationURL + "Tag/" + this.Id;
            }
        }
        public List<string> ODHTagIds { get; set; }
    }

    //TODO ADD Linked + Meta 
    //EventTopics
    //ActivityTypes
    //PoiTypes
    //AccommodationTypes
    //ODHActivityPoiTypes
    //ODHTag
    //AccommodationFeatures
    //ArticleTypes
    //EventShort
    //GastronomyTypes
    //VenueTypes
    //Location??



    #endregion

    public static class ODHConstant
    {
        public static string ApplicationURL
        {
            get
            {
                return ""; //ConfigurationManager.AppSettings["currenturl"] + "/";
                //return Environment.GetEnvironmentVariable("CurrentODHURL") + "/";
            }
        }
    }
}
