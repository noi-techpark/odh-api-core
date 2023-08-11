// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using AspNetCore.CacheOutput;
using DataModel;
using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OdhApiCore.Responses;
using ServiceReferenceLCS;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class DistinctController : OdhController
    {        
        public DistinctController(IWebHostEnvironment env, ISettings settings, ILogger<ODHTagController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {
        }

        #region SWAGGER Exposed API
      
        /// <summary>
        /// GET Generic Distinct search of fields
        /// </summary>
        /// <param name="odhtype">Mandatory search trough Entities (metadata, accommodation, odhactivitypoi, event, webcam, measuringpoint, ltsactivity, ltspoi, ltsgastronomy, article ..... )</param>
        /// <param name="fields">Mandatory Select a fields for the Distinct Query, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Array of string</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[OdhCacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 3600, CacheKeyGenerator = typeof(CustomCacheKeyGenerator), MustRevalidate = true)]
        [HttpGet, Route("Distinct")]        
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        public async Task<IActionResult> GetDistinctAsync(
            string? odhtype = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? rawfilter = null,
            string? rawsort = null,            
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            var fieldstodisplay = fields ?? Array.Empty<string>();
            
            return await GetDistinct(validforentity: odhtype, fieldstodisplay, rawfilter, rawsort, removenullvalues: removenullvalues, null, cancellationToken);
        }

        #endregion
    
        #region GETTER

        private Task<IActionResult> GetDistinct(
            string validforentity, string[] fields, string? rawfilter, string? rawsort, bool removenullvalues,
            PGGeoSearchResult geosearchresult, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                if (validforentity == null)
                    return BadRequest("odhtype missing");

                if (fields.Count() == 0)
                    return BadRequest("please add a field");

                var table = ODHTypeHelper.TranslateTypeString2Table(validforentity);

                //Generates a distinct()
                var select = GetSelectDistinct(fields);

                //Custom Fields filter
                //if (fields.Length > 0)
                //    select += string.Join("", fields.Where(x => x != "Id").Select(field => $", data#>>'\\{{{field.Replace(".", ",")}\\}}' as \"{field}\""));

                ////Remove first , from select
                //if (select.StartsWith(", "))
                //    select = select.Substring(2);        

                var query =
                        QueryFactory.Query()
                        .SelectRaw(select.select)
                        .From(table)
                        .ApplyRawFilter(rawfilter)
                        //.ApplyOrdering_GeneratedColumns(geosearchresult, rawsort)
                        .Anonymous_Logged_UserRule_GeneratedColumn(FilterClosedData, !ReducedData);
                
                //Get as Text
                var newquery =
                    QueryFactory.Query()
                    //.SelectRaw("\"Sources\"->>0")
                    .SelectRaw(select.mainselect)
                    .From(query, "subsel")
                    .Distinct();

                //var data = await newquery.GetAsync<string>();

                var test = await newquery.GetAsync();



                //var fieldsTohide = FieldsToHide;

                return test;  //.Select(raw => raw.TransformRawData(null, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide))
                        //.Where(json => json != null)
                        //.Select(json => json!);
            });
        }

        private DistinctSelectObj GetSelectDistinct(string[] fields)
        {
            DistinctSelectObj myselectobj = new DistinctSelectObj();            

            string select = "";
            string mainselect = "";
            
            //Custom Fields filter
            if (fields.Length > 0)
            {
                //select += string.Join("", fields.Where(x => !x.Contains("[*]") && !x.Contains("[]")).Select(field => $", data#>>'\\{{{field.Replace(".", ",")}\\}}' as \"{field}\""));                

                foreach(var field in fields.Where(x => !x.Contains("[*]") && !x.Contains("[]")))
                {
                    select += $", data#>>'\\{{{field.Replace(".", ",")}\\}}' as \"{field}\"";

                    myselectobj.selectinfo.TryAddOrUpdate(field, false);

                    mainselect += $", \"{field}\" as \"{field}\"";
                }

                //select += string.Join("", fields.Where(x => x.Contains("[*]") || x.Contains("[]")).Select(field => $", unnest(json_array_to_pg_array(data#>'\\{{{field.Replace(".[*]", "").Replace(".[]", "").Replace(".", ",") }\\}}'))"));

                foreach (var field in fields.Where(x => x.Contains("[*]") || x.Contains("[]")))
                {
                    // Tags.[*].Id --> jsonb_path_query(data#>'{Tags}', '$[*] ? (@ <> null).Id')   Tags|.Id
                    // Sources.[*] --> jsonb_path_query(data#>'{Sources}', '$[*] ? (@ <> null)') Sources|

                    var tempfield = field.Replace(".[*]", "|").Replace(".[].", "|");
                    var splitted = tempfield.Split('|');

                    select += $", jsonb_path_query(data#>'\\{{{splitted[0].Replace(".", ",")}\\}}', '$\\[*\\] ? (@ <> null){splitted[1]}') as \"{splitted[0] + splitted[1]}\"";

                    myselectobj.selectinfo.TryAddOrUpdate(splitted[0] + splitted[1], true);

                    mainselect += $", \"{splitted[0] + splitted[1]}\"->>0 as \"{splitted[0] + splitted[1]}\"";
                }
                
                //SELECT DISTINCT hallo->> 0
                //from(select jsonb_path_query(data#>'{Sources}', '$[*] ? (@ <> null)') as hallo from metadata) as test
            }                

            //Remove first , from select
            if (select.StartsWith(", "))
                select = select.Substring(2);

            if (mainselect.StartsWith(", "))
                mainselect = mainselect.Substring(2);            

            myselectobj.select = select;
            myselectobj.mainselect = mainselect;

            return myselectobj;
        }
        

        #endregion
    }

    public class DistinctSelectObj
    {
        public DistinctSelectObj()
        {
            selectinfo = new Dictionary<string, bool>();
        }

        public string select { get; set; }
        public string mainselect { get; set; }        
        public Dictionary<string, bool> selectinfo { get; set; }
    }
}