using Microsoft.Extensions.Logging;
using Npgsql;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;

namespace OdhApiCore.Factories
{
    /// <summary>
    /// For the time of writing QueryFactory doesn't implement IDisposable
    /// so it is a bit dangerous to use with ASP.NET's DI when used directly.
    /// This implementation is safe!<br />
    /// Howewer at the time of writing there exists an issue to fix that:
    /// https://github.com/sqlkata/querybuilder/issues/213
    /// </summary>
    public class PostgresQueryFactory : IDisposable
    {
        private NpgsqlConnection? connection;
        private readonly QueryFactory queryFactory;

        public PostgresQueryFactory(ISettings settings, ILogger<PostgresConnectionFactory> logger)
        {
            connection = new NpgsqlConnection(settings.PostgresConnectionString);
            var compiler = new PostgresCompiler();
            queryFactory = new QueryFactory(connection, compiler)
            {
                Logger = info =>
                    logger.LogDebug("SQL: {sql} {@parameters}", info.RawSql, info.NamedBindings)
            };
        }

        public QueryFactory QueryFactory => queryFactory;

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
