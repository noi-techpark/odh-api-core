using AspNetCore.CacheOutput;
using DataModel;
using Helper;
using LCS;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
using OdhApiCore.Responses;
using SIAG;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    /// <summary>
    /// WebcamInfo Api (data provided by LTS ActivityData) SOME DATA Available as OPENDATA
    /// </summary>
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class WeatherController : OdhController
    {
        private readonly ISettings settings;

        public WeatherController(IWebHostEnvironment env, ISettings settings, ILogger<WeatherController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {
            this.settings = settings;
        }

        #region SwaggerExposed API

        /// <summary>
        /// GET Current Suedtirol Weather LIVE
        /// </summary>
        /// <param name="language">Language</param>
        /// <param name="locfilter">Locfilter (possible values: filter by StationData 1 = Schlanders, 2 = Meran, 3 = Bozen, 4 = Sterzing, 5 = Brixen, 6 = Bruneck | filter nearest Station to Region,TV,Municipality,Fraction reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction), '' = No Filter). IF a Locfilter is set, only Stationdata is provided.</param>
        /// <returns>Weather Object</returns>
        [ProducesResponseType(typeof(Weather), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Weather")]
        public async Task<ActionResult<Weather>> GetWeather(
            string? language = "en", 
            string? locfilter = null,
            bool extended = true,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await Get(language ?? "en", locfilter, extended, cancellationToken);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        /// <summary>
        /// GET District Weather LIVE
        /// </summary>
        /// <param name="language">Language</param>
        /// <param name="locfilter">Locfilter (possible values: filter by District 1 = Etschtal/Überetsch/Unterland, 2 = Burggrafenamt, 3 = Vinschgau, 4 = Eisacktal und Sarntal, 5 = Wipptal, 6 = Pustertal/Dolomiten, 7 = Ladinien-Dolomiten | filter nearest DistrictWeather to Region,TV,Municipality,Fraction tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction))</param>
        /// <returns>Bezirks Weather Object</returns>        
        [ProducesResponseType(typeof(BezirksWeather), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Weather/District")]
        public async Task<ActionResult<BezirksWeather>> GetDistrictWeather(
            string locfilter,
            string? language = "en",             
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await GetBezirksWetter(language ?? "en", locfilter, cancellationToken);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
        
        /// <summary>
        /// GET Current Realtime Weather LIVE
        /// </summary>
        /// <param name="language">Language</param>        
        /// <returns>WeatherRealTime Object</returns>
        [ProducesResponseType(typeof(IEnumerable<WeatherRealTime>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Weather/Realtime")]
        public async Task<ActionResult<IEnumerable<WeatherRealTime>>> GetRealtimeWeather(
            string? language = "en",
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await GetRealTimeWeather(language ?? "en", cancellationToken);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        /// <summary>
        /// GET Measuringpoint LIST
        /// </summary>
        /// <param name="elements">Elements to retrieve</param>
        /// <param name="locfilter">Locfilter (Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction), 'null' = No Filter), (default:'null')</param>        
        /// <param name="areafilter">Area ID (multiple IDs possible, separated by ",")</param>
        /// <param name="skiareafilter">Skiarea ID</param>
        /// <param name="active">Active Filter (possible Values: 'true' only Active Measuringpoints, 'false' only Disabled Measuringpoints, (default:'null')</param>
        /// <param name="odhactive">ODH Active Filter Measuringpoints Filter (possible Values: 'true' only published Measuringpoints, 'false' only not published Measuringpoints, (default:'null')</param>                
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null)<a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>List of Measuringpoint Objects</returns>
        [ProducesResponseType(typeof(IEnumerable<Measuringpoint>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 3600, CacheKeyGenerator = typeof(CustomCacheKeyGenerator))]
        [HttpGet, Route("Weather/Measuringpoint")]
        public async Task<IActionResult> GetMeasuringPoints(
            string? idlist = null,
            string? locfilter = null, 
            string? areafilter = null, 
            string? skiareafilter = null,
            string? language = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? updatefrom = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            string? seed = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            return await GetMeasuringPointList(
                fields: fields ?? Array.Empty<string>(), language: language, idfilter: idlist,
                    searchfilter: searchfilter, locfilter: locfilter, areafilter: areafilter,
                    skiareafilter: skiareafilter, active: active,
                    smgactive: odhactive, seed: seed, lastchange: updatefrom,
                    geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, 
                    removenullvalues: removenullvalues, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// GET Measuringpoint SINGLE
        /// </summary>
        /// <param name="id">Measuringpoint ID</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Measuringpoint Object</returns>
        [ProducesResponseType(typeof(Measuringpoint), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Weather/Measuringpoint/{id}", Name = "SingleWeather")]
        public async Task<IActionResult> GetMeasuringPoint(
            string id,
            string? language = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetMeasuringPointSingle(id, language, fields: fields ?? Array.Empty<string>(), removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Snowreport Data LIVE
        /// </summary>
        /// <param name="lang">Language</param>
        /// <param name="skiareaid">Skiarea ID</param>
        /// <returns>Snowreport BaseData Object</returns>
        [ProducesResponseType(typeof(SnowReportBaseData), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 3600, CacheKeyGenerator = typeof(CustomCacheKeyGenerator))]
        [HttpGet, Route("Weather/SnowReport")]
        public async Task<ActionResult<SnowReportBaseData>> GetSnowReportBase(
            string? skiareaid,
            string? lang = "de",             
            CancellationToken cancellationToken = default)
        {
            try
            {
                if(!String.IsNullOrEmpty(skiareaid))
                {
                    var snowreport = await GetSnowReportBaseData(lang ?? "de", skiareaid, cancellationToken);

                    return Ok(snowreport);
                }
                else
                {                    
                    //Get all skiareaids
                    var query = QueryFactory.Query()
                        .Select("id")
                        .From("skiareas")
                        .Where("gen_active", true);

                    var skiareaids = await query.GetAsync<string>();

                    List<SnowReportBaseData> snowreportbasedatalist = new List<SnowReportBaseData>();

                    //Fall 1 Getter auf ALL
                    foreach (var myskiareaid in skiareaids)
                    {
                        var result = await GetSnowReportBaseData(lang, myskiareaid, cancellationToken);

                        if (result != null)
                            snowreportbasedatalist.Add(result);
                    }

                    return Ok(snowreportbasedatalist);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        #endregion

        #region SiagWeather

        /// GET Current Suedtirol Weather LIVE Request
        private async Task<ActionResult<Weather>> Get(string language, string? locfilter, bool extended, CancellationToken cancellationToken = default)
        {
            var weatherresult = default(Weather);

            if(String.IsNullOrEmpty(locfilter))
            {
                weatherresult = await SIAG.GetWeatherData.GetCurrentWeatherAsync(language, settings.XmlConfig.XmldirWeather, settings.SiagConfig.Username, settings.SiagConfig.Password, extended);
            }
            else
            {
                string stationidtype = "";
                int n;
                if (int.TryParse(locfilter, out n))
                    stationidtype = "Id";
                else
                {
                    //Locfilter kann hier sein TV, Municipality und Fraktion
                    if (locfilter.Contains("reg"))
                    {
                        stationidtype = "Region";
                        locfilter = locfilter.Replace("reg", "");
                    }
                    if (locfilter.Contains("tvs"))
                    {
                        stationidtype = "TV";
                        locfilter = locfilter.Replace("tvs", "");
                    }
                    if (locfilter.Contains("mun"))
                    {
                        stationidtype = "Municipality";
                        locfilter = locfilter.Replace("mun", "");
                    }
                    if (locfilter.Contains("fra"))
                    {
                        stationidtype = "Fraktion";
                        locfilter = locfilter.Replace("fra", "");
                    }
                }

                weatherresult = await SIAG.GetWeatherData.GetCurrentStationWeatherAsync(language, locfilter, stationidtype, settings.XmlConfig.XmldirWeather, settings.SiagConfig.Username, settings.SiagConfig.Password);
            }

            return Ok(weatherresult);
        }
              
        /// GET Bezirkswetter by LocFilter LIVE Request
        private async Task<ActionResult<BezirksWeather>> GetBezirksWetter(string language, string locfilter, CancellationToken cancellationToken = default)
        {
            string bezirksid = "";
            string tvrid = "";
            string regid = "";

            int n;
            if (int.TryParse(locfilter, out n))
                bezirksid = n.ToString();
            else
            {
                //Locfilter kann hier sein Region TV, Municipality und Fraktion
                if (locfilter.Contains("reg"))
                {
                    regid = locfilter.Replace("reg", "");
                }

                if (locfilter.Contains("tvs"))
                {
                    tvrid = locfilter.Replace("tvs", "");
                }
                if (locfilter.Contains("mun"))
                {
                    var query =
                   QueryFactory.Query()
                       .SelectRaw("data ->>'TourismvereinId'")
                       .From("municipalities")
                       .Where("id", locfilter.Replace("mun", "").ToUpper());

                    tvrid = await query.FirstOrDefaultAsync<string>();
                }
                if (locfilter.Contains("fra"))
                {
                    var query =
                   QueryFactory.Query()
                       .SelectRaw("data ->>'TourismvereinId'")
                       .From("districts")
                       .Where("id", locfilter.Replace("fra", "").ToUpper());

                    tvrid = await query.FirstOrDefaultAsync<string>();
                }
            }

            var weatherresult = await GetWeatherData.GetCurrentBezirkWeatherAsync(language, bezirksid, tvrid, regid, settings.XmlConfig.XmldirWeather, settings.SiagConfig.Username, settings.SiagConfig.Password);
            return Ok(weatherresult);
        }

        /// GET Current Suedtirol Weather Realtime LIVE Request
        private async Task<ActionResult<IEnumerable<WeatherRealTime>>> GetRealTimeWeather(string language, CancellationToken cancellationToken = default)
        {
            var weatherresult = await GetWeatherData.GetCurrentRealTimeWEatherAsync(language);
            return Ok(weatherresult);
        }

        #endregion

        #region Measuringpoint

        /// GET Measuringpoints LIST
        private Task<IActionResult> GetMeasuringPointList(
            string? language,
            string? idfilter,
            string? locfilter,
            string? areafilter,
            string? skiareafilter,
            bool? active,
            bool? smgactive,
            string? lastchange,
            string? searchfilter,
            string? seed,
            string[] fields,
            PGGeoSearchResult geosearchresult,
            string? rawfilter,
            string? rawsort,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return DoAsyncReturn(async () =>
            {
                //Fix add are to every arefilter item
                string? arefilterwithprefix = String.IsNullOrEmpty(areafilter) ? "" : "are" + areafilter;
                string? skiarefilterwithprefix = String.IsNullOrEmpty(skiareafilter) ? "" : "ska" + skiareafilter;

                MeasuringPointsHelper mymeasuringpointshelper = await MeasuringPointsHelper.Create(QueryFactory, idfilter, locfilter,
                    arefilterwithprefix, skiarefilterwithprefix, active, smgactive, lastchange, cancellationToken);

                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From("measuringpoints")
                        .MeasuringpointWhereExpression(
                            idlist: mymeasuringpointshelper.idlist, districtlist: mymeasuringpointshelper.districtlist, municipalitylist: mymeasuringpointshelper.municipalitylist,
                            tourismvereinlist: mymeasuringpointshelper.tourismvereinlist, regionlist: mymeasuringpointshelper.regionlist, arealist: mymeasuringpointshelper.arealist,
                            activefilter: mymeasuringpointshelper.active, smgactivefilter: mymeasuringpointshelper.smgactive,
                            searchfilter: searchfilter, language: language, lastchange: mymeasuringpointshelper.lastchange,
                            filterClosedData: FilterClosedData)
                        .ApplyRawFilter(rawfilter)
                        .ApplyOrdering_GeneratedColumns(ref seed, geosearchresult, rawsort);//.ApplyOrdering(ref seed, geosearchresult, rawsort);


                var data =
                        await query
                            .GetAsync<JsonRaw>();

                var dataTransformed =
                    data.Select(
                        raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, userroles: UserRolesList)
                    );

                return dataTransformed;
            });
        }

        /// GET Measuringpoint SINGLE by ID
        private Task<IActionResult> GetMeasuringPointSingle(
            string id, 
            string? language, 
            string[] fields,
            bool removenullvalues,
            CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("measuringpoints")
                        .Select("data")
                        .Where("id", id)
                        .When(FilterClosedData, q => q.FilterClosedData());

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, userroles: UserRolesList);
            });

        }

        #endregion

        #region SnowReport

        /// GET Snowreport Data by SkiareaID LIVE
         private async Task<SnowReportBaseData> GetSnowReportBaseData(
             string lang, 
             string skiareaid,
             CancellationToken cancellationToken)
        {

            var query = QueryFactory.Query()
                       .SelectRaw("data")
                       .From("skiareas")
                       .Where("id", skiareaid);                       
            
            var skiarearaw = await query.FirstOrDefaultAsync<JsonRaw>();
            var skiarea = JsonConvert.DeserializeObject<SkiArea>(skiarearaw.Value);

            //Schoffts net afn SkiArea object zu bringen kriag do ollm a laars object
            //var skiarea2 = await query.FirstOrDefaultAsync<SkiArea>();

            //Des hingegen geat
            //var skiareastring = await query.FirstOrDefaultAsync<string>();
            //var skiareaobject = JsonConvert.DeserializeObject<SkiArea>(skiareastring);

            if (skiarearaw == null)
                throw new Exception("skiarea not found!");

            var mysnowreport = GetSnowReport.GetLiveSnowReport(lang, skiarea, "SMG", settings.LcsConfig.Username, settings.LcsConfig.Password, settings.LcsConfig.MessagePassword);

            return mysnowreport;
        }
      
        #endregion

    }
}