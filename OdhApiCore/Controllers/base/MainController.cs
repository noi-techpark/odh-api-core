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
   
    [ApiController]
    public class MainController : ControllerBase
    {
        [HttpGet, Route("api", Name = "TourismApi")]
        public IActionResult Get()
        {
            return Ok(GetMainApi(Url.Link("", null)));
        }

        private static IEnumerable<TourismData> GetMainApi(string url)
        {
            //if (url.Contains("api"))
            //    url = url.Replace("api","");

            List<TourismData> tourismdatalist = new List<TourismData>();

            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "ODHActivityPoi", Description = "Activities Pois and Gastronomies of South Tyrol from various Data Sources (LTS, Suedtirol Wein, Siag, IDM...)", Id = "it.bz.opendatahub.odhactivitypoi", SwaggerUrl = "index#/ODHActivityPoi", License = "CC0/Proprietary" }); ;
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Activity", Description = "Activities of South Tyrol from Source LTS", Id = "it.bz.opendatahub.activity", SwaggerUrl = "index#/Activity", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Accommodation", Description = "Accommodations of South Tyrol from Source LTS and HGV", Id = "it.bz.opendatahub.accommodation", SwaggerUrl = "/swagger/ui/index#/Accommodation", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Article", Description = "Various Articles about South Tyrol by IDM (Recipes, Press Articles ...)", Id = "it.bz.opendatahub.article", SwaggerUrl = "/swagger/ui/index#/Article", License = "CC0/Proprietary" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Event", Description = "Events of South Tyrol by LTS", Id = "it.bz.opendatahub.event", SwaggerUrl = "index#/Event", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Venue", Description = "Venue (Eventlocations) of South Tyrol by LTS in Alpinebits DestinationData format", Id = "it.bz.opendatahub.venue", SwaggerUrl = "index#/Venue", License = "CC0/Proprietary" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "EventShort", Description = "Events at NOI Techpark/Eurac by Eurac & Noi", Id = "it.bz.opendatahub.noi.event", SwaggerUrl = "index#/EventShort", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Gastronomy", Description = "Gastronomies of South Tyrol by LTS", Id = "it.bz.opendatahub.gastronomy", SwaggerUrl = "index#/Gastronomy", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Poi", Description = "Points of Interests of South Tyrol by LTS", Id = "it.bz.opendatahub.poi", SwaggerUrl = "index#/Poi", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Weather", Description = "Weather and Measuringpoints of South Tyrol by Province BZ", Id = "it.bz.opendatahub.tourism.weather", SwaggerUrl = "index#/Weather", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Weather/SnowReport", Description = "Snow Conditions of South Tyrol by LTS", Id = "it.bz.opendatahub.snowreport", SwaggerUrl = "index#!/Weather/Weather_GetSnowReportBase", License = "" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Weather/Measuringpoint", Description = "Measuringpoints of South Tyrol by LTS", Id = "it.bz.opendatahub.measuringpoint", SwaggerUrl = "", License = "" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "WebcamInfo", Description = "Webcams of South Tyrol by LTS", Id = "it.bz.opendatahub.webcam", SwaggerUrl = "", License = "index#/WebcamInfo" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "MetaRegion", Description = "MetaRegions (Region Clusters) of South Tyrol", Id = "it.bz.opendatahub.metaregion", SwaggerUrl = "index#/Common/Common_GetMetaRegions", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Region", Description = "Regions of South Tyrol", Id = "it.bz.opendatahub.region", SwaggerUrl = "index#/Common/Common_GetRegions", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "ExperienceArea", Description = "ExperienceAreas of South Tyrol by IDM (custom IDM defined Clusters of Locations)", Id = "it.bz.opendatahub.experiencearea", SwaggerUrl = "index#/Common/Common_GetExperienceAreas", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "TourismAssociation", Description = "TourismAssociations of South Tyrol by IDM", Id = "it.bz.opendatahub.tourismassociation", SwaggerUrl = "index#/Common/Common_GetTourismverein", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Municipality", Description = "Municipalities of South Tyrol by IDM", Id = "it.bz.opendatahub.municipality", SwaggerUrl = "index#/Common/Common_GetMunicipality", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "District", Description = "Districts of South Tyrol by IDM", Id = "it.bz.opendatahub.district", SwaggerUrl = "index#/Common/Common_GetDistrict", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Area", Description = "Areas of South Tyrol by LTS (custom Cluster of Locations) ", Id = "it.bz.opendatahub.area", SwaggerUrl = "index#/Common/Common_GetAreas", License = "" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "SkiRegion", Description = "SkiRegions of South Tyrol by IDM", Id = "it.bz.opendatahub.skiregion", SwaggerUrl = "index#!/Common/Common_GetSkiRegion", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "SkiArea", Description = "SkiAreas of South Tyrol by IDM", Id = "it.bz.opendatahub.skiarea", SwaggerUrl = "index#!/Common/Common_GetSkiAreas", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "WineAward", Description = "Wine Awards by Suedtirol Wein", Id = "it.bz.opendatahub.wineaward", SwaggerUrl = "index#/Common/Common_GetWineAwardsList", License = "CC0" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "ODHTag", Description = "Tags used by Open Data Hub in various Datatypes", Id = "it.bz.opendatahub.odhtag", SwaggerUrl = "index#/ODHTag", License = "" });
            tourismdatalist.Add(new TourismData(url) { ApiIdentifier = "Location", Description = "Location Lists of South Tyrol (including id + name of Regions, Municipalities ....)", Id = "it.bz.opendatahub.location", SwaggerUrl = "index#/Location", License = "" });

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

        public string ApiIdentifier { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public string License { get; set; }

        private string swaggerUrl;
        public string SwaggerUrl
        {
            get { return Uri.EscapeUriString(ApplicationURL + "swagger/ui/" + Uri.EscapeUriString(swaggerUrl)); }
            set { swaggerUrl = value; }
        }

        public string Self
        {
            get
            {
                return Uri.EscapeUriString(ApplicationURL + "api/" + Uri.EscapeUriString(this.ApiIdentifier));
                //return Uri.EscapeUriString(MyHttpContext.AppBaseUrl + "api/" + Uri.EscapeUriString(this.ApiIdentifier));
            }
        }
    }
}
