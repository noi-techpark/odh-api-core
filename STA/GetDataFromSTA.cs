using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace STA
{
    public class GetDataFromSTA
    {
        public const string csvurl = "c:\\temp\\210208.sudtirolmobil.Verkaufsstellen.csv";

        public static async Task<ParseResult<STAVendingPoint>> ImportCSVFromSTA(string? csvcontent)
        {
            try
            {
                //FORM DOCS
                //https://joshclose.github.io/CsvHelper/getting-started/

                //CSVReader Config
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    NewLine = Environment.NewLine,
                };
                var records = default(IEnumerable<STAVendingPoint>);

                //Import from File
                if (csvcontent == null)
                {
                    using (var reader = new StreamReader(csvurl))
                    using (var csv = new CsvReader(reader, config))
                    {
                        //csv.Configuration.Delimiter = ";";
                        csv.Read();
                        csv.ReadHeader();
                        records = csv.GetRecords<STAVendingPoint>();

                        ParseResult<STAVendingPoint> myresult = new STA.ParseResult<STAVendingPoint>();
                        myresult.Success = true;
                        myresult.Error = false;
                        myresult.records = records.ToList();

                        return myresult;
                    }
                }
                else
                {
                    using (var reader = new StreamReader(GenerateStreamFromString(csvcontent)))
                    using (var csv = new CsvReader(reader, config))
                    {
                        //csv.Configuration.Delimiter = ";";
                        csv.Read();
                        csv.ReadHeader();
                        records = csv.GetRecords<STAVendingPoint>();

                        ParseResult<STAVendingPoint> myresult = new STA.ParseResult<STAVendingPoint>();
                        myresult.Success = true;
                        myresult.Error = false;
                        myresult.records = records.ToList();

                        return myresult;
                    }
                }
            }
            catch(Exception ex)
            {
                return new ParseResult<STAVendingPoint>() { Error = true, Success = false, ErrorMessage = ex.Message, records = null };
            }
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

    }

    public class ParseResult<T>
    {
        public bool Success { get; set; }
        public bool Error { get; set; }
        public string ErrorMessage { get; set; }

        public IEnumerable<T> records { get; set; }
    }
}
