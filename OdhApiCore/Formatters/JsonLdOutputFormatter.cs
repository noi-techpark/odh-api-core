using DataModel;
using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdhApiCore.Formatters
{
    public class JsonLdOutputFormatter : TextOutputFormatter
    {
        public JsonLdOutputFormatter() : base()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/ld+json"));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        private static Task BadRequest(OutputFormatterWriteContext context)
        {
            context.HttpContext.Response.StatusCode = 401;
            return context.HttpContext.Response.WriteAsync("Bad Request");
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            if (context.Object is JsonRaw jsonRaw)
            {
                var language = "en";
                var jToken = JToken.Parse(jsonRaw.Value);
                if (jToken is JObject jObject)
                {
                    var fieldsFromQueryString = new[] {
                        $"AccoDetail.{language}.Name",
                        $"AccoDetail.{language}.Shortdesc",
                        $"AccoDetail.{language}.Website"
                    };
                    // TODO: Extract Type form meta information: rewrite JsonTransformer logic, because it is already stripped away
                    // TODO: Add image with URL https://doc.lts.it/DocSite/ImageRender.aspx?ID={id}&W=800
                    var transformedJToken = JsonTransformerMethods.FilterByFields(jObject, fieldsFromQueryString, language);
                    if (transformedJToken != null)
                        await context.HttpContext.Response.WriteAsync(transformedJToken.ToString());
                }
                else
                {
                    await BadRequest(context);
                }
            }
            else
            {
                await BadRequest(context);
            }
        }
    }
}
