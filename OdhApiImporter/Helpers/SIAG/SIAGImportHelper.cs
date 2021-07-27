using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SIAG;
using DataModel;
using Newtonsoft.Json;

namespace OdhApiImporter.Helpers
{
    public class SIAGImportHelper
    {
        private readonly QueryFactory QueryFactory;
        private readonly ISettings settings;

        public SIAGImportHelper(ISettings settings, QueryFactory queryfactory)
        {
            this.QueryFactory = queryfactory;
            this.settings = settings;
        }

        public async Task<string> SaveWeatherToHistoryTable()
        {
            var weatherresponsetaskde = await SIAG.GetWeatherData.GetSiagWeatherData("de", settings.SiagConfig.Username, settings.SiagConfig.Password, true);
            var weatherresponsetaskit = await SIAG.GetWeatherData.GetSiagWeatherData("de", settings.SiagConfig.Username, settings.SiagConfig.Password, true);
            var weatherresponsetasken = await SIAG.GetWeatherData.GetSiagWeatherData("de", settings.SiagConfig.Username, settings.SiagConfig.Password, true);

            //Save all Responses to rawdata table

            var siagweatherde = JsonConvert.DeserializeObject<SIAG.WeatherModel.SiagWeather>(weatherresponsetaskde);
            var siagweatherit = JsonConvert.DeserializeObject<SIAG.WeatherModel.SiagWeather>(weatherresponsetaskit); 
            var siagweatheren = JsonConvert.DeserializeObject<SIAG.WeatherModel.SiagWeather>(weatherresponsetasken);

            RawDataStore rawData = new RawDataStore();
            rawData.importdate = DateTime.Now;
            rawData.type = "weather";
            rawData.sourceid = siagweatherde.id.ToString();
            //rawData.id = 0;
            rawData.datasource = "siag";
            rawData.sourceinterface = "http://daten.buergernetz.bz.it/services/weather/bulletin";
            rawData.raw = JsonConvert.SerializeObject(new { de = siagweatherde, it = siagweatherit, en = siagweatheren });
            

            var insertresult = await QueryFactory.Query("rawdata")
                  .InsertAsync(rawData);

            return insertresult.ToString();

            //TODO
            //Save parsed Response to measurement history table
            //weatherresult = await SIAG.GetWeatherData.ParseSiagWeatherDataToODHWeather(language, settings.XmlConfig.XmldirWeather, weatherresponsetask, extended);

        }
    }
}
