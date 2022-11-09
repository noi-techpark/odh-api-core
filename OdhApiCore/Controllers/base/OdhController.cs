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

        protected bool FilterCC0License
        {
            get
            {
                var roles = new[] {
                    "IDM",
                    "AllImages"
                };
                return !roles.Any(User.IsInRole);
            }
        }

        //TODO EXTEND THIS ALSO TO ODHActivityPoiReader etc...
        /// <summary>
        /// If User has Role DataReader or Reader of this Entity set Filtercloseddata to false
        /// </summary>
        protected bool FilterClosedData
        {
            get
            {
                var roles = new[] {
                    "IDM"
                    //"DataReader",
                    //$"{this.ControllerContext.ActionDescriptor.ControllerName}Reader"
                };
                return !roles.Any(User.IsInRole);
            }
        }
        protected bool ReducedData => FiltertoReduced;

        /// <summary>
        /// If user is in Role IDM or LTS display full LTS Data
        /// </summary>
        protected bool FiltertoReduced
        {
            get
            {
                var roles = new[] {
                    "IDM",
                    "LTS"
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
                List<string> fieldstohide = new();

                var roleclaims = User.Claims.Where(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(claim => claim.Value).ToList();

                //Search all settings with Entity = Controllername
                var fields = settings.Field2HideConfig
                    .Where(x => x.Entity == this.ControllerContext.RouteData.Values["controller"]?.ToString() ||
                                string.IsNullOrEmpty(x.Entity));

                foreach(var field in fields)
                {
                    if(roleclaims.Intersect(field.DisplayOnRoles ?? new()).Count() == 0)
                    {
                        fieldstohide.AddRange(field.Fields ?? new());
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

        protected async Task<IActionResult> UpsertData<T>(T data, string table, bool errorwhendataexists = false, bool errorwhendataisnew = false) where T : IIdentifiable, IImportDateassigneable, IMetaData
        {
            //TODO Username and provenance of the insert/edit

            return Ok(await QueryFactory.UpsertData<T>(data, table, errorwhendataexists, errorwhendataisnew));          
        }

        protected async Task<IActionResult> DeleteData(string id, string table)
        {
            return Ok(await QueryFactory.DeleteData(id, table));            
        }
    }    
}