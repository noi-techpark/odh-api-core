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
using System.Drawing.Printing;
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
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="odhtype">Mandatory search trough Entities (metadata, accommodation, odhactivitypoi, event, webcam, measuringpoint, ltsactivity, ltspoi, ltsgastronomy, article ..... )</param>
        /// <param name="fields">Mandatory Select a fields for the Distinct Query, More fields are indicated by separator ',' example fields=Source, arrays are selected with a .[*] example HasLanguage.[*] / Features.[*].Id  (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawsort</a></param>
        /// <param name="getasarray">Get only first selected field as simple string Array</param>        
        /// <returns>Array of string/object</returns>        
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
            uint? pagenumber = null,
            PageSize pagesize = null!,
            string? odhtype = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? seed = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool getasarray = false,            
            CancellationToken cancellationToken = default)
        {
            var fieldstodisplay = fields ?? Array.Empty<string>();
            
            return await GetDistinct(pagenumber, pagesize, odhtype, fieldstodisplay, seed, rawfilter, rawsort, getasarray, null, cancellationToken);
        }

        #endregion
    
        #region GETTER

        private Task<IActionResult> GetDistinct(uint? pagenumber, int? pagesize,
            string odhtype, string[] fields, string? seed, string? rawfilter, string? rawsort, bool? getasarray,
            PGGeoSearchResult geosearchresult, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                if (odhtype == null)
                    return BadRequest("odhtype missing");

                if (fields.Count() == 0)
                    return BadRequest("please add a field");

                var table = ODHTypeHelper.TranslateTypeString2Table(odhtype);

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
                        //.ApplyOrdering(new PGGeoSearchResult() { geosearch = false }, rawsort)
                        .Anonymous_Logged_UserRule_GeneratedColumn(FilterClosedData, !ReducedData);
                
                //Get as Text
                var mainquery =
                    QueryFactory.Query()
                    //.SelectRaw("\"Sources\"->>0")
                    .SelectRaw(select.mainselect)
                    .From(query, "subsel")
                    .Distinct()
                    .OrderByRawIfNotNull(GetOrderStatement(rawsort, select));

                if(getasarray != null && getasarray.Value)
                {
                    var data = await mainquery.GetAsync<string>();

                    return data;
                }                

                //Get Paged
                if (pagenumber.HasValue)
                {
                    // Get paginated data
                    var data =
                        await mainquery
                            .PaginateAsync(
                            page: (int)pagenumber,
                                perPage: pagesize ?? 25);

                    uint totalpages = (uint)data.TotalPages;
                    uint totalcount = (uint)data.Count;

                    return ResponseHelpers.GetResult<dynamic>(
                        pagenumber.Value,
                        totalpages,
                        totalcount,
                        seed,
                        data.List,
                        Url);
                }
                else
                {
                    var data = await mainquery.GetAsync();
                    return data;
                }

                //var fieldsTohide = FieldsToHide;

                //.Select(raw => raw.TransformRawData(null, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide))
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

                    //The use of json_path_query filters out all null values but when used two json_pah_querys the result is always a tuple <null, value> <value, value> need to investigate
                    //select += $", jsonb_path_query(data#>'\\{{{field.Replace(".", ",")}\\}}', '$\\[*\\] ? (@ <> null)') as \"{field}\"";

                    myselectobj.selectinfo.TryAddOrUpdate(field, false);

                    mainselect += $", \"{field}\" as \"{field}\"";
                    //mainselect += $", \"{field}\"->>0 as \"{field}\"";
                }

                //select += string.Join("", fields.Where(x => x.Contains("[*]") || x.Contains("[]")).Select(field => $", unnest(json_array_to_pg_array(data#>'\\{{{field.Replace(".[*]", "").Replace(".[]", "").Replace(".", ",") }\\}}'))"));

                foreach (var field in fields.Where(x => x.Contains("[*]") || x.Contains("[]")))
                {
                    string astoadd = "";
                    if (field.Contains("[*]"))
                        astoadd = ".\\[*\\]";
                    if (field.Contains("[]"))
                        astoadd = ".\\[\\]";

                    // Tags.[*].Id --> jsonb_path_query(data#>'{Tags}', '$[*] ? (@ <> null).Id')   Tags|.Id
                    // Sources.[*] --> jsonb_path_query(data#>'{Sources}', '$[*] ? (@ <> null)') Sources|

                    var tempfield = field.Replace(".[*]", "|").Replace(".[].", "|");
                    var splitted = tempfield.Split('|');

                    select += $", jsonb_path_query(data#>'\\{{{splitted[0].Replace(".", ",")}\\}}', '$\\[*\\] ? (@ <> null){splitted[1]}') as \"{splitted[0] + splitted[1]}\"";

                    myselectobj.selectinfo.TryAddOrUpdate(splitted[0] + astoadd + splitted[1], true);

                    mainselect += $", \"{splitted[0] + splitted[1]}\"->>0 as \"{splitted[0] + astoadd + splitted[1]}\"";
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
        
        private string? GetOrderStatement(string? rawsort, DistinctSelectObj distinctselectobj)
        {
            if (String.IsNullOrEmpty(rawsort))
                return null;

            string orderby = "";

            var rawsorttemp = rawsort.Split(',');

            foreach (var rawsortitem in rawsorttemp)
            {
                string direction = " ASC";
                string rawsortfield = rawsortitem;

                if (rawsortitem.StartsWith("-"))
                {
                    direction = " DESC";
                    rawsortfield = rawsortitem.Replace("-", "");
                }
                    
                if(distinctselectobj.selectinfo.Select(x => x.Key.Replace("\\", "")).Contains(rawsortfield))
                {            
                    var rawsortterm = distinctselectobj.selectinfo.Where(x => x.Key.Replace("\\", "").Contains(rawsortfield)).FirstOrDefault();

                    if(rawsortterm.Key != null)
                        orderby += $"\"{rawsortterm.Key}\" {direction} ,";
                }                
            }

            if(orderby.EndsWith(","))
                orderby = orderby.Substring(0, orderby.Length - 1);

            return orderby;
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