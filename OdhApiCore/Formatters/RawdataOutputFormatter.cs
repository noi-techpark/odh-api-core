using CsvHelper;
using DataModel;
using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using OdhApiCore.Responses;
using SqlKata.Execution;
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
    public class RawdataOutputFormatter : TextOutputFormatter
    {
        public RawdataOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/rawdata"));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        //public RawdataOutputFormatter(QueryFactory queryFactory)
        //{
            

        //    this.QueryFactory = queryFactory;
        //}

        //protected QueryFactory QueryFactory { get; }

        private string ConvertToRawdataObject(JsonRaw jsonRaw, QueryFactory QueryFactory)
        {
            //Get Id of jsonRaw
            //Load rawid

            var rawid = QueryFactory.Query()
                        .Select("rawdataid")
                        .From("accommodations")
                        .Where("id", "5CEA544EE34639034F07B79D4AEEB603")
                        .Get<string>();

            return "<xmltest>test</xmltest>";
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var queryFactory = (QueryFactory)context.HttpContext.RequestServices.GetService(typeof(QueryFactory));

            if (context.Object is JsonRaw jsonRaw)
            {
                var transformed = ConvertToRawdataObject(jsonRaw, queryFactory);
                if (transformed != null)
                {
                    //var jsonLD = JsonConvert.SerializeObject(transformed);
                    await context.HttpContext.Response.WriteAsync(transformed);
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

        private void GetRawDatafromDB(string rawdataid)
        {

            //QueryFactory.Query()
            //            .SelectRaw("data")
            //            .From("webcams")
        }
    }
}
