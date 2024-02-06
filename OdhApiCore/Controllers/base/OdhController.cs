// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper;
using Helper.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OdhNotifier;
using Schema.NET;
using ServiceReferenceLCS;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
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

        public OdhController(IWebHostEnvironment env, ISettings settings, ILogger<OdhController> logger, QueryFactory queryFactory, IOdhPushNotifier odhpushnotifier)
        {
            this.env = env;
            this.settings = settings;
            this.Logger = logger;
            this.QueryFactory = queryFactory;
            this.OdhPushnotifier = odhpushnotifier;
        }

        protected ILogger<OdhController> Logger { get; }

        protected QueryFactory QueryFactory { get; }

        private IOdhPushNotifier OdhPushnotifier;

        /// <summary>
        /// When not in this role Images which does not have a CC0 License are filtered out maybe obsolete ?
        /// </summary>
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
        
        /// <summary>
        /// If User is in Role IDM or A22 set FilterClosedData to false
        /// </summary>
        protected bool FilterClosedData
        {
            get
            {
                var roles = new[] {
                    "IDM",
                    "A22"
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

        /// <summary>
        /// ADD all relevant roles for data filtering
        /// </summary>
        protected IEnumerable<string> UserRolesToFilter
        {
            get
            {               
                var roles = new[] {
                    "IDM",
                    "LTS",
                    "A22",
                    "STA"
                };

                if (!roles.Any(User.IsInRole))
                    return new List<string>() { "ANONYMOUS" };
                else
                {
                    var userroles = new List<string>();

                    foreach (var role in roles)
                    {
                        if(User.IsInRole(role))
                            userroles.Add(role);
                    }

                    return userroles;
                }                
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

        //Test if there is an additionalfilter
        protected Dictionary<string,string> AdditionalFiltersToAdd
        {
            get
            {
                var additionalfilterdict = new Dictionary<string,string>();

                if (Request.Path.Value != null)
                {
                    //GET ENDPOINT (after v1)

                    var rolesforendpoint = User.Claims.Where(c => c.Type == ClaimTypes.Role && c.Value.StartsWith(Request.Path.GetPathNextTo("/", "v1") + "_"));
                    foreach (var role in rolesforendpoint)
                    {
                        var splittedrole = role.Value.Split("_");
                        if (splittedrole.Length == 3)
                        {
                            additionalfilterdict.TryAddOrUpdate(splittedrole[1], splittedrole[2]);
                        }
                    }
                }

                return additionalfilterdict;
            }
        }


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

        protected Func<string,string> UrlGeneratorStatic
        {
            get
            {
                return self =>
                {
                    var location = new Uri($"{Request.Scheme}://{Request.Host}/" + self);
                    return location.AbsoluteUri;
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

        //TODO Upsert Data and push to all published Channels

        protected async Task<IActionResult> UpsertData<T>(T data, string table, bool errorwhendataexists = false, bool errorwhendataisnew = false, string editsource = "api", string? deletecondition = null) where T : IIdentifiable, IImportDateassigneable, IMetaData 
        {
            //TODO Username and provenance of the insert/edit
            //Get the Username
            string editor = this.User != null && this.User.Identity != null && this.User.Identity.Name != null ? this.User.Identity.Name : "anonymous";
            string operation = errorwhendataexists && !errorwhendataisnew ? "CREATE" : "UPDATE";

            if (HttpContext.Request.Headers.ContainsKey("Referer") && !String.IsNullOrEmpty(HttpContext.Request.Headers["Referer"]))
                editsource = HttpContext.Request.Headers["Referer"];

            var result = await QueryFactory.UpsertData<T>(data, table, operation, editor, editsource, errorwhendataexists, errorwhendataisnew, deletecondition);

            //TODO push modified data to all published Channels

            return ReturnCRUDResult(result);
        }

        protected async Task<IActionResult> UpsertDataAndCompare<T>(T data, string table, bool errorwhendataexists = false, bool errorwhendataisnew = false, string editsource = "api", string? deletecondition = null) where T : IIdentifiable, IImportDateassigneable, IMetaData, IPublishedOn, new()
        {
            //TODO Username and provenance of the insert/edit
            //Get the Username
            string editor = this.User != null && this.User.Identity != null && this.User.Identity.Name != null ? this.User.Identity.Name : "anonymous";
            string operation = errorwhendataexists && !errorwhendataisnew ? "CREATE" : "UPDATE";

            if (HttpContext.Request.Headers.ContainsKey("Referer") && !String.IsNullOrEmpty(HttpContext.Request.Headers["Referer"]))
                editsource = HttpContext.Request.Headers["Referer"];

            var result = await QueryFactory.UpsertDataAndCompare<T>(data, table, operation, editor, editsource, errorwhendataexists, errorwhendataisnew, true, deletecondition);

            //push modified data to all published Channels
            result.pushed = await CheckIfObjectChangedAndPush(result, result.id, result.odhtype);
            
            return ReturnCRUDResult(result);
        }

        protected async Task<IActionResult> UpsertDataAndFullCompare<T>(T data, string table, bool errorwhendataexists = false, bool errorwhendataisnew = false, string editsource = "api", string? deletecondition = null) where T : IIdentifiable, IImportDateassigneable, IMetaData, IPublishedOn, IImageGalleryAware, new()
        {
            //TODO Username and provenance of the insert/edit
            //Get the Username
            string editor = this.User != null && this.User.Identity != null && this.User.Identity.Name != null ? this.User.Identity.Name : "anonymous";
            string operation = errorwhendataexists && !errorwhendataisnew ? "CREATE" : "UPDATE";

            if (HttpContext.Request.Headers.ContainsKey("Referer") && !String.IsNullOrEmpty(HttpContext.Request.Headers["Referer"]))
                editsource = HttpContext.Request.Headers["Referer"];

            var result = await QueryFactory.UpsertDataAndFullCompare<T>(data, table, operation, editor, editsource, errorwhendataexists, errorwhendataisnew, true, true, deletecondition);

            //push modified data to all published Channels
            result.pushed = await CheckIfObjectChangedAndPush(result, result.id, result.odhtype);

            return ReturnCRUDResult(result);
        }


        //TODO Delete Data and push to all published Channels

        protected async Task<IActionResult> DeleteData(string id, string table, bool casesensitive = true, string? deletecondition = null)
        {
            //TODO Add logic which checks if user is authorized to delete data
            //Return not found if wrong ID
            //Return forbitten 403 if 
            //Return 401 if unauthorized

            var result = await QueryFactory.DeleteData(id, table, casesensitive, deletecondition);
            //push modified data to all published Channels
            result.pushed = await CheckIfObjectChangedAndPush(result, result.id, result.odhtype);
            
            return ReturnCRUDResult(result);
        }

        protected async Task<IActionResult> DeleteData<T>(string id, string table, string? deletecondition = null) where T : IIdentifiable, IMetaData, IPublishedOn, IImportDateassigneable, new()
        {
            //TODO Add logic which checks if user is authorized to delete data
            //Return not found if wrong ID
            //Return forbitten 403 if 
            //Return 401 if unauthorized

            var result = await QueryFactory.DeleteData<T>(id, table, deletecondition);
            //push modified data to all published Channels
            result.pushed = await CheckIfObjectChangedAndPush(result, result.id, result.odhtype);

            return ReturnCRUDResult(result);         
        }

        protected async Task<IDictionary<string, NotifierResponse>?> CheckIfObjectChangedAndPush(PGCRUDResult myupdateresult, string id, string datatype, string pushorigin = "odh.api.push")
        {
            IDictionary<string, NotifierResponse>? pushresults = default(IDictionary<string, NotifierResponse>);

            //Check if data has changed and Push To all channels
            if (myupdateresult.error == 0 && myupdateresult.objectchanged != null && myupdateresult.objectchanged > 0 && myupdateresult.pushchannels != null && myupdateresult.pushchannels.Count > 0)
            {
                //Check if image has changed
                bool hasimagechanged = false;
                if (myupdateresult.objectimagechanged != null && myupdateresult.objectimagechanged.Value > 0)
                    hasimagechanged = true;

                pushresults = await OdhPushnotifier.PushToPublishedOnServices(id, datatype.ToLower(), pushorigin, hasimagechanged, false, "api", myupdateresult.pushchannels.ToList());
            }

            return pushresults;
        }

        protected IActionResult ReturnCRUDResult(PGCRUDResult result)
        {
            switch (result.errorreason)
            {
                case "": return Ok(result);
                case "Not Allowed": return Forbid();
                case "Not Found": return NotFound();
                case "Bad Request": return BadRequest();
                case "No Data": return BadRequest();
                case "Internal Error": return StatusCode(500);
                default:
                    return Ok(result);
            }
        }
    }
}