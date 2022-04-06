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
        private static void ParseContactInfo(string language, XElement companydata, ODHActivityPoi mywinecompany)
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

        private static void ParseDetailInfo(string language, XElement companydata, ODHActivityPoi mywinecompany)
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

        private static void ParseImageGalleryData(IDictionary<string, XElement> companydata, ODHActivityPoi mywinecompany)
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
                myimagegallery.License = null; //"CC0";

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

                if(imageurlmedia != myimagegallery2.ImageUrl)
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

            //    if (imageurlmedia != myimagegallery3.ImageUrl)
            //        myimagegallerylist.Add(myimagegallery3);
            //}

            mywinecompany.ImageGallery = myimagegallerylist;
        }

        private static void ParsePropertyData(string language, XElement companydata, ODHActivityPoi mywinecompany)
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

            //new hasonlineshop
            if (companydata.Element("hasonlineshop") != null)
            {
                PoiProperty myprop17 = new PoiProperty();
                myprop17.Name = "hasonlineshop";

                myprop17.Value = companydata.Element("hasonlineshop").Value;
                mypropertylist.Add(myprop17);
            }

            //new hasdeliveryservice
            if (companydata.Element("hasdeliveryservice") != null)
            {
                PoiProperty myprop18 = new PoiProperty();
                myprop18.Name = "hasdeliveryservice";

                myprop18.Value = companydata.Element("hasdeliveryservice").Value;
                mypropertylist.Add(myprop18);
            }

            //new onlineshopurl
            if (companydata.Element("onlineshopurl") != null)
            {
                PoiProperty myprop19 = new PoiProperty();
                myprop19.Name = "onlineshopurl";

                myprop19.Value = companydata.Element("onlineshopurl").Value;
                mypropertylist.Add(myprop19);
            }
            // new deliveryserviceurl
            if (companydata.Element("deliveryserviceurl") != null)
            {
                PoiProperty myprop20 = new PoiProperty();
                myprop20.Name = "deliveryserviceurl";

                myprop20.Value = companydata.Element("deliveryserviceurl").Value;
                mypropertylist.Add(myprop20);
            }
            //new h1
            if (companydata.Element("h1") != null)
            {
                PoiProperty myprop21 = new PoiProperty();
                myprop21.Name = "h1";

                myprop21.Value = companydata.Element("h1").Value;
                mypropertylist.Add(myprop21);
            }
            //new h2
            if (companydata.Element("h2") != null)
            {
                PoiProperty myprop22 = new PoiProperty();
                myprop22.Name = "h2";

                myprop22.Value = companydata.Element("h2").Value;
                mypropertylist.Add(myprop22);
            }
       
            //new quote
            if (companydata.Element("quote") != null)
            {
                PoiProperty myprop23 = new PoiProperty();
                myprop23.Name = "quote";

                myprop23.Value = companydata.Element("quote").Value;
                mypropertylist.Add(myprop23);
            }

            //new quote author
            if (companydata.Element("quoteauthor") != null)
            {
                PoiProperty myprop24 = new PoiProperty();
                myprop24.Name = "quoteauthor";

                myprop24.Value = companydata.Element("quoteauthor").Value;
                mypropertylist.Add(myprop24);
            }

            //new descriptionsparklingwineproducer
            if (companydata.Element("descriptionsparklingwineproducer") != null)
            {
                PoiProperty myprop25 = new PoiProperty();
                myprop25.Name = "descriptionsparklingwineproducer";

                myprop25.Value = companydata.Element("descriptionsparklingwineproducer").Value;
                mypropertylist.Add(myprop25);
            }

            //new h1sparklingwineproducer
            if (companydata.Element("h1sparklingwineproducer") != null)
            {
                PoiProperty myprop26 = new PoiProperty();
                myprop26.Name = "h1sparklingwineproducer";

                myprop26.Value = companydata.Element("h1sparklingwineproducer").Value;
                mypropertylist.Add(myprop26);
            }

            //new h2sparklingwineproducer
            if (companydata.Element("h2sparklingwineproducer") != null)
            {
                PoiProperty myprop27 = new PoiProperty();
                myprop27.Name = "h2sparklingwineproducer";

                myprop27.Value = companydata.Element("h2sparklingwineproducer").Value;
                mypropertylist.Add(myprop27);
            }

            if (companydata.Element("imagesparklingwineproducer") != null)
            {
                PoiProperty myprop28 = new PoiProperty();
                myprop28.Name = "imagesparklingwineproducer";

                myprop28.Value = companydata.Element("imagesparklingwineproducer").Value;
                mypropertylist.Add(myprop28);
            }

            //new <hasdirectsales>
            if (companydata.Element("hasdirectsales") != null)
            {
                PoiProperty myprop29 = new PoiProperty();
                myprop29.Name = "hasdirectsales";

                myprop29.Value = companydata.Element("hasdirectsales").Value;
                mypropertylist.Add(myprop29);
            }

            mywinecompany.PoiProperty.TryAddOrUpdate(language, mypropertylist);
        }

        private static void ParseImporterData(string language, XElement companydata, ODHActivityPoi mywinecompany)
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

        public static ODHActivityPoi ParsetheCompanyData(ODHActivityPoi mywinecompany, IDictionary<string, XElement> companydata, List<string> haslanguage)
        {
            mywinecompany.LastChange = DateTime.Now;

            mywinecompany.Id = companydata["de"].Element("id").Value;
            mywinecompany.CustomId = companydata["de"].Element("id").Value;

            //ADD MAPPING
            var suedtirolweinid = new Dictionary<string, string>() { { "id", mywinecompany.Id } };
            mywinecompany.Mapping.TryAddOrUpdate("suedtirolwein", suedtirolweinid);

            mywinecompany.Source = "SuedtirolWein";
            mywinecompany.SyncSourceInterface = "SuedtirolWein";
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

            //importercontactInfos

            //Fix for not working Link on Suedtirol Wein (Interface returns example https://intranet.suedtirolwein.com/media/ca823aee-1aee-4746-8ff6-a69c3b68f44a/eberle-logo.jpg instead of https://suedtirolwein.com/media/b7094f82-f2e6-42ad-9533-96ca5641945d/eberlehof.jpg
            //if (mywinecompany.ContactInfos != null)
            //{
            //	foreach (var contactinfo in mywinecompany.ContactInfos.Values)
            //	{
            //                 if (!String.IsNullOrEmpty(contactinfo.LogoUrl))
            //                     contactinfo.LogoUrl = contactinfo.LogoUrl; //.Replace("intranet.", "");
            //	}
            //}

            //if (mywinecompany.ImageGallery != null)
            //{
            //	foreach (var image in mywinecompany.ImageGallery)
            //	{
            //                 if (!String.IsNullOrEmpty(image.ImageUrl))
            //                     image.ImageUrl = image.ImageUrl; //.Replace("intranet.", "");
            //	}
            //}


            return mywinecompany;
        }

        //public static ODHActivityPoi ParsetheCompanyDataPG(ODHActivityPoi mywinecompany, XElement companydata["de"], XElement companydata["it"], XElement companydata["en"], List<string> haslanguage)
        //{
        //    mywinecompany.LastChange = DateTime.Now;

        //    mywinecompany.Id = companydata["de"].Element("id").Value;
        //    mywinecompany.CustomId = companydata["de"].Element("id").Value;

        //    mywinecompany.Source = "SuedtirolWein";
        //    mywinecompany.SyncSourceInterface = "SuedtirolWein";
        //    mywinecompany.SyncUpdateMode = "Full";

        //    mywinecompany.Type = "Essen Trinken";
        //    mywinecompany.SubType = "Weinkellereien";

        //    mywinecompany.Shortname = companydata["de"].Element("title").Value;

        //    //Contactinfo DE
        //    ContactInfos contactinfode = new ContactInfos();

        //    contactinfode.CompanyName = companydata["de"].Element("title").Value;
        //    contactinfode.Address = companydata["de"].Element("address").Value;
        //    contactinfode.ZipCode = companydata["de"].Element("zipcode").Value;
        //    contactinfode.CountryCode = "IT";
        //    contactinfode.City = companydata["de"].Element("place").Value;
        //    contactinfode.CountryName = "Italien";
        //    contactinfode.Phonenumber = companydata["de"].Element("phone").Value;

        //    string webadresseDE = "";
        //    //Webadress gschicht
        //    if (!String.IsNullOrEmpty(companydata["de"].Element("homepage").Value))
        //    {
        //        webadresseDE = companydata["de"].Element("homepage").Value.Contains("http") ? companydata["de"].Element("homepage").Value : "http://" + companydata["de"].Element("homepage").Value;
        //    }

        //    contactinfode.Url = webadresseDE;
        //    contactinfode.Email = companydata["de"].Element("email").Value;
        //    contactinfode.LogoUrl = companydata["de"].Element("logo").Value;
        //    contactinfode.Language = "de";

        //    mywinecompany.ContactInfos.TryAddOrUpdate("de", contactinfode);

        //    if (haslanguage.Contains("it"))
        //    {
        //        //Contactinfo IT
        //        ContactInfos contactinfoit = new ContactInfos();

        //        contactinfoit.CompanyName = companydata["it"].Element("title").Value;
        //        contactinfoit.Address = companydata["it"].Element("address").Value;
        //        contactinfoit.ZipCode = companydata["it"].Element("zipcode").Value;
        //        contactinfoit.City = companydata["it"].Element("place").Value;
        //        contactinfoit.CountryName = "Italia";
        //        contactinfoit.CountryCode = "IT";
        //        contactinfoit.Phonenumber = companydata["it"].Element("phone").Value;

        //        string webadresseIT = "";
        //        //Webadress gschicht
        //        if (!String.IsNullOrEmpty(companydata["it"].Element("homepage").Value))
        //        {
        //            webadresseIT = companydata["it"].Element("homepage").Value.Contains("http") ? companydata["it"].Element("homepage").Value : "http://" + companydata["it"].Element("homepage").Value;
        //        }
        //        else
        //        {
        //            webadresseIT = webadresseDE;
        //        }

        //        contactinfoit.Url = webadresseIT;
        //        contactinfoit.Email = companydata["it"].Element("email").Value;
        //        contactinfoit.LogoUrl = companydata["it"].Element("logo").Value;
        //        contactinfoit.Language = "it";

        //        mywinecompany.ContactInfos.TryAddOrUpdate("it", contactinfoit);
        //    }

        //    if (haslanguage.Contains("en"))
        //    {
        //        //Contactinfo EN
        //        ContactInfos contactinfoen = new ContactInfos();

        //        contactinfoen.CompanyName = companydata["en"].Element("title").Value;
        //        contactinfoen.Address = companydata["en"].Element("address").Value;
        //        contactinfoen.ZipCode = companydata["en"].Element("zipcode").Value;
        //        contactinfoen.City = companydata["en"].Element("place").Value;
        //        contactinfoen.CountryName = "Italy";
        //        contactinfoen.CountryCode = "IT";
        //        contactinfoen.Phonenumber = companydata["en"].Element("phone").Value;

        //        string webadresseEN = "";
        //        //Webadress gschicht
        //        if (!String.IsNullOrEmpty(companydata["en"].Element("homepage").Value))
        //        {
        //            webadresseEN = companydata["en"].Element("homepage").Value.Contains("http") ? companydata["en"].Element("homepage").Value : "http://" + companydata["en"].Element("homepage").Value;
        //        }
        //        else
        //        {
        //            webadresseEN = webadresseDE;
        //        }

        //        contactinfoen.Url = webadresseEN;
        //        contactinfoen.Email = companydata["en"].Element("email").Value;
        //        contactinfoen.LogoUrl = companydata["en"].Element("logo").Value;
        //        contactinfoen.Language = "en";

        //        mywinecompany.ContactInfos.TryAddOrUpdate("en", contactinfoen);
        //    }

        //    //Detail DE
        //    Detail mydetailde = new Detail();

        //    if (mywinecompany.Detail != null)
        //        if (mywinecompany.Detail.ContainsKey("de"))
        //            mydetailde = mywinecompany.Detail["de"];

        //    mydetailde.Title = companydata["de"].Element("title").Value; ;
        //    mydetailde.BaseText = companydata["de"].Element("content").Value;
        //    //mydetailde.AdditionalText = companydata["de"].Element("wines").Value;
        //    mydetailde.Language = "de";
        //    mywinecompany.Detail.TryAddOrUpdate("de", mydetailde);

        //    if (haslanguage.Contains("it"))
        //    {
        //        //Detail IT
        //        Detail mydetailit = new Detail();

        //        if (mywinecompany.Detail != null)
        //            if (mywinecompany.Detail.ContainsKey("it"))
        //                mydetailit = mywinecompany.Detail["it"];

        //        mydetailit.Title = companydata["it"].Element("title").Value; ;
        //        mydetailit.BaseText = companydata["it"].Element("content").Value;
        //        //mydetailit.AdditionalText = companydata["it"].Element("wines").Value;
        //        mydetailit.Language = "it";
        //        mywinecompany.Detail.TryAddOrUpdate("it", mydetailit);
        //    }

        //    if (haslanguage.Contains("en"))
        //    {
        //        //Detail EN
        //        Detail mydetailen = new Detail();

        //        if (mywinecompany.Detail != null)
        //            if (mywinecompany.Detail.ContainsKey("en"))
        //                mydetailen = mywinecompany.Detail["en"];

        //        mydetailen.Title = companydata["en"].Element("title").Value; ;
        //        mydetailen.BaseText = companydata["en"].Element("content").Value;
        //        //mydetailen.AdditionalText = companydata["en"].Element("wines").Value;
        //        mydetailen.Language = "en";
        //        mywinecompany.Detail.TryAddOrUpdate("en", mydetailen);
        //    }


        //    //ImageGallery
        //    List<ImageGallery> myimagegallerylist = new List<ImageGallery>();

        //    if (!String.IsNullOrEmpty(companydata["de"].Element("media").Value))
        //    {
        //        ImageGallery myimagegallery = new ImageGallery();
        //        myimagegallery.ImageUrl = companydata["de"].Element("media").Value;
        //        myimagegallery.ImageSource = "SuedtirolWein";
        //        myimagegallery.IsInGallery = true;
        //        myimagegallery.ListPosition = 0;
        //        myimagegallery.CopyRight = "Suedtirol Wein";

        //        myimagegallerylist.Add(myimagegallery);
        //    }
        //    if (!String.IsNullOrEmpty(companydata["de"].Element("mediadetail").Value))
        //    {
        //        ImageGallery myimagegallery2 = new ImageGallery();
        //        myimagegallery2.ImageUrl = companydata["de"].Element("mediadetail").Value;
        //        myimagegallery2.ImageSource = "SuedtirolWein";
        //        myimagegallery2.IsInGallery = true;
        //        myimagegallery2.ListPosition = 1;
        //        myimagegallery2.CopyRight = "Suedtirol Wein";
        //        myimagegallerylist.Add(myimagegallery2);
        //    }
        //    if (!String.IsNullOrEmpty(companydata["de"].Element("logo").Value))
        //    {
        //        ImageGallery myimagegallery3 = new ImageGallery();
        //        myimagegallery3.ImageUrl = companydata["de"].Element("logo").Value;
        //        myimagegallery3.ImageSource = "SuedtirolWein";
        //        myimagegallery3.IsInGallery = true;
        //        myimagegallery3.ListPosition = 2;
        //        myimagegallery3.CopyRight = "Suedtirol Wein";
        //        myimagegallerylist.Add(myimagegallery3);
        //    }

        //    mywinecompany.ImageGallery = myimagegallerylist;

        //    //PROPS DE
        //    List<PoiProperty> mypropertylist = new List<PoiProperty>();
        //    if (!String.IsNullOrEmpty(companydata["de"].Element("openingtimeswineshop").Value))
        //    {
        //        PoiProperty myprop = new PoiProperty();
        //        myprop.Name = "openingtimeswineshop";
        //        myprop.Value = companydata["de"].Element("openingtimeswineshop").Value;
        //        mypropertylist.Add(myprop);
        //    }
        //    if (!String.IsNullOrEmpty(companydata["de"].Element("openingtimesguides").Value))
        //    {
        //        PoiProperty myprop2 = new PoiProperty();
        //        myprop2.Name = "openingtimesguides";
        //        myprop2.Value = companydata["de"].Element("openingtimesguides").Value;
        //        mypropertylist.Add(myprop2);
        //    }
        //    if (!String.IsNullOrEmpty(companydata["de"].Element("openingtimesgastronomie").Value))
        //    {
        //        PoiProperty myprop3 = new PoiProperty();
        //        myprop3.Name = "openingtimesgastronomie";
        //        myprop3.Value = companydata["de"].Element("openingtimesgastronomie").Value;
        //        mypropertylist.Add(myprop3);
        //    }
        //    if (!String.IsNullOrEmpty(companydata["de"].Element("companyholiday").Value))
        //    {
        //        PoiProperty myprop4 = new PoiProperty();
        //        myprop4.Name = "companyholiday";
        //        myprop4.Value = companydata["de"].Element("companyholiday").Value;
        //        mypropertylist.Add(myprop4);
        //    }

        //    //De sochen glabi war besser als boolwerte ozuspeichern lei schaugn wohin.............
        //    //hallo hasvisits, hasovernichtgs, hasbiowine........

        //    if (!String.IsNullOrEmpty(companydata["de"].Element("hasvisits").Value))
        //    {
        //        PoiProperty myprop5 = new PoiProperty();
        //        myprop5.Name = "hasvisits";

        //        //string hasvisitsstr = "false";

        //        //if (companydata["de"].Element("hasvisits").Value == "True")
        //        //    hasvisitsstr = "true";

        //        myprop5.Value = companydata["de"].Element("hasvisits").Value;
        //        mypropertylist.Add(myprop5);
        //    }
        //    if (!String.IsNullOrEmpty(companydata["de"].Element("hasovernights").Value))
        //    {
        //        PoiProperty myprop6 = new PoiProperty();
        //        myprop6.Name = "hasovernights";

        //        //string hasovernightsstr = "nein";

        //        //if (companydata["de"].Element("hasovernights").Value == "True")
        //        //    hasovernightsstr = "ja";

        //        myprop6.Value = companydata["de"].Element("hasovernights").Value;
        //        mypropertylist.Add(myprop6);
        //    }
        //    if (!String.IsNullOrEmpty(companydata["de"].Element("hasbiowine").Value))
        //    {
        //        PoiProperty myprop7 = new PoiProperty();
        //        myprop7.Name = "hasbiowine";

        //        //string hasbiowinestr = "nein";

        //        //if (companydata["de"].Element("hasbiowine").Value == "True")
        //        //    hasbiowinestr = "ja";

        //        myprop7.Value = companydata["de"].Element("hasbiowine").Value;
        //        mypropertylist.Add(myprop7);
        //    }
        //    if (!String.IsNullOrEmpty(companydata["de"].Element("wines").Value))
        //    {
        //        PoiProperty myprop8 = new PoiProperty();
        //        myprop8.Name = "wines";
        //        myprop8.Value = companydata["de"].Element("wines").Value;
        //        mypropertylist.Add(myprop8);
        //    }

        //    mywinecompany.PoiProperty.TryAddOrUpdate("de", mypropertylist);

        //    if (haslanguage.Contains("it"))
        //    {

        //        //PROPS IT
        //        List<PoiProperty> mypropertylistit = new List<PoiProperty>();
        //        if (!String.IsNullOrEmpty(companydata["it"].Element("openingtimeswineshop").Value))
        //        {
        //            PoiProperty myprop = new PoiProperty();
        //            myprop.Name = "openingtimeswineshop";
        //            myprop.Value = companydata["it"].Element("openingtimeswineshop").Value;
        //            mypropertylistit.Add(myprop);
        //        }
        //        if (!String.IsNullOrEmpty(companydata["it"].Element("openingtimesguides").Value))
        //        {
        //            PoiProperty myprop2 = new PoiProperty();
        //            myprop2.Name = "openingtimesguides";
        //            myprop2.Value = companydata["it"].Element("openingtimesguides").Value;
        //            mypropertylistit.Add(myprop2);
        //        }
        //        if (!String.IsNullOrEmpty(companydata["it"].Element("openingtimesgastronomie").Value))
        //        {
        //            PoiProperty myprop3 = new PoiProperty();
        //            myprop3.Name = "openingtimesgastronomie";
        //            myprop3.Value = companydata["it"].Element("openingtimesgastronomie").Value;
        //            mypropertylistit.Add(myprop3);
        //        }
        //        if (!String.IsNullOrEmpty(companydata["it"].Element("companyholiday").Value))
        //        {
        //            PoiProperty myprop4 = new PoiProperty();
        //            myprop4.Name = "companyholiday";
        //            myprop4.Value = companydata["it"].Element("companyholiday").Value;
        //            mypropertylistit.Add(myprop4);
        //        }
        //        if (!String.IsNullOrEmpty(companydata["it"].Element("hasvisits").Value))
        //        {
        //            PoiProperty myprop5 = new PoiProperty();
        //            myprop5.Name = "hasvisits";

        //            //string hasvisitsstr = "no";

        //            //if (companydata["it"].Element("hasvisits").Value == "True")
        //            //    hasvisitsstr = "si";

        //            myprop5.Value = companydata["it"].Element("hasvisits").Value;
        //            mypropertylistit.Add(myprop5);
        //        }
        //        if (!String.IsNullOrEmpty(companydata["it"].Element("hasovernights").Value))
        //        {
        //            PoiProperty myprop6 = new PoiProperty();
        //            myprop6.Name = "hasovernights";

        //            //string hasovernightsstr = "no";

        //            //if (companydata["it"].Element("hasovernights").Value == "True")
        //            //    hasovernightsstr = "si";

        //            myprop6.Value = companydata["it"].Element("hasovernights").Value;
        //            mypropertylistit.Add(myprop6);
        //        }
        //        if (!String.IsNullOrEmpty(companydata["it"].Element("hasbiowine").Value))
        //        {
        //            PoiProperty myprop7 = new PoiProperty();
        //            myprop7.Name = "hasbiowine";

        //            //string hasbiowinestr = "no";

        //            //if (companydata["it"].Element("hasbiowine").Value == "True")
        //            //    hasbiowinestr = "si";

        //            myprop7.Value = companydata["it"].Element("hasbiowine").Value;
        //            mypropertylistit.Add(myprop7);
        //        }
        //        if (!String.IsNullOrEmpty(companydata["it"].Element("wines").Value))
        //        {
        //            PoiProperty myprop8 = new PoiProperty();
        //            myprop8.Name = "wines";
        //            myprop8.Value = companydata["it"].Element("wines").Value;
        //            mypropertylistit.Add(myprop8);
        //        }

        //        mywinecompany.PoiProperty.TryAddOrUpdate("it", mypropertylistit);
        //    }

        //    if (haslanguage.Contains("en"))
        //    {

        //        //PROPS EN
        //        List<PoiProperty> mypropertylisten = new List<PoiProperty>();
        //        if (!String.IsNullOrEmpty(companydata["en"].Element("openingtimeswineshop").Value))
        //        {
        //            PoiProperty myprop = new PoiProperty();
        //            myprop.Name = "openingtimeswineshop";
        //            myprop.Value = companydata["en"].Element("openingtimeswineshop").Value;
        //            mypropertylisten.Add(myprop);
        //        }
        //        if (!String.IsNullOrEmpty(companydata["en"].Element("openingtimesguides").Value))
        //        {
        //            PoiProperty myprop2 = new PoiProperty();
        //            myprop2.Name = "openingtimesguides";
        //            myprop2.Value = companydata["en"].Element("openingtimesguides").Value;
        //            mypropertylisten.Add(myprop2);
        //        }
        //        if (!String.IsNullOrEmpty(companydata["en"].Element("openingtimesgastronomie").Value))
        //        {
        //            PoiProperty myprop3 = new PoiProperty();
        //            myprop3.Name = "openingtimesgastronomie";
        //            myprop3.Value = companydata["en"].Element("openingtimesgastronomie").Value;
        //            mypropertylisten.Add(myprop3);
        //        }
        //        if (!String.IsNullOrEmpty(companydata["en"].Element("companyholiday").Value))
        //        {
        //            PoiProperty myprop4 = new PoiProperty();
        //            myprop4.Name = "companyholiday";
        //            myprop4.Value = companydata["en"].Element("companyholiday").Value;
        //            mypropertylisten.Add(myprop4);
        //        }
        //        if (!String.IsNullOrEmpty(companydata["en"].Element("hasvisits").Value))
        //        {
        //            PoiProperty myprop5 = new PoiProperty();
        //            myprop5.Name = "hasvisits";

        //            //string hasvisitsstr = "no";

        //            //if (companydata["en"].Element("hasvisits").Value == "True")
        //            //    hasvisitsstr = "yes";

        //            myprop5.Value = companydata["en"].Element("hasvisits").Value;
        //            mypropertylisten.Add(myprop5);
        //        }
        //        if (!String.IsNullOrEmpty(companydata["en"].Element("hasovernights").Value))
        //        {
        //            PoiProperty myprop6 = new PoiProperty();
        //            myprop6.Name = "hasovernights";

        //            //string hasovernightsstr = "no";

        //            //if (companydata["en"].Element("hasovernights").Value == "True")
        //            //    hasovernightsstr = "yes";

        //            myprop6.Value = companydata["en"].Element("hasovernights").Value;
        //            mypropertylisten.Add(myprop6);
        //        }
        //        if (!String.IsNullOrEmpty(companydata["en"].Element("hasbiowine").Value))
        //        {
        //            PoiProperty myprop7 = new PoiProperty();
        //            myprop7.Name = "hasbiowine";

        //            //string hasbiowinestr = "no";

        //            //if (companydata["en"].Element("hasbiowine").Value == "True")
        //            //    hasbiowinestr = "yes";

        //            myprop7.Value = companydata["en"].Element("hasbiowine").Value;
        //            mypropertylisten.Add(myprop7);
        //        }
        //        if (!String.IsNullOrEmpty(companydata["en"].Element("wines").Value))
        //        {
        //            PoiProperty myprop8 = new PoiProperty();
        //            myprop8.Name = "wines";
        //            myprop8.Value = companydata["en"].Element("wines").Value;
        //            mypropertylisten.Add(myprop8);
        //        }

        //        mywinecompany.PoiProperty.TryAddOrUpdate("en", mypropertylisten);
        //    }

        //    //Wineids

        //    if (!String.IsNullOrEmpty(companydata["de"].Element("wineids").Value))
        //    {
        //        List<string> poiServices = new List<string>();
        //        var wineids = companydata["de"].Element("wines").Value.Split(',');
        //        poiServices = wineids.ToList();
        //        mywinecompany.PoiServices = poiServices;
        //    }



        //    if (!String.IsNullOrEmpty(companydata["de"].Element("latidude").Value) && !String.IsNullOrEmpty(companydata["de"].Element("longitude").Value))
        //    {
        //        List<GpsInfo> gpsinfolist = new List<GpsInfo>();
        //        GpsInfo mygps = new GpsInfo();
        //        mygps.Latitude = Convert.ToDouble(companydata["de"].Element("latidude").Value, CultureInfo.InvariantCulture);
        //        mygps.Longitude = Convert.ToDouble(companydata["de"].Element("longitude").Value, CultureInfo.InvariantCulture);
        //        mygps.Gpstype = "position";
        //        gpsinfolist.Add(mygps);
        //        mywinecompany.GpsInfo = gpsinfolist;
        //        //Locationinfo in basis auf GPSPunkt ausrechnen!!! mochmer dernoch


        //        mywinecompany.GpsPoints.TryAddOrUpdate("position", mygps);

        //    }

        //    //Fix for not working Link on Suedtirol Wein (Interface returns example https://intranet.suedtirolwein.com/media/ca823aee-1aee-4746-8ff6-a69c3b68f44a/eberle-logo.jpg instead of https://suedtirolwein.com/media/b7094f82-f2e6-42ad-9533-96ca5641945d/eberlehof.jpg
        //    if (mywinecompany.ContactInfos != null)
        //    {
        //        foreach (var contactinfo in mywinecompany.ContactInfos.Values)
        //        {
        //            if (!String.IsNullOrEmpty(contactinfo.LogoUrl))
        //                contactinfo.LogoUrl = contactinfo.LogoUrl.Replace("intranet.", "");
        //        }
        //    }

        //    if (mywinecompany.ImageGallery != null)
        //    {
        //        foreach (var image in mywinecompany.ImageGallery)
        //        {
        //            if (!String.IsNullOrEmpty(image.ImageUrl))
        //                image.ImageUrl = image.ImageUrl.Replace("intranet.", "");
        //        }
        //    }

        //    return mywinecompany;
        //}


    }
}
