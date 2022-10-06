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

namespace OdhApiImporter.Helpers
{
    public class SiagWeatherImportHelper : ImportHelper, IImportHelper
    {
        public SiagWeatherImportHelper(ISettings settings, QueryFactory queryfactory, string table) : base(settings, queryfactory, table)
        {

        }

        public Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Save Weather to History Table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<UpdateDetail> SaveWeatherToHistoryTable(CancellationToken cancellationToken, string? id = null)
        {
            string? weatherresponsetaskde = "";
            string? weatherresponsetaskit = "";
            string? weatherresponsetasken = "";

            if (!String.IsNullOrEmpty(id))
            {
                weatherresponsetaskde = await SIAG.GetWeatherData.GetSiagWeatherData("de", settings.SiagConfig.Username, settings.SiagConfig.Password, true, id);
                weatherresponsetaskit = await SIAG.GetWeatherData.GetSiagWeatherData("it", settings.SiagConfig.Username, settings.SiagConfig.Password, true, id);
                weatherresponsetasken = await SIAG.GetWeatherData.GetSiagWeatherData("en", settings.SiagConfig.Username, settings.SiagConfig.Password, true, id);
            }
            else
            {
                weatherresponsetaskde = await SIAG.GetWeatherData.GetSiagWeatherData("de", settings.SiagConfig.Username, settings.SiagConfig.Password, true);
                weatherresponsetaskit = await SIAG.GetWeatherData.GetSiagWeatherData("it", settings.SiagConfig.Username, settings.SiagConfig.Password, true);
                weatherresponsetasken = await SIAG.GetWeatherData.GetSiagWeatherData("en", settings.SiagConfig.Username, settings.SiagConfig.Password, true);
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
                var odhweatherresultde = await SIAG.GetWeatherData.ParseSiagWeatherDataToODHWeather("de", settings.XmlConfig.XmldirWeather, weatherresponsetaskde, true);
                var odhweatherresultit = await SIAG.GetWeatherData.ParseSiagWeatherDataToODHWeather("it", settings.XmlConfig.XmldirWeather, weatherresponsetaskit, true);
                var odhweatherresulten = await SIAG.GetWeatherData.ParseSiagWeatherDataToODHWeather("en", settings.XmlConfig.XmldirWeather, weatherresponsetasken, true);

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
                myweatherhistory.LicenseInfo = Helper.LicenseHelper.GetLicenseforWeather();
                myweatherhistory.FirstImport = DateTime.Now;
                myweatherhistory.HasLanguage = new List<string>() { "de", "it", "en" };
                myweatherhistory.LastChange = odhweatherresultde.date;
                myweatherhistory.Shortname = odhweatherresultde.evolutiontitle;


                var insertresult = await QueryFactory.UpsertData<WeatherHistoryLinked>(myweatherhistory, "weatherdatahistory", insertresultraw, true);

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
        
    }
}
