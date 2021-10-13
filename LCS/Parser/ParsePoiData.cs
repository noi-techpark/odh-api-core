using DataModel;
using Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCS.Parser
{
    public class ParsePoiData
    {
        public static CultureInfo myculture = new CultureInfo("en");

        //Get the POI Detail Information //string activitytype
        public static LTSPoi GetPoiDetailLTSNEW(string rid, LTSPoi hike, string ltsuser, string ltspswd, string ltsmsgpswd)
        {

            List<string> myactivitylist = new List<string>();
            myactivitylist.Add(rid);

            var mypoirequestde = GetPoiDataLCS.GetPoiDetailRequest("de", "0", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", myactivitylist, "SMG", ltsmsgpswd);
            var mypoirequestit = GetPoiDataLCS.GetPoiDetailRequest("it", "0", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", myactivitylist, "SMG", ltsmsgpswd);
            var mypoirequesten = GetPoiDataLCS.GetPoiDetailRequest("en", "0", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", myactivitylist, "SMG", ltsmsgpswd);

            GetPoiDataLCS mypoisearch = new GetPoiDataLCS(ltsuser, ltspswd);
            var myactivityresponsede = mypoisearch.GetPoiDetail(mypoirequestde);
            var myactivityresponseit = mypoisearch.GetPoiDetail(mypoirequestit);
            var myactivityresponseen = mypoisearch.GetPoiDetail(mypoirequesten);

            //If LTS has the data no more deactivate and return
            if (myactivityresponsede.POIs == null)
            {
                hike.Active = false;

                return hike;
            }

            var thepoide = myactivityresponsede.POIs.POI.FirstOrDefault();
            var thepoiit = myactivityresponseit.POIs.POI.FirstOrDefault();
            var thepoien = myactivityresponseen.POIs.POI.FirstOrDefault();


            string nome = "";

            if (thepoide.Name != null)
                nome = thepoide.Name.FirstOrDefault().InnerText;
            else
                nome = "kein Name";

            hike.Shortname = nome.Trim();
            hike.Id = thepoide.RID;

            if (thepoide.ID != null)
                if (thepoide.ID.StartsWith("SMG-"))
                    hike.SmgId = thepoide.ID.Replace("SMG-", "");

            //SMG ACtive übernehmen  
            hike.Active = true;
            hike.SmgActive = hike.SmgActive;

            if (thepoide.GeoDatas.GeoData.FirstOrDefault().Altitude != null)
                hike.AltitudeDifference = thepoide.GeoDatas.GeoData.FirstOrDefault().Altitude.Difference;
            else
                hike.AltitudeDifference = 0;

            if (thepoide.GeoDatas.GeoData.FirstOrDefault().Distance != null)
                hike.DistanceLength = thepoide.GeoDatas.GeoData.FirstOrDefault().Distance.Length;
            else
                hike.DistanceLength = 0;

            if (thepoide.GeoDatas.GeoData.FirstOrDefault().Distance != null)
            {
                if (thepoide.GeoDatas.GeoData.FirstOrDefault().Distance.Duration != null)
                {
                    //var mydurationtemp = TimeSpan.Parse(thepoide.GeoDatas.GeoData.FirstOrDefault().Distance.Duration);
                    //hike.DistanceDuration = Convert.ToDouble(mydurationtemp.Hours + "," + mydurationtemp.Minutes);

                    //Ondere Meglichkeit
                    string duration = thepoide.GeoDatas.GeoData.FirstOrDefault().Distance.Duration;
                    var durationsplittet = duration.Split(':');
                    hike.DistanceDuration = Convert.ToDouble(durationsplittet[0] + "," + durationsplittet[1]);
                }
                else
                    hike.DistanceDuration = 0;
            }
            else
                hike.DistanceDuration = 0;

            //HasFreeEntrance gibts jetzt

            //hike.HasRentals = Convert.ToBoolean(thepoide.Features.HasRentals);
            //hike.IsWithLigth = Convert.ToBoolean(thepoide.Features.IsWithLight);
            hike.IsOpen = Convert.ToBoolean(thepoide.News.Status.IsOpen);
            hike.HasFreeEntrance = Convert.ToBoolean(thepoide.Features.HasFreeEntrance);

            //hike.IsPrepared = Convert.ToBoolean(thepoide.News.Status.IsPrepared);
            //hike.RunToValley = Convert.ToBoolean(thepoide.News.RunToValley.IsPossible);

            hike.LastChange = DateTime.Now; //Convert.ToDateTime(thepoide.News.Status.LastChange);

            //ContactInfos

            //List<ContactInfos> contactinfolist = new List<ContactInfos>();

            if (thepoide.ContactInfos.ContactInfo != null)
            {
                var parsedcontactinfode = thepoide.ContactInfos.ContactInfo.FirstOrDefault();
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

                    mycontactinfo.Phonenumber = parsedcontactinfode.Phones.Phone.FirstOrDefault().PhoneNumber;
                    mycontactinfo.Url = parsedcontactinfode.URLs.URL.FirstOrDefault().InnerText;

                    //contactinfolist.Add(mycontactinfo);

                    hike.ContactInfos.TryAddOrUpdate("de", mycontactinfo);
                }
            }

            if (thepoiit.ContactInfos.ContactInfo != null)
            {
                var parsedcontactinfoit = thepoiit.ContactInfos.ContactInfo.FirstOrDefault();
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

                    mycontactinfo.Phonenumber = parsedcontactinfoit.Phones.Phone.FirstOrDefault().PhoneNumber;
                    mycontactinfo.Url = parsedcontactinfoit.URLs.URL.FirstOrDefault().InnerText;

                    //contactinfolist.Add(mycontactinfo);

                    hike.ContactInfos.TryAddOrUpdate("it", mycontactinfo);
                }
            }

            if (thepoien.ContactInfos.ContactInfo != null)
            {
                var parsedcontactinfoen = thepoien.ContactInfos.ContactInfo.FirstOrDefault();
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

                    mycontactinfo.Phonenumber = parsedcontactinfoen.Phones.Phone.FirstOrDefault().PhoneNumber;
                    mycontactinfo.Url = parsedcontactinfoen.URLs.URL.FirstOrDefault().InnerText;

                    //contactinfolist.Add(mycontactinfo);
                    hike.ContactInfos.TryAddOrUpdate("en", mycontactinfo);
                }
            }

            //hike.ContactInfos = contactinfolist.ToList();

            //Ende ContactInfos

            //Multimedia Infos

            //List<Detail> detaillist = new List<Detail>();
            List<ImageGallery> imagegallerylist = new List<ImageGallery>();

            var parsedmultimediainfode = thepoide.MultimediaDescriptions.MultimediaDescription.FirstOrDefault();
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
                        myimggallery.ImageName = imageelement.RID;
                        myimggallery.ImageUrl = imageelement.ImageFormat.URL.InnerText;
                        myimggallery.IsInGallery = true;
                        //myimggallery.Language = "de";
                        myimggallery.ImageSource = "LTS";
                        myimggallery.ListPosition = i;
                        myimggallery.Width = imageelement.ImageFormat.Width;
                        myimggallery.CopyRight = imageelement.ImageFormat.CopyrightOwner;
                        myimggallery.License = imageelement.ImageFormat.License;

                        imagegallerylist.Add(myimggallery);
                        i++;
                    }
                }

                var mytextitems = parsedmultimediainfode.TextItems;

                Detail mydetailde = new Detail();

                mydetailde.Title = thepoide.Name.FirstOrDefault() != null ? thepoide.Name.FirstOrDefault().InnerText.Trim() : "";
                mydetailde.Header = "";
                mydetailde.Language = "de";
                mydetailde.AdditionalText = "";
                mydetailde.IntroText = "";
                //mydetailde.MainImage = "";

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
                        //Kurzbeschreibung A143F1FF0A0A4185A907208D643D3BB8 --> Placing in Introtext
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "A143F1FF0A0A4185A907208D643D3BB8")
                        {
                            mydetailde.IntroText = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Parken AFB5C1F8662949108105DAC81DD8B18F
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "AFB5C1F8662949108105DAC81DD8B18F")
                        {
                            mydetailde.ParkingInfo = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Öffentliche Verkehrsmittel C67DBA804FAD4B4EB64A4A758771B723
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "C67DBA804FAD4B4EB64A4A758771B723")
                        {
                            mydetailde.PublicTransportationInfo = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Tipp des Autors 643EDEE9CE2D44AC932FC714256B6C9C
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "643EDEE9CE2D44AC932FC714256B6C9C")
                        {
                            mydetailde.AuthorTip = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Sicherheitshinweise 73EC4F8F7AAC48CE82A23E8EBDCA750E
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "73EC4F8F7AAC48CE82A23E8EBDCA750E")
                        {
                            mydetailde.SafetyInfo = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Ausrüstung 933D29E15E82415DBEDE877C477212D2
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "933D29E15E82415DBEDE877C477212D2")
                        {
                            mydetailde.EquipmentInfo = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                    }
                }

                //detaillist.Add(mydetailde);
                hike.Detail.TryAddOrUpdate("de", mydetailde);
            }

            //it
            var parsedmultimediainfoit = thepoiit.MultimediaDescriptions.MultimediaDescription.FirstOrDefault();
            if (parsedmultimediainfoit != null)
            {
                if (parsedmultimediainfoit.ImageItems.ImageItem != null)
                {
                    int i = 0;
                    foreach (var imageelement in parsedmultimediainfoit.ImageItems.ImageItem)
                    {
                        imagegallerylist.Where(x => x.ListPosition == i && x.ImageSource == "LTS").FirstOrDefault().ImageDesc.TryAddOrUpdate("it", imageelement.Description.FirstOrDefault().InnerText);
                        i++;
                        //ImageGallery myimggallery = new ImageGallery();
                        //myimggallery.Height = imageelement.ImageFormat.Height;
                        //myimggallery.ImageDesc = imageelement.Description.FirstOrDefault().InnerText;
                        //myimggallery.ImageName = imageelement.RID;
                        //myimggallery.ImageUrl = imageelement.ImageFormat.URL.InnerText;
                        //myimggallery.IsInGallery = true;
                        //myimggallery.Language = "it";
                        //myimggallery.ListPosition = 0;
                        //myimggallery.Width = imageelement.ImageFormat.Width;

                        //imagegallerylist.Add(myimggallery);
                    }
                }

                var mytextitems = parsedmultimediainfoit.TextItems;

                Detail mydetailit = new Detail();

                mydetailit.Title = thepoiit.Name.FirstOrDefault() != null ? thepoiit.Name.FirstOrDefault().InnerText.Trim() : "";
                mydetailit.Header = "";
                mydetailit.Language = "it";
                mydetailit.AdditionalText = "";
                mydetailit.IntroText = "";
                //mydetailit.MainImage = "";

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
                        //Kurzbeschreibung A143F1FF0A0A4185A907208D643D3BB8 --> Placing in Introtext
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "A143F1FF0A0A4185A907208D643D3BB8")
                        {
                            mydetailit.IntroText = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Parken AFB5C1F8662949108105DAC81DD8B18F
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "AFB5C1F8662949108105DAC81DD8B18F")
                        {
                            mydetailit.ParkingInfo = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Öffentliche Verkehrsmittel C67DBA804FAD4B4EB64A4A758771B723
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "C67DBA804FAD4B4EB64A4A758771B723")
                        {
                            mydetailit.PublicTransportationInfo = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Tipp des Autors 643EDEE9CE2D44AC932FC714256B6C9C
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "643EDEE9CE2D44AC932FC714256B6C9C")
                        {
                            mydetailit.AuthorTip = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Sicherheitshinweise 73EC4F8F7AAC48CE82A23E8EBDCA750E
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "73EC4F8F7AAC48CE82A23E8EBDCA750E")
                        {
                            mydetailit.SafetyInfo = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Ausrüstung 933D29E15E82415DBEDE877C477212D2
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "933D29E15E82415DBEDE877C477212D2")
                        {
                            mydetailit.EquipmentInfo = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                    }
                }

                //detaillist.Add(mydetailit);
                hike.Detail.TryAddOrUpdate("it", mydetailit);
            }

            //en
            var parsedmultimediainfoen = thepoien.MultimediaDescriptions.MultimediaDescription.FirstOrDefault();
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

                mydetailen.Title = thepoien.Name.FirstOrDefault() != null ? thepoien.Name.FirstOrDefault().InnerText.Trim() : "";
                mydetailen.Header = "";
                mydetailen.Language = "en";
                mydetailen.AdditionalText = "";
                mydetailen.IntroText = "";
                //mydetailen.MainImage = "";

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
                        //Kurzbeschreibung A143F1FF0A0A4185A907208D643D3BB8 --> Placing in Introtext
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "A143F1FF0A0A4185A907208D643D3BB8")
                        {
                            mydetailen.IntroText = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Parken AFB5C1F8662949108105DAC81DD8B18F
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "AFB5C1F8662949108105DAC81DD8B18F")
                        {
                            mydetailen.ParkingInfo = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Öffentliche Verkehrsmittel C67DBA804FAD4B4EB64A4A758771B723
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "C67DBA804FAD4B4EB64A4A758771B723")
                        {
                            mydetailen.PublicTransportationInfo = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Tipp des Autors 643EDEE9CE2D44AC932FC714256B6C9C
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "643EDEE9CE2D44AC932FC714256B6C9C")
                        {
                            mydetailen.AuthorTip = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Sicherheitshinweise 73EC4F8F7AAC48CE82A23E8EBDCA750E
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "73EC4F8F7AAC48CE82A23E8EBDCA750E")
                        {
                            mydetailen.SafetyInfo = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                        //Ausrüstung 933D29E15E82415DBEDE877C477212D2
                        if (textelement.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().RID == "933D29E15E82415DBEDE877C477212D2")
                        {
                            mydetailen.EquipmentInfo = textelement.Description.Count() > 0 ? textelement.Description.FirstOrDefault().InnerText : "";
                        }
                    }
                }
                //detaillist.Add(mydetailen);

                hike.Detail.TryAddOrUpdate("en", mydetailen);
            }

            hike.ImageGallery = imagegallerylist.ToList();
            //hike.Detail = detaillist.ToList();

            //Ende MultimediaInfos

            //hike.Type = thepoide.EnumCodes.EnumCode != null ? thepoide.EnumCodes.EnumCode.FirstOrDefault().Name.FirstOrDefault().InnerText : GetPoiMainType(activitytype)["de"];
            //hike.SubType = thepoide.EnumCodes.EnumCode != null ?  thepoide.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText : "";


            //Tags beibehalten. Maintyp + Subtyp als Tag übernehmen     
            //List<string> currenttags = new List<string>();

            //if (thepoide.EnumCodes.EnumCode != null)
            //{
            //    string type = thepoide.EnumCodes.EnumCode != null ? thepoide.EnumCodes.EnumCode.FirstOrDefault().Name.FirstOrDefault().InnerText : GetPoiMainType(activitytype)["de"];
            //    string subtype = thepoide.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").Count() > 0 ? thepoide.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText : "no Subtype";

            //    if (!currenttags.Contains(type))
            //        currenttags.Add(type);
            //    //if (!currenttags.Contains(subtype))
            //    //    currenttags.Add(subtype);                
            //}

            //NEU gehe alle LTS Tags durch
            //foreach (var ltstag in thepoide.Tags.Tag)
            //{
            //    currenttags.Add(ltstag.Name.FirstOrDefault().InnerText);
            //}

            //hike.SmgTags = currenttags.ToList();

            List<LTSTags> ltstaglist = new List<LTSTags>();

            foreach (var ltstag in thepoide.Tags.Tag)
            {
                LTSTags myltstag = new LTSTags();
                myltstag.LTSRID = ltstag.RID;
                myltstag.Id = ltstag.Name.FirstOrDefault().InnerText.Trim().Replace("/", "");

                myltstag.TagName.TryAddOrUpdate("de", ltstag.Name != null ? ltstag.Name.FirstOrDefault().InnerText : "");

                //Sonderfix Tag DE enthält /
                if (myltstag.TagName["de"].Contains("/"))
                    myltstag.TagName.TryAddOrUpdate("de", myltstag.TagName["de"].Replace("/", ""));

                //NEW add also TIN info
                if (ltstag.Tins != null)
                {
                    foreach (var ltstin in ltstag.Tins.Tin)
                    {
                        LTSTins mytin = new LTSTins();
                        mytin.TinName.TryAddOrUpdate("de", ltstin.Name != null ? ltstin.Name.FirstOrDefault().InnerText : "");
                        mytin.LTSRID = ltstin.RID;

                        myltstag.LTSTins.Add(mytin);
                    }
                }

                ltstaglist.Add(myltstag);
            }

            foreach (var ltstag in thepoiit.Tags.Tag)
            {
                var currentltstagit = ltstaglist.Where(x => x.LTSRID == ltstag.RID).FirstOrDefault();
                currentltstagit.TagName.TryAddOrUpdate("it", ltstag.Name != null ? ltstag.Name.FirstOrDefault().InnerText : "");

                //add the tin info in IT
                if (ltstag.Tins != null)
                {
                    foreach (var ltstin in ltstag.Tins.Tin)
                    {
                        var currentltstinit = currentltstagit.LTSTins.Where(x => x.LTSRID == ltstin.RID).FirstOrDefault();
                        currentltstinit.TinName.TryAddOrUpdate("it", ltstin.Name != null ? ltstin.Name.FirstOrDefault().InnerText : "");
                    }
                }
            }
            foreach (var ltstag in thepoien.Tags.Tag)
            {
                var currentltstagen = ltstaglist.Where(x => x.LTSRID == ltstag.RID).FirstOrDefault();
                currentltstagen.TagName.TryAddOrUpdate("en", ltstag.Name != null ? ltstag.Name.FirstOrDefault().InnerText : "");

                //add the tin info in EN and set the id to EN object
                if (ltstag.Tins != null)
                {
                    foreach (var ltstin in ltstag.Tins.Tin)
                    {
                        var currentltstinen = currentltstagen.LTSTins.Where(x => x.LTSRID == ltstin.RID).FirstOrDefault();
                        currentltstinen.TinName.TryAddOrUpdate("en", ltstin.Name != null ? ltstin.Name.FirstOrDefault().InnerText : "");

                        //Set ID
                        currentltstinen.Id = ltstin.Name.FirstOrDefault().InnerText.ComputeSHA1Hash();
                    }
                }
            }

            hike.LTSTags = ltstaglist;

            //Add to SMGTags
            hike.SmgTags = ltstaglist.Select(x => x.Id).ToList();

            //Ende Tagging

            //AdditionalPoiInfos

            List<AdditionalPoiInfos> myaddpoiinfoslist = new List<AdditionalPoiInfos>();

            AdditionalPoiInfos myaddpoiinfosde = new AdditionalPoiInfos();
            AdditionalPoiInfos myaddpoiinfosit = new AdditionalPoiInfos();
            AdditionalPoiInfos myaddpoiinfosen = new AdditionalPoiInfos();

            myaddpoiinfosde.Language = "de";
            myaddpoiinfosit.Language = "it";
            myaddpoiinfosen.Language = "en";

            //myaddpoiinfosde.MainType = thepoide.EnumCodes.EnumCode != null ? thepoide.EnumCodes.EnumCode.FirstOrDefault().Name.FirstOrDefault().InnerText : GetPoiMainType(activitytype)["de"];
            //myaddpoiinfosit.MainType = thepoide.EnumCodes.EnumCode != null ? thepoiit.EnumCodes.EnumCode.FirstOrDefault().Name.FirstOrDefault().InnerText : GetPoiMainType(activitytype)["it"];
            //myaddpoiinfosen.MainType = thepoide.EnumCodes.EnumCode != null ? thepoien.EnumCodes.EnumCode.FirstOrDefault().Name.FirstOrDefault().InnerText : GetPoiMainType(activitytype)["en"];

            //myaddpoiinfosde.SubType = thepoide.EnumCodes.EnumCode != null ? thepoide.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText : ""; ;
            //myaddpoiinfosit.SubType = thepoide.EnumCodes.EnumCode != null ? thepoiit.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText : ""; ;
            //myaddpoiinfosen.SubType = thepoide.EnumCodes.EnumCode != null ? thepoien.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText : ""; ;

            if (thepoide.News.Novelty.Description != null)
            {
                myaddpoiinfosde.Novelty = thepoide.News.Novelty.Description.InnerText;
            }
            if (thepoiit.News.Novelty.Description != null)
            {
                myaddpoiinfosit.Novelty = thepoiit.News.Novelty.Description.InnerText;
            }
            if (thepoien.News.Novelty.Description != null)
            {
                myaddpoiinfosen.Novelty = thepoien.News.Novelty.Description.InnerText;
            }

            hike.AdditionalPoiInfos.TryAddOrUpdate("de", myaddpoiinfosde);
            hike.AdditionalPoiInfos.TryAddOrUpdate("it", myaddpoiinfosit);
            hike.AdditionalPoiInfos.TryAddOrUpdate("en", myaddpoiinfosen);

            //Ende AdditionalPoiInfos

            //hike.DistrictId = "";
            hike.TourismorganizationId = thepoide.Owner.RID;

            hike.OwnerRid = thepoide.Owner.RID;

            //hike.RegionId = "";

            List<GpsInfo> mygpsinfolist = new List<GpsInfo>();

            var positionslist = thepoide.GeoDatas.GeoData.FirstOrDefault().Positions;

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

            var gpstracklist = thepoide.GeoDatas.GeoData.FirstOrDefault().GPSTracks;

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

                        //EN und IT Info?

                        mygpstrack.GpxTrackDesc.TryAddOrUpdate("de", gpstrack.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText);

                        mygpstracklist.Add(mygpstrack);
                    }
                }
            }

            var gpstrackslistit = thepoiit.GeoDatas.GeoData.FirstOrDefault().GPSTracks;

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

            var gpstrackslisten = thepoien.GeoDatas.GeoData.FirstOrDefault().GPSTracks;

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


            List<string> arearidlist = new List<string>();

            if (thepoide.Memberships != null)
            {
                var members = thepoide.Memberships.Membership;

                ////Achtung des tuatmer lei uane Area inni
                //foreach (var member in members)
                //{
                //    if (member.Area != null)
                //    {
                //        string z = member.Area.FirstOrDefault().RID;
                //        arearidlist.Add(z);
                //    }
                //}

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

            //OperationSchedules muss ich mir noch genauer anschauen
            List<OperationSchedule> operationschedulelist = new List<OperationSchedule>();

            if (thepoide.OperationSchedules != null)
            {
                if (thepoide.OperationSchedules.OperationSchedule != null)
                {
                    foreach (var myoperationschedule in thepoide.OperationSchedules.OperationSchedule)
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

            //Neu Status abfragen
            if (thepoide.News.Status.IsEnabled == 1)
                hike.Active = true;
            else
                hike.Active = false;

            if (thepoide.News.Status.CopyrightChecked == 1)
                hike.CopyrightChecked = true;
            else if (thepoide.News.Status.CopyrightChecked == 0)
                hike.CopyrightChecked = false;
            else
                hike.CopyrightChecked = null;

            List<string> childpois = new List<string>();
            //Beacon Search Info
            if (thepoide.Beacons != null)
            {
                if (thepoide.Beacons.Beacon != null)
                {
                    foreach (var beacon in thepoide.Beacons.Beacon)
                    {
                        childpois.Add(beacon.RID);
                    }
                }
            }
            hike.ChildPoiIds = childpois.ToList();


            //Add The available Languages
            //Check wo überall in Details sprachknoten enthalten sind     
            var availablelanguages = hike.Detail.Select(x => x.Key).ToList();
            hike.HasLanguage = availablelanguages;

            return hike;
        }

        public static Dictionary<string, string> GetPoiMainType(string mainType)
        {
            Dictionary<string, string> maintypedict = new Dictionary<string, string>();

            switch (mainType)
            {
                case "ACTIVE":
                    maintypedict.Add("de", "Sport und Freizeiteinrichtungen");
                    maintypedict.Add("it", "Sport ed strutture per l'intrattenimento");
                    maintypedict.Add("en", "sports and free time facilities");
                    break;
                case "ARTISAN":
                    maintypedict.Add("de", "Kunsthandwerker");
                    maintypedict.Add("it", "artigianato artistico");
                    maintypedict.Add("en", "arts and crafts");
                    break;
                case "HEALTH":
                    maintypedict.Add("de", "Ärzte, Apotheken");
                    maintypedict.Add("it", "Salute e benessere");
                    maintypedict.Add("en", "health and well-being");
                    break;
                case "NIGHTLIFE":
                    maintypedict.Add("de", "Nachtleben und Unterhaltung");
                    maintypedict.Add("it", "Vita notturna ed intrattenimento");
                    maintypedict.Add("en", "Nightlife and entertainment");
                    break;
                case "SHOP":
                    maintypedict.Add("de", "Geschäfte");
                    maintypedict.Add("it", "Negozi e servizi");
                    maintypedict.Add("en", "Shops and services");
                    break;
                case "SIGHTSEEN":
                    maintypedict.Add("de", "Kultur und Sehenswürdigkeiten");
                    maintypedict.Add("it", "cultura e lughi di interesse");
                    maintypedict.Add("en", "cultur and sights");
                    break;
                case "SERVICE":
                    maintypedict.Add("de", "Öffentliche Einrichtungen");
                    maintypedict.Add("it", "Prestazioni di servizi");
                    maintypedict.Add("en", "Services");
                    break;
                case "SERVICEPROVIDER":
                    maintypedict.Add("de", "Dienstleister");
                    maintypedict.Add("it", "Prestazioni di servizi");
                    maintypedict.Add("en", "Serviceprovider");
                    break;
                case "MOBILITY_TRAFFIC":
                    maintypedict.Add("de", "Verkehr und Transport");
                    maintypedict.Add("it", "Traffico e trasporto");
                    maintypedict.Add("en", "Traffic and transport");
                    break;
            }

            return maintypedict;
        }

    }
}
