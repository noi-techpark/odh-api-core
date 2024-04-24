// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OdhNotifier;
using PushServer;
using SqlKata;
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
        private readonly IWebHostEnvironment env;

        public PushNotificationController(IWebHostEnvironment env, ISettings settings, ILogger<PushNotificationController> logger, QueryFactory queryFactory, IOdhPushNotifier odhpushnotifier)
            : base(env, settings, logger, queryFactory, odhpushnotifier)
        {
            this.settings = settings;
            this.env = env;
        }

        #region Exposed Generic Endpoint

        /// <summary>
        /// POST Generic Push Endpoint
        /// </summary>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,PushMessageWriter")]
        [HttpPost, Route("PushData/{id}/{type}/{publisher}")]
        public async Task<IActionResult> PushData(string id, string type, string publisher, bool usemocks = true, string ? language = null)
        {
            IDictionary<string, PushResponse> resultdict = new Dictionary<string, PushResponse>();

            //If environment is set on production deactivate usemocks
            if (!env.IsDevelopment())
                usemocks = false;

            foreach (var publish in publisher.Split(","))
            {
                var data = new PushResponse();
                data.Publisher = publish;
                data.Date = DateTime.Now;
                data.Id = Guid.NewGuid().ToString();

                PushObject pushobject = new PushObject();
                pushobject.Type = type;
                pushobject.Id = id;

                data.PushObject = pushobject;

                //Check if data can be pushed
                var checkobjectresult = await CheckPublishedOnAttribute(id, type, publish);
                
                if (checkobjectresult.Item1)
                {
                    switch (publish)
                    {
                        //FCM Push
                        case "noi-communityapp":
                            data.Result = usemocks ? new PushResult() { Error = "", Success = true, Messages = 1, Response = "This is a mockup response" } : await GetDataAndSendFCMMessage(type, id, publish, language);
                            break;

                        //NOTIFIER
                        case "idm-marketplace":
                            if (usemocks)
                                data.Result = new NotifierResponse() { HttpStatusCode = System.Net.HttpStatusCode.OK, Response = "This is a mockup response", Service = "idm-marketplace", Success = true };
                            else
                            {
                                var notifierresult = await OdhPushnotifier.PushToPublishedOnServices(id, type, "manual.push", new Dictionary<string, bool> { { "imageschanged", true } }, false, "push.api", new List<string>() { publish });
                                data.Result = notifierresult != null &&
                                              notifierresult.ContainsKey(publish) ?
                                              notifierresult[publish] :
                                              new { Success = false };
                            }
                            break;
                        default:
                            data.Result = new { Response = "Publisher is not registered on this api", Success = false };
                            break;
                    }
                }
                else
                {
                    data.Result = new { Response = checkobjectresult.Item2, Success = false };
                }


                //Save the result in a push table
                var insertresult = await QueryFactory.Query("pushresults")
                    .InsertAsync(new JsonBData() { id = data.Id, data = new JsonRaw(data) });

                resultdict.Add(publish, data);
            }

            return Ok(resultdict);
        }

        private async Task<(bool, string)> CheckPublishedOnAttribute(string id, string type, string publish)
        {
            //Check if the object has the publisher in the PublishedOn Array
            var mytable = ODHTypeHelper.TranslateTypeString2Table(type);
           
            var query =
              QueryFactory.Query(mytable)
                  .Select("data")
                  .Where("id", ODHTypeHelper.ConvertIdbyTypeString(type, id));

            var data = await query.FirstOrDefaultAsync<JsonRaw?>();

            if (data is not { })
                return (false, "data not found");

            var myobject = ODHTypeHelper.ConvertJsonRawToObject(type, data);


            if (myobject == null)
                return (false, "data not found");
            else if(myobject != null && myobject is IPublishedOn)
            {
                //check if publisher is set
                if ((myobject as IPublishedOn).PublishedOn != null && (myobject as IPublishedOn).PublishedOn.Contains(publish))
                    return (true,"");
                else
                    return (false,"publisher not activated for this record");
            }

            return (false, "something went wrong");
        }

        #endregion


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
                  .Where("id", ODHTypeHelper.ConvertIdbyTypeString(type, id));
                  //.When(FilterClosedData, q => q.FilterClosedData());

            // TODO: Create a logic that constructs a message out of the object

            var pushserverconfig = settings.PushServerConfig;

            // TODO: Construct the message

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
        public async Task<IActionResult> GetFCM(string type, string id, string identifier, string? language = null)
        {            
            return Ok(GetDataAndSendFCMMessage(type, id, identifier, language));            
        }

        private async Task<PushResult> GetDataAndSendFCMMessage(string type, string id, string identifier, string? language = null)
        {
            try
            {
                //Get the object
                var mytable = ODHTypeHelper.TranslateTypeString2Table(type);
                var mytype = ODHTypeHelper.TranslateTypeString2Type(type);

                var query =
                  QueryFactory.Query(mytable)
                      .Select("data")
                      .Where("id", ODHTypeHelper.ConvertIdbyTypeString(type, id));
                //.When(FilterClosedData, q => q.FilterClosedData());


                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                if (data is not { })
                    throw new Exception("no data");

                var myobject = ODHTypeHelper.ConvertJsonRawToObject(type, data);

                List<string> languages = new List<string>();

                if (language != null)
                    languages = language.Split(',').ToList();
                if (myobject != null && myobject is IHasLanguage && (myobject as IHasLanguage).HasLanguage != null)
                    languages = (myobject as IHasLanguage).HasLanguage.ToList();

                List<FCMModels> messages = new();

                foreach (var lang in languages)
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

                List<PushResult> resultlist = new();

                foreach (var message in messages)
                {
                    var result = await FCMPushNotification.SendNotification(message, " https://fcm.googleapis.com/fcm/send", pushserverconfig.SenderId, pushserverconfig.ServerKey);

                    resultlist.Add(result);
                }

                return PushResult.MergeMultipleFCMPushNotificationResponses(resultlist);
            }
            catch (Exception ex)
            {
                return new PushResult() { Error = ex.Message, Response = "Error", Success = false };
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
