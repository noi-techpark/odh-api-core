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
    [Route("api/SmgTag")]
    [EnableCors("CorsPolicy")]
    public class OdhTagController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private string connectionString = "";

        public OdhTagController(IConfiguration config)
        {
            configuration = config;
            connectionString = configuration.GetConnectionString("PGConnection");
        }

        #region GETTER

        /// <summary>
        /// GET Complete List of SMGTags
        /// </summary>
        /// <returns>Collection of SMGTag Object</returns>
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        [HttpGet, Route("")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Get()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {

                    conn.Open();

                    string select = "*";
                    string orderby = "data ->>'MainEntity', data ->>'Shortname'";
                    string where = "";

                    var myresult = PostgresSQLHelper.SelectFromTableDataAsString(conn, "smgtags", select, where, orderby, 0, null);

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
        /// GET Filtered List of SMGTags by smgtagtype
        /// </summary>
        /// <param name="smgtagtype">Smg Tag Type</param>
        /// <returns>Collection of SMGTag Object</returns>
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        [HttpGet, Route("Filtered/{smgtagtype}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetFiltered(string smgtagtype)
        {
            IList<string> smgtagtypelist = smgtagtype.ConvertToList(',');

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string select = "*";
                    string orderby = "data ->>'MainEntity', data ->>'Shortname'";
                    string where = "";

                    if (smgtagtypelist.Count > 0)
                    {
                        if (smgtagtypelist.Count == 1)
                        {
                            where = where + "data @> '{\"ValidForEntity\": [\"" + smgtagtypelist.FirstOrDefault().ToLower() + "\"] }'";
                        }
                        else
                        {
                            string smgtagtypeliststring = "";
                            foreach (var smgtagtypeid in smgtagtypelist)
                            {
                                smgtagtypeliststring = smgtagtypeliststring + "data @> '{ \"ValidForEntity\": [\"" + smgtagtypeid.ToLower() + "\"]}' OR ";
                            }
                            smgtagtypeliststring = smgtagtypeliststring.Remove(smgtagtypeliststring.Length - 4);

                            where = where + smgtagtypeliststring;
                        }
                    }

                    var myresult = PostgresSQLHelper.SelectFromTableDataAsString(conn, "smgtags", select, where, orderby, 0, null);

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
        /// GET Single SMGTag by ID
        /// </summary>
        /// <param name="id">ID of the SMGTag</param>
        /// <returns>SMGTag Object</returns>
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        [HttpGet, Route("Single/{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetSingle(string id)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string selectexp = "*";
                    string whereexp = "id LIKE '" + id.ToLower() + "'";

                    var data = PostgresSQLHelper.SelectFromTableDataAsString(conn, "smgtags", selectexp, whereexp, "", 0, null);

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

        #region GET Localized

        /// <summary>
        /// GET Localized List of SMGTags
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>Collection of Localized SMGTag Object</returns>
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        [HttpGet, Route("{language}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetLocalized(string language)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string select = "*";
                    string orderby = "data ->>'MainEntity', data ->>'Shortname'";
                    string where = "";

                    var myresult = PostgresSQLHelper.SelectFromTableDataAsObject<SmgTags>(conn, "smgtags", select, where, orderby, 0, null);

                    var localizedresult = myresult.TransformToLocalizedSmgTag(language);

                    conn.Close();

                    return Content(JsonConvert.SerializeObject(localizedresult), "application/json", Encoding.UTF8);                    
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }            
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
        public IActionResult GetFilteredLocalized(string language, string smgtagtype)
        {
            IList<string> smgtagtypelist = smgtagtype.ConvertToList(',');

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {

                    conn.Open();

                    string select = "*";
                    string orderby = "data ->>'MainEntity', data ->>'Shortname'";
                    string where = "";

                    if (smgtagtypelist.Count > 0)
                    {
                        if (smgtagtypelist.Count == 1)
                        {
                            where = where + "data @> '{\"ValidForEntity\": [\"" + smgtagtypelist.FirstOrDefault().ToLower() + "\"] }'";
                        }
                        else
                        {
                            string smgtagtypeliststring = "";
                            foreach (var smgtagtypeid in smgtagtypelist)
                            {
                                smgtagtypeliststring = smgtagtypeliststring + "data @> '{ \"ValidForEntity\": [\"" + smgtagtypeid.ToLower() + "\"]}' OR ";
                            }
                            smgtagtypeliststring = smgtagtypeliststring.Remove(smgtagtypeliststring.Length - 4);

                            where = where + smgtagtypeliststring;
                        }
                    }

                    var myresult = PostgresSQLHelper.SelectFromTableDataAsObject<SmgTags>(conn, "smgtags", select, where, orderby, 0, null);
                    var localizedresult = myresult.TransformToLocalizedSmgTag(language);

                    conn.Close();

                    return Content(JsonConvert.SerializeObject(localizedresult), "application/json", Encoding.UTF8);                    
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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
        public IActionResult GetSingleLocalized(string id, string language)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string selectexp = "*";
                    string whereexp = "id LIKE '" + id.ToLower() + "'";

                    var data = PostgresSQLHelper.SelectFromTableDataAsObject<SmgTags>(conn, "smgtags", selectexp, whereexp, "", 0, null);
                    var localizedresult = data.TransformToLocalizedSmgTag(language);

                    conn.Close();

                    return Content(JsonConvert.SerializeObject(localizedresult.FirstOrDefault()), "application/json", Encoding.UTF8);                    
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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
        public IActionResult GetReducedLocalized(string language)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {

                    conn.Open();
                    string select = "data->'Id' as Id, data->'TagName'->'" + language.ToLower() + "' as Name";
                    string orderby = "";
                    string where = "data->'TagName'->>'" + language.ToLower() + "' NOT LIKE ''";

                    var data = PostgresSQLHelper.SelectFromTableDataAsIdAndString(conn, "smgtags", select, where, orderby, 0, null, new List<string>() { "Id", "Name" });

                    conn.Close();

                    return Content("[" + String.Join(",", data) + "]", "application/json", Encoding.UTF8);                    
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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
        public IActionResult GetReducedFilteredLocalized(string language, string smgtagtype)
        {
            try
            {
                IList<string> smgtagtypelist = smgtagtype.ConvertToList(',');

                using (var conn = new NpgsqlConnection(connectionString))
                {

                    conn.Open();
                    string select = "data->'Id' as Id, data->'TagName'->'" + language.ToLower() + "' as Name";
                    string orderby = "";
                    string where = "";

                    if (smgtagtypelist.Count > 0)
                    {
                        if (smgtagtypelist.Count == 1)
                        {
                            where = where + "data @> '{\"ValidForEntity\": [\"" + smgtagtypelist.FirstOrDefault().ToLower() + "\"] }'";
                        }
                        else
                        {
                            string smgtagtypeliststring = "";
                            foreach (var smgtagtypeid in smgtagtypelist)
                            {
                                smgtagtypeliststring = smgtagtypeliststring + "data @> '{ \"ValidForEntity\": [\"" + smgtagtypeid.ToLower() + "\"]}' OR ";
                            }
                            smgtagtypeliststring = smgtagtypeliststring.Remove(smgtagtypeliststring.Length - 4);

                            where = where + smgtagtypeliststring;
                        }
                    }

                    where = where + "AND data->'TagName'->>'" + language.ToLower() + "' NOT LIKE ''";

                    var data = PostgresSQLHelper.SelectFromTableDataAsIdAndString(conn, "smgtags", select, where, orderby, 0, null, new List<string>() { "Id", "Name" });

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
}