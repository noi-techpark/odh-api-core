using Helper;
using Npgsql;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Factories
{
    public class PostgresConnectionFactory : IPostGreSQLConnectionFactory
    {
        private readonly string connectionString;

        public PostgresConnectionFactory(ISettings settings)
        {
            this.connectionString = settings.PostgresConnectionString;
        }

        public async Task<NpgsqlConnection> GetConnectionAndOpenAsync(CancellationToken cancellationToken)
        {
            // TODO: additional initialization logic goes here
            var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync(cancellationToken);
            return conn;
        }
    }
}