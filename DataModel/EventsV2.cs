// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class EventsV2 : IIdentifiable, IActivateable, IHasLanguage, IImageGalleryAware, IContactInfosAware, IMetaData, IMappingAware, IDetailInfosAware, ILicenseInfo, IGPSPointsAware, IPublishedOn
    {
        //MetaData Information, Contains Source, LastUpdate, 
        public Metadata? _Meta { get; set; }

        //Self Link to this Data
        public string Self
        {
            get
            {
                return "Events/" + Uri.EscapeDataString(this.Id);
            }
        }
        public string? Id { get; set; }
        public string? Shortname { get; set; }
        public bool Active { get; set; }        
        public LicenseInfo? LicenseInfo { get; set; }
        

        public IDictionary<string, Detail> Detail { get; set; }
        public ICollection<ImageGallery>? ImageGallery { get; set; }
        public IDictionary<string, ContactInfos> ContactInfos { get; set; }
        public LocationInfoLinked? LocationInfo { get; set; }
        public IDictionary<string, GpsInfo> GpsPoints { get; set; }

        public ICollection<string>? HasLanguage { get; set; }


        public ICollection<string>? PublishedOn { get; set; }
        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }
        public ICollection<RelatedContent>? RelatedContent { get; set; }

        public List<Tags> Tags { get; set; }

        //TODO Add Venue

        //TODO Add Subevent

        //TODO Add Documents

        //TODO Add EventDates

        //TODO Properties LIST
        public IDictionary<string,string>? Properties { get; set; }

        //TODO Properties Language Based LIST
        public IDictionary<string, IDictionary<string,string>>? PropertiesLocalized { get; set; }

    }

    public class DateRanges
    {
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }


    }



}
