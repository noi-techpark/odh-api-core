using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace STA
{
    public class GetDataFromSTA
    {
        public const string csvurl = "c:\\temp\\210208.sudtirolmobil.Verkaufsstellen.csv";

        public static async Task<IEnumerable<STAVendingPoint>> ImportCSVFromSTA()
        {
            //FORM DOCS
            //https://joshclose.github.io/CsvHelper/getting-started/

            //CSVReader Config
            //var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            //{
            //    NewLine = Environment.NewLine,
            //};
            var records = default(IEnumerable<STAVendingPoint>);

            using (var reader = new StreamReader(csvurl))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = csv.GetRecords<STAVendingPoint>();
            }

            return records;
        }

    }
}
