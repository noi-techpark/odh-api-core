using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace OdhApiCore.Controllers.api
{
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class LocationController : OdhController
    {
        private readonly IHttpClientFactory httpClientFactory;

        public LocationController(IWebHostEnvironment env, ISettings settings, ILogger<AccommodationController> logger, QueryFactory queryFactory, IHttpClientFactory httpClientFactory)
           : base(env, settings, logger, queryFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        #region SWAGGER Exposed Api

        #endregion

        #region Private GETTER

        #endregion
    }
}
