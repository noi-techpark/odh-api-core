using DataModel;
using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.api
{
    public class JsonLDController : OdhController
    {        
        public JsonLDController(IWebHostEnvironment env, ISettings settings, ILogger<JsonLDController> logger, QueryFactory queryFactory, IHttpClientFactory httpClientFactory)
            : base(env, settings, logger, queryFactory)
        {
            
        }

        /// <summary>
        /// GET Detail Data in JSON LD Format (Schema.org Datatypes as output)
        /// </summary>
        /// <param name="type">Data Type to transform currently available: ('accommodation', 'gastronomy', 'event', 'recipe', 'poi', 'region', 'tv', 'municipality', 'district', 'skiarea') required</param>
        /// <param name="Id">ID of the data to transform, required</param>
        /// <param name="language">Output Language, standard EN</param>
        /// <param name="idtoshow">ID to show on Json LD @id, not provided Id of ODH api call is taken</param>
        /// <param name="imageurltoshow">image url to show on Json LD @image, not provided image url of data is taken</param>
        /// <param name="urltoshow">url to show on Json LD @id, not provided idtoshow is taken, idtoshow not provided url is filled with url of the data</param>
        /// <param name="showid">Show the @id property in Json LD default value true</param>
        /// <returns></returns>
        [Authorize(Roles = "DataReader")]
        [HttpGet, Route("api/JsonLD/DetailInLD")]
        public async Task<IActionResult> GetDetailInLD(string type, string Id, string language = "en", string idtoshow = "", string urltoshow = "", string imageurltoshow = "", bool showid = true)
        {
            try
            {
                var myobject = default(List<object>);

                switch (type.ToLower())
                {
                    case "accommodation":
                        myobject = await LoadFromRavenDBSchemaNet<Accommodation>(Id, language, idtoshow, urltoshow, imageurltoshow, type.ToLower(), showid);
                        break;
                    case "gastronomy":
                        myobject = await LoadFromRavenDBSchemaNet<SmgPoi>(Id, language, idtoshow, urltoshow, imageurltoshow, type.ToLower(), showid);
                        break;
                    case "event":
                        myobject = await LoadFromRavenDBSchemaNet<Event>(Id, language, idtoshow, urltoshow, imageurltoshow, type.ToLower(), showid);
                        break;
                    case "recipe":
                        myobject = await LoadFromRavenDBSchemaNet<RecipeArticle>(Id, language, idtoshow, urltoshow, imageurltoshow, type.ToLower(), showid);
                        break;
                    case "poi":
                        myobject = await LoadFromRavenDBSchemaNet<SmgPoi>(Id, language, idtoshow, urltoshow, imageurltoshow, type.ToLower(), showid);
                        break;
                    case "region":
                        myobject = await LoadFromRavenDBSchemaNet<Region>(Id, language, idtoshow, urltoshow, imageurltoshow, type.ToLower(), showid);
                        break;
                    case "tv":
                        myobject = await LoadFromRavenDBSchemaNet<Tourismverein>(Id, language, idtoshow, urltoshow, imageurltoshow, type.ToLower(), showid);
                        break;
                    case "municipality":
                        myobject = await LoadFromRavenDBSchemaNet<Municipality>(Id, language, idtoshow, urltoshow, imageurltoshow, type.ToLower(), showid);
                        break;
                    case "district":
                        myobject = await LoadFromRavenDBSchemaNet<District>(Id, language, idtoshow, urltoshow, imageurltoshow, type.ToLower(), showid);
                        break;
                    case "skiarea":
                        myobject = await LoadFromRavenDBSchemaNet<SkiArea>(Id, language, idtoshow, urltoshow, imageurltoshow, type.ToLower(), showid);
                        break;
                    default:
                        myobject = new List<object>();
                        myobject.Add(new { message = "JSON LD not available" });
                        break;
                }

                var myjson = "";

                if (myobject != null)
                {
                    if (type.ToLower() == "event")
                        myjson = JsonConvert.SerializeObject(myobject, Newtonsoft.Json.Formatting.None,
                                    new JsonSerializerSettings
                                    {
                                        NullValueHandling = NullValueHandling.Ignore
                                    });
                    else
                        myjson = JsonConvert.SerializeObject(myobject.FirstOrDefault(), Newtonsoft.Json.Formatting.None,
                                    new JsonSerializerSettings
                                    {
                                        NullValueHandling = NullValueHandling.Ignore
                                    });


                    //switch(myobject.Item1)
                    //{
                    //    case "Accommodation":

                    //        string typetoinsert = "\"@type\":\"Hotel\",";
                    //        myjson = myjson.Insert(1, typetoinsert);
                    //        break;
                    //}

                    return Ok(myjson);

                    //return new HttpResponseMessage()
                    //{
                    //    StatusCode = HttpStatusCode.OK,
                    //    Content = new StringContent(myjson, Encoding.UTF8, "application/ld+json")
                    //};
                }
                else
                    return NotFound();
                    //return new HttpResponseMessage()
                    //{
                    //    StatusCode = HttpStatusCode.NotFound,
                    //    Content = new StringContent("object not found", Encoding.UTF8, "application/ld+json")
                    //};
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<List<object>> LoadFromRavenDBSchemaNet<T>(string Id, string language, string idtoshow, string urltoshow, string imagetoshow, string type, bool showid)
        {
            var query =
                  QueryFactory.Query(type)
                      .Select("data")
                      .Where("id", Id.ToUpper())
                      .When(FilterClosedData, q => q.FilterClosedData());

            var myobject = await query.FirstOrDefaultAsync<JsonRaw?>();

            if (myobject != null)
            {
                var myparsedobject = JsonConvert.DeserializeObject<T>(myobject.Value);
                if (myparsedobject is { })
                    return JsonLDTransformer.TransformToSchemaNet.TransformDataToSchemaNet<T>(myparsedobject, type, language, null, idtoshow, urltoshow, imagetoshow, showid);               
            }

            return new();
        }

    }
}
