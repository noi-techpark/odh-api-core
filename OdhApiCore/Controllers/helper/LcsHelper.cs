using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OdhApiCore.Controllers.helper
{
    public class LcsHelper
    {
        public readonly string lcsrequestlanguage;        
        public readonly List<string> accoidlist;        
        public readonly List<Helper.LCS.LCSRoomStay> myroomdata;        
        public readonly string arrival;
        public readonly string departure;
        public readonly List<string> service;
        public readonly int rooms;
        public readonly string source;
        
        public static LcsHelper Create(
            List<string> accoidlist, string language, string roominfo, string boardfilter,
            string arrival, string departure, string source)
        {
            return new LcsHelper(
                accoidlist: accoidlist, language: language,
                roominfo: roominfo, boardfilter: boardfilter, arrival: arrival, 
                departure: departure, source: source);
        }

        private LcsHelper(
            List<string> accoidlist, string language, string roominfo, string boardfilter,
            string arrival, string departure, string source)
        {

            service = Helper.AccoListCreator.CreateBoardListLCSfromFlag(boardfilter);
            myroomdata = Helper.LCS.GetAccommodationDataLCS.RoomstayTransformer(roominfo);
            
            rooms = myroomdata.Count;

            this.arrival = arrival;
            this.departure = departure;

            this.source = source;

            if (language.ToLower() == "nl" || language.ToLower() == "cs" || language.ToLower() == "pl" || language.ToLower() == "fr" || language.ToLower() == "ru")
                lcsrequestlanguage = "en";
            else
                lcsrequestlanguage = language.ToLower();

            this.accoidlist = accoidlist;            
        }

    }
}
