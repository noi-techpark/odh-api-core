using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Helper;

namespace OdhApiImporter
{      
    public interface ISettings
    {
        string PostgresConnectionString { get; }
        MssConfig MssConfig { get; }
        LcsConfig LcsConfig { get; }
        CDBConfig CDBConfig { get; }
        SiagConfig SiagConfig { get; }
        XmlConfig XmlConfig { get; }
        JsonConfig JsonConfig { get; }
        S3ImageresizerConfig S3ImageresizerConfig { get; }
        EBMSConfig EbmsConfig { get; }
        RavenConfig RavenConfig { get; }
        DSSConfig DSSConfig { get; }
        List<NotifierConfig> NotifierConfig { get; }
    }

    public class Settings : ISettings
    {
        private readonly IConfiguration configuration;
        private readonly Lazy<string> connectionString;
        private readonly MssConfig mssConfig;
        private readonly LcsConfig lcsConfig; 
        private readonly CDBConfig cdbConfig;
        private readonly SiagConfig siagConfig;
        private readonly XmlConfig xmlConfig;
        private readonly JsonConfig jsonConfig;
        private readonly S3ImageresizerConfig s3imageresizerConfig;
        private readonly EBMSConfig ebmsConfig;
        private readonly RavenConfig ravenConfig;
        private readonly DSSConfig dssConfig;
        private readonly List<NotifierConfig> notifierConfig;

        public Settings(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = new Lazy<string>(() =>
            this.configuration.GetConnectionString("PGConnection"));
            var mss = this.configuration.GetSection("MssConfig");
            this.mssConfig = new MssConfig(mss.GetValue<string>("Username", ""), mss.GetValue<string>("Password", ""));
            var lcs = this.configuration.GetSection("LcsConfig");
            this.lcsConfig = new LcsConfig(lcs.GetValue<string>("Username", ""), lcs.GetValue<string>("Password", ""), lcs.GetValue<string>("MessagePassword", ""));
            var cdb = this.configuration.GetSection("CDBConfig");
            this.cdbConfig = new CDBConfig(cdb.GetValue<string>("Username", ""), cdb.GetValue<string>("Password", ""), cdb.GetValue<string>("Url", ""));
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
            var dss = this.configuration.GetSection("DSSConfig");
            this.dssConfig = new DSSConfig(dss.GetValue<string>("Username", ""), dss.GetValue<string>("Password", ""), dss.GetValue<string>("ServiceUrl", ""));

            var notifierconfiglist = this.configuration.GetSection("NotifierConfig").GetChildren();
            this.notifierConfig = new List<NotifierConfig>();

            var notifierconfigdict = this.configuration.GetSection("NotifierConfig").GetChildren();
            if (notifierconfigdict != null)
            {
                foreach (var notifiercfg in notifierconfiglist)
                {
                    this.notifierConfig.Add(new NotifierConfig(notifiercfg.Key, notifiercfg.GetValue<string>("Url", ""), notifiercfg.GetValue<string>("User", ""), notifiercfg.GetValue<string>("Password", "")));
                }
            }                
        }

        public string PostgresConnectionString => this.connectionString.Value;

        public MssConfig MssConfig => this.mssConfig;
        public LcsConfig LcsConfig => this.lcsConfig;
        public CDBConfig CDBConfig => this.cdbConfig;
        public SiagConfig SiagConfig => this.siagConfig;
        public XmlConfig XmlConfig => this.xmlConfig;
        public JsonConfig JsonConfig => this.jsonConfig;
        public EBMSConfig EbmsConfig => this.ebmsConfig;
        public S3ImageresizerConfig S3ImageresizerConfig => this.s3imageresizerConfig;
        public RavenConfig RavenConfig => this.ravenConfig;
        public DSSConfig DSSConfig => this.dssConfig;
        public List<NotifierConfig> NotifierConfig => this.notifierConfig;
    }
}
