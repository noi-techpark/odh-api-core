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

	public class FCMPushNotification
	{
		public FCMPushNotification()
		{
			// TODO: Add constructor logic here
		}

		public bool Successful
		{
			get;
			set;
		}

		public string Response
		{
			get;
			set;
		}
		public Exception Error
		{
			get;
			set;
		}


		//Working
		public FCMPushNotification SendNotification(string _title, string _message, string _topic, string _fcm_device_id, string fcm_auth_key, string fcm_sender_id)
		{
			FCMPushNotification result = new FCMPushNotification();
			try
			{
				result.Successful = true;
				result.Error = null;
				// var value = message;
				var requestUri = "https://fcm.googleapis.com/fcm/send";

				WebRequest webRequest = WebRequest.Create(requestUri);
				webRequest.Method = "POST";
				webRequest.Headers.Add(string.Format("Authorization: key={0}", fcm_auth_key));
				webRequest.Headers.Add(string.Format("Sender: id={0}", fcm_sender_id));
				webRequest.ContentType = "application/json";

				string tostring = "";

				if (!String.IsNullOrEmpty(_fcm_device_id))
					tostring = _fcm_device_id;
				else
					tostring = "/topics/" + _topic;

				var data = new
				{
					// to = YOUR_FCM_DEVICE_ID, // Uncoment this if you want to test for single device
					//to = "/topics/" + _topic, // this is for topic 
					to = tostring,
					notification = new
					{
						title = _title,
						body = _message,
						//icon="myicon"
					}
				};
				//var serializer = new JavaScriptSerializer();
				//var json = serializer.Serialize(data);

				var json = JsonConvert.SerializeObject(data);

				Byte[] byteArray = Encoding.UTF8.GetBytes(json);

				webRequest.ContentLength = byteArray.Length;
				using (Stream dataStream = webRequest.GetRequestStream())
				{
					dataStream.Write(byteArray, 0, byteArray.Length);

					using (WebResponse webResponse = webRequest.GetResponse())
					{
						using (Stream dataStreamResponse = webResponse.GetResponseStream())
						{
							using (StreamReader tReader = new StreamReader(dataStreamResponse))
							{
								String sResponseFromServer = tReader.ReadToEnd();
								result.Response = sResponseFromServer;
							}
						}
					}
				}

			}
			catch (Exception ex)
			{
				result.Successful = false;
				result.Response = null;
				result.Error = ex;
			}
			return result;
		}
	}

	#endregion
}
