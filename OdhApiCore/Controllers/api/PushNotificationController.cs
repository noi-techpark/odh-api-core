using DataModel;
using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PushServer;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
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

        #region Using NOI PushServer

        /// <summary>
        /// GET Send a PushMessage to NOI Pushserver
        /// </summary>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,PushMessageWriter")]
        [HttpGet, Route("PushNotification/{type}/{id}")]
        public async Task<IActionResult> Get(string type, string id)
        {
            //Get the object
            var mytable = ODHTypeHelper.TranslateTypeString2Table(type);
            var mytype = ODHTypeHelper.TranslateTypeString2Type(type);

            var query =
              QueryFactory.Query(mytable)
                  .Select("data")
                  .Where("id", ODHTypeHelper.ConvertIdbyTypeString(type, id))
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
        [Authorize(Roles = "DataWriter,DataCreate,PushMessageWriter")]
        [HttpPost, Route("PushNotification")]
        public async Task<IActionResult> Post([FromBody] PushServerMessage message)
        {
            var pushserverconfig = settings.PushServerConfig;

            var result = await SendToPushServer.SendMessageToPushServer(pushserverconfig.ServiceUrl, message, pushserverconfig.User, pushserverconfig.Password, message.destination.language);

            return Ok(result);
        }

        #endregion

        #region Using FCM Google Api

        /// <summary>
        /// GET Send a PushMessage to FCM Google Api
        /// </summary>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,PushMessageWriter")]
        [HttpGet, Route("FCMMessage/{type}/{id}/{identifier}/{language}")]
        public async Task<IActionResult> GetFCM(string type, string id, string identifier, string language)
        {
            try
            {
                //Get the object
                var mytable = ODHTypeHelper.TranslateTypeString2Table(type);
                var mytype = ODHTypeHelper.TranslateTypeString2Type(type);

                var query =
                  QueryFactory.Query(mytable)
                      .Select("data")
                      .Where("id", ODHTypeHelper.ConvertIdbyTypeString(type, id))
                      .When(FilterClosedData, q => q.FilterClosedData());

                var fieldsTohide = FieldsToHide;

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                var myobject = ODHTypeHelper.ConvertJsonRawToObject(type, data);

                //Multilanguage support
                var langarr = language.Split(',');

                List<FCMModels> messages = new List<FCMModels>();

                foreach (var lang in langarr)
                {
                    //Construct the message
                    var message = FCMMessageConstructor.ConstructMyMessage(identifier, lang.ToLower(), myobject);

                    if (message != null)
                        messages.Add(message);
                    else
                        throw new Exception("Message could not be constructed");
                }

                var pushserverconfig = settings.FCMConfig.Where(x => x.Identifier == identifier).FirstOrDefault();

                if (pushserverconfig == null)
                    throw new Exception("PushserverConfig could not be found");

                Dictionary<string, FCMPushNotificationResponse> resultlist = new Dictionary<string, FCMPushNotificationResponse>();

                foreach (var message in messages)
                {
                    var result = await FCMPushNotification.SendNotification(message, " https://fcm.googleapis.com/fcm/send", pushserverconfig.SenderId, pushserverconfig.ServerKey);

                    resultlist.Add(message.to, result);
                }

                return Ok(resultlist);
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }

        }


        /// <summary>
        /// POST Send a PushMessage directly to Google Api
        /// </summary>
        /// <param name="message">FCMModels Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,PushMessageWriter")]
        [HttpPost, Route("FCMMessage/{identifier}")]
        public async Task<IActionResult> PostFCMMessage(string identifier, [FromBody] FCMModels message)
        {
            //TODO add configurable FCM setting where config can be accessed by identifier
            var pushserverconfig = settings.FCMConfig.Where(x => x.Identifier == identifier).FirstOrDefault();

            if (pushserverconfig != null)
            {
                var result = await FCMPushNotification.SendNotification(message, " https://fcm.googleapis.com/fcm/send", pushserverconfig.SenderId, pushserverconfig.ServerKey);

                return Ok(result);
            }
            else
                return BadRequest("not found");
        }

        #endregion
    }
}
