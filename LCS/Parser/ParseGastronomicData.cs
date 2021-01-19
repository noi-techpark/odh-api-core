using DataModel;
using Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LCS
{
    public class ParseGastronomicData
    {
        //public static string xmldir = "C:\\VSProjects\\SuedtirolDB\\GastronomicData\\xml\\";
        //public static string xmldir = "..\\..\\xml\\";
        public static CultureInfo myculture = new CultureInfo("en");

        //Gets Gastronomy Detail
        public static Gastronomy GetGastronomyDetailLTS(string gastroRID, Gastronomy gastro, bool newgastro, string ltsuser, string ltspswd, string ltsmsgpswd)
        {
            var mygastrodetailrequestde = GetGastronomicDataLCS.GetGastronomicDataDetailRequestAsync(gastroRID, "de", "1", "1", "1", "1", "1", "1", "1", "1", "SMG", ltsmsgpswd);
            var mygastrodetailrequestit = GetGastronomicDataLCS.GetGastronomicDataDetailRequestAsync(gastroRID, "it", "1", "1", "1", "1", "1", "1", "1", "1", "SMG", ltsmsgpswd);
            var mygastrodetailrequesten = GetGastronomicDataLCS.GetGastronomicDataDetailRequestAsync(gastroRID, "en", "1", "1", "1", "1", "1", "1", "1", "1", "SMG", ltsmsgpswd);

            GetGastronomicDataLCS mygastrosearch = new GetGastronomicDataLCS(ltsuser, ltspswd);
            var mygastroresponsede = mygastrosearch.GetGastronomicDataDetail(mygastrodetailrequestde);
            var mygastroresponseit = mygastrosearch.GetGastronomicDataDetail(mygastrodetailrequestit);
            var mygastroresponseen = mygastrosearch.GetGastronomicDataDetail(mygastrodetailrequesten);

            //NEW if Service gives <Error Code="450" ShortText="WRONG_RID" Type="13">Gastronomic RID(s) for GastronomicDataDetail not found or not a gastronomic object.</Error>

            if (mygastroresponsede.Errors != null)
            {
                if (mygastroresponsede.Errors.Error.FirstOrDefault() != null)
                    if (mygastroresponsede.Errors.Error.FirstOrDefault().ShortText == "WRONG_RID")
                    {
                        gastro.Active = false;
                        if (newgastro)
                            gastro.SmgActive = false;
                        gastro.LastChange = DateTime.Now;

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Gastronomy " + gastro.Id + " deactivated on source, deactivating!");

                        return gastro;
                    }
            }

            var thegastrononomyde = mygastroresponsede.GastronomicData.FirstOrDefault();
            var thegastrononomyit = mygastroresponseit.GastronomicData.FirstOrDefault();
            var thegastrononomyen = mygastroresponseen.GastronomicData.FirstOrDefault();

            string nome = "no name";

            if (thegastrononomyde.ContactInfos != null)
                if (thegastrononomyde.ContactInfos.ContactInfo.FirstOrDefault().CompanyName != null)
                    nome = thegastrononomyde.ContactInfos.ContactInfo.FirstOrDefault().CompanyName.InnerText;

            gastro.Shortname = nome;


            //Gastro anhängen weil es sein kann dass Hotel auch Gastro ist
            gastro.Id = "GASTRO" + thegastrononomyde.RID;

            //SMG Active fehlt hier noch check wie man das rausfinden kann

            DateTime firstimported = DateTime.MinValue;

            if (newgastro == true)
            {
                gastro.SmgActive = true;
                gastro.Active = true;
                gastro.FirstImport = DateTime.Now;
                gastro.LastChange = DateTime.Now;
            }
            else
            {
                gastro.Active = gastro.Active;
                gastro.SmgActive = gastro.SmgActive;
                gastro.LastChange = DateTime.Now;
            }



            //Add The available Languages
            List<string> availablelangs = new List<string>() { "de", "it", "en" };
            gastro.HasLanguage = availablelangs;

            //GASTRONOMY ACTIVE INFO (NEW)
            //INFO FROM AMUTSCHLECHNER
            //IsEnabled definiert ob das Objekt aktiv ist.Vorher musste man das über die Suche prüfen.
            //LastChange: Wann wurde das Objekt das letzte Mal geändert.
            //Mode: Das Objekt soll nur dann publiziert werden wenn Mode > 0.
            gastro.Active = Convert.ToBoolean(thegastrononomyde.News.Status.IsEnabled);
            gastro.RepresentationRestriction = thegastrononomyde.News != null ? Convert.ToInt32(thegastrononomyde.News.RepresentationRestriction.Mode) : 0;

            //if (gastro.RepresentationRestriction > 0 && gastro.Active)
            //    gastro.SmgActive = true;
            //else
            //    gastro.SmgActive = false;

            //FIXES
            if (String.IsNullOrEmpty(gastro.Shortname) || gastro.Shortname == "no name")
            {
                gastro.Shortname = "no name";
                gastro.SmgActive = false;
                gastro.Active = false;
            }


            //Contactinfo DE
            if (thegastrononomyde.ContactInfos.ContactInfo != null)
            {
                var parsedcontactinfo = thegastrononomyde.ContactInfos.ContactInfo.FirstOrDefault();
                if (parsedcontactinfo != null)
                {

                    //de
                    ContactInfos mycontactinfode = new ContactInfos();

                    var myadresselement = parsedcontactinfo.Addresses;

                    if (myadresselement != null)
                    {
                        mycontactinfode.Address = myadresselement.Address.FirstOrDefault().AddressLine;

                        mycontactinfode.City = parsedcontactinfo.Addresses.Address.FirstOrDefault().CityName;
                        mycontactinfode.CountryCode = parsedcontactinfo.Addresses.Address.FirstOrDefault().CountryName.Code;
                        mycontactinfode.CountryName = parsedcontactinfo.Addresses.Address.FirstOrDefault().CountryName != null ? parsedcontactinfo.Addresses.Address.FirstOrDefault().CountryName.InnerText : "";
                        mycontactinfode.ZipCode = parsedcontactinfo.Addresses.Address.FirstOrDefault().PostalCode;

                        mycontactinfode.Email = parsedcontactinfo.Emails != null ? parsedcontactinfo.Emails.Email.FirstOrDefault().InnerText : "";
                        mycontactinfode.Faxnumber = "";

                        mycontactinfode.Givenname = parsedcontactinfo.Names != null ? parsedcontactinfo.Names.Name.FirstOrDefault().GivenName : "";
                        mycontactinfode.Surname = parsedcontactinfo.Names != null ? parsedcontactinfo.Names.Name.FirstOrDefault().Surname : "";
                        mycontactinfode.NamePrefix = parsedcontactinfo.Names != null ? parsedcontactinfo.Names.Name.FirstOrDefault().NamePrefix : "";

                        mycontactinfode.CompanyName = parsedcontactinfo.CompanyName != null ? parsedcontactinfo.CompanyName.InnerText : "";

                        mycontactinfode.Language = "de";

                        mycontactinfode.Phonenumber = parsedcontactinfo.Phones != null ? parsedcontactinfo.Phones.Phone.Where(x => x.PhoneTechType == "1").Count() > 0 ? parsedcontactinfo.Phones.Phone.Where(x => x.PhoneTechType == "1").FirstOrDefault().PhoneNumber : "" : "";
                        mycontactinfode.Faxnumber = parsedcontactinfo.Phones != null ? parsedcontactinfo.Phones.Phone.Where(x => x.PhoneTechType == "3").Count() > 0 ? parsedcontactinfo.Phones.Phone.Where(x => x.PhoneTechType == "3").FirstOrDefault().PhoneNumber : "" : "";

                        mycontactinfode.Url = parsedcontactinfo.URLs != null ? parsedcontactinfo.URLs.URL.FirstOrDefault().InnerText : "";

                        //contactinfolist.Add(mycontactinfode);
                        gastro.ContactInfos.TryAddOrUpdate("de", mycontactinfode);
                    }
                    else
                    {
                        mycontactinfode.Language = "de";
                        mycontactinfode.CompanyName = "kein name";
                        gastro.ContactInfos.TryAddOrUpdate("de", mycontactinfode);
                    }
                }
            }

            //Contactinfo IT
            if (thegastrononomyde.ContactInfos.ContactInfo != null)
            {
                var parsedcontactinfo = thegastrononomyit.ContactInfos.ContactInfo.FirstOrDefault();
                if (parsedcontactinfo != null)
                {

                    ContactInfos mycontactinfoit = new ContactInfos();

                    var myadresselement = parsedcontactinfo.Addresses;
                    if (myadresselement != null)
                    {
                        mycontactinfoit.Address = myadresselement.Address.FirstOrDefault().AddressLine;

                        mycontactinfoit.City = parsedcontactinfo.Addresses.Address.FirstOrDefault().CityName;
                        mycontactinfoit.CountryCode = parsedcontactinfo.Addresses.Address.FirstOrDefault().CountryName.Code;
                        mycontactinfoit.CountryName = parsedcontactinfo.Addresses.Address.FirstOrDefault().CountryName != null ? parsedcontactinfo.Addresses.Address.FirstOrDefault().CountryName.InnerText : "";
                        mycontactinfoit.ZipCode = parsedcontactinfo.Addresses.Address.FirstOrDefault().PostalCode;

                        mycontactinfoit.Email = parsedcontactinfo.Emails != null ? parsedcontactinfo.Emails.Email.FirstOrDefault().InnerText : "";
                        mycontactinfoit.Faxnumber = "";

                        mycontactinfoit.Givenname = parsedcontactinfo.Names != null ? parsedcontactinfo.Names.Name.FirstOrDefault().GivenName : "";
                        mycontactinfoit.Surname = parsedcontactinfo.Names != null ? parsedcontactinfo.Names.Name.FirstOrDefault().Surname : "";
                        mycontactinfoit.NamePrefix = parsedcontactinfo.Names != null ? parsedcontactinfo.Names.Name.FirstOrDefault().NamePrefix : "";

                        mycontactinfoit.CompanyName = parsedcontactinfo.CompanyName != null ? parsedcontactinfo.CompanyName.InnerText : "";

                        mycontactinfoit.Language = "it";

                        mycontactinfoit.Phonenumber = parsedcontactinfo.Phones != null ? parsedcontactinfo.Phones.Phone.Where(x => x.PhoneTechType == "1").Count() > 0 ? parsedcontactinfo.Phones.Phone.Where(x => x.PhoneTechType == "1").FirstOrDefault().PhoneNumber : "" : "";
                        mycontactinfoit.Faxnumber = parsedcontactinfo.Phones != null ? parsedcontactinfo.Phones.Phone.Where(x => x.PhoneTechType == "3").Count() > 0 ? parsedcontactinfo.Phones.Phone.Where(x => x.PhoneTechType == "3").FirstOrDefault().PhoneNumber : "" : "";



                        mycontactinfoit.Url = parsedcontactinfo.URLs != null ? parsedcontactinfo.URLs.URL.FirstOrDefault().InnerText : "";

                        //contactinfolist.Add(mycontactinfoit);

                        gastro.ContactInfos.TryAddOrUpdate("it", mycontactinfoit);
                    }
                    else
                    {
                        mycontactinfoit.Language = "it";
                        mycontactinfoit.CompanyName = "senza nome";
                        gastro.ContactInfos.TryAddOrUpdate("it", mycontactinfoit);
                    }
                }
            }

            //Contactinfo EN
            if (thegastrononomyen.ContactInfos.ContactInfo != null)
            {
                var parsedcontactinfo = thegastrononomyen.ContactInfos.ContactInfo.FirstOrDefault();
                if (parsedcontactinfo != null)
                {
                    ContactInfos mycontactinfoen = new ContactInfos();

                    var myadresselement = parsedcontactinfo.Addresses;
                    if (myadresselement != null)
                    {
                        mycontactinfoen.Address = myadresselement.Address.FirstOrDefault().AddressLine;

                        mycontactinfoen.City = parsedcontactinfo.Addresses.Address.FirstOrDefault().CityName;
                        mycontactinfoen.CountryCode = parsedcontactinfo.Addresses.Address.FirstOrDefault().CountryName.Code;
                        mycontactinfoen.CountryName = parsedcontactinfo.Addresses.Address.FirstOrDefault().CountryName != null ? parsedcontactinfo.Addresses.Address.FirstOrDefault().CountryName.InnerText : "";
                        mycontactinfoen.ZipCode = parsedcontactinfo.Addresses.Address.FirstOrDefault().PostalCode;

                        mycontactinfoen.Email = parsedcontactinfo.Emails != null ? parsedcontactinfo.Emails.Email.FirstOrDefault().InnerText : "";
                        mycontactinfoen.Faxnumber = "";

                        mycontactinfoen.Givenname = parsedcontactinfo.Names != null ? parsedcontactinfo.Names.Name.FirstOrDefault().GivenName : "";
                        mycontactinfoen.Surname = parsedcontactinfo.Names != null ? parsedcontactinfo.Names.Name.FirstOrDefault().Surname : "";
                        mycontactinfoen.NamePrefix = parsedcontactinfo.Names != null ? parsedcontactinfo.Names.Name.FirstOrDefault().NamePrefix : "";

                        mycontactinfoen.CompanyName = parsedcontactinfo.CompanyName != null ? parsedcontactinfo.CompanyName.InnerText : "";

                        mycontactinfoen.Language = "en";

                        mycontactinfoen.Phonenumber = parsedcontactinfo.Phones != null ? parsedcontactinfo.Phones.Phone.Where(x => x.PhoneTechType == "1").Count() > 0 ? parsedcontactinfo.Phones.Phone.Where(x => x.PhoneTechType == "1").FirstOrDefault().PhoneNumber : "" : "";
                        mycontactinfoen.Faxnumber = parsedcontactinfo.Phones != null ? parsedcontactinfo.Phones.Phone.Where(x => x.PhoneTechType == "3").Count() > 0 ? parsedcontactinfo.Phones.Phone.Where(x => x.PhoneTechType == "3").FirstOrDefault().PhoneNumber : "" : "";

                        mycontactinfoen.Url = parsedcontactinfo.URLs != null ? parsedcontactinfo.URLs.URL.FirstOrDefault().InnerText : "";

                        //contactinfolist.Add(mycontactinfoen);

                        gastro.ContactInfos.TryAddOrUpdate("en", mycontactinfoen);
                    }
                    else
                    {
                        mycontactinfoen.Language = "en";
                        mycontactinfoen.CompanyName = "no name";
                        gastro.ContactInfos.TryAddOrUpdate("en", mycontactinfoen);
                    }
                }
            }

            //gastro.ContactInfos = contactinfolist.ToList();

            //Ende ContactInfos

            //MultimediaInfos
            //List<Detail> detaillist = new List<Detail>();
            List<ImageGallery> imagegallerylist = new List<ImageGallery>();

            //DE Infos
            var parsedmultimediainfogeneralde = thegastrononomyde.MultimediaDescriptions.MultimediaDescription;
            if (parsedmultimediainfogeneralde != null)
            {
                var parsedmultimediainfode = parsedmultimediainfogeneralde.Where(x => x.InfoCode == "23").FirstOrDefault();

                if (parsedmultimediainfode.ImageItems.ImageItem != null)
                {
                    foreach (var imageelement in parsedmultimediainfode.ImageItems.ImageItem)
                    {
                        ImageGallery myimggallery = new ImageGallery();
                        myimggallery.Height = imageelement.ImageFormat.Height;
                        myimggallery.ImageDesc.TryAddOrUpdate("de", imageelement.Description != null ? imageelement.Description.FirstOrDefault().InnerText : "");
                        myimggallery.ImageName = imageelement.RID;
                        myimggallery.ImageUrl = imageelement.ImageFormat.URL != null ? imageelement.ImageFormat.URL.InnerText : "";
                        myimggallery.IsInGallery = true;
                        //myimggallery.Language = "de";
                        myimggallery.ListPosition = Convert.ToInt32(imageelement.ImageFormat.RecordID) - 1;
                        myimggallery.Width = imageelement.ImageFormat.Width;
                        myimggallery.ValidFrom = imageelement.ImageFormat.ApplicableStart != null ? Convert.ToDateTime("2000-" + imageelement.ImageFormat.ApplicableStart) : DateTime.MinValue;
                        myimggallery.ValidTo = imageelement.ImageFormat.ApplicableEnd != null ? Convert.ToDateTime("2000-" + imageelement.ImageFormat.ApplicableEnd) : DateTime.MaxValue;
                        myimggallery.CopyRight = imageelement.ImageFormat.CopyrightOwner;
                        myimggallery.License = imageelement.ImageFormat.License;

                        //Hack if LicenseInfo is null
                        if (myimggallery.License == null)
                            myimggallery.License = "Closed";

                        imagegallerylist.Add(myimggallery);
                    }
                }

                var parsedtextinfo = parsedmultimediainfogeneralde.Where(x => x.InfoCode == "1").FirstOrDefault();
                var parsedshorttextinfo = parsedmultimediainfogeneralde.Where(x => x.InfoCode == "17").FirstOrDefault();

                Detail mydetailde = new Detail();

                //Titel
                mydetailde.Title = thegastrononomyde.ContactInfos.ContactInfo.FirstOrDefault().CompanyName != null ? thegastrononomyde.ContactInfos.ContactInfo.FirstOrDefault().CompanyName.InnerText : "no name";
                mydetailde.Header = "";
                mydetailde.Language = "de";
                mydetailde.AdditionalText = "";
                //mydetailde.MainImage = "";
                mydetailde.GetThereText = "";
                if (parsedtextinfo != null)
                    mydetailde.BaseText = parsedtextinfo.TextItems.TextItem.FirstOrDefault().Description != null ? parsedtextinfo.TextItems.TextItem.FirstOrDefault().Description.FirstOrDefault().InnerText : "";
                if (parsedshorttextinfo != null)
                    mydetailde.IntroText = parsedshorttextinfo.TextItems.TextItem.FirstOrDefault().Description != null ? parsedshorttextinfo.TextItems.TextItem.FirstOrDefault().Description.FirstOrDefault().InnerText : "";

                //detaillist.Add(mydetailde);

                gastro.Detail.TryAddOrUpdate("de", mydetailde);
            }

            //IT Infos
            var parsedmultimediainfogeneralit = thegastrononomyit.MultimediaDescriptions.MultimediaDescription;
            if (parsedmultimediainfogeneralit != null)
            {
                var parsedmultimediainfoit = parsedmultimediainfogeneralit.Where(x => x.InfoCode == "23").FirstOrDefault();

                if (parsedmultimediainfoit.ImageItems.ImageItem != null)
                {
                    foreach (var imageelement in parsedmultimediainfoit.ImageItems.ImageItem)
                    {
                        imagegallerylist.Where(x => x.ListPosition == (Convert.ToInt32(imageelement.ImageFormat.RecordID) - 1)).FirstOrDefault().ImageDesc.TryAddOrUpdate("it", imageelement.Description != null ? imageelement.Description.FirstOrDefault().InnerText : "");

                        //ImageGallery myimggallery = new ImageGallery();
                        //myimggallery.Height = imageelement.ImageFormat.Height;
                        //myimggallery.ImageDesc = imageelement.Description != null ? imageelement.Description.FirstOrDefault().InnerText : "";
                        //myimggallery.ImageName = imageelement.RID;
                        //myimggallery.ImageUrl = imageelement.ImageFormat.URL != null ? imageelement.ImageFormat.URL.InnerText : "";
                        //myimggallery.IsInGallery = true;
                        //myimggallery.Language = "it";
                        //myimggallery.ListPosition = Convert.ToInt32(imageelement.ImageFormat.RecordID) - 1;
                        //myimggallery.Width = imageelement.ImageFormat.Width;

                        //imagegallerylist.Add(myimggallery);
                    }
                }

                var parsedtextinfo = parsedmultimediainfogeneralit.Where(x => x.InfoCode == "1").FirstOrDefault();
                var parsedshorttextinfo = parsedmultimediainfogeneralit.Where(x => x.InfoCode == "17").FirstOrDefault();

                Detail mydetailit = new Detail();

                mydetailit.Title = thegastrononomyit.ContactInfos.ContactInfo.FirstOrDefault().CompanyName != null ? thegastrononomyit.ContactInfos.ContactInfo.FirstOrDefault().CompanyName.InnerText : "no name"; ;
                mydetailit.Header = "";
                mydetailit.Language = "it";
                mydetailit.AdditionalText = "";
                //mydetailit.MainImage = "";
                mydetailit.GetThereText = "";
                if (parsedtextinfo != null)
                    mydetailit.BaseText = parsedtextinfo.TextItems.TextItem.FirstOrDefault().Description != null ? parsedtextinfo.TextItems.TextItem.FirstOrDefault().Description.FirstOrDefault().InnerText : "";
                if (parsedshorttextinfo != null)
                    mydetailit.IntroText = parsedshorttextinfo.TextItems.TextItem.FirstOrDefault().Description != null ? parsedshorttextinfo.TextItems.TextItem.FirstOrDefault().Description.FirstOrDefault().InnerText : "";

                //detaillist.Add(mydetailit);

                gastro.Detail.TryAddOrUpdate("it", mydetailit);
            }

            //en
            //DE Infos
            var parsedmultimediainfogeneralen = thegastrononomyen.MultimediaDescriptions.MultimediaDescription;
            if (parsedmultimediainfogeneralen != null)
            {
                var parsedmultimediainfoen = parsedmultimediainfogeneralen.Where(x => x.InfoCode == "23").FirstOrDefault();

                if (parsedmultimediainfoen.ImageItems.ImageItem != null)
                {
                    foreach (var imageelement in parsedmultimediainfoen.ImageItems.ImageItem)
                    {
                        imagegallerylist.Where(x => x.ListPosition == (Convert.ToInt32(imageelement.ImageFormat.RecordID) - 1)).FirstOrDefault().ImageDesc.TryAddOrUpdate("en", imageelement.Description != null ? imageelement.Description.FirstOrDefault().InnerText : "");
                        //ImageGallery myimggallery = new ImageGallery();
                        //myimggallery.Height = imageelement.ImageFormat.Height;
                        //myimggallery.ImageDesc = imageelement.Description != null ? imageelement.Description.FirstOrDefault().InnerText : "";
                        //myimggallery.ImageName = imageelement.RID;
                        //myimggallery.ImageUrl = imageelement.ImageFormat.URL != null ? imageelement.ImageFormat.URL.InnerText : "";
                        //myimggallery.IsInGallery = true;
                        //myimggallery.Language = "en";
                        //myimggallery.ListPosition = Convert.ToInt32(imageelement.ImageFormat.RecordID) - 1;
                        //myimggallery.Width = imageelement.ImageFormat.Width;

                        //imagegallerylist.Add(myimggallery);
                    }
                }

                var parsedtextinfo = parsedmultimediainfogeneralen.Where(x => x.InfoCode == "1").FirstOrDefault();
                var parsedshorttextinfo = parsedmultimediainfogeneralen.Where(x => x.InfoCode == "17").FirstOrDefault();

                Detail mydetailen = new Detail();

                mydetailen.Title = thegastrononomyen.ContactInfos.ContactInfo.FirstOrDefault().CompanyName != null ? thegastrononomyen.ContactInfos.ContactInfo.FirstOrDefault().CompanyName.InnerText : "no name"; ;
                mydetailen.Header = "";
                mydetailen.Language = "en";
                mydetailen.AdditionalText = "";
                //mydetailen.MainImage = "";
                mydetailen.GetThereText = "";
                if (parsedtextinfo != null)
                    mydetailen.BaseText = parsedtextinfo.TextItems.TextItem.FirstOrDefault().Description != null ? parsedtextinfo.TextItems.TextItem.FirstOrDefault().Description.FirstOrDefault().InnerText : "";
                if (parsedshorttextinfo != null)
                    mydetailen.IntroText = parsedshorttextinfo.TextItems.TextItem.FirstOrDefault().Description != null ? parsedshorttextinfo.TextItems.TextItem.FirstOrDefault().Description.FirstOrDefault().InnerText : "";

                //detaillist.Add(mydetailen);

                gastro.Detail.TryAddOrUpdate("en", mydetailen);
            }


            gastro.ImageGallery = imagegallerylist.ToList();
            //gastro.Detail = detaillist.ToList();

            //Ende Multimediainfos


            //Position Element

            var position = thegastrononomyde.Position;

            if (position != null)
            {
                gastro.Gpstype = "GPSCenter";
                gastro.AltitudeUnitofMeasure = "m";
                gastro.Altitude = position.Altitude;
                gastro.Longitude = Convert.ToDouble(position.Longitude, myculture);
                gastro.Latitude = Convert.ToDouble(position.Latitude, myculture);
            }

            //Ende Position


            //District Element
            //District fehlt in Objektmodell
            gastro.DistrictId = thegastrononomyde.District.RID;
            //Ende District Element
            //OperationSchedule Element


            List<OperationSchedule> operationschedulelist = new List<OperationSchedule>();

            var operationschedules = thegastrononomyde.OperationSchedules;
            if (operationschedules != null)
            {
                foreach (var myoperationschedule in operationschedules.OperationSchedule)
                {
                    OperationSchedule theoperationschedule = new OperationSchedule();
                    theoperationschedule.OperationscheduleName["de"] = myoperationschedule.Name != null ? myoperationschedule.Name.FirstOrDefault().InnerText : "";
                    theoperationschedule.Start = DateTime.Parse(myoperationschedule.Start);
                    theoperationschedule.Stop = DateTime.Parse(myoperationschedule.End);
                    theoperationschedule.Type = myoperationschedule.Type == null ? "1" : myoperationschedule.Type;

                    if (myoperationschedule.OperationTime != null)
                    {
                        List<OperationScheduleTime> operationscheduletimelist = new List<OperationScheduleTime>();
                        foreach (var timeschedule in myoperationschedule.OperationTime)
                        {
                            OperationScheduleTime myopscheduletime = new OperationScheduleTime();
                            myopscheduletime.Start = timeschedule.Start == null ? TimeSpan.Parse("00:00:00") : TimeSpan.Parse(timeschedule.Start);
                            myopscheduletime.End = timeschedule.End == null ? TimeSpan.Parse("23:59:00") : TimeSpan.Parse(timeschedule.End);
                            myopscheduletime.Monday = timeschedule.Mon;
                            myopscheduletime.Tuesday = timeschedule.Tue;
                            myopscheduletime.Wednesday = timeschedule.Weds;
                            myopscheduletime.Thuresday = timeschedule.Thur;
                            myopscheduletime.Friday = timeschedule.Fri;
                            myopscheduletime.Saturday = timeschedule.Sat;
                            myopscheduletime.Sunday = timeschedule.Sun;
                            myopscheduletime.State = timeschedule.State;
                            myopscheduletime.Timecode = timeschedule.OperationTimeCode;

                            operationscheduletimelist.Add(myopscheduletime);
                        }

                        theoperationschedule.OperationScheduleTime = operationscheduletimelist.ToList();
                    }
                    operationschedulelist.Add(theoperationschedule);
                }
            }

            gastro.OperationSchedule = operationschedulelist.ToList();
            //Ende OperationSchedule Element

            //Facilities Element

            List<Facilities> facilitieslist = new List<Facilities>();

            var facilitiesde = thegastrononomyde.Facilities;

            if (thegastrononomyde.Facilities != null)
            {
                foreach (var facility in facilitiesde.Facility)
                {
                    Facilities myfac = new Facilities();
                    myfac.Id = facility.RID;
                    myfac.Shortname = facility.FacilityName.FirstOrDefault().InnerText;
                    //myfac.Language = "de";

                    facilitieslist.Add(myfac);
                }
                //var facilitiesit = thegastrononomyit.Facilities;
                //foreach (var facility in facilitiesit.Facility)
                //{
                //    SuedtirolDB.Facilities myfac = new Facilities();
                //    myfac.Id = facility.RID;
                //    myfac.Shortname = facility.FacilityName.FirstOrDefault().InnerText;
                //    myfac.Language = "it";

                //    facilitieslist.Add(myfac);
                //}
                //var facilitiesen = thegastrononomyen.Facilities;
                //foreach (var facility in facilitiesen.Facility)
                //{
                //    SuedtirolDB.Facilities myfac = new Facilities();
                //    myfac.Id = facility.RID;
                //    myfac.Shortname = facility.FacilityName.FirstOrDefault().InnerText;
                //    myfac.Language = "en";

                //    facilitieslist.Add(myfac);
                //}                
            }
            //Ende Facilities Element
            gastro.Facilities = facilitieslist.ToList();

            //CategoryCodes Element

            List<CategoryCodes> categorycodeslist = new List<CategoryCodes>();

            var categorycodesde = thegastrononomyde.CategoryCodes;

            if (categorycodesde != null)
            {
                foreach (var catcode in categorycodesde.GastronomicCategory)
                {
                    CategoryCodes mycatcode = new CategoryCodes();
                    mycatcode.Id = catcode.RID;
                    mycatcode.Shortname = catcode.CategoryName.FirstOrDefault().InnerText;
                    //mycatcode.Language = "de";

                    categorycodeslist.Add(mycatcode);
                }

                //var categorycodesit = thegastrononomyit.CategoryCodes;
                //foreach (var catcode in categorycodesit.GastronomicCategory)
                //{
                //    SuedtirolDB.CategoryCodes mycatcode = new CategoryCodes();
                //    mycatcode.Id = catcode.RID;
                //    mycatcode.Shortname = catcode.CategoryName.FirstOrDefault().InnerText;
                //    mycatcode.Language = "it";

                //    categorycodeslist.Add(mycatcode);
                //}

                //var categorycodesen = thegastrononomyen.CategoryCodes;
                //foreach (var catcode in categorycodesen.GastronomicCategory)
                //{
                //    SuedtirolDB.CategoryCodes mycatcode = new CategoryCodes();
                //    mycatcode.Id = catcode.RID;
                //    mycatcode.Shortname = catcode.CategoryName.FirstOrDefault().InnerText;
                //    mycatcode.Language = "en";

                //    categorycodeslist.Add(mycatcode);
                //}                
            }
            gastro.CategoryCodes = categorycodeslist.ToList();
            //Ende CategoryCodes Element


            //Dishrates Element

            List<DishRates> dishrateslist = new List<DishRates>();

            var dishratesde = thegastrononomyde.DishRates;

            if (dishratesde != null)
            {
                foreach (var dishrate in dishratesde.DishRate)
                {
                    DishRates mydishrate = new DishRates();
                    mydishrate.Id = dishrate.DishCodeRID;
                    mydishrate.Shortname = dishrate.DishName != null ? dishrate.DishName.FirstOrDefault().InnerText : "";
                    mydishrate.CurrencyCode = dishrate.CurrencyCode;
                    mydishrate.MaxAmount = dishrate.MaxAmount;
                    mydishrate.MinAmount = dishrate.MinAmount;
                    //mydishrate.Language = "de";

                    dishrateslist.Add(mydishrate);
                }

                //var dishratesit = thegastrononomyit.DishRates;
                //foreach (var dishrate in dishratesit.DishRate)
                //{
                //    SuedtirolDB.DishRates mydishrate = new DishRates();
                //    mydishrate.Id = dishrate.DishCodeRID;
                //    mydishrate.Shortname = dishrate.DishName != null ? dishrate.DishName.FirstOrDefault().InnerText : "";
                //    mydishrate.CurrencyCode = dishrate.CurrencyCode;
                //    mydishrate.MaxAmount = dishrate.MaxAmount;
                //    mydishrate.MinAmount = dishrate.MinAmount;
                //    mydishrate.Language = "it";

                //    dishrateslist.Add(mydishrate);
                //}

                //var dishratesen = thegastrononomyen.DishRates;
                //foreach (var dishrate in dishratesen.DishRate)
                //{
                //    SuedtirolDB.DishRates mydishrate = new DishRates();
                //    mydishrate.Id = dishrate.DishCodeRID;
                //    mydishrate.Shortname = dishrate.DishName != null ? dishrate.DishName.FirstOrDefault().InnerText : "";
                //    mydishrate.CurrencyCode = dishrate.CurrencyCode;
                //    mydishrate.MaxAmount = dishrate.MaxAmount;
                //    mydishrate.MinAmount = dishrate.MinAmount;
                //    mydishrate.Language = "en";

                //    dishrateslist.Add(mydishrate);
                //}                
            }
            gastro.DishRates = dishrateslist.ToList();
            //Ende Dishrates Element

            //CapacityCeremony Element

            List<CapacityCeremony> capacitycerlist = new List<CapacityCeremony>();

            var capacityceremonyde = thegastrononomyde.CapacityCeremonies;

            if (capacityceremonyde.CapacityCeremony != null)
            {
                foreach (var capceremony in capacityceremonyde.CapacityCeremony)
                {
                    CapacityCeremony mycapcer = new CapacityCeremony();
                    mycapcer.Id = capceremony.CeremonyCodeRID;
                    mycapcer.Shortname = capceremony.CeremonyName != null ? capceremony.CeremonyName.FirstOrDefault().InnerText : "";
                    mycapcer.MaxSeatingCapacity = capceremony.MaxSeatingCapacity;
                    //mycapcer.Language = "de";
                    capacitycerlist.Add(mycapcer);
                }
                //var capacityceremonyit = thegastrononomyit.CapacityCeremonies;
                //foreach (var capceremony in capacityceremonyit.CapacityCeremony)
                //{
                //    SuedtirolDB.CapacityCeremony mycapcer = new CapacityCeremony();
                //    mycapcer.Id = capceremony.CeremonyCodeRID;
                //    mycapcer.Shortname = capceremony.CeremonyName != null ? capceremony.CeremonyName.FirstOrDefault().InnerText : "";
                //    mycapcer.MaxSeatingCapacity = capceremony.MaxSeatingCapacity;
                //    mycapcer.Language = "it";
                //    capacitycerlist.Add(mycapcer);
                //}
                //var capacityceremonyen = thegastrononomyen.CapacityCeremonies;
                //foreach (var capceremony in capacityceremonyen.CapacityCeremony)
                //{
                //    SuedtirolDB.CapacityCeremony mycapcer = new CapacityCeremony();
                //    mycapcer.Id = capceremony.CeremonyCodeRID;
                //    mycapcer.Shortname = capceremony.CeremonyName != null ? capceremony.CeremonyName.FirstOrDefault().InnerText : "";
                //    mycapcer.MaxSeatingCapacity = capceremony.MaxSeatingCapacity;
                //    mycapcer.Language = "en";
                //    capacitycerlist.Add(mycapcer);
                //}                
            }
            gastro.CapacityCeremony = capacitycerlist.ToList();

            gastro.MaxSeatingCapacity = thegastrononomyde.CapacityCeremonies.MaxSeatingCapacity;

            ////Setting SEO Infos directly on Gastro Object
            //if (gastro.Detail.ContainsKey("de"))
            //{
            //    string city = GastroHelper.GetCityForGastroSeo("de", gastro);

            //    gastro.Detail["de"].MetaTitle = gastro.Detail["de"].Title + " • " + city + " (Südtirol)";
            //    gastro.Detail["de"].MetaDesc = "Kontakt •  Reservierung •  Öffnungszeiten → " + gastro.Detail["de"].Title + ", " + city + ". Hier finden Feinschmecker das passende Restaurant, Cafe, Almhütte, uvm.";
            //}
            //if (gastro.Detail.ContainsKey("it"))
            //{
            //    string city = GastroHelper.GetCityForGastroSeo("it", gastro);

            //    gastro.Detail["it"].MetaTitle = gastro.Detail["it"].Title + " • " + city + " (Alto Adige)";
            //    gastro.Detail["it"].MetaDesc = "Contatto • prenotazione • orari d'apertura → " + gastro.Detail["it"].Title + ", " + city + ". Il posto giusto per i buongustai: ristorante, cafè, baita, e tanto altro.";
            //}
            //if (gastro.Detail.ContainsKey("en"))
            //{
            //    string city = GastroHelper.GetCityForGastroSeo("en", gastro);

            //    gastro.Detail["en"].MetaTitle = gastro.Detail["en"].Title + " • " + city + " (South Tyrol)";
            //    gastro.Detail["en"].MetaDesc = "•  Contact •  reservation •  opening times →  " + gastro.Detail["en"].Title + ". Find the perfect restaurant, cafe, alpine chalet in South Tyrol.";
            //}

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Gastronomy " + gastro.Id + " added");

            return gastro;
        }

        //Gets the Gastronomy List
        public static void GetGastronomyListLTS(string destinationpath, string ltsuser, string ltspswd, string ltsmsgpswd)
        {
            Console.WriteLine("Get Gastronomy RIDs");

            XDocument mygastrolist = new XDocument();
            XElement mygastro = new XElement("Gastronomies");

            var mygastrorequest = GetGastronomicDataLCS.GetGastronomicDataSearchRequestAsync("", "1", "25", "de", "1", "0", "0", "0", "0", "0", "", "0", "0", "0", new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), "SMG", ltsmsgpswd);

            GetGastronomicDataLCS mygastrosearch = new GetGastronomicDataLCS(ltsuser, ltspswd);

            var mygastroresponse = mygastrosearch.GetGastronomicDataSearch(mygastrorequest);
            mygastro.Add(GetGastroRid(mygastroresponse).ToList());

            string resultrid = mygastroresponse.Result.RID;
            int pages = mygastroresponse.Paging.PagesQty;

            for (int i = 2; i <= pages; i++)
            {
                mygastrorequest = GetGastronomicDataLCS.GetGastronomicDataSearchRequestAsync(resultrid, i.ToString(), "25", "de", "1", "0", "0", "0", "0", "0", "", "0", "0", "0", new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), "SMG", ltsmsgpswd);

                mygastroresponse = mygastrosearch.GetGastronomicDataSearch(mygastrorequest);
                mygastro.Add(GetGastroRid(mygastroresponse).ToList());
            }

            string destinationdir = "";

            if (destinationpath != "")
                destinationdir = destinationpath;

            mygastrolist.Add(mygastro);
            mygastrolist.Save(destinationdir + "GastronomyActiveListLCS.xml");

            Console.WriteLine("Gastronomy List generated");
        }

        //Gets the Gastronomy List
        public static void GetGastronomyListLTS(string destinationpath, List<string> tourismorganizations, string ltsuser, string ltspswd, string ltsmsgpswd)
        {
            Console.WriteLine("Get Gastronomy RIDs");

            XDocument mygastrolist = new XDocument();
            XElement mygastro = new XElement("Gastronomies");

            var mygastrorequest = GetGastronomicDataLCS.GetGastronomicDataSearchRequestAsync("", "1", "25", "de", "1", "0", "0", "0", "0", "0", "", "0", "0", "0", new List<string>(), new List<string>(), tourismorganizations, new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), "SMG", ltsmsgpswd);

            GetGastronomicDataLCS mygastrosearch = new GetGastronomicDataLCS(ltsuser, ltspswd);

            var mygastroresponse = mygastrosearch.GetGastronomicDataSearch(mygastrorequest);
            mygastro.Add(GetGastroRid(mygastroresponse).ToList());

            string resultrid = mygastroresponse.Result.RID;
            int pages = mygastroresponse.Paging.PagesQty;

            for (int i = 2; i <= pages; i++)
            {
                mygastrorequest = GetGastronomicDataLCS.GetGastronomicDataSearchRequestAsync(resultrid, i.ToString(), "25", "de", "1", "0", "0", "0", "0", "0", "", "0", "0", "0", new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), "SMG", ltsmsgpswd);

                mygastroresponse = mygastrosearch.GetGastronomicDataSearch(mygastrorequest);
                mygastro.Add(GetGastroRid(mygastroresponse).ToList());
            }

            string destinationdir = "";

            if (destinationpath != "")
                destinationdir = destinationpath;

            mygastrolist.Add(mygastro);
            mygastrolist.Save(destinationdir + "GastronomyListTVs.xml");

            Console.WriteLine("Gastronomy List generated");
        }

        //Gets the RID List Gastronomies
        private static List<XElement> GetGastroRid(ServiceReferenceLCS.GastronomicDataSearchRS myresult)
        {
            List<XElement> mylist = new List<XElement>();

            foreach (var z in myresult.GastronomicData)
            {
                XElement mygastronomy = new XElement("Gastronomy");
                mygastronomy.Add(new XAttribute("RID", z.RID));

                mylist.Add(mygastronomy);
            }

            return mylist;
        }

        //Gibs leider nicht
        //public static void GetGastronomyChangedLTS(string destinationpath)
        //{
        //    Console.WriteLine("Get Gastronomy RIDs of changed Gastronomies");

        //    XDocument mygastrolist = new XDocument();
        //    XElement mygastro = new XElement("Gastronomies");

        //    var mygastrorequest = GetGastronomicDataLCS.GetGastronomicDataSearchRequestAsync("", "1", "25", "de", "1", "0", "0", "0", "0", "0", "", "0", "0", "0", new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), "SMG");

        //    GetGastronomicDataLCS mygastrosearch = new GetGastronomicDataLCS();

        //    var mygastroresponse = mygastrosearch.GetGastronomicDataSearch(mygastrorequest);
        //    mygastro.Add(GetGastroRid(mygastroresponse).ToList());

        //    string resultrid = mygastroresponse.Result.RID;
        //    int pages = mygastroresponse.Paging.PagesQty;

        //    for (int i = 2; i <= pages; i++)
        //    {
        //        mygastrorequest = GetGastronomicDataLCS.GetGastronomicDataSearchRequestAsync(resultrid, i.ToString(), "25", "de", "1", "0", "0", "0", "0", "0", "", "0", "0", "0", new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), "SMG");

        //        mygastroresponse = mygastrosearch.GetGastronomicDataSearch(mygastrorequest);
        //        mygastro.Add(GetGastroRid(mygastroresponse).ToList());
        //    }

        //    mygastrolist.Add(mygastro);
        //    mygastrolist.Save(destinationpath + "GastronomyList.xml");

        //    Console.WriteLine("Gastronomy List generated");
        //}

    }
}

