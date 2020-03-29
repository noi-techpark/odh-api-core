using Helper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    //[Route("api/ODHTag")]
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class OdhTagController : OdhController
    {
        public OdhTagController(IWebHostEnvironment env, ISettings settings, ILogger<OdhTagController> logger, IPostGreSQLConnectionFactory connectionFactory, Factories.PostgresQueryFactory queryFactory)
            : base(env, settings, logger, connectionFactory, queryFactory)
        {
        }

        #region SWAGGER Exposed API

        /// <summary>
        /// GET ODHTag List
        /// </summary>
        /// <param name="validforentity">Filter on Tags valid on Entitys (accommodation, activity, poi, smgpoi, package, gastronomy, event, article, common .. etc..)</param>
        /// <param name="localizationlanguage"></param>
        /// <returns>Collection of ODHTag Objects</returns>
        //[SwaggerResponse(HttpStatusCode.OK, "Array of SmgTags Objects", typeof(IEnumerable<SmgTags>))]
        [HttpGet, Route("api/ODHTag")]
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        public async Task<IActionResult> GetODHTagsAsync(string localizationlanguage = "", string validforentity = "", CancellationToken cancellationToken = default)
        {
            //Fall 1 Getter auf ALL
            if (string.IsNullOrEmpty(validforentity) && string.IsNullOrEmpty(localizationlanguage))
            {
                return await Get(cancellationToken);
            }
            //Fall 2 Getter auf GetFiltered
            else if (!string.IsNullOrEmpty(validforentity) && string.IsNullOrEmpty(localizationlanguage))
            {
                return await GetFiltered(validforentity, cancellationToken);
            }
            //Fall 4 GET auf GetLocalized
            else if (string.IsNullOrEmpty(validforentity) && !string.IsNullOrEmpty(localizationlanguage))
            {
                return await GetLocalized(localizationlanguage, cancellationToken);
            }
            //Fall 5 GET auf GetFilteredLocalized
            else if (!string.IsNullOrEmpty(validforentity) && !string.IsNullOrEmpty(localizationlanguage))
            {
                return await GetFilteredLocalized(localizationlanguage, validforentity, cancellationToken);
            }
            else
            {
                throw new Exception("not supported");
            }
        }

        /// <summary>
        /// GET ODHTag Single
        /// </summary>
        /// <param name="id"></param>
        /// <param name="localizationlanguage"></param>
        /// <returns>ODHTag Object</returns>
        //[SwaggerResponse(HttpStatusCode.OK, "SmgTag Object", typeof(SmgTags))]
        [HttpGet, Route("api/ODHTag/{id}")]
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        public async Task<IActionResult> GetODHTagSingle(string id, string localizationlanguage = "", CancellationToken cancellationToken = default)
        {
            //Fall 1 Getter auf ALL
            if (string.IsNullOrEmpty(localizationlanguage))
            {
                return await GetSingle(id, cancellationToken);
            }
            //Fall 2 Getter auf GetFiltered
            else
            {
                return await GetSingleLocalized(id, localizationlanguage, cancellationToken);
            }
        }

        /// <summary>
        /// GET ODHTag List REDUCED
        /// </summary>
        /// <param name="validforentity"></param>
        /// <param name="localizationlanguage"></param>
        /// <returns></returns>
        //[SwaggerResponse(HttpStatusCode.OK, "Array of SmgTagReduced Objects", typeof(IEnumerable<SmgTagReduced>))]
        [HttpGet, Route("api/ODHTagReduced")]
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
        #endregion

        #region GETTER

        /// <summary>
        /// GET Complete List of SMGTags
        /// </summary>
        /// <returns>Collection of SMGTag Object</returns>
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        //[HttpGet, Route("")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        private Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async connectionFactory =>
            {
                var query = QueryFactory.Query("smgtags")
                    .Select("data")
                    .OrderByRaw("data->>'MainEntity', data ->>'Shortname'");

                return await query.GetAsync<JsonRaw>();
            });
        }

        /// <summary>
        /// GET Filtered List of SMGTags by smgtagtype
        /// </summary>
        /// <param name="smgtagtype">Smg Tag Type</param>
        /// <returns>Collection of SMGTag Object</returns>
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        //[HttpGet, Route("Filtered/{smgtagtype}")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        private Task<IActionResult> GetFiltered(string smgtagtype, CancellationToken cancellationToken)
        {
            var smgtagtypelist = smgtagtype.Split(',', StringSplitOptions.RemoveEmptyEntries);

            return DoAsyncReturn(async connectionFactory =>
            {
                var query =
                    QueryFactory.Query("smgtags")
                        .Select("data")
                        .WhereInJsonb(
                            smgtagtypelist,
                            id => new { ValidForEntity = new[] { id.ToLower() } }
                        )
                        .OrderByRaw("data->>'MainEntity', data->>'Shortname'");

                return await query.GetAsync<JsonRaw>();
            });
        }

        /// <summary>
        /// GET Single SMGTag by ID
        /// </summary>
        /// <param name="id">ID of the SMGTag</param>
        /// <returns>SMGTag Object</returns>
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        //[HttpGet, Route("Single/{id}")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        private Task<IActionResult> GetSingle(string id, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async connectionFactory =>
            {
                return await QueryFactory.Query("smgtags")
                    .Select("data")
                    .Where("id", id.ToLower())
                    .FirstOrDefaultAsync<JsonRaw>();
            });
        }

        #endregion

        #region GET Localized

        /// <summary>
        /// GET Localized List of SMGTags
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>Collection of Localized SMGTag Object</returns>
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        [HttpGet, Route("{language}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task<IActionResult> GetLocalized(string language, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async connectionFactory =>
            {
                var query =
                    QueryFactory.Query("smgtags")
                        .Select("data")
                        .OrderByRaw("data->>'MainEntity', data->>'Shortname'");

                var data = await query.GetAsync<JsonRaw>();

                return data
                    .Select(raw => JsonConvert.DeserializeObject<SmgTags>(raw.Value))
                    .TransformToLocalizedSmgTag(language);
            });
        }

        /// <summary>
        /// GET Filtered List of Localized SMGTags by smgtagtype
        /// </summary>
        /// <param name="language">Langauge</param>
        /// <param name="smgtagtype">Smg Tag Type</param>
        /// <returns>Collection of Localized SMGTag Object</returns>
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        [HttpGet, Route("Filtered/{language}/{smgtagtype}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task<IActionResult> GetFilteredLocalized(
            string language, string smgtagtype, CancellationToken cancellationToken)
        {
            var smgtagtypelist = smgtagtype.Split(',', StringSplitOptions.RemoveEmptyEntries);

            return DoAsyncReturn(async connectionFactory =>
            {
                var query = QueryFactory.Query("smgtags")
                    .Select("data")
                    .WhereInJsonb(
                        smgtagtypelist,
                        id => new { ValidForEntity = new[] { id.ToLower() } }
                    )
                    .OrderByRaw("data->>'MainEntity', data->>'Shortname'");

                var data = await query.GetAsync<JsonRaw>();

                return data
                    .Select(raw => JsonConvert.DeserializeObject<SmgTags>(raw.Value))
                    .TransformToLocalizedSmgTag(language);
            });
        }

        /// <summary>
        /// GET Single Localized SMGTag Object by ID
        /// </summary>
        /// <param name="id">SMGTag ID</param>
        /// <param name="language">Language</param>
        /// <returns>Localized SMGTag Object</returns>
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        [HttpGet, Route("Single/{language}/{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task<IActionResult> GetSingleLocalized(string id, string language, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async connectionFactory =>
            {
                var data =
                    await QueryFactory.Query("smgtags")
                        .Select("data")
                        .Where("id", id.ToLower())
                        .FirstOrDefaultAsync<JsonRaw>();

                if (data != null)
                    return JsonConvert.DeserializeObject<SmgTags>(data.Value)
                        .TransformToLocalizedSmgTag(language);
                else
                    return null;
            });
        }

        #endregion

        #region GET REDUCED
        class ReducedResult
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
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
            return DoAsyncReturn(async connectionFactory =>
            {
                string select = $"data->>'Id' as Id, data->'TagName'->>'{language.ToLower()}' as Name";
                string where = $"data->'TagName'->>'{language.ToLower()}' NOT LIKE ''";

                var data =
                    QueryFactory.Query("smgtags")
                        .SelectRaw(select)
                        .WhereRaw(where);

                return await data.GetAsync<ReducedResult>();
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

            return DoAsyncReturn(async connectionFactory =>
            {
                string select = $"data->'Id' as Id, data->'TagName'->'{language.ToLower()}' as Name";
                string where = $"data->'TagName'->>'{language.ToLower()}' NOT LIKE ''";

                var data =
                    QueryFactory.Query("smgtags")
                        .SelectRaw(select)
                        .WhereRaw(where)
                        .WhereInJsonb(
                            smgtagtypelist,
                            smgtag => new { ValidForEntity = new[] { smgtag.ToLower() } }
                        );

                return await data.GetAsync<ReducedResult>();
            });
        }

        #endregion
    }
}