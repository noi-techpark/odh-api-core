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
    /// Gastronomy Api (data provided by LTS GastronomyData) SOME DATA Available as OPENDATA
    /// </summary>
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class GastronomyController : OdhController
    {
        // Only for test purposes

        public GastronomyController(IWebHostEnvironment env, ISettings settings, ILogger<GastronomyController> logger, IPostGreSQLConnectionFactory connectionFactory, Factories.PostgresQueryFactory queryFactory)
            : base(env, settings, logger, connectionFactory, queryFactory)
        {
        }

        #region SWAGGER Exposed API

        

        #endregion

        #region GETTER

        
        #endregion

        #region CUSTOM METHODS

      
        #endregion

        #region Obsolete here for Compatibility reasons


        #endregion
    }
}