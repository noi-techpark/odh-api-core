using DataModel;
using Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LCS
{
    public class ParseActivityData
    {
        //public static string xmldir = "C:\\VSProjects\\SuedtirolDB\\ActivityData\\xml\\";
        //public static string xmldir = "..\\..\\xml\\";
        public static CultureInfo myculture = new CultureInfo("en");

        //Get the Activity Detail Information
        public static PoiBaseInfos GetActivitiesDetailLTS(string activitytype, string rid, PoiBaseInfos hike, string ltsuser, string ltspswd, string ltsmsgpswd)
        {
            List<string> myactivitylist = new List<string>();
            myactivitylist.Add(rid);

            var myactivityrequestde = GetActivityDataLCS.GetActivityDetailRequest("de", "0", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "0", myactivitylist, "SMG", ltsmsgpswd);
            var myactivityrequestit = GetActivityDataLCS.GetActivityDetailRequest("it", "0", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "0", myactivitylist, "SMG", ltsmsgpswd);
            var myactivityrequesten = GetActivityDataLCS.GetActivityDetailRequest("en", "0", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "0", myactivitylist, "SMG", ltsmsgpswd);

            GetActivityDataLCS myactivitysearch = new GetActivityDataLCS(ltsuser, ltspswd);
            var myactivityresponsede = myactivitysearch.GetActivityDetail(myactivityrequestde);
            var myactivityresponseit = myactivitysearch.GetActivityDetail(myactivityrequestit);
            var myactivityresponseen = myactivitysearch.GetActivityDetail(myactivityrequesten);

            //If LTS has the data no more deactivate and return
            if (myactivityresponsede.Activities == null)
            {
                hike.Active = false;

                return hike;
            }

            var theactivityde = myactivityresponsede.Activities.Activity.FirstOrDefault();
            var theactivityit = myactivityresponseit.Activities.Activity.FirstOrDefault();
            var theactivityen = myactivityresponseen.Activities.Activity.FirstOrDefault();

            //TODO If Date LastChange equal to imported Date do nothing
            //if(Convert.ToDateTime(theactivityde.News.Status.LastChange) > hike.LastChange)


            string nome = "";

            if (theactivityde.Name != null)
                nome = theactivityde.Name.FirstOrDefault().InnerText;
            else
                nome = "kein Name";

            hike.Shortname = nome.Trim();
            hike.Id = theactivityde.RID;

            if (theactivityde.ID != null)
                if (theactivityde.ID.StartsWith("SMG-"))
                    hike.SmgId = theactivityde.ID.Replace("SMG-", "");

            //Highlight wird von SMG definiert
            //hike.Highlight = hike.Highlight;
            //No net in Servicemodel aktiv 
            //NEU GEAT IATZ IBER DIE AREA
            //if (theactivityde.News.Status.FavouriteSpecified)

            //    if (theactivityde.News.Status.Favourite == 2)
            //        hike.Highlight = Convert.ToBoolean(theactivityde.News.Status.Favourite);
            //    else
            //        hike.Highlight = false;
            //else
            //    hike.Highlight = false;


            //SMG ACtive übernehmen  
            hike.Active = true;
            hike.SmgActive = hike.SmgActive;


            //hike.Type = thepoide.EnumCodes.EnumCode != null ? thepoide.EnumCodes.EnumCode.FirstOrDefault().Name.FirstOrDefault().InnerText : GetPoiMainType(activitytype)["de"];
            //hike.SubType = thepoide.EnumCodes.EnumCode != null ? thepoide.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText : "";


            hike.Type = theactivityde.EnumCodes != null ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Name != null ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Name.FirstOrDefault().InnerText : GetActivityMainType(activitytype)["de"] : GetActivityMainType(activitytype)["de"];

            //Bei lift subtype level 2 fehlt komplett? ging das nicht
            hike.SubType = theactivityde.EnumCodes != null ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code != null ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").Count() > 0 ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText : "no Subtype" : "no Subtype" : "no Subtype";

            //PoiType setzen falls vorhanden
            hike.PoiType = theactivityde.EnumCodes != null ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code != null ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "2").Count() > 0 ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "2").FirstOrDefault().Name.FirstOrDefault().InnerText : "" : "" : "";

            if (activitytype == "SLOPE" || activitytype == "SKITRACK")
            {
                hike.PoiType = theactivityde.EnumCodes != null ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code != null ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "0").Count() > 0 ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "0").FirstOrDefault().Name.FirstOrDefault().InnerText : "" : "" : "";

            }


            //OBSOLETE????
            //hike.Difficulty = ""; //theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "0").Count() > 0 ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "0").FirstOrDefault().Name.FirstOrDefault().InnerText : "";

            //Altitude Total
            if (theactivityde.GeoDatas.GeoData.FirstOrDefault().Altitude != null)
            {
                hike.AltitudeDifference = theactivityde.GeoDatas.GeoData.FirstOrDefault().Altitude.Difference;
                hike.AltitudeHighestPoint = theactivityde.GeoDatas.GeoData.FirstOrDefault().Altitude.Max != null ? theactivityde.GeoDatas.GeoData.FirstOrDefault().Altitude.Max : 0;
                hike.AltitudeLowestPoint = theactivityde.GeoDatas.GeoData.FirstOrDefault().Altitude.Min != null ? theactivityde.GeoDatas.GeoData.FirstOrDefault().Altitude.Min : 0;
            }
            else
            {
                hike.AltitudeDifference = 0;
                hike.AltitudeHighestPoint = 0;
                hike.AltitudeLowestPoint = 0;
            }

            if (theactivityde.GeoDatas.GeoData.FirstOrDefault().Distance != null)
            {
                hike.DistanceLength = theactivityde.GeoDatas.GeoData.FirstOrDefault().Distance.Length;
                hike.AltitudeSumUp = theactivityde.GeoDatas.GeoData.FirstOrDefault().Distance.SumUp != null ? theactivityde.GeoDatas.GeoData.FirstOrDefault().Distance.SumUp : 0;
                hike.AltitudeSumDown = theactivityde.GeoDatas.GeoData.FirstOrDefault().Distance.SumDown != null ? theactivityde.GeoDatas.GeoData.FirstOrDefault().Distance.SumDown : 0;
            }
            else
            {
                hike.DistanceLength = 0;
                hike.AltitudeSumUp = 0;
                hike.AltitudeSumDown = 0;
            }


            if (theactivityde.GeoDatas.GeoData.FirstOrDefault().Distance != null)
            {
                if (theactivityde.GeoDatas.GeoData.FirstOrDefault().Distance.Duration != null)
                {
                    //Do wirfts an Exception iber 23 stunden
                    //var mydurationtemp = TimeSpan.Parse(theactivityde.GeoDatas.GeoData.FirstOrDefault().Distance.Duration);
                    //hike.DistanceDuration = Convert.ToDouble(mydurationtemp.Hours + "," + mydurationtemp.Minutes);

                    // seconds


                    //Ondere Meglichkeit TO TEST!!!!
                    string duration = theactivityde.GeoDatas.GeoData.FirstOrDefault().Distance.Duration;
                    var durationsplittet = duration.Split(':');
                    //DES GEAT NET!
                    TimeSpan durationts = new TimeSpan(int.Parse(durationsplittet[0]), int.Parse(durationsplittet[1]), 0);

                    //int tshours = Convert.ToInt32(durationsplittet[0]);
                    //int tsminutes = Convert.ToInt32(durationsplittet[1]);

                    //hike.DistanceDuration = Math.Round(tshours + tsminutes / 60.0, 2);
                    hike.DistanceDuration = Math.Round(durationts.TotalHours, 2);

                    //hike.DistanceDuration = Convert.ToDouble(durationsplittet[0] + "," + durationsplittet[1]);

                }
                else
                    hike.DistanceDuration = 0;
            }
            else
                hike.DistanceDuration = 0;

            hike.HasRentals = theactivityde.Features.HasRentals != null ? Convert.ToBoolean(theactivityde.Features.HasRentals) : false;
            hike.IsWithLigth = theactivityde.Features.IsWithLight != null ? Convert.ToBoolean(theactivityde.Features.IsWithLight) : false;
            hike.IsOpen = theactivityde.News.Status.IsOpen != null ? Convert.ToBoolean(theactivityde.News.Status.IsOpen) : false;
            hike.IsPrepared = theactivityde.News.Status.IsPrepared != null ? Convert.ToBoolean(theactivityde.News.Status.IsPrepared) : false;
            hike.RunToValley = theactivityde.News.RunToValley.IsPossible != null ? Convert.ToBoolean(theactivityde.News.RunToValley.IsPossible) : false;

            hike.FeetClimb = theactivityde.Features.FeetClimb != null ? Convert.ToBoolean(theactivityde.Features.FeetClimb) : false;
            hike.LiftAvailable = theactivityde.Features.LiftAvailable != null ? Convert.ToBoolean(theactivityde.Features.LiftAvailable) : false;

            //New Way Number + Right to use the way, set this info only if i get it from interface
            if (theactivityde.WayNumber != null && theactivityde.WayNumber != 0)
                hike.WayNumber = theactivityde.WayNumber;
            else
                hike.WayNumber = null;

            //LTS Internal
            //if (theactivityde.RightToUseTheWay != null && theactivityde.RightToUseTheWay != 0)
            //    hike.RightToUseTheWay = Convert.ToBoolean(theactivityde.RightToUseTheWay);
            //else
            //    hike.RightToUseTheWay = null;



            hike.BikeTransport = theactivityde.Features.BikeTransport != null ? Convert.ToBoolean(theactivityde.Features.BikeTransport) : false;

            hike.LastChange = DateTime.Now; //Convert.ToDateTime(theactivityde.News.Status.LastChange);          



            //Ratings
            if (theactivityde.Rating != null)
            {
                Ratings myrating = new Ratings();
                myrating.Difficulty = theactivityde.Rating.Difficulty != null ? theactivityde.Rating.Difficulty.ToString() : "";
                myrating.Experience = theactivityde.Rating.Experience != null ? theactivityde.Rating.Experience.ToString() : "";
                myrating.Landscape = theactivityde.Rating.Landscape != null ? theactivityde.Rating.Landscape.ToString() : "";
                myrating.Stamina = theactivityde.Rating.Stamina != null ? theactivityde.Rating.Stamina.ToString() : "";

                myrating.Technique = theactivityde.Rating.Technique != null ? theactivityde.Rating.Technique.ToString() : "";

                //Sonderfall TEchnik bei Alpinklettern, Hochtouren, Klettersteige
                foreach (var myltstag in theactivityde.Tags.Tag)
                {
                    if (myltstag.Name.FirstOrDefault().InnerText == "Alpinklettern" || myltstag.Name.FirstOrDefault().InnerText == "Hochtour")
                        myrating.Technique = theactivityde.Rating.Technique_UIAA != null ? theactivityde.Rating.Technique_UIAA : "";

                    if (myltstag.Name.FirstOrDefault().InnerText == "Klettersteig")
                        myrating.Technique = theactivityde.Rating.Technique_VF != null ? theactivityde.Rating.Technique_VF : "";

                }

                hike.Ratings = myrating;

                hike.Difficulty = theactivityde.Rating.Difficulty != null ? theactivityde.Rating.Difficulty.ToString() : "";
            }


            //ContactInfos

            //List<ContactInfos> contactinfolist = new List<ContactInfos>();

            if (theactivityde.ContactInfos.ContactInfo != null)
            {
                var parsedcontactinfode = theactivityde.ContactInfos.ContactInfo.FirstOrDefault();
                if (parsedcontactinfode != null)
                {

                    ContactInfos mycontactinfo = new ContactInfos();

                    var myadresselement = parsedcontactinfode.Addresses;

                    mycontactinfo.Address = myadresselement.Address.FirstOrDefault().AddressLine;

                    mycontactinfo.City = parsedcontactinfode.Addresses.Address.FirstOrDefault().CityName;
                    mycontactinfo.CountryCode = parsedcontactinfode.Addresses.Address.FirstOrDefault().CountryName.Code;
                    mycontactinfo.CountryName = parsedcontactinfode.Addresses.Address.FirstOrDefault().CountryName.InnerText;
                    mycontactinfo.ZipCode = parsedcontactinfode.Addresses.Address.FirstOrDefault().PostalCode;

                    mycontactinfo.Email = parsedcontactinfode.Emails.Email.FirstOrDefault().InnerText;
                    mycontactinfo.Faxnumber = "";

                    mycontactinfo.Givenname = parsedcontactinfode.Names.Name.FirstOrDefault().GivenName;
                    mycontactinfo.Surname = parsedcontactinfode.Names.Name.FirstOrDefault().Surname;
                    mycontactinfo.NamePrefix = parsedcontactinfode.Names.Name.FirstOrDefault().NamePrefix;

                    mycontactinfo.CompanyName = parsedcontactinfode.CompanyName.InnerText;

                    mycontactinfo.Language = "de";

                    //mycontactinfo.Phonenumber = parsedcontactinfode.Phones.Phone.FirstOrDefault().PhoneNumber;
                    mycontactinfo.Phonenumber = parsedcontactinfode.Phones != null ? parsedcontactinfode.Phones.Phone.Where(x => x.PhoneTechType == "1").Count() > 0 ? parsedcontactinfode.Phones.Phone.Where(x => x.PhoneTechType == "1").FirstOrDefault().PhoneNumber : "" : "";
                    mycontactinfo.Faxnumber = parsedcontactinfode.Phones != null ? parsedcontactinfode.Phones.Phone.Where(x => x.PhoneTechType == "3").Count() > 0 ? parsedcontactinfode.Phones.Phone.Where(x => x.PhoneTechType == "3").FirstOrDefault().PhoneNumber : "" : "";



                    mycontactinfo.Url = parsedcontactinfode.URLs.URL.FirstOrDefault().InnerText;

                    //contactinfolist.Add(mycontactinfo);

                    hike.ContactInfos.TryAddOrUpdate("de", mycontactinfo);
                }
            }

            if (theactivityit.ContactInfos.ContactInfo != null)
            {
                var parsedcontactinfoit = theactivityit.ContactInfos.ContactInfo.FirstOrDefault();
                if (parsedcontactinfoit != null)
                {

                    ContactInfos mycontactinfo = new ContactInfos();

                    var myadresselement = parsedcontactinfoit.Addresses;

                    mycontactinfo.Address = myadresselement.Address.FirstOrDefault().AddressLine;

                    mycontactinfo.City = parsedcontactinfoit.Addresses.Address.FirstOrDefault().CityName;
                    mycontactinfo.CountryCode = parsedcontactinfoit.Addresses.Address.FirstOrDefault().CountryName.Code;
                    mycontactinfo.CountryName = parsedcontactinfoit.Addresses.Address.FirstOrDefault().CountryName.InnerText;
                    mycontactinfo.ZipCode = parsedcontactinfoit.Addresses.Address.FirstOrDefault().PostalCode;

                    mycontactinfo.Email = parsedcontactinfoit.Emails.Email.FirstOrDefault().InnerText;
                    mycontactinfo.Faxnumber = "";

                    mycontactinfo.Givenname = parsedcontactinfoit.Names.Name.FirstOrDefault().GivenName;
                    mycontactinfo.Surname = parsedcontactinfoit.Names.Name.FirstOrDefault().Surname;
                    mycontactinfo.NamePrefix = parsedcontactinfoit.Names.Name.FirstOrDefault().NamePrefix;

                    mycontactinfo.CompanyName = parsedcontactinfoit.CompanyName.InnerText;

                    mycontactinfo.Language = "it";

                    //mycontactinfo.Phonenumber = parsedcontactinfoit.Phones.Phone.FirstOrDefault().PhoneNumber;
                    mycontactinfo.Phonenumber = parsedcontactinfoit.Phones != null ? parsedcontactinfoit.Phones.Phone.Where(x => x.PhoneTechType == "1").Count() > 0 ? parsedcontactinfoit.Phones.Phone.Where(x => x.PhoneTechType == "1").FirstOrDefault().PhoneNumber : "" : "";
                    mycontactinfo.Faxnumber = parsedcontactinfoit.Phones != null ? parsedcontactinfoit.Phones.Phone.Where(x => x.PhoneTechType == "3").Count() > 0 ? parsedcontactinfoit.Phones.Phone.Where(x => x.PhoneTechType == "3").FirstOrDefault().PhoneNumber : "" : "";



                    mycontactinfo.Url = parsedcontactinfoit.URLs.URL.FirstOrDefault().InnerText;

                    //contactinfolist.Add(mycontactinfo);
                    hike.ContactInfos.TryAddOrUpdate("it", mycontactinfo);
                }
            }

            if (theactivityen.ContactInfos.ContactInfo != null)
            {
                var parsedcontactinfoen = theactivityen.ContactInfos.ContactInfo.FirstOrDefault();
                if (parsedcontactinfoen != null)
                {

                    ContactInfos mycontactinfo = new ContactInfos();

                    var myadresselement = parsedcontactinfoen.Addresses;

                    mycontactinfo.Address = myadresselement.Address.FirstOrDefault().AddressLine;

                    mycontactinfo.City = parsedcontactinfoen.Addresses.Address.FirstOrDefault().CityName;
                    mycontactinfo.CountryCode = parsedcontactinfoen.Addresses.Address.FirstOrDefault().CountryName.Code;
                    mycontactinfo.CountryName = parsedcontactinfoen.Addresses.Address.FirstOrDefault().CountryName.InnerText;
                    mycontactinfo.ZipCode = parsedcontactinfoen.Addresses.Address.FirstOrDefault().PostalCode;

                    mycontactinfo.Email = parsedcontactinfoen.Emails.Email.FirstOrDefault().InnerText;
                    mycontactinfo.Faxnumber = "";

                    mycontactinfo.Givenname = parsedcontactinfoen.Names.Name.FirstOrDefault().GivenName;
                    mycontactinfo.Surname = parsedcontactinfoen.Names.Name.FirstOrDefault().Surname;
                    mycontactinfo.NamePrefix = parsedcontactinfoen.Names.Name.FirstOrDefault().NamePrefix;

                    mycontactinfo.CompanyName = parsedcontactinfoen.CompanyName.InnerText;

                    mycontactinfo.Language = "en";

                    //mycontactinfo.Phonenumber = parsedcontactinfoen.Phones.Phone.FirstOrDefault().PhoneNumber;
                    mycontactinfo.Phonenumber = parsedcontactinfoen.Phones != null ? parsedcontactinfoen.Phones.Phone.Where(x => x.PhoneTechType == "1").Count() > 0 ? parsedcontactinfoen.Phones.Phone.Where(x => x.PhoneTechType == "1").FirstOrDefault().PhoneNumber : "" : "";
                    mycontactinfo.Faxnumber = parsedcontactinfoen.Phones != null ? parsedcontactinfoen.Phones.Phone.Where(x => x.PhoneTechType == "3").Count() > 0 ? parsedcontactinfoen.Phones.Phone.Where(x => x.PhoneTechType == "3").FirstOrDefault().PhoneNumber : "" : "";


                    mycontactinfo.Url = parsedcontactinfoen.URLs.URL.FirstOrDefault().InnerText;

                    //contactinfolist.Add(mycontactinfo);
                    hike.ContactInfos.TryAddOrUpdate("en", mycontactinfo);
                }
            }

            //Ende ContactInfos

            //Multimedia Infos            

            List<ImageGallery> imagegallerylist = new List<ImageGallery>();

            //Bei Update alle Imagegallery Elemente welche nicht SMG Typ sind löschen.
            if (hike.ImageGallery != null)
            {
                foreach (var imggallery in hike.ImageGallery.Where(x => x.ImageSource == "SMG"))
                {
                    imagegallerylist.Add(imggallery);
                }
            }

            var parsedmultimediainfode = theactivityde.MultimediaDescriptions.MultimediaDescription.FirstOrDefault();
            if (parsedmultimediainfode != null)
            {
                if (parsedmultimediainfode.ImageItems.ImageItem != null)
                {
                    int i = 0;

                    foreach (var imageelement in parsedmultimediainfode.ImageItems.ImageItem)
                    {
                        ImageGallery myimggallery = new ImageGallery();
                        myimggallery.Height = imageelement.ImageFormat.Height;
                        myimggallery.ImageDesc.TryAddOrUpdate("de", imageelement.Description.FirstOrDefault().InnerText);
                        myimggallery.ImageSource = "LTS";
                        myimggallery.ImageName = imageelement.RID;
                        myimggallery.ImageUrl = imageelement.ImageFormat.URL.InnerText;
                        myimggallery.IsInGallery = true; // imageelement.ImageFormat.IsEnabled;                        
                        myimggallery.ListPosition = i;
                        myimggallery.Width = imageelement.ImageFormat.Width;
                        myimggallery.CopyRight = imageelement.ImageFormat.CopyrightOwner;
                        myimggallery.License = imageelement.ImageFormat.License;

                        //Kimmp oanmol mit . oanmol mit - oanmol isch Monat vorne oanmol Tog wilder kaas

                        //string applicablestart = imageelement.ImageFormat.ApplicableStart;
                        //if (!String.IsNullOrEmpty(applicablestart))
                        //{
                        //    string startmonth = applicablestart.Substring(0, 2);
                        //    string startday = applicablestart.Substring(3, 2);
                        //    myimggallery.ValidFrom = DateTime.Parse("2000-" + startmonth + "-" + startday);
                        //}
                        //else
                        //{
                        //    myimggallery.ValidFrom = new DateTime(2000, 1, 1);
                        //}

                        //string applicableend = imageelement.ImageFormat.ApplicableEnd;
                        //if (!String.IsNullOrEmpty(applicableend))
                        //{
                        //    string startmonth = applicableend.Substring(0, 2);
                        //    string startday = applicableend.Substring(3, 2);
                        //    myimggallery.ValidTo = DateTime.Parse("2000-" + startmonth + "-" + startday);
                        //}
                        //else
                        //{
                        //    myimggallery.ValidTo = new DateTime(2000, 12, 31);
                        //}


                        //myimggallery.ValidFrom = String.IsNullOrEmpty(imageelement.ImageFormat.ApplicableStart) ? new DateTime(2000, 1, 1) : DateTime.Parse(imageelement.ImageFormat.ApplicableStart + ".2000");
                        //myimggallery.ValidTo = String.IsNullOrEmpty(imageelement.ImageFormat.ApplicableEnd) ? new DateTime(2000, 12, 31) : DateTime.Parse(imageelement.ImageFormat.ApplicableEnd + ".2000");

                        imagegallerylist.Add(myimggallery);
                        i++;
                    }
                }

                var mytextitems = parsedmultimediainfode.TextItems;

                Detail mydetailde = new Detail();
                //Wenn Detail vorhanden nehmen vorhandenes und überschreibe nur Muassimer onschaugn magari mit der Extension method glöst
                if (hike.Detail != null)
                {
                    if (hike.Detail.ContainsKey("de"))
                        mydetailde = hike.Detail["de"];
                }
                //mydetailde.Title = theactivityde.Name != null ? theactivityde.Name.FirstOrDefault().InnerText.Trim() : "";
                mydetailde.Title = theactivityde.Name != null ? theactivityde.Name.FirstOrDefault().InnerText != null ? theactivityde.Name.FirstOrDefault().InnerText.Trim() : "" : "";
                //mydetailde.Header = "";
                mydetailde.Language = "de";
                //mydetailde.AdditionalText = "";
                //mydetailde.Alttext = "";
                //mydetailde.IntroText = "";                

                if (mytextitems.TextItem != null)
                {
                    foreach (var textelement in mytextitems.TextItem)
                    {
                        //Allgemeine Beschreibung
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "732512C9492340F4AB30FFB800461BE7")
                        {
                            mydetailde.BaseText = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Anfahrtstext
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "C26C826A239C47BDB773B4E4B3F27547")
                        {
                            mydetailde.GetThereText = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Wegbeschreibung
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "ECEB7021321445648CE37A9A84D64930")
                        {
                            mydetailde.AdditionalText = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                    }
                }

                //detaillist.Add(mydetailde);
                hike.Detail.TryAddOrUpdate("de", mydetailde);
            }

            //it
            var parsedmultimediainfoit = theactivityit.MultimediaDescriptions.MultimediaDescription.FirstOrDefault();
            if (parsedmultimediainfoit != null)
            {
                if (parsedmultimediainfoit.ImageItems.ImageItem != null)
                {
                    int i = 0;

                    foreach (var imageelement in parsedmultimediainfoit.ImageItems.ImageItem)
                    {
                        imagegallerylist.Where(x => x.ListPosition == i && x.ImageSource == "LTS").FirstOrDefault().ImageDesc.TryAddOrUpdate("it", imageelement.Description.FirstOrDefault().InnerText);
                        i++;
                        //ImageGallery myimggallery = hike.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault();
                        //myimggallery.ImageDesc.Add("it", imageelement.Description.FirstOrDefault().InnerText);                        

                        //myimggallery.Height = imageelement.ImageFormat.Height;                        
                        //myimggallery.ImageName = imageelement.RID;
                        //myimggallery.ImageUrl = imageelement.ImageFormat.URL.InnerText;
                        //myimggallery.IsInGallery = true;
                        //myimggallery.Language = "it";
                        //myimggallery.ListPosition = 0;
                        //myimggallery.Width = imageelement.ImageFormat.Width;


                    }
                }

                var mytextitems = parsedmultimediainfoit.TextItems;

                Detail mydetailit = new Detail();
                if (hike.Detail != null)
                {
                    if (hike.Detail.ContainsKey("it"))
                        mydetailit = hike.Detail["it"];
                }

                mydetailit.Title = theactivityit.Name != null ? theactivityit.Name.FirstOrDefault().InnerText != null ? theactivityit.Name.FirstOrDefault().InnerText.Trim() : "" : "";

                //mydetailit.Header = "";
                mydetailit.Language = "it";
                //mydetailit.AdditionalText = "";
                //mydetailit.Alttext = "";
                //mydetailit.IntroText = "";                

                if (mytextitems.TextItem != null)
                {
                    foreach (var textelement in mytextitems.TextItem)
                    {
                        //Allgemeine Beschreibung
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "732512C9492340F4AB30FFB800461BE7")
                        {
                            mydetailit.BaseText = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Anfahrtstext
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "C26C826A239C47BDB773B4E4B3F27547")
                        {
                            mydetailit.GetThereText = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Wegbeschreibung
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "ECEB7021321445648CE37A9A84D64930")
                        {
                            mydetailit.AdditionalText = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                    }
                }

                //detaillist.Add(mydetailit);
                hike.Detail.TryAddOrUpdate("it", mydetailit);
            }

            //en
            var parsedmultimediainfoen = theactivityen.MultimediaDescriptions.MultimediaDescription.FirstOrDefault();
            if (parsedmultimediainfoen != null)
            {

                if (parsedmultimediainfoen.ImageItems.ImageItem != null)
                {
                    int i = 0;
                    foreach (var imageelement in parsedmultimediainfoen.ImageItems.ImageItem)
                    {
                        imagegallerylist.Where(x => x.ListPosition == i && x.ImageSource == "LTS").FirstOrDefault().ImageDesc.TryAddOrUpdate("en", imageelement.Description.FirstOrDefault().InnerText);
                        i++;
                        //ImageGallery myimggallery = new ImageGallery();
                        //myimggallery.Height = imageelement.ImageFormat.Height;
                        //myimggallery.ImageDesc = imageelement.Description.FirstOrDefault().InnerText;
                        //myimggallery.ImageName = imageelement.RID;
                        //myimggallery.ImageUrl = imageelement.ImageFormat.URL.InnerText;
                        //myimggallery.IsInGallery = true;
                        //myimggallery.Language = "en";
                        //myimggallery.ListPosition = 0;
                        //myimggallery.Width = imageelement.ImageFormat.Width;

                        //imagegallerylist.Add(myimggallery);
                    }
                }


                var mytextitems = parsedmultimediainfoen.TextItems;

                Detail mydetailen = new Detail();
                if (hike.Detail != null)
                {
                    if (hike.Detail.ContainsKey("en"))
                        mydetailen = hike.Detail["en"];
                }

                //mydetailen.Title = theactivityen.Name != null ? theactivityen.Name.FirstOrDefault().InnerText.Trim() : "";
                mydetailen.Title = theactivityen.Name != null ? theactivityen.Name.FirstOrDefault().InnerText != null ? theactivityen.Name.FirstOrDefault().InnerText.Trim() : "" : "";
                //mydetailen.Header = "";
                mydetailen.Language = "en";
                //mydetailen.AdditionalText = "";
                //mydetailen.Alttext = "";
                //mydetailen.IntroText = "";                

                if (mytextitems.TextItem != null)
                {
                    foreach (var textelement in mytextitems.TextItem)
                    {
                        //Allgemeine Beschreibung
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "732512C9492340F4AB30FFB800461BE7")
                        {
                            mydetailen.BaseText = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Anfahrtstext
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "C26C826A239C47BDB773B4E4B3F27547")
                        {
                            mydetailen.GetThereText = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Wegbeschreibung
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "ECEB7021321445648CE37A9A84D64930")
                        {
                            mydetailen.AdditionalText = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                    }
                }
                //detaillist.Add(mydetailen);
                hike.Detail.TryAddOrUpdate("en", mydetailen);
            }

            hike.ImageGallery = imagegallerylist.ToList();
            //hike.Detail = detaillist.ToList();

            //Ende MultimediaInfos

            //AdditionalPoiInfos

            //List<AdditionalPoiInfos> myaddpoiinfoslist = new List<AdditionalPoiInfos>();

            AdditionalPoiInfos myaddpoiinfosde = new AdditionalPoiInfos();
            AdditionalPoiInfos myaddpoiinfosit = new AdditionalPoiInfos();
            AdditionalPoiInfos myaddpoiinfosen = new AdditionalPoiInfos();

            if (theactivityde.EnumCodes != null)
            {
                var myenumcodesde = theactivityde.EnumCodes.EnumCode.FirstOrDefault();

                if (myenumcodesde.Code != null)
                {
                    foreach (var enumcode in myenumcodesde.Code)
                    {
                        //if (enumcode.Level == "0")
                        //{
                        //    myaddpoiinfosde.Difficulty = enumcode.Name.FirstOrDefault().InnerText;
                        //}
                        if (enumcode.Level == "1")
                        {
                            myaddpoiinfosde.SubType = enumcode.Name.FirstOrDefault().InnerText;
                        }
                    }
                }
            }
            if (theactivityde.EnumCodes != null)
            {
                var myenumcodesit = theactivityit.EnumCodes.EnumCode.FirstOrDefault();

                if (myenumcodesit.Code != null)
                {
                    foreach (var enumcode in myenumcodesit.Code)
                    {
                        //if (enumcode.Level == "0")
                        //{


                        //    myaddpoiinfosit.Difficulty = enumcode.Name.FirstOrDefault().InnerText;
                        //}
                        if (enumcode.Level == "1")
                        {
                            myaddpoiinfosit.SubType = enumcode.Name.FirstOrDefault().InnerText;
                        }
                    }
                }
            }
            if (theactivityde.EnumCodes != null)
            {
                var myenumcodesen = theactivityen.EnumCodes.EnumCode.FirstOrDefault();

                if (myenumcodesen.Code != null)
                {
                    foreach (var enumcode in myenumcodesen.Code)
                    {
                        //if (enumcode.Level == "0")
                        //{
                        //    myaddpoiinfosen.Difficulty = enumcode.Name.FirstOrDefault().InnerText;
                        //}
                        if (enumcode.Level == "1")
                        {
                            myaddpoiinfosen.SubType = enumcode.Name.FirstOrDefault().InnerText;
                        }
                    }
                }
            }
            myaddpoiinfosde.Language = "de";
            myaddpoiinfosit.Language = "it";
            myaddpoiinfosen.Language = "en";

            myaddpoiinfosde.MainType = theactivityde.EnumCodes != null ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Name != null ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Name.FirstOrDefault().InnerText : GetActivityMainType(activitytype)["de"] : GetActivityMainType(activitytype)["de"];
            myaddpoiinfosit.MainType = theactivityit.EnumCodes != null ? theactivityit.EnumCodes.EnumCode.FirstOrDefault().Name != null ? theactivityit.EnumCodes.EnumCode.FirstOrDefault().Name.FirstOrDefault().InnerText : GetActivityMainType(activitytype)["it"] : GetActivityMainType(activitytype)["it"];
            myaddpoiinfosen.MainType = theactivityen.EnumCodes != null ? theactivityen.EnumCodes.EnumCode.FirstOrDefault().Name != null ? theactivityen.EnumCodes.EnumCode.FirstOrDefault().Name.FirstOrDefault().InnerText : GetActivityMainType(activitytype)["en"] : GetActivityMainType(activitytype)["en"];

            myaddpoiinfosde.SubType = theactivityde.EnumCodes != null ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code != null ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").Count() > 0 ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText : "" : "" : "";
            myaddpoiinfosit.SubType = theactivityit.EnumCodes != null ? theactivityit.EnumCodes.EnumCode.FirstOrDefault().Code != null ? theactivityit.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").Count() > 0 ? theactivityit.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText : "" : "" : "";
            myaddpoiinfosen.SubType = theactivityen.EnumCodes != null ? theactivityen.EnumCodes.EnumCode.FirstOrDefault().Code != null ? theactivityen.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").Count() > 0 ? theactivityen.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText : "" : "" : "";

            myaddpoiinfosde.PoiType = theactivityde.EnumCodes != null ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code != null ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "2").Count() > 0 ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "2").FirstOrDefault().Name.FirstOrDefault().InnerText : "" : "" : "";
            myaddpoiinfosit.PoiType = theactivityit.EnumCodes != null ? theactivityit.EnumCodes.EnumCode.FirstOrDefault().Code != null ? theactivityit.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "2").Count() > 0 ? theactivityit.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "2").FirstOrDefault().Name.FirstOrDefault().InnerText : "" : "" : "";
            myaddpoiinfosen.PoiType = theactivityen.EnumCodes != null ? theactivityen.EnumCodes.EnumCode.FirstOrDefault().Code != null ? theactivityen.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "2").Count() > 0 ? theactivityen.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "2").FirstOrDefault().Name.FirstOrDefault().InnerText : "" : "" : "";


            List<string> assignedltstags = new List<string>();

            //NEU gehe alle LTS Tags durch
            foreach (var ltstag in theactivityde.Tags.Tag)
            {
                assignedltstags.Add(ltstag.Name.FirstOrDefault().InnerText);
            }

            if (activitytype == "SLIDE" || activitytype == "SLOPE" || activitytype == "SKITRACK")
            {
                if (hike.SubType == "" || hike.SubType == "no Subtype")
                {
                    myaddpoiinfosde.SubType = SubTypeSpecialTranslator("de", hike.Type, assignedltstags);
                    myaddpoiinfosit.SubType = SubTypeSpecialTranslator("it", hike.Type, assignedltstags);
                    myaddpoiinfosen.SubType = SubTypeSpecialTranslator("en", hike.Type, assignedltstags);
                }
            }

            if (theactivityde.News.Novelty.Description != null)
            {
                myaddpoiinfosde.Novelty = theactivityde.News.Novelty.Description.InnerText;
            }
            if (theactivityit.News.Novelty.Description != null)
            {
                myaddpoiinfosit.Novelty = theactivityit.News.Novelty.Description.InnerText;
            }
            if (theactivityen.News.Novelty.Description != null)
            {
                myaddpoiinfosen.Novelty = theactivityen.News.Novelty.Description.InnerText;
            }

            hike.AdditionalPoiInfos.TryAddOrUpdate("de", myaddpoiinfosde);
            hike.AdditionalPoiInfos.TryAddOrUpdate("it", myaddpoiinfosit);
            hike.AdditionalPoiInfos.TryAddOrUpdate("en", myaddpoiinfosen);

            //myaddpoiinfoslist.Add(myaddpoiinfosde);
            //myaddpoiinfoslist.Add(myaddpoiinfosit);
            //myaddpoiinfoslist.Add(myaddpoiinfosen);

            //hike.AdditionalPoiInfos = myaddpoiinfoslist.ToList();

            //Ende AdditionalPoiInfos

            //hike.DistrictId = "";
            hike.TourismorganizationId = theactivityde.Owner.RID;

            hike.OwnerRid = theactivityde.Owner.RID;

            //hike.RegionId = "";

            List<GpsInfo> mygpsinfolist = new List<GpsInfo>();

            var positionslist = theactivityde.GeoDatas.GeoData.FirstOrDefault().Positions;

            if (positionslist != null)
            {
                var positions = positionslist.Position;

                if (positions != null)
                {
                    foreach (var position in positions)
                    {
                        GpsInfo mygpsinfo = new GpsInfo();

                        mygpsinfo.Gpstype = position.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().Name.FirstOrDefault().InnerText;
                        mygpsinfo.AltitudeUnitofMeasure = "m";
                        mygpsinfo.Altitude = position.Altitude;
                        mygpsinfo.Longitude = Convert.ToDouble(position.Longitude, myculture);
                        mygpsinfo.Latitude = Convert.ToDouble(position.Latitude, myculture);

                        mygpsinfolist.Add(mygpsinfo);
                    }

                    hike.GpsInfo = mygpsinfolist.ToList();
                }
            }

            List<GpsTrack> mygpstracklist = new List<GpsTrack>();

            var gpstracklist = theactivityde.GeoDatas.GeoData.FirstOrDefault().GPSTracks;

            if (gpstracklist != null)
            {
                var gpstracks = gpstracklist.GPSTrack;
                //TODO Language Specific GPXTrackdesc noch rausfieseln
                if (gpstracks != null)
                {
                    foreach (var gpstrack in gpstracks)
                    {
                        GpsTrack mygpstrack = new GpsTrack();

                        mygpstrack.Id = gpstrack.RID;
                        mygpstrack.GpxTrackUrl = gpstrack.File.URL.InnerText;

                        if (gpstrack.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText == "Übersicht")
                            mygpstrack.Type = "overview";
                        if (gpstrack.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText == "Datei zum herunterladen")
                            mygpstrack.Type = "detailed";


                        //EN und IT Info?

                        mygpstrack.GpxTrackDesc.TryAddOrUpdate("de", gpstrack.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText);

                        mygpstracklist.Add(mygpstrack);
                    }
                }
            }

            var gpstrackslistit = theactivityit.GeoDatas.GeoData.FirstOrDefault().GPSTracks;

            if (gpstrackslistit != null)
            {
                var gpstracksit = gpstrackslistit.GPSTrack;
                //TODO Language Specific GPXTrackdesc noch rausfieseln
                if (gpstracksit != null)
                {
                    foreach (var gpstrack in gpstracksit)
                    {
                        GpsTrack mygpstrack = mygpstracklist.Where(x => x.Id == gpstrack.RID).FirstOrDefault();
                        mygpstrack.GpxTrackDesc.TryAddOrUpdate("it", gpstrack.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText);
                        //mygpstracklist.Add(mygpstrack);
                    }
                }
            }

            var gpstrackslisten = theactivityen.GeoDatas.GeoData.FirstOrDefault().GPSTracks;

            if (gpstrackslisten != null)
            {
                var gpstracksen = gpstrackslisten.GPSTrack;
                //TODO Language Specific GPXTrackdesc noch rausfieseln
                if (gpstracksen != null)
                {
                    foreach (var gpstrack in gpstracksen)
                    {
                        GpsTrack mygpstrack = mygpstracklist.Where(x => x.Id == gpstrack.RID).FirstOrDefault();
                        mygpstrack.GpxTrackDesc.TryAddOrUpdate("en", gpstrack.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText);
                        //mygpstracklist.Add(mygpstrack);
                    }
                }
            }

            hike.GpsTrack = mygpstracklist.ToList();


            //Exposition

            List<string> expositionlist = new List<string>();

            var myexpositions = theactivityde.GeoDatas.GeoData.FirstOrDefault().Expositions;
            if (myexpositions != null)
            {
                foreach (var myexposition in myexpositions.Exposition)
                {
                    expositionlist.Add(myexposition.Value);
                }
            }

            hike.Exposition = expositionlist.ToList();


            List<string> arearidlist = new List<string>();

            if (theactivityde.Memberships != null)
            {
                var members = theactivityde.Memberships.Membership;

                foreach (var member in members)
                {
                    if (member.Area != null)
                    {
                        foreach (var myarea in member.Area)
                        {
                            string z = myarea.RID;
                            arearidlist.Add(z);
                        }
                    }
                }

                hike.AreaId = arearidlist.ToList();
            }

            //Neu Smg Favorit über Area
            if (hike.AreaId.Contains("EEDD568AC5B14A9DB6BED6C2592483BF"))
                hike.Highlight = true;
            else
                hike.Highlight = false;


            //Tags beibehalten. Maintyp + Subtyp als Tag übernehmen    
            List<string> currenttags = new List<string>();

            //Des tiamer mol weck
            //if(hike.SmgTags != null)
            //    currenttags = hike.SmgTags.ToList();

            string type = theactivityde.EnumCodes != null ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Name != null ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Name.FirstOrDefault().InnerText : GetActivityMainType(activitytype)["de"] : GetActivityMainType(activitytype)["de"];
            string subtype = theactivityde.EnumCodes != null ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code != null ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").Count() > 0 ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText : "no Subtype" : "no Subtype" : "no Subtype";

            if (!currenttags.Contains(type))
                currenttags.Add(type);



            //if (!currenttags.Contains(subtype))
            //    currenttags.Add(subtype);


            //NEU gehe alle LTS Tags durch
            foreach (var ltstag in theactivityde.Tags.Tag)
            {
                currenttags.Add(ltstag.Name.FirstOrDefault().InnerText);
            }

            //Achtung bei Pisten Rodelbahnen und Loipen muss ich den Subtype beibehalten der nun fehlt
            if (activitytype == "SLIDE" || activitytype == "SLOPE" || activitytype == "SKITRACK")
            {
                if (hike.SubType == "" || hike.SubType == "no Subtype")
                {
                    hike.SubType = SubTypeSpecialTranslator("de", hike.Type, assignedltstags);
                }
            }

            //SONDERFALL Tags Wenn es ein Lift ist werden die Level 1 + Level 2 Infos mit reingeschrieben
            if (activitytype == "LIFT" || activitytype == "SLOPE")
            {
                currenttags.Add(hike.SubType);

                //Lei ban Lift in PoiTyp derzuatian
                if (activitytype == "LIFT")
                    if (!String.IsNullOrEmpty(hike.PoiType))
                        currenttags.Add(hike.PoiType);
            }


            hike.SmgTags = currenttags.Distinct().ToList();

            //Fixen
            if (hike.SmgTags.Contains("no Subtype"))
            {
                hike.SmgTags.Remove("no Subtype");
            }

            //OperationSchedules muss ich mir noch genauer anschauen
            List<OperationSchedule> operationschedulelist = new List<OperationSchedule>();

            if (theactivityde.OperationSchedules != null)
            {
                if (theactivityde.OperationSchedules.OperationSchedule != null)
                {
                    foreach (var myoperationschedule in theactivityde.OperationSchedules.OperationSchedule)
                    {
                        OperationSchedule theoperationschedule = new OperationSchedule();
                        theoperationschedule.OperationscheduleName["de"] = myoperationschedule.Name != null ? myoperationschedule.Name.FirstOrDefault().InnerText : "";
                        theoperationschedule.Start = DateTime.Parse(myoperationschedule.Start);
                        theoperationschedule.Stop = DateTime.Parse(myoperationschedule.End);
                        theoperationschedule.Type = myoperationschedule.Type;

                        List<OperationScheduleTime> myopeningtimes = new List<OperationScheduleTime>();

                        if (myoperationschedule.OperationTime != null)
                        {
                            foreach (var operationscheduletime in myoperationschedule.OperationTime)
                            {
                                OperationScheduleTime mytime = new OperationScheduleTime();
                                mytime.Start = operationscheduletime.Start == null ? TimeSpan.Parse("00:00:00") : TimeSpan.Parse(operationscheduletime.Start);
                                mytime.End = operationscheduletime.End == null ? TimeSpan.Parse("23:59:00") : TimeSpan.Parse(operationscheduletime.End);
                                mytime.Monday = operationscheduletime.Mon;
                                mytime.Tuesday = operationscheduletime.Tue;
                                mytime.Wednesday = operationscheduletime.Weds;
                                mytime.Thuresday = operationscheduletime.Thur;
                                mytime.Friday = operationscheduletime.Fri;
                                mytime.Saturday = operationscheduletime.Sat;
                                mytime.Sunday = operationscheduletime.Sun;
                                mytime.Timecode = 1;
                                mytime.State = 2;

                                myopeningtimes.Add(mytime);
                            }

                            theoperationschedule.OperationScheduleTime = myopeningtimes.ToList();
                        }
                        operationschedulelist.Add(theoperationschedule);
                    }
                }
            }
            hike.OperationSchedule = operationschedulelist.ToList();
            //Ende OperationSchedule Activity

            //Status
            if (theactivityde.News.Status.IsEnabled == 1)
                hike.Active = true;
            else
                hike.Active = false;


            if (theactivityde.News.Status.CopyrightChecked == 1)
                hike.CopyrightChecked = true;
            else if (theactivityde.News.Status.CopyrightChecked == 0)
                hike.CopyrightChecked = false;
            else
                hike.CopyrightChecked = null;


            //Add The available Languages
            //Check wo überall in Details sprachknoten enthalten sind     
            var availablelanguages = hike.Detail.Select(x => x.Key).ToList();
            hike.HasLanguage = availablelanguages;

            return hike;
        }

        public static Dictionary<string, string> GetActivityMainType(string mainType)
        {
            Dictionary<string, string> maintypedict = new Dictionary<string, string>();

            switch (mainType)
            {
                case "ALPINE":
                    maintypedict.Add("de", "Berg");
                    maintypedict.Add("it", "Montagna");
                    maintypedict.Add("en", "Mountain");
                    break;
                case "BIKE":
                    maintypedict.Add("de", "Radfahren");
                    maintypedict.Add("it", "Andare in bicicletta");
                    maintypedict.Add("en", "Cycling");
                    break;
                case "CITYTOUR":
                    maintypedict.Add("de", "Stadtrundgang");
                    maintypedict.Add("it", "Tour della città");
                    maintypedict.Add("en", "City tour");
                    break;
                case "EQUESTRIANISM":
                    maintypedict.Add("de", "Pferdesport");
                    maintypedict.Add("it", "Equitazione");
                    maintypedict.Add("en", "Equestrianism");
                    break;
                case "HIKE":
                    maintypedict.Add("de", "Wandern");
                    maintypedict.Add("it", "Escursionismo");
                    maintypedict.Add("en", "Hiking");
                    break;
                case "RUNNING_FITNESS":
                    maintypedict.Add("de", "Laufen und Fitness");
                    maintypedict.Add("it", "Corsa e fitness");
                    maintypedict.Add("en", "Running and fitness");
                    break;
                case "SLOPE":
                    maintypedict.Add("de", "Piste");
                    maintypedict.Add("it", "piste");
                    maintypedict.Add("en", "slopes");
                    break;
                case "SLIDE":
                    maintypedict.Add("de", "Rodelbahnen");
                    maintypedict.Add("it", "Piste");
                    maintypedict.Add("en", "Slides");
                    break;
                case "SKITRACK":
                    maintypedict.Add("de", "Loipen");
                    maintypedict.Add("it", "Pista di fondo");
                    maintypedict.Add("en", "Cross-country ski-tracks");
                    break;
                case "LIFT":
                    maintypedict.Add("de", "Aufstiegsanlagen");
                    maintypedict.Add("it", "ascensioni");
                    maintypedict.Add("en", "lifts");
                    break;
            }

            return maintypedict;
        }

        private static string SubTypeSpecialTranslator(string lang, string maintype, List<string> ltstags)
        {
            string subtypetoreturn = "";

            if (lang == "de")
            {

                if (maintype == "Loipen")
                {
                    if (ltstags.Contains("Klassisch und Skating"))
                        return "klassisch und Freistil";
                    else if (ltstags.Contains("Skating"))
                        return "Freistil";
                    else if (ltstags.Contains("Klassisch"))
                        return "klassisch";
                    else
                        return "klassisch und Freistil";
                }
                else if (maintype == "Piste")
                {
                    if (ltstags.Contains("Rundkurs"))
                        return "Rundkurs";
                    else if (ltstags.Contains("Snowpark"))
                        return "Snowpark";
                    else if (ltstags.Contains("Weitere Pisten"))
                        return "Ski alpin";
                    else
                        return "Ski alpin";

                }
                else if (maintype == "Rodelbahnen")
                {
                    if (ltstags.Contains("Schienenrodelbahn"))
                        return "Alpin Bob";
                    else if (ltstags.Contains("Eisbahnen"))
                        return "Eisbahnen";
                    else if (ltstags.Contains("Rodelbahnen"))
                        return "Rodelbahn";
                    else if (ltstags.Contains("Schneebahnen"))
                        return "Schneebahnen";
                    else if (ltstags.Contains("Weitere Rodeln"))
                        return "Rodelbahn";
                    else
                        return "Rodelbahn";
                }
            }
            else if (lang == "it")
            {

                if (maintype == "Loipen")
                {
                    if (ltstags.Contains("Klassisch und Skating"))
                        return "Classica e stile libero";
                    else if (ltstags.Contains("Skating"))
                        return "Stile libero";
                    else if (ltstags.Contains("Klassisch"))
                        return "classica";
                    else
                        return "Classica e stile libero";
                }
                else if (maintype == "Piste")
                {
                    if (ltstags.Contains("Rundkurs"))
                        return "Giro sciistico";
                    else if (ltstags.Contains("Snowpark"))
                        return "Snowpark";
                    else if (ltstags.Contains("Weitere Pisten"))
                        return "Sci alpino";
                    else
                        return "Sci alpino";

                }
                else if (maintype == "Rodelbahnen")
                {
                    if (ltstags.Contains("Schienenrodelbahn"))
                        return "Alpin Bob";
                    else if (ltstags.Contains("Eisbahnen"))
                        return "Pista per slittini";
                    else if (ltstags.Contains("Rodelbahnen"))
                        return "Pista per slittini";
                    else if (ltstags.Contains("Schneebahnen"))
                        return "Pista per slittini";
                    else if (ltstags.Contains("Weitere Rodeln"))
                        return "Pista per slittini";
                    else
                        return "Pista per slittini";
                }
            }
            else if (lang == "en")
            {

                if (maintype == "Loipen")
                {
                    if (ltstags.Contains("Klassisch und Skating"))
                        return "klassisch und Freistil";
                    else if (ltstags.Contains("Skating"))
                        return "Freestyle";
                    else if (ltstags.Contains("Klassisch"))
                        return "classic";
                    else
                        return "classic and skating";
                }
                else if (maintype == "Piste")
                {
                    if (ltstags.Contains("Rundkurs"))
                        return "Ski alpin (tour)";
                    else if (ltstags.Contains("Snowpark"))
                        return "Snowpark";
                    else if (ltstags.Contains("Weitere Pisten"))
                        return "Ski alpin";
                    else
                        return "Ski alpin";

                }
                else if (maintype == "Rodelbahnen")
                {
                    if (ltstags.Contains("Schienenrodelbahn"))
                        return "Alpin Bob";
                    else if (ltstags.Contains("Eisbahnen"))
                        return "Coasting slide";
                    else if (ltstags.Contains("Rodelbahnen"))
                        return "Coasting slide";
                    else if (ltstags.Contains("Schneebahnen"))
                        return "Coasting slide";
                    else if (ltstags.Contains("Weitere Rodeln"))
                        return "Coasting slide";
                    else
                        return "Coasting slide";
                }
            }

            return subtypetoreturn;
        }

        ////Get the Activity Detail Information OLLERHOND NO TODO!
        //public static PoiBaseInfos GetActivitiesDetailSmgPoi(string rid, SmgPoi hike)
        //{
        //    //Hier gilt es zu definieren was alles Upgedated wird!!!!

        //    List<string> myactivitylist = new List<string>();
        //    myactivitylist.Add(rid);

        //    var myactivityrequestde = GetActivityDataLCS.GetActivityDetailRequest("de", "0", "1", "1", "1", "1", "1", "0", "1", "1", "1", "1", "1", "1", myactivitylist, "SMG");
        //    var myactivityrequestit = GetActivityDataLCS.GetActivityDetailRequest("it", "0", "1", "1", "1", "1", "1", "0", "1", "1", "1", "1", "1", "1", myactivitylist, "SMG");
        //    var myactivityrequesten = GetActivityDataLCS.GetActivityDetailRequest("en", "0", "1", "1", "1", "1", "1", "0", "1", "1", "1", "1", "1", "1", myactivitylist, "SMG");

        //    GetActivityDataLCS myactivitysearch = new GetActivityDataLCS();
        //    var myactivityresponsede = myactivitysearch.GetActivityDetail(myactivityrequestde);
        //    var myactivityresponseit = myactivitysearch.GetActivityDetail(myactivityrequestit);
        //    var myactivityresponseen = myactivitysearch.GetActivityDetail(myactivityrequesten);

        //    var theactivityde = myactivityresponsede.Activities.Activity.FirstOrDefault();
        //    var theactivityit = myactivityresponseit.Activities.Activity.FirstOrDefault();
        //    var theactivityen = myactivityresponseen.Activities.Activity.FirstOrDefault();

        //    //TODO If Date LastChange equal to imported Date do nothing
        //    //if(Convert.ToDateTime(theactivityde.News.Status.LastChange) > hike.LastChange)


        //    string nome = "";

        //    if (theactivityde.Name != null)
        //        nome = theactivityde.Name.FirstOrDefault().InnerText;
        //    else
        //        nome = "kein Name";

        //    hike.Shortname = nome.Trim();

        //    //hike.Id = theactivityde.RID;

        //    //if (theactivityde.ID != null)
        //    //    if (theactivityde.ID.StartsWith("SMG-"))
        //    //        hike.SmgId = theactivityde.ID.Replace("SMG-", "");

        //    //Highlight wird von SMG definiert
        //    //hike.Highlight = hike.Highlight;
        //    //No net in Servicemodel aktiv 
        //    //if (theactivityde.News.Status.FavouriteSpecified)
        //    //    hike.Highlight = Convert.ToBoolean(theactivityde.News.Status.Favourite);
        //    //else
        //    //    hike.Highlight = false;

        //    //SMG ACtive übernehmen  
        //    hike.Active = hike.Active;
        //    hike.SmgActive = hike.SmgActive;


        //    //hike.Type = theactivityde.EnumCodes.EnumCode.FirstOrDefault().Name.FirstOrDefault().InnerText;
        //    //Bei lift ging das nicht

        //    //hike.SubType = theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").Count() > 0 ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText : "no Subtype";

        //    //OBSOLET
        //    //hike.Difficulty = theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "0").Count() > 0 ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "0").FirstOrDefault().Name.FirstOrDefault().InnerText : "";

        //    if (theactivityde.GeoDatas.GeoData.FirstOrDefault().Altitude != null)
        //        hike.AltitudeDifference = theactivityde.GeoDatas.GeoData.FirstOrDefault().Altitude.Difference;
        //    else
        //        hike.AltitudeDifference = 0;

        //    if (theactivityde.GeoDatas.GeoData.FirstOrDefault().Distance != null)
        //        hike.DistanceLength = theactivityde.GeoDatas.GeoData.FirstOrDefault().Distance.Length;
        //    else
        //        hike.DistanceLength = 0;

        //    if (theactivityde.GeoDatas.GeoData.FirstOrDefault().Distance != null)
        //    {
        //        if (theactivityde.GeoDatas.GeoData.FirstOrDefault().Distance.Duration != null)
        //        {
        //            //Do wirfts an Exception iber 23 stunden
        //            //var mydurationtemp = TimeSpan.Parse(theactivityde.GeoDatas.GeoData.FirstOrDefault().Distance.Duration);
        //            //hike.DistanceDuration = Convert.ToDouble(mydurationtemp.Hours + "," + mydurationtemp.Minutes);


        //            //Ondere Meglichkeit
        //            string duration = theactivityde.GeoDatas.GeoData.FirstOrDefault().Distance.Duration;
        //            var durationsplittet = duration.Split(':');
        //            hike.DistanceDuration = Convert.ToDouble(durationsplittet[0] + "," + durationsplittet[1]);

        //        }
        //        else
        //            hike.DistanceDuration = 0;
        //    }
        //    else
        //        hike.DistanceDuration = 0;

        //    hike.HasRentals = Convert.ToBoolean(theactivityde.Features.HasRentals);
        //    hike.IsWithLigth = Convert.ToBoolean(theactivityde.Features.IsWithLight);
        //    hike.IsOpen = Convert.ToBoolean(theactivityde.News.Status.IsOpen);
        //    hike.IsPrepared = Convert.ToBoolean(theactivityde.News.Status.IsPrepared);
        //    hike.RunToValley = Convert.ToBoolean(theactivityde.News.RunToValley.IsPossible);

        //    hike.LastChange = DateTime.Now; //Convert.ToDateTime(theactivityde.News.Status.LastChange);


        //    //ContactInfos

        //    //List<ContactInfos> contactinfolist = new List<ContactInfos>();

        //    if (theactivityde.ContactInfos.ContactInfo != null)
        //    {
        //        var parsedcontactinfode = theactivityde.ContactInfos.ContactInfo.FirstOrDefault();
        //        if (parsedcontactinfode != null)
        //        {

        //            ContactInfos mycontactinfo = hike.ContactInfos["de"];

        //            var myadresselement = parsedcontactinfode.Addresses;

        //            mycontactinfo.Address = myadresselement.Address.FirstOrDefault().AddressLine;

        //            mycontactinfo.City = parsedcontactinfode.Addresses.Address.FirstOrDefault().CityName;
        //            mycontactinfo.CountryCode = parsedcontactinfode.Addresses.Address.FirstOrDefault().CountryName.Code;
        //            mycontactinfo.CountryName = parsedcontactinfode.Addresses.Address.FirstOrDefault().CountryName.InnerText;
        //            mycontactinfo.ZipCode = parsedcontactinfode.Addresses.Address.FirstOrDefault().PostalCode;

        //            mycontactinfo.Email = parsedcontactinfode.Emails.Email.FirstOrDefault().InnerText;
        //            mycontactinfo.Faxnumber = "";

        //            mycontactinfo.Givenname = parsedcontactinfode.Names.Name.FirstOrDefault().GivenName;
        //            mycontactinfo.Surname = parsedcontactinfode.Names.Name.FirstOrDefault().Surname;
        //            mycontactinfo.NamePrefix = parsedcontactinfode.Names.Name.FirstOrDefault().NamePrefix;

        //            mycontactinfo.CompanyName = parsedcontactinfode.CompanyName.InnerText;

        //            mycontactinfo.Language = "de";

        //            mycontactinfo.Phonenumber = parsedcontactinfode.Phones.Phone.FirstOrDefault().PhoneNumber;
        //            mycontactinfo.Url = parsedcontactinfode.URLs.URL.FirstOrDefault().InnerText;

        //            //contactinfolist.Add(mycontactinfo);

        //            hike.ContactInfos.TryAddOrUpdate("de", mycontactinfo);
        //        }
        //    }

        //    if (theactivityit.ContactInfos.ContactInfo != null)
        //    {
        //        var parsedcontactinfoit = theactivityit.ContactInfos.ContactInfo.FirstOrDefault();
        //        if (parsedcontactinfoit != null)
        //        {

        //            ContactInfos mycontactinfo = hike.ContactInfos["it"];

        //            var myadresselement = parsedcontactinfoit.Addresses;

        //            mycontactinfo.Address = myadresselement.Address.FirstOrDefault().AddressLine;

        //            mycontactinfo.City = parsedcontactinfoit.Addresses.Address.FirstOrDefault().CityName;
        //            mycontactinfo.CountryCode = parsedcontactinfoit.Addresses.Address.FirstOrDefault().CountryName.Code;
        //            mycontactinfo.CountryName = parsedcontactinfoit.Addresses.Address.FirstOrDefault().CountryName.InnerText;
        //            mycontactinfo.ZipCode = parsedcontactinfoit.Addresses.Address.FirstOrDefault().PostalCode;

        //            mycontactinfo.Email = parsedcontactinfoit.Emails.Email.FirstOrDefault().InnerText;
        //            mycontactinfo.Faxnumber = "";

        //            mycontactinfo.Givenname = parsedcontactinfoit.Names.Name.FirstOrDefault().GivenName;
        //            mycontactinfo.Surname = parsedcontactinfoit.Names.Name.FirstOrDefault().Surname;
        //            mycontactinfo.NamePrefix = parsedcontactinfoit.Names.Name.FirstOrDefault().NamePrefix;

        //            mycontactinfo.CompanyName = parsedcontactinfoit.CompanyName.InnerText;

        //            mycontactinfo.Language = "it";

        //            mycontactinfo.Phonenumber = parsedcontactinfoit.Phones.Phone.FirstOrDefault().PhoneNumber;
        //            mycontactinfo.Url = parsedcontactinfoit.URLs.URL.FirstOrDefault().InnerText;

        //            //contactinfolist.Add(mycontactinfo);
        //            hike.ContactInfos.TryAddOrUpdate("it", mycontactinfo);
        //        }
        //    }

        //    if (theactivityen.ContactInfos.ContactInfo != null)
        //    {
        //        var parsedcontactinfoen = theactivityen.ContactInfos.ContactInfo.FirstOrDefault();
        //        if (parsedcontactinfoen != null)
        //        {

        //            ContactInfos mycontactinfo = hike.ContactInfos["en"];

        //            var myadresselement = parsedcontactinfoen.Addresses;

        //            mycontactinfo.Address = myadresselement.Address.FirstOrDefault().AddressLine;

        //            mycontactinfo.City = parsedcontactinfoen.Addresses.Address.FirstOrDefault().CityName;
        //            mycontactinfo.CountryCode = parsedcontactinfoen.Addresses.Address.FirstOrDefault().CountryName.Code;
        //            mycontactinfo.CountryName = parsedcontactinfoen.Addresses.Address.FirstOrDefault().CountryName.InnerText;
        //            mycontactinfo.ZipCode = parsedcontactinfoen.Addresses.Address.FirstOrDefault().PostalCode;

        //            mycontactinfo.Email = parsedcontactinfoen.Emails.Email.FirstOrDefault().InnerText;
        //            mycontactinfo.Faxnumber = "";

        //            mycontactinfo.Givenname = parsedcontactinfoen.Names.Name.FirstOrDefault().GivenName;
        //            mycontactinfo.Surname = parsedcontactinfoen.Names.Name.FirstOrDefault().Surname;
        //            mycontactinfo.NamePrefix = parsedcontactinfoen.Names.Name.FirstOrDefault().NamePrefix;

        //            mycontactinfo.CompanyName = parsedcontactinfoen.CompanyName.InnerText;

        //            mycontactinfo.Language = "en";

        //            mycontactinfo.Phonenumber = parsedcontactinfoen.Phones.Phone.FirstOrDefault().PhoneNumber;
        //            mycontactinfo.Url = parsedcontactinfoen.URLs.URL.FirstOrDefault().InnerText;

        //            //contactinfolist.Add(mycontactinfo);
        //            hike.ContactInfos.TryAddOrUpdate("en", mycontactinfo);
        //        }
        //    }

        //    //hike.ContactInfos = contactinfolist.ToList();

        //    //Ende ContactInfos

        //    //Multimedia Infos            

        //    List<ImageGallery> imagegallerylist = new List<ImageGallery>();

        //    //Bei Update alle Imagegallery Elemente welche nicht SMG Typ sind löschen.
        //    if (hike.ImageGallery != null)
        //    {
        //        foreach (var imggallery in hike.ImageGallery.Where(x => x.ImageSource == "SMG"))
        //        {
        //            imagegallerylist.Add(imggallery);
        //        }
        //    }

        //    var parsedmultimediainfode = theactivityde.MultimediaDescriptions.MultimediaDescription.FirstOrDefault();
        //    if (parsedmultimediainfode != null)
        //    {
        //        if (parsedmultimediainfode.ImageItems.ImageItem != null)
        //        {
        //            int i = 0;

        //            foreach (var imageelement in parsedmultimediainfode.ImageItems.ImageItem)
        //            {
        //                ImageGallery myimggallery = new ImageGallery();
        //                myimggallery.Height = imageelement.ImageFormat.Height;
        //                myimggallery.ImageDesc.TryAddOrUpdate("de", imageelement.Description.FirstOrDefault().InnerText);
        //                myimggallery.ImageSource = "LTS";
        //                myimggallery.ImageName = imageelement.RID;
        //                myimggallery.ImageUrl = imageelement.ImageFormat.URL.InnerText;
        //                myimggallery.IsInGallery = true;
        //                myimggallery.ListPosition = i;
        //                myimggallery.Width = imageelement.ImageFormat.Width;
        //                //Kimmp oanmol mit . oanmol mit - oanmol isch Monat vorne oanmol Tog wilder kaas

        //                //string applicablestart = imageelement.ImageFormat.ApplicableStart;
        //                //if (!String.IsNullOrEmpty(applicablestart))
        //                //{
        //                //    string startmonth = applicablestart.Substring(0, 2);
        //                //    string startday = applicablestart.Substring(3, 2);
        //                //    myimggallery.ValidFrom = DateTime.Parse("2000-" + startmonth + "-" + startday);
        //                //}
        //                //else
        //                //{
        //                //    myimggallery.ValidFrom = new DateTime(2000, 1, 1);
        //                //}

        //                //string applicableend = imageelement.ImageFormat.ApplicableEnd;
        //                //if (!String.IsNullOrEmpty(applicableend))
        //                //{
        //                //    string startmonth = applicableend.Substring(0, 2);
        //                //    string startday = applicableend.Substring(3, 2);
        //                //    myimggallery.ValidTo = DateTime.Parse("2000-" + startmonth + "-" + startday);
        //                //}
        //                //else
        //                //{
        //                //    myimggallery.ValidTo = new DateTime(2000, 12, 31);
        //                //}


        //                //myimggallery.ValidFrom = String.IsNullOrEmpty(imageelement.ImageFormat.ApplicableStart) ? new DateTime(2000, 1, 1) : DateTime.Parse(imageelement.ImageFormat.ApplicableStart + ".2000");
        //                //myimggallery.ValidTo = String.IsNullOrEmpty(imageelement.ImageFormat.ApplicableEnd) ? new DateTime(2000, 12, 31) : DateTime.Parse(imageelement.ImageFormat.ApplicableEnd + ".2000");

        //                imagegallerylist.Add(myimggallery);
        //                i++;
        //            }
        //        }

        //        var mytextitems = parsedmultimediainfode.TextItems;

        //        Detail mydetailde = new Detail();
        //        //Wenn Detail vorhanden nehmen vorhandenes und überschreibe nur Muassimer onschaugn magari mit der Extension method glöst
        //        if (hike.Detail != null)
        //        {
        //            if (hike.Detail.ContainsKey("de"))
        //                mydetailde = hike.Detail["de"];
        //        }
        //        mydetailde.Title = theactivityde.Name != null ? theactivityde.Name.FirstOrDefault().InnerText.Trim() : "";
        //        //mydetailde.Header = "";
        //        mydetailde.Language = "de";
        //        //mydetailde.AdditionalText = "";
        //        //mydetailde.Alttext = "";
        //        //mydetailde.IntroText = "";                

        //        if (mytextitems.TextItem != null)
        //        {
        //            foreach (var textelement in mytextitems.TextItem)
        //            {
        //                //Allgemeine Beschreibung
        //                if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "732512C9492340F4AB30FFB800461BE7")
        //                {
        //                    mydetailde.BaseText = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
        //                }
        //                //Anfahrtstext
        //                if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "C26C826A239C47BDB773B4E4B3F27547")
        //                {
        //                    mydetailde.GetThereText = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
        //                }
        //            }
        //        }

        //        //detaillist.Add(mydetailde);
        //        hike.Detail.TryAddOrUpdate("de", mydetailde);
        //    }

        //    //it
        //    var parsedmultimediainfoit = theactivityit.MultimediaDescriptions.MultimediaDescription.FirstOrDefault();
        //    if (parsedmultimediainfoit != null)
        //    {
        //        if (parsedmultimediainfoit.ImageItems.ImageItem != null)
        //        {
        //            int i = 0;

        //            foreach (var imageelement in parsedmultimediainfoit.ImageItems.ImageItem)
        //            {
        //                imagegallerylist.Where(x => x.ListPosition == i && x.ImageSource == "LTS").FirstOrDefault().ImageDesc.TryAddOrUpdate("it", imageelement.Description.FirstOrDefault().InnerText);
        //                i++;
        //                //ImageGallery myimggallery = hike.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault();
        //                //myimggallery.ImageDesc.Add("it", imageelement.Description.FirstOrDefault().InnerText);                        

        //                //myimggallery.Height = imageelement.ImageFormat.Height;                        
        //                //myimggallery.ImageName = imageelement.RID;
        //                //myimggallery.ImageUrl = imageelement.ImageFormat.URL.InnerText;
        //                //myimggallery.IsInGallery = true;
        //                //myimggallery.Language = "it";
        //                //myimggallery.ListPosition = 0;
        //                //myimggallery.Width = imageelement.ImageFormat.Width;


        //            }
        //        }

        //        var mytextitems = parsedmultimediainfoit.TextItems;

        //        Detail mydetailit = new Detail();
        //        if (hike.Detail != null)
        //        {
        //            if (hike.Detail.ContainsKey("it"))
        //                mydetailit = hike.Detail["it"];
        //        }

        //        mydetailit.Title = theactivityit.Name != null ? theactivityit.Name.FirstOrDefault().InnerText.Trim() : "";
        //        //mydetailit.Header = "";
        //        mydetailit.Language = "it";
        //        //mydetailit.AdditionalText = "";
        //        //mydetailit.Alttext = "";
        //        //mydetailit.IntroText = "";                

        //        if (mytextitems.TextItem != null)
        //        {
        //            foreach (var textelement in mytextitems.TextItem)
        //            {
        //                //Allgemeine Beschreibung
        //                if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "732512C9492340F4AB30FFB800461BE7")
        //                {
        //                    mydetailit.BaseText = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
        //                }
        //                //Anfahrtstext
        //                if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "C26C826A239C47BDB773B4E4B3F27547")
        //                {
        //                    mydetailit.GetThereText = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
        //                }
        //            }
        //        }

        //        //detaillist.Add(mydetailit);
        //        hike.Detail.TryAddOrUpdate("it", mydetailit);
        //    }

        //    //en
        //    var parsedmultimediainfoen = theactivityen.MultimediaDescriptions.MultimediaDescription.FirstOrDefault();
        //    if (parsedmultimediainfoen != null)
        //    {

        //        if (parsedmultimediainfoen.ImageItems.ImageItem != null)
        //        {
        //            int i = 0;
        //            foreach (var imageelement in parsedmultimediainfoen.ImageItems.ImageItem)
        //            {
        //                imagegallerylist.Where(x => x.ListPosition == i && x.ImageSource == "LTS").FirstOrDefault().ImageDesc.TryAddOrUpdate("en", imageelement.Description.FirstOrDefault().InnerText);
        //                i++;
        //                //ImageGallery myimggallery = new ImageGallery();
        //                //myimggallery.Height = imageelement.ImageFormat.Height;
        //                //myimggallery.ImageDesc = imageelement.Description.FirstOrDefault().InnerText;
        //                //myimggallery.ImageName = imageelement.RID;
        //                //myimggallery.ImageUrl = imageelement.ImageFormat.URL.InnerText;
        //                //myimggallery.IsInGallery = true;
        //                //myimggallery.Language = "en";
        //                //myimggallery.ListPosition = 0;
        //                //myimggallery.Width = imageelement.ImageFormat.Width;

        //                //imagegallerylist.Add(myimggallery);
        //            }
        //        }


        //        var mytextitems = parsedmultimediainfoen.TextItems;

        //        Detail mydetailen = new Detail();
        //        if (hike.Detail != null)
        //        {
        //            if (hike.Detail.ContainsKey("en"))
        //                mydetailen = hike.Detail["en"];
        //        }

        //        mydetailen.Title = theactivityen.Name != null ? theactivityen.Name.FirstOrDefault().InnerText.Trim() : "";
        //        //mydetailen.Header = "";
        //        mydetailen.Language = "en";
        //        //mydetailen.AdditionalText = "";
        //        //mydetailen.Alttext = "";
        //        //mydetailen.IntroText = "";                

        //        if (mytextitems.TextItem != null)
        //        {
        //            foreach (var textelement in mytextitems.TextItem)
        //            {
        //                //Allgemeine Beschreibung
        //                if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "732512C9492340F4AB30FFB800461BE7")
        //                {
        //                    mydetailen.BaseText = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
        //                }
        //                //Anfahrtstext
        //                if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "C26C826A239C47BDB773B4E4B3F27547")
        //                {
        //                    mydetailen.GetThereText = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
        //                }
        //            }
        //        }
        //        //detaillist.Add(mydetailen);
        //        hike.Detail.TryAddOrUpdate("en", mydetailen);
        //    }

        //    hike.ImageGallery = imagegallerylist.ToList();
        //    //hike.Detail = detaillist.ToList();

        //    //Ende MultimediaInfos

        //    //AdditionalPoiInfos

        //    //List<AdditionalPoiInfos> myaddpoiinfoslist = new List<AdditionalPoiInfos>();

        //    AdditionalPoiInfos myaddpoiinfosde = hike.AdditionalPoiInfos["de"];
        //    AdditionalPoiInfos myaddpoiinfosit = hike.AdditionalPoiInfos["it"];
        //    AdditionalPoiInfos myaddpoiinfosen = hike.AdditionalPoiInfos["en"];

        //    //var myenumcodesde = theactivityde.EnumCodes.EnumCode.FirstOrDefault();

        //    //foreach (var enumcode in myenumcodesde.Code)
        //    //{
        //    //    //if (enumcode.Level == "0")
        //    //    //{
        //    //    //    myaddpoiinfosde.Difficulty = enumcode.Name.FirstOrDefault().InnerText;
        //    //    //}
        //    //    if (enumcode.Level == "1")
        //    //    {
        //    //        myaddpoiinfosde.SubType = enumcode.Name.FirstOrDefault().InnerText;
        //    //    }
        //    //}

        //    //var myenumcodesit = theactivityit.EnumCodes.EnumCode.FirstOrDefault();

        //    //foreach (var enumcode in myenumcodesit.Code)
        //    //{
        //    //    //if (enumcode.Level == "0")
        //    //    //{


        //    //    //    myaddpoiinfosit.Difficulty = enumcode.Name.FirstOrDefault().InnerText;
        //    //    //}
        //    //    if (enumcode.Level == "1")
        //    //    {
        //    //        myaddpoiinfosit.SubType = enumcode.Name.FirstOrDefault().InnerText;
        //    //    }
        //    //}

        //    //var myenumcodesen = theactivityen.EnumCodes.EnumCode.FirstOrDefault();

        //    //foreach (var enumcode in myenumcodesen.Code)
        //    //{
        //    //    //if (enumcode.Level == "0")
        //    //    //{
        //    //    //    myaddpoiinfosen.Difficulty = enumcode.Name.FirstOrDefault().InnerText;
        //    //    //}
        //    //    if (enumcode.Level == "1")
        //    //    {
        //    //        myaddpoiinfosen.SubType = enumcode.Name.FirstOrDefault().InnerText;
        //    //    }
        //    //}

        //    //myaddpoiinfosde.Language = "de";
        //    //myaddpoiinfosit.Language = "it";
        //    //myaddpoiinfosen.Language = "en";

        //    //myaddpoiinfosde.MainType = theactivityde.EnumCodes.EnumCode.FirstOrDefault().Name.FirstOrDefault().InnerText;
        //    //myaddpoiinfosit.MainType = theactivityit.EnumCodes.EnumCode.FirstOrDefault().Name.FirstOrDefault().InnerText;
        //    //myaddpoiinfosen.MainType = theactivityen.EnumCodes.EnumCode.FirstOrDefault().Name.FirstOrDefault().InnerText;

        //    //myaddpoiinfosde.SubType = theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").Count() > 0 ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText : "";
        //    //myaddpoiinfosit.SubType = theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").Count() > 0 ? theactivityit.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText : "";
        //    //myaddpoiinfosen.SubType = theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").Count() > 0 ? theactivityen.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText : "";


        //    if (theactivityde.News.Novelty.Description != null)
        //    {
        //        myaddpoiinfosde.Novelty = theactivityde.News.Novelty.Description.InnerText;
        //    }
        //    if (theactivityit.News.Novelty.Description != null)
        //    {
        //        myaddpoiinfosit.Novelty = theactivityit.News.Novelty.Description.InnerText;
        //    }
        //    if (theactivityen.News.Novelty.Description != null)
        //    {
        //        myaddpoiinfosen.Novelty = theactivityen.News.Novelty.Description.InnerText;
        //    }

        //    hike.AdditionalPoiInfos.TryAddOrUpdate("de", myaddpoiinfosde);
        //    hike.AdditionalPoiInfos.TryAddOrUpdate("it", myaddpoiinfosit);
        //    hike.AdditionalPoiInfos.TryAddOrUpdate("en", myaddpoiinfosen);

        //    //myaddpoiinfoslist.Add(myaddpoiinfosde);
        //    //myaddpoiinfoslist.Add(myaddpoiinfosit);
        //    //myaddpoiinfoslist.Add(myaddpoiinfosen);

        //    //hike.AdditionalPoiInfos = myaddpoiinfoslist.ToList();

        //    //Ende AdditionalPoiInfos

        //    //hike.DistrictId = "";

        //    //Des tiamer a nimmer weilmers onders zuaweisen wellen kennen!
        //    //hike.TourismorganizationId = theactivityde.Owner.RID;

        //    //hike.RegionId = "";

        //    List<GpsInfo> mygpsinfolist = new List<GpsInfo>();

        //    var positions = theactivityde.GeoDatas.GeoData.FirstOrDefault().Positions.Position;

        //    if (positions != null)
        //    {
        //        foreach (var position in positions)
        //        {
        //            GpsInfo mygpsinfo = new GpsInfo();

        //            mygpsinfo.Gpstype = position.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().Name.FirstOrDefault().InnerText;
        //            mygpsinfo.AltitudeUnitofMeasure = "m";
        //            mygpsinfo.Altitude = position.Altitude;
        //            mygpsinfo.Longitude = Convert.ToDouble(position.Longitude, myculture);
        //            mygpsinfo.Latitude = Convert.ToDouble(position.Latitude, myculture);

        //            mygpsinfolist.Add(mygpsinfo);
        //        }

        //        hike.GpsInfo = mygpsinfolist.ToList();
        //    }

        //    List<GpsTrack> mygpstracklist = new List<GpsTrack>();

        //    var gpstracks = theactivityde.GeoDatas.GeoData.FirstOrDefault().GPSTracks.GPSTrack;
        //    //TODO Language Specific GPXTrackdesc noch rausfieseln
        //    if (gpstracks != null)
        //    {
        //        foreach (var gpstrack in gpstracks)
        //        {
        //            GpsTrack mygpstrack = new GpsTrack();

        //            mygpstrack.Id = gpstrack.RID;
        //            mygpstrack.GpxTrackUrl = gpstrack.File.URL.InnerText;

        //            //EN und IT Info?

        //            mygpstrack.GpxTrackDesc.TryAddOrUpdate("de", gpstrack.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText);

        //            mygpstracklist.Add(mygpstrack);
        //        }
        //    }
        //    var gpstracksit = theactivityit.GeoDatas.GeoData.FirstOrDefault().GPSTracks.GPSTrack;
        //    //TODO Language Specific GPXTrackdesc noch rausfieseln
        //    if (gpstracksit != null)
        //    {
        //        foreach (var gpstrack in gpstracksit)
        //        {
        //            GpsTrack mygpstrack = mygpstracklist.Where(x => x.Id == gpstrack.RID).FirstOrDefault();
        //            mygpstrack.GpxTrackDesc.TryAddOrUpdate("it", gpstrack.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText);
        //            //mygpstracklist.Add(mygpstrack);
        //        }
        //    }

        //    var gpstracksen = theactivityen.GeoDatas.GeoData.FirstOrDefault().GPSTracks.GPSTrack;
        //    //TODO Language Specific GPXTrackdesc noch rausfieseln
        //    if (gpstracksen != null)
        //    {
        //        foreach (var gpstrack in gpstracksen)
        //        {
        //            GpsTrack mygpstrack = mygpstracklist.Where(x => x.Id == gpstrack.RID).FirstOrDefault();
        //            mygpstrack.GpxTrackDesc.TryAddOrUpdate("en", gpstrack.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText);
        //            //mygpstracklist.Add(mygpstrack);
        //        }
        //    }


        //    hike.GpsTrack = mygpstracklist.ToList();

        //    List<string> arearidlist = new List<string>();

        //    if (theactivityde.Memberships != null)
        //    {
        //        var members = theactivityde.Memberships.Membership;

        //        foreach (var member in members)
        //        {
        //            if (member.Area != null)
        //            {
        //                foreach (var myarea in member.Area)
        //                {
        //                    string z = myarea.RID;
        //                    arearidlist.Add(z);
        //                }
        //            }
        //        }

        //        hike.AreaId = arearidlist.ToList();
        //    }

        //    //Tags lassen wir wie sie sind 
        //    //List<string> currenttags = new List<string>();

        //    //if (hike.SmgTags != null)
        //    //    currenttags = hike.SmgTags.ToList();

        //    //string type = theactivityde.EnumCodes.EnumCode.FirstOrDefault().Name.FirstOrDefault().InnerText;
        //    //string subtype = theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").Count() > 0 ? theactivityde.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText : "no Subtype";

        //    //if (!currenttags.Contains(type))
        //    //    currenttags.Add(type);
        //    //if (!currenttags.Contains(subtype))
        //    //    currenttags.Add(subtype);

        //    //hike.SmgTags = currenttags.ToList();

        //    //OperationSchedules muss ich mir noch genauer anschauen
        //    List<OperationSchedule> operationschedulelist = new List<OperationSchedule>();

        //    if (theactivityde.OperationSchedules != null)
        //    {
        //        if (theactivityde.OperationSchedules.OperationSchedule != null)
        //        {
        //            foreach (var myoperationschedule in theactivityde.OperationSchedules.OperationSchedule)
        //            {
        //                OperationSchedule theoperationschedule = new OperationSchedule();
        //                theoperationschedule.OperationscheduleName["de"] = myoperationschedule.Name != null ? myoperationschedule.Name.FirstOrDefault().InnerText : "";
        //                theoperationschedule.Start = DateTime.Parse(myoperationschedule.Start);
        //                theoperationschedule.Stop = DateTime.Parse(myoperationschedule.End);
        //                theoperationschedule.Type = myoperationschedule.Type;

        //                List<OperationScheduleTime> myopeningtimes = new List<OperationScheduleTime>();

        //                if (myoperationschedule.OperationTime != null)
        //                {
        //                    foreach (var operationscheduletime in myoperationschedule.OperationTime)
        //                    {
        //                        OperationScheduleTime mytime = new OperationScheduleTime();
        //                        mytime.Start = operationscheduletime.Start == null ? TimeSpan.Parse("00:00:00") : TimeSpan.Parse(operationscheduletime.Start);
        //                        mytime.End = operationscheduletime.End == null ? TimeSpan.Parse("23:59:00") : TimeSpan.Parse(operationscheduletime.End);
        //                        mytime.Monday = operationscheduletime.Mon;
        //                        mytime.Tuesday = operationscheduletime.Tue;
        //                        mytime.Wednesday = operationscheduletime.Weds;
        //                        mytime.Thuresday = operationscheduletime.Thur;
        //                        mytime.Friday = operationscheduletime.Fri;
        //                        mytime.Saturday = operationscheduletime.Sat;
        //                        mytime.Sunday = operationscheduletime.Sun;
        //                        mytime.Timecode = 0;

        //                        myopeningtimes.Add(mytime);
        //                    }

        //                    theoperationschedule.OperationScheduleTime = myopeningtimes.ToList();
        //                }
        //                operationschedulelist.Add(theoperationschedule);
        //            }
        //        }
        //    }
        //    hike.OperationSchedule = operationschedulelist.ToList();
        //    //Ende OperationSchedule Activity

        //    //Add The available Languages
        //    //Check wo überall in Details sprachknoten enthalten sind     
        //    //var availablelanguages = hike.Detail.Select(x => x.Key).ToList();
        //    //hike.HasLanguage = availablelanguages;

        //    return hike;
        //}



    }
}
