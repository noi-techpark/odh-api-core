using Helper;
using Npgsql;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore
{
    public class PostGreSQLConnectionFactory : IPostGreSQLConnectionFactory
    {
        private readonly string connectionString;

        public PostGreSQLConnectionFactory(ISettings settings)
        {
            this.connectionString = settings.PostgresConnectionString;
        }

        public async Task<NpgsqlConnection> GetConnection(CancellationToken cancellationToken)
        {
            // TODO: additional initialization logic goes here
            var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync(cancellationToken);
            return conn;
        }
    }
}