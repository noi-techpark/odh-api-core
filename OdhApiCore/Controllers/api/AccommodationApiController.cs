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

namespace OdhApiCore.Controllers
{
    /// <summary>
    /// Accommodation Api (data provided by LTS / Availability Requests provided by HGV/LTS) SOME DATA Available as OPENDATA 
    /// </summary>
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class AccommodationController : OdhController
    {      
        public AccommodationController(IWebHostEnvironment env, ISettings settings, ILogger<ActivityController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {
        }

        #region SWAGGER Exposed API

        //ACCO TYPES

        /// <summary>
        /// GET Accommodation Types List
        /// </summary>
        /// <returns>Collection of AccommodationType Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ArticleTypes), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,AccoReader")]
        [HttpGet, Route("AccommodationTypes")]
        public Task<IActionResult> GetAllAccommodationTypesList(CancellationToken cancellationToken)
        {
            return GetAccoTypeList(cancellationToken);
        }

        /// <summary>
        /// GET Accommodation Types Single
        /// </summary>
        /// <param name="id">ID of the AccommodationType</param>
        /// <returns>AccommodationType Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ArticleTypes), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,AccoReader")]
        [HttpGet, Route("AccommodationTypes/{*id}")]
        public Task<IActionResult> GetAllAccommodationTypessingle(string id, CancellationToken cancellationToken)
        {
            return GetAccoTypeSingle(id, cancellationToken);
        }

        /// <summary>
        /// GET Accommodation Feature List (LTS Features)
        /// </summary>
        /// <param name="source">IF source = "lts" the Features list is returned in XML Format directly from LTS, (default: blank)</param>
        /// <returns>Collection of AccoFeatures Object / XML LTS</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ArticleTypes), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,AccoReader")]
        [HttpGet, Route("AccommodationFeatures")]
        public Task<IActionResult> GetAllAccommodationFeaturesList(string source, CancellationToken cancellationToken = default)
        {
            if (!String.IsNullOrEmpty(source) && source == "lts")
                return null;
            //return GetFeatureList(cancellationToken); TODO
            else
                return GetAccoFeatureList(cancellationToken);
        }

        /// <summary>
        /// GET Accommodation Feature Single (LTS Features)
        /// </summary>
        /// <param name="id">ID of the AccommodationFeature</param>
        /// <returns>AccoFeatures Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(AccoFeature), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,AccoReader")]
        [HttpGet, Route("AccommodationFeatures/{id}", Name = "SingleAccommodationFeatures")]
        public Task<IActionResult> GetAllAccommodationFeaturesSingle(string id, CancellationToken cancellationToken)
        {
            return GetAccoFeatureSingle(id, cancellationToken);
        }


        #endregion


        #region CUSTOM METHODS

        private Task<IActionResult> GetAccoTypeList(CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("accommodationtypes")
                        .SelectRaw("data")
                        .When(FilterClosedData, q => q.FilterClosedData());

                var data = await query.GetAsync<JsonRaw?>();

                return data;
            });
        }

        private Task<IActionResult> GetAccoTypeSingle(string id, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("accommodationtypes")
                        .Select("data")
                         //.WhereJsonb("Key", "ilike", id)
                         .Where("id", id.ToLower())
                        .When(FilterClosedData, q => q.FilterClosedData());
                //.Where("Key", "ILIKE", id);

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data;
            });
        }

        private Task<IActionResult> GetAccoFeatureList(CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("accommodationfeatures")
                        .SelectRaw("data")
                        .When(FilterClosedData, q => q.FilterClosedData());

                var data = await query.GetAsync<JsonRaw?>();

                return data;
            });
        }

        private Task<IActionResult> GetAccoFeatureSingle(string id, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("accommodationfeatures")
                        .Select("data")
                         //.WhereJsonb("Key", "ilike", id)
                         .Where("id", id.ToUpper())
                        .When(FilterClosedData, q => q.FilterClosedData());
                //.Where("Key", "ILIKE", id);

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data;
            });
        }


        #endregion

    }
}