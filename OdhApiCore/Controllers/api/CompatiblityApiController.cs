using Helper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

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
        public CompatiblityApiController(ISettings settings, IPostGreSQLConnectionFactory connectionFactory)
            : base(settings, connectionFactory)
        {
        }

        #region Poi Controller API

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
        [HttpGet, Route("api/PoiReduced")]
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
            return DoAsyncReturnString(async connectionFactory =>
            {
                PoiHelper mypoihelper = await PoiHelper.CreateAsync(
                    connectionFactory, poitype, subtypefilter, null, locfilter, areafilter,
                    highlightfilter, active, smgactive, smgtags, cancellationToken);

                string select = $"data->'Id' as Id, data->'Detail'->'{language}'->'Title' as Name";
                string orderby = "data ->>'Shortname' ASC";

                var (whereexpression, parameters) = PostgresSQLWhereBuilder.CreatePoiWhereExpression(
                    mypoihelper.idlist, mypoihelper.poitypelist, mypoihelper.subtypelist,
                    mypoihelper.smgtaglist, new List<string>(), new List<string>(), mypoihelper.tourismvereinlist,
                    mypoihelper.regionlist, mypoihelper.arealist, mypoihelper.highlight,
                    mypoihelper.active, mypoihelper.smgactive);


                PostgresSQLHelper.ApplyGeoSearchWhereOrderby(ref whereexpression, ref orderby, geosearchresult);

                var data = await PostgresSQLHelper.SelectFromTableDataAsJsonParametrizedAsync(
                    connectionFactory, "pois", select, (whereexpression, parameters), orderby, 0, null,
                    new List<string>() { "Id", "Name" }, cancellationToken).ToListAsync();

                return JsonConvert.SerializeObject(data);
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
        private Task<IActionResult> GetActivityReduced(
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

    }

}
