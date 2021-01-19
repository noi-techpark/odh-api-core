﻿using DataModel;
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
    //[Route("ODHTag")]
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class ODHTagController : OdhController
    {
        public ODHTagController(IWebHostEnvironment env, ISettings settings, ILogger<ODHTagController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {
        }

        #region SWAGGER Exposed API

        /// <summary>
        /// GET ODHTag List
        /// </summary>
        /// <param name="validforentity">Filter on Tags valid on Entitys (accommodation, activity, poi, smgpoi, package, gastronomy, event, article, common .. etc..)</param>
        /// <param name="language"></param>
        /// <returns>Collection of ODHTag Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<SmgTags>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("ODHTag")]
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        public async Task<IActionResult> GetODHTagsAsync(
            string? language = null,
            string? validforentity = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            string? localizationlanguage = null,  //TODO ignore this in swagger
            CancellationToken cancellationToken = default)
        {
            //Compatibility
            if (String.IsNullOrEmpty(language) && !String.IsNullOrEmpty(localizationlanguage))
                language = localizationlanguage;


            return await Get(language, validforentity, fields: fields ?? Array.Empty<string>(), 
                  searchfilter, rawfilter, rawsort,
                    cancellationToken);
           
        }

        /// <summary>
        /// GET ODHTag Single
        /// </summary>
        /// <param name="id"></param>
        /// <param name="language"></param>
        /// <returns>ODHTag Object</returns>
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(SmgTags), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("ODHTag/{id}", Name = "SingleODHTag")]
        //[Authorize(Roles = "DataReader,CommonReader,AccoReader,ActivityReader,PoiReader,ODHPoiReader,PackageReader,GastroReader,EventReader,ArticleReader")]
        public async Task<IActionResult> GetODHTagSingle(string id,
            string? language = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? localizationlanguage = null,   //TODO ignore this in swagger
            CancellationToken cancellationToken = default)
        {
            //Compatibility
            if (String.IsNullOrEmpty(language) && !String.IsNullOrEmpty(localizationlanguage))
                language = localizationlanguage;

            return await GetSingle(id, language, fields: fields ?? Array.Empty<string>(), cancellationToken);
        }

        #endregion

        #region GETTER

        private Task<IActionResult> Get(string? language, string? smgtagtype, string[] fields,
            string? searchfilter, string? rawfilter, string? rawsort,
            CancellationToken cancellationToken)
        {
            var mysmgtagtypelist = (smgtagtype ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries);

            return DoAsyncReturn(async () =>
            {
                //if (String.IsNullOrEmpty(rawsort))
                //    rawsort = "data #>>'\\{MainEntity\\}', data#>>'\\{Shortname\\}'";
                
                var query = 
                    QueryFactory.Query()
                    .SelectRaw("data")
                    .From("smgtags")
                    .ODHTagWhereExpression(
                        languagelist: new List<string>(), 
                        smgtagtypelist: mysmgtagtypelist,
                        searchfilter: searchfilter,
                        language: language,
                        filterClosedData: FilterClosedData
                        )
                    .ApplyRawFilter(rawfilter)
                    .ApplyOrdering(new PGGeoSearchResult() { geosearch = false }, rawsort, "data #>>'\\{MainEntity\\}', data#>>'\\{Shortname\\}'");


                var data = await query.GetAsync<JsonRaw>();

                return data.Select(raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, urlGenerator: UrlGenerator));
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
        private Task<IActionResult> GetSingle(string id, string? language, string[] fields, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var data = await QueryFactory.Query("smgtags")
                    .Select("data")
                    .Where("id", id.ToLower())
                    .When(FilterClosedData, q => q.FilterClosedData())
                    .FirstOrDefaultAsync<JsonRaw>();

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, urlGenerator: UrlGenerator);
            });
        }

        #endregion
    }
}