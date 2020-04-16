using Npgsql;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    public interface IPostGreSQLConnectionFactory
    {
        [System.Obsolete]
        public Task<NpgsqlConnection> GetConnectionAndOpenAsync(CancellationToken cancellationToken);
    }
}
