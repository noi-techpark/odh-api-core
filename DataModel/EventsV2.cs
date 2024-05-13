// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataModel
{
    #region EventsV2 Datamodel
    public class EventV2 : IIdentifiable, IActivateable, IHasLanguage, IImageGalleryAware, IContactInfosAware, IMetaData, IMappingAware, IDetailInfosAware, ILicenseInfo, IPublishedOn, IVideoItemsAware, IImportDateassigneable, ISource
    {
        public EventV2()
        {
            Detail = new Dictionary<string, Detail>();
            ContactInfos = new Dictionary<string, ContactInfos>();
            Mapping = new Dictionary<string, IDictionary<string, string>>();
            AdditionalProperties = new Dictionary<string, dynamic>();
            VideoItems = new Dictionary<string, ICollection<VideoItems>>();
        }

        //MetaData Information, Contains Source, LastUpdate
        public Metadata? _Meta { get; set; }

        //License Information
        public LicenseInfo? LicenseInfo { get; set; }

        //Self Link to this Data
        public string Self
        {
            get
            {
                return this.Id != null ? "EventV2/" + Uri.EscapeDataString(this.Id) : null;                
            }
        }

        //Id Shortname and Active Info
        public string? Id { get; set; }
        public string? Shortname { get; set; }
        public bool Active { get; set; }
        
        //Firstimport and LastChange Section (Here for compatibility reasons could also be removed)
        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }

        //Source 
        public string? Source { get; set; }

        //HasLanguage, for which Languages the dataset has information
        public ICollection<string>? HasLanguage { get; set; }
        
        //Publishedon Array, Event is published for channel xy
        public ICollection<string>? PublishedOn { get; set; }
        
        //Mapping Section, to store Ids and other information of the data provider
        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }
        
        //RelatedContent, could be used to store Parent/Child Event Information
        public ICollection<RelatedContent>? RelatedContent { get; set; }

        //Indicates if this is a Parent Event
        public bool? IsRoot { get; set; }

        //Dynamic AdditionalProperties field to store Provider Specific data that does not fit into the fields
        public IDictionary<string, dynamic> AdditionalProperties { get; set; }

        //Converting EventTopis to Tags so we have the same structure 
        public ICollection<TagV2> Tags { get; set; }

        //Description and Contactinfo
        public IDictionary<string, Detail> Detail { get; set; }
        public IDictionary<string, ContactInfos> ContactInfos { get; set; }        

        //ImageGallery and Video Data
        public ICollection<ImageGallery>? ImageGallery { get; set; }
        public IDictionary<string, ICollection<VideoItems>>? VideoItems { get; set; }        

        //Documents for this Event
        public IDictionary<string, List<DocumentDetailed>>? Documents { get; set; }

        //EventInfo Section contains all Infos about Event Dates, Venues etc....
        public ICollection<EventInfo> EventInfo { get; set; }

        //Each Event has a "main" Venue, to discuss if this 
        public List<string> VenueIds { get; set; }

        [SwaggerSchema(Description = "generated field", ReadOnly = true)]
        public ICollection<VenueLink> Venues
        {
            get
            {
                return this.VenueIds != null ? this.VenueIds.Select(x => new VenueLink() { Id = x, Self = "VenueV2/" + x }).ToList() : new List<VenueLink>();
            }
        }

        //TO Check, section for Event URLS?

        //TO Check, section for Booking Info        
    }

    public class VenueLink
    {
        public string Id { get; set; }
        public string? Self { get; set; }
    }

    public class EventInfo
    {
        //Begin and Enddate
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }

        //Begin and Enddate in UTC (could be created automatically)
        public double BeginUTC { get; set; }
        public double EndUTC { get; set; }

        //Assigned Venue
        public List<string> VenueIds { get; set; }

        [SwaggerSchema(Description = "generated field", ReadOnly = true)]
        public ICollection<VenueLink> Venues
        {
            get
            {
                return this.VenueIds != null ? this.VenueIds.Select(x => new VenueLink() { Id = x, Self = "VenueV2/" + x }).ToList() : new List<VenueLink>();
            }
        }

        //Dynamic Additional Properties field
        public IDictionary<string, dynamic> AdditionalProperties { get; set; }

        //Detail Information
        public IDictionary<string, Detail> Detail { get; set; }

        //Documents
        public IDictionary<string, List<DocumentDetailed>?> Documents { get; set; }        

        //Capacity of the Event Venue Combination (not always the same as the Venue Capacity)
        public int? Capacity { get; set; }
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

    #endregion

    #region VenueV2 Datamodel

    public class VenueV2: IIdentifiable, IActivateable, IHasLanguage, IImageGalleryAware, IContactInfosAware, IMetaData, IMappingAware, IDetailInfosAware, ILicenseInfo, IPublishedOn, IVideoItemsAware, IImportDateassigneable, ISource
    {
        public VenueV2()
        {
            Detail = new Dictionary<string, Detail>();
            ContactInfos = new Dictionary<string, ContactInfos>();
            Mapping = new Dictionary<string, IDictionary<string, string>>();
            AdditionalProperties = new Dictionary<string, dynamic>();
            VideoItems = new Dictionary<string, ICollection<VideoItems>>();
        }

        //MetaData Information, Contains Source, LastUpdate
        public Metadata? _Meta { get; set; }

        //License Information
        public LicenseInfo? LicenseInfo { get; set; }

        //Self Link to this Data
        public string Self
        {
            get
            {
                return this.Id != null ? "VenueV2/" + Uri.EscapeDataString(this.Id) : null;
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

        //We only store the Info which is the Parent
        public bool? IsRoot { get; set; }

        public IDictionary<string, dynamic> AdditionalProperties { get; set; }
      
        //Description and Contactinfo
        public IDictionary<string, Detail> Detail { get; set; }
        public IDictionary<string, ContactInfos> ContactInfos { get; set; }

        //ImageGallery
        public ICollection<ImageGallery>? ImageGallery { get; set; }
        public IDictionary<string, ICollection<VideoItems>>? VideoItems { get; set; }


        public VenueInfo VenueInfo { get; set; }
        public LocationInfo? LocationInfo { get; set; }                
        public ICollection<GpsInfo>? GpsInfo { get; set; }
                
        public DistanceInfo? DistanceInfo { get; set; }
        public ICollection<OperationSchedule>? OperationSchedule { get; set; }
        
        public ICollection<VenueSetupV2>? Capacity { get; set; }

        //TO CHECK Tags Categorization is done via Tags ?????
        public ICollection<TagV2> Tags { get; set; }
    }

    public class TagV2 : Tags
    {
        public string Code { get; set; }
    }

    public class VenueSetupV2 : TagV2
    {
        public int Capacity { get; set; }      
    }

    public class VenueInfo
    {
        public int? Beds { get; set; }
        public int? RoomCount { get; set; }
        public int? SquareMeters { get; set; }
        public bool? Indoor { get; set; }               
    }

    #endregion

    #region Event V2 Converters

    public class EventV2Converter
    {
        public static (IEnumerable<EventV2>, IEnumerable<VenueV2>) ConvertEventListToEventV2<T>(IEnumerable<T> events) where T : IIdentifiable
        {
            var eventsV2 = new List<EventV2>();
            var venues = new List<VenueV2>();

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

        private static (EventV2, IEnumerable<VenueV2>) ConvertEventToEventV2(EventLinked eventv1)
        {
            //Try to map all to EventsV2
            EventV2 eventv2 = new EventV2();
            var venues = new List<VenueV2>();

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

            //Where can we store the Organizerinfo???
            //eventv2.Organizer = eventv1.OrganizerInfos;

            if(eventv2.Mapping.ContainsKey("lts"))
            {
                if(!eventv2.Mapping["lts"].ContainsKey("rid"))
                    eventv2.Mapping["lts"].Add("rid", eventv1.Id);
                if (!eventv2.Mapping["lts"].ContainsKey("classificationrid"))
                    eventv2.Mapping["lts"].Add("classificationrid", eventv1.ClassificationRID);                
            }
            else
                eventv2.Mapping.Add("lts", new Dictionary<string, string>() { { "rid", eventv1.Id }, { "classificationrid", eventv1.ClassificationRID } });

            //Topics to Tags
            eventv2.Tags = new List<TagV2>();

            if (eventv1.TopicRIDs != null)
            {
                foreach (var tag in eventv1.TopicRIDs)
                {
                    eventv2.Tags.Add(new TagV2() { Id = tag, Code = "", Source = "lts" });
                }
            }

            //Creating Venue
            VenueV2 venue = new VenueV2();

            string venuename = eventv1.EventAdditionalInfos.GetEnglishOrFirstKeyFromDictionary().Location;

            if (String.IsNullOrEmpty(venuename))
                venuename = eventv1.EventAdditionalInfos.GetEnglishOrFirstKeyFromDictionary().Mplace;

            venue.Id = Regex.Replace(eventv1.ContactInfos.GetEnglishOrFirstKeyFromDictionary().CompanyName, "[^0-9a-zA-Z]+", ""); //What should we use as Id?

            venue.Shortname = venuename;
            venue.GpsInfo = eventv1.GpsInfo;
            venue.LocationInfo = eventv1.LocationInfo;
            venue.ContactInfos = eventv1.ContactInfos;

            venue.Detail = new Dictionary<string, Detail>();

            foreach (var lang in eventv1.HasLanguage)
            {
                Detail venuedetail = new Detail();
                venuedetail.Language = lang;

                string venuetitle = eventv1.EventAdditionalInfos[lang].Location;

                if (String.IsNullOrEmpty(venuename))
                    venuetitle = eventv1.EventAdditionalInfos[lang].Mplace;

                venuedetail.Title = venuetitle;
                venuedetail.BaseText = eventv1.EventAdditionalInfos[lang].Reg;

                venue.Detail[lang] = venuedetail;
            }

            venues.Add(venue);

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

        private static (EventV2, IEnumerable<VenueV2>) ConvertEventShortToEventV2(EventShortLinked eventv1)
        {
            //Try to map all to EventsV2
            EventV2 eventv2 = new EventV2();

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

            if (eventv2.Mapping == null)
                eventv2.Mapping.Add("ebms", new Dictionary<string, string>() { { "id", eventv1.EventId.ToString() } });

            //Putting all info into Detail
            eventv2.Detail = new Dictionary<string, Detail>();
            foreach (var lang in eventv2.HasLanguage)
            {
                Detail detail = new Detail() { Title = eventv1.EventTitle[lang], Language = lang, BaseText = eventv1.EventText != null && eventv1.EventText.ContainsKey(lang) ? eventv1.EventText[lang] : "" };

                eventv2.Detail.Add(lang, detail);
            }

            //Adding CustomTagging, TechnologyFields to Tags
            eventv2.Tags = new List<TagV2>();

            if (eventv1.TechnologyFields != null)
            {
                foreach (var tag in eventv1.TechnologyFields)
                {
                    eventv2.Tags.Add(new TagV2() { Id = tag, Source = "noi", Code = "" });
                }
            }
            if (eventv1.CustomTagging != null)
            {
                foreach (var tag in eventv1.CustomTagging)
                {
                    eventv2.Tags.Add(new TagV2() { Id = tag, Source = "noi", Code = "" });
                }
            }

            //Adding EventDocument, Documents as DocumentDetailed
            eventv2.Documents = new Dictionary<string, List<DocumentDetailed>>();
            foreach (var documentkvp in eventv1.Documents)
            {
                List<DocumentDetailed> documents = new List<DocumentDetailed>();
                foreach (var doc in documentkvp.Value)
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

            //Where to store the OrganizerInfo??
            //eventv2.Organizer = new Dictionary<string, ContactInfos>();
            //foreach (var lang in eventv1.HasLanguage)
            //{
            //    ContactInfos contactinfo = new ContactInfos();
            //    contactinfo.Url = eventv1.WebAddress;
            //    contactinfo.Language = lang;

            //    contactinfo.CompanyName = eventv1.CompanyName;
            //    contactinfo.Email = eventv1.CompanyMail;
            //    contactinfo.City = eventv1.CompanyCity;
            //    contactinfo.Address = eventv1.CompanyAddressLine1;
            //    contactinfo.Phonenumber = eventv1.CompanyPhone;
            //    contactinfo.CountryName = eventv1.CompanyCountry;
            //    contactinfo.Faxnumber = eventv1.CompanyFax;
            //    contactinfo.ZipCode = eventv1.CompanyPostalCode;
            //    contactinfo.Url = eventv1.CompanyUrl;
            //    contactinfo.Tax = eventv1.CompanyId;

            //    eventv2.Organizer.Add(lang, contactinfo);
            //}

            //Adding EventLocation, AnchorVenue, AnchorVenueRoomMapping, AnchorVenueShort, EndDate, StartDate, StartDateUTC, EndDateUTC, RoomBooked
            var venues = new HashSet<VenueV2>();

            eventv2.EventInfo = new List<EventInfo>();
            foreach (var room in eventv1.RoomBooked)
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
                VenueV2 venue = new VenueV2();
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
            if (!String.IsNullOrEmpty(eventv1.VideoUrl))
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

    public static class DictionaryExtensionsTemp
    {
        //TODO Migrate
        public static T GetEnglishOrFirstKeyFromDictionary<T>(this IDictionary<string, T> dict)
        {
            if (dict.ContainsKey("en"))
                return dict["en"];
            else
                return dict.FirstOrDefault().Value;
        }
    }

    #endregion

    #region VenueV2 Converter

    public class VenueV2Converter
    {
        public static IEnumerable<VenueV2> ConvertVenueListToVenueV2(IEnumerable<VenueLinked> venues)
        {            
            var venuestoreturn = new List<VenueV2>();

            foreach (var venue in venues)
            {
               
                var result = ConvertVenueLinkedToVenueV2(venue);
                venuestoreturn.AddRange(result);               
            }

            return venuestoreturn;
        }

        private static IEnumerable<VenueV2> ConvertVenueLinkedToVenueV2(VenueLinked venuev1)
        {
            List<VenueV2> venues = new List<VenueV2>();

            //Root Venue
            VenueV2 venuev2 = new VenueV2();

            venuev2.Id = venuev1.Id;
            venuev2.HasLanguage = venuev1.HasLanguage;
            venuev2.Active = venuev1.Active;
            venuev2._Meta = venuev1._Meta;
            venuev2.LastChange = venuev1.LastChange;
            venuev2.FirstImport = venuev1.FirstImport;
            venuev2.LicenseInfo = venuev1.LicenseInfo;
            venuev2.Shortname = venuev1.Shortname;
            venuev2.PublishedOn = venuev1.PublishedOn;
            venuev2.Source = venuev1.Source;


            venuev2.IsRoot = true;
            venuev2.Detail = venuev1.Detail;
            venuev2.ImageGallery = venuev1.ImageGallery;
            venuev2.ContactInfos = venuev1.ContactInfos;
            venuev2.GpsInfo = venuev1.GpsInfo;

            venuev2.VenueInfo = new VenueInfo();
            venuev2.VenueInfo.Indoor = null;
            venuev2.VenueInfo.SquareMeters = null;
            venuev2.VenueInfo.Beds = venuev1.Beds;
            venuev2.VenueInfo.RoomCount = venuev1.RoomCount;

            venuev2.Tags = ConvertVenueFeatureToTag(venuev1.VenueCategory).ToList();

            venuev2.OperationSchedule = venuev1.OperationSchedule;
            venuev2.LocationInfo = venuev1.LocationInfo;

            //Add mapping
            venuev2.Mapping.Add("lts", new Dictionary<string, string>() { { "rid", venuev2.Id } });


            venuev2.RelatedContent = new List<RelatedContent>();


            //Subvenue
            foreach (var subvenuev1 in venuev1.RoomDetails)
            {
                VenueV2 subvenuev2 = new VenueV2();
                subvenuev2.Id = subvenuev1.Id;
                //Infos from root
                subvenuev2.HasLanguage = venuev1.HasLanguage;
                subvenuev2.Active = venuev1.Active;
                subvenuev2._Meta = venuev1._Meta;
                subvenuev2.LastChange = venuev1.LastChange;
                subvenuev2.FirstImport = venuev1.FirstImport;
                subvenuev2.LicenseInfo = venuev1.LicenseInfo;                
                subvenuev2.PublishedOn = venuev1.PublishedOn;
                subvenuev2.Source = venuev1.Source;
                subvenuev2.GpsInfo = venuev1.GpsInfo;
                subvenuev2.LocationInfo = venuev1.LocationInfo;

                subvenuev2.IsRoot = false;
                subvenuev2.Detail = subvenuev1.Detail;
                subvenuev2.ImageGallery = subvenuev1.ImageGallery;
                subvenuev2.Shortname = subvenuev1.Shortname;
                subvenuev2.VenueInfo = new VenueInfo();
                subvenuev2.VenueInfo.Indoor = subvenuev1.Indoor;
                subvenuev2.VenueInfo.SquareMeters = subvenuev1.SquareMeters;
                subvenuev2.VenueInfo.Beds = null;
                subvenuev2.VenueInfo.RoomCount = null;

                subvenuev2.Tags = ConvertVenueFeatureToTag(subvenuev1.VenueFeatures).ToList();
                subvenuev2.Capacity = ConvertVenueSetupToTag(subvenuev1.VenueSetup).ToList();

                //Add relation
                subvenuev2.RelatedContent = new List<RelatedContent>();
                subvenuev2.RelatedContent.Add(new RelatedContent() { Id = venuev2.Id, Type = "venue" });

                //Add mapping
                subvenuev2.Mapping.Add("lts", new Dictionary<string, string>() { { "rid", subvenuev2.Id } });
                subvenuev2.Mapping.Add("noi", new Dictionary<string, string>() { { "parent_Id", venuev2.Id } });

                venues.Add(subvenuev2);

                venuev2.RelatedContent.Add(new RelatedContent() { Id = subvenuev2.Id, Type = "venue" });
            }

            venues.Add(venuev2);

            return venues;
        }      

        private static IEnumerable<VenueSetupV2> ConvertVenueSetupToTag(IEnumerable<VenueSetup> venuesetups)
        {
            List<VenueSetupV2> tagstoreturn = new List<VenueSetupV2>();
            foreach (var venuesetup in venuesetups)
            {
                VenueSetupV2 tagv2 = new VenueSetupV2();
                tagv2.Id = venuesetup.Id;
                tagv2.Code = venuesetup.VenueCode;
                tagv2.Capacity = venuesetup.Capacity;
                tagv2.Source = "lts";

                tagstoreturn.Add(tagv2);
            }

            return tagstoreturn;
        }

        private static IEnumerable<TagV2> ConvertVenueFeatureToTag(IEnumerable<VenueType> venuetypes)
        {
            List<TagV2> tagstoreturn = new List<TagV2>();
            foreach(var venuetype in venuetypes)
            {
                TagV2 tagv2 = new TagV2();
                tagv2.Id = venuetype.Id;
                tagv2.Code = venuetype.VenueCode;
                tagv2.Source = "lts";                

                tagstoreturn.Add(tagv2);
            }

            return tagstoreturn;
        }

    }


    #endregion


}
