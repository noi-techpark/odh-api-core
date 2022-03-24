using DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EBMS
{
    public class ImportEBMSData
    {
        public const string serviceurlebmsrest = @"https://emea-interface.ungerboeck.com/clients/Bozen/PROD/EventExportAPI/api/event/masterdata/?organization=20";

        public static async Task<string> GetEBMSEventsFromService(string user, string pass)
        {
            try
            {
                CredentialCache wrCache = new CredentialCache();
                wrCache.Add(new Uri(serviceurlebmsrest), "Basic", new NetworkCredential(user, pass));

                using (var handler = new HttpClientHandler { Credentials = wrCache })
                {
                    using (var client = new HttpClient(handler))
                    {
                        var myresponse = await client.GetAsync(serviceurlebmsrest);

                        var myresponsestring = await myresponse.Content.ReadAsStringAsync();

                        return myresponsestring;
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static List<Tuple<EventShortLinked, EBMSEventREST>> GetEbmsEvents(string user, string pass)
        {
            try
            {
                List<Tuple<EventShortLinked, EBMSEventREST>> myeventshortlist = new List<Tuple<EventShortLinked, EBMSEventREST>>();

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                var response = GetEBMSEventsFromService(user, pass).Result;

                var eventarray = JsonConvert.DeserializeObject<List<EBMSEventREST>>(response);

                foreach (var myevent in eventarray)
                {
                    var eventtosave = new EventShortLinked();

                    //CUSTOM Props

                    eventtosave.Id = "eventshort-" + myevent.EventId;
                    eventtosave.Source = "EBMS";

                    //ADD MAPPING
                    var dssrid = new Dictionary<string, string>() { { "rid", myevent.EventId.ToString() } };
                    eventtosave.Mapping.TryAddOrUpdate("ebms", dssrid);


                    //Interface Props

                    eventtosave.EventId = myevent.EventId;
                    //Hauptbeschreibung
                    eventtosave.EventDescription = myevent.EventDescription ?? "";
                    //Beschreibung DE
                    eventtosave.EventDescriptionDE = myevent.EventDescriptionAlt1 ?? "";
                    //Beschreibung IT
                    eventtosave.EventDescriptionIT = myevent.EventDescriptionAlt2 ?? "";
                    //Beschreibung EN
                    eventtosave.EventDescriptionEN = myevent.EventDescriptionAlt3 ?? "";

                    eventtosave.Shortname = eventtosave.EventDescription;

                    //Hauptsaal/ort
                    eventtosave.AnchorVenue = myevent.AnchorVenue;
                    //Hauptsaal/ort soll für die Ausgabe verwendet werden
                    eventtosave.AnchorVenueShort = myevent.AnchorVenueShort ?? "";
                    //letzte Änderung
                    eventtosave.ChangedOn = myevent.ChangedOn;
                    eventtosave.LastChange = myevent.ChangedOn;
                    //Beginndatum
                    eventtosave.StartDate = new DateTime(myevent.StartDate.Year, myevent.StartDate.Month, myevent.StartDate.Day, myevent.StartTime.Hour, myevent.StartTime.Minute, myevent.StartTime.Second);
                    //Beginnzeit
                    //eventtosave.StartTime = myevent.StartTime;
                    //Ende Datum
                    eventtosave.EndDate = new DateTime(myevent.EndDate.Year, myevent.EndDate.Month, myevent.EndDate.Day, myevent.EndTime.Hour, myevent.EndTime.Minute, myevent.EndTime.Second);
                    //Endzeit

                    //UTC
                    //Int32 unixTimestampEStart = (Int32)(DateTime.UtcNow.Subtract(eventtosave.StartDate)).TotalSeconds;
                    //Int32 unixTimestampEEnd = (Int32)(DateTime.UtcNow.Subtract()).TotalSeconds;
                    eventtosave.EndDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(eventtosave.EndDate);
                    eventtosave.StartDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(eventtosave.StartDate);


                    //eventtosave.EndTime = myevent.EndTime;
                    //URL für externe Webseite (noch nicht ausgefüllt)
                    eventtosave.WebAddress = myevent.WebAddress ?? "";
                    //Spezialfelder

                    //FÜR NOI ANZEIGE AKTIV (Oklären ob des no der Foll isch)
                    eventtosave.Display1 = myevent.Display1;
                    //Intranet Eurac (Y / N)
                    eventtosave.Display2 = myevent.Display2;
                    //Webseite Eurac ( Y /N)
                    eventtosave.Display3 = myevent.Display3;
                    //diese sind nicht belegt, könnten verwendet werden

                    //Eurac Videowall (Y / N) Wenn hier N wird ganzes Event nicht angezeigt
                    eventtosave.Display4 = myevent.Display4;

                    if (String.IsNullOrEmpty(myevent.Display4))
                        eventtosave.Display4 = "N";

                    eventtosave.Display5 = myevent.Display5;
                    eventtosave.Display6 = myevent.Display6;
                    eventtosave.Display7 = myevent.Display7;
                    eventtosave.Display8 = myevent.Display8;
                    eventtosave.Display9 = myevent.Display9;

                    if (myevent.Company != null)
                    {
                        var myeventcompany = myevent.Company.FirstOrDefault();

                        //CRM Modul Account (Firma) interessiert uns nicht
                        eventtosave.CompanyName = myeventcompany.Name ?? "";
                        eventtosave.CompanyId = myeventcompany.Code ?? "";
                        eventtosave.CompanyAddressLine1 = myeventcompany.AddressLine1 ?? "";
                        eventtosave.CompanyAddressLine2 = myeventcompany.AddressLine2 ?? "";
                        eventtosave.CompanyAddressLine3 = myeventcompany.AddressLine3 ?? "";
                        eventtosave.CompanyPostalCode = myeventcompany.PostalCode ?? "";
                        eventtosave.CompanyCity = myeventcompany.City ?? "";
                        eventtosave.CompanyCountry = myeventcompany.Country ?? "";
                        eventtosave.CompanyPhone = myeventcompany.Phone ?? "";
                        eventtosave.CompanyFax = myeventcompany.Fax ?? "";
                        eventtosave.CompanyMail = myeventcompany.Mail ?? "";
                        eventtosave.CompanyUrl = myeventcompany.Url ?? "";
                    }
                    else
                    {
                        eventtosave.CompanyName = "";
                        eventtosave.CompanyId = "";
                        eventtosave.CompanyAddressLine1 = "";
                        eventtosave.CompanyAddressLine2 = "";
                        eventtosave.CompanyAddressLine3 = "";
                        eventtosave.CompanyPostalCode = "";
                        eventtosave.CompanyCity = "";
                        eventtosave.CompanyCountry = "";
                        eventtosave.CompanyPhone = "";
                        eventtosave.CompanyFax = "";
                        eventtosave.CompanyMail = "";
                        eventtosave.CompanyUrl = "";
                    }

                    if (myevent.Contact != null)
                    {
                        var myeventcontact = myevent.Contact.FirstOrDefault();

                        //Person aus Modul CRM (interessiert uns nicht)
                        eventtosave.ContactCode = myeventcontact.Code ?? "";
                        eventtosave.ContactFirstName = myeventcontact.FirstName ?? "";
                        eventtosave.ContactLastName = myeventcontact.LastName ?? "";
                        eventtosave.ContactPhone = myeventcontact.Phone ?? "";
                        eventtosave.ContactCell = myeventcontact.Cell ?? "";
                        eventtosave.ContactFax = myeventcontact.Fax ?? "";
                        eventtosave.ContactEmail = myeventcontact.Email ?? "";
                        eventtosave.ContactAddressLine1 = myeventcontact.AddressLine1 ?? "";
                        eventtosave.ContactAddressLine2 = myeventcontact.AddressLine2 ?? "";
                        eventtosave.ContactAddressLine3 = myeventcontact.AddressLine3 ?? "";
                        eventtosave.ContactPostalCode = myeventcontact.PostalCode ?? "";
                        eventtosave.ContactCity = myeventcontact.City ?? "";
                        eventtosave.ContactCountry = myeventcontact.Country ?? "";
                    }
                    else
                    {
                        //To be compatible
                        eventtosave.ContactCode = "";
                        eventtosave.ContactFirstName = "";
                        eventtosave.ContactLastName = "";
                        eventtosave.ContactPhone = "";
                        eventtosave.ContactCell = "";
                        eventtosave.ContactFax = "";
                        eventtosave.ContactEmail = "";
                        eventtosave.ContactAddressLine1 = "";
                        eventtosave.ContactAddressLine2 = "";
                        eventtosave.ContactAddressLine3 = "";
                        eventtosave.ContactPostalCode = "";
                        eventtosave.ContactCity = "";
                        eventtosave.ContactCountry = "";
                    }

                    //gebuchten Sääle von spezifischen Event
                    //Space : Code für Raum von DB
                    //SpaceDesc: Beschreibung --> zu nehmen
                    //SpaceAbbrev: Abgekürzte Beschreibung 
                    //SoaceType : EC = Eurac, NO = Noi
                    //Comnment: entweder x oder leer --> x bedeutet bitte nicht anzeigen!!!!!!!
                    //Subtitle: Untertitel vom Saal (anzeigen)
                    //Zeiten (diese sind relevant, diese anzeigen)

                    List<RoomBooked> mybookingroomlist = new List<RoomBooked>();

                    string eventlocation = "";

                    if (myevent.Bookings != null)
                    {
                        foreach (var bookingroom in myevent.Bookings)
                        {
                            RoomBooked myroom = new RoomBooked();
                            myroom.Comment = bookingroom.Comment ?? "";
                            myroom.EndDate = new DateTime(bookingroom.EndDate.Year, bookingroom.EndDate.Month, bookingroom.EndDate.Day, bookingroom.EndTime.Hour, bookingroom.EndTime.Minute, bookingroom.EndTime.Second);
                            myroom.StartDate = new DateTime(bookingroom.StartDate.Year, bookingroom.StartDate.Month, bookingroom.StartDate.Day, bookingroom.StartTime.Hour, bookingroom.StartTime.Minute, bookingroom.StartTime.Second);

                            //Int32 unixTimestampStart = (Int32)(DateTime.UtcNow.Subtract(myroom.StartDate)).TotalSeconds;
                            //Int32 unixTimestampEnd = (Int32)(DateTime.UtcNow.Subtract(myroom.EndDate)).TotalSeconds;

                            myroom.EndDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(myroom.EndDate);
                            myroom.StartDateUTC = Helper.DateTimeHelper.DateTimeToUnixTimestampMilliseconds(myroom.StartDate);


                            myroom.Space = bookingroom.Space ?? "";
                            myroom.SpaceAbbrev = bookingroom.SpaceAbbrev ?? "";
                            myroom.SpaceDesc = bookingroom.SpaceDesc ?? "";
                            myroom.SpaceType = bookingroom.SpaceType ?? "";
                            myroom.Subtitle = bookingroom.Subtitle ?? "";

                            if (bookingroom.SpaceType == "EC")
                                eventlocation = "EC";
                            else if (bookingroom.SpaceType == "NO")
                                eventlocation = "NOI";

                            mybookingroomlist.Add(myroom);
                        }

                        eventtosave.RoomBooked = mybookingroomlist;

                        eventtosave.EventLocation = eventlocation;

                        if (eventlocation == "NOI")
                            eventtosave.Display1 = eventtosave.Display4;


                        //all das interessiert nicht
                        //eventtosave.AbstractsEN = myevent.AbstractsEN;
                        //eventtosave.AbstractsGER = myevent.AbstractsGER;
                        //eventtosave.AbstractsIT = myevent.AbstractsIT;
                        ////gehört zu Abstract
                        //eventtosave.Documents = myevent.Documents;

                        myeventshortlist.Add(Tuple.Create(eventtosave, myevent));                        
                    }
                }
                
                return myeventshortlist;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);

                return null;
            }
        }
    }

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
