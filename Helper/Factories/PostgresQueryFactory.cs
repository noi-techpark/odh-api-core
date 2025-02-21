// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using Microsoft.Extensions.Logging;
using Npgsql;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace Helper.Factories
{
    class OdhPostgresCompiler : PostgresCompiler
    {
        public OdhPostgresCompiler()
        {
            parameterPlaceholder = "$$";
        }
    }

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
            //https://stackoverflow.com/questions/77694133/postgis-ef-nettopologysuite-exception-writing-derived-type-to-db
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(settings.PostgresConnectionString);
            dataSourceBuilder.UseNetTopologySuite();
            var dataSource = dataSourceBuilder.Build();
            
            Connection = dataSource.OpenConnection(); //new NpgsqlConnection(settings.PostgresConnectionString);
            //trying to get NetTopologySuite to work

            //Connection = new NpgsqlConnection(settings.PostgresConnectionString);

            Compiler = new OdhPostgresCompiler();            
            Logger = info =>
                logger.LogDebug("SQL: {sql} {@parameters}", info.RawSql, info.NamedBindings);
        }

        public new void Dispose()
        {
            base.Dispose();
            if (Connection != null)
            {
                Connection.Dispose();
                Connection = null;
            }
        }
    }
}
