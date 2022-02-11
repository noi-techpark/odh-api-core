using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace OdhApiCore.Controllers.api
{
    public class PushNotificationController : OdhController
    {
        public PushNotificationController(IWebHostEnvironment env, ISettings settings, ILogger<ActivityController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {

        }

    }
}
