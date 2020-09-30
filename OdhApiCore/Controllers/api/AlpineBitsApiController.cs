using System;
using System.Collections.Generic;
using System.Linq;
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
    public class AlpineBitsController : OdhController
    {
        private readonly ISettings settings;

        public AlpineBitsController(IWebHostEnvironment env, ISettings settings, ILogger<AlpineBitsController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {
            this.settings = settings;
        }

        #region SwaggerExposed API



        #endregion
    }
}
