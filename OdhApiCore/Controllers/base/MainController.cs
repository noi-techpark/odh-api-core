using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceReferenceLCS;

namespace OdhApiCore.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class MainController : ControllerBase
    {
        //Solved with Redirect
        //[HttpGet, Route("api")]
        [HttpGet, Route("v1", Name = "TourismApi")]
        public IActionResult Get()
        {
            //var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
            var location = new Uri($"{Request.Scheme}://{Request.Host}");

            return Ok(GetMainApi(location.AbsoluteUri));
        }

        private static IEnumerable<TourismData> GetMainApi(string url)
        {
            List<TourismData> tourismdatalist = new List<TourismData>();

            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Find", Description = "Search through opendatahub datasets", Id = "it.bz.opendatahub.find", OdhType = "", SwaggerUrl = "https://tourism.opendatahub.bz.it/swagger/index.html#/Search", License = "CC0/Proprietary" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "ODHActivityPoi", Description = "Activities Pois and Gastronomies of South Tyrol from various Data Sources (LTS, Suedtirol Wein, Siag, IDM...)", Id = "it.bz.opendatahub.odhactivitypoi", OdhType="odhactivitypoi", SwaggerUrl = "ODHActivityPoi", License = "CC0/Proprietary" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Activity", Description = "Activities of South Tyrol from Source LTS", Id = "it.bz.opendatahub.ltsactivity", OdhType = "ltsactivity", SwaggerUrl = "Activity", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Accommodation", Description = "Accommodations of South Tyrol from Source LTS and HGV", Id = "it.bz.opendatahub.accommodation", OdhType = "accommodation", SwaggerUrl = "Accommodation", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Article", Description = "Various Articles about South Tyrol by IDM (Recipes, Press Articles ...)", Id = "it.bz.opendatahub.article", OdhType = "article", SwaggerUrl = "Article", License = "CC0/Proprietary" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Event", Description = "Events of South Tyrol by LTS", Id = "it.bz.opendatahub.event", OdhType = "event", SwaggerUrl = "Event", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Venue", Description = "Venue (Eventlocations) of South Tyrol by LTS in Alpinebits DestinationData format", Id = "it.bz.opendatahub.venue", OdhType = "venue", SwaggerUrl = "Venue", License = "CC0/Proprietary" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "EventShort", Description = "Events at NOI Techpark/Eurac by Eurac & Noi", Id = "it.bz.opendatahub.noi.eventshort", OdhType = "eventshort", SwaggerUrl = "EventShort", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Gastronomy", Description = "Gastronomies of South Tyrol by LTS", Id = "it.bz.opendatahub.ltsgastronomy", OdhType = "ltsgastronomy", SwaggerUrl = "Gastronomy", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Poi", Description = "Points of Interests of South Tyrol by LTS", Id = "it.bz.opendatahub.poi", OdhType = "ltspoi", SwaggerUrl = "Poi", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Weather", Description = "Weather and Measuringpoints of South Tyrol by Province BZ", Id = "it.bz.opendatahub.tourism.weather", OdhType = "weather", SwaggerUrl = "Weather", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Weather/SnowReport", Description = "Snow Conditions of South Tyrol by LTS", Id = "it.bz.opendatahub.snowreport", OdhType = "snowreport", SwaggerUrl = "Weather/get_v1_Weather_SnowReport", License = "" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Weather/Measuringpoint", Description = "Measuringpoints of South Tyrol by LTS", Id = "it.bz.opendatahub.measuringpoint", OdhType = "measuringpoint", SwaggerUrl = "Weather/get_v1_Weather_Measuringpoint", License = "" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "WebcamInfo", Description = "Webcams of South Tyrol by LTS", Id = "it.bz.opendatahub.webcam", OdhType = "webcam", SwaggerUrl = "", License = "WebcamInfo" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "MetaRegion", Description = "MetaRegions (Region Clusters) of South Tyrol", Id = "it.bz.opendatahub.metaregion", OdhType = "metaregion", SwaggerUrl = "Common/get_v1_MetaRegion", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Region", Description = "Regions of South Tyrol", Id = "it.bz.opendatahub.region", OdhType = "region", SwaggerUrl = "Common/get_v1_Region", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "ExperienceArea", Description = "ExperienceAreas of South Tyrol by IDM (custom IDM defined Clusters of Locations)", Id = "it.bz.opendatahub.experiencearea", OdhType = "experiencearea", SwaggerUrl = "Common/get_v1_ExperienceArea", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "TourismAssociation", Description = "TourismAssociations of South Tyrol by IDM", Id = "it.bz.opendatahub.tourismassociation", OdhType = "tourismassociation", SwaggerUrl = "Common/get_v1_TourismAssociation", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Municipality", Description = "Municipalities of South Tyrol by IDM", Id = "it.bz.opendatahub.municipality", OdhType = "municipality", SwaggerUrl = "Common/get_v1_Municipality", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "District", Description = "Districts of South Tyrol by IDM", Id = "it.bz.opendatahub.district", OdhType = "district", SwaggerUrl = "Common/get_v1_District", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Area", Description = "Areas of South Tyrol by LTS (custom Cluster of Locations) ", Id = "it.bz.opendatahub.area", OdhType = "area", SwaggerUrl = "Common/get_v1_Area", License = "" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "SkiRegion", Description = "SkiRegions of South Tyrol by IDM", Id = "it.bz.opendatahub.skiregion", OdhType = "skiregion", SwaggerUrl = "Common/get_v1_SkiRegion", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "SkiArea", Description = "SkiAreas of South Tyrol by IDM", Id = "it.bz.opendatahub.skiarea", OdhType = "skiarea", SwaggerUrl = "Common/get_v1_SkiArea", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "WineAward", Description = "Wine Awards by Suedtirol Wein", Id = "it.bz.opendatahub.wineaward", OdhType = "wineaward", SwaggerUrl = "Common/get_v1_WineAward", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "ODHTag", Description = "Tags used by Open Data Hub in various Datatypes", Id = "it.bz.opendatahub.odhtag", OdhType = "odhtag", SwaggerUrl = "ODHTag", License = "" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Location", Description = "Location Lists of South Tyrol (including id + name of Regions, Municipalities ....)",  Id = "it.bz.opendatahub.location", OdhType = "location", SwaggerUrl = "Location", License = "" });

            return tourismdatalist;
        }
    }    

    public class TourismData
    {
        public TourismData(string url)
        {
            ApplicationURL = url;
        }

        private string ApplicationURL { get; set; }

        public string ApiIdentifier { get; set; } = default!;
        public string Id { get; set; } = default!;
        public string OdhType { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string License { get; set; } = default!;

        private string swaggerUrl = default!;
        public string SwaggerUrl
        {
            get { return Uri.EscapeDataString(ApplicationURL + "swagger/index.html#/" + swaggerUrl); }
            set { swaggerUrl = value; }
        }

        public string Self
        {
            get
            {
                return Uri.EscapeDataString(ApplicationURL + "v1/" + Uri.EscapeDataString(this.ApiIdentifier));
                //return Uri.EscapeDataString(MyHttpContext.AppBaseUrl + "api/" + Uri.EscapeDataString(this.ApiIdentifier));
            }
        }
    }
}
