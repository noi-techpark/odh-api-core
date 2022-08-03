using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OdhApiCore.Controllers.helper
{
    public class MssHelper
    {
        public readonly string mssrequestlanguage;
        public readonly string[] mybokchannels;
        public readonly List<string> accoidlist;
        public readonly string idsofchannel;
        public readonly List<Tuple<string,string,List<string>>> myroomdata;
        public readonly XElement xoffertype;
        public readonly XElement xhoteldetails;
        public readonly DateTime arrival;
        public readonly DateTime departure;
        public readonly int service;
        public readonly int rooms;
        public readonly string hgvservicecode;
        public readonly string source;
        public readonly string mssversion;

        public static MssHelper Create(
            List<string> accoidlist, string idsofchannel,
            string bokfilter, string language, string roominfo, string boardfilter,
            string arrival, string departure, int? detail, 
            string source, string mssversion)
        {
            return new MssHelper(
                accoidlist: accoidlist, idsofchannel: idsofchannel, bokfilter: bokfilter, language: language,
                roominfo: roominfo, boardfilter: boardfilter, arrival: arrival, 
                departure: departure, detail: detail,
                source: source, mssversion: mssversion);
        }

        private MssHelper(
            List<string> accoidlist, string idsofchannel,
            string bokfilter, string language, string roominfo, string boardfilter,
            string arrival, string departure, int? detail, string source,
            string mssversion)
        {
            if (bokfilter.EndsWith(","))
                bokfilter = bokfilter.Substring(0, bokfilter.Length - 1);

            mybokchannels = bokfilter.Split(',');

            var servicetuple = Helper.AccoListCreator.CreateBoardListHGVfromFlag(boardfilter);

            service = servicetuple.Item1;
            hgvservicecode = servicetuple.Item2;

            myroomdata = Helper.AccoListCreator.BuildMyRoomInfo(roominfo);

            rooms = myroomdata.Count;

            int? offerdetail = null;
            int hoteldetail = 524288;

            if (detail != null && detail == 1)
            {
                offerdetail = 33081;
                if (mssversion == "2")
                    offerdetail = 1343801;

                hoteldetail = 524800;
            }

            xoffertype = new XElement("offer_details", offerdetail);
            xhoteldetails = new XElement("hotel_details", hoteldetail); //524288

            this.arrival = DateTime.Parse(arrival);
            this.departure = DateTime.Parse(departure);

            this.source = source;
            this.mssversion = mssversion;

            if (language.ToLower() == "nl" || language.ToLower() == "cs" || language.ToLower() == "pl" || language.ToLower() == "fr" || language.ToLower() == "ru")
                mssrequestlanguage = "en";
            else
                mssrequestlanguage = language.ToLower();

            this.accoidlist = accoidlist != null && accoidlist.Count > 0 ? accoidlist.Select(x => x.ToUpper()).ToList() : accoidlist ?? new();            

            this.idsofchannel = idsofchannel;
        }

    }
}
