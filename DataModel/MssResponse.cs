using System;
using System.Collections.Generic;

namespace DataModel
{
    public class MssResult
    {
        public string? ResultId { get; set; }
        public string? CheapestChannel { get; set; }
        public double? Cheapestprice { get; set; }
        public int? bookableHotels { get; set; }
        public virtual ICollection<MssResponseShort>? MssResponseShort { get; set; } = new HashSet<MssResponseShort>();
    }

    public class MssResponseShort
    {
        public int HotelId { get; set; }
        public string? A0RID { get; set; }
        public bool Bookable { get; set; }
        public string? ChannelID { get; set; }
        public string? Channellink { get; set; }
        public string? OfferId { get; set; }
        public string? OfferGid { get; set; }
        public int? OfferTyp { get; set; }
        public int? OfferShow { get; set; }
        public double CheapestOffer { get; set; }
        public string? CheapestOfferString { get; set; }

        //neu zusatz
        public string? OnlinepaymentMethods { get; set; }
        public string? OnlinepaymentPrepayment { get; set; }
        public string? OnlinepaymentCCards { get; set; }

        //magari die Cheapestoffers pro Service ospeichern??
        public double? CheapestOffer_ws { get; set; }
        public double? CheapestOffer_bb { get; set; }
        public double? CheapestOffer_hb { get; set; }
        public double? CheapestOffer_fb { get; set; }
        public double? CheapestOffer_ai { get; set; }

        public virtual ICollection<RoomDetails> RoomDetails { get; set; } = new List<RoomDetails>();
    }

    public class RoomDetails
    {
        public string? RoomId { get; set; }
        public int? RoomSeq { get; set; }
        public double TotalPrice { get; set; }
        public string? OfferId { get; set; }

        public double? Price_ws { get; set; }
        public double? Price_bb { get; set; }
        public double? Price_hb { get; set; }
        public double? Price_fb { get; set; }
        public double? Price_ai { get; set; }

        public double? Roomtype { get; set; }
        public double? Roomfree { get; set; }

        //Zusatz RoomMax
        public int? Roommax { get; set; }
        //Zusatz RoomMin
        public int? Roommin { get; set; }
        //Zusatz RoomStd
        public int? Roomstd { get; set; }

        public string? Roomtitle { get; set; }
        public string? Roomdesc { get; set; }
        public string? RoomChannelLink { get; set; }
        //public string Service { get; set; }

        public string? TotalPriceString { get; set; }

        //public virtual ICollection<RoomPictures> RoomPictures => new List<RoomPictures>();
        public virtual ICollection<RoomPictures> RoomPictures { get; set; }

        ///NEU MSS Umstellung FELDER

        public PaymentTerm? PaymentTerm { get; set; }
        public CancelPolicy? CancelPolicy { get; set; }
    }

    public class RoomPictures
    {
        public string? Pictureurl { get; set; }
    }

    public class PaymentTerm
    {
        public string? Id { get; set; }
        public int Methods { get; set; }
        public int Prepayment { get; set; }
        public int Ccards { get; set; }
        public int Priority { get; set; }
        public string? Description { get; set; }

        public Bank? Bank { get; set; }
    }

    public class Bank
    {
        public string? Name { get; set; }
        public string? Iban { get; set; }
        public string? Swift { get; set; }
    }

    public class CancelPolicy
    {
        public string? Id { get; set; }
        public int? Refundable { get; set; }
        public DateTime? RefundableUntil { get; set; }
        public string? Description { get; set; }

        public List<Penalty> Penalties { get; set; }  //=> new List<Penalty>();
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
