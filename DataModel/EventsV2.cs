// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class EventsV2 : IIdentifiable, IActivateable, IHasLanguage, IImageGalleryAware, IContactInfosAware, IMetaData, IMappingAware, IDetailInfosAware, ILicenseInfo, IGPSInfoAware, IPublishedOn, IVideoItemsAware
    {
        //MetaData Information, Contains Source, LastUpdate
        public Metadata? _Meta { get; set; }

        //License Information
        public LicenseInfo? LicenseInfo { get; set; }

        //Self Link to this Data
        public string Self
        {
            get
            {
                return this.Id != null ? "Events/" + Uri.EscapeDataString(this.Id) : null;                
            }
        }

        //Id Shortname and Active Info
        public string? Id { get; set; }
        public string? Shortname { get; set; }
        public bool Active { get; set; }

        //Language, Publishedon, Mapping and RelatedContent
        public ICollection<string>? HasLanguage { get; set; }
        public ICollection<string>? PublishedOn { get; set; }
        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }
        public ICollection<RelatedContent>? RelatedContent { get; set; }

        //Tags
        public List<Tags> Tags { get; set; }


        //Description and Contactinfo
        public IDictionary<string, Detail> Detail { get; set; }
        public IDictionary<string, ContactInfos> ContactInfos { get; set; }


        //ImageGallery and Video Data
        public ICollection<ImageGallery>? ImageGallery { get; set; }
        public IDictionary<string, ICollection<VideoItems>>? VideoItems { get; set; }

        //Gps Information and LocationInfo or should the venue GPS Info used?
        public ICollection<GpsInfo> GpsInfo { get; set; }
        public LocationInfoLinked? LocationInfo { get; set; }
        

        //Assigned Venues or should we use RelatedContent?
        public List<string> VenueIds { get; set; }

        [SwaggerSchema(Description = "generated field", ReadOnly = true)]
        public ICollection<VenueLink> Venues
        {
            get
            {
                return this.Venues != null ? this.VenueIds.Select(x => new VenueLink() { Id = x, Self = "Venue/" + x }).ToList() : new List<VenueLink>();
            }
        }

        //TODO Add Subevent Use RelatedContent?

        //TODO Add Documents
        public IDictionary<string, List<Document>?> Documents { get; set; }


        //TODO Add EventDates

        //TODO Add Booking Info

        //to check if this is needed?

        //TODO Properties LIST (simple Key Value List)
        public IDictionary<string,string>? Properties { get; set; }

        //TODO Properties Language Based LIST (Key Value List Language Dependant)
        public IDictionary<string, IDictionary<string,string>>? PropertiesLocalized { get; set; }

    }

    public class VenueLink
    {
        public string Id { get; set; }
        public string? Self { get; set; }
    }

    public class DateRanges
    {
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
    }



}
