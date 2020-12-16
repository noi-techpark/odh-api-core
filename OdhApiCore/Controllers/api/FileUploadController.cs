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
using Amazon.Runtime;
using Amazon.S3;
using Amazon;
using Amazon.S3.Model;

namespace OdhApiCore.Controllers.api
{
    [ApiExplorerSettings(IgnoreApi = true)]
    //[Route("api/[controller]")]
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

        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataModify,DataCreate,ODHPoiCreate,ODHPoiModify,ODHPoiManager,CommonCreate,CommonModify,CommonManager,ArticleCreate,ArticleModify,ArticleManager,EventShortManager,EventShortCreate")]
        [HttpPost, Route("api/FileUpload/{type}/{directory}")]
        public async Task<IActionResult> PostFormData(string type, string directory, IFormCollection form)
        {
            var filenames = new List<string>();

            // read from settings
            var keyid = settings.S3ImageresizerConfig.AccessKey;
            var key = settings.S3ImageresizerConfig.SecretKey;
            var bucketName = settings.S3ImageresizerConfig.BucketAccessPoint;

            var creds = new BasicAWSCredentials(keyid, key);
            var config = new AmazonS3Config();
            config.RegionEndpoint = RegionEndpoint.EUWest1;
            var client = new AmazonS3Client(creds, config);

            foreach (var file in form.Files)
            {
                var filename = $"{Guid.NewGuid()}.{Path.GetExtension(file.FileName)}";
                var request = new UploadPartRequest();
                request.BucketName = bucketName;
                request.Key = filename;
                request.InputStream = file.OpenReadStream();
                var response = await client.UploadPartAsync(request);
                filenames.Add(String.Format("{0},{1}", settings.S3ImageresizerConfig.Url, filename));
            }
            return Ok(filenames);
        }

        //TODO have a look here https://weblog.west-wind.com/posts/2017/sep/14/accepting-raw-request-body-content-in-aspnet-core-api-controllers
        //TODO UPload on S3 https://stackoverflow.com/questions/56813075/aws-c-sharp-net-core-how-to-upload-a-jpg-image-to-s3-bucket-without-saving-i
        //TODO https://docs.aws.amazon.com/sdkfornet1/latest/apidocs/html/T_Amazon_S3_Model_PutObjectRequest.htm

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
