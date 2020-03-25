using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            Console.WriteLine("CAN I WRITE? {0}", type);
            return base.CanWriteType(type);
        }

        //public override bool CanWriteResult(OutputFormatterCanWriteContext context)
        //{
        //    Console.WriteLine("CAN I WRITE RESULT? {0}", context.Object.GetType());
        //    return base.CanWriteResult(context);
        //}

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            await context.HttpContext.Response.WriteAsync("hello!");
        }
    }
}
