using DataModel;
using Helper;
using Microsoft.AspNetCore.Server.IIS.Core;
using Newtonsoft.Json.Linq;
using OdhNotifier;
using SqlKata;
using SqlKata.Execution;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace OdhNotifier
{
    public interface IOdhPushNotifier
    {
        Task<IDictionary<string, NotifierResponse>> PushToAllRegisteredServices(string id, string type, string updatemode, bool imagechanged, bool isdelete, string origin, string? referer = null, List<string>? excludeservices = null);
        Task<IDictionary<string, NotifierResponse>> PushToPublishedOnServices(string id, string type, string updatemode, bool imagechanged, bool isdelete, string origin, List<string> publishedonlist, string? referer = null);
        Task<IDictionary<string, NotifierResponse>> PushFailureQueueToPublishedonService(List<string> publishedonlist, string? referer = null)
    }

    public class OdhPushNotifier : IOdhPushNotifier, IDisposable
    {
        private readonly ISettings settings;
        protected QueryFactory QueryFactory { get; }

        List<NotifierConfig> notifierconfiglist;

        public OdhPushNotifier(ISettings settings, QueryFactory queryFactory)
        {
            this.settings = settings;
            this.QueryFactory = queryFactory;

            this.notifierconfiglist = settings.NotifierConfig;
        }

        /// <summary>
        /// Pushes to all registered Services in config, a service can be manually excluded by passing and exclude list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="updatemode"></param>
        /// <param name="origin"></param>
        /// <param name="referer"></param>
        /// <param name="excludeservices"></param>
        /// <returns></returns>
        public async Task<IDictionary<string, NotifierResponse>> PushToAllRegisteredServices(string id, string type, string updatemode, bool imagechanged, bool isdelete, string origin, string? referer = null, List<string>? excludeservices = null)
        {
            IDictionary<string, NotifierResponse> notifierresponselist = new Dictionary<string, NotifierResponse>();

            foreach (var notifyconfig in notifierconfiglist)
            {
                if (excludeservices != null && excludeservices.Contains(notifyconfig.ServiceName.ToLower()))
                    continue;

                NotifyMetaGenerated meta = new NotifyMetaGenerated(notifyconfig, id, type, imagechanged, isdelete, updatemode, origin, referer);

                NotifierResponse notifierresponse = new NotifierResponse();

                var response = await SendNotify(meta);
                notifierresponse.HttpStatusCode = response.Item1;
                notifierresponse.Response = response.Item2;
                notifierresponse.Service = notifyconfig.ServiceName;

                notifierresponselist.TryAddOrUpdate(notifyconfig.ServiceName, notifierresponse);
            }

            return notifierresponselist;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="updatemode"></param>
        /// <param name="origin"></param>
        /// <param name="referer"></param>
        /// <param name="excludeservices"></param>
        /// <returns></returns>
        public async Task<IDictionary<string, NotifierResponse>> PushToPublishedOnServices(string id, string type, string updatemode, bool imagechanged, bool isdelete, string origin, List<string> publishedonlist, string? referer = null)
        {
            IDictionary<string, NotifierResponse> notifierresponselist = new Dictionary<string, NotifierResponse>();

            foreach (var notifyconfig in notifierconfiglist)
            {                
                if(publishedonlist.Contains(notifyconfig.ServiceName.ToLower()))
                {
                    //Compare and push?

                    NotifyMetaGenerated meta = new NotifyMetaGenerated(notifyconfig, id, type, imagechanged, isdelete, updatemode, origin, referer);

                    NotifierResponse notifierresponse = new NotifierResponse();

                    var response = await SendNotify(meta);
                    notifierresponse.HttpStatusCode = response.Item1;
                    notifierresponse.Response = response.Item2 ;
                    notifierresponse.Service = notifyconfig.ServiceName;

                    notifierresponselist.TryAddOrUpdate(notifyconfig.ServiceName, notifierresponse);
                }                
            }

            return notifierresponselist;
        }

        private async Task<Tuple<HttpStatusCode, object?>> SendNotify(NotifyMeta notify)
        {
            var requesturl = notify.Url;
            bool imageupdate = true;

            try
            {
                if (CheckValidTypes(notify))
                {
                    using (var client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(5);

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
                                entity = notify.Type,
                                skipImage = notify.HasImagechanged ? false : true,
                                isHardDelete = notify.IsDelete
                            }));

                            imageupdate = notify.HasImagechanged ? true : false;

                            data.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                            response = await client.PostAsync(requesturl, data);
                        }


                        if (response != null && (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created))
                        {
                            return await ReturnHttpResponse(response, notify, imageupdate, "");
                        }
                        else if (response != null)
                        {
                            throw new Exception(response.ReasonPhrase);
                        }
                        else
                        {
                            throw new Exception("response null");
                        }
                    }
                }
                else
                    throw new Exception("type not valid!");
            }
            catch (TaskCanceledException ex)
            {
                await WriteToFailureQueue(notify, ex.Message);

                var response = new HttpResponseMessage(HttpStatusCode.RequestTimeout);
                return await ReturnHttpResponse(response, notify, imageupdate, ex.Message);
            }
            catch (Exception ex)
            {
                await WriteToFailureQueue(notify, ex.Message);

                var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                return await ReturnHttpResponse(response, notify, imageupdate, ex.Message);
            }
        }

        private async Task<Tuple<HttpStatusCode, object?>> ReturnHttpResponse(HttpResponseMessage response, NotifyMeta notify, bool imageupdate, string error)
        {
            var responsecontent = await ReadResponse(response, notify.Destination);

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                GenerateLog(notify.Id, notify.Destination, notify.Type.ToLower() + ".push.trigger", "api", notify.UdateMode, imageupdate, responsecontent, null, true);                                
            }
            else
            {
                GenerateLog(notify.Id, notify.Destination, notify.Type.ToLower() + ".push.error", "api", notify.UdateMode, imageupdate, responsecontent, error, false);                
            }

            //var responseobj = JObject.Parse(responsestring);

            return Tuple.Create(response.StatusCode, responsecontent);
        }

        private async Task<object?> ReadResponse(HttpResponseMessage response, string service)
        {
            try
            {
                //TODO Switch between response service types

               return await response.Content.ReadFromJsonAsync<IdmMarketPlacePushResponse>();
            }
            catch
            {
                return await response.Content.ReadAsStringAsync();
            }
        }


        private void GenerateLog(string id, string destination, string message, string origin, string updatemode, bool? imageupdate, object? response, string? exception, bool success)
        {
            NotifyLog log = new NotifyLog() { id = id, destination = destination, message = message, origin = origin, imageupdate = imageupdate, updatemode = updatemode, response = JsonSerializer.Serialize(response), exception = exception, success = success };
            
            Console.WriteLine(JsonSerializer.Serialize(log));
        }

        private void GenerateLog(string id, string destination, string message, string origin, string updatemode, bool? imageupdate, string? response, string? exception, bool success)
        {
            NotifyLog log = new NotifyLog() { id = id, destination = destination, message = message, origin = origin, imageupdate = imageupdate, updatemode = updatemode, response = response, exception = exception, success = success };

            Console.WriteLine(JsonSerializer.Serialize(log));
        }

        private async Task WriteToFailureQueue(NotifyMeta notify, string exmessage)
        {
            NotifierFailureQueue myfailure = new NotifierFailureQueue();
            myfailure.Id = Guid.NewGuid().ToString();
            myfailure.ItemId = notify.Id;
            myfailure.Type = notify.Type;
            myfailure.Exception = exmessage;
            myfailure.LastChange = DateTime.Now;
            myfailure.Service = notify.Destination;
            myfailure.PushUrl = notify.Url;
            myfailure.Status = "open";
            myfailure.RetryCount = 1;
            myfailure.IsDeleteOperation = notify.IsDelete;
            myfailure.HasImageChanged = notify.HasImagechanged;

            await QueryFactory.Query("notificationfailures")
                       .UpdateAsync(new JsonBData() { id = myfailure.Id, data = new JsonRaw(myfailure) });
        }

        private async Task UpdateFailureQueue(NotifyMeta notify, string exmessage, NotifierFailureQueue myfailure)
        {            
            myfailure.ItemId = notify.Id;
            myfailure.Type = notify.Type;
            myfailure.Exception = exmessage;
            myfailure.LastChange = DateTime.Now;
            myfailure.Service = notify.Destination;
            myfailure.PushUrl = notify.Url;
            myfailure.Status = "open";
            myfailure.RetryCount = myfailure.RetryCount++;
            myfailure.IsDeleteOperation = notify.IsDelete;
            myfailure.HasImageChanged = notify.HasImagechanged;

            await QueryFactory.Query("notificationfailures")
                       .UpdateAsync(new JsonBData() { id = myfailure.Id, data = new JsonRaw(myfailure) });
        }

        //Valid Types for Push
        private static bool CheckValidTypes(NotifyMeta notify)
        {
            if (notify.ValidTypes.Select(x => x.ToLower()).Contains(notify.Type.ToLower()))
                return true;
            else
                return false;
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            //TODO            
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        private async Task<IEnumerable<NotifierFailureQueue>> GetFromFailureQueue(string service, string status)
        {
            var query = QueryFactory.Query("notificationfailures")
                .Where("gen_service", service)
                .Where("gen_status", status);

            var data = await query.GetObjectListAsync<NotifierFailureQueue>();

            return data;
        }

        public async Task<IDictionary<string, NotifierResponse>> PushFailureQueueToPublishedonService(List<string> publishedonlist, string? referer = null)
        {
            IDictionary<string, NotifierResponse> notifierresponselist = new Dictionary<string, NotifierResponse>();

            foreach (var notifyconfig in notifierconfiglist)
            {
                //GET All failed pushes            
                if (publishedonlist.Contains(notifyconfig.ServiceName.ToLower()))
                {
                    var failedpushes = await GetFromFailureQueue(notifyconfig.ServiceName.ToLower(), "open");

                    foreach(var failedpush in failedpushes)
                    {
                        NotifyMetaGenerated meta = new NotifyMetaGenerated(notifyconfig, failedpush.Id, failedpush.Type, failedpush.HasImageChanged != null ? failedpush.HasImageChanged.Value : false, false, "failurequeue.push", "api", referer);

                        NotifierResponse notifierresponse = new NotifierResponse();

                        var response = await SendNotify(meta);
                        notifierresponse.HttpStatusCode = response.Item1;
                        notifierresponse.Response = response.Item2;
                        notifierresponse.Service = notifyconfig.ServiceName;

                        notifierresponselist.TryAddOrUpdate(notifyconfig.ServiceName, notifierresponse);
                    }
                    
                    
                }
            }

            return notifierresponselist;
        }

    }

    public class NotifyMetaGenerated : NotifyMeta
    {
        public NotifyMetaGenerated(NotifierConfig notifyconfig, string id, string type, bool hasimagechanged, bool isdelete, string updatemode, string origin, string? referer = null)
        {
            //Set by parameters
            this.Id = id;
            this.Type = type;
            this.UdateMode = updatemode;
            this.Origin = origin;
            this.Referer = referer;
            this.HasImagechanged = hasimagechanged;
            this.IsDelete = isdelete;

            switch (notifyconfig.ServiceName.ToLower())
            {
                case "idm-marketplace":

                    //From Config
                    this.Url = notifyconfig.Url;
                    this.Headers = new Dictionary<string, string>() {
                        { "client_id", notifyconfig.User },
                        { "client_secret", notifyconfig.Password }
                    };

                    //Prefilled
                    this.Destination = "idm-marketplace";
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
                            "ODH_TAG",
                            "SKI_AREA"
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
                return type.ToLower() switch
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
