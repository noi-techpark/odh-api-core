using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STA
{
    public class GetDataFromSTA
    {
        public const string csvurl = "c:\\temp\\210208.sudtirolmobil.Verkaufsstellen.csv";

        public static Task<ParseResult<STAVendingPoint>> ImportCSVFromSTA(string? csvcontent)
        {
            try
            {
                //FORM DOCS
                //https://joshclose.github.io/CsvHelper/getting-started/

                //CSVReader Config
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    //NewLine = "\r\n" Environment.NewLine,
                    //MissingFieldFound = null  //Hack for server?
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

                        ParseResult<STAVendingPoint> myresult =
                            new STA.ParseResult<STAVendingPoint>();
                        myresult.Success = true;
                        myresult.Error = false;
                        myresult.records = records.ToList();

                        return Task.FromResult(myresult);
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

                        ParseResult<STAVendingPoint> myresult =
                            new STA.ParseResult<STAVendingPoint>();
                        myresult.Success = true;
                        myresult.Error = false;
                        myresult.records = records.ToList();

                        return Task.FromResult(myresult);
                    }
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(
                    new ParseResult<STAVendingPoint>()
                    {
                        Error = true,
                        Success = false,
                        ErrorMessage = ex.Message,
                        records = Enumerable.Empty<STAVendingPoint>()
                    }
                );
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

        public static MemoryStream GenerateMemoryStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }
    }

    public class ParseResult<T>
    {
        public bool Success { get; set; }
        public bool Error { get; set; }
        public string ErrorMessage { get; set; } = "";

        public IEnumerable<T> records { get; set; } = Enumerable.Empty<T>();
    }
}
