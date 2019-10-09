using Npgsql;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    public interface IPostGreSQLConnectionFactory
    {
        public Task<NpgsqlConnection> GetConnection(CancellationToken cancellationToken);
    }
}
