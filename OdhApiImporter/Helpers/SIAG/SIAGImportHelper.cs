using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SIAG;

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


            //Save parsed Response to measurement history table


            //weatherresult = await SIAG.GetWeatherData.ParseSiagWeatherDataToODHWeather(language, settings.XmlConfig.XmldirWeather, weatherresponsetask, extended);

            return null;
        }
    }
}
