using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OdhApiCore.Controllers.api
{
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class AlpineBitsApiController : OdhController
    {

    }
}
