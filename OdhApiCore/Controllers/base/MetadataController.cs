using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.CacheOutput;
using DataModel;
using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OdhApiCore.Controllers.api;
using OdhApiCore.Responses;
using Schema.NET;
using ServiceReferenceLCS;
using SqlKata.Execution;

namespace OdhApiCore.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [EnableCors("CorsPolicy")]
    public class MetadataController : OdhController
    {
        private static string absoluteUri = "";
        private readonly ISettings settings;

        public MetadataController(IWebHostEnvironment env, ISettings settings, ILogger<ODHActivityPoiController> logger, QueryFactory queryFactory) : base(env, settings, logger, queryFactory)
        {
            this.settings = settings;
        }
       
        [HttpGet, Route("", Name = "TourismApi")]
        [HttpGet, Route("Metadata", Name = "TourismApiMetaData")]
        public async Task<IActionResult> Get(
            bool fromtable = false,
            string? language = null,
            uint pagenumber = 1,
            PageSize pagesize = null!,
            string? seed = null,
            string? updatefrom = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default
            )
        {
            //var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
            var location = new Uri($"{Request.Scheme}://{Request.Host}");
            absoluteUri = location.AbsoluteUri;

            if(!fromtable)
            {
                var result = await GetMetadata();
                return Ok(result);
            }
            else
            {
                return await GetMetadataFromTable(fields: fields ?? Array.Empty<string>(), language: language, pagenumber: pagenumber, pagesize: pagesize,
                    seed: seed, lastchange: updatefrom, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);                    
            }           
        }

        #region POST PUT DELETE

        /// <summary>
        /// POST Insert new MetaData
        /// </summary>
        /// <param name="metadata">TourismData Object</param>
        /// <returns>Http Response</returns>
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,MetadataManager,MetadataCreate")]
        [InvalidateCacheOutput(nameof(Get))]
        [ProducesResponseType(typeof(PGCRUDResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost, Route("Metadata")]
        public Task<IActionResult> Post([FromBody] TourismMetaData metadata)
        {
            return DoAsyncReturn(async () =>
            {
                //GENERATE ID
                //metadata.Id = Helper.IdGenerator.GenerateIDFromType(metadata);
                
                return await UpsertData<TourismMetaData>(metadata, "metadata", true);
            });
        }

        /// <summary>
        /// PUT Modify existing Metadata
        /// </summary>
        /// <param name="id">Metadata Id</param>
        /// <param name="metadata">TourismData Object</param>
        /// <returns>Http Response</returns>
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataModify,MetadataManager,MetadataModify")]
        [InvalidateCacheOutput(nameof(Get))]
        [ProducesResponseType(typeof(PGCRUDResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut, Route("Metadata/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] TourismMetaData metadata)
        {
            return DoAsyncReturn(async () =>
            {
                //Check ID uppercase lowercase
                //metadata.Id = Helper.IdGenerator.CheckIdFromType<TourismMetaData>(id);

                return await UpsertData<TourismMetaData>(metadata, "metadata", false, true);
            });
        }

        /// <summary>
        /// DELETE Metadata by Id
        /// </summary>
        /// <param name="id">Metadata Id</param>
        /// <returns>Http Response</returns>
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataDelete,MetadataManager,MetadataDelete")]
        [InvalidateCacheOutput(nameof(Get))]
        [ProducesResponseType(typeof(PGCRUDResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete, Route("MetaData/{id}")]
        public Task<IActionResult> Delete(string id)
        {
            return DoAsyncReturn(async () =>
            {
                //Check ID uppercase lowercase
                id = Helper.IdGenerator.CheckIdFromType<TourismMetaData>(id);

                return await DeleteData(id, "metadata");
            });
        }


        #endregion

        #region HELPERS

        private async Task<IEnumerable<TourismMetaData>> GetMetadata()
        {
            List<TourismMetaData> tourismdatalist = new List<TourismMetaData>();

            string fileName = Path.Combine(settings.JsonConfig.Jsondir, $"v1.json");

            using (StreamReader r = new StreamReader(fileName))
            {
                string json = await r.ReadToEndAsync();

                tourismdatalist = JsonConvert.DeserializeObject<List<TourismMetaData>>(json);
            }

            return tourismdatalist;
        }

        private Task<IActionResult> GetMetadataFromTable(
            string[] fields, string? language, uint pagenumber, int? pagesize,
            string? seed, string? lastchange,
            string? rawfilter, string? rawsort, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {               
                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From("metadata")                           
                        .ApplyRawFilter(rawfilter)
                        .ApplyOrdering_GeneratedColumns(ref seed, new PGGeoSearchResult() { geosearch = false }, rawsort);

                // Get paginated data
                var data =
                    await query
                        .PaginateAsync<JsonRaw>(
                            page: (int)pagenumber,
                            perPage: pagesize ?? 25);

                var fieldsTohide = FieldsToHide;

                var dataTransformed =
                    data.List.Select(
                        raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide)
                    );

                uint totalpages = (uint)data.TotalPages;
                uint totalcount = (uint)data.Count;

                return ResponseHelpers.GetResult(
                    pagenumber,
                    totalpages,
                    totalcount,
                    seed,
                    dataTransformed,
                    Url);
            });
        }


        public static string GetAbsoluteUri()
        {
            return absoluteUri;
        }

        private async Task<ActionResult> WriteJsonToTable()
        {
            var jsondata = await GetMetadata();
            string table = "metadata";
            int result = 0;

            foreach(var json in jsondata)
            {
                //Check if data exists
                var query = QueryFactory.Query(table)
                          .Select("data")
                          .Where("id", json.Id);

                var queryresult = await query.GetAsync<TourismMetaData>();

                if (queryresult == null || queryresult.Count() == 0)
                {                    
                    result = await QueryFactory.Query(table)
                       .InsertAsync(new JsonBData() { id = json.Id, data = new JsonRaw(json) });                    
                }
                else
                {
                    result = await QueryFactory.Query(table).Where("id", json.Id)
                            .UpdateAsync(new JsonBData() { id = json.Id, data = new JsonRaw(json) });                    
                }

            }

            return Ok("Result: " + result);
        }


        #endregion
    }

    public class TourismMetaData : IMetaData, IImportDateassigneable, IIdentifiable
    {        
        public TourismMetaData()
        {
            _Meta = new Metadata();
        }

        public string ApiIdentifier { get; set; } = default!;
        
        public string ApiFilter { get; set; }

        public string Id { get; set; } = default!;
        public string OdhType { get; set; } = default!;
        public string Description { get; set; } = default!;

        private string swaggerUrl = default!;
        public string SwaggerUrl
        {
            get { return Uri.EscapeUriString(MetadataController.GetAbsoluteUri() + "swagger/index.html#/" + swaggerUrl); }
            set { swaggerUrl = value; }
        }

        public string Self
        {
            get
            {
                return Uri.EscapeUriString(MetadataController.GetAbsoluteUri() + "v1/") + this.ApiIdentifier + this.ApiFilter;                
            }
        }

        public string Source { get; set; }

        public string License { get; set; } = default!;

        public string LicenseType { get; set; }

        public string LicenseInfo { get; set; }

        public bool Deprecated { get; set; }

        public bool SingleDataset { get; set; }
        public Metadata _Meta { get; set; }
        public DateTime? FirstImport { get; set; }
        public DateTime? LastChange { get; set; }
        public string? Shortname { get; set; }
    }
}
