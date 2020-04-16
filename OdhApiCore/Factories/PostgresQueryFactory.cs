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
    public class PostgresQueryFactory : QueryFactory, IDisposable
    {
        public PostgresQueryFactory(ISettings settings, ILogger<QueryFactory> logger)
        {
            Connection = new NpgsqlConnection(settings.PostgresConnectionString);
            Compiler = new PostgresCompiler();
            Logger = info => logger.LogDebug("SQL: {sql} {@parameters}", info.RawSql, info.NamedBindings);
        }

        public void Dispose()
        {
            if (Connection != null)
            {
                Connection.Dispose();
                Connection = null;
            }
        }
    }
}
