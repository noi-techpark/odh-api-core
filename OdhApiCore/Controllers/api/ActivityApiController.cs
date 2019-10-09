using Helper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    /// <summary>
    /// Activity Api (data provided by LTS ActivityData) SOME DATA Available as OPENDATA 
    /// </summary>
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class ActivityController : OdhController
    {
        // Only for test purposes

        public ActivityController(ISettings settings, IPostGreSQLConnectionFactory connectionFactory)
            : base(settings, connectionFactory)
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
            string? language = null,
            uint pagenumber = 1,
            uint pagesize = 10,
            string? activitytype = "1023",
            string? subtype = null,
            string? idlist = null,
            string? locfilter = null,
            string? areafilter = null,
            string? distancefilter = null,
            string? altitudefilter = null,
            string? durationfilter = null,
            bool? highlight = null,
            string? difficultyfilter = null,
            string? odhtagfilter = null,
            bool? active = null,
            bool? odhactive = null,
            string? seed = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            return await GetFiltered(
                    language, pagenumber, pagesize, activitytype, subtype, idlist, locfilter, areafilter, distancefilter,
                    altitudefilter, durationfilter, highlight, difficultyfilter, active, odhactive, odhtagfilter, seed,
                    geosearchresult, cancellationToken);
        }

        /// <summary>
        /// GET Activity Single 
        /// </summary>
        /// <param name="id">ID of the Activity</param>
        /// <returns>Activity Object</returns>
        //[SwaggerResponse(HttpStatusCode.OK, "Activity Object", typeof(GBLTSActivity))]
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [ProducesResponseType(typeof(GBLTSActivity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("api/Activity/{id}")]
        public async Task<IActionResult> GetActivitySingle(string id, CancellationToken cancellationToken)
        {
            return await GetSingle(id, cancellationToken);
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
        [ProducesResponseType(typeof(IEnumerable<ActivityPoiReduced>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
            bool? highlight = null,
            string? difficultyfilter = null,
            string? odhtagfilter = null,
            bool? active = null,
            bool? odhactive = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            return await GetReduced(
                language, activitytype, subtype, locfilter, areafilter, distancefilter, altitudefilter, durationfilter,
                highlight, difficultyfilter, active, odhactive, odhtagfilter, geosearchresult, cancellationToken);
        }

        //Special

        /// <summary>
        /// GET Activity Types List
        /// </summary>
        /// <returns>Collection of ActivityTypes Object</returns>                
        //[CacheOutputUntilToday(23, 59)]
        [ProducesResponseType(typeof(IEnumerable<ActivityTypes>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("api/ActivityTypes")]
        public async Task<IActionResult> GetAllActivityTypesListAsync(CancellationToken cancellationToken)
        {
            return await GetActivityTypesListAsync(cancellationToken);
        }

        /// <summary>
        /// GET Activity Changed List by Date
        /// </summary>
        /// <param name="pagenumber">Pagenumber, (default:1)</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="updatefrom">Date from Format (yyyy-MM-dd) (all GBActivityPoi with LastChange >= datefrom are passed), (default: DateTime.Now - 1 Day)</param>
        /// <returns>Collection of PoiBaseInfos Objects</returns>
        [ProducesResponseType(typeof(IEnumerable<GBLTSActivity>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("api/ActivityChanged")]
        public async Task<IActionResult> GetAllActivityChanged(
            uint pagenumber = 1,
            uint pagesize = 10,
            string? seed = null,
            string? updatefrom = null,
            CancellationToken cancellationToken = default
            )
        {
            updatefrom ??= String.Format("{0:yyyy-MM-dd}", DateTime.Now.AddDays(-1));

            return await GetLastChanged(pagenumber, pagesize, updatefrom, seed, cancellationToken);
        }

        #endregion

        #region GETTER

        /// <summary>
        /// GET Activities List
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
         private Task<IActionResult> GetFiltered(
            string? language, uint pagenumber, uint pagesize, string? activitytype, string? subtypefilter, string? idfilter,
            string? locfilter, string? areafilter, string? distancefilter, string? altitudefilter,
            string? durationfilter, bool? highlightfilter, string? difficultyfilter, bool? active, bool? smgactive,
            string? smgtags, string? seed, PGGeoSearchResult geosearchresult, CancellationToken cancellationToken)
        {
            return DoAsyncReturnString(async connectionFactory =>
            {
                ActivityHelper myactivityhelper = await ActivityHelper.CreateAsync(
                    connectionFactory, activitytype, subtypefilter, idfilter, locfilter, areafilter, distancefilter,
                    altitudefilter, durationfilter, highlightfilter, difficultyfilter, active, smgactive, smgtags,
                    cancellationToken);

                string select = "*";
                string orderby = "";

                var (whereexpression, parameters) = PostgresSQLWhereBuilder.CreateActivityWhereExpression(
                    myactivityhelper.idlist, myactivityhelper.activitytypelist, myactivityhelper.subtypelist,
                    myactivityhelper.difficultylist, myactivityhelper.smgtaglist, new List<string>(), new List<string>(),
                    myactivityhelper.tourismvereinlist, myactivityhelper.regionlist, myactivityhelper.arealist,
                    myactivityhelper.distance, myactivityhelper.distancemin, myactivityhelper.distancemax,
                    myactivityhelper.duration, myactivityhelper.durationmin, myactivityhelper.durationmax,
                    myactivityhelper.altitude, myactivityhelper.altitudemin, myactivityhelper.altitudemax,
                    myactivityhelper.highlight, myactivityhelper.active, myactivityhelper.smgactive);

                string? myseed = PostgresSQLOrderByBuilder.BuildSeedOrderBy(ref orderby, seed, "data ->>'Shortname' ASC");

                PostgresSQLHelper.ApplyGeoSearchWhereOrderby(ref whereexpression, ref orderby, geosearchresult);

                uint pageskip = pagesize * (pagenumber - 1);

                var dataTask = PostgresSQLHelper.SelectFromTableDataAsStringParametrizedAsync(
                    connectionFactory, "activities", select, (whereexpression, parameters), orderby, pagesize, pageskip,
                    cancellationToken);
                var count = await PostgresSQLHelper.CountDataFromTableParametrizedAsync(
                    connectionFactory, "activities", (whereexpression, parameters), cancellationToken);

                uint totalcount = count;
                uint totalpages = PostgresSQLHelper.PGPagingHelper(totalcount, pagesize);

                var data = dataTask.Select(raw => raw.TransformRawData(language, checkCC0: CheckCC0License));

                return PostgresSQLHelper.GetResultJson(
                    pagenumber,
                    totalpages,
                    totalcount,
                    myseed,
                    await data.ToListAsync());
            });
        }

        /// <summary>
        /// GET Single Activity
        /// </summary>
        /// <param name="id">ID of the Activity</param>
        /// <returns>Activity Object</returns>                
        private Task<IActionResult> GetSingle(string id, CancellationToken cancellationToken)
        {
            return DoAsyncReturnString(async connectionFactory =>
            {
                var where = PostgresSQLWhereBuilder.CreateIdListWhereExpression(id.ToUpper());
                var data = await PostgresSQLHelper.SelectFromTableDataAsStringParametrizedAsync(
                    connectionFactory, "activities", "*", where, "", 0,
                    null, cancellationToken).ToListAsync();

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
        private Task<IActionResult> GetReduced(
            string? language, string? activitytype, string? subtypefilter, string? locfilter, string? areafilter,
            string? distancefilter, string? altitudefilter, string? durationfilter, bool? highlightfilter,
            string? difficultyfilter, bool? active, bool? smgactive, string? smgtags,
            PGGeoSearchResult geosearchresult, CancellationToken cancellationToken)
        {
            return DoAsyncReturnString(async connectionFactory =>
            {
                ActivityHelper myactivityhelper = await ActivityHelper.CreateAsync(
                    connectionFactory, activitytype, subtypefilter, null, locfilter, areafilter, distancefilter,
                    altitudefilter, durationfilter, highlightfilter, difficultyfilter, active, smgactive, smgtags,
                    cancellationToken);

                string select = $"data->'Id' as Id, data->'Detail'->'{language}'->'Title' as Name";
                string orderby = "data ->>'Shortname' ASC";

                var (whereexpression, parameters) = PostgresSQLWhereBuilder.CreateActivityWhereExpression(
                    myactivityhelper.idlist, myactivityhelper.activitytypelist, myactivityhelper.subtypelist,
                    myactivityhelper.difficultylist, myactivityhelper.smgtaglist, new List<string>(), new List<string>(),
                    myactivityhelper.tourismvereinlist, myactivityhelper.regionlist, myactivityhelper.arealist,
                    myactivityhelper.distance, myactivityhelper.distancemin, myactivityhelper.distancemax,
                    myactivityhelper.duration, myactivityhelper.durationmin, myactivityhelper.durationmax,
                    myactivityhelper.altitude, myactivityhelper.altitudemin, myactivityhelper.altitudemax,
                    myactivityhelper.highlight, myactivityhelper.active, myactivityhelper.smgactive);

                PostgresSQLHelper.ApplyGeoSearchWhereOrderby(ref whereexpression, ref orderby, geosearchresult);

                var data = await PostgresSQLHelper.SelectFromTableDataAsJsonParametrizedAsync(
                    connectionFactory, "activities", select, (whereexpression, parameters), orderby, 0, null,
                    new List<string>() { "Id", "Name" }, cancellationToken).ToListAsync();

                return JsonConvert.SerializeObject(data);
            });
        }

        #endregion

        #region CUSTOM METHODS

        /// <summary>
        /// GET Activity Types List (Localized Type Names and Bitmasks)
        /// </summary>
        /// <returns>Collection of ActivityTypes Object</returns>        
        private Task<IActionResult> GetActivityTypesListAsync(CancellationToken cancellationToken)
        {
            return DoAsyncReturnString(async connectionFactory =>
            {
                List<ActivityTypes> mysuedtiroltypeslist = new List<ActivityTypes>();

                //Get LTS Tagging Types List                
                var ltstaggingtypes = PostgresSQLHelper.SelectFromTableDataAsObjectAsync<LTSTaggingType>(
                        connectionFactory, "ltstaggingtypes", "*", "", "", 0,
                        null, cancellationToken);

                foreach (ActivityTypeFlag myactivitytype in EnumHelper.GetValues<ActivityTypeFlag>())
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();

                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivityType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypeFlag>(id);

                    mysmgpoitype.TypeDesc = await Helper.LTSTaggingHelper.GetActivityTypeDescAsync(
                        Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id),
                        ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }

                //Berg Types
                foreach (ActivityTypeBerg myactivitytype in EnumHelper.GetValues<ActivityTypeBerg>())
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();

                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivitySubType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "Berg";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypeBerg>(id);

                    mysmgpoitype.TypeDesc = await Helper.LTSTaggingHelper.GetActivityTypeDescAsync(
                        Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id),
                        ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }

                //Radfahren Types
                foreach (ActivityTypeRadfahren myactivitytype in EnumHelper.GetValues<ActivityTypeRadfahren>())
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();
                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivitySubType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "Radfahren";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypeRadfahren>(id);

                    mysmgpoitype.TypeDesc = await Helper.LTSTaggingHelper.GetActivityTypeDescAsync(
                        Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id),
                        ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }
                //Stadtrundgang Types
                foreach (ActivityTypeOrtstouren myactivitytype in EnumHelper.GetValues<ActivityTypeOrtstouren>())
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();
                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivitySubType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "Stadtrundgang";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypeOrtstouren>(id);

                    mysmgpoitype.TypeDesc = await Helper.LTSTaggingHelper.GetActivityTypeDescAsync(
                        Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id),
                        ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }
                //Pferdesport Types
                foreach (ActivityTypePferde myactivitytype in EnumHelper.GetValues<ActivityTypePferde>())
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();
                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivitySubType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "Pferdesport";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypePferde>(id);

                    mysmgpoitype.TypeDesc = await Helper.LTSTaggingHelper.GetActivityTypeDescAsync(
                        Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id),
                        ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }
                //Wandern Types
                foreach (ActivityTypeWandern myactivitytype in EnumHelper.GetValues<ActivityTypeWandern>())
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();
                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivitySubType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "Wandern";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypeWandern>(id);

                    mysmgpoitype.TypeDesc = await Helper.LTSTaggingHelper.GetActivityTypeDescAsync(
                        Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id),
                        ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }
                //LaufenundFitness Types
                foreach (ActivityTypeLaufenFitness myactivitytype in EnumHelper.GetValues<ActivityTypeLaufenFitness>())
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();
                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivitySubType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "Laufen und Fitness";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypeLaufenFitness>(id);

                    mysmgpoitype.TypeDesc = await Helper.LTSTaggingHelper.GetActivityTypeDescAsync(
                        Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id),
                        ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }
                //Loipen Types
                foreach (ActivityTypeLoipen myactivitytype in EnumHelper.GetValues<ActivityTypeLoipen>())
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();
                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivitySubType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "Loipen";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypeLoipen>(id);

                    mysmgpoitype.TypeDesc = await Helper.LTSTaggingHelper.GetActivityTypeDescAsync(
                        Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id),
                        ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }
                //Rodelbahnen Types
                foreach (ActivityTypeRodeln myactivitytype in EnumHelper.GetValues<ActivityTypeRodeln>())
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();
                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivitySubType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "Rodelbahnen";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypeRodeln>(id);

                    mysmgpoitype.TypeDesc = await Helper.LTSTaggingHelper.GetActivityTypeDescAsync(
                        Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id),
                        ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }
                //Piste Types
                foreach (ActivityTypePisten myactivitytype in EnumHelper.GetValues<ActivityTypePisten>())
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();
                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivitySubType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "Piste";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypePisten>(id);

                    mysmgpoitype.TypeDesc = await Helper.LTSTaggingHelper.GetActivityTypeDescAsync(
                        Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id),
                        ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }
                //Aufstiegsanlagen Types
                foreach (ActivityTypeAufstiegsanlagen myactivitytype in EnumHelper.GetValues<ActivityTypeAufstiegsanlagen>())
                {
                    ActivityTypes mysmgpoitype = new ActivityTypes();

                    string? id = myactivitytype.GetDescription();
                    mysmgpoitype.Id = id;
                    mysmgpoitype.Type = "ActivitySubType"; // +mysuedtiroltype.TypeParent;
                    mysmgpoitype.Parent = "Aufstiegsanlagen";

                    mysmgpoitype.Bitmask = FlagsHelper.GetFlagofType<ActivityTypeAufstiegsanlagen>(id);

                    mysmgpoitype.TypeDesc = await Helper.LTSTaggingHelper.GetActivityTypeDescAsync(
                        Helper.LTSTaggingHelper.LTSActivityTaggingTagTranslator(id),
                        ltstaggingtypes) as Dictionary<string, string>;

                    mysuedtiroltypeslist.Add(mysmgpoitype);
                }

                return JsonConvert.SerializeObject(mysuedtiroltypeslist);
            });
        }

        /// <summary>
        /// GET Paged Activity List based on LastChange Date
        /// </summary>        
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page</param>
        /// <param name="updatefrom">Date from (all Activity with LastChange >= datefrom are passed)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting</param>
        /// <returns>Result Object with Collection of Activity Objects</returns>
        private Task<IActionResult> GetLastChanged(
            uint pagenumber, uint pagesize, string updatefrom, string? seed,
            CancellationToken cancellationToken)
        {
            return DoAsyncReturnString(async connectionFactory =>
            {
                DateTime updatefromDT = Convert.ToDateTime(updatefrom);

                string select = "*";
                string orderby = "";

                string? myseed = PostgresSQLOrderByBuilder.BuildSeedOrderBy(
                    ref orderby, seed, "data ->>'Shortname' ASC");

                uint pageskip = pagesize * (pagenumber - 1);

                var where = PostgresSQLWhereBuilder.CreateLastChangedWhereExpression(updatefrom);

                var dataTask = PostgresSQLHelper.SelectFromTableDataAsStringParametrizedAsync(
                    connectionFactory, "activities", select, where, orderby, pagesize, pageskip,
                    cancellationToken);
                var count = await PostgresSQLHelper.CountDataFromTableParametrizedAsync(
                    connectionFactory, "activities", where, cancellationToken);

                uint totalcount = count;
                uint totalpages = PostgresSQLHelper.PGPagingHelper(totalcount, pagesize);

                return PostgresSQLHelper.GetResultJson(
                    pagenumber, totalpages, totalcount, -1, myseed, await dataTask.ToListAsync());
            });
        }

        #endregion
    }
}