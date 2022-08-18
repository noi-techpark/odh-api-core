using DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PushServer
{
	#region Direct Send FCM to Firebase

    public class FCMPushNotificationResponse
    {
        public bool Success { get; set; }
        public string? Response { get; set; }
        public Exception? Error { get; set; }
    }

    public class FCMPushNotification
    {
        public static async Task<FCMPushNotificationResponse> SendNotification(
            FCMModels fcmmessage,
            string fcmurl,
            string fcmsenderid,
            string fcmauthkey
        )
        {
            FCMPushNotification result = new FCMPushNotification();
            try
            {
                HttpClient myclient = new HttpClient();

                myclient.DefaultRequestHeaders.TryAddWithoutValidation(
                    "Authorization",
                    "key=" + fcmauthkey
                );
                myclient.DefaultRequestHeaders.TryAddWithoutValidation(
                    "Sender",
                    string.Format("id={0}", fcmsenderid)
                );

                var myresponse = await myclient.PostAsync(
                    fcmurl,
                    new StringContent(
                        JsonConvert.SerializeObject(fcmmessage),
                        Encoding.UTF8,
                        "application/json"
                    )
                );

                var myresponseparsed = await myresponse.Content.ReadAsStringAsync();

                return new FCMPushNotificationResponse()
                {
                    Error = null,
                    Response = myresponseparsed,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new FCMPushNotificationResponse()
                {
                    Error = ex,
                    Response = "Error",
                    Success = false
                };
            }
        }
    }

	#endregion
}
