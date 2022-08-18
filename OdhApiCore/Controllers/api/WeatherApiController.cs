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
using OdhApiCore.Filters;
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
    /// Weather Api (data provided by SIAG, LTS) SOME DATA Available as OPENDATA
    /// </summary>
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class WeatherController : OdhController
    {
        private readonly ISettings settings;

        public WeatherController(
            IWebHostEnvironment env,
            ISettings settings,
            ILogger<WeatherController> logger,
            QueryFactory queryFactory
        ) : base(env, settings, logger, queryFactory)
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
        public async Task<IActionResult> GetWeather(
            uint? pagenumber = null,
            PageSize pagesize = null!,
            string? language = "en",
            string? locfilter = null,
            bool extended = true,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                return await Get(
                    pagenumber,
                    pagesize,
                    language ?? "en",
                    locfilter,
                    extended,
                    cancellationToken
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { error = ex.Message }
                );
            }
        }

        /// <summary>
        /// GET Suedtirol Weather HISTORY
        /// </summary>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="language">Language</param>
        /// <param name="datefilter">DateFilter Format dd/MM/yyyy</param>
        /// <returns>Weather Object</returns>
        [ProducesResponseType(typeof(Weather), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("WeatherHistory")]
        public async Task<IActionResult> GetWeatherHistory(
            uint pagenumber = 1,
            PageSize pagesize = null!,
            string? language = null,
            string? idlist = null,
            string? locfilter = null,
            string? datefrom = null,
            string? dateto = null,
            string? seed = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))] string[]? fields = null,
            string? searchfilter = null,
            string? lastchange = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                return await GetWeatherHistoryList(
                    pagenumber: pagenumber,
                    pagesize: pagesize,
                    language: language,
                    idfilter: idlist,
                    locfilter: locfilter,
                    datefrom: datefrom,
                    dateto: dateto,
                    lastchange: lastchange,
                    searchfilter: searchfilter,
                    seed: seed,
                    fields: fields ?? Array.Empty<string>(),
                    new PGGeoSearchResult(),
                    rawfilter,
                    rawsort,
                    removenullvalues,
                    cancellationToken
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { error = ex.Message }
                );
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
        public async Task<IActionResult> GetDistrictWeather(
            uint? pagenumber = null,
            PageSize pagesize = null!,
            string? locfilter = null,
            string? language = "en",
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                return await GetBezirksWetter(
                    pagenumber,
                    pagesize,
                    language ?? "en",
                    locfilter,
                    cancellationToken
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { error = ex.Message }
                );
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
        public async Task<IActionResult> GetRealtimeWeather(
            uint? pagenumber = null,
            PageSize pagesize = null!,
            string? language = "en",
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                return await GetRealTimeWeather(
                    pagenumber,
                    pagesize,
                    language ?? "en",
                    cancellationToken
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { error = ex.Message }
                );
            }
        }

        /// <summary>
        /// GET Measuringpoint LIST
        /// </summary>
        /// <param name="idlist">IDFilter (Separator ',' List of Gastronomy IDs), (default:'null')</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, not provided disables Random Sorting, (default:'null') </param>
        /// <param name="locfilter">Locfilter (Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction), 'null' = (No Filter), (default:'null')</param>
        /// <param name="areafilter">Area ID (multiple IDs possible, separated by ",")</param>
        /// <param name="skiareafilter">Skiarea ID</param>
        /// <param name="active">Active Filter (possible Values: 'true' only Active Measuringpoints, 'false' only Disabled Measuringpoints), (default:'null')</param>
        /// <param name="odhactive">ODH Active Filter Measuringpoints Filter (possible Values: 'true' only published Measuringpoints, 'false' only not published Measuringpoints), (default:'null')</param>
        /// <param name="latitude">GeoFilter FLOAT Latitude Format: '46.624975', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter' target="_blank">Wiki geosort</a></param>
        /// <param name="longitude">GeoFilter FLOAT Longitude Format: '11.369909', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter' target="_blank">Wiki geosort</a></param>
        /// <param name="radius">Radius INTEGER to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter' target="_blank">Wiki geosort</a></param>
        /// <param name="publishedon">Published On Filter (Separator ',' List of publisher IDs), (default:'null')</param>
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>
        /// <returns>List of Measuringpoint Objects</returns>
        [ProducesResponseType(typeof(IEnumerable<Measuringpoint>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [OdhCacheOutput(
            ClientTimeSpan = 0,
            ServerTimeSpan = 3600,
            CacheKeyGenerator = typeof(CustomCacheKeyGenerator),
            MustRevalidate = true
        )]
        [HttpGet, Route("Weather/Measuringpoint")]
        public async Task<IActionResult> GetMeasuringPoints(
            uint? pagenumber = null,
            PageSize pagesize = null!,
            string? idlist = null,
            string? locfilter = null,
            string? areafilter = null,
            string? skiareafilter = null,
            string? language = null,
            string? source = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? publishedon = null,
            string? updatefrom = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            string? seed = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))] string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default
        )
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(
                latitude,
                longitude,
                radius
            );

            return await GetMeasuringPointList(
                pagenumber,
                pagesize,
                fields: fields ?? Array.Empty<string>(),
                language: language,
                idfilter: idlist,
                searchfilter: searchfilter,
                locfilter: locfilter,
                areafilter: areafilter,
                skiareafilter: skiareafilter,
                source: source,
                active: active,
                publishedon: publishedon,
                smgactive: odhactive,
                seed: seed,
                lastchange: updatefrom,
                geosearchresult: geosearchresult,
                rawfilter: rawfilter,
                rawsort: rawsort,
                removenullvalues: removenullvalues,
                cancellationToken: cancellationToken
            );
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
            [ModelBinder(typeof(CommaSeparatedArrayBinder))] string[]? fields = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default
        )
        {
            return await GetMeasuringPointSingle(
                id,
                language,
                fields: fields ?? Array.Empty<string>(),
                removenullvalues,
                cancellationToken
            );
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
        [OdhCacheOutput(
            ClientTimeSpan = 0,
            ServerTimeSpan = 3600,
            CacheKeyGenerator = typeof(CustomCacheKeyGenerator)
        )]
        [HttpGet, Route("Weather/SnowReport")]
        public async Task<ActionResult> GetSnowReportBase(
            uint? pagenumber = null,
            PageSize pagesize = null!,
            string? skiareaid = null,
            string? lang = "en",
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                if (!String.IsNullOrEmpty(skiareaid))
                {
                    var snowreport = await GetSnowReportBaseData(
                        lang,
                        skiareaid,
                        cancellationToken
                    );

                    if (pagenumber != null)
                    {
                        return Ok(
                            ResponseHelpers.GetResult(
                                pagenumber.Value,
                                1,
                                1,
                                null,
                                new List<SnowReportBaseData>() { snowreport },
                                Url
                            )
                        );
                    }
                    else
                    {
                        return Ok(snowreport);
                    }
                }
                else
                {
                    //Get all skiareaids
                    var query = QueryFactory
                        .Query()
                        .Select("id")
                        .From("skiareas")
                        .Where("gen_active", true);

                    var skiareaids = await query.GetAsync<string>();

                    List<SnowReportBaseData> snowreportbasedatalist =
                        new List<SnowReportBaseData>();

                    //Fall 1 Getter auf ALL
                    foreach (var myskiareaid in skiareaids)
                    {
                        var result = await GetSnowReportBaseData(
                            lang,
                            myskiareaid,
                            cancellationToken
                        );

                        if (result != null)
                            snowreportbasedatalist.Add(result);
                    }

                    if (pagenumber != null)
                    {
                        return Ok(
                            ResponseHelpers.GetResult(
                                pagenumber.Value,
                                1,
                                (uint)snowreportbasedatalist.Count,
                                null,
                                snowreportbasedatalist,
                                Url
                            )
                        );
                    }
                    else
                    {
                        return Ok(snowreportbasedatalist);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { error = ex.Message }
                );
            }
        }

        #endregion

        #region SiagWeather

        /// GET Current Suedtirol Weather LIVE Request
        private async Task<IActionResult> Get(
            uint? pagenumber,
            int? pagesize,
            string language,
            string? locfilter,
            bool extended,
            CancellationToken cancellationToken
        )
        {
            var weatherresult = default(Weather);

            if (String.IsNullOrEmpty(locfilter))
            {
                //Get Weather General from Siag and Parse it to ODH Format
                var weatherresponsetask = await SIAG.GetWeatherData.GetSiagWeatherData(
                    language,
                    settings.SiagConfig.Username,
                    settings.SiagConfig.Password,
                    extended
                );
                weatherresult = await SIAG.GetWeatherData.ParseSiagWeatherDataToODHWeather(
                    language,
                    settings.XmlConfig.XmldirWeather,
                    weatherresponsetask,
                    extended
                );
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

                weatherresult = await SIAG.GetWeatherData.GetCurrentStationWeatherAsync(
                    language,
                    locfilter,
                    stationidtype,
                    settings.XmlConfig.XmldirWeather,
                    settings.SiagConfig.Username,
                    settings.SiagConfig.Password
                );
            }

            if (pagenumber != null)
            {
                return Ok(
                    ResponseHelpers.GetResult(
                        pagenumber.Value,
                        1,
                        1,
                        null,
                        new List<Weather?>() { weatherresult },
                        Url
                    )
                );
            }
            else
            {
                return Ok(weatherresult);
            }
        }

        /// GET Bezirkswetter by LocFilter LIVE Request
        private async Task<IActionResult> GetBezirksWetter(
            uint? pagenumber,
            int? pagesize,
            string language,
            string? locfilter,
            CancellationToken cancellationToken
        )
        {
            string bezirksid = "";
            string tvrid = "";
            string regid = "";

            if (!String.IsNullOrEmpty(locfilter))
            {
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
                        var query = QueryFactory
                            .Query()
                            .SelectRaw("data ->>'TourismvereinId'")
                            .From("municipalities")
                            .Where("id", locfilter.Replace("mun", "").ToUpper());

                        tvrid = await query.FirstOrDefaultAsync<string>();
                    }
                    if (locfilter.Contains("fra"))
                    {
                        var query = QueryFactory
                            .Query()
                            .SelectRaw("data ->>'TourismvereinId'")
                            .From("districts")
                            .Where("id", locfilter.Replace("fra", "").ToUpper());

                        tvrid = await query.FirstOrDefaultAsync<string>();
                    }
                }
            }

            var weatherresult = await GetWeatherData.GetCurrentBezirkWeatherAsync(
                language,
                bezirksid,
                tvrid,
                regid,
                settings.XmlConfig.XmldirWeather,
                settings.SiagConfig.Username,
                settings.SiagConfig.Password
            );

            if (pagenumber != null)
            {
                return Ok(
                    ResponseHelpers.GetResult(
                        pagenumber.Value,
                        1,
                        (uint)weatherresult.Count(),
                        null,
                        weatherresult,
                        Url
                    )
                );
            }
            else
            {
                //Compatibility Hack
                if (weatherresult.Count() == 1)
                    return Ok(weatherresult.FirstOrDefault());
                else
                    return Ok(weatherresult);
            }
        }

        /// GET Current Suedtirol Weather Realtime LIVE Request
        private async Task<IActionResult> GetRealTimeWeather(
            uint? pagenumber,
            int? pagesize,
            string language,
            CancellationToken cancellationToken
        )
        {
            var weatherresult = await GetWeatherData.GetCurrentRealTimeWEatherAsync(language);

            if (pagenumber != null)
            {
                return Ok(
                    ResponseHelpers.GetResult(
                        pagenumber.Value,
                        1,
                        (uint)weatherresult.Count(),
                        null,
                        weatherresult,
                        Url
                    )
                );
            }
            else
            {
                return Ok(weatherresult);
            }
        }

        /// GET Suedtirol Weather from History Table
        private Task<IActionResult> GetWeatherHistoryList(
            uint pagenumber,
            int? pagesize,
            string? language,
            string? idfilter,
            string? locfilter,
            string? datefrom,
            string? dateto,
            string? lastchange,
            string? searchfilter,
            string? seed,
            string[] fields,
            PGGeoSearchResult geosearchresult,
            string? rawfilter,
            string? rawsort,
            bool removenullvalues,
            CancellationToken cancellationToken
        )
        {
            return DoAsyncReturn(async () =>
            {
                WeatherHelper myweatherhelper = await WeatherHelper.CreateAsync(
                    QueryFactory,
                    idfilter,
                    locfilter,
                    language,
                    datefrom,
                    dateto,
                    lastchange,
                    cancellationToken
                );

                var query = QueryFactory
                    .Query()
                    .SelectRaw("data")
                    .From("weatherdatahistory")
                    .WeatherHistoryWhereExpression(
                        languagelist: myweatherhelper.languagelist,
                        idlist: myweatherhelper.idlist,
                        sourcelist: new List<string>(),
                        begindate: myweatherhelper.datefrom,
                        enddate: myweatherhelper.dateto,
                        searchfilter: searchfilter,
                        language: language,
                        lastchange: myweatherhelper.lastchange,
                        filterClosedData: false
                    )
                    .ApplyRawFilter(rawfilter)
                    .ApplyOrdering(ref seed, geosearchresult, rawsort);
                //.ApplyOrdering_GeneratedColumns(ref seed, geosearchresult, rawsort);//


                var data = await query.PaginateAsync<JsonRaw>(
                    page: (int)pagenumber,
                    perPage: pagesize ?? 25
                );

                var fieldsTohide = FieldsToHide;

                var dataTransformed = data.List.Select(
                    raw =>
                        raw.TransformRawData(
                            language,
                            fields,
                            checkCC0: false,
                            filterClosedData: false,
                            filteroutNullValues: removenullvalues,
                            urlGenerator: UrlGenerator,
                            fieldstohide: fieldsTohide
                        )
                );

                //return dataTransformed;

                uint totalpages = (uint)data.TotalPages;
                uint totalcount = (uint)data.Count;

                return ResponseHelpers.GetResult(
                    pagenumber,
                    totalpages,
                    totalcount,
                    seed,
                    dataTransformed,
                    Url
                );
            });
        }

        #endregion

        #region Measuringpoint

        /// GET Measuringpoints LIST
        private Task<IActionResult> GetMeasuringPointList(
            uint? pagenumber,
            int? pagesize,
            string? language,
            string? idfilter,
            string? locfilter,
            string? areafilter,
            string? skiareafilter,
            string? source,
            bool? active,
            bool? smgactive,
            string? lastchange,
            string? publishedon,
            string? searchfilter,
            string? seed,
            string[] fields,
            PGGeoSearchResult geosearchresult,
            string? rawfilter,
            string? rawsort,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default
        )
        {
            return DoAsyncReturn(async () =>
            {
                //Fix add are to every arefilter item
                string? arefilterwithprefix = String.IsNullOrEmpty(areafilter)
                    ? ""
                    : "are" + areafilter;
                string? skiarefilterwithprefix = String.IsNullOrEmpty(skiareafilter)
                    ? ""
                    : "ska" + skiareafilter;

                MeasuringPointsHelper mymeasuringpointshelper = await MeasuringPointsHelper.Create(
                    QueryFactory,
                    idfilter,
                    locfilter,
                    arefilterwithprefix,
                    skiarefilterwithprefix,
                    source,
                    active,
                    smgactive,
                    lastchange,
                    publishedon,
                    cancellationToken
                );

                var query = QueryFactory
                    .Query()
                    .SelectRaw("data")
                    .From("measuringpoints")
                    .MeasuringpointWhereExpression(
                        idlist: mymeasuringpointshelper.idlist,
                        districtlist: mymeasuringpointshelper.districtlist,
                        municipalitylist: mymeasuringpointshelper.municipalitylist,
                        tourismvereinlist: mymeasuringpointshelper.tourismvereinlist,
                        regionlist: mymeasuringpointshelper.regionlist,
                        arealist: mymeasuringpointshelper.arealist,
                        activefilter: mymeasuringpointshelper.active,
                        smgactivefilter: mymeasuringpointshelper.smgactive,
                        publishedonlist: mymeasuringpointshelper.publishedonlist,
                        sourcelist: mymeasuringpointshelper.sourcelist,
                        searchfilter: searchfilter,
                        language: language,
                        lastchange: mymeasuringpointshelper.lastchange,
                        filterClosedData: FilterClosedData,
                        reducedData: ReducedData
                    )
                    .ApplyRawFilter(rawfilter)
                    .ApplyOrdering_GeneratedColumns(ref seed, geosearchresult, rawsort); //.ApplyOrdering(ref seed, geosearchresult, rawsort);

                //Hack Paging on Measuringpoints
                if (pagenumber != null)
                {
                    var data = await query.PaginateAsync<JsonRaw>(
                        page: (int)pagenumber,
                        perPage: pagesize ?? 25
                    );

                    var fieldsTohide = FieldsToHide;

                    var dataTransformed = data.List.Select(
                        raw =>
                            raw.TransformRawData(
                                language,
                                fields,
                                checkCC0: false,
                                filterClosedData: false,
                                filteroutNullValues: removenullvalues,
                                urlGenerator: UrlGenerator,
                                fieldstohide: fieldsTohide
                            )
                    );

                    uint totalpages = (uint)data.TotalPages;
                    uint totalcount = (uint)data.Count;

                    return ResponseHelpers.GetResult(
                        pagenumber.Value,
                        totalpages,
                        totalcount,
                        seed,
                        dataTransformed,
                        Url
                    );
                }
                else
                {
                    var data = await query.GetAsync<JsonRaw>();

                    var fieldsTohide = FieldsToHide;

                    var dataTransformed = data.Select(
                        raw =>
                            raw.TransformRawData(
                                language,
                                fields,
                                checkCC0: FilterCC0License,
                                filterClosedData: FilterClosedData,
                                filteroutNullValues: removenullvalues,
                                urlGenerator: UrlGenerator,
                                fieldstohide: fieldsTohide
                            )
                    );

                    return dataTransformed;
                }
            });
        }

        /// GET Measuringpoint SINGLE by ID
        private Task<IActionResult> GetMeasuringPointSingle(
            string id,
            string? language,
            string[] fields,
            bool removenullvalues,
            CancellationToken cancellationToken
        )
        {
            return DoAsyncReturn(async () =>
            {
                var query = QueryFactory
                    .Query("measuringpoints")
                    .Select("data")
                    .Where("id", id.ToUpper())
                    .Anonymous_Logged_UserRule_GeneratedColumn(FilterClosedData, !ReducedData);
                //.When(FilterClosedData, q => q.FilterClosedData());

                var fieldsTohide = FieldsToHide;

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data?.TransformRawData(
                    language,
                    fields,
                    checkCC0: FilterCC0License,
                    filterClosedData: FilterClosedData,
                    filteroutNullValues: removenullvalues,
                    urlGenerator: UrlGenerator,
                    fieldstohide: fieldsTohide
                );
            });
        }

        #endregion

        #region SnowReport

        /// GET Snowreport Data by SkiareaID LIVE
        private async Task<SnowReportBaseData> GetSnowReportBaseData(
            string? lang,
            string skiareaid,
            CancellationToken cancellationToken
        )
        {
            if (lang == null)
                throw new Exception("parameter lang is null");

            var query = QueryFactory
                .Query()
                .SelectRaw("data")
                .From("skiareas")
                .Where("id", skiareaid);

            var skiarearaw = await query.FirstOrDefaultAsync<JsonRaw>();

            if (skiarearaw == null)
                throw new Exception("skiarea not found!");

            var skiarea = JsonConvert.DeserializeObject<SkiArea>(skiarearaw.Value);

            //Schoffts net afn SkiArea object zu bringen kriag do ollm a laars object
            //var skiarea2 = await query.FirstOrDefaultAsync<SkiArea>();

            //Des hingegen geat
            //var skiareastring = await query.FirstOrDefaultAsync<string>();
            //var skiareaobject = JsonConvert.DeserializeObject<SkiArea>(skiareastring);

            var mysnowreport = GetSnowReport.GetLiveSnowReport(
                lang,
                skiarea!,
                "SMG",
                settings.LcsConfig.Username,
                settings.LcsConfig.Password,
                settings.LcsConfig.MessagePassword
            );

            return mysnowreport;
        }

        #endregion
    }
}
