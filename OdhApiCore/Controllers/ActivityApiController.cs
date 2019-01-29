using System;
using System.Collections.Generic;
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

namespace OdhApiCore.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    public class ActivityController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private string connectionString = "";

        public ActivityController(IConfiguration config)
        {
            configuration = config;
            connectionString = configuration.GetConnectionString("PGConnection");
        }

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
        [HttpGet, Route("All/{activitytype}/{elements}/{seed}")]
        public IActionResult GetAll(string activitytype, int elements, string seed)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    ActivityHelper myactivityhelper = new ActivityHelper(activitytype, "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", connectionString);

                    conn.Open();

                    string select = "*";
                    string orderby = "";

                    string where = PostgresSQLHelper.CreateActivityWhereExpression(myactivityhelper.idlist, myactivityhelper.activitytypelist, myactivityhelper.subtypelist, myactivityhelper.difficultylist, myactivityhelper.smgtaglist, new List<string>(), new List<string>(), myactivityhelper.tourismvereinlist, myactivityhelper.regionlist, myactivityhelper.arealist, myactivityhelper.distance, myactivityhelper.distancemin, myactivityhelper.distancemax, myactivityhelper.duration, myactivityhelper.durationmin, myactivityhelper.durationmax, myactivityhelper.altitude, myactivityhelper.altitudemin, myactivityhelper.altitudemax, myactivityhelper.highlight, myactivityhelper.active, myactivityhelper.smgactive);

                    if (seed != "null")
                    {
                        string myseed = Helper.CreateSeed.GetSeed(seed);
                        orderby = "md5(id || '" + myseed + "')";
                    }
                    else
                    {
                        orderby = "data ->>'Shortname' ASC";
                    }

                    var myresult = PostgresSQLHelper.SelectFromTableDataAsString(conn, "activities", select, where, orderby, elements, null);

                    conn.Close();

                    return Content("[" + String.Join(",", myresult) + "]", "application/json", Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }        
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
        [HttpGet, Route("Paged/{activitytype}/{pagenumber}/{pagesize}/{seed}")]
        public IActionResult GetPaged(string activitytype, int pagenumber, int pagesize, string seed, PGGeoSearchResult geosearchresult)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    ActivityHelper myactivityhelper = new ActivityHelper(activitytype, "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", connectionString);

                    conn.Open();

                    string myseed = seed;

                    string select = "*";
                    string orderby = "";

                    string where = PostgresSQLHelper.CreateActivityWhereExpression(myactivityhelper.idlist, myactivityhelper.activitytypelist, myactivityhelper.subtypelist, myactivityhelper.difficultylist, myactivityhelper.smgtaglist, new List<string>(), new List<string>(), myactivityhelper.tourismvereinlist, myactivityhelper.regionlist, myactivityhelper.arealist, myactivityhelper.distance, myactivityhelper.distancemin, myactivityhelper.distancemax, myactivityhelper.duration, myactivityhelper.durationmin, myactivityhelper.durationmax, myactivityhelper.altitude, myactivityhelper.altitudemin, myactivityhelper.altitudemax, myactivityhelper.highlight, myactivityhelper.active, myactivityhelper.smgactive);


                    if (seed != "null")
                    {
                        myseed = Helper.CreateSeed.GetSeed(seed);
                        //orderby = "md5(data->>'Id' || '" + seed + "')";
                        orderby = "md5(id || '" + myseed + "')";
                    }
                    else
                    {
                        orderby = "data ->>'Shortname' ASC";
                    }

                    PostgresSQLHelper.ApplyGeoSearchWhereOrderby(ref where, ref orderby, geosearchresult);

                    int pageskip = pagesize * (pagenumber - 1);

                    var data = PostgresSQLHelper.SelectFromTableDataAsString(conn, "activities", select, where, orderby, pagesize, pageskip);
                    var count = PostgresSQLHelper.CountDataFromTable(conn, "activities", where);

                    conn.Close();

                    int totalcount = Convert.ToInt32(count);
                    int totalpages = 0;

                    if (totalcount % pagesize == 0)
                        totalpages = totalcount / pagesize;
                    else
                        totalpages = (totalcount / pagesize) + 1;          

                    return Content(PostgresSQLHelper.GetResultJson(pagenumber, totalpages, totalcount, myseed, String.Join(",", data)), "application/json", Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }

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
        [HttpGet, Route("Filtered/{pagenumber}/{pagesize}/{activitytype}/{subtypefilter}/{idfilter}/{locfilter}/{areafilter}/{distancefilter}/{altitudefilter}/{durationfilter}/{highlightfilter}/{difficultyfilter}/{active}/{smgactive}/{smgtags}/{seed}")]
        public IActionResult GetFiltered(int pagenumber, int pagesize, string activitytype, string subtypefilter, string idfilter, string locfilter, string areafilter, string distancefilter, string altitudefilter, string durationfilter, string highlightfilter, string difficultyfilter, string active, string smgactive, string smgtags, string seed, PGGeoSearchResult geosearchresult)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    ActivityHelper myactivityhelper = new ActivityHelper(activitytype, subtypefilter, idfilter, locfilter, areafilter, distancefilter, altitudefilter, durationfilter, highlightfilter, difficultyfilter, active, smgactive, smgtags, connectionString);

                    conn.Open();

                    string myseed = seed;

                    string select = "*";
                    string orderby = "";

                    string where = PostgresSQLHelper.CreateActivityWhereExpression(myactivityhelper.idlist, myactivityhelper.activitytypelist, myactivityhelper.subtypelist, myactivityhelper.difficultylist, myactivityhelper.smgtaglist, new List<string>(), new List<string>(), myactivityhelper.tourismvereinlist, myactivityhelper.regionlist, myactivityhelper.arealist, myactivityhelper.distance, myactivityhelper.distancemin, myactivityhelper.distancemax, myactivityhelper.duration, myactivityhelper.durationmin, myactivityhelper.durationmax, myactivityhelper.altitude, myactivityhelper.altitudemin, myactivityhelper.altitudemax, myactivityhelper.highlight, myactivityhelper.active, myactivityhelper.smgactive);


                    if (seed != "null")
                    {
                        myseed = Helper.CreateSeed.GetSeed(seed);
                        //orderby = "md5(data->>'Id' || '" + seed + "')";
                        orderby = "md5(id || '" + myseed + "')";
                    }
                    else
                    {
                        orderby = "data ->>'Shortname' ASC";
                    }

                    PostgresSQLHelper.ApplyGeoSearchWhereOrderby(ref where, ref orderby, geosearchresult);

                    int pageskip = pagesize * (pagenumber - 1);

                    var data = PostgresSQLHelper.SelectFromTableDataAsString(conn, "activities", select, where, orderby, pagesize, pageskip);
                    var count = PostgresSQLHelper.CountDataFromTable(conn, "activities", where);

                    conn.Close();

                    int totalcount = Convert.ToInt32(count);
                    int totalpages = 0;

                    if (totalcount % pagesize == 0)
                        totalpages = totalcount / pagesize;
                    else
                        totalpages = (totalcount / pagesize) + 1;

                    //return new HttpResponseMessage()
                    //{
                    //    StatusCode = HttpStatusCode.OK,
                    //    Content = new StringContent(PostgresSQLHelper.GetResultJson(pagenumber, totalpages, totalcount, myseed, String.Join(",", data)), Encoding.UTF8, "application/json")
                    //};

                    return Content(PostgresSQLHelper.GetResultJson(pagenumber, totalpages, totalcount, myseed, String.Join(",", data)), "application/json", Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        /// <summary>
        /// GET Single Activity
        /// </summary>
        /// <param name="id">ID of the Activity</param>
        /// <returns>Activity Object</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataReader,ActivityReader")]
        [HttpGet, Route("Single/{id}")]
        public IActionResult GetSingle(string id)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string selectexp = "*";

                    string whereexp = "id LIKE '" + id.ToUpper() + "'";

                    var data = PostgresSQLHelper.SelectFromTableDataAsString(conn, "activities", selectexp, whereexp, "", 0, null);

                    conn.Close();

                    return Content(String.Join(",", data), "application/json", Encoding.UTF8);                    
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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
        [HttpGet, Route("api/Activity/All/Localized/{language}/{activitytype}/{elements}/{seed}")]
        public IActionResult GetLocalized(string language, string activitytype, int elements, string seed)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    ActivityHelper myactivityhelper = new ActivityHelper(activitytype, "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", connectionString);

                    conn.Open();

                    string select = "*";
                    string orderby = "";

                    string where = PostgresSQLHelper.CreateActivityWhereExpression(myactivityhelper.idlist, myactivityhelper.activitytypelist, myactivityhelper.subtypelist, myactivityhelper.difficultylist, myactivityhelper.smgtaglist, new List<string>(), new List<string>(), myactivityhelper.tourismvereinlist, myactivityhelper.regionlist, myactivityhelper.arealist, myactivityhelper.distance, myactivityhelper.distancemin, myactivityhelper.distancemax, myactivityhelper.duration, myactivityhelper.durationmin, myactivityhelper.durationmax, myactivityhelper.altitude, myactivityhelper.altitudemin, myactivityhelper.altitudemax, myactivityhelper.highlight, myactivityhelper.active, myactivityhelper.smgactive);

                    if (seed != "null")
                    {
                        string myseed = Helper.CreateSeed.GetSeed(seed);
                        //orderby = "md5(data->>'Id' || '" + seed + "')";
                        orderby = "md5(id || '" + myseed + "')";
                    }
                    else
                    {
                        orderby = "data ->>'Shortname' ASC";
                    }

                    var myresult = PostgresSQLHelper.SelectFromTableDataAsLtsPoiLocalizedObject(conn, "activities", select, where, orderby, elements, null, language);

                    conn.Close();                    

                    return Content(JsonConvert.SerializeObject(myresult), "application/json", Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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
        [HttpGet, Route("api/Activity/Paged/Localized/{language}/{activitytype}/{pagenumber}/{pagesize}/{seed}")]
        public IActionResult GetPagedLocalized(string language, string activitytype, int pagenumber, int pagesize, string seed, PGGeoSearchResult geosearchresult)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    ActivityHelper myactivityhelper = new ActivityHelper(activitytype, "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", connectionString);

                    conn.Open();

                    string select = "*";
                    string orderby = "";

                    string where = PostgresSQLHelper.CreateActivityWhereExpression(myactivityhelper.idlist, myactivityhelper.activitytypelist, myactivityhelper.subtypelist, myactivityhelper.difficultylist, myactivityhelper.smgtaglist, new List<string>(), new List<string>(), myactivityhelper.tourismvereinlist, myactivityhelper.regionlist, myactivityhelper.arealist, myactivityhelper.distance, myactivityhelper.distancemin, myactivityhelper.distancemax, myactivityhelper.duration, myactivityhelper.durationmin, myactivityhelper.durationmax, myactivityhelper.altitude, myactivityhelper.altitudemin, myactivityhelper.altitudemax, myactivityhelper.highlight, myactivityhelper.active, myactivityhelper.smgactive);

                    string myseed = seed;

                    if (seed != "null")
                    {
                        myseed = Helper.CreateSeed.GetSeed(seed);
                        //orderby = "md5(data->>'Id' || '" + seed + "')";
                        orderby = "md5(id || '" + myseed + "')";
                    }
                    else
                    {
                        orderby = "data ->>'Shortname' ASC";
                    }

                    PostgresSQLHelper.ApplyGeoSearchWhereOrderby(ref where, ref orderby, geosearchresult);

                    int pageskip = pagesize * (pagenumber - 1);

                    var data = PostgresSQLHelper.SelectFromTableDataAsLtsPoiLocalizedObject(conn, "activities", select, where, orderby, pagesize, pageskip, language);
                    var count = PostgresSQLHelper.CountDataFromTable(conn, "activities", where);

                    conn.Close();

                    int totalcount = Convert.ToInt32(count);
                    int totalpages = 0;

                    if (totalcount % pagesize == 0)
                        totalpages = totalcount / pagesize;
                    else
                        totalpages = (totalcount / pagesize) + 1;

                    return Content(PostgresSQLHelper.GetResultJson(pagenumber, totalpages, totalcount, -1, myseed, JsonConvert.SerializeObject(data)), "application/json", Encoding.UTF8);              
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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
        [HttpGet, Route("api/Activity/Filtered/Localized/{language}/{pagenumber}/{pagesize}/{activitytype}/{subtypefilter}/{idfilter}/{locfilter}/{areafilter}/{distancefilter}/{altitudefilter}/{durationfilter}/{highlightfilter}/{difficultyfilter}/{active}/{smgactive}/{smgtags}/{seed}")]
        public IActionResult GetFilteredLocalized(string language, int pagenumber, int pagesize, string activitytype, string subtypefilter, string idfilter, string locfilter, string areafilter, string distancefilter, string altitudefilter, string durationfilter, string highlightfilter, string difficultyfilter, string active, string smgactive, string smgtags, string seed, PGGeoSearchResult geosearchresult)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {

                    ActivityHelper myactivityhelper = new ActivityHelper(activitytype, subtypefilter, idfilter, locfilter, areafilter, distancefilter, altitudefilter, durationfilter, highlightfilter, difficultyfilter, active, smgactive, smgtags, connectionString);

                    conn.Open();

                    string myseed = seed;

                    string select = "*";
                    string orderby = "";

                    string where = PostgresSQLHelper.CreateActivityWhereExpression(myactivityhelper.idlist, myactivityhelper.activitytypelist, myactivityhelper.subtypelist, myactivityhelper.difficultylist, myactivityhelper.smgtaglist, new List<string>(), new List<string>(), myactivityhelper.tourismvereinlist, myactivityhelper.regionlist, myactivityhelper.arealist, myactivityhelper.distance, myactivityhelper.distancemin, myactivityhelper.distancemax, myactivityhelper.duration, myactivityhelper.durationmin, myactivityhelper.durationmax, myactivityhelper.altitude, myactivityhelper.altitudemin, myactivityhelper.altitudemax, myactivityhelper.highlight, myactivityhelper.active, myactivityhelper.smgactive);

                    if (seed != "null")
                    {
                        myseed = Helper.CreateSeed.GetSeed(seed);
                        //orderby = "md5(data->>'Id' || '" + seed + "')";
                        orderby = "md5(id || '" + myseed + "')";
                    }
                    else
                    {
                        orderby = "data ->>'Shortname' ASC";
                        //orderby = "md5(id || '" + myseed + "')";
                    }

                    PostgresSQLHelper.ApplyGeoSearchWhereOrderby(ref where, ref orderby, geosearchresult);

                    int pageskip = pagesize * (pagenumber - 1);

                    var data = PostgresSQLHelper.SelectFromTableDataAsLtsPoiLocalizedObject(conn, "activities", select, where, orderby, pagesize, pageskip, language);
                    var count = PostgresSQLHelper.CountDataFromTable(conn, "activities", where);

                    conn.Close();

                    int totalcount = Convert.ToInt32(count);
                    int totalpages = 0;

                    if (totalcount % pagesize == 0)
                        totalpages = totalcount / pagesize;
                    else
                        totalpages = (totalcount / pagesize) + 1;

                    return Content(PostgresSQLHelper.GetResultJson(pagenumber, totalpages, totalcount, -1, myseed, JsonConvert.SerializeObject(data)), "application/json", Encoding.UTF8);
            
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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
        public IActionResult GetSingleLocalized(string language, string id)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string selectexp = "*";

                    //string whereexp = "data @> '{\"Id\" : \"" + id + "\" }'";
                    string whereexp = "id LIKE '" + id.ToUpper() + "'";

                    var data = PostgresSQLHelper.SelectFromTableDataAsLtsPoiLocalizedObject(conn, "activities", selectexp, whereexp, "", 0, null, language);

                    conn.Close();

                    return Content(JsonConvert.SerializeObject(data.FirstOrDefault()), "application/json", Encoding.UTF8);                   
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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
        public IActionResult GetReduced(string language, string activitytype, string subtypefilter, string locfilter, string areafilter, string distancefilter, string altitudefilter, string durationfilter, string highlightfilter, string difficultyfilter, string active, string smgactive, string smgtags, PGGeoSearchResult geosearchresult)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    ActivityHelper myactivityhelper = new ActivityHelper(activitytype, subtypefilter, "null", locfilter, areafilter, distancefilter, altitudefilter, durationfilter, highlightfilter, difficultyfilter, active, smgactive, smgtags, connectionString);

                    conn.Open();
                    string select = "data->'Id' as Id, data->'Detail'->'" + language + "'->'Title' as Name";
                    string orderby = "data ->>'Shortname' ASC";

                    string where = PostgresSQLHelper.CreateActivityWhereExpression(myactivityhelper.idlist, myactivityhelper.activitytypelist, myactivityhelper.subtypelist, myactivityhelper.difficultylist, myactivityhelper.smgtaglist, new List<string>(), new List<string>(), myactivityhelper.tourismvereinlist, myactivityhelper.regionlist, myactivityhelper.arealist, myactivityhelper.distance, myactivityhelper.distancemin, myactivityhelper.distancemax, myactivityhelper.duration, myactivityhelper.durationmin, myactivityhelper.durationmax, myactivityhelper.altitude, myactivityhelper.altitudemin, myactivityhelper.altitudemax, myactivityhelper.highlight, myactivityhelper.active, myactivityhelper.smgactive);

                    PostgresSQLHelper.ApplyGeoSearchWhereOrderby(ref where, ref orderby, geosearchresult);

                    var data = PostgresSQLHelper.SelectFromTableDataAsIdAndString(conn, "activities", select, where, orderby, 0, null, new List<string>() { "Id", "Name" });

                    conn.Close();

                    return Content("[" + String.Join(",", data) + "]", "application/json", Encoding.UTF8);                    
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }


        }



        #endregion
    }

    public class ActivityHelper
    {
        public List<string> activitytypelist;
        public List<string> subtypelist;
        public List<string> idlist;
        public List<string> arealist;
        public List<string> smgtaglist;
        public List<string> difficultylist;
        public List<string> tourismvereinlist;
        public List<string> regionlist;
        public bool distance;
        public int distancemin;
        public int distancemax;
        public bool altitude;
        public int altitudemin;
        public int altitudemax;
        public bool duration;
        public int durationmin;
        public int durationmax;
        public bool? highlight;
        public bool? active;
        public bool? smgactive;

        public ActivityHelper(string activitytype, string subtypefilter, string idfilter, string locfilter, string areafilter, string distancefilter, string altitudefilter, string durationfilter, string highlightfilter, string difficultyfilter, string activefilter, string smgactivefilter, string smgtags, string connectionString)
        {
            activitytypelist = new List<string>();
            int typeinteger = 0;

            if (activitytype != "null")
            {
                if (int.TryParse(activitytype, out typeinteger))
                {
                    //Sonderfall wenn alles abgefragt wird um keine unnötige Where zu erzeugen
                    if (typeinteger != 1023)
                        activitytypelist = Helper.ActivityPoiListCreator.CreateActivityTypefromFlag(activitytype);
                }
                else
                {
                    activitytypelist.Add(activitytype);
                }
            }
            if (activitytypelist.Count > 0)
                subtypelist = Helper.ActivityPoiListCreator.CreateActivitySubTypefromFlag(activitytypelist.FirstOrDefault(), subtypefilter);
            else
                subtypelist = new List<string>();

            idlist = Helper.CommonListCreator.CreateIdList(idfilter.ToUpper());
            //TODO
            arealist = new List<string>();
            if (areafilter != "null")
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    arealist = LocationListCreator.CreateActivityAreaListPG(areafilter, conn);

                    conn.Close();
                }
            }



            smgtaglist = CommonListCreator.CreateIdList(smgtags);
            difficultylist = CommonListCreator.CreateDifficultyList(difficultyfilter, activitytype);


            tourismvereinlist = new List<string>();
            regionlist = new List<string>();

            if (locfilter.Contains("reg"))
                regionlist = CommonListCreator.CreateDistrictIdList(locfilter, "reg");
            if (locfilter.Contains("tvs"))
                tourismvereinlist = CommonListCreator.CreateDistrictIdList(locfilter, "tvs");

            //Sonderfall für MetaRegion hole mir alle DistrictIds dieser MetaRegion
            if (locfilter.Contains("mta"))
            {
                List<string> metaregionlist = CommonListCreator.CreateDistrictIdList(locfilter, "mta");

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string where = "";

                    if (metaregionlist.Count == 1)
                    {
                        where = "data @> '{\"Id\" : \"" + metaregionlist.FirstOrDefault() + "\" }'";
                    }
                    else
                    {
                        string idliststring = "";
                        foreach (var mtaid in metaregionlist)
                        {
                            idliststring = idliststring + "'\"" + mtaid + "\"', ";
                        }
                        idliststring = idliststring.Remove(idliststring.Length - 2);

                        where = where + "data->'Id' in (" + idliststring + ")";
                    }

                    var mymetaregion = PostgresSQLHelper.SelectFromTableDataAsObject<MetaRegion>(conn, "metaregions", "*", where, "", 0, null);

                    conn.Close();

                    tourismvereinlist.AddRange(mymetaregion.SelectMany(x => x.TourismvereinIds));
                }

            }

            //Distance
            distance = false;
            distancemin = 0;
            distancemax = 0;
            var distancefilterresult = CommonListCreator.CreateRangeString(distancefilter);
            if (distancefilterresult != null)
            {
                distance = true;
                distancemin = distancefilterresult.Item1 * 1000;
                distancemax = distancefilterresult.Item2 * 1000;
            }

            //Altitude
            altitude = false;
            altitudemin = 0;
            altitudemax = 0;
            var altitudefilterresult = CommonListCreator.CreateRangeString(altitudefilter);
            if (altitudefilterresult != null)
            {
                altitude = true;
                altitudemin = altitudefilterresult.Item1;
                altitudemax = altitudefilterresult.Item2;
            }

            //Duration
            duration = false;
            durationmin = 0;
            durationmax = 0;
            var durationfilterresult = CommonListCreator.CreateRangeString(durationfilter);
            if (durationfilterresult != null)
            {
                duration = true;
                durationmin = durationfilterresult.Item1;
                durationmax = durationfilterresult.Item2;
            }

            //highlight
            highlight = null;
            if (highlightfilter == "true")
                highlight = true;
            if (highlightfilter == "false")
                highlight = false;

            //active
            active = null;
            if (activefilter == "true")
                active = true;
            if (activefilter == "false")
                active = false;

            //smgactive
            smgactive = null;
            if (smgactivefilter == "true")
                smgactive = true;
            if (smgactivefilter == "false")
                smgactive = false;

        }
    }
}