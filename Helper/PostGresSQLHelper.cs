using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    public static class PGExtensions
    {
        public static void AddPGParameters(this NpgsqlCommand command, List<PGParameters>? whereparameters)
        {
            if (whereparameters != null)
            {
                foreach (var parameter in whereparameters)
                {
                    switch (parameter.Type)
                    {
                        case NpgsqlTypes.NpgsqlDbType.Date:
                            command.Parameters.AddWithValue(parameter.Name, parameter.Type, Convert.ToDateTime(parameter.Value));
                            break;
                        default:
                            command.Parameters.AddWithValue(parameter.Name, parameter.Type, parameter.Value);
                            break;
                    }
                }
            }
        }
    }

    public class PostgresSQLHelper
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
        public static async Task<List<string>> SelectFromTableDataAsStringAsync(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, CancellationToken cancellationToken)
        {
            try
            {

                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                NpgsqlDataReader dr = (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                List<string> lstSelect = new List<string>();
                while (await dr.ReadAsync())
                {
                    var value = dr[1].ToString();
                    if (value != null)
                        lstSelect.Add(value);
                }

                await dr.CloseAsync();

                await command.DisposeAsync();

                return lstSelect;
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
        public static async Task<string> SelectFromTableDataAsStringSingleAsync(NpgsqlConnection conn, string tablename, string selectexp, string id, CancellationToken cancellationToken)
        {
            try
            {
                var command = new NpgsqlCommand("SELECT " + selectexp + " FROM " + tablename + " WHERE id LIKE @id", conn);
                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, id);

                command.Connection = conn;

                NpgsqlDataReader dr = (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                string result = "";
                while (dr.Read())
                {
                    var value = dr[1].ToString();
                    if (value != null)
                        result = value;
                }
                await dr.CloseAsync();

                await command.DisposeAsync();

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
        public static async Task<List<string>> SelectFromTableDataAsIdAndStringAsync(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, List<string> fieldstoadd, CancellationToken cancellationToken)
        {
            try
            {

                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                NpgsqlDataReader dr = (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);


                List<string> lstSelect = new List<string>();
                while (await dr.ReadAsync(cancellationToken))
                {
                    bool isvalueempty = false;

                    int i = 0;

                    string strtoadd = "{";

                    foreach (string s in fieldstoadd)
                    {
                        strtoadd = strtoadd + "\"" + s + "\":" + dr[i].ToString() + ",";

                        // FIXME: "null"?
                        if (String.IsNullOrEmpty(dr[i].ToString()) || dr[i].ToString() == "null" || dr[i].ToString() == "\"\"")
                            isvalueempty = true;

                        i++;
                    }
                    strtoadd = strtoadd.Remove(strtoadd.Length - 1);
                    strtoadd = strtoadd + "}";

                    if (!isvalueempty)
                    {
                        lstSelect.Add(strtoadd);
                    }
                }

                await dr.CloseAsync();

                await command.DisposeAsync();

                return lstSelect;
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
        public static async Task<List<string>> SelectFromTableDataAsIdAndStringAndTypeAsync(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, List<string> fieldstoadd, string type, CancellationToken cancellationToken)
        {
            try
            {

                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                NpgsqlDataReader dr = (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);


                List<string> lstSelect = new List<string>();
                while (await dr.ReadAsync())
                {
                    string strtoadd = "{";
                    int i = 0;
                    foreach (string s in fieldstoadd)
                    {
                        if (s != "themeIds")
                            strtoadd = strtoadd + "\"" + s + "\":" + dr[i].ToString() + ",";
                        else
                        {
                            var themeids = JsonConvert.DeserializeObject<List<string>>(dr[i].ToString());
                            strtoadd = strtoadd + "\"" + s + "\":\"" + String.Join(",", themeids) + "\",";
                        }
                        i++;
                    }


                    strtoadd = strtoadd + "\"typ\":\"" + type + "\"";

                    strtoadd = strtoadd + "}";

                    lstSelect.Add(strtoadd);
                }

                await dr.CloseAsync();

                await command.DisposeAsync();

                return lstSelect;
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
        public static async Task<List<string>> SelectFromTableDataAsIdAsync(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, CancellationToken cancellationToken)
        {
            try
            {

                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                NpgsqlDataReader dr = (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);


                List<string> lstSelect = new List<string>();
                while (await dr.ReadAsync())
                {
                    var value = dr[0].ToString();
                    if (value != null)
                        lstSelect.Add(value);
                }

                await dr.CloseAsync();

                await command.DisposeAsync();

                return lstSelect;
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
        public static async Task<List<T>> SelectFromTableDataAsObjectAsync<T>(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, CancellationToken cancellationToken)
        {
            try
            {

                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                NpgsqlDataReader dr = (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                List<T> lstSelect = new List<T>();
                while (await dr.ReadAsync())
                {
                    var data = JsonConvert.DeserializeObject<T>(dr[1].ToString());

                    lstSelect.Add(data);
                }

                await dr.CloseAsync();

                await command.DisposeAsync();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Data queried " + lstSelect.Count + " Results");

                return lstSelect;
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
        public static async Task<T> SelectFromTableDataAsObjectSingleAsync<T>(NpgsqlConnection conn, string tablename, string id, CancellationToken cancellationToken)
        {
            try
            {
                //string whereexp = "Id LIKE '" + id + "'";
                //string commandText = CreatetDatabaseCommand("*", tablename, whereexp, "", null, 0);

                var command = new NpgsqlCommand("SELECT * FROM " + tablename + " WHERE id LIKE @id", conn);
                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, id);

                command.Connection = conn;

                NpgsqlDataReader dr = (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                List<T> lstSelect = new List<T>();
                while (await dr.ReadAsync())
                {
                    var data = JsonConvert.DeserializeObject<T>(dr[1].ToString());

                    lstSelect.Add(data);
                }

                await dr.CloseAsync();

                await command.DisposeAsync();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Data queried " + lstSelect.Count + " Results");

                return lstSelect.FirstOrDefault();
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
        public static async Task<List<Tuple<string, T>>> SelectFromTableIdAndDataAsObjectAsync<T>(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, CancellationToken cancellationToken)
        {
            try
            {
                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                NpgsqlDataReader dr = (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                List<Tuple<string, T>> lstSelect = new List<Tuple<string, T>>();
                while (await dr.ReadAsync())
                {
                    var key = dr[0].ToString();
                    if (key != null)
                    {
                        var data = JsonConvert.DeserializeObject<T>(dr[1].ToString());
                        lstSelect.Add(Tuple.Create<string, T>(key, data));
                    }
                }

                await dr.CloseAsync();

                await command.DisposeAsync();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Data queried " + lstSelect.Count + " Results");

                return lstSelect;
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
        public static async Task<List<T>> SelectFromTableDataAsObjectExtendedAsync<T>(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, List<string> fieldstodeserialize, CancellationToken cancellationToken)
        {
            try
            {

                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                NpgsqlDataReader dr = (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                List<T> lstSelect = new List<T>();
                while (await dr.ReadAsync())
                {
                    int i = 0;
                    string stringtodeserialize = "{";
                    foreach (string s in fieldstodeserialize)
                    {
                        stringtodeserialize = stringtodeserialize + "\"" + s + "\":" + dr[i].ToString() + ",";
                        i++;
                    }
                    stringtodeserialize = stringtodeserialize.Remove(stringtodeserialize.Length - 1);
                    stringtodeserialize = stringtodeserialize + "}";

                    var data = JsonConvert.DeserializeObject<T>(stringtodeserialize);

                    lstSelect.Add(data);
                }

                await dr.CloseAsync();

                await command.DisposeAsync();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Data queried " + lstSelect.Count + " Results");

                return lstSelect;
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
        public static async Task<long> CountDataFromTableAsync(NpgsqlConnection conn, string tablename, string whereexp, CancellationToken cancellationToken)
        {
            try
            {

                string commandText = "SELECT COUNT(*) FROM " + tablename;

                if (!String.IsNullOrEmpty(whereexp))
                {
                    commandText = commandText + " WHERE " + whereexp;
                }


                commandText = commandText + ";";

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                Int64 count = (Int64)await (command.ExecuteScalarAsync());

                await command.DisposeAsync();

                return count;
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        #endregion

        #region Generic Database Query Methods (To use on api exposed Methods)

        public static string CreatetDatabaseCommand(string selectexp, string tablename, string whereexp, string sortexp, Nullable<int> offset, int limit)
        {
            string commandText = "SELECT " + selectexp + " FROM " + tablename;

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

            commandText = commandText + ";";

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
        public static async Task<long> CountDataFromTableParametrizedAsync(NpgsqlConnection conn, string tablename, string whereexp, List<PGParameters> whereparameters, CancellationToken cancellationToken)
        {
            try
            {

                string commandText = "SELECT COUNT(*) FROM " + tablename;

                if (!String.IsNullOrEmpty(whereexp))
                {
                    commandText = commandText + " WHERE " + whereexp;
                }

                commandText = commandText + ";";

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                command.AddPGParameters(whereparameters);

                Int64 count = (Int64)await command.ExecuteScalarAsync();

                await command.DisposeAsync();

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
        public static async Task<List<string>> SelectFromTableDataFirstOnlyParametrizedAsync(NpgsqlConnection conn, string tablename, string selectexp, string where, List<PGParameters> whereparameters, string sortexp, int limit, Nullable<int> offset, CancellationToken cancellationToken)
        {
            try
            {
                string commandText = CreatetDatabaseCommand(selectexp, tablename, where, sortexp, offset, limit);
                var command = new NpgsqlCommand(commandText, conn);

                command.AddPGParameters(whereparameters);

                command.Connection = conn;

                NpgsqlDataReader dr = (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                List<string> lstSelect = new List<string>();
                while (await dr.ReadAsync())
                {
                    var value = dr[0].ToString();
                    if (value != null)
                        lstSelect.Add(value);
                }

                await dr.CloseAsync();

                await command.DisposeAsync();

                return lstSelect;
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
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
        public static async Task<List<string>> SelectFromTableDataAsStringParametrizedAsync(NpgsqlConnection conn, string tablename, string selectexp, string where, List<PGParameters>? whereparameters, string sortexp, int limit, Nullable<int> offset, CancellationToken cancellationToken)
        {
            try
            {
                string commandText = CreatetDatabaseCommand(selectexp, tablename, where, sortexp, offset, limit);
                var command = new NpgsqlCommand(commandText, conn);

                command.AddPGParameters(whereparameters);

                command.Connection = conn;

                NpgsqlDataReader dr = (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                List<string> lstSelect = new List<string>();
                while (await dr.ReadAsync())
                {
                    var value = dr[1].ToString();
                    if (value != null)
                        lstSelect.Add(value);
                }

                await dr.CloseAsync();

                await command.DisposeAsync();

                return lstSelect;
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
        public static async Task<List<string>> SelectFromTableDataAsJsonParametrizedAsync(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, List<PGParameters>? whereparameters, string sortexp, int limit, Nullable<int> offset, List<string> fieldstoadd, CancellationToken cancellationToken)
        {
            try
            {
                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                command.AddPGParameters(whereparameters);

                NpgsqlDataReader dr = (NpgsqlDataReader)await command.ExecuteReaderAsync(cancellationToken);

                List<string> lstSelect = new List<string>();
                while (await dr.ReadAsync())
                {
                    bool isvalueempty = false;

                    int i = 0;

                    string strtoadd = "{";

                    foreach (string s in fieldstoadd)
                    {
                        strtoadd = strtoadd + "\"" + s + "\":" + dr[i].ToString() + ",";

                        // FIXME: "null"?
                        if (String.IsNullOrEmpty(dr[i].ToString()) || dr[i].ToString() == "null" || dr[i].ToString() == "\"\"")
                            isvalueempty = true;

                        i++;
                    }
                    strtoadd = strtoadd.Remove(strtoadd.Length - 1);
                    strtoadd = strtoadd + "}";

                    if (!isvalueempty)
                    {
                        lstSelect.Add(strtoadd);
                    }
                }

                await dr.CloseAsync();

                await command.DisposeAsync();

                return lstSelect;
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
        public static async Task<List<T>> SelectFromTableDataAsObjectParametrizedAsync<T>(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, List<PGParameters>? whereparameters, string sortexp, int limit, Nullable<int> offset, CancellationToken cancellationToken)
        {
            try
            {
                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                command.AddPGParameters(whereparameters);

                NpgsqlDataReader dr = (NpgsqlDataReader)await command.ExecuteReaderAsync();

                List<T> lstSelect = new List<T>();
                while (await dr.ReadAsync())
                {
                    var data = JsonConvert.DeserializeObject<T>(dr[1].ToString());

                    lstSelect.Add(data);
                }

                await dr.CloseAsync();

                await command.DisposeAsync();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Data queried " + lstSelect.Count + " Results");

                return lstSelect;
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
        public static async Task<List<T>> SelectFromTableDataAsObjectExtendedParametrizedAsync<T>(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, List<PGParameters> whereparameters, string sortexp, int limit, Nullable<int> offset, List<string> fieldstodeserialize, CancellationToken cancellationToken)
        {
            try
            {

                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                command.AddPGParameters(whereparameters);

                NpgsqlDataReader dr = (NpgsqlDataReader)await command.ExecuteReaderAsync();

                List<T> lstSelect = new List<T>();
                while (await dr.ReadAsync())
                {
                    int i = 0;
                    string stringtodeserialize = "{";
                    foreach (string s in fieldstodeserialize)
                    {
                        stringtodeserialize = stringtodeserialize + "\"" + s + "\":" + dr[i].ToString() + ",";
                        i++;
                    }
                    stringtodeserialize = stringtodeserialize.Remove(stringtodeserialize.Length - 1);
                    stringtodeserialize = stringtodeserialize + "}";

                    var data = JsonConvert.DeserializeObject<T>(stringtodeserialize);

                    lstSelect.Add(data);
                }

                await dr.CloseAsync();

                await command.DisposeAsync();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Data queried " + lstSelect.Count + " Results");

                return lstSelect;
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
        public static async Task<List<T>> SelectFromTableDataAsLocalizedObjectParametrizedAsync<V, T>(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, List<PGParameters> whereparameters, string sortexp, int limit, Nullable<int> offset, string language, Func<V, string, T> transformer, CancellationToken cancellationToken)
        {
            try
            {
                //CultureInfo myculture = new CultureInfo("en");

                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                command.AddPGParameters(whereparameters);

                NpgsqlDataReader dr = (NpgsqlDataReader)await command.ExecuteReaderAsync();

                List<T> lstSelect = new List<T>();
                while (await dr.ReadAsync())
                {
                    var pgdata = JsonConvert.DeserializeObject<V>(dr[1].ToString());

                    var transformeddata = transformer(pgdata, language);

                    lstSelect.Add(transformeddata);
                }

                await dr.CloseAsync();

                await command.DisposeAsync();

                return lstSelect;
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        #endregion       

        #region Generic Insert Method

        public static async Task<string> InsertDataIntoTableAsync(NpgsqlConnection conn, string tablename, string data, string id, CancellationToken cancellationToken)
        {
            try
            {
                ////Fix the single quotes
                //data = data.Replace("'", "''");

                //string commandText = "INSERT INTO " + tablename + "(id, data) VALUES('" + id + "','" + data + "');";

                //var command = new NpgsqlCommand(commandText);
                //command.Connection = conn;

                var command = new NpgsqlCommand("INSERT INTO " + tablename + "(id, data) VALUES(@id,@data)", conn);
                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, id);
                command.Parameters.AddWithValue("data", NpgsqlTypes.NpgsqlDbType.Jsonb, data);

                int affectedrows = await command.ExecuteNonQueryAsync();

                await command.DisposeAsync();

                return affectedrows.ToString();
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        public static async Task<string> InsertDataIntoTableAsync(NpgsqlConnection conn, string tablename, object data, string id, CancellationToken cancellationToken)
        {
            try
            {
                var command = new NpgsqlCommand("INSERT INTO " + tablename + "(id, data) VALUES(@id,@data)", conn);
                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, id);
                command.Parameters.AddWithValue("data", NpgsqlTypes.NpgsqlDbType.Jsonb, JsonConvert.SerializeObject(data));

                int affectedrows = await command.ExecuteNonQueryAsync();

                await command.DisposeAsync();

                return affectedrows.ToString();
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        #endregion

        #region Generic Update Method

        public static async Task<string> UpdateDataFromTable(NpgsqlConnection conn, string tablename, string data, string id, CancellationToken cancellationToken)
        {
            try
            {
                ////Fix the single quotes
                //data = data.Replace("'", "''");                

                //string commandText = "UPDATE " + tablename + " SET data = '" + data + "' WHERE id ='" + id + "';";

                //var command = new NpgsqlCommand(commandText);

                //command.Connection = conn;

                var command = new NpgsqlCommand("UPDATE " + tablename + " SET data = @data WHERE id = @id", conn);
                command.Parameters.AddWithValue("data", NpgsqlTypes.NpgsqlDbType.Jsonb, data);
                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, id);

                int affectedrows = await command.ExecuteNonQueryAsync();

                await command.DisposeAsync();

                return affectedrows.ToString();
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        public static async Task<string> UpdateDataFromTable(NpgsqlConnection conn, string tablename, object data, string id, CancellationToken cancellationToken)
        {
            try
            {
                var command = new NpgsqlCommand("UPDATE " + tablename + " SET data = @data WHERE id = @id", conn);
                command.Parameters.AddWithValue("data", NpgsqlTypes.NpgsqlDbType.Jsonb, JsonConvert.SerializeObject(data));
                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, id);

                int affectedrows = await command.ExecuteNonQueryAsync();

                await command.DisposeAsync();

                return affectedrows.ToString();
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }


        #endregion

        #region Generic Delete Method

        public static async Task<string> DeleteDataFromTableAsync(NpgsqlConnection conn, string tablename, string idvalue, CancellationToken cancellationToken)
        {
            try
            {

                //string commandText = "DELETE FROM " + tablename + " WHERE id = '" + idvalue + "';";

                //var command = new NpgsqlCommand(commandText);
                //command.Connection = conn;

                var command = new NpgsqlCommand("DELETE FROM " + tablename + " WHERE id = @id", conn);
                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, idvalue);

                int affectedrows = await command.ExecuteNonQueryAsync();

                await command.DisposeAsync();

                return affectedrows.ToString();
            }
            catch (DbException ex)
            {
                throw new PostGresSQLHelperException(ex);
            }
        }

        #endregion

        #region Generic Helpers

        public static string GetResultJson(int pagenumber, int totalpages, int totalcount, string? seed, string data)
        {
            string resultstr = "";

            if (data.StartsWith("["))
                resultstr = "{" + "\"TotalResults\":" + totalcount + ",\"TotalPages\":" + totalpages + ",\"CurrentPage\":" + pagenumber + ",\"Seed\":\"" + seed + "\",\"Items\":" + data + "}";
            else
                resultstr = "{" + "\"TotalResults\":" + totalcount + ",\"TotalPages\":" + totalpages + ",\"CurrentPage\":" + pagenumber + ",\"Seed\":\"" + seed + "\",\"Items\":[" + data + "]}";

            return resultstr;
        }

        public static string GetResultJson(int pagenumber, int totalpages, int totalcount, int onlineresults, string? seed, string data)
        {
            string resultstr = "";

            if (data.StartsWith("["))
                resultstr = "{" + "\"TotalResults\":" + totalcount + ",\"TotalPages\":" + totalpages + ",\"CurrentPage\":" + pagenumber + ",\"OnlineResults\":" + onlineresults + ",\"Seed\":\"" + seed + "\",\"Items\":" + data + "}";
            else
                resultstr = "{" + "\"TotalResults\":" + totalcount + ",\"TotalPages\":" + totalpages + ",\"CurrentPage\":" + pagenumber + ",\"OnlineResults\":" + onlineresults + ",\"Seed\":\"" + seed + "\",\"Items\":[" + data + "]}";

            return resultstr;
        }

        public static string GetResultJson(int pagenumber, int totalpages, int totalcount, int onlineresults, string resultid, string? seed, string data)
        {
            string resultstr = "";

            if (data.StartsWith("["))
                resultstr = "{" + "\"TotalResults\":" + totalcount + ",\"TotalPages\":" + totalpages + ",\"CurrentPage\":" + pagenumber + ",\"OnlineResults\":" + onlineresults + ",\"ResultId\":\"" + resultid + "\",\"Seed\":\"" + seed + "\",\"Items\":" + data + "}";
            else
                resultstr = "{" + "\"TotalResults\":" + totalcount + ",\"TotalPages\":" + totalpages + ",\"CurrentPage\":" + pagenumber + ",\"OnlineResults\":" + onlineresults + ",\"ResultId\":\"" + resultid + "\",\"Seed\":\"" + seed + "\",\"Items\":[" + data + "]}";

            return resultstr;
        }

        public static string GetResultJsonLowercase(int pagenumber, int totalpages, int totalcount, int onlineresults, string resultid, string seed, string data)
        {
            string resultstr = "";

            if (data.StartsWith("["))
                resultstr = "{" + "\"totalResults\":" + totalcount + ",\"totalPages\":" + totalpages + ",\"currentPage\":" + pagenumber + ",\"onlineResults\":" + onlineresults + ",\"resultId\":\"" + resultid + "\",\"seed\":\"" + seed + "\",\"items\":" + data + "}";
            else
                resultstr = "{" + "\"totalResults\":" + totalcount + ",\"totalPages\":" + totalpages + ",\"currentPage\":" + pagenumber + ",\"onlineResults\":" + onlineresults + ",\"resultId\":\"" + resultid + "\",\"seed\":\"" + seed + "\",\"items\":[" + data + "]}";

            return resultstr;
        }

        #endregion

        #region Geo Helpers

        //For Activities Pois and Smgpois

        public static string GetGeoWhereSimple(double latitude, double longitude, int radius)
        {
            return "earth_distance(ll_to_earth(" + latitude.ToString(CultureInfo.InvariantCulture) + ", " + longitude.ToString(CultureInfo.InvariantCulture) + "),ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision)) < " + radius.ToString();
        }

        //public static string GetGeoWhereSimple(string latitude, string longitude, string radius)
        //{
        //    return "earth_distance(ll_to_earth(" + latitude + ", " + longitude + "),ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision)) < " + radius;
        //}

        public static string GetGeoOrderBySimple(double latitude, double longitude)
        {
            return "earth_distance(ll_to_earth(" + latitude.ToString(CultureInfo.InvariantCulture) + ", " + longitude.ToString(CultureInfo.InvariantCulture) + "),ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision))";
        }

        //public static string GetGeoOrderBySimple(string latitude, string longitude)
        //{
        //    return "earth_distance(ll_to_earth(" + latitude + ", " + longitude + "),ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision))";
        //}

        public static string GetGeoWhereExtended(double latitude, double longitude, int radius)
        {
            return "earth_distance(ll_to_earth(" + latitude.ToString(CultureInfo.InvariantCulture) + ", " + longitude.ToString(CultureInfo.InvariantCulture) + "),ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision)) < " + radius.ToString();
        }

        //public static string GetGeoWhereExtended(string latitude, string longitude, string radius)
        //{
        //    return "earth_distance(ll_to_earth(" + latitude + ", " + longitude + "),ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision)) < " + radius;
        //}

        public static string GetGeoOrderByExtended(double latitude, double longitude)
        {
            return "earth_distance(ll_to_earth(" + latitude.ToString(CultureInfo.InvariantCulture) + ", " + longitude.ToString(CultureInfo.InvariantCulture) + "),ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision))";
        }

        public static string GetGeoOrderByExtended(string latitude, string longitude)
        {
            return "earth_distance(ll_to_earth(" + latitude + ", " + longitude + "),ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision))";
        }

        public static string GetGeoWhereBoundingBoxes(string latitude, string longitude, string radius)
        {
            return "earth_box(ll_to_earth(" + latitude + ", " + longitude + "), " + radius + ") @> ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision) and earth_distance(ll_to_earth(" + latitude + ", " + longitude + "), ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision)) < " + radius;
        }

        public static string GetGeoWhereBoundingBoxes(double latitude, double longitude, int radius)
        {
            return "earth_box(ll_to_earth(" + latitude.ToString(CultureInfo.InvariantCulture) + ", " + longitude.ToString(CultureInfo.InvariantCulture) + "), " + radius.ToString() + ") @> ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision) and earth_distance(ll_to_earth(" + latitude.ToString(CultureInfo.InvariantCulture) + ", " + longitude.ToString(CultureInfo.InvariantCulture) + "), ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision)) < " + radius.ToString();
        }

        public static string GetGeoWhereBoundingBoxesExtended(string latitude, string longitude, string radius)
        {
            return "earth_box(ll_to_earth(" + latitude + ", " + longitude + "), " + radius + ") @> ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision) and earth_distance(ll_to_earth(" + latitude + ", " + longitude + "), ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision)) < " + radius;
        }

        public static string GetGeoWhereBoundingBoxesExtended(double latitude, double longitude, int radius)
        {
            return "earth_box(ll_to_earth(" + latitude.ToString(CultureInfo.InvariantCulture) + ", " + longitude.ToString(CultureInfo.InvariantCulture) + "), " + radius.ToString() + ") @> ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision) and earth_distance(ll_to_earth(" + latitude.ToString(CultureInfo.InvariantCulture) + ", " + longitude.ToString(CultureInfo.InvariantCulture) + "), ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision)) < " + radius.ToString();
        }

        //For Accommodations
        public static void ApplyGeoSearchWhereOrderbySimple(ref string where, ref string orderby, PGGeoSearchResult geosearchresult)
        {
            if (geosearchresult != null)
            {
                if (geosearchresult.geosearch)
                {
                    if (!String.IsNullOrEmpty(where))
                        where = where + " AND ";

                    where = where + PostgresSQLHelper.GetGeoWhereSimple(geosearchresult.latitude, geosearchresult.longitude, geosearchresult.radius);
                    orderby = PostgresSQLHelper.GetGeoOrderBySimple(geosearchresult.latitude, geosearchresult.longitude);
                }
            }
        }

        //For Activities Pois and GBActivityPoi
        public static void ApplyGeoSearchWhereOrderby(ref string where, ref string orderby, PGGeoSearchResult geosearchresult)
        {
            if (geosearchresult != null)
            {
                if (geosearchresult.geosearch)
                {
                    if (!String.IsNullOrEmpty(where))
                        where = where + " AND ";

                    where = where + PostgresSQLHelper.GetGeoWhereExtended(geosearchresult.latitude, geosearchresult.longitude, geosearchresult.radius);
                    orderby = PostgresSQLHelper.GetGeoOrderByExtended(geosearchresult.latitude, geosearchresult.longitude);
                }
            }
        }

        #endregion

        public static int PGPagingHelper(int totalcount, int pagesize)
        {
            int totalpages = 0;

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
