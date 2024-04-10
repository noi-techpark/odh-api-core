// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

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
	

	public class FCMPushNotification
	{			
		public static async Task<PushResult> SendNotification(FCMModels fcmmessage, string fcmurl, string fcmsenderid, string fcmauthkey)
		{
			FCMPushNotification result = new FCMPushNotification();
			try
			{				
				HttpClient myclient = new HttpClient();
				
				myclient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + fcmauthkey);
				myclient.DefaultRequestHeaders.TryAddWithoutValidation("Sender", string.Format("id={0}", fcmsenderid));
			
				var myresponse = await myclient.PostAsync(fcmurl, new StringContent(JsonConvert.SerializeObject(fcmmessage), Encoding.UTF8, "application/json"));

				var myresponseparsed = await myresponse.Content.ReadAsStringAsync();

				return new PushResult() { Error = null, Response = myresponseparsed, Success = true, Messages = 1 };
			}
			catch (Exception ex)
			{
				return new PushResult() { Error = ex.Message, Response = "Error", Success = false, Messages = 1 };
			}			
		}

		
	}

	#endregion
}
