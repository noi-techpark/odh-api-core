using Microsoft.Extensions.Configuration;
using System;

namespace OdhApiCore
{
    public interface ISettings
    {
        string PostgresConnectionString { get; }
    }

    public class Settings : ISettings
    {
        private readonly IConfiguration configuration;
        private readonly Lazy<string> connectionString;

        public Settings(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = new Lazy<string>(() =>
                this.configuration.GetConnectionString("PGConnection"));
        }

        public string PostgresConnectionString => this.connectionString.Value;
    }
}
