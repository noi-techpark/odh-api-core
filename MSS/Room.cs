using System;
using System.Collections.Generic;
using System.Text;

namespace MSS
{
    public class Room
    {
        private string roomSeq;
        public string RoomSeq
        {
            get { return roomSeq; }
            set { roomSeq = value; }
        }

        private string roomType;
        public string RoomType
        {
            get { return roomType; }
            set { roomType = value; }
        }

        private List<string> person;
        public List<string> Person
        {
            get { return person; }
            set { person = value; }
        }
    }

    public class CheapestOfferlist
    {
        public string RoomId { get; set; }
        public int RoomSeq { get; set; }
        public double Price { get; set; }

        public int? RoomFree { get; set; }
    }
}
