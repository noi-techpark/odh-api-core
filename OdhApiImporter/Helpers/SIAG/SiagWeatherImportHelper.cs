// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SIAG;
using DataModel;
using Newtonsoft.Json;
using System.Threading;
using System.Xml.Linq;
using Helper;
using SIAG.Model;
using System.IO;

namespace OdhApiImporter.Helpers
{
    public class SiagWeatherImportHelper : ImportHelper, IImportHelper
    {
        public SiagWeatherImportHelper(ISettings settings, QueryFactory queryfactory, string table, string importerURL) : base(settings, queryfactory, table, importerURL)
        {

        }

        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, List<string>? idlist = null, CancellationToken cancellationToken = default)
        {
            var updateresult = new UpdateDetail();

            if (idlist != null)
            {
                var updateresulttemp = new UpdateDetail();

                foreach (var id in idlist)
                {
                    updateresulttemp = await SaveWeatherToHistoryTable(cancellationToken, id);

                    updateresult = GenericResultsHelper.MergeUpdateDetail(new List<UpdateDetail>() { updateresult, updateresulttemp });
                }
            }
            else
                updateresult = await SaveWeatherToHistoryTable(cancellationToken, null);

            return updateresult;
        }

        public async Task<UpdateDetail> SaveWeatherToHistoryTable(CancellationToken cancellationToken, string? id = null)
        {
            string? weatherresponsetaskde = "";
            string? weatherresponsetaskit = "";
            string? weatherresponsetasken = "";
            string source = "opendata";

            IEnumerable<WeatherDistrictLinked> weatherdistrictde = default(IEnumerable<WeatherDistrictLinked>);
            IEnumerable<WeatherDistrictLinked> weatherdistrictit = default(IEnumerable<WeatherDistrictLinked>);
            IEnumerable<WeatherDistrictLinked> weatherdistricten = default(IEnumerable<WeatherDistrictLinked>);

            IEnumerable<WeatherForecastLinked> weatherforecastde = default(IEnumerable<WeatherForecastLinked>);
            IEnumerable<WeatherForecastLinked> weatherforecastit = default(IEnumerable<WeatherForecastLinked>);
            IEnumerable<WeatherForecastLinked> weatherforecasten = default(IEnumerable<WeatherForecastLinked>);

            if (!String.IsNullOrEmpty(id))
            {
                weatherresponsetaskde = await SIAG.GetWeatherData.GetSiagWeatherData("de", settings.SiagConfig.Username, settings.SiagConfig.Password, true, source, id);
                weatherresponsetaskit = await SIAG.GetWeatherData.GetSiagWeatherData("it", settings.SiagConfig.Username, settings.SiagConfig.Password, true, source, id);
                weatherresponsetasken = await SIAG.GetWeatherData.GetSiagWeatherData("en", settings.SiagConfig.Username, settings.SiagConfig.Password, true, source, id);
            }
            else
            {
                weatherresponsetaskde = await SIAG.GetWeatherData.GetSiagWeatherData("de", settings.SiagConfig.Username, settings.SiagConfig.Password, true, source);
                weatherresponsetaskit = await SIAG.GetWeatherData.GetSiagWeatherData("it", settings.SiagConfig.Username, settings.SiagConfig.Password, true, source);
                weatherresponsetasken = await SIAG.GetWeatherData.GetSiagWeatherData("en", settings.SiagConfig.Username, settings.SiagConfig.Password, true, source);

                //if id is empty retrieve also DistrictWeather and WeatherForecast
                weatherdistrictde = await GetWeatherData.GetCurrentBezirkWeatherAsync("de", "1,2,3,4,5,6,7,8", null, null, null, settings.SiagConfig.Username, settings.SiagConfig.Password, true, source);
                weatherdistrictit = await GetWeatherData.GetCurrentBezirkWeatherAsync("it", "1,2,3,4,5,6,7,8", null, null, null, settings.SiagConfig.Username, settings.SiagConfig.Password, true, source);
                weatherdistricten = await GetWeatherData.GetCurrentBezirkWeatherAsync("en", "1,2,3,4,5,6,7,8", null, null, null, settings.SiagConfig.Username, settings.SiagConfig.Password, true, source);

                //WeatherForecast
                weatherforecastde = await GetWeatherData.GetWeatherForeCastAsync("de", null, await GetWeatherForecastFromS3());
                weatherforecastit = await GetWeatherData.GetWeatherForeCastAsync("it", null, await GetWeatherForecastFromS3());
                weatherforecasten = await GetWeatherData.GetWeatherForeCastAsync("en", null, await GetWeatherForecastFromS3());

            }

            if (!String.IsNullOrEmpty(weatherresponsetaskde) && !String.IsNullOrEmpty(weatherresponsetaskit) && !String.IsNullOrEmpty(weatherresponsetasken))
            {
                //Save all Responses to rawdata table

                var siagweatherde = JsonConvert.DeserializeObject<SIAG.WeatherModel.SiagWeather>(weatherresponsetaskde);
                var siagweatherit = JsonConvert.DeserializeObject<SIAG.WeatherModel.SiagWeather>(weatherresponsetaskit);
                var siagweatheren = JsonConvert.DeserializeObject<SIAG.WeatherModel.SiagWeather>(weatherresponsetasken);

                RawDataStore rawData = new RawDataStore();
                rawData.importdate = DateTime.Now;
                rawData.type = "weather";
                rawData.sourceid = siagweatherde?.id.ToString() ?? "";
                rawData.datasource = "siag";
                rawData.sourceinterface = "weatherbulletin";
                rawData.sourceurl = "http://daten.buergernetz.bz.it/services/weather/bulletin";
                rawData.raw = JsonConvert.SerializeObject(new { de = siagweatherde, it = siagweatherit, en = siagweatheren });
                rawData.license = "open";
                rawData.rawformat = "json";

                var insertresultraw = await QueryFactory.Query("rawdata")
                      .InsertGetIdAsync<int>(rawData);

                //Save parsed Response to measurement history table
                var odhweatherresultde = await SIAG.GetWeatherData.ParseSiagWeatherDataToODHWeather("de", settings.XmlConfig.XmldirWeather, weatherresponsetaskde, true, source);
                var odhweatherresultit = await SIAG.GetWeatherData.ParseSiagWeatherDataToODHWeather("it", settings.XmlConfig.XmldirWeather, weatherresponsetaskit, true, source);
                var odhweatherresulten = await SIAG.GetWeatherData.ParseSiagWeatherDataToODHWeather("en", settings.XmlConfig.XmldirWeather, weatherresponsetasken, true, source);

                //Insert into Measuringhistorytable
                //var insertresultde = await QueryFactory.Query("weatherdatahistory")
                //      .InsertAsync(new JsonBDataRaw { id = odhweatherresultde.Id + "_de", data = new JsonRaw(odhweatherresultde), raw = weatherresponsetaskde });
                //var insertresultit = await QueryFactory.Query("weatherdatahistory")
                //      .InsertAsync(new JsonBDataRaw { id = odhweatherresultde.Id + "_it", data = new JsonRaw(odhweatherresultit), raw = weatherresponsetaskit });
                //var insertresulten = await QueryFactory.Query("weatherdatahistory")
                //      .InsertAsync(new JsonBDataRaw { id = odhweatherresultde.Id + "_en", data = new JsonRaw(odhweatherresulten), raw = weatherresponsetasken });

                var myweatherhistory = new WeatherHistoryLinked();

                myweatherhistory.Id = odhweatherresultde.Id.ToString();
                myweatherhistory.Weather.Add("de", odhweatherresultde);
                myweatherhistory.Weather.Add("it", odhweatherresultit);
                myweatherhistory.Weather.Add("en", odhweatherresulten);
                myweatherhistory.LicenseInfo = Helper.LicenseHelper.GetLicenseforWeather(source);
                myweatherhistory.FirstImport = DateTime.Now;
                myweatherhistory.HasLanguage = new List<string>() { "de", "it", "en" };
                myweatherhistory.LastChange = odhweatherresultde.date;
                myweatherhistory.Shortname = odhweatherresultde.evolutiontitle;


                var insertresult = await QueryFactory.UpsertData<WeatherHistoryLinked>(myweatherhistory, "weatherdatahistory", insertresultraw, "siag.weather.import", importerURL, true);

                //var insertresult = await QueryFactory.Query("weatherdatahistory")
                //      .InsertAsync(new JsonBDataRaw { id = odhweatherresultde.Id.ToString(), data = new JsonRaw(myweatherhistory), rawdataid = insertresultraw });

                ////Save to PG
                ////Check if data exists                    
                //var result = await QueryFactory.UpsertData<ODHActivityPoi>(odhactivitypoi!, "weatherdatahistory", insertresultraw);

                return new UpdateDetail() { created = insertresult.created, updated = insertresult.updated, deleted = insertresult.deleted, error = insertresult.error };                    
            }
            else
                throw new Exception("No weatherdata received from source!");
        }

        private async Task<SiagWeatherForecastModel> GetWeatherForecastFromS3()
        {
            if (!settings.S3Config.ContainsKey("dc-meteorology-province-forecast"))
                throw new Exception("No weatherforecast file found");

           
            using (StreamReader r = new StreamReader(settings.JsonConfig.Jsondir + settings.S3Config["dc-meteorology-province-forecast"].Filename))
            {
                string json = r.ReadToEnd();

                if (json != null)
                    return JsonConvert.DeserializeObject<SiagWeatherForecastModel>(json);
                else
                    throw new Exception("Unable to parse file");
            }
        }


    }
}
