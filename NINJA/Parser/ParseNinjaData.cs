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
                case "Convegni/conferenze":
                    topic = new TopicLinked
                    {
                        TopicRID = "0D25868CC23242D6AC97AEB2973CB3D6",
                        TopicInfo = "Tagungen/Vorträge"
                    };
                    break;
                case "Sport":
                    topic = new TopicLinked
                    {
                        TopicRID = "162C0067811B477DA725D2F5F2D98398",
                        TopicInfo = "Sport"
                    };
                    break;
                case "Enogastronomia/prodotti":
                    topic = new TopicLinked
                    {
                        TopicRID = "252200A028C8449D9A6205369A6D0D36",
                        TopicInfo = "Gastronomie/Typische Produkte"
                    };
                    break;
                case "Artigianato/tradizioni":
                    topic = new TopicLinked
                    {
                        TopicRID = "33BDC54BD39946F4852B3394B00610AE",
                        TopicInfo = "Handwerk/Brauchtum"
                    };
                    break;
                case "Fiere/mercati":
                    topic = new TopicLinked
                    {
                        TopicRID = "4C4961D9FC5B48EEB73067BEB9D4402A",
                        TopicInfo = "Messen/Märkte"
                    };
                    break;
                case "Teatro/cinema":
                    topic = new TopicLinked
                    {
                        TopicRID = "6884FE362C88434B9F49725E3328112B",
                        TopicInfo = "Theater/Vorführungen"
                    };
                    break;
                case "Corsi/lezioni":
                    topic = new TopicLinked
                    {
                        TopicRID = "767F6F43FC394CE9A3C8A9725C6FF134",
                        TopicInfo = "Kurse/Bildung"
                    };
                    break;
                case "Musica/danza":
                    topic = new TopicLinked
                    {
                        TopicRID = "7E048074BA004EC58E29E330A9AA476B",
                        TopicInfo = "Musik/Tanz"
                    };
                    break;
                case "Sagre/feste":
                    topic = new TopicLinked
                    {
                        TopicRID = "9C3449EE278C4D94AA5A7C286729DEA0",
                        TopicInfo = "Volksfeste/Festivals"
                    };
                    break;
                case "Gite/escursioni":
                    topic = new TopicLinked
                    {
                        TopicRID = "ACE8B613F2074A7BB59C0B1DD40A43CD",
                        TopicInfo = "Wanderungen/Ausflüge"
                    };
                    break;
                case "Visite guidate":
                    topic = new TopicLinked
                    {
                        TopicRID = "B5467FEFE5C74FA5AD32B83793A76165",
                        TopicInfo = "Führungen/Besichtigungen"
                    };
                    break;
                case "Mostre/arte":
                    topic = new TopicLinked
                    {
                        TopicRID = "C72CE969B98947FABC99CBC7B033F28E",
                        TopicInfo = "Ausstellungen/Kunst"
                    };
                    break;
                case "Famiglia":
                    topic = new TopicLinked
                    {
                        TopicRID = "D98B49DF24C342D09A8161836435CF86",
                        TopicInfo = "Familie"
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
            if (id != "------" && place != null)
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

                //Try to convert price to double
                if (Double.TryParse(ninjaevent.price, out var pricedouble))
                {
                    if (pricedouble > 0)
                    {
                        foreach (var language in languages)
                        {
                            EventPrice myeventprice = new EventPrice();
                            myeventprice.Language = language;
                            myeventprice.Price = pricedouble;
                            myeventprice.Type = ninjaevent.event_type_key;

                            myevent.EventPrice.TryAddOrUpdate(language, myeventprice);
                        }
                    }
                }


                //Add Type info
                myevent.Topics = GetTopicRid(ninjaevent.event_type_key);
                myevent.TopicRIDs = myevent.Topics.Select(x => x.TopicRID).ToList();

                //Console.WriteLine("Parsing: " + ninjaevent.begin_date + " " + ninjaevent.begin_time);

                //TODO PARSING FAILS IF format of datetime is not exactly as described
                //Date Info
                myevent.DateBegin = DateTime.ParseExact(ninjaevent.begin_date + " " + ninjaevent.begin_time, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                myevent.DateEnd = DateTime.ParseExact(ninjaevent.end_date + " " + ninjaevent.end_time, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                //DateTime.TryParse(ninjaevent.begin_date + " " + ninjaevent.begin_time, CultureInfo.InvariantCulture, out evendatebegin);
                //DateTime.TryParse(ninjaevent.end_date + " " + ninjaevent.end_time, CultureInfo.InvariantCulture, out evendateend);

                //CultureInfo myculture = new CultureInfo("en-GB");
                //string begindate = ninjaevent.begin_date + " " + ninjaevent.begin_time + ":00";
                //string enddate = ninjaevent.end_date + " " + ninjaevent.end_time + ":00";
                //myevent.DateBegin = Convert.ToDateTime(begindate, myculture);
                //myevent.DateEnd = Convert.ToDateTime(enddate, myculture);

                myevent.NextBeginDate = myevent.DateBegin;

                myevent.EventDate = new List<EventDate>()
            {
                new EventDate()
                {
                    Begin = TimeSpan.Parse(ninjaevent.begin_time),
                    From = DateTime.ParseExact(ninjaevent.begin_date, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    End = TimeSpan.Parse(ninjaevent.end_time),
                    To = DateTime.ParseExact(ninjaevent.end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Ticket = ticket,
                    MaxPersons = !String.IsNullOrEmpty(ninjaevent.number_of_seats) && int.TryParse(ninjaevent.number_of_seats, out var numberofseatsint) ? numberofseatsint : 0
                }
            };

                myevent.Ticket = ticketstr;
                myevent.PayMet = paymet;

                myevent.Shortname = myevent.Detail.FirstOrDefault().Value.Title;
                myevent.LastChange = DateTime.Now;
                myevent._Meta.LastUpdate = myevent.LastChange;

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
            else
                return null;
        }
    }
}
