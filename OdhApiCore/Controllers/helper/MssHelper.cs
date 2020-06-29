using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OdhApiCore.Controllers.helper
{
    public class MssHelper
    {
        public string? mssrequestlanguage;        
        public string[] mybokchannels;
        public List<string>? a0ridlist;
        public List<string>? hgvidlist;
        public List<Tuple<string,string,List<string>>> myroomdata;        
        public XElement xoffertype;
        public XElement xhoteldetails;
        public DateTime? arrival;
        public DateTime? departure;
        public int? service;
        public int? rooms;
        public string? hgvservicecode;
        public string? source;
        public string? mssversion;

        public static MssHelper Create(
            List<string> a0ridlist, List<string> hgvidlist,
            string? bokfilter, string? language, string? roominfo, string? boardfilter,
            string? arrival, string? departure, int? hoteldetails, int? offerdetails, 
            string? source, string? mssversion)
        {
            return new MssHelper(
                a0ridlist: a0ridlist, hgvidlist: hgvidlist, bokfilter: bokfilter, language: language,
                roominfo: roominfo, boardfilter: boardfilter, arrival: arrival, 
                departure: departure, hoteldetails: hoteldetails, offerdetails: offerdetails,
                source: source, mssversion: mssversion);
        }

        private MssHelper(
            List<string> a0ridlist, List<string> hgvidlist,
            string? bokfilter, string? language, string? roominfo, string? boardfilter,
            string? arrival, string? departure, int? hoteldetails, int? offerdetails, string? source, 
            string? mssversion)
        {
            if (bokfilter != null && bokfilter.EndsWith(","))
                bokfilter = bokfilter.Substring(0, bokfilter.Length - 1);

            mybokchannels = bokfilter.Split(',');

            var servicetuple = Helper.AccoListCreator.CreateBoardListHGVfromFlag(boardfilter);

            service = servicetuple.Item1;
            hgvservicecode = servicetuple.Item2;

            myroomdata = Helper.AccoListCreator.BuildMyRoomInfo(roominfo);

            rooms = myroomdata.Count;
            
            xoffertype = new XElement("offer_details");
            xhoteldetails = new XElement("hotel_details", hoteldetails); //524288

            this.arrival = DateTime.Parse(arrival);
            this.departure = DateTime.Parse(departure);

            this.source = source;
            this.mssversion = mssversion;

            if (language.ToLower() == "nl" || language.ToLower() == "cs" || language.ToLower() == "pl" || language.ToLower() == "fr" || language.ToLower() == "ru")
                mssrequestlanguage = "en";
            else
                mssrequestlanguage = language.ToLower();

            this.a0ridlist = a0ridlist;

            this.hgvidlist = hgvidlist;
        }

    }
}
