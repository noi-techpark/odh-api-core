using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DataModel
{
    public class DDVenue
        : IIdentifiable,
            IMetaData,
            IImportDateassigneable,
            ILicenseInfo,
            ISource,
            IMappingAware
    {
        public DDVenue()
        {
            //Mapping New
            Mapping = new Dictionary<string, IDictionary<string, string>>();
        }

        public string Self
        {
            get { return "Venue/" + Uri.EscapeDataString(this.Id); }
        }

        public Metadata? _Meta { get; set; }
        public string? type { get; set; }

        //public string id { get; set; }
        [JsonProperty("id")]
        public string? Id { get; set; }

        public DDMeta? meta { get; set; }
        public DDLinks? links { get; set; }
        public DDAttributes attributes { get; set; }
        public DDRelationships? relationships { get; set; }

        //Additional ODH Data for Venues
        public ODHData? odhdata { get; set; }

        [JsonIgnore]
        public string? Shortname { get; set; }

        [JsonIgnore]
        public DateTime? FirstImport { get; set; }

        [JsonIgnore]
        public DateTime? LastChange { get; set; }

        [JsonIgnore]
        public LicenseInfo? LicenseInfo { get; set; }

        [JsonIgnore]
        public string? Source { get; set; }

        [JsonIgnore]
        //New Mapping
        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }
    }

    public class ODHData
    {
        public ODHData()
        {
            GpsPoints = new Dictionary<string, GpsInfo>();
            //Mapping New
            Mapping = new Dictionary<string, IDictionary<string, string>>();
        }

        public LicenseInfo? LicenseInfo { get; set; }

        public string? Shortname { get; set; }

        public bool Active { get; set; }
        public bool ODHActive { get; set; }

        //public ICollection<ODHTags> ODHTags { get; set; }

        public ICollection<ODHTags> ODHTags
        {
            get
            {
                return this.SmgTags != null
                    ? this.SmgTags
                        .Select(
                            x =>
                                new ODHTags()
                                {
                                    Id = x,
                                    Self = ODHConstant.ApplicationURL + "ODHTag/" + x
                                }
                        )
                        .ToList()
                    : new List<ODHTags>();
            }
        }

        public ICollection<string> SmgTags { get; set; }

        public LocationInfoLinked LocationInfo { get; set; }
        public ICollection<string> HasLanguage { get; set; }

        public ICollection<VenueType> VenueCategory { get; set; }

        public ICollection<GpsInfo> GpsInfo { get; set; }

        public string Source { get; set; }
        public string SyncSourceInterface { get; set; }

        //New Details
        public Nullable<int> RoomCount { get; set; }
        public ICollection<VenueRoomDetails> RoomDetails { get; set; }

        //added
        public IDictionary<string, GpsInfo> GpsPoints { get; set; }

        public List<string>? PublishedOn { get; set; }

        //New Mapping
        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }

        public DistanceInfo DistanceInfo { get; set; }
    }

    public class VenueType
    {
        public string Id { get; set; }
        public string VenueCode { get; set; }

        public string Self
        {
            get { return "VenueTypes/" + Uri.EscapeDataString(this.Id); }
        }
    }

    public class VenueSetup
    {
        public string Id { get; set; }
        public int Capacity { get; set; }
        public string VenueCode { get; set; }

        public string Self
        {
            get { return "VenueTypes/" + Uri.EscapeDataString(this.Id); }
        }
    }

    public class VenueRoomDetails
    {
        public string? Id { get; set; }
        public string Shortname { get; set; }

        public int? SquareMeters { get; set; }

        //public int maxCapacity { get; set; }

        public bool? Indoor { get; set; }

        public ICollection<VenueType>? VenueFeatures { get; set; }
        public ICollection<VenueSetup> VenueSetup { get; set; }
    }

    public class DDMeta
    {
        public DateTime lastUpdate { get; set; }
        public string dataProvider { get; set; }
    }

    public class DDLinks
    {
        public string self { get; set; }
    }

    public class DDLinksRelated
    {
        public string related { get; set; }
    }

    public class DDAttributes
    {
        [JsonProperty("abstract")]
        public IDictionary<string, string>? _abstract { get; set; }
        public ICollection<string>? categories { get; set; }
        public IDictionary<string, string>? description { get; set; }
        public ICollection<DDGeometry>? geometries { get; set; }
        public IDictionary<string, string>? howToArrive { get; set; }
        public IDictionary<string, string> name { get; set; }
        public IDictionary<string, string>? shortName { get; set; }
        public IDictionary<string, string>? url { get; set; }
        public ICollection<DDAddress>? address { get; set; }

        public DDOpeninghours? openingHours { get; set; }

        public int? beds { get; set; }
    }

    public class DDAttributesSubVenues : DDAttributes
    {
        public ICollection<DDAvailablesetup> availableSetups { get; set; }
        public DDDimension? dimension { get; set; }
    }

    public class DDAddress
    {
        public IDictionary<string, string> street { get; set; }
        public IDictionary<string, string> city { get; set; }
        public string? region { get; set; }
        public string country { get; set; }
        public IDictionary<string, string>? complement { get; set; }
        public string[]? categories { get; set; }
        public string? zipcode { get; set; }
        public DDContactpoint? contactPoint { get; set; }
    }

    public class DDContactpoint
    {
        public string email { get; set; }
        public string telephone { get; set; }
        public DDAddress address { get; set; }
    }

    public class DDOpeninghours
    {
        public ICollection<DDOpeninghoursweekly> weeklySchedules { get; set; }
        public IDictionary<string, ICollection<DDOpeningTimes>> dailySchedules { get; set; }
    }

    public class DDOpeninghoursweekly
    {
        public string validFrom { get; set; }
        public string validTo { get; set; }
        public ICollection<DDOpeningTimes> monday { get; set; }
        public ICollection<DDOpeningTimes> tuesday { get; set; }
        public ICollection<DDOpeningTimes> wednesday { get; set; }
        public ICollection<DDOpeningTimes> thursday { get; set; }
        public ICollection<DDOpeningTimes> friday { get; set; }
        public ICollection<DDOpeningTimes> saturday { get; set; }
        public ICollection<DDOpeningTimes> sunday { get; set; }
    }

    public class DDOpeningTimes
    {
        public string opens { get; set; }
        public string closes { get; set; }
    }

    public class DDGeometry
    {
        public string type { get; set; }
        public double[] coordinates { get; set; }
    }

    public class DDRelationships
    {
        public IDictionary<
            string,
            ICollection<DDMultimediadescriptions>
        >? multimediaDescriptions { get; set; }
        public ICollection<DDSubVenue>? subVenues { get; set; }
    }

    public class DDMultimediadescriptions
    {
        public string type { get; set; }
        public string id { get; set; }
        public ICollection<string> categories { get; set; }
        public DDAttributesMultimedia attributes { get; set; }
        public DDLinks links { get; set; }

        //public DDRelationships relationships { get; set; }
        public DDRelationshipsMultiMedia relationships { get; set; }
    }

    public class DDAttributesMultimedia
    {
        [JsonProperty("abstract")]
        public IDictionary<string, string> _abstract { get; set; }
        public string contentType { get; set; }
        public string url { get; set; }
        public List<string> categories { get; set; }
        public IDictionary<string, string> description { get; set; }
        public Nullable<int> duration { get; set; }
        public Nullable<int> height { get; set; }
        public string license { get; set; }
        public IDictionary<string, string> name { get; set; }
        public IDictionary<string, string> shortName { get; set; }
        public Nullable<int> width { get; set; }
    }

    public class DDRelationshipsMultiMedia
    {
        public DDCopyrightowner copyrightOwner { get; set; }
    }

    public class DDCopyrightowner
    {
        public DDCopyrightownerData data { get; set; }
        public DDLinksRelated links { get; set; }
    }

    public class DDCopyrightownerData
    {
        public string type { get; set; }
        public string id { get; set; }
        public DDAttributesMultimedia attributes { get; set; }
        //public DDRelationships relationships { get; set; }
    }

    public class DDDimension
    {
        public int squareMeters { get; set; }
        public int doorHeight { get; set; }
        public int doorWidth { get; set; }
        public int height { get; set; }
        public float length { get; set; }
        public float width { get; set; }
    }

    public class DDAvailablesetup
    {
        public string setup { get; set; }
        public int capacity { get; set; }
    }

    public class DDSubVenue : DDVenue
    {
        public new DDAttributesSubVenues attributes { get; set; }
    }

    public class DDVenueCodes
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Code { get; set; }
        public IDictionary<string, string> TypeDesc { get; set; }
        public IDictionary<string, string> Name { get; set; }
        public int Bitmask { get; set; }
        public string Type { get; set; }
    }
}
