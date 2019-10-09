using Microsoft.Extensions.Configuration;
using System;

namespace OdhApiCore
{
    public interface ISettings
    {
        string PostgresConnectionString { get; }
        bool CheckCC0License { get; }
    }

    public class Settings : ISettings
    {
        private readonly IConfiguration configuration;
        private readonly Lazy<string> connectionString;
        private readonly Lazy<bool> checkCC0License;

        public Settings(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = new Lazy<string>(() =>
            {
                return this.configuration.GetConnectionString("PGConnection");
            });
            this.checkCC0License = new Lazy<bool>(() =>
            {
                return this.configuration.GetValue("CheckCC0License", false);
            });
        }

        public string PostgresConnectionString => this.connectionString.Value;
        public bool CheckCC0License => this.checkCC0License.Value;
    }
}
