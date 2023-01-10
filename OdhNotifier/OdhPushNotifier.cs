using OdhNotifier;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OdhNotifier
{
    public class OdhPushNotifier
    {        
        public static void SendNotify(NotifyMeta notify, string notifymode, string notifydestination)
        {

        }

        private static void NotifyAsPost(NotifyMeta notify)
        {

        }

        private static void NotifyAsGet(NotifyMeta notify)
        {

        }        
    }  

    public class NotifyMeta
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
    }

    public class NotifyLog
    {
        public string message { get; set; }
        public string id { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public bool imageupdate { get; set; }
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

public class SinfoUpdateController
{
    private const string serviceurl = @"";

    public async Task<HttpResponseMessage> Get(string type, string id)
    {
        var requesturl = serviceurl + type.ToLower() + "/" + id;

        try
        {
            if (CheckValidTypes(type))
            {
                NotifyLog log = new NotifyLog() { id = id, destination = "sinfo", message = type.ToLower() + ".push.trigger", origin = "api" };
                Console.WriteLine(JsonSerializer.Serialize(log));

                using (var client = new HttpClient())
                {
                    var myresponse = await client.GetAsync(requesturl);

                    //tracesource.TraceEvent(TraceEventType.Information, 0, "Pimcore Service response:" + myresponse.);


                    return myresponse;
                }
            }
            else
                throw new Exception("type not valid!");
        }
        catch (Exception ex)
        {
            await WriteToFailureQueue(id, type, requesturl, "sinfo", ex.Message);

            NotifyLog log = new NotifyLog() { id = id, destination = "sinfo", message = type.ToLower() + ".push.error", origin = "api" };
            Console.WriteLine(JsonSerializer.Serialize(log));

            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            response.Content = new StringContent(ex.Message);

            return response;
        }
    }

    public static async Task CallSinfoService(string type, string id, string updatemode)
    {
        var requesturl = serviceurl + type.ToLower() + "/" + id;

        try
        {
            if (CheckValidTypes(type))
            {
                //tracesource.TraceEvent(TraceEventType.Information, 0, "Calling Pimcore update service on:" + requesturl);

                using (var client = new HttpClient())
                {
                    var myresponse = await client.GetAsync(requesturl);
                    //tracesource.TraceEvent(TraceEventType.Information, 0, "Pimcore Service response:" + myresponse.);

                    if (myresponse.StatusCode == HttpStatusCode.OK)
                    {
                        NotifyLog log = new NotifyLog() { id = id, destination = "sinfo", message = type.ToLower() + ".push.trigger", origin = "api", imageupdate = true, updatemode = updatemode };
                        Console.WriteLine(JsonSerializer.Serialize(log));
                    }
                    else
                    {
                        throw new Exception("http status error code: " + myresponse.StatusCode);
                    }
                }
            }
            else
                throw new Exception("type not valid!");
        }
        catch (Exception ex)
        {
            await WriteToFailureQueue(id, type, requesturl, "sinfo", ex.Message);

            NotifyLog log = new NotifyLog() { id = id, destination = "sinfo", message = type.ToLower() + ".push.error", origin = "api" };
            Console.WriteLine(JsonSerializer.Serialize(log));            
        }
    }

    public static async Task CallSinfoService(string type, string id, bool imagechanged, string updatemode)
    {        
        var requesturl = serviceurl + type.ToLower() + "/" + id;

        if (!imagechanged)
            requesturl = requesturl + "?skipimage=true";

        try
        {
            if (CheckValidTypes(type))
            {
                //tracesource.TraceEvent(TraceEventType.Information, 0, "Calling Pimcore update service on:" + requesturl);

                using (var client = new HttpClient())
                {
                    var myresponse = await client.GetAsync(requesturl);
                    //tracesource.TraceEvent(TraceEventType.Information, 0, "Pimcore Service response:" + myresponse.);

                    if (myresponse.StatusCode == HttpStatusCode.OK)
                    {
                        NotifyLog log = new NotifyLog() { id = id, destination = "sinfo", message = type.ToLower() + ".push.trigger", origin = "api", imageupdate = imagechanged, updatemode = updatemode };
                        Console.WriteLine(JsonSerializer.Serialize(log));                        
                    }
                    else
                    {
                        throw new Exception("http status error code: " + myresponse.StatusCode);
                    }
                }
            }
            else
                throw new Exception("type not valid!");
        }
        catch (Exception ex)
        {
            await WriteToFailureQueue(id, type, requesturl, "sinfo", ex.Message);

            NotifyLog log = new NotifyLog() { id = id, destination = "sinfo", message = type.ToLower() + ".push.error", origin = "api" };
            Console.WriteLine(JsonSerializer.Serialize(log));
        }
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

    private static bool CheckValidTypes(string type)
    {
        List<string> validtypelist = new List<string>()
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

        if (validtypelist.Contains(type.ToLower()))
            return true;
        else
            return false;
    }
}

public class ODHCoreUpdateController
{
    static string serviceurl = "";

    public static async Task CallODHCoreUpdateService(string type, string id, string updatemode = "default")
    {        
        var requesturl = serviceurl + type.ToLower() + "/Update/" + id;

        try
        {
            if (CheckValidTypesForODHCore(type))
            {
                //tracesource.TraceEvent(TraceEventType.Information, 0, "Calling Pimcore update service on:" + requesturl);

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Referrer = new Uri("https://service.suedtirol.info");

                    var myresponse = await client.GetAsync(requesturl);
                    //tracesource.TraceEvent(TraceEventType.Information, 0, "Pimcore Service response:" + myresponse.);

                    if (myresponse.StatusCode == HttpStatusCode.OK)
                    {
                        NotifyLog log = new NotifyLog() { id = id, destination = "odh", message = type.ToLower() + ".push.trigger", origin = "api", imageupdate = true, updatemode = updatemode };
                        Console.WriteLine(JsonSerializer.Serialize(log));
                    }
                    else
                    {
                        throw new Exception("http status error code: " + myresponse.StatusCode);
                    }
                }
            }
            else
                throw new Exception("type not valid!");
        }
        catch (Exception ex)
        {
            await WriteToFailureQueueODHCore(id, type, requesturl, "odh", ex.Message);

            NotifyLog log = new NotifyLog() { id = id, destination = "odh", message = type.ToLower() + ".push.error", origin = "api" };
            Console.WriteLine(JsonSerializer.Serialize(log));            
        }
    }

    public static async Task WriteToFailureQueueODHCore(string id, string type, string url, string service, string exmessage)
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

    private static bool CheckValidTypesForODHCore(string type)
    {
        List<string> validtypelist = new List<string>()
            {
                "accommodation",
                "gastronomy",
                "activity",
                "poi",
                "odhactivitypoi",
                "event",
                "webcam",
                "metaregion",
                "region",
                "tv",
                "municipality",
                "district",
                "experiencearea",
                "skiarea",
                "skiregion",
                "odhtag",
                "article",
                "measuringpoint",
                "venue",
                "wine"
            };

        if (validtypelist.Contains(type.ToLower()))
            return true;
        else
            return false;
    }
}
