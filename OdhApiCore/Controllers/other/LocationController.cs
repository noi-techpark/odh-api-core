using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading;
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
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="rawfilter"><a href='https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter' target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href='https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter' target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Reduced List of Locations Objects</returns>        
        [ProducesResponseType(typeof(IEnumerable<LocHelperclass>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Location")]
        public async Task<IActionResult> GetTheLocationList(
            string? language = "en",
            string? type = "null",
            bool showall = true,
            string? locfilter = null,
            CancellationToken cancellationToken = default)
        {
            return await GetLocationInfoFiltered(language, locfilter, showall, type, cancellationToken);            
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
            string? locfilter = null,
            CancellationToken cancellationToken = default)
        {
            return await GetSkiAreaInfoFiltered(language ?? "en", locfilter, cancellationToken);
        }

        #endregion

        #region Private GETTER

        /// <summary>
        /// Get Filtered LocationList based on TV &amp; TVBs
        /// </summary>
        /// <param name="lang">Language</param>
        /// <param name="locfilter">Location Filter</param>
        /// <returns>Collection of Reduced Location Objects</returns>
        private async Task<IActionResult> GetLocationInfoFiltered(string lang, string? locfilter, bool allactivedata, string? type, CancellationToken cancellationToken)
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

            var mymetaregionlistreduced = default(IEnumerable<LocHelperclassDynamic>);
            var myregionlistreduced = default(IEnumerable<LocHelperclassDynamic>);
            var mylocalitylistreduced = default(IEnumerable<LocHelperclassDynamic>);
            var mytvlistreduced = default(IEnumerable<LocHelperclassDynamic>);
            var myfractionlistreduced = default(IEnumerable<LocHelperclassDynamic>);            

            if (loclist.Count > 0)
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

                        //var mymetaregionlistreduced = new LocHelperclassDynamic { typ = "mta", name = mymetaregion.Detail[lang].Title, id = mymetaregion.Id };
                        //var myregionlistreduced = myregionlist.Select(x => new LocHelperclass { typ = "reg", name = x.Detail[lang].Title, id = x.Id });
                        //var mylocalitylistreduced = mylocalitylist.Select(x => new LocHelperclass { typ = "mun", name = x.Detail[lang].Title, id = x.Id });
                        //var mytvlistreduced = mytvlist.Select(x => new LocHelperclass { typ = "tvs", name = x.Detail[lang].Title, id = x.Id });
                        //var myfractionlistreduced = myfractionlist.Select(x => new LocHelperclass { typ = "fra", name = x.Detail[lang].Title, id = x.Id });

                        mymetaregionlistreduced = CreateLocHelperClassDynamic<MetaRegion>("mta", new List<MetaRegion>() { mymetaregion }, lang);
                        myregionlistreduced = CreateLocHelperClassDynamic<Region>("reg", myregionlist, lang);
                        mylocalitylistreduced = CreateLocHelperClassDynamic<Municipality>("mun", mylocalitylist, lang);
                        mytvlistreduced = CreateLocHelperClassDynamic<Tourismverein>("tvs", mytvlist, lang);
                        myfractionlistreduced = CreateLocHelperClassDynamic<District>("fra", myfractionlist, lang);

                        if (locationtypes.Contains("mta"))
                            mylocationlist.AddRange(mymetaregionlistreduced);
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

                        myregionlistreduced = new List<LocHelperclassDynamic> { new LocHelperclassDynamic { typ = "reg", name = myregion.Detail[lang].Title, id = myregion.Id } };
                        mylocalitylistreduced = mylocalitylist.Select(x => new LocHelperclassDynamic { typ = "mun", name = x.Detail[lang].Title, id = x.Id });
                        mytvlistreduced = mytvlist.Select(x => new LocHelperclassDynamic { typ = "tvs", name = x.Detail[lang].Title, id = x.Id });
                        myfractionlistreduced = myfractionlist.Select(x => new LocHelperclassDynamic { typ = "fra", name = x.Detail[lang].Title, id = x.Id });

                        myregionlistreduced = CreateLocHelperClassDynamic<Region>("reg", new List<Region>() { myregion }, lang);
                        mylocalitylistreduced = CreateLocHelperClassDynamic<Municipality>("mun", mylocalitylist, lang);
                        mytvlistreduced = CreateLocHelperClassDynamic<Tourismverein>("tvs", mytvlist, lang);
                        myfractionlistreduced = CreateLocHelperClassDynamic<District>("fra", myfractionlist, lang);

                        if (locationtypes.Contains("reg"))
                            mylocationlist.Add(myregionlistreduced.FirstOrDefault());
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

                        mytvlistreduced = new List<LocHelperclassDynamic>() { new LocHelperclassDynamic { typ = "tvs", name = mytv.Detail[lang].Title, id = mytv.Id } };
                        mylocalitylistreduced = mylocalitylist.Select(x => new LocHelperclassDynamic { typ = "mun", name = x.Detail[lang].Title, id = x.Id });
                        myfractionlistreduced = myfractionlist.Select(x => new LocHelperclassDynamic { typ = "fra", name = x.Detail[lang].Title, id = x.Id });
                                                ;
                        mytvlistreduced = CreateLocHelperClassDynamic<Tourismverein>("tvs", new List<Tourismverein>() { mytv }, lang);
                        mylocalitylistreduced = CreateLocHelperClassDynamic<Municipality>("mun", mylocalitylist, lang);                        
                        myfractionlistreduced = CreateLocHelperClassDynamic<District>("fra", myfractionlist, lang);

                        if (locationtypes.Contains("tvs"))
                            mylocationlist.Add(mytvlistreduced.FirstOrDefault());
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

                        mylocalitylistreduced = CreateLocHelperClassDynamic<Municipality>("mun", new List<Municipality>() { mymun }, lang);
                        myfractionlistreduced = CreateLocHelperClassDynamic<District>("fra", myfractionlist, lang);

                        if (locationtypes.Contains("mun"))
                            mylocationlist.Add(mylocalitylistreduced.FirstOrDefault());
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

                        myfractionlistreduced = CreateLocHelperClassDynamic<District>("fra", new List<District>() { myfra }, lang);

                        if (locationtypes.Contains("fra"))
                            mylocationlist.Add(myfractionlistreduced.FirstOrDefault());
                    } 
                }
            }
            else
            {
                if (locationtypes.Contains("mta"))
                {
                    var mymetaregionlist = await QueryFactory.Query()
                       .Select("data")
                       .From("metaregions")
                       .WhereRaw("data->'Active'='true'")
                       .GetObjectListAsync<MetaRegion>();

                   mymetaregionlistreduced = CreateLocHelperClassDynamic<MetaRegion>("mta", mymetaregionlist, lang);
                   
                    mylocationlist.AddRange(mymetaregionlistreduced);                   
                }

                if (locationtypes.Contains("reg"))
                {
                    var myregionlist = await QueryFactory.Query()
                       .Select("data")
                       .From("regions")
                       .WhereRaw("data->'Active'='true'")
                       .GetObjectListAsync<Region>();

                    myregionlistreduced = CreateLocHelperClassDynamic<Region>("reg", myregionlist, lang);

                    mylocationlist.AddRange(myregionlistreduced);
                }

                if (locationtypes.Contains("tvs"))
                {
                    var mytvlist = await QueryFactory.Query()
                       .Select("data")
                       .From("tvs")
                       .WhereRaw("data->'Active'='true'")
                       .GetObjectListAsync<Tourismverein>();

                   mytvlistreduced = CreateLocHelperClassDynamic<Tourismverein>("tvs", mytvlist, lang);
                   
                    mylocationlist.AddRange(mytvlistreduced);
                }

                if (locationtypes.Contains("mun"))
                {
                    var mylocalitylist = await QueryFactory.Query()
                       .Select("data")
                       .From("municipalities")
                       .WhereRaw(defaultmunfrafilter)
                       .GetObjectListAsync<Municipality>();

                   
                    mylocalitylistreduced = CreateLocHelperClassDynamic<Municipality>("mun", mylocalitylist, lang);
                   
                    mylocationlist.AddRange(mylocalitylistreduced);
                }

                if (locationtypes.Contains("fra"))
                {
                    var myfractionlist = await QueryFactory.Query()
                       .Select("data")
                       .From("districts")
                       .WhereRaw(defaultmunfrafilter)
                       .GetObjectListAsync<District>();

                    myfractionlistreduced = CreateLocHelperClassDynamic<District>("fra", myfractionlist, lang);

                    mylocationlist.AddRange(myfractionlistreduced);
                }
            }

            //return mylocationlist;

            //Transform to JsonRAW List
            var jsonrawlist = mylocationlist.Select(x => new JsonRaw(x)).ToList();
            return Ok(jsonrawlist);
        }

        /// <summary>
        /// Get Filtered SkiAreaList based on TV &amp; TVBs
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="locfilter"></param>
        /// <returns>Collection of Reduced Location Objects</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<IActionResult> GetSkiAreaInfoFiltered(string lang, string? locfilter, CancellationToken cancellationToken)
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

            //return mylocationlist;

            //Transform to JsonRAW List
            return Ok(mylocationlist.Select(x => new JsonRaw(x)).ToList());
        }

        #endregion

        #region HelperMethods


        private IEnumerable<LocHelperclassDynamic> CreateLocHelperClassDynamic<T>(string typ , IEnumerable<T> locationlist, string? lang)  where T : IIdentifiable, IDetailInfosAware
        {
            try
            {
                var locationlistreduced = default(IEnumerable<LocHelperclassDynamic>);

                if (lang != null)
                    locationlistreduced = locationlist.Select(x => new LocHelperclassDynamic { typ = typ, name = x.Detail[lang].Title, id = x.Id });
                else
                    locationlistreduced = locationlist.Select(x => new LocHelperclassDynamic { typ = typ, name = x.Detail.ToDictionary(y => y.Key, y => y.Value.Title), id = x.Id });

                return locationlistreduced;
            }
            catch(Exception ex)
            {
                return null;
            }
        }       

        #endregion
    }
}
