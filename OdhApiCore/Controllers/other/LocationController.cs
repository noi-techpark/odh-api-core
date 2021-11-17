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
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default 'en')</param>
        /// <param name="type">Type ('mta','reg','tvs','mun','fra') Separator ',' : 'null' returns all Location Objects (default)</param>     
        /// <param name="showall">Show all Data (true = all, false = show only data marked as visible)</param>
        /// <param name="locfilter">Locfilter (Separator ',') possible values: mta + MetaREGIONID = (Filter by MetaRegion), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction), (default:'null')</param>
        /// <returns>Reduced List of Locations Objects</returns>        
        [ProducesResponseType(typeof(IEnumerable<LocHelperclass>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Location")]
        public async Task<IActionResult> GetTheLocationList(
            string? language = "en",
            string? type = "null",
            bool showall = true,
            string? locfilter = null)
        {
            return Ok(await GetLocationInfoFiltered(language ?? "en", locfilter, showall, type));            
        } 

        /// <summary>
        /// GET Skiarea List (Use in locfilter as "ska")
        /// </summary>
        /// <param name="language">Localization Language</param>
        /// <param name="locfilter">Locfilter (Separator ',') possible values: mta + MetaREGIONID = (Filter by MetaRegion), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), (default:'null')</param>
        /// <returns>Reduced List of Locations Objects</returns>        
        [ProducesResponseType(typeof(IEnumerable<LocHelperclass>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,GastroReader")]
        [HttpGet, Route("Location/Skiarea")]
        public async Task<IActionResult> GetTheSkiareaList(
            string? language = "en",
            string? locfilter = null)
        {
            return Ok(await GetSkiAreaInfoFiltered(language ?? "en", locfilter));
        }

        #endregion

        #region Private GETTER

        /// <summary>
        /// Get Filtered LocationList based on TV &amp; TVBs
        /// </summary>
        /// <param name="lang">Language</param>
        /// <param name="locfilter">Location Filter</param>
        /// <returns>Collection of Reduced Location Objects</returns>
        private async Task<List<LocHelperclass>> GetLocationInfoFiltered(string lang, string? locfilter, bool allactivedata = false, string? type = null)
        {
            List<LocHelperclass> mylocationlist = new List<LocHelperclass>();
            List<Tuple<string, string>> loclist = new List<Tuple<string, string>>();

            //Split the string
            if (locfilter != null && locfilter != "null")
            {
                if (locfilter.Substring(locfilter.Length - 1, 1) == ",")
                    locfilter = locfilter.Substring(0, locfilter.Length - 1);

                var splittedfilter = locfilter.Split(',');

                foreach (var filter in splittedfilter)
                {
                    string loctype = filter.ToLower().Substring(0, 3);
                    string locid = filter.Substring(3).ToUpper();

                    loclist.Add(Tuple.Create(locid, loctype));
                }
            }

            string defaultmunfrafilter = "data->'VisibleInSearch'='true'";
            if (allactivedata == true)
                defaultmunfrafilter = "data->'Active'='true'";

            List<string> locationtypes = new List<string>() { "mta", "reg", "tvs", "mun", "fra" };  

            if (type != null)
                if(type != "null")
                    locationtypes = type.ToLower().Split(',').ToList();

            if(loclist.Count > 0)
            {
                foreach (var locitem in loclist)
                {
                    var loctype = locitem.Item2;
                    var locid = locitem.Item1;

                    if (loctype == "mta")
                    {
                        var mymetaregion = await QueryFactory.Query()
                               .Select("data")
                               .From("metaregions")
                               .Where("id", locid)
                               .GetObjectSingleAsync<MetaRegion>();

                        var regionlist = mymetaregion.RegionIds ?? Enumerable.Empty<string>();

                        string regionlistwhere = "data->>'RegionId' IN (" + String.Join(",", regionlist) + ")" + locid + "' AND " + defaultmunfrafilter;
                        var myregionlist = await QueryFactory.Query()
                               .Select("data")
                               .From("regions")
                               .WhereRaw(regionlistwhere)
                               .GetObjectListAsync<Region>();

                        string tvlistwhere = "data->>'RegionId' IN (" + String.Join(",", regionlist) + ")" + locid + "' AND " + defaultmunfrafilter;
                        var mytvlist = await QueryFactory.Query()
                               .Select("data")
                               .From("tvs")
                               .WhereRaw(tvlistwhere)
                               .GetObjectListAsync<Tourismverein>();

                        string localitylistwhere = "data->>'RegionId' IN (" + String.Join(",", regionlist) + ")" + locid + "' AND " + defaultmunfrafilter;
                        var mylocalitylist = await QueryFactory.Query()
                               .Select("data")
                               .From("municipalities")
                               .WhereRaw(localitylistwhere)
                               .GetObjectListAsync<Municipality>();

                        string fractionlistwhere = "data->>'RegionId' IN (" + String.Join(",", regionlist) + ")" + locid + "' AND " + defaultmunfrafilter;
                        var myfractionlist = await QueryFactory.Query()
                               .Select("data")
                               .From("districts")
                               .WhereRaw(fractionlistwhere)
                               .GetObjectListAsync<District>();

                        var mymetaregionlistreduced = new LocHelperclass { typ = "reg", name = mymetaregion.Detail[lang].Title, id = mymetaregion.Id };
                        var myregionlistreduced = myregionlist.Select(x => new LocHelperclass { typ = "reg", name = x.Detail[lang].Title, id = x.Id });
                        var mylocalitylistreduced = mylocalitylist.Select(x => new LocHelperclass { typ = "mun", name = x.Detail[lang].Title, id = x.Id });
                        var mytvlistreduced = mytvlist.Select(x => new LocHelperclass { typ = "tvs", name = x.Detail[lang].Title, id = x.Id });
                        var myfractionlistreduced = myfractionlist.Select(x => new LocHelperclass { typ = "fra", name = x.Detail[lang].Title, id = x.Id });

                        if (locationtypes.Contains("mta"))
                            mylocationlist.Add(mymetaregionlistreduced);
                        if (locationtypes.Contains("reg"))
                            mylocationlist.AddRange(myregionlistreduced);
                        if (locationtypes.Contains("tvs"))
                            mylocationlist.AddRange(mylocalitylistreduced);
                        if (locationtypes.Contains("mun"))
                            mylocationlist.AddRange(mytvlistreduced);
                        if (locationtypes.Contains("fra"))
                            mylocationlist.AddRange(myfractionlistreduced);
                    }
                    else if (loctype == "reg")
                    {

                        var myregion = await QueryFactory.Query()
                               .Select("data")
                               .From("regions")
                               .Where("id", locid)
                               .GetObjectSingleAsync<Region>();

                        string tvlistwhere = "data->>'RegionId' = '" + locid + "' AND " + defaultmunfrafilter;
                        var mytvlist = await QueryFactory.Query()
                               .Select("data")
                               .From("tvs")
                               .WhereRaw(tvlistwhere)
                               .GetObjectListAsync<Tourismverein>();

                        string localitylistwhere = "data->>'RegionId' = '" + locid + "' AND " + defaultmunfrafilter;
                        var mylocalitylist = await QueryFactory.Query()
                               .Select("data")
                               .From("municipalities")
                               .WhereRaw(localitylistwhere)
                               .GetObjectListAsync<Municipality>();

                        string fractionlistwhere = "data->>'RegionId' = '" + locid + "' AND " + defaultmunfrafilter;
                        var myfractionlist = await QueryFactory.Query()
                               .Select("data")
                               .From("districts")
                               .WhereRaw(fractionlistwhere)
                               .GetObjectListAsync<District>();

                        var myregionlistreduced = new LocHelperclass { typ = "reg", name = myregion.Detail[lang].Title, id = myregion.Id };
                        var mylocalitylistreduced = mylocalitylist.Select(x => new LocHelperclass { typ = "mun", name = x.Detail[lang].Title, id = x.Id });
                        var mytvlistreduced = mytvlist.Select(x => new LocHelperclass { typ = "tvs", name = x.Detail[lang].Title, id = x.Id });
                        var myfractionlistreduced = myfractionlist.Select(x => new LocHelperclass { typ = "fra", name = x.Detail[lang].Title, id = x.Id });

                        if (locationtypes.Contains("reg"))
                            mylocationlist.Add(myregionlistreduced);
                        if (locationtypes.Contains("tvs"))
                            mylocationlist.AddRange(mylocalitylistreduced);
                        if (locationtypes.Contains("mun"))
                            mylocationlist.AddRange(mytvlistreduced);
                        if (locationtypes.Contains("fra"))
                            mylocationlist.AddRange(myfractionlistreduced);
                    }
                    else if (loctype == "tvs")
                    {
                        var mytv = await QueryFactory.Query()
                               .Select("data")
                               .From("tvs")
                               .Where("id", locid)
                               .GetObjectSingleAsync<Tourismverein>();

                        string localitylistwhere = "data->>'TourismvereinId' = '" + locid + "' AND " + defaultmunfrafilter;
                        var mylocalitylist = await QueryFactory.Query()
                               .Select("data")
                               .From("municipalities")
                               .WhereRaw(localitylistwhere)
                               .GetObjectListAsync<Municipality>();

                        string fractionlistwhere = "data->>'TourismvereinId' = '" + locid + "' AND " + defaultmunfrafilter;
                        var myfractionlist = await QueryFactory.Query()
                               .Select("data")
                               .From("districts")
                               .WhereRaw(fractionlistwhere)
                               .GetObjectListAsync<District>();

                        var mytvlistreduced = new LocHelperclass { typ = "tvs", name = mytv.Detail[lang].Title, id = mytv.Id };
                        var mylocalitylistreduced = mylocalitylist.Select(x => new LocHelperclass { typ = "mun", name = x.Detail[lang].Title, id = x.Id });
                        var myfractionlistreduced = myfractionlist.Select(x => new LocHelperclass { typ = "fra", name = x.Detail[lang].Title, id = x.Id });

                        if (locationtypes.Contains("tvs"))
                            mylocationlist.Add(mytvlistreduced);
                        if (locationtypes.Contains("mun"))
                            mylocationlist.AddRange(mylocalitylistreduced);
                        if (locationtypes.Contains("fra"))
                            mylocationlist.AddRange(myfractionlistreduced);
                    }
                    else if (loctype == "mun")
                    {
                        var mymun = await QueryFactory.Query()
                               .Select("data")
                               .From("municipalities")
                               .Where("id", locid)
                               .GetObjectSingleAsync<Municipality>();

                        string fractionlistwhere = "data->>'MunicipalityId' = '" + locid + "' AND " + defaultmunfrafilter;
                        var myfractionlist = await QueryFactory.Query()
                               .Select("data")
                               .From("districts")
                               .WhereRaw(fractionlistwhere)
                               .GetObjectListAsync<District>();

                        var mylocalitylistreduced = new LocHelperclass { typ = "mun", name = mymun.Detail[lang].Title, id = mymun.Id };
                        var myfractionlistreduced = myfractionlist.Select(x => new LocHelperclass { typ = "fra", name = x.Detail[lang].Title, id = x.Id });

                        if (locationtypes.Contains("mun"))
                            mylocationlist.Add(mylocalitylistreduced);
                        if (locationtypes.Contains("fra"))
                            mylocationlist.AddRange(myfractionlistreduced);
                    }
                    else if (loctype == "fra")
                    {
                        var myfra = await QueryFactory.Query()
                               .Select("data")
                               .From("districts")
                               .Where("id", locid)
                               .GetObjectSingleAsync<District>();

                        var myfralistreduced = new LocHelperclass { typ = "fra", name = myfra.Detail[lang].Title, id = myfra.Id };

                        if (locationtypes.Contains("fra"))
                            mylocationlist.Add(myfralistreduced);
                    } 
                }
            }
            else
            {
                if (locationtypes.Contains("mta"))
                {
                    var mymetaregion = await QueryFactory.Query()
                       .Select("data")
                       .From("metaregions")
                       .WhereRaw("data->'Active'='true'")
                       .GetObjectListAsync<MetaRegion>();

                    var mymetaregionlistreduced = mymetaregion.Select(x => new LocHelperclass { typ = "mta", name = x.Detail[lang].Title, id = x.Id });
                    mylocationlist.AddRange(mymetaregionlistreduced);
                }

                if (locationtypes.Contains("reg"))
                {
                    var myregion = await QueryFactory.Query()
                       .Select("data")
                       .From("regions")
                       .WhereRaw("data->'Active'='true'")
                       .GetObjectListAsync<Region>();

                    var myregionlistreduced = myregion.Select(x => new LocHelperclass { typ = "reg", name = x.Detail[lang].Title, id = x.Id });
                    mylocationlist.AddRange(myregionlistreduced);
                }

                if (locationtypes.Contains("tvs"))
                {
                    var mytv = await QueryFactory.Query()
                       .Select("data")
                       .From("tvs")
                       .WhereRaw("data->'Active'='true'")
                       .GetObjectListAsync<Tourismverein>();

                    var mytourismvereinlistreduced = mytv.Select(x => new LocHelperclass { typ = "tvs", name = x.Detail[lang].Title, id = x.Id });
                    mylocationlist.AddRange(mytourismvereinlistreduced);
                }

                if (locationtypes.Contains("mun"))
                {
                    var mylocalitylist = await QueryFactory.Query()
                       .Select("data")
                       .From("municipalities")
                       .WhereRaw(defaultmunfrafilter)
                       .GetObjectListAsync<Municipality>();

                    var mylocalitylistreduced = mylocalitylist.Select(x => new LocHelperclass { typ = "mun", name = x.Detail[lang].Title, id = x.Id });
                    mylocationlist.AddRange(mylocalitylistreduced);
                }

                if (locationtypes.Contains("fra"))
                {
                    var myfractionlist = await QueryFactory.Query()
                       .Select("data")
                       .From("districts")
                       .WhereRaw(defaultmunfrafilter)
                       .GetObjectListAsync<District>();

                    var myfractionlistreduced = myfractionlist.Select(x => new LocHelperclass { typ = "fra", name = x.Detail[lang].Title, id = x.Id });
                    mylocationlist.AddRange(myfractionlistreduced);
                }
            }

            return mylocationlist;
        }

        /// <summary>
        /// Get Filtered SkiAreaList based on TV &amp; TVBs
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="locfilter"></param>
        /// <returns>Collection of Reduced Location Objects</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<List<LocHelperclass>> GetSkiAreaInfoFiltered(string lang, string? locfilter)
        {
            List<LocHelperclass> mylocationlist = new List<LocHelperclass>();

            List<Tuple<string, string>> loclist = new List<Tuple<string, string>>();

            //Split the string
            if (locfilter != null && locfilter != "null")
            {
                if (locfilter.Substring(locfilter.Length - 1, 1) == ",")
                    locfilter = locfilter.Substring(0, locfilter.Length - 1);

                var splittedfilter = locfilter.Split(',');

                foreach (var filter in splittedfilter)
                {
                    string loctype = filter.ToLower().Substring(0, 3);
                    string locid = filter.Substring(3).ToUpper();

                    loclist.Add(Tuple.Create(locid, loctype));
                }
            }

            if (loclist.Count > 0)
            {
                foreach (var locitem in loclist)
                {
                    var loctype = locitem.Item2;
                    var locid = locitem.Item1;

                    if (loctype == "tvs")
                    {
                        //string skiarealistwhere = "data @> '{ \"TourismvereinIds\": [\"" + locid + "\"]}'";
                        //var myskiarealist = PostgresSQLHelper.SelectFromTableDataAsObject<SkiArea>(conn, "skiareas", "*", skiarealistwhere, "", 0, null);

                        var myskiarealistquery =
                           QueryFactory.Query()
                               .Select("data")
                               .From("skiareas")
                               .WhereInJsonb(
                                new List<string>() { locid },
                                tvid => new { TourismvereinIds = new[] { tvid.ToUpper() } }
                               );

                        var myskiarealist = await myskiarealistquery.GetObjectListAsync<SkiArea>();

                        var myskiarealistreduced = myskiarealist.Select(x => new LocHelperclass { typ = "ska", name = x.Detail[lang].Title, id = x.Id });

                        mylocationlist.AddRange(myskiarealistreduced);
                    }
                    else if (loctype == "reg")
                    {
                        //string skiarealistwhere = "data @> '{ \"RegionIds\": [\"" + locid + "\"]}'";

                        var myskiarealistquery =
                           QueryFactory.Query()
                               .Select("data")
                               .From("skiareas")
                                .WhereInJsonb(
                                new List<string>() { locid },
                                regid => new { RegionIds = new[] { regid.ToUpper() } }
                               );

                        var myskiarealist = await myskiarealistquery.GetObjectListAsync<SkiArea>();

                        var myskiarealistreduced = myskiarealist.Select(x => new LocHelperclass { typ = "ska", name = x.Detail[lang].Title, id = x.Id });

                        mylocationlist.AddRange(myskiarealistreduced);
                    }
                    else if (loctype == "mta")
                    {
                        var mymetaregion = await QueryFactory.Query()
                               .Select("data")
                               .From("metaregions")
                               .Where("id", locid)
                               .GetObjectSingleAsync<MetaRegion>();

                        //string regionfilter = "";
                        //foreach (var regionid in mymetaregion.RegionIds ?? new List<string>())
                        //{
                        //    regionfilter = regionfilter + "data @> '{ \"RegionIds\": [\"" + regionid + "\"]}' OR ";
                        //}

                        //regionfilter = regionfilter.Remove(regionfilter.Length - 4);

                        var myskiarealistquery =
                          QueryFactory.Query()
                              .Select("data")
                              .From("skiareas")
                              .WhereInJsonb(
                                 mymetaregion.RegionIds?.ToList() ?? new List<string>(),
                                regid => new { RegionIds = new[] { regid.ToUpper() } }
                               );

                        var myskiarealist = await myskiarealistquery.GetObjectListAsync<SkiArea>();

                        var myskiarealistreduced = myskiarealist.Select(x => new LocHelperclass { typ = "ska", name = x.Detail[lang].Title, id = x.Id });

                        mylocationlist.AddRange(myskiarealistreduced);
                    }
                }
            }
            else
            {
                var myskiarealistquery =
                QueryFactory.Query()
                    .Select("data")
                    .From("skiareas");

                var myskiarealist = await myskiarealistquery.GetObjectListAsync<SkiArea>();

                var myskiarealistreduced = myskiarealist.Select(x => new LocHelperclass { typ = "ska", name = x.Detail[lang].Title, id = x.Id });

                mylocationlist.AddRange(myskiarealistreduced);
            }

            //TO TEST
            List<JsonRaw> mylocationlistraw = mylocationlist.Select(x => new JsonRaw(JsonConvert.SerializeObject(x))).ToList();

            return mylocationlist;
        }

        #endregion
    }
}
