using Helper.MSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Helper
{
    public class GetMssData
    {
        /// <summary>
        /// MSS Response mit IDList, die IDs der Requests werden anhand der DB generiert (welches Hotel ist wo buchbar), Parallele Requests auf die Kanäle
        /// </summary>
        /// <param name="lang">Language</param>
        /// <param name="A0Ridlist">Liste aller Hotels</param>
        /// <param name="mybookingchannels">Liste aller abzufragenden bookingchannels</param>
        /// <param name="myroomdata">Zimmerinfos mit Personen</param>
        /// <param name="arrival">Ankunftsdatum</param>
        /// <param name="departure">Abreisedatum</param>
        /// <param name="service">Verpflegungsart</param>
        /// <returns></returns>
        public static async Task<MssResult> GetMssResponse(string lang, List<string> A0Ridlist, string[] mybookingchannels, List<Tuple<string, string, List<string>>> myroomdata, DateTime arrival, DateTime departure, int service, string hgvservicecode, XElement offerdetails, XElement hoteldetails, int rooms, string source, string version, string mssuser, string msspswd, bool withoutmssids = false)
        {
            try
            {

                List<Room> myroompersons = (from x in myroomdata
                                            select new Room { RoomSeq = x.Item1, RoomType = x.Item2, Person = x.Item3 }).ToList();

                var myroomlist = MssRequest.BuildRoomData(myroompersons);

                XElement mychannels = MssRequest.BuildChannelList(mybookingchannels);

                XElement myidlist = default(XElement);

                if (withoutmssids)
                    myidlist = MssRequest.BuildIDList(new List<string>());
                else
                    myidlist = MssRequest.BuildIDList(A0Ridlist);

                XElement mytyp = MssRequest.BuildType("10");

                ////Add Logging
                //if (A0Ridlist != null)
                //{
                //    var tracesource = new TraceSource("MssData");
                //    tracesource.TraceEvent(TraceEventType.Information, 0, "MSS Request Hotel ID Count: " + A0Ridlist.Count + " Period: " + arrival.ToShortDateString() + " " + departure.ToShortDateString() + " Service: " + service.ToString() + " Rooms: " + myroompersons.Count + " Result from Cache: " + withoutmssids.ToString());
                //}

                XDocument myrequest = MssRequest.BuildPostData(myidlist, mychannels, myroomlist, arrival, departure, offerdetails, hoteldetails, mytyp, service, lang, source, version, mssuser, msspswd);

                var myresponses = MssRequest.RequestAsync(myrequest);

                await Task.WhenAll(myresponses);

                Task<string> activityresponsecontent = myresponses.Result.Content.ReadAsStringAsync();

                await Task.WhenAll(activityresponsecontent);

                XElement allmyresponses = XElement.Parse(activityresponsecontent.Result);

                List<XElement> allmyoffers = (from xy in allmyresponses.Element("result").Elements("hotel")
                                              where
                                                  xy.Elements("channel").Count() > 0
                                              select xy).ToList();

                XElement myresult = new XElement("root");
                myresult.Add(
                    allmyresponses.Element("header"),
                    new XElement("result",
                    allmyoffers));

                //Und iatz no parsen
                MssResult myparsedresponse = ParseMssResponse.ParsemyMssResponse(lang, hgvservicecode, myresult, A0Ridlist, myroompersons, source, version);

                return myparsedresponse;
            }
            catch (Exception ex)
            {
                //var tracesource = new TraceSource("MssData");
                //tracesource.TraceEvent(TraceEventType.Error, 0, "MSS Request Error: " + ex.Message);

                return null;
            }
        }


        #region obsoleteCode

        ///// <summary>
        ///// MSS Response mit IDList, die IDs der Requests werden anhand der DB generiert (welches Hotel ist wo buchbar), Parallele Requests auf die Kanäle
        ///// </summary>
        ///// <param name="lang">Language</param>
        ///// <param name="A0Ridlist">Liste aller Hotels</param>
        ///// <param name="mybookingchannels">Liste aller abzufragenden bookingchannels</param>
        ///// <param name="myroomdata">Zimmerinfos mit Personen</param>
        ///// <param name="arrival">Ankunftsdatum</param>
        ///// <param name="departure">Abreisedatum</param>
        ///// <param name="service">Verpflegungsart</param>
        ///// <returns></returns>
        //public static async Task<MssResult> GetMssResponseAsync(string lang, List<string> A0Ridlist, string[] mybookingchannels, List<Tuple<string, string, List<string>>> myroomdata, DateTime arrival, DateTime departure, int service, string hgvservicecode, XElement offerdetails, XElement hoteldetails, int rooms, string source)
        //{
        //    try
        //    {

        //        List<Room> myroompersons = (from x in myroomdata
        //                                    select new Room { RoomSeq = x.Item1, RoomType = x.Item2, Person = x.Item3 }).ToList();

        //        var myroomlist = MssRequest.BuildRoomData(myroompersons);
        //        XElement mytyp = MssRequest.BuildType("10");

        //        List<XDocument> myrequests = (from x in mybookingchannels
        //                                      let y = GetMyChannelTuple(x, A0Ridlist, MssRequest.BuildChannelList, MssRequest.BuildIDList)
        //                                      where y != null
        //                                      select MssRequest.BuildPostData(y.Item1, y.Item2, myroomlist, arrival, departure, offerdetails, hoteldetails, mytyp, service, lang, source)).ToList();


        //        List<Task<HttpResponseMessage>> myresponses = (from x in myrequests
        //                                                       select MssRequest.RequestAsync(x)).ToList();

        //        await Task.WhenAll(myresponses);

        //        List<Task<string>> activityresponsecontent = (from x in myresponses
        //                                                      select x.Result.Content.ReadAsStringAsync()).ToList();

        //        await Task.WhenAll(activityresponsecontent);

        //        List<XElement> allmyresponses = (from xy in activityresponsecontent
        //                                         select XElement.Parse(xy.Result)).ToList();

        //        XElement myroot = new XElement("mssresponseroot");
        //        myroot.Add(allmyresponses);


        //        XDocument encodedDoc8 = new XDocument(
        //        new XDeclaration("1.0", "utf-8", "yes"), myroot);


        //        //var servicecode = mysuedtirolcontainer.AccoBoardSet.Where(x => x.BoardIdHGV == service).FirstOrDefault().BoardnameHGV;            
        //        //Und iatz no parsen
        //        MssResult myparsedresponse = ParseMssResponse.ParsemyMssResponse(lang, hgvservicecode, encodedDoc8, myroompersons, source);

        //        return myparsedresponse;
        //    }
        //    catch(Exception ex)
        //    {
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// MSS Response ohne Idlist Parallele Requests auf die Kanäle
        ///// </summary>
        ///// <param name="mysuedtirolcontainer"></param>
        ///// <param name="lang"></param>
        ///// <param name="mybookingchannels"></param>
        ///// <param name="myroomdata"></param>
        ///// <param name="arrival"></param>
        ///// <param name="departure"></param>
        ///// <param name="service"></param>
        ///// <returns></returns>
        //public static async Task<MssResult> GetMssResponseAsyncWithoutIDs(string lang, List<string> A0Ridlist, string[] mybookingchannels, List<Tuple<string, string, List<string>>> myroomdata, DateTime arrival, DateTime departure, int service, string hgvservicecode, XElement offerdetails, XElement hoteldetails, int rooms, string source)
        //{
        //    try
        //    {
        //        List<Room> myroompersons = (from x in myroomdata
        //                                    select new Room { RoomSeq = x.Item1, RoomType = x.Item2, Person = x.Item3 }).ToList();

        //        var myroomlist = MssRequest.BuildRoomData(myroompersons);
        //        XElement mytyp = MssRequest.BuildType("10");

        //        List<XDocument> myrequests = (from x in mybookingchannels
        //                                      select MssRequest.BuildPostData(new XElement("channel_id", x), myroomlist, arrival, departure, offerdetails, hoteldetails, mytyp, service, lang, source)).ToList();


        //        List<Task<HttpResponseMessage>> myresponses = (from x in myrequests
        //                                                       select MssRequest.RequestAsync(x)).ToList();


        //        await Task.WhenAll(myresponses);

        //        List<Task<string>> activityresponsecontent = (from x in myresponses
        //                                                      select x.Result.Content.ReadAsStringAsync()).ToList();

        //        await Task.WhenAll(activityresponsecontent);

        //        List<XElement> allmyresponses = (from xy in activityresponsecontent
        //                                         select XElement.Parse(xy.Result)).ToList();

        //        XElement myroot = new XElement("mssresponseroot");
        //        myroot.Add(allmyresponses);

        //        XDocument encodedDoc8 = new XDocument(
        //        new XDeclaration("1.0", "utf-8", "yes"), myroot);


        //        //var servicecode = mysuedtirolcontainer.AccoBoardSet.Where(x => x.BoardIdHGV == service).FirstOrDefault().BoardnameHGV;

        //        //Und iatz no parsen
        //        MssResult myparsedresponse = ParseMssResponse.ParsemyMssResponse(lang, hgvservicecode, encodedDoc8, A0Ridlist, myroompersons, source);

        //        return myparsedresponse;
        //    }
        //    catch(Exception ex)
        //    {
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// MSS Response ohne IDlist auf alle Kanäle zugleich
        ///// </summary>
        ///// <param name="lang"></param>
        ///// <param name="A0Ridlist"></param>
        ///// <param name="mybookingchannels"></param>
        ///// <param name="myroomdata"></param>
        ///// <param name="arrival"></param>
        ///// <param name="departure"></param>
        ///// <param name="service"></param>
        ///// <param name="hgvservicecode"></param>
        ///// <returns></returns>
        //public static async Task<MssResult> GetMssResponseAsyncWithoutIDsALL(string lang, List<string> A0Ridlist, string[] mybookingchannels, List<Tuple<string, string, List<string>>> myroomdata, DateTime arrival, DateTime departure, int service, string hgvservicecode, XElement offerdetails, XElement hoteldetails, int rooms, string source)
        //{
        //    try
        //    {

        //        List<Room> myroompersons = (from x in myroomdata
        //                                    select new Room { RoomSeq = x.Item1, RoomType = x.Item2, Person = x.Item3 }).ToList();

        //        var myroomlist = MssRequest.BuildRoomData(myroompersons);
        //        XElement mytyp = MssRequest.BuildType("10");


        //        XElement mychannels = MssRequest.BuildChannelList(mybookingchannels);

        //        XDocument myrequests = MssRequest.BuildPostData2(mychannels, myroomlist, arrival, departure, offerdetails, hoteldetails, mytyp, service, lang, source);


        //        var myresponses = MssRequest.RequestAsync(myrequests);

        //        await Task.WhenAll(myresponses);

        //        Task<string> activityresponsecontent = myresponses.Result.Content.ReadAsStringAsync();

        //        await Task.WhenAll(activityresponsecontent);

        //        XElement allmyresponses = XElement.Parse(activityresponsecontent.Result);



        //        List<XElement> allmyoffers = (from xy in allmyresponses.Element("result").Elements("hotel")
        //                                      where
        //                                          xy.Elements("channel").Count() > 0
        //                                      select xy).ToList();

        //        XElement myresult = new XElement("root");
        //        myresult.Add(
        //            allmyresponses.Element("header"),
        //            new XElement("result",
        //            allmyoffers));


        //        //XElement myroot = new XElement("mssresponseroot");
        //        //myroot.Add(myresult);



        //        //XDocument encodedDoc8 = new XDocument(
        //        //new XDeclaration("1.0", "utf-8", "yes"), myroot);


        //        //var servicecode = mysuedtirolcontainer.AccoBoardSet.Where(x => x.BoardIdHGV == service).FirstOrDefault().BoardnameHGV;



        //        //Und iatz no parsen
        //        MssResult myparsedresponse = ParseMssResponse.ParsemyMssResponse(lang, hgvservicecode, myresult, A0Ridlist, myroompersons, source);

        //        return myparsedresponse;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}




        //private static Tuple<XElement, XElement> GetMyChannelTuple(string bookingportal, List<string> A0Ridlist, Func<string, XElement> buildChannelList, Func<List<string>, XElement> buildidList)
        //{
        //    List<string> myhotelchannellist = new List<string>();


        //    //List<int> myhotelchannellist = (from y in mycontainer.BookingportalSet
        //    //                                where y.Shortname == bookingportal
        //    //                                select y.Id).ToList();

        //    //Sonderfall LTS muassi no mochn
        //    if (bookingportal == "pos")
        //    {
        //        myhotelchannellist.Add("valgardena");
        //        myhotelchannellist.Add("eggental");
        //        myhotelchannellist.Add("valgardena2");
        //        myhotelchannellist.Add("altabadia");
        //        myhotelchannellist.Add("hochpustertal");
        //        myhotelchannellist.Add("seiseralm");
        //        myhotelchannellist.Add("crontour");
        //        myhotelchannellist.Add("suedtirolssueden");
        //        myhotelchannellist.Add("roterhahn");
        //    }
        //    else
        //    {
        //        myhotelchannellist.Add(bookingportal);
        //    }

        //    //List<string> mya0rids = (from x in mycontainer.BookingIDsSet
        //    //                         where A0Ridlist.Contains(x.Accommodation.A0RID) && myhotelchannellist.Contains(x.BookingportalId)
        //    //                         select x.Accommodation.A0RID).ToList();

        //    List<string> mya0rids = new List<string>();

        //    var documentStore = InitializeDocumentStore();
        //    using(var session = documentStore.OpenSession())
        //    {

        //        //DEs geat no net ! Index fahler
        //        //var bookableaccos = session.Query<Accommodation, AccommodationsByBookingPortal>()
        //        //    .Where(x => x.Id.In(A0Ridlist))
        //        //    .Where(x => x.AccoBookingChannel.Any(y => y.Id.In(myhotelchannellist)))                                        
        //        //    .TransformWith<AccoListTransformer, string>()
        //        //    .ToList();

        //        var bookableaccos = session.Query<Accommodation, AccommodationsByBookingPortal>()
        //            .Where(x => x.Id.In(A0Ridlist))
        //            .Where(x => x.AccoBookingChannel.Any(y => y.Id.In(myhotelchannellist)))
        //            .ProjectFromIndexFieldsInto<AccoBookList>()
        //            .ToList();

        //         mya0rids = bookableaccos.Select(x => x.Id).ToList();
        //    }

        //    if (mya0rids.Count > 0)
        //    {
        //        var myidlist = buildidList(mya0rids);

        //        //Channel Data            
        //        var mychannellist = buildChannelList(bookingportal);

        //        Tuple<XElement, XElement> mytuple = new Tuple<XElement, XElement>(myidlist, mychannellist);

        //        return mytuple;
        //    }
        //    else
        //    {
        //        //return (Tuple<XElement, XElement>)Enumerable.Empty<Tuple<XElement, XElement>>();
        //        return null;
        //    }
        //}


        //private static IDocumentStore InitializeDocumentStore()
        //{
        //    var documentStore = new DocumentStore
        //    {
        //        Url = "http://localhost:8080",
        //        DefaultDatabase = "SuedtirolDB"
        //    };
        //    documentStore.Initialize();
        //    return documentStore;
        //}

        #endregion
    }
}
