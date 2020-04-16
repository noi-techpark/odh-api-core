using Microsoft.Extensions.Configuration;
using System;

namespace OdhApiCore
{
    public interface ISettings
    {
        string PostgresConnectionString { get; }
        bool CheckCC0License { get; }
        bool FilterClosedData { get; }
    }

    public class Settings : ISettings
    {
        private readonly IConfiguration configuration;
        private readonly Lazy<string> connectionString;
        private readonly Lazy<bool> checkCC0License;
        private readonly Lazy<bool> filterClosedData;

        public Settings(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = new Lazy<string>(() =>
                this.configuration.GetConnectionString("PGConnection"));
            this.checkCC0License = new Lazy<bool>(() =>
                this.configuration.GetValue("CheckCC0License", false));
            this.filterClosedData = new Lazy<bool>(() =>
                this.configuration.GetValue("FilterClosedData", true));
        }

        public string PostgresConnectionString => this.connectionString.Value;
        public bool CheckCC0License => this.checkCC0License.Value;
        public bool FilterClosedData => this.filterClosedData.Value;
    }
}
