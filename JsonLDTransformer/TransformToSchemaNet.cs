using JsonLDTransformer.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schema.NET;
using System.Xml;
using HtmlAgilityPack;
using DataModel;
using Newtonsoft.Json;

namespace JsonLDTransformer
{
    public class TransformToSchemaNet
    {
        public static List<object> TransformDataToSchemaNet<T>(T data, string currentroute, string type,  string language, object parentobject = null, string idtoshow = "", string urltoshow = "", string imageurltoshow = "", bool showid = true)
        {
            var objectlist = new List<object>();

            switch (type)
            {
                case "accommodation":
                    objectlist.Add(TransformAccommodationToLD((DataModel.Accommodation)(object)data, currentroute, language, idtoshow, urltoshow, imageurltoshow, showid));
                    break;
                case "gastronomy":
                    objectlist.Add(TransformGastronomyToLD((DataModel.ODHActivityPoi)(object)data, currentroute, language, idtoshow, urltoshow, imageurltoshow, showid));
                    break;
                case "poi":
                    objectlist.Add(TransformActivityPoiToLD((DataModel.ODHActivityPoi)(object)data, currentroute, language, idtoshow, urltoshow, imageurltoshow, showid));
                    break;
                case "skiarea":
                    objectlist.Add(TransformSkiResortToLD((DataModel.SkiArea)(object)data, (DataModel.SkiRegion)(object)parentobject, currentroute, language, idtoshow, urltoshow, imageurltoshow, showid));
                    break;
                case "region":
                    objectlist.Add(TransformPlaceToLD((DataModel.Region)(object)data, currentroute, language, idtoshow, urltoshow, imageurltoshow, showid));
                    break;
                case "tv":
                    objectlist.Add(TransformPlaceToLD((DataModel.Tourismverein)(object)data, currentroute, language, idtoshow, urltoshow, imageurltoshow, showid));
                    break;
                case "municipality":
                    objectlist.Add(TransformPlaceToLD((DataModel.Municipality)(object)data, currentroute, language, idtoshow, urltoshow, imageurltoshow, showid));
                    break;
                case "district":
                    objectlist.Add(TransformPlaceToLD((DataModel.District)(object)data, currentroute, language, idtoshow, urltoshow, imageurltoshow, showid));
                    break;
                case "recipe":
                    objectlist.Add(TransformRecipeToLD((DataModel.Article)(object)data, currentroute, language, idtoshow, urltoshow, imageurltoshow, showid));
                    break;
                case "specialannouncement":
                    objectlist.Add(TransformSpecialAnnouncementToLD((DataModel.Article)(object)data, currentroute, language, idtoshow, urltoshow, imageurltoshow, showid));
                    break;

                case "event":

                    //Achtung pro EventDate Eintrag 1 Event anlegen
                    //des hoasst i muass a listen zruggeben

                    objectlist.AddRange(TransformEventToLD((DataModel.Event)(object)data, currentroute, language, idtoshow, urltoshow, imageurltoshow, showid));
                    break;
            }


            return objectlist;
        }

        #region Accommodation

        private static Schema.NET.Hotel TransformAccommodationToLD(DataModel.Accommodation acco, string currentroute, string language, string passedid, string passedurl, string passedimage, bool showid)
        {
            string fallbacklanguage = "en";

            Schema.NET.Hotel myhotel = new Schema.NET.Hotel();

            if (showid)
            {
                if (String.IsNullOrEmpty(passedid))
                    myhotel.Id = new Uri(currentroute);
                else
                    myhotel.Id = new Uri(passedid);
            }

            myhotel.Description = acco.AccoDetail.ContainsKey(language) ? acco.AccoDetail[language].Shortdesc : acco.AccoDetail.ContainsKey(fallbacklanguage) ? acco.AccoDetail[fallbacklanguage].Shortdesc : "";
            myhotel.Email = acco.AccoDetail.ContainsKey(language) ? acco.AccoDetail[language].Email : acco.AccoDetail.ContainsKey(fallbacklanguage) ? acco.AccoDetail[fallbacklanguage].Email : "";
            myhotel.Name = acco.AccoDetail.ContainsKey(language) ? acco.AccoDetail[language].Name : acco.AccoDetail.ContainsKey(fallbacklanguage) ? acco.AccoDetail[fallbacklanguage].Name : "";
            myhotel.Telephone = acco.AccoDetail.ContainsKey(language) ? acco.AccoDetail[language].Phone : acco.AccoDetail.ContainsKey(fallbacklanguage) ? acco.AccoDetail[fallbacklanguage].Phone : "";


            if (String.IsNullOrEmpty(passedimage))
            {
                if (acco.ImageGallery != null)
                    if (acco.ImageGallery.Count > 0)
                        if (!String.IsNullOrEmpty(acco.ImageGallery.FirstOrDefault().ImageUrl))
                            myhotel.Image = new Uri(acco.ImageGallery.FirstOrDefault().ImageUrl + "&W=800");
            }
            else
                myhotel.Image = new Uri(passedimage);



            //URL OVERWRITE
            if (String.IsNullOrEmpty(passedurl) && String.IsNullOrEmpty(passedid))
            {
                string url = acco.AccoDetail.ContainsKey(language) ? acco.AccoDetail[language].Website : acco.AccoDetail.ContainsKey(fallbacklanguage) ? acco.AccoDetail[fallbacklanguage].Website : "";
                if (CheckURLValid(url))
                    myhotel.Url = new Uri(url);

            }
            else if (!String.IsNullOrEmpty(passedurl))
            {
                myhotel.Url = new Uri(passedurl);
            }
            else if (!String.IsNullOrEmpty(passedid))
            {
                myhotel.Url = new Uri(passedid);
            }


            PostalAddress myaddress = new PostalAddress();
            //myaddress.Type = "http://schema.org/PostalAddress";
            myaddress.StreetAddress = acco.AccoDetail.ContainsKey(language) ? acco.AccoDetail[language].Street : acco.AccoDetail.ContainsKey(fallbacklanguage) ? acco.AccoDetail[fallbacklanguage].Street : "";
            myaddress.PostalCode = acco.AccoDetail.ContainsKey(language) ? acco.AccoDetail[language].Zip : acco.AccoDetail.ContainsKey(fallbacklanguage) ? acco.AccoDetail[fallbacklanguage].Zip : "";
            myaddress.AddressLocality = acco.AccoDetail.ContainsKey(language) ? acco.AccoDetail[language].City : acco.AccoDetail.ContainsKey(fallbacklanguage) ? acco.AccoDetail[fallbacklanguage].City : "";
            myaddress.AddressRegion = getRegionDependingonLanguage(language);
            myaddress.AddressCountry = getCountryDependingonLanguage(language);
            myaddress.Telephone = acco.AccoDetail.ContainsKey(language) ? acco.AccoDetail[language].Phone : acco.AccoDetail.ContainsKey(fallbacklanguage) ? acco.AccoDetail[fallbacklanguage].Phone : "";

            string adressurl = acco.AccoDetail.ContainsKey(language) ? acco.AccoDetail[language].Website : acco.AccoDetail.ContainsKey(fallbacklanguage) ? acco.AccoDetail[fallbacklanguage].Website : "";
            if (CheckURLValid(adressurl))
                myhotel.Url = new Uri(adressurl);

            myaddress.Email = acco.AccoDetail.ContainsKey(language) ? acco.AccoDetail[language].Email : acco.AccoDetail.ContainsKey(fallbacklanguage) ? acco.AccoDetail[fallbacklanguage].Email : "";
            myaddress.FaxNumber = acco.AccoDetail.ContainsKey(language) ? acco.AccoDetail[language].Fax : acco.AccoDetail.ContainsKey(fallbacklanguage) ? acco.AccoDetail[fallbacklanguage].Fax : "";
            myaddress.AlternateName = acco.AccoDetail.ContainsKey(language) ? acco.AccoDetail[language].NameAddition : acco.AccoDetail.ContainsKey(fallbacklanguage) ? acco.AccoDetail[fallbacklanguage].NameAddition : "";
            myaddress.Name = acco.AccoDetail.ContainsKey(language) ? acco.AccoDetail[language].Name : acco.AccoDetail.ContainsKey(fallbacklanguage) ? acco.AccoDetail[fallbacklanguage].Name : "";

            myhotel.Address = myaddress;

            GeoCoordinates mygeo = new GeoCoordinates();
            //mygeo.type = "http://schema.org/GeoCoordinates";
            mygeo.Latitude = acco.Latitude;
            mygeo.Longitude = acco.Longitude;

            myhotel.Geo = mygeo;

            //New Star Rating:
            //An official rating for a lodging business or food establishment, e.g.from national associations or standards bodies.Use the author property to indicate the rating organization, e.g. as an Organization with name such as (e.g.HOTREC, DEHOGA, WHR, or Hotelstars).
            //Currently disabled because it's a rating by an official agency..... not the category of the hotel
            var starratingvalue = GetStarRating(acco.AccoCategoryId);
            Rating starrating = new Rating();
            starrating.RatingValue = starratingvalue;
            myhotel.StarRating = starrating;

            //New Price Range
            var pricerange = GetPriceRange(acco.AccoCategoryId);
            myhotel.PriceRange = pricerange;

            //New Trust You infos Display only if State = 2 and Active = true
            if (acco.TrustYouActive)
            {
                if (acco.TrustYouState == 2)
                {
                    AggregateRating aggregaterating = new AggregateRating();
                    aggregaterating.RatingValue = acco.TrustYouScore / 10;
                    aggregaterating.ReviewCount = acco.TrustYouResults;
                    aggregaterating.BestRating = 100;

                    Organization trustyou = new Organization();
                    trustyou.Name = "TrustYou";
                    trustyou.Url = new Uri("https://www.trustyou.com/");

                    aggregaterating.Author = trustyou;

                    myhotel.AggregateRating = aggregaterating;
                }
            }


            return myhotel;
        }

        private static double GetStarRating(string categoryid)
        {
            if (categoryid == "1star" || categoryid == "1flower" || categoryid == "1sun")
                return 1;
            else if (categoryid == "2stars" || categoryid == "2flowers" || categoryid == "2suns")
                return 2;
            else if (categoryid == "3stars" || categoryid == "3flowers" || categoryid == "3suns")
                return 3;
            else if (categoryid == "3sstars")
                return 3.5;
            else if (categoryid == "4stars" || categoryid == "4flowers" || categoryid == "4suns")
                return 4;
            else if (categoryid == "4sstars")
                return 4.5;
            else if (categoryid == "5stars" || categoryid == "5flowers" || categoryid == "5suns")
                return 5;
            else
                return 0;
        }

        private static string GetPriceRange(string categoryid)
        {
            string comparator = "";

            if (categoryid == "1star" || categoryid == "1flower" || categoryid == "1sun" || categoryid == "2stars" || categoryid == "2flowers" || categoryid == "2suns")
                comparator = "1";
            else if (categoryid == "3stars" || categoryid == "3flowers" || categoryid == "3suns" || categoryid == "3sstars")
                comparator = "2";
            else if (categoryid == "4stars" || categoryid == "4flowers" || categoryid == "4suns")
                comparator = "3";
            else if (categoryid == "4sstars" || categoryid == "5stars" || categoryid == "5flowers" || categoryid == "5suns")
                comparator = "4";

            switch (comparator)
            {
                case "1":
                    return "€";
                case "2":
                    return "€€";
                case "3":
                    return "€€€";
                case "4":
                    return "€€€€";
                default:
                    return "€€";
            }
        }

        #endregion

        #region Gastronomy

        private static Schema.NET.Restaurant TransformGastronomyToLD(DataModel.ODHActivityPoi gastro, string currentroute, string language, string passedid, string passedurl, string passedimage, bool showid)
        {
            Schema.NET.Restaurant mygastro = new Schema.NET.Restaurant();

            string fallbacklanguage = "en";

            if (showid)
            {
                if (String.IsNullOrEmpty(passedid))
                    mygastro.Id = new Uri(currentroute);
                else
                    mygastro.Id = new Uri(passedid);
            }
            //mygastro.type = "Restaurant";


            mygastro.Description = gastro.Detail.ContainsKey(language) ? gastro.Detail[language].BaseText : gastro.Detail.ContainsKey(fallbacklanguage) ? gastro.Detail[fallbacklanguage].BaseText : "";
            mygastro.Name = gastro.Detail.ContainsKey(language) ? gastro.Detail[language].Title : gastro.Detail.ContainsKey(fallbacklanguage) ? gastro.Detail[fallbacklanguage].Title : "";

            mygastro.Email = gastro.ContactInfos.ContainsKey(language) ? gastro.ContactInfos[language].Email : gastro.ContactInfos.ContainsKey(fallbacklanguage) ? gastro.ContactInfos[fallbacklanguage].Email : "";
            mygastro.Telephone = gastro.ContactInfos.ContainsKey(language) ? gastro.ContactInfos[language].Phonenumber : gastro.ContactInfos.ContainsKey(fallbacklanguage) ? gastro.ContactInfos[fallbacklanguage].Phonenumber : "";


            if (String.IsNullOrEmpty(passedimage))
            {
                if (gastro.ImageGallery != null)
                    if (gastro.ImageGallery.Count > 0)
                        if (!String.IsNullOrEmpty(gastro.ImageGallery.FirstOrDefault().ImageUrl))
                            mygastro.Image = new Uri(gastro.ImageGallery.FirstOrDefault().ImageUrl);
            }
            else
                mygastro.Image = new Uri(passedimage);


            //URL OVERWRITE
            if (String.IsNullOrEmpty(passedurl) && String.IsNullOrEmpty(passedid))
            {
                string url = gastro.ContactInfos.ContainsKey(language) ? gastro.ContactInfos[language].Url : gastro.ContactInfos.ContainsKey(fallbacklanguage) ? gastro.ContactInfos[fallbacklanguage].Url : "";
                if (CheckURLValid(url))
                    mygastro.Url = new Uri(url);
            }
            else if (!String.IsNullOrEmpty(passedurl))
            {
                mygastro.Url = new Uri(passedurl);
            }
            else if (!String.IsNullOrEmpty(passedid))
            {
                mygastro.Url = new Uri(passedid);
            }




            PostalAddress myaddress = new PostalAddress();
            //myaddress.type = "http://schema.org/PostalAddress";
            myaddress.StreetAddress = gastro.ContactInfos.ContainsKey(language) ? gastro.ContactInfos[language].Address : gastro.ContactInfos.ContainsKey(fallbacklanguage) ? gastro.ContactInfos[fallbacklanguage].Address : "";
            myaddress.PostalCode = gastro.ContactInfos.ContainsKey(language) ? gastro.ContactInfos[language].ZipCode : gastro.ContactInfos.ContainsKey(fallbacklanguage) ? gastro.ContactInfos[fallbacklanguage].ZipCode : "";
            myaddress.AddressLocality = gastro.ContactInfos.ContainsKey(language) ? gastro.ContactInfos[language].City : gastro.ContactInfos.ContainsKey(fallbacklanguage) ? gastro.ContactInfos[fallbacklanguage].City : "";
            myaddress.AddressRegion = getRegionDependingonLanguage(language);
            myaddress.AddressCountry = gastro.ContactInfos.ContainsKey(language) ? gastro.ContactInfos[language].CountryName : gastro.ContactInfos.ContainsKey(fallbacklanguage) ? gastro.ContactInfos[fallbacklanguage].CountryName : "";

            myaddress.Telephone = gastro.ContactInfos.ContainsKey(language) ? gastro.ContactInfos[language].Phonenumber : gastro.ContactInfos.ContainsKey(fallbacklanguage) ? gastro.ContactInfos[fallbacklanguage].Phonenumber : "";

            string adressurl = gastro.ContactInfos.ContainsKey(language) ? gastro.ContactInfos[language].Url : gastro.ContactInfos.ContainsKey(fallbacklanguage) ? gastro.ContactInfos[fallbacklanguage].Url : "";
            if (CheckURLValid(adressurl))
                myaddress.Url = new Uri(adressurl);

            myaddress.Email = gastro.ContactInfos.ContainsKey(language) ? gastro.ContactInfos[language].Email : gastro.ContactInfos.ContainsKey(fallbacklanguage) ? gastro.ContactInfos[fallbacklanguage].Email : "";
            myaddress.FaxNumber = gastro.ContactInfos.ContainsKey(language) ? gastro.ContactInfos[language].Faxnumber : gastro.ContactInfos.ContainsKey(fallbacklanguage) ? gastro.ContactInfos[fallbacklanguage].Faxnumber : "";
            myaddress.AlternateName = gastro.ContactInfos.ContainsKey(language) ? gastro.ContactInfos[language].CompanyName : gastro.ContactInfos.ContainsKey(fallbacklanguage) ? gastro.ContactInfos[fallbacklanguage].CompanyName : "";
            myaddress.Name = gastro.Detail.ContainsKey(language) ? gastro.Detail[language].Title : gastro.Detail.ContainsKey(fallbacklanguage) ? gastro.Detail[fallbacklanguage].Title : "";

            mygastro.Address = myaddress;

            GeoCoordinates mygeo = new GeoCoordinates();
            //mygeo.type = "http://schema.org/GeoCoordinates";

            mygeo.Latitude = gastro.GpsInfo != null ? gastro.GpsInfo.FirstOrDefault().Latitude : 0;
            mygeo.Longitude = gastro.GpsInfo != null ? gastro.GpsInfo.FirstOrDefault().Longitude : 0;

            mygastro.Geo = mygeo;

            Person founder = new Person();
            //founder.type = "http://schema.org/Person";
            founder.Name = gastro.ContactInfos.ContainsKey(language) ? gastro.ContactInfos[language].Givenname + " " + gastro.ContactInfos[language].Surname : gastro.ContactInfos.ContainsKey(fallbacklanguage) ? gastro.ContactInfos[fallbacklanguage].Givenname + " " + gastro.ContactInfos[fallbacklanguage].Surname : "";

            mygastro.Founder = founder;

            mygastro.ServesCuisine = GetCuisine(gastro.Facilities);
            mygastro.OpeningHours = GetOpeningDatesGastronomy(gastro.OperationSchedule);

            return mygastro;
        }

        private static List<string> GetCuisine(ICollection<Facilities> facilities)
        {
            var validcuisine = ValidCuisineCodes();

            if (facilities != null)
                return facilities.Where(x => validcuisine.Contains(x.Id)).Select(x => x.Shortname).ToList();
            else
                return new List<string>();
        }

        private static List<string> ValidCuisineCodes()
        {
            return new List<string>()
            {
                "3091F5B92F534F67986C08151E6F4454",
                "71A7D4A821F7437EA1DC05CEE9655A5A",
                "11A6BEA7EEFC4716BDF8FBD5E15C0CFB",
                "A469B187953944A0AF49C5EBE13DCF00",
                "F42DBD202D6E4289AF48D138DA09ECB7",
                "2476B5BBAEB7467C9A0099F06D0ED004",
                "30DC854F943D42CF8DB140CF4A90EC7E",
                "D1F124A123554B14AB9600F2313ED051",
                "CB8AF7CB80E844758B18E9C4E2D84035",
                "50FFF83EB75944DE9F6F15CC51E85E7A",
                "6322DE8AFE8E406F886E7C40D0DC1ADD",
                "0E9721E540FB4D84BADC0DFA24F0543B",
                "C48E7E7679B04835B6744650E129BABF",
                "167850CF26984D50A59A5F42EB24A0AD",
                "4F9335FDAB834B11B36CD4C163F990A7",
                "69621AE51DF942A1BBED32D460E65132",
                "22F0D9C42B06423EB63E1F2F27B7CA3A",
                "BC08B00995564BB28997C55C870120D1",
                "0E55D7C2A7BC4866BF8438C522C17254",
                "FC627623C6994E37927F6048E32B79C2",
                "21A903DE35654070803DFDDF29C67291",
                "8E28215F82BA430EA016BA5D1C776A30",
                "D413EF912D18462CA0055A44F55351D1",
                "BC6B57D90AFB496098DD0D059D04EE7C",
                "B36D855D60CB4D79BA78F3FEFEE9F9D3",
                "AD8426538FCF4D8A81E06BE044088BAA",
                "5C84265DA5F84F84A7896808ACCB675A"
            };
        }

        private static List<string> GetOpeningDatesGastronomy(ICollection<OperationSchedule> operationschedules)
        {
            List<string> openingtime = new List<string>();
            //openingtime.Add("Mo-Sa 11:00-14:30");
            //openingtime.Add("Mo-Th 17:00-21:30");

            if (operationschedules != null)
            {
                foreach (var operationschedule in operationschedules.Where(x => x.Start <= DateTime.Now && x.Stop >= DateTime.Now))
                {
                    //Zersch schaugn obs ollaweil offen isch!



                    //Wenn Zeiten drinn

                    //Achtung do muassi die richtigen fir heint finden sischt sein mearere!

                    if (operationschedule.OperationScheduleTime != null)
                    {
                        foreach (var operationtime in operationschedule.OperationScheduleTime.Where(x => x.State == 2 && x.Timecode == 1))
                        {
                            if (operationtime.Monday)
                                openingtime.Add("Mo " + operationtime.Start + " - " + operationtime.End);
                            if (operationtime.Tuesday)
                                openingtime.Add("Tu " + operationtime.Start + " - " + operationtime.End);
                            if (operationtime.Wednesday)
                                openingtime.Add("We " + operationtime.Start + " - " + operationtime.End);
                            if (operationtime.Thuresday)
                                openingtime.Add("Th " + operationtime.Start + " - " + operationtime.End);
                            if (operationtime.Friday)
                                openingtime.Add("Fr " + operationtime.Start + " - " + operationtime.End);
                            if (operationtime.Saturday)
                                openingtime.Add("Sa " + operationtime.Start + " - " + operationtime.End);
                            if (operationtime.Sunday)
                                openingtime.Add("Su " + operationtime.Start + " - " + operationtime.End);
                        }

                    }

                    //Keine Zeiten drinn
                }
            }

            return openingtime;
        }

        #endregion

        #region Event

        private static List<Schema.NET.Event> TransformEventToLD(DataModel.Event theevent, string currentroute, string language, string passedid, string passedurl, string passedimage, bool showid)
        {
            string fallbacklanguage = "en";

            List<Schema.NET.Event> myeventlist = new List<Schema.NET.Event>();

            foreach (var theeventsingle in theevent.EventDate)
            {
                Schema.NET.Event myevent = new Schema.NET.Event();

                //myevent.context = "http://schema.org";
                if (showid)
                {
                    if (String.IsNullOrEmpty(passedid))
                        myevent.Id = new Uri(currentroute);
                    else
                        myevent.Id = new Uri(passedid);
                    //myevent.type  = "Event";
                }

                myevent.Description = theevent.Detail.ContainsKey(language) ? theevent.Detail[language].BaseText : theevent.Detail.ContainsKey(fallbacklanguage) ? theevent.Detail[fallbacklanguage].BaseText : "";
                myevent.Name = theevent.Detail.ContainsKey(language) ? theevent.Detail[language].Title : theevent.Detail.ContainsKey(fallbacklanguage) ? theevent.Detail[fallbacklanguage].Title : "";


                if (String.IsNullOrEmpty(passedimage))
                {
                    if (theevent.ImageGallery != null)
                        if (theevent.ImageGallery.Count > 0)
                            if (!String.IsNullOrEmpty(theevent.ImageGallery.FirstOrDefault().ImageUrl))
                                myevent.Image = new Uri(theevent.ImageGallery.FirstOrDefault().ImageUrl + "&W=800");
                }
                else
                    myevent.Image = new Uri(passedimage);


                //URL OVERWRITE
                if (String.IsNullOrEmpty(passedurl) && String.IsNullOrEmpty(passedid))
                {
                    string url = theevent.ContactInfos.ContainsKey(language) ? theevent.ContactInfos[language].Url : theevent.ContactInfos.ContainsKey(fallbacklanguage) ? theevent.ContactInfos[fallbacklanguage].Url : "";
                    if (CheckURLValid(url))
                        myevent.Url = new Uri(url);
                }
                else if (!String.IsNullOrEmpty(passedurl))
                {
                    myevent.Url = new Uri(passedurl);
                }
                else if (!String.IsNullOrEmpty(passedid))
                {
                    myevent.Url = new Uri(passedid);
                }



                //myevent.email = theevent.ContactInfos[language].Email;
                //myevent.telephone = theevent.ContactInfos[language].Phonenumber;

                PostalAddress myaddress = new PostalAddress();
                //myaddress.type = "http://schema.org/PostalAddress";
                myaddress.StreetAddress = theevent.ContactInfos.ContainsKey(language) ? theevent.ContactInfos[language].Address : theevent.ContactInfos.ContainsKey(fallbacklanguage) ? theevent.ContactInfos[fallbacklanguage].Address : "";
                myaddress.PostalCode = theevent.ContactInfos.ContainsKey(language) ? theevent.ContactInfos[language].ZipCode : theevent.ContactInfos.ContainsKey(fallbacklanguage) ? theevent.ContactInfos[fallbacklanguage].ZipCode : "";
                myaddress.AddressLocality = theevent.ContactInfos.ContainsKey(language) ? theevent.ContactInfos[language].City : theevent.ContactInfos.ContainsKey(fallbacklanguage) ? theevent.ContactInfos[fallbacklanguage].City : "";
                myaddress.AddressRegion = getRegionDependingonLanguage(language);
                myaddress.AddressCountry = theevent.ContactInfos.ContainsKey(language) ? theevent.ContactInfos[language].CountryName : theevent.ContactInfos.ContainsKey(fallbacklanguage) ? theevent.ContactInfos[fallbacklanguage].CountryName : "";

                Place location = new Place();
                location.Address = myaddress;
                //location.type = "Place";
                location.Name = theevent.ContactInfos.ContainsKey(language) ? theevent.ContactInfos[language].CompanyName : theevent.ContactInfos.ContainsKey(fallbacklanguage) ? theevent.ContactInfos[fallbacklanguage].CompanyName : "";
                myevent.Location = location;

                Organization organization = new Organization();

                organization.Name = theevent.OrganizerInfos.ContainsKey(language) ? theevent.OrganizerInfos[language].CompanyName : theevent.OrganizerInfos.ContainsKey(fallbacklanguage) ? theevent.OrganizerInfos[fallbacklanguage].CompanyName : "";


                string adressurl = theevent.OrganizerInfos.ContainsKey(language) ? theevent.OrganizerInfos[language].Url : theevent.OrganizerInfos.ContainsKey(fallbacklanguage) ? theevent.OrganizerInfos[fallbacklanguage].Url : "";
                if (CheckURLValid(adressurl))
                    organization.Url = new Uri(adressurl);


                organization.Email = theevent.OrganizerInfos.ContainsKey(language) ? theevent.OrganizerInfos[language].Email : theevent.OrganizerInfos.ContainsKey(fallbacklanguage) ? theevent.OrganizerInfos[fallbacklanguage].Email : "";
                organization.Telephone = theevent.OrganizerInfos.ContainsKey(language) ? theevent.OrganizerInfos[language].Phonenumber : theevent.OrganizerInfos.ContainsKey(fallbacklanguage) ? theevent.OrganizerInfos[fallbacklanguage].Phonenumber : "";

                PostalAddress organizeraddress = new PostalAddress();
                //organizeraddress.type = "http://schema.org/PostalAddress";
                organizeraddress.StreetAddress = theevent.OrganizerInfos.ContainsKey(language) ? theevent.OrganizerInfos[language].Address : theevent.OrganizerInfos.ContainsKey(fallbacklanguage) ? theevent.OrganizerInfos[fallbacklanguage].Address : "";
                organizeraddress.PostalCode = theevent.OrganizerInfos.ContainsKey(language) ? theevent.OrganizerInfos[language].ZipCode : theevent.OrganizerInfos.ContainsKey(fallbacklanguage) ? theevent.OrganizerInfos[fallbacklanguage].ZipCode : "";
                organizeraddress.AddressLocality = theevent.OrganizerInfos.ContainsKey(language) ? theevent.OrganizerInfos[language].City : theevent.OrganizerInfos.ContainsKey(fallbacklanguage) ? theevent.OrganizerInfos[fallbacklanguage].City : "";
                organizeraddress.AddressRegion = getRegionDependingonLanguage(language);
                organizeraddress.AddressCountry = theevent.OrganizerInfos.ContainsKey(language) ? theevent.OrganizerInfos[language].CountryName : theevent.OrganizerInfos.ContainsKey(fallbacklanguage) ? theevent.OrganizerInfos[fallbacklanguage].CountryName : "";
                organization.Address = organizeraddress;
                myevent.Organizer = organization;

                //OFfers

                if (theevent.EventPrice != null)
                {
                    if (theevent.EventPrice.ContainsKey(language))
                    {
                        Offer myoffer = new Offer();
                        //myoffer.type = "http://schema.org/Offer";

                        myoffer.Price = theevent.EventPrice[language].Price.ToString();
                        myoffer.PriceCurrency = "€";
                        myoffer.Name = theevent.EventPrice[language].ShortDesc;
                        myoffer.Description = theevent.EventPrice[language].Description;

                        myoffer.ValidFrom = new DateTimeOffset(Convert.ToDateTime(String.Format("{0:yyyy-MM-dd}", theeventsingle.From) + "T" + theeventsingle.Begin.Value.ToString("hh\\:mm")));
                        myoffer.ValidThrough = new DateTimeOffset(Convert.ToDateTime(String.Format("{0:yyyy-MM-dd}", theeventsingle.To) + "T" + theeventsingle.End.Value.ToString("hh\\:mm")));

                        myevent.Offers = myoffer;
                    }
                    else if (theevent.EventPrice.ContainsKey(fallbacklanguage))
                    {
                        Offer myoffer = new Offer();
                        //myoffer.type = "http://schema.org/Offer";

                        myoffer.Price = theevent.EventPrice[fallbacklanguage].Price.ToString();
                        myoffer.PriceCurrency = "€";
                        myoffer.Name = theevent.EventPrice[fallbacklanguage].ShortDesc;
                        myoffer.Description = theevent.EventPrice[fallbacklanguage].Description;

                        myoffer.ValidFrom = new DateTimeOffset(Convert.ToDateTime(String.Format("{0:yyyy-MM-dd}", theeventsingle.From) + "T" + theeventsingle.Begin.Value.ToString("hh\\:mm")));
                        myoffer.ValidThrough = new DateTimeOffset(Convert.ToDateTime(String.Format("{0:yyyy-MM-dd}", theeventsingle.To) + "T" + theeventsingle.End.Value.ToString("hh\\:mm")));

                        myevent.Offers = myoffer;
                    }
                }

                myevent.StartDate = new DateTimeOffset(Convert.ToDateTime(String.Format("{0:yyyy-MM-dd}", theeventsingle.From) + "T" + theeventsingle.Begin.Value.ToString("hh\\:mm")));
                myevent.EndDate = new DateTimeOffset(Convert.ToDateTime(String.Format("{0:yyyy-MM-dd}", theeventsingle.To) + "T" + theeventsingle.End.Value.ToString("hh\\:mm")));

                myeventlist.Add(myevent);
            }


            return myeventlist;
        }

        #endregion

        #region Tourismattraction

        private static Schema.NET.TouristAttraction TransformActivityPoiToLD(DataModel.ODHActivityPoi poi, string currentroute, string language, string passedid, string passedurl, string passedimage, bool showid)
        {
            string fallbacklanguage = "en";

            Schema.NET.TouristAttraction mypoi = new Schema.NET.TouristAttraction();

            if (showid)
            {
                if (String.IsNullOrEmpty(passedid))
                    mypoi.Id = new Uri(currentroute);
                else
                    mypoi.Id = new Uri(passedid);
            }

            mypoi.Description = poi.Detail.ContainsKey(language) ? poi.Detail[language].BaseText : poi.Detail.ContainsKey(fallbacklanguage) ? poi.Detail[fallbacklanguage].BaseText : "";
            mypoi.Name = poi.Detail.ContainsKey(language) ? poi.Detail[language].Title : poi.Detail.ContainsKey(fallbacklanguage) ? poi.Detail[fallbacklanguage].Title : "";

            mypoi.FaxNumber = poi.ContactInfos.ContainsKey(language) ? poi.ContactInfos[language].Faxnumber : poi.ContactInfos.ContainsKey(fallbacklanguage) ? poi.ContactInfos[fallbacklanguage].Faxnumber : "";
            mypoi.Telephone = poi.ContactInfos.ContainsKey(language) ? poi.ContactInfos[language].Phonenumber : poi.ContactInfos.ContainsKey(fallbacklanguage) ? poi.ContactInfos[fallbacklanguage].Phonenumber : "";



            if (String.IsNullOrEmpty(passedimage))
            {
                if (poi.ImageGallery != null)
                    if (poi.ImageGallery.Count > 0)
                        if (!String.IsNullOrEmpty(poi.ImageGallery.FirstOrDefault().ImageUrl))
                            mypoi.Image = new Uri(poi.ImageGallery.FirstOrDefault().ImageUrl);
            }
            else
                mypoi.Image = new Uri(passedimage);





            //URL OVERWRITE
            if (String.IsNullOrEmpty(passedurl) && String.IsNullOrEmpty(passedid))
            {
                string url = poi.ContactInfos.ContainsKey(language) ? poi.ContactInfos[language].Url : poi.ContactInfos.ContainsKey(fallbacklanguage) ? poi.ContactInfos[fallbacklanguage].Url : "";
                if (CheckURLValid(url))
                    mypoi.Url = new Uri(url);
            }
            else if (!String.IsNullOrEmpty(passedurl))
            {
                mypoi.Url = new Uri(passedurl);
            }
            else if (!String.IsNullOrEmpty(passedid))
            {
                mypoi.Url = new Uri(passedid);
            }


            PostalAddress myaddress = new PostalAddress();
            //myaddress.Type = "http://schema.org/PostalAddress";
            myaddress.StreetAddress = poi.ContactInfos.ContainsKey(language) ? poi.ContactInfos[language].Address : poi.ContactInfos.ContainsKey(fallbacklanguage) ? poi.ContactInfos[fallbacklanguage].Address : "";
            myaddress.PostalCode = poi.ContactInfos.ContainsKey(language) ? poi.ContactInfos[language].ZipCode : poi.ContactInfos.ContainsKey(fallbacklanguage) ? poi.ContactInfos[fallbacklanguage].ZipCode : "";
            myaddress.AddressLocality = poi.ContactInfos.ContainsKey(language) ? poi.ContactInfos[language].City : poi.ContactInfos.ContainsKey(fallbacklanguage) ? poi.ContactInfos[fallbacklanguage].City : "";
            myaddress.AddressRegion = getRegionDependingonLanguage(language);
            myaddress.AddressCountry = poi.ContactInfos.ContainsKey(language) ? poi.ContactInfos[language].CountryName : poi.ContactInfos.ContainsKey(fallbacklanguage) ? poi.ContactInfos[fallbacklanguage].CountryName : "";
            myaddress.Telephone = poi.ContactInfos.ContainsKey(language) ? poi.ContactInfos[language].Phonenumber : poi.ContactInfos.ContainsKey(fallbacklanguage) ? poi.ContactInfos[fallbacklanguage].Phonenumber : "";

            string adressurl = poi.ContactInfos.ContainsKey(language) ? poi.ContactInfos[language].Url : poi.ContactInfos.ContainsKey(fallbacklanguage) ? poi.ContactInfos[fallbacklanguage].Url : "";
            if (CheckURLValid(adressurl))
                myaddress.Url = new Uri(adressurl);

            myaddress.Email = poi.ContactInfos.ContainsKey(language) ? poi.ContactInfos[language].Email : poi.ContactInfos.ContainsKey(fallbacklanguage) ? poi.ContactInfos[fallbacklanguage].Email : "";
            myaddress.FaxNumber = poi.ContactInfos.ContainsKey(language) ? poi.ContactInfos[language].Faxnumber : poi.ContactInfos.ContainsKey(fallbacklanguage) ? poi.ContactInfos[fallbacklanguage].Faxnumber : "";
            myaddress.AlternateName = poi.ContactInfos.ContainsKey(language) ? poi.ContactInfos[language].CompanyName : poi.ContactInfos.ContainsKey(fallbacklanguage) ? poi.ContactInfos[fallbacklanguage].CompanyName : "";
            myaddress.Name = poi.Detail.ContainsKey(language) ? poi.Detail[language].Title : poi.Detail.ContainsKey(fallbacklanguage) ? poi.Detail[fallbacklanguage].Title : "";


            mypoi.Address = myaddress;

            GeoCoordinates mygeo = new GeoCoordinates();
            //mygeo.type = "http://schema.org/GeoCoordinates";

            if (poi.GpsInfo != null && poi.GpsInfo.Count > 0)
            {
                mygeo.Latitude = poi.GpsInfo.FirstOrDefault().Latitude;
                mygeo.Longitude = poi.GpsInfo.FirstOrDefault().Longitude;

                mypoi.Geo = mygeo;
            }

            mypoi.AvailableLanguage = GetAvailableLanguages(poi.HasLanguage.ToList());

            return mypoi;
        }

        //I think here is meant the language of the Tourist Attraction itself, not in what languages the data is available
        private static Language GetAvailableLanguages(List<string> languagelist)
        {
            var languagetoreturn = new Language();

            List<string> languages = new List<string>(); ;

            foreach (var lang in languagelist)
            {
                if (lang == "de")
                    languages.Add("German");
                else if (lang == "it")
                    languages.Add("Italian");
                else if (lang == "en")
                    languages.Add("English");
                else if (lang == "nl")
                    languages.Add("Dutch");
                else if (lang == "cs")
                    languages.Add("Czech");
                else if (lang == "pl")
                    languages.Add("Polish");
                else if (lang == "fr")
                    languages.Add("French");
                else if (lang == "ru")
                    languages.Add("Russian");
            }

            languagetoreturn.Name = languages.ToArray();


            return languagetoreturn;
        }

        private static List<string> GetOpeningDatesActivityPoi(ICollection<OperationSchedule> operationschedules)
        {
            List<string> openingtime = new List<string>();

            if (operationschedules != null)
            {
                //Type 1 Normal
                //Type 2 Day and Month recurring
                //Type 3 year and month not to consider (season)

                foreach (var operationschedule in operationschedules.Where(x => x.Start <= DateTime.Now && x.Stop >= DateTime.Now && x.Type == "1"))
                {
                    if (operationschedule.OperationScheduleTime != null)
                    {
                        foreach (var operationtime in operationschedule.OperationScheduleTime)
                        {
                            if (operationtime.Monday && operationtime.Tuesday && operationtime.Wednesday && operationtime.Thuresday && operationtime.Friday && operationtime.Saturday && operationtime.Sunday)
                            {
                                openingtime.Add("Mo-Su " + operationtime.Start + " - " + operationtime.End);
                            }
                            else
                            {

                                if (operationtime.Monday)
                                    openingtime.Add("Mo " + operationtime.Start + " - " + operationtime.End);
                                if (operationtime.Tuesday)
                                    openingtime.Add("Tu " + operationtime.Start + " - " + operationtime.End);
                                if (operationtime.Wednesday)
                                    openingtime.Add("We " + operationtime.Start + " - " + operationtime.End);
                                if (operationtime.Thuresday)
                                    openingtime.Add("Th " + operationtime.Start + " - " + operationtime.End);
                                if (operationtime.Friday)
                                    openingtime.Add("Fr " + operationtime.Start + " - " + operationtime.End);
                                if (operationtime.Saturday)
                                    openingtime.Add("Sa " + operationtime.Start + " - " + operationtime.End);
                                if (operationtime.Sunday)
                                    openingtime.Add("Su " + operationtime.Start + " - " + operationtime.End);
                            }
                        }

                    }
                    //No opening time defined always open?
                    else
                    {
                        openingtime.Add("Mo-Su 00:00 - 23:59");
                    }
                }

                //Check Type 2 year not to consider
                foreach (var operationschedule in operationschedules.Where(x => x.Type == "2"))
                {
                    var now = default(DateTime);
                    var nowmonth = DateTime.Now.Month;
                    var nowday = DateTime.Now.Day;

                    //Hack for lap year
                    if (nowmonth == 2 && nowday == 29)
                        nowday = 28;

                    //If period goes over the year
                    if (operationschedule.Stop.Year > operationschedule.Start.Year)
                    {
                        if (nowmonth < operationschedule.Stop.Month)
                            now = new DateTime(operationschedule.Stop.Year, nowmonth, nowday);
                        else
                            now = new DateTime(operationschedule.Start.Year, nowmonth, nowday);
                    }
                    //else take the year of the period
                    else
                        now = new DateTime(operationschedule.Stop.Year, nowmonth, nowday);


                    //Case 1 normal inside: start 01-05-2018 end 01-10-2018 Now: 03-05-2020		--> 01-05-2018 <= 03-05-2018 && 01-10-2018 >= 03-05-2018 OK
                    //Case 1 normal outside: start 01-05-2019 end 01-10-2019 Now: 05-11-2020	--> 01-05-2018 <= 05-11-2018 && 01-10-2018 >= 05-11-2018 NOT OK

                    //Case 2 overyear inside: start 01-10-2018 end 01-02-2019 No: 01-12-2020	--> 01-10-2018 <= 01-12-2018 && 01-02-2019 >= 01-12-2018 OK
                    //Case 2 overyear inside: start 01-10-2018 end 01-02-2019 No: 01-01-2021	--> 01-10-2018 <= 01-01-2019 && 01-02-2019 >= 01-01-2019 OK
                    //Case 2 overyear outside: start 01-10-2018 end 01-02-2019 No: 01-05-2021	--> 01-10-2018 <= 01-05-2018 && 01-02-2019 >= 01-05-2018 NOT OK
                    //Case 2 overyear outside: start 01-10-2018 end 01-02-2019 No: 01-03-2021	--> 01-10-2018 <= 01-03-2018 && 01-02-2019 >= 01-03-2018 NOT OK

                    if (operationschedule.Start <= now && operationschedule.Stop >= now)
                    {
                        if (operationschedule.OperationScheduleTime != null)
                        {
                            foreach (var operationtime in operationschedule.OperationScheduleTime)
                            {
                                if (operationtime.Monday && operationtime.Tuesday && operationtime.Wednesday && operationtime.Thuresday && operationtime.Friday && operationtime.Saturday && operationtime.Sunday)
                                {
                                    openingtime.Add("Mo-Su " + operationtime.Start + " - " + operationtime.End);
                                }
                                else
                                {

                                    if (operationtime.Monday)
                                        openingtime.Add("Mo " + operationtime.Start + " - " + operationtime.End);
                                    if (operationtime.Tuesday)
                                        openingtime.Add("Tu " + operationtime.Start + " - " + operationtime.End);
                                    if (operationtime.Wednesday)
                                        openingtime.Add("We " + operationtime.Start + " - " + operationtime.End);
                                    if (operationtime.Thuresday)
                                        openingtime.Add("Th " + operationtime.Start + " - " + operationtime.End);
                                    if (operationtime.Friday)
                                        openingtime.Add("Fr " + operationtime.Start + " - " + operationtime.End);
                                    if (operationtime.Saturday)
                                        openingtime.Add("Sa " + operationtime.Start + " - " + operationtime.End);
                                    if (operationtime.Sunday)
                                        openingtime.Add("Su " + operationtime.Start + " - " + operationtime.End);
                                }
                            }

                        }

                        //No opening time defined always open?
                        else
                        {
                            openingtime.Add("Mo-Su 00:00 - 23:59");
                        }
                    }
                }
            }

            return openingtime;
        }


        #endregion

        #region Recipe

        private static Schema.NET.Recipe TransformRecipeToLD(DataModel.Article recipe, string currentroute, string language, string passedid, string passedurl, string passedimage, bool showid)
        {
            string fallbacklanguage = "en";

            //Check if data has this fallbacklanguage
            if (!recipe.HasLanguage.Contains(language))
                language = recipe.HasLanguage.FirstOrDefault();


            Schema.NET.Recipe myrecipe = new Schema.NET.Recipe();

            if (showid)
            {
                if (String.IsNullOrEmpty(passedid))
                    myrecipe.Id = new Uri(currentroute);
                else
                    myrecipe.Id = new Uri(passedid);
            }


            //Image Overwrite
            if (String.IsNullOrEmpty(passedimage))
            {
                if (recipe.ImageGallery != null)
                    if (recipe.ImageGallery.Count > 0)
                        if (!String.IsNullOrEmpty(recipe.ImageGallery.FirstOrDefault().ImageUrl))
                            myrecipe.Image = new Uri(recipe.ImageGallery.FirstOrDefault().ImageUrl);
            }
            else
                myrecipe.Image = new Uri(passedimage);


            myrecipe.Name = recipe.Detail.ContainsKey(language) ? recipe.Detail[language].Title : recipe.Detail.ContainsKey(fallbacklanguage) ? recipe.Detail[fallbacklanguage].Title : "";
            myrecipe.Description = recipe.Detail.ContainsKey(language) ? recipe.Detail[language].IntroText : recipe.Detail.ContainsKey(fallbacklanguage) ? recipe.Detail[fallbacklanguage].IntroText : "";

            //URL OVERWRITE
            if (String.IsNullOrEmpty(passedurl) && String.IsNullOrEmpty(passedid))
            {
                string url = recipe.ContactInfos.ContainsKey(language) ? recipe.ContactInfos[language].Url : recipe.ContactInfos.ContainsKey(fallbacklanguage) ? recipe.ContactInfos[fallbacklanguage].Url : "";
                if (CheckURLValid(url))
                    myrecipe.Url = new Uri(url);
            }
            else if (!String.IsNullOrEmpty(passedurl))
            {
                myrecipe.Url = new Uri(passedurl);
            }
            else if (!String.IsNullOrEmpty(passedid))
            {
                myrecipe.Url = new Uri(passedid);
            }

            myrecipe.InLanguage = language;
            myrecipe.DatePublished = new DateTimeOffset(recipe.LastChange.Value);
            myrecipe.DateModified = new DateTimeOffset(recipe.LastChange.Value);
            myrecipe.DateCreated = new DateTimeOffset(recipe.FirstImport.Value);

            var recipedetails = GetRecipeDetails(recipe, language);

            if (!String.IsNullOrEmpty(recipedetails.recipeInstructions))
                myrecipe.RecipeInstructions = recipedetails.recipeInstructions;

            if (recipedetails.recipeIngredients != null)
                myrecipe.RecipeIngredient = recipedetails.recipeIngredients.ToArray();

            if (!String.IsNullOrEmpty(recipedetails.recipeYield))
                myrecipe.RecipeYield = recipedetails.recipeYield;

            if (recipedetails.cookTime != TimeSpan.Zero)
                myrecipe.CookTime = recipedetails.cookTime;

            if (recipedetails.prepTime != TimeSpan.Zero)
                myrecipe.PrepTime = recipedetails.prepTime;

            if (recipedetails.recipeNutritionInfo != null)
                myrecipe.Nutrition = recipedetails.recipeNutritionInfo;

            if (recipedetails.author != null)
                myrecipe.Author = recipedetails.author;

            if (recipedetails.aggregateRating != null)
                myrecipe.AggregateRating = recipedetails.aggregateRating;


            if (!String.IsNullOrEmpty(recipedetails.recipeCategory))
                myrecipe.RecipeCategory = recipedetails.recipeCategory;

            if (!String.IsNullOrEmpty(recipedetails.recipeCuisinetype))
                myrecipe.RecipeCuisine = recipedetails.recipeCuisinetype;

            if (!String.IsNullOrEmpty(recipedetails.recipeKeywords))
                myrecipe.Keywords = recipedetails.recipeKeywords;

            return myrecipe;
        }

        private static RecipeDetails GetRecipeDetails(DataModel.Article recipe, string language)
        {
            RecipeDetails myrecipedetail = new RecipeDetails();

            string recipeinstructions = "";
            string recipeYield = "";
            List<string> recipeIngredients = new List<string>();
            TimeSpan cookTime = TimeSpan.Zero;
            TimeSpan prepTime = TimeSpan.Zero;
            AggregateRating aggregaterating = null;
            NutritionInformation nutritioninfo = null;
            Organization author = null;

            string recipecategory = "";
            string recipekeywords = "";
            string recipetypecuisine = "";

            string languagetouse = language;

            if (!recipe.AdditionalArticleInfos.ContainsKey(language))
            {
                languagetouse = "en";
            }

            if (recipe.AdditionalArticleInfos.ContainsKey(languagetouse))
            {
                if (recipe.AdditionalArticleInfos[languagetouse].Elements != null)
                {
                    foreach (var recipeelement in recipe.AdditionalArticleInfos[languagetouse].Elements)
                    {
                        if (recipeelement.Key == "zubereitungstext")
                        {
                            recipeinstructions = recipeinstructions + recipeelement.Value;
                        }
                        if (recipeelement.Key == "tipptext")
                        {
                            recipeinstructions = recipeinstructions + recipeelement.Value;
                        }
                        if (recipeelement.Key == "personen")
                        {
                            Tuple<string, string> persons = new Tuple<string, string>("persons", "person");
                            if (languagetouse == "de")
                                persons = new Tuple<string, string>("Personen", "Person");
                            else if (languagetouse == "it")
                                persons = new Tuple<string, string>("persone", "persona");

                            int personnumber = 0;
                            if (int.TryParse(recipeelement.Value, out personnumber))
                            {
                                if (personnumber == 1)
                                {
                                    recipeYield = recipeelement.Value + " " + persons.Item2;
                                }
                                else
                                {
                                    recipeYield = recipeelement.Value + " " + persons.Item1;
                                }
                            }
                            else
                                recipeYield = recipeelement.Value;


                        }
                        if (recipeelement.Key == "zeit")
                        {
                            char[] separator = { ':' };

                            var kochzeit = recipeelement.Value.Split(separator, 2);

                            if (kochzeit.Length == 2)
                            {
                                int kochzeithours = 0;
                                int kochzeitminutes = 0;


                                int.TryParse(kochzeit[0], out kochzeithours);
                                int.TryParse(kochzeit[1], out kochzeitminutes);

                                if (kochzeithours != 0 || kochzeitminutes != 0)
                                    cookTime = new TimeSpan(kochzeithours, kochzeitminutes, 0);
                            }
                        }
                        if (recipeelement.Key.Contains("zutat"))
                        {
                            //ACHTUNG DES ISCH ALS HTML LISTE DRINN UMWONDELN
                            var doc = new HtmlDocument();
                            doc.LoadHtml(recipeelement.Value);

                            if (doc.DocumentNode.SelectNodes("//li") != null)
                            {
                                foreach (HtmlNode li in doc.DocumentNode.SelectNodes("//li"))
                                {
                                    recipeIngredients.Add(li.InnerHtml);
                                }
                            }
                            else if (doc.DocumentNode.SelectNodes("//p") != null)
                            {
                                foreach (HtmlNode li in doc.DocumentNode.SelectNodes("//p"))
                                {
                                    recipeIngredients.Add(li.InnerHtml);
                                }
                            }
                            else
                            {
                                recipeIngredients.Add(recipeelement.Value);
                            }
                        }
                        //"vorbereitungszeit"
                        if (recipeelement.Key == "vorbereitungszeit")
                        {
                            char[] separator = { ':' };

                            var verbereitungszeit = recipeelement.Value.Split(separator, 2);

                            if (verbereitungszeit.Length == 2)
                            {
                                int verbereitungszeithours = 0;
                                int verbereitungszeitminutes = 0;

                                int.TryParse(verbereitungszeit[0], out verbereitungszeithours);
                                int.TryParse(verbereitungszeit[1], out verbereitungszeitminutes);

                                if (verbereitungszeithours != 0 || verbereitungszeitminutes != 0)
                                    prepTime = new TimeSpan(verbereitungszeithours, verbereitungszeitminutes, 0);
                            }
                        }
                        //"keywords"
                        if (recipeelement.Key == "keywords")
                        {
                            recipekeywords = recipeelement.Value;
                        }
                        //"kategorie"
                        if (recipeelement.Key == "kategorie")
                        {
                            recipecategory = recipeelement.Value;
                        }
                        //"artkueche"
                        if (recipeelement.Key == "artkueche")
                        {
                            recipetypecuisine = recipeelement.Value;
                        }
                        //NEW ADDED 
                        //"kalorien"
                        if (recipeelement.Key == "kalorien")
                        {
                            nutritioninfo = new NutritionInformation();
                            nutritioninfo.Calories = recipeelement.Value;
                        }

                        //"bewertung"
                        if (recipeelement.Key == "bewertung")
                        {

                            char[] separator = { '-' };

                            var rating = recipeelement.Value.Split(separator, 2);

                            if (rating.Length == 2)
                            {
                                int ratingcount = 0;
                                int.TryParse(rating[1], out ratingcount);

                                aggregaterating = new AggregateRating();

                                aggregaterating.RatingValue = rating[0];
                                aggregaterating.RatingCount = ratingcount;
                                aggregaterating.BestRating = 10;
                            }


                            //Organization trustyou = new Organization();
                            //trustyou.Name = "TrustYou";
                            //trustyou.Url = new Uri("https://www.trustyou.com/");

                            //aggregaterating.Author = trustyou;						
                        }

                        //"author"
                        if (recipeelement.Key == "author")
                        {
                            author = new Organization();
                            author.Name = recipeelement.Value;
                        }
                    }

                }
            }

            myrecipedetail.cookTime = cookTime;
            myrecipedetail.recipeIngredients = recipeIngredients;
            myrecipedetail.recipeInstructions = recipeinstructions;
            myrecipedetail.recipeYield = recipeYield;
            myrecipedetail.prepTime = prepTime;
            myrecipedetail.recipeKeywords = recipekeywords;
            myrecipedetail.recipeCategory = recipecategory;
            myrecipedetail.recipeCuisinetype = recipetypecuisine;
            myrecipedetail.author = author;
            myrecipedetail.recipeNutritionInfo = nutritioninfo;
            myrecipedetail.aggregateRating = aggregaterating;

            return myrecipedetail;
        }

        #endregion

        #region Skiarea

        private static Schema.NET.SkiResort TransformSkiResortToLD(DataModel.SkiArea skiarea, DataModel.SkiRegion skiregion, string currentroute, string language, string passedid, string passedurl, string passedimage, bool showid)
        {
            string fallbacklanguage = "en";

            Schema.NET.SkiResort myskiarea = new Schema.NET.SkiResort();

            if (showid)
            {
                if (String.IsNullOrEmpty(passedid))
                    myskiarea.Id = new Uri(currentroute);
                else
                    myskiarea.Id = new Uri(passedid);
            }

            myskiarea.Description = skiarea.Detail.ContainsKey(language) ? skiarea.Detail[language].BaseText : skiarea.Detail.ContainsKey(fallbacklanguage) ? skiarea.Detail[fallbacklanguage].BaseText : "";
            myskiarea.FaxNumber = skiarea.ContactInfos.ContainsKey(language) ? skiarea.ContactInfos[language].Faxnumber : skiarea.ContactInfos.ContainsKey(fallbacklanguage) ? skiarea.ContactInfos[fallbacklanguage].Faxnumber : "";
            myskiarea.Name = skiarea.Detail.ContainsKey(language) ? skiarea.Detail[language].Title : skiarea.Detail.ContainsKey(fallbacklanguage) ? skiarea.Detail[fallbacklanguage].Title : "";
            myskiarea.Telephone = skiarea.ContactInfos.ContainsKey(language) ? skiarea.ContactInfos[language].Phonenumber : skiarea.ContactInfos.ContainsKey(fallbacklanguage) ? skiarea.ContactInfos[fallbacklanguage].Phonenumber : "";

            //Image Overwrite
            if (String.IsNullOrEmpty(passedimage))
            {
                if (skiarea.ImageGallery != null)
                    if (skiarea.ImageGallery.Count > 0)
                        if (!String.IsNullOrEmpty(skiarea.ImageGallery.FirstOrDefault().ImageUrl))
                            myskiarea.Image = new Uri(skiarea.ImageGallery.FirstOrDefault().ImageUrl);
            }
            else
                myskiarea.Image = new Uri(passedimage);

            //URL OVERWRITE
            if (String.IsNullOrEmpty(passedurl) && String.IsNullOrEmpty(passedid))
            {
                string url = skiarea.ContactInfos.ContainsKey(language) ? skiarea.ContactInfos[language].Url : skiarea.ContactInfos.ContainsKey(fallbacklanguage) ? skiarea.ContactInfos[fallbacklanguage].Url : "";
                if (CheckURLValid(url))
                    myskiarea.Url = new Uri(url);
            }
            else if (!String.IsNullOrEmpty(passedurl))
            {
                myskiarea.Url = new Uri(passedurl);
            }
            else if (!String.IsNullOrEmpty(passedid))
            {
                myskiarea.Url = new Uri(passedid);
            }

            string logo = skiarea.ContactInfos.ContainsKey(language) ? skiarea.ContactInfos[language].LogoUrl : skiarea.ContactInfos.ContainsKey(fallbacklanguage) ? skiarea.ContactInfos[fallbacklanguage].LogoUrl : "";
            if (CheckURLValid(logo))
                myskiarea.Logo = new Uri(logo);


            myskiarea.OpeningHours = GetOpeningDatesSkiArea(skiarea.OperationSchedule);


            if (CheckURLValid(skiarea.SkiAreaMapURL))
                myskiarea.HasMap = new Uri(skiarea.SkiAreaMapURL);

            myskiarea.IsAccessibleForFree = false;



            PostalAddress myaddress = new PostalAddress();
            //myaddress.Type = "http://schema.org/PostalAddress";
            myaddress.StreetAddress = skiarea.ContactInfos.ContainsKey(language) ? skiarea.ContactInfos[language].Address : skiarea.ContactInfos.ContainsKey(fallbacklanguage) ? skiarea.ContactInfos[fallbacklanguage].Address : "";
            myaddress.PostalCode = skiarea.ContactInfos.ContainsKey(language) ? skiarea.ContactInfos[language].ZipCode : skiarea.ContactInfos.ContainsKey(fallbacklanguage) ? skiarea.ContactInfos[fallbacklanguage].ZipCode : "";
            myaddress.AddressLocality = skiarea.ContactInfos.ContainsKey(language) ? skiarea.ContactInfos[language].City : skiarea.ContactInfos.ContainsKey(fallbacklanguage) ? skiarea.ContactInfos[fallbacklanguage].City : "";
            myaddress.AddressRegion = getRegionDependingonLanguage(language);
            myaddress.AddressCountry = skiarea.ContactInfos.ContainsKey(language) ? skiarea.ContactInfos[language].CountryName : skiarea.ContactInfos.ContainsKey(fallbacklanguage) ? skiarea.ContactInfos[fallbacklanguage].CountryName : "";
            myaddress.Telephone = skiarea.ContactInfos.ContainsKey(language) ? skiarea.ContactInfos[language].Phonenumber : skiarea.ContactInfos.ContainsKey(fallbacklanguage) ? skiarea.ContactInfos[fallbacklanguage].Phonenumber : "";

            string adressurl = skiarea.ContactInfos.ContainsKey(language) ? skiarea.ContactInfos[language].Url : skiarea.ContactInfos.ContainsKey(fallbacklanguage) ? skiarea.ContactInfos[fallbacklanguage].Url : "";
            if (CheckURLValid(adressurl))
                myaddress.Url = new Uri(adressurl);

            myaddress.Email = skiarea.ContactInfos.ContainsKey(language) ? skiarea.ContactInfos[language].Email : skiarea.ContactInfos.ContainsKey(fallbacklanguage) ? skiarea.ContactInfos[fallbacklanguage].Email : "";
            myaddress.FaxNumber = skiarea.ContactInfos.ContainsKey(language) ? skiarea.ContactInfos[language].Faxnumber : skiarea.ContactInfos.ContainsKey(fallbacklanguage) ? skiarea.ContactInfos[fallbacklanguage].Faxnumber : "";
            myaddress.AlternateName = skiarea.ContactInfos.ContainsKey(language) ? skiarea.ContactInfos[language].CompanyName : skiarea.ContactInfos.ContainsKey(fallbacklanguage) ? skiarea.ContactInfos[fallbacklanguage].CompanyName : "";
            myaddress.Name = skiarea.ContactInfos.ContainsKey(language) ? skiarea.Detail[language].Title : skiarea.ContactInfos.ContainsKey(fallbacklanguage) ? skiarea.Detail[fallbacklanguage].Title : "";


            myskiarea.Address = myaddress;

            GeoCoordinates mygeo = new GeoCoordinates();
            //mygeo.type = "http://schema.org/GeoCoordinates";
            mygeo.Latitude = skiarea.Latitude;
            mygeo.Longitude = skiarea.Longitude;

            myskiarea.Geo = mygeo;

            //Mitglied OSA DSS         

            if (skiregion != null)
            {
                var parentorganization = new Organization();
                parentorganization.Description = skiregion.Detail.ContainsKey(language) ? skiregion.Detail[language].BaseText : skiregion.Detail.ContainsKey(fallbacklanguage) ? skiregion.Detail[fallbacklanguage].BaseText : "";
                parentorganization.FaxNumber = skiregion.ContactInfos.ContainsKey(language) ? skiregion.ContactInfos[language].Faxnumber : skiregion.ContactInfos.ContainsKey(fallbacklanguage) ? skiregion.ContactInfos[fallbacklanguage].Faxnumber : "";
                parentorganization.Name = skiregion.Detail.ContainsKey(language) ? skiregion.Detail[language].Title : skiregion.Detail.ContainsKey(fallbacklanguage) ? skiregion.Detail[fallbacklanguage].Title : "";
                parentorganization.Telephone = skiregion.ContactInfos.ContainsKey(language) ? skiregion.ContactInfos[language].Phonenumber : skiregion.ContactInfos.ContainsKey(fallbacklanguage) ? skiregion.ContactInfos[fallbacklanguage].Phonenumber : "";
                string url = skiregion.ContactInfos.ContainsKey(language) ? skiregion.ContactInfos[language].Url : skiregion.ContactInfos.ContainsKey(fallbacklanguage) ? skiregion.ContactInfos[fallbacklanguage].Url : "";
                if (CheckURLValid(url))
                    parentorganization.Url = new Uri(url);

                PostalAddress myskiregionaddress = new PostalAddress();
                //myaddress.Type = "http://schema.org/PostalAddress";
                myskiregionaddress.StreetAddress = skiregion.ContactInfos.ContainsKey(language) ? skiregion.ContactInfos[language].Address : skiregion.ContactInfos.ContainsKey(fallbacklanguage) ? skiregion.ContactInfos[fallbacklanguage].Address : "";
                myskiregionaddress.PostalCode = skiregion.ContactInfos.ContainsKey(language) ? skiregion.ContactInfos[language].ZipCode : skiregion.ContactInfos.ContainsKey(fallbacklanguage) ? skiregion.ContactInfos[fallbacklanguage].ZipCode : "";
                myskiregionaddress.AddressLocality = skiregion.ContactInfos.ContainsKey(language) ? skiregion.ContactInfos[language].City : skiregion.ContactInfos.ContainsKey(fallbacklanguage) ? skiregion.ContactInfos[fallbacklanguage].City : "";
                myskiregionaddress.AddressRegion = getRegionDependingonLanguage(language);
                myskiregionaddress.AddressCountry = skiregion.ContactInfos.ContainsKey(language) ? skiregion.ContactInfos[language].CountryName : skiregion.ContactInfos.ContainsKey(fallbacklanguage) ? skiregion.ContactInfos[fallbacklanguage].CountryName : "";
                myskiregionaddress.Telephone = skiregion.ContactInfos.ContainsKey(language) ? skiregion.ContactInfos[language].Phonenumber : skiregion.ContactInfos.ContainsKey(fallbacklanguage) ? skiregion.ContactInfos[fallbacklanguage].Phonenumber : "";

                string adressurlskiregion = skiregion.ContactInfos.ContainsKey(language) ? skiregion.ContactInfos[language].Url : skiregion.ContactInfos.ContainsKey(fallbacklanguage) ? skiregion.ContactInfos[fallbacklanguage].Url : "";
                if (CheckURLValid(adressurl))
                    myskiregionaddress.Url = new Uri(adressurlskiregion);

                parentorganization.Address = myskiregionaddress;

                myskiarea.ParentOrganization = parentorganization;
            }


            //PriceRange
            var liftcount = 0;
            if (int.TryParse(skiarea.LiftCount, out liftcount))
            {
                if (liftcount != 0)
                {
                    if (liftcount < 6)
                        myskiarea.PriceRange = "€";
                    else if (liftcount >= 6 && liftcount < 20)
                        myskiarea.PriceRange = "€€";
                    else if (liftcount >= 20)
                        myskiarea.PriceRange = "€€€";
                }
            }


            return myskiarea;
        }

        private static List<string> GetOpeningDatesSkiArea(ICollection<OperationSchedule> operationschedules)
        {
            List<string> openingtime = new List<string>();

            if (operationschedules != null)
            {
                foreach (var operationschedule in operationschedules.Where(x => x.Start <= DateTime.Now && x.Stop >= DateTime.Now))
                {
                    //IF there are 

                    if (operationschedule.OperationScheduleTime != null)
                    {
                        foreach (var operationtime in operationschedule.OperationScheduleTime)
                        {
                            if (operationtime.Monday && operationtime.Tuesday && operationtime.Wednesday && operationtime.Thuresday && operationtime.Friday && operationtime.Saturday && operationtime.Sunday)
                            {
                                openingtime.Add("Mo-Su " + operationtime.Start + " - " + operationtime.End);
                            }
                            else
                            {

                                if (operationtime.Monday)
                                    openingtime.Add("Mo " + operationtime.Start + " - " + operationtime.End);
                                if (operationtime.Tuesday)
                                    openingtime.Add("Tu " + operationtime.Start + " - " + operationtime.End);
                                if (operationtime.Wednesday)
                                    openingtime.Add("We " + operationtime.Start + " - " + operationtime.End);
                                if (operationtime.Thuresday)
                                    openingtime.Add("Th " + operationtime.Start + " - " + operationtime.End);
                                if (operationtime.Friday)
                                    openingtime.Add("Fr " + operationtime.Start + " - " + operationtime.End);
                                if (operationtime.Saturday)
                                    openingtime.Add("Sa " + operationtime.Start + " - " + operationtime.End);
                                if (operationtime.Sunday)
                                    openingtime.Add("Su " + operationtime.Start + " - " + operationtime.End);
                            }
                        }

                    }
                    //No opening time defined valid for whole week default time entered for skiareas 08:30 - 16:30
                    else
                    {
                        openingtime.Add("Mo-Su 08:30 - 16:30");
                    }
                }
            }

            return openingtime;
        }


        #endregion

        #region Place

        private static Schema.NET.Place TransformPlaceToLD(DataModel.Region placetotrasform, string currentroute, string language, string passedid, string passedurl, string passedimage, bool showid)
        {
            string fallbacklanguage = "en";

            Schema.NET.Place place = new Schema.NET.Place();

            if (showid)
            {
                if (String.IsNullOrEmpty(passedid))
                    place.Id = new Uri(currentroute);
                else
                    place.Id = new Uri(passedid);
            }

            place.Description = placetotrasform.Detail.ContainsKey(language) ? placetotrasform.Detail[language].BaseText : placetotrasform.Detail.ContainsKey(fallbacklanguage) ? placetotrasform.Detail[fallbacklanguage].BaseText : "";
            place.Name = placetotrasform.Detail.ContainsKey(language) ? placetotrasform.Detail[language].Title : placetotrasform.Detail.ContainsKey(fallbacklanguage) ? placetotrasform.Detail[fallbacklanguage].Title : "";

            place.FaxNumber = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Faxnumber : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Faxnumber : "";
            place.Telephone = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Phonenumber : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Phonenumber : "";

            //Image Overwrite
            if (String.IsNullOrEmpty(passedimage))
            {
                if (placetotrasform.ImageGallery != null)
                    if (placetotrasform.ImageGallery.Count > 0)
                        if (!String.IsNullOrEmpty(placetotrasform.ImageGallery.FirstOrDefault().ImageUrl))
                            place.Image = new Uri(placetotrasform.ImageGallery.FirstOrDefault().ImageUrl);
            }
            else
                place.Image = new Uri(passedimage);

            // URL OVERWRITE
            if (String.IsNullOrEmpty(passedurl) && String.IsNullOrEmpty(passedid))
            {
                string url = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Url : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Url : "";
                if (CheckURLValid(url))
                    place.Url = new Uri(url);
            }
            else if (!String.IsNullOrEmpty(passedurl))
            {
                if (CheckURLValid(passedurl))
                    place.Url = new Uri(passedurl);
            }
            else if (!String.IsNullOrEmpty(passedid))
            {
                if (CheckURLValid(passedid))
                    place.Url = new Uri(passedid);
            }

            string logo = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].LogoUrl : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].LogoUrl : "";
            if (CheckURLValid(logo))
                place.Logo = new Uri(logo);


            PostalAddress myaddress = new PostalAddress();
            //myaddress.Type = "http://schema.org/PostalAddress";
            myaddress.StreetAddress = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Address : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Address : "";
            myaddress.PostalCode = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].ZipCode : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].ZipCode : "";
            myaddress.AddressLocality = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].City : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].City : "";
            myaddress.AddressRegion = getRegionDependingonLanguage(language);
            myaddress.AddressCountry = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].CountryName : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].CountryName : "";
            myaddress.Telephone = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Phonenumber : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Phonenumber : "";

            string adressurl = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Url : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Url : "";
            if (CheckURLValid(adressurl))
                myaddress.Url = new Uri(adressurl);

            myaddress.Email = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Email : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Email : "";
            myaddress.FaxNumber = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Faxnumber : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Faxnumber : "";
            myaddress.AlternateName = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].CompanyName : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].CompanyName : "";
            myaddress.Name = placetotrasform.Detail.ContainsKey(language) ? placetotrasform.Detail[language].Title : placetotrasform.Detail.ContainsKey(fallbacklanguage) ? placetotrasform.Detail[fallbacklanguage].Title : "";


            place.Address = myaddress;

            GeoCoordinates mygeo = new GeoCoordinates();
            //mygeo.type = "http://schema.org/GeoCoordinates";
            mygeo.Latitude = placetotrasform.Latitude;
            mygeo.Longitude = placetotrasform.Longitude;

            place.Geo = mygeo;

            return place;
        }

        private static Schema.NET.Place TransformPlaceToLD(DataModel.Tourismverein placetotrasform, string currentroute, string language, string passedid, string passedurl, string passedimage, bool showid)
        {
            string fallbacklanguage = "en";

            Schema.NET.Place place = new Schema.NET.Place();

            if (showid)
            {
                if (String.IsNullOrEmpty(passedid))
                    place.Id = new Uri(currentroute);
                else
                    place.Id = new Uri(passedid);
            }

            place.Description = placetotrasform.Detail.ContainsKey(language) ? placetotrasform.Detail[language].BaseText : placetotrasform.Detail.ContainsKey(fallbacklanguage) ? placetotrasform.Detail[fallbacklanguage].BaseText : "";
            place.Name = placetotrasform.Detail.ContainsKey(language) ? placetotrasform.Detail[language].Title : placetotrasform.Detail.ContainsKey(fallbacklanguage) ? placetotrasform.Detail[fallbacklanguage].Title : "";

            place.FaxNumber = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Faxnumber : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Faxnumber : "";
            place.Telephone = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Phonenumber : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Phonenumber : "";


            //Image Overwrite
            if (String.IsNullOrEmpty(passedimage))
            {
                if (placetotrasform.ImageGallery != null)
                    if (placetotrasform.ImageGallery.Count > 0)
                        if (!String.IsNullOrEmpty(placetotrasform.ImageGallery.FirstOrDefault().ImageUrl))
                            place.Image = new Uri(placetotrasform.ImageGallery.FirstOrDefault().ImageUrl);
            }
            else
                place.Image = new Uri(passedimage);

            //URL OVERWRITE
            if (String.IsNullOrEmpty(passedurl) && String.IsNullOrEmpty(passedid))
            {
                string url = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Url : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Url : "";
                if (CheckURLValid(url))
                    place.Url = new Uri(url);
            }
            else if (!String.IsNullOrEmpty(passedurl))
            {
                if (CheckURLValid(passedurl))
                    place.Url = new Uri(passedurl);
            }
            else if (!String.IsNullOrEmpty(passedid))
            {
                if (CheckURLValid(passedid))
                    place.Url = new Uri(passedid);
            }

            string logo = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].LogoUrl : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].LogoUrl : "";
            if (CheckURLValid(logo))
                place.Logo = new Uri(logo);


            PostalAddress myaddress = new PostalAddress();
            //myaddress.Type = "http://schema.org/PostalAddress";
            myaddress.StreetAddress = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Address : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Address : "";
            myaddress.PostalCode = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].ZipCode : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].ZipCode : "";
            myaddress.AddressLocality = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].City : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].City : "";
            myaddress.AddressRegion = getRegionDependingonLanguage(language);
            myaddress.AddressCountry = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].CountryName : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].CountryName : "";
            myaddress.Telephone = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Phonenumber : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Phonenumber : "";

            string adressurl = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Url : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Url : "";
            if (CheckURLValid(adressurl))
                myaddress.Url = new Uri(adressurl);

            myaddress.Email = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Email : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Email : "";
            myaddress.FaxNumber = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Faxnumber : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Faxnumber : "";
            myaddress.AlternateName = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].CompanyName : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].CompanyName : "";
            myaddress.Name = placetotrasform.Detail.ContainsKey(language) ? placetotrasform.Detail[language].Title : placetotrasform.Detail.ContainsKey(fallbacklanguage) ? placetotrasform.Detail[fallbacklanguage].Title : "";


            place.Address = myaddress;

            GeoCoordinates mygeo = new GeoCoordinates();
            //mygeo.type = "http://schema.org/GeoCoordinates";
            mygeo.Latitude = placetotrasform.Latitude;
            mygeo.Longitude = placetotrasform.Longitude;

            place.Geo = mygeo;

            return place;
        }

        private static Schema.NET.Place TransformPlaceToLD(DataModel.Municipality placetotrasform, string currentroute, string language, string passedid, string passedurl, string passedimage, bool showid)
        {
            string fallbacklanguage = "en";

            Schema.NET.Place place = new Schema.NET.Place();

            if (showid)
            {
                if (String.IsNullOrEmpty(passedid))
                    place.Id = new Uri(currentroute);
                else
                    place.Id = new Uri(passedid);
            }

            place.Description = placetotrasform.Detail.ContainsKey(language) ? placetotrasform.Detail[language].BaseText : placetotrasform.Detail.ContainsKey(fallbacklanguage) ? placetotrasform.Detail[fallbacklanguage].BaseText : "";
            place.Name = placetotrasform.Detail.ContainsKey(language) ? placetotrasform.Detail[language].Title : placetotrasform.Detail.ContainsKey(fallbacklanguage) ? placetotrasform.Detail[fallbacklanguage].Title : "";

            place.FaxNumber = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Faxnumber : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Faxnumber : "";
            place.Telephone = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Phonenumber : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Phonenumber : "";

            //Image Overwrite
            if (String.IsNullOrEmpty(passedimage))
            {
                if (placetotrasform.ImageGallery != null)
                    if (placetotrasform.ImageGallery.Count > 0)
                        if (!String.IsNullOrEmpty(placetotrasform.ImageGallery.FirstOrDefault().ImageUrl))
                            place.Image = new Uri(placetotrasform.ImageGallery.FirstOrDefault().ImageUrl);
            }
            else
                place.Image = new Uri(passedimage);


            //URL OVERWRITE
            if (String.IsNullOrEmpty(passedurl) && String.IsNullOrEmpty(passedid))
            {
                string url = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Url : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Url : "";
                if (CheckURLValid(url))
                    place.Url = new Uri(url);
            }
            else if (!String.IsNullOrEmpty(passedurl))
            {
                if (CheckURLValid(passedurl))
                    place.Url = new Uri(passedurl);
            }
            else if (!String.IsNullOrEmpty(passedid))
            {
                if (CheckURLValid(passedid))
                    place.Url = new Uri(passedid);
            }

            string logo = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].LogoUrl : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].LogoUrl : "";
            if (CheckURLValid(logo))
                place.Logo = new Uri(logo);

            PostalAddress myaddress = new PostalAddress();
            //myaddress.Type = "http://schema.org/PostalAddress";
            myaddress.StreetAddress = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Address : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Address : "";
            myaddress.PostalCode = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].ZipCode : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].ZipCode : "";
            myaddress.AddressLocality = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].City : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].City : "";
            myaddress.AddressRegion = getRegionDependingonLanguage(language);
            myaddress.AddressCountry = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].CountryName : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].CountryName : "";
            myaddress.Telephone = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Phonenumber : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Phonenumber : "";

            string adressurl = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Url : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Url : "";
            if (CheckURLValid(adressurl))
                myaddress.Url = new Uri(adressurl);

            myaddress.Email = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Email : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Email : "";
            myaddress.FaxNumber = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Faxnumber : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Faxnumber : "";
            myaddress.AlternateName = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].CompanyName : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].CompanyName : "";
            myaddress.Name = placetotrasform.Detail.ContainsKey(language) ? placetotrasform.Detail[language].Title : placetotrasform.Detail.ContainsKey(fallbacklanguage) ? placetotrasform.Detail[fallbacklanguage].Title : "";


            place.Address = myaddress;

            GeoCoordinates mygeo = new GeoCoordinates();
            //mygeo.type = "http://schema.org/GeoCoordinates";
            mygeo.Latitude = placetotrasform.Latitude;
            mygeo.Longitude = placetotrasform.Longitude;

            place.Geo = mygeo;

            return place;
        }

        private static Schema.NET.Place TransformPlaceToLD(DataModel.District placetotrasform, string currentroute, string language, string passedid, string passedurl, string passedimage, bool showid)
        {
            string fallbacklanguage = "en";
            //Winery, Museum

            Schema.NET.Place place = new Schema.NET.Place();

            if (showid)
            {
                if (String.IsNullOrEmpty(passedid))
                    place.Id = new Uri(currentroute);
                else
                    place.Id = new Uri(passedid);
            }

            place.Description = placetotrasform.Detail.ContainsKey(language) ? placetotrasform.Detail[language].BaseText : placetotrasform.Detail.ContainsKey(fallbacklanguage) ? placetotrasform.Detail[fallbacklanguage].BaseText : "";
            place.Name = placetotrasform.Detail.ContainsKey(language) ? placetotrasform.Detail[language].Title : placetotrasform.Detail.ContainsKey(fallbacklanguage) ? placetotrasform.Detail[fallbacklanguage].Title : "";

            place.FaxNumber = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Faxnumber : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Faxnumber : "";
            place.Telephone = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Phonenumber : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Phonenumber : "";


            //Image Overwrite
            if (String.IsNullOrEmpty(passedimage))
            {
                if (placetotrasform.ImageGallery != null)
                    if (placetotrasform.ImageGallery.Count > 0)
                        if (!String.IsNullOrEmpty(placetotrasform.ImageGallery.FirstOrDefault().ImageUrl))
                            place.Image = new Uri(placetotrasform.ImageGallery.FirstOrDefault().ImageUrl);
            }
            else
                place.Image = new Uri(passedimage);

            //URL OVERWRITE
            if (String.IsNullOrEmpty(passedurl) && String.IsNullOrEmpty(passedid))
            {
                string url = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Url : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Url : "";
                if (CheckURLValid(url))
                    place.Url = new Uri(url);
            }
            else if (!String.IsNullOrEmpty(passedurl))
            {
                if (CheckURLValid(passedurl))
                    place.Url = new Uri(passedurl);
            }
            else if (!String.IsNullOrEmpty(passedid))
            {
                if (CheckURLValid(passedid))
                    place.Url = new Uri(passedid);
            }


            string logo = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].LogoUrl : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].LogoUrl : "";
            if (CheckURLValid(logo))
                place.Logo = new Uri(logo);


            PostalAddress myaddress = new PostalAddress();
            //myaddress.Type = "http://schema.org/PostalAddress";
            myaddress.StreetAddress = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Address : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Address : "";
            myaddress.PostalCode = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].ZipCode : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].ZipCode : "";
            myaddress.AddressLocality = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].City : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].City : "";
            myaddress.AddressRegion = getRegionDependingonLanguage(language);
            myaddress.AddressCountry = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].CountryName : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].CountryName : "";
            myaddress.Telephone = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Phonenumber : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Phonenumber : "";

            string adressurl = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Url : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Url : "";
            if (CheckURLValid(adressurl))
                myaddress.Url = new Uri(adressurl);

            myaddress.Email = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Email : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Email : "";
            myaddress.FaxNumber = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].Faxnumber : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].Faxnumber : "";
            myaddress.AlternateName = placetotrasform.ContactInfos.ContainsKey(language) ? placetotrasform.ContactInfos[language].CompanyName : placetotrasform.ContactInfos.ContainsKey(fallbacklanguage) ? placetotrasform.ContactInfos[fallbacklanguage].CompanyName : "";
            myaddress.Name = placetotrasform.Detail.ContainsKey(language) ? placetotrasform.Detail[language].Title : placetotrasform.Detail.ContainsKey(fallbacklanguage) ? placetotrasform.Detail[fallbacklanguage].Title : "";


            place.Address = myaddress;

            GeoCoordinates mygeo = new GeoCoordinates();
            //mygeo.type = "http://schema.org/GeoCoordinates";
            mygeo.Latitude = placetotrasform.Latitude;
            mygeo.Longitude = placetotrasform.Longitude;

            place.Geo = mygeo;

            return place;
        }

        #endregion

        #region SpecialAnnouncement

        private static SpecialAnnouncement TransformSpecialAnnouncementToLD(DataModel.Article specialannouncement, string currentroute, string language, string passedid, string passedurl, string passedimage, bool showid)
        {
            string fallbacklanguage = "en";

            //Check if data has this fallbacklanguage
            if (!specialannouncement.HasLanguage.Contains(language))
                language = specialannouncement.HasLanguage.FirstOrDefault();

            //Schema.NET.CreativeWork SpecialAnnouncement
            SpecialAnnouncement specialannouncementobj = new SpecialAnnouncement();

            //Setting custom Type
            specialannouncementobj.Type = "SpecialAnnouncement";

            if (showid)
            {
                if (String.IsNullOrEmpty(passedid))
                    specialannouncementobj.Id = new Uri(currentroute);
                else
                    specialannouncementobj.Id = new Uri(passedid);
            }

            //Image Overwrite
            if (String.IsNullOrEmpty(passedimage))
            {
                if (specialannouncement.ImageGallery != null)
                    if (specialannouncement.ImageGallery.Count > 0)
                        if (!String.IsNullOrEmpty(specialannouncement.ImageGallery.FirstOrDefault().ImageUrl))
                            specialannouncementobj.Image = new Uri(specialannouncement.ImageGallery.FirstOrDefault().ImageUrl);
            }
            else
                specialannouncementobj.Image = new Uri(passedimage);


            specialannouncementobj.Name = specialannouncement.Detail.ContainsKey(language) ? specialannouncement.Detail[language].Title : specialannouncement.Detail.ContainsKey(fallbacklanguage) ? specialannouncement.Detail[fallbacklanguage].Title : "";
            specialannouncementobj.Description = specialannouncement.Detail.ContainsKey(language) ? specialannouncement.Detail[language].IntroText : specialannouncement.Detail.ContainsKey(fallbacklanguage) ? specialannouncement.Detail[fallbacklanguage].IntroText : "";

            //NEW
            specialannouncementobj.AlternateName = specialannouncement.Detail.ContainsKey(language) ? specialannouncement.Detail[language].AdditionalText : specialannouncement.Detail.ContainsKey(fallbacklanguage) ? specialannouncement.Detail[fallbacklanguage].AdditionalText : "";
            specialannouncementobj.Text = specialannouncement.Detail.ContainsKey(language) ? specialannouncement.Detail[language].BaseText : specialannouncement.Detail.ContainsKey(fallbacklanguage) ? specialannouncement.Detail[fallbacklanguage].BaseText : "";

            //URL OVERWRITE
            if (String.IsNullOrEmpty(passedurl) && String.IsNullOrEmpty(passedid))
            {
                string url = specialannouncement.ContactInfos.ContainsKey(language) ? specialannouncement.ContactInfos[language].Url : specialannouncement.ContactInfos.ContainsKey(fallbacklanguage) ? specialannouncement.ContactInfos[fallbacklanguage].Url : "";
                if (CheckURLValid(url))
                    specialannouncementobj.Url = new Uri(url);
            }
            else if (!String.IsNullOrEmpty(passedurl))
            {
                specialannouncementobj.Url = new Uri(passedurl);
            }
            else if (!String.IsNullOrEmpty(passedid))
            {
                specialannouncementobj.Url = new Uri(passedid);
            }

            specialannouncementobj.InLanguage = language;
            specialannouncementobj.DatePublished = new DateTimeOffset(specialannouncement.LastChange.Value);
            specialannouncementobj.DateModified = new DateTimeOffset(specialannouncement.LastChange.Value);
            specialannouncementobj.DateCreated = new DateTimeOffset(specialannouncement.FirstImport.Value);
            specialannouncementobj.DatePosted = new DateTimeOffset(specialannouncement.LastChange.Value);
            if (specialannouncement.ExpirationDate != null)
                specialannouncementobj.Expires = (DateTime)specialannouncement.ExpirationDate;

            //TODOS

            //specialannouncementobj.Abstract
            //specialannouncementobj.CopyrightNotice
            //specialannouncementobj.License
            //specialannouncementobj.SdPublisher
            //specialannouncementobj.AnnouncementLocation
            //specialannouncementobj.GovernmentBenefitsInfo


            //Create Place for SpatialCoverage
            //location.AddressCountry = specialannouncement.ContactInfos.ContainsKey(language) ? specialannouncement.ContactInfos[language].CountryName : specialannouncement.ContactInfos.ContainsKey(fallbacklanguage) ? specialannouncement.ContactInfos[fallbacklanguage].CountryName : "";

            //Place location = new Place();
            //location.Address = myaddress;
            //location.type = "Place";
            //location.Name = specialannouncement.ContactInfos.ContainsKey(language) ? specialannouncement.ContactInfos[language].CompanyName : specialannouncement.ContactInfos.ContainsKey(fallbacklanguage) ? specialannouncement.ContactInfos[fallbacklanguage].CompanyName : "";

            //Deactivated at moment
            //if (specialannouncement.SpatialCoverage != null && specialannouncement.SpatialCoverage.Count > 0)
            //{
            //    List<Place> placelist = new List<Place>();

            //    foreach (var spatialcoverage in specialannouncement.SpatialCoverage)
            //    {
            //        Place location = new Place();
            //        location.Name = spatialcoverage.Name[language];

            //        if (spatialcoverage.GpsInfo != null)
            //        {
            //            GeoCoordinates mygeo = new GeoCoordinates();
            //            //mygeo.type = "http://schema.org/GeoCoordinates";
            //            mygeo.Latitude = spatialcoverage.GpsInfo.Latitude;
            //            mygeo.Longitude = spatialcoverage.GpsInfo.Longitude;

            //            location.Geo = mygeo;
            //        }

            //        placelist.Add(location);
            //    }

            //    specialannouncementobj.SpatialCoverage = new OneOrMany<IPlace>(placelist);
            //}
            //else
            //{
                //Create Place for SpatialCoverage
                PostalAddress myaddress = new PostalAddress();
                //myaddress.type = "http://schema.org/PostalAddress";
                myaddress.StreetAddress = specialannouncement.ContactInfos.ContainsKey(language) ? specialannouncement.ContactInfos[language].Address : specialannouncement.ContactInfos.ContainsKey(fallbacklanguage) ? specialannouncement.ContactInfos[fallbacklanguage].Address : "";
                myaddress.PostalCode = specialannouncement.ContactInfos.ContainsKey(language) ? specialannouncement.ContactInfos[language].ZipCode : specialannouncement.ContactInfos.ContainsKey(fallbacklanguage) ? specialannouncement.ContactInfos[fallbacklanguage].ZipCode : "";
                myaddress.AddressLocality = specialannouncement.ContactInfos.ContainsKey(language) ? specialannouncement.ContactInfos[language].City : specialannouncement.ContactInfos.ContainsKey(fallbacklanguage) ? specialannouncement.ContactInfos[fallbacklanguage].City : "";
                myaddress.AddressRegion = getRegionDependingonLanguage(language);
                myaddress.AddressCountry = specialannouncement.ContactInfos.ContainsKey(language) ? specialannouncement.ContactInfos[language].CountryName : specialannouncement.ContactInfos.ContainsKey(fallbacklanguage) ? specialannouncement.ContactInfos[fallbacklanguage].CountryName : "";
                //location.AddressCountry = specialannouncement.ContactInfos.ContainsKey(language) ? specialannouncement.ContactInfos[language].CountryName : specialannouncement.ContactInfos.ContainsKey(fallbacklanguage) ? specialannouncement.ContactInfos[fallbacklanguage].CountryName : "";

                Place location = new Place();
                location.Address = myaddress;

                specialannouncementobj.SpatialCoverage = location;
            //}

            //LocalBusiness locbusiness = new LocalBusiness();
            //locbusiness.Address = myaddress;
            //locbusiness.Name = specialannouncement.ContactInfos.ContainsKey(language) ? specialannouncement.ContactInfos[language].CompanyName : specialannouncement.ContactInfos.ContainsKey(fallbacklanguage) ? specialannouncement.ContactInfos[fallbacklanguage].CompanyName : "";


            //specialannouncementobj.AnnouncementLocation = locbusiness;

            //New get trough the Article

            var additionalinfos = specialannouncement.AdditionalArticleInfos.ContainsKey(language) ? specialannouncement.AdditionalArticleInfos[language] : specialannouncement.AdditionalArticleInfos.ContainsKey(fallbacklanguage) ? specialannouncement.AdditionalArticleInfos[fallbacklanguage] : null;

            if (additionalinfos != null)
            {
                foreach (var additionalinfo in additionalinfos.Elements)
                {
                    if (additionalinfo.Key.ToLower() == "diseasepreventioninfo")
                    {
                        if (Uri.TryCreate(additionalinfo.Value, UriKind.Absolute, out var uriresult))
                            specialannouncementobj.DiseasePreventionInfo = uriresult;
                        else
                            specialannouncementobj.DiseasePreventionInfo = additionalinfo.Value;
                    }
                    else if (additionalinfo.Key.ToLower() == "diseasespreadstatistics")
                    {
                        if (Uri.TryCreate(additionalinfo.Value, UriKind.Absolute, out var uriresult))
                            specialannouncementobj.DiseaseSpreadStatistics = uriresult;
                        else
                            specialannouncementobj.DiseaseSpreadStatistics = additionalinfo.Value;
                    }
                    else if (additionalinfo.Key.ToLower() == "gettingtestedinfo")
                    {
                        if (Uri.TryCreate(additionalinfo.Value, UriKind.Absolute, out var uriresult))
                            specialannouncementobj.GettingTestedInfo = uriresult;
                        else
                            specialannouncementobj.GettingTestedInfo = additionalinfo.Value;
                    }
                    else if (additionalinfo.Key.ToLower() == "newsupdatesandguidelines")
                    {
                        if (Uri.TryCreate(additionalinfo.Value, UriKind.Absolute, out var uriresult))
                            specialannouncementobj.NewsUpdatesAndGuidelines = uriresult;
                        else
                            specialannouncementobj.NewsUpdatesAndGuidelines = additionalinfo.Value;
                    }
                    else if (additionalinfo.Key.ToLower() == "publictransportclosuresinfo")
                    {
                        if (Uri.TryCreate(additionalinfo.Value, UriKind.Absolute, out var uriresult))
                            specialannouncementobj.PublicTransportClosuresInfo = uriresult;
                        else
                            specialannouncementobj.PublicTransportClosuresInfo = additionalinfo.Value;
                    }
                    else if (additionalinfo.Key.ToLower() == "quarantineguidelines")
                    {
                        if (Uri.TryCreate(additionalinfo.Value, UriKind.Absolute, out var uriresult))
                            specialannouncementobj.QuarantineGuidelines = uriresult;
                        else
                            specialannouncementobj.QuarantineGuidelines = additionalinfo.Value;
                    }
                    else if (additionalinfo.Key.ToLower() == "schoolclosuresinfo")
                    {
                        if (Uri.TryCreate(additionalinfo.Value, UriKind.Absolute, out var uriresult))
                            specialannouncementobj.SchoolClosuresInfo = uriresult;
                        else
                            specialannouncementobj.SchoolClosuresInfo = additionalinfo.Value;
                    }
                    else if (additionalinfo.Key.ToLower() == "travelbans")
                    {
                        if (Uri.TryCreate(additionalinfo.Value, UriKind.Absolute, out var uriresult))
                            specialannouncementobj.TravelBans = uriresult;
                        else
                            specialannouncementobj.TravelBans = additionalinfo.Value;
                    }
                    //New expires and overwrite published date
                    else if (additionalinfo.Key.ToLower() == "expires")
                    {
                        if (DateTime.TryParse(additionalinfo.Value, out var dateresult))
                            specialannouncementobj.Expires = dateresult;
                    }
                    else if (additionalinfo.Key.ToLower() == "dateposted")
                    {
                        if (DateTime.TryParse(additionalinfo.Value, out var dateresult))
                            specialannouncementobj.DatePosted = dateresult;
                    }
                }
            }

            return specialannouncementobj;
        }

        #endregion


        private static string getRegionDependingonLanguage(string language)
        {
            switch (language)
            {
                case "de":
                    return "Südtirol";
                case "it":
                    return "Alto Adige";
                case "en":
                    return "South Tyrol";
                case "nl":
                    return "Zuid-Tirol";
                case "cs":
                    return "Jižní Tyrolsko";
                case "pl":
                    return "Południowy Tyrol";
                case "fr":
                    return "Sud-Tyrol";
                case "ru":
                    return "Южный Тироль";
                default:
                    return "Südtirol - Alto Adige";
            }
        }

        private static string getCountryDependingonLanguage(string language)
        {
            switch (language)
            {
                case "de":
                    return "Italien";
                case "it":
                    return "Italia";
                case "en":
                    return "Italy";
                case "nl":
                    return "Italië";
                case "cs":
                    return "Itálie";
                case "pl":
                    return "Włochy";
                case "fr":
                    return "Italie";
                case "ru":
                    return "Италия";
                default:
                    return "Italy";
            }
        }

        public static bool CheckURLValid(string source)
        {
            Uri uriResult;
            return Uri.TryCreate(source, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttp;
        }
    }

    public class RecipeDetails
    {
        public TimeSpan cookTime { get; set; }
        public string recipeYield { get; set; }
        public List<string> recipeIngredients { get; set; }
        public string recipeInstructions { get; set; }

        //NEW
        //"kalorien" --> nutrition (Nutritioninformation.... calories)
        public NutritionInformation recipeNutritionInfo { get; set; }
        //"vorbereitungszeit" --> prepTime (Duration)
        public TimeSpan prepTime { get; set; }
        //"keywords" --> keywords (TEXT)
        public string recipeKeywords { get; set; }
        //"kategorie" --> recipecategory  (TEXT)
        public string recipeCategory { get; set; }

        //"artkueche" --> recipecousine  (TEXT)
        public string recipeCuisinetype { get; set; }


        //"author" --> author (Organization or Person)
        public Organization author { get; set; }

        //"bewertung" --> aggregaterating (Aggregaterating)
        public AggregateRating aggregateRating { get; set; }


    }

    public class SpecialAnnouncement : Schema.NET.CreativeWork
    {
        [System.Runtime.Serialization.DataMemberAttribute(Name = "@type", Order = 1)]
        public new string Type { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "datePosted")]
        [JsonConverter(typeof(DateTimeToIso8601DateValuesJsonConverter))]
        public Values<int?, DateTime?, DateTimeOffset?> DatePosted { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "announcementLocation")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<ICivicStructure, ILocalBusiness> AnnouncementLocation { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "category")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<IPhysicalActivity, string, IThing, Uri> Category { get; set; }

        //Webcontent is missing using text

        [System.Runtime.Serialization.DataMemberAttribute(Name = "diseasePreventionInfo")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<string, Uri> DiseasePreventionInfo { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "diseaseSpreadStatistics")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<string, Uri> DiseaseSpreadStatistics { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "gettingTestedInfo")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<string, Uri> GettingTestedInfo { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "governmentBenefitsInfo")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public OneOrMany<IGovernmentService> GovernmentBenefitsInfo { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "newsUpdatesAndGuidelines")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<string, Uri> NewsUpdatesAndGuidelines { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "publicTransportClosuresInfo")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<string, Uri> PublicTransportClosuresInfo { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "quarantineGuidelines")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<string, Uri> QuarantineGuidelines { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "schoolClosuresInfo")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<string, Uri> SchoolClosuresInfo { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "travelBans")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<string, Uri> TravelBans { get; set; }

        //Missing props of CreativeWork
        [System.Runtime.Serialization.DataMemberAttribute(Name = "abstract")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public OneOrMany<string> Abstract { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "copyrightNotice")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public OneOrMany<string> CopyrightNotice { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute(Name = "sdPublisher")]
        [JsonConverter(typeof(ValuesJsonConverter))]
        public Values<IOrganization, IPerson> SdPublisher { get; set; }
    }
}
