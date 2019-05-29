using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace OdhApiCoreTests
{
    public class TestsHelper
    {
        public static IConfigurationRoot GetIConfigurationRoot()
        {
            return new ConfigurationBuilder()
                //.SetBasePath(outputPath)
                //.AddJsonFile("appsettings.json", optional: true)
                //.AddUserSecrets("e3dfcccf-0cb3-423a-b302-e3e92e95c128")
                .AddEnvironmentVariables()
                .Build();
        }

        //public static KavaDocsConfiguration GetApplicationConfiguration(string outputPath)
        //{
        //    var configuration = new KavaDocsConfiguration();

        //    var iConfig = GetIConfigurationRoot(outputPath);

        //    iConfig
        //        .GetSection("KavaDocs")
        //        .Bind(configuration);

        //    return configuration;
        //}
    }
}
