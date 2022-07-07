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

        #region Using NOI PushServer

        /// <summary>
        /// GET Send a PushMessage to NOI Pushserver
        /// </summary>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,PushMessageWriter")]
        [HttpGet, Route("PushNotification/{type}/{id}")]
        public async Task<IActionResult> Get(string type, string id )
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

            //Construct the message
            var message = ConstructMyMessage(identifier, language, myobject);

            if (message != null)
                return await PostFCMMessage(identifier, message);
            else
                return BadRequest("Message could not be created");
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
                //Complete the message
                

                var result = await FCMPushNotification.SendNotification(message, " https://fcm.googleapis.com/fcm/send", pushserverconfig.SenderId, pushserverconfig.ServerKey);

                return Ok(result);
            }
            else
                return BadRequest("not found");
        }

        #endregion

        #region Helpers

        public static FCMModels ConstructMyMessage(string identifier, string language, IIdentifiable myobject)
        {
            var message = default(FCMModels);

            if(identifier == "it.bz.noi.community" && myobject is ArticlesLinked)
            {
                message.to = "/topics/newsfeednoi_" + language.ToLower();

                string deeplink = "noi-community://it.bz.noi.community/newsDetails/" + myobject.Id;

                message = new FCMModels();
                message.data = new { deep_link = deeplink };
                FCMNotification notification = new FCMNotification();

                message.notification = notification;
            }

            return message;
        }

        //{
        //    "to": "/topics/newsfeednoi_it",
	       // "notification": {        
		      //  "title": "News Push",
		      //  "body": "Check out the news feed test",
		      //  "sound": "default",
		      //  "link": "noi-community://it.bz.noi.community/newsDetails/B5159512-A6E7-42C6-8792-1E947B3531A7",
        //        "icon": "ic_notification"
	       // },
	       // "data": {
		      //  "deep_link": "noi-community://it.bz.noi.community/newsDetails/B5159512-A6E7-42C6-8792-1E947B3531A7"
	       // }
        //}


    #endregion
}
}
