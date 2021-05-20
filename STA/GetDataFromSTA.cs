using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Globalization;
using System.IO;

namespace STA
{
    public class GetDataFromSTA
    {
        public const string csvurl = "c:\\temp\\210208.sudtirolmobil.Verkaufsstellen.csv";

        public static void ImportCSVFromSTA()
        {
            //CSVReader Config
            //var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            //{
            //    NewLine = Environment.NewLine,
            //};

            using (var reader = new StreamReader(csvurl))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<STAVendingPoint>();
            }
        }

    }
}
