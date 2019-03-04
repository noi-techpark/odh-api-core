using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Helper
{
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
        public static List<string> SelectFromTableDataAsString(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset)
        {
            try
            {

                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                NpgsqlDataReader dr = command.ExecuteReader();

                List<string> lstSelect = new List<string>();
                while (dr.Read())
                {
                    lstSelect.Add(dr[1].ToString());
                }

                dr.Close();

                return lstSelect;
            }
            catch (Exception ex)
            {
                return null;
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
        public static string SelectFromTableDataAsStringSingle(NpgsqlConnection conn, string tablename, string selectexp, string id)
        {
            try
            {
                var command = new NpgsqlCommand("SELECT " + selectexp + " FROM " + tablename + " WHERE id LIKE @id", conn);
                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, id);

                command.Connection = conn;

                NpgsqlDataReader dr = command.ExecuteReader();

                string result = "";
                while (dr.Read())
                {
                    result = dr[1].ToString();
                }
                dr.Close();

                return result;
            }
            catch (Exception ex)
            {
                return null;
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
        public static List<string> SelectFromTableDataAsIdAndString(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, List<string> fieldstoadd)
        {
            try
            {

                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                NpgsqlDataReader dr = command.ExecuteReader();


                List<string> lstSelect = new List<string>();
                while (dr.Read())
                {
                    bool isvalueempty = false;

                    int i = 0;

                    string strtoadd = "{";

                    foreach (string s in fieldstoadd)
                    {
                        strtoadd = strtoadd + "\"" + s + "\":" + dr[i].ToString() + ",";

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

                dr.Close();


                return lstSelect;
            }
            catch (Exception ex)
            {
                return null;
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
        public static List<string> SelectFromTableDataAsIdAndStringAndType(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, List<string> fieldstoadd, string type)
        {
            try
            {

                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                NpgsqlDataReader dr = command.ExecuteReader();


                List<string> lstSelect = new List<string>();
                while (dr.Read())
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

                dr.Close();


                return lstSelect;
            }
            catch (Exception ex)
            {
                return null;
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
        public static List<string> SelectFromTableDataAsId(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset)
        {
            try
            {

                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                NpgsqlDataReader dr = command.ExecuteReader();


                List<string> lstSelect = new List<string>();
                while (dr.Read())
                {
                    lstSelect.Add(dr[0].ToString());
                }

                dr.Close();


                return lstSelect;
            }
            catch (Exception ex)
            {
                return null;
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
        public static List<T> SelectFromTableDataAsObject<T>(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset)
        {
            try
            {

                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                NpgsqlDataReader dr = command.ExecuteReader();

                List<T> lstSelect = new List<T>();
                while (dr.Read())
                {
                    var data = JsonConvert.DeserializeObject<T>(dr[1].ToString());

                    lstSelect.Add(data);
                }

                dr.Close();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Data queried " + lstSelect.Count + " Results");

                return lstSelect;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error :" + ex);

                return null;
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
        public static T SelectFromTableDataAsObjectSingle<T>(NpgsqlConnection conn, string tablename, string id)
        {
            try
            {
                //string whereexp = "Id LIKE '" + id + "'";
                //string commandText = CreatetDatabaseCommand("*", tablename, whereexp, "", null, 0);

                var command = new NpgsqlCommand("SELECT * FROM " + tablename + " WHERE id LIKE @id", conn);
                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, id);

                command.Connection = conn;

                NpgsqlDataReader dr = command.ExecuteReader();

                List<T> lstSelect = new List<T>();
                while (dr.Read())
                {
                    var data = JsonConvert.DeserializeObject<T>(dr[1].ToString());

                    lstSelect.Add(data);
                }

                dr.Close();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Data queried " + lstSelect.Count + " Results");

                return lstSelect.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error :" + ex);

                return default(T);
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
        public static List<Tuple<string, T>> SelectFromTableIdAndDataAsObject<T>(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset)
        {
            try
            {
                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                NpgsqlDataReader dr = command.ExecuteReader();

                List<Tuple<string, T>> lstSelect = new List<Tuple<string, T>>();
                while (dr.Read())
                {
                    var data = JsonConvert.DeserializeObject<T>(dr[1].ToString());

                    lstSelect.Add(Tuple.Create<string, T>(dr[0].ToString(), data));
                }

                dr.Close();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Data queried " + lstSelect.Count + " Results");

                return lstSelect;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error :" + ex);

                return null;
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
        public static List<T> SelectFromTableDataAsObjectExtended<T>(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, List<string> fieldstodeserialize)
        {
            try
            {

                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                NpgsqlDataReader dr = command.ExecuteReader();

                List<T> lstSelect = new List<T>();
                while (dr.Read())
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

                dr.Close();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Data queried " + lstSelect.Count + " Results");

                return lstSelect;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error :" + ex);

                return null;
            }
        }

        /// <summary>
        /// Counts the elements of the table
        /// </summary>
        /// <param name="conn">PG Connection</param>
        /// <param name="tablename">Table name</param>
        /// <param name="whereexp">Where Expression</param>
        /// <returns>Elements Count as Long</returns>
        public static long CountDataFromTable(NpgsqlConnection conn, string tablename, string whereexp)
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

                Int64 count = (Int64)command.ExecuteScalar();

                return count;
            }
            catch (Exception ex)
            {
                return 0;
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
        public static long CountDataFromTableParametrized(NpgsqlConnection conn, string tablename, string whereexp, List<PGParameters> whereparameters)
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

                if (whereparameters != null)
                {
                    foreach (var parameter in whereparameters)
                    {
                        command.Parameters.AddWithValue(parameter.Name, parameter.Type, parameter.Value);
                    }
                }

                Int64 count = (Int64)command.ExecuteScalar();

                return count;
            }
            catch (Exception ex)
            {
                return 0;
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
        public static List<string> SelectFromTableDataFirstOnlyParametrized(NpgsqlConnection conn, string tablename, string selectexp, string where, List<PGParameters> whereparameters, string sortexp, int limit, Nullable<int> offset)
        {
            try
            {
                string commandText = CreatetDatabaseCommand(selectexp, tablename, where, sortexp, offset, limit);
                var command = new NpgsqlCommand(commandText, conn);

                if (whereparameters != null)
                {
                    foreach (var parameter in whereparameters)
                    {
                        command.Parameters.AddWithValue(parameter.Name, parameter.Type, parameter.Value);
                    }
                }

                command.Connection = conn;

                NpgsqlDataReader dr = command.ExecuteReader();

                List<string> lstSelect = new List<string>();
                while (dr.Read())
                {
                    lstSelect.Add(dr[0].ToString());
                }
                dr.Close();

                return lstSelect;
            }
            catch (Exception ex)
            {
                return null;
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
        public static List<string> SelectFromTableDataAsStringParametrized(NpgsqlConnection conn, string tablename, string selectexp, string where, List<PGParameters> whereparameters, string sortexp, int limit, Nullable<int> offset)
        {
            try
            {
                string commandText = CreatetDatabaseCommand(selectexp, tablename, where, sortexp, offset, limit);
                var command = new NpgsqlCommand(commandText, conn);

                if (whereparameters != null)
                {
                    foreach (var parameter in whereparameters)
                    {
                        command.Parameters.AddWithValue(parameter.Name, parameter.Type, parameter.Value);
                    }
                }

                command.Connection = conn;

                NpgsqlDataReader dr = command.ExecuteReader();

                List<string> lstSelect = new List<string>();
                while (dr.Read())
                {
                    lstSelect.Add(dr[1].ToString());
                }
                dr.Close();

                return lstSelect;
            }
            catch (Exception ex)
            {
                return null;
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
        public static List<string> SelectFromTableDataAsJsonParametrized(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, List<PGParameters> whereparameters, string sortexp, int limit, Nullable<int> offset, List<string> fieldstoadd)
        {
            try
            {
                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                if (whereparameters != null)
                {
                    foreach (var parameter in whereparameters)
                    {
                        command.Parameters.AddWithValue(parameter.Name, parameter.Type, parameter.Value);
                    }
                }

                NpgsqlDataReader dr = command.ExecuteReader();


                List<string> lstSelect = new List<string>();
                while (dr.Read())
                {
                    bool isvalueempty = false;

                    int i = 0;

                    string strtoadd = "{";

                    foreach (string s in fieldstoadd)
                    {
                        strtoadd = strtoadd + "\"" + s + "\":" + dr[i].ToString() + ",";

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

                dr.Close();


                return lstSelect;
            }
            catch (Exception ex)
            {
                return null;
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
        public static List<T> SelectFromTableDataAsObjectParametrized<T>(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, List<PGParameters> whereparameters, string sortexp, int limit, Nullable<int> offset)
        {
            try
            {
                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                if (whereparameters != null)
                {
                    foreach (var parameter in whereparameters)
                    {
                        command.Parameters.AddWithValue(parameter.Name, parameter.Type, parameter.Value);
                    }
                }

                NpgsqlDataReader dr = command.ExecuteReader();

                List<T> lstSelect = new List<T>();
                while (dr.Read())
                {
                    var data = JsonConvert.DeserializeObject<T>(dr[1].ToString());

                    lstSelect.Add(data);
                }

                dr.Close();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Data queried " + lstSelect.Count + " Results");

                return lstSelect;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error :" + ex);

                return null;
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
        public static List<T> SelectFromTableDataAsObjectExtendedParametrized<T>(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, List<PGParameters> whereparameters, string sortexp, int limit, Nullable<int> offset, List<string> fieldstodeserialize)
        {
            try
            {

                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                if (whereparameters != null)
                {
                    foreach (var parameter in whereparameters)
                    {
                        command.Parameters.AddWithValue(parameter.Name, parameter.Type, parameter.Value);
                    }
                }

                NpgsqlDataReader dr = command.ExecuteReader();

                List<T> lstSelect = new List<T>();
                while (dr.Read())
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

                dr.Close();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Data queried " + lstSelect.Count + " Results");

                return lstSelect;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error :" + ex);

                return null;
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
        public static List<T> SelectFromTableDataAsLocalizedObjectParametrized<V, T>(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, List<PGParameters> whereparameters, string sortexp, int limit, Nullable<int> offset, string language, Func<V, string, T> transformer)
        {
            try
            {
                //CultureInfo myculture = new CultureInfo("en");

                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

                var command = new NpgsqlCommand(commandText);
                command.Connection = conn;

                if (whereparameters != null)
                {
                    foreach (var parameter in whereparameters)
                    {
                        command.Parameters.AddWithValue(parameter.Name, parameter.Type, parameter.Value);
                    }
                }

                NpgsqlDataReader dr = command.ExecuteReader();

                List<T> lstSelect = new List<T>();
                while (dr.Read())
                {
                    var pgdata = JsonConvert.DeserializeObject<V>(dr[1].ToString());

                    var transformeddata = transformer(pgdata, language);

                    lstSelect.Add(transformeddata);
                }

                dr.Close();

                return lstSelect;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion       

        #region Generic Insert Method

        public static string InsertDataIntoTable(NpgsqlConnection conn, string tablename, string data, string id)
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

                int affectedrows = command.ExecuteNonQuery();

                return affectedrows.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string InsertDataIntoTable(NpgsqlConnection conn, string tablename, object data, string id)
        {
            try
            {
                var command = new NpgsqlCommand("INSERT INTO " + tablename + "(id, data) VALUES(@id,@data)", conn);
                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, id);
                command.Parameters.AddWithValue("data", NpgsqlTypes.NpgsqlDbType.Jsonb, JsonConvert.SerializeObject(data));

                int affectedrows = command.ExecuteNonQuery();

                return affectedrows.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region Generic Update Method

        public static string UpdateDataFromTable(NpgsqlConnection conn, string tablename, string data, string id)
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

                int affectedrows = command.ExecuteNonQuery();

                return affectedrows.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string UpdateDataFromTable(NpgsqlConnection conn, string tablename, object data, string id)
        {
            try
            {
                var command = new NpgsqlCommand("UPDATE " + tablename + " SET data = @data WHERE id = @id", conn);
                command.Parameters.AddWithValue("data", NpgsqlTypes.NpgsqlDbType.Jsonb, JsonConvert.SerializeObject(data));
                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, id);

                int affectedrows = command.ExecuteNonQuery();

                return affectedrows.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        #endregion

        #region Generic Delete Method

        public static string DeleteDataFromTable(NpgsqlConnection conn, string tablename, string idvalue)
        {
            try
            {

                //string commandText = "DELETE FROM " + tablename + " WHERE id = '" + idvalue + "';";

                //var command = new NpgsqlCommand(commandText);
                //command.Connection = conn;

                var command = new NpgsqlCommand("DELETE FROM " + tablename + " WHERE id = @id", conn);
                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, idvalue);

                int affectedrows = command.ExecuteNonQuery();

                return affectedrows.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region Generic Helpers

        public static string GetResultJson(int pagenumber, int totalpages, int totalcount, string seed, string data)
        {
            string resultstr = "";

            if (data.StartsWith("["))
                resultstr = "{" + "\"TotalResults\":" + totalcount + ",\"TotalPages\":" + totalpages + ",\"CurrentPage\":" + pagenumber + ",\"Seed\":\"" + seed + "\",\"Items\":" + data + "}";
            else
                resultstr = "{" + "\"TotalResults\":" + totalcount + ",\"TotalPages\":" + totalpages + ",\"CurrentPage\":" + pagenumber + ",\"Seed\":\"" + seed + "\",\"Items\":[" + data + "]}";

            return resultstr;
        }

        public static string GetResultJson(int pagenumber, int totalpages, int totalcount, int onlineresults, string seed, string data)
        {
            string resultstr = "";

            if (data.StartsWith("["))
                resultstr = "{" + "\"TotalResults\":" + totalcount + ",\"TotalPages\":" + totalpages + ",\"CurrentPage\":" + pagenumber + ",\"OnlineResults\":" + onlineresults + ",\"Seed\":\"" + seed + "\",\"Items\":" + data + "}";
            else
                resultstr = "{" + "\"TotalResults\":" + totalcount + ",\"TotalPages\":" + totalpages + ",\"CurrentPage\":" + pagenumber + ",\"OnlineResults\":" + onlineresults + ",\"Seed\":\"" + seed + "\",\"Items\":[" + data + "]}";

            return resultstr;
        }

        public static string GetResultJson(int pagenumber, int totalpages, int totalcount, int onlineresults, string resultid, string seed, string data)
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
        public string Name { get; set; }
        public NpgsqlTypes.NpgsqlDbType Type { get; set; }
        public string Value { get; set; }
    }
}
