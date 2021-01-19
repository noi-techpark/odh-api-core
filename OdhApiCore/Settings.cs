using Microsoft.Extensions.Configuration;
using System;

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

    public class S3ImageresizerConfig
    {
        public S3ImageresizerConfig(string url, string bucketaccesspoint, string accesskey, string secretkey)
        {
            this.Url = url;
            this.BucketAccessPoint = bucketaccesspoint;
            this.AccessKey = accesskey;
            this.SecretKey = secretkey;
        }

        public string Url { get; private set; }
        public string BucketAccessPoint { get; private set; }
        public string AccessKey { get; private set; }
        public string SecretKey { get; private set; }
    }

    public interface ISettings
    {
        string PostgresConnectionString { get; }
        MssConfig MssConfig { get; }
        LcsConfig LcsConfig { get; }
        SiagConfig SiagConfig { get; }
        XmlConfig XmlConfig { get; }
        S3ImageresizerConfig S3ImageresizerConfig { get; }
    }

    public class Settings : ISettings
    {
        private readonly IConfiguration configuration;
        private readonly Lazy<string> connectionString;
        private readonly MssConfig mssConfig;
        private readonly LcsConfig lcsConfig;
        private readonly SiagConfig siagConfig;
        private readonly XmlConfig xmlConfig;
        private readonly S3ImageresizerConfig s3imageresizerConfig;

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
            var s3img = this.configuration.GetSection("S3ImageresizerConfig");
            this.s3imageresizerConfig = new S3ImageresizerConfig(s3img.GetValue<string>("Url", ""), s3img.GetValue<string>("BucketAccessPoint", ""), s3img.GetValue<string>("AccessKey", ""), s3img.GetValue<string>("SecretKey", ""));
        }

        public string PostgresConnectionString => this.connectionString.Value;

        public MssConfig MssConfig => this.mssConfig;
        public LcsConfig LcsConfig => this.lcsConfig;
        public SiagConfig SiagConfig => this.siagConfig;
        public XmlConfig XmlConfig => this.xmlConfig;
        public S3ImageresizerConfig S3ImageresizerConfig => this.s3imageresizerConfig;
    }
}
