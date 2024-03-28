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
    public class EventsV2 : IIdentifiable, IActivateable, IHasLanguage, IImageGalleryAware, IContactInfosAware, IMetaData, IMappingAware, IDetailInfosAware, ILicenseInfo, IPublishedOn, IVideoItemsAware, IImportDateassigneable, ISource
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

        //Event Organizer ??
        public IDictionary<string, ContactInfos> Organizer { get; set; }

        //ImageGallery and Video Data
        public ICollection<ImageGallery>? ImageGallery { get; set; }
        public IDictionary<string, ICollection<VideoItems>>? VideoItems { get; set; }

        //Gps Information and LocationInfo or should the venue GPS Info used?
        //public ICollection<GpsInfo> GpsInfo { get; set; }
        //public LocationInfoLinked? LocationInfo { get; set; } // should this be part of the venue?


        public IDictionary<string, List<DocumentDetailed>>? Documents { get; set; }

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
                return this.VenueIds != null ? this.VenueIds.Select(x => new VenueLink() { Id = x, Self = ODHConstant.ApplicationURL + "Venue/" + x }).ToList() : new List<VenueLink>();
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
        public EventPublisher EventPublisher { get; set; }
        public bool SignOn { get; set; }

        public EventBooking EventBooking { get; set; }

        public EventPrice EventPrice { get; set; }
    }

    //EventShort Specific
    public class EventEuracNoiInfo
    {
        public bool? ExternalOrganizer { get; set; }
        public bool? SoldOut { get; set; }
        public AgeRange? TypicalAgeRange { get; set; }
        public string EventLocation { get; set; }
    }

    public class EventV2Converter
    {
        public static (IEnumerable<EventsV2>, IEnumerable<VenueLinked>) ConvertEventListToEventV2<T>(IEnumerable<T> events) where T : IIdentifiable
        {
            var eventsV2 = new List<EventsV2>(); 
            var venues = new List<VenueLinked>();

            foreach (var eventv1 in events)
            {
                if (eventv1 is EventShortLinked)
                {
                    var result = ConvertEventShortToEventV2(eventv1 as EventShortLinked);
                    eventsV2.Add(result.Item1);
                    venues.AddRange(result.Item2);
                }
                if (eventv1 is EventLinked)
                {
                    var result = ConvertEventToEventV2(eventv1 as EventLinked);
                    eventsV2.Add(result.Item1);
                    venues.AddRange(result.Item2);
                }                
            }

            return (eventsV2, venues);
        }

        private static (EventsV2, IEnumerable<VenueLinked>) ConvertEventToEventV2(EventLinked eventv1)
        {
            //Try to map all to EventsV2
            EventsV2 eventv2 = new EventsV2();
            var venues = new List<VenueLinked>();

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
            //eventv2.GpsInfo = eventv1.GpsInfo; add to venue
            eventv2.ContactInfos = eventv1.ContactInfos;
            eventv2.Organizer = eventv1.OrganizerInfos;

            if (eventv2.Mapping == null)
                eventv2.Mapping.Add("lts", new Dictionary<string, string>() { { "id", eventv1.Id } });

            //Topics to Tags
            eventv2.Tags = new List<Tags>();

            if (eventv1.TopicRIDs != null)
            {
                foreach (var tag in eventv1.TopicRIDs)
                {
                    eventv2.Tags.Add(new Tags() { Id = tag, Source = "lts" });
                }
            }

            //Creating Venue
            VenueLinked venue = new VenueLinked();
            venue.Id = "";
            venue.Shortname = eventv1.EventAdditionalInfos["en"].Location;
            venue.GpsInfo = eventv1.GpsInfo;
            venue.LocationInfo = eventv1.LocationInfo;

            eventv2.EventInfo = new List<EventInfo>();
            foreach (var eventdate in eventv1.EventDate)
            {
                EventInfo eventinfo = new EventInfo();
                eventinfo.Begin = eventdate.From.Date + eventdate.Begin.Value;
                eventinfo.End = eventdate.To.Date + eventdate.End.Value;

                //Add From, To and Begin End
                eventinfo.VenueIds = new List<string>() { venue.Id };

                eventv2.EventInfo.Add(eventinfo);
            }

            return (eventv2, venues);
        }

        private static (EventsV2, IEnumerable<VenueLinked>)  ConvertEventShortToEventV2(EventShortLinked eventv1)
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
            //eventv2.GpsInfo = eventv1.GpsInfo; //todo add to Venue
            eventv2.LicenseInfo = eventv1.LicenseInfo;
            eventv2.Source = eventv1.Source;

            if(eventv2.Mapping == null)
                eventv2.Mapping.Add("ebms", new Dictionary<string, string>() { { "id", eventv1.EventId.ToString() } });

            //Putting all info into Detail
            eventv2.Detail = new Dictionary<string, Detail>();
            foreach (var lang in eventv2.HasLanguage)
            {
                Detail detail = new Detail() { Title = eventv1.EventTitle[lang], Language = lang, BaseText = eventv1.EventText != null && eventv1.EventText.ContainsKey(lang) ? eventv1.EventText[lang] : "" };

                eventv2.Detail.Add(lang, detail);
            }
            
            //Adding CustomTagging, TechnologyFields to Tags
            eventv2.Tags = new List<Tags>();

            if (eventv1.TechnologyFields != null)
            {
                foreach (var tag in eventv1.TechnologyFields)
                {
                    eventv2.Tags.Add(new Tags() { Id = tag, Source = "noi" });
                }
            }
            if (eventv1.CustomTagging != null)
            {
                foreach (var tag in eventv1.CustomTagging)
                {
                    eventv2.Tags.Add(new Tags() { Id = tag, Source = "noi" });
                }
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

            //WebAddress adding to contactinfo
            eventv2.ContactInfos = new Dictionary<string, ContactInfos>();
            foreach (var lang in eventv1.HasLanguage)
            {
                ContactInfos contactinfo = new ContactInfos();
                contactinfo.Url = eventv1.WebAddress;
                contactinfo.Language = lang;

                contactinfo.Email = eventv1.ContactEmail;
                contactinfo.City = eventv1.ContactCity;
                contactinfo.Address = eventv1.ContactAddressLine1;
                contactinfo.Phonenumber = eventv1.ContactPhone;
                contactinfo.CountryName = eventv1.ContactCountry;
                contactinfo.Surname = eventv1.ContactLastName;
                contactinfo.Givenname = eventv1.ContactFirstName;
                contactinfo.Faxnumber = eventv1.ContactFax;
                contactinfo.ZipCode = eventv1.ContactPostalCode;
                contactinfo.Tax = eventv1.ContactCode;

                eventv2.ContactInfos.Add(lang, contactinfo);
            }

            eventv2.Organizer = new Dictionary<string, ContactInfos>();
            foreach (var lang in eventv1.HasLanguage)
            {
                ContactInfos contactinfo = new ContactInfos();
                contactinfo.Url = eventv1.WebAddress;
                contactinfo.Language = lang;

                contactinfo.CompanyName = eventv1.CompanyName;
                contactinfo.Email = eventv1.CompanyMail;
                contactinfo.City = eventv1.CompanyCity;
                contactinfo.Address = eventv1.CompanyAddressLine1;
                contactinfo.Phonenumber = eventv1.CompanyPhone;
                contactinfo.CountryName = eventv1.CompanyCountry;
                contactinfo.Faxnumber = eventv1.CompanyFax;
                contactinfo.ZipCode = eventv1.CompanyPostalCode;
                contactinfo.Url = eventv1.CompanyUrl;
                contactinfo.Tax = eventv1.CompanyId;

                eventv2.Organizer.Add(lang, contactinfo);
            }

            //Adding EventLocation, AnchorVenue, AnchorVenueRoomMapping, AnchorVenueShort, EndDate, StartDate, StartDateUTC, EndDateUTC, RoomBooked
            var venues = new HashSet<VenueLinked>();

            eventv2.EventInfo = new List<EventInfo>();
            foreach(var room in eventv1.RoomBooked)
            {
                EventInfo eventinfo = new EventInfo();

                eventinfo.Begin = room.StartDate;
                eventinfo.BeginUTC = room.StartDateUTC;
                eventinfo.End = room.EndDate;
                eventinfo.EndUTC = room.EndDateUTC;

                eventinfo.VenueIds = new List<string>();

                eventinfo.Detail = new Dictionary<string, Detail>()
                {
                    { "en", new Detail(){ Title = room.Subtitle } }
                };

                //Space, SpaceDesc, SpaceType, Comment, SpaceAbbrev, SpaceDescRoomMapping

                //Create Venue
                VenueLinked venue = new VenueLinked();
                venue.Id = "eventeuracnoi_" + room.Space.ToLower() + "_" + room.SpaceType;
                venue.Shortname = room.SpaceAbbrev;
                //venue.LocationInfo = Todo create locationinfo
                venue.GpsInfo = eventv1.GpsInfo;

                venue.Detail = new Dictionary<string, Detail>()
                {
                    { "en", new Detail(){ Title = room.SpaceDesc } }
                };

                if (venues.Count == 0 || !venues.Select(x => x.Id).ToList().Contains(venue.Id))
                    venues.Add(venue);

                
                eventinfo.VenueIds.Add(venue.Id);

                eventv2.EventInfo.Add(eventinfo);
            }

            //Video
            if(!String.IsNullOrEmpty(eventv1.VideoUrl))
            {
                eventv2.VideoItems = new Dictionary<string, ICollection<VideoItems>>()
                {
                    {
                        "en", new List<VideoItems>(){ new VideoItems(){ Url = eventv1.VideoUrl } }
                    }
                };
            }


            //ExternalOrganizer, SoldOut, TypicalAgeRange
            EventEuracNoiInfo additionalinfo = new EventEuracNoiInfo();
            additionalinfo.ExternalOrganizer = eventv1.ExternalOrganizer;
            additionalinfo.SoldOut = eventv1.SoldOut;
            additionalinfo.TypicalAgeRange = eventv1.TypicalAgeRange;
            additionalinfo.EventLocation = eventv1.EventLocation;

            eventv2.AdditionalProperties = new Dictionary<string, dynamic>() { { "additionalinfo", additionalinfo } };


            return (eventv2, venues);
        }
    }
}
