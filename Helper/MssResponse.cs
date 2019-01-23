using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    //Lei zun testen!
    public class MssResult
    {
        public MssResult()
        {
            this.MssResponseShort = new HashSet<MssResponseShort>();
        }

        public string ResultId { get; set; }
        public string CheapestChannel { get; set; }
        public double Cheapestprice { get; set; }
        public int bookableHotels { get; set; }
        public virtual ICollection<MssResponseShort> MssResponseShort { get; set; }
    }


    public class MssResponseShort
    {
        public MssResponseShort()
        {
            this.RoomDetails = new HashSet<RoomDetails>();
        }

        public int HotelId { get; set; }
        public string A0RID { get; set; }
        public bool Bookable { get; set; }
        public string ChannelID { get; set; }
        public string Channellink { get; set; }
        public string OfferId { get; set; }
        public string OfferGid { get; set; }
        public Nullable<int> OfferTyp { get; set; }
        public Nullable<int> OfferShow { get; set; }
        public double CheapestOffer { get; set; }
        public string CheapestOfferString { get; set; }

        //neu zusatz
        public string OnlinepaymentMethods { get; set; }
        public string OnlinepaymentPrepayment { get; set; }
        public string OnlinepaymentCCards { get; set; }

        //magari die Cheapestoffers pro Service ospeichern??
        public Nullable<double> CheapestOffer_ws { get; set; }
        public Nullable<double> CheapestOffer_bb { get; set; }
        public Nullable<double> CheapestOffer_hb { get; set; }
        public Nullable<double> CheapestOffer_fb { get; set; }
        public Nullable<double> CheapestOffer_ai { get; set; }

        public virtual ICollection<RoomDetails> RoomDetails { get; set; }
    }

    public class RoomDetails
    {
        public string RoomId { get; set; }
        public Nullable<int> RoomSeq { get; set; }
        public double TotalPrice { get; set; }
        public string OfferId { get; set; }

        public Nullable<double> Price_ws { get; set; }
        public Nullable<double> Price_bb { get; set; }
        public Nullable<double> Price_hb { get; set; }
        public Nullable<double> Price_fb { get; set; }
        public Nullable<double> Price_ai { get; set; }

        public Nullable<int> Roomtype { get; set; }
        public Nullable<int> Roomfree { get; set; }

        //Zusatz RoomMax
        public Nullable<int> Roommax { get; set; }
        //Zusatz RoomMin
        public Nullable<int> Roommin { get; set; }
        //Zusatz RoomStd
        public Nullable<int> Roomstd { get; set; }


        public string Roomtitle { get; set; }
        public string Roomdesc { get; set; }
        public string RoomChannelLink { get; set; }
        //public string Service { get; set; }

        public string TotalPriceString { get; set; }

        public virtual ICollection<RoomPictures> RoomPictures { get; set; }

        ///NEU MSS Umstellung FELDER

        public PaymentTerm PaymentTerm { get; set; }
        public CancelPolicy CancelPolicy { get; set; }
    }

    public class RoomPictures
    {
        public string Pictureurl { get; set; }
    }

    public class PaymentTerm
    {
        public string Id { get; set; }
        public int Methods { get; set; }
        public int Prepayment { get; set; }
        public int Ccards { get; set; }
        public int Priority { get; set; }
        public string Description { get; set; }

        public Bank Bank { get; set; }
    }

    public class Bank
    {
        public string Name { get; set; }
        public string Iban { get; set; }
        public string Swift { get; set; }
    }

    public class CancelPolicy
    {
        public string Id { get; set; }
        public int? Refundable { get; set; }
        public DateTime? RefundableUntil { get; set; }
        public string Description { get; set; }

        public List<Penalty> Penalties { get; set; }
    }

    public class Penalty
    {
        public int? Percent { get; set; }
        public DateTime? Datefrom { get; set; }
        public int? Daysarrival { get; set; }
    }


    //TODO
    public class MssResponseComplete
    {
    }
}
