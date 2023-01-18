﻿using DataModel;
using Helper;
using OdhNotifier;
using System.ComponentModel;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OdhNotifier
{
    public class OdhPushNotifier
    {
        public static async Task<HttpResponseMessage> SendNotify(NotifyMeta notify)
        {
            return await Notify(notify);
        }

        private static async Task<HttpResponseMessage> Notify(NotifyMeta notify)
        {
            var requesturl = notify.Url;
            bool imageupdate = true;

            try
            {
                if (CheckValidTypes(notify))
                {
                    using (var client = new HttpClient())
                    {
                        //Add all Headers
                        if (notify.Headers != null)
                        {
                            foreach (var header in notify.Headers)
                            {
                                client.DefaultRequestHeaders.Add(header.Key, header.Value);
                            }
                        }

                        //Add Referer Header
                        if (!String.IsNullOrEmpty(notify.Referer))
                            client.DefaultRequestHeaders.Referrer = new Uri(notify.Referer);

                        //Add Additional Parameters
                        if (notify.Parameters != null)
                        {
                            requesturl = requesturl + "?";
                            foreach (var parameter in notify.Parameters)
                            {
                                if (parameter.Key == "skipimage" && parameter.Value == "true")
                                    imageupdate = false;

                                requesturl = requesturl + parameter.Key + "=" + parameter.Value;

                                if (notify.Parameters.Last().Key != parameter.Key)
                                {
                                    requesturl = requesturl + "&";
                                }
                            }
                        }

                        HttpResponseMessage response = default(HttpResponseMessage);

                        //GET or POST the data to the service
                        if (notify.Mode.ToLower() == "get")
                        {
                            response = await client.GetAsync(requesturl);
                        }
                        else if (notify.Mode.ToLower() == "post")
                        {
                            var data = new StringContent(JsonSerializer.Serialize(new
                            {
                                id = notify.Id,
                                entity = notify.Type
                            }));

                            data.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                            response = await client.PostAsync(requesturl, data);
                        }


                        if (response != null && response.StatusCode == HttpStatusCode.OK)
                        {
                            return ReturnHttpResponse(response, notify, imageupdate, "");
                        }
                        else if (response != null)
                        {
                            return ReturnHttpResponse(response, notify, imageupdate, response.ReasonPhrase);
                        }
                        else
                        {
                            return ReturnHttpResponse(response, notify, imageupdate, "no response");
                        }
                    }
                }
                else
                    throw new Exception("type not valid!");
            }
            catch (Exception ex)
            {
                var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                return ReturnHttpResponse(response, notify, imageupdate, ex.Message);
            }
        }

        public static HttpResponseMessage ReturnHttpResponse(HttpResponseMessage response, NotifyMeta notify, bool imageupdate, string error)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                GenerateLog(notify.Id, notify.Destination, notify.Type + ".push.trigger", "api", notify.UdateMode, imageupdate, null);

                //clear requestmessage
                response.RequestMessage = null;

                return response;
            }
            else
            {
                GenerateLog(notify.Id, notify.Destination, notify.Type + ".push.error", "api", notify.UdateMode, imageupdate, error);

                //clear requestmessage
                response.RequestMessage = null;

                return response;
            }
        }

        public static void GenerateLog(string id, string destination, string message, string origin, string updatemode, bool? imageupdate, string? exception)
        {
            NotifyLog log = new NotifyLog() { id = id, destination = destination, message = message, origin = origin, imageupdate = imageupdate, updatemode = updatemode };
            Console.WriteLine(JsonSerializer.Serialize(log));
        }

        public static async Task WriteToFailureQueue(string id, string type, string url, string service, string exmessage)
        {
            PushFailureQueue myfailure = new PushFailureQueue();
            myfailure.Id = "";
            myfailure.ItemId = id;
            myfailure.Type = type;
            myfailure.Exception = exmessage;
            myfailure.LastChange = DateTime.Now;
            myfailure.Service = service;
            myfailure.PushUrl = url;
            myfailure.Status = "open";
            myfailure.RetryCount = 1;

            ////Not best practice but should work
            //using (var session = GlobalDocumentStore.DocumentStore.OpenAsyncSession())
            //{
            //    await session.StoreAsync(myfailure);
            //    await session.SaveChangesAsync();
            //}
        }

        //Valid Types for Push
        private static bool CheckValidTypes(NotifyMeta notify)
        {
            if (notify.ValidTypes.Select(x => x.ToLower()).Contains(notify.Type.ToLower()))
                return true;
            else
                return false;
        }
    }


    public class NotifyLog
    {
        public string message { get; set; }
        public string id { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public bool? imageupdate { get; set; }
        public string updatemode { get; set; }
    }

    public class PushFailureQueue
    {
        public string Id { get; set; }
        public string ItemId { get; set; }
        public string Type { get; set; }
        public string Exception { get; set; }
        public string Status { get; set; }
        public string PushUrl { get; set; }
        public string Service { get; set; }
        public DateTime LastChange { get; set; }
        public Nullable<int> RetryCount { get; set; }
    }

    public class PushToAll
    {
        public static async Task PushToAllRegisteredServices(List<NotifierConfig> notifierconfiglist, string id, string type, string updatemode, string origin, string referer, List<string> excludeservices)
        {
            foreach (var notifyconfig in notifierconfiglist)
            {
                if (excludeservices != null && excludeservices.Contains(notifyconfig.ServiceName))
                    continue;

                NotifyMetaGenerated meta = new NotifyMetaGenerated(notifyconfig, id, type, updatemode, origin, referer);

                await OdhPushNotifier.SendNotify(meta);
            }
        }
    }

    public class NotifyMetaGenerated : NotifyMeta
    {
        public NotifyMetaGenerated(NotifierConfig notifyconfig, string id, string type, string updatemode, string origin, string? referer = null)
        {
            //Set by parameters
            this.Id = id;
            this.Type = type;
            this.UdateMode = updatemode;
            this.Origin = origin;
            this.Referer = referer;

            switch (notifyconfig.ServiceName.ToLower())
            {
                case "marketplace":

                    //From Config
                    this.Url = notifyconfig.Url;
                    this.Headers = new Dictionary<string, string>() {
                        { "client_id", notifyconfig.User },
                        { "client_secret", notifyconfig.Password }
                    };

                    //Prefilled
                    this.Destination = "marketplace";
                    this.Mode = "post";

                    this.ValidTypes = new List<string>()
                        {
                            "ACCOMMODATION",
                            "REGION",
                            "MUNICIPALITY",
                            "DISTRICT",
                            "TOURISM_ASSOCIATION",
                            "ODH_ACTIVITY_POI",
                            "EVENT",
                            "ODH_TAG"
                        };

                    //Translate Type
                    this.Type = TransformType(type, "marketplace");

                    break;
                case "sinfo":

                    //From Config
                    this.Url = notifyconfig.Url + "accommodation/2657B7CBCB85380B253D2FBE28AF100E";
                    this.Parameters = new Dictionary<string, string>() {
                        { "skipimage", "true" }
                    };

                    //Prefilled
                    this.Destination = "sinfo";
                    this.Mode = "get";

                    this.ValidTypes = new List<string>()
                        {
                            "accommodation",
                            "activity",
                            "article",
                            "event",
                            "metaregion",
                            "region",
                            "experiencearea",
                            "municipality",
                            "tvs",
                            "district",
                            "skiregion",
                            "skiarea",
                            "gastronomy",
                            "odhactivitypoi",
                            "smgtags"
                        };
                    break;
                default:
                    this.Mode = "get";
                    this.Url = notifyconfig.Url;
                    break;
            }
        }

        public string TransformType(string type, string servicename)
        {
            if (servicename == "marketplace")
            {
                return type switch
                {
                    "accommodation" => "ACCOMMODATION",
                    "activity" => "NOT SUPPORTED", //ok deprecated
                    "article" => "NOT SUPPORTED", //Recipes?
                    "event" => "EVENT",
                    "metaregion" => "NOT SUPPORTED", //to check
                    "region" => "REGION",
                    "experiencearea" => "NOT SUPPORTED", //to check
                    "municipality" => "MUNICIPALITY",
                    "tvs" => "TOURISM_ASSOCIATION",
                    "district" => "DISTRICT",
                    "skiregion" => "NOT SUPPORTED",  //to check
                    "skiarea" => "NOT SUPPORTED", //to check
                    "gastronomy" => "NOT SUPPORTED", //ok deprecated
                    "odhactivitypoi" => "ODH_ACTIVITY_POI",
                    "smgtags" => "ODH_TAG",
                    _ => "NOT SUPPORTED"
                };
            }
            else
                return type;
        }

    } 
}