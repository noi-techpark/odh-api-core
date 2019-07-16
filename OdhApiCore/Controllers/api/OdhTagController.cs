using System;
using System.Collections.Generic;
using System.Linq;
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
    //[Route("api/ODHTag")]
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class OdhTagController : OdhController
    {
        public OdhTagController(ISettings settings) : base(settings)
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
        public async Task<IActionResult> GetODHTagsAsync(string localizationlanguage = "", string validforentity = "")
        {
            //Fall 1 Getter auf ALL
            if (string.IsNullOrEmpty(validforentity) && string.IsNullOrEmpty(localizationlanguage))
            {
                return await Get();
            }
            //Fall 2 Getter auf GetFiltered
            else if (!string.IsNullOrEmpty(validforentity) && string.IsNullOrEmpty(localizationlanguage))
            {
                return await GetFiltered(validforentity);
            }
            //Fall 4 GET auf GetLocalized
            else if (string.IsNullOrEmpty(validforentity) && !string.IsNullOrEmpty(localizationlanguage))
            {
                return await GetLocalized(localizationlanguage);
            }
            //Fall 5 GET auf GetFilteredLocalized
            else if (!string.IsNullOrEmpty(validforentity) && !string.IsNullOrEmpty(localizationlanguage))
            {
                return await GetFilteredLocalized(localizationlanguage, validforentity);
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
        public async Task<IActionResult> GetODHTagSingle(string id, string localizationlanguage = "")
        {
            //Fall 1 Getter auf ALL
            if (string.IsNullOrEmpty(localizationlanguage))
            {
                return await GetSingle(id);
            }
            //Fall 2 Getter auf GetFiltered
            else
            {
                return await GetSingleLocalized(id, localizationlanguage);
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
        public async Task<IActionResult> GetODHTagsReduced(string localizationlanguage, string validforentity = "")
        {
            if (string.IsNullOrEmpty(validforentity) && !string.IsNullOrEmpty(localizationlanguage))
            {
                return await GetReducedLocalized(localizationlanguage);
            }
            //Fall 7 GET auf GetReducedFilteredLocalized
            else if (!string.IsNullOrEmpty(validforentity) && !string.IsNullOrEmpty(localizationlanguage))
            {
                return await GetReducedFilteredLocalized(localizationlanguage, validforentity);
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
        private Task<IActionResult> Get()
        {
            return DoAsync(async conn =>
            {
                string select = "*";
                string orderby = "data ->>'MainEntity', data ->>'Shortname'";
                string where = "";

                var myresult = await PostgresSQLHelper.SelectFromTableDataAsStringParametrizedAsync(conn, "smgtags", select, where, null, orderby, 0, null);

                return "[" + String.Join(",", myresult) + "]";
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
        private Task<IActionResult> GetFiltered(string smgtagtype)
        {
            IList<string> smgtagtypelist = smgtagtype.ConvertToList(',');

            return DoAsync(async conn =>
            {
                string select = "*";
                string orderby = "data ->>'MainEntity', data ->>'Shortname'";
                string where = "";
                List<PGParameters> parameters = new List<PGParameters>();

                if (smgtagtypelist.Count > 0)
                {
                    int counter = 1;
                    string smgtagtypeliststring = "";
                    foreach (var smgtagtypeid in smgtagtypelist)
                    {
                        smgtagtypeliststring = smgtagtypeliststring + "data @> @smgtag" + counter + " OR ";
                        parameters.Add(new PGParameters() { Name = "smgtag" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"ValidForEntity\": [\"" + smgtagtypeid.ToLower() + "\"]}" });
                        counter++;
                    }
                    smgtagtypeliststring = smgtagtypeliststring.Remove(smgtagtypeliststring.Length - 4);

                    where = where + smgtagtypeliststring;
                }

                var myresult = await PostgresSQLHelper.SelectFromTableDataAsStringParametrizedAsync(conn, "smgtags", select, where, parameters, orderby, 0, null);

                return "[" + String.Join(",", myresult) + "]";
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
        private Task<IActionResult> GetSingle(string id)
        {
            return DoAsync(async conn =>
            {
                var where = PostgresSQLWhereBuilder.CreateIdListWhereExpression(id.ToLower());
                var data = await PostgresSQLHelper.SelectFromTableDataAsStringParametrizedAsync(conn, "smgtags", "*", where.Item1, where.Item2, "", 0, null);

                return String.Join(",", data);
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
        public Task<IActionResult> GetLocalized(string language)
        {
            return DoAsync(async conn =>
            {
                string select = "*";
                string orderby = "data ->>'MainEntity', data ->>'Shortname'";
                string where = "";

                var myresult = await PostgresSQLHelper.SelectFromTableDataAsObjectParametrizedAsync<SmgTags>(conn, "smgtags", select, where, null, orderby, 0, null);
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
        public Task<IActionResult> GetFilteredLocalized(string language, string smgtagtype)
        {
            IList<string> smgtagtypelist = smgtagtype.ConvertToList(',');

            return DoAsync(async conn =>
            {
                string select = "*";
                string orderby = "data ->>'MainEntity', data ->>'Shortname'";
                string where = "";
                List<PGParameters> parameters = new List<PGParameters>();

                if (smgtagtypelist.Count > 0)
                {
                    int counter = 1;
                    string smgtagtypeliststring = "";
                    foreach (var smgtagtypeid in smgtagtypelist)
                    {
                        smgtagtypeliststring = smgtagtypeliststring + "data @> @smgtag" + counter + " OR ";
                        parameters.Add(new PGParameters() { Name = "smgtag" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"ValidForEntity\": [\"" + smgtagtypeid.ToLower() + "\"]}" });
                        counter++;
                    }
                    smgtagtypeliststring = smgtagtypeliststring.Remove(smgtagtypeliststring.Length - 4);

                    where = where + smgtagtypeliststring;
                }

                var myresult = await PostgresSQLHelper.SelectFromTableDataAsObjectParametrizedAsync<SmgTags>(conn, "smgtags", select, where, parameters, orderby, 0, null);
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
        public Task<IActionResult> GetSingleLocalized(string id, string language)
        {
            return DoAsync(async conn =>
            {
                var where = PostgresSQLWhereBuilder.CreateIdListWhereExpression(id.ToLower());
                var data = await PostgresSQLHelper.SelectFromTableDataAsObjectParametrizedAsync<SmgTags>(conn, "smgtags", "*", where.Item1, where.Item2, "", 0, null);
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
        public Task<IActionResult> GetReducedLocalized(string language)
        {
            return DoAsync(async conn =>
            {
                string select = "data->'Id' as Id, data->'TagName'->'" + language.ToLower() + "' as Name";
                string orderby = "";
                string where = "data->'TagName'->>'" + language.ToLower() + "' NOT LIKE ''";

                var data = await PostgresSQLHelper.SelectFromTableDataAsJsonParametrizedAsync(conn, "smgtags", select, where, null, orderby, 0, null, new List<string>() { "Id", "Name" });

                return "[" + String.Join(",", data) + "]";
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
        public Task<IActionResult> GetReducedFilteredLocalized(string language, string smgtagtype)
        {            
            IList<string> smgtagtypelist = smgtagtype.ConvertToList(',');

            return DoAsync(async conn =>
            {
                string select = "data->'Id' as Id, data->'TagName'->'" + language.ToLower() + "' as Name";
                string orderby = "";
                string where = "";
                List<PGParameters> parameters = new List<PGParameters>();

                if (smgtagtypelist.Count > 0)
                {
                    int counter = 1;
                    string smgtagtypeliststring = "";
                    foreach (var smgtagtypeid in smgtagtypelist)
                    {
                        smgtagtypeliststring = smgtagtypeliststring + "data @> @smgtag" + counter + " OR ";
                        parameters.Add(new PGParameters() { Name = "smgtag" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"ValidForEntity\": [\"" + smgtagtypeid.ToLower() + "\"]}" });
                        counter++;
                    }
                    smgtagtypeliststring = smgtagtypeliststring.Remove(smgtagtypeliststring.Length - 4);

                    where = where + smgtagtypeliststring;
                }

                where = where + " AND data->'TagName'->>'" + language.ToLower() + "' NOT LIKE ''";

                var data = await PostgresSQLHelper.SelectFromTableDataAsJsonParametrizedAsync(conn, "smgtags", select, where, parameters, orderby, 0, null, new List<string>() { "Id", "Name" });

                return "[" + String.Join(",", data) + "]";
            });
        }

        #endregion
    }
}