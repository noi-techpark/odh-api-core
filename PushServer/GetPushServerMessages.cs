using DataModel;
using SIAG;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushServer
{
    public class GetPushServerMessages
    {
		#region Weather

		public static PushServerMessage? GetWeatherInfo(string siaguser, string siagpswd, string xmldir, string language, string group, TraceSource tracesource)
		{
			try
			{
				var weatherdata = GetWeatherForPushServer(language, "", "", xmldir, siaguser, siagpswd).Result;
				//var weatherdatait = GetWeatherForPushServer("it", "", "", xmldir, siaguser, siagpswd).Result;
				//var weatherdataen = GetWeatherForPushServer("en", "", "", xmldir, siaguser, siagpswd).Result;

				var weatherdatacondition = weatherdata.Conditions.Where(x => x.date.Date == DateTime.Now.Date).FirstOrDefault();
				//var weatherdataconditionit = weatherdatait.Conditions.Where(x => x.date.Date == DateTime.Now.Date).FirstOrDefault();
				//var weatherdataconditionen = weatherdataen.Conditions.Where(x => x.date.Date == DateTime.Now.Date).FirstOrDefault();

				if (weatherdatacondition is not { })
					return null;

				tracesource.TraceEvent(TraceEventType.Information, 0, "Weatherinfo GET success, language " + language);

				return ParseWeatherData(weatherdatacondition, language, group);
			}
			catch (Exception ex)
			{
				tracesource.TraceEvent(TraceEventType.Error, 0, "PushServer Get Weather Error, language: " + language + " Error: " + ex.Message);

				return null;
			}
		}

		private static async Task<Weather> GetWeatherForPushServer(string language, string locfilter, string stationidtype, string xmldir, string siaguser, string siagpswd)
		{
			var result = await GetWeatherData.GetSiagWeatherData(language, siaguser, siagpswd, true);

			return await GetWeatherData.ParseSiagWeatherDataToODHWeather(language, xmldir, result, true);
		}

		private static PushServerMessage ParseWeatherData(Conditions weather, string language, string group)
		{
			PushServerMessage weathermessage = new PushServerMessage();
			weathermessage.title = weather.Title;
			weathermessage.text = weather.Weatherdesc;
			weathermessage.image = weather.WeatherImgurl;
			weathermessage.video = "";

			PushServerDestination pushserverdestination = new PushServerDestination();
			pushserverdestination.group = group;
			pushserverdestination.language = language;

			weathermessage.destination = pushserverdestination;

			return weathermessage;
		}

		#endregion

		//#region Tip of the day

		//public static PushServerMessage GetTipOfTheDay(IDocumentStore documentStore, string language, string group, TraceSource tracesource)
		//{
		//	try
		//	{
		//		//Get Tip of The Day
		//		var mytip = GetTipofTheDayforPushServer(documentStore);

		//		//Parse Tip of the Day to 
		//		var parsedtip = ParseTipofTheDayData(mytip, language, group);

		//		tracesource.TraceEvent(TraceEventType.Information, 0, "Tip GET success, language " + language);

		//		return parsedtip;
		//	}
		//	catch (Exception ex)
		//	{
		//		tracesource.TraceEvent(TraceEventType.Error, 0, "PushServer Get Tip Error" + ex.Message);

		//		return null;
		//	}
		//}

		//private static SmgPoi GetTipofTheDayforPushServer(IDocumentStore documentStore)
		//{
		//	List<string> mylanglist = new List<string>() { "de", "it", "en" };

		//	//Typen
		//	List<string> typestoexclude = new List<string>() { "Essen Trinken", "Anderes" };
		//	//Subtypen
		//	List<string> subtypestoexclude = new List<string>() { "Radverleih", "Skischulen Skiverleih" };
		//	//PoiTypen
		//	List<string> poitypestoexclude = new List<string>();


		//	Helper.TipOfTheDayHelper.AssignTipsFilters(typestoexclude, subtypestoexclude, poitypestoexclude);

		//	using (var session = documentStore.OpenSession())
		//	{
		//		var mytip = session.Query<SmgPoiIndexed, SmgPoiMegaFilter>()
		//				.Where(x => x.Highlight == true)
		//				//.Where(x => x.Type != "Essen Trinken" && x.Type != "Winter")
		//				.Where(x => !x.Type.In(typestoexclude) && !x.SubType.In(subtypestoexclude) && !x.PoiType.In(poitypestoexclude)) //Des war nui schaugn obs geat
		//				.WhereIf(mylanglist.Count > 0, x => x.HasLanguage.In(mylanglist))
		//				.Where(x => x.Active == true)
		//				.Customize(x => x.RandomOrdering())
		//				.ProjectFromIndexFieldsInto<SmgPoi>()
		//				.FirstOrDefault();

		//		return mytip;
		//	}
		//}

		//private static PushServerMessage ParseTipofTheDayData(SmgPoi mytip, string language, string group)
		//{
		//	PushServerMessage weathermessage = new PushServerMessage();
		//	weathermessage.title = System.Net.WebUtility.HtmlDecode(mytip.Detail[language].Title);

		//	//Achtung hier muss ein sanitizer her
		//	weathermessage.text = System.Net.WebUtility.HtmlDecode(mytip.Detail[language].BaseText);

		//	weathermessage.image = mytip.ImageGallery.FirstOrDefault().ImageUrl;
		//	weathermessage.video = "";

		//	PushServerDestination pushserverdestination = new PushServerDestination();
		//	pushserverdestination.group = group;
		//	pushserverdestination.language = language;

		//	weathermessage.destination = pushserverdestination;

		//	return weathermessage;
		//}


		//#endregion
	}
}
