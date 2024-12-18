// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBMS
{
    public class EBMSEventREST
    {
        public int EventId { get; set; }
        public string EventDescription { get; set; }
        public string EventDescriptionAlt1 { get; set; }
        public string EventDescriptionAlt2 { get; set; }
        public string EventDescriptionAlt3 { get; set; }
        public string AnchorVenue { get; set; }
        public string AnchorVenueShort { get; set; }
        public DateTime ChangedOn { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string WebAddress { get; set; }
        public string Display1 { get; set; }
        public string Display2 { get; set; }
        public string Display3 { get; set; }
        public string Display4 { get; set; }
        public string Display5 { get; set; }
        public string Display6 { get; set; }
        public string Display7 { get; set; }
        public string Display8 { get; set; }
        public string Display9 { get; set; }
        public EBMSCompany[] Company { get; set; }
        public EBMSContact[] Contact { get; set; }
        public EBMSBooking[] Bookings { get; set; }
        //public <Nullable>string Notes { get; set; }
        //public <Nullable>string Documents { get; set; }
    }

    public class EBMSCompany
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Mail { get; set; }
        public string Url { get; set; }
    }

    public class EBMSContact
    {
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Cell { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
    }

    public class EBMSBooking
    {
        public string Space { get; set; }
        public string SpaceDesc { get; set; }
        public string SpaceAbbrev { get; set; }
        public string SpaceType { get; set; }
        public string Subtitle { get; set; }
        public string Comment { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
