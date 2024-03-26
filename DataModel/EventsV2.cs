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
    public class EventsV2 : IIdentifiable, IActivateable, IHasLanguage, IImageGalleryAware, IContactInfosAware, IMetaData, IMappingAware, IDetailInfosAware, ILicenseInfo, IGPSInfoAware, IPublishedOn, IVideoItemsAware, IImportDateassigneable, ISource
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
                return "EventV2/" + Uri.EscapeDataString(this.Id);
            }
        }

        //Id Shortname and Active Info
        public string? Id { get; set; }
        public string? Shortname { get; set; }
        public bool Active { get; set; }
        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }

        public string? Source { get; set; }

        //Language, Publishedon, Mapping and RelatedContent
        public ICollection<string>? HasLanguage { get; set; }
        public ICollection<string>? PublishedOn { get; set; }
        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }
        //We use RelatedContent to store Parent/Child Event Information
        public ICollection<RelatedContent>? RelatedContent { get; set; }

        public IDictionary<string, dynamic> AdditionalProperties { get; set; }

        //Tags
        public List<Tags> Tags { get; set; }


        //Description and Contactinfo
        public IDictionary<string, Detail> Detail { get; set; }
        public IDictionary<string, ContactInfos> ContactInfos { get; set; }

        //Event Organizer
        public IDictionary<string, ContactInfos> Organizer { get; set; }

        //ImageGallery and Video Data
        public ICollection<ImageGallery>? ImageGallery { get; set; }
        public IDictionary<string, ICollection<VideoItems>>? VideoItems { get; set; }

        //Gps Information and LocationInfo or should the venue GPS Info used?
        public ICollection<GpsInfo> GpsInfo { get; set; }
        public LocationInfoLinked? LocationInfo { get; set; }
                   

        public IDictionary<string, List<DocumentDetailed>?> Documents { get; set; }

        //TODO Add EventDates
        public ICollection<EventInfo> EventInfo { get; set; }

        //TODO Event URLS ? 

        //TODO Add Booking Info

        //TODO Add Subevent Use RelatedContent?
    }

    public class VenueLink
    {
        public string Id { get; set; }
        public string? Self { get; set; }
    }

    public class EventInfo
    {
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }

        public double BeginUTC { get; set; }

        public double EndUTC { get; set; }

        public List<string> VenueIds { get; set; }

        [SwaggerSchema(Description = "generated field", ReadOnly = true)]
        public ICollection<VenueLink> Venues
        {
            get
            {
                return this.Venues != null ? this.VenueIds.Select(x => new VenueLink() { Id = x, Self = "Venue/" + x }).ToList() : new List<VenueLink>();
            }
        }

        //to check if this is needed
        public IDictionary<string, dynamic> AdditionalProperties { get; set; }

        public IDictionary<string, Detail> Detail { get; set; }

        public IDictionary<string, List<DocumentDetailed>?> Documents { get; set; }
    }

    public class DocumentDetailed : Document
    {
        public string Description { get; set; }
        public string DocumentExtension { get; set; }
        public string DocumentMimeType { get; set; }
    }

    //SFSCon Specific


    //LTS Specific
    public class EventLTSInfo
    {

    }

    //EventShort Specific
    public class EventEBMSInfo
    {

    }

    public class EventV2Converter
    {
        public static IEnumerable<EventsV2> ConvertEventListToEventV2<T>(IEnumerable<T> events) where T : IIdentifiable
        {
            var eventsV2 = new List<EventsV2>();

            foreach(var eventv1 in events)
            {
                if (eventv1 is EventShortLinked)
                    eventsV2.Add(ConvertEventShortToEventV2(eventv1 as EventShortLinked));
                if (eventv1 is EventLinked)
                    eventsV2.Add(ConvertEventToEventV2(eventv1 as EventLinked));
            }

            return eventsV2;
        }

        private static EventsV2 ConvertEventToEventV2(EventLinked eventv1)
        {
            //Try to map all to EventsV2
            EventsV2 eventv2 = new EventsV2();

            eventv2.PublishedOn = eventv1.PublishedOn;
            eventv2.Id = eventv1.Id;
            eventv2.ImageGallery = eventv1.ImageGallery;
            eventv2.Shortname = eventv1.Shortname;
            eventv2.FirstImport = eventv1.FirstImport;
            eventv2.LastChange = eventv1.LastChange;
            eventv2.Active = eventv1.Active;
            eventv2.Mapping = eventv1.Mapping;
            eventv2.HasLanguage = eventv1.HasLanguage;
            eventv2.LicenseInfo = eventv1.LicenseInfo;
            eventv2.Source = eventv1.Source;

            eventv2.Detail = eventv1.Detail;
            eventv2.GpsInfo = eventv1.GpsInfo;
            eventv2.ContactInfos = eventv1.ContactInfos;

            //SMGTags as Tags


            return eventv2;
        }

        private static EventsV2 ConvertEventShortToEventV2(EventShortLinked eventv1)
        {
            //Try to map all to EventsV2
            EventsV2 eventv2 = new EventsV2();

            eventv2.PublishedOn = eventv1.PublishedOn;
            eventv2.Id = eventv1.Id;
            eventv2.ImageGallery = eventv1.ImageGallery;
            eventv2.Shortname = eventv1.Shortname;
            eventv2.FirstImport = eventv1.FirstImport;
            eventv2.LastChange = eventv1.LastChange;
            eventv2.Active = eventv1.Active.Value;
            eventv2.Mapping = eventv1.Mapping;
            eventv2.HasLanguage = eventv1.HasLanguage;
            eventv2.GpsInfo = eventv1.GpsInfo;
            eventv2.LicenseInfo = eventv1.LicenseInfo;
            eventv2.Source = eventv1.Source;

            //Putting all info into Detail
            Detail detailde = new Detail() { Title = eventv1.EventTitle["de"], Language = "de", BaseText = eventv1.EventText["de"] };
            Detail detailit = new Detail() { Title = eventv1.EventTitle["it"], Language = "it", BaseText = eventv1.EventText["it"] };
            Detail detailen = new Detail() { Title = eventv1.EventTitle["en"], Language = "en", BaseText = eventv1.EventText["en"] };

            eventv2.Detail.Add("de", detailde);
            eventv2.Detail.Add("it", detailit);
            eventv2.Detail.Add("en", detailen);


            //Adding CustomTagging, TechnologyFields to Tags
            eventv2.Tags = new List<Tags>();
            foreach (var tag in eventv1.TechnologyFields)
            {
                eventv2.Tags.Add(new Tags() { Id = tag, Source = "noi" });
            }
            foreach (var tag in eventv1.CustomTagging)
            {
                eventv2.Tags.Add(new Tags() { Id = tag, Source = "noi" });
            }

            //Adding EventDocument, Documents as DocumentDetailed
            eventv2.Documents = new Dictionary<string, List<DocumentDetailed>>();
            foreach (var documentkvp in eventv1.Documents)
            {
                List<DocumentDetailed> documents = new List<DocumentDetailed>();
                foreach(var doc in documentkvp.Value)
                {
                    documents.Add(new DocumentDetailed() { DocumentName = doc.DocumentName, DocumentURL = doc.DocumentURL, Language = doc.Language });
                }

                eventv2.Documents.Add(documentkvp.Key, documents);
            }
            
            //Adding EventLocation, AnchorVenue, AnchorVenueRoomMapping, AnchorVenueShort, EndDate, StartDate, StartDateUTC, EndDateUTC, RoomBooked


            //ExternalOrganizer, SoldOut, TypicalAgeRange

            //WebAddress




            return eventv2;
        }
    }
}
