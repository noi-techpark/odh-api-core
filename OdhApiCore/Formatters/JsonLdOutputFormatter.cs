using DataModel;
using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
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

        private object? Transform(PathString path, JsonRaw jsonRaw)
        {
            if (path.StartsWithSegments("/v1/Accommodation"))
            {
                var acco = JsonConvert.DeserializeObject<Accommodation>(jsonRaw.Value);
                return JsonLDTransformer.TransformToLD.TransformAccommodationToLD(acco, "de");
            }
            else if (path.StartsWithSegments("/v1/Gastronomy"))
            {
                var gastro = JsonConvert.DeserializeObject<Gastronomy>(jsonRaw.Value);
                return JsonLDTransformer.TransformToLD.TransformGastronomyToLD(gastro, "de");
            }
            else if (path.StartsWithSegments("/v1/Event"))
            {
                var @event = JsonConvert.DeserializeObject<Event>(jsonRaw.Value);
                return JsonLDTransformer.TransformToLD.TransformEventToLD(@event, "de");
            }
            else
            {
                return null;
            }
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            if (context.Object is JsonRaw jsonRaw)
            {
                var transformed = Transform(context.HttpContext.Request.Path, jsonRaw);
                if (transformed != null)
                {
                    var jsonLD = JsonConvert.SerializeObject(transformed);
                    await context.HttpContext.Response.WriteAsync(jsonLD);
                }
                else
                {
                    await context.HttpContext.Response.WriteAsync("Not implemented");
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
