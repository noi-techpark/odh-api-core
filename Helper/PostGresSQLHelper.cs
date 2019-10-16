using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    public static class PGExtensions
    {
        private static void AddPGParameters(
            this NpgsqlCommand command,
            IEnumerable<PGParameters>? whereparameters)
        {
            if (whereparameters != null)
            {
                foreach (var parameter in whereparameters)
                {
                    switch (parameter.Type)
                    {
                        case NpgsqlTypes.NpgsqlDbType.Date:
                            command.Parameters.AddWithValue(
                                parameter.Name,
                                parameter.Type,
                                Convert.ToDateTime(parameter.Value));
                            break;
                        default:
                            command.Parameters.AddWithValue(
                                parameter.Name,
                                parameter.Type,
                                parameter.Value);
                            break;
                    }
                }
            }
        }

        [System.Diagnostics.Conditional("TRACE")]
        private static void TraceQueryInformation(string cmdText, IEnumerable<PGParameters>? parameters)
        {
            var parametersString =
               (parameters != null) ?
                    $"\n{string.Join("\n", parameters.Select(x => $"\t@{x.Name}: '{x.Value}' [{x.Type}]"))}" :
                    null;
            System.Diagnostics.Trace.TraceInformation($"SQL: {cmdText}{parametersString}");
        }

        public static NpgsqlCommand CreateCommand(this NpgsqlConnection connection, string cmdText, IEnumerable<PGParameters>? parameters = null)
        {
            TraceQueryInformation(cmdText, parameters);
            var command = new NpgsqlCommand(cmdText, connection);
            command.AddPGParameters(parameters);
            return command;
        }
    }

    public static class PostgresSQLHelper
    {
        #region String Select Methods

        /// <summary>
        /// Returns List of JSON Data as STRING (Only Data)
        /// </summary>
        /// <param name="conn">PG Connection</param>
        /// <param name="tablename">Table Name</param>
        /// <param name="whereexp">Where Expression</param>
        /// <param name="sortexp">Sort Expression</param>
        /// <param name="limit">Limit</param>
        /// <param name="offset">Offset</param>
        /// <returns>List of JSON Strings</returns>
        public static IAsyncEnumerable<JsonRaw> SelectFromTableDataAsStringAsync(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, string selectexp, string whereexp,
            string sortexp, uint limit, uint? offset, CancellationToken cancellationToken)
        {
            async IAsyncEnumerable<JsonRaw> inner()
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);
                string commandText = CreateDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                using var command = conn.CreateCommand(commandText);

                using NpgsqlDataReader dr =
                    (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                while (await dr.ReadAsync(cancellationToken))
                {
                    var value = dr[1].ToString();
                    if (value != null)
                        yield return new JsonRaw(value);
                }
            }
            try
            {
                return inner();
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        /// <summary>
        /// Returns JSON Data as STRING (Only Data)
        /// </summary>
        /// <param name="conn">PG Connection</param>
        /// <param name="tablename">Table Name</param>
        /// <param name="selectexp">Sort Expression</param>
        /// <param name="whereexp">Where Expression (if empty set to id LIKE @id)</param>
        /// <param name="parameterdict">String Dictionary with parameters (key, value)</param>
        /// <returns>List of JSON Strings</returns>
        public static async Task<string> SelectFromTableDataAsStringSingleAsync(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, string selectexp, string id,
            CancellationToken cancellationToken)
        {
            try
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);
                using var command = conn.CreateCommand($"SELECT {selectexp} FROM {tablename} WHERE id LIKE @id");
                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, id);

                using NpgsqlDataReader dr =
                    (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                string result = "";
                while (await dr.ReadAsync(cancellationToken))
                {
                    var value = dr[1].ToString();
                    if (value != null)
                        result = value; // FIXME: is this correct?
                }
                return result;
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        /// <summary>
        /// Returns List of JSON Data as Tuple List (Id, String)
        /// </summary>
        /// <param name="conn">PG Connection</param>
        /// <param name="tablename">Table Name</param>
        /// <param name="whereexp">Where Expression</param>
        /// <param name="sortexp">Sort Expression</param>
        /// <param name="limit">Limit</param>
        /// <param name="offset">Offset</param>
        /// <returns>List of JSON Strings</returns>
        public static IAsyncEnumerable<JsonRaw> SelectFromTableDataAsIdAndStringAsync(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, string selectexp, string whereexp,
            string sortexp, uint limit, uint? offset, IEnumerable<string> fieldstoadd,
            CancellationToken cancellationToken)
        {
            async IAsyncEnumerable<JsonRaw> inner()
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);
                string commandText = CreateDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                using var command = conn.CreateCommand(commandText);

                using NpgsqlDataReader dr =
                    (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                while (await dr.ReadAsync(cancellationToken))
                {
                    bool isvalueempty = false;

                    int i = 0;

                    string strtoadd = "{";

                    foreach (string s in fieldstoadd)
                    {
                        strtoadd = $"{strtoadd}\"{s}\":{dr[i].ToString()},";

                        // FIXME: "null"?
                        if (String.IsNullOrEmpty(dr[i].ToString()) || dr[i].ToString() == "null" || dr[i].ToString() == "\"\"")
                            isvalueempty = true;

                        i++;
                    }
                    strtoadd = strtoadd.Remove(strtoadd.Length - 1);
                    strtoadd += "}";

                    if (!isvalueempty)
                    {
                        yield return new JsonRaw(strtoadd);
                    }
                }
            }
            try
            {
                return inner();
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        /// <summary>
        /// Returns List of JSON Data as Tuple List (Id, String)
        /// </summary>
        /// <param name="conn">PG Connection</param>
        /// <param name="tablename">Table Name</param>
        /// <param name="whereexp">Where Expression</param>
        /// <param name="sortexp">Sort Expression</param>
        /// <param name="limit">Limit</param>
        /// <param name="offset">Offset</param>
        /// <returns>List of JSON Strings</returns>
        public static IAsyncEnumerable<JsonRaw> SelectFromTableDataAsIdAndStringAndTypeAsync(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, string selectexp, string whereexp,
            string sortexp, uint limit, uint? offset, IEnumerable<string> fieldstoadd, string type,
            CancellationToken cancellationToken)
        {
            async IAsyncEnumerable<JsonRaw> inner()
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);
                string commandText = CreateDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                using var command = conn.CreateCommand(commandText);

                using NpgsqlDataReader dr =
                    (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                while (await dr.ReadAsync())
                {
                    string strtoadd = "{";
                    int i = 0;
                    foreach (string s in fieldstoadd)
                    {
                        if (s != "themeIds")
                            strtoadd = $"{strtoadd}\"{s}\":{dr[i].ToString()},";
                        else
                        {
                            var themeids = JsonConvert.DeserializeObject<List<string>>(dr[i].ToString() ?? "");
                            strtoadd = $"{strtoadd}\"{s}\":\"{String.Join(",", themeids)}\",";
                        }
                        i++;
                    }

                    strtoadd = $"{strtoadd}\"typ\":\"{type}\"";

                    strtoadd += "}";

                    yield return new JsonRaw(strtoadd);
                }
            }
            try
            {
                return inner();
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        /// <summary>
        /// Returns List of JSON Data as List (Id)
        /// </summary>
        /// <param name="conn">PG Connection</param>
        /// <param name="tablename">Table Name</param>
        /// <param name="whereexp">Where Expression</param>
        /// <param name="sortexp">Sort Expression</param>
        /// <param name="limit">Limit</param>
        /// <param name="offset">Offset</param>
        /// <returns>List of JSON Strings</returns>
        public static IAsyncEnumerable<JsonRaw> SelectFromTableDataAsIdAsync(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, string selectexp, string whereexp,
            string sortexp, uint limit, uint? offset, CancellationToken cancellationToken)
        {
            async IAsyncEnumerable<JsonRaw> inner()
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);
                string commandText = CreateDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                using var command = conn.CreateCommand(commandText);

                using NpgsqlDataReader dr =
                    (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                while (await dr.ReadAsync(cancellationToken))
                {
                    var value = dr[0].ToString();
                    if (value != null)
                        yield return new JsonRaw(value);
                }
            }
            try
            {
                return inner();
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        #endregion

        #region Object Select Methods

        /// <summary>
        /// Returns List of JSON Data mapped to given OBJECT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="tablename"></param>
        /// <param name="whereexp"></param>
        /// <param name="sortexp"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static IAsyncEnumerable<T> SelectFromTableDataAsObjectAsync<T>(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, string selectexp, string whereexp,
            string sortexp, uint limit, uint? offset, CancellationToken cancellationToken)
        {
            async IAsyncEnumerable<T> inner()
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);
                string commandText = CreateDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                using var command = conn.CreateCommand(commandText);

                using NpgsqlDataReader dr =
                    (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                int count = 0;
                while (await dr.ReadAsync(cancellationToken))
                {
                    var data = JsonConvert.DeserializeObject<T>(dr[1].ToString() ?? "");

                    yield return data;

                    count++;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Data queried {count} Results");
            }
            try
            {
                return inner();
            }
            catch (DbException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error :" + ex);

                throw new PostGresSQLHelperException(ex);
            }
        }

        /// <summary>
        /// Returns Single JSON Data mapped to given OBJECT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="tablename"></param>
        /// <param name="whereexp"></param>
        /// <param name="sortexp"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static IAsyncEnumerable<T> SelectFromTableDataAsObjectAsync<T>(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, string id,
            CancellationToken cancellationToken)
        {
            async IAsyncEnumerable<T> inner()
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);
                //string whereexp = "Id LIKE '" + id + "'";
                //string commandText = CreatetDatabaseCommand("*", tablename, whereexp, "", null, 0);

                using var command = conn.CreateCommand($"SELECT * FROM {tablename} WHERE id LIKE @id");
                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, id);

                using NpgsqlDataReader dr =
                    (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                int count = 0;
                while (await dr.ReadAsync(cancellationToken))
                {
                    var data = JsonConvert.DeserializeObject<T>(dr[1].ToString() ?? "");
                    yield return data;
                    count++;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Data queried {count} Results");
            }
            try
            {
                return inner();
            }
            catch (DbException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error :" + ex);

                throw new PostGresSQLHelperException(ex);
            }
        }

        /// <summary>
        /// Return List of Tuple with ID and JSON Data mapped to given Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="tablename"></param>
        /// <param name="whereexp"></param>
        /// <param name="sortexp"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static IAsyncEnumerable<(string, T)> SelectFromTableIdAndDataAsObjectAsync<T>(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, string selectexp, string whereexp,
            string sortexp, uint limit, uint? offset, CancellationToken cancellationToken)
        {
            async IAsyncEnumerable<(string, T)> inner()
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);
                string commandText = CreateDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                using var command = conn.CreateCommand(commandText);

                using NpgsqlDataReader dr =
                    (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                int count = 0;
                while (await dr.ReadAsync(cancellationToken))
                {
                    var key = dr[0].ToString();
                    if (key != null)
                    {
                        var data = JsonConvert.DeserializeObject<T>(dr[1].ToString() ?? "");
                        yield return (key, data);
                        count++;
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Data queried {count} Results");
            }
            try
            {
                return inner();
            }
            catch (DbException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error :" + ex);

                throw new PostGresSQLHelperException(ex);
            }
        }

        /// <summary>
        /// Returns List of JSON Data mapped to given OBJECT by passed Fields to deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="tablename"></param>
        /// <param name="whereexp"></param>
        /// <param name="sortexp"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static IAsyncEnumerable<T> SelectFromTableDataAsObjectExtendedAsync<T>(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, string selectexp, string whereexp,
            string sortexp, uint limit, uint? offset, IEnumerable<string> fieldstodeserialize,
            CancellationToken cancellationToken)
        {
            async IAsyncEnumerable<T> inner()
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);
                string commandText = CreateDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                using var command = conn.CreateCommand(commandText);

                using NpgsqlDataReader dr =
                    (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                int count = 0;
                while (await dr.ReadAsync(cancellationToken))
                {
                    int i = 0;
                    string stringtodeserialize = "{";
                    foreach (string s in fieldstodeserialize)
                    {
                        stringtodeserialize = $"{stringtodeserialize}\"{s}\":{dr[i].ToString()},";
                        i++;
                    }
                    stringtodeserialize = stringtodeserialize.Remove(stringtodeserialize.Length - 1);
                    stringtodeserialize += "}";

                    var data = JsonConvert.DeserializeObject<T>(stringtodeserialize);

                    yield return data;
                    count++;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Data queried {count} Results");
            }
            try
            {
                return inner();
            }
            catch (DbException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error :" + ex);

                throw new PostGresSQLHelperException(ex);
            }
        }

        /// <summary>
        /// Counts the elements of the table
        /// </summary>
        /// <param name="conn">PG Connection</param>
        /// <param name="tablename">Table name</param>
        /// <param name="whereexp">Where Expression</param>
        /// <returns>Elements Count as Long</returns>
        public static async Task<long> CountDataFromTableAsync(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, string whereexp, CancellationToken cancellationToken)
        {
            try
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);
                string commandText = $"SELECT COUNT(*) FROM {tablename}";

                if (!String.IsNullOrEmpty(whereexp))
                {
                    commandText = commandText + " WHERE " + whereexp;
                }
                commandText += ";";

                using var command = conn.CreateCommand(commandText);

                long count = (long)await (command.ExecuteScalarAsync());

                return count;
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        #endregion

        #region Generic Database Query Methods (To use on api exposed Methods)

        public static string CreateDatabaseCommand(
            string selectexp, string tablename, string whereexp, string sortexp, uint? offset, uint limit)
        {
            // Add total count column
            selectexp += "(*)";  //", count(*) OVER() AS full_count";

            string commandText = $"SELECT {selectexp} FROM {tablename}";

            if (!String.IsNullOrEmpty(whereexp))
            {
                commandText = commandText + " WHERE " + whereexp;
            }

            if (!String.IsNullOrEmpty(sortexp))
            {
                commandText = commandText + " ORDER BY " + sortexp;
            }

            if (offset != null)
            {
                commandText = commandText + " OFFSET " + offset;
            }

            if (limit > 0)
            {
                commandText = commandText + " LIMIT " + limit;
            }

            commandText += ";";

            return commandText;
        }

        #endregion

        #region Parametrized Select Methods

        /// <summary>
        /// Counts the elements of the table with Parameters
        /// </summary>
        /// <param name="conn">PG Connection</param>
        /// <param name="tablename">Table name</param>
        /// <param name="whereexp">Where Expression</param>
        /// <returns>Elements Count as Long</returns>
        public static async Task<uint> CountDataFromTableParametrizedAsync(
            IPostGreSQLConnectionFactory connectionFactory, string tablename,
            (string whereexp, IEnumerable<PGParameters> whereparameters) where,
            CancellationToken cancellationToken)
        {
            try
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);
                string commandText = $"SELECT COUNT(*) FROM {tablename}";

                if (!String.IsNullOrEmpty(where.whereexp))
                {
                    commandText = commandText + " WHERE " + where.whereexp;
                }
                commandText += ";";

                using var command = conn.CreateCommand(commandText, where.whereparameters);

                uint count = (uint)(long)await command.ExecuteScalarAsync(cancellationToken);

                return count;
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        /// <summary>
        /// SELECT Id Only as String with Parameters (TO GET ONLY ID)
        /// </summary>
        /// <param name="conn">PG Connection</param>
        /// <param name="tablename">Table Name</param>
        /// <param name="selectexp">Sort Expression</param>
        /// <param name="whereexp">Where Expression (if empty set to id LIKE @id)</param>
        /// <param name="parameterdict">String Dictionary with parameters (key, value)</param>
        /// <returns>List of JSON Strings</returns>
        public static IAsyncEnumerable<JsonRaw> SelectFromTableDataFirstOnlyParametrizedAsync(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, string selectexp,
            (string whereexp, IEnumerable<PGParameters> whereparameters) where,
            string sortexp, uint limit, uint? offset,
            CancellationToken cancellationToken)
        {
            async IAsyncEnumerable<JsonRaw> inner()
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);
                string commandText = CreateDatabaseCommand(selectexp, tablename, where.whereexp, sortexp, offset, limit);
                using var command = conn.CreateCommand(commandText, where.whereparameters);

                using NpgsqlDataReader dr =
                    (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                while (await dr.ReadAsync(cancellationToken))
                {
                    var value = dr[0].ToString();
                    if (value != null)
                        yield return new JsonRaw(value);
                }
            }
            try
            {
                return inner();
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        private static async Task<(long, IEnumerable<JsonRaw>)> TransformSQLResult(IAsyncEnumerable<(long, JsonRaw)> sqlResult, CancellationToken cancellationToken)
        {
            var list = await sqlResult.ToListAsync(cancellationToken);
            return list.Aggregate(
                (0L, new List<JsonRaw>(capacity: list.Count)),
                (xs, x) =>
                {
                    var (_, jsons) = xs;
                    var (totalCount, json) = x;
                    jsons.Add(json);
                    return (totalCount, jsons);
                });
        }

        /// <summary>
        /// SELECT Json Data as String with Parameters (TO GET ONLY data Element)
        /// </summary>
        /// <param name="conn">PG Connection</param>
        /// <param name="tablename">Table Name</param>
        /// <param name="selectexp">Sort Expression</param>
        /// <param name="whereexp">Where Expression (if empty set to id LIKE @id)</param>
        /// <param name="parameterdict">String Dictionary with parameters (key, value)</param>
        /// <returns>List of JSON Strings</returns>
        public static Task<(long, IEnumerable<JsonRaw>)> SelectFromTableDataAsStringParametrizedAsync(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, string selectexp,
            (string whereexpression, IEnumerable<PGParameters>? whereparameters) where,
            string sortexp, uint limit, uint? offset, CancellationToken cancellationToken)
        {
            async IAsyncEnumerable<(long, JsonRaw)> inner()
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);
                string commandText = CreateDatabaseCommand(selectexp, tablename, where.whereexpression, sortexp, offset, limit);
                using var command = conn.CreateCommand(commandText, where.whereparameters);

                using NpgsqlDataReader dr =
                    (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                while (await dr.ReadAsync(cancellationToken))
                {
                    long totalCount = (long)limit; //(long)dr["full_count"];
                    string value = (string)dr["data"];
                    if (value != null)
                        yield return (totalCount, new JsonRaw(value));
                }
            }
            try
            {
                return TransformSQLResult(inner(), cancellationToken);
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        /// <summary>
        /// Select Reduced Json (Id, String) with Parameters  (TO GET Jsons with passed Fields Ex. REDUCED LISTS)
        /// </summary>
        /// <param name="conn">PG Connection</param>
        /// <param name="tablename">Table Name</param>
        /// <param name="whereexp">Where Expression</param>
        /// <param name="sortexp">Sort Expression</param>
        /// <param name="limit">Limit</param>
        /// <param name="offset">Offset</param>
        /// <returns>List of JSON Strings</returns>
        public static IAsyncEnumerable<JsonRaw> SelectFromTableDataAsJsonParametrizedAsync(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, string selectexp,
            (string whereexp, IEnumerable<PGParameters>? whereparameters) where,
            string sortexp, uint limit, uint? offset, IEnumerable<string> fieldstoadd,
            CancellationToken cancellationToken)
        {
            async IAsyncEnumerable<JsonRaw> inner()
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);
                string commandText = CreateDatabaseCommand(selectexp, tablename, where.whereexp, sortexp, offset, limit);

                using var command = conn.CreateCommand(commandText, where.whereparameters);

                using NpgsqlDataReader dr =
                    (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                while (await dr.ReadAsync(cancellationToken))
                {
                    bool isvalueempty = false;

                    int i = 0;

                    string strtoadd = "{";

                    foreach (string s in fieldstoadd)
                    {
                        strtoadd = $"{strtoadd}\"{s}\":{dr[i].ToString()},";

                        // FIXME: "null"?
                        if (String.IsNullOrEmpty(dr[i].ToString()) || dr[i].ToString() == "null" || dr[i].ToString() == "\"\"")
                            isvalueempty = true;

                        i++;
                    }
                    strtoadd = strtoadd.Remove(strtoadd.Length - 1);
                    strtoadd += "}";

                    if (!isvalueempty)
                    {
                        yield return new JsonRaw(strtoadd);
                    }
                }
            }
            try
            {
                return inner();
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        /// <summary>
        /// Select Json Data Mapped to Object with Parameters (TO GET data Element Deserialized as OBJECT)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="tablename"></param>
        /// <param name="whereexp"></param>
        /// <param name="sortexp"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static IAsyncEnumerable<T> SelectFromTableDataAsObjectParametrizedAsync<T>(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, string selectexp,
            (string whereexp, IEnumerable<PGParameters>? whereparameters) where,
            string sortexp, uint limit, uint? offset, CancellationToken cancellationToken)
        {
            async IAsyncEnumerable<T> inner()
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);
                string commandText = CreateDatabaseCommand(
                    selectexp,
                    tablename,
                    where.whereexp,
                    sortexp,
                    offset,
                    limit);

                using var command = conn.CreateCommand(commandText, where.whereparameters);

                using NpgsqlDataReader dr =
                    (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                int count = 0;
                while (await dr.ReadAsync(cancellationToken))
                {
                    var data = JsonConvert.DeserializeObject<T>(dr[1].ToString() ?? "");
                    yield return data;
                    count++;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Data queried {count} Results");
            }
            try
            {
                return inner();
            }
            catch (DbException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error :" + ex);

                throw new PostGresSQLHelperException(ex);
            }
        }

        /// <summary>
        /// Returns List of JSON Data mapped to given OBJECT by passed Fields to deserialize (TO GET OBJECT with passed Fields )
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="tablename"></param>
        /// <param name="whereexp"></param>
        /// <param name="sortexp"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static IAsyncEnumerable<T> SelectFromTableDataAsObjectExtendedParametrizedAsync<T>(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, string selectexp,
            (string whereexp, IEnumerable<PGParameters> whereparameters) where,
            string sortexp, uint limit, uint? offset, IEnumerable<string> fieldstodeserialize,
            CancellationToken cancellationToken)
        {
            async IAsyncEnumerable<T> inner()
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);
                string commandText = CreateDatabaseCommand(
                    selectexp,
                    tablename,
                    where.whereexp,
                    sortexp,
                    offset,
                    limit);

                using var command = conn.CreateCommand(commandText, where.whereparameters);

                using NpgsqlDataReader dr =
                    (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                int count = 0;
                while (await dr.ReadAsync(cancellationToken))
                {
                    int i = 0;
                    string stringtodeserialize = "{";
                    foreach (string s in fieldstodeserialize)
                    {
                        stringtodeserialize = $"{stringtodeserialize}\"{s}\":{dr[i].ToString()},";
                        i++;
                    }
                    stringtodeserialize = stringtodeserialize.Remove(stringtodeserialize.Length - 1);
                    stringtodeserialize += "}";

                    var data = JsonConvert.DeserializeObject<T>(stringtodeserialize);

                    yield return data;

                    count++;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Data queried {count} Results");
            }
            try
            {
                return inner();
            }
            catch (DbException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error :" + ex);

                throw new PostGresSQLHelperException(ex);
            }
        }

        /// <summary>
        /// Select Json Data Map it to Object and Transform it with passed Function with Parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="tablename"></param>
        /// <param name="whereexp"></param>
        /// <param name="sortexp"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static IAsyncEnumerable<T> SelectFromTableDataAsLocalizedObjectParametrizedAsync<V, T>(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, string selectexp,
            (string whereexp, IEnumerable<PGParameters> whereparameters) where,
            string sortexp, uint limit, uint? offset, string language,
            Func<V, string, T> transformer, CancellationToken cancellationToken)
        {
            async IAsyncEnumerable<T> inner()
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);
                //CultureInfo myculture = new CultureInfo("en");

                string commandText = CreateDatabaseCommand(
                    selectexp,
                    tablename,
                    where.whereexp,
                    sortexp,
                    offset,
                    limit);

                using var command = conn.CreateCommand(commandText, where.whereparameters);

                using NpgsqlDataReader dr =
                    (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                while (await dr.ReadAsync(cancellationToken))
                {
                    var pgdata = JsonConvert.DeserializeObject<V>(dr[1].ToString() ?? "");

                    var transformeddata = transformer(pgdata, language);

                    yield return transformeddata;
                }
            }
            try
            {
                return inner();
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        #endregion

        #region Generic Insert Method

        public static async Task<string> InsertDataIntoTableAsync(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, string data, string id,
            CancellationToken cancellationToken)
        {
            try
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);
                ////Fix the single quotes
                //data = data.Replace("'", "''");

                //string commandText = "INSERT INTO " + tablename + "(id, data) VALUES('" + id + "','" + data + "');";

                //var command = new NpgsqlCommand(commandText);
                //command.Connection = conn;

                using var command = conn.CreateCommand($"INSERT INTO {tablename} (id, data) VALUES (@id, @data)");
                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, id);
                command.Parameters.AddWithValue("data", NpgsqlTypes.NpgsqlDbType.Jsonb, data);

                int affectedrows =
                    await command.ExecuteNonQueryAsync(cancellationToken);

                return affectedrows.ToString();
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        public static async Task<string> InsertDataIntoTableAsync(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, object data,
            string id, CancellationToken cancellationToken)
        {
            try
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);
                using var command = conn.CreateCommand($"INSERT INTO {tablename} (id, data) VALUES (@id, @data)");
                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, id);
                command.Parameters.AddWithValue("data", NpgsqlTypes.NpgsqlDbType.Jsonb, JsonConvert.SerializeObject(data));

                int affectedrows = await command.ExecuteNonQueryAsync(cancellationToken);

                return affectedrows.ToString();
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        #endregion

        #region Generic Update Method

        public static async Task<string> UpdateDataFromTable(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, string data, string id, CancellationToken cancellationToken)
        {
            try
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);

                ////Fix the single quotes
                //data = data.Replace("'", "''");

                //string commandText = "UPDATE " + tablename + " SET data = '" + data + "' WHERE id ='" + id + "';";

                //var command = new NpgsqlCommand(commandText);

                //command.Connection = conn;

                using var command = conn.CreateCommand($"UPDATE {tablename} SET data = @data WHERE id = @id");
                command.Parameters.AddWithValue("data", NpgsqlTypes.NpgsqlDbType.Jsonb, data);
                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, id);

                int affectedrows = await command.ExecuteNonQueryAsync(cancellationToken);

                return affectedrows.ToString();
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        public static async Task<string> UpdateDataFromTable(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, object data, string id, CancellationToken cancellationToken)
        {
            try
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);

                using var command = conn.CreateCommand($"UPDATE {tablename} SET data = @data WHERE id = @id");
                command.Parameters.AddWithValue("data", NpgsqlTypes.NpgsqlDbType.Jsonb, JsonConvert.SerializeObject(data));
                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, id);

                int affectedrows = await command.ExecuteNonQueryAsync(cancellationToken);

                return affectedrows.ToString();
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }


        #endregion

        #region Generic Delete Method

        public static async Task<string> DeleteDataFromTableAsync(
            IPostGreSQLConnectionFactory connectionFactory, string tablename, string idvalue, CancellationToken cancellationToken)
        {
            try
            {
                using var conn = await connectionFactory.GetConnection(cancellationToken);

                //string commandText = "DELETE FROM " + tablename + " WHERE id = '" + idvalue + "';";

                //var command = new NpgsqlCommand(commandText);
                //command.Connection = conn;

                using var command = conn.CreateCommand($"DELETE FROM {tablename} WHERE id = @id");
                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, idvalue);

                int affectedrows = await command.ExecuteNonQueryAsync(cancellationToken);

                return affectedrows.ToString();
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        #endregion

        #region Generic Helpers

        public static string GetResultJson<T>(
            uint pagenumber, uint totalpages, uint totalcount, string? seed, IEnumerable<T> data)
            where T : notnull
        {
            return JsonConvert.SerializeObject(new
            {
                TotalResults = totalcount,
                TotalPages = totalpages,
                CurrentPage = pagenumber,
                Seed = seed,
                Items = data
            });
        }

        public static string GetResultJson<T>(
            uint pagenumber, uint totalpages, uint totalcount, int onlineresults, string? seed,
            IEnumerable<T> data)
            where T : notnull
        {
            return JsonConvert.SerializeObject(new
            {
                TotalResults = totalcount,
                TotalPages = totalpages,
                CurrentPage = pagenumber,
                OnlineResults = onlineresults,
                Seed = seed,
                Items = data
            });
        }

        public static string GetResultJson<T>(
            uint pagenumber, uint totalpages, uint totalcount, int onlineresults,
            string resultid, string? seed, IEnumerable<T> data)
            where T : notnull
        {
            return JsonConvert.SerializeObject(new
            {
                TotalResults = totalcount,
                TotalPages = totalpages,
                CurrentPage = pagenumber,
                OnlineResults = onlineresults,
                ResultId = resultid,
                Seed = seed,
                Items = data
            });
        }

        public static string GetResultJsonLowercase<T>(
            uint pagenumber, uint totalpages, uint totalcount, int onlineresults,
            string resultid, string seed, IEnumerable<T> data)
            where T : notnull
        {
            return JsonConvert.SerializeObject(new
            {
                totalResults = totalcount,
                totalPages = totalpages,
                currentPage = pagenumber,
                onlineResults = onlineresults,
                resultId = resultid,
                seed,
                items = data
            });
        }

        #endregion

        #region Geo Helpers

        //For Activities Pois and Smgpois

        public static string GetGeoWhereSimple(double latitude, double longitude, int radius)
        {
            return $"earth_distance(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}),ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision)) < {radius.ToString()}";
        }

        //public static string GetGeoWhereSimple(string latitude, string longitude, string radius)
        //{
        //    return "earth_distance(ll_to_earth(" + latitude + ", " + longitude + "),ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision)) < " + radius;
        //}

        public static string GetGeoOrderBySimple(double latitude, double longitude)
        {
            return $"earth_distance(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}),ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision))";
        }

        //public static string GetGeoOrderBySimple(string latitude, string longitude)
        //{
        //    return "earth_distance(ll_to_earth(" + latitude + ", " + longitude + "),ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision))";
        //}

        public static string GetGeoWhereExtended(double latitude, double longitude, int radius)
        {
            return $"earth_distance(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}),ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision)) < {radius.ToString()}";
        }

        //public static string GetGeoWhereExtended(string latitude, string longitude, string radius)
        //{
        //    return "earth_distance(ll_to_earth(" + latitude + ", " + longitude + "),ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision)) < " + radius;
        //}

        public static string GetGeoOrderByExtended(double latitude, double longitude)
        {
            return $"earth_distance(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}),ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision))";
        }

        public static string GetGeoOrderByExtended(string latitude, string longitude)
        {
            return $"earth_distance(ll_to_earth({latitude}, {longitude}),ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision))";
        }

        public static string GetGeoWhereBoundingBoxes(string latitude, string longitude, string radius)
        {
            return $"earth_box(ll_to_earth({latitude}, {longitude}), {radius}) @> ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision) and earth_distance(ll_to_earth({latitude}, {longitude}), ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision)) < {radius}";
        }

        public static string GetGeoWhereBoundingBoxes(double latitude, double longitude, int radius)
        {
            return $"earth_box(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}), {radius.ToString()}) @> ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision) and earth_distance(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}), ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision)) < {radius.ToString()}";
        }

        public static string GetGeoWhereBoundingBoxesExtended(string latitude, string longitude, string radius)
        {
            return $"earth_box(ll_to_earth({latitude}, {longitude}), {radius}) @> ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision) and earth_distance(ll_to_earth({latitude}, {longitude}), ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision)) < {radius}";
        }

        public static string GetGeoWhereBoundingBoxesExtended(double latitude, double longitude, int radius)
        {
            return $"earth_box(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}), {radius.ToString()}) @> ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision) and earth_distance(ll_to_earth({latitude.ToString(CultureInfo.InvariantCulture)}, {longitude.ToString(CultureInfo.InvariantCulture)}), ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision)) < {radius.ToString()}";
        }

        //For Accommodations
        public static void ApplyGeoSearchWhereOrderbySimple(
            ref string where, ref string orderby, PGGeoSearchResult geosearchresult)
        {
            if (geosearchresult != null)
            {
                if (geosearchresult.geosearch)
                {
                    if (!String.IsNullOrEmpty(where))
                        where += " AND ";

                    where += PostgresSQLHelper.GetGeoWhereSimple(
                        geosearchresult.latitude,
                        geosearchresult.longitude,
                        geosearchresult.radius);
                    orderby = PostgresSQLHelper.GetGeoOrderBySimple(
                        geosearchresult.latitude,
                        geosearchresult.longitude);
                }
            }
        }

        //For Activities Pois and GBActivityPoi
        public static void ApplyGeoSearchWhereOrderby(
            ref string where,
            ref string orderby,
            PGGeoSearchResult geosearchresult)
        {
            if (geosearchresult != null)
            {
                if (geosearchresult.geosearch)
                {
                    if (!String.IsNullOrEmpty(where))
                        where += " AND ";

                    where += PostgresSQLHelper.GetGeoWhereExtended(
                        geosearchresult.latitude,
                        geosearchresult.longitude,
                        geosearchresult.radius);
                    orderby = PostgresSQLHelper.GetGeoOrderByExtended(
                        geosearchresult.latitude,
                        geosearchresult.longitude);
                }
            }
        }

        #endregion

        public static uint PGPagingHelper(uint totalcount, uint pagesize)
        {
            uint totalpages;
            if (totalcount % pagesize == 0)
                totalpages = totalcount / pagesize;
            else
                totalpages = (totalcount / pagesize) + 1;

            return totalpages;
        }

    }

    public class PGParameters
    {
        public string? Name { get; set; }
        public NpgsqlTypes.NpgsqlDbType Type { get; set; }
        public string? Value { get; set; }
    }
}
