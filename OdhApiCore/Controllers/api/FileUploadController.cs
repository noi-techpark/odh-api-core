using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Text;

namespace OdhApiCore.Controllers.api
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly ISettings settings;
        protected ILogger<OdhController> Logger { get; }

        public FileUploadController(IWebHostEnvironment env, ISettings settings, ILogger<OdhController> logger)
        {
            this.env = env;
            this.settings = settings;
            this.Logger = logger;
        }

        //TODO have a look here https://weblog.west-wind.com/posts/2017/sep/14/accepting-raw-request-body-content-in-aspnet-core-api-controllers


        //[ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataModify,DataCreate,ODHPoiCreate,ODHPoiModify,ODHPoiManager,CommonCreate,CommonModify,CommonManager,ArticleCreate,ArticleModify,ArticleManager,EventShortManager,EventShortCreate")]
        //[HttpPost, Route("api/FileUpload/{type}/{directory}")]
        //public async Task<IActionResult> PostFormData(string type, string directory)
        //{
        //    //using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
        //    //{
        //    //    return await reader.ReadToEndAsync();
        //    //}

        //    // Check if the request contains multipart/form-data.
        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }

        //    string root = System.Configuration.ConfigurationManager.AppSettings["imageresizerpath"] + "images\\" + type + "\\" + directory;

        //    var provider = new CustomMultipartFormDataStreamProvider(root);
        //    string filename = "";

        //    try
        //    {
        //        // Read the form data.
        //        var result = await Request.Content.ReadAsMultipartAsync(provider);

        //        //This illustrates how to get the file names.
        //        foreach (MultipartFileData file in provider.FileData)
        //        {
        //            //filename = file.Headers.ContentDisposition.FileName.Trim('\"');

        //            string filepath = System.Configuration.ConfigurationManager.AppSettings["imageresizer"] + @"ImageHandler.ashx?src=images/" + type + "/" + directory + "/";

        //            filename = filepath + Path.GetFileName(file.LocalFileName);
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK, filename);
        //    }
        //    catch (System.Exception e)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
        //    }

        //}

        //[ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataMofify,DataCreate,ODHPoiCreate,ODHPoiModify,ODHPoiManager,CommonCreate,CommonModify,CommonManager,ArticleCreate,ArticleModify,ArticleManager")]
        //[HttpPost, Route("api/GpxFileUpload")]
        //public async Task<IActionResult> PostGpxData()
        //{
        //    string root = HttpContext.Current.Server.MapPath("~/Upload/Gpx");
        //    var provider = new CustomMultipartFormDataStreamProvider(root);
        //    string filename = "";

        //    try
        //    {
        //        // Read the form data.
        //        await Request.Content.ReadAsMultipartAsync(provider);

        //        //This illustrates how to get the file names.
        //        foreach (MultipartFileData file in provider.FileData)
        //        {
        //            filename = Path.GetFileName(file.LocalFileName);
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK, filename);
        //    }
        //    catch (System.Exception e)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
        //    }

        //}

        //[ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataModify,DataCreate,ODHPoiCreate,ODHPoiModify,ODHPoiManager,CommonCreate,CommonModify,CommonManager,ArticleCreate,ArticleModify,ArticleManager,EventShortManager,EventShortCreate")]
        //[HttpPost, Route("api/DocFileUpload/{type}")]
        //public async Task<IActionResult> PostDocumentData(string type)
        //{
        //    // Check if the request contains multipart/form-data.
        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }

        //    string root = System.Configuration.ConfigurationManager.AppSettings["imageresizerpath"] + "images\\eventshort\\" + type + "\\";

        //    var provider = new CustomMultipartFormDataStreamProvider(root);
        //    string filename = "";

        //    try
        //    {
        //        // Read the form data.
        //        var result = await Request.Content.ReadAsMultipartAsync(provider);

        //        //This illustrates how to get the file names.
        //        foreach (MultipartFileData file in provider.FileData)
        //        {
        //            //filename = file.Headers.ContentDisposition.FileName.Trim('\"');

        //            string filepath = System.Configuration.ConfigurationManager.AppSettings["imageresizer"] + @"images/eventshort/" + type + "/";

        //            filename = filepath + Path.GetFileName(file.LocalFileName);
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK, filename);
        //    }
        //    catch (System.Exception e)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
        //    }

        //}

        //[ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataMofify,DataCreate,DataDelete,ODHPoiCreate,ODHPoiModify,ODHPoiManager,ODHPoiUpdate,CommonCreate,CommonModify,CommonManager,CommonDelete,ArticleCreate,ArticleModify,ArticleManager,ArticleDelete")]
        //[HttpDelete, Route("api/FileDelete/{filepath}")]
        //public IActionResult Delete(string filepath)
        //{
        //    string root = System.Configuration.ConfigurationManager.AppSettings["imageresizerpath"] + filepath.Replace("|", "\\").Replace("$", ".");

        //    try
        //    {
        //        File.Delete(root);
        //        return Ok();
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}
    }

    //TODO: https://stackoverflow.com/questions/46866849/multipartformdatastreamprovider-for-asp-net-core-2

    //public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    //{
    //    public CustomMultipartFormDataStreamProvider(string path)
    //        : base(path)
    //    { }

    //    public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
    //    {
    //        var fileExtension = Path.GetExtension(headers.ContentDisposition.FileName.Replace("\"", string.Empty));

    //        Guid filenameguid = Guid.NewGuid();

    //        var name = filenameguid.ToString() + fileExtension;
            
    //        return name;
    //    }
    //}
}
