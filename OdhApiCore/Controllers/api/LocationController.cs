using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DataModel;
using Helper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SqlKata.Execution;

namespace OdhApiCore.Controllers.api
{
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class LocationController : OdhController
    {
        private readonly IHttpClientFactory httpClientFactory;

        public LocationController(IWebHostEnvironment env, ISettings settings, ILogger<AccommodationController> logger, QueryFactory queryFactory, IHttpClientFactory httpClientFactory)
           : base(env, settings, logger, queryFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        #region SWAGGER Exposed Api

        /// <summary>
        /// GET Location List (Use in locfilter)
        /// </summary>
        /// <param name="language">Localization Language, (default:'en')</param>
        /// <param name="type">Type ('mta','reg','tvs','mun','fra') Separator ',' : 'null' returns all Location Objects (default)</param>     
        /// <param name="showall">Show all Data (true = all, false = show only data market as visible)</param>
        /// <param name="locfilter">Locfilter (Separator ',' possible values: mta + MetaREGIONID = (Filter by MetaRegion), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction)), (default:'null')</param>
        /// <returns>Reduced List of Locations Objects</returns>        
        [ProducesResponseType(typeof(IEnumerable<LocHelperclass>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("api/Location")]
        public async Task<IActionResult> GetTheLocationList(
            string language = "en",
            string type = "null",
            bool showall = true,
            string locfilter = "null")
        {
            //var result = default(List<LocHelperclass>);

            //if (type == "null" && locfilter == "null")
            //    result = await GetLocationInfoFiltered(language, showall);
            //else if (type != "null" && locfilter == "null")
            //    result = await GetLocationInfoFilteredByType(language, type, showall);
            //else
            //    result = await ;

            //return Ok(await GetLocationInfoFiltered(language, locfilter, showall, type));

            return null;
        } 

        /// <summary>
        /// GET Skiarea List (Use in locfilter as "ska")
        /// </summary>
        /// <param name="language">Localization Language, (default:'en')</param>
        /// <param name="locfilter">Locfilter (Separator ',' possible values: mta + MetaREGIONID = (Filter by MetaRegion), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), (default:'null')</param>
        /// <returns>Reduced List of Locations Objects</returns>        
        [ProducesResponseType(typeof(IEnumerable<LocHelperclass>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,GastroReader")]
        [HttpGet, Route("api/Location/Skiarea")]
        public async Task<IActionResult> GetTheSkiareaList(
            string language = "en",
            string locfilter = "null")
        {
            //return Ok(await GetSkiAreaInfoFiltered(language, locfilter));

            return null;
        }

        #endregion

        #region Private GETTER

        ///// <summary>
        ///// Get Filtered LocationList based on TV & TVBs
        ///// </summary>
        ///// <param name="lang">Language</param>
        ///// <param name="locfilter">Location Filter</param>
        ///// <returns>Collection of Reduced Location Objects</returns>
        //private async Task<List<LocHelperclass>> GetLocationInfoFiltered(string lang, string locfilter, bool allactivedata = false, string type = "null")
        //{
        //    List<LocHelperclass> mylocationlist = new List<LocHelperclass>();

        //    string loctype = locfilter.ToLower().Substring(0, 3);
        //    string locid = locfilter.Substring(3).ToUpper();

        //    string defaultmunfrafilter = "data->'VisibleInSearch'='true'";
        //    if (allactivedata)
        //        defaultmunfrafilter = "data->'Active'='true'";

        //    List<string> locationtypes = new List<string>() { "mta", "reg", "tvs", "mun", "", "fra" };

        //    if (type != "null")
        //        locationtypes = type.ToLower().Split(',').ToList();


        //    if (loctype == "mta")
        //    {
        //        using (var conn = new NpgsqlConnection(GlobalPGConnection.PGConnectionString))
        //        {
        //            conn.Open();

        //            var mymetaregion = PostgresSQLHelper.SelectFromTableDataAsObject<MetaRegion>(conn, "metaregions", "*", "Id = '" + locid + "'", "", 0, null).FirstOrDefault();

        //            var regionlist = mymetaregion.RegionIds;

        //            string regionlistwhere = "data->>'RegionId' IN (" + String.Join(",", regionlist) + ")" + locid + "' AND " + defaultmunfrafilter;
        //            var myregionlist = PostgresSQLHelper.SelectFromTableDataAsObject<Region>(conn, "regions", "*", regionlistwhere, "", 0, null);

        //            string tvlistwhere = "data->>'RegionId' IN (" + String.Join(",", regionlist) + ")" + locid + "' AND " + defaultmunfrafilter;
        //            var mytvlist = PostgresSQLHelper.SelectFromTableDataAsObject<Tourismverein>(conn, "tvs", "*", tvlistwhere, "", 0, null);

        //            string localitylistwhere = "data->>'RegionId' IN (" + String.Join(",", regionlist) + ")" + locid + "' AND " + defaultmunfrafilter;
        //            var mylocalitylist = PostgresSQLHelper.SelectFromTableDataAsObject<Municipality>(conn, "municipalities", "*", localitylistwhere, "", 0, null);

        //            string fractionlistwhere = "data->>'RegionId' IN (" + String.Join(",", regionlist) + ")" + locid + "' AND " + defaultmunfrafilter;
        //            var myfractionlist = PostgresSQLHelper.SelectFromTableDataAsObject<District>(conn, "districts", "*", fractionlistwhere, "", 0, null);


        //            var mymetaregionlistreduced = new LocHelperclass { typ = "reg", name = mymetaregion.Detail[lang].Title, id = mymetaregion.Id };
        //            var myregionlistreduced = myregionlist.Select(x => new LocHelperclass { typ = "reg", name = x.Detail[lang].Title, id = x.Id });
        //            var mylocalitylistreduced = mylocalitylist.Select(x => new LocHelperclass { typ = "mun", name = x.Detail[lang].Title, id = x.Id });
        //            var mytvlistreduced = mytvlist.Select(x => new LocHelperclass { typ = "tvs", name = x.Detail[lang].Title, id = x.Id });
        //            var myfractionlistreduced = myfractionlist.Select(x => new LocHelperclass { typ = "fra", name = x.Detail[lang].Title, id = x.Id });

        //            if (locationtypes.Contains("mta"))
        //                mylocationlist.Add(mymetaregionlistreduced);
        //            if (locationtypes.Contains("reg"))
        //                mylocationlist.AddRange(myregionlistreduced);
        //            if (locationtypes.Contains("tvs"))
        //                mylocationlist.AddRange(mylocalitylistreduced);
        //            if (locationtypes.Contains("mun"))
        //                mylocationlist.AddRange(mytvlistreduced);
        //            if (locationtypes.Contains("fra"))
        //                mylocationlist.AddRange(myfractionlistreduced);
        //            conn.Close();
        //        }
        //    }
        //    if (loctype == "reg")
        //    {
        //        using (var conn = new NpgsqlConnection(GlobalPGConnection.PGConnectionString))
        //        {
        //            conn.Open();

        //            var myregion = PostgresSQLHelper.SelectFromTableDataAsObject<Region>(conn, "regions", "*", "Id = '" + locid + "'", "", 0, null).FirstOrDefault();

        //            string tvlistwhere = "data->>'RegionId' = '" + locid + "' AND " + defaultmunfrafilter;
        //            var mytvlist = PostgresSQLHelper.SelectFromTableDataAsObject<Tourismverein>(conn, "tvs", "*", tvlistwhere, "", 0, null);

        //            string localitylistwhere = "data->>'RegionId' = '" + locid + "' AND " + defaultmunfrafilter;
        //            var mylocalitylist = PostgresSQLHelper.SelectFromTableDataAsObject<Municipality>(conn, "municipalities", "*", localitylistwhere, "", 0, null);

        //            string fractionlistwhere = "data->>'RegionId' = '" + locid + "' AND " + defaultmunfrafilter;
        //            var myfractionlist = PostgresSQLHelper.SelectFromTableDataAsObject<District>(conn, "districts", "*", fractionlistwhere, "", 0, null);


        //            var myregionlistreduced = new LocHelperclass { typ = "reg", name = myregion.Detail[lang].Title, id = myregion.Id };
        //            var mylocalitylistreduced = mylocalitylist.Select(x => new LocHelperclass { typ = "mun", name = x.Detail[lang].Title, id = x.Id });
        //            var mytvlistreduced = mytvlist.Select(x => new LocHelperclass { typ = "tvs", name = x.Detail[lang].Title, id = x.Id });
        //            var myfractionlistreduced = myfractionlist.Select(x => new LocHelperclass { typ = "fra", name = x.Detail[lang].Title, id = x.Id });

        //            if (locationtypes.Contains("reg"))
        //                mylocationlist.Add(myregionlistreduced);
        //            if (locationtypes.Contains("tvs"))
        //                mylocationlist.AddRange(mylocalitylistreduced);
        //            if (locationtypes.Contains("mun"))
        //                mylocationlist.AddRange(mytvlistreduced);
        //            if (locationtypes.Contains("fra"))
        //                mylocationlist.AddRange(myfractionlistreduced);
        //            conn.Close();
        //        }
        //    }
        //    if (loctype == "tvs")
        //    {
        //        using (var conn = new NpgsqlConnection(GlobalPGConnection.PGConnectionString))
        //        {
        //            conn.Open();

        //            var mytv = PostgresSQLHelper.SelectFromTableDataAsObject<Tourismverein>(conn, "tvs", "*", "Id = '" + locid + "'", "", 0, null).FirstOrDefault();

        //            string localitylistwhere = "data->>'TourismvereinId' = '" + locid + "' AND " + defaultmunfrafilter;
        //            var mylocalitylist = PostgresSQLHelper.SelectFromTableDataAsObject<Municipality>(conn, "municipalities", "*", localitylistwhere, "", 0, null);

        //            string fractionlistwhere = "data->>'TourismvereinId' = '" + locid + "' AND " + defaultmunfrafilter;
        //            var myfractionlist = PostgresSQLHelper.SelectFromTableDataAsObject<District>(conn, "districts", "*", fractionlistwhere, "", 0, null);

        //            var mytvlistreduced = new LocHelperclass { typ = "tvs", name = mytv.Detail[lang].Title, id = mytv.Id };
        //            var mylocalitylistreduced = mylocalitylist.Select(x => new LocHelperclass { typ = "mun", name = x.Detail[lang].Title, id = x.Id });
        //            var myfractionlistreduced = myfractionlist.Select(x => new LocHelperclass { typ = "fra", name = x.Detail[lang].Title, id = x.Id });

        //            if (locationtypes.Contains("tvs"))
        //                mylocationlist.Add(mytvlistreduced);
        //            if (locationtypes.Contains("mun"))
        //                mylocationlist.AddRange(mylocalitylistreduced);
        //            if (locationtypes.Contains("fra"))
        //                mylocationlist.AddRange(myfractionlistreduced);

        //            conn.Close();
        //        }
        //    }
        //    if (loctype == "mun")
        //    {
        //        using (var conn = new NpgsqlConnection(GlobalPGConnection.PGConnectionString))
        //        {
        //            conn.Open();

        //            var mymun = PostgresSQLHelper.SelectFromTableDataAsObject<Municipality>(conn, "mun", "*", "Id = '" + locid + "'", "", 0, null).FirstOrDefault();

        //            string fractionlistwhere = "data->>'MunicipalityId' = '" + locid + "' AND " + defaultmunfrafilter;
        //            var myfractionlist = PostgresSQLHelper.SelectFromTableDataAsObject<District>(conn, "districts", "*", fractionlistwhere, "", 0, null);

        //            var mylocalitylistreduced = new LocHelperclass { typ = "mun", name = mymun.Detail[lang].Title, id = mymun.Id };
        //            var myfractionlistreduced = myfractionlist.Select(x => new LocHelperclass { typ = "fra", name = x.Detail[lang].Title, id = x.Id });

        //            if (locationtypes.Contains("mun"))
        //                mylocationlist.Add(mylocalitylistreduced);
        //            if (locationtypes.Contains("fra"))
        //                mylocationlist.AddRange(myfractionlistreduced);

        //            conn.Close();
        //        }
        //    }
        //    if (loctype == "fra")
        //    {
        //        using (var conn = new NpgsqlConnection(GlobalPGConnection.PGConnectionString))
        //        {
        //            conn.Open();

        //            var myfra = PostgresSQLHelper.SelectFromTableDataAsObject<District>(conn, "districts", "*", "Id = '" + locid + "'", "", 0, null).FirstOrDefault();

        //            var myfralistreduced = new LocHelperclass { typ = "fra", name = myfra.Detail[lang].Title, id = myfra.Id };

        //            if (locationtypes.Contains("fra"))
        //                mylocationlist.Add(myfralistreduced);

        //            conn.Close();
        //        }
        //    }
        //    else
        //    {
        //        if (locationtypes.Contains("mta"))
        //        {
        //            var mymetaregion = PostgresSQLHelper.SelectFromTableDataAsObject<MetaRegion>(conn, "metaregions", "*", "data->'Active'='true'", "", 0, null);

        //            var mymetaregionlistreduced = mymetaregion.Select(x => new LocHelperclass { typ = "mta", name = x.Detail[lang].Title, id = x.Id });
        //            mylocationlist.AddRange(mymetaregionlistreduced);
        //        }

        //        if (locationtypes.Contains("reg"))
        //        {
        //            var myregion = PostgresSQLHelper.SelectFromTableDataAsObject<Region>(conn, "regions", "*", "data->'Active'='true'", "", 0, null);

        //            var myregionlistreduced = myregion.Select(x => new LocHelperclass { typ = "reg", name = x.Detail[lang].Title, id = x.Id });
        //            mylocationlist.AddRange(myregionlistreduced);
        //        }

        //        if (locationtypes.Contains("tvs"))
        //        {
        //            var mytv = PostgresSQLHelper.SelectFromTableDataAsObject<Tourismverein>(conn, "tvs", "*", "data->'Active'='true'", "", 0, null);

        //            var mytourismvereinlistreduced = mytv.Select(x => new LocHelperclass { typ = "tvs", name = x.Detail[lang].Title, id = x.Id });
        //            mylocationlist.AddRange(mytourismvereinlistreduced);
        //        }

        //        if (locationtypes.Contains("mun"))
        //        {
        //            var mylocalitylist = PostgresSQLHelper.SelectFromTableDataAsObject<Municipality>(conn, "municipalities", "*", defaultmunfrafilter, "", 0, null);

        //            var mylocalitylistreduced = mylocalitylist.Select(x => new LocHelperclass { typ = "mun", name = x.Detail[lang].Title, id = x.Id });
        //            mylocationlist.AddRange(mylocalitylistreduced);
        //        }

        //        if (locationtypes.Contains("fra"))
        //        {
        //            var myfractionlist = PostgresSQLHelper.SelectFromTableDataAsObject<District>(conn, "districts", "*", defaultmunfrafilter, "", 0, null);

        //            var myfractionlistreduced = myfractionlist.Select(x => new LocHelperclass { typ = "fra", name = x.Detail[lang].Title, id = x.Id });
        //            mylocationlist.AddRange(myfractionlistreduced);
        //        }
        //    }

        //    return mylocationlist;
        //}


        ///// <summary>
        ///// Get Filtered SkiAreaList based on TV & TVBs
        ///// </summary>
        ///// <param name="lang"></param>
        ///// <param name="locfilter"></param>
        ///// <returns>Collection of Reduced Location Objects</returns>
        //[ApiExplorerSettings(IgnoreApi = true)]
        //private async Task<List<LocHelperclass>> GetSkiAreaInfoFiltered(string lang, string locfilter)
        //{
        //    List<LocHelperclass> mylocationlist = new List<LocHelperclass>();

        //    string loctype = "";
        //    string locid = "";

        //    if (locfilter == "null")
        //    {
        //        loctype = "";
        //    }
        //    else
        //    {
        //        loctype = locfilter.Substring(0, 3);
        //        locid = locfilter.Substring(3).ToUpper();
        //    }


        //    if (loctype == "tvs")
        //    {
        //        string skiarealistwhere = "data @> '{ \"TourismvereinIds\": [\"" + locid + "\"]}'";
        //        //var myskiarealist = PostgresSQLHelper.SelectFromTableDataAsObject<SkiArea>(conn, "skiareas", "*", skiarealistwhere, "", 0, null);

        //        var myskiarealistquery =
        //           QueryFactory.Query()
        //               .Select("data")
        //               .From("municipalities")
        //               .WhereRaw(skiarealistwhere);

        //        var myskiareliststring = await myskiarealistquery.GetAreaforRegionPGAsync GetAsync<string>();
        //        var myskiarelist = JsonConvert.DeserializeObject<IEnumerable<SkiArea>>(myskiareliststring);

        //        var myskiarealistreduced = myskiarelist.Select(x => new LocHelperclass { typ = "ska", name = x.Detail[lang].Title, id = x.Id });

        //        mylocationlist.AddRange(myskiarealistreduced);
        //    }
        //        if (loctype == "reg")
        //        {
        //            string skiarealistwhere = "data @> '{ \"RegionIds\": [\"" + locid + "\"]}'";
        //            var myskiarealist = PostgresSQLHelper.SelectFromTableDataAsObject<SkiArea>(conn, "skiareas", "*", skiarealistwhere, "", 0, null);

        //            var myskiarealistreduced = myskiarealist.Select(x => new LocHelperclass { typ = "ska", name = x.Detail[lang].Title, id = x.Id });

        //            mylocationlist.AddRange(myskiarealistreduced);
        //        }
        //        if (loctype == "mta")
        //        {
        //            var mymetaregion = PostgresSQLHelper.SelectFromTableDataAsObject<MetaRegion>(conn, "metaregions", "*", "Id = '" + locid + "'", "", 0, null).FirstOrDefault();

        //            string regionfilter = "";
        //            foreach (var regionid in mymetaregion.RegionIds)
        //            {
        //                regionfilter = regionfilter + "data @> '{ \"RegionIds\": [\"" + regionid + "\"]}' OR ";
        //            }

        //            regionfilter = regionfilter.Remove(regionfilter.Length - 4);

        //            var myskiarealist = PostgresSQLHelper.SelectFromTableDataAsObject<SkiArea>(conn, "skiareas", "*", regionfilter, "", 0, null);

        //            var myskiarealistreduced = myskiarealist.Select(x => new LocHelperclass { typ = "ska", name = x.Detail[lang].Title, id = x.Id });

        //            mylocationlist.AddRange(myskiarealistreduced);
        //        }
        //    if (String.IsNullOrEmpty(loctype))
        //    {
        //        var myskiarealist = PostgresSQLHelper.SelectFromTableDataAsObject<SkiArea>(conn, "skiareas", "*", "", "", 0, null);

        //        var myskiarealistreduced = myskiarealist.Select(x => new LocHelperclass { typ = "ska", name = x.Detail[lang].Title, id = x.Id });

        //        mylocationlist.AddRange(myskiarealistreduced);
        //    }               

        //    return mylocationlist;
        //}

        #endregion
    }
}
