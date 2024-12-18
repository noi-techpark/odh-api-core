// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DataModel;
using Helper;

namespace LCS
{
    public class ParseAccoSearchResult
    {
        public static MssResult ParsemyLCSResponse(
            string lang,
            ServiceReferenceLCS.AccommodationDataSearchRS lcsresponse,
            int rooms
        )
        {
            if (lcsresponse != null)
            {
                CultureInfo culturede = CultureInfo.CreateSpecificCulture("de");

                MssResult result = new MssResult();
                result.ResultId = lcsresponse.Result != null ? lcsresponse.Result.RID : "";
                result.bookableHotels =
                    lcsresponse.Result != null ? lcsresponse.Result.ResultsQty : 0;
                result.CheapestChannel = "lts";
                result.Cheapestprice = 0;

                if (lcsresponse.AccommodationData != null)
                {
                    foreach (var lcssearchdetail in lcsresponse.AccommodationData)
                    {
                        MssResponseShort lcsresponseshort = new MssResponseShort();

                        lcsresponseshort.A0RID = lcssearchdetail.RID;
                        lcsresponseshort.HotelId = 0;
                        lcsresponseshort.ChannelID = "lts";

                        //Check Cheapest Offer
                        List<CheapestOffer> CheapestOffer_ws = new List<CheapestOffer>();
                        List<CheapestOffer> CheapestOffer_bb = new List<CheapestOffer>();
                        List<CheapestOffer> CheapestOffer_hb = new List<CheapestOffer>();
                        List<CheapestOffer> CheapestOffer_fb = new List<CheapestOffer>();
                        List<CheapestOffer> CheapestOffer_ai = new List<CheapestOffer>();

                        //Foreach room offer
                        foreach (var roomdetail in lcssearchdetail.RoomStay)
                        {
                            foreach (var roomdetailoffer in roomdetail.Rate)
                            {
                                RoomDetails myroomdetail = new RoomDetails();

                                myroomdetail.RoomSeq = roomdetailoffer.RoomStayIndex;
                                myroomdetail.RoomId = roomdetail.RID;
                                myroomdetail.OfferId = roomdetailoffer.RID;
                                myroomdetail.Price_ai = roomdetailoffer.AmountAI;
                                myroomdetail.Price_bb = roomdetailoffer.AmountBB;
                                myroomdetail.Price_fb = roomdetailoffer.AmountFB;
                                myroomdetail.Price_hb = roomdetailoffer.AmountHB;
                                myroomdetail.Price_ws = roomdetailoffer.AmountWS;
                                myroomdetail.TotalPrice = 0;
                                myroomdetail.TotalPriceString = "";
                                myroomdetail.Roomtitle = roomdetail.Name;
                                //myroomdetail.Roomdesc = roomdetailoffer.
                                myroomdetail.Roomfree = roomdetail.Qty;
                                myroomdetail.Roommax = 0;
                                myroomdetail.Roommin = 0;
                                myroomdetail.Roomstd = 0;
                                //TODO
                                myroomdetail.Roomtype =
                                    roomdetail.Genre != null
                                        ? Convert.ToInt32(roomdetail.Genre)
                                        : 0; //roomdetail.Genre;
                                //myroomdetail.Roomtype = 0;

                                if (roomdetailoffer.AmountAI != 0)
                                {
                                    var mycheapestofferai = new CheapestOffer()
                                    {
                                        RoomId = roomdetail.RID,
                                        Price = roomdetailoffer.AmountAI,
                                        RoomSeq = (int)roomdetailoffer.RoomStayIndex,
                                        RoomFree = roomdetail.Qty,
                                    };
                                    CheapestOffer_ai.Add(mycheapestofferai);
                                }
                                if (roomdetailoffer.AmountBB != 0)
                                {
                                    var mycheapestofferbb = new CheapestOffer()
                                    {
                                        RoomId = roomdetail.RID,
                                        Price = roomdetailoffer.AmountBB,
                                        RoomSeq = (int)roomdetailoffer.RoomStayIndex,
                                        RoomFree = roomdetail.Qty,
                                    };
                                    CheapestOffer_bb.Add(mycheapestofferbb);
                                }
                                if (roomdetailoffer.AmountHB != 0)
                                {
                                    var mycheapestofferhb = new CheapestOffer()
                                    {
                                        RoomId = roomdetail.RID,
                                        Price = roomdetailoffer.AmountHB,
                                        RoomSeq = (int)roomdetailoffer.RoomStayIndex,
                                        RoomFree = roomdetail.Qty,
                                    };
                                    CheapestOffer_hb.Add(mycheapestofferhb);
                                }
                                if (roomdetailoffer.AmountFB != 0)
                                {
                                    var mycheapestofferfb = new CheapestOffer()
                                    {
                                        RoomId = roomdetail.RID,
                                        Price = roomdetailoffer.AmountFB,
                                        RoomSeq = (int)roomdetailoffer.RoomStayIndex,
                                        RoomFree = roomdetail.Qty,
                                    };
                                    CheapestOffer_fb.Add(mycheapestofferfb);
                                }
                                if (roomdetailoffer.AmountWS != 0)
                                {
                                    var mycheapestofferws = new CheapestOffer()
                                    {
                                        RoomId = roomdetail.RID,
                                        Price = roomdetailoffer.AmountWS,
                                        RoomSeq = (int)roomdetailoffer.RoomStayIndex,
                                        RoomFree = roomdetail.Qty,
                                    };
                                    CheapestOffer_ws.Add(mycheapestofferws);
                                }

                                lcsresponseshort.RoomDetails.Add(myroomdetail);
                            }
                        }

                        //Getting cheapest offer
                        var cheapestofferobj_ai = RoomCalculationHelper.CalculateCheapestRooms(
                            CheapestOffer_ai,
                            rooms,
                            "ai"
                        );
                        lcsresponseshort.CheapestOffer_ai =
                            cheapestofferobj_ai != null ? cheapestofferobj_ai.Price : 0;
                        if (cheapestofferobj_ai != null && cheapestofferobj_ai.Price > 0)
                            lcsresponseshort.CheapestOfferDetail.Add(cheapestofferobj_ai);

                        var cheapestofferobj_bb = RoomCalculationHelper.CalculateCheapestRooms(
                            CheapestOffer_bb,
                            rooms,
                            "bb"
                        );
                        lcsresponseshort.CheapestOffer_bb =
                            cheapestofferobj_bb != null ? cheapestofferobj_bb.Price : 0;
                        if (cheapestofferobj_bb != null && cheapestofferobj_bb.Price > 0)
                            lcsresponseshort.CheapestOfferDetail.Add(cheapestofferobj_bb);

                        var cheapestofferobj_hb = RoomCalculationHelper.CalculateCheapestRooms(
                            CheapestOffer_hb,
                            rooms,
                            "hb"
                        );
                        lcsresponseshort.CheapestOffer_hb =
                            cheapestofferobj_hb != null ? cheapestofferobj_hb.Price : 0;
                        if (cheapestofferobj_hb != null && cheapestofferobj_hb.Price > 0)
                            lcsresponseshort.CheapestOfferDetail.Add(cheapestofferobj_hb);

                        var cheapestofferobj_fb = RoomCalculationHelper.CalculateCheapestRooms(
                            CheapestOffer_fb,
                            rooms,
                            "fb"
                        );
                        lcsresponseshort.CheapestOffer_fb =
                            cheapestofferobj_fb != null ? cheapestofferobj_fb.Price : 0;
                        if (cheapestofferobj_fb != null && cheapestofferobj_fb.Price > 0)
                            lcsresponseshort.CheapestOfferDetail.Add(cheapestofferobj_fb);

                        var cheapestofferobj_ws = RoomCalculationHelper.CalculateCheapestRooms(
                            CheapestOffer_ws,
                            rooms,
                            "ws"
                        );
                        lcsresponseshort.CheapestOffer_ws =
                            cheapestofferobj_ws != null ? cheapestofferobj_ws.Price : 0;
                        if (cheapestofferobj_ws != null && cheapestofferobj_ws.Price > 0)
                            lcsresponseshort.CheapestOfferDetail.Add(cheapestofferobj_ws);

                        //Cheapest Offer General
                        if (lcsresponseshort.CheapestOffer == 0)
                        {
                            List<double> cheapestoffertotal = new List<double>();
                            if (lcsresponseshort.CheapestOffer_ai > 0)
                                cheapestoffertotal.Add((double)lcsresponseshort.CheapestOffer_ai);
                            if (lcsresponseshort.CheapestOffer_bb > 0)
                                cheapestoffertotal.Add((double)lcsresponseshort.CheapestOffer_bb);
                            if (lcsresponseshort.CheapestOffer_fb > 0)
                                cheapestoffertotal.Add((double)lcsresponseshort.CheapestOffer_fb);
                            if (lcsresponseshort.CheapestOffer_hb > 0)
                                cheapestoffertotal.Add((double)lcsresponseshort.CheapestOffer_hb);
                            if (lcsresponseshort.CheapestOffer_ws > 0)
                                cheapestoffertotal.Add((double)lcsresponseshort.CheapestOffer_ws);

                            if (cheapestoffertotal.Count > 0)
                            {
                                var cheapestofferdouble = cheapestoffertotal
                                    .OrderBy(x => x)
                                    .FirstOrDefault();
                                lcsresponseshort.CheapestOffer = cheapestofferdouble;
                                lcsresponseshort.CheapestOfferString = String.Format(
                                    culturede,
                                    "{0:0,0.00}",
                                    cheapestofferdouble
                                );
                            }
                        }

                        //Add only if there is a valid Offer (enough Roomfree etc..)
                        if (
                            lcsresponseshort.CheapestOfferDetail != null
                            && lcsresponseshort.CheapestOfferDetail.Count > 0
                        )
                            result.MssResponseShort.Add(lcsresponseshort);
                    }
                }

                return result;
            }
            else
                return null;
        }
    }

    public class AccoTestResult
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
