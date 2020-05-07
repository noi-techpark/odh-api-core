using Helper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OdhApiCore.Controllers.helper;
using OdhApiCore.Responses;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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

        public AccommodationController(IWebHostEnvironment env, ISettings settings, ILogger<ActivityController> logger, QueryFactory queryFactory, IHttpClientFactory httpClientFactory)
            : base(env, settings, logger, queryFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        #region SWAGGER Exposed API

        /// <summary>
        /// GET Accommodation List
        /// </summary>
        /// <param name="pagenumber">Pagenumber, (default:1)</param>
        /// <param name="pagesize">Elements per Page (If availabilitycheck set, pagesize has no effect all Accommodations are returned), (default:10)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="categoryfilter">Categoryfilter (BITMASK values: 1 = (not categorized), 2 = (1star), 4 = (1flower), 8 = (1sun), 14 = (1star/1flower/1sun), 16 = (2stars), 32 = (2flowers), 64 = (2suns), 112 = (2stars/2flowers/2suns), 128 = (3stars), 256 = (3flowers), 512 = (3suns), 1024 = (3sstars), 1920 = (3stars/3flowers/3suns/3sstars), 2048 = (4stars), 4096 = (4flowers), 8192 = (4suns), 16384 = (4sstars), 30720 = (4stars/4flowers/4suns/4sstars), 32768 = (5stars), 65536 = (5flowers), 131072 = (5suns), 229376 = (5stars/5flowers/5suns), 'null' = No Filter), (default:'null')</param>
        /// <param name="typefilter">Typefilter (BITMASK values: 1 = (HotelPension), 2 = (BedBreakfast), 4 = (Farm), 8 = (Camping), 16 = (Youth), 32 = (Mountain), 64 = (Apartment), 128 = (Not defined),'null' = No Filter), (default:'null')</param>
        /// <param name="boardfilter">Boardfilter (BITMASK values: 0 = (all boards), 1 = (without board), 2 = (breakfast), 4 = (half board), 8 = (full board), 16 = (All inclusive), 'null' = No Filter), (default:'0')</param>
        /// <param name="featurefilter">FeatureFilter (BITMASK values: 1 = (Group-friendly), 2 = (Meeting rooms), 4 = (Swimming pool), 8 = (Sauna), 16 = (Garage), 32 = (Pick-up service), 64 = (WLAN), 128 = (Barrier-free), 256 = (Special menus for allergy sufferers), 512 = (Pets welcome), 'null' = No Filter), (default:'null')</param>
        /// <param name="themefilter">Themefilter (BITMASK values: 1 = (Gourmet), 2 = (At altitude), 4 = (Regional wellness offerings), 8 = (on the wheels), 16 = (With family), 32 = (Hiking), 64 = (In the vineyards), 128 = (Urban vibe), 256 = (At the ski resort), 512 = (Mediterranean), 1024 = (In the Dolomites), 2048 = (Alpine), 4096 = (Small and charming), 8192 = (Huts and mountain inns), 16384 = (Rural way of life), 32768 = (Balance), 65536 = (Christmas markets), 'null' = No Filter), (default:'null')</param>
        /// <param name="badgefilter">BadgeFilter (BITMASK values: 1 = (Belvita Wellness Hotel), 2 = (Familyhotel), 4 = (Bikehotel), 8 = (Red Rooster Farm), 16 = (Barrier free certificated), 32 = (Vitalpina Hiking Hotel), 64 = (Private Rooms in South Tyrol), 128 = (Vinum Hotels), 'null' = No Filter), (default:'null')</param>        
        /// <param name="idfilter">IDFilter (Separator ',' List of Accommodation IDs, 'null' = No Filter), (default:'null')</param>
        /// <param name="locfilter">Locfilter (Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction), 'null' = No Filter), (default:'null')</param>        
        /// <param name="odhtagfilter">ODHTag Filter (refers to Array SmgTags) (String, Separator ',' more ODHTags possible, 'null' = No Filter, available ODHTags reference to 'api/ODHTag?validforentity=accommodation'), (default:'null')</param>
        /// <param name="odhactive">ODHActive Filter (refers to field SmgActive) (possible Values: 'null' Displays all Accommodations, 'true' only ODH Active Accommodations, 'false' only ODH Disabled Accommodations, (default:'null')</param>       
        /// <param name="active">TIC Active Filter (possible Values: 'null' Displays all Accommodations, 'true' only TIC Active Accommodations, 'false' only TIC Disabled Accommodations, (default:'null')</param>       
        /// <param name="availabilitycheck">Availability Check enabled/disabled (possible Values: 'true', 'false), (default Value: 'false') NOT AVAILABLE AS OPEN DATA</param>
        /// <param name="arrival">Arrival Date (yyyy-MM-dd) REQUIRED, (default:'Today')</param>
        /// <param name="departure">Departure Date (yyyy-MM-dd) REQUIRED, (default:'Tomorrow')</param>
        /// <param name="roominfo">Roominfo Filter REQUIRED (Splitter for Rooms '|' Splitter for Persons Ages ',') (Room Types: 0=notprovided, 1=room, 2=apartment, 4=pitch/tent(onlyLTS), 8=dorm(onlyLTS)) possible Values Example 1-18,10|1-18 = 2 Rooms, Room 1 for 2 person Age 18 and Age 10, Room 2 for 1 Person Age 18), (default:'1-18,18')</param>/// <param name="bokfilter">Booking Channels Filter REQUIRED (Separator ',' possible values: hgv = (Booking Südtirol), htl = (Hotel.de), exp = (Expedia), bok = (Booking.com), lts = (LTS Availability check)), (default:'hgv')</param>
        /// <param name="source">Source for MSS availability check, (default:'sinfo')</param>
        /// <param name="availabilitychecklanguage">Language of the Availability Response (possible values: 'de','it','en'), (default:'en')</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="updatefrom">Date from Format (yyyy-MM-dd) (all GBActivityPoi with LastChange >= datefrom are passed), (default: null)</param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null)</param>
        /// <returns>Collection of Accommodation Objects</returns>
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<Accommodation>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Accommodation")]
        public async Task<IActionResult> GetAccommodations(
            uint pagenumber = 1,
            uint pagesize = 10,
            string? seed = null,
            string? categoryfilter = null,
            string? typefilter = null,
            string? boardfilter = null,
            string? featurefilter = null,
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
            LegacyBool availabilitycheck = null!,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,            
            string? language = null,
            string? updatefrom = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            CancellationToken cancellationToken = default)
        {
            
            //bool availabilitysearchallowed = CheckAvailabilitySearch(User);
            
            //Contains 6 Methods GETPAGED, GETFILTERED, GETAVAILABLE, GETAVAILABLELCS, GETAVAILABLEMSSANDLCS

            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            List<string> bokfilterlist = bokfilter.Split(',').ToList();

            if (availabilitycheck?.Value != true)
            {
                return await GetFiltered(
                    fields: fields ?? Array.Empty<string>(), language: language, pagenumber: pagenumber,
                    pagesize: pagesize, idfilter: idfilter, locfilter: locfilter, categoryfilter: categoryfilter,
                    typefilter: typefilter, boardfilter: boardfilter, featurefilter: featurefilter, themefilter: themefilter, badgefilter: badgefilter,
                    altitudefilter: altitudefilter, active: active, smgactive: odhactive, bookablefilter: bookablefilter, smgtagfilter: odhtagfilter,
                    seed: seed, updatefrom: updatefrom, searchfilter: searchfilter, geosearchresult, cancellationToken);
            }

            //Fall 3 Available MSS
            //else if (availabilitycheck?.Value == true && (bokfilterlist.Contains("hgv") || bokfilterlist.Contains("htl") || bokfilterlist.Contains("exp") || bokfilterlist.Contains("bok")) && !bokfilterlist.Contains("lts"))
            //{
            //    if (availabilitysearchallowed)
            //    {
            //        if (String.IsNullOrEmpty(arrival))
            //            arrival = String.Format("{0:yyyy-MM-dd}", DateTime.Now);
            //        if (String.IsNullOrEmpty(departure))
            //            departure = String.Format("{0:yyyy-MM-dd}", DateTime.Now.AddDays(1));
            //        if (boardfilter == null)
            //            boardfilter = "0";

            //        throw new NotImplementedException(); //return await GetAvailableAsync(pagenumber, pagesize, availabilitychecklanguage, categoryfilter, typefilter, boardfilter, featurefilter, themefilter, badgefilter, idfilter, locfilter, active?.Value, odhactive?.Value, odhtagfilter, arrival, departure, roominfo, bokfilter, seed, geosearchresult, source, fieldselector, language, updatefrom, searchfilter);
            //    }
            //    else
            //        return BadRequest("AvailabilitySearch only available for logged Users, not as open data!");
            //}

            ////Fall 4 Available LCS
            //else if (availabilitycheck?.Value == true && !bokfilterlist.Contains("hgv") && !bokfilterlist.Contains("htl") && !bokfilterlist.Contains("exp") && !bokfilterlist.Contains("bok") && bokfilterlist.Contains("lts"))
            //{
            //    if (availabilitysearchallowed)
            //    {
            //        if (String.IsNullOrEmpty(arrival))
            //            arrival = String.Format("{0:yyyy-MM-dd}", DateTime.Now);
            //        if (String.IsNullOrEmpty(departure))
            //            departure = String.Format("{0:yyyy-MM-dd}", DateTime.Now.AddDays(1));
            //        if (boardfilter == null)
            //            boardfilter = "0";

            //        throw new NotImplementedException(); // return await GetAvailableLCSAsync(pagenumber, pagesize, availabilitychecklanguage, categoryfilter, typefilter, boardfilter, featurefilter, themefilter, badgefilter, idfilter, locfilter, active?.Value, odhactive?.Value, odhtagfilter, arrival, departure, roominfo, seed, geosearchresult, fieldselector, language, updatefrom, searchfilter);
            //    }
            //    else
            //        return BadRequest("AvailabilitySearch only available for logged Users, not as open data!");
            //}

            ////Fall 5 Available MSS and LCS
            //else if (availabilitycheck?.Value == true && (bokfilterlist.Contains("hgv") || bokfilterlist.Contains("htl") || bokfilterlist.Contains("exp") || bokfilterlist.Contains("bok")) && bokfilterlist.Contains("lts"))
            //{
            //    if (availabilitysearchallowed)
            //    {
            //        if (String.IsNullOrEmpty(arrival))
            //            arrival = String.Format("{0:yyyy-MM-dd}", DateTime.Now);
            //        if (String.IsNullOrEmpty(departure))
            //            departure = String.Format("{0:yyyy-MM-dd}", DateTime.Now.AddDays(1));
            //        if (boardfilter == null)
            //            boardfilter = "0";

            //        throw new NotImplementedException(); // return await GetAvailableMSSLCSAsync(pagenumber, pagesize, availabilitychecklanguage, categoryfilter, typefilter, boardfilter, featurefilter, themefilter, badgefilter, idfilter, locfilter, active?.Value, odhactive?.Value, odhtagfilter, arrival, departure, roominfo, bokfilter, seed, geosearchresult, source, fieldselector, language, updatefrom, searchfilter);
            //    }
            //    else
            //        return BadRequest("AvailabilitySearch only available for logged Users, not as open data!");
            //}
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
        /// <param name="boardfilter">Boardfilter (BITMASK values: 0 = (all boards), 1 = (without board), 2 = (breakfast), 4 = (half board), 8 = (full board), 16 = (All inclusive), 'null' = No Filter), (default:'0')</param>
        /// <param name="availabilitycheck">Availability Check enabled/disabled (possible Values: 'true', 'false), (default Value: 'false') NOT AVAILABLE AS OPEN DATA</param>
        /// <param name="arrival">Arrival Date (yyyy-MM-dd) REQUIRED, (default:'Today')</param>
        /// <param name="departure">Departure Date (yyyy-MM-dd) REQUIRED, (default:'Tomorrow')</param>
        /// <param name="roominfo">Roominfo Filter REQUIRED (Splitter for Rooms '|' Splitter for Persons Ages ',') (Room Types: 0=notprovided, 1=room, 2=apartment, 4=pitch/tent(onlyLTS), 8=dorm(onlyLTS)) possible Values Example 1-18,10|1-18 = 2 Rooms, Room 1 for 2 person Age 18 and Age 10, Room 2 for 1 Person Age 18), (default:'1-18,18')</param>/// <param name="source">Source for MSS availability check, (default:'sinfo')</param>
        /// <param name="bokfilter">Booking Channels Filter REQUIRED (Separator ',' possible values: hgv = (Booking Südtirol), htl = (Hotel.de), exp = (Expedia), bok = (Booking.com), lts = (LTS Availability check)), (default:'hgv')</param>
        /// <param name="availabilitychecklanguage">Language of the Availability Response (possible values: 'de','it','en'), (default:'de')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <returns>Accommodation Object</returns>
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(Accommodation), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        [HttpGet, Route("Accommodation/{id}", Name = "SingleAccommodation")]
        [TypeFilter(typeof(Filters.MssInterceptorAttribute))]
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
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            CancellationToken cancellationToken = default)
        {
            //string table = CheckOpenData(User);
            //bool availabilitysearchallowed = CheckAvailabilitySearch(User);

            //Contains 4 Methods GETSINGLE, GETBYHGVID, GETAVAILABLESINGLE, GETAVAILABLESINGLELCS

            //List<string> bokfilterlist = bokfilter != null ? bokfilter.Split(',').ToList() : new List<string>();

            //var result = await GetMSSAvailability(
            //    language: language ?? "de", arrival: arrival, departure: departure, boardfilter: boardfilter ?? "0",
            //    roominfo: roominfo, bokfilter: bokfilter, detail: null, bookableaccoIDs: new List<string>(), source = source ?? "sinfo");

            //Fall 1 GET Single
            //if (availabilitycheck?.Value != true && idsource == "lts")
            if (true)
            {
                return await GetSingle(id, language, fields: fields ?? Array.Empty<string>(), cancellationToken);
            }

            //Fall 2 GET by HGVId
            //else if (availabilitycheck?.Value != true && idsource == "hgv")
            //{
            //    return GetbyHgvId(id, fieldselector, table, language);
            //}

            ////Fall 3 GET MSS
            //else if (availabilitycheck?.Value == true && (bokfilterlist.Contains("hgv") || bokfilterlist.Contains("htl") || bokfilterlist.Contains("exp") || bokfilterlist.Contains("bok")) && !bokfilterlist.Contains("lts"))
            //{
            //    if (availabilitysearchallowed)
            //    {
            //        string idtocheck = id;

            //        if (idsource == "hgv")
            //            idtocheck = GetAccoIdByHGVId(id);

            //        if (String.IsNullOrEmpty(arrival))
            //            arrival = String.Format("{0:yyyy-MM-dd}", DateTime.Now);
            //        if (String.IsNullOrEmpty(departure))
            //            departure = String.Format("{0:yyyy-MM-dd}", DateTime.Now.AddDays(1));
            //        if (boardfilter == null)
            //            boardfilter = "0";

            //        return await GetAvailableSingleAsync(availabilitychecklanguage, boardfilter, arrival, departure, roominfo, idtocheck, bokfilter, source, fieldselector, language);
            //    }
            //    else
            //        return BadRequest("AvailabilitySearch only available for logged Users, not as open data!");
            //}

            ////Fall 4 GET LCS
            //else if (availabilitycheck?.Value == true && !bokfilterlist.Contains("hgv") && !bokfilterlist.Contains("htl") && !bokfilterlist.Contains("exp") && !bokfilterlist.Contains("bok") && bokfilterlist.Contains("lts"))
            //{
            //    if (availabilitysearchallowed)
            //    {
            //        string idtocheck = id;

            //        if (idsource == "hgv")
            //            idtocheck = GetAccoIdByHGVId(id);

            //        if (String.IsNullOrEmpty(arrival))
            //            arrival = String.Format("{0:yyyy-MM-dd}", DateTime.Now);
            //        if (String.IsNullOrEmpty(departure))
            //            departure = String.Format("{0:yyyy-MM-dd}", DateTime.Now.AddDays(1));
            //        if (boardfilter == null)
            //            boardfilter = "0";

            //        return await GetAvailableSingleLCSAsync(availabilitychecklanguage, boardfilter, arrival, departure, roominfo, idtocheck, fieldselector, language);
            //    }
            //    else
            //        return BadRequest("AvailabilitySearch only available for logged Users, not as open data!");
            //}

            else
            {
                return BadRequest("not supported");
            }
        }


        //ACCO TYPES

        /// <summary>
        /// GET Accommodation Types List
        /// </summary>
        /// <returns>Collection of AccommodationType Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<ArticleTypes>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,AccoReader")]
        [HttpGet, Route("AccommodationTypes")]
        public async Task<IActionResult> GetAllAccommodationTypesList(CancellationToken cancellationToken)
        {
            return await GetAccoTypeList(cancellationToken);
        }

        /// <summary>
        /// GET Accommodation Types Single
        /// </summary>
        /// <param name="id">ID of the AccommodationType</param>
        /// <returns>AccommodationType Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ArticleTypes), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,AccoReader")]
        [HttpGet, Route("AccommodationTypes/{id}", Name = "SingleAccommodationTypes")]
        public async Task<IActionResult> GetAllAccommodationTypessingle(string id, CancellationToken cancellationToken)
        {
            return await GetAccoTypeSingle(id, cancellationToken);
        }

        /// <summary>
        /// GET Accommodation Feature List (LTS Features)
        /// </summary>
        /// <param name="source">IF source = "lts" the Features list is returned in XML Format directly from LTS, (default: blank)</param>
        /// <returns>Collection of AccoFeatures Object / XML LTS</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<AccoFeature>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,AccoReader")]
        [HttpGet, Route("AccommodationFeatures")]
        public async Task<IActionResult> GetAllAccommodationFeaturesList(string? source = null, CancellationToken cancellationToken = default)
        {
            if (!String.IsNullOrEmpty(source) && source == "lts")
                return await Task.FromResult<IActionResult>(Ok());
            //return GetFeatureList(cancellationToken); TODO
            else
                return await GetAccoFeatureList(cancellationToken);
        }

        /// <summary>
        /// GET Accommodation Feature Single (LTS Features)
        /// </summary>
        /// <param name="id">ID of the AccommodationFeature</param>
        /// <returns>AccoFeatures Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(AccoFeature), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,AccoReader")]
        [HttpGet, Route("AccommodationFeatures/{id}", Name = "SingleAccommodationFeatures")]
        public async Task<IActionResult> GetAllAccommodationFeaturesSingle(string id, CancellationToken cancellationToken)
        {
            return await GetAccoFeatureSingle(id, cancellationToken);
        }


        #endregion

        #region GETTER

        private Task<IActionResult> GetFiltered(string[] fields, string? language, uint pagenumber, uint pagesize, string? idfilter, string? locfilter,
            string? categoryfilter, string? typefilter, string? boardfilter, string? featurefilter, string? themefilter, string? badgefilter, string? altitudefilter, 
            bool? active, bool? smgactive, bool? bookablefilter, string? smgtagfilter, string? seed, string? updatefrom, string? searchfilter,
            PGGeoSearchResult geosearchresult, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                AccommodationHelper myhelper = await AccommodationHelper.CreateAsync(
                    QueryFactory, idfilter: idfilter, locfilter: locfilter, boardfilter: boardfilter, categoryfilter: categoryfilter, typefilter: typefilter,
                    featurefilter: featurefilter, badgefilter: badgefilter, themefilter: themefilter, altitudefilter: altitudefilter, smgtags: smgtagfilter, activefilter: active, 
                    smgactivefilter: smgactive, bookablefilter: bookablefilter, lastchange: updatefrom, cancellationToken);

                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From("accommodations")
                        .AccommodationWhereExpression(
                            idlist: myhelper.idlist, accotypelist: myhelper.accotypelist,
                            categorylist: myhelper.categorylist, featurelist: myhelper.featurelist,
                            badgelist: myhelper.badgelist, themelist: myhelper.themelist, 
                            boardlist: myhelper.boardlist, smgtaglist: myhelper.smgtaglist,
                            districtlist: myhelper.districtlist, municipalitylist: myhelper.municipalitylist, 
                            tourismvereinlist: myhelper.tourismvereinlist, regionlist: myhelper.regionlist, 
                            apartmentfilter: myhelper.apartment, bookable: myhelper.bookable, altitude: myhelper.altitude,
                            altitudemin: myhelper.altitudemin, altitudemax: myhelper.altitudemax,
                            activefilter: myhelper.active, smgactivefilter: myhelper.smgactive,
                            searchfilter: searchfilter, language: language, lastchange: myhelper.lastchange, languagelist: new List<string>(),
                            filterClosedData: FilterClosedData)
                        .OrderBySeed(ref seed, "data #>>'\\{Shortname\\}' ASC")
                        .GeoSearchFilterAndOrderby(geosearchresult);

                // Get paginated data
                var data =
                    await query
                        .PaginateAsync<JsonRaw>(
                            page: (int)pagenumber,
                            perPage: (int)pagesize);

                var dataTransformed =
                    data.List.Select(
                        raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, urlGenerator: UrlGenerator)
                    );

                uint totalpages = (uint)data.TotalPages;
                uint totalcount = (uint)data.Count;

                return ResponseHelpers.GetResult(
                    pagenumber,
                    totalpages,
                    totalcount,
                    seed,
                    dataTransformed,
                    Url);
            });
        }

        private Task<IActionResult> GetSingle(string id, string? language, string[] fields, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("accommodations")
                        .Select("data")
                        .Where("id", id.ToUpper())
                        .When(FilterClosedData, q => q.FilterClosedData());

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, urlGenerator: UrlGenerator);
            });
        }


        #endregion

        #region SPECIALMETHODS



        #endregion

        #region CUSTOM METHODS

        private Task<IActionResult> GetAccoTypeList(CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("accommodationtypes")
                        .SelectRaw("data")
                        .When(FilterClosedData, q => q.FilterClosedData());

                var data = await query.GetAsync<JsonRaw?>();

                return data;
            });
        }

        private Task<IActionResult> GetAccoTypeSingle(string id, CancellationToken cancellationToken)
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

                return data;
            });
        }

        private Task<IActionResult> GetAccoFeatureList(CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("accommodationfeatures")
                        .SelectRaw("data")
                        .When(FilterClosedData, q => q.FilterClosedData());

                var data = await query.GetAsync<JsonRaw?>();

                return data;
            });
        }

        private Task<IActionResult> GetAccoFeatureSingle(string id, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("accommodationfeatures")
                        .Select("data")
                         //.WhereJsonb("Key", "ilike", id)
                         .Where("id", id.ToUpper())
                        .When(FilterClosedData, q => q.FilterClosedData());
                //.Where("Key", "ILIKE", id);

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data;
            });
        }


        #endregion

        #region PRIVATEHELPERS

        private async Task<MssResult> GetMSSAvailability(string language, string arrival, string departure, string boardfilter, string roominfo, string bokfilter, string detail, List<string> bookableaccoIDs, string idsofchannel, string source, bool withoutmssids = false, string mssversion = "2")
        {
            int? offerdetail = null;
            int hoteldetail = 524288;

            if (detail == "1")
            {
                offerdetail = 33081;
                hoteldetail = 524800;
            }

            MssHelper myhelper = MssHelper.Create(bookableaccoIDs, idsofchannel, bokfilter, language, roominfo, boardfilter, arrival, departure, hoteldetail, offerdetail, source, mssversion);
                       
            //Achtung muassi no schaugn!
            if (bookableaccoIDs.Count > 0)
            {
                //0 MSS Method Olle channels affamol mit IDList
                var myparsedresponse = await GetMssData.GetMssResponse(
                    httpClientFactory.CreateClient("mss"),
                    lang: myhelper.mssrequestlanguage, idlist: myhelper.accoidlist, idsofchannel: idsofchannel, mybookingchannels: myhelper.mybokchannels,
                    myroomdata: myhelper.myroomdata, arrival: myhelper.arrival, departure: myhelper.departure, service: myhelper.service,
                    hgvservicecode: myhelper.hgvservicecode, offerdetails: myhelper.xoffertype, hoteldetails: myhelper.xhoteldetails,
                    rooms: myhelper.rooms, source: myhelper.source, version: myhelper.mssversion, mssuser: "", msspswd: "", withoutmssids: withoutmssids
                    );
               
                if (myparsedresponse != null)
                    return myparsedresponse;
            }
            return new MssResult() { bookableHotels = 0, CheapestChannel = "", Cheapestprice = 0, ResultId = "", MssResponseShort = new List<MssResponseShort>() };
        }

        //private async Task<MssResult> GetLCSAvailability(string language, string arrival, string departure, string boardfilter, string roominfo, List<string> bookableaccoIDs, string source)
        //{
        //    //-------------------------------------------------MSSREQUEST------------------------------------------------------------

        //    var service = Common.AccoListCreator.CreateBoardListLCSfromFlag(boardfilter);
        //    var myroomdata = GetAccommodationDataLCS.RoomstayTransformer(roominfo);

        //    //Achtung muassi no schaugn!
        //    if (bookableaccoIDs.Count > 0)
        //    {
        //        var accosearchrequest = GetAccommodationDataLCS.GetAccommodationDataSearchRequest("", "1", "10000", "de", "1", "", "", "0", "0", arrival, departure, "1", "0", service, bookableaccoIDs, new List<string>(), new List<string>(), new List<string>(), myroomdata, source, ltsmessagepswd);

        //        var myaccosearchlcs = new GetAccommodationDataLCS(ltsuser, ltspswd);
        //        var response = await myaccosearchlcs.GetAccommodationDataSearchAsync(accosearchrequest);
        //        var myparsedresponse = ParseAccoSearchResult.ParsemyLCSResponse(language, response, myroomdata.Count);

        //        return myparsedresponse;
        //    }
        //    else
        //        return new MssResult() { bookableHotels = 0, CheapestChannel = "", Cheapestprice = 0, ResultId = "", MssResponseShort = new List<MssResponseShort>() };
        //}


        private bool CheckArrivalAndDeparture(string arrival, string departure)
        {
            DateTime now = DateTime.Now;
            DateTime arrivaldt = DateTime.Parse(arrival);
            DateTime departuredt = DateTime.Parse(departure);

            if (arrivaldt.Date == departuredt.Date)
                return false;

            if (arrivaldt <= now.Date.AddDays(-1) || departuredt <= now.Date.AddDays(-1))
                return false;
            else
                return true;
        }

        #endregion
    }
}