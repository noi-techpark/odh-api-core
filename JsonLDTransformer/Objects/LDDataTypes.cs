// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonLDTransformer.Objects
{
    public class ObjectLD
    {
        [JsonProperty(PropertyName = "@context", Order = 0)]
        public string context { get; set; }

        [JsonProperty(PropertyName = "@type", Order = 1)]
        public string type { get; set; }

        [JsonProperty(PropertyName = "@id", Order = 2)]
        public string id { get; set; }
    }


    public class RecipeLD : ObjectLD
    {
    }

    public class EventLD : ObjectLD
    {
        [JsonProperty(Order = 3)]
        public string name { get; set; }

        [JsonProperty(Order = 4)]
        public string description { get; set; }

        [JsonProperty(Order = 5)]
        public string image { get; set; }

        [JsonProperty(Order = 6)]
        public string startDate { get; set; }

        [JsonProperty(Order = 7)]
        public string endDate { get; set; }

        [JsonProperty(Order = 8)]
        public string url { get; set; }     

        [JsonProperty(Order = 9)]
        public PlaceLD location { get; set; }

        [JsonProperty(Order = 10)]
        public OfferLD offers { get; set; }

        [JsonProperty(Order = 11)]
        public OrganizationLD organizer { get; set; }

    }

    public class RestaurantLD : ObjectLD
    {
        [JsonProperty(Order = 3)]
        public string name { get; set; }

        [JsonProperty(Order = 4)]
        public string image { get; set; }

        [JsonProperty(Order = 5)]
        public string description { get; set; }

        [JsonProperty(Order = 6)]
        public string telephone { get; set; }

        [JsonProperty(Order = 7)]
        public string email { get; set; }

        [JsonProperty(Order = 8)]
        public string url { get; set; }

        [JsonProperty(Order = 9)]
        public addressLD address { get; set; }

        [JsonProperty(Order = 10)]
        public geoLD geo { get; set; }

        [JsonProperty(Order = 11)]
        public List<string> openingHours { get; set; }

        [JsonProperty(Order = 12)]
        public List<string> servesCuisine { get; set; }

        [JsonProperty(Order = 8)]
        public personLD founder { get; set; }


    }

    public class TouristAttractionLD : ObjectLD
    {
    }

    public class ArticleLD : ObjectLD
    {
    }

    public class SkiResortLD : ObjectLD
    {
    }

    public class OrganizationLD : ObjectLD
    {
        [JsonProperty(Order = 3)]
        public string name { get; set; }

        [JsonProperty(Order = 4)]
        public string telephone { get; set; }

        [JsonProperty(Order = 5)]
        public string email { get; set; }

        [JsonProperty(Order = 6)]
        public string url { get; set; }

        [JsonProperty(Order = 7)]
        public addressLD address { get; set; }

    }

    public class PlaceLD 
    {
        [JsonProperty(PropertyName = "@type", Order = 0)]
        public string type { get; set; }

        [JsonProperty(Order = 1)]
        public string name { get; set; }

        [JsonProperty(Order = 2)]
        public addressLD address { get; set; }  
    }

    public class OfferLD
    {
        [JsonProperty(PropertyName = "@type", Order = 0)]
        public string type { get; set; }

        [JsonProperty(Order = 1)]
        public string name { get; set; }

        [JsonProperty(Order = 2)]
        public string description { get; set; }

        [JsonProperty(Order = 3)]
        public string url { get; set; }

        [JsonProperty(Order = 4)]
        public string validFrom { get; set; }

        [JsonProperty(Order = 5)]
        public string validTrough { get; set; }

        [JsonProperty(Order = 6)]
        public string price { get; set; }

        [JsonProperty(Order = 7)]
        public string priceCurrency { get; set; }
    }

    public class LodgingBusinessLD : ObjectLD
    {
    }

    public class BookLD : ObjectLD
    {
    }

    public class StoreLD : ObjectLD
    {
    }

    public class LocalBusinessLD : ObjectLD
    {
    }

    public class HotelLD : ObjectLD
    {
        //[JsonProperty(PropertyName = "@context", Order = 0)]
        //public string context { get; set; }

        //[JsonProperty(PropertyName = "@type", Order = 1)]
        //public string type { get; set; }

        //[JsonProperty(PropertyName = "@id", Order = 2)]
        //public string id { get; set; }

        [JsonProperty(Order = 3)]
        public string name { get; set; }

        [JsonProperty(Order = 4)]
        public string image { get; set; }

        [JsonProperty(Order = 5)]
        public string description { get; set; }
        
        [JsonProperty(Order = 6)]
        public string telephone { get; set; }

        [JsonProperty(Order = 7)]
        public string email { get; set; }

        [JsonProperty(Order = 8)]
        public string url { get; set; }

        [JsonProperty(Order = 9)]
        public addressLD address { get; set; }

        [JsonProperty(Order = 10)]
        public geoLD geo { get; set; }
    }
}

//Wiederverwendete Propertys
public class addressLD
{
    [JsonProperty(PropertyName = "@type", Order = 0)]
    public string type { get; set; }

    [JsonProperty(Order = 1)]
    public string streetAddress { get; set; }
    [JsonProperty(Order = 2)]
    public string postalCode { get; set; }
    [JsonProperty(Order = 3)]
    public string addressLocality { get; set; }
    [JsonProperty(Order = 4)]
    public string addressRegion { get; set; }
    [JsonProperty(Order = 5)]
    public string addressCountry { get; set; }
}

public class geoLD
{
    [JsonProperty(PropertyName = "@type", Order = 0)]
    public string type { get; set; }

    [JsonProperty(Order = 1)]
    public double latitude { get; set; }
    [JsonProperty(Order = 2)]
    public double longitude { get; set; }
}

public class personLD
{
    [JsonProperty(PropertyName = "@type", Order = 0)]
    public string type { get; set; }

    [JsonProperty(Order = 1)]
    public string name { get; set; }
   
}