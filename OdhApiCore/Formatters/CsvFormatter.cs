using CsvHelper;
using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdhApiCore.Formatters
{
    public class CsvOutputFormatter : TextOutputFormatter
    {
        public CsvOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanWriteType(Type type)
        {
            return base.CanWriteType(type);
        }

        public class Foo
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var result = context.Object as JsonResult<JsonRaw>;
            if (result != null)
            {
                static ExpandoObject ConvertToExpandoObject(Dictionary<string, object> dict)
                {
                    var eo = new ExpandoObject();
                    var eoColl = (ICollection<KeyValuePair<string, object>>)eo;
                    foreach (var kvp in dict)
                    {
                        // Filter out IEnumerables, because they cannot be serialized to CSV
                        if (kvp.Value is IEnumerable<object>)
                            continue;
                        eoColl.Add(kvp);
                    }
                    dynamic eoDynamic = eo;
                    return eoDynamic;
                }

                var data =
                    (from item in result.Items
                     let dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(item.Value)
                     select ConvertToExpandoObject(dict)).ToList<dynamic>();

                var stream = context.HttpContext.Response.Body;

                using var writer = new StreamWriter(stream);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                await csv.WriteRecordsAsync(data);
            }
            else
            {
                context.HttpContext.Response.StatusCode = 401;
                await context.HttpContext.Response.WriteAsync("Bad Request");
            }
        }
    }
}
