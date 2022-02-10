using DataModel;
using Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    [ApiController]
    [Route("v1")]    
    [FormatFilter]
    public abstract class OdhController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly ISettings settings;

        public OdhController(IWebHostEnvironment env, ISettings settings, ILogger<OdhController> logger, QueryFactory queryFactory)
        {
            this.env = env;
            this.settings = settings;
            this.Logger = logger;
            this.QueryFactory = queryFactory;
        }

        protected ILogger<OdhController> Logger { get; }

        protected QueryFactory QueryFactory { get; }

        protected bool FilterCC0License => FilterClosedData;

        //TODO EXTEND THIS ALSO TO ODHActivityPoiReader etc...
        protected bool FilterClosedData
        {
            get
            {
                var roles = new[] {
                    "DataReader",
                    $"{this.ControllerContext.ActionDescriptor.ControllerName}Reader"
                };
                return !roles.Any(User.IsInRole);
            }
        }
        
        protected IEnumerable<string> UserRolesList
        {
            get
            {
                var roleclaims = User.Claims.Where(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(claim => claim.Value).ToList();

                return roleclaims ?? new List<string>();
            }
        }

        //public bool CheckAvailabilitySearch()
        //{
        //    List<string> roles = new List<string>() { "DataReader", "AccoReader" };

        //    foreach(var role in roles)
        //    {
        //        if (User.IsInRole(role))
        //            return true;                
        //    }

        //    return false;
        //}

        protected IEnumerable<string> FieldsToHide
        {
            get
            {
                List<string> fieldstohide = new List<string>();

                var roleclaims = User.Claims.Where(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(claim => claim.Value).ToList();

                //Search all settings with Entity = Controllername
                var fields = settings.Field2HideConfig
                    .Where(x => x.Entity == this.ControllerContext.RouteData.Values["controller"].ToString() ||
                                String.IsNullOrEmpty(x.Entity));

                foreach(var field in fields)
                {
                    if(roleclaims.Intersect(field.DisplayOnRoles).Count() == 0)
                    {
                        fieldstohide.AddRange(field.Fields);
                    }
                }

                return fieldstohide;
            }
        }

        protected Func<string, string> UrlGenerator
        {
            get
            {
                return self =>
                {
                    var chunks = self.Split('/', 2);
                    if (chunks.Length < 2)
                        return self;

                    //Hack if there is another / in the route to check if it is not generating side effects
                    if(chunks[1].Split('/').Count() > 1)
                    {
                        chunks[1] = chunks[1].Split('/')[1];
                    }    

                    var (controller, id) = (chunks[0], chunks[1]);
                    return Url.Link($"Single{controller}", new { id })!;
                };
            }
        }        

        protected async Task<IActionResult> DoAsync(Func<Task<IActionResult>> f)
        {
            try
            {
                return await f();
            }
            catch (PostGresSQLHelperException ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
            catch (JsonPathException ex)
            {
                return this.BadRequest(new
                {
                    error = "Invalid JSONPath selection",
                    path = ex.Path,
                    details = env.IsDevelopment() ? ex.ToString() : ex.Message
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Request Error")
                    return this.BadRequest(new { error = env.IsDevelopment() ? ex.ToString() : ex.Message });
                else if (ex.Message == "No data")
                    return this.StatusCode(StatusCodes.Status404NotFound, new { error = env.IsDevelopment() ? ex.ToString() : ex.Message });
                else
                    return this.StatusCode(StatusCodes.Status500InternalServerError, new { error = env.IsDevelopment() ? ex.ToString() : ex.Message });
            }
        }

        protected Task<IActionResult> DoAsyncReturn(Func<Task<object?>> f)
        {
            return DoAsync(async () =>
            {
                object? result = await f();
                if (result == null)
                    return this.NotFound();
                else
                    return this.Ok(result);
            });
        }

        //Provide Methods for POST, PUT, DELETE passing DataType etc...

        protected async Task<IActionResult> UpsertData<T>(T data, string table) where T : IIdentifiable, IImportDateassigneable, IMetaData
        {
            if (data == null)
                throw new Exception("No data");

            //Check if data exists
            var query = QueryFactory.Query(table)
                      .Select("data")
                      .Where("id", data.Id);

            var queryresult = await query.GetAsync<T>();

            string operation = "";
            var crudresult = 0;

            //Setting MetaInfo
            data._Meta = MetadataHelper.GetMetadataobject<T>(data);

            if (queryresult == null || queryresult.Count() == 0)
            {
                data.FirstImport = DateTime.Now;
                data.LastChange = DateTime.Now;

                crudresult = await QueryFactory.Query(table)
                   .InsertAsync(new JsonBData() { id = data.Id, data = new JsonRaw(data) });
                operation = "INSERT";
            }
            else
            {
                data.LastChange = DateTime.Now;

                crudresult = await QueryFactory.Query(table).Where("id", data.Id)
                        .UpdateAsync(new JsonBData() { id = data.Id, data = new JsonRaw(data) });
                operation = "UPDATE";
            }
                        
            return Ok(new GenericResult() { Message = String.Format("{0} success: {1} recordsmodified: {2}", operation, data.Id, crudresult) });
        }

        protected async Task<IActionResult> DeleteData(string id, string table)
        {
            if (String.IsNullOrEmpty(id))
                throw new Exception("No data");

            //Check if data exists
            var query =
                  QueryFactory.Query(table)
                      .Select("data")
                      .Where("id", id);

            var crudresult = 0;

            if (query == null)
            {
                throw new Exception("No data");
            }
            else
            {
                crudresult = await QueryFactory.Query(table).Where("id", id)
                        .DeleteAsync();                
            }

            return Ok(new GenericResult() { Message = String.Format("DELETE success: {0} recordsmodified: {1}", id, crudresult) });
        }

        //Temporary Here will be outsourced to Importer

    }    
}