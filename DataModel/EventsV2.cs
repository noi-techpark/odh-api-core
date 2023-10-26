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
    public class EventsV2 : IIdentifiable, IActivateable, IImageGalleryAware, IContactInfosAware, IImportDateassigneable, IMetaData, IMappingAware, ISource, IDetailInfosAware, ILicenseInfo, IGPSPointsAware, IPublishedOn
    {
        public Metadata? _Meta { get; set; }

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
        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }
        public LicenseInfo? LicenseInfo { get; set; }
        public string Source { get; set; }


        public IDictionary<string, Detail> Detail { get; set; }
        public ICollection<ImageGallery>? ImageGallery { get; set; }
        public IDictionary<string, ContactInfos> ContactInfos { get; set; }
        public new LocationInfoLinked? LocationInfo { get; set; }
        public IDictionary<string, GpsInfo> GpsPoints { get; set; }



        public ICollection<string>? PublishedOn { get; set; }
        public IDictionary<string, IDictionary<string, string>> Mapping { get; set; }
        public ICollection<RelatedContent>? RelatedContent { get; set; }

        public List<Tags> Tags { get; set; }

        //TODO Add Venue

        //TODO Add Subevent

        //TODO Add Documents

        //TODO Add EventDates

        //TODO Properties LIST

        //TODO Properties Language Based LIST

    }



}
