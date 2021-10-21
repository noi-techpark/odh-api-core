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
            //TODO extract language
            //TODO new JsonSerializerSettings {  NullValueHandling = NullValueHandling.Ignore  })

            if (path.StartsWithSegments("/v1/Accommodation"))
            {
                var acco = JsonConvert.DeserializeObject<Accommodation>(jsonRaw.Value);
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet<Accommodation>(acco, "accommodation", "de");                
            }
            else if (path.StartsWithSegments("/v1/Gastronomy"))
            {
                var gastro = JsonConvert.DeserializeObject<SmgPoi>(jsonRaw.Value);
                //return JsonLDTransformer.TransformToLD.TransformGastronomyToLD(gastro, "de");
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet<SmgPoi>(gastro, "gastronomy", "de");
            }
            else if (path.StartsWithSegments("/v1/Event"))
            {
                var @event = JsonConvert.DeserializeObject<Event>(jsonRaw.Value);
                //return JsonLDTransformer.TransformToLD.TransformEventToLD(@event, "de");
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet<Event>(@event, "event", "de");
            }
            else if (path.StartsWithSegments("/v1/ODHActivityPoi"))
            {
                var poi = JsonConvert.DeserializeObject<SmgPoi>(jsonRaw.Value);
                //return JsonLDTransformer.TransformToLD.TransformEventToLD(@event, "de");
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet<SmgPoi>(poi, "poi", "de");
            }
            else if (path.StartsWithSegments("/v1/Region"))
            {
                var region = JsonConvert.DeserializeObject<Region>(jsonRaw.Value);
                //return JsonLDTransformer.TransformToLD.TransformEventToLD(@event, "de");
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet<Region>(region, "region", "de");
            }
            else if (path.StartsWithSegments("/v1/TourismAssociation"))
            {
                var tv = JsonConvert.DeserializeObject<Tourismverein>(jsonRaw.Value);
                //return JsonLDTransformer.TransformToLD.TransformEventToLD(@event, "de");
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet<Tourismverein>(tv, "tv", "de");
            }
            else if (path.StartsWithSegments("/v1/Municipality"))
            {
                var municipality = JsonConvert.DeserializeObject<Municipality>(jsonRaw.Value);
                //return JsonLDTransformer.TransformToLD.TransformEventToLD(@event, "de");
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet<Municipality>(municipality, "municipality", "de");
            }
            else if (path.StartsWithSegments("/v1/District"))
            {
                var district = JsonConvert.DeserializeObject<District>(jsonRaw.Value);
                //return JsonLDTransformer.TransformToLD.TransformEventToLD(@event, "de");
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet<District>(district, "district", "de");
            }
            else if (path.StartsWithSegments("/v1/SkiArea"))
            {
                var skiarea = JsonConvert.DeserializeObject<SkiArea>(jsonRaw.Value);
                //return JsonLDTransformer.TransformToLD.TransformEventToLD(@event, "de");
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet<SkiArea>(skiarea, "skiarea", "de");
            }
            else if (path.StartsWithSegments("/v1/Article"))
            {
                //TODO Ensure that the article is of type Recipe or SpecialAnnouncement!

                var recipe = JsonConvert.DeserializeObject<RecipeArticle>(jsonRaw.Value);
                //return JsonLDTransformer.TransformToLD.TransformEventToLD(@event, "de");
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet<RecipeArticle>(recipe, "recipe", "de");
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
