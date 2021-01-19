using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MSS
{
    public class GetMssSpecial
    {
        //Objekt Validity
        public static async Task<List<Package>> GetMssSpecialPackages(HttpClient httpClient, string lang, List<string> Packageidlist, DateTime from, DateTime to, XElement specialdetails, int typ, int service, List<Tuple<string, string, List<string>>> myroomdata, string source, string version, string mssuser, string msspswd)
        {
            try
            {
                XElement myroomlist = new XElement("room");

                if (myroomdata != null)
                {
                    List<Room> myroompersons = (from x in myroomdata
                                                select new Room { RoomSeq = x.Item1, RoomType = x.Item2, Person = x.Item3 }).ToList();

                    myroomlist = MssRequest.BuildRoomData(myroompersons);
                }

                XElement myidlist = MssRequest.BuildOfferIDList(Packageidlist);

                XDocument myrequest = MssRequest.BuildSpecialPostData(myidlist, myroomlist, from, to, specialdetails, typ, service, lang, source, version, mssuser, msspswd);

                var myresponses = MssRequest.RequestSpecialAsync(httpClient, myrequest);

                await Task.WhenAll(myresponses);

                Task<string> specialresponsecontent = myresponses.Result.Content.ReadAsStringAsync();

                await Task.WhenAll(specialresponsecontent);

                XElement fullresponse = XElement.Parse(specialresponsecontent.Result);

                var myparsedresponse = ParseMssSpecialResponse.ParseMySpecialResponse(lang, fullresponse);

                return myparsedresponse;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<MssResult> GetMssSpecialPackages(HttpClient httpClient, string lang, List<string> hgvIdList, List<string> offerIdList, DateTime from, DateTime to, XElement specialdetails, XElement hoteldetails, int typ, int service, string hgvservicecode, List<Tuple<string, string, List<string>>> myroomdata, string source, string version, string mssuser, string msspswd)
        {
            try
            {
                XElement myroomlist = new XElement("room");

                List<Room> myroompersons = (from x in myroomdata
                                            select new Room { RoomSeq = x.Item1, RoomType = x.Item2, Person = x.Item3 }).ToList();

                myroomlist = MssRequest.BuildRoomData(myroompersons);


                XElement myidlist = MssRequest.BuildIDList(hgvIdList);
                XElement myofferidlist = MssRequest.BuildOfferIDList(offerIdList);

                XDocument myrequest = MssRequest.BuildSpecialPostDataCheckAvailability(myidlist, myofferidlist, myroomlist, from, to, specialdetails, hoteldetails, typ, service, lang, source, version, mssuser, msspswd);
                var myresponses = MssRequest.RequestSpecialAsync(httpClient, myrequest);

                await Task.WhenAll(myresponses);

                Task<string> specialresponsecontent = myresponses.Result.Content.ReadAsStringAsync();

                await Task.WhenAll(specialresponsecontent);

                XElement fullresponse = XElement.Parse(specialresponsecontent.Result);

                //do muassmen iatz schaugn!
                var myparsedresponse = ParseMssSpecialResponse.ParsemyMssSpecialResponse(lang, hgvservicecode, fullresponse, myroompersons);

                //var myparsedresponse = new MssResult();

                return myparsedresponse;

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        #region Premium included

        public static async Task<List<Package>> GetMssSpecialPackages(HttpClient httpClient, string lang, List<string> Packageidlist, DateTime from, DateTime to, XElement specialdetails, int typ, int premium, int service, List<Tuple<string, string, List<string>>> myroomdata, string source, string version, string mssuser, string msspswd)
        {
            try
            {
                XElement myroomlist = new XElement("room");

                if (myroomdata != null)
                {
                    List<Room> myroompersons = (from x in myroomdata
                                                select new Room { RoomSeq = x.Item1, RoomType = x.Item2, Person = x.Item3 }).ToList();

                    myroomlist = MssRequest.BuildRoomData(myroompersons);
                }

                XElement myidlist = MssRequest.BuildOfferIDList(Packageidlist);

                XDocument myrequest = MssRequest.BuildSpecialPostDatawithPremium(myidlist, myroomlist, from, to, specialdetails, typ, premium, service, lang, source, version, mssuser, msspswd);

                var myresponses = MssRequest.RequestSpecialAsync(httpClient, myrequest);

                await Task.WhenAll(myresponses);

                Task<string> specialresponsecontent = myresponses.Result.Content.ReadAsStringAsync();

                await Task.WhenAll(specialresponsecontent);

                XElement fullresponse = XElement.Parse(specialresponsecontent.Result);

                var myparsedresponse = ParseMssSpecialResponse.ParseMySpecialResponse(lang, fullresponse);

                return myparsedresponse;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<MssResult> GetMssSpecialPackages(HttpClient httpClient, string lang, List<string> hgvIdList, List<string> offerIdList, DateTime from, DateTime to, XElement specialdetails, XElement hoteldetails, int typ, int premium, int service, string hgvservicecode, List<Tuple<string, string, List<string>>> myroomdata, string source, string version, string mssuser, string msspswd)
        {
            try
            {
                XElement myroomlist = new XElement("room");

                List<Room> myroompersons = (from x in myroomdata
                                            select new Room { RoomSeq = x.Item1, RoomType = x.Item2, Person = x.Item3 }).ToList();

                myroomlist = MssRequest.BuildRoomData(myroompersons);


                XElement myidlist = MssRequest.BuildIDList(hgvIdList);
                XElement myofferidlist = MssRequest.BuildOfferIDList(offerIdList);

                XDocument myrequest = MssRequest.BuildSpecialPostDataCheckAvailability(myidlist, myofferidlist, myroomlist, from, to, specialdetails, hoteldetails, typ, service, lang, source, version, mssuser, msspswd);
                var myresponses = MssRequest.RequestSpecialAsync(httpClient, myrequest);

                await Task.WhenAll(myresponses);

                Task<string> specialresponsecontent = myresponses.Result.Content.ReadAsStringAsync();

                await Task.WhenAll(specialresponsecontent);

                XElement fullresponse = XElement.Parse(specialresponsecontent.Result);

                //do muassmen iatz schaugn!
                var myparsedresponse = ParseMssSpecialResponse.ParsemyMssSpecialResponse(lang, hgvservicecode, fullresponse, myroompersons);

                //var myparsedresponse = new MssResult();

                return myparsedresponse;

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        #endregion

    }
}
