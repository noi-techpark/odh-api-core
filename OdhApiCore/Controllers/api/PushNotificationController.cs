using DataModel;
using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PushServer;
using SqlKata.Execution;
using System.Linq;
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
        /// GET Send a PushMessage to NOI Pushserver
        /// </summary>
        /// <param name="message">PushServerMessage Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataCreate")]
        [HttpGet, Route("PushNotification/{type}/{id}")]
        public async Task<IActionResult> Get(string type, string id)
        {
            //Get the object
            var mytable = ODHTypeHelper.TranslateTypeString2Table(type);
            var mytype = ODHTypeHelper.TranslateTypeString2Type(type);

            var query =
              QueryFactory.Query(mytable)
                  .Select("data")
                  .Where("id", ODHTypeHelper.ConvertIdbyTypeString(type,id))
                  .When(FilterClosedData, q => q.FilterClosedData());

            var fieldsTohide = FieldsToHide;

            var data = await query.FirstOrDefaultAsync<JsonRaw?>();

            var myobject = ODHTypeHelper.ConvertJsonRawToObject(type, data);

            //TODO Create a logic that constructs a message out of the object

            var pushserverconfig = settings.PushServerConfig;

            //TODO Construct the message
            var message = new PushServerMessage();

            var result = await SendToPushServer.SendMessageToPushServer(pushserverconfig.ServiceUrl, message, pushserverconfig.User, pushserverconfig.Password, message.destination.language);

            return Ok(result);
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


        /// <summary>
        /// POST Send a PushMessage to NOI Pushserver
        /// </summary>
        /// <param name="message">PushServerMessage Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate")]
        [HttpPost, Route("FCMMessage/{identifier}")]
        public async Task<IActionResult> PostFCMMessage(string identifier, [FromBody] FCMModels message)
        {
            //TODO add configurable FCM setting where config can be accessed by identifier
            var pushserverconfig = settings.FCMConfig.Where(x => x.Identifier == identifier).FirstOrDefault();

            if (pushserverconfig != null)
            {
                //Complete the message
                

                var result = await FCMPushNotification.SendNotification(message, " https://fcm.googleapis.com/fcm/send", pushserverconfig.SenderId, pushserverconfig.ServerKey);

                return Ok(result);
            }
            else
                return BadRequest("not found");
        }

    }
}
