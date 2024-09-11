// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using SqlKata.Execution;
using System;
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

        private string? ConvertToRawdataObject(JsonRaw jsonRaw, QueryFactory QueryFactory)
        {
            try
            {
                if (jsonRaw != null)
                {
                    //Get Id of jsonRaw
                    dynamic? jsonvalue = JsonConvert.DeserializeObject(jsonRaw.Value);
                    string id = Convert.ToString(jsonvalue?.Id);
                    string odhtype = Convert.ToString(jsonvalue?._Meta.Type);

                    string table = ODHTypeHelper.TranslateTypeString2Table(odhtype);

                    //Load rawid
                    var rawid = QueryFactory.Query()
                           .Select("rawdataid")
                           .From(table)
                           .Where("id", id)
                           .Get<string>()
                           .FirstOrDefault();

                    var rawdata = QueryFactory.Query()
                        .Select("raw")
                        .From("rawdata")
                        .Where("id", rawid)
                        .Get<string>()
                        .FirstOrDefault();

                    return rawdata;
                }
                else
                    return null;
                
            }
            catch(Exception)
            {
                return null;
            }                       
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var queryFactory = (QueryFactory?)context.HttpContext.RequestServices.GetService(typeof(QueryFactory));

            if (context.Object is JsonRaw jsonRaw && queryFactory is { })
            {
                var transformed = ConvertToRawdataObject(jsonRaw, queryFactory);
                if (transformed != null)
                {
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
    }
}
