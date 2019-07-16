using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Helper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql;
using Swashbuckle.AspNetCore.Swagger;

namespace OdhApiCore.Controllers
{
    //[Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class ActivityController : OdhController
    {
        public ActivityController(ISettings settings) : base(settings)
        {
        }


        #region SWAGGER Exposed API

        //Standard GETTER

        /// <summary>
        /// GET Activity List
        /// </summary>
        /// <param name="pagenumber">Pagenumber, (default:1)</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="activitytype">Type of the Activity ('null' = Filter disabled, possible values: BITMASK: 'Mountains = 1','Cycling = 2','Local tours = 4','Horses = 8','Hiking = 16','Running and fitness = 32','Cross-country ski-track = 64','Tobbogan run = 128','Slopes = 256','Lifts = 512'), (default:'1023' == ALL), REFERENCE TO: GET /api/ActivityTypes </param>
        /// <param name="subtype">Subtype of the Activity (BITMASK Filter = available SubTypes depends on the selected Activity Type), (default:'null')</param>
        /// <param name="idlist">IDFilter (Separator ',' List of Activity IDs), (default:'null')</param>
        /// <param name="locfilter">Locfilter (Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction)), (default:'null')</param>
        /// <param name="areafilter">AreaFilter (Separator ',' IDList of AreaIDs separated by ','), (default:'null')</param>
        /// <param name="distancefilter">Distance Range Filter (Separator ',' example Value: 15,40 Distance from 15 up to 40 Km), (default:'null')</param>
        /// <param name="altitudefilter">Altitude Range Filter (Separator ',' example Value: 500,1000 Altitude from 500 up to 1000 metres), (default:'null')</param>
        /// <param name="durationfilter">Duration Range Filter (Separator ',' example Value: 1,3 Duration from 1 to 3 hours), (default:'null')</param>
        /// <param name="highlight">Hightlight Filter (possible values: 'false' = only Activities with Highlight false, 'true' = only Activities with Highlight true), (default:'null')</param>
        /// <param name="difficultyfilter">Difficulty Filter (possible values: '1' = easy, '2' = medium, '3' = difficult), (default:'null')</param>      
        /// <param name="odhtagfilter">Taglist Filter (String, Separator ',' more Tags possible, available Tags reference to 'api/ODHTag?validforentity=activity'), (default:'null')</param>        
        /// <param name="active">Active Activities Filter (possible Values: 'true' only Active Activities, 'false' only Disabled Activities</param>
        /// <param name="odhactive"> odhactive (Published) Activities Filter (possible Values: 'true' only published Activities, 'false' only not published Activities, (default:'null')</param>        
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <returns>Collection of Activity Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<GBLTSActivity>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("api/Activity")]
        public async Task<IActionResult> GetActivityList(
            int pagenumber = 1,
            int pagesize = 10,
            string? activitytype = "1023",
            string? subtype = null,
            string? idlist = null,
            string? locfilter = null,
            string? areafilter = null,
            string? distancefilter = null,
            string? altitudefilter = null,
            string? durationfilter = null,
            string? highlight = null,
            string? difficultyfilter = null,
            string? odhtagfilter = null,
            string? active = null,
            string? odhactive = null,
            string? seed = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            //FAll 1 keine Filter
            if (subtype == null && idlist == null && locfilter == null && areafilter == null && distancefilter == null && altitudefilter == null && durationfilter == null && highlight == null && odhtagfilter == null && odhactive == null && active == null)
                return await GetPaged(activitytype, pagenumber, pagesize, seed, geosearchresult);
            else
                return await GetFiltered(pagenumber, pagesize, activitytype, subtype, idlist, locfilter, areafilter, distancefilter, altitudefilter, durationfilter, highlight, difficultyfilter, active, odhactive, odhtagfilter, seed, geosearchresult);
        }

        /// <summary>
        /// GET Activity Single 
        /// </summary>
        /// <param name="id">ID of the Activity</param>
        /// <returns>Activity Object</returns>
        //[SwaggerResponse(HttpStatusCode.OK, "Activity Object", typeof(GBLTSActivity))]
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("api/Activity/{id}")]
        public async Task<IActionResult> GetActivitySingle(string id)
        {
            return await GetSingle(id);
        }


        //Localized GETTER

        /// <summary>
        /// GET Activity List Localized
        /// </summary>
        /// <param name="language">Localization Language, (default:'en')</param>
        /// <param name="pagenumber">Pagenumber, (default:1)</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="activitytype">Type of the Activity ('null' = Filter disabled, possible values: BITMASK: 'Mountains = 1','Cycling = 2','Local tours = 4','Horses = 8','Hiking = 16','Running and fitness = 32','Cross-country ski-track = 64','Tobbogan run = 128','Slopes = 256','Lifts = 512'), (default:'1023' == ALL), REFERENCE TO: GET /api/ActivityTypes </param>
        /// <param name="subtype">Subtype of the Activity (BITMASK Filter = available SubTypes depends on the selected Activity Type), (default:'null')</param>
        /// <param name="idlist">IDFilter (Separator ',' List of Activity IDs), (default:'null')</param>
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
        /// <returns>Collection of ActivityLocalized Objects</returns>        
        //[SwaggerResponse(HttpStatusCode.OK, "Array of ActivityLocalized Objects", typeof(IEnumerable<GBLTSActivityPoiLocalized>))]
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("api/ActivityLocalized")]
        public async Task<IActionResult> GetActivityFilteredLocalized(
            string language = "en",
            int pagenumber = 1,
            int pagesize = 10,
            string? activitytype = "1023",
            string? subtype = null,
            string? idlist = null,
            string? locfilter = null,
            string? areafilter = null,
            string? distancefilter = null,
            string? altitudefilter = null,
            string? durationfilter = null,
            string? highlight = null,
            string? difficultyfilter = null,
            string? odhtagfilter = null,
            string? active = null,
            string? odhactive = null,
            string? seed = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            //FAll 1 keine Filter
            if (subtype ==  null && idlist == null && locfilter == null && areafilter == null && distancefilter == null && altitudefilter == null && durationfilter == null && highlight == null && odhtagfilter == null && odhactive == null && active == null)
                return await GetPagedLocalized(language, activitytype, pagenumber, pagesize, seed, geosearchresult);
            else
                return await GetFilteredLocalized(language, pagenumber, pagesize, activitytype, subtype, idlist, locfilter, areafilter, distancefilter, altitudefilter, durationfilter, highlight, difficultyfilter, active, odhactive, odhtagfilter, seed, geosearchresult);
        }


        /// <summary>
        /// GET Activity Single Localized
        /// </summary>
        /// <param name="language">Localization Language, (default:'en')</param>
        /// <param name="id">ID of the Activity</param>        
        /// <returns>ActivityLocalized Object</returns>
        //[SwaggerResponse(HttpStatusCode.OK, "ActivityLocalized Object", typeof(GBLTSActivityPoiLocalized))]
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("api/ActivityLocalized/{id}")]
        public async Task<IActionResult> GetActivitySingleLocalized(string id, string language = "en")
        {
            return await GetSingleLocalized(language, id);
        }

        //REDUCED

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
        //[SwaggerResponse(HttpStatusCode.OK, "Array of Activity Reduced Objects", typeof(IEnumerable<ActivityPoiReduced>))]
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("api/ActivityReduced")]
        public async Task<IActionResult> GetActivityReduced(
            string? language = "en",
            string? activitytype = "1023",
            string? subtype = null,
            string? locfilter = null,
            string? areafilter = null,
            string? distancefilter = null,
            string? altitudefilter = null,
            string? durationfilter = null,
            string? highlight = null,
            string? difficultyfilter = null,
            string? odhtagfilter = null,
            string? active = null,
            string? odhactive = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            return await GetReduced(language, activitytype, subtype, locfilter, areafilter, distancefilter, altitudefilter, durationfilter, highlight, difficultyfilter, active, odhactive, odhtagfilter, geosearchresult);
        }

        //Special

        /// <summary>
        /// GET Activity Types List
        /// </summary>
        /// <returns>Collection of ActivityTypes Object</returns>                
        //[CacheOutputUntilToday(23, 59)]
        //[SwaggerResponse(HttpStatusCode.OK, "Array of ActivityType Objects", typeof(IEnumerable<ActivityTypes>))]
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("api/ActivityTypes")]
        public async Task<IActionResult> GetAllActivityTypesListAsync()
        {
            return await GetActivityTypesListAsync();
        }

        /// <summary>
        /// GET Activity Changed List by Date
        /// </summary>
        /// <param name="pagenumber">Pagenumber, (default:1)</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="updatefrom">Date from Format (yyyy-MM-dd) (all GBActivityPoi with LastChange >= datefrom are passed), (default: DateTime.Now - 1 Day)</param>
        /// <returns>Collection of PoiBaseInfos Objects</returns>
        //[SwaggerResponse(HttpStatusCode.OK, "Array of PoiBaseInfos Objects", typeof(IEnumerable<GBLTSActivity>))]
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("api/ActivityChanged")]
        public async Task<IActionResult> GetAllActivityChanged(
            int pagenumber = 1,
            int pagesize = 10,
            string? seed = null,
            string? updatefrom = null
            )
        {
            updatefrom = updatefrom ?? String.Format("{0:yyyy-MM-dd}", DateTime.Now.AddDays(-1));

            return await GetLastChanged(pagenumber, pagesize, updatefrom, seed);
        }

        #endregion

        #region GETTER

        /// <summary>
        /// GET Full Activities List  (max 1024)
        /// </summary>
        /// <param name="activitytype">Type of the Activity (possible values: STRINGS: 'Berg','Radfahren','Stadtrundgang','Pferdesport','Wandern','Laufen und Fitness','Loipen','Rodelbahnen','Piste','Aufstiegsanlagen' : BITMASK also possible: 'Berg = 1','Radfahren = 2','Stadtrundgang = 4','Pferdesport = 8','Wandern = 16','Laufen und Fitness = 32','Loipen = 64','Rodelbahnen = 128,'Piste = 256,'Aufstiegsanlagen = 512) </param>
        /// <param name="elements">Elements to retrieve (max. 1024)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting</param>
        /// <returns>Collection of Activity Objects</returns>        
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("All/{activitytype}/{elements}/{seed?}")]
        public Task<IActionResult> GetAll(string activitytype, int elements, string? seed)
        {
            return DoAsync(async conn =>
            {
                ActivityHelper myactivityhelper = await ActivityHelper.CreateAsync(conn, activitytype, null, null, null, null, null, null, null, null, null, null, null, null);

                string select = "*";
                string orderby = "";

                var where = PostgresSQLWhereBuilder.CreateActivityWhereExpression(myactivityhelper.idlist, myactivityhelper.activitytypelist, myactivityhelper.subtypelist, myactivityhelper.difficultylist, myactivityhelper.smgtaglist, new List<string>(), new List<string>(), myactivityhelper.tourismvereinlist, myactivityhelper.regionlist, myactivityhelper.arealist, myactivityhelper.distance, myactivityhelper.distancemin, myactivityhelper.distancemax, myactivityhelper.duration, myactivityhelper.durationmin, myactivityhelper.durationmax, myactivityhelper.altitude, myactivityhelper.altitudemin, myactivityhelper.altitudemax, myactivityhelper.highlight, myactivityhelper.active, myactivityhelper.smgactive);
                string? myseed = PostgresSQLOrderByBuilder.BuildSeedOrderBy(ref orderby, seed, "data ->>'Shortname' ASC");

                var myresult = await PostgresSQLHelper.SelectFromTableDataAsStringParametrizedAsync(conn, "activities", select, where.Item1, where.Item2, orderby, elements, null);                

                return "[" + String.Join(",", myresult) + "]";
            });  
        }

        /// <summary>
        /// GET Paged Activities List
        /// </summary>
        /// <param name="activitytype">Type of the Activity (possible values: STRINGS: 'Berg','Radfahren','Stadtrundgang','Pferdesport','Wandern','Laufen und Fitness','Loipen','Rodelbahnen','Piste','Aufstiegsanlagen' : BITMASK also possible: 'Berg = 1','Radfahren = 2','Stadtrundgang = 4','Pferdesport = 8','Wandern = 16','Laufen und Fitness = 32','Loipen = 64','Rodelbahnen = 128,'Piste = 256,'Aufstiegsanlagen = 512) </param>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting</param>
        /// <returns>Result Object with Collection of Activity Objects</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("Paged/{activitytype}/{pagenumber}/{pagesize}/{seed?}")]
        public Task<IActionResult> GetPaged(string? activitytype, int pagenumber, int pagesize, string? seed, PGGeoSearchResult geosearchresult)
        {
            return DoAsync(async conn =>
            {
                ActivityHelper myactivityhelper = await ActivityHelper.CreateAsync(conn, activitytype, null, null, null, null, null, null, null, null, null, null, null, null);

                string select = "*";
                string orderby = "";

                var where = PostgresSQLWhereBuilder.CreateActivityWhereExpression(myactivityhelper.idlist, myactivityhelper.activitytypelist, myactivityhelper.subtypelist, myactivityhelper.difficultylist, myactivityhelper.smgtaglist, new List<string>(), new List<string>(), myactivityhelper.tourismvereinlist, myactivityhelper.regionlist, myactivityhelper.arealist, myactivityhelper.distance, myactivityhelper.distancemin, myactivityhelper.distancemax, myactivityhelper.duration, myactivityhelper.durationmin, myactivityhelper.durationmax, myactivityhelper.altitude, myactivityhelper.altitudemin, myactivityhelper.altitudemax, myactivityhelper.highlight, myactivityhelper.active, myactivityhelper.smgactive);
                string whereexpression = where.Item1;
                string? myseed = PostgresSQLOrderByBuilder.BuildSeedOrderBy(ref orderby, seed, "data ->>'Shortname' ASC");

                PostgresSQLHelper.ApplyGeoSearchWhereOrderby(ref whereexpression, ref orderby, geosearchresult);

                int pageskip = pagesize * (pagenumber - 1);

                //Normal
                var data = await PostgresSQLHelper.SelectFromTableDataAsStringParametrizedAsync(conn, "activities", select, whereexpression, where.Item2, orderby, pagesize, pageskip);
                var count = await PostgresSQLHelper.CountDataFromTableParametrizedAsync(conn, "activities", whereexpression, where.Item2);



                //With Materialized View
                //Stopwatch stopWatch = new Stopwatch();
                //stopWatch.Start();

                //var data = PostgresSQLHelper.SelectFromTableDataAsStringParametrized(conn, "activity_fast_lookup", "id,data", whereexpression, where.Item2, "shortname ASC", pagesize, pageskip);
                //var count = PostgresSQLHelper.CountDataFromTableParametrized(conn, "activity_fast_lookup", whereexpression, where.Item2);

                //stopWatch.Stop();
                //Debug.WriteLine(stopWatch.ElapsedMilliseconds);

                int totalcount = (int)count;
                int totalpages = PostgresSQLHelper.PGPagingHelper(totalcount, pagesize);

                return PostgresSQLHelper.GetResultJson(pagenumber, totalpages, totalcount, myseed, String.Join(",", data));
            });
        }

        /// <summary>
        /// GET Paged Filtered Activities List
        /// </summary>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page</param>
        /// <param name="activitytype">Type of the Activity (possible values: STRINGS: 'Berg','Radfahren','Stadtrundgang','Pferdesport','Wandern','Laufen und Fitness','Loipen','Rodelbahnen','Piste','Aufstiegsanlagen' - BITMASK also possible: 'Berg = 1','Radfahren = 2','Stadtrundgang = 4','Pferdesport = 8','Wandern = 16','Laufen und Fitness = 32','Loipen = 64','Rodelbahnen = 128,'Piste = 256,'Aufstiegsanlagen = 512) </param>
        /// <param name="subtypefilter">Subtype of the Activity ('null' = Filter disabled, BITMASK Filter = available SubTypes depends on the selected Activity Type)</param>
        /// <param name="idfilter">IDFilter (Separator ',' List of Activity IDs, 'null' = No Filter)</param>
        /// <param name="locfilter">Locfilter (Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction), 'null' = No Filter)</param>
        /// <param name="areafilter">AreaFilter (Separator ',' IDList of AreaIDs separated by ',', 'null' : Filter disabled)</param>
        /// <param name="distancefilter">Distance Range Filter (Separator ',' example Value: 15,40 Distance from 15 up to 40 Km) 'null' : disables Filter</param>
        /// <param name="altitudefilter">Altitude Range Filter (Separator ',' example Value: 500,1000 Altitude from 500 up to 1000 metres) 'null' : disables Filter</param>
        /// <param name="durationfilter">Duration Range Filter (Separator ',' example Value: 1,3 Duration from 1 to 3 hours) 'null' : disables Filter</param>
        /// <param name="highlightfilter">Hightlight Filter (possible values: 'null' = Filter disabled, 'false' = only Activities with Highlight false, 'true' = only Activities with Highlight true)</param>
        /// <param name="difficultyfilter">Difficulty Filter (possible values: 'null' = Filter disabled, '1' = easy, '2' = medium, '3' = difficult)</param>      
        /// <param name="active">Active Filter (possible Values: 'null' Displays all Activities, 'true' only Active Activities, 'false' only Disabled Activities</param>
        /// <param name="smgactive">SMGActive Filter (possible Values: 'null' Displays all Activities, 'true' only SMG Active Activities, 'false' only SMG Disabled Activities</param>
        /// <param name="smgtags">SMGTag Filter (String, Separator ',' more SMGTags possible, 'null' = No Filter, available SMGTags reference to 'api/SmgTag/ByMainEntity/Activity')</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting</param>
        /// <returns>Result Object with Collection of Activities Objects</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("Filtered/{pagenumber}/{pagesize}/{activitytype}/{subtypefilter}/{idfilter}/{locfilter}/{areafilter}/{distancefilter}/{altitudefilter}/{durationfilter}/{highlightfilter}/{difficultyfilter}/{active}/{smgactive}/{smgtags}/{seed?}")]
        public Task<IActionResult> GetFiltered(int pagenumber, int pagesize, string? activitytype, string? subtypefilter, string? idfilter, string? locfilter, string? areafilter, string? distancefilter, string? altitudefilter, string? durationfilter, string? highlightfilter, string? difficultyfilter, string? active, string? smgactive, string? smgtags, string? seed, PGGeoSearchResult geosearchresult)
        {
            return DoAsync(async conn =>
            {           
                ActivityHelper myactivityhelper = await ActivityHelper.CreateAsync(conn, activitytype, subtypefilter, idfilter, locfilter, areafilter, distancefilter, altitudefilter, durationfilter, highlightfilter, difficultyfilter, active, smgactive, smgtags);

                string select = "*";
                string orderby = "";

                var where = PostgresSQLWhereBuilder.CreateActivityWhereExpression(myactivityhelper.idlist, myactivityhelper.activitytypelist, myactivityhelper.subtypelist, myactivityhelper.difficultylist, myactivityhelper.smgtaglist, new List<string>(), new List<string>(), myactivityhelper.tourismvereinlist, myactivityhelper.regionlist, myactivityhelper.arealist, myactivityhelper.distance, myactivityhelper.distancemin, myactivityhelper.distancemax, myactivityhelper.duration, myactivityhelper.durationmin, myactivityhelper.durationmax, myactivityhelper.altitude, myactivityhelper.altitudemin, myactivityhelper.altitudemax, myactivityhelper.highlight, myactivityhelper.active, myactivityhelper.smgactive);
                string whereexpression = where.Item1;
                string? myseed = PostgresSQLOrderByBuilder.BuildSeedOrderBy(ref orderby, seed, "data ->>'Shortname' ASC");


                PostgresSQLHelper.ApplyGeoSearchWhereOrderby(ref whereexpression, ref orderby, geosearchresult);

                int pageskip = pagesize * (pagenumber - 1);

                var data = await PostgresSQLHelper.SelectFromTableDataAsStringParametrizedAsync(conn, "activities", select, whereexpression, where.Item2, orderby, pagesize, pageskip);
                var count = await PostgresSQLHelper.CountDataFromTableParametrizedAsync(conn, "activities", whereexpression, where.Item2);                

                int totalcount = (int)count;
                int totalpages = PostgresSQLHelper.PGPagingHelper(totalcount, pagesize);

                return PostgresSQLHelper.GetResultJson(pagenumber, totalpages, totalcount, myseed, String.Join(",", data));
            });
        }


        /// <summary>
        /// GET Single Activity
        /// </summary>
        /// <param name="id">ID of the Activity</param>
        /// <returns>Activity Object</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("Single/{id}")]
        public Task<IActionResult> GetSingle(string id)
        {
            return DoAsync(async conn =>
            {
                var where = PostgresSQLWhereBuilder.CreateIdListWhereExpression(id.ToUpper());
                var data = await PostgresSQLHelper.SelectFromTableDataAsStringParametrizedAsync(conn, "activities", "*", where.Item1, where.Item2, "", 0, null);

                return String.Join(",", data);
            });
        }

        #endregion

        #region LOCALIZED GETTER

        /// <summary>
        /// GET Full Activity List Localized  (max 1024)
        /// </summary>
        /// <param name="language">Localization Language</param>
        /// <param name="activitytype">Type of the Activity (possible values: STRINGS: 'Berg','Radfahren','Stadtrundgang','Pferdesport','Wandern','Laufen und Fitness','Loipen','Rodelbahnen','Piste','Aufstiegsanlagen' : BITMASK also possible: 'Berg = 1','Radfahren = 2','Stadtrundgang = 4','Pferdesport = 8','Wandern = 16','Laufen und Fitness = 32','Loipen = 64','Rodelbahnen = 128,'Piste = 256,'Aufstiegsanlagen = 512) </param>
        /// <param name="elements">Elements to retrieve (max. 1024)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting</param>
        /// <returns>Collection of Activity Object Localized</returns>
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet, Route("api/Activity/All/Localized/{language}/{activitytype}/{elements}/{seed?}")]
        public Task<IActionResult> GetLocalized(string language, string activitytype, int elements, string seed)
        {         
            return DoAsync(async conn =>
            {
                ActivityHelper myactivityhelper = await ActivityHelper.CreateAsync(conn, activitytype, null, null, null, null, null, null, null, null, null, null, null, null);

                string select = "*";
                string orderby = "";

                var where = PostgresSQLWhereBuilder.CreateActivityWhereExpression(myactivityhelper.idlist, myactivityhelper.activitytypelist, myactivityhelper.subtypelist, myactivityhelper.difficultylist, myactivityhelper.smgtaglist, new List<string>(), new List<string>(), myactivityhelper.tourismvereinlist, myactivityhelper.regionlist, myactivityhelper.arealist, myactivityhelper.distance, myactivityhelper.distancemin, myactivityhelper.distancemax, myactivityhelper.duration, myactivityhelper.durationmin, myactivityhelper.durationmax, myactivityhelper.altitude, myactivityhelper.altitudemin, myactivityhelper.altitudemax, myactivityhelper.highlight, myactivityhelper.active, myactivityhelper.smgactive);
                
                string? myseed = PostgresSQLOrderByBuilder.BuildSeedOrderBy(ref orderby, seed, "data ->>'Shortname' ASC");

                var myresult = await PostgresSQLHelper.SelectFromTableDataAsLocalizedObjectParametrizedAsync<GBLTSPoi, GBLTSActivityPoiLocalized>(conn, "activities", select, where.Item1, where.Item2, "", 0, null, language, PostgresSQLTransformer.TransformToGBLTSActivityPoiLocalized);

                return JsonConvert.SerializeObject(myresult);
            });
        }

        /// <summary>
        /// GET Paged Activities List Localized
        /// </summary>
        /// <param name="language">Localization Language</param>
        /// <param name="activitytype">Type of the Activity (possible values: STRINGS: 'Berg','Radfahren','Stadtrundgang','Pferdesport','Wandern','Laufen und Fitness','Loipen','Rodelbahnen','Piste','Aufstiegsanlagen' : BITMASK also possible: 'Berg = 1','Radfahren = 2','Stadtrundgang = 4','Pferdesport = 8','Wandern = 16','Laufen und Fitness = 32','Loipen = 64','Rodelbahnen = 128,'Piste = 256,'Aufstiegsanlagen = 512) </param>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting</param>
        /// <returns>Result Object with Collection of Activity Objects Localized</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("api/Activity/Paged/Localized/{language}/{activitytype}/{pagenumber}/{pagesize}/{seed?}")]
        public Task<IActionResult> GetPagedLocalized(string language, string? activitytype, int pagenumber, int pagesize, string? seed, PGGeoSearchResult geosearchresult)
        {          
            return DoAsync(async conn =>
            {
                ActivityHelper myactivityhelper = await ActivityHelper.CreateAsync(conn, activitytype, null, null, null, null, null, null, null, null, null, null, null, null);

                string select = "*";
                string orderby = "";
                var where = PostgresSQLWhereBuilder.CreateActivityWhereExpression(myactivityhelper.idlist, myactivityhelper.activitytypelist, myactivityhelper.subtypelist, myactivityhelper.difficultylist, myactivityhelper.smgtaglist, new List<string>(), new List<string>(), myactivityhelper.tourismvereinlist, myactivityhelper.regionlist, myactivityhelper.arealist, myactivityhelper.distance, myactivityhelper.distancemin, myactivityhelper.distancemax, myactivityhelper.duration, myactivityhelper.durationmin, myactivityhelper.durationmax, myactivityhelper.altitude, myactivityhelper.altitudemin, myactivityhelper.altitudemax, myactivityhelper.highlight, myactivityhelper.active, myactivityhelper.smgactive);

                string? myseed = PostgresSQLOrderByBuilder.BuildSeedOrderBy(ref orderby, seed, "data ->>'Shortname' ASC");

                string whereexpression = where.Item1;

                PostgresSQLHelper.ApplyGeoSearchWhereOrderby(ref whereexpression, ref orderby, geosearchresult);

                int pageskip = pagesize * (pagenumber - 1);

                var data = await PostgresSQLHelper.SelectFromTableDataAsLocalizedObjectParametrizedAsync<GBLTSPoi, GBLTSActivityPoiLocalized>(conn, "activities", select, whereexpression, where.Item2, orderby, pagesize, pageskip, language, PostgresSQLTransformer.TransformToGBLTSActivityPoiLocalized);
                var count = await PostgresSQLHelper.CountDataFromTableParametrizedAsync(conn, "activities", whereexpression, where.Item2);

                int totalcount = (int)count;
                int totalpages = PostgresSQLHelper.PGPagingHelper(totalcount, pagesize);

                return PostgresSQLHelper.GetResultJson(pagenumber, totalpages, totalcount, -1, myseed, JsonConvert.SerializeObject(data));
            });
        }

        /// <summary>
        /// GET Paged Filtered Activities List Localized
        /// </summary>
        /// <param name="language">Localization Language</param>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page</param>
        /// <param name="activitytype">Type of the Activity (possible values: STRINGS: 'Berg','Radfahren','Stadtrundgang','Pferdesport','Wandern','Laufen und Fitness','Loipen','Rodelbahnen','Piste','Aufstiegsanlagen' : BITMASK also possible: 'Berg = 1','Radfahren = 2','Stadtrundgang = 4','Pferdesport = 8','Wandern = 16','Laufen und Fitness = 32','Loipen = 64','Rodelbahnen = 128,'Piste = 256,'Aufstiegsanlagen = 512) </param>
        /// <param name="subtypefilter">Subtype of the Activity ('null' = Filter disabled, BITMASK Filter = available SubTypes depends on the selected Activity Type)</param>
        /// <param name="idfilter">IDFilter (Separator ',' List of Activity IDs, 'null' = No Filter)</param>
        /// <param name="locfilter">Locfilter (Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction), 'null' = No Filter)</param>
        /// <param name="areafilter">AreaFilter (Separator ',' IDList of AreaIDs separated by ',', 'null' : Filter disabled)</param>
        /// <param name="distancefilter">Distance Range Filter (Separator ',' example Value: 15,40 Distance from 15 up to 40 Km) 'null' : disables Filter</param>
        /// <param name="altitudefilter">Altitude Range Filter (Separator ',' example Value: 500,1000 Altitude from 500 up to 1000 metres) 'null' : disables Filter</param>
        /// <param name="durationfilter">Duration Range Filter (Separator ',' example Value: 1,3 Duration from 1 to 3 hours) 'null' : disables Filter</param>
        /// <param name="highlightfilter">Hightlight Filter (possible values: 'null' = Filter disabled, 'false' = only Activities with Highlight false, 'true' = only Activities with Highlight true)</param>
        /// <param name="difficultyfilter">Difficulty Filter (possible values: 'null' = Filter disabled, '1' = easy, '2' = medium, '3' = difficult)</param>  
        /// <param name="active">Active Filter (possible Values: 'null' Displays all Activities, 'true' only Active Activities, 'false' only Disabled Activities</param>
        /// <param name="smgactive">SMGActive Filter (possible Values: 'null' Displays all Activities, 'true' only SMG Active Activities, 'false' only SMG Disabled Activities</param>
        /// <param name="smgtags">SMGTag Filter (String, Separator ',' more SMGTags possible, 'null' = No Filter, available SMGTags reference to 'api/SmgTag/ByMainEntity/Activity')</param>   /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting</param>
        /// <returns>Result Object with Collection of Activity Objects Localized</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("api/Activity/Filtered/Localized/{language}/{pagenumber}/{pagesize}/{activitytype}/{subtypefilter}/{idfilter}/{locfilter}/{areafilter}/{distancefilter}/{altitudefilter}/{durationfilter}/{highlightfilter}/{difficultyfilter}/{active}/{smgactive}/{smgtags}/{seed?}")]
        public Task<IActionResult> GetFilteredLocalized(string language, int pagenumber, int pagesize, string? activitytype, string? subtypefilter, string? idfilter, string? locfilter, string? areafilter, string? distancefilter, string? altitudefilter, string? durationfilter, string? highlightfilter, string? difficultyfilter, string? active, string? smgactive, string? smgtags, string? seed, PGGeoSearchResult geosearchresult)
        {          
            return DoAsync(async conn =>
            {
                ActivityHelper myactivityhelper = await ActivityHelper.CreateAsync(conn, activitytype, subtypefilter, idfilter, locfilter, areafilter, distancefilter, altitudefilter, durationfilter, highlightfilter, difficultyfilter, active, smgactive, smgtags);

                string select = "*";
                string orderby = "";

                var where = PostgresSQLWhereBuilder.CreateActivityWhereExpression(myactivityhelper.idlist, myactivityhelper.activitytypelist, myactivityhelper.subtypelist, myactivityhelper.difficultylist, myactivityhelper.smgtaglist, new List<string>(), new List<string>(), myactivityhelper.tourismvereinlist, myactivityhelper.regionlist, myactivityhelper.arealist, myactivityhelper.distance, myactivityhelper.distancemin, myactivityhelper.distancemax, myactivityhelper.duration, myactivityhelper.durationmin, myactivityhelper.durationmax, myactivityhelper.altitude, myactivityhelper.altitudemin, myactivityhelper.altitudemax, myactivityhelper.highlight, myactivityhelper.active, myactivityhelper.smgactive);

                string? myseed = PostgresSQLOrderByBuilder.BuildSeedOrderBy(ref orderby, seed, "data ->>'Shortname' ASC");

                string whereexpression = where.Item1;

                PostgresSQLHelper.ApplyGeoSearchWhereOrderby(ref whereexpression, ref orderby, geosearchresult);

                int pageskip = pagesize * (pagenumber - 1);

                var data = await PostgresSQLHelper.SelectFromTableDataAsLocalizedObjectParametrizedAsync<GBLTSPoi, GBLTSActivityPoiLocalized>(conn, "activities", select, whereexpression, where.Item2, orderby, pagesize, pageskip, language, PostgresSQLTransformer.TransformToGBLTSActivityPoiLocalized);
                var count = await PostgresSQLHelper.CountDataFromTableParametrizedAsync(conn, "activities", whereexpression, where.Item2);

                int totalcount = (int)count;
                int totalpages = PostgresSQLHelper.PGPagingHelper(totalcount, pagesize);

                return PostgresSQLHelper.GetResultJson(pagenumber, totalpages, totalcount, -1, myseed, JsonConvert.SerializeObject(data));
            });
        }

        /// <summary>
        /// GET Single Activity Localized
        /// </summary>
        /// <param name="language">Localization Language</param>
        /// <param name="id">ID of the Activity</param>
        /// <returns>Activity Localized Object</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("api/Activity/Localized/{language}/{id}")]
        public Task<IActionResult> GetSingleLocalized(string language, string id)
        {
            return DoAsync(async conn =>
            {
                var where = PostgresSQLWhereBuilder.CreateIdListWhereExpression(id.ToUpper());
                var data = await PostgresSQLHelper.SelectFromTableDataAsLocalizedObjectParametrizedAsync<GBLTSPoi, GBLTSActivityPoiLocalized>(conn, "activities", "*", where.Item1, where.Item2, "", 0, null, language, PostgresSQLTransformer.TransformToGBLTSActivityPoiLocalized);

                return JsonConvert.SerializeObject(data.FirstOrDefault());
            });
        }

        #endregion

        #region REDUCED GETTER

        /// <summary>
        /// GET Reduced Activity List Filtered
        /// </summary>
        /// <param name="language">Language of the Reduced List</param>
        /// <param name="activitytype">Type of the Activity (possible values: STRINGS: 'Berg','Radfahren','Stadtrundgang','Pferdesport','Wandern','Laufen und Fitness','Loipen','Rodelbahnen','Piste','Aufstiegsanlagen' : BITMASK also possible: 'Berg = 1','Radfahren = 2','Stadtrundgang = 4','Pferdesport = 8','Wandern = 16','Laufen und Fitness = 32','Loipen = 64','Rodelbahnen = 128,'Piste = 256,'Aufstiegsanlagen = 512) </param>
        /// <param name="subtypefilter">Subtype of the Activity ('null' = Filter disabled, BITMASK Filter = available SubTypes depends on the selected Activity Type)</param> 
        /// <param name="locfilter">Locfilter (Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction), 'null' = No Filter)</param>
        /// <param name="areafilter">AreaFilter (Separator ',' IDList of AreaIDs separated by ',', 'null' : Filter disabled)</param>
        /// <param name="distancefilter">Distance Range Filter (Separator ',' example Value: 15,40 Distance from 15 up to 40 Km) 'null' : disables Filter</param>
        /// <param name="altitudefilter">Altitude Range Filter (Separator ',' example Value: 500,1000 Altitude from 500 up to 1000 metres) 'null' : disables Filter</param>
        /// <param name="durationfilter">Duration Range Filter (Separator ',' example Value: 1,3 Duration from 1 to 3 hours) 'null' : disables Filter</param>
        /// <param name="highlightfilter">Hightlight Filter (possible values: 'null' = Filter disabled, 'false' = only Activities with Highlight false, 'true' = only Activities with Highlight true)</param>
        /// <param name="difficultyfilter">Difficulty Filter (possible values: 'null' = Filter disabled, '1' = easy, '2' = medium, '3' = difficult)</param>  
        /// <param name="active">Active Filter (possible Values: 'null' Displays all Activities, 'true' only Active Activities, 'false' only Disabled Activities</param>
        /// <param name="smgactive">SMGActive Filter (possible Values: 'null' Displays all Activities, 'true' only SMG Active Activities, 'false' only SMG Disabled Activities</param>
        /// <param name="smgtags">SMGTag Filter (String, Separator ',' more SMGTags possible, 'null' = No Filter, available SMGTags reference to 'api/SmgTag/ByMainEntity/Activity')</param>   /// <returns>Collection of Reduced Activity Objects</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[CacheOutput(ClientTimeSpan = 3600, ServerTimeSpan = 3600)]
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("api/Activity/ReducedAsync/{language}/{activitytype}/{subtypefilter}/{locfilter}/{areafilter}/{distancefilter}/{altitudefilter}/{durationfilter}/{highlightfilter}/{difficultyfilter}/{active}/{smgactive}/{smgtags}")]
        public Task<IActionResult> GetReduced(string? language, string? activitytype, string? subtypefilter, string? locfilter, string? areafilter, string? distancefilter, string? altitudefilter, string? durationfilter, string? highlightfilter, string? difficultyfilter, string? active, string? smgactive, string? smgtags, PGGeoSearchResult geosearchresult)
        {          
            return DoAsync(async conn =>
            {
                ActivityHelper myactivityhelper = await ActivityHelper.CreateAsync(conn, activitytype, subtypefilter, null, locfilter, areafilter, distancefilter, altitudefilter, durationfilter, highlightfilter, difficultyfilter, active, smgactive, smgtags);

                string select = "data->'Id' as Id, data->'Detail'->'" + language + "'->'Title' as Name";
                string orderby = "data ->>'Shortname' ASC";

                var where = PostgresSQLWhereBuilder.CreateActivityWhereExpression(myactivityhelper.idlist, myactivityhelper.activitytypelist, myactivityhelper.subtypelist, myactivityhelper.difficultylist, myactivityhelper.smgtaglist, new List<string>(), new List<string>(), myactivityhelper.tourismvereinlist, myactivityhelper.regionlist, myactivityhelper.arealist, myactivityhelper.distance, myactivityhelper.distancemin, myactivityhelper.distancemax, myactivityhelper.duration, myactivityhelper.durationmin, myactivityhelper.durationmax, myactivityhelper.altitude, myactivityhelper.altitudemin, myactivityhelper.altitudemax, myactivityhelper.highlight, myactivityhelper.active, myactivityhelper.smgactive);
                string whereexpression = where.Item1;

                PostgresSQLHelper.ApplyGeoSearchWhereOrderby(ref whereexpression, ref orderby, geosearchresult);

                var data = await PostgresSQLHelper.SelectFromTableDataAsJsonParametrizedAsync(conn, "activities", select, whereexpression, where.Item2, orderby, 0, null, new List<string>() { "Id", "Name" });

                return "[" + String.Join(",", data) + "]";
            });
        }

        #endregion

        #region CUSTOM METHODS

        /// <summary>
        /// GET Activity Types List (Localized Type Names and Bitmasks)
        /// </summary>
        /// <returns>Collection of ActivityTypes Object</returns>
        //[CacheOutput(ClientTimeSpan = 18000, ServerTimeSpan = 18000)]
        [ApiExplorerSettings(IgnoreApi = true)]
        //[CacheOutputUntilToday(23, 59)]
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("api/Activity/GetActivityTypesList")]
        public async Task<IActionResult> GetActivityTypesListAsync()
        {
            try
            {
                List<ActivityTypes> mysuedtiroltypeslist = new List<ActivityTypes>();

                //Get LTS Tagging Types List

                List<LTSTaggingType> ltstaggingtypes = new List<LTSTaggingType>();
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    ltstaggingtypes = await PostgresSQLHelper.SelectFromTableDataAsObjectAsync<LTSTaggingType>(conn, "ltstaggingtypes", "*", "", "", 0, null);
                }

                foreach (ActivityTypeFlag myactivitytype in Enum.GetValues(typeof(ActivityTypeFlag)))
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();

                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivityType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypeFlag>(id);

                    mysmgpoitype.TypeDesc = Helper.LTSTaggingHelper.GetActivityTypeDesc(Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id), ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }

                //Berg Types
                foreach (ActivityTypeBerg myactivitytype in Enum.GetValues(typeof(ActivityTypeBerg)))
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();

                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivitySubType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "Berg";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypeBerg>(id);

                    mysmgpoitype.TypeDesc = Helper.LTSTaggingHelper.GetActivityTypeDesc(Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id), ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }

                //Radfahren Types
                foreach (ActivityTypeRadfahren myactivitytype in Enum.GetValues(typeof(ActivityTypeRadfahren)))
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();
                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivitySubType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "Radfahren";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypeRadfahren>(id);

                    mysmgpoitype.TypeDesc = Helper.LTSTaggingHelper.GetActivityTypeDesc(Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id), ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }
                //Stadtrundgang Types
                foreach (ActivityTypeOrtstouren myactivitytype in Enum.GetValues(typeof(ActivityTypeOrtstouren)))
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();
                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivitySubType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "Stadtrundgang";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypeOrtstouren>(id);

                    mysmgpoitype.TypeDesc = Helper.LTSTaggingHelper.GetActivityTypeDesc(Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id), ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }
                //Pferdesport Types
                foreach (ActivityTypePferde myactivitytype in Enum.GetValues(typeof(ActivityTypePferde)))
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();
                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivitySubType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "Pferdesport";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypePferde>(id);

                    mysmgpoitype.TypeDesc = Helper.LTSTaggingHelper.GetActivityTypeDesc(Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id), ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }
                //Wandern Types
                foreach (ActivityTypeWandern myactivitytype in Enum.GetValues(typeof(ActivityTypeWandern)))
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();
                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivitySubType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "Wandern";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypeWandern>(id);

                    mysmgpoitype.TypeDesc = Helper.LTSTaggingHelper.GetActivityTypeDesc(Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id), ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }
                //LaufenundFitness Types
                foreach (ActivityTypeLaufenFitness myactivitytype in Enum.GetValues(typeof(ActivityTypeLaufenFitness)))
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();
                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivitySubType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "Laufen und Fitness";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypeLaufenFitness>(id);

                    mysmgpoitype.TypeDesc = Helper.LTSTaggingHelper.GetActivityTypeDesc(Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id), ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }
                //Loipen Types
                foreach (ActivityTypeLoipen myactivitytype in Enum.GetValues(typeof(ActivityTypeLoipen)))
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();
                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivitySubType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "Loipen";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypeLoipen>(id);

                    mysmgpoitype.TypeDesc = Helper.LTSTaggingHelper.GetActivityTypeDesc(Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id), ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }
                //Rodelbahnen Types
                foreach (ActivityTypeRodeln myactivitytype in Enum.GetValues(typeof(ActivityTypeRodeln)))
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();
                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivitySubType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "Rodelbahnen";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypeRodeln>(id);

                    mysmgpoitype.TypeDesc = Helper.LTSTaggingHelper.GetActivityTypeDesc(Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id), ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }
                //Piste Types
                foreach (ActivityTypePisten myactivitytype in Enum.GetValues(typeof(ActivityTypePisten)))
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();
                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivitySubType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "Piste";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypePisten>(id);

                    mysmgpoitype.TypeDesc = Helper.LTSTaggingHelper.GetActivityTypeDesc(Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id), ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }
                //Aufstiegsanlagen Types
                foreach (ActivityTypeAufstiegsanlagen myactivitytype in Enum.GetValues(typeof(ActivityTypeAufstiegsanlagen)))
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();
                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivitySubType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "Aufstiegsanlagen";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypeAufstiegsanlagen>(id);

                    mysmgpoitype.TypeDesc = Helper.LTSTaggingHelper.GetActivityTypeDesc(Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id), ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }

                return Content(JsonConvert.SerializeObject(mysuedtiroltypeslist), "application/json", Encoding.UTF8);                
            }
            catch(Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
	
		/// <summary>
		/// GET Paged Activity List based on LastChange Date
		/// </summary>        
		/// <param name="pagenumber">Pagenumber</param>
		/// <param name="pagesize">Elements per Page</param>
		/// <param name="updatefrom">Date from (all Activity with LastChange >= datefrom are passed)</param>
		/// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting</param>
		/// <returns>Result Object with Collection of Activity Objects</returns>
		[ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("api/Activity/GetActivityLastChanged/Paged/{pagenumber}/{pagesize}/{updatefrom}/{seed?}")]
        public Task<IActionResult> GetLastChanged(int pagenumber, int pagesize, string updatefrom, string? seed)
        {
            return DoAsync(async conn =>
            {                
                DateTime updatefromDT = Convert.ToDateTime(updatefrom);

                string select = "*";
                string orderby = "";

                string? myseed = PostgresSQLOrderByBuilder.BuildSeedOrderBy(ref orderby, seed, "data ->>'Shortname' ASC");

                int pageskip = pagesize * (pagenumber - 1);

                var where = PostgresSQLWhereBuilder.CreateLastChangedWhereExpression(updatefrom);

                var data = await PostgresSQLHelper.SelectFromTableDataAsStringParametrizedAsync(conn, "activities", select, where.Item1, where.Item2, orderby, pagesize, pageskip);
                var count = await PostgresSQLHelper.CountDataFromTableParametrizedAsync(conn, "activities", where.Item1, where.Item2);

                int totalcount = (int)count;
                int totalpages = PostgresSQLHelper.PGPagingHelper(totalcount, pagesize);

                return PostgresSQLHelper.GetResultJson(pagenumber, totalpages, totalcount, -1, myseed, String.Join(",", data));
            });
        }		

		#endregion 
    }
}