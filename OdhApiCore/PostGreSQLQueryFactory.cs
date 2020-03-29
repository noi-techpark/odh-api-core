using Microsoft.Extensions.Logging;
using Npgsql;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdhApiCore
{
    /// <summary>
    /// For the time of writing QueryFactory doesn't implement IDisposable
    /// so it is a bit dangerous to use with ASP.NET's DI when used directly.
    /// This implementation is safe!<br />
    /// Howewer at the time of writing there exists an issue to fix that:
    /// https://github.com/sqlkata/querybuilder/issues/213
    /// </summary>
    public class PostGreSQLQueryFactory : IDisposable
    {
        private NpgsqlConnection? connection;
        private readonly QueryFactory queryFactory;

        public PostGreSQLQueryFactory(ISettings settings, ILogger<PostGreSQLConnectionFactory> logger)
        {
            connection = new NpgsqlConnection(settings.PostgresConnectionString);
            connection.Disposed += (sender, args) =>
                logger.LogDebug("PostgreSQL: {connectionState}", "Disposed");
            connection.StateChange += (sender, args) =>
                logger.LogDebug("PostgreSQL: {connectionState}", args.CurrentState);
            var compiler = new PostgresCompiler();
            queryFactory = new QueryFactory(connection, compiler)
            {
                Logger = info =>
                    logger.LogDebug("SQL: {sql} {@parameters}", info.RawSql, info.NamedBindings)
            };
        }

        public Query Query() =>
            queryFactory.Query();

        public Query Query(string table) =>
            queryFactory.Query(table);

        public void Dispose()
        {
            if (connection != null)
            {
                connection.Dispose();
                connection = null;
            }
        }
    }
}
