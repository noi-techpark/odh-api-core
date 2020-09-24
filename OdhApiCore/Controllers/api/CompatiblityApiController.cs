using DataModel;
using Helper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace OdhApiCore.Controllers.api
{
    /// <summary>
    /// Compatiblity Api (Old Routes & methods for compatiblity reasons)
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class CompatiblityApiController : OdhController
    {
        public CompatiblityApiController(IWebHostEnvironment env, ISettings settings, ILogger<CompatiblityApiController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {
        }

        #region PoiController

        //Reduced GETTER

        /// <summary>
        /// GET Poi List Reduced
        /// </summary>
        /// <param name="language">Localization Language, (default:'en')</param>
        /// <param name="poitype">Type of the Poi ('null' = Filter disabled, possible values: BITMASK 'Doctors, Pharmacies = 1','Shops = 2','Culture and sights= 4','Nightlife and entertainment = 8','Public institutions = 16','Sports and leisure = 32','Traffic and transport = 64', 'Service providers' = 128, 'Craft' = 256), (default:'511' == ALL), REFERENCE TO: GET /api/PoiTypes </param>
        /// <param name="subtype">Subtype of the Activity (BITMASK Filter = available SubTypes depends on the selected poiType), (default:'null')</param>
        /// <param name="locfilter">Locfilter (Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMASSOCIATIONID = (Filter by Tourismassociation), 'null' = No Filter), (default:'null')</param>
        /// <param name="areafilter">AreaFilter (Alternate Locfilter, can be combined with locfilter) (Separator ',' possible values: reg + REGIONID = (Filter by Region), tvs + TOURISMASSOCIATIONID = (Filter by Tourismassociation), skr + SKIREGIONID = (Filter by Skiregion), ska + SKIAREAID = (Filter by Skiarea), are + AREAID = (Filter by LTS Area), 'null' = No Filter), (default:'null')</param>
        /// <param name="highlight">Hightlight Filter (possible values: 'false' = only Pois with Highlight false, 'true' = only Pois with Highlight true), (default:'null')</param>
        /// <param name="odhtagfilter">ODH Taglist Filter (refers to Array SmgTags) (String, Separator ',' more Tags possible, available Tags reference to 'api/ODHTag?validforentity=poi'), (default:'null')</param>        
        /// <param name="active">Active Pois Filter (possible Values: 'true' only Active Pois, 'false' only Disabled Pois</param>
        /// <param name="odhactive">ODH Active (Published) Pois Filter (Refers to field SmgActive) Pois Filter (possible Values: 'true' only published Pois, 'false' only not published Pois, (default:'null')</param>        
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <returns>Collection of Poi Reduced Objects</returns>        
        [ProducesResponseType(typeof(IEnumerable<ActivityPoiReduced>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,PoiReader")]
        [HttpGet, Route("PoiReduced")]
        public async Task<IActionResult> GetPoiReduced(
            string? language = "en",
            string? poitype = "511",
            string? subtype = null,
            string? locfilter = null,
            string? areafilter = null,
            LegacyBool highlight = null!,
            string? odhtagfilter = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            CancellationToken cancellationToken = default)
        {
            //TODO
            //CheckOpenData(User);

            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            return await GetPoiReduced(language, poitype, subtype, locfilter, areafilter, highlight, active, odhactive, odhtagfilter, geosearchresult, cancellationToken);
        }


        /// <summary>
        /// GET Reduced POI List
        /// </summary>
        /// <param name="language">Localization Language</param>
        /// <param name="poitype">Type of the Poi (possible values: STRINGS: 'Ärtze, Apotheken','Geschäfte und Dienstleister','Kultur und Sehenswürdigkeiten','Nachtleben und Unterhaltung','Öffentliche Einrichtungen','Sport und Freizeit','Verkehr und Transport' : BITMASK also possible: 'Ärtze, Apotheken = 1','Geschäfte und Dienstleister = 2','Kultur und Sehenswürdigkeiten = 4','Nachtleben und Unterhaltung = 8','Öffentliche Einrichtungen = 16','Sport und Freizeit = 32','Verkehr und Transport = 64')</param>
        /// <param name="subtypefilter">Subtype of the Poi ('null' = Filter disabled, available Subtypes depends on the activitytype BITMASK)</param>        
        /// <param name="locfilter">Locfilter (Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), 'null' = No Filter)</param>
        /// <param name="areafilter">AreaFilter (Separator ',' IDList of AreaIDs separated by ',', 'null' : Filter disabled)</param>
        /// <param name="highlightfilter">Highlight Filter (Show only Highlights possible values: 'true' : show only Highlight Pois, 'null' Filter disabled)</param>
        /// <param name="active">Active Filter (possible Values: 'null' Displays all Pois, 'true' only Active Pois, 'false' only Disabled Pois</param>
        /// <param name="smgactive">SMGActive Filter (possible Values: 'null' Displays all Pois, 'true' only SMG Active Pois, 'false' only SMG Disabled Pois</param>
        /// <param name="smgtags">SMGTag Filter (String, Separator ',' more SMGTags possible, 'null' = No Filter)</param>
        /// <returns>Collection of Reduced Poi Objects</returns>
        private Task<IActionResult> GetPoiReduced(
            string? language, string? poitype, string? subtypefilter, string? locfilter,
            string? areafilter, bool? highlightfilter, bool? active, bool? smgactive,
            string? smgtags, PGGeoSearchResult geosearchresult, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                PoiHelper mypoihelper = await PoiHelper.CreateAsync(
                    QueryFactory, poitype, subtypefilter, null, locfilter, areafilter,
                    highlightfilter, active, smgactive, smgtags, null, cancellationToken);

                string select = $"data#>>'\\{{Id\\}}' as Id, data#>>'\\{{Detail,{language},Title\\}}' as Name";
                string orderby = "data#>>'\\{Shortname\\}' ASC";

                var query =
                    QueryFactory.Query()
                        .SelectRaw(select)
                        .From("activities")
                        .PoiWhereExpression(
                            idlist: mypoihelper.idlist, poitypelist: mypoihelper.poitypelist, subtypelist: mypoihelper.subtypelist,
                            smgtaglist: mypoihelper.smgtaglist, districtlist: new List<string>(), municipalitylist: new List<string>(),
                            tourismvereinlist: mypoihelper.tourismvereinlist, regionlist: mypoihelper.regionlist,
                            arealist: mypoihelper.arealist, highlight: mypoihelper.highlight, activefilter: mypoihelper.active,
                            smgactivefilter: mypoihelper.smgactive, searchfilter: null, language: language, lastchange: null, languagelist: new List<string>(),
                            filterClosedData: FilterClosedData
                        )
                        .OrderByRaw(orderby)
                        .GeoSearchFilterAndOrderby(geosearchresult);

                // Get paginated data
                return await query.GetAsync<ResultReduced>();
            });
        }

        #endregion

        #region ActivityController       

        /// <summary>
        /// GET Activity List Reduced
        /// </summary>
        /// <param name="language">Localization Language, (default:'en')</param>
        /// <param name="activitytype">Type of the Activity ('null' = Filter disabled, possible values: BITMASK: 'Mountains = 1','Cycling = 2','Local tours = 4','Horses = 8','Hiking = 16','Running and fitness = 32','Cross-country ski-track = 64','Tobbogan run = 128','Slopes = 256','Lifts = 512'), (default:'1023' == ALL), REFERENCE TO: GET /api/ActivityTypes </param>
        /// <param name="subtype">Subtype of the Activity (BITMASK Filter = available SubTypes depends on the selected Activity Type), (default:'null')</param>
        /// <param name="locfilter">Locfilter (Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction)), (default:'null')</param>
        /// <param name="areafilter">AreaFilter (Separator ',' IDList of AreaIDs separated by ','), (default:'null')</param>
        /// <param name="distancefilter">Distance Range Filter (Separator ',' example Value: 15,40 Distance from 15 up to 40 Km), (default:'null')</param>
        /// <param name="altitudefilter">Altitude Range Filter (Separator ',' example Value: 500,1000 Altitude from 500 up to 1000 metres), (default:'null')</param>
        /// <param name="durationfilter">Duration Range Filter (Separator ',' example Value: 1,3 Duration from 1 to 3 hours), (default:'null')</param>
        /// <param name="highlight">Hightlight Filter (possible values: 'false' = only Activities with Highlight false, 'true' = only Activities with Highlight true), (default:'null')</param>
        /// <param name="difficultyfilter">Difficulty Filter (possible values: '1' = easy, '2' = medium, '3' = difficult), (default:'null')</param>      
        /// <param name="odhtagfilter">Taglist Filter (String, Separator ',' more Tags possible, available Tags reference to 'api/SmgTag/ByMainEntity/Activity'), (default:'null')</param>        
        /// <param name="active">Active Activities Filter (possible Values: 'true' only Active Activities, 'false' only Disabled Activities</param>
        /// <param name="odhactive"> odhactive (Published) Activities Filter (possible Values: 'true' only published Activities, 'false' only not published Activities, (default:'null')</param>        
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <returns>Collection of Activity Reduced Objects</returns>        
        [ProducesResponseType(typeof(IEnumerable<ActivityPoiReduced>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("ActivityReduced")]
        public async Task<IActionResult> GetActivityReduced(
            string? language = "en",
            string? activitytype = "1023",
            string? subtype = null,
            string? locfilter = null,
            string? areafilter = null,
            string? distancefilter = null,
            string? altitudefilter = null,
            string? durationfilter = null,
            LegacyBool highlight = null!,
            string? difficultyfilter = null,
            string? odhtagfilter = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            return await GetActivityReduced(
                language, activitytype, subtype, locfilter, areafilter, distancefilter, altitudefilter, durationfilter,
                highlight, difficultyfilter, active, odhactive, odhtagfilter, geosearchresult, cancellationToken);
        }

        private Task<IActionResult> GetActivityReduced(
            string? language, string? activitytype, string? subtypefilter, string? locfilter, string? areafilter,
            string? distancefilter, string? altitudefilter, string? durationfilter, bool? highlightfilter,
            string? difficultyfilter, bool? active, bool? smgactive, string? smgtags,
            PGGeoSearchResult geosearchresult, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                ActivityHelper myactivityhelper = await ActivityHelper.CreateAsync(
                    QueryFactory, activitytype: activitytype, subtypefilter: subtypefilter, idfilter: null,
                    locfilter: locfilter, areafilter: areafilter, distancefilter: distancefilter,
                    altitudefilter: altitudefilter, durationfilter: durationfilter, highlightfilter: highlightfilter,
                    difficultyfilter: difficultyfilter, activefilter: active, smgactivefilter: smgactive,
                    smgtags: smgtags, lastchange: null, cancellationToken: cancellationToken);
                
                string select = $"data#>>'\\{{Id\\}}' as Id, data#>>'\\{{Detail,{language},Title\\}}' as Name";
                string orderby = "data#>>'\\{Shortname\\}' ASC";

                var query =
                    QueryFactory.Query()
                        .SelectRaw(select)
                        .From("activities")
                        .ActivityWhereExpression(
                            idlist: myactivityhelper.idlist, activitytypelist: myactivityhelper.activitytypelist,
                            subtypelist: myactivityhelper.subtypelist, difficultylist: myactivityhelper.difficultylist,
                            smgtaglist: myactivityhelper.smgtaglist, districtlist: new List<string>(),
                            municipalitylist: new List<string>(), tourismvereinlist: myactivityhelper.tourismvereinlist,
                            regionlist: myactivityhelper.regionlist, arealist: myactivityhelper.arealist,
                            distance: myactivityhelper.distance, distancemin: myactivityhelper.distancemin,
                            distancemax: myactivityhelper.distancemax, duration: myactivityhelper.duration,
                            durationmin: myactivityhelper.durationmin, durationmax: myactivityhelper.durationmax,
                            altitude: myactivityhelper.altitude, altitudemin: myactivityhelper.altitudemin,
                            altitudemax: myactivityhelper.altitudemax, highlight: myactivityhelper.highlight,
                            activefilter: myactivityhelper.active, smgactivefilter: myactivityhelper.smgactive,
                            searchfilter: null, language: language, lastchange: null, languagelist: new List<string>(),
                            filterClosedData: FilterClosedData)
                        .OrderByRaw(orderby)
                        .GeoSearchFilterAndOrderby(geosearchresult);

                // Get paginated data
                return await query.GetAsync<ResultReduced>();
            });
        }

        #endregion

        #region GastronomyController

        /// <summary>
        /// GET Gastronomy Reduced List
        /// </summary>
        /// <param name="language">Localization Language, (default:'en')</param>
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
        /// <returns>Collection of GastronomyReduced Objects</returns>        
        [ProducesResponseType(typeof(IEnumerable<GastronomyReduced>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("GastronomyReduced")]
        public async Task<IActionResult> GetGastronomyListReduced(
            string? language = "en",
            string? locfilter = null,
            string? dishcodefilter = null,
            string? ceremonycodefilter = null,
            string? categorycodefilter = null,
            string? facilitycodefilter = null,
            string? cuisinecodefilter = null,
            string? odhtagfilter = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            return await GetGastronomyReduced(language, locfilter, dishcodefilter, ceremonycodefilter, categorycodefilter, facilitycodefilter, cuisinecodefilter, active?.Value, odhactive?.Value, odhtagfilter, geosearchresult, cancellationToken);
        }

        private Task<IActionResult> GetGastronomyReduced(
            string? language, string? locfilter, string? dishcodefilter,
            string? ceremonycodefilter, string? categorycodefilter, string? facilitycodefilter,
            string? cuisinecodefilter, bool? active, bool? smgactive, string? smgtagfilter,
            PGGeoSearchResult geosearchresult, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                GastronomyHelper mygastronomyhelper = await GastronomyHelper.CreateAsync(
                    QueryFactory, idfilter: null, locfilter: locfilter, categorycodefilter: categorycodefilter,
                    dishcodefilter: dishcodefilter, ceremonycodefilter: ceremonycodefilter, facilitycodefilter: facilitycodefilter,
                    cuisinecodefilter: cuisinecodefilter, activefilter: active, smgactivefilter: smgactive, smgtags: smgtagfilter,
                    lastchange: null, cancellationToken);

                string select = $"data#>>'\\{{Id\\}}' as Id, data#>>'\\{{Detail,{language},Title\\}}' as Name";
                string orderby = "data#>>'\\{Shortname\\}' ASC";

                var query =
                    QueryFactory.Query()
                        .SelectRaw(select)
                        .From("gastronomies")
                        .GastronomyWhereExpression(
                           idlist: mygastronomyhelper.idlist, dishcodeslist: mygastronomyhelper.dishcodesids, ceremonycodeslist: mygastronomyhelper.ceremonycodesids,
                            categorycodeslist: mygastronomyhelper.categorycodesids, facilitycodeslist: mygastronomyhelper.facilitycodesids,
                            smgtaglist: mygastronomyhelper.smgtaglist, districtlist: mygastronomyhelper.districtlist,
                            municipalitylist: mygastronomyhelper.municipalitylist, tourismvereinlist: mygastronomyhelper.tourismvereinlist,
                            regionlist: mygastronomyhelper.regionlist, activefilter: mygastronomyhelper.active,
                            smgactivefilter: mygastronomyhelper.smgactive,
                            searchfilter: null, language: language, lastchange: null, languagelist: new List<string>(),
                            filterClosedData: FilterClosedData
                        )
                        .OrderByRaw(orderby)
                        .GeoSearchFilterAndOrderby(geosearchresult);

                // Get paginated data
                return await query.GetAsync<ResultReduced>();
            });
        }

        #endregion

        #region ODHTagController

        /// <summary>
        /// GET ODHTag List REDUCED
        /// </summary>
        /// <param name="validforentity"></param>
        /// <param name="localizationlanguage"></param>
        /// <returns></returns>
        //[SwaggerResponse(HttpStatusCode.OK, "Array of SmgTagReduced Objects", typeof(IEnumerable<SmgTagReduced>))]
        [HttpGet, Route("ODHTagReduced")]
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        public async Task<IActionResult> GetODHTagsReduced(string localizationlanguage, string validforentity = "", CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(validforentity) && !string.IsNullOrEmpty(localizationlanguage))
            {
                return await GetReducedLocalized(localizationlanguage, cancellationToken);
            }
            //Fall 7 GET auf GetReducedFilteredLocalized
            else if (!string.IsNullOrEmpty(validforentity) && !string.IsNullOrEmpty(localizationlanguage))
            {
                return await GetReducedFilteredLocalized(localizationlanguage, validforentity, cancellationToken);
            }
            else
            {
                return StatusCode(StatusCodes.Status501NotImplemented, new { error = "not implemented" });
            }
        }

        /// <summary>
        /// GET Complete Reduced SMGTag List
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>Collection Localized Reduced SMGTag Object</returns>
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        [HttpGet, Route("ReducedAsync/{language}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task<IActionResult> GetReducedLocalized(string language, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                string select = $"data#>>'\\{{Id\\}}' as Id, data#>>'{{TagName,{language.ToLower()}}}' as Name";
                string where = $"data#>>'\\{{TagName,{language.ToLower()}\\}}' NOT LIKE ''";

                var data =
                    QueryFactory.Query("smgtags")
                        .SelectRaw(select)
                        .WhereRaw(where)
                        .When(FilterClosedData, q => q.FilterClosedData());

                return await data.GetAsync<ResultReduced>();
            });
        }

        /// <summary>
        /// GET Filtered Reduced SMGTag List by smgtagtype
        /// </summary>
        /// <param name="language">Language</param>
        /// <param name="smgtagtype">SMGTag Type</param>
        /// <returns>Collection Localized Reduced SMGTag Object</returns>
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        [HttpGet, Route("Reduced/{language}/{smgtagtype}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task<IActionResult> GetReducedFilteredLocalized(
            string language, string smgtagtype, CancellationToken cancellationToken)
        {
            var smgtagtypelist = smgtagtype.Split(',', StringSplitOptions.RemoveEmptyEntries);

            return DoAsyncReturn(async () =>
            {
                string select = $"data#>>'\\{{Id\\}}' as Id, data#>>'\\{{TagName,{language.ToLower()}\\}}' as Name";
                string where = $"data#>>'\\{{TagName,{language.ToLower()}\\}}' NOT LIKE ''";

                var data =
                    QueryFactory.Query("smgtags")
                        .SelectRaw(select)
                        .WhereRaw(where)
                        .WhereInJsonb(
                            smgtagtypelist,
                            smgtag => new { ValidForEntity = new[] { smgtag.ToLower() } }
                        )
                        .When(FilterClosedData, q => q.FilterClosedData());

                return await data.GetAsync<ResultReduced>();
            });
        }



        #endregion

        #region ODHActivityPoiController

        ////Reduced GETTER

        /// <summary>
        /// GET ODHActivityPoi List Reduced
        /// </summary>
        /// <param name="language">Localization Language, (default:'en')</param>
        /// <param name="type">Type of the ODHActivityPoi ('null' = Filter disabled, possible values: BITMASK: 1 = Wellness, 2 = Winter, 4 = Summer, 8 = Culture, 16 = Other, 32 = Gastronomy), (default: 63 == ALL), REFERENCE TO: GET /api/ODHActivityPoiTypes </param>
        /// <param name="subtype">Subtype of the ODHActivityPoi ('null' = Filter disabled, BITMASK Filter, available SubTypes depends on the selected Maintype reference to ODHActivityPoiTypes)</param>
        /// <param name="poitype">Additional Type of the ODHActivityPoi ('null' = Filter disabled, BITMASK Filter, available SubTypes depends on the selected Maintype, SubType reference to ODHActivityPoiTypes)</param>
        /// <param name="locfilter">Locfilter (Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMASSOCIATIONID = (Filter by Tourismassociation), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction), 'null' = No Filter), (default:'null')</param>
        /// <param name="areafilter">AreaFilter (Alternate Locfilter, can be combined with locfilter) (Separator ',' possible values: reg + REGIONID = (Filter by Region), tvs + TOURISMASSOCIATIONID = (Filter by Tourismassociation), skr + SKIREGIONID = (Filter by Skiregion), ska + SKIAREAID = (Filter by Skiarea), are + AREAID = (Filter by LTS Area), 'null' = No Filter), (default:'null')</param>
        /// <param name="highlight">Hightlight Filter (possible values: 'false' = only ODHActivityPoi with Highlight false, 'true' = only ODHActivityPoi with Highlight true), (default:'null')</param>
        /// <param name="source">Source Filter (possible Values: 'null' Displays all ODHActivityPoi, 'None', 'ActivityData', 'PoiData', 'GastronomicData', 'MuseumData', 'Magnolia', 'Content', 'SuedtirolWein', 'Archapp' (default:'null')</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <param name="odhtagfilter">ODH Taglist Filter (refers to Array SmgTags) (String, Separator ',' more Tags possible, available Tags reference to 'api/ODHTag?validforentity=smgpoi'), (default:'null')</param>        
        /// <param name="active">Active ODHActivityPoi Filter (possible Values: 'true' only active ODHActivityPoi, 'false' only not active ODHActivityPoi, (default:'null')</param>        
        /// <param name="odhactive">ODH Active (Published) ODHActivityPoi Filter (Refers to field SmgActive) (possible Values: 'true' only published ODHActivityPoi, 'false' only not published ODHActivityPoi, (default:'null')</param>        
        /// <returns>Collection of ActivityPoiReduced Objects</returns>        
        [ProducesResponseType(typeof(IEnumerable<ActivityPoiReduced>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("ODHActivityPoiReduced")]
        public async Task<IActionResult> GetODHActivityPoiListReduced(
            string? language = "en",
            string? type = "63",
            string? subtype = null,
            string? poitype = null,
            string? locfilter = null,
            //string langfilter = null,
            string? areafilter = null,
            LegacyBool highlight = null!,
            string? source = null,
            string odhtagfilter = null!,
            LegacyBool odhactive = null!,
            LegacyBool active = null!,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            return await GetODHActivityPoiReduced(language, type, subtype, poitype, locfilter, areafilter, highlight?.Value, active?.Value, odhactive?.Value, source, odhtagfilter, geosearchresult, cancellationToken);
        }

        /// <summary>
        /// GET Reduced ODHActivity Poi List
        /// </summary>
        /// <param name="language">Localization Language</param>
        /// <param name="poitype">Type of the Poi (possible values: STRINGS: 'Ärtze, Apotheken','Geschäfte und Dienstleister','Kultur und Sehenswürdigkeiten','Nachtleben und Unterhaltung','Öffentliche Einrichtungen','Sport und Freizeit','Verkehr und Transport' : BITMASK also possible: 'Ärtze, Apotheken = 1','Geschäfte und Dienstleister = 2','Kultur und Sehenswürdigkeiten = 4','Nachtleben und Unterhaltung = 8','Öffentliche Einrichtungen = 16','Sport und Freizeit = 32','Verkehr und Transport = 64')</param>
        /// <param name="subtypefilter">Subtype of the Poi ('null' = Filter disabled, available Subtypes depends on the activitytype BITMASK)</param>        
        /// <param name="locfilter">Locfilter (Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), 'null' = No Filter)</param>
        /// <param name="areafilter">AreaFilter (Separator ',' IDList of AreaIDs separated by ',', 'null' : Filter disabled)</param>
        /// <param name="highlightfilter">Highlight Filter (Show only Highlights possible values: 'true' : show only Highlight Pois, 'null' Filter disabled)</param>
        /// <param name="active">Active Filter (possible Values: 'null' Displays all Pois, 'true' only Active Pois, 'false' only Disabled Pois</param>
        /// <param name="smgactive">SMGActive Filter (possible Values: 'null' Displays all Pois, 'true' only SMG Active Pois, 'false' only SMG Disabled Pois</param>
        /// <param name="smgtags">SMGTag Filter (String, Separator ',' more SMGTags possible, 'null' = No Filter)</param>
        /// <returns>Collection of Reduced Poi Objects</returns>
        private Task<IActionResult> GetODHActivityPoiReduced(
            string? language, string? type, string? subtype, string? poitype, string? locfilter,
            string? areafilter, bool? highlightfilter, bool? active, bool? smgactive, string? source,
            string? smgtags, PGGeoSearchResult geosearchresult, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                ODHActivityPoiHelper helper = await ODHActivityPoiHelper.CreateAsync(
                    QueryFactory, type, subtype, poitype, null, locfilter, areafilter,
                    language, source, highlightfilter, active, smgactive, smgtags, null, cancellationToken);

                string select = $"data#>'\\{{Id\\}}' as Id, data#>>'\\{{Detail,{language},Title}}' as Name";
                string orderby = "data#>>'\\{Shortname\\}' ASC";

                var query =
                    QueryFactory.Query()
                        .SelectRaw(select)
                        .From("smgpois")
                        .ODHActivityPoiWhereExpression(
                            idlist: helper.idlist, typelist: helper.typelist, subtypelist: helper.subtypelist, poitypelist: helper.poitypelist,
                            smgtaglist: helper.smgtaglist, districtlist: helper.districtlist, municipalitylist: helper.municipalitylist,
                            tourismvereinlist: helper.tourismvereinlist, regionlist: helper.regionlist,
                            arealist: helper.arealist, highlight: helper.highlight, activefilter: helper.active,
                            smgactivefilter: helper.smgactive, sourcelist: helper.sourcelist, languagelist: helper.languagelist,
                            searchfilter: null, language: language, lastchange: null, filterClosedData: FilterClosedData
                        )
                        .OrderByRaw(orderby)
                        .GeoSearchFilterAndOrderby(geosearchresult);

                // Get paginated data
                return await query.GetAsync<ResultReduced>();
            });
        }


        #endregion

        #region EventController

        /// <summary>
        /// GET Event List Reduced
        /// </summary>
        /// <param name="language">Localization Language, (default:'en')</param>
        /// <param name="locfilter">Locfilter (Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction), 'null' = No Filter), (default:'null')</param>        
        /// <param name="rancfilter">Rancfilter (Ranc 0-5 possible)</param>
        /// <param name="typefilter">Typefilter (Type of Event: not used yet)</param>
        /// <param name="topicfilter">Topic ID Filter (Filter by Topic ID) BITMASK</param>
        /// <param name="orgfilter">Organization Filter (Filter by Organizer RID)</param>
        /// <param name="odhtagfilter">ODH Taglist Filter (refers to Array SmgTags) (String, Separator ',' more Tags possible, available Tags reference to 'api/ODHTag?validforentity=event'), (default:'null')</param>        
        /// <param name="active">Active Events Filter (possible Values: 'true' only Active Events, 'false' only Disabled Events, (default:'null')</param>
        /// <param name="odhactive">ODH Active (Published) Events Filter (Refers to field SmgActive) Events Filter (possible Values: 'true' only published Events, 'false' only not published Events, (default:'null')</param>                
        /// <param name="begindate">BeginDate of Events (Format: yyyy-MM-dd)</param>
        /// <param name="enddate">EndDate of Events (Format: yyyy-MM-dd)</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <returns>Collection of EventReduced Objects</returns>        
        [ProducesResponseType(typeof(IEnumerable<EventReduced>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("EventReduced")]
        public async Task<IActionResult> GetEventsReduced(
            string? language = "en",
            string? locfilter = null,
            string? rancfilter = null,
            string? typefilter = null,
            string? topicfilter = null,
            string? orgfilter = null,
            string? odhtagfilter = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? begindate = null,
            string? enddate = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            return await GetEventReduced(language, locfilter, rancfilter, typefilter, topicfilter, orgfilter, odhactive?.Value, active?.Value, begindate, enddate, odhtagfilter, geosearchresult, cancellationToken);
        }

        private Task<IActionResult> GetEventReduced(string? language, string? locfilter, string? rancfilter,
            string? typefilter, string? topicfilter, string? orgfilter, bool? smgactive, bool? active,
            string? begindate, string? enddate, string? smgtagfilter, PGGeoSearchResult geosearchresult, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                EventHelper helper = await EventHelper.CreateAsync(
                    QueryFactory, null, locfilter, rancfilter, typefilter, topicfilter, orgfilter, begindate,
                    enddate, active, smgactive, smgtagfilter, null, cancellationToken);
                
                string select = $"data#>>'\\{{Id\\}}' as Id, data#>>'\\{{Detail,{language},Title\\}}' as Name";
                string orderby = "data#>>'\\{Shortname\\}' ASC";

                var query =
                    QueryFactory.Query()
                        .SelectRaw(select)
                        .From("events")
                        .EventWhereExpression(
                            idlist: helper.idlist, topiclist: helper.topicrids, typelist: helper.typeidlist, ranclist: helper.rancidlist,
                            smgtaglist: helper.smgtaglist, districtlist: helper.districtlist, municipalitylist: helper.municipalitylist,
                            tourismvereinlist: helper.tourismvereinlist, regionlist: helper.regionlist,
                            orglist: helper.orgidlist, begindate: helper.begin, enddate: helper.end, activefilter: helper.active,
                            smgactivefilter: helper.smgactive, languagelist: new List<string>(),
                            searchfilter: null, language: language, lastchange: null, filterClosedData: FilterClosedData
                        )
                        .OrderByRaw(orderby)
                        .GeoSearchFilterAndOrderby(geosearchresult);

                // Get paginated data
                return await query.GetAsync<ResultReduced>();
            });
        }


        #endregion

        #region ArticleController

        /// <summary>
        /// GET Article List Reduced
        /// </summary>
        /// <param name="language">Localization Language, (default:'en')</param>
        /// <param name="articletype">Type of the Article ('null' = Filter disabled, possible values: BITMASK values: 1 = basearticle, 2 = book article, 4 = contentarticle, 8 = eventarticle, 16 = pressarticle, 32 = recipe, 64 = touroperator , 128 = b2b), (also possible for compatibily reasons: basisartikel, buchtippartikel, contentartikel, veranstaltungsartikel, presseartikel, rezeptartikel, reiseveranstalter, b2bartikel ) (default:'255' == ALL), REFERENCE TO: GET /api/ArticleTypes</param>
        /// <param name="articlesubtype">Sub Type of the Article (depends on the Maintype of the Article 'null' = Filter disabled)</param>
        /// <param name="odhtagfilter">ODH Taglist Filter (refers to Array SmgTags) (String, Separator ',' more Tags possible, available Tags reference to 'api/ODHTag?validforentity=article'), (default:'null')</param>                
        /// <param name="active">Active Articles Filter (possible Values: 'true' only Active Articles, 'false' only Disabled Articles</param>
        /// <param name="odhactive">ODH Active (Published) Activities Filter (Refers to field SmgActive) Article Filter (possible Values: 'true' only published Article, 'false' only not published Articles, (default:'null')</param>        
        /// <returns>Collection of Article Objects</returns>        
        [ProducesResponseType(typeof(IEnumerable<ArticleReduced>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("ArticleReduced")]
        public Task<IActionResult> GetArticleReducedList(
            string? language = "en",
            string? articletype = "255",
            string? articlesubtype = null,
            string? odhtagfilter = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            CancellationToken cancellationToken = default
           )
        {
            return GetArticleReduced(language, articletype, articlesubtype, active?.Value, odhactive?.Value, odhtagfilter, cancellationToken);
        }

        private Task<IActionResult> GetArticleReduced(string? language, string? articletype, string? articlesubtype, bool? active, bool? smgactive, string? smgtags, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                ArticleHelper helper = ArticleHelper.Create(
                    articletype, articlesubtype, null, language, null, active, smgactive, smgtags, null);

                string select = $"data#>>'\\{{Id\\}}' as Id, data#>>'\\{{Detail,{language},Title\\}}' as Name"; 
                string orderby = "data#>>'\\{Shortname\\}' ASC";

                var query =
                    QueryFactory.Query()
                        .SelectRaw(select)
                        .From("articles")
                        .ArticleWhereExpression(
                            idlist: helper.idlist, typelist: helper.typelist, subtypelist: helper.subtypelist, languagelist: helper.languagelist,
                            smgtaglist: helper.smgtaglist, highlight: helper.highlight, activefilter: helper.active,
                            smgactivefilter: helper.smgactive, searchfilter: null, language: language, lastchange: null, filterClosedData: FilterClosedData
                        )
                        .OrderByRaw(orderby);

                // Get paginated data
                return await query.GetAsync<ResultReduced>();
            });
        }


        #endregion

        #region WebcamInfoController

        /// <summary>
        /// GET Webcam Reduced List
        /// </summary>
        /// <param name="language">Localization Language, (default:'en')</param>
        /// <param name="source">Source Filter(String, ), (default:'null')</param>        
        /// <param name="active">Active Webcam Filter (possible Values: 'true' only Active Gastronomies, 'false' only Disabled Gastronomies</param>
        /// <param name="odhactive">ODH Active (refers to field SmgActive) (Published) Webcam Filter (possible Values: 'true' only published Webcam, 'false' only not published Webcam, (default:'null')</param>        
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in KM. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <returns>Collection of WebcamInfoReduced Objects</returns>        
        //[Authorize(Roles = "DataReader,ActivityReader,PoiReader,SmgPoiReader,GastroReader,AccoReader,PackageReader,ArticleReader,EventReader,CommonReader,WebcamReader")]
        [HttpGet, Route("WebcamInfoReduced")]
        public Task<IActionResult> GetWebcamListReduced(
            string? language = "en",
            string? source = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            string? updatefrom = null,
            CancellationToken cancellationToken = default
      )
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            return GetWebcamReduced(language, source, active?.Value, odhactive?.Value, updatefrom, geosearchresult, cancellationToken);
        }

        private Task<IActionResult> GetWebcamReduced(string? language, string? sourcefilter, bool? active, bool? smgactive, string? datefrom, PGGeoSearchResult geosearchresult, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                WebcamInfoHelper helper = WebcamInfoHelper.Create(sourcefilter, null, active, smgactive, datefrom);

                string select = $"data#>>'\\{{Id\\}}' as Id, data#>>'\\{{Webcamname,{language}\\}}' as Name";
                string orderby = "data#>>'\\{Shortname\\}' ASC";

                var query =
                    QueryFactory.Query()
                        .SelectRaw(select)
                        .From("webcams")
                        .WebCamInfoWhereExpression(
                            languagelist: new List<string>(), idlist: helper.idlist, sourcelist: helper.sourcelist, activefilter: helper.active,
                            smgactivefilter: helper.smgactive, searchfilter: null, language: language, lastchange: null, filterClosedData: FilterClosedData
                        )
                        .OrderByRaw(orderby);

                // Get paginated data
                return await query.GetAsync<ResultReduced>();
            });
        }

        #endregion

        #region AccommodationController

        #endregion

        #region Common

        ////Reduced GETTER

        /// <summary>
        /// GET MetaRegion Reduced List
        /// </summary>
        /// <param name="language">Localization Language, (default:'en')</param>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <returns>Collection of CommonReduced Objects</returns>    
        [ProducesResponseType(typeof(IEnumerable<CommonReduced>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("MetaRegionReduced")]
        public async Task<IActionResult> GetMetaRegionsReduced(
            string language = "en",
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            string ? searchfilter = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, null, null, null, null, null, cancellationToken);

            return await GetCommonReduced("metaregions", searchfilter, language, commonhelper, geosearchresult, cancellationToken);
        }

        /// <summary>
        /// GET ExperienceArea Reduced List
        /// </summary>
        /// <param name="language">Localization Language, (default:'en')</param>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="visibleinsearch">Filter only Elements flagged with visibleinsearch: (possible values: 'true','false'), (default:'false')</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <returns>Collection of ExperienceAreaName Objects</returns>        
        [ProducesResponseType(typeof(IEnumerable<CommonReduced>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("ExperienceAreaReduced")]
        public async Task<IActionResult> GetExperienceAreasReduced(
            string language = "en",
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            bool? visibleinsearch = null,
            string? searchfilter = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, visibleinsearch, null, null, null, null, cancellationToken);

            return await GetCommonReduced("experienceareas", searchfilter, language, commonhelper, geosearchresult, cancellationToken);
        }

        /// <summary>
        /// GET Region Reduced List
        /// </summary>
        /// <param name="language">Localization Language, (default:'en')</param>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <returns>Collection of CommonReduced Objects</returns>        
        [ProducesResponseType(typeof(IEnumerable<CommonReduced>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("api/RegionReduced")]
        public async Task<IActionResult> GetRegionsReduced(
            string language = "en",
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            //bool? visibleinsearch = null,
            string? searchfilter = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, null, null, null, null, null, cancellationToken);

            return await GetCommonReduced("regions", searchfilter, language, commonhelper, geosearchresult, cancellationToken);
        }

        /// <summary>
        /// GET TourismAssociation Reduced List
        /// </summary>
        /// <param name="language">Localization Language, (default:'en')</param>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <returns>Collection of CommonReduced Objects</returns>        
        [ProducesResponseType(typeof(IEnumerable<CommonReduced>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("TourismAssociationReduced")]
        public async Task<IActionResult> GetTourismvereinReduced(
            string language = "en",
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            //bool? visibleinsearch = null,
            string? searchfilter = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, null, null, null, null, null, cancellationToken);

            return await GetCommonReduced("tvs", searchfilter, language, commonhelper, geosearchresult, cancellationToken);
        }

        /// <summary>
        /// GET Municipality Reduced List
        /// </summary>
        /// <param name="language">Localization Language, (default:'en')</param>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="visibleinsearch">Filter only Elements flagged with visibleinsearch: (possible values: 'true','false'), (default:'false')</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <returns>Collection of CommonReduced Objects</returns>        
        [ProducesResponseType(typeof(IEnumerable<CommonReduced>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("MunicipalityReduced")]
        public async Task<IActionResult> GetMunicipalityReduced(
            string language = "en",
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            bool? visibleinsearch = null,
            string? searchfilter = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, visibleinsearch, null, null, null, null, cancellationToken);

            return await GetCommonReduced("municipalities", searchfilter, language, commonhelper, geosearchresult, cancellationToken);
        }

        /// <summary>
        /// GET District Reduced List
        /// </summary>
        /// <param name="language">Localization Language, (default:'en')</param>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="visibleinsearch">Filter only Elements flagged with visibleinsearch: (possible values: 'true','false'), (default:'false')</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <returns>Collection of CommonReduced Objects</returns>        
        [ProducesResponseType(typeof(IEnumerable<CommonReduced>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("DistrictReduced")]
        public async Task<IActionResult> GetDistrictReduced(
            string language = "en",
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            bool? visibleinsearch = null,
            string? searchfilter = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, visibleinsearch, null, null, null, null, cancellationToken);

            return await GetCommonReduced("districts", searchfilter, language, commonhelper, geosearchresult, cancellationToken);
        }

        /// <summary>
        /// GET SkiRegion Reduced List
        /// </summary>
        /// <param name="language">Localization Language, (default:'en')</param>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <returns>Collection of CommonReduced Objects</returns>        
        [ProducesResponseType(typeof(IEnumerable<CommonReduced>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("SkiRegionReduced")]
        public async Task<IActionResult> GetSkiRegionReduced(
            string language = "en",
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            //bool? visibleinsearch = null,
            string? searchfilter = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, null, null, null, null, null, cancellationToken);

            return await GetCommonReduced("skiregions", searchfilter, language, commonhelper, geosearchresult, cancellationToken);
        }

        /// <summary>
        /// GET SkiArea Reduced List
        /// </summary>
        /// <param name="language">Localization Language, (default:'en')</param>
        /// <param name="elements">Elements to retrieve (0 = Get All)</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <returns>Collection of CommonReduced Objects</returns>        
        [ProducesResponseType(typeof(IEnumerable<CommonReduced>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("SkiAreaReduced")]
        public async Task<IActionResult> GetSkiAreaReduced(
            string language = "en",
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            //bool? visibleinsearch = null,
            string? searchfilter = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, null, null, null, null, null, null, null, cancellationToken);

            return await GetCommonReduced("skiareas", searchfilter, language, commonhelper, geosearchresult, cancellationToken);
        }



        private Task<IActionResult> GetCommonReduced(string tablename, string? searchfilter, string? language, CommonHelper commonhelper, PGGeoSearchResult geosearchresult, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                string select = $"data#>>'\\{{Id\\}}' as Id, data#>>'\\{{Detail,{language},Title\\}}' as Name"; 
                string orderby = "data#>>'\\{Shortname\\}' ASC";

                var query =
                    QueryFactory.Query()
                        .SelectRaw(select)
                        .From(tablename)
                        .CommonWhereExpression(languagelist: new List<string>(), lastchange: commonhelper.lastchange, visibleinsearch: commonhelper.visibleinsearch,
                                               searchfilter: searchfilter, language: language, filterClosedData: FilterClosedData)
                        .OrderByRaw(orderby)
                        .GeoSearchFilterAndOrderby(geosearchresult);

                // Get paginated data
                return await query.GetAsync<ResultReduced>();
            });
        }     

        #endregion
    }

    class ResultReduced
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }
}
