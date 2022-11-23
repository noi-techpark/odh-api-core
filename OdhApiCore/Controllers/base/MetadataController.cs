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
            bool writetotable = false,
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

            if (writetotable)
                await WriteJsonToTable();

            if (!fromtable)
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

        /// <summary>
        /// GET Metadata Single 
        /// </summary>
        /// <param name="id">ID of the Metadata</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>TourismMetadata Object</returns>
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(TourismMetaData), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Metadata/{id}", Name = "SingleMetadata")]
        public async Task<IActionResult> GetArticleSingle(
            string id,
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetSingle(id, language, fields: fields ?? Array.Empty<string>(), removenullvalues, cancellationToken);
        }

        #region GET

        private Task<IActionResult> GetMetadataFromTable(
          string[] fields, string? language, uint pagenumber, int? pagesize,
          string? seed, string? lastchange,
          string? rawfilter, string? rawsort, bool removenullvalues, 
          CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                if (rawsort == null)
                    rawsort = "Id";

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

                var dataTransformed =
                    data.List.Select(
                        raw => raw.TransformRawData(language, fields, checkCC0: false, filterClosedData: false, filteroutNullValues: removenullvalues, urlGenerator: UrlGeneratorStatic, fieldstohide: new List<string>())
                    );

                //TODO WRITE A NEW urlGenerator for metadata

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

        private Task<IActionResult> GetSingle(string id, string? language, string[] fields, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("metadata")
                        .Select("data")
                        .Where("id", id.ToLower());

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                var fieldsTohide = FieldsToHide;

                return data?.TransformRawData(language, fields, checkCC0: false, filterClosedData: false, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide);
            });
        }

        #endregion

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
                json.FirstImport = DateTime.Now;
                json.LastChange = DateTime.Now;
                json.Shortname = json.ApiIdentifier;
                json._Meta = new Metadata() { Id = json.Id, LastUpdate  = json.LastChange, Reduced = false, Source = "odh", Type = "odhmetadata" };

                json.ApiVersion = "v1";
                json.ApiDescription = new Dictionary<string, string>();
                json.ApiDescription.TryAddOrUpdate("en", json.Description);
                json.Sources = new List<string>();
                json.Sources.Add(json.Source);
                json.DatabrowserActive = false;
                json.ApiAccess = new Dictionary<string, string>();
                json.ApiAccess.TryAddOrUpdate(json.Source, json.LicenseType);
                json.Output = new Dictionary<string, string>();
                json.Output.TryAddOrUpdate("default","application/json");
                json.Output.TryAddOrUpdate("ld+json", "application/ld+json");
                json.PathParam = json.ApiIdentifier.Split('/').ToList();


                if (json.ApiIdentifier != "Find")
                {                    
                    var currentloc = new Uri($"{Request.Scheme}://{Request.Host}/v1/" + json.ApiIdentifier);

                    json.RecordCount = Convert.ToInt32(await MetaDataApiRecordCount.GetApiRecordCount(currentloc.AbsoluteUri, json.ApiFilter, ""));
                }

         
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
            get { return "swagger/index.html#/" + swaggerUrl; }
            set { swaggerUrl = value; }
        }

        public string Self
        {
            get
            {
                return this.ApiIdentifier + this.ApiFilter;                
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

        public ICollection<string> Sources { get; set; }

        public int? RecordCount { get; set; }

        public IDictionary<string, string> Output { get; set; }

        public IDictionary<string,string> ApiDescription { get; set; }

        public string? ApiVersion { get; set; }

        public ICollection<string>? PathParam { get; set; }

        public bool? DatabrowserActive { get; set; }

        public IDictionary<string, string> ApiAccess { get; set; }
    }
}
