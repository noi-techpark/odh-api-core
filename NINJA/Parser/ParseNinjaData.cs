using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DataModel;
using Helper;

namespace NINJA.Parser
{
    public class ParseNinjaData
    {
        //TODO Redefine Mapping ODH - Centro Trevi / Drin
        public static List<TopicLinked> GetTopicRid(string ninjaeventtype)
        {
            TopicLinked topic = new TopicLinked();

            switch (ninjaeventtype)
            {
                case "Evento":
                    topic = new TopicLinked
                    {
                        TopicRID = "C72CE969B98947FABC99CBC7B033F28E",
                        TopicInfo = "Ausstellungen/Kunst"
                    };
                    break;
                case "Mostra":
                    topic = new TopicLinked
                    {
                        TopicRID = "C72CE969B98947FABC99CBC7B033F28E",
                        TopicInfo = "Ausstellungen/Kunst"
                    };
                    break;
                default:
                    topic = new TopicLinked
                    {
                        TopicRID = "C72CE969B98947FABC99CBC7B033F28E",
                        TopicInfo = "Ausstellungen/Kunst"
                    };
                    break;
            }

            return new List<TopicLinked>()
            {
                topic
            };
        }

        public static EventLinked ParseNinjaEventToODHEvent(string id, NinjaEvent ninjaevent, NinjaData<NinjaPlaceRoom> place, NinjaData<NinjaPlaceRoom> room)
        {
            EventLinked myevent = new EventLinked();
            myevent.Id = id.ToUpper();

            string source = !String.IsNullOrEmpty(place.sname) ? place.sname.ToLower() : "ninja";

            Metadata metainfo = new Metadata() { Id = id, LastUpdate = DateTime.Now, Source = source, Type = "event" };
            myevent._Meta = metainfo;

            myevent.Source = source;

            LicenseInfo licenseInfo = new LicenseInfo() { ClosedData = false, Author = "", License = "CC0", LicenseHolder = source };
            myevent.LicenseInfo = licenseInfo;

            //Maybe needeed by DD Transformer
            myevent.Ranc = 0;
            myevent.Type = "1";
            myevent.SignOn = "0";

            //Take only Languages that are defined on title
            var languages = ninjaevent.title.Keys;

            //Detail Info
            foreach (var language in languages)
            {
                Detail mydetail = new Detail();
                mydetail.Language = language;
                mydetail.Title = ninjaevent.title != null ? ninjaevent.title.ContainsKey(language) ? ninjaevent.title[language] : "no title" : "no title";
                mydetail.BaseText = ninjaevent.decription != null ? ninjaevent.decription.ContainsKey(language) ? ninjaevent.decription[language] : "" : "";

                myevent.Detail.TryAddOrUpdate(language, mydetail);
            }

            bool ticket = false;
            string ticketstr = "0";

            string paymet = "0";

            //Ticket and Price Info
            if (ninjaevent.ticket == "Yes")
            {
                //myevent.Ticket = ninjaevent.price;
                ticket = true;
                ticketstr = "1";
                paymet = "1";
            }

            if (ninjaevent.price > 0)
            {
                foreach (var language in languages)
                {
                    EventPrice myeventprice = new EventPrice();
                    myeventprice.Language = language;
                    myeventprice.Price = ninjaevent.price != null ? ninjaevent.price.Value : 0;
                    myeventprice.Type = ninjaevent.event_type_key;

                    myevent.EventPrice.TryAddOrUpdate(language, myeventprice);
                }
            }


            //Add Type info
            myevent.Topics = GetTopicRid(ninjaevent.event_type_key);
            myevent.TopicRIDs = myevent.Topics.Select(x => x.TopicRID).ToList();

            //Console.WriteLine("Parsing: " + ninjaevent.begin_date + " " + ninjaevent.begin_time);

            //CultureInfo myculture = new CultureInfo("it-IT");
            ////Date Info
            //myevent.DateBegin = DateTime.ParseExact(ninjaevent.begin_date + " " + ninjaevent.begin_time, "dd/MM/yyyy HH:mm", myculture);
            //myevent.DateEnd = DateTime.ParseExact(ninjaevent.end_date + " " + ninjaevent.end_time, "dd/MM/yyyy HH:mm", myculture);            

            CultureInfo myculture = new CultureInfo("en-GB");

            myevent.DateBegin = Convert.ToDateTime(ninjaevent.begin_date + " " + ninjaevent.begin_time, myculture);
            myevent.DateEnd = Convert.ToDateTime(ninjaevent.end_date + " " + ninjaevent.end_time, myculture);

            myevent.NextBeginDate = myevent.DateBegin;

            myevent.EventDate = new List<EventDate>()
            {
                new EventDate()
                {
                    Begin = TimeSpan.Parse(ninjaevent.begin_time),
                    From = DateTime.Parse(ninjaevent.begin_date),
                    End = TimeSpan.Parse(ninjaevent.end_time),
                    To = DateTime.Parse(ninjaevent.end_date),
                    Ticket = ticket,
                    MaxPersons = ninjaevent.number_of_seats != null ? ninjaevent.number_of_seats.Value : 0
                }
            };

            myevent.Ticket = ticketstr;
            myevent.PayMet = paymet;

            myevent.Shortname = myevent.Detail.FirstOrDefault().Value.Title;
            myevent.LastChange = DateTime.Now;

            //Gps Info
            myevent.Latitude = place != null ? place.scoordinate.y : 0;
            myevent.Longitude = place != null ? place.scoordinate.x : 0;
            myevent.Gpstype = "position";            

            IDictionary<string, string> floor = new Dictionary<string, string>();
            floor.Add(new KeyValuePair<string, string>("de", "Stock"));
            floor.Add(new KeyValuePair<string, string>("it", "piano"));
            floor.Add(new KeyValuePair<string, string>("en", "floor"));

            //Contact Info            
            foreach (var language in languages)
            {
                if (room != null)
                {
                    string floorstr = " ";
                    if (!String.IsNullOrEmpty(room.smetadata.floor.ToString()) && floor.ContainsKey(language))
                        floorstr = floorstr + room.smetadata.floor + " " + floor[language];

                    ContactInfos mycontact = new ContactInfos();
                    mycontact.Language = language;
                    mycontact.Address = room.smetadata.address != null ? room.smetadata.address.ContainsKey(language) ? room.smetadata.address[language] + floorstr : "" : "";
                    mycontact.City = room.smetadata.city != null ? room.smetadata.city.ContainsKey(language) ? room.smetadata.city[language] : "" : "";
                    mycontact.CompanyName = room.smetadata.name != null ? room.smetadata.name.ContainsKey(language) ? room.smetadata.name[language] : "" : "";
                    mycontact.Phonenumber = room.smetadata.phone;
                    mycontact.Email = room.smetadata.email;
                    mycontact.ZipCode = room.smetadata.zipcode;
                    mycontact.Email = room.smetadata.email;
                    mycontact.CountryCode = "IT";
                    myevent.ContactInfos.TryAddOrUpdate(language, mycontact);
                }
            }

            //Organizer Info
            foreach (var language in languages)
            {
                if (place != null)
                {
                    string floorstr = " ";
                    if (!String.IsNullOrEmpty(place.smetadata.floor.ToString()) && floor.ContainsKey(language))
                        floorstr = floorstr + place.smetadata.floor + " " + floor[language];

                    ContactInfos orgcontact = new ContactInfos();
                    orgcontact.Language = language;
                    orgcontact.Address = place.smetadata.address != null ? place.smetadata.address.ContainsKey(language) ? place.smetadata.address[language] + floorstr : "" : "";
                    orgcontact.City = place.smetadata.city != null ? place.smetadata.city.ContainsKey(language) ? place.smetadata.city[language] : "" : "";
                    orgcontact.CompanyName = place.smetadata.name != null ? place.smetadata.name.ContainsKey(language) ? place.smetadata.name[language] : "" : "";
                    orgcontact.Phonenumber = place.smetadata.phone;
                    orgcontact.Email = place.smetadata.email;
                    orgcontact.ZipCode = place.smetadata.zipcode;
                    orgcontact.CountryCode = "IT";

                    myevent.OrganizerInfos.TryAddOrUpdate(language, orgcontact);
                }
            }

            myevent.OrgRID = place.sname;

            //Event Additional Infos
            foreach (var language in languages)
            {
                EventAdditionalInfos eventadditionalinfo = new EventAdditionalInfos();
                eventadditionalinfo.Language = language;
                eventadditionalinfo.Location = room != null ? room.smetadata.name.ContainsKey(language) ? room.smetadata.name[language] : "" : "";
                eventadditionalinfo.Reg = ninjaevent.link_to_ticket_info;

                myevent.EventAdditionalInfos.TryAddOrUpdate(language, eventadditionalinfo);
            }

            myevent.EventPublisher = new List<EventPublisher>()
            {
                new EventPublisher()
                {
                    Publish = 1,
                    PublisherRID = ninjaevent.place,
                    Ranc = 0
                }
            };

            myevent.HasLanguage = languages;

            myevent.ImageGallery = new List<ImageGallery>();

            return myevent;
        }
    }
}
