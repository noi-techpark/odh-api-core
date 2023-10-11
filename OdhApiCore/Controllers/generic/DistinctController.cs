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
using SqlKata;
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

                var select = string.Join(
                    ", ",
                    fields.Select(field => field.Replace("[", "\\[").Replace("]", "\\]"))
                          .Select(field => $@"jsonb_path_query(data, '$.{field}')#>>'\{{\}}' as ""{field}""")
                );

                var query =
                        QueryFactory.Query()
                        .Distinct()
                        .SelectRaw(select)
                        .From(table)
                        .ApplyRawFilter(rawfilter);
                        // .Anonymous_Logged_UserRule_GeneratedColumn(FilterClosedData, !ReducedData);
                
                var data = await query.GetAsync();
                
                Console.WriteLine(JsonConvert.SerializeObject(data));

                return data;
            });
        }
        
        #endregion
    }
}