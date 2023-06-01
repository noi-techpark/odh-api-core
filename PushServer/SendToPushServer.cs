// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Newtonsoft.Json;
using SIAG;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushServer
{
	public class SendToPushServer
	{
		public static async Task<string> SendMessageToPushServer(string serviceurl, PushServerMessage data, string pushserveruser, string pushserverpass, string language)
		{
			try
			{
				if (data == null)
					throw new Exception("data is null");

				//HttpClient myclient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
				//myclient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate");
				//HttpClient myclient = new HttpClient(new Cred { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
				HttpClient myclient = new HttpClient();

				var byteArray = Encoding.ASCII.GetBytes(pushserveruser + ":" + pushserverpass);
				myclient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

				//myclient.DefaultRequestHeaders.Add("","");

				//var json = JsonConvert.SerializeObject(data, Formatting.Indented);

				var myresponse = await myclient.PostAsync(serviceurl, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));

				var myresponseparsed = await myresponse.Content.ReadAsStringAsync();

				//tracesource.TraceEvent(TraceEventType.Information, 0, "Pushed Message:" + json);
				//tracesource.TraceEvent(TraceEventType.Information, 0, "PushServer Send Weather success language , info: " + myresponseparsed + " language " + language);

				return myresponseparsed;
			}
			catch (Exception ex)
			{
				//tracesource.TraceEvent(TraceEventType.Error, 0, "PushServer Send Weather FAILED, error: " + ex.Message + " language " + language);

				return ex.Message;
			}
		}

		public static async Task<string> SendCustomMessageToPushServer(string serviceurl, PushServerCustomMessage data, string pushserveruser, string pushserverpass)
		{
			try
			{
				if (data == null)
					throw new Exception("data is null");

				//HttpClient myclient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
				//myclient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate");
				//HttpClient myclient = new HttpClient(new Cred { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
				HttpClient myclient = new HttpClient();

				var byteArray = Encoding.ASCII.GetBytes(pushserveruser + ":" + pushserverpass);
				myclient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

				//myclient.DefaultRequestHeaders.Add("","");

				var json = JsonConvert.SerializeObject(data, Formatting.Indented);

				var myresponse = await myclient.PostAsync(serviceurl, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));

				var myresponseparsed = await myresponse.Content.ReadAsStringAsync();

				//tracesource.TraceEvent(TraceEventType.Information, 0, "PushServer Send Weather success, info: " + myresponseparsed);

				return myresponseparsed;
			}
			catch (Exception ex)
			{
				//tracesource.TraceEvent(TraceEventType.Error, 0, "PushServer Send Weather FAILED, error: " + ex.Message);

				return ex.Message;
			}
		}		

	}

}
