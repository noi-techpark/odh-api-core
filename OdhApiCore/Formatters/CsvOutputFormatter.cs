using CsvHelper;
using DataModel;
using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using OdhApiCore.Responses;
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

        //protected override bool CanWriteType(Type type)
        //{
        //    return base.CanWriteType(type);
        //}

        private static dynamic ConvertToExpandoObject(Dictionary<string, object> dict)
        {
            var eo = new ExpandoObject();
            var eoColl = (ICollection<KeyValuePair<string, object>>)eo!;
            foreach (var kvp in dict)
            {
                // Filter out IEnumerables, because they cannot be serialized to CSV
                if (kvp.Value is IEnumerable<object>)
                {
                    //Try to cast to string
                    //var stringlist = kvp.Value as Newtonsoft.Json.Linq.JArray;
                    var stringlist = kvp.Value as IEnumerable<object>;
                    if (stringlist != null)
                        eoColl.Add(
                            new KeyValuePair<string, object>(kvp.Key, String.Join("|", stringlist))
                        );
                    else
                        continue;
                }
                else
                {
                    eoColl.Add(kvp);
                }
            }
            return eo;
        }

        public override async Task WriteResponseBodyAsync(
            OutputFormatterWriteContext context,
            Encoding selectedEncoding
        )
        {
            //Works only with List Method add an understandable Exception if it is used on single methods
            //Add support if an object list is returned instead of JsonRAW issue 4762

            var result = context.Object as IResponse<JsonRaw>;
            if (result != null)
            {
                var data = (
                    from item in result.Items
                    let dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(item.Value)
                    select ConvertToExpandoObject(dict)
                ).ToList<dynamic>();

                await WriteCSVStream(context, data);
            }
            else if (context.Object != null)
            {
                var listresult = context.Object as IEnumerable<JsonRaw>;

                if (listresult != null)
                {
                    var data = (
                        from item in listresult
                        let dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(
                            item.Value
                        )
                        select ConvertToExpandoObject(dict)
                    ).ToList<dynamic>();

                    await WriteCSVStream(context, data);
                }
                else
                {
                    await OutputFormatterHelper.NotImplemented(context);
                }
            }
            else
            {
                await OutputFormatterHelper.BadRequest(context);
            }
        }

        private async Task WriteCSVStream(OutputFormatterWriteContext context, List<dynamic>? data)
        {
            var stream = context.HttpContext.Response.Body;

            await using var writer = new StreamWriter(
                stream,
                leaveOpen: true,
                encoding: Encoding.UTF8
            );
            await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            await csv.WriteRecordsAsync(data);
            await writer.FlushAsync();
        }
    }
}
