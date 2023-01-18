using OdhNotifier;
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
                        if(notify.Headers != null)
                        {
                            foreach (var header in notify.Headers)
                            {
                                client.DefaultRequestHeaders.Add(header.Key, header.Value);
                            }
                        }                        

                        //Add Referer Header
                        if(!String.IsNullOrEmpty(notify.Referer))
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
                        if(notify.Mode.ToLower() == "get")
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
                            GenerateLog(notify.Id, notify.Destination, notify.Type + ".push.trigger", "api", notify.UdateMode, imageupdate, null);

                            return response;
                        }
                        else if (response != null)
                        {
                            throw new Exception("http status error code: " + response.StatusCode);
                        }
                        else
                        {
                            throw new Exception("no response");
                        }
                    }
                }
                else
                    throw new Exception("type not valid!");
            }
            catch (Exception ex)
            {
                await WriteToFailureQueue(notify.Id, notify.Type, notify.Url, notify.Destination, ex.Message);

                GenerateLog(notify.Id, notify.Destination, notify.Type + ".push.error", "api", notify.UdateMode, imageupdate, ex.Message);

                var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.Message);

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
            if (notify.ValidTypes.Contains(notify.Type.ToLower()))
                return true;
            else
                return false;
        }
    }  
    
    public class NotifyMeta
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
        public string Mode { get; set; }

        public string Destination { get; set; }

        public string UdateMode { get; set; }
        public string Origin { get; set; }

        public string Referer { get; set; }

        public IDictionary<string,string> Headers { get; set; }

        public IDictionary<string, string> Parameters { get; set; }

        public List<string> ValidTypes { get; set; }
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
}
