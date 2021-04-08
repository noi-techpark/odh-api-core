using System;
using System.Collections.Generic;
using System.Text;

namespace Helper
{
    public class AccoListCreator
    {
        //Boardlist für Query
        public static List<string> CreateBoardList(string? boardfilter)
        {
            List<string> boardids = new List<string>();

            if (!String.IsNullOrEmpty(boardfilter))
            {

                if (boardfilter != "null")
                {

                    if (boardfilter.Substring(boardfilter.Length - 1, 1) == ",")
                        boardfilter = boardfilter.Substring(0, boardfilter.Length - 1);

                    switch (boardfilter)
                    {
                        case "Board0":
                            boardids.Add("without board");
                            boardids.Add("breakfast");
                            boardids.Add("half board");
                            boardids.Add("full board");
                            boardids.Add("All Inclusive");
                            break;
                        case "Board1":
                            boardids.Add("without board");
                            break;
                        case "Board2":
                            boardids.Add("breakfast");
                            break;
                        case "Board3":
                            boardids.Add("half board");
                            break;
                        case "Board4":
                            boardids.Add("full board");
                            break;
                        case "Board5":
                            boardids.Add("All Inclusive");
                            break;
                    }
                }

            }
            return boardids;
        }

        //Boardlist Packages für Query
        public static List<string> CreateBoardListPackages(string? boardfilter)
        {
            List<string> boardids = new List<string>();
            if (!String.IsNullOrEmpty(boardfilter))
            {
                if (boardfilter != "null")
                {

                    if (boardfilter.Substring(boardfilter.Length - 1, 1) == ",")
                        boardfilter = boardfilter.Substring(0, boardfilter.Length - 1);

                    switch (boardfilter)
                    {
                        case "Board0":
                            boardids.Add("1");
                            boardids.Add("2");
                            boardids.Add("3");
                            boardids.Add("4");
                            boardids.Add("5");
                            break;
                        case "Board1":
                            boardids.Add("1");
                            break;
                        case "Board2":
                            boardids.Add("2");
                            break;
                        case "Board3":
                            boardids.Add("3");
                            break;
                        case "Board4":
                            boardids.Add("4");
                            break;
                        case "Board5":
                            boardids.Add("5");
                            break;
                    }
                }
            }
            return boardids;
        }

        //Board HGV Id Creator
        public static Tuple<int, string> CreateBoardListHGV(string? boardfilter)
        {
            Tuple<int, string> boardidhgv = new Tuple<int, string>(0, "");
            if (!String.IsNullOrEmpty(boardfilter))
            {
                if (boardfilter != "null")
                {

                    if (boardfilter.Substring(boardfilter.Length - 1, 1) == ",")
                        boardfilter = boardfilter.Substring(0, boardfilter.Length - 1);

                    switch (boardfilter)
                    {
                        case "Board0":
                            boardidhgv = Tuple.Create<int, string>(0, "price_ws,price_bb,price_hb,price_fb,price_ai");
                            break;
                        case "Board1":
                            boardidhgv = Tuple.Create<int, string>(1, "price_ws");
                            break;
                        case "Board2":
                            boardidhgv = Tuple.Create<int, string>(2, "price_bb");
                            break;
                        case "Board3":
                            boardidhgv = Tuple.Create<int, string>(3, "price_hb");
                            break;
                        case "Board4":
                            boardidhgv = Tuple.Create<int, string>(4, "price_fb");
                            break;
                        case "Board5":
                            boardidhgv = Tuple.Create<int, string>(5, "price_ai");
                            break;
                    }
                }
            }
            return boardidhgv;
        }

        //Board HGV Id Creator
        public static Tuple<int, string> CreateBoardListHGV2(string? boardfilter)
        {
            Tuple<int, string> boardidhgv = new Tuple<int, string>(0, "");
            if (!String.IsNullOrEmpty(boardfilter))
            {
                if (boardfilter != "null")
                {

                    if (boardfilter.Substring(boardfilter.Length - 1, 1) == ",")
                        boardfilter = boardfilter.Substring(0, boardfilter.Length - 1);

                    switch (boardfilter)
                    {
                        case "0":
                            boardidhgv = Tuple.Create<int, string>(0, "price_ws,price_bb,price_hb,price_fb,price_ai");
                            break;
                        case "1":
                            boardidhgv = Tuple.Create<int, string>(1, "price_ws");
                            break;
                        case "2":
                            boardidhgv = Tuple.Create<int, string>(2, "price_bb");
                            break;
                        case "3":
                            boardidhgv = Tuple.Create<int, string>(3, "price_hb");
                            break;
                        case "4":
                            boardidhgv = Tuple.Create<int, string>(4, "price_fb");
                            break;
                        case "5":
                            boardidhgv = Tuple.Create<int, string>(5, "price_ai");
                            break;
                    }
                }
            }
            return boardidhgv;
        }

        public static List<string> CreateAccoTypeList(string? accotypefilter)
        {
            List<string> typeids = new List<string>();
            if (!String.IsNullOrEmpty(accotypefilter))
            {
                if (accotypefilter != "null")
                {
                    if (accotypefilter.Substring(accotypefilter.Length - 1, 1) == ",")
                        accotypefilter = accotypefilter.Substring(0, accotypefilter.Length - 1);

                    var splittedfilter = accotypefilter.Split(',');

                    foreach (var filter in splittedfilter)
                    {
                        switch (filter)
                        {
                            case "Type1":
                                typeids.Add("HotelPension");
                                break;
                            case "Type2":
                                typeids.Add("BedBreakfast");
                                break;
                            case "Type3":
                                typeids.Add("Farm");
                                break;
                            case "Type4":
                                typeids.Add("Camping");
                                break;
                            case "Type5":
                                typeids.Add("Youth");
                                break;
                            case "Type6":
                                typeids.Add("Mountain");
                                break;
                            case "Type7":
                                //Muassi no ban import mochn
                                typeids.Add("Apartment");
                                break;
                        }

                    }
                }
            }
            return typeids;
        }

        public static List<string> CreateCategoryList(string? categoryfilter)
        {
            List<string> categoryids = new List<string>();
            if (!String.IsNullOrEmpty(categoryfilter))
            {
                if (categoryfilter != "null")
                {
                    if (categoryfilter.Substring(categoryfilter.Length - 1, 1) == ",")
                        categoryfilter = categoryfilter.Substring(0, categoryfilter.Length - 1);

                    var splittedfilter = categoryfilter.Split(',');

                    foreach (var filter in splittedfilter)
                    {
                        switch (filter)
                        {

                            case "Cat0":
                                categoryids.Add("Notcategorized");
                                break;
                            case "Cat1":
                                categoryids.Add("1star");
                                categoryids.Add("1flower");
                                categoryids.Add("1sun");
                                break;
                            case "Cat2":
                                categoryids.Add("2stars");
                                categoryids.Add("2flowers");
                                categoryids.Add("2suns");
                                break;
                            case "Cat3s":
                                categoryids.Add("3sstars");
                                break;
                            case "Cat3":
                                categoryids.Add("3stars");
                                categoryids.Add("3flowers");
                                categoryids.Add("3suns");
                                break;
                            case "Cat4":
                                categoryids.Add("4stars");
                                categoryids.Add("4flowers");
                                categoryids.Add("4suns");
                                break;
                            case "Cat4s":
                                categoryids.Add("4sstars");
                                break;
                            case "Cat5":
                                categoryids.Add("5stars");
                                categoryids.Add("5flowers");
                                categoryids.Add("5suns");
                                break;
                        }
                    }
                }
            }
            return categoryids;
        }

        public static List<string> CreateThemeList(string? themefilter)
        {
            List<string> themeids = new List<string>();
            if (!String.IsNullOrEmpty(themefilter))
            {
                if (themefilter != "null")
                {
                    if (themefilter.Substring(themefilter.Length - 1, 1) == ",")
                        themefilter = themefilter.Substring(0, themefilter.Length - 1);


                    var splittedfilter = themefilter.Split(',');

                    foreach (var filter in splittedfilter)
                    {
                        switch (filter)
                        {
                            case "Theme1":
                                themeids.Add("Gourmet");
                                break;
                            case "Theme2":
                                themeids.Add("In der Höhe");
                                break;
                            case "Theme3":
                                themeids.Add("Regionale Wellness");
                                break;
                            case "Theme4":
                                themeids.Add("Biken");
                                break;
                            case "Theme5":
                                themeids.Add("Familie");
                                break;
                            case "Theme6":
                                themeids.Add("Wandern");
                                break;
                            case "Theme7":
                                themeids.Add("Wein");
                                break;
                            case "Theme8":
                                themeids.Add("Städtisches Flair");
                                break;
                            case "Theme9":
                                themeids.Add("Am Skigebiet");
                                break;
                            case "Theme10":
                                themeids.Add("Mediterran");
                                break;
                            case "Theme11":
                                themeids.Add("Dolomiten");
                                break;
                            case "Theme12":
                                themeids.Add("Alpin");
                                break;
                            case "Theme13":
                                themeids.Add("Kleine Betriebe");
                                break;
                            case "Theme14":
                                themeids.Add("Hütten und Berggasthöfe");
                                break;
                            case "Theme15":
                                themeids.Add("Bäuerliche Welten");
                                break;
                            case "Theme16":
                                themeids.Add("Balance");
                                break;
                            case "Theme17":
                                themeids.Add("Christkindlmarkt");
                                break;
                        }

                    }
                }
            }
            return themeids;
        }

        public static Dictionary<int, bool> CreateThemeListDict(string? themefilter)
        {
            Dictionary<int, bool> themeids = new Dictionary<int, bool>();

            themeids.Add(1, false);
            themeids.Add(2, false);
            themeids.Add(3, false);
            themeids.Add(4, false);
            themeids.Add(5, false);
            themeids.Add(6, false);
            themeids.Add(7, false);
            themeids.Add(8, false);
            themeids.Add(9, false);
            themeids.Add(10, false);
            themeids.Add(11, false);
            themeids.Add(12, false);
            themeids.Add(13, false);
            themeids.Add(14, false);
            themeids.Add(15, false);
            themeids.Add(16, false);
            themeids.Add(17, false);
            if (!String.IsNullOrEmpty(themefilter))
            {
                if (themefilter != "null")
                {
                    if (themefilter.Substring(themefilter.Length - 1, 1) == ",")
                        themefilter = themefilter.Substring(0, themefilter.Length - 1);

                    var splittedfilter = themefilter.Split(',');

                    foreach (var filter in splittedfilter)
                    {
                        switch (filter)
                        {
                            case "Theme1":
                                themeids[1] = true;
                                break;
                            case "Theme2":
                                themeids[2] = true;
                                break;
                            case "Theme3":
                                themeids[3] = true;
                                break;
                            case "Theme4":
                                themeids[4] = true;
                                break;
                            case "Theme5":
                                themeids[5] = true;
                                break;
                            case "Theme6":
                                themeids[6] = true;
                                break;
                            case "Theme7":
                                themeids[7] = true;
                                break;
                            case "Theme8":
                                themeids[8] = true;
                                break;
                            case "Theme9":
                                themeids[9] = true;
                                break;
                            case "Theme10":
                                themeids[10] = true;
                                break;
                            case "Theme11":
                                themeids[11] = true;
                                break;
                            case "Theme12":
                                themeids[12] = true;
                                break;
                            case "Theme13":
                                themeids[13] = true;
                                break;
                            case "Theme14":
                                themeids[14] = true;
                                break;
                            case "Theme15":
                                themeids[15] = true;
                                break;
                            case "Theme16":
                                themeids[16] = true;
                                break;
                            case "Theme17":
                                themeids[17] = true;
                                break;
                        }

                    }
                }
            }
            return themeids;
        }

        public static List<string> CreateBadgeList(string? badgefilter)
        {
            List<string> badgeids = new List<string>();
            if (!String.IsNullOrEmpty(badgefilter))
            {
                if (badgefilter != "null")
                {
                    if (badgefilter.Substring(badgefilter.Length - 1, 1) == ",")
                        badgefilter = badgefilter.Substring(0, badgefilter.Length - 1);

                    var splittedfilter = badgefilter.Split(',');

                    foreach (var filter in splittedfilter)
                    {
                        switch (filter)
                        {
                            case "Badge1":
                                badgeids.Add("Wellnesshotel");
                                break;
                            case "Badge2":
                                badgeids.Add("Familienhotel");
                                break;
                            case "Badge3":
                                badgeids.Add("Bikehotel");
                                break;
                            case "Badge4":
                                badgeids.Add("Bauernhof");
                                break;
                            case "Badge5":
                                badgeids.Add("Behindertengerecht");
                                break;
                            case "Badge6":
                                badgeids.Add("Wanderhotel");
                                break;
                            case "Badge7":
                                badgeids.Add("Südtirol Privat");
                                break;
                        }
                    }
                }
            }
            return badgeids;
        }

        public static List<string> CreateFeaturesList(string? featurefilter)
        {
            List<string> featureids = new List<string>();
            if (!String.IsNullOrEmpty(featurefilter))
            {
                if (featurefilter != "null")
                {
                    if (featurefilter.Substring(featurefilter.Length - 1, 1) == ",")
                        featurefilter = featurefilter.Substring(0, featurefilter.Length - 1);

                    var splittedfilter = featurefilter.Split(',');

                    foreach (var filter in splittedfilter)
                    {
                        switch (filter)
                        {
                            case "Feat1":
                                featureids.Add("Ruhig gelegen");
                                break;
                            case "Feat2":
                                featureids.Add("Tagung");
                                break;
                            case "Feat3":
                                featureids.Add("Schwimmbad");
                                break;
                            case "Feat4":
                                featureids.Add("Sauna");
                                break;
                            case "Feat5":
                                featureids.Add("Garage");
                                break;
                            case "Feat6":
                                featureids.Add("Abholservice");
                                break;
                            case "Feat7":
                                featureids.Add("Wlan");
                                break;
                            case "Feat8":
                                featureids.Add("Barrierefrei");
                                break;
                            case "Feat9":
                                featureids.Add("Allergikerküche");
                                break;
                            case "Feat10":
                                featureids.Add("Kleine Haustiere");
                                break;
                            case "Feat11":
                                featureids.Add("Gruppenfreundlich");
                                break;
                        }
                    }
                }
            }
            return featureids;
        }

        public static Dictionary<int, bool> CreateFeatureListDict(string? featurefilter)
        {
            Dictionary<int, bool> featids = new Dictionary<int, bool>();

            featids.Add(1, false);
            featids.Add(2, false);
            featids.Add(3, false);
            featids.Add(4, false);
            featids.Add(5, false);
            featids.Add(6, false);
            featids.Add(7, false);
            featids.Add(8, false);
            featids.Add(9, false);
            featids.Add(10, false);
            featids.Add(11, false);
            if (!String.IsNullOrEmpty(featurefilter))
            {
                if (featurefilter != "null")
                {
                    if (featurefilter.Substring(featurefilter.Length - 1, 1) == ",")
                        featurefilter = featurefilter.Substring(0, featurefilter.Length - 1);


                    var splittedfilter = featurefilter.Split(',');




                    foreach (var filter in splittedfilter)
                    {
                        switch (filter)
                        {
                            case "Feat1":
                                featids[1] = true;
                                break;
                            case "Feat2":
                                featids[2] = true;
                                break;
                            case "Feat3":
                                featids[3] = true;
                                break;
                            case "Feat4":
                                featids[4] = true;
                                break;
                            case "Feat5":
                                featids[5] = true;
                                break;
                            case "Feat6":
                                featids[6] = true;
                                break;
                            case "Feat7":
                                featids[7] = true;
                                break;
                            case "Feat8":
                                featids[8] = true;
                                break;
                            case "Feat9":
                                featids[9] = true;
                                break;
                            case "Feat10":
                                featids[10] = true;
                                break;
                            case "Feat11":
                                featids[11] = true;
                                break;
                        }

                    }
                }
            }
            return featids;
        }

        //Generiert meine Roominfo de komische Tupleliste
        public static List<Tuple<string, string, List<string>>> BuildMyRoomInfo(string roominfo)
        {
            if (!String.IsNullOrEmpty(roominfo) && roominfo != "null")
            {
                //roominfo aufteilen Form 1Z-1P-18 oder 1Z-2P-18.18,1Z-1P-18                
                List<Tuple<string, string, List<string>>> myroominfo = new List<Tuple<string, string, List<string>>>();

                var zimmerinfos = roominfo.Split('|');
                int roomseq = 1;

                foreach (var zimmerinfo in zimmerinfos)
                {
                    List<string> mypersons = new List<string>();

                    var myspittetzimmerinfo = zimmerinfo.Split('-');

                    var mypersoninfo = myspittetzimmerinfo[1].Split(',');
                    foreach (string s in mypersoninfo)
                    {
                        mypersons.Add(s);
                    }

                    var myroom = new Tuple<string, string, List<string>>(roomseq.ToString(), myspittetzimmerinfo[0].Substring(0), mypersons);

                    myroominfo.Add(myroom);
                    roomseq++;
                }

                return myroominfo;
            }
            else
            {
                //Return standard 2 Person 1 Room
                List<Tuple<string, string, List<string>>> myroominfostd = new List<Tuple<string, string, List<string>>>();
                var myroomstd = new Tuple<string, string, List<string>>("1", "0", new List<string>() { "18", "18" });
                myroominfostd.Add(myroomstd);

                return myroominfostd;
            }

        }

        #region Flags

        public static List<string> CreateAccoTypeListfromFlag(string? accotypefilter)
        {
            List<string> typeids = new List<string>();
            
            if (!String.IsNullOrEmpty(accotypefilter) && accotypefilter != "null")
            {                
                if (int.TryParse(accotypefilter, out var accotypefilterint))
                {
                    AccommodationTypeFlag myaccotypeflag = (AccommodationTypeFlag)accotypefilterint;

                    var myflags = myaccotypeflag.GetFlags().GetDescriptionList();

                    foreach (var myflag in myflags)
                    {
                        typeids.Add(myflag);
                    }
                }                                    
            }

            return typeids;
        }

        public static List<string> CreateCategoryListfromFlag(string? categoryfilter)
        {
            List<string> categoryids = new List<string>();

            if (!String.IsNullOrEmpty(categoryfilter) && categoryfilter != "null")
            {                
                if (int.TryParse(categoryfilter, out var accocatfilterint))
                {                    
                    AccommodationCategoryFlag myaccotypeflag = (AccommodationCategoryFlag)accocatfilterint;

                    var myflags = myaccotypeflag.GetFlags().GetDescriptionList();

                    foreach (var myflag in myflags)
                    {
                        categoryids.Add(myflag);
                    }
                }
            }

            return categoryids;
        }

        public static List<string> CreateThemeListfromFlag(string? themefilter)
        {
            List<string> themeids = new List<string>();

            if (!String.IsNullOrEmpty(themefilter) && themefilter != "null")
            {
                if (int.TryParse(themefilter, out var themefilterint))
                {                    
                    AccoThemeFlag myaccothemeflag = (AccoThemeFlag)themefilterint;

                    var myflags = myaccothemeflag.GetFlags().GetDescriptionList();

                    foreach (var myflag in myflags)
                    {
                        themeids.Add(myflag);
                    }
                }
            }

            return themeids;
        }

        public static Dictionary<string, bool> CreateThemeListDictfromFlag(string? themefilter)
        {
            Dictionary<string, bool> themeids = new Dictionary<string, bool>();

            themeids.Add("Gourmet", false);
            themeids.Add("In der Höhe", false);
            themeids.Add("Regionale Wellness", false);
            themeids.Add("Biken", false);
            themeids.Add("Familie", false);
            themeids.Add("Wandern", false);
            themeids.Add("Wein", false);
            themeids.Add("Städtisches Flair", false);
            themeids.Add("Am Skigebiet", false);
            themeids.Add("Mediterran", false);
            themeids.Add("Dolomiten", false);
            themeids.Add("Alpin", false);
            themeids.Add("Kleine Betriebe", false);
            themeids.Add("Hütten und Berggasthöfe", false);
            themeids.Add("Bäuerliche Welten", false);
            themeids.Add("Balance", false);
            themeids.Add("Christkindlmarkt", false);

            if (!String.IsNullOrEmpty(themefilter) && themefilter != "null")
            {
                if (int.TryParse(themefilter, out var themefilterint))
                {                    
                    AccoThemeFlag myaccothemeflag = (AccoThemeFlag)themefilterint;

                    var myflags = myaccothemeflag.GetFlags().GetDescriptionList();

                    foreach (var myflag in myflags)
                    {
                        themeids[myflag] = true;
                    }
                }
            }

            return themeids;
        }

        public static List<string> CreateBadgeListfromFlag(string? badgefilter)
        {
            List<string> badgeids = new List<string>();

            if (!String.IsNullOrEmpty(badgefilter) && badgefilter != "null")
            {
                if (int.TryParse(badgefilter, out var accobadgefilterint))
                {                    
                    AccoBadgeFlag myaccobadgeflag = (AccoBadgeFlag)accobadgefilterint;

                    var myflags = myaccobadgeflag.GetFlags().GetDescriptionList();

                    foreach (var myflag in myflags)
                    {
                        badgeids.Add(myflag);
                    }
                }
            }

            return badgeids;
        }

        public static List<string> CreateFeaturesListfromFlag(string? featurefilter)
        {
            List<string> featureids = new List<string>();

            if (!String.IsNullOrEmpty(featurefilter) && featurefilter != "null")
            {
                if (int.TryParse(featurefilter, out var accofeatfilterint))
                {                    
                    AccoFeatureFlag myaccofeatflag = (AccoFeatureFlag)accofeatfilterint;

                    var myflags = myaccofeatflag.GetFlags().GetDescriptionList();

                    foreach (var myflag in myflags)
                    {
                        featureids.Add(myflag);
                    }
                }
            }

            return featureids;
        }

        public static Dictionary<string, bool> CreateFeatureListDictfromFlag(string? featurefilter)
        {
            Dictionary<string, bool> featids = new Dictionary<string, bool>();

            featids.Add("Ruhig gelegen", false);
            featids.Add("Tagung", false);
            featids.Add("Schwimmbad", false);
            featids.Add("Sauna", false);
            featids.Add("Garage", false);
            featids.Add("Abholservice", false);
            featids.Add("Wlan", false);
            featids.Add("Barrierefrei", false);
            featids.Add("Allergikerküche", false);
            featids.Add("Kleine Haustiere", false);
            featids.Add("Gruppenfreundlich", false);

            if (!String.IsNullOrEmpty(featurefilter) && featurefilter != "null")
            {
                if (int.TryParse(featurefilter, out var accofeatfilterint))
                {                    
                    AccoFeatureFlag myaccofeatflag = (AccoFeatureFlag)accofeatfilterint;

                    var myflags = myaccofeatflag.GetFlags().GetDescriptionList();

                    foreach (var myflag in myflags)
                    {
                        featids[myflag] = true;
                    }
                }
            }

            return featids;
        }

        public static List<string> CreateBoardListFromFlag(string? boardfilter)
        {
            List<string> boardids = new List<string>();

            if (!String.IsNullOrEmpty(boardfilter) && boardfilter != "null")
            {
                if (int.TryParse(boardfilter, out var boardfilterint))
                {                 
                    AccoBoardFlag myaccoboardflag = (AccoBoardFlag)boardfilterint;

                    var myflags = myaccoboardflag.GetFlags().GetDescriptionList();

                    foreach (var myflag in myflags)
                    {
                        boardids.Add(myflag);
                    }
                }
            }

            return boardids;
        }

        public static List<string> CreateBoardListPackagesfromFlag(string? boardfilter)
        {
            List<string> boardids = new List<string>();

            if (!String.IsNullOrEmpty(boardfilter) && boardfilter != "null")
            {
                if (int.TryParse(boardfilter, out var boardfilterint))
                {                    
                    PackageBoardFlag myaccoboardflag = (PackageBoardFlag)boardfilterint;

                    var myflags = myaccoboardflag.GetFlags().GetDescriptionList();

                    foreach (var myflag in myflags)
                    {
                        boardids.Add(myflag);
                    }
                }
            }

            return boardids;
        }

        //Board HGV Id Creator
        public static Tuple<int, string> CreateBoardListHGVfromFlag(string? boardfilter)
        {
            Tuple<int, string> boardidhgv = new Tuple<int, string>(0, "");

            if (!String.IsNullOrEmpty(boardfilter) && boardfilter != "null")
            {
                if (boardfilter.Substring(boardfilter.Length - 1, 1) == ",")
                    boardfilter = boardfilter.Substring(0, boardfilter.Length - 1);

                switch (boardfilter)
                {
                    case "0":
                        boardidhgv = Tuple.Create<int, string>(0, "price_ws,price_bb,price_hb,price_fb,price_ai");
                        break;
                    case "1":
                        boardidhgv = Tuple.Create<int, string>(1, "price_ws");
                        break;
                    case "2":
                        boardidhgv = Tuple.Create<int, string>(2, "price_bb");
                        break;
                    case "4":
                        boardidhgv = Tuple.Create<int, string>(3, "price_hb");
                        break;
                    case "8":
                        boardidhgv = Tuple.Create<int, string>(4, "price_fb");
                        break;
                    case "16":
                        boardidhgv = Tuple.Create<int, string>(5, "price_ai");
                        break;
                }

            }
            return boardidhgv;
        }


        //Board (Mealplans) on LCS Creator
        public static List<string> CreateBoardListLCSfromFlag(string? boardfilter)
        {
            List<string> boardlcs = new List<string>();

            if (!String.IsNullOrEmpty(boardfilter) && boardfilter != "null")
            {
                if (boardfilter.Substring(boardfilter.Length - 1, 1) == ",")
                    boardfilter = boardfilter.Substring(0, boardfilter.Length - 1);

                switch (boardfilter)
                {
                    case "0":
                        boardlcs.Add("1");
                        boardlcs.Add("3");
                        boardlcs.Add("10");
                        boardlcs.Add("12");
                        boardlcs.Add("14");
                        break;
                    case "1":
                        boardlcs.Add("14");
                        break;
                    case "2":
                        boardlcs.Add("3");
                        break;
                    case "4":
                        boardlcs.Add("12");
                        break;
                    case "8":
                        boardlcs.Add("10");
                        break;
                    case "16":
                        boardlcs.Add("14");
                        break;
                }
            }
            return boardlcs;
        }

        #endregion

    }
}
