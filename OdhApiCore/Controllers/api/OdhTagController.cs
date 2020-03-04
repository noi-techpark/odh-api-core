using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    //[Route("api/ODHTag")]
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class OdhTagController : OdhController
    {
        public OdhTagController(ISettings settings, IPostGreSQLConnectionFactory connectionFactory)
            : base(settings, connectionFactory)
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

        //TEST METHOD PERFORMANCE
        [HttpGet, Route("api/TestPerf")]
        [Authorize(Roles = "TourismReader")]
        public IActionResult GetTest(string hallo = "", CancellationToken cancellationToken = default)
        {
            string toreturn = "{ \"es\": \"geat\" }";
            if(!String.IsNullOrEmpty(hallo))
                toreturn = "{ \"hallo\": \"" + hallo + "\" }";

            return this.Content(toreturn, "application/json", Encoding.UTF8);
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
            return DoAsyncReturnString(async connectionFactory =>
            {
                string select = "*";
                string orderby = "data ->>'MainEntity', data ->>'Shortname'";
                string where = "";

                var (totalCount, myresult) = await PostgresSQLHelper.SelectFromTableDataAsStringParametrizedAsync(
                    connectionFactory, "smgtags", select, (where, null),
                    orderby, 0, null, cancellationToken);

                return JsonConvert.SerializeObject(myresult);
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

            return DoAsyncReturnString(async connectionFactory =>
            {
                string select = "*";
                string orderby = "data ->>'MainEntity', data ->>'Shortname'";
                string where = "";
                List<PGParameters> parameters = new List<PGParameters>();

                if (smgtagtypelist.Length > 0)
                {
                    int counter = 1;
                    string smgtagtypeliststring = "";
                    foreach (var smgtagtypeid in smgtagtypelist)
                    {
                        smgtagtypeliststring = $"{smgtagtypeliststring}data @> @smgtag{counter} OR ";
                        parameters.Add(new PGParameters() { Name = "smgtag" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"ValidForEntity\": [\"" + smgtagtypeid.ToLower() + "\"]}" });
                        counter++;
                    }
                    smgtagtypeliststring = smgtagtypeliststring.Remove(smgtagtypeliststring.Length - 4);

                    where = where + smgtagtypeliststring;
                }

                var (totalCount, myresult) = await PostgresSQLHelper.SelectFromTableDataAsStringParametrizedAsync(
                    connectionFactory, "smgtags", select, (where, parameters),
                    orderby, 0, null, cancellationToken);

                return JsonConvert.SerializeObject(myresult);
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
            return DoAsyncReturnString(async connectionFactory =>
            {
                var where = PostgresSQLWhereBuilder.CreateIdListWhereExpression(id.ToLower());
                var (totalCount, data) = await PostgresSQLHelper.SelectFromTableDataAsStringParametrizedAsync(
                    connectionFactory, "smgtags", "*", where,
                    "", 0, null, cancellationToken);

                return JsonConvert.SerializeObject(data);
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
            return DoAsyncReturnString(async connectionFactory =>
            {
                string select = "*";
                string orderby = "data ->>'MainEntity', data ->>'Shortname'";
                string where = "";

                var myresult = await PostgresSQLHelper.SelectFromTableDataAsObjectParametrizedAsync<SmgTags>(
                    connectionFactory, "smgtags", select, (where, null), orderby, 0, null, cancellationToken).ToListAsync();
                var localizedresult = myresult.TransformToLocalizedSmgTag(language);

                return JsonConvert.SerializeObject(localizedresult);
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

            return DoAsyncReturnString(async connectionFactory =>
            {
                string select = "*";
                string orderby = "data ->>'MainEntity', data ->>'Shortname'";
                string where = "";
                List<PGParameters> parameters = new List<PGParameters>();

                if (smgtagtypelist.Length > 0)
                {
                    int counter = 1;
                    string smgtagtypeliststring = "";
                    foreach (var smgtagtypeid in smgtagtypelist)
                    {
                        smgtagtypeliststring = $"{smgtagtypeliststring}data @> @smgtag{counter} OR ";
                        parameters.Add(new PGParameters()
                        {
                            Name = "smgtag" + counter,
                            Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                            Value = $"{{ \"ValidForEntity\": [\"{smgtagtypeid.ToLower()}\"]}}"
                        });
                        counter++;
                    }
                    smgtagtypeliststring = smgtagtypeliststring.Remove(smgtagtypeliststring.Length - 4);

                    where = where + smgtagtypeliststring;
                }

                var myresult = await PostgresSQLHelper.SelectFromTableDataAsObjectParametrizedAsync<SmgTags>(
                    connectionFactory, "smgtags", select, (where, parameters),
                    orderby, 0, null, cancellationToken).ToListAsync();
                var localizedresult = myresult.TransformToLocalizedSmgTag(language);

                return JsonConvert.SerializeObject(localizedresult);
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
            return DoAsyncReturnString(async connectionFactory =>
            {
                var where = PostgresSQLWhereBuilder.CreateIdListWhereExpression(id.ToLower());
                var data = await PostgresSQLHelper.SelectFromTableDataAsObjectParametrizedAsync<SmgTags>(
                    connectionFactory, "smgtags", "*", where,
                    "", 0, null, cancellationToken).ToListAsync();
                var localizedresult = data.TransformToLocalizedSmgTag(language);

                return JsonConvert.SerializeObject(localizedresult.FirstOrDefault());
            });
        }

        #endregion

        #region GET REDUCED

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
            return DoAsyncReturnString(async connectionFactory =>
            {
                string select = $"data->'Id' as Id, data->'TagName'->'{language.ToLower()}' as Name";
                string orderby = "";
                string where = $"data->'TagName'->>'{language.ToLower()}' NOT LIKE ''";

                var data = await PostgresSQLHelper.SelectFromTableDataAsJsonParametrizedAsync(
                    connectionFactory, "smgtags", select, (where, null), orderby, 0,
                    null, new List<string>() { "Id", "Name" }, cancellationToken).ToListAsync();

                return JsonConvert.SerializeObject(data);
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

            return DoAsyncReturnString(async connectionFactory =>
            {
                string select = $"data->'Id' as Id, data->'TagName'->'{language.ToLower()}' as Name";
                string orderby = "";
                string where = "";
                List<PGParameters> parameters = new List<PGParameters>();

                if (smgtagtypelist.Length > 0)
                {
                    int counter = 1;
                    string smgtagtypeliststring = "";
                    foreach (var smgtagtypeid in smgtagtypelist)
                    {
                        smgtagtypeliststring = $"{smgtagtypeliststring}data @> @smgtag{counter} OR ";
                        parameters.Add(new PGParameters() { Name = "smgtag" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"ValidForEntity\": [\"" + smgtagtypeid.ToLower() + "\"]}" });
                        counter++;
                    }
                    smgtagtypeliststring = smgtagtypeliststring.Remove(smgtagtypeliststring.Length - 4);

                    where = where + smgtagtypeliststring;
                }

                where = $"{where} AND data->'TagName'->>'{language.ToLower()}' NOT LIKE ''";

                var data = await PostgresSQLHelper.SelectFromTableDataAsJsonParametrizedAsync(
                    connectionFactory, "smgtags", select, (where, parameters), orderby, 0, null,
                    new List<string>() { "Id", "Name" }, cancellationToken).ToListAsync();

                return JsonConvert.SerializeObject(data);
            });
        }

        #endregion
    }
}