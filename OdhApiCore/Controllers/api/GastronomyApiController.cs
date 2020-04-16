using Helper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OdhApiCore.Responses;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    /// <summary>
    /// Gastronomy Api (data provided by LTS GastronomyData) SOME DATA Available as OPENDATA
    /// </summary>
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class GastronomyController : OdhController
    {
        // Only for test purposes

        public GastronomyController(IWebHostEnvironment env, ISettings settings, ILogger<GastronomyController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {
        }

        #region SWAGGER Exposed API

        /// <summary>
        /// GET Gastronomy List
        /// </summary>
        /// <param name="pagenumber">Pagenumber, (default:1)</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of Activity IDs), (default:'null')</param>
        /// <param name="locfilter">Locfilter (Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction)), (default:'null')</param>
        /// <param name="dishcodefilter">Dish Code Filter (BITMASK values: 1 = (Speisen), 2 = (Vorspeise), 4 = (Hauptspeise), 8 = (Nachspeise), 16 = (Tagesgericht), 32 = (Menü), 64 = (Degustationsmenü), 128 = (Kindermenüs), 256 = (Mittagsmenüs)</param>
        /// <param name="ceremonycodefilter">Ceremony Code Filter (BITMASK  values: 1 = (Familienfeiern), 2 = (Hochzeiten), 4 = (Geburtstagsfeiern), 8 = (Firmenessen), 16 = (Weihnachtsessen), 32 = (Sylvestermenü), 64 = (Seminare / Tagungen), 128 = (Versammlungen)</param>
        /// <param name="categorycodefilter">Category Code Filter (BITMASK  values: 1 = (Restaurant), 2 = (Bar / Café / Bistro), 4 = (Pub / Disco), 8 = (Apres Ski), 16 = (Jausenstation), 32 = (Pizzeria), 64 = (Bäuerlicher Schankbetrieb), 128 = (Buschenschank), 256 = (Hofschank), 512 = (Törggele Lokale), 1024 = (Schnellimbiss), 2048 = (Mensa), 4096 = (Vinothek /Weinhaus / Taverne), 8192 = (Eisdiele), 16348 = (Gasthaus), 32768 = (Gasthof), 65536 = (Braugarten), 131072 = (Schutzhütte), 262144 = (Alm), 524288 = (Skihütte)</param>
        /// <param name="facilitycodefilter">Facility Code Filter (BITMASK  values: 1 = (American Express), 2 = (Diners Club), 4 = (Eurocard / Mastercard), 8 = (Visa), 16 = (Hunde erlaubt), 32 = (Geeignet für Busse), 64 = (Garten), 128 = (Garagen), 256 = (Bierbar), 512 = (Kinderspielplatz), 1024 = (Spielzimmer), 2048 = (Spielplatz), 4096 = (Parkplätze), 8192 = (Raucherräume), 16348 = (Terrasse), 32768 = (Behindertengerecht), 65536 = (Biergarten), 131072 = (Aussichtsterrasse), 262144 = (Wintergarten), 524288 = (Gault Millau Südtirol), 1048576 = (Guida Espresso), 2097152 = (Gambero Rosso), 4194304 = (Feinschmecker), 8388608 = (Aral Schlemmer Atlas), 16777216 = (Varta Führer), 33554432 = (Bertelsmann), 67108864 = (Preis für Südtiroler Weinkultur), 134217728 = (Michelin), 268435456 = (Roter Hahn), 536870912 = (Tafelspitz))</param>       
        /// <param name="cuisinecodefilter">Cuisine Code Filter (BITMASK  values: 1 = (Vegetarische Küche), 2 = (Glutenfreie Küche), 4 = (Laktosefreie Kost), 8 = (Warme Küche), 16 = (Südtiroler Spezialitäten), 32 = (Gourmet Küche), 64 = (Italienische Küche), 128 = (Internationale Küche), 256 = (Pizza), 512 = (Fischspezialitäten), 1024 = (Asiatische Küche), 2048 = (Wildspezialitäten), 4096 = (Produkte eigener Erzeugung), 8192 = (Diätküche), 16348 = (Grillspezialitäten), 32768 = (Ladinische Küche), 65536 = (Kleine Karte), 131072 = (Fischwochen), 262144 = (Spargelwochen), 524288 = (Lammwochen), 1048576 = (Wildwochen), 2097152 = (Vorspeisewochen), 4194304 = (Nudelwochen), 8388608 = (Kräuterwochen), 16777216 = (Kindermenüs), 33554432 = (Mittagsmenüs))</param>       
        /// <param name="odhtagfilter">ODH Taglist Filter (refers to Array SmgTags) (String, Separator ',' more Tags possible, available Tags reference to 'api/ODHTag?validforentity=gastronomy'), (default:'null')</param>        
        /// <param name="active">Active Gastronomies Filter (possible Values: 'true' only Active Gastronomies, 'false' only Disabled Gastronomies</param>
        /// <param name="odhactive">ODH Active (Published) Gastronomies Filter (Refers to field SmgActive) Gastronomies Filter (possible Values: 'true' only published Gastronomies, 'false' only not published Gastronomies, (default:'null')</param>        
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param> 
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="updatefrom">Date from Format (yyyy-MM-dd) (all GBActivityPoi with LastChange >= datefrom are passed), (default: null)</param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null)</param>
        /// <returns>Collection of Gastronomy Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<Gastronomy>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("api/Gastronomy")]
        public async Task<IActionResult> GetGastronomyList(
            string? language = null,
            uint pagenumber = 1,
            uint pagesize = 10,
            string? idlist = null,
            string? locfilter = null,
            string? dishcodefilter = null,
            string? ceremonycodefilter = null,
            string? categorycodefilter = null,
            string? facilitycodefilter = null,
            string? cuisinecodefilter = null,
            string? odhtagfilter = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? lastchange = null, 
            string? seed = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            CancellationToken cancellationToken = default)
        {           
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            return await GetFiltered(
                    fields: fields ?? Array.Empty<string>(), language: language, pagenumber: pagenumber,
                    pagesize: pagesize, dishcodefilter: dishcodefilter, categorycodefilter: categorycodefilter, 
                    facilitycodefilter: facilitycodefilter, cuisinecodefilter: cuisinecodefilter, ceremonycodefilter: ceremonycodefilter, idfilter: idlist,
                    searchfilter: searchfilter, locfilter: locfilter, active: active,
                    smgactive: odhactive, smgtags: odhtagfilter, seed: seed, lastchange: lastchange,
                    geosearchresult: geosearchresult, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// GET Gastronomy Single 
        /// </summary>
        /// <param name="id">ID of the Gastronomy</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <returns>Gastronomy Object</returns>
        [ProducesResponseType(typeof(GBLTSActivity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("api/Gastronomy/{id}")]
        public async Task<IActionResult> GetGastronomySingle(
            string id,
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            CancellationToken cancellationToken = default)
        {
            return await GetSingle(id, language, fields: fields ?? Array.Empty<string>(), cancellationToken);
        }

        /// <summary>
        /// GET Gastronomy Types List
        /// </summary>
        /// <returns>Collection of GastronomyTypes Object</returns>                
        [ProducesResponseType(typeof(IEnumerable<GastronomyTypes>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,GastroReader")]        
        [HttpGet, Route("api/GastronomyTypes")]
        public async Task<IActionResult> GetAllGastronomyTypesList(CancellationToken cancellationToken = default)
        {
            return await GetGastronomyTypesListAsync(cancellationToken);
        }

        /// <summary>
        /// GET Gastronomy Types Single
        /// </summary>
        /// <returns>GastronomyTypes Object</returns>                
        [ProducesResponseType(typeof(GastronomyTypes), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,GastroReader")]        
        [HttpGet, Route("api/GastronomyTypes/{id}")]
        public async Task<IActionResult> GetAllGastronomyTypesList(string id, CancellationToken cancellationToken = default)
        {
            return await GetGastronomyTypesSingleAsync(id, cancellationToken);
        }

        #endregion

        #region GETTER

        private Task<IActionResult> GetFiltered(
            string[] fields, string? language, uint pagenumber, uint pagesize, 
            string? categorycodefilter, string? dishcodefilter, string? ceremonycodefilter, string? facilitycodefilter, string? cuisinecodefilter,
            string? idfilter, string? searchfilter, string? locfilter, bool? active, bool? smgactive,
            string? smgtags, string? seed, string? lastchange, PGGeoSearchResult geosearchresult, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                GastronomyHelper mygastronomyhelper = await GastronomyHelper.CreateAsync(
                    QueryFactory, idfilter, locfilter, categorycodefilter,
                    dishcodefilter, ceremonycodefilter, facilitycodefilter, cuisinecodefilter,
                    active, smgactive, smgtags, lastchange, cancellationToken);

                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From("gastronomies")
                        .GastronomyWhereExpression(
                            idlist: mygastronomyhelper.idlist, dishcodeslist: mygastronomyhelper.dishcodesids, ceremonycodeslist: mygastronomyhelper.ceremonycodesids,
                            categorycodeslist: mygastronomyhelper.categorycodesids, facilitycodeslist: mygastronomyhelper.facilitycodesids,
                            smgtaglist: mygastronomyhelper.smgtaglist, districtlist: mygastronomyhelper.districtlist,
                            municipalitylist: mygastronomyhelper.municipalitylist, tourismvereinlist: mygastronomyhelper.tourismvereinlist,
                            regionlist: mygastronomyhelper.regionlist, activefilter: mygastronomyhelper.active, 
                            smgactivefilter: mygastronomyhelper.smgactive,
                            searchfilter: searchfilter, language: language, lastchange: mygastronomyhelper.lastchange)
                        .OrderBySeed(ref seed, "data ->>'Shortname' ASC")
                        .GeoSearchFilterAndOrderby(geosearchresult);

                // Get paginated data
                var data =
                    await query
                        .PaginateAsync<JsonRaw>(
                            page: (int)pagenumber,
                            perPage: (int)pagesize);

                var dataTransformed =
                    data.List.Select(
                        raw => raw.TransformRawData(language, fields, checkCC0: CheckCC0License, filterClosedData: FilterClosedData)
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
                    QueryFactory.Query("gastronomies")
                        .Select("data")
                        .Where("id", id);

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data?.TransformRawData(language, fields, checkCC0: CheckCC0License, filterClosedData: FilterClosedData);
            });
        }

        #endregion

        #region CUSTOM METHODS

        /// <summary>
        /// GET Gastronomy Types List
        /// </summary>
        /// <returns>Collection of GastronomyTypes Object</returns>
        private Task<IActionResult> GetGastronomyTypesListAsync(CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("gastronomytypes")
                        .SelectRaw("data");

                var data = await query.GetAsync<JsonRaw?>();

                return data;
            });
        }

        /// <summary>
        /// GET Gastronomy Types Single
        /// </summary>
        /// <returns>GastronomyTypes Object</returns>
        private Task<IActionResult> GetGastronomyTypesSingleAsync(string id, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("gastronomytypes")
                        .Select("data")
                        .WhereJsonb("Key", id.ToUpper());
                //.Where("data ->>'Key'", "ILIKE", id);

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data;
            });
        }

        #endregion
    }
}