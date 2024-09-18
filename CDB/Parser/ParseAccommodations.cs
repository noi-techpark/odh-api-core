// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DataModel;
using Helper;

namespace CDB.Parser
{
    public class ParseAccommodations
    {        
        public static CultureInfo myculture = new CultureInfo("en");
        
        public static AccommodationLinked ParseMyAccommodation(XDocument myresponse,
            List<string> languages,
            string A0RID,
            AccommodationLinked myacco,
            bool newacco, string xmldir,
            XDocument mytypes,
            XDocument mycategories,
            XDocument myboards,
            XDocument myfeatures,
            XDocument mybookingchannels,
            XDocument myvinumlist,
            XDocument mywinelist,
            XDocument mycitylist,
            XDocument myskiarealist,
            XDocument mymediterranenlist,
            XDocument dolomiteslist,
            XDocument alpinelist,
            XDocument roomamenitylist
            )
        {
            try
            {           
                var definitionhead = myresponse.Root.Element("HotelDefinition").Element("Head");

                //ACHTUNG HIER MUSS noch das mehrsprachige implementiert werden.
                var category = myresponse.Root.Element("HotelCategory").Element("Head").Element("Data"); //OK
                var adress = myresponse.Root.Element("HotelAddress").Element("Head").Element("Data"); //OK
                var publicity = myresponse.Root.Element("HotelPublicity").Element("Head").Elements("Data"); //OK
                var definition = myresponse.Root.Element("HotelDefinition").Element("Head").Element("Data"); //OK
                var coords = myresponse.Root.Element("HotelAddressCoord").HasElements ? myresponse.Root.Element("HotelAddressCoord").Element("Head").Element("Data") : null; //OK
                var webdata = myresponse.Root.Element("HotelWebData").Element("Head").Elements("Data"); //OK
                var images = myresponse.Root.Element("HotelFoto").Element("Head").Elements("Data");
                var gallery = myresponse.Root.Element("HotelGallery").Element("Head").Element("Data");
                var position = myresponse.Root.Element("HotelPosition").Element("Head").Element("Data");
                var tin = myresponse.Root.Element("HotelTin").Element("Head").Elements("Data");
                var hotelvg = myresponse.Root.Element("HotelVG").Element("Head").Elements("Data");
                var hotelmg = myresponse.Root.Element("HotelMG").Element("Head").Elements("Data");
                var hotelpos = myresponse.Root.Element("HotelPOS").Element("Head").Elements("Data");
                var ratings = myresponse.Root.Element("HotelRatings").Element("Head").Element("Data");

                Console.ForegroundColor = ConsoleColor.Green;

                DateTime firstimported = DateTime.MinValue;

                List<string> additionalfeaturestoadd = new List<string>();

                string vatnumber = "";

                if (newacco == true)
                {
                    myacco = new AccommodationLinked();
                    newacco = true;
                    myacco.Id = A0RID;
                    myacco.FirstImport = DateTime.Now;
                    myacco.LastChange = DateTime.Now;
                    myacco.Active = true;

                    myacco.HasLanguage = new List<string>() { "de", "it", "en" };
                }
                else
                {
                    myacco.LastChange = DateTime.Now;
                    myacco.FirstImport = myacco.FirstImport;
                    myacco.Active = myacco.Active;

                    if (myacco.HasLanguage == null)
                        myacco.HasLanguage = new List<string>() { "de", "it", "en" };
                }

                myacco.Source = "LTS";

                if (definition != null)
                {
                    //if (definition.Attribute("A0TAUnits") != null)
                    //    myacco.Units = Convert.ToInt32(definition.Attribute("A0TAUnits").Value);
                    //else
                    //    myacco.Units = 0;

                    //if (definition.Attribute("A0TABeds") != null)
                    //    myacco.Beds = Convert.ToInt32(definition.Attribute("A0TABeds").Value);
                    //else
                    //    myacco.Beds = 0;

                    //myacco.Units = 0;
                    //myacco.Beds = 0;

                    if (definition.Attribute("A0Ene") != null)
                    {
                        if (definition.Attribute("A0Ene").Value == "1")
                            myacco.Active = true;
                        else
                            myacco.Active = false;
                    }

                    if (definition.Attribute("IDMActive") != null)
                    {
                        if (definition.Attribute("IDMActive").Value == "1")
                            myacco.SmgActive = true;
                        else
                            myacco.SmgActive = false;
                    }

                    if (definition.Attribute("A0Apa") != null)
                    {
                        if (definition.Attribute("A0Apa").Value == "1")
                            myacco.HasApartment = true;
                        else
                            myacco.HasApartment = false;
                    }

                    if (definition.Attribute("A0Roo") != null)
                    {
                        if (definition.Attribute("A0Roo").Value == "1")
                            myacco.HasRoom = true;
                        else
                            myacco.HasRoom = false;
                    }

                    if (definition.Attribute("A0MTV") != null)
                    {
                        if (definition.Attribute("A0MTV").Value == "1")
                            myacco.TVMember = true;
                        else
                            myacco.TVMember = false;
                    }

                    if (definition.Attribute("A0Cmp") != null)
                    {
                        if (definition.Attribute("A0Cmp").Value == "1")
                            myacco.IsCamping = true;
                        else
                            myacco.IsCamping = false;
                    }

                    if (definition.Attribute("A0Gst") != null)
                    {
                        if (definition.Attribute("A0Gst").Value == "1")
                            myacco.IsGastronomy = true;
                        else
                            myacco.IsGastronomy = false;
                    }

                    if (definition.Attribute("A0Acc") != null)
                    {
                        if (definition.Attribute("A0Acc").Value == "1")
                            myacco.IsAccommodation = true;
                        else
                            myacco.IsAccommodation = false;
                    }

                    if (definition.Attribute("A0Rep") != null)
                    {
                        if (definition.Attribute("A0Rep") != null)
                        {
                            var representation = 0;
                            Int32.TryParse(definition.Attribute("A0Rep").Value, out representation);

                            myacco.Representation = representation;
                        }
                    }


                    if (definition.Attribute("A0TVatNo") != null)
                    {
                        vatnumber = definition.Attribute("A0TVatNo").Value;
                    }

                    myacco.DistrictId = definition.Attribute("S7RID").Value;

                    myacco.MainLanguage = definition.Attribute("LngID").Value;

                }

                if (category != null)
                {
                    //Setting Type

                    string ltsTypeRid = category.Attribute("T4RID").Value;

                    var mytype = mytypes.Root.Elements("AccoType").Where(x => x.Attribute("RID").Value == ltsTypeRid).FirstOrDefault().Attribute("SmgType").Value;
                    myacco.AccoTypeId = mytype;

                    additionalfeaturestoadd.Add(ltsTypeRid);

                    //Setting Category

                    string ltsCatRid = category.Attribute("T6RID").Value;

                    var mycategory = mycategories.Root.Elements("Data").Where(x => x.Attribute("T0RID").Value == ltsCatRid).FirstOrDefault().Elements("DataLng").Where(x => x.Attribute("LngID").Value == "EN").FirstOrDefault().Attribute("T1Des").Value;
                    myacco.AccoCategoryId = mycategory;

                    additionalfeaturestoadd.Add(ltsCatRid);

                    //Setting Board Infos
                    var boardings = category.Elements("Board");

                    List<string> accoboardings = new List<string>();
                   
                    foreach (XElement myboardelement in boardings)
                    {
                        string boardrid = myboardelement.Attribute("T8RID").Value;

                        additionalfeaturestoadd.Add(boardrid);

                        var myboard = myboards.Root.Elements("Data").Where(x => x.Attribute("T0RID").Value == boardrid).FirstOrDefault().Elements("DataLng").Where(x => x.Attribute("LngID").Value == "EN").FirstOrDefault().Attribute("T1Des").Value;

                        if (myboard != null)
                            accoboardings.Add(myboard);
                    }
                    myacco.BoardIds = accoboardings.ToList();
                }

                List<AccoFeatureLinked> featurelist = new List<AccoFeatureLinked>();

                if (tin.Count() > 0)
                { 
                    //Features
                    foreach (XElement thetin in tin)
                    {
                        string tinrid = thetin.Attribute("T0RID").Value;

                        var myfeature = myfeatures.Root.Elements("Data").Where(x => x.Attribute("T0RID").Value == tinrid).FirstOrDefault();

                        if (myfeature != null)
                        {
                            var myfeatureparsed = myfeature.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "EN").FirstOrDefault();

                            if (myfeatureparsed != null)
                            {
                                var myfeatureparsed2 = myfeatureparsed.Attribute("T1Des").Value;

                                //Getting HGV ID if available

                                string hgvamenityid = "";

                                //var myamenity = roomamenitylist.Root.Elements("amenity").Elements("ltsrid").Where(x => x.Value == tinrid).FirstOrDefault();

                                var myamenity = roomamenitylist.Root.Elements("amenity").Where(x => x.Element("ltsrid").Value == tinrid).FirstOrDefault();

                                if (myamenity != null)
                                    hgvamenityid = myamenity.Element("hgvid").Value;


                                if (myfeatureparsed2 != null)
                                    featurelist.Add(new AccoFeatureLinked() { Id = tinrid, Name = myfeatureparsed2, HgvId = hgvamenityid });
                            }
                        }
                        
                    }
                }
                //Add Category, Board and Type to features
                foreach (var featuretoadd in additionalfeaturestoadd)
                {
                    var myfeature = myfeatures.Root.Elements("Data").Where(x => x.Attribute("T0RID").Value == featuretoadd).FirstOrDefault();

                    if (myfeature != null)
                    {
                        var myfeatureparsed = myfeature.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "EN").FirstOrDefault();

                        if (myfeatureparsed != null)
                        {
                            var myfeatureparsed2 = myfeatureparsed.Attribute("T1Des").Value;

                            //Getting HGV ID if available

                            string hgvamenityid = "";

                            //var myamenity = roomamenitylist.Root.Elements("amenity").Elements("ltsrid").Where(x => x.Value == tinrid).FirstOrDefault();

                            var myamenity = roomamenitylist.Root.Elements("amenity").Where(x => x.Element("ltsrid").Value == featuretoadd).FirstOrDefault();

                            if (myamenity != null)
                                hgvamenityid = myamenity.Element("hgvid").Value;

                            if (myfeatureparsed2 != null)
                                featurelist.Add(new AccoFeatureLinked() { Id = featuretoadd, Name = myfeatureparsed2, HgvId = hgvamenityid });
                        }
                    }
                }

                myacco.Features = featurelist.ToList();

                List<AccoDetail> myaccodetailslist = new List<AccoDetail>();
                
                //Details            
                foreach (string mylang in languages)
                {
                    AccoDetail mydetail = new AccoDetail();

                    if (adress != null)
                    {
                        //De Adress
                        mydetail.Language = mylang;

                        var adresslng = adress.Elements("AddressLng").Where(x => x.Attribute("LngID").Value == mylang.ToUpper()).FirstOrDefault();

                        if (adresslng != null)
                        {
                            mydetail.CountryCode = "IT";
                            mydetail.City = adresslng.Attribute("A2Cit").Value;
                            mydetail.Email = adresslng.Attribute("A2EMa").Value;
                            mydetail.NameAddition = adresslng.Attribute("A2Na2").Value;
                            mydetail.Name = adresslng.Attribute("A2Nam").Value;
                            mydetail.Vat = vatnumber;

                            if (mylang.ToUpper() == "DE")
                                myacco.Shortname = adresslng.Attribute("A2Nam").Value;

                            if (String.IsNullOrEmpty(myacco.Shortname))
                            {
                                myacco.Shortname = "No Name";
                                myacco.SmgActive = false;
                            }

                            mydetail.Street = adresslng.Attribute("A2Str").Value;
                        }
                        mydetail.Fax = adress.Attribute("A1Fax").Value;
                        mydetail.Firstname = adress.Attribute("A1FNa").Value;
                        mydetail.Lastname = adress.Attribute("A1LNa").Value;
                        mydetail.Mobile = adress.Attribute("A1Mob").Value;


                        mydetail.Phone = adress.Attribute("A1Pho").Value;
                        mydetail.Zip = adress.Attribute("A1ZIP").Value;                     
                    }
                    if (publicity.Count() > 0)
                    {                        
                        var mydesc = publicity.Where(x => x.Attribute("LngID").Value == mylang.ToUpper()).FirstOrDefault();

                        if (mydesc != null)
                        {
                            mydetail.Longdesc = mydesc.Attribute("A7LoT").Value;
                            mydetail.Shortdesc = mydesc.Attribute("A7ShT").Value;
                        }
                    }
                    if (webdata.Count() > 0)
                    {
                        var mywebdata = webdata.Where(x => x.Attribute("LngID").Value == mylang.ToUpper()).FirstOrDefault();

                        if (mywebdata != null)
                        {
                            mydetail.Website = mywebdata.Attribute("A3Web").Value; ;                            
                        }
                    }                    

                    myacco.AccoDetail.TryAddOrUpdate(mylang, mydetail);
                }
                
                if (coords != null)
                {
                    myacco.GpsInfo = new List<GpsInfo>();

                    GpsInfo myinfo = new GpsInfo();

                    myinfo.Gpstype = "position";

                    myinfo.Longitude = Convert.ToDouble(coords.Attribute("A1GEP").Value, myculture);
                    myinfo.Latitude = Convert.ToDouble(coords.Attribute("A1GNP").Value, myculture);

                    if (position != null)
                    {
                        myinfo.Altitude = Convert.ToDouble(position.Attribute("A0Alt").Value, myculture);
                        myinfo.AltitudeUnitofMeasure = "m";
                    }

                    myacco.GpsInfo.Add(myinfo);
                }
                else if (coords == null)
                {
                    myacco.GpsInfo = null;                 
                }

                if (ratings != null)
                {
                    myacco.TrustYouID = ratings.Attribute("A20ID").Value;
                    myacco.TrustYouResults = Convert.ToInt32(ratings.Attribute("A20Cou").Value);

                    double ratingresult = 0;

                    var style = NumberStyles.AllowDecimalPoint;

                    if (Double.TryParse(ratings.Attribute("A20Rat").Value, style, myculture, out ratingresult))
                        myacco.TrustYouScore = ratingresult * 10; //Fix because i didn't save the comma. and the app is working with example to display 94 --> 940
                    else
                        myacco.TrustYouScore = 0;

                    if (ratings.Attribute("A20Ene").Value == "1")
                        myacco.TrustYouActive = true;
                    else if (ratings.Attribute("A20Ene").Value == "0")
                        myacco.TrustYouActive = false;

                    myacco.TrustYouState = Convert.ToInt32(ratings.Attribute("A20Sta").Value);                    
                }
                else if (ratings == null)
                {
                    myacco.TrustYouID = null;
                    myacco.TrustYouResults = 0;
                    myacco.TrustYouScore = 0;                    
                }

                List<ImageGallery> imagegallerylist = new List<ImageGallery>();

                if (images != null)
                {
                    ///Summer and Winter Image
                    foreach (var theimage in images)
                    {
                        ImageGallery mainimagede = new ImageGallery();

                        //DE
                        mainimagede.ImageUrl = theimage.Attribute("A8Fot").Value;
                        //if (theimage.Attribute("A8ID").Value == "1")
                        //    mainimagede.IsInGallery = true;
                        //else
                        //    mainimagede.IsInGallery = false;
                        mainimagede.IsInGallery = true;


                        //mainimagede.Language = "de";
                        mainimagede.Height = Convert.ToInt32(theimage.Attribute("A8PxH").Value);
                        mainimagede.Width = Convert.ToInt32(theimage.Attribute("A8PxW").Value);
                        mainimagede.ValidFrom = Convert.ToDateTime(theimage.Attribute("A8VaF").Value);
                        mainimagede.ValidTo = Convert.ToDateTime(theimage.Attribute("A8VaT").Value);
                        mainimagede.ListPosition = 0;
                        mainimagede.ImageDesc.TryAddOrUpdate("de", theimage.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "DE").Count() > 0 ? theimage.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "DE").FirstOrDefault().Attribute("A9Nam").Value : "");
                        mainimagede.ImageDesc.TryAddOrUpdate("it", theimage.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "IT").Count() > 0 ? theimage.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "IT").FirstOrDefault().Attribute("A9Nam").Value : "");
                        mainimagede.ImageDesc.TryAddOrUpdate("en", theimage.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "EN").Count() > 0 ? theimage.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "EN").FirstOrDefault().Attribute("A9Nam").Value : "");

                        mainimagede.CopyRight = theimage.Attribute("A8Cop") != null ? theimage.Attribute("A8Cop").Value : "";

                        mainimagede.License = theimage.Attribute("S31Cod") != null ? theimage.Attribute("S31Cod").Value : "";

                        //New Check date and give Image Tag
                        if (mainimagede.ValidFrom != null && mainimagede.ValidTo != null)
                        {
                            List<string> imagetaglist = new List<string>();

                            //Date is set 
                            var checkbegindate = ((DateTime)mainimagede.ValidFrom).Date;
                            var checkenddate = ((DateTime)mainimagede.ValidTo).Date;

                            var summer = new DateTime(mainimagede.ValidFrom.Value.Year, 7, 15).Date;
                            var winter = new DateTime(mainimagede.ValidTo.Value.Year, 1, 15).Date;

                            //check if date is into 15.07
                            if (summer >= checkbegindate && summer <= checkenddate)
                                imagetaglist.Add("Summer");

                            //check if date is into 15.01
                            if (winter >= checkbegindate && winter <= checkenddate)
                                imagetaglist.Add("Winter");

                            mainimagede.ImageTags = imagetaglist;
                        }

                        //TODO Add Image ID from URL
                       


                        imagegallerylist.Add(mainimagede);
                    }
                    
                }
                
                if (gallery != null)
                {
                    foreach (XElement galleryelement in gallery.Elements("Foto"))
                    {                        
                        //DE

                        ImageGallery mygallery = new ImageGallery();

                        mygallery.ImageUrl = galleryelement.Attribute("A15Fot").Value;
                        mygallery.ListPosition = Convert.ToInt32(galleryelement.Attribute("A15ID").Value);

                        if (galleryelement.Attribute("A15Ene").Value == "1")
                            mygallery.IsInGallery = true;
                        else
                            mygallery.IsInGallery = false;

                        mygallery.Height = Convert.ToInt32(galleryelement.Attribute("A15PxW").Value);
                        mygallery.Width = Convert.ToInt32(galleryelement.Attribute("A15PxH").Value);

                        mygallery.CopyRight = galleryelement.Attribute("A15Cop") != null ? galleryelement.Attribute("A15Cop").Value : "";
                        mygallery.License = galleryelement.Attribute("S31Cod") != null ? galleryelement.Attribute("S31Cod").Value : "";
                        
                        if (galleryelement.Elements("FotoLng").Where(x => x.Attribute("LngID").Value == "DE").Count() > 0)
                        {
                            mygallery.ImageDesc.TryAddOrUpdate("de", galleryelement.Elements("FotoLng").Where(x => x.Attribute("LngID").Value == "DE").Count() > 0 ? galleryelement.Elements("FotoLng").Where(x => x.Attribute("LngID").Value == "DE").FirstOrDefault().Attribute("A16Nam").Value : "");
                            mygallery.ImageDesc.TryAddOrUpdate("it", galleryelement.Elements("FotoLng").Where(x => x.Attribute("LngID").Value == "IT").Count() > 0 ? galleryelement.Elements("FotoLng").Where(x => x.Attribute("LngID").Value == "IT").FirstOrDefault().Attribute("A16Nam").Value : "");
                            mygallery.ImageDesc.TryAddOrUpdate("en", galleryelement.Elements("FotoLng").Where(x => x.Attribute("LngID").Value == "EN").Count() > 0 ? galleryelement.Elements("FotoLng").Where(x => x.Attribute("LngID").Value == "EN").FirstOrDefault().Attribute("A16Nam").Value : "");
                        }
                        else if (gallery.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "DE").Count() > 0)
                        {
                            mygallery.ImageDesc.TryAddOrUpdate("de", gallery.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "DE").Count() > 0 ? gallery.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "DE").FirstOrDefault().Attribute("A14Nam").Value : "");
                            mygallery.ImageDesc.TryAddOrUpdate("it", gallery.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "IT").Count() > 0 ? gallery.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "IT").FirstOrDefault().Attribute("A14Nam").Value : "");
                            mygallery.ImageDesc.TryAddOrUpdate("en", gallery.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "EN").Count() > 0 ? gallery.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "EN").FirstOrDefault().Attribute("A14Nam").Value : "");
                        }
                        else
                        {
                            mygallery.ImageDesc.TryAddOrUpdate("de", "");
                            mygallery.ImageDesc.TryAddOrUpdate("it", "");
                            mygallery.ImageDesc.TryAddOrUpdate("en", "");
                        }

                        imagegallerylist.Add(mygallery);
                    }                    
                }

                myacco.ImageGallery = imagegallerylist.ToList();

                if (myacco.ImageGallery.Count() > 0)
                {
                    myacco.ImageGallery = myacco.ImageGallery.OrderBy(x => x.ListPosition).ToList();
                }
                
                string hgvid = null;

                if (hotelpos.Count() > 0)
                {
                    bool bookable = false;

                    List<AccoBookingChannel> mybookingchannelslist = new List<AccoBookingChannel>();

                    foreach (XElement pos in hotelpos)
                    {
                        AccoBookingChannel mybooking = new AccoBookingChannel();

                        string pos1id = pos.Attribute("POS1ID").Value;
                        var bookingportal = mybookingchannels.Root.Elements("BookingChannel").Where(x => x.Attribute("Pos1ID").Value == pos1id).FirstOrDefault();
                        
                        if (pos1id == "2")
                            hgvid = pos.Attribute("POS2HotelRID").Value;

                        mybooking.Pos1ID = pos1id;
                        mybooking.Portalname = bookingportal.Attribute("Portalname").Value;
                        mybooking.Id = bookingportal.Attribute("Shortname").Value;
                        mybooking.BookingId = pos.Attribute("POS2HotelRID").Value;

                        if (String.IsNullOrEmpty(hgvid))
                        {
                            if (mybooking.Id == "hgv")
                                hgvid = pos.Attribute("POS2HotelRID").Value;
                        }

                        mybookingchannelslist.Add(mybooking);

                        bookable = true;                        
                    }

                    myacco.AccoBookingChannel = mybookingchannelslist.ToList();

                    if (bookable)
                        myacco.IsBookable = true;
                    else
                        myacco.IsBookable = false;
                }
                else
                {
                    if (myacco.AccoBookingChannel != null)
                    {
                        if (myacco.AccoBookingChannel.Count > 0)
                        {
                            myacco.AccoBookingChannel.Clear();
                            myacco.IsBookable = false;
                        }
                    }                    
                }

                myacco.HgvId = hgvid;

                if (hotelvg.Count() > 0)
                {
                    foreach (XElement myvg in hotelvg)
                    {
                        string vgRID = myvg.Attribute("G0RID").Value;
                        myacco.TourismVereinId = vgRID;
                        
                        //myacco.OwnerRid = vgRID;                        
                    }                   
                }
                if (hotelmg.Count() > 0)
                {
                    List<string> marketinggrouplist = new List<string>();

                    foreach (XElement mymg in hotelmg)
                    {

                        string mgRID = mymg.Attribute("G0RID").Value;
                        marketinggrouplist.Add(mgRID);
                    }
                    myacco.MarketingGroupIds = marketinggrouplist.ToList();                    
                }
                else
                {
                    if (myacco.MarketingGroupIds != null)
                    {
                        if (myacco.MarketingGroupIds.Count > 0)
                        {
                            myacco.MarketingGroupIds.Clear();
                        }
                    }

                }

                //Add LTS Id as Mapping
                var ltsriddict = new Dictionary<string, string>() { { "rid", myacco.Id.ToUpper() } };

                //Add LTS A0R_ID as Mapping 
                if (definitionhead.Attribute("A0R_ID") != null)
                {
                    ltsriddict.TryAddOrUpdate("a0r_id", definitionhead.Attribute("A0R_ID").Value);
                }

                myacco.Mapping.TryAddOrUpdate("lts", ltsriddict);

                //Fix Add HGV Mapping if present
                if (!String.IsNullOrEmpty(myacco.HgvId))
                {
                    var hgviddict = new Dictionary<string, string>() { { "id", myacco.HgvId } };

                    myacco.Mapping.TryAddOrUpdate("hgv", hgviddict);
                }

                //Special (Mapping Features to Marketinggroup) (B79228E62B5A4D14B2BF35E7B79B8580 ) + 2 (B5757D0688674594955606382A5E126C)  + 3 (31F741E8D6D8444A9BB571A2DF193F69
                MapFeaturetoMarketingGroup(myacco, "B79228E62B5A4D14B2BF35E7B79B8580");
                MapFeaturetoMarketingGroup(myacco, "B5757D0688674594955606382A5E126C");
                MapFeaturetoMarketingGroup(myacco, "31F741E8D6D8444A9BB571A2DF193F69");

                UpdateAusstattung(myacco);
                UpdateThemes(myacco, mywinelist, mycitylist, myskiarealist, mymediterranenlist, dolomiteslist, alpinelist);
                UpdateBadges(myacco, myvinumlist);
                UpdateAusstattungToSmgTags(myacco);
                
                //IF Badge Behindertengerecht is present add it as Tag
                UpdateBadgesToSmgTags(myacco, "Behindertengerecht", "barrier-free");
                
                return myacco;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);

                return null;
            }
        }

        //Import of an Additional Language ex NL(FL) CZ, PL)
        public static AccommodationLinked ParseMyLanguageAccommodation(AccommodationLinked myacco, XDocument myresponse, string language, string ltslanguage)
        {
            var adress = myresponse.Root.Element("HotelAddress").Element("Head").Element("Data"); //OK
            var publicity = myresponse.Root.Element("HotelPublicity").Element("Head").Elements("Data"); //OK
            var webdata = myresponse.Root.Element("HotelWebData").Element("Head").Elements("Data"); //OK

            AccoDetail mydetail = new AccoDetail();

            if (adress != null)
            {
                //De Address
                mydetail.Language = language;

                var adresslng = adress.Elements("AddressLng").Where(x => x.Attribute("LngID").Value == ltslanguage.ToUpper()).FirstOrDefault();

                if (adresslng != null)
                {
                    mydetail.City = adresslng.Attribute("A2Cit").Value;
                    mydetail.Email = adresslng.Attribute("A2EMa").Value;
                    mydetail.NameAddition = adresslng.Attribute("A2Na2").Value;
                    mydetail.Name = adresslng.Attribute("A2Nam").Value;
                    mydetail.Street = adresslng.Attribute("A2Str").Value;

                    if (String.IsNullOrEmpty(mydetail.Name))
                        mydetail.Name = myacco.AccoDetail["en"].Name;
                    if (String.IsNullOrEmpty(mydetail.City))
                        mydetail.City = myacco.AccoDetail["en"].City;
                    if (String.IsNullOrEmpty(mydetail.Email))
                        mydetail.Email = myacco.AccoDetail["en"].Email;
                    if (String.IsNullOrEmpty(mydetail.Street))
                        mydetail.Street = myacco.AccoDetail["en"].Street;
                }
                else
                {
                    mydetail.City = myacco.AccoDetail["en"].City;
                    mydetail.Email = myacco.AccoDetail["en"].Email;
                    mydetail.NameAddition = myacco.AccoDetail["en"].NameAddition;
                    mydetail.Name = myacco.AccoDetail["en"].Name;
                    mydetail.Street = myacco.AccoDetail["en"].Street;
                }

                mydetail.Fax = adress.Attribute("A1Fax").Value;
                mydetail.Firstname = adress.Attribute("A1FNa").Value;
                mydetail.Lastname = adress.Attribute("A1LNa").Value;
                mydetail.Mobile = adress.Attribute("A1Mob").Value;


                mydetail.Phone = adress.Attribute("A1Pho").Value;
                mydetail.Zip = adress.Attribute("A1ZIP").Value;                
            }
            if (publicity.Count() > 0)
            {                
                var mydesc = publicity.Where(x => x.Attribute("LngID").Value == ltslanguage.ToUpper()).FirstOrDefault();

                if (mydesc != null)
                {
                    mydetail.Longdesc = mydesc.Attribute("A7LoT").Value;
                    mydetail.Shortdesc = mydesc.Attribute("A7ShT").Value; 
                }                
            }
            if (webdata.Count() > 0)
            {
                var mywebdata = webdata.Where(x => x.Attribute("LngID").Value == ltslanguage.ToUpper()).FirstOrDefault();

                if (mywebdata != null)
                {
                    mydetail.Website = mywebdata.Attribute("A3Web").Value;
                }
                else
                {
                    mydetail.Website = myacco.AccoDetail["en"].Website;
                }                
            }
            else
            {
                mydetail.Website = myacco.AccoDetail["en"].Website;
            }
            
            myacco.AccoDetail.TryAddOrUpdate(language, mydetail);

            if (!myacco.HasLanguage.Contains(language))
                myacco.HasLanguage.Add(language);
            
            return myacco;
        }

        //Special Mapping etc...
        private static void MapFeaturetoMarketingGroup(Accommodation myacco, string featureid)
        {
            if (myacco.Features != null && myacco.Features.Count > 0 && myacco.Features.Select(x => x.Id).ToList().Contains(featureid))
            {
                if (myacco.MarketingGroupIds == null)
                    myacco.MarketingGroupIds = new List<string>();

                myacco.MarketingGroupIds.Add(featureid);
            }
        }


        //Update Badge Information
        private static void UpdateBadges(Accommodation myacco, XDocument myvinumlist)
        {
            myacco.BadgeIds = new List<string>();

            if (myacco.MarketingGroupIds != null)
            {
                var badge1 = myacco.MarketingGroupIds.Where(x => x == "8C40CB6844F14E4A821F4EBF8231A4E8").Count();
                if (badge1 > 0)
                    myacco.BadgeIds.Add("Wellnesshotel");

                var badge2 = myacco.MarketingGroupIds.Where(x => x == "A0BF7E47F4AC224EB089327BE4725C8B").Count();
                if (badge2 > 0)
                    myacco.BadgeIds.Add("Familienhotel");

                var badge3 = myacco.MarketingGroupIds.Where(x => x == "F2CAAF48AC1C4EE88342FB4E59610A68").Count();
                if (badge3 > 0)
                    myacco.BadgeIds.Add("Bikehotel");

                var badge4 = myacco.MarketingGroupIds.Where(x => x == "3CC3D40C8CAC46B7928CE76C9D7A6FF6").Count();
                if (badge4 > 0)
                    myacco.BadgeIds.Add("Bauernhof");

                var badge5 = myacco.MarketingGroupIds.Where(x => x == "476007E6DF974CFC98BBBEDD8787EC81").Count();
                if (badge5 > 0)
                    myacco.BadgeIds.Add("Behindertengerecht");

                var badge6 = myacco.MarketingGroupIds.Where(x => x == "3EA6116A6103498799B642C9C56D8301").Count();
                if (badge6 > 0)
                    myacco.BadgeIds.Add("Wanderhotel");

                var badge7 = myacco.MarketingGroupIds.Where(x => x == "4796D94E3AD54135973DF8574E52679E").Count();
                if (badge7 > 0)
                {
                    myacco.BadgeIds.Add("Südtirol Privat");
                }

                //Badge 8 Vinum Hotels

                //XDocument myvinumlist = XDocument.Load(xmldir + "Vinum.xml");
                //Look into xml Vinumlist
                var isinwinelist = myvinumlist.Root.Elements("Hotel").Where(x => x.Value == myacco.Id.ToUpper()).FirstOrDefault();
                if (isinwinelist != null)
                {
                    myacco.BadgeIds.Add("Vinumhotel");
                }

                //Badge 9,10,11 Nachhaltigkeitslabel Südtirol Level 1 (B79228E62B5A4D14B2BF35E7B79B8580 ) + 2 (B5757D0688674594955606382A5E126C)  + 3 (31F741E8D6D8444A9BB571A2DF193F69 )
                var badge9 = myacco.MarketingGroupIds.Where(x => x == "B79228E62B5A4D14B2BF35E7B79B8580").Count();
                if (badge9 > 0)
                {
                    myacco.BadgeIds.Add("SustainabilityLevel1");
                }
                var badge10 = myacco.MarketingGroupIds.Where(x => x == "B5757D0688674594955606382A5E126C").Count();
                if (badge10 > 0)
                {
                    myacco.BadgeIds.Add("SustainabilityLevel2");
                }
                var badge11 = myacco.MarketingGroupIds.Where(x => x == "31F741E8D6D8444A9BB571A2DF193F69").Count();
                if (badge11 > 0)
                {
                    myacco.BadgeIds.Add("SustainabilityLevel3");
                }
            }
        }

        //Update Theme Information
        private static void UpdateThemes(Accommodation myacco, XDocument mywinelist, XDocument mycitylist, XDocument myskiarealist, XDocument mymediterranenlist, XDocument dolomiteslist, XDocument alpinelist)
        {
            myacco.ThemeIds = new List<string>();

            //Thema 1
            //Gourmet 
            //Feature = 46AD7938616B4D4882A006BEF3B199A4 
            //Feature = F0A385D0E8E44944AFCA3893712A1420
            //Feature = 2FA54F6F350748AE9CD1A389A5C9EDD9
            //Feature = C0E761D71CC44F4C80D75FF68ED72C55 
            //Feature = 6797D594C7BF4C7AA6D384B234EC7C44
            //Feature = E5775068F5644E92B7CF94BDFCDA5175
            //Feature = 1FFD5352501542BF8BCB24B7BF75CF4F
            //Feature = 5060F78090604B2E97A96D86B97D2E0B
            if (myacco.Features != null)
            {
                var gourmet = myacco.Features.Where(x => x.Id == "46AD7938616B4D4882A006BEF3B199A4" ||
                    x.Id == "F0A385D0E8E44944AFCA3893712A1420" ||
                    x.Id == "2FA54F6F350748AE9CD1A389A5C9EDD9" ||
                    x.Id == "C0E761D71CC44F4C80D75FF68ED72C55" ||
                    x.Id == "6797D594C7BF4C7AA6D384B234EC7C44" ||
                    x.Id == "E5775068F5644E92B7CF94BDFCDA5175" ||
                    x.Id == "1FFD5352501542BF8BCB24B7BF75CF4F" ||
                    x.Id == "5060F78090604B2E97A96D86B97D2E0B");

                if (gourmet.Count() > 0)
                {
                    myacco.ThemeIds.Add("Gourmet");
                }
            }

            //Thema 2
            //In der Höhe 
            //Altitude > 1000            
            var altitude = myacco.Altitude;

            if (altitude != null)
            {
                int altitudeint = Convert.ToInt32(altitude);

                if (altitudeint > 1000)
                {
                    myacco.ThemeIds.Add("In der Höhe");
                }
            }


            //Thema 3
            //Regionale Wellness- und Heilanwendungen
            //Feature = B5CFA063BEEB4631B7A0DE836030E2ED UND 
            //Feature = E72CE3544DA2475E97B9C034DA6F1595 UND
            //( Feature = D417529377CB430389E07787D8A3A483 ODER
            //  Feature = 5E57209D17244BA09A0400A498E549AE ) UND
            //( Feature = 7BCAF604E17B46F2A2C6CAE70C5B621F ODER
            //  Feature = B2103635BD224E64A812FD2BF53C8DCA ) UND
            if (myacco.Features != null)
            {
                var regionalewellness = myacco.Features.Where(x =>
                    x.Id == "B5CFA063BEEB4631B7A0DE836030E2ED" ||
                    x.Id == "E72CE3544DA2475E97B9C034DA6F1595" ||
                    x.Id == "D417529377CB430389E07787D8A3A483" ||
                    x.Id == "5E57209D17244BA09A0400A498E549AE" ||
                    x.Id == "7BCAF604E17B46F2A2C6CAE70C5B621F" ||
                    x.Id == "B2103635BD224E64A812FD2BF53C8DCA");

                bool wellness1 = false;
                bool wellness2 = false;
                bool wellness3 = false;
                bool wellness4 = false;
                bool wellness5 = false;
                bool wellness6 = false;

                if (regionalewellness.Count() > 0)
                {
                    foreach (var mywellness in regionalewellness)
                    {
                        if (mywellness.Id == "B5CFA063BEEB4631B7A0DE836030E2ED")
                            wellness1 = true;
                        if (mywellness.Id == "E72CE3544DA2475E97B9C034DA6F1595")
                            wellness2 = true;
                        if (mywellness.Id == "D417529377CB430389E07787D8A3A483")
                            wellness3 = true;
                        if (mywellness.Id == "5E57209D17244BA09A0400A498E549AE")
                            wellness4 = true;
                        if (mywellness.Id == "7BCAF604E17B46F2A2C6CAE70C5B621F")
                            wellness5 = true;
                        if (mywellness.Id == "B2103635BD224E64A812FD2BF53C8DCA")
                            wellness6 = true;
                    }

                    if (wellness1 && wellness2 && (wellness3 || wellness4) && (wellness5 || wellness6))
                    {
                        myacco.ThemeIds.Add("Regionale Wellness");
                    }
                }
            }

            //Thema 4
            //Biken
            //Feature = 8068941DF6F34B9D955965062614A3C2 UND 
            //Feature = 349A4D98B26B448A908679142C3394D6 UND
            //Feature = BF108AD2B62042DF9FEAD4E865E11E75 UND
            if (myacco.Features != null)
            {
                var biken = myacco.Features.Where(x =>
                    x.Id == "8068941DF6F34B9D955965062614A3C2" ||
                    x.Id == "349A4D98B26B448A908679142C3394D6" ||
                    x.Id == "BF108AD2B62042DF9FEAD4E865E11E75" ||
                    x.Id == "05988DB63E5146E481C95279FB285C6A" ||
                    x.Id == "5F22AD3E93D54E99B7E6F97719A47153");

                bool biken1 = false;
                bool biken2 = false;
                bool biken3 = false;
                bool biken4 = false;
                bool biken5 = false;
                bool bikenbadge = false;

                if (myacco.MarketingGroupIds != null && myacco.MarketingGroupIds.Where(x => x == "F2CAAF48AC1C4EE88342FB4E59610A68").Count() > 0)
                    bikenbadge = true;

                if (biken.Count() > 0)
                {
                    foreach (var mybiken in biken)
                    {
                        if (mybiken.Id == "8068941DF6F34B9D955965062614A3C2")
                            biken1 = true;
                        if (mybiken.Id == "349A4D98B26B448A908679142C3394D6")
                            biken2 = true;
                        if (mybiken.Id == "BF108AD2B62042DF9FEAD4E865E11E75")
                            biken3 = true;
                        if (mybiken.Id == "05988DB63E5146E481C95279FB285C6A")
                            biken4 = true;
                        if (mybiken.Id == "5F22AD3E93D54E99B7E6F97719A47153")
                            biken5 = true;
                    }

                    if ((biken1 && biken2 && biken3) || biken4 || biken5 || bikenbadge)
                    {
                        myacco.ThemeIds.Add("Biken");
                    }
                }
            }
            //Thema 5
            //Familie
            //Feature = 8B808C230FE34263BE3787680DA253C7 UND 
            //Feature = 36C354DC30F14DD7B1CCFEE78E82132C UND
            //Feature = 188A9BADC0324C10B0013F108CE5EA5C UND
            if (myacco.Features != null)
            {
                var familie = myacco.Features.Where(x =>
                    x.Id == "8B808C230FE34263BE3787680DA253C7" ||
                    x.Id == "36C354DC30F14DD7B1CCFEE78E82132C" ||
                    x.Id == "188A9BADC0324C10B0013F108CE5EA5C");

                bool familie1 = false;
                bool familie2 = false;
                bool familie3 = false;

                if (familie.Count() > 0)
                {
                    foreach (var myfamilie in familie)
                    {
                        if (myfamilie.Id == "8B808C230FE34263BE3787680DA253C7")
                            familie1 = true;
                        if (myfamilie.Id == "36C354DC30F14DD7B1CCFEE78E82132C")
                            familie2 = true;
                        if (myfamilie.Id == "188A9BADC0324C10B0013F108CE5EA5C")
                            familie3 = true;
                    }

                    if (familie1 && familie2 && familie3)
                    {
                        myacco.ThemeIds.Add("Familie");
                    }
                }
            }

            //Thema 6
            //Wandern
            //Feature = 0A6193AD6EBC4BC18E83D7CEEEF53E45 UND                         
            //Feature = 42E4EFB64AD14393BC28DBC20F273B9D UND
            if (myacco.Features != null)
            {
                var wandern = myacco.Features.Where(x =>
                    x.Id == "0A6193AD6EBC4BC18E83D7CEEEF53E45" ||
                    x.Id == "42E4EFB64AD14393BC28DBC20F273B9D");

                bool wandern1 = false;
                bool wandern2 = false;

                if (wandern.Count() > 0)
                {
                    foreach (var mywandern in wandern)
                    {
                        if (mywandern.Id == "0A6193AD6EBC4BC18E83D7CEEEF53E45")
                            wandern1 = true;
                        if (mywandern.Id == "42E4EFB64AD14393BC28DBC20F273B9D")
                            wandern2 = true;
                    }

                    if (wandern1 && wandern2)
                    {
                        myacco.ThemeIds.Add("Wandern");
                    }
                }
            }
            //Thema 7
            //Wein
            //Feature = 0A6193AD6EBC4BC18E83D7CEEEF53E45 UND                         
            //Feature = 42E4EFB64AD14393BC28DBC20F273B9D UND

            var weinaltitude = myacco.Altitude;

            if (weinaltitude != null)
            {
                int weinaltitudeint = Convert.ToInt32(weinaltitude);

                if (weinaltitudeint <= 900)
                {
                    //XDocument mywinelist = XDocument.Load(xmldir + "Wine.xml");

                    //In Weinliste schauen
                    var isinwinelist = mywinelist.Root.Elements("Fraction").Where(x => x.Value == myacco.DistrictId).FirstOrDefault();

                    if (isinwinelist != null)
                    {
                        myacco.ThemeIds.Add("Wein");
                    }
                }
            }


            //Thema 8
            //Städtisches Flair
            //in Liste

            var cityaltitude = myacco.Altitude;

            if (cityaltitude != null)
            {
                int cityaltitudeint = Convert.ToInt32(cityaltitude);

                if (cityaltitudeint <= 1100)
                {
                    //XDocument mycitylist = XDocument.Load(xmldir + "City.xml");
                    //In Liste schauen
                    var isincitieslist = mycitylist.Root.Elements("Fraction").Where(x => x.Value == myacco.DistrictId).FirstOrDefault();

                    if (isincitieslist != null)
                    {
                        myacco.ThemeIds.Add("Städtisches Flair");
                    }
                }
            }


            //Thema 9
            //Skigebiete
            //in Liste
            //XDocument myskiarealist = XDocument.Load(xmldir + "NearSkiArea.xml");
            //In Liste schauen
            var isinskiarealist = myskiarealist.Root.Elements("Fraction").Where(x => x.Value == myacco.DistrictId).Count();

            if (isinskiarealist > 0)
            {
                myacco.ThemeIds.Add("Am Skigebiet");
            }


            //Thema 10
            //Mediterranes Südtirol
            //in Liste
            //XDocument mymediterranenlist = XDocument.Load(xmldir + "Mediterranean.xml");
            //In Liste schauen
            var isinmediterranlist = mymediterranenlist.Root.Elements("Fraction").Where(x => x.Value == myacco.DistrictId).Count();

            if (isinmediterranlist > 0)
            {
                myacco.ThemeIds.Add("Mediterran");
            }


            //Thema 11
            //In den Dolomiten
            //in Liste
            //XDocument dolomiteslist = XDocument.Load(xmldir + "Dolomites.xml");
            //In Liste schauen
            var isindolomitenlist = dolomiteslist.Root.Elements("Fraction").Where(x => x.Value == myacco.DistrictId).Count();

            if (isindolomitenlist > 0)
            {
                myacco.ThemeIds.Add("Dolomiten");
            }

            //Thema 12
            //Alpines Südtirol
            //in Liste

            //XDocument alpinelist = XDocument.Load(xmldir + "Alpine.xml");
            //In Liste schauen
            var isinalpinelist = alpinelist.Root.Elements("Fraction").Where(x => x.Value == myacco.DistrictId).Count();

            if (isinalpinelist > 0)
            {
                myacco.ThemeIds.Add("Alpin");
            }


            //Thema 13
            //Kleine Betriebe
            //Units > 0 und klianer < 20

            //NOT MORE USED
            //if (myacco.Units > 0)
            //{
            //    if (myacco.Units < 20)
            //    {
            //        myacco.ThemeIds.Add("Kleine Betriebe");
            //    }
            //}            


            //Thema 14
            //Hütten & Berggasthöfe            
            if (myacco.AccoTypeId == "Mountain")
            {
                myacco.ThemeIds.Add("Hütten und Berggasthöfe");
            }


            //Thema 15
            //Bäuerliche Welten            
            if (myacco.AccoTypeId == "Farm")
            {
                myacco.ThemeIds.Add("Bäuerliche Welten");
            }


            //Thema 16
            //Bonus Vacanze
            if (myacco.Features != null)
            {
                var balance = myacco.Features.Where(x => x.Id == "D448B037F37843B3B49C15CAFBBC5669").Count();

                if (balance > 0)
                {
                    myacco.ThemeIds.Add("Bonus Vacanze");
                }
            }


            //Thema 17 Christkindlmarkt

            List<string> tvtoassign = new List<string>();
            tvtoassign.Add("5228229451CA11D18F1400A02427D15E"); //Bozen
            tvtoassign.Add("5228229751CA11D18F1400A02427D15E"); //Brixen
            tvtoassign.Add("5228229851CA11D18F1400A02427D15E"); //Bruneck            
            tvtoassign.Add("522822FF51CA11D18F1400A02427D15E"); //Sterzing
            tvtoassign.Add("522822BE51CA11D18F1400A02427D15E"); //Meran

            if (tvtoassign.Contains(myacco.TourismVereinId.ToUpper()))
                myacco.ThemeIds.Add("Christkindlmarkt");


            //Thema 18 Nachhaltigkeit NEU

            List<string> sustainabilityodhtagtocheck = new List<string>();
            sustainabilityodhtagtocheck.Add("bio hotels südtirol"); //Bozen
            sustainabilityodhtagtocheck.Add("ecolabel hotels"); //Brixen
            sustainabilityodhtagtocheck.Add("gstc hotels"); //Bruneck            
            sustainabilityodhtagtocheck.Add("klimahotel"); //Sterzing

            //check if one of this odhtags is assigned

            var sustainabilityfeaturecheck = myacco.MarketingGroupIds != null ? myacco.MarketingGroupIds.Where(x => x == "3EA6116A6103498799B642C9C56D8301").Count() : 0;
            var sustainabilitytagintersection = myacco.SmgTags != null && myacco.SmgTags.Count > 0 ? sustainabilityodhtagtocheck.Intersect(myacco.SmgTags).Count() : 0;

            if (sustainabilityfeaturecheck > 0 || sustainabilitytagintersection > 0)
                myacco.ThemeIds.Add("Sustainability");


            Console.WriteLine("Thema hinzugefügt");
            Console.WriteLine("weiter....");
        }

        //Update Ausstattung Information
        private static void UpdateAusstattung(Accommodation myacco)
        {
            myacco.SpecialFeaturesIds = new List<string>();

            if (myacco.Features != null)
            {
                //Merkmal 1
                //Ruhig gelegen Feature = B6BD3F6011E5488DBF802B0C58F87AA1 

                var ruhiggelegen = myacco.Features.Where(x => x.Id == "B6BD3F6011E5488DBF802B0C58F87AA1").Count();
                if (ruhiggelegen > 0)
                {
                    myacco.SpecialFeaturesIds.Add("Ruhig gelegen");
                }

                //Merkmal 2
                //Tagung möglich Feature = FF81E4F50465484883DBF40CFB82BB0C UND 3101F60F0A594C0B9BBC8F4E2D7A2919             
                bool tagung1condition = false;
                bool tagung2condition = false;

                var tagung = myacco.Features.Where(x => x.Id == "FF81E4F50465484883DBF40CFB82BB0C" || x.Id == "3101F60F0A594C0B9BBC8F4E2D7A2919");
                if (tagung != null)
                {
                    foreach (var mytagung in tagung)
                    {
                        if (mytagung.Id == "FF81E4F50465484883DBF40CFB82BB0C")
                            tagung1condition = true;
                        if (mytagung.Id == "3101F60F0A594C0B9BBC8F4E2D7A2919")
                            tagung2condition = true;
                        if (tagung1condition && tagung2condition)
                        {
                            myacco.SpecialFeaturesIds.Add("Tagung");
                        }
                    }
                }

                //Merkmal 3
                //Schwimmbad Feature = 7BCAF604E17B46F2A2C6CAE70C5B621F ODER B2103635BD224E64A812FD2BF53C8DCA
                var schwimmbad = myacco.Features.Where(x => x.Id == "7BCAF604E17B46F2A2C6CAE70C5B621F" || x.Id == "B2103635BD224E64A812FD2BF53C8DCA").Count();
                if (schwimmbad > 0)
                {
                    //foreach (var myschwimmbad in schwimmbad)
                    //{
                    myacco.SpecialFeaturesIds.Add("Schwimmbad");
                    //}
                }

                //Merkmal 4
                //Sauna Feature = D417529377CB430389E07787D8A3A483 ODER 5E57209D17244BA09A0400A498E549AE
                var sauna = myacco.Features.Where(x => x.Id == "D417529377CB430389E07787D8A3A483" || x.Id == "5E57209D17244BA09A0400A498E549AE").Count();
                if (sauna > 0)
                {
                    //foreach (var mysauna in sauna)
                    //{
                    myacco.SpecialFeaturesIds.Add("Sauna");
                    //}
                }

                //Merkmal 5
                //Garage Feature = D579D1C8EA8445018CA5BB6DABEA0C26
                var garage = myacco.Features.Where(x => x.Id == "D579D1C8EA8445018CA5BB6DABEA0C26").Count();
                if (garage > 0)
                {
                    //foreach (var mygarage in garage)
                    //{
                    myacco.SpecialFeaturesIds.Add("Garage");
                    //}
                }

                //Merkmal 6
                //Abholservice Feature = 60F2408993E249F9A847F1B28C5B11E8
                var abholservice = myacco.Features.Where(x => x.Id == "60F2408993E249F9A847F1B28C5B11E8").Count();
                if (abholservice > 0)
                {
                    myacco.SpecialFeaturesIds.Add("Abholservice");
                }

                //Merkmal 7
                //Wlan Feature = 700A920BE6D6426CBF3EC623C2E922C2 OR 098EB30324EA492DBD99F323AE20A621
                var wlan = myacco.Features.Where(x => x.Id == "700A920BE6D6426CBF3EC623C2E922C2" || x.Id == "098EB30324EA492DBD99F323AE20A621").Count();
                if (wlan > 0)
                {
                    myacco.SpecialFeaturesIds.Add("Wlan");
                }


                //Merkmal 8
                //Barrierefrei Feature = B7E9EE4A91544849B69D5A5564DDCDFB            
                var barriere = myacco.Features.Where(x => x.Id == "B7E9EE4A91544849B69D5A5564DDCDFB").Count();
                if (barriere > 0)
                {
                    myacco.SpecialFeaturesIds.Add("Barrierefrei");
                }

                //Merkmal 9
                //Allergikerküche Feature = 71A7D4A821F7437EA1DC05CEE9655A5A OR 11A6BEA7EEFC4716BDF8FBD5E15C0CFB
                var allergiker = myacco.Features.Where(x => x.Id == "71A7D4A821F7437EA1DC05CEE9655A5A" || x.Id == "11A6BEA7EEFC4716BDF8FBD5E15C0CFB").Count();
                if (allergiker > 0)
                {
                    //foreach (var myallergiker in allergiker)
                    //{
                    myacco.SpecialFeaturesIds.Add("Allergikerküche");
                    //}
                }

                //Merkmal 10
                //Kleine Haustiere Feature = D9DCDD52FE444818AAFAB0E02FD92D91 OR FC80F2ECCE5A40AA8EDE458CBECC3D45         
                var kleinehaustiere = myacco.Features.Where(x => x.Id == "D9DCDD52FE444818AAFAB0E02FD92D91" || x.Id == "FC80F2ECCE5A40AA8EDE458CBECC3D45").Count();
                if (kleinehaustiere > 0)
                {
                    //foreach (var mykleinehaustiere in kleinehaustiere)
                    //{
                    myacco.SpecialFeaturesIds.Add("Kleine Haustiere");
                    //}
                }

                //Merkmal 11
                //Gruppenfreundlich Feature = 828CA68E3ABC4BA69587ACCB728E8858 OR BBBE370E1A9547B09D27AE0D94C066A3 OR B7C49ECF3CE1470EBA17F34D10D163A1       
                var gruppenfreundlich = myacco.Features.Where(x => x.Id == "828CA68E3ABC4BA69587ACCB728E8858" || x.Id == "BBBE370E1A9547B09D27AE0D94C066A3" || x.Id == "B7C49ECF3CE1470EBA17F34D10D163A1").Count();
                if (gruppenfreundlich > 0)
                {
                    //foreach (var mygruppenfreundlich in gruppenfreundlich)
                    //{
                    myacco.SpecialFeaturesIds.Add("Gruppenfreundlich");
                    //}
                }

                Console.WriteLine("Ausstattung hinzugefügt");
                Console.WriteLine("weiter....");


                //Spezialfall Covid 19
                var bonusvacanze = myacco.Features.Where(x => x.Id == "D448B037F37843B3B49C15CAFBBC5669").Count();

                if (bonusvacanze > 0)
                {
                    //Füge SMGTag Balance hinzu
                    if (myacco.SmgTags == null)
                        myacco.SmgTags = new List<string>();

                    if (!myacco.SmgTags.Contains("Bonus Vacanze"))
                        myacco.SmgTags.Add("Bonus Vacanze");
                }
                if (bonusvacanze == 0)
                {
                    if (myacco.SmgTags != null)
                    {
                        if (myacco.SmgTags.Contains("Bonus Vacanze"))
                            myacco.SmgTags.Remove("Bonus Vacanze");
                    }
                }

                //Spezialfall guestcard if one of this guestcards is set
                //"035577098B254201A865684EF050C851",
                //"CEE3703E4E3B44E3BD1BEE3F559DD31C",
                //"C7758584EFDE47B398FADB6BDBD0F198",
                //"C3C7ABEB0F374A0F811788B775D96AC0",
                //"3D703D2EA16645BD9EA3273069A0B918",
                //"D02AE2F641A4496AB1D2C4871475293D",
                //"DA4CAD333B8D45448AAEA9E966C68380",
                //"500AEFA8868748899BEC826B5E81951C",
                //"DE13880FA929461797146596FA3FFC07",
                //"49E9FF69F86846BD9915A115988C5484",
                //"FAEB6769EC564CBF982D454DCEEBCB27",
                //"3FD7253E3F6340E1AF642EA3DE005128",
                //"24E475F20FF64D748EBE7033C2DBC3A8",
                //"056486AFBEC4471EA32B3DB658A96D48",
                //"8192350ABF6B41DA89B255B340003991",
                //"3CB7D42AD51C4E2BA061CF9838A3735D",
                //"9C8140EB332F46E794DFDDB240F9A9E4"
                //new C414648944CE49D38506D176C5B58486 merancard_allyear


                var guestcard = myacco.Features.Where(x => x.Id == "035577098B254201A865684EF050C851" || x.Id == "CEE3703E4E3B44E3BD1BEE3F559DD31C" || x.Id == "C7758584EFDE47B398FADB6BDBD0F198" ||
                                                           x.Id == "C3C7ABEB0F374A0F811788B775D96AC0" || x.Id == "3D703D2EA16645BD9EA3273069A0B918" || x.Id == "D02AE2F641A4496AB1D2C4871475293D" ||
                                                           x.Id == "DA4CAD333B8D45448AAEA9E966C68380" || x.Id == "500AEFA8868748899BEC826B5E81951C" ||
                                                           x.Id == "49E9FF69F86846BD9915A115988C5484" || x.Id == "FAEB6769EC564CBF982D454DCEEBCB27" || x.Id == "3FD7253E3F6340E1AF642EA3DE005128" ||
                                                           x.Id == "24E475F20FF64D748EBE7033C2DBC3A8" || x.Id == "056486AFBEC4471EA32B3DB658A96D48" || x.Id == "8192350ABF6B41DA89B255B340003991" ||
                                                           x.Id == "3CB7D42AD51C4E2BA061CF9838A3735D" || x.Id == "9C8140EB332F46E794DFDDB240F9A9E4" || x.Id == "C414648944CE49D38506D176C5B58486" ||
                                                           x.Id == "6ACF61213EA347C6B1EB409D4A473B6D" || x.Id == "99803FF36D51415CAFF64183CC26F736" ||
                                                           x.Id == "B69F991C1E45422B9D457F716DEAA82B" || x.Id == "F4D3B02B107843C894ED517FC7DC8A39" || x.Id == "895C9B57E0D54B449C82F035538D4A79" ||
                                                           x.Id == "742AA043BD5847C79EE93EEADF0BE0D2").Count();
                if (guestcard > 0)
                {
                    myacco.SpecialFeaturesIds.Add("Guestcard");
                }
            }
        }

        private static void UpdateAusstattungToSmgTags(Accommodation myacco)
        {
            RemoveTagIf("035577098B254201A865684EF050C851", "bozencardplus", myacco);
            RemoveTagIf("CEE3703E4E3B44E3BD1BEE3F559DD31C", "rittencard", myacco);
            RemoveTagIf("C7758584EFDE47B398FADB6BDBD0F198", "klausencard", myacco);
            RemoveTagIf("C3C7ABEB0F374A0F811788B775D96AC0", "brixencard", myacco);
            //RemoveTagIf("455984E79EE6437B8D01793895AFDBE6", "almencardplus", myacco);
            RemoveTagIf("3D703D2EA16645BD9EA3273069A0B918", "almencardplus", myacco);
            RemoveTagIf("D02AE2F641A4496AB1D2C4871475293D", "activecard", myacco);
            RemoveTagIf("DA4CAD333B8D45448AAEA9E966C68380", "winepass", myacco);
            RemoveTagIf("500AEFA8868748899BEC826B5E81951C", "ultentalcard", myacco);
            RemoveTagIf("DE13880FA929461797146596FA3FFC07", "merancard", myacco);
            RemoveTagIf("49E9FF69F86846BD9915A115988C5484", "vinschgaucard", myacco);
            RemoveTagIf("FAEB6769EC564CBF982D454DCEEBCB27", "algundcard", myacco);
            RemoveTagIf("3FD7253E3F6340E1AF642EA3DE005128", "holidaypass", myacco);
            RemoveTagIf("24E475F20FF64D748EBE7033C2DBC3A8", "valgardenamobilcard", myacco);

            //Renamed
            //RemoveTagIf("056486AFBEC4471EA32B3DB658A96D48", "vilnoessdolomitimobilcard", myacco);
            RemoveTagIf("056486AFBEC4471EA32B3DB658A96D48", "dolomitimobilcard", myacco);

            RemoveTagIf("9C8140EB332F46E794DFDDB240F9A9E4", "mobilactivcard", myacco);
            //NEU
            RemoveTagIf("8192350ABF6B41DA89B255B340003991", "suedtirolguestpass", myacco);
            RemoveTagIf("3CB7D42AD51C4E2BA061CF9838A3735D", "holidaypass3zinnen", myacco);
            RemoveTagIf("19ABB47430F64287BEA96237A2E99899", "seiseralm_balance", myacco);
            RemoveTagIf("D1C1C206AA0B4025A98EE83C2DBC2DFA", "workation", myacco);
            //new
            RemoveTagIf("C414648944CE49D38506D176C5B58486", "merancard_allyear", myacco);
            RemoveTagIf("6ACF61213EA347C6B1EB409D4A473B6D", "dolomiti_museumobilcard", myacco);
            //new 15.01
            RemoveTagIf("99803FF36D51415CAFF64183CC26F736", "sarntalcard", myacco);

            //new 09.04.24
            RemoveTagIf("B69F991C1E45422B9D457F716DEAA82B", "suedtirolguestpass_passeiertal_premium", myacco); //Südtirol Guest Pass Passeiertal Premium
            RemoveTagIf("F4D3B02B107843C894ED517FC7DC8A39", "suedtirolguestpass_mobilcard", myacco); //Südtirol Guest Pass Mobilcard
            RemoveTagIf("895C9B57E0D54B449C82F035538D4A79", "suedtirolguestpass_museumobilcard", myacco); //Südtirol Alto Adige Guest Pass+museumobil Card

            RemoveTagIf("742AA043BD5847C79EE93EEADF0BE0D2", "natzschabscard", myacco); //Natz Schabs Card

            List<string> guestcardlist = new List<string>()
            {
                "035577098B254201A865684EF050C851",
                "CEE3703E4E3B44E3BD1BEE3F559DD31C",
                "C7758584EFDE47B398FADB6BDBD0F198",
                "C3C7ABEB0F374A0F811788B775D96AC0",
                "3D703D2EA16645BD9EA3273069A0B918",
                "D02AE2F641A4496AB1D2C4871475293D",
                "DA4CAD333B8D45448AAEA9E966C68380",
                "500AEFA8868748899BEC826B5E81951C",
                "49E9FF69F86846BD9915A115988C5484",
                "FAEB6769EC564CBF982D454DCEEBCB27",
                "3FD7253E3F6340E1AF642EA3DE005128",
                "24E475F20FF64D748EBE7033C2DBC3A8",
                "056486AFBEC4471EA32B3DB658A96D48",
                "8192350ABF6B41DA89B255B340003991",
                "3CB7D42AD51C4E2BA061CF9838A3735D",
                "9C8140EB332F46E794DFDDB240F9A9E4",
                "C414648944CE49D38506D176C5B58486",
                "6ACF61213EA347C6B1EB409D4A473B6D",
                "99803FF36D51415CAFF64183CC26F736",
                "B69F991C1E45422B9D457F716DEAA82B",
                "F4D3B02B107843C894ED517FC7DC8A39",
                "895C9B57E0D54B449C82F035538D4A79",
                "742AA043BD5847C79EE93EEADF0BE0D2"
            };

            RemoveTagIf(guestcardlist, "guestcard", myacco);

            //NEW
            RemoveTagIf("05988DB63E5146E481C95279FB285C6A", "accomodation bed bike", myacco);
            RemoveTagIf("5F22AD3E93D54E99B7E6F97719A47153", "accomodation bett bike sport", myacco);
        }

        private static void UpdateBadgesToSmgTags(Accommodation myacco, string badgename, string tagname)
        {
            if (myacco.BadgeIds != null && myacco.BadgeIds.Count() > 0)
            {
                if (myacco.BadgeIds.Select(x => x.ToLower()).Contains(badgename.ToLower()))
                {
                    if (myacco.SmgTags == null)
                        myacco.SmgTags = new List<string>();

                    if (!myacco.SmgTags.Contains(tagname))
                        myacco.SmgTags.Add(tagname);

                }
                else
                {
                    if (myacco.SmgTags != null)
                    {
                        if (myacco.SmgTags.Contains(tagname))
                            myacco.SmgTags.Remove(tagname);
                    }
                }
            }
        }

        private static void RemoveTagIf(string featureId, string tagname, Accommodation myacco)
        {
            if (myacco.Features != null)
            {
                var property = myacco.Features.Where(x => x.Id == featureId).Count();

                if (property > 0)
                {
                    if (myacco.SmgTags == null)
                        myacco.SmgTags = new List<string>();

                    if (!myacco.SmgTags.Contains(tagname))
                        myacco.SmgTags.Add(tagname);
                }
                if (property == 0)
                {
                    if (myacco.SmgTags != null)
                    {
                        if (myacco.SmgTags.Contains(tagname))
                            myacco.SmgTags.Remove(tagname);
                    }
                }
            }
        }

        private static void RemoveTagIf(List<string> featurelist, string tagname, Accommodation myacco)
        {
            if (myacco.Features != null)
            {
                var property = myacco.Features.Where(x => featurelist.Contains(x.Id)).Count();

                if (property > 0)
                {
                    if (myacco.SmgTags == null)
                        myacco.SmgTags = new List<string>();

                    if (!myacco.SmgTags.Contains(tagname))
                        myacco.SmgTags.Add(tagname);
                }
                if (property == 0)
                {
                    if (myacco.SmgTags != null)
                    {
                        if (myacco.SmgTags.Contains(tagname))
                            myacco.SmgTags.Remove(tagname);
                    }
                }
            }
        }
    }
}
