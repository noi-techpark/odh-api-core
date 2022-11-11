﻿using DataModel;
using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OdhApiCore.Formatters
{
    public class JsonLdOutputFormatter : TextOutputFormatter
    {
        public JsonLdOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/ld+json"));
            // Hack because Output formatter Mapping does not work with + inside
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/ldjson"));            

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        private List<object>? Transform(PathString path, JsonRaw jsonRaw)
        {
            //TODO: extract language

            if (path.StartsWithSegments("/v1/Accommodation"))
            {
                var acco = JsonConvert.DeserializeObject<Accommodation>(jsonRaw.Value);
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet(acco, "accommodation", "de");
            }
            else if (path.StartsWithSegments("/v1/Gastronomy"))
            {
                var gastro = JsonConvert.DeserializeObject<ODHActivityPoi>(jsonRaw.Value);
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet(gastro, "gastronomy", "de");
            }
            else if (path.StartsWithSegments("/v1/Event"))
            {
                var @event = JsonConvert.DeserializeObject<Event>(jsonRaw.Value);
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet(@event, "event", "de");
            }
            else if (path.StartsWithSegments("/v1/ODHActivityPoi"))
            {
                var poi = JsonConvert.DeserializeObject<ODHActivityPoi>(jsonRaw.Value);
                //return JsonLDTransformer.TransformToLD.TransformEventToLD(@event, "de");
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet(poi, "poi", "de");
            }
            else if (path.StartsWithSegments("/v1/Region"))
            {
                var region = JsonConvert.DeserializeObject<Region>(jsonRaw.Value);
                //return JsonLDTransformer.TransformToLD.TransformEventToLD(@event, "de");
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet(region, "region", "de");
            }
            else if (path.StartsWithSegments("/v1/TourismAssociation"))
            {
                var tv = JsonConvert.DeserializeObject<Tourismverein>(jsonRaw.Value);
                //return JsonLDTransformer.TransformToLD.TransformEventToLD(@event, "de");
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet(tv, "tv", "de");
            }
            else if (path.StartsWithSegments("/v1/Municipality"))
            {
                var municipality = JsonConvert.DeserializeObject<Municipality>(jsonRaw.Value);
                //return JsonLDTransformer.TransformToLD.TransformEventToLD(@event, "de");
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet(municipality, "municipality", "de");
            }
            else if (path.StartsWithSegments("/v1/District"))
            {
                var district = JsonConvert.DeserializeObject<District>(jsonRaw.Value);
                //return JsonLDTransformer.TransformToLD.TransformEventToLD(@event, "de");
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet(district, "district", "de");
            }
            else if (path.StartsWithSegments("/v1/SkiArea"))
            {
                var skiarea = JsonConvert.DeserializeObject<SkiArea>(jsonRaw.Value);
                //return JsonLDTransformer.TransformToLD.TransformEventToLD(@event, "de");
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet(skiarea, "skiarea", "de");
            }
            else if (path.StartsWithSegments("/v1/Article"))
            {
                //TODO Ensure that the article is of type Recipe or SpecialAnnouncement!

                var recipe = JsonConvert.DeserializeObject<Article>(jsonRaw.Value);
                //return JsonLDTransformer.TransformToLD.TransformEventToLD(@event, "de");
                return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet(recipe, "recipe", "de");
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
                    var jsonLD = "";
                    var options = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };

                    if (transformed.Count == 1)
                    {
                        jsonLD = System.Text.Json.JsonSerializer.Serialize(transformed.FirstOrDefault(), options); //TODO REMOVE NULL VALUES
                            //JsonConvert.SerializeObject(transformed.FirstOrDefault(), Newtonsoft.Json.Formatting.None, jsonsettings);
                                    
                    }
                    else if (transformed.Count > 1)
                    {
                        jsonLD = System.Text.Json.JsonSerializer.Serialize(transformed, options);
                    }

                    
                    await context.HttpContext.Response.WriteAsync(jsonLD);
                    //await context.HttpContext.Response.WriteAsync(transformed);
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
