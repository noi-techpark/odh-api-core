using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OdhApiCore
{
    public class MssConfig
    {
        public MssConfig(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        public string Username { get; private set; }
        public string Password { get; private set;  }
    }

    public class LcsConfig
    {
        public LcsConfig(string username, string password, string messagepassword)
        {
            this.Username = username;
            this.Password = password;
            this.MessagePassword = messagepassword;
        }

        public string Username { get; private set; }
        public string Password { get; private set; }

        public string MessagePassword { get; private set; }
    }

    public class SiagConfig
    {
        public SiagConfig(string username, string password)
        {
            this.Username = username;
            this.Password = password;            
        }

        public string Username { get; private set; }
        public string Password { get; private set; }        
    }

    public class XmlConfig
    {
        public XmlConfig(string xmldir, string xmldirweather)
        {
            this.Xmldir = xmldir;
            this.XmldirWeather = xmldirweather;
        }

        public string Xmldir { get; private set; }
        public string XmldirWeather { get; private set; }        
    }

    public class JsonConfig
    {
        public JsonConfig(string jsondir)
        {
            this.Jsondir = jsondir;            
        }

        public string Jsondir { get; private set; }
    }

    public class S3ImageresizerConfig
    {
        public S3ImageresizerConfig(string url, string docurl, string bucketaccesspoint, string accesskey, string secretkey)
        {
            this.Url = url;
            this.DocUrl = docurl;
            this.BucketAccessPoint = bucketaccesspoint;
            this.AccessKey = accesskey;
            this.SecretKey = secretkey;
        }

        public string Url { get; private set; }
        public string DocUrl { get; private set; }
        public string BucketAccessPoint { get; private set; }
        public string AccessKey { get; private set; }
        public string SecretKey { get; private set; }
    }

    public class EBMSConfig
    {
        public EBMSConfig(string user, string password)
        {
            this.User = user;
            this.Password = password;
        }

        public string User { get; private set; }
        public string Password { get; private set; }
    }

    public class RavenConfig
    {
        public RavenConfig(string user, string password, string serviceurl)
        {
            this.User = user;
            this.Password = password;
            this.ServiceUrl = serviceurl;
        }

        public string User { get; private set; }
        public string Password { get; private set; }
        public string ServiceUrl { get; private set; }
    }

    public class Field2HideConfig
    {
        public Field2HideConfig(string entity, string fields, string validforroles)
        {
            this.Entity = entity;
            this.DisplayOnRoles = validforroles != null ? validforroles.Split(',').ToList() : null;
            this.Fields = fields != null ? fields.Split(',').ToList() : null;
        }

        public string Entity { get; private set; }
        public List<string>? Fields { get; private set; }
        public List<string>? DisplayOnRoles { get; private set; }
    }

    public class RequestInterceptorConfig
    {
        public RequestInterceptorConfig(string action, string controller, string querystrings, string redirectaction, string redirectcontroller, string redirectquerystrings)
        {
            this.Action = action;
            this.Controller = controller;
            this.QueryStrings = querystrings != null ? querystrings.Split('&').ToList() : null;
            this.RedirectAction = redirectaction;
            this.RedirectController = redirectcontroller;
            this.RedirectQueryStrings = redirectquerystrings != null ? redirectquerystrings.Split('&').ToList() : null;
        }

        public string Action { get; private set; }
        public string Controller { get; private set; }
        public List<string>? QueryStrings { get; private set; }
        public string RedirectAction { get; private set; }
        public string RedirectController { get; private set; }
        public List<string>? RedirectQueryStrings { get; private set; }
    }

    public class PushServerConfig
    {
        public PushServerConfig(string user, string password, string serviceurl)
        {
            this.User = user;
            this.Password = password;
            this.ServiceUrl = serviceurl;
        }

        public string User { get; private set; }
        public string Password { get; private set; }
        public string ServiceUrl { get; private set; }
    }

    public class FCMConfig
    {
        public FCMConfig(string serverkey, string senderid)
        {
            this.ServerKey = serverkey;
            this.SenderId = senderid;            
        }

        public string ServerKey { get; private set; }
        public string SenderId { get; private set; }        
    }

    public class RateLimitConfig
    {
        public RateLimitConfig(string type, int timewindow, int maxrequests)
        {
            this.Type = type;
            this.TimeWindow = timewindow;
            this.MaxRequests = maxrequests;
        }

        public string Type { get; private set; }
        public int TimeWindow { get; private set; }
        public int MaxRequests { get; private set; }
    }    

    public interface ISettings
    {
        string PostgresConnectionString { get; }
        MssConfig MssConfig { get; }
        LcsConfig LcsConfig { get; }
        SiagConfig SiagConfig { get; }
        XmlConfig XmlConfig { get; }
        JsonConfig JsonConfig { get; }
        S3ImageresizerConfig S3ImageresizerConfig { get; }
        EBMSConfig EbmsConfig { get; }
        RavenConfig RavenConfig { get; }
        PushServerConfig PushServerConfig { get; }
        FCMConfig FCMConfig { get; }
        List<Field2HideConfig> Field2HideConfig { get; }
        List<RequestInterceptorConfig> RequestInterceptorConfig { get; }
        List<RateLimitConfig> RateLimitConfig { get; }
        List<string>? NoRateLimitRoutesConfig { get; }
    }

    public class Settings : ISettings
    {
        private readonly IConfiguration configuration;
        private readonly Lazy<string> connectionString;
        private readonly MssConfig mssConfig;
        private readonly LcsConfig lcsConfig;
        private readonly SiagConfig siagConfig;
        private readonly XmlConfig xmlConfig;
        private readonly JsonConfig jsonConfig;
        private readonly S3ImageresizerConfig s3imageresizerConfig;
        private readonly EBMSConfig ebmsConfig;
        private readonly RavenConfig ravenConfig;
        private readonly PushServerConfig pushserverConfig;
        private readonly FCMConfig fcmConfig;
        private readonly List<Field2HideConfig> field2hideConfig;
        private readonly List<RequestInterceptorConfig> requestInterceptorConfig;
        private readonly List<RateLimitConfig> rateLimitConfig;
        private readonly List<string>? noRateLimitRoutesConfig;

        public Settings(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = new Lazy<string>(() =>
            this.configuration.GetConnectionString("PGConnection"));
            var mss = this.configuration.GetSection("MssConfig");
            this.mssConfig = new MssConfig(mss.GetValue<string>("Username", ""), mss.GetValue<string>("Password", ""));
            var lcs = this.configuration.GetSection("LcsConfig");
            this.lcsConfig = new LcsConfig(lcs.GetValue<string>("Username", ""), lcs.GetValue<string>("Password", ""), lcs.GetValue<string>("MessagePassword", ""));
            var siag = this.configuration.GetSection("SiagConfig");
            this.siagConfig = new SiagConfig(siag.GetValue<string>("Username", ""), siag.GetValue<string>("Password", ""));
            var xml = this.configuration.GetSection("XmlConfig");
            this.xmlConfig = new XmlConfig(xml.GetValue<string>("Xmldir", ""), xml.GetValue<string>("XmldirWeather", ""));
            var json = this.configuration.GetSection("JsonConfig");
            this.jsonConfig = new JsonConfig(json.GetValue<string>("Jsondir", ""));
            var s3img = this.configuration.GetSection("S3ImageresizerConfig");
            this.s3imageresizerConfig = new S3ImageresizerConfig(s3img.GetValue<string>("Url", ""), s3img.GetValue<string>("DocUrl", ""), s3img.GetValue<string>("BucketAccessPoint", ""), s3img.GetValue<string>("AccessKey", ""), s3img.GetValue<string>("SecretKey", ""));
            var ebms = this.configuration.GetSection("EBMSConfig");
            this.ebmsConfig = new EBMSConfig(ebms.GetValue<string>("EBMSUser", ""), ebms.GetValue<string>("EBMSPassword", ""));
            var raven = this.configuration.GetSection("RavenConfig");
            this.ravenConfig = new RavenConfig(raven.GetValue<string>("Username", ""), raven.GetValue<string>("Password", ""), raven.GetValue<string>("ServiceUrl", ""));
            var pushserver = this.configuration.GetSection("PushServerConfig");
            this.pushserverConfig = new PushServerConfig(pushserver.GetValue<string>("Username", ""), pushserver.GetValue<string>("Password", ""), pushserver.GetValue<string>("ServiceUrl", ""));
            var fcm = this.configuration.GetSection("FCMConfig");
            this.fcmConfig = new FCMConfig(fcm.GetValue<string>("ServerKey", ""), fcm.GetValue<string>("SenderId", ""));
            var field2hidelist = this.configuration.GetSection("Field2HideConfig").GetChildren();
            this.field2hideConfig = new List<Field2HideConfig>();
            foreach (var field2hide in field2hidelist)
            {
                this.field2hideConfig.Add(new Field2HideConfig(field2hide.GetValue<string>("Entity", ""), field2hide.GetValue<string>("Fields", ""), field2hide.GetValue<string>("DisplayOnRoles", "")));
            }

            var requestinterceptorlist = this.configuration.GetSection("RequestInterceptorConfig").GetChildren();
            this.requestInterceptorConfig = new List<RequestInterceptorConfig>();

            foreach (var requestinterceptor in requestinterceptorlist)
            {
                this.requestInterceptorConfig.Add(new RequestInterceptorConfig(requestinterceptor.GetValue<string>("Action", ""), requestinterceptor.GetValue<string>("Controller", ""), requestinterceptor.GetValue<string>("QueryStrings", ""), 
                    requestinterceptor.GetValue<string>("RedirectAction", ""), requestinterceptor.GetValue<string>("RedirectController", ""), requestinterceptor.GetValue<string>("RedirectQueryStrings", "")));
            }

            var ratelimitlist = this.configuration.GetSection("RateLimitConfig").GetChildren();
            this.rateLimitConfig = new List<RateLimitConfig>();
            foreach (var ratelimitconfig in ratelimitlist)
            {
                this.rateLimitConfig.Add(new RateLimitConfig(ratelimitconfig.GetValue<string>("Type", ""), ratelimitconfig.GetValue<int>("TimeWindow", 0), ratelimitconfig.GetValue<int>("MaxRequests", 0)));
            }

            var noratelimitroutes = this.configuration.GetSection("NoRateLimitRoutesConfig").GetChildren();
            this.noRateLimitRoutesConfig = new List<string>();            
            foreach (var routepath in noratelimitroutes)
            {
                this.noRateLimitRoutesConfig.Add(routepath.GetValue<string>("Path",""));                
            }
        }

        public string PostgresConnectionString => this.connectionString.Value;

        public MssConfig MssConfig => this.mssConfig;
        public LcsConfig LcsConfig => this.lcsConfig;
        public SiagConfig SiagConfig => this.siagConfig;
        public XmlConfig XmlConfig => this.xmlConfig;
        public JsonConfig JsonConfig => this.jsonConfig;
        public EBMSConfig EbmsConfig => this.ebmsConfig;
        public S3ImageresizerConfig S3ImageresizerConfig => this.s3imageresizerConfig;
        public RavenConfig RavenConfig => this.ravenConfig;
        public PushServerConfig PushServerConfig => this.pushserverConfig;
        public FCMConfig FCMConfig => this.fcmConfig;
        public List<Field2HideConfig> Field2HideConfig => this.field2hideConfig;
        public List<RequestInterceptorConfig> RequestInterceptorConfig => this.requestInterceptorConfig;
        public List<RateLimitConfig> RateLimitConfig => this.rateLimitConfig;
        public List<string>? NoRateLimitRoutesConfig => this.noRateLimitRoutesConfig;
    }
}
