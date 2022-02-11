using DataModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PushServer;
using SqlKata.Execution;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.api
{
    public class PushNotificationController : OdhController
    {
        private readonly ISettings settings;

        public PushNotificationController(IWebHostEnvironment env, ISettings settings, ILogger<ActivityController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {
            this.settings = settings;
        }

        /// <summary>
        /// POST Send a PushMessage to NOI Pushserver
        /// </summary>
        /// <param name="message">PushServerMessage Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate")]        
        [HttpPost, Route("PushNotification")]
        public async Task<IActionResult> Post([FromBody] PushServerMessage message)
        {
            var pushserverconfig = settings.PushServerConfig;

            var result = await SendToPushServer.SendMessageToPushServer(pushserverconfig.ServiceUrl, message, pushserverconfig.User, pushserverconfig.Password, message.destination.language);

            return Ok(result);
        }

    }
}
