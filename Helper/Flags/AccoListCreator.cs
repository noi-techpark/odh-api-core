// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Text;

namespace Helper
{
    public class AccoListCreator
    {
        //Boardlist Packages Creator
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
                            boardidhgv = Tuple.Create<int, string>(
                                0,
                                "price_ws,price_bb,price_hb,price_fb,price_ai"
                            );
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

        //Generates Roominfo Tuple List
        public static List<Tuple<string, string, List<string>>> BuildMyRoomInfo(string roominfo)
        {
            if (!String.IsNullOrEmpty(roominfo) && roominfo != "null")
            {
                //roominfo aufteilen Form 1Z-1P-18 oder 1Z-2P-18.18,1Z-1P-18
                List<Tuple<string, string, List<string>>> myroominfo =
                    new List<Tuple<string, string, List<string>>>();

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

                    var myroom = new Tuple<string, string, List<string>>(
                        roomseq.ToString(),
                        myspittetzimmerinfo[0].Substring(0),
                        mypersons
                    );

                    myroominfo.Add(myroom);
                    roomseq++;
                }

                return myroominfo;
            }
            else
            {
                //Return standard 2 Person 1 Room
                List<Tuple<string, string, List<string>>> myroominfostd =
                    new List<Tuple<string, string, List<string>>>();
                var myroomstd = new Tuple<string, string, List<string>>(
                    "1",
                    "0",
                    new List<string>() { "18", "18" }
                );
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
                    AccommodationCategoryFlag myaccotypeflag =
                        (AccommodationCategoryFlag)accocatfilterint;

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
            themeids.Add("Sustainability", false);

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
            featids.Add("Guestcard", false);

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
                        boardidhgv = Tuple.Create<int, string>(
                            0,
                            "price_ws,price_bb,price_hb,price_fb,price_ai"
                        );
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
