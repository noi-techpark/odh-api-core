using DataModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Helper.LCS
{
    public class ParseAccoSearchResult
    {
        public static MssResult ParsemyLCSResponse(string lang, ServiceReferenceLCS.AccommodationDataSearchRS lcsresponse, int rooms)
        {
            if (lcsresponse != null)
            {

                CultureInfo culturede = CultureInfo.CreateSpecificCulture("de");


                MssResult result = new MssResult();
                result.ResultId = lcsresponse.Result != null ? lcsresponse.Result.RID : "";
                result.bookableHotels = lcsresponse.Result != null ? lcsresponse.Result.ResultsQty : 0;
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

                        //Check Günstigstes Angebot
                        List<Tuple<int, double>> CheapestOffer_ws = new List<Tuple<int, double>>();
                        List<Tuple<int, double>> CheapestOffer_bb = new List<Tuple<int, double>>();
                        List<Tuple<int, double>> CheapestOffer_hb = new List<Tuple<int, double>>();
                        List<Tuple<int, double>> CheapestOffer_fb = new List<Tuple<int, double>>();
                        List<Tuple<int, double>> CheapestOffer_ai = new List<Tuple<int, double>>();


                        //Für jedes Zimmerangebot
                        foreach (var roomdetail in lcssearchdetail.RoomStay)
                        {
                            foreach (var roomdetailoffer in roomdetail.Rate)
                            {
                                RoomDetails myroomdetail = new RoomDetails();

                                myroomdetail.RoomSeq = roomdetailoffer.RoomStayIndex;
                                myroomdetail.RoomId = roomdetailoffer.RID;
                                myroomdetail.OfferId = roomdetail.RID;
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
                                myroomdetail.Roomtype = roomdetail.Genre != null ? Convert.ToInt32(roomdetail.Genre) : 0;  //roomdetail.Genre;
                                //myroomdetail.Roomtype = 0;

                                //Check billigstes Angebot
                                if (roomdetailoffer.AmountAI != 0)
                                {
                                    var mytupleai = new Tuple<int, double>((int)roomdetailoffer.RoomStayIndex, roomdetailoffer.AmountAI);
                                    CheapestOffer_ai.Add(mytupleai);
                                }
                                if (roomdetailoffer.AmountBB != 0)
                                {
                                    var mytupleai = new Tuple<int, double>((int)roomdetailoffer.RoomStayIndex, roomdetailoffer.AmountBB);
                                    CheapestOffer_bb.Add(mytupleai);
                                }
                                if (roomdetailoffer.AmountHB != 0)
                                {
                                    var mytupleai = new Tuple<int, double>((int)roomdetailoffer.RoomStayIndex, roomdetailoffer.AmountHB);
                                    CheapestOffer_hb.Add(mytupleai);
                                }
                                if (roomdetailoffer.AmountFB != 0)
                                {
                                    var mytupleai = new Tuple<int, double>((int)roomdetailoffer.RoomStayIndex, roomdetailoffer.AmountFB);
                                    CheapestOffer_fb.Add(mytupleai);
                                }
                                if (roomdetailoffer.AmountWS != 0)
                                {
                                    var mytupleai = new Tuple<int, double>((int)roomdetailoffer.RoomStayIndex, roomdetailoffer.AmountWS);
                                    CheapestOffer_ws.Add(mytupleai);
                                }


                                lcsresponseshort.RoomDetails.Add(myroomdetail);
                            }



                        }

                        //Billigstes Angebot holen
                        double cheapestaioffer = 0;
                        for (int i = 1; i <= rooms; i++)
                        {
                            var cheapestoffertemp = (from x in CheapestOffer_ai
                                                     where x.Item1 == i
                                                     orderby x.Item2 ascending
                                                     select x.Item2).FirstOrDefault();

                            cheapestaioffer = cheapestaioffer + cheapestoffertemp;
                        }

                        lcsresponseshort.CheapestOffer_ai = cheapestaioffer;

                        double cheapestbboffer = 0;
                        for (int i = 1; i <= rooms; i++)
                        {
                            var cheapestoffertemp = (from x in CheapestOffer_bb
                                                     where x.Item1 == i
                                                     orderby x.Item2 ascending
                                                     select x.Item2).FirstOrDefault();

                            cheapestbboffer = cheapestbboffer + cheapestoffertemp;
                        }

                        lcsresponseshort.CheapestOffer_bb = cheapestbboffer;

                        double cheapesthboffer = 0;
                        for (int i = 1; i <= rooms; i++)
                        {
                            var cheapestoffertemp = (from x in CheapestOffer_hb
                                                     where x.Item1 == i
                                                     orderby x.Item2 ascending
                                                     select x.Item2).FirstOrDefault();

                            cheapesthboffer = cheapesthboffer + cheapestoffertemp;
                        }

                        lcsresponseshort.CheapestOffer_hb = cheapesthboffer;

                        double cheapestfboffer = 0;
                        for (int i = 1; i <= rooms; i++)
                        {
                            var cheapestoffertemp = (from x in CheapestOffer_fb
                                                     where x.Item1 == i
                                                     orderby x.Item2 ascending
                                                     select x.Item2).FirstOrDefault();

                            cheapestfboffer = cheapestfboffer + cheapestoffertemp;
                        }

                        lcsresponseshort.CheapestOffer_fb = cheapestfboffer;

                        double cheapestwsoffer = 0;
                        for (int i = 1; i <= rooms; i++)
                        {
                            var cheapestoffertemp = (from x in CheapestOffer_ws
                                                     where x.Item1 == i
                                                     orderby x.Item2 ascending
                                                     select x.Item2).FirstOrDefault();

                            cheapestwsoffer = cheapestwsoffer + cheapestoffertemp;
                        }

                        lcsresponseshort.CheapestOffer_ws = cheapestwsoffer;


                        //Neu Cheapest Offer General
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
                                var cheapestofferdouble = cheapestoffertotal.OrderBy(x => x).FirstOrDefault();
                                lcsresponseshort.CheapestOffer = cheapestofferdouble;
                                lcsresponseshort.CheapestOfferString = String.Format(culturede, "{0:0,0.00}", cheapestofferdouble);
                            }

                        }


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
