// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SuedtirolWein.Parser
{
    public class ParseCompanyData
    {
        private static void ParseContactInfo(string language, XElement companydata, ODHActivityPoiLinked mywinecompany)
        {
            ContactInfos contactinfo = new ContactInfos();

            contactinfo.CompanyName = companydata.Element("title").Value;
            contactinfo.Address = companydata.Element("address") != null ? companydata.Element("address").Value : null;
            contactinfo.ZipCode = companydata.Element("zipcode") != null ? companydata.Element("zipcode").Value : null;
            contactinfo.City = companydata.Element("place") != null ? companydata.Element("place").Value : null;

            string countryname = "Italy";
            if (language == "de")
                countryname = "Italien";
            if (language == "it")
                countryname = "Italia";

            contactinfo.CountryName = countryname;
            contactinfo.CountryCode = "IT";
            contactinfo.Phonenumber = companydata.Element("phone") != null ? companydata.Element("phone").Value : null;

            string webadresseIT = "";
            //Webadress gschicht
            if (companydata.Element("homepage") != null)
            {
                webadresseIT = companydata.Element("homepage").Value.Contains("http") ? companydata.Element("homepage").Value : "http://" + companydata.Element("homepage").Value;
            }

            contactinfo.Url = webadresseIT;
            contactinfo.Email = companydata.Element("email") != null ? companydata.Element("email").Value : null;
            contactinfo.LogoUrl = companydata.Element("logo") != null ? companydata.Element("logo").Value : null;
            contactinfo.Language = language;

            mywinecompany.ContactInfos.TryAddOrUpdate(language, contactinfo);
        }

        private static void ParseDetailInfo(string language, XElement companydata, ODHActivityPoiLinked mywinecompany)
        {
            //Detail IT
            Detail mydetail = new Detail();

            if (mywinecompany.Detail != null)
                if (mywinecompany.Detail.ContainsKey(language))
                    mydetail = mywinecompany.Detail[language];


            mydetail.Title = companydata.Element("title").Value;

            mydetail.BaseText = companydata.Element("companydescription") != null ? companydata.Element("companydescription").Value : null;

            mydetail.Header = companydata.Element("slogan") != null ? companydata.Element("slogan").Value : null;
            mydetail.SubHeader = companydata.Element("subtitle") != null ? companydata.Element("subtitle").Value : null;
            mydetail.IntroText = companydata.Element("quote") != null ? companydata.Element("quote").Value : null;


            mydetail.Language = language;
            mywinecompany.Detail.TryAddOrUpdate(language, mydetail);
        }

        private static void ParseImageGalleryData(IDictionary<string, XElement> companydata, ODHActivityPoiLinked mywinecompany)
        {
            //ImageGallery
            List<ImageGallery> myimagegallerylist = new List<ImageGallery>();

            string imageurlmedia = "";

            if (companydata["de"].Element("media") != null)
            {
                ImageGallery myimagegallery = new ImageGallery();
                myimagegallery.ImageUrl = companydata["de"].Element("media").Value;
                myimagegallery.ImageSource = "SuedtirolWein";
                myimagegallery.IsInGallery = true;
                myimagegallery.ListPosition = 0;
                myimagegallery.CopyRight = "Suedtirol Wein";
                myimagegallery.License = "";
                myimagegallery.LicenseHolder = "https://www.suedtirolwein.com/";

                //Image Title
                if (companydata["de"].Element("imagemetatitle") != null)
                    myimagegallery.ImageTitle.TryAddOrUpdate("de", companydata["de"].Element("imagemetatitle").Value);
                //Image Description
                if (companydata["de"].Element("imagemetadescription") != null)
                    myimagegallery.ImageDesc.TryAddOrUpdate("de", companydata["de"].Element("imagemetadescription").Value);
                //Image Alttext
                if (companydata["de"].Element("imagemetaalt") != null)
                    myimagegallery.ImageAltText.TryAddOrUpdate("de", companydata["de"].Element("imagemetaalt").Value);

                imageurlmedia = myimagegallery.ImageUrl;

                myimagegallerylist.Add(myimagegallery);
            }
            if (companydata["it"] != null && companydata["it"].Element("media") != null)
            {
                if (myimagegallerylist.Count == 1)
                {
                    if (companydata["it"].Element("imagemetatitle") != null)
                        myimagegallerylist.FirstOrDefault().ImageTitle.TryAddOrUpdate("it", companydata["it"].Element("imagemetatitle").Value);
                    if (companydata["it"].Element("imagemetadescription") != null)
                        myimagegallerylist.FirstOrDefault().ImageDesc.TryAddOrUpdate("it", companydata["it"].Element("imagemetadescription").Value);
                    if (companydata["it"].Element("imagemetaalt") != null)
                        myimagegallerylist.FirstOrDefault().ImageAltText.TryAddOrUpdate("it", companydata["it"].Element("imagemetaalt").Value);

                }
            }
            if (companydata["en"] != null && companydata["en"].Element("media") != null)
            {
                if (myimagegallerylist.Count == 1)
                {
                    if (companydata["en"].Element("imagemetatitle") != null)
                        myimagegallerylist.FirstOrDefault().ImageTitle.TryAddOrUpdate("en", companydata["en"].Element("imagemetatitle").Value);
                    if (companydata["en"].Element("imagemetadescription") != null)
                        myimagegallerylist.FirstOrDefault().ImageDesc.TryAddOrUpdate("en", companydata["en"].Element("imagemetadescription").Value);
                    if (companydata["en"].Element("imagemetaalt") != null)
                        myimagegallerylist.FirstOrDefault().ImageAltText.TryAddOrUpdate("en", companydata["en"].Element("imagemetaalt").Value);
                }
            }
            if (companydata["ru"] != null && companydata["ru"].Element("media") != null)
            {
                if (myimagegallerylist.Count == 1)
                {
                    if (companydata["ru"].Element("imagemetatitle") != null)
                        myimagegallerylist.FirstOrDefault().ImageTitle.TryAddOrUpdate("ru", companydata["ru"].Element("imagemetatitle").Value);
                    if (companydata["ru"].Element("imagemetadescription") != null)
                        myimagegallerylist.FirstOrDefault().ImageDesc.TryAddOrUpdate("ru", companydata["ru"].Element("imagemetadescription").Value);
                    if (companydata["ru"].Element("imagemetaalt") != null)
                        myimagegallerylist.FirstOrDefault().ImageAltText.TryAddOrUpdate("ru", companydata["ru"].Element("imagemetaalt").Value);
                }
            }
            if (companydata["jp"] != null && companydata["jp"].Element("media") != null)
            {
                if (myimagegallerylist.Count == 1)
                {
                    if (companydata["jp"].Element("imagemetatitle") != null)
                        myimagegallerylist.FirstOrDefault().ImageTitle.TryAddOrUpdate("jp", companydata["jp"].Element("imagemetatitle").Value);
                    if (companydata["jp"].Element("imagemetadescription") != null)
                        myimagegallerylist.FirstOrDefault().ImageDesc.TryAddOrUpdate("jp", companydata["jp"].Element("imagemetadescription").Value);
                    if (companydata["jp"].Element("imagemetaalt") != null)
                        myimagegallerylist.FirstOrDefault().ImageAltText.TryAddOrUpdate("jp", companydata["jp"].Element("imagemetaalt").Value);
                }
            }

            //Fix add mediadetail only if url differs from media

            if (companydata["de"].Element("mediadetail") != null)
            {
                ImageGallery myimagegallery2 = new ImageGallery();
                myimagegallery2.ImageUrl = companydata["de"].Element("mediadetail").Value;
                myimagegallery2.ImageSource = "SuedtirolWein";
                myimagegallery2.IsInGallery = true;
                myimagegallery2.ListPosition = 1;
                myimagegallery2.CopyRight = "Suedtirol Wein";
                myimagegallery2.License = "";
                myimagegallery2.LicenseHolder = "https://www.suedtirolwein.com/";

                if (imageurlmedia != myimagegallery2.ImageUrl)
                    myimagegallerylist.Add(myimagegallery2);
            }
            //if (companydata["de"].Element("logo") != null)
            //{
            //    ImageGallery myimagegallery3 = new ImageGallery();
            //    myimagegallery3.ImageUrl = companydata["de"].Element("logo").Value;
            //    myimagegallery3.ImageSource = "SuedtirolWein";
            //    myimagegallery3.IsInGallery = true;
            //    myimagegallery3.ListPosition = 2;
            //    myimagegallery3.CopyRight = "Suedtirol Wein";
            //    myimagegallery3.License = "";
            //    myimagegallery3.LicenseHolder = "https://www.suedtirolwein.com/";

            //    if (imageurlmedia != myimagegallery3.ImageUrl)
            //        myimagegallerylist.Add(myimagegallery3);
            //}

            mywinecompany.ImageGallery = myimagegallerylist;
        }

        private static void ParsePropertyData(string language, XElement companydata, ODHActivityPoiLinked mywinecompany)
        {
            //PROPS DE
            List<PoiProperty> mypropertylist = new List<PoiProperty>();
            if (companydata.Element("openingtimeswineshop") != null)
            {
                PoiProperty myprop = new PoiProperty();
                myprop.Name = "openingtimeswineshop";
                myprop.Value = companydata.Element("openingtimeswineshop").Value;
                mypropertylist.Add(myprop);
            }
            if (companydata.Element("openingtimesguides") != null)
            {
                PoiProperty myprop2 = new PoiProperty();
                myprop2.Name = "openingtimesguides";
                myprop2.Value = companydata.Element("openingtimesguides").Value;
                mypropertylist.Add(myprop2);
            }
            if (companydata.Element("openingtimesgastronomie") != null)
            {
                PoiProperty myprop3 = new PoiProperty();
                myprop3.Name = "openingtimesgastronomie";
                myprop3.Value = companydata.Element("openingtimesgastronomie").Value;
                mypropertylist.Add(myprop3);
            }
            if (companydata.Element("companyholiday") != null)
            {
                PoiProperty myprop4 = new PoiProperty();
                myprop4.Name = "companyholiday";
                myprop4.Value = companydata.Element("companyholiday").Value;
                mypropertylist.Add(myprop4);
            }
            
            if (companydata.Element("hasvisits") != null)
            {
                PoiProperty myprop5 = new PoiProperty();
                myprop5.Name = "hasvisits";
          
                myprop5.Value = companydata.Element("hasvisits").Value;
                mypropertylist.Add(myprop5);
            }
            if (companydata.Element("hasovernights") != null)
            {
                PoiProperty myprop6 = new PoiProperty();
                myprop6.Name = "hasovernights";

                myprop6.Value = companydata.Element("hasovernights").Value;
                mypropertylist.Add(myprop6);
            }
            if (companydata.Element("hasbiowine") != null)
            {
                PoiProperty myprop7 = new PoiProperty();
                myprop7.Name = "hasbiowine";

                myprop7.Value = companydata.Element("hasbiowine").Value;
                mypropertylist.Add(myprop7);
            }
            if (companydata.Element("wines") != null)
            {
                PoiProperty myprop8 = new PoiProperty();
                myprop8.Name = "wines";
                myprop8.Value = companydata.Element("wines").Value;
                mypropertylist.Add(myprop8);
            }
            if (companydata.Element("hasaccomodation") != null)
            {
                PoiProperty myprop9 = new PoiProperty();
                myprop9.Name = "hasaccomodation";

                myprop9.Value = companydata.Element("hasaccomodation").Value;
                mypropertylist.Add(myprop9);
            }
            if (companydata.Element("isvinumhotel") != null)
            {
                PoiProperty myprop10 = new PoiProperty();
                myprop10.Name = "isvinumhotel";

                myprop10.Value = companydata.Element("isvinumhotel").Value;
                mypropertylist.Add(myprop10);
            }
            if (companydata.Element("isanteprima") != null)
            {
                PoiProperty myprop11 = new PoiProperty();
                myprop11.Name = "isanteprima";

                myprop11.Value = companydata.Element("isanteprima").Value;
                mypropertylist.Add(myprop11);
            }
            if (companydata.Element("iswinestories") != null)
            {
                PoiProperty myprop12 = new PoiProperty();
                myprop12.Name = "iswinestories";

                myprop12.Value = companydata.Element("iswinestories").Value;
                mypropertylist.Add(myprop12);
            }
            if (companydata.Element("iswinesummit") != null)
            {
                PoiProperty myprop13 = new PoiProperty();
                myprop13.Name = "iswinesummit";

                myprop13.Value = companydata.Element("iswinesummit").Value;
                mypropertylist.Add(myprop13);
            }
            if (companydata.Element("issparklingwineassociation") != null)
            {
                PoiProperty myprop14 = new PoiProperty();
                myprop14.Name = "issparklingwineassociation";

                myprop14.Value = companydata.Element("issparklingwineassociation").Value;
                mypropertylist.Add(myprop14);
            }
            if (companydata.Element("iswinery") != null)
            {
                PoiProperty myprop15 = new PoiProperty();
                myprop15.Name = "iswinery";

                myprop15.Value = companydata.Element("iswinery").Value;
                mypropertylist.Add(myprop15);
            }
            if (companydata.Element("iswineryassociation") != null)
            {
                PoiProperty myprop16 = new PoiProperty();
                myprop16.Name = "iswineryassociation";

                myprop16.Value = companydata.Element("iswineryassociation").Value;
                mypropertylist.Add(myprop16);
            }

            //hasonlineshop
            if (companydata.Element("hasonlineshop") != null)
            {
                PoiProperty myprop17 = new PoiProperty();
                myprop17.Name = "hasonlineshop";

                myprop17.Value = companydata.Element("hasonlineshop").Value;
                mypropertylist.Add(myprop17);
            }

            //hasdeliveryservice
            if (companydata.Element("hasdeliveryservice") != null)
            {
                PoiProperty myprop18 = new PoiProperty();
                myprop18.Name = "hasdeliveryservice";

                myprop18.Value = companydata.Element("hasdeliveryservice").Value;
                mypropertylist.Add(myprop18);
            }

            //onlineshopurl
            if (companydata.Element("onlineshopurl") != null)
            {
                PoiProperty myprop19 = new PoiProperty();
                myprop19.Name = "onlineshopurl";

                myprop19.Value = companydata.Element("onlineshopurl").Value;
                mypropertylist.Add(myprop19);
            }
            //deliveryserviceurl
            if (companydata.Element("deliveryserviceurl") != null)
            {
                PoiProperty myprop20 = new PoiProperty();
                myprop20.Name = "deliveryserviceurl";

                myprop20.Value = companydata.Element("deliveryserviceurl").Value;
                mypropertylist.Add(myprop20);
            }
            //h1
            if (companydata.Element("h1") != null)
            {
                PoiProperty myprop21 = new PoiProperty();
                myprop21.Name = "h1";

                myprop21.Value = companydata.Element("h1").Value;
                mypropertylist.Add(myprop21);
            }
            //h2
            if (companydata.Element("h2") != null)
            {
                PoiProperty myprop22 = new PoiProperty();
                myprop22.Name = "h2";

                myprop22.Value = companydata.Element("h2").Value;
                mypropertylist.Add(myprop22);
            }
       
            //quote
            if (companydata.Element("quote") != null)
            {
                PoiProperty myprop23 = new PoiProperty();
                myprop23.Name = "quote";

                myprop23.Value = companydata.Element("quote").Value;
                mypropertylist.Add(myprop23);
            }

            //quote author
            if (companydata.Element("quoteauthor") != null)
            {
                PoiProperty myprop24 = new PoiProperty();
                myprop24.Name = "quoteauthor";

                myprop24.Value = companydata.Element("quoteauthor").Value;
                mypropertylist.Add(myprop24);
            }

            //descriptionsparklingwineproducer
            if (companydata.Element("descriptionsparklingwineproducer") != null)
            {
                PoiProperty myprop25 = new PoiProperty();
                myprop25.Name = "descriptionsparklingwineproducer";

                myprop25.Value = companydata.Element("descriptionsparklingwineproducer").Value;
                mypropertylist.Add(myprop25);
            }

            //h1sparklingwineproducer
            if (companydata.Element("h1sparklingwineproducer") != null)
            {
                PoiProperty myprop26 = new PoiProperty();
                myprop26.Name = "h1sparklingwineproducer";

                myprop26.Value = companydata.Element("h1sparklingwineproducer").Value;
                mypropertylist.Add(myprop26);
            }

            //h2sparklingwineproducer
            if (companydata.Element("h2sparklingwineproducer") != null)
            {
                PoiProperty myprop27 = new PoiProperty();
                myprop27.Name = "h2sparklingwineproducer";

                myprop27.Value = companydata.Element("h2sparklingwineproducer").Value;
                mypropertylist.Add(myprop27);
            }

            //<imagesparklingwineproducer>
            if (companydata.Element("imagesparklingwineproducer") != null)
            {
                PoiProperty myprop28 = new PoiProperty();
                myprop28.Name = "imagesparklingwineproducer";

                myprop28.Value = companydata.Element("imagesparklingwineproducer").Value;
                mypropertylist.Add(myprop28);
            }

            //<hasdirectsales>
            if (companydata.Element("hasdirectsales") != null)
            {
                PoiProperty myprop29 = new PoiProperty();
                myprop29.Name = "hasdirectsales";

                myprop29.Value = companydata.Element("hasdirectsales").Value;
                mypropertylist.Add(myprop29);
            }

            //isskyalpspartner
            if (companydata.Element("isskyalpspartner") != null)
            {
                PoiProperty myprop30 = new PoiProperty();
                myprop30.Name = "isskyalpspartner";

                myprop30.Value = companydata.Element("isskyalpspartner").Value;
                mypropertylist.Add(myprop30);
            }

            mywinecompany.PoiProperty.TryAddOrUpdate(language, mypropertylist);
        }

        private static void ParseImporterData(string language, XElement companydata, ODHActivityPoiLinked mywinecompany)
        {
            List<AdditionalContact> importercontactInfos = new List<AdditionalContact>();

            if (companydata.Element("importers") != null)
            {
                if (companydata.Element("importers").Elements("importer") != null)
                {
                    foreach (var companyimporterde in companydata.Element("importers").Elements("importer"))
                    {
                        if (companyimporterde.HasElements)
                        {
                            AdditionalContact myadditionalcontactde = new AdditionalContact();
                            myadditionalcontactde.Type = "wineimporter";

                            ContactInfos myimportercontactinfode = new ContactInfos();

                            if (companyimporterde.Element("importername") != null)
                                myimportercontactinfode.CompanyName = companyimporterde.Element("importername").Value;
                            if (companyimporterde.Element("importeraddress") != null)
                                myimportercontactinfode.Address = companyimporterde.Element("importeraddress").Value;
                            if (companyimporterde.Element("importerzipcode") != null)
                                myimportercontactinfode.ZipCode = companyimporterde.Element("importerzipcode").Value;
                            if (companyimporterde.Element("importerplace") != null)
                                myimportercontactinfode.City = companyimporterde.Element("importerplace").Value;
                            if (companyimporterde.Element("importerphone") != null)
                                myimportercontactinfode.Phonenumber = companyimporterde.Element("importerphone").Value;
                            if (companyimporterde.Element("importeremail") != null)
                                myimportercontactinfode.Email = companyimporterde.Element("importeremail").Value;
                            if (companyimporterde.Element("importerhomepage") != null)
                                myimportercontactinfode.Url = companyimporterde.Element("importerhomepage").Value;
                            if (companyimporterde.Element("importercontactperson") != null)
                                myimportercontactinfode.Givenname = companyimporterde.Element("importercontactperson").Value;
                            if (companyimporterde.Element("importerdescription") != null)
                                myadditionalcontactde.Description = companyimporterde.Element("importerdescription").Value;

                            myimportercontactinfode.Language = language;

                            myadditionalcontactde.ContactInfos = myimportercontactinfode;

                            importercontactInfos.Add(myadditionalcontactde);
                        }
                    }
                }
            }
            if (importercontactInfos.Count > 0)
            {
                if (mywinecompany.AdditionalContact == null)
                    mywinecompany.AdditionalContact = new Dictionary<string, List<AdditionalContact>>();

                mywinecompany.AdditionalContact.TryAddOrUpdate(language, importercontactInfos);
            }
        }

        public static ODHActivityPoiLinked ParsetheCompanyData(ODHActivityPoiLinked mywinecompany, IDictionary<string, XElement> companydata, List<string> haslanguage)
        {
            mywinecompany.LastChange = DateTime.Now;

            mywinecompany.Id = companydata["de"].Element("id").Value;
            mywinecompany.CustomId = companydata["de"].Element("id").Value;

            //ADD MAPPING
            var suedtirolweinid = new Dictionary<string, string>() { { "id", mywinecompany.Id } };
            mywinecompany.Mapping.TryAddOrUpdate("suedtirolwein", suedtirolweinid);

            mywinecompany.Source = "suedtirolwein";
            mywinecompany.SyncSourceInterface = "suedtirolwein";
            mywinecompany.SyncUpdateMode = "Full";

            mywinecompany.Type = "Essen Trinken";
            mywinecompany.SubType = "Weinkellereien";

            mywinecompany.Shortname = companydata["de"].Element("title").Value;

            //Contactinfo for all languages
            foreach (var language in haslanguage)
            {
                ParseContactInfo(language, companydata[language], mywinecompany);
            }

            //Detailinfo for all languages
            foreach (var language in haslanguage)
            {
                ParseDetailInfo(language, companydata[language], mywinecompany);
            }

            //Propertyinfo for all languages
            ParseImageGalleryData(companydata, mywinecompany);

            //Propertyinfo for all languages
            foreach (var language in haslanguage)
            {
                ParsePropertyData(language, companydata[language], mywinecompany);
            }

            //Wineids
            if (companydata["de"].Element("wineids") != null)
            {
                List<string> poiServices = new List<string>();
                var wineids = companydata["de"].Element("wines").Value.Split(',');
                poiServices = wineids.ToList();
                mywinecompany.PoiServices = poiServices;
            }

            if (companydata["de"].Element("latidude") != null && companydata["de"].Element("longitude") != null)
            {
                if (!companydata["de"].Element("latidude").Value.Contains("°") && !companydata["de"].Element("longitude").Value.Contains("°"))
                {
                    if (companydata["de"].Element("latidude").Value != "0" && companydata["de"].Element("longitude").Value != "0")
                    {
                        List<GpsInfo> gpsinfolist = new List<GpsInfo>();
                        GpsInfo mygps = new GpsInfo();
                        mygps.Latitude = Convert.ToDouble(companydata["de"].Element("latidude").Value, CultureInfo.CurrentCulture);
                        mygps.Longitude = Convert.ToDouble(companydata["de"].Element("longitude").Value, CultureInfo.CurrentCulture);
                        mygps.Gpstype = "position";
                        gpsinfolist.Add(mygps);
                        mywinecompany.GpsInfo = gpsinfolist;
                    }
                }
            }

            //WineImporters for all languages
            foreach (var language in haslanguage)
            {
                ParseImporterData(language, companydata[language], mywinecompany);
            }

            return mywinecompany;
        }       
    }
}
