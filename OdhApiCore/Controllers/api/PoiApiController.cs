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

namespace OdhApiCore.Controllers.api
{
    /// <summary>
    /// Poi Api (data provided by LTS PoiData) SOME DATA Available as OPENDATA
    /// </summary>
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class PoiController : OdhController
    {
        public PoiController(IWebHostEnvironment env, ISettings settings, ILogger<PoiController> logger, IPostGreSQLConnectionFactory connectionFactory, Factories.PostgresQueryFactory queryFactory)
            : base(env, settings, logger, connectionFactory, queryFactory)
        {
        }

        #region SWAGGER Exposed API

        /// <summary>
        /// GET Poi List
        /// </summary>
        /// <param name="pagenumber">Pagenumber, (default:1)</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="poitype">Type of the Poi ('null' = Filter disabled, possible values: BITMASK 'Doctors, Pharmacies = 1','Shops = 2','Culture and sights= 4','Nightlife and entertainment = 8','Public institutions = 16','Sports and leisure = 32','Traffic and transport = 64', 'Service providers' = 128, 'Craft' = 256), (default:'511' == ALL), REFERENCE TO: GET /api/PoiTypes </param>
        /// <param name="subtype">Subtype of the Poi ('null' = Filter disabled, available Subtypes depends on the poitype BITMASK), (default:'null')</param>
        /// <param name="idlist">IDFilter (Separator ',' List of Activity IDs, 'null' = No Filter), (default:'null')</param>
        /// <param name="locfilter">Locfilter (Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMASSOCIATIONID = (Filter by Tourismassociation), 'null' = No Filter), (default:'null')</param>
        /// <param name="areafilter">AreaFilter (Alternate Locfilter, can be combined with locfilter) (Separator ',' possible values: reg + REGIONID = (Filter by Region), tvs + TOURISMASSOCIATIONID = (Filter by Tourismassociation), skr + SKIREGIONID = (Filter by Skiregion), ska + SKIAREAID = (Filter by Skiarea), are + AREAID = (Filter by LTS Area), 'null' = No Filter), (default:'null')</param>
        /// <param name="highlight">Highlight Filter (Show only Highlights possible values: 'true' : show only Highlight Pois, 'null' Filter disabled), (default:'null')</param>
        /// <param name="odhtagfilter">ODH Taglist Filter (refers to Array SmgTags) (String, Separator ',' more Tags possible, available Tags reference to 'api/ODHTag?validforentity=poi'), (default:'null')</param>
        /// <param name="active">Active Pois Filter (possible Values: 'true' only Active Pois, 'false' only Disabled Pois, (default:'null')</param>
        /// <param name="odhactive">ODH Active (Published) Pois Filter (Refers to field SmgActive) Pois Filter (possible Values: 'true' only published Pois, 'false' only not published Pois, (default:'null')</param>
        /// <param name="lastchange">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="latitude">GeoFilter Latitude Format: '46.624975', 'null' = disabled, (default:'null')</param>
        /// <param name="longitude">GeoFilter Longitude Format: '11.369909', 'null' = disabled, (default:'null')</param>
        /// <param name="radius">Radius to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null')</param>
        /// <returns>Collection of LTSPoi Objects</returns>
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<GBLTSPoi>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("api/Poi")]
        public async Task<IActionResult> GetPoiList(
            string? language = null,
            uint pagenumber = 1,
            uint pagesize = 10,
            string? poitype = "511",
            string? subtype = null,
            string? idlist = null,
            string? areafilter = null,
            LegacyBool highlight = null!,
            string? locfilter = null,
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
            //TODO
            //CheckOpenData(User);

            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            return await GetFiltered(
                fields: fields ?? Array.Empty<string>(), language: language, pagenumber: pagenumber, pagesize: pagesize,
                activitytype: poitype, subtypefilter: subtype, idfilter: idlist, searchfilter: searchfilter,
                locfilter: locfilter, areafilter: areafilter, highlightfilter: highlight, active: active, smgactive: odhactive,
                smgtags: odhtagfilter, seed: seed, lastchange: lastchange, geosearchresult: geosearchresult,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// GET Poi Single
        /// </summary>
        /// <param name="id">ID of the Poi</param>
        /// <returns>GBLTSPoi Object</returns>
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(GBLTSPoi), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,PoiReader")]
        [HttpGet, Route("api/Poi/{id}")]
        public async Task<IActionResult> GetPoiSingle(
            string id, 
            string? language = null, 
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
         CancellationToken cancellationToken = default)
        {
            //TODO
            //CheckOpenData(User);

            return await GetSingle(id, language, fields: fields ?? Array.Empty<string>(), cancellationToken);
        }

        /// <summary>
        /// GET Poi Types List
        /// </summary>
        /// <returns>Collection of PoiType Object</returns>
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        //[CacheOutputUntilToday(23, 59)]
        [ProducesResponseType(typeof(IEnumerable<PoiTypes>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,PoiReader")]
        [HttpGet, Route("api/PoiTypes")]
        public async Task<IActionResult> GetAllPoiTypesList(CancellationToken cancellationToken)
        {
            return await GetPoiTypesList(cancellationToken);
        }

        /// <summary>
        /// GET Poi Types Single
        /// </summary>
        /// <returns>PoiType Object</returns>
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        //[CacheOutputUntilToday(23, 59)]
        [ProducesResponseType(typeof(PoiTypes), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,PoiReader")]
        [HttpGet, Route("api/PoiTypes/{id}")]
        public async Task<IActionResult> GetAllPoiTypesSingle(string id, string language,  CancellationToken cancellationToken)
        {
            return await GetPoiTypesSingleAsync(id, cancellationToken);
        }

        #endregion

        #region GETTER

        /// <summary>
        /// GET Pois List
        /// </summary>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page</param>
        /// <param name="activitytype">Type of the Poi (possible values: STRINGS: 'Ärzte, Apotheken','Geschäfte und Dienstleister','Kultur und Sehenswürdigkeiten','Nachtleben und Unterhaltung','Öffentliche Einrichtungen','Sport und Freizeit','Verkehr und Transport' : BITMASK also possible: 'Ärtze, Apotheken = 1','Geschäfte und Dienstleister = 2','Kultur und Sehenswürdigkeiten = 4','Nachtleben und Unterhaltung = 8','Öffentliche Einrichtungen = 16','Sport und Freizeit = 32','Verkehr und Transport = 64')</param>
        /// <param name="subtypefilter">Subtype of the Poi ('null' = Filter disabled, available Subtypes depends on the activitytype BITMASK)</param>
        /// <param name="idfilter">IDFilter (Separator ',' List of Activity IDs, 'null' = No Filter)</param>
        /// <param name="locfilter">Locfilter (Separator ',' possible values: reg + REGIONID = (Filter by Region), reg + REGIONID = (Filter by Region), tvs + TOURISMVEREINID = (Filter by Tourismverein), mun + MUNICIPALITYID = (Filter by Municipality), fra + FRACTIONID = (Filter by Fraction), 'null' = No Filter)</param>
        /// <param name="areafilter">AreaFilter (Separator ',' IDList of AreaIDs separated by ',', 'null' : Filter disabled)</param>
        /// <param name="highlightfilter">Highlight Filter (Show only Highlights possible values: 'true' : show only Highlight Pois, 'null' Filter disabled)</param>
        /// <param name="active">Active Filter (possible Values: 'null' Displays all Pois, 'true' only Active Pois, 'false' only Disabled Pois</param>
        /// <param name="smgactive">SMGActive Filter (possible Values: 'null' Displays all Pois, 'true' only SMG Active Pois, 'false' only SMG Disabled Pois</param>
        /// <param name="smgtags">SMGTag Filter (String, Separator ',' more SMGTags possible, 'null' = No Filter)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting</param>
        /// <returns>Result Object with Collection of Pois</returns>
        private Task<IActionResult> GetFiltered(
            string[] fields, string? language, uint pagenumber, uint pagesize, string? activitytype, string? subtypefilter,
            string? idfilter, string? searchfilter, string? locfilter, string? areafilter, bool? highlightfilter, bool? active,
            bool? smgactive, string? smgtags, string? seed, string? lastchange, PGGeoSearchResult geosearchresult,
            CancellationToken cancellationToken)
        {

            return DoAsyncReturn(async connectionFactory =>
            {
                PoiHelper myactivityhelper = await PoiHelper.CreateAsync(
                    QueryFactory.QueryFactory, poitype: activitytype, subtypefilter: subtypefilter, idfilter: idfilter,
                    locfilter: locfilter, areafilter: areafilter, highlightfilter: highlightfilter, activefilter: active,
                    smgactivefilter: smgactive, smgtags: smgtags, lastchange: lastchange, cancellationToken: cancellationToken);

                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From("pois")
                        .PoiWhereExpression(
                            idlist: myactivityhelper.idlist, poitypelist: myactivityhelper.poitypelist,
                            subtypelist: myactivityhelper.subtypelist, smgtaglist: myactivityhelper.smgtaglist,
                            districtlist: new List<string>(), municipalitylist: new List<string>(),
                            tourismvereinlist: myactivityhelper.tourismvereinlist, regionlist: myactivityhelper.regionlist,
                            arealist: myactivityhelper.arealist, highlight: myactivityhelper.highlight,
                            activefilter: myactivityhelper.active, smgactivefilter: myactivityhelper.smgactive,
                            searchfilter: searchfilter, language: language, lastchange: myactivityhelper.lastchange
                        )
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
                        raw => raw.TransformRawData(language, fields, checkCC0: CheckCC0License)
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

        /// <summary>
        /// GET Single Poi
        /// </summary>
        /// <param name="id">ID of Poi</param>
        /// <returns>Poi Object</returns>
        private Task<IActionResult> GetSingle(string id, string? language, string[] fields, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async connectionFactory =>
            {
                var query =
                    QueryFactory.Query("pois")
                        .Select("data")
                        .Where("id", id);

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data?.TransformRawData(language, fields, checkCC0: CheckCC0License);
            });
        }

        #endregion

        #region CUSTOM METHODS

        /// <summary>
        /// GET Poi Types List
        /// </summary>
        /// <returns>Collection of PoiTypes Object</returns>
        private Task<IActionResult> GetPoiTypesList(CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async connectionFactory =>
            {
                var query =
                     QueryFactory.Query("poitypes")
                         .SelectRaw("data");

                var data = await query.GetAsync<JsonRaw?>();

                return data;
            });
        }

        /// <summary>
        /// GET Poi Types Single
        /// </summary>
        /// <returns>PoiTypes Object</returns>
        private Task<IActionResult> GetPoiTypesSingleAsync(string id, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async connectionFactory =>
            {
                var query =
                    QueryFactory.Query("poitypes")
                        .Select("data")
                        .WhereJsonb("Key", id.ToLower());
                        //.Where("Key", "ILIKE", id);

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data;
            });
        }

        #endregion

        #region POST PUT DELETE

        ///// <summary>
        ///// POST Insert new Poi
        ///// </summary>
        ///// <param name="poi">Poi Object</param>
        ///// <returns>HttpResponseMessage</returns>
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataCreate,PoiManager,PoiCreate")]
        //[HttpPost, Route("api/Poi")]
        //public HttpResponseMessage Post([FromBody]GBLTSPoi poi)
        //{
        //    try
        //    {
        //        if (poi != null)
        //        {
        //            using (var conn = new NpgsqlConnection(GlobalPGConnection.PGConnectionString))
        //            {

        //                conn.Open();

        //                PostgresSQLHelper.InsertDataIntoTable(conn, "pois", JsonConvert.SerializeObject(poi), poi.Id.ToUpper());

        //                return Request.CreateResponse(HttpStatusCode.Created, new GenericResult() { Message = "Insert Poi succeeded, Id:" + poi.Id.ToUpper() }, "application/json");
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception("No Poi Data provided");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, new GenericResult() { Message = ex.Message }, "application/json");
        //    }
        //}

        ///// <summary>
        ///// PUT Modify existing Poi
        ///// </summary>
        ///// <param name="id">Poi Id</param>
        ///// <param name="poi">Poi Object</param>
        ///// <returns>HttpResponseMessage</returns>
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataModify,PoiManager,PoiModify")]
        //[HttpPut, Route("api/Poi/{id}")]
        //public HttpResponseMessage Put(string id, [FromBody]GBLTSPoi poi)
        //{
        //    try
        //    {
        //        if (poi != null && id != null)
        //        {
        //            using (var conn = new NpgsqlConnection(GlobalPGConnection.PGConnectionString))
        //            {

        //                conn.Open();

        //                PostgresSQLHelper.UpdateDataFromTable(conn, "pois", JsonConvert.SerializeObject(poi), poi.Id.ToUpper());

        //                return Request.CreateResponse(HttpStatusCode.Created, new GenericResult() { Message = "UPDATE Poi succeeded, Id:" + poi.Id.ToUpper() }, "application/json");
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception("No Poi Data provided");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, new GenericResult() { Message = ex.Message }, "application/json");
        //    }
        //}

        ///// <summary>
        ///// DELETE Poi by Id
        ///// </summary>
        ///// <param name="id">Poi Id</param>
        ///// <returns>HttpResponseMessage</returns>
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataDelete,PoiManager,PoiDelete")]
        //[HttpDelete, Route("api/Poi/{id}")]
        //public HttpResponseMessage Delete(string id)
        //{
        //    try
        //    {
        //        if (id != null)
        //        {

        //            using (var conn = new NpgsqlConnection(GlobalPGConnection.PGConnectionString))
        //            {

        //                conn.Open();

        //                PostgresSQLHelper.DeleteDataFromTable(conn, "pois", id.ToUpper());

        //                return Request.CreateResponse(HttpStatusCode.Created, new GenericResult() { Message = "DELETE Poi succeeded, Id:" + id.ToUpper() }, "application/json");
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception("No Poi Id provided");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, new GenericResult() { Message = ex.Message }, "application/json");
        //    }
        //}

        #endregion

        #region Obsolete here for Compatibility reasons

        /// <summary>
        /// GET Poi Changed List by Date
        /// </summary>
        /// <param name="pagenumber">Pagenumber, (default:1)</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="updatefrom">Date from Format (yyyy-MM-dd) (all GBActivityPoi with LastChange >= datefrom are passed), (default: DateTime.Now - 1 Day)</param>
        /// <returns>Collection of GBLTSPoi Objects</returns>
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<GBLTSPoi>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,PoiReader")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet, Route("api/PoiChanged")]
        public async Task<IActionResult> GetAllPoisChanged(
            uint pagenumber = 1,
            uint pagesize = 10,
            string? seed = null,
            string? updatefrom = null,
            CancellationToken cancellationToken = default
            )
        {
            //TODO
            //CheckOpenData(User);

            updatefrom ??= String.Format("{0:yyyy-MM-dd}", DateTime.Now.AddDays(-1));

            return await GetPoiList(
                language: null, pagenumber: pagenumber, pagesize: pagesize, poitype: null, subtype: null,
                idlist: null, areafilter: null, highlight: new LegacyBool(null), locfilter: null,
                odhtagfilter: null, active: new LegacyBool(null), odhactive: new LegacyBool(null),
                lastchange: updatefrom, seed: seed, latitude: null, longitude: null, radius: null,
                fields: null, searchfilter: null, cancellationToken: cancellationToken);
        }

        #endregion
    }

}
