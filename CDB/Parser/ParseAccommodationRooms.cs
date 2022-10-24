using DataModel;
using Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CDB.Parser
{
    public class ParseAccommodationRooms
    {
        public static CultureInfo myculture = new CultureInfo("en");

        /// <summary>
        /// GETs Accommodation Rooms
        /// </summary>
        /// <param name="myresponse"></param>
        /// <param name="languages"></param>
        /// <param name="A0RID"></param>
        /// <param name="xmldir"></param>        
        /// <returns></returns>
        public static List<AccoRoom> ParseMyAccoRoom(XDocument myresponse, XDocument myfeatures, XDocument roomamenities, List<string> languages, string A0RID, string xmldir)
        {
            try
            {
                var groupdefinition = myresponse.Root.Element("GroupDefinition").Elements("Head"); //OK
                var groupname = myresponse.Root.Element("GroupName").Elements("Head"); //OK
                var grouprdata = myresponse.Root.Element("GroupRData").Elements("Head"); //OK
                var grouptin = myresponse.Root.Element("GroupTin").Elements("Head"); //OK
                var grouppublicity = myresponse.Root.Element("GroupPublicity").Elements("Head"); //OK
                var groupfoto = myresponse.Root.Element("GroupFoto").Elements("Head"); //OK

                Console.ForegroundColor = ConsoleColor.Green;

                List<AccoRoom> myroomlist = new List<AccoRoom>();

                foreach (XElement myroom in groupdefinition)
                {
                    if (myroom.Attribute("B0RID") != null)
                    {
                        string b0rid = myroom.Attribute("B0RID").Value;
                        string typ = myroom.Elements("Data").FirstOrDefault().Attribute("B0Typ").Value;

                        AccoRoom theroom = new AccoRoom();
                        theroom.A0RID = A0RID;
                        theroom.Id = b0rid;
                        theroom.Roomtype = GetRoomType(typ);

                        theroom.RoomtypeInt = Convert.ToInt32(typ);
                        theroom.RoomClassificationCodes = AlpineBitsHelper.GetRoomClassificationCode(theroom.Roomtype);

                        //NEU
                        theroom.Source = "LTS";
                        theroom.LTSId = b0rid;
                        theroom.HGVId = "";


                        if (myroom.Elements("Data").FirstOrDefault().Attribute("B0Cod") != null)
                            theroom.RoomCode = myroom.Elements("Data").FirstOrDefault().Attribute("B0Cod").Value;
                        else
                            theroom.RoomCode = "";

                        theroom.PriceFrom = null;


                        //Room numbers
                        var roomnumbers = new List<string>();

                        if (grouprdata != null)
                        {
                            //Is the B0RID Attribute Set
                            if (grouprdata.Elements("Data").Attributes("B0RID").Count() > 0)
                            {
                                var roomnumberobject = grouprdata.Elements("Data").Where(x => x.Attribute("B0RID").Value == b0rid).ToList();

                                if (roomnumberobject != null)
                                {
                                    var roomnumberobjeclist = roomnumberobject.Select(x => x.Attribute("F1Nam").Value).ToList();

                                    if (roomnumberobjeclist != null)
                                        roomnumbers = roomnumberobjeclist;
                                }
                            }
                        }

                        theroom.RoomNumbers = roomnumbers;

                        string roommax = myroom.Elements("Data").FirstOrDefault().Attribute("B0Max").Value;
                        string roommin = myroom.Elements("Data").FirstOrDefault().Attribute("B0Min").Value;
                        string roomstd = myroom.Elements("Data").FirstOrDefault().Attribute("B0Std").Value;
                        string roomqty = myroom.Elements("Data").FirstOrDefault().Attribute("B0Qty").Value;

                        if (!String.IsNullOrEmpty(roommax))
                            theroom.Roommax = Convert.ToInt32(roommax);
                        else
                            theroom.Roommax = null;

                        if (!String.IsNullOrEmpty(roommin))
                            theroom.Roommin = Convert.ToInt32(roommin);
                        else
                            theroom.Roommin = null;

                        if (!String.IsNullOrEmpty(roomstd))
                            theroom.Roomstd = Convert.ToInt32(roomstd);
                        else
                            theroom.Roomstd = null;

                        if (!String.IsNullOrEmpty(roomqty))
                            theroom.RoomQuantity = Convert.ToInt32(roomqty);
                        else
                            theroom.RoomQuantity = null;
                        

                        var myparsedroom = ParseMyRoomDetails(theroom, languages, groupname.Where(x => x.Attribute("B0RID").Value == b0rid).FirstOrDefault(), grouptin.Where(x => x.Attribute("B0RID").Value == b0rid).FirstOrDefault(), grouppublicity.Where(x => x.Attribute("B0RID").Value == b0rid).FirstOrDefault(), groupfoto.Where(x => x.Attribute("B0RID").Value == b0rid).FirstOrDefault(), myfeatures, roomamenities, xmldir);

                        myparsedroom.LastChange = DateTime.Now;

                        myroomlist.Add(myparsedroom);
                    }
                }

                return myroomlist;
            }
            catch (Exception ex)
            {                
                Console.WriteLine(ex.Message);

                return null;
            }
        }

        public static AccoRoom ParseMyRoomDetails(AccoRoom myroom, List<string> languages, XElement groupname, XElement grouptin, XElement grouppublicity, XElement groupfoto, XDocument myfeatures, XDocument roomamenities, string xmldir)
        {
            myroom.Shortname = groupname.Elements("Data").Where(x => x.Attribute("LngID").Value.ToUpper() == "DE").Count() > 0 ? groupname.Elements("Data").Where(x => x.Attribute("LngID").Value.ToUpper() == "DE").FirstOrDefault().Attribute("B1Des").Value : "not defined";

            if (grouptin.Elements("Data").Count() > 0)
            {

                List<AccoFeature> featurelist = new List<AccoFeature>();

                //Features
                foreach (XElement thetin in grouptin.Elements("Data"))
                {
                    string tinrid = thetin.Attribute("T0RID").Value;

                    var myfeature = myfeatures.Root.Elements("Data").Where(x => x.Attribute("T0RID").Value == tinrid).FirstOrDefault().Elements("DataLng").Where(x => x.Attribute("LngID").Value.ToUpper() == "EN").FirstOrDefault().Attribute("T1Des").Value;

                    //HGV ID Feature + OTA Code
                    string hgvamenityid = "";
                    string otacodes = "";

                    var myamenity = roomamenities.Root.Elements("amenity").Where(x => x.Element("ltsrid").Value == tinrid).FirstOrDefault();

                    if (myamenity != null)
                    {
                        hgvamenityid = myamenity.Element("hgvid").Value;
                        otacodes = myamenity.Element("ota_codes") != null ? myamenity.Element("ota_codes").Value : "";
                    }

                    List<int> amenitycodes = null;

                    if (!String.IsNullOrEmpty(otacodes))
                    {
                        var otacodessplittet = otacodes.Split(',').ToList();
                        amenitycodes = new List<int>();

                        foreach (var otacodesplit in otacodessplittet)
                        {
                            amenitycodes.Add(Convert.ToInt32(otacodesplit));
                        }
                    }

                    if (myfeature != null)
                        featurelist.Add(new AccoFeature() { Id = tinrid, Name = myfeature, HgvId = hgvamenityid, OtaCodes = otacodes, RoomAmenityCodes = amenitycodes });
                }
                myroom.Features = featurelist.ToList();

                Console.WriteLine("Room Tins imported!");
            }

            List<AccoDetail> myaccodetailslist = new List<AccoDetail>();

            myroom.HasLanguage = new List<string>();

            //Details            
            foreach (string mylang in languages)
            {
                AccoRoomDetail mydetail = new AccoRoomDetail();

                if (groupname.Elements("Data") != null)
                {
                    //De Adress
                    mydetail.Language = mylang;

                    if (!myroom.HasLanguage.Contains(mylang))
                        myroom.HasLanguage.Add(mylang);
          
                    mydetail.Name = groupname.Elements("Data").Where(x => x.Attribute("LngID").Value == mylang.ToUpper()).Count() > 0 ? groupname.Elements("Data").Where(x => x.Attribute("LngID").Value == mylang.ToUpper()).FirstOrDefault().Attribute("B1Des").Value : "";

                    Console.WriteLine("Name imported!");
                }
                if (grouppublicity.Elements("Data").Count() > 0)
                {
                    var mydesc = grouppublicity.Elements("Data").Where(x => x.Attribute("LngID").Value == mylang.ToUpper()).FirstOrDefault();

                    if (mydesc != null)
                    {
                        mydetail.Longdesc = mydesc.Attribute("B3ShT").Value;
                        mydetail.Shortdesc = mydesc.Attribute("B3LoT").Value;
                        //???mydetail.Shortdesc = mydesc.Attribute("A7GTC").Value;
                    }
                    Console.WriteLine("Publicity imported!");
                }

                myroom.AccoRoomDetail.TryAddOrUpdate(mylang, mydetail);
            }


            List<ImageGallery> imagegallerylist = new List<ImageGallery>();

            if (groupfoto.Elements("Data") != null)
            {
                int i = 0;
                ///ACHTUNG SOMMER UND WINTERBILD
                foreach (var theimage in groupfoto.Elements("Data"))
                {
                    ImageGallery mainimage = new ImageGallery();

                    mainimage.ImageUrl = theimage.Attribute("B4Fot").Value;
                    mainimage.Height = Convert.ToInt32(theimage.Attribute("B4PxH").Value);
                    mainimage.Width = Convert.ToInt32(theimage.Attribute("B4PxW").Value);
                    mainimage.ImageSource = "LTS";
                    mainimage.ListPosition = i;

                    mainimage.CopyRight = theimage.Attribute("B4Cop") != null ? theimage.Attribute("B4Cop").Value : "";
                    mainimage.License = theimage.Attribute("S31Cod") != null ? theimage.Attribute("S31Cod").Value : "";

                    imagegallerylist.Add(mainimage);
                    i++;
                }

            }

            myroom.ImageGallery = imagegallerylist.ToList();

            return myroom;
        }

        public static string GetRoomType(string roomtype)
        {
            string myroomtype = "";

            switch (roomtype)
            {
                case "0":
                    myroomtype = "undefined";
                    break;
                case "1":
                    myroomtype = "room";
                    break;
                case "2":
                    myroomtype = "apartment";
                    break;
                case "3":
                    myroomtype = "pitch";
                    break;
                case "4":
                    myroomtype = "dorm";
                    break;

                    //case "3":
                    //    myroomtype = "campsite";
                    //    break;
                    //case "4":
                    //    myroomtype = "caravan";
                    //    break;
                    //case "5":
                    //    myroomtype = "tentarea";
                    //    break;
                    //case "6":
                    //    myroomtype = "bungalow";
                    //    break;
                    //case "7":
                    //    myroomtype = "camp";
                    //    break;
            }

            return myroomtype;
        }

        public static string GetRoomTypeStringFromAvailabilityResponse(Int32 roomgenre)
        {
            string myroomtype = "";

            switch (roomgenre)
            {
                case 1:
                    myroomtype = "room";
                    break;
                case 2:
                    myroomtype = "apartment";
                    break;
                case 3:
                    myroomtype = "apartment";
                    break;
                case 4:
                    myroomtype = "apartment";
                    break;
                case 5:
                    myroomtype = "apartment";
                    break;
                case 6:
                    myroomtype = "pitch";
                    break;
                case 7:
                    myroomtype = "pitch";
                    break;
                case 8:
                    myroomtype = "dorm";
                    break;
                case 9:
                    myroomtype = "pitch";
                    break;
            }

            return myroomtype;
        }

        public static Int32 GetRoomTypeIntFromAvailabilityResponse(Int32 roomgenre)
        {
            switch (roomgenre)
            {
                case 1:
                    return 1;
                case 2:
                    return 2;
                case 3:
                    return 2;
                case 4:
                    return 2;
                case 5:
                    return 2;
                case 6:
                    return 3;
                case 7:
                    return 3;
                case 8:
                    return 4;
                case 9:
                    return 3;
                default:
                    return 0;
            }
        }

    }
}
