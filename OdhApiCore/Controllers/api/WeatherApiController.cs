using DataModel;
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
    /// WebcamInfo Api (data provided by LTS ActivityData) SOME DATA Available as OPENDATA
    /// </summary>
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class WeatherApiController : OdhController
    {        
        public WeatherApiController(IWebHostEnvironment env, ISettings settings, ILogger<ActivityController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {
        }   
    }
}