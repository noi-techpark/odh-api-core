// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class AccommodationV2 : AccommodationLinked
    {
        //New, holds all Infos of Trust You
        public IDictionary<string, Review>? Review { get; set; }

        //New, holds all Infos of Is/Has etc.. Properties
        public AccoProperties? AccoProperties { get; set; }

        //New, operationschedules also available on Accommodation
        public ICollection<OperationSchedule>? OperationSchedule { get; set; }

        //New Rateplans
        public ICollection<RatePlan>? RatePlan { get; set; }


        [SwaggerDeprecated("Deprecated, use Review.trustyou")]
        public new string? TrustYouID { get { return this.Review != null && this.Review.ContainsKey("trustyou") ? this.Review["trustyou"].ReviewId : ""; } }

        [SwaggerDeprecated("Deprecated, use Review.trustyou")]
        public new double? TrustYouScore { get { return this.Review != null && this.Review.ContainsKey("trustyou") ? this.Review["trustyou"].Score : null; } }

        [SwaggerDeprecated("Deprecated, use Review.trustyou")]
        public new int? TrustYouResults { get { return this.Review != null && this.Review.ContainsKey("trustyou") ? this.Review["trustyou"].Results : null; } }

        [SwaggerDeprecated("Deprecated, use Review.trustyou")]
        public new bool? TrustYouActive { get { return this.Review != null && this.Review.ContainsKey("trustyou") ? this.Review["trustyou"].Active : null; } }

        [SwaggerDeprecated("Deprecated, use Review.trustyou")]
        public new int? TrustYouState { get { return this.Review != null && this.Review.ContainsKey("trustyou") ? this.Review["trustyou"].StateInteger : null; } }

        //Accommodation Properties

        [SwaggerDeprecated("Deprecated, use AccoProperties.HasApartment")]
        public new bool? HasApartment { get { return this.AccoProperties.HasApartment; } }

        [SwaggerDeprecated("Deprecated, use AccoProperties.HasRoom")]
        public new bool? HasRoom { get { return this.AccoProperties.HasRoom; } }

        [SwaggerDeprecated("Deprecated, use AccoProperties.IsCamping")]
        public new bool? IsCamping { get { return this.AccoProperties.IsCamping; } }

        [SwaggerDeprecated("Deprecated, use AccoProperties.IsGastronomy")]
        public bool? IsGastronomy { get { return this.AccoProperties.IsGastronomy; } }

        [SwaggerDeprecated("Deprecated, use AccoProperties.IsBookable")]
        public new bool? IsBookable { get { return this.AccoProperties.IsBookable; } }

        [SwaggerDeprecated("Deprecated, use AccoProperties.IsAccommodation")]
        public new bool? IsAccommodation { get { return this.AccoProperties.IsAccommodation; } }

        [SwaggerDeprecated("Deprecated, use AccoProperties.TVMember")]
        public new bool? TVMember { get { return this.AccoProperties.TVMember; } }
    }

    public class AccommodationRoomV2 : AccommodationRoomLinked
    {
        //Overwrites The Features
        public new ICollection<AccoFeatureLinked>? Features { get; set; }

        //New Price From per Unit
        public Nullable<double> PriceFromPerUnit { get; set; }

        //New Accommodation Room Properties
        public AccommodationRoomProperties? Properties { get; set; }
    }

    //New Room Properties
    public class AccommodationRoomProperties
    {
        //New Properties
        public double? SquareMeters { get; set; }
        public int? SleepingRooms { get; set; }
        public int? Toilets { get; set; }
        public int? LivingRooms { get; set; }
        public int? DiningRooms { get; set; }
        public int? Baths { get; set; }
    }

    public class AccoProperties
    {
        public bool? HasApartment { get; set; }
        public bool? HasRoom { get; set; }
        public bool? IsCamping { get; set; }
        public bool? IsGastronomy { get; set; }
        public bool? IsBookable { get; set; }
        public bool? IsAccommodation { get; set; }
        public bool? HasDorm { get; set; }
        public bool? HasPitches { get; set; }
        public bool? TVMember { get; set; }

        //TO REMOVE?
        //public string? GastronomyId { get; set; }
        //public string? DistrictId { get; set; }
        //public string? TourismVereinId { get; set; }
        //public string? MainLanguage { get; set; }
    }

    //Shift Trust You To Reviews by using Dictionary
    public class Review
    {
        public string? ReviewId { get; set; }
        public double? Score { get; set; }
        public int? Results { get; set; }
        public bool? Active { get; set; }
        public string? State { get; set; }
        public int? StateInteger { get; set; }
        public string Provider { get; set; }
    }

    //Rateplans
    public class RatePlan
    {
        public RatePlan()
        {
            Name = new Dictionary<string, string>();
            Description = new Dictionary<string, string>();
        }

        public string ChargeType { get; set; }
        public string RatePlanId { get; set; }
        public string Code { get; set; }
        public IDictionary<string, string>? Name { get; set; }
        public IDictionary<string, string>? Description { get; set; }
        public DateTime LastUpdate { get; set; }
        public string? Visibility { get; set; }
    }

    public class AccommodationV2Helper
    {
        public static string GetTrustYouState(int trustyoustate)
        {
            //According to old LTS Documentation State (0=not rated, 1=do not display, 2=display)
            switch (trustyoustate)
            {
                case 2: return "rated";
                case 1: return "underValued";
                case 0: return "notRated";
                default: return "";
            }
        }
    }
}
