using Microsoft.Extensions.Configuration;

namespace OdhApiCore
{
    public interface ISettings
    {
        string PostgresConnectionString { get; }
    }

    public class Settings : ISettings
    {
        private readonly IConfiguration configuration;

        public Settings(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string PostgresConnectionString => this.configuration.GetConnectionString("PGConnection");
    }
}
