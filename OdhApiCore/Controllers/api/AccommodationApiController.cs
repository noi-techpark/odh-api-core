using AspNetCore.CacheOutput;
using CDB;
using DataModel;
using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSS;
using OdhApiCore.Filters;
using OdhApiCore.Responses;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OdhApiCore.Controllers
{
    /// <summary>
    /// Accommodation Api (data provided by LTS / Availability Requests provided by HGV/LTS) SOME DATA Available as OPENDATA 
    /// </summary>
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class AccommodationController : OdhController
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ISettings settings;

        public AccommodationController(IWebHostEnvironment env, ISettings settings, ILogger<AccommodationController> logger, QueryFactory queryFactory, IHttpClientFactory httpClientFactory)
            : base(env, settings, logger, queryFactory)
        {
            this.httpClientFactory = httpClientFactory;
            this.settings = settings;
        }

        //Duplicate on AVailabilitySearchInterceptorAttribute
        private bool CheckAvailabilitySearch(System.Security.Claims.ClaimsPrincipal User)
        {
            List<string> roles = new List<string>() { "DataReader", "AccoReader" };

            foreach (var role in roles)
            {
                if (User.IsInRole(role))
                    return true;
            }
            
            return false;
        }

        #region SWAGGER Exposed API

        /// <summary>
        /// GET Accommodation List
        /// </summary>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page (If availabilitycheck set, pagesize has no effect all Accommodations are returned), (default:10)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="categoryfilter">Categoryfilter BITMASK values: 1 = (not categorized), 2 = (1star), 4 = (1flower), 8 = (1sun), 14 = (1star/1flower/1sun), 16 = (2stars), 32 = (2flowers), 64 = (2suns), 112 = (2stars/2flowers/2suns), 128 = (3stars), 256 = (3flowers), 512 = (3suns), 1024 = (3sstars), 1920 = (3stars/3flowers/3suns/3sstars), 2048 = (4stars), 4096 = (4flowers), 8192 = (4suns), 16384 = (4sstars), 30720 = (4stars/4flowers/4suns/4sstars), 32768 = (5stars), 65536 = (5flowers), 131072 = (5suns), 229376 = (5stars/5flowers/5suns), 'null' = (No Filter), (default:'null')</param>
        /// <param name="typefilter">Typefilter BITMASK values: 1 = (HotelPension), 2 = (BedBreakfast), 4 = (Farm), 8 = (Camping), 16 = (Youth), 32 = (Mountain), 64 = (Apartment), 128 = (Not defined),'null' = (No Filter), (default:'null')</param>
        /// <param name="boardfilter">Boardfilter BITMASK values: 0 = (all boards), 1 = (without board), 2 = (breakfast), 4 = (half board), 8 = (full board), 16 = (All inclusive), 'null' = (No Filter), (default:'null')</param>
        /// <param name="featurefilter">FeatureFilter BITMASK values: 1 = (Group-friendly), 2 = (Meeting rooms), 4 = (Swimming pool), 8 = (Sauna), 16 = (Garage), 32 = (Pick-up service), 64 = (WLAN), 128 = (Barrier-free), 256 = (Special menus for allergy sufferers), 512 = (Pets welcome), 'null' = (No Filter), (default:'null')</param>
        /// <param name="featureidfilter">Feature Id Filter, LIST filter over ALL Features available. Separator ',' List of Feature IDs, 'null' = (No Filter), (default:'null')</param>
        /// <param name="themefilter">Themefilter BITMASK values: 1 = (Gourmet), 2 = (At altitude), 4 = (Regional wellness offerings), 8 = (on the wheels), 16 = (With family), 32 = (Hiking), 64 = (In the vineyards), 128 = (Urban vibe), 256 = (At the ski resort), 512 = (Mediterranean), 1024 = (In the Dolomites), 2048 = (Alpine), 4096 = (Small and charming), 8192 = (Huts and mountain inns), 16384 = (Rural way of life), 32768 = (Balance), 65536 = (Christmas markets), 131072 = (Sustainability), 'null' = (No Filter), (default:'null')</param>
        /// <param name="badgefilter">BadgeFilter BITMASK values: 1 = (Belvita Wellness Hotel), 2 = (Familyhotel), 4 = (Bikehotel), 8 = (Red Rooster Farm), 16 = (Barrier free certificated), 32 = (Vitalpina Hiking Hotel), 64 = (Private Rooms in South Tyrol), 128 = (Vinum Hotels), 'null' = (No Filter), (default:'null')</param>        
        /// <param name="idfilter">IDFilter LIST Separator ',' List of Accommodation IDs, 'null' = (No Filter), (default:'null')</param>
        /// <param name="locfilter">Locfilter SPECIAL Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction), 'null' = (No Filter), (default:'null') <a href="https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#location-filter-locfilter" target="_blank">Wiki locfilter</a></param>        
        /// <param name="odhtagfilter">ODHTag Filter LIST (refers to Array SmgTags) (String, Separator ',' more ODHTags possible, 'null' = No Filter, available ODHTags reference to 'v1/ODHTag?validforentity=accommodation'), (default:'null')</param>
        /// <param name="odhactive">ODHActive Filter BOOLEAN (refers to field SmgActive) (possible Values: 'null' Displays all Accommodations, 'true' only ODH Active Accommodations, 'false' only ODH Disabled Accommodations), (default:'null')</param>       
        /// <param name="active">TIC Active Filter BOOLEAN (possible Values: 'null' Displays all Accommodations, 'true' only TIC Active Accommodations, 'false' only TIC Disabled Accommodations), (default:'null')</param>       
        /// <param name="altitudefilter">Altitude Range Filter SPECIAL (Separator ',' example Value: 500,1000 Altitude from 500 up to 1000 metres), (default:'null')</param>
        /// <param name="availabilitycheck">Availability Check BOOLEAN (possible Values: 'true', 'false), (default Value: 'false') NOT AVAILABLE AS OPEN DATA, IF Availabilty Check is true certain filters are Required</param>
        /// <param name="arrival">Arrival DATE (yyyy-MM-dd) REQUIRED ON Availabilitycheck = true, (default:'Today's date')</param>
        /// <param name="departure">Departure DATE (yyyy-MM-dd) REQUIRED ON Availabilitycheck = true, (default:'Tomorrow's date')</param>
        /// <param name="roominfo">Roominfo Filter REQUIRED ON Availabilitycheck = true (Splitter for Rooms '|' Splitter for Persons Ages ',') (Room Types: 0=notprovided, 1=room, 2=apartment, 4=pitch/tent(onlyLTS), 8=dorm(onlyLTS)) possible Values Example 1-18,10|1-18 = 2 Rooms, Room 1 for 2 person Age 18 and Age 10, Room 2 for 1 Person Age 18), (default:'1-18,18')</param>
        /// <param name="bokfilter">Booking Channels Filter REQUIRED ON Availabilitycheck = true (Separator ',' possible values: hgv = (Booking Südtirol), htl = (Hotel.de), exp = (Expedia), bok = (Booking.com), lts = (LTS Availability check)), (default:'hgv')</param>
        /// <param name="source">Source for MSS availability check, (default:'sinfo')</param>
        /// <param name="availabilitychecklanguage">Language of the Availability Response (possible values: 'de','it','en')</param>
        /// <param name="detail">Detail of the Availablity check (string, 1 = full Details, 0 = basic Details (default))</param>
        /// <param name="latitude">GeoFilter FLOAT Latitude Format: '46.624975', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="longitude">GeoFilter FLOAT Longitude Format: '11.369909', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="radius">Radius INTEGER to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="language">Language field selector, displays data and fields in the selected language, possible values: 'de|it|en|nl|cs|pl|fr|ru' only one language supported (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="publishedon">Published On Filter (Separator ',' List of publisher IDs), (default:'null')</param>       
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter' target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href='https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter' target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href='https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter' target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of Accommodation Objects</returns>
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(JsonResult<AccommodationLinked>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        
        [TypeFilter(typeof(Filters.AvailabilitySearchInterceptorAttribute))]
        [OdhCacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 3600, CacheKeyGenerator = typeof(CustomCacheKeyGenerator), MustRevalidate = true)]
        [HttpGet, Route("Accommodation", Name = "AccommodationList")]
        public async Task<IActionResult> GetAccommodations(
            uint pagenumber = 1,
            PageSize pagesize = null!,
            string? seed = null,
            string? categoryfilter = null,
            string? typefilter = null,
            string? boardfilter = null,
            string? featurefilter = null,
            string? featureidfilter = null,
            string? themefilter = null,
            string? badgefilter = null,
            string? idfilter = null,
            string? locfilter = null,
            string? altitudefilter = null,
            string? odhtagfilter = null,
            LegacyBool odhactive = null!,
            LegacyBool active = null!,
            LegacyBool bookablefilter = null!,
            string? arrival = null,
            string? departure = null,
            string? roominfo = "1-18,18",
            string? bokfilter = "hgv",
            string? source = "sinfo",
            string? availabilitychecklanguage = "en",
            string? detail = "0",
            LegacyBool availabilitycheck = null!,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            string? publishedon = null,
            string? language = null,
            string? langfilter = null,
            string? updatefrom = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            //if availabilitysearch requested and User not logged
            if (availabilitycheck?.Value == true && !CheckAvailabilitySearch(User))
            {
                return Unauthorized("User not allowed for availabilitysearch");
            }

            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            List<string> bokfilterlist = bokfilter?.Split(',').ToList() ?? new List<string>();

            if (availabilitycheck?.Value != true)
            {
                return await GetFiltered(
                    fields: fields ?? Array.Empty<string>(), language: language, pagenumber: pagenumber,
                    pagesize: pagesize, idfilter: idfilter, idlist: new List<string>(), locfilter: locfilter, categoryfilter: categoryfilter,
                    typefilter: typefilter, boardfilter: boardfilter, featurefilter: featurefilter, featureidfilter: featureidfilter, themefilter: themefilter, badgefilter: badgefilter,
                    altitudefilter: altitudefilter, active: active, smgactive: odhactive, bookablefilter: bookablefilter, smgtagfilter: odhtagfilter, publishedon: publishedon,
                    seed: seed, updatefrom: updatefrom, langfilter: langfilter, searchfilter: searchfilter, geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }
            else if (availabilitycheck?.Value == true)
            {
                var accobooklist = Request.HttpContext.Items["accobooklist"];
                var accoavailabilitymss = Request.HttpContext.Items["mssavailablity"];
                var accoavailabilitylcs = Request.HttpContext.Items["lcsavailablity"];

                var availableonlineaccos = new List<string>();
                if (accoavailabilitymss != null)
                    availableonlineaccos.AddRange(((MssResult?)accoavailabilitymss)?.MssResponseShort?.Select(x => x.A0RID?.ToUpper() ?? "").Distinct().ToList() ?? new List<string>());
                if (accoavailabilitylcs != null)
                    availableonlineaccos.AddRange(((MssResult?)accoavailabilitylcs)?.MssResponseShort?.Select(x => x.A0RID?.ToUpper() ?? "").Distinct().ToList() ?? new List<string>());

                //TODO SORT ORDER???

                return await GetFiltered(
                    fields: fields ?? Array.Empty<string>(), language: language, pagenumber: pagenumber,
                    pagesize: pagesize, idfilter: idfilter, idlist: availableonlineaccos, locfilter: locfilter, categoryfilter: categoryfilter,
                    typefilter: typefilter, boardfilter: boardfilter, featurefilter: featurefilter, featureidfilter: featureidfilter, themefilter: themefilter, badgefilter: badgefilter,
                    altitudefilter: altitudefilter, active: active, smgactive: odhactive, bookablefilter: bookablefilter, smgtagfilter: odhtagfilter, publishedon: publishedon,
                    seed: seed, updatefrom: updatefrom, langfilter: langfilter, searchfilter: searchfilter, geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }            
            else
            {
                return BadRequest("not supported!");
            }
        }

        /// <summary>
        /// GET Accommodation Single
        /// </summary>
        /// <param name="id">ID of the Accommodation</param>
        /// <param name="idsource">ID Source Filter (possible values:'lts','hgv'), (default:'lts')</param>        
        /// <param name="boardfilter">Boardfilter BITMASK values: 0 = (all boards), 1 = (without board), 2 = (breakfast), 4 = (half board), 8 = (full board), 16 = (All inclusive), 'null' = (No Filter), (default:'null')</param>
        /// <param name="availabilitycheck">Availability Check enabled/disabled (possible Values: 'true', 'false), (default Value: 'false') NOT AVAILABLE AS OPEN DATA</param>
        /// <param name="arrival">Arrival Date (yyyy-MM-dd) REQUIRED, (default:'Today')</param>
        /// <param name="departure">Departure Date (yyyy-MM-dd) REQUIRED, (default:'Tomorrow')</param>
        /// <param name="roominfo">Roominfo Filter REQUIRED (Splitter for Rooms '|' Splitter for Persons Ages ',') (Room Types: 0=notprovided, 1=room, 2=apartment, 4=pitch/tent(onlyLTS), 8=dorm(onlyLTS)) possible Values Example 1-18,10|1-18 = 2 Rooms, Room 1 for 2 person Age 18 and Age 10, Room 2 for 1 Person Age 18), (default:'1-18,18')</param>/// <param name="source">Source for MSS availability check, (default:'sinfo')</param>
        /// <param name="bokfilter">Booking Channels Filter REQUIRED (Separator ',' possible values: hgv = (Booking Südtirol), htl = (Hotel.de), exp = (Expedia), bok = (Booking.com), lts = (LTS Availability check)), (default:'hgv')</param>
        /// <param name="availabilitychecklanguage">Language of the Availability Response (possible values: 'de','it','en')</param>
        /// <param name="detail">Detail of the Availablity check (string, 1 = full Details, 0 = basic Details (default))</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Accommodation Object</returns>
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(AccommodationLinked), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        [HttpGet, Route("Accommodation/{id}", Name = "SingleAccommodation")]
        [TypeFilter(typeof(Filters.AvailabilitySearchInterceptorAttribute))]
        public async Task<IActionResult> GetAccommodation(
            string id,
            string? idsource = "lts",
            string? availabilitychecklanguage = "en",
            string? boardfilter = null,
            string? arrival = null,
            string? departure = null,
            string? roominfo = "1-18,18",
            string? bokfilter = "hgv",
            string? source = "sinfo",
            LegacyBool availabilitycheck = null!,
            string? detail = "0",
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            //if availabilitysearch requested and User not logged
            if (availabilitycheck?.Value == true && !CheckAvailabilitySearch(User))
            {
                return Unauthorized("User not allowed for availabilitysearch");
            }

            if (idsource == "hgv")
                return await GetSingleByHgvId(id, language, fields: fields ?? Array.Empty<string>(), removenullvalues, cancellationToken);
            else
                return await GetSingle(id, language, fields: fields ?? Array.Empty<string>(), removenullvalues, cancellationToken);           
        }

        //ACCO TYPES

        /// <summary>
        /// GET Accommodation Types List
        /// </summary>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="language">Language field selector, displays data and fields in the selected language, possible values: 'de|it|en|nl|cs|pl|fr|ru' only one language supported (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter' target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href='https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter' target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href='https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter' target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of AccommodationType Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<AccoTypes>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        
        [HttpGet, Route("AccommodationTypes")]
        public async Task<IActionResult> GetAllAccommodationTypesList(
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetAccoTypeList(language, fields: fields ?? Array.Empty<string>(), searchfilter, rawfilter, rawsort, removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Accommodation Types Single
        /// </summary>
        /// <param name="id">ID of the AccommodationType</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="language">Language field selector, displays data and fields in the selected language, possible values: 'de|it|en|nl|cs|pl|fr|ru' only one language supported (default:'null' all languages are displayed)</param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>AccommodationType Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(AccoTypes), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,AccoReader")]
        [HttpGet, Route("AccommodationTypes/{id}", Name = "SingleAccommodationTypes")]
        public async Task<IActionResult> GetAllAccommodationTypessingle(
            string id,
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetAccoTypeSingle(id, language, fields: fields ?? Array.Empty<string>(), removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Accommodation Feature List (LTS Features)
        /// </summary>
        /// <param name="ltst0idfilter">Filtering by LTS T0ID, filter behaviour is "startswith" so it is possible to send only one character, (default: blank)</param>
        /// <param name="source">IF source = "lts" the Features list is returned in XML Format directly from LTS, (default: blank)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="language">Language field selector, displays data and fields in the selected language, possible values: 'de|it|en|nl|cs|pl|fr|ru' only one language supported (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter' target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href='https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter' target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href='https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter' target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of AccoFeatures Object / XML LTS</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<AccoFeature>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "DataReader,AccoReader")]
        [HttpGet, Route("AccommodationFeatures")]
        public async Task<IActionResult> GetAllAccommodationFeaturesList(
            string? language,
            string? ltst0idfilter = null,
            string? source = null,            
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            if (!String.IsNullOrEmpty(source) && source == "lts")                
                return GetFeatureListXML(cancellationToken); 
            else
                return await GetAccoFeatureList(language, ltst0idfilter, fields: fields ?? Array.Empty<string>(), searchfilter, rawfilter, rawsort, removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Accommodation Feature Single (LTS Features)
        /// </summary>
        /// <param name="id">ID of the AccommodationFeature</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="language">Language field selector, displays data and fields in the selected language, possible values: 'de|it|en|nl|cs|pl|fr|ru' only one language supported (default:'null' all languages are displayed)</param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>AccoFeatures Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(AccoFeature), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,AccoReader")]
        [HttpGet, Route("AccommodationFeatures/{id}", Name = "SingleAccommodationFeatures")]
        public async Task<IActionResult> GetAllAccommodationFeaturesSingle(
            string id,
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetAccoFeatureSingle(id, language, fields: fields ?? Array.Empty<string>(), removenullvalues, cancellationToken);
        }

        // ACCO ROOMS

        /// <summary>
        /// GET Accommodation Room Info by Accommodation
        /// </summary>
        /// <param name="accoid">Accommodation ID</param>
        /// <param name="idsource">ID Source Filter (possible values:'lts','hgv'), (default:'lts')</param>        
        /// <param name="getall">Get Rooms from all sources (If an accommodation is bookable on Booking Southtyrol, rooms from this source are returned, setting getall to true returns also LTS Rooms), (default:false)</param>        
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter' target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href='https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter' target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href='https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter' target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of AccoRoom Objects</returns>
        [ProducesResponseType(typeof(IEnumerable<AccommodationRoomLinked>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,AccoReader")]
        [HttpGet, Route("AccommodationRoom", Name = "AccommodationRoomList")]
        public async Task<IActionResult> GetAccoRoomInfos(
            string accoid,
            string? idsource = "lts",
            bool getall = false,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null,
            string? updatefrom = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default
            )
        {
            string idtocheck = accoid;

            if (idsource == "hgv")
            {
                idtocheck = await GetAccoIdByHgvId(accoid, cancellationToken);
            }    

            return await GetAccommodationRooms(idtocheck, fields: fields ?? Array.Empty<string>(), language, getall, updatefrom, langfilter, searchfilter, rawfilter, rawsort, removenullvalues, cancellationToken);
        }

        // ACCO ROOMS

        /// <summary>
        /// GET Accommodation Room Info Single
        /// </summary>
        /// <param name="id">AccommodationRoom ID</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>AccommodationRoom Object</returns>
        [ProducesResponseType(typeof(AccommodationRoomLinked), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,AccoReader")]        
        [HttpGet, Route("AccommodationRoom/{id}", Name = "SingleAccommodationRoom")]
        public async Task<IActionResult> GetAccoRoomInfosById(
            string id,
            string? language = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {            
            return await GetSingleAccommodationRoom(id, language, fields: fields ?? Array.Empty<string>(), removenullvalues, cancellationToken);
        }

        //SPECIAL GETTER

        /// <summary>
        /// POST Pass Accommodation Ids and get Accommodations with Availability Information / Availability Information Only
        /// </summary>
        /// <param name="availabilitychecklanguage">Language of the Availability Response</param>
        /// <param name="boardfilter">Boardfilter (BITMASK values: 0 = (all boards), 1 = (without board), 2 = (breakfast), 4 = (half board), 8 = (full board), 16 = (All inclusive), 'null' = No Filter)</param>
        /// <param name="arrival">Arrival Date (yyyy-MM-dd) REQUIRED</param>
        /// <param name="departure">Departure Date (yyyy-MM-dd) REQUIRED</param>
        /// <param name="roominfo">Roominfo Filter REQUIRED (Splitter for Rooms '|' Splitter for Persons Ages ',') (Room Types: 0=notprovided, 1=room, 2=apartment, 4=pitch/tent(onlyLTS), 8=dorm(onlyLTS)) possible Values Example 1-18,10|1-18 = 2 Rooms, Room 1 for 2 person Age 18 and Age 10, Room 2 for 1 Person Age 18), (default:'1-18,18')</param>/// <param name="bokfilter">Booking Channels Filter (Separator ',' possible values: hgv = (Booking Südtirol), htl = (Hotel.de), exp = (Expedia), bok = (Booking.com), lts = (LTS Availability check), (default:hgv)) REQUIRED</param>              
        /// <param name="detail">Include Offer Details (String, 1 = full Details)</param>
        /// <param name="source">Source of the Requester (possible value: 'sinfo' = Suedtirol.info, 'sbalance' = Südtirol Balance) REQUIRED</param>        
        /// <param name="withoutids">Search over all bookable Accommodations (No Ids have to be provided as Post Data, when set to true, all passed Ids are omitted) (default: false)</param>        
        /// <param name="availabilityonly">Get only availability information without Accommodation information</param>
        /// <param name="idfilter">Posted Accommodation IDs (Separated by , must be specified in the POST Body as raw)</param>
        /// <returns>Result Object with Collection of Accommodation Objects</returns>        
        [ProducesResponseType(typeof(JsonResultWithBookingInfo<AccommodationLinked>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,AccoReader,PackageReader")]
        [TypeFilter(typeof(Filters.AvailabilitySearchInterceptorAttribute))]
        [HttpPost, Route("AccommodationAvailable")]
        //[HttpPost, Route("AvailabilityCheck")]
        public async Task<IActionResult> PostAvailableAccommodations(
            [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] string? idfilter = null,
            string? availabilitychecklanguage = "en",
            string? boardfilter = null,
            string? arrival = null,
            string? departure = null,
            string? roominfo = "1-18,18",
            string? bokfilter = "hgv",
            string? source = "sinfo",
            string? detail = "0",
            bool withoutids = false,            
            bool availabilityonly = false,
            CancellationToken cancellationToken = default)
        {
            //if availabilitysearch requested and User not logged
            if (!CheckAvailabilitySearch(User))
            {
                return Unauthorized("User not allowed for availabilitysearch");
            }

            //For Compatiblity if Route equals AvailabilityCheck return only Availability Response
            var usedroute = ControllerContext.ActionDescriptor.AttributeRouteInfo.Template;

            if (usedroute == "v1/AvailabilityCheck")
                availabilityonly = true;

            //If no ids in the post body, makes ure withoutids is checked (make use of cached MSS)
            if((idfilter == null || String.IsNullOrEmpty(idfilter)) && withoutids == false)
                return BadRequest("No Ids in the POST Body, Availability Search over all Accommodations only with withoutids set to true");

            var accobooklist = Request.HttpContext.Items["accobooklist"];
            var accoavailabilitymss = Request.HttpContext.Items["mssavailablity"];
            var accoavailabilitylcs = Request.HttpContext.Items["lcsavailablity"];

            var accosonmss = ((MssResult?)accoavailabilitymss)?.MssResponseShort?.Select(x => x.A0RID?.ToUpper() ?? "").Distinct().ToList() ?? new List<string>();
            var accosonlcs = ((MssResult?)accoavailabilitylcs)?.MssResponseShort?.Select(x => x.A0RID?.ToUpper() ?? "").Distinct().ToList() ?? new List<string>();

            var availableonlineaccos = new List<string>();
            if (accoavailabilitymss != null)
                availableonlineaccos.AddRange(accosonmss);
            if (accoavailabilitylcs != null)
                availableonlineaccos.AddRange(accosonlcs);

            var resultid = ((MssResult?)accoavailabilitymss)?.ResultId ?? "";

            //Counts
            var requestedtotal = accobooklist != null ? ((List<string>)accobooklist).Count : 0;

            var availableonline = accosonmss.Count;
            var availableonrequest = accosonlcs.Count;

            if (availabilityonly)
            {
                var toreturn = new List<MssResponseShort>();

                if (bokfilter.Contains("hgv") && accoavailabilitymss != null)
                    toreturn.AddRange(((MssResult?)accoavailabilitymss)?.MssResponseShort?.ToList());
                if (bokfilter.Contains("lts") && accoavailabilitylcs != null)
                    toreturn.AddRange(((MssResult?)accoavailabilitylcs)?.MssResponseShort?.ToList());

                //return immediately the mss response
                var result = ResponseHelpers.GetResult(
                   1,
                   1,
                   (UInt32)requestedtotal,
                   requestedtotal,
                   availableonline,
                   availableonrequest,
                   resultid,
                   "",
                   toreturn,
                   Url);

                return (Ok(result));
            }
            else
            {
                return await GetFiltered(
                fields: Array.Empty<string>(), language: null, pagenumber: 1,
                pagesize: int.MaxValue, idfilter: idfilter, idlist: availableonlineaccos, locfilter: null, categoryfilter: null,
                typefilter: null, boardfilter: boardfilter, featurefilter: null, featureidfilter: null, themefilter: null, badgefilter: null,
                altitudefilter: null, active: null, smgactive: null, bookablefilter: null, smgtagfilter: null,
                publishedon: null, seed: null, updatefrom: null, langfilter: null, searchfilter: null, new PGGeoSearchResult() { geosearch = false, latitude = 0, longitude = 0, radius = 0 }, 
                rawfilter: null, rawsort: null, removenullvalues: false, cancellationToken);
            }                      
        }

        /// <summary>
        /// POST Pass Accommodation Ids and get Accommodations with Availability Information / Availability Information Only
        /// </summary>
        /// <param name="availabilitychecklanguage">Language of the Availability Response</param>
        /// <param name="boardfilter">Boardfilter (BITMASK values: 0 = (all boards), 1 = (without board), 2 = (breakfast), 4 = (half board), 8 = (full board), 16 = (All inclusive), 'null' = No Filter)</param>
        /// <param name="arrival">Arrival Date (yyyy-MM-dd) REQUIRED</param>
        /// <param name="departure">Departure Date (yyyy-MM-dd) REQUIRED</param>
        /// <param name="roominfo">Roominfo Filter REQUIRED (Splitter for Rooms '|' Splitter for Persons Ages ',') (Room Types: 0=notprovided, 1=room, 2=apartment, 4=pitch/tent(onlyLTS), 8=dorm(onlyLTS)) possible Values Example 1-18,10|1-18 = 2 Rooms, Room 1 for 2 person Age 18 and Age 10, Room 2 for 1 Person Age 18), (default:'1-18,18')</param>/// <param name="bokfilter">Booking Channels Filter (Separator ',' possible values: hgv = (Booking Südtirol), htl = (Hotel.de), exp = (Expedia), bok = (Booking.com), lts = (LTS Availability check), (default:hgv)) REQUIRED</param>              
        /// <param name="detail">Include Offer Details (String, 1 = full Details)</param>
        /// <param name="source">Source of the Requester (possible value: 'sinfo' = Suedtirol.info, 'sbalance' = Südtirol Balance) REQUIRED</param>        
        /// <param name="withoutids">Search over all bookable Accommodations (No Ids have to be provided as Post Data, when set to true, all passed Ids are omitted) (default: false)</param>        
        /// <param name="idfilter">Posted Accommodation IDs (Separated by , must be specified in the POST Body as raw)</param>        
        /// <returns>Result Object with Collection of Accommodation Objects</returns>        
        [ProducesResponseType(typeof(JsonResultWithBookingInfo<MssResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,AccoReader,PackageReader")]
        [TypeFilter(typeof(Filters.AvailabilitySearchInterceptorAttribute))]        
        [HttpPost, Route("AvailabilityCheck")]
        public async Task<IActionResult> PostAvailableAccommodationsOnlyMssResult(
            [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] string? idfilter = null,
            string? availabilitychecklanguage = "en",
            string? boardfilter = null,
            string? arrival = null,
            string? departure = null,
            string? roominfo = "1-18,18",
            string? bokfilter = "hgv",
            string? source = "sinfo",
            string? detail = "0",
            bool withoutids = false,            
            CancellationToken cancellationToken = default)
        {
            return await PostAvailableAccommodations(idfilter, availabilitychecklanguage, boardfilter, arrival, departure, roominfo, bokfilter, source, detail, withoutids, true, cancellationToken);
        }

        #endregion

        #region GETTER

        private Task<IActionResult> GetFiltered(string[] fields, string? language, uint pagenumber, int? pagesize, string? idfilter, List<string> idlist, string? locfilter,
            string? categoryfilter, string? typefilter, string? boardfilter, string? featurefilter, string? featureidfilter, string? themefilter, string? badgefilter, string? altitudefilter, 
            bool? active, bool? smgactive, bool? bookablefilter, string? smgtagfilter, string? publishedon,
            string? seed, string? updatefrom, string? langfilter, string? searchfilter, 
            PGGeoSearchResult geosearchresult, string? rawfilter, string? rawsort, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                AccommodationHelper myhelper = await AccommodationHelper.CreateAsync(
                    QueryFactory, idfilter: idfilter, locfilter: locfilter, boardfilter: boardfilter, categoryfilter: categoryfilter, typefilter: typefilter,
                    featurefilter: featurefilter, featureidfilter: featureidfilter, badgefilter: badgefilter, themefilter: themefilter, altitudefilter: altitudefilter, smgtags: smgtagfilter, activefilter: active, 
                    smgactivefilter: smgactive, bookablefilter: bookablefilter, lastchange: updatefrom, langfilter: langfilter, publishedonfilter: publishedon, cancellationToken);

                //Fix if idlist from availabilitysearch is added use this instead of idfilter
                if (idlist.Count > 0)
                    myhelper.idlist = idlist;

                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From("accommodations")
                        .AccommodationWhereExpression(
                            idlist: myhelper.idlist, accotypelist: myhelper.accotypelist,
                            categorylist: myhelper.categorylist, featurelist: myhelper.featurelist, featureidlist: myhelper.featureidlist,
                            badgelist: myhelper.badgelist, themelist: myhelper.themelist, 
                            boardlist: myhelper.boardlist, smgtaglist: myhelper.smgtaglist,
                            districtlist: myhelper.districtlist, municipalitylist: myhelper.municipalitylist, 
                            tourismvereinlist: myhelper.tourismvereinlist, regionlist: myhelper.regionlist, 
                            apartmentfilter: myhelper.apartment, bookable: myhelper.bookable, altitude: myhelper.altitude,
                            altitudemin: myhelper.altitudemin, altitudemax: myhelper.altitudemax,
                            activefilter: myhelper.active, smgactivefilter: myhelper.smgactive, publishedonlist: myhelper.publishedonlist,
                            searchfilter: searchfilter, language: language, lastchange: myhelper.lastchange, languagelist: myhelper.languagelist,
                            filterClosedData: FilterClosedData, reducedData: ReducedData)
                        .ApplyRawFilter(rawfilter)
                        .ApplyOrdering_GeneratedColumns(ref seed, geosearchresult, rawsort);

                // Get paginated data
                var data =
                    await query
                        .PaginateAsync<JsonRaw>(
                            page: (int)pagenumber,
                            perPage: pagesize ?? 25);

                var fieldsTohide = FieldsToHide;

                var dataTransformed =
                    data.List.Select(
                        raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide)
                    );

                uint totalpages = (uint)data.TotalPages;
                uint totalcount = (uint)data.Count;

                var availableonline = Request.HttpContext.Items["mssavailablity"] != null ? ((MssResult?)Request.HttpContext.Items["mssavailablity"]).MssResponseShort.Count : 0;
                var availableonrequest = Request.HttpContext.Items["lcsavailablity"] != null ? ((MssResult?)Request.HttpContext.Items["lcsavailablity"]).MssResponseShort.Count : 0;
                var accobooklist = Request.HttpContext.Items["accobooklist"];
                var accosrequested = accobooklist != null ? ((List<string>)accobooklist).Count : 0;
                var resultid = ((MssResult?)Request.HttpContext.Items["mssavailablity"])?.ResultId ?? "";

                if (availableonline > 0)
                {
                    return ResponseHelpers.GetResult(
                      pagenumber,
                      totalpages,
                      (UInt32)accosrequested,
                      accosrequested,
                      availableonline,
                      availableonrequest,
                      resultid,
                      seed,
                      dataTransformed,
                      Url);
                }
                else
                {
                    return ResponseHelpers.GetResult(
                       pagenumber,
                       totalpages,
                       totalcount,
                       seed,
                       dataTransformed,
                       Url);
                }
            });
        }

        private Task<IActionResult> GetSingle(string id, string? language, string[] fields, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("accommodations")
                        .Select("data")
                        .Where("id", id.ToUpper())
                        //.When(FilterClosedData, q => q.FilterClosedData());
                        .Anonymous_Logged_UserRule_GeneratedColumn(FilterClosedData, !ReducedData);

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();
                var fieldsTohide = FieldsToHide;

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide);
            });
        }

        private Task<IActionResult> GetSingleByHgvId(string id, string? language, string[] fields, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("accommodations")
                        .Select("data")
                        .Where("gen_hgvid", "ILIKE", id)
                        //.When(FilterClosedData, q => q.FilterClosedData());
                        .Anonymous_Logged_UserRule_GeneratedColumn(FilterClosedData, !ReducedData);

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();
                var fieldsTohide = FieldsToHide;

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide);
            });
        }

        private async Task<string> GetAccoIdByHgvId(string id, CancellationToken cancellationToken)
        {
            var query =
                QueryFactory.Query("accommodations")
                    .Select("id")
                    .Where("gen_hgvid", "ILIKE", id)
                    //.When(FilterClosedData, q => q.FilterClosedData());
                    .Anonymous_Logged_UserRule_GeneratedColumn(FilterClosedData, !ReducedData);

            var data = await query.FirstOrDefaultAsync<string?>();

            return data ?? "";
        }

        private Task<IActionResult> GetAccommodationRooms(
            string id,
            string[] fields,
            string? language,
            bool all,
            string? updatefrom, string? langfilter,
            string? searchfilter,
            string? rawfilter,
            string? rawsort,
            bool removenullvalues,
            CancellationToken cancellationToken
            )
        {
            return DoAsyncReturn(async () =>
            {
                var languagelist = Helper.CommonListCreator.CreateIdList(langfilter);

                var query =
                    QueryFactory.Query("accommodationrooms")
                        .Select("data")
                        //.WhereRaw("data#>>'\\{A0RID\\}' ILIKE ?", id)
                        .Where("gen_a0rid", "ILIKE", id)
                        //.When(FilterClosedData, q => q.FilterClosedData())
                        .When(languagelist.Count > 0, q => q.HasLanguageFilterAnd_GeneratedColumn(languagelist))
                        .When(!String.IsNullOrEmpty(updatefrom), q => q.LastChangedFilter_GeneratedColumn(updatefrom))
                        .SearchFilter(PostgresSQLWhereBuilder.AccoRoomNameFieldsToSearchFor(language), searchfilter)
                        .ApplyRawFilter(rawfilter)
                        .OrderOnlyByRawSortIfNotNull(rawsort)
                        .Anonymous_Logged_UserRule_GeneratedColumn(FilterClosedData, !ReducedData);

                var data = await query.GetAsync<JsonRaw?>();
                var fieldsTohide = FieldsToHide;

                var dataTransformed =
                   data.Select(
                       raw => raw?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide)
                   );

                return dataTransformed;
            });
        }

        /// <summary>
        /// GET Single Accommodation Room
        /// </summary>
        /// <param name="id">ID of Accommodation Room</param>
        /// <returns>Poi Object</returns>
        private Task<IActionResult> GetSingleAccommodationRoom(
            string id,
            string? language,
            string[] fields,
            bool removenullvalues,
            CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("accommodationrooms")
                        .Select("data")
                        .Where("id", id.ToUpper())
                        //.When(FilterClosedData, q => q.FilterClosedData());
                        .Anonymous_Logged_UserRule_GeneratedColumn(FilterClosedData, !ReducedData);

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();
                var fieldsTohide = FieldsToHide;

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide);
            });
        }

        #endregion

        #region CUSTOM AVAILABILITY GETTER



        #endregion

        #region CATEGORIES

        private Task<IActionResult> GetAccoTypeList(string? language, string[] fields, string? searchfilter, string? rawfilter, string? rawsort, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("accommodationtypes")
                        .SelectRaw("data")
                        .When(FilterClosedData, q => q.FilterClosedData())
                        .SearchFilter(PostgresSQLWhereBuilder.TypeDescFieldsToSearchFor(language), searchfilter)
                        .ApplyRawFilter(rawfilter)
                        .OrderOnlyByRawSortIfNotNull(rawsort);

                var data = await query.GetAsync<JsonRaw?>();
                var fieldsTohide = FieldsToHide;

                var dataTransformed =
                    data.Select(
                        raw => raw?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide)
                    );

                return dataTransformed;
            });
        }

        private Task<IActionResult> GetAccoTypeSingle(string id, string? language, string[] fields, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("accommodationtypes")
                        .Select("data")
                         //.WhereJsonb("Key", "ilike", id)
                        .Where("id", id.ToLower())
                        .When(FilterClosedData, q => q.FilterClosedData());
                //.Where("Key", "ILIKE", id);

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();
                var fieldsTohide = FieldsToHide;

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide);
            });
        }

        private Task<IActionResult> GetAccoFeatureList(string? language, string? ltst0filter, string[] fields, string? searchfilter, string? rawfilter, string? rawsort, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("accommodationfeatures")
                        .SelectRaw("data")
                        .When(!String.IsNullOrEmpty(ltst0filter), q => q.WhereJsonb("CustomId", "like", ltst0filter + "%"))
                        .When(FilterClosedData, q => q.FilterClosedData())
                        .SearchFilter(PostgresSQLWhereBuilder.TypeDescFieldsToSearchFor(language), searchfilter)
                        .ApplyRawFilter(rawfilter)
                        .OrderOnlyByRawSortIfNotNull(rawsort);

                var data = await query.GetAsync<JsonRaw?>();
                var fieldsTohide = FieldsToHide;

                var dataTransformed =
                    data.Select(
                        raw => raw?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide)
                    );

                return dataTransformed;
            });
        }

        private Task<IActionResult> GetAccoFeatureSingle(string id, string? language, string[] fields, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("accommodationfeatures")
                        .Select("data")
                         //.WhereJsonb("Key", "ilike", id)
                         .Where("id", id.ToUpper())
                        .When(FilterClosedData, q => q.FilterClosedData());
                
                var data = await query.FirstOrDefaultAsync<JsonRaw?>();
                var fieldsTohide = FieldsToHide;

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide);
            });
        }

        private IActionResult GetFeatureListXML(CancellationToken cancellationToken)
        {
            XDocument mytins = GetAccommodationDataCDB.GetTinfromCDB("1", settings.CDBConfig.Username, settings.CDBConfig.Password, settings.CDBConfig.Url);

            //return new ContentResult { Content = mytins.ToString(), ContentType = "text/xml", StatusCode = 200 };

            return Ok(mytins);
        }

        #endregion

        #region POST PUT DELETE

        /// <summary>
        /// POST Insert new Accommodation
        /// </summary>
        /// <param name="accommodation">Accommodation Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,AccoManager,AccoCreate,AccommodationWriter,AccommodationManager,AccommodationCreate")]
        [HttpPost, Route("Accommodation")]
        public Task<IActionResult> Post([FromBody] AccommodationLinked accommodation)
        {
            return DoAsyncReturn(async () =>
            {
                accommodation.Id = !String.IsNullOrEmpty(accommodation.Id) ? accommodation.Id.ToUpper() : "NOID";
                return await UpsertData<AccommodationLinked>(accommodation, "accommodations");
            });
        }

        /// <summary>
        /// PUT Modify existing Accommodation
        /// </summary>
        /// <param name="id">Accommodation Id</param>
        /// <param name="accommodation">Accommodation Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataModify,AccoManager,AccoModify,AccommodationWriter,AccommodationManager,AccommodationModify")]
        [HttpPut, Route("Accommodation/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] AccommodationLinked accommodation)
        {
            return DoAsyncReturn(async () =>
            {
                accommodation.Id = id.ToUpper();
                return await UpsertData<AccommodationLinked>(accommodation, "accommodations");
            });
        }

        /// <summary>
        /// DELETE Accommodation by Id
        /// </summary>
        /// <param name="id">Accommodation Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataDelete,AccoManager,AccoDelete,AccommodationWriter,AccommodationManager,AccommodationDelete")]
        [HttpDelete, Route("Accommodation/{id}")]
        public Task<IActionResult> Delete(string id)
        {
            return DoAsyncReturn(async () =>
            {
                id = id.ToUpper();
                return await DeleteData(id, "accommodations");
            });
        }


        #endregion
    }
}