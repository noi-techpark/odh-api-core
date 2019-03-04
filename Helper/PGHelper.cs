//using Newtonsoft.Json;
//using Npgsql;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;

//namespace Helper
//{
//    public class PostgresSQLHelper
//    {

//        #region Generic Database Query Methods

//        public static string CreatetDatabaseCommand(string selectexp, string tablename, string whereexp, string sortexp, Nullable<int> offset, int limit)
//        {
//            string commandText = "SELECT " + selectexp + " FROM " + tablename;

//            if (!String.IsNullOrEmpty(whereexp))
//            {
//                commandText = commandText + " WHERE " + whereexp;
//            }

//            if (!String.IsNullOrEmpty(sortexp))
//            {
//                commandText = commandText + " ORDER BY " + sortexp;
//            }

//            if (offset != null)
//            {
//                commandText = commandText + " OFFSET " + offset;
//            }

//            if (limit > 0)
//            {
//                commandText = commandText + " LIMIT " + limit;
//            }

//            commandText = commandText + ";";

//            return commandText;
//        }

//        /// <summary>
//        /// Return List of Tuple with ID and JSON Data mapped to given Object
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<Tuple<string, T>> SelectFromTableDataAsIdAndObject<T>(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset)
//        {
//            try
//            {
//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;

//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<Tuple<string, T>> lstSelect = new List<Tuple<string, T>>();
//                while (dr.Read())
//                {                    
//                    var data = JsonConvert.DeserializeObject<T>(dr[1].ToString());
//                    var id = dr[0].ToString();

//                    lstSelect.Add(Tuple.Create<string, T>(id, data));
//                }

//                dr.Close();

//                Console.ForegroundColor = ConsoleColor.Green;
//                Console.WriteLine("Data queried " + lstSelect.Count + " Results");

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine("Error :" + ex);

//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to given OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<T> SelectFromTableDataAsObject<T>(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset)
//        {
//            try
//            {

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;

//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<T> lstSelect = new List<T>();
//                while (dr.Read())
//                {
//                    var data = JsonConvert.DeserializeObject<T>(dr[1].ToString());

//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                Console.ForegroundColor = ConsoleColor.Green;
//                Console.WriteLine("Data queried " + lstSelect.Count + " Results");

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine("Error :" + ex);

//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns Single JSON Data mapped to given OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static T SelectFromTableDataAsObject<T>(NpgsqlConnection conn, string tablename, string id)
//        {
//            try
//            {
//                string whereexp = "Id LIKE '" + id + "'";
//                string commandText = CreatetDatabaseCommand("*", tablename, whereexp, "", null, 0);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;

//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<T> lstSelect = new List<T>();
//                while (dr.Read())
//                {
//                    //zu testen
//                    // Place this at the beginning of your program to use Json.NET everywhere (recommended)
//                    //NpgsqlConnection.GlobalTypeMapper.UseJsonNet();
//                    // Or to temporarily use JsonNet on a single connection only:
//                    //conn.TypeMapper.UseJsonNet();
//                    //var data = dr.GetFieldValue<T>(1);

//                    var data = JsonConvert.DeserializeObject<T>(dr[1].ToString());

//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                Console.ForegroundColor = ConsoleColor.Green;
//                Console.WriteLine("Data queried " + lstSelect.Count + " Results");

//                return lstSelect.FirstOrDefault();
//            }
//            catch (Exception ex)
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine("Error :" + ex);

//                return default(T);
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to given OBJECT by passed Fields to deserialize
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<T> SelectFromTableDataAsObjectExtended<T>(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, List<string> fieldstoadd)
//        {
//            try
//            {

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;

//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<T> lstSelect = new List<T>();
//                while (dr.Read())
//                {
//                    int i = 0;
//                    string stringtodeserialize = "{";
//                    foreach (string s in fieldstoadd)
//                    {
//                        stringtodeserialize = stringtodeserialize + "\"" + s + "\":" + dr[i].ToString() + ",";
//                        i++;
//                    }
//                    stringtodeserialize = stringtodeserialize.Remove(stringtodeserialize.Length - 1);
//                    stringtodeserialize = stringtodeserialize + "}";

//                    var data = JsonConvert.DeserializeObject<T>(stringtodeserialize);

//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                Console.ForegroundColor = ConsoleColor.Green;
//                Console.WriteLine("Data queried " + lstSelect.Count + " Results");

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine("Error :" + ex);

//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data as STRING (Only Data)
//        /// </summary>
//        /// <param name="conn">PG Connection</param>
//        /// <param name="tablename">Table Name</param>
//        /// <param name="whereexp">Where Expression</param>
//        /// <param name="sortexp">Sort Expression</param>
//        /// <param name="limit">Limit</param>
//        /// <param name="offset">Offset</param>
//        /// <returns>List of JSON Strings</returns>
//        public static List<string> SelectFromTableDataAsString(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset)
//        {
//            try
//            {

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;

//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<string> lstSelect = new List<string>();
//                while (dr.Read())
//                {
//                    lstSelect.Add(dr[1].ToString());
//                }

//                dr.Close();


//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data as Tuple List (Id, String)
//        /// </summary>
//        /// <param name="conn">PG Connection</param>
//        /// <param name="tablename">Table Name</param>
//        /// <param name="whereexp">Where Expression</param>
//        /// <param name="sortexp">Sort Expression</param>
//        /// <param name="limit">Limit</param>
//        /// <param name="offset">Offset</param>
//        /// <returns>List of JSON Strings</returns>
//        public static List<string> SelectFromTableDataAsStringExtended(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, List<string> fieldstoadd)
//        {
//            try
//            {

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;

//                NpgsqlDataReader dr = command.ExecuteReader();


//                List<string> lstSelect = new List<string>();
//                while (dr.Read())
//                {
//                    bool isvalueempty = false;

//                    int i = 0;

//                    string strtoadd = "{";

//                    foreach (string s in fieldstoadd)
//                    {
//                        strtoadd = strtoadd + "\"" + s + "\":" + dr[i].ToString() + ",";

//                        if (String.IsNullOrEmpty(dr[i].ToString()) || dr[i].ToString() == "null" || dr[i].ToString() == "\"\"")
//                            isvalueempty = true;

//                        i++;
//                    }
//                    strtoadd = strtoadd.Remove(strtoadd.Length - 1);
//                    strtoadd = strtoadd + "}";

//                    if (!isvalueempty)
//                    {
//                        lstSelect.Add(strtoadd);
//                    }
//                }

//                dr.Close();


//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data as Tuple List (Id, String)
//        /// </summary>
//        /// <param name="conn">PG Connection</param>
//        /// <param name="tablename">Table Name</param>
//        /// <param name="whereexp">Where Expression</param>
//        /// <param name="sortexp">Sort Expression</param>
//        /// <param name="limit">Limit</param>
//        /// <param name="offset">Offset</param>
//        /// <returns>List of JSON Strings</returns>
//        public static List<string> SelectFromTableDataAsIdAndStringAndType(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, List<string> fieldstoadd, string type)
//        {
//            try
//            {

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;

//                NpgsqlDataReader dr = command.ExecuteReader();


//                List<string> lstSelect = new List<string>();
//                while (dr.Read())
//                {
//                    string strtoadd = "{";
//                    int i = 0;
//                    foreach (string s in fieldstoadd)
//                    {
//                        if (s != "themeIds")
//                            strtoadd = strtoadd + "\"" + s + "\":" + dr[i].ToString() + ",";
//                        else
//                        {
//                            var themeids = JsonConvert.DeserializeObject<List<string>>(dr[i].ToString());
//                            strtoadd = strtoadd + "\"" + s + "\":\"" + String.Join(",", themeids) + "\",";
//                        }
//                        i++;
//                    }


//                    strtoadd = strtoadd + "\"typ\":\"" + type + "\"";

//                    strtoadd = strtoadd + "}";

//                    lstSelect.Add(strtoadd);
//                }

//                dr.Close();


//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data as List (Id)
//        /// </summary>
//        /// <param name="conn">PG Connection</param>
//        /// <param name="tablename">Table Name</param>
//        /// <param name="whereexp">Where Expression</param>
//        /// <param name="sortexp">Sort Expression</param>
//        /// <param name="limit">Limit</param>
//        /// <param name="offset">Offset</param>
//        /// <returns>List of JSON Strings</returns>
//        public static List<string> SelectFromTableDataAsId(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset)
//        {
//            try
//            {

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;

//                NpgsqlDataReader dr = command.ExecuteReader();


//                List<string> lstSelect = new List<string>();
//                while (dr.Read())
//                {
//                    lstSelect.Add(dr[0].ToString());
//                }

//                dr.Close();


//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Counts the elements of the table
//        /// </summary>
//        /// <param name="conn">PG Connection</param>
//        /// <param name="tablename">Table name</param>
//        /// <param name="whereexp">Where Expression</param>
//        /// <returns>Elements Count as Long</returns>
//        public static long CountDataFromTable(NpgsqlConnection conn, string tablename, string whereexp)
//        {
//            try
//            {

//                string commandText = "SELECT COUNT(*) FROM " + tablename;

//                if (!String.IsNullOrEmpty(whereexp))
//                {
//                    commandText = commandText + " WHERE " + whereexp;
//                }


//                commandText = commandText + ";";

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;

//                Int64 count = (Int64)command.ExecuteScalar();

//                return count;
//            }
//            catch (Exception ex)
//            {
//                return 0;
//            }
//        }

//        #endregion

//        #region Type specific Select Helpers

//        /// <summary>
//        /// Returns List of JSON Data as LocHelperclass (id, typ, name)
//        /// </summary>
//        /// <param name="conn">PG Connection</param>
//        /// <param name="tablename">Table Name</param>
//        /// <param name="whereexp">Where Expression</param>
//        /// <param name="sortexp">Sort Expression</param>
//        /// <param name="limit">Limit</param>
//        /// <param name="offset">Offset</param>
//        /// <returns>List of JSON Strings</returns>
//        public static List<LocHelperclass> SelectFromTableDataAsLocHelperObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string typ)
//        {
//            try
//            {

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;

//                NpgsqlDataReader dr = command.ExecuteReader();


//                List<LocHelperclass> lstSelect = new List<LocHelperclass>();
//                while (dr.Read())
//                {
//                    LocHelperclass lochelper = new LocHelperclass();
//                    lochelper.id = dr[0].ToString();
//                    lochelper.typ = typ;
//                    lochelper.name = dr[1].ToString();

//                    lstSelect.Add(lochelper);
//                }

//                dr.Close();


//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data as LocHelperclass (id, typ, name)
//        /// </summary>
//        /// <param name="conn">PG Connection</param>
//        /// <param name="tablename">Table Name</param>
//        /// <param name="whereexp">Where Expression</param>
//        /// <param name="sortexp">Sort Expression</param>
//        /// <param name="limit">Limit</param>
//        /// <param name="offset">Offset</param>
//        /// <returns>List of JSON Strings</returns>
//        public static List<AccoHelperClassExtended> SelectFromTableDataAsAccoHelperClassExtended(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string typ)
//        {
//            try
//            {

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;

//                NpgsqlDataReader dr = command.ExecuteReader();


//                List<AccoHelperClassExtended> lstSelect = new List<AccoHelperClassExtended>();
//                while (dr.Read())
//                {
//                    AccoHelperClassExtended lochelper = new AccoHelperClassExtended();
//                    lochelper.id = dr[0].ToString();
//                    lochelper.typ = typ;
//                    lochelper.name = dr[1].ToString();

//                    var themeids = JsonConvert.DeserializeObject<List<string>>(dr[2].ToString());
//                    lochelper.themeIds = String.Join(",", themeids);

//                    lstSelect.Add(lochelper);
//                }

//                dr.Close();


//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data as LocHelperclassCategories (id, typ, name, categoryid)
//        /// </summary>
//        /// <param name="conn">PG Connection</param>
//        /// <param name="tablename">Table Name</param>
//        /// <param name="whereexp">Where Expression</param>
//        /// <param name="sortexp">Sort Expression</param>
//        /// <param name="limit">Limit</param>
//        /// <param name="offset">Offset</param>
//        /// <returns>List of JSON Strings</returns>
//        public static List<LocHelperclassCategories> SelectFromTableDataAsLocHelperclassCategoriesObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string typ, string categoryid)
//        {
//            try
//            {

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;

//                NpgsqlDataReader dr = command.ExecuteReader();


//                List<LocHelperclassCategories> lstSelect = new List<LocHelperclassCategories>();
//                while (dr.Read())
//                {
//                    LocHelperclassCategories lochelper = new LocHelperclassCategories();
//                    lochelper.id = dr[0].ToString();
//                    lochelper.typ = typ;
//                    lochelper.name = dr[1].ToString();
//                    lochelper.categoryid = categoryid;

//                    lstSelect.Add(lochelper);
//                }

//                dr.Close();


//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data as LocHelperclassCategories (id, typ, name, categoryid)
//        /// </summary>
//        /// <param name="conn">PG Connection</param>
//        /// <param name="tablename">Table Name</param>
//        /// <param name="whereexp">Where Expression</param>
//        /// <param name="sortexp">Sort Expression</param>
//        /// <param name="limit">Limit</param>
//        /// <param name="offset">Offset</param>
//        /// <returns>List of JSON Strings</returns>
//        public static List<LocHelperclassCategoriesSubCategories> SelectFromTableDataAsLocHelperclassCategoriesSubCategoriesObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string typ, string categoryid, string subcategoryid)
//        {
//            try
//            {

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;

//                NpgsqlDataReader dr = command.ExecuteReader();


//                List<LocHelperclassCategoriesSubCategories> lstSelect = new List<LocHelperclassCategoriesSubCategories>();
//                while (dr.Read())
//                {
//                    LocHelperclassCategoriesSubCategories lochelper = new LocHelperclassCategoriesSubCategories();
//                    lochelper.id = dr[0].ToString();
//                    lochelper.typ = typ;
//                    lochelper.name = dr[1].ToString();
//                    lochelper.categoryid = categoryid;
//                    lochelper.subcategoryid = subcategoryid;

//                    lstSelect.Add(lochelper);
//                }

//                dr.Close();


//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data as LocHelperclass (id, typ, name)
//        /// </summary>
//        /// <param name="conn">PG Connection</param>
//        /// <param name="tablename">Table Name</param>
//        /// <param name="whereexp">Where Expression</param>
//        /// <param name="sortexp">Sort Expression</param>
//        /// <param name="limit">Limit</param>
//        /// <param name="offset">Offset</param>
//        /// <returns>List of JSON Strings</returns>
//        public static List<SmgPoiRelatedContentReduced> SelectFromTableDataAsSmgPoiRelatedContentReducedObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset)
//        {
//            try
//            {

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;

//                NpgsqlDataReader dr = command.ExecuteReader();


//                List<SmgPoiRelatedContentReduced> lstSelect = new List<SmgPoiRelatedContentReduced>();
//                while (dr.Read())
//                {
//                    SmgPoiRelatedContentReduced lochelper = new SmgPoiRelatedContentReduced();
//                    lochelper.Id = dr[0].ToString();
//                    lochelper.Type = dr[1].ToString();
//                    lochelper.Name = dr[2].ToString();

//                    lstSelect.Add(lochelper);
//                }

//                dr.Close();


//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data as LocHelperclass (id, typ, name)
//        /// </summary>
//        /// <param name="conn">PG Connection</param>
//        /// <param name="tablename">Table Name</param>
//        /// <param name="whereexp">Where Expression</param>
//        /// <param name="sortexp">Sort Expression</param>
//        /// <param name="limit">Limit</param>
//        /// <param name="offset">Offset</param>
//        /// <returns>List of JSON Strings</returns>
//        public static List<SmgPoiRelatedContentReduced> SelectFromTableDataAsEventRelatedContentReducedObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string typ)
//        {
//            try
//            {

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;

//                NpgsqlDataReader dr = command.ExecuteReader();


//                List<SmgPoiRelatedContentReduced> lstSelect = new List<SmgPoiRelatedContentReduced>();
//                while (dr.Read())
//                {
//                    SmgPoiRelatedContentReduced lochelper = new SmgPoiRelatedContentReduced();
//                    lochelper.Id = dr[0].ToString();
//                    lochelper.Type = typ;
//                    lochelper.Name = dr[1].ToString();

//                    lstSelect.Add(lochelper);
//                }

//                dr.Close();


//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to AccommodationLocalized OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<AccommodationLocalized> SelectFromTableDataAsAccommodationLocalizedObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<AccommodationLocalized> lstSelect = new List<AccommodationLocalized>();
//                while (dr.Read())
//                {
//                    var acco = JsonConvert.DeserializeObject<Accommodation>(dr[1].ToString());

//                    AccommodationLocalized data = new AccommodationLocalized();
//                    data.AccoBookingChannel = acco.AccoBookingChannel;
//                    data.AccoCategoryId = acco.AccoCategoryId;
//                    data.AccoDetail = acco.AccoDetail != null ? acco.AccoDetail.ContainsKey(language) ? acco.AccoDetail[language] : null : null;
//                    data.AccoTypeId = acco.AccoTypeId;
//                    data.Altitude = acco.Altitude;
//                    data.AltitudeUnitofMeasure = acco.AltitudeUnitofMeasure;
//                    data.BadgeIds = acco.BadgeIds;
//                    data.Beds = acco.Beds;
//                    data.BoardIds = acco.BoardIds;
//                    //Features = acco.Features,
//                    data.FirstImport = acco.FirstImport;
//                    data.GastronomyId = acco.GastronomyId;
//                    data.Gpstype = acco.Gpstype;
//                    data.HgvId = acco.HgvId;
//                    data.HasApartment = acco.HasApartment;
//                    data.HasRoom = acco.HasRoom;
//                    data.Id = acco.Id;
//                    data.IsBookable = acco.IsBookable;
//                    data.DistrictId = acco.DistrictId;
//                    data.LastChange = acco.LastChange;
//                    data.Latitude = acco.Latitude;
//                    data.Longitude = acco.Longitude;
//                    //data.LocationInfo = new LocationInfoLocalized()
//                    //{
//                    //    DistrictInfo = new DistrictInfoLocalized() { Id = acco.LocationInfo.DistrictInfo.Id, Name = acco.LocationInfo.DistrictInfo.Name[language] },
//                    //    MunicipalityInfo = new MunicipalityInfoLocalized() { Id = acco.LocationInfo.MunicipalityInfo.Id, Name = acco.LocationInfo.MunicipalityInfo.Name[language] },
//                    //    TvInfo = new TvInfoLocalized() { Id = acco.LocationInfo.TvInfo.Id, Name = acco.LocationInfo.TvInfo.Name[language] },
//                    //    RegionInfo = new RegionInfoLocalized() { Id = acco.LocationInfo.RegionInfo.Id, Name = acco.LocationInfo.RegionInfo.Name[language] }
//                    //};
//                    var distinfolocalized = new DistrictInfoLocalized() { };
//                    if (acco.LocationInfo != null)
//                    {
//                        if (acco.LocationInfo.DistrictInfo != null)
//                        {
//                            distinfolocalized.Id = acco.LocationInfo.DistrictInfo.Id;
//                            distinfolocalized.Name = acco.LocationInfo.DistrictInfo.Name.ContainsKey(language) ? acco.LocationInfo.DistrictInfo.Name[language] : "";
//                        }
//                    }

//                    var muninfolocalized = new MunicipalityInfoLocalized() { };
//                    if (acco.LocationInfo != null)
//                    {
//                        if (acco.LocationInfo.MunicipalityInfo != null)
//                        {
//                            muninfolocalized.Id = acco.LocationInfo.MunicipalityInfo.Id;
//                            muninfolocalized.Name = acco.LocationInfo.MunicipalityInfo.Name.ContainsKey(language) ? acco.LocationInfo.MunicipalityInfo.Name[language] : "";
//                        }
//                    }

//                    var reginfolocalized = new RegionInfoLocalized() { };
//                    if (acco.LocationInfo != null)
//                    {
//                        if (acco.LocationInfo.RegionInfo != null)
//                        {
//                            reginfolocalized.Id = acco.LocationInfo.RegionInfo.Id;
//                            reginfolocalized.Name = acco.LocationInfo.RegionInfo.Name.ContainsKey(language) ? acco.LocationInfo.RegionInfo.Name[language] : "";
//                        }
//                    }

//                    var tvinfolocalized = new TvInfoLocalized() { };
//                    if (acco.LocationInfo != null)
//                    {
//                        if (acco.LocationInfo.TvInfo != null)
//                        {
//                            tvinfolocalized.Id = acco.LocationInfo.TvInfo.Id;
//                            tvinfolocalized.Name = acco.LocationInfo.TvInfo.Name.ContainsKey(language) ? acco.LocationInfo.TvInfo.Name[language] : "";
//                        }
//                    }

//                    data.LocationInfo = new LocationInfoLocalized()
//                    {
//                        DistrictInfo = distinfolocalized,
//                        MunicipalityInfo = muninfolocalized,
//                        TvInfo = tvinfolocalized,
//                        RegionInfo = reginfolocalized
//                    };

//                    data.ImageGallery = acco.ImageGallery != null ? acco.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
//                    data.MainLanguage = acco.MainLanguage;
//                    //MarketingGroupIds = acco.MarketingGroupIds,
//                    data.Shortname = acco.Shortname;
//                    data.SmgActive = acco.SmgActive;
//                    data.SmgTags = acco.SmgTags;
//                    data.SpecialFeaturesIds = acco.SpecialFeaturesIds;
//                    data.ThemeIds = acco.ThemeIds;
//                    data.TourismVereinId = acco.TourismVereinId;
//                    data.TrustYouID = acco.TrustYouID;
//                    data.TrustYouResults = acco.TrustYouResults;
//                    data.TrustYouScore = acco.TrustYouScore;
//                    data.TVMember = acco.TVMember;
//                    data.Units = acco.Units;

//                    //data.Id = dr[0].ToString();
//                    ////data.Active = Convert.ToBoolean(dr[1].ToString());
//                    //data.HgvId = dr[2].ToString();
//                    //data.Shortname = dr[3].ToString();
//                    //data.Units = Convert.ToInt32(dr[4].ToString());
//                    //data.Beds = Convert.ToInt32(dr[5].ToString());
//                    //data.HasApartment = Convert.ToBoolean(dr[6].ToString());
//                    //data.HasRoom = Convert.ToBoolean(dr[7].ToString());
//                    ////data.IsCamping = Convert.ToBoolean(dr[8].ToString());
//                    ////data.IsGastronomy = Convert.ToBoolean(dr[9].ToString());
//                    //data.IsBookable = Convert.ToBoolean(dr[10].ToString());
//                    ////data.IsAccommodation = Convert.ToBoolean(dr[11].ToString());
//                    //data.SmgActive = Convert.ToBoolean(dr[12].ToString());
//                    //data.TVMember = Convert.ToBoolean(dr[13].ToString());
//                    //data.TourismVereinId = dr[14].ToString();
//                    //data.MainLanguage = dr[15].ToString();
//                    //data.FirstImport = Convert.ToDateTime(dr[16].ToString());
//                    //data.LastChange = Convert.ToDateTime(dr[17].ToString());
//                    //data.Gpstype = dr[18].ToString();
//                    //data.Latitude = Convert.ToDouble(dr[19].ToString(), myculture);
//                    //data.Longitude = Convert.ToDouble(dr[20].ToString(), myculture);
//                    //data.Altitude = Convert.ToDouble(dr[21].ToString(), myculture);
//                    //data.AltitudeUnitofMeasure = dr[22].ToString();
//                    //data.AccoCategoryId = dr[23].ToString();
//                    //data.AccoTypeId = dr[24].ToString();
//                    //data.DistrictId = dr[25].ToString();
//                    //data.BoardIds = JsonConvert.DeserializeObject<ICollection<string>>(dr[26].ToString());
//                    ////data.MarketingGroupIds = dr[27].ToString();
//                    ////data.Features = dr[28].ToString();
//                    //data.BadgeIds = JsonConvert.DeserializeObject<ICollection<string>>(dr[29].ToString());
//                    //data.ThemeIds = JsonConvert.DeserializeObject<ICollection<string>>(dr[30].ToString());
//                    //data.SpecialFeaturesIds = JsonConvert.DeserializeObject<ICollection<string>>(dr[31].ToString());

//                    //var accodetail = JsonConvert.DeserializeObject<Dictionary<string, AccoDetail>>(dr[32].ToString());
//                    //data.AccoDetail = accodetail[language];

//                    //data.AccoBookingChannel = JsonConvert.DeserializeObject<ICollection<AccoBookingChannel>>(dr[33].ToString());

//                    //var myimagegallery = JsonConvert.DeserializeObject<ICollection<ImageGallery>>(dr[34].ToString());
//                    //data.ImageGallery = myimagegallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc[language] : null, ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle[language] : null, ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList();
//                    //var mylocinfo = JsonConvert.DeserializeObject<LocationInfo>(dr[35].ToString());
//                    //data.LocationInfo = new LocationInfoLocalized()
//                    //{
//                    //    DistrictInfo = new DistrictInfoLocalized() { Id = mylocinfo.DistrictInfo.Id, Name = mylocinfo.DistrictInfo.Name[language] },
//                    //    MunicipalityInfo = new MunicipalityInfoLocalized() { Id = mylocinfo.MunicipalityInfo.Id, Name = mylocinfo.MunicipalityInfo.Name[language] },
//                    //    TvInfo = new TvInfoLocalized() { Id = mylocinfo.TvInfo.Id, Name = mylocinfo.TvInfo.Name[language] },
//                    //    RegionInfo = new RegionInfoLocalized() { Id = mylocinfo.RegionInfo.Id, Name = mylocinfo.RegionInfo.Name[language] }
//                    //};

//                    //data.GastronomyId = dr[36].ToString();
//                    //data.SmgTags = JsonConvert.DeserializeObject<ICollection<string>>(dr[37].ToString());
//                    ////data.HasLanguage = dr[38].ToString();
//                    //data.MssResponseShort = null; //dr[39].ToString();
//                    //data.TrustYouID = dr[40].ToString();
//                    //data.TrustYouScore = Convert.ToDouble(dr[41].ToString(), myculture);
//                    //data.TrustYouResults = Convert.ToInt32(dr[42].ToString());


//                    //var data = JsonConvert.DeserializeObject<T>();

//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to AccommodationLocalized OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<AccoListObject> SelectFromTableDataAsAccoListObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<AccoListObject> lstSelect = new List<AccoListObject>();
//                while (dr.Read())
//                {
//                    var acco = JsonConvert.DeserializeObject<Accommodation>(dr[1].ToString());

//                    AccoListObject data = new AccoListObject();
//                    data.Id = acco.Id;
//                    data.Name = acco.AccoDetail.ContainsKey(language) ? acco.AccoDetail[language].Name : "";
//                    data.Type = acco.AccoTypeId;
//                    data.Category = acco.AccoCategoryId;
//                    data.District = acco.LocationInfo.DistrictInfo != null ? acco.LocationInfo.DistrictInfo.Name[language] : null;
//                    data.Municipality = acco.LocationInfo.MunicipalityInfo != null ? acco.LocationInfo.MunicipalityInfo.Name[language] : null;
//                    data.Tourismverein = acco.LocationInfo.TvInfo != null ? acco.LocationInfo.TvInfo.Name[language] : null;
//                    data.Region = acco.LocationInfo.RegionInfo != null ? acco.LocationInfo.RegionInfo.Name[language] : null;
//                    data.TrustYouID = acco.TrustYouID;
//                    data.TrustYouResults = acco.TrustYouResults;
//                    data.TrustYouScore = acco.TrustYouScore;
//                    data.SuedtirolinfoLink = "https://www.suedtirol.info/" + language + "/tripmapping/acco/" + acco.Id.ToUpper();
//                    data.ImageGallery = acco.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ? acco.ImageGallery.Where(x => x.ListPosition == 0).ToList() : null;

//                    //if(acco.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0)
//                    //    data.ImageGallery = acco.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc[language] : null, ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle[language] : null, ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList();

//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to PoiBaseInfosLocalized OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<GBLTSActivityPoiLocalized> SelectFromTableDataAsLtsPoiLocalizedObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<GBLTSActivityPoiLocalized> lstSelect = new List<GBLTSActivityPoiLocalized>();
//                while (dr.Read())
//                {
//                    var poibaseinfo = JsonConvert.DeserializeObject<GBLTSPoi>(dr[1].ToString());

//                    GBLTSActivityPoiLocalized data = new GBLTSActivityPoiLocalized();

//                    data.Id = poibaseinfo.Id;
//                    data.LastChange = poibaseinfo.LastChange;
//                    data.FirstImport = poibaseinfo.FirstImport;
//                    data.Active = poibaseinfo.Active;
//                    data.AdditionalPoiInfos = poibaseinfo.AdditionalPoiInfos != null ? poibaseinfo.AdditionalPoiInfos.ContainsKey(language) ? poibaseinfo.AdditionalPoiInfos[language] : null : null;
//                    data.AltitudeDifference = poibaseinfo.AltitudeDifference;
//                    data.AltitudeHighestPoint = poibaseinfo.AltitudeHighestPoint;
//                    data.AltitudeLowestPoint = poibaseinfo.AltitudeLowestPoint;
//                    data.AltitudeSumDown = poibaseinfo.AltitudeSumDown;
//                    data.AltitudeSumUp = poibaseinfo.AltitudeSumUp;
//                    data.AreaId = poibaseinfo.AreaId;
//                    data.ContactInfos = poibaseinfo.ContactInfos != null ? poibaseinfo.ContactInfos.ContainsKey(language) ? poibaseinfo.ContactInfos[language] : null : null;
//                    data.Detail = poibaseinfo.Detail != null ? poibaseinfo.Detail.ContainsKey(language) ? poibaseinfo.Detail[language] : null : null;
//                    data.Difficulty = poibaseinfo.Difficulty;
//                    data.DistanceDuration = poibaseinfo.DistanceDuration;
//                    data.DistanceLength = poibaseinfo.DistanceLength;
//                    data.Exposition = poibaseinfo.Exposition;
//                    data.FeetClimb = poibaseinfo.FeetClimb;
//                    data.GpsInfo = poibaseinfo.GpsInfo;
//                    data.GpsTrack = poibaseinfo.GpsTrack;
//                    data.HasFreeEntrance = poibaseinfo.HasFreeEntrance;
//                    data.HasRentals = poibaseinfo.HasRentals;
//                    data.Highlight = poibaseinfo.Highlight;
//                    data.IsOpen = poibaseinfo.IsOpen;
//                    data.IsPrepared = poibaseinfo.IsPrepared;
//                    data.IsWithLigth = poibaseinfo.IsWithLigth;
//                    data.LiftAvailable = poibaseinfo.LiftAvailable;
//                    data.OperationSchedule = poibaseinfo.OperationSchedule;
//                    data.Ratings = poibaseinfo.Ratings;
//                    data.RunToValley = poibaseinfo.RunToValley;
//                    data.Shortname = poibaseinfo.Shortname;
//                    data.SmgActive = poibaseinfo.SmgActive;
//                    data.SmgId = poibaseinfo.SmgId;
//                    data.SubType = poibaseinfo.SubType;
//                    data.TourismorganizationId = poibaseinfo.TourismorganizationId;
//                    data.Type = poibaseinfo.Type;
//                    data.SmgTags = poibaseinfo.SmgTags;

//                    var distinfolocalized = new DistrictInfoLocalized() { };
//                    if (poibaseinfo.LocationInfo != null)
//                    {
//                        if (poibaseinfo.LocationInfo.DistrictInfo != null)
//                        {
//                            distinfolocalized.Id = poibaseinfo.LocationInfo.DistrictInfo.Id;
//                            distinfolocalized.Name = poibaseinfo.LocationInfo.DistrictInfo.Name.ContainsKey(language) ? poibaseinfo.LocationInfo.DistrictInfo.Name[language] : "";
//                        }
//                    }

//                    var muninfolocalized = new MunicipalityInfoLocalized() { };
//                    if (poibaseinfo.LocationInfo != null)
//                    {
//                        if (poibaseinfo.LocationInfo.MunicipalityInfo != null)
//                        {
//                            muninfolocalized.Id = poibaseinfo.LocationInfo.MunicipalityInfo.Id;
//                            muninfolocalized.Name = poibaseinfo.LocationInfo.MunicipalityInfo.Name.ContainsKey(language) ? poibaseinfo.LocationInfo.MunicipalityInfo.Name[language] : "";
//                        }
//                    }

//                    var reginfolocalized = new RegionInfoLocalized() { };
//                    if (poibaseinfo.LocationInfo != null)
//                    {
//                        if (poibaseinfo.LocationInfo.RegionInfo != null)
//                        {
//                            reginfolocalized.Id = poibaseinfo.LocationInfo.RegionInfo.Id;
//                            reginfolocalized.Name = poibaseinfo.LocationInfo.RegionInfo.Name.ContainsKey(language) ? poibaseinfo.LocationInfo.RegionInfo.Name[language] : "";
//                        }
//                    }

//                    var tvinfolocalized = new TvInfoLocalized() { };
//                    if (poibaseinfo.LocationInfo != null)
//                    {
//                        if (poibaseinfo.LocationInfo.TvInfo != null)
//                        {
//                            tvinfolocalized.Id = poibaseinfo.LocationInfo.TvInfo.Id;
//                            tvinfolocalized.Name = poibaseinfo.LocationInfo.TvInfo.Name.ContainsKey(language) ? poibaseinfo.LocationInfo.TvInfo.Name[language] : "";
//                        }
//                    }

//                    data.LocationInfo = new LocationInfoLocalized()
//                    {
//                        DistrictInfo = distinfolocalized,
//                        MunicipalityInfo = muninfolocalized,
//                        TvInfo = tvinfolocalized,
//                        RegionInfo = reginfolocalized
//                    };
//                    data.ImageGallery = poibaseinfo.ImageGallery != null ? poibaseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;


//                    List<LTSTagsLocalized> ltstagslocalized = new List<LTSTagsLocalized>();

//                    if (poibaseinfo.LTSTags != null)
//                    {
//                        foreach (var ltstag in poibaseinfo.LTSTags)
//                        {
//                            ltstagslocalized.Add(new LTSTagsLocalized() { Id = ltstag.Id, Level = ltstag.Level, TagName = ltstag.TagName.ContainsKey(language) ? ltstag.TagName[language] : "" });
//                        }
//                    }

//                    data.LTSTags = ltstagslocalized;
//                    data.GpsPoints = poibaseinfo.GpsPoints;

//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to EventLocalized OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<EventLocalized> SelectFromTableDataAsEventLocalizedObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<EventLocalized> lstSelect = new List<EventLocalized>();
//                while (dr.Read())
//                {
//                    var psdata = JsonConvert.DeserializeObject<Event>(dr[1].ToString());

//                    EventLocalized data = new EventLocalized();

//                    data.Id = psdata.Id;
//                    data.Active = psdata.Active;
//                    data.Altitude = psdata.Altitude;
//                    data.AltitudeUnitofMeasure = psdata.AltitudeUnitofMeasure;
//                    data.ContactInfos = psdata.ContactInfos != null ? psdata.ContactInfos.ContainsKey(language) ? psdata.ContactInfos[language] : null : null;
//                    data.DateBegin = psdata.DateBegin;
//                    data.DateEnd = psdata.DateEnd;
//                    data.Detail = psdata.Detail != null ? psdata.Detail.ContainsKey(language) ? psdata.Detail[language] : null : null;
//                    data.DistrictId = psdata.DistrictId;
//                    data.DistrictIds = psdata.DistrictIds;
//                    data.EventAdditionalInfos = psdata.EventAdditionalInfos != null ? psdata.EventAdditionalInfos.ContainsKey(language) ? psdata.EventAdditionalInfos[language] : null : null;
//                    data.EventDate = psdata.EventDate;
//                    data.EventPrice = psdata.EventPrice != null ? psdata.EventPrice.ContainsKey(language) ? psdata.EventPrice[language] : null : null;
//                    data.EventPublisher = psdata.EventPublisher;
//                    data.Gpstype = psdata.Gpstype;
//                    data.ImageGallery = psdata.ImageGallery != null ? psdata.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
//                    data.Latitude = psdata.Latitude;
//                    data.Longitude = psdata.Longitude;
//                    data.OrganizerInfos = psdata.OrganizerInfos != null ? psdata.OrganizerInfos.ContainsKey(language) ? psdata.OrganizerInfos[language] : null : null;
//                    data.OrgRID = psdata.OrgRID;
//                    data.PayMet = psdata.PayMet;
//                    data.Ranc = psdata.Ranc;
//                    data.Shortname = psdata.Shortname;
//                    data.SignOn = psdata.SignOn;
//                    data.SmgActive = psdata.SmgActive;
//                    data.SmgTags = psdata.SmgTags;
//                    data.Ticket = psdata.Ticket;
//                    data.TopicRIDs = psdata.TopicRIDs;
//                    data.Type = psdata.Type;

//                    var distinfolocalized = new DistrictInfoLocalized() { };
//                    if (psdata.LocationInfo != null)
//                    {
//                        if (psdata.LocationInfo.DistrictInfo != null)
//                        {
//                            distinfolocalized.Id = psdata.LocationInfo.DistrictInfo.Id;
//                            distinfolocalized.Name = psdata.LocationInfo.DistrictInfo.Name.ContainsKey(language) ? psdata.LocationInfo.DistrictInfo.Name[language] : "";
//                        }
//                    }

//                    var muninfolocalized = new MunicipalityInfoLocalized() { };
//                    if (psdata.LocationInfo != null)
//                    {
//                        if (psdata.LocationInfo.MunicipalityInfo != null)
//                        {
//                            muninfolocalized.Id = psdata.LocationInfo.MunicipalityInfo.Id;
//                            muninfolocalized.Name = psdata.LocationInfo.MunicipalityInfo.Name.ContainsKey(language) ? psdata.LocationInfo.MunicipalityInfo.Name[language] : "";
//                        }
//                    }

//                    var reginfolocalized = new RegionInfoLocalized() { };
//                    if (psdata.LocationInfo != null)
//                    {
//                        if (psdata.LocationInfo.RegionInfo != null)
//                        {
//                            reginfolocalized.Id = psdata.LocationInfo.RegionInfo.Id;
//                            reginfolocalized.Name = psdata.LocationInfo.RegionInfo.Name.ContainsKey(language) ? psdata.LocationInfo.RegionInfo.Name[language] : "";
//                        }
//                    }

//                    var tvinfolocalized = new TvInfoLocalized() { };
//                    if (psdata.LocationInfo != null)
//                    {
//                        if (psdata.LocationInfo.TvInfo != null)
//                        {
//                            tvinfolocalized.Id = psdata.LocationInfo.TvInfo.Id;
//                            tvinfolocalized.Name = psdata.LocationInfo.TvInfo.Name.ContainsKey(language) ? psdata.LocationInfo.TvInfo.Name[language] : "";
//                        }
//                    }

//                    data.LocationInfo = new LocationInfoLocalized()
//                    {
//                        DistrictInfo = distinfolocalized,
//                        MunicipalityInfo = muninfolocalized,
//                        TvInfo = tvinfolocalized,
//                        RegionInfo = reginfolocalized
//                    };

//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to GastronomyLocalized OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<GastronomyLocalized> SelectFromTableDataAsGastronomyLocalizedObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<GastronomyLocalized> lstSelect = new List<GastronomyLocalized>();
//                while (dr.Read())
//                {
//                    var pgdata = JsonConvert.DeserializeObject<Gastronomy>(dr[1].ToString());

//                    GastronomyLocalized data = new GastronomyLocalized();

//                    data.Id = pgdata.Id;
//                    data.AccommodationId = pgdata.AccommodationId;
//                    data.Active = pgdata.Active;
//                    data.Altitude = pgdata.Altitude;
//                    data.AltitudeUnitofMeasure = pgdata.AltitudeUnitofMeasure;
//                    data.CapacityCeremony = pgdata.CapacityCeremony;
//                    data.CategoryCodes = pgdata.CategoryCodes;
//                    data.ContactInfos = pgdata.ContactInfos != null ? pgdata.ContactInfos.ContainsKey(language) ? pgdata.ContactInfos[language] : null : null;
//                    data.Detail = pgdata.Detail != null ? pgdata.Detail.ContainsKey(language) ? pgdata.Detail[language] : null : null;
//                    data.DishRates = pgdata.DishRates;
//                    data.DistrictId = pgdata.DistrictId;
//                    data.Facilities = pgdata.Facilities;
//                    data.FirstImport = pgdata.FirstImport;
//                    data.Gpstype = pgdata.Gpstype;
//                    data.ImageGallery = pgdata.ImageGallery != null ? pgdata.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
//                    data.LastChange = pgdata.LastChange;
//                    data.Latitude = pgdata.Latitude;
//                    data.Longitude = pgdata.Longitude;
//                    data.MarketinggroupId = pgdata.MarketinggroupId;
//                    data.MaxSeatingCapacity = pgdata.MaxSeatingCapacity;
//                    data.OperationSchedule = data.OperationSchedule;
//                    data.Shortname = pgdata.Shortname;
//                    data.SmgActive = pgdata.SmgActive;
//                    data.SmgTags = pgdata.SmgTags;
//                    data.Type = pgdata.Type;

//                    var distinfolocalized = new DistrictInfoLocalized() { };
//                    if (pgdata.LocationInfo != null)
//                    {
//                        if (pgdata.LocationInfo.DistrictInfo != null)
//                        {
//                            distinfolocalized.Id = pgdata.LocationInfo.DistrictInfo.Id;
//                            distinfolocalized.Name = pgdata.LocationInfo.DistrictInfo.Name.ContainsKey(language) ? pgdata.LocationInfo.DistrictInfo.Name[language] : "";
//                        }
//                    }

//                    var muninfolocalized = new MunicipalityInfoLocalized() { };
//                    if (pgdata.LocationInfo != null)
//                    {
//                        if (pgdata.LocationInfo.MunicipalityInfo != null)
//                        {
//                            muninfolocalized.Id = pgdata.LocationInfo.MunicipalityInfo.Id;
//                            muninfolocalized.Name = pgdata.LocationInfo.MunicipalityInfo.Name.ContainsKey(language) ? pgdata.LocationInfo.MunicipalityInfo.Name[language] : "";
//                        }
//                    }

//                    var reginfolocalized = new RegionInfoLocalized() { };
//                    if (pgdata.LocationInfo != null)
//                    {
//                        if (pgdata.LocationInfo.RegionInfo != null)
//                        {
//                            reginfolocalized.Id = pgdata.LocationInfo.RegionInfo.Id;
//                            reginfolocalized.Name = pgdata.LocationInfo.RegionInfo.Name.ContainsKey(language) ? pgdata.LocationInfo.RegionInfo.Name[language] : "";
//                        }
//                    }

//                    var tvinfolocalized = new TvInfoLocalized() { };
//                    if (pgdata.LocationInfo != null)
//                    {
//                        if (pgdata.LocationInfo.TvInfo != null)
//                        {
//                            tvinfolocalized.Id = pgdata.LocationInfo.TvInfo.Id;
//                            tvinfolocalized.Name = pgdata.LocationInfo.TvInfo.Name.ContainsKey(language) ? pgdata.LocationInfo.TvInfo.Name[language] : "";
//                        }
//                    }

//                    data.LocationInfo = new LocationInfoLocalized()
//                    {
//                        DistrictInfo = distinfolocalized,
//                        MunicipalityInfo = muninfolocalized,
//                        TvInfo = tvinfolocalized,
//                        RegionInfo = reginfolocalized
//                    };

//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to GastronomyLocalized OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<GastronomyRelatedContentReduced> SelectFromTableDataAsGastronomyRelatedContentReducedObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<GastronomyRelatedContentReduced> lstSelect = new List<GastronomyRelatedContentReduced>();
//                while (dr.Read())
//                {
//                    var pgdata = JsonConvert.DeserializeObject<Gastronomy>(dr[1].ToString());

//                    GastronomyRelatedContentReduced data = new GastronomyRelatedContentReduced();

//                    data.Id = pgdata.Id;
//                    data.Name = pgdata.Detail != null ? pgdata.Detail.ContainsKey(language) ? pgdata.Detail[language].Title : "" : "";
//                    data.Type = pgdata.CategoryCodes != null ? String.Join(",", pgdata.CategoryCodes.Select(x => x.Shortname)) : null;

//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to PoiBaseInfosLocalized OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<ArticleBaseInfosLocalized> SelectFromTableDataAsArticleBaseInfosLocalizedLocalizedObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<ArticleBaseInfosLocalized> lstSelect = new List<ArticleBaseInfosLocalized>();
//                while (dr.Read())
//                {
//                    var poibaseinfo = JsonConvert.DeserializeObject<Article>(dr[1].ToString());

//                    ArticleBaseInfosLocalized data = new ArticleBaseInfosLocalized();

//                    data.Id = poibaseinfo.Id;
//                    data.LastChange = poibaseinfo.LastChange;
//                    data.FirstImport = poibaseinfo.FirstImport;
//                    data.Active = poibaseinfo.Active;
//                    data.AdditionalArticleInfos = poibaseinfo.AdditionalArticleInfos != null ? poibaseinfo.AdditionalArticleInfos.ContainsKey(language) ? poibaseinfo.AdditionalArticleInfos[language] : null : null;
//                    data.ContactInfos = poibaseinfo.ContactInfos != null ? poibaseinfo.ContactInfos.ContainsKey(language) ? poibaseinfo.ContactInfos[language] : null : null;
//                    data.Detail = poibaseinfo.Detail != null ? poibaseinfo.Detail.ContainsKey(language) ? poibaseinfo.Detail[language] : null : null;
//                    data.GpsInfo = poibaseinfo.GpsInfo;
//                    data.GpsTrack = poibaseinfo.GpsTrack;
//                    data.Highlight = poibaseinfo.Highlight;
//                    data.OperationSchedule = poibaseinfo.OperationSchedule;
//                    data.Shortname = poibaseinfo.Shortname;
//                    data.SmgActive = poibaseinfo.SmgActive;
//                    data.SubType = poibaseinfo.SubType;
//                    data.Type = poibaseinfo.Type;
//                    data.SmgTags = poibaseinfo.SmgTags;

//                    data.ImageGallery = poibaseinfo.ImageGallery != null ? poibaseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;

//                    //MarketingGroupIds = acco.MarketingGroupIds,                    





//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to PoiBaseInfosLocalized OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<ODHActivityPoiLocalized> SelectFromTableDataAsSmgPoiLocalizedLocalizedObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<ODHActivityPoiLocalized> lstSelect = new List<ODHActivityPoiLocalized>();
//                while (dr.Read())
//                {
//                    var poibaseinfo = JsonConvert.DeserializeObject<ODHActivityPoi>(dr[1].ToString());

//                    ODHActivityPoiLocalized data = new ODHActivityPoiLocalized();

//                    data.Id = poibaseinfo.Id;
//                    data.LastChange = poibaseinfo.LastChange;
//                    data.FirstImport = poibaseinfo.FirstImport;
//                    data.Active = poibaseinfo.Active;
//                    data.AdditionalPoiInfos = poibaseinfo.AdditionalPoiInfos != null ? poibaseinfo.AdditionalPoiInfos.Count > 0 ? poibaseinfo.AdditionalPoiInfos.ContainsKey(language) ? poibaseinfo.AdditionalPoiInfos[language] : null : null : null;
//                    data.ContactInfos = poibaseinfo.ContactInfos != null ? poibaseinfo.ContactInfos.ContainsKey(language) ? poibaseinfo.ContactInfos[language] : null : null;
//                    data.Detail = poibaseinfo.Detail != null ? poibaseinfo.Detail.ContainsKey(language) ? poibaseinfo.Detail[language] : null : null;
//                    data.GpsInfo = poibaseinfo.GpsInfo;
//                    data.GpsTrack = poibaseinfo.GpsTrack;
//                    data.GpsPoints = poibaseinfo.GpsPoints;
//                    data.Highlight = poibaseinfo.Highlight;
//                    data.OperationSchedule = poibaseinfo.OperationSchedule;
//                    data.Shortname = poibaseinfo.Shortname;
//                    data.SmgActive = poibaseinfo.SmgActive;
//                    data.SubType = poibaseinfo.SubType;
//                    data.Type = poibaseinfo.Type;
//                    data.SmgTags = poibaseinfo.SmgTags;
//                    data.ImageGallery = poibaseinfo.ImageGallery != null ? poibaseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
//                    data.AltitudeDifference = poibaseinfo.AltitudeDifference;
//                    data.AltitudeHighestPoint = poibaseinfo.AltitudeHighestPoint;
//                    data.AltitudeLowestPoint = poibaseinfo.AltitudeLowestPoint;
//                    data.AltitudeSumDown = poibaseinfo.AltitudeSumDown;
//                    data.AltitudeSumUp = poibaseinfo.AltitudeSumUp;
//                    data.AreaId = poibaseinfo.AreaId;
//                    data.Difficulty = poibaseinfo.Difficulty;
//                    data.DistanceDuration = poibaseinfo.DistanceDuration;
//                    data.DistanceLength = poibaseinfo.DistanceLength;
//                    data.Exposition = poibaseinfo.Exposition;
//                    data.FeetClimb = poibaseinfo.FeetClimb;
//                    data.HasFreeEntrance = poibaseinfo.HasFreeEntrance;
//                    data.HasRentals = poibaseinfo.HasRentals;
//                    data.IsOpen = poibaseinfo.IsOpen;
//                    data.IsPrepared = poibaseinfo.IsPrepared;
//                    data.IsWithLigth = poibaseinfo.IsWithLigth;
//                    data.LiftAvailable = poibaseinfo.LiftAvailable;
//                    data.BikeTransport = poibaseinfo.BikeTransport;
//                    data.PoiProperty = poibaseinfo.PoiProperty != null ? poibaseinfo.PoiProperty.ContainsKey(language) ? poibaseinfo.PoiProperty[language] : null : null;
//                    data.PoiServices = poibaseinfo.PoiServices;
//                    data.PoiType = poibaseinfo.PoiType;
//                    data.Ratings = poibaseinfo.Ratings;
//                    data.RunToValley = poibaseinfo.RunToValley;
//                    data.SmgId = poibaseinfo.SmgId;
//                    data.SubType = poibaseinfo.SubType;
//                    data.TourismorganizationId = poibaseinfo.TourismorganizationId;
//                    data.AgeFrom = poibaseinfo.AgeFrom;
//                    data.AgeTo = poibaseinfo.AgeTo;
//                    data.SyncSourceInterface = poibaseinfo.SyncSourceInterface;
//                    data.SyncUpdateMode = poibaseinfo.SyncUpdateMode;
//                    data.Source = poibaseinfo.Source;
//                    //data.MaxSeatingCapacity = poibaseinfo.MaxSeatingCapacity weiter

//                    var distinfolocalized = new DistrictInfoLocalized() { };
//                    if (poibaseinfo.LocationInfo != null)
//                    {
//                        if (poibaseinfo.LocationInfo.DistrictInfo != null)
//                        {
//                            distinfolocalized.Id = poibaseinfo.LocationInfo.DistrictInfo.Id;
//                            distinfolocalized.Name = poibaseinfo.LocationInfo.DistrictInfo.Name.ContainsKey(language) ? poibaseinfo.LocationInfo.DistrictInfo.Name[language] : "";
//                        }
//                    }

//                    var muninfolocalized = new MunicipalityInfoLocalized() { };
//                    if (poibaseinfo.LocationInfo != null)
//                    {
//                        if (poibaseinfo.LocationInfo.MunicipalityInfo != null)
//                        {
//                            muninfolocalized.Id = poibaseinfo.LocationInfo.MunicipalityInfo.Id;
//                            muninfolocalized.Name = poibaseinfo.LocationInfo.MunicipalityInfo.Name.ContainsKey(language) ? poibaseinfo.LocationInfo.MunicipalityInfo.Name[language] : "";
//                        }
//                    }

//                    var reginfolocalized = new RegionInfoLocalized() { };
//                    if (poibaseinfo.LocationInfo != null)
//                    {
//                        if (poibaseinfo.LocationInfo.RegionInfo != null)
//                        {
//                            reginfolocalized.Id = poibaseinfo.LocationInfo.RegionInfo.Id;
//                            reginfolocalized.Name = poibaseinfo.LocationInfo.RegionInfo.Name.ContainsKey(language) ? poibaseinfo.LocationInfo.RegionInfo.Name[language] : "";
//                        }
//                    }

//                    var tvinfolocalized = new TvInfoLocalized() { };
//                    if (poibaseinfo.LocationInfo != null)
//                    {
//                        if (poibaseinfo.LocationInfo.TvInfo != null)
//                        {
//                            tvinfolocalized.Id = poibaseinfo.LocationInfo.TvInfo.Id;
//                            tvinfolocalized.Name = poibaseinfo.LocationInfo.TvInfo.Name.ContainsKey(language) ? poibaseinfo.LocationInfo.TvInfo.Name[language] : "";
//                        }
//                    }

//                    data.LocationInfo = new LocationInfoLocalized()
//                    {
//                        DistrictInfo = distinfolocalized,
//                        MunicipalityInfo = muninfolocalized,
//                        TvInfo = tvinfolocalized,
//                        RegionInfo = reginfolocalized
//                    };
//                    //MarketingGroupIds = acco.MarketingGroupIds,                    





//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to AccommodationLocalized OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<PackageLocalized> SelectFromTableDataAsPackageLocalizedObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<PackageLocalized> lstSelect = new List<PackageLocalized>();
//                while (dr.Read())
//                {
//                    var package = JsonConvert.DeserializeObject<Package>(dr[1].ToString());

//                    PackageLocalized data = new PackageLocalized();
//                    data.Active = package.Active;

//                    data.ChannelInfo = package.ChannelInfo;
//                    data.ChildrenMin = package.ChildrenMin;
//                    data.DaysArrival = package.DaysArrival;
//                    data.DaysArrivalMax = package.DaysArrivalMax;
//                    data.DaysArrivalMin = package.DaysArrivalMin;
//                    data.DaysDeparture = package.DaysDeparture;
//                    data.DaysDurMax = package.DaysDurMax;
//                    data.DaysDurMin = package.DaysDurMin;
//                    data.HotelHgvId = package.HotelHgvId;
//                    data.HotelId = package.HotelId;
//                    data.Id = package.Id;
//                    data.ImageGallery = package.ImageGallery != null ? package.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
//                    data.Inclusive = package.Inclusive != null ? package.Inclusive.Select(x => new InclusiveLocalized()
//                    {
//                        ImageGallery = x.Value.ImageGallery != null ? x.Value.ImageGallery.Select(y => new ImageGalleryLocalized() { Height = y.Height, ImageDesc = y.ImageDesc.ContainsKey(language) ? y.ImageDesc[language] : "", ImageName = y.ImageName, ImageSource = y.ImageSource, ImageTitle = y.ImageTitle.ContainsKey(language) ? y.ImageTitle[language] : "", ImageUrl = y.ImageUrl, IsInGallery = y.IsInGallery, ListPosition = y.ListPosition, ValidFrom = y.ValidFrom, ValidTo = y.ValidTo, Width = y.Width, CopyRight = y.CopyRight }).ToList() : null,
//                        PackageDetail = x.Value.PackageDetail != null ? x.Value.PackageDetail.ContainsKey(language) ? x.Value.PackageDetail[language] : null : null,
//                        PriceId = x.Value.PriceId,
//                        PriceTyp = x.Value.PriceTyp
//                    }).ToList() : null;
//                    data.LocationInfo = new LocationInfoLocalized()
//                    {
//                        DistrictInfo = package.LocationInfo != null ? package.LocationInfo.DistrictInfo != null ? new DistrictInfoLocalized() { Id = package.LocationInfo.DistrictInfo.Id, Name = package.LocationInfo.DistrictInfo.Name[language] } : new DistrictInfoLocalized() : new DistrictInfoLocalized(),
//                        MunicipalityInfo = package.LocationInfo != null ? package.LocationInfo.MunicipalityInfo != null ? new MunicipalityInfoLocalized() { Id = package.LocationInfo.MunicipalityInfo.Id, Name = package.LocationInfo.MunicipalityInfo.Name[language] } : new MunicipalityInfoLocalized() : new MunicipalityInfoLocalized(),
//                        TvInfo = package.LocationInfo != null ? package.LocationInfo.TvInfo != null ? new TvInfoLocalized() { Id = package.LocationInfo.TvInfo.Id, Name = package.LocationInfo.TvInfo.Name[language] } : new TvInfoLocalized() : new TvInfoLocalized(),
//                        RegionInfo = package.LocationInfo != null ? package.LocationInfo.RegionInfo != null ? new RegionInfoLocalized() { Id = package.LocationInfo.RegionInfo.Id, Name = package.LocationInfo.RegionInfo.Name[language] } : new RegionInfoLocalized() : new RegionInfoLocalized()
//                    };
//                    data.OfferId = package.OfferId;
//                    data.Offertyp = package.Offertyp;
//                    data.PackageDetail = package.PackageDetail != null ? package.PackageDetail.ContainsKey(language) ? package.PackageDetail[language] : null : null;
//                    data.Season = package.Season;
//                    data.Shortname = package.Shortname;
//                    data.SmgActive = package.SmgActive;
//                    data.SmgTags = package.SmgTags;
//                    data.Specialtyp = package.Specialtyp;
//                    data.ValidStart = package.ValidStart;
//                    data.ValidStop = package.ValidStop;
//                    data.Services = package.Services;
//                    data.PackageThemeDetail = package.PackageThemeDetail != null ? package.PackageThemeDetail.Select(x => new PackageThemeLocalized()
//                    {
//                        ThemeId = x.ThemeId,
//                        ThemeDetail = x.ThemeDetail.ContainsKey(language) ? x.ThemeDetail[language] : null
//                    }).ToList() : null;



//                    //var distinfolocalized = new DistrictInfoLocalized() { };
//                    //if (acco.LocationInfo.DistrictInfo != null)
//                    //{
//                    //    distinfolocalized.Id = acco.LocationInfo.DistrictInfo.Id;
//                    //    distinfolocalized.Name = acco.LocationInfo.DistrictInfo.Name.ContainsKey(language) ? acco.LocationInfo.DistrictInfo.Name[language] : "";
//                    //}

//                    //var muninfolocalized = new MunicipalityInfoLocalized() { };
//                    //if (acco.LocationInfo.MunicipalityInfo != null)
//                    //{
//                    //    muninfolocalized.Id = acco.LocationInfo.MunicipalityInfo.Id;
//                    //    muninfolocalized.Name = acco.LocationInfo.MunicipalityInfo.Name.ContainsKey(language) ? acco.LocationInfo.MunicipalityInfo.Name[language] : "";
//                    //}

//                    //var reginfolocalized = new RegionInfoLocalized() { };
//                    //if (acco.LocationInfo.RegionInfo != null)
//                    //{
//                    //    reginfolocalized.Id = acco.LocationInfo.RegionInfo.Id;
//                    //    reginfolocalized.Name = acco.LocationInfo.RegionInfo.Name.ContainsKey(language) ? acco.LocationInfo.RegionInfo.Name[language] : "";
//                    //}

//                    //var tvinfolocalized = new TvInfoLocalized() { };
//                    //if (acco.LocationInfo.TvInfo != null)
//                    //{
//                    //    tvinfolocalized.Id = acco.LocationInfo.TvInfo.Id;
//                    //    tvinfolocalized.Name = acco.LocationInfo.TvInfo.Name.ContainsKey(language) ? acco.LocationInfo.TvInfo.Name[language] : "";
//                    //}

//                    //data.LocationInfo = new LocationInfoLocalized()
//                    //{
//                    //    DistrictInfo = distinfolocalized,
//                    //    MunicipalityInfo = muninfolocalized,
//                    //    TvInfo = tvinfolocalized,
//                    //    RegionInfo = reginfolocalized
//                    //};


//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to PackageBookListObject OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<PackageBookList> SelectFromTableDataAsPackageBookListObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<PackageBookList> lstSelect = new List<PackageBookList>();
//                while (dr.Read())
//                {
//                    var package = JsonConvert.DeserializeObject<Package>(dr[1].ToString());

//                    PackageBookList data = new PackageBookList();
//                    data.Id = package.Id;
//                    data.OfferId = package.OfferId;
//                    data.HotelId = package.HotelId;
//                    data.HotelHgvId = package.HotelHgvId;

//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to PoiBaseInfosLocalized OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<BaseInfosLocalized> SelectFromTableDataAsBaseInfosLocalizedObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<BaseInfosLocalized> lstSelect = new List<BaseInfosLocalized>();
//                while (dr.Read())
//                {
//                    var baseinfo = JsonConvert.DeserializeObject<BaseInfos>(dr[1].ToString());

//                    BaseInfosLocalized data = new BaseInfosLocalized();

//                    data.Id = baseinfo.Id;
//                    data.Active = baseinfo.Active;
//                    data.ContactInfos = baseinfo.ContactInfos != null ? baseinfo.ContactInfos.ContainsKey(language) ? baseinfo.ContactInfos[language] : null : null;
//                    data.Detail = baseinfo.Detail != null ? baseinfo.Detail.ContainsKey(language) ? baseinfo.Detail[language] : null : null;
//                    data.Gpstype = baseinfo.Gpstype;
//                    data.Latitude = baseinfo.Latitude;
//                    data.Longitude = baseinfo.Longitude;
//                    data.Altitude = baseinfo.Altitude;
//                    data.AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure;
//                    data.ImageGallery = baseinfo.ImageGallery != null ? baseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
//                    data.CustomId = baseinfo.CustomId;
//                    data.Shortname = baseinfo.Shortname;
//                    data.SmgActive = baseinfo.SmgActive;
//                    data.SmgTags = baseinfo.SmgTags;



//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to RegionLocalized OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<RegionLocalized> SelectFromTableDataAsRegionLocalizedObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<RegionLocalized> lstSelect = new List<RegionLocalized>();
//                while (dr.Read())
//                {
//                    var baseinfo = JsonConvert.DeserializeObject<Region>(dr[1].ToString());

//                    RegionLocalized data = new RegionLocalized();

//                    data.Id = baseinfo.Id;
//                    data.Active = baseinfo.Active;
//                    data.ContactInfos = baseinfo.ContactInfos != null ? baseinfo.ContactInfos.ContainsKey(language) ? baseinfo.ContactInfos[language] : null : null;
//                    data.Detail = baseinfo.Detail != null ? baseinfo.Detail.ContainsKey(language) ? baseinfo.Detail[language] : null : null;
//                    data.Gpstype = baseinfo.Gpstype;
//                    data.Latitude = baseinfo.Latitude;
//                    data.Longitude = baseinfo.Longitude;
//                    data.Altitude = baseinfo.Altitude;
//                    data.AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure;
//                    data.ImageGallery = baseinfo.ImageGallery != null ? baseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
//                    data.CustomId = baseinfo.CustomId;
//                    data.Shortname = baseinfo.Shortname;
//                    data.SmgActive = baseinfo.SmgActive;
//                    data.SmgTags = baseinfo.SmgTags;
//                    data.DetailThemed = baseinfo.DetailThemed != null ? baseinfo.DetailThemed.ContainsKey(language) ? baseinfo.DetailThemed[language] : null : null;
//                    data.GpsPolygon = baseinfo.GpsPolygon;
//                    data.Webcam = baseinfo.Webcam != null ? baseinfo.Webcam.Select(x => new WebcamLocalized() { GpsInfo = x.GpsInfo, WebcamId = x.WebcamId, Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "", Webcamurl = x.Webcamurl }).ToList() : null;
//                    data.VisibleInSearch = baseinfo.VisibleInSearch;
//                    data.SkiareaIds = baseinfo.SkiareaIds;

//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to TourismvereinLocalized OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<TourismvereinLocalized> SelectFromTableDataAsTourismvereinLocalizedObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<TourismvereinLocalized> lstSelect = new List<TourismvereinLocalized>();
//                while (dr.Read())
//                {
//                    var baseinfo = JsonConvert.DeserializeObject<Tourismverein>(dr[1].ToString());

//                    TourismvereinLocalized data = new TourismvereinLocalized();

//                    data.Id = baseinfo.Id;
//                    data.Active = baseinfo.Active;
//                    data.ContactInfos = baseinfo.ContactInfos != null ? baseinfo.ContactInfos.ContainsKey(language) ? baseinfo.ContactInfos[language] : null : null;
//                    data.Detail = baseinfo.Detail != null ? baseinfo.Detail.ContainsKey(language) ? baseinfo.Detail[language] : null : null;
//                    data.Gpstype = baseinfo.Gpstype;
//                    data.Latitude = baseinfo.Latitude;
//                    data.Longitude = baseinfo.Longitude;
//                    data.Altitude = baseinfo.Altitude;
//                    data.AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure;
//                    data.ImageGallery = baseinfo.ImageGallery != null ? baseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
//                    data.CustomId = baseinfo.CustomId;
//                    data.Shortname = baseinfo.Shortname;
//                    data.SmgActive = baseinfo.SmgActive;
//                    data.SmgTags = baseinfo.SmgTags;
//                    data.GpsPolygon = baseinfo.GpsPolygon;
//                    data.Webcam = baseinfo.Webcam != null ? baseinfo.Webcam.Select(x => new WebcamLocalized() { GpsInfo = x.GpsInfo, WebcamId = x.WebcamId, Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "", Webcamurl = x.Webcamurl }).ToList() : null;
//                    data.VisibleInSearch = baseinfo.VisibleInSearch;
//                    data.SkiareaIds = baseinfo.SkiareaIds;
//                    data.RegionId = baseinfo.RegionId;

//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to MunicipalityLocalized OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<MunicipalityLocalized> SelectFromTableDataAsMunicipalityLocalizedObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<MunicipalityLocalized> lstSelect = new List<MunicipalityLocalized>();
//                while (dr.Read())
//                {
//                    var baseinfo = JsonConvert.DeserializeObject<Municipality>(dr[1].ToString());

//                    MunicipalityLocalized data = new MunicipalityLocalized();

//                    data.Id = baseinfo.Id;
//                    data.Active = baseinfo.Active;
//                    data.ContactInfos = baseinfo.ContactInfos != null ? baseinfo.ContactInfos.ContainsKey(language) ? baseinfo.ContactInfos[language] : null : null;
//                    data.Detail = baseinfo.Detail != null ? baseinfo.Detail.ContainsKey(language) ? baseinfo.Detail[language] : null : null;
//                    data.Gpstype = baseinfo.Gpstype;
//                    data.Latitude = baseinfo.Latitude;
//                    data.Longitude = baseinfo.Longitude;
//                    data.Altitude = baseinfo.Altitude;
//                    data.AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure;
//                    data.ImageGallery = baseinfo.ImageGallery != null ? baseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
//                    data.CustomId = baseinfo.CustomId;
//                    data.Shortname = baseinfo.Shortname;
//                    data.SmgActive = baseinfo.SmgActive;
//                    data.SmgTags = baseinfo.SmgTags;
//                    data.GpsPolygon = baseinfo.GpsPolygon;
//                    data.Webcam = baseinfo.Webcam != null ? baseinfo.Webcam.Select(x => new WebcamLocalized() { GpsInfo = x.GpsInfo, WebcamId = x.WebcamId, Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "", Webcamurl = x.Webcamurl }).ToList() : null;
//                    data.VisibleInSearch = baseinfo.VisibleInSearch;
//                    data.RegionId = baseinfo.RegionId;
//                    data.Plz = baseinfo.Plz;
//                    data.TourismvereinId = baseinfo.TourismvereinId;
//                    data.SiagId = baseinfo.SiagId;
//                    data.Inhabitants = baseinfo.Inhabitants;
//                    data.IstatNumber = baseinfo.IstatNumber;

//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to DistrictLocalized OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<DistrictLocalized> SelectFromTableDataAsDistrictLocalizedObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<DistrictLocalized> lstSelect = new List<DistrictLocalized>();
//                while (dr.Read())
//                {
//                    var baseinfo = JsonConvert.DeserializeObject<District>(dr[1].ToString());

//                    DistrictLocalized data = new DistrictLocalized();

//                    data.Id = baseinfo.Id;
//                    data.Active = baseinfo.Active;
//                    data.ContactInfos = baseinfo.ContactInfos != null ? baseinfo.ContactInfos.ContainsKey(language) ? baseinfo.ContactInfos[language] : null : null;
//                    data.Detail = baseinfo.Detail != null ? baseinfo.Detail.ContainsKey(language) ? baseinfo.Detail[language] : null : null;
//                    data.Gpstype = baseinfo.Gpstype;
//                    data.Latitude = baseinfo.Latitude;
//                    data.Longitude = baseinfo.Longitude;
//                    data.Altitude = baseinfo.Altitude;
//                    data.AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure;
//                    data.ImageGallery = baseinfo.ImageGallery != null ? baseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
//                    data.CustomId = baseinfo.CustomId;
//                    data.Shortname = baseinfo.Shortname;
//                    data.SmgActive = baseinfo.SmgActive;
//                    data.SmgTags = baseinfo.SmgTags;
//                    data.GpsPolygon = baseinfo.GpsPolygon;
//                    data.Webcam = baseinfo.Webcam != null ? baseinfo.Webcam.Select(x => new WebcamLocalized() { GpsInfo = x.GpsInfo, WebcamId = x.WebcamId, Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "", Webcamurl = x.Webcamurl }).ToList() : null;
//                    data.VisibleInSearch = baseinfo.VisibleInSearch;
//                    data.RegionId = baseinfo.RegionId;
//                    data.IsComune = baseinfo.IsComune;
//                    data.TourismvereinId = baseinfo.TourismvereinId;
//                    data.SiagId = baseinfo.SiagId;
//                    data.MunicipalityId = baseinfo.MunicipalityId;

//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to MetaRegionLocalized OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<MetaRegionLocalized> SelectFromTableDataAsMetaRegionLocalizedObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<MetaRegionLocalized> lstSelect = new List<MetaRegionLocalized>();
//                while (dr.Read())
//                {
//                    var baseinfo = JsonConvert.DeserializeObject<MetaRegion>(dr[1].ToString());

//                    MetaRegionLocalized data = new MetaRegionLocalized();

//                    data.Id = baseinfo.Id;
//                    data.Active = baseinfo.Active;
//                    data.ContactInfos = baseinfo.ContactInfos != null ? baseinfo.ContactInfos.ContainsKey(language) ? baseinfo.ContactInfos[language] : null : null;
//                    data.Detail = baseinfo.Detail != null ? baseinfo.Detail.ContainsKey(language) ? baseinfo.Detail[language] : null : null;
//                    data.Gpstype = baseinfo.Gpstype;
//                    data.Latitude = baseinfo.Latitude;
//                    data.Longitude = baseinfo.Longitude;
//                    data.Altitude = baseinfo.Altitude;
//                    data.AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure;
//                    data.ImageGallery = baseinfo.ImageGallery != null ? baseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
//                    data.CustomId = baseinfo.CustomId;
//                    data.Shortname = baseinfo.Shortname;
//                    data.SmgActive = baseinfo.SmgActive;
//                    data.SmgTags = baseinfo.SmgTags;
//                    data.GpsPolygon = baseinfo.GpsPolygon;
//                    data.Webcam = baseinfo.Webcam != null ? baseinfo.Webcam.Select(x => new WebcamLocalized() { GpsInfo = x.GpsInfo, WebcamId = x.WebcamId, Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "", Webcamurl = x.Webcamurl }).ToList() : null;
//                    data.VisibleInSearch = baseinfo.VisibleInSearch;
//                    data.DistrictIds = baseinfo.DistrictIds;
//                    data.TourismvereinIds = baseinfo.TourismvereinIds;
//                    data.RegionIds = baseinfo.RegionIds;
//                    data.DetailThemed = baseinfo.DetailThemed != null ? baseinfo.DetailThemed.ContainsKey(language) ? baseinfo.DetailThemed[language] : null : null;


//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to MetaRegionLocalized OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<ExperienceAreaLocalized> SelectFromTableDataAsExperienceAreaLocalizedObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<ExperienceAreaLocalized> lstSelect = new List<ExperienceAreaLocalized>();
//                while (dr.Read())
//                {
//                    var baseinfo = JsonConvert.DeserializeObject<ExperienceArea>(dr[1].ToString());

//                    ExperienceAreaLocalized data = new ExperienceAreaLocalized();

//                    data.Id = baseinfo.Id;
//                    data.Active = baseinfo.Active;
//                    data.ContactInfos = baseinfo.ContactInfos != null ? baseinfo.ContactInfos.ContainsKey(language) ? baseinfo.ContactInfos[language] : null : null;
//                    data.Detail = baseinfo.Detail != null ? baseinfo.Detail.ContainsKey(language) ? baseinfo.Detail[language] : null : null;
//                    data.Gpstype = baseinfo.Gpstype;
//                    data.Latitude = baseinfo.Latitude;
//                    data.Longitude = baseinfo.Longitude;
//                    data.Altitude = baseinfo.Altitude;
//                    data.AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure;
//                    data.ImageGallery = baseinfo.ImageGallery != null ? baseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
//                    data.CustomId = baseinfo.CustomId;
//                    data.Shortname = baseinfo.Shortname;
//                    data.SmgActive = baseinfo.SmgActive;
//                    data.SmgTags = baseinfo.SmgTags;
//                    data.GpsPolygon = baseinfo.GpsPolygon;
//                    //data.Webcam = baseinfo.Webcam != null ? baseinfo.Webcam.Select(x => new WebcamLocalized() { GpsInfo = x.GpsInfo, WebcamId = x.WebcamId, Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "", Webcamurl = x.Webcamurl }).ToList() : null;
//                    data.VisibleInSearch = baseinfo.VisibleInSearch;
//                    data.DistrictIds = baseinfo.DistrictIds;
//                    data.TourismvereinIds = baseinfo.TourismvereinIds;
//                    //data.RegionIds = baseinfo.RegionIds;
//                    //data.DetailThemed = baseinfo.DetailThemed != null ? baseinfo.DetailThemed.ContainsKey(language) ? baseinfo.DetailThemed[language] : null : null;


//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to SkiRegionLocalized OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<SkiRegionLocalized> SelectFromTableDataAsSkiRegionLocalizedObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<SkiRegionLocalized> lstSelect = new List<SkiRegionLocalized>();
//                while (dr.Read())
//                {
//                    var baseinfo = JsonConvert.DeserializeObject<SkiRegion>(dr[1].ToString());

//                    SkiRegionLocalized data = new SkiRegionLocalized();

//                    data.Id = baseinfo.Id;
//                    data.Active = baseinfo.Active;
//                    data.ContactInfos = baseinfo.ContactInfos != null ? baseinfo.ContactInfos.ContainsKey(language) ? baseinfo.ContactInfos[language] : null : null;
//                    data.Detail = baseinfo.Detail != null ? baseinfo.Detail.ContainsKey(language) ? baseinfo.Detail[language] : null : null;
//                    data.Gpstype = baseinfo.Gpstype;
//                    data.Latitude = baseinfo.Latitude;
//                    data.Longitude = baseinfo.Longitude;
//                    data.Altitude = baseinfo.Altitude;
//                    data.AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure;
//                    data.ImageGallery = baseinfo.ImageGallery != null ? baseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
//                    data.CustomId = baseinfo.CustomId;
//                    data.Shortname = baseinfo.Shortname;
//                    data.SmgActive = baseinfo.SmgActive;
//                    data.SmgTags = baseinfo.SmgTags;
//                    data.GpsPolygon = baseinfo.GpsPolygon;
//                    data.Webcam = baseinfo.Webcam != null ? baseinfo.Webcam.Select(x => new WebcamLocalized() { GpsInfo = x.GpsInfo, WebcamId = x.WebcamId, Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "", Webcamurl = x.Webcamurl }).ToList() : null;

//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped to SkiAreaLocalized OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<SkiAreaLocalized> SelectFromTableDataAsSkiAreaLocalizedObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<SkiAreaLocalized> lstSelect = new List<SkiAreaLocalized>();
//                while (dr.Read())
//                {
//                    var baseinfo = JsonConvert.DeserializeObject<SkiArea>(dr[1].ToString());

//                    SkiAreaLocalized data = new SkiAreaLocalized();

//                    data.Id = baseinfo.Id;
//                    data.Active = baseinfo.Active;
//                    data.ContactInfos = baseinfo.ContactInfos != null ? baseinfo.ContactInfos.ContainsKey(language) ? baseinfo.ContactInfos[language] : null : null;
//                    data.Detail = baseinfo.Detail != null ? baseinfo.Detail.ContainsKey(language) ? baseinfo.Detail[language] : null : null;
//                    data.Gpstype = baseinfo.Gpstype;
//                    data.Latitude = baseinfo.Latitude;
//                    data.Longitude = baseinfo.Longitude;
//                    data.Altitude = baseinfo.Altitude;
//                    data.AltitudeUnitofMeasure = baseinfo.AltitudeUnitofMeasure;
//                    data.ImageGallery = baseinfo.ImageGallery != null ? baseinfo.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc.ContainsKey(language) ? x.ImageDesc[language] : "" : "", ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle.ContainsKey(language) ? x.ImageTitle[language] : "" : "", ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList() : null;
//                    data.CustomId = baseinfo.CustomId;
//                    data.Shortname = baseinfo.Shortname;
//                    data.SmgActive = baseinfo.SmgActive;
//                    data.SmgTags = baseinfo.SmgTags;
//                    data.GpsPolygon = baseinfo.GpsPolygon;
//                    data.Webcam = baseinfo.Webcam != null ? baseinfo.Webcam.Select(x => new WebcamLocalized() { GpsInfo = x.GpsInfo, WebcamId = x.WebcamId, Webcamname = x.Webcamname.ContainsKey(language) ? x.Webcamname[language] : "", Webcamurl = x.Webcamurl }).ToList() : null;

//                    data.SkiRegionId = baseinfo.SkiRegionId;
//                    data.SkiAreaMapURL = baseinfo.SkiAreaMapURL;
//                    data.TotalSlopeKm = baseinfo.TotalSlopeKm;
//                    data.SlopeKmBlue = baseinfo.SlopeKmBlue;
//                    data.SlopeKmRed = baseinfo.SlopeKmRed;
//                    data.SlopeKmBlack = baseinfo.SlopeKmBlack;
//                    data.LiftCount = baseinfo.LiftCount;
//                    data.AltitudeFrom = baseinfo.AltitudeFrom;
//                    data.AltitudeTo = baseinfo.AltitudeTo;
//                    data.SkiRegionName = baseinfo.SkiRegionName != null ? baseinfo.SkiRegionName.ContainsKey(language) ? baseinfo.SkiRegionName[language] : "" : "";
//                    data.AreaId = baseinfo.AreaId;
//                    data.OperationSchedule = baseinfo.OperationSchedule;
//                    data.TourismvereinIds = baseinfo.TourismvereinIds;
//                    data.RegionIds = baseinfo.RegionIds;
//                    data.LocationInfo = new LocationInfoLocalized()
//                    {
//                        DistrictInfo = baseinfo.LocationInfo != null ? baseinfo.LocationInfo.DistrictInfo != null ? new DistrictInfoLocalized() { Id = baseinfo.LocationInfo.DistrictInfo.Id, Name = baseinfo.LocationInfo.DistrictInfo.Name[language] } : new DistrictInfoLocalized() : new DistrictInfoLocalized(),
//                        MunicipalityInfo = baseinfo.LocationInfo != null ? baseinfo.LocationInfo.MunicipalityInfo != null ? new MunicipalityInfoLocalized() { Id = baseinfo.LocationInfo.MunicipalityInfo.Id, Name = baseinfo.LocationInfo.MunicipalityInfo.Name[language] } : new MunicipalityInfoLocalized() : new MunicipalityInfoLocalized(),
//                        TvInfo = baseinfo.LocationInfo != null ? baseinfo.LocationInfo.TvInfo != null ? new TvInfoLocalized() { Id = baseinfo.LocationInfo.TvInfo.Id, Name = baseinfo.LocationInfo.TvInfo.Name[language] } : new TvInfoLocalized() : new TvInfoLocalized(),
//                        RegionInfo = baseinfo.LocationInfo != null ? baseinfo.LocationInfo.RegionInfo != null ? new RegionInfoLocalized() { Id = baseinfo.LocationInfo.RegionInfo.Id, Name = baseinfo.LocationInfo.RegionInfo.Name[language] } : new RegionInfoLocalized() : new RegionInfoLocalized()

//                    };

//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }


//        //Mobile

//        /// <summary>
//        /// Returns List of JSON Data mapped from Accommodation to MobileData OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<MobileData> SelectFromTableDataTransformAccoToMobileDataObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<MobileData> lstSelect = new List<MobileData>();
//                while (dr.Read())
//                {
//                    var acommodation = JsonConvert.DeserializeObject<Accommodation>(dr[1].ToString());

//                    var nulldouble = 0;

//                    MobileData data = new MobileData();

//                    data.Id = acommodation.Id;
//                    data.Name = acommodation.AccoDetail[language] != null ? acommodation.AccoDetail[language].Name != null ? acommodation.AccoDetail[language].Name : "" : "";
//                    data.Image = acommodation.ImageGallery.Count > 0 ? acommodation.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ? acommodation.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl : acommodation.ImageGallery.FirstOrDefault().ImageUrl : "noimage";
//                    data.Region = acommodation.LocationInfo.RegionInfo.Name[language];
//                    data.Tourismverein = acommodation.LocationInfo.TvInfo.Name[language];
//                    data.Latitude = acommodation.Latitude;
//                    data.Longitude = acommodation.Longitude;
//                    data.Type = "acco";
//                    data.Category = new Dictionary<string, string>()
//                                                 {
//                                                     //Smgpoi
//                                                     { "maintype", "" },
//                                                     { "subtype", "" },
//                                                     { "poitype", "" },
//                                                     //Accommodation
//                                                     { "board", String.Join(",", acommodation.BoardIds) },
//                                                     { "type", acommodation.AccoTypeId },
//                                                     { "category", acommodation.AccoCategoryId },
//                                                     { "theme", String.Join(",", acommodation.ThemeIds) },
//                                                     { "badge", String.Join(",", acommodation.BadgeIds) },
//                                                     { "specialfeature", String.Join(",", acommodation.SpecialFeaturesIds) },
//                                                     // //Gastronomy
//                                                     //{ "categorycodes", "" },                                                     
//                                                     //{ "ceremonycodes", "" },
//                                                     //{ "dishcodes", "" },
//                                                     ////EventTopic
//                                                     //{ "EventTopic ", "" }
//                                                 };
//                    data.Additional = new Dictionary<string, string>()
//                                                 {
//                                                     //SmgPoi
//                                                     { "altitudedifference", nulldouble.ToString() },
//                                                     { "distanceduration", nulldouble.ToString() },
//                                                     { "difficulty", nulldouble.ToString() },
//                                                     { "distancelength", nulldouble.ToString() },
//                                                     //Accommodation
//                                                     { "trustyouid", acommodation.TrustYouID != null ? acommodation.TrustYouID.ToString() : "" },
//                                                     { "trustyouscore", acommodation.TrustYouScore.ToString() },
//                                                     { "trustyouratings", acommodation.TrustYouResults.ToString() },        
//                                                     //Gastronomy (no property)
//                                                     //Event
//                                                     { "topevent", "false" },
//                                                     { "begindate", "" },
//                                                     { "enddate", "" }
//                                                 };



//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped from SmgPoi to MobileData OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<MobileData> SelectFromTableDataTransformSmgPoiToMobileDataObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<MobileData> lstSelect = new List<MobileData>();
//                while (dr.Read())
//                {
//                    var smgpoi = JsonConvert.DeserializeObject<SmgPoi>(dr[1].ToString());

//                    var nulldouble = 0;

//                    MobileData data = new MobileData();

//                    data.Id = smgpoi.Id;
//                    data.Name = smgpoi.Detail[language] != null ? smgpoi.Detail[language].Title != null ? smgpoi.Detail[language].Title : "" : "";
//                    //Wenn Image leer "noimage", wenn ImageListpos == 0 
//                    //Image = activity.ImageGallery != null ? activity.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ? activity.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageSource == "LTS" ? activity.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl + "&W=200" : activity.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl + "&width=200" : activity.ImageGallery.FirstOrDefault().ImageUrl + "&width=200" : "noimage",
//                    data.Image = smgpoi.ImageGallery.Count > 0 ? smgpoi.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ? smgpoi.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl : smgpoi.ImageGallery.FirstOrDefault().ImageUrl : "noimage";
//                    data.Region = smgpoi.LocationInfo.RegionInfo != null ? smgpoi.LocationInfo.RegionInfo.Name[language] : "";
//                    data.Tourismverein = smgpoi.LocationInfo.TvInfo != null ? smgpoi.LocationInfo.TvInfo.Name[language] : "";
//                    data.Latitude = smgpoi.GpsInfo != null ? smgpoi.GpsInfo.Count > 0 ? smgpoi.GpsInfo.Where(x => x.Gpstype == "position").Count() > 0 ? smgpoi.GpsInfo.Where(x => x.Gpstype == "position").FirstOrDefault().Latitude : smgpoi.GpsInfo.FirstOrDefault().Latitude : 0 : 0;
//                    data.Longitude = smgpoi.GpsInfo != null ? smgpoi.GpsInfo.Count > 0 ? smgpoi.GpsInfo.Where(x => x.Gpstype == "position").Count() > 0 ? smgpoi.GpsInfo.Where(x => x.Gpstype == "position").FirstOrDefault().Longitude : smgpoi.GpsInfo.FirstOrDefault().Longitude : 0 : 0;
//                    data.Type = "smgpoi";
//                    data.Category = new Dictionary<string, string>()
//                                                 {
//                                                     //Smgpoi
//                                                     { "maintype", smgpoi.AdditionalPoiInfos[language].MainType },
//                                                     { "subtype", smgpoi.AdditionalPoiInfos[language].SubType },
//                                                     { "poitype", smgpoi.AdditionalPoiInfos[language].PoiType },
//                                                     //Accommodation
//                                                     { "board", "" },
//                                                     { "type", "" },
//                                                     { "category", "" },
//                                                     { "theme", "" },
//                                                     { "badge", "" },
//                                                     { "specialfeature", "" },
//                                                     ////Gastronomy
//                                                     //{ "categorycodes", "" },                                                     
//                                                     //{ "ceremonycodes", "" },
//                                                     //{ "dishcodes", "" },
//                                                     ////Events
//                                                     //{ "eventtopic ", "" }
//                                                 };
//                    data.Additional = new Dictionary<string, string>()
//                                                 {
//                                                     //SmgPoi
//                                                     { "altitudedifference", smgpoi.AltitudeDifference.ToString() },
//                                                     { "distanceduration", smgpoi.DistanceDuration.ToString() },
//                                                     { "difficulty", smgpoi.Difficulty != null ? smgpoi.Difficulty : nulldouble.ToString() },
//                                                     { "distancelength", smgpoi.DistanceLength.ToString() },
//                                                     //Accommodation
//                                                     { "trustyouid", "" },
//                                                     { "trustyouscore", nulldouble.ToString() },
//                                                     { "trustyouratings", nulldouble.ToString() },   
//                                                     //Gastronomy
//                                                     //Event
//                                                     { "topevent", "false" },
//                                                     { "begindate", "" },
//                                                     { "enddate", "" }
//                                                 };



//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped from Event to MobileData OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<MobileData> SelectFromTableDataTransformEventToMobileDataObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<MobileData> lstSelect = new List<MobileData>();
//                while (dr.Read())
//                {
//                    var eventinfo = JsonConvert.DeserializeObject<Event>(dr[1].ToString());

//                    var nulldouble = 0;

//                    MobileData data = new MobileData();

//                    data.Id = eventinfo.Id;
//                    data.Name = eventinfo.Detail[language] != null ? eventinfo.Detail[language].Title != null ? eventinfo.Detail[language].Title : "" : "";
//                    //Image = eventinfo.ImageGallery != null ? eventinfo.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ? eventinfo.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageSource == "LTS" ? eventinfo.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl + "&W=200" : eventinfo.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl + "&width=200" : eventinfo.ImageGallery.FirstOrDefault().ImageUrl + "&width=200" : "noimage",
//                    data.Image = eventinfo.ImageGallery.Count > 0 ? eventinfo.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ? eventinfo.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl : eventinfo.ImageGallery.FirstOrDefault().ImageUrl : "noimage";
//                    data.Region = eventinfo.LocationInfo.RegionInfo.Name[language];
//                    data.Tourismverein = eventinfo.LocationInfo.TvInfo.Name[language];
//                    data.Latitude = eventinfo.Latitude;
//                    data.Longitude = eventinfo.Longitude;
//                    data.Type = "event";
//                    data.Category = new Dictionary<string, string>()
//                                                 {
//                                                     //Smgpoi
//                                                     { "maintype", "" },
//                                                     { "subtype", "" },
//                                                     { "poitype", "" },
//                                                     //Accommodation
//                                                     { "board", "" },
//                                                     { "type", "" },
//                                                     { "category", "" },
//                                                     { "theme", "" },
//                                                     { "badge", "" },
//                                                     { "specialfeature", "" },
//                                                     // //Gastronomy
//                                                     //{ "categorycodes", "" },                                                     
//                                                     //{ "ceremonycodes", "" },
//                                                     //{ "dishcodes", "" },
//                                                     ////Event
//                                                     //{ "eventtopic", String.Join(",", eventinfo.TopicRIDs) }
//                                                 };
//                    data.Additional = new Dictionary<string, string>()
//                                                 {
//                                                     //SmgPoi
//                                                     { "altitudedifference", nulldouble.ToString() },
//                                                     { "distanceduration", nulldouble.ToString() },
//                                                     { "difficulty", nulldouble.ToString() },
//                                                     { "distancelength", nulldouble.ToString() },
//                                                     //Accommodation
//                                                     { "trustyouscore", nulldouble.ToString() },
//                                                     { "trustyouratings", nulldouble.ToString() },  
//                                                     //Gastronomy
//                                                     //Event                                                     
//                                                     { "topevent", eventinfo.SmgTags != null ? eventinfo.SmgTags.Contains("TopEvent") ? "true" : "false" : "false" },
//                                                     { "begindate", String.Format("{0:MM/dd/yyyy}", eventinfo.DateBegin) },
//                                                     { "enddate", String.Format("{0:MM/dd/yyyy}", eventinfo.DateEnd) }
//                                                     //{ "begindate", ((DateTime)eventinfo.DateBegin).ToShortDateString() },
//                                                     //{ "enddate", ((DateTime)eventinfo.DateEnd).ToShortDateString() }

//                                                 };



//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns List of JSON Data mapped from SmgPoi to MobileDataExtended OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<MobileDataExtended> SelectFromTableDataTransformSmgPoisToMobileDataExtendedObject(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<MobileDataExtended> lstSelect = new List<MobileDataExtended>();
//                while (dr.Read())
//                {
//                    var activity = JsonConvert.DeserializeObject<ODHActivityPoi>(dr[1].ToString());

//                    var nulldouble = 0;

//                    MobileDataExtended data = new MobileDataExtended();

//                    data.Id = activity.Id;
//                    data.Name = activity.Detail[language] != null ? activity.Detail[language].Title != null ? activity.Detail[language].Title : "" : "";
//                    //Wenn Image leer "noimage", wenn ImageListpos == 0 
//                    data.ShortDesc = activity.Detail[language] != null ? activity.Detail[language].MetaDesc != null ? activity.Detail[language].MetaDesc : "" : "";
//                    data.Image = activity.ImageGallery.Count > 0 ? activity.ImageGallery.Where(x => x.ListPosition == 0).Count() > 0 ? activity.ImageGallery.Where(x => x.ListPosition == 0).FirstOrDefault().ImageUrl : activity.ImageGallery.FirstOrDefault().ImageUrl : "noimage";
//                    data.Region = activity.LocationInfo.RegionInfo.Name[language];
//                    data.Tourismverein = activity.LocationInfo.TvInfo.Name[language];
//                    data.Latitude = activity.GpsInfo != null ? activity.GpsInfo.Count > 0 ? activity.GpsInfo.Where(x => x.Gpstype == "position").Count() > 0 ? activity.GpsInfo.Where(x => x.Gpstype == "position").FirstOrDefault().Latitude : activity.GpsInfo.FirstOrDefault().Latitude : 0 : 0;
//                    data.Longitude = activity.GpsInfo != null ? activity.GpsInfo.Count > 0 ? activity.GpsInfo.Where(x => x.Gpstype == "position").Count() > 0 ? activity.GpsInfo.Where(x => x.Gpstype == "position").FirstOrDefault().Longitude : activity.GpsInfo.FirstOrDefault().Longitude : 0 : 0;
//                    data.Type = "smgpoi";
//                    data.Category = new Dictionary<string, string>()
//                                                 {
//                                                     //Smgpoi
//                                                     { "maintype", activity.AdditionalPoiInfos[language].MainType },
//                                                     { "subtype", activity.AdditionalPoiInfos[language].SubType },
//                                                     { "poitype", activity.AdditionalPoiInfos[language].PoiType },
//                                                     //Accommodation
//                                                     { "board", "" },
//                                                     { "type", "" },
//                                                     { "category", "" },
//                                                     { "theme", "" },
//                                                     { "badge", "" },
//                                                     { "specialfeature", "" },
//                                                     ////Gastronomy
//                                                     //{ "categorycodes", "" },                                                     
//                                                     //{ "ceremonycodes", "" },
//                                                     //{ "dishcodes", "" },
//                                                     ////Events
//                                                     //{ "eventtopic ", "" }
//                                                 };
//                    data.Additional = new Dictionary<string, string>()
//                                                 {
//                                                     //SmgPoi
//                                                     { "altitudedifference", activity.AltitudeDifference.ToString() },
//                                                     { "distanceduration", activity.DistanceDuration.ToString() },
//                                                     { "difficulty", activity.Difficulty != null ? activity.Difficulty : nulldouble.ToString() },
//                                                     { "distancelength", activity.DistanceLength.ToString() },
//                                                     //Accommodation
//                                                     { "trustyouid", "" },
//                                                     { "trustyouscore", nulldouble.ToString() },
//                                                     { "trustyouratings", nulldouble.ToString() },   
//                                                     //Gastronomy
//                                                     //Event
//                                                     { "topevent", "false" },
//                                                     { "begindate", "" },
//                                                     { "enddate", "" }
//                                                 };
//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns Single JSON Data mapped to given OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static MobileDetail SelectFromTableDataAsEventMobileDetail(NpgsqlConnection conn, string tablename, string id, string language)
//        {
//            try
//            {
//                string whereexp = "Id LIKE '" + id + "'";
//                string commandText = CreatetDatabaseCommand("*", tablename, whereexp, "", null, 0);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;

//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<MobileDetail> lstSelect = new List<MobileDetail>();
//                while (dr.Read())
//                {
//                    var myevent = JsonConvert.DeserializeObject<Event>(dr[1].ToString());

//                    MobileDetail data = new MobileDetail();
//                    data.Id = myevent.Id;
//                    data.AltitudeDifference = 0;
//                    data.AltitudeHighestPoint = 0;
//                    data.AltitudeLowestPoint = 0;
//                    data.AltitudeSumDown = 0;
//                    data.AltitudeSumUp = 0;
//                    //CapacityCeremony = null,
//                    //CategoryCodes = null,
//                    data.ContactInfos = myevent.ContactInfos[language];
//                    data.DateBegin = myevent.DateBegin;
//                    data.DateEnd = myevent.DateEnd;
//                    data.Detail = myevent.Detail[language];
//                    data.Difficulty = "";
//                    //DishRates = null,
//                    data.DistanceDuration = 0;
//                    data.DistanceLength = 0;
//                    data.EventDate = myevent.EventDate;
//                    data.Exposition = null;
//                    //Facilities = null,
//                    data.FeetClimb = false;
//                    data.GpsInfo = Enumerable.Repeat<GpsInfo>(new GpsInfo() { Altitude = myevent.Altitude, Latitude = myevent.Latitude, Longitude = myevent.Longitude, Gpstype = "position" }, 1).ToList();
//                    data.GpsTrack = null;
//                    data.HasFreeEntrance = false;
//                    data.HasRentals = false;
//                    data.Highlight = myevent.SmgTags != null ? myevent.SmgTags.Contains("TopEvent") ? true : false : false;
//                    //data.ImageGallery = myevent.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc[language], ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle[language], ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList();
//                    data.ImageGallery = myevent.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc[language] : null, ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle[language] : null, ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList();
//                    data.IsOpen = false;
//                    data.IsPrepared = false;
//                    data.IsWithLigth = false;
//                    data.LiftAvailable = false;
//                    data.LocationInfo = new LocationInfoLocalized()
//                    {
//                        DistrictInfo = new DistrictInfoLocalized() { Id = myevent.LocationInfo.DistrictInfo.Id, Name = myevent.LocationInfo.DistrictInfo.Name[language] },
//                        MunicipalityInfo = new MunicipalityInfoLocalized() { Id = myevent.LocationInfo.MunicipalityInfo.Id, Name = myevent.LocationInfo.MunicipalityInfo.Name[language] },
//                        TvInfo = new TvInfoLocalized() { Id = myevent.LocationInfo.TvInfo.Id, Name = myevent.LocationInfo.TvInfo.Name[language] },
//                        RegionInfo = new RegionInfoLocalized() { Id = myevent.LocationInfo.RegionInfo.Id, Name = myevent.LocationInfo.RegionInfo.Name[language] }
//                    };
//                    data.MainType = "event";
//                    data.MaxSeatingCapacity = 0;
//                    data.Novelty = "";
//                    data.OperationSchedule = null;
//                    data.OrganizerInfos = myevent.OrganizerInfos[language];
//                    data.PayMet = myevent.PayMet;
//                    data.PoiType = "";
//                    //PoiProperty = myevent.TopicRIDs.Select(x => new PoiProperty(){ Name = "TopicRID", Value = x }).ToList(),
//                    data.PoiProperty = null;
//                    data.PoiServices = null; //myevent.TopicRIDs,
//                    data.Ranc = myevent.Ranc;
//                    data.Ratings = null;
//                    data.RunToValley = false;
//                    data.SignOn = myevent.SignOn;
//                    data.SmgTags = myevent.SmgTags;
//                    data.SubType = language != "de" ? language != "it" ? "Event" : "Evento" : "Veranstaltung";
//                    data.Ticket = myevent.Ticket;
//                    //TopicRIDs = myevent.TopicRIDs,
//                    data.Type = language != "de" ? language != "it" ? "Event" : "Evento" : "Veranstaltung";

//                    lstSelect.Add(data);
//                }

//                dr.Close();


//                return lstSelect.FirstOrDefault();
//            }
//            catch (Exception ex)
//            {

//                return null;
//            }
//        }

//        /// <summary>
//        /// Returns Single JSON Data mapped to given OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static MobileDetail SelectFromTableDataASmgPoiMobileDetail(NpgsqlConnection conn, string tablename, string id, string language)
//        {
//            try
//            {
//                string whereexp = "Id LIKE '" + id + "'";
//                string commandText = CreatetDatabaseCommand("*", tablename, whereexp, "", null, 0);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;

//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<MobileDetail> lstSelect = new List<MobileDetail>();
//                while (dr.Read())
//                {
//                    var activity = JsonConvert.DeserializeObject<ODHActivityPoi>(dr[1].ToString());

//                    MobileDetail data = new MobileDetail();
//                    data.Id = activity.Id;
//                    //AdditionalPoiInfos = activity.AdditionalPoiInfos[language],
//                    data.AltitudeDifference = activity.AltitudeDifference;
//                    data.AltitudeHighestPoint = activity.AltitudeHighestPoint;
//                    data.AltitudeLowestPoint = activity.AltitudeLowestPoint;
//                    data.AltitudeSumDown = activity.AltitudeSumDown;
//                    data.AltitudeSumUp = activity.AltitudeSumUp;
//                    //CapacityCeremony = null,
//                    //CategoryCodes = null,                                             
//                    data.ContactInfos = activity.ContactInfos[language];
//                    data.DateBegin = null;
//                    data.DateEnd = null;
//                    data.Detail = activity.Detail[language];
//                    data.Difficulty = activity.Difficulty;
//                    //DishRates = null,                                             
//                    data.DistanceDuration = activity.DistanceDuration;
//                    data.DistanceLength = activity.DistanceLength;
//                    data.EventDate = null;
//                    data.Exposition = activity.Exposition;
//                    //Facilities = null,
//                    data.FeetClimb = activity.FeetClimb;
//                    data.GpsInfo = activity.GpsInfo;
//                    data.GpsTrack = activity.GpsTrack;
//                    data.HasFreeEntrance = activity.HasFreeEntrance;
//                    data.HasRentals = activity.HasRentals;
//                    data.Highlight = activity.Highlight;
//                    //data.ImageGallery = activity.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc[language], ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle[language], ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList();
//                    data.ImageGallery = activity.ImageGallery.Select(x => new ImageGalleryLocalized() { Height = x.Height, ImageDesc = x.ImageDesc.Count > 0 ? x.ImageDesc[language] : null, ImageName = x.ImageName, ImageSource = x.ImageSource, ImageTitle = x.ImageTitle.Count > 0 ? x.ImageTitle[language] : null, ImageUrl = x.ImageUrl, IsInGallery = x.IsInGallery, ListPosition = x.ListPosition, ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, Width = x.Width, CopyRight = x.CopyRight }).ToList();
//                    data.IsOpen = activity.IsOpen;
//                    data.IsPrepared = activity.IsPrepared;
//                    data.IsWithLigth = activity.IsWithLigth;
//                    data.LiftAvailable = activity.LiftAvailable;
//                    data.LocationInfo = new LocationInfoLocalized()
//                    {
//                        DistrictInfo = new DistrictInfoLocalized() { Id = activity.LocationInfo.DistrictInfo.Id, Name = activity.LocationInfo.DistrictInfo.Name[language] },
//                        MunicipalityInfo = new MunicipalityInfoLocalized() { Id = activity.LocationInfo.MunicipalityInfo.Id, Name = activity.LocationInfo.MunicipalityInfo.Name[language] },
//                        TvInfo = new TvInfoLocalized() { Id = activity.LocationInfo.TvInfo.Id, Name = activity.LocationInfo.TvInfo.Name[language] },
//                        RegionInfo = new RegionInfoLocalized() { Id = activity.LocationInfo.RegionInfo.Id, Name = activity.LocationInfo.RegionInfo.Name[language] }
//                    };
//                    data.MainType = "smgpoi";
//                    data.MaxSeatingCapacity = 0;
//                    data.Novelty = activity.AdditionalPoiInfos[language].Novelty;
//                    data.OperationSchedule = activity.OperationSchedule;
//                    data.OrganizerInfos = null;
//                    data.PayMet = "";
//                    data.PoiType = activity.AdditionalPoiInfos[language].PoiType;
//                    //PoiProperty = activity.SyncSourceInterface == "GastronomicData" ? activity.CategoryCodes.Select(x => new PoiProperty() { Name = x.Id, Value = x.Shortname }).ToList() : activity.PoiProperty[language],
//                    data.PoiProperty = activity.PoiProperty != null ? activity.PoiProperty.Count > 0 ? activity.PoiProperty[language].ToDictionary(e => e.Name, v => v.Value) : null : null;
//                    data.PoiServices = activity.SyncSourceInterface == "GastronomicData" ? activity.Facilities.Select(x => x.Id).ToList() : activity.PoiServices;
//                    data.Ranc = 0;
//                    data.Ratings = activity.Ratings;
//                    data.RunToValley = activity.RunToValley;
//                    data.SignOn = "";
//                    data.SmgTags = activity.SmgTags;
//                    data.SubType = activity.AdditionalPoiInfos[language].SubType;
//                    data.Ticket = "";
//                    //TopicRIDs = null, 
//                    data.Type = activity.AdditionalPoiInfos[language].MainType;


//                    lstSelect.Add(data);
//                }

//                dr.Close();

//                return lstSelect.FirstOrDefault();
//            }
//            catch (Exception ex)
//            {

//                return null;
//            }
//        }


//        /// <summary>
//        /// Returns List of JSON Data mapped from SmgPoi to MobileDataExtended OBJECT
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="conn"></param>
//        /// <param name="tablename"></param>
//        /// <param name="whereexp"></param>
//        /// <param name="sortexp"></param>
//        /// <param name="limit"></param>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        public static List<MobileHtmlLocalized> SelectFromTableDataTransformToMobileHtmlLocalized(NpgsqlConnection conn, string tablename, string selectexp, string whereexp, string sortexp, int limit, Nullable<int> offset, string language)
//        {
//            try
//            {
//                //CultureInfo myculture = new CultureInfo("en");

//                string commandText = CreatetDatabaseCommand(selectexp, tablename, whereexp, sortexp, offset, limit);

//                var command = new NpgsqlCommand(commandText);
//                command.Connection = conn;


//                NpgsqlDataReader dr = command.ExecuteReader();

//                List<MobileHtmlLocalized> lstSelect = new List<MobileHtmlLocalized>();
//                while (dr.Read())
//                {
//                    var data = JsonConvert.DeserializeObject<MobileHtml>(dr[1].ToString());

//                    MobileHtmlLocalized mobilehtml = new MobileHtmlLocalized();

//                    mobilehtml.Id = data.Id;
//                    mobilehtml.HtmlText = data.HtmlText[language];
//                    lstSelect.Add(mobilehtml);
//                }

//                dr.Close();

//                return lstSelect;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }


//        #endregion

//        #region Generic Insert Method

//        public static string InsertDataIntoTable(NpgsqlConnection conn, string tablename, string data, string id)
//        {
//            try
//            {
//                ////Fix the single quotes
//                //data = data.Replace("'", "''");

//                //string commandText = "INSERT INTO " + tablename + "(id, data) VALUES('" + id + "','" + data + "');";

//                //var command = new NpgsqlCommand(commandText);
//                //command.Connection = conn;

//                var command = new NpgsqlCommand("INSERT INTO " + tablename + "(id, data) VALUES(@id,@data)", conn);
//                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, id);
//                command.Parameters.AddWithValue("data", NpgsqlTypes.NpgsqlDbType.Jsonb, data);

//                int affectedrows = command.ExecuteNonQuery();

//                return affectedrows.ToString();
//            }
//            catch (Exception ex)
//            {
//                return ex.Message;
//            }
//        }

//        public static string InsertDataIntoTable(NpgsqlConnection conn, string tablename, object data, string id)
//        {
//            try
//            {
//                var command = new NpgsqlCommand("INSERT INTO " + tablename + "(id, data) VALUES(@id,@data)", conn);
//                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, id);
//                command.Parameters.AddWithValue("data", NpgsqlTypes.NpgsqlDbType.Jsonb, JsonConvert.SerializeObject(data));

//                int affectedrows = command.ExecuteNonQuery();

//                return affectedrows.ToString();
//            }
//            catch (Exception ex)
//            {
//                return ex.Message;
//            }
//        }

//        #endregion

//        #region Generic Update Method

//        public static string UpdateDataFromTable(NpgsqlConnection conn, string tablename, string data, string id)
//        {
//            try
//            {
//                ////Fix the single quotes
//                //data = data.Replace("'", "''");                

//                //string commandText = "UPDATE " + tablename + " SET data = '" + data + "' WHERE id ='" + id + "';";

//                //var command = new NpgsqlCommand(commandText);

//                //command.Connection = conn;

//                var command = new NpgsqlCommand("UPDATE " + tablename + " SET data = @data WHERE id = @id", conn);
//                command.Parameters.AddWithValue("data", NpgsqlTypes.NpgsqlDbType.Jsonb, data);
//                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, id);

//                int affectedrows = command.ExecuteNonQuery();

//                return affectedrows.ToString();
//            }
//            catch (Exception ex)
//            {
//                return ex.Message;
//            }
//        }

//        public static string UpdateDataFromTable(NpgsqlConnection conn, string tablename, object data, string id)
//        {
//            try
//            {
//                var command = new NpgsqlCommand("UPDATE " + tablename + " SET data = @data WHERE id = @id", conn);
//                command.Parameters.AddWithValue("data", NpgsqlTypes.NpgsqlDbType.Jsonb, JsonConvert.SerializeObject(data));
//                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, id);

//                int affectedrows = command.ExecuteNonQuery();

//                return affectedrows.ToString();
//            }
//            catch (Exception ex)
//            {
//                return ex.Message;
//            }
//        }


//        #endregion

//        #region Generic Delete Method

//        public static string DeleteDataFromTable(NpgsqlConnection conn, string tablename, string idvalue)
//        {
//            try
//            {

//                //string commandText = "DELETE FROM " + tablename + " WHERE id = '" + idvalue + "';";

//                //var command = new NpgsqlCommand(commandText);
//                //command.Connection = conn;

//                var command = new NpgsqlCommand("DELETE FROM " + tablename + " WHERE id = @id", conn);
//                command.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Text, idvalue);

//                int affectedrows = command.ExecuteNonQuery();

//                return affectedrows.ToString();
//            }
//            catch (Exception ex)
//            {
//                return ex.Message;
//            }
//        }

//        #endregion

//        #region Where Helpers

//        public static string CreateAccoWhereExpression(List<string> idlist, List<string> accotypelist, bool apartmentfilter, List<string> categorylist, Dictionary<string, bool> featurelist, List<string> badgelist, Dictionary<string, bool> themelist, List<string> boardlist, List<string> smgtaglist, List<string> districtlist, List<string> municipalitylist, List<string> tourismvereinlist, List<string> regionlist, bool? activefilter, bool? smgactivefilter, bool? bookable)
//        {
//            //Do muassimer die where zommbaschgeln

//            string whereexpression = "";
//            //IDLIST
//            if (idlist.Count > 0)
//            {
//                //Tuning force to use GIN Index
//                if (idlist.Count == 1)
//                {
//                    //whereexpression = whereexpression + "data @> '{\"Id\" : \"" + idlist.FirstOrDefault() + "\" }'";
//                    whereexpression = whereexpression + "Id = '" + idlist.FirstOrDefault() + "'";
//                }
//                else
//                {
//                    string idliststring = "";
//                    foreach (var accoid in idlist)
//                    {
//                        idliststring = idliststring + "'\"" + accoid.ToUpper() + "\"', ";
//                    }
//                    idliststring = idliststring.Remove(idliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'Id' in (" + idliststring + ")";
//                }
//            }
//            //DISTRICT
//            if (districtlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (districtlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"DistrictId\" : \"" + districtlist.FirstOrDefault() + "\" }'";
//                }
//                else
//                {
//                    string districtliststring = "";
//                    foreach (var distid in districtlist)
//                    {
//                        districtliststring = districtliststring + "'\"" + distid.ToUpper() + "\"', ";
//                    }
//                    districtliststring = districtliststring.Remove(districtliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'DistrictId' in (" + districtliststring + ")";
//                }


//            }
//            //MUNICIPALITY
//            if (municipalitylist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (municipalitylist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"MunicipalityInfo\": { \"Id\": \"" + municipalitylist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string municipalityliststring = "";
//                    foreach (var munid in municipalitylist)
//                    {
//                        municipalityliststring = municipalityliststring + "'\"" + munid.ToUpper() + "\"', ";
//                    }
//                    municipalityliststring = municipalityliststring.Remove(municipalityliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'MunicipalityInfo' -> 'Id' in (" + municipalityliststring + ")";
//                }
//            }
//            //TOURISMVEREIN
//            if (tourismvereinlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (tourismvereinlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"TvInfo\": { \"Id\": \"" + tourismvereinlist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string tvliststring = "";
//                    foreach (var tvid in tourismvereinlist)
//                    {
//                        tvliststring = tvliststring + "'\"" + tvid.ToUpper() + "\"', ";
//                    }
//                    tvliststring = tvliststring.Remove(tvliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'TvInfo' -> 'Id' in (" + tvliststring + ")";
//                }
//            }
//            //REGION
//            if (regionlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (regionlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"RegionInfo\": { \"Id\": \"" + regionlist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string regionliststring = "";
//                    foreach (var tvid in regionlist)
//                    {
//                        regionliststring = regionliststring + "'\"" + tvid.ToUpper() + "\"', ";
//                    }
//                    regionliststring = regionliststring.Remove(regionliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'RegionInfo' -> 'Id' in (" + regionliststring + ")";
//                }
//            }

//            //Board Info schaugn ob des geat!!! umgekearter foll
//            if (boardlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string boardliststring = "";
//                foreach (var boardid in boardlist)
//                {
//                    boardliststring = boardliststring + "data @> '{ \"BoardIds\": [\"" + boardid + "\"]}' OR ";
//                }
//                boardliststring = boardliststring.Remove(boardliststring.Length - 4);

//                whereexpression = whereexpression + boardliststring + ")";
//            }

//            //Badges Info
//            if (badgelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";



//                string badgeliststring = "";
//                foreach (var badgeid in badgelist)
//                {
//                    badgeliststring = badgeliststring + "data @> '{ \"BadgeIds\": [\"" + badgeid + "\"]}' OR ";
//                }
//                badgeliststring = badgeliststring.Remove(badgeliststring.Length - 4);

//                whereexpression = whereexpression + badgeliststring + ")";

//            }

//            //Category Info
//            if (categorylist.Count > 0)
//            {


//                //Tuning force to use GIN Index
//                if (categorylist.Count == 1)
//                {
//                    if (!String.IsNullOrEmpty(whereexpression))
//                        whereexpression = whereexpression + " AND ";

//                    whereexpression = whereexpression + "data @> '{\"AccoCategoryId\": \"" + categorylist.FirstOrDefault() + "\" }'";
//                }
//                //Tuned to use GIN Index brings laut performance tests af der console ober net so im live betrieb schun! ALSO Bei kleinen Ergebnismengen ist dies schneller bei grösseren das untere
//                //else
//                //{
//                //    if (!String.IsNullOrEmpty(whereexpression))
//                //        whereexpression = whereexpression + " AND (";
//                //    else
//                //        whereexpression = whereexpression + "(";

//                //    string categoryliststring = "";
//                //    foreach (var categoryid in categorylist)
//                //    {
//                //        categoryliststring = categoryliststring + "data @> '{ \"AccoCategoryId\": \"" + categoryid + "\"}' OR ";
//                //    }
//                //    categoryliststring = categoryliststring.Remove(categoryliststring.Length - 4);

//                //    whereexpression = whereexpression + categoryliststring + ")";
//                //}
//                //Konstante laufzeit hot des.... net so stork noch ergebnismenge....
//                else
//                {
//                    if (!String.IsNullOrEmpty(whereexpression))
//                        whereexpression = whereexpression + " AND (";
//                    else
//                        whereexpression = whereexpression + "(";

//                    string categoryliststring = "";
//                    foreach (var categoryid in categorylist)
//                    {
//                        categoryliststring = categoryliststring + "'\"" + categoryid + "\"', ";
//                    }
//                    categoryliststring = categoryliststring.Remove(categoryliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'AccoCategoryId' in (" + categoryliststring + "))x";
//                }
//            }

//            //AccoType Info schaugn ob des geat!!! umgekearter foll
//            if (accotypelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (accotypelist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"AccoTypeId\": \"" + accotypelist.FirstOrDefault() + "\" }'";
//                }
//                else
//                {

//                    string accotypeliststring = "";
//                    foreach (var accotypeid in accotypelist)
//                    {
//                        accotypeliststring = accotypeliststring + "'\"" + accotypeid + "\"', ";
//                    }
//                    accotypeliststring = accotypeliststring.Remove(accotypeliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'AccoTypeId' in (" + accotypeliststring + ")";
//                }
//            }

//            //Theme Info
//            if (themelist.Where(x => x.Value == true).Count() > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string themeliststring = "";
//                foreach (var themeid in themelist.Where(x => x.Value == true))
//                {
//                    themeliststring = themeliststring + "data @> '{ \"ThemeIds\": [\"" + themeid.Key + "\"]}' AND ";
//                }
//                themeliststring = themeliststring.Remove(themeliststring.Length - 4);

//                whereexpression = whereexpression + themeliststring + ")";
//            }

//            //Theme Info
//            if (featurelist.Where(x => x.Value == true).Count() > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string featureliststring = "";
//                foreach (var featureid in featurelist.Where(x => x.Value == true))
//                {
//                    featureliststring = featureliststring + "data @> '{ \"SpecialFeaturesIds\": [\"" + featureid.Key + "\"]}' AND ";
//                }
//                featureliststring = featureliststring.Remove(featureliststring.Length - 4);

//                whereexpression = whereexpression + featureliststring + ")";
//            }

//            //OLLE DE NO TESTEN

//            //Apartment
//            if (apartmentfilter)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"HasApartment\" : " + apartmentfilter.ToString().ToLower() + "}'";
//            }

//            //Active
//            if (activefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Active\" : " + activefilter.ToString().ToLower() + "}'";
//            }

//            //SmgActive
//            if (smgactivefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"SmgActive\" : " + smgactivefilter.ToString().ToLower() + "}'";
//            }
//            //SmgTags Info
//            if (smgtaglist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string smgtagliststring = "";
//                foreach (var smgtag in smgtaglist)
//                {
//                    smgtagliststring = smgtagliststring + "data @> '{ \"SmgTags\": [\"" + smgtag.ToLower() + "\"]}' OR ";
//                }
//                smgtagliststring = smgtagliststring.Remove(smgtagliststring.Length - 4);

//                whereexpression = whereexpression + smgtagliststring + ")";
//            }

//            //Bookable
//            if (bookable != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"IsBookable\" : " + bookable.ToString().ToLower() + " }'";
//            }


//            //    //Sonderfall Apartment
//            //                .WhereIf(apartmentfilter, x => x.HasApartment == true)  
//            //    //BAdge geschichten   
//            //                .WhereIf(mysmgtaglist.Count > 0, x => x.SmgTags.In(mysmgtaglist))
//            //                .WhereIf(smgactivefilter != null, x => x.SmgActive == smgactivefilter)
//            //                .WhereIf(activefilter != null, x => x.Active == activefilter)


//            return whereexpression;
//        }

//        public static string CreateAccoWhereExpressionExtended(List<string> idlist, List<string> accotypelist, bool apartmentfilter, List<string> categorylist, Dictionary<string, bool> featurelist, List<string> badgelist, Dictionary<string, bool> themelist, List<string> boardlist, List<string> smgtaglist, List<string> districtlist, List<string> municipalitylist, List<string> tourismvereinlist, List<string> regionlist, bool? activefilter, bool? smgactivefilter, bool? bookable, bool altitude, int altitudemin, int altitudemax)
//        {
//            //Do muassimer die where zommbaschgeln

//            string whereexpression = "";
//            //IDLIST
//            if (idlist.Count > 0)
//            {
//                //Tuning force to use GIN Index
//                if (idlist.Count == 1)
//                {
//                    //whereexpression = whereexpression + "data @> '{\"Id\" : \"" + idlist.FirstOrDefault() + "\" }'";
//                    whereexpression = whereexpression + "Id = '" + idlist.FirstOrDefault() + "'";
//                }
//                else
//                {
//                    string idliststring = "";
//                    foreach (var accoid in idlist)
//                    {
//                        idliststring = idliststring + "'\"" + accoid.ToUpper() + "\"', ";
//                    }
//                    idliststring = idliststring.Remove(idliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'Id' in (" + idliststring + ")";
//                }
//            }
//            //DISTRICT
//            if (districtlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (districtlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"DistrictId\" : \"" + districtlist.FirstOrDefault() + "\" }'";
//                }
//                else
//                {
//                    string districtliststring = "";
//                    foreach (var distid in districtlist)
//                    {
//                        districtliststring = districtliststring + "'\"" + distid.ToUpper() + "\"', ";
//                    }
//                    districtliststring = districtliststring.Remove(districtliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'DistrictId' in (" + districtliststring + ")";
//                }


//            }
//            //MUNICIPALITY
//            if (municipalitylist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (municipalitylist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"MunicipalityInfo\": { \"Id\": \"" + municipalitylist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string municipalityliststring = "";
//                    foreach (var munid in municipalitylist)
//                    {
//                        municipalityliststring = municipalityliststring + "'\"" + munid.ToUpper() + "\"', ";
//                    }
//                    municipalityliststring = municipalityliststring.Remove(municipalityliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'MunicipalityInfo' -> 'Id' in (" + municipalityliststring + ")";
//                }
//            }
//            //TOURISMVEREIN
//            if (tourismvereinlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (tourismvereinlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"TvInfo\": { \"Id\": \"" + tourismvereinlist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string tvliststring = "";
//                    foreach (var tvid in tourismvereinlist)
//                    {
//                        tvliststring = tvliststring + "'\"" + tvid.ToUpper() + "\"', ";
//                    }
//                    tvliststring = tvliststring.Remove(tvliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'TvInfo' -> 'Id' in (" + tvliststring + ")";
//                }
//            }
//            //REGION
//            if (regionlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (regionlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"RegionInfo\": { \"Id\": \"" + regionlist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string regionliststring = "";
//                    foreach (var tvid in regionlist)
//                    {
//                        regionliststring = regionliststring + "'\"" + tvid.ToUpper() + "\"', ";
//                    }
//                    regionliststring = regionliststring.Remove(regionliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'RegionInfo' -> 'Id' in (" + regionliststring + ")";
//                }
//            }

//            //Board Info schaugn ob des geat!!! umgekearter foll
//            if (boardlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string boardliststring = "";
//                foreach (var boardid in boardlist)
//                {
//                    boardliststring = boardliststring + "data @> '{ \"BoardIds\": [\"" + boardid + "\"]}' OR ";
//                }
//                boardliststring = boardliststring.Remove(boardliststring.Length - 4);

//                whereexpression = whereexpression + boardliststring + ")";
//            }

//            //Badges Info
//            if (badgelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";



//                string badgeliststring = "";
//                foreach (var badgeid in badgelist)
//                {
//                    badgeliststring = badgeliststring + "data @> '{ \"BadgeIds\": [\"" + badgeid + "\"]}' OR ";
//                }
//                badgeliststring = badgeliststring.Remove(badgeliststring.Length - 4);

//                whereexpression = whereexpression + badgeliststring + ")";

//            }

//            //Category Info
//            if (categorylist.Count > 0)
//            {


//                //Tuning force to use GIN Index
//                if (categorylist.Count == 1)
//                {
//                    if (!String.IsNullOrEmpty(whereexpression))
//                        whereexpression = whereexpression + " AND ";

//                    whereexpression = whereexpression + "data @> '{\"AccoCategoryId\": \"" + categorylist.FirstOrDefault() + "\" }'";
//                }
//                //Tuned to use GIN Index brings laut performance tests af der console ober net so im live betrieb schun! ALSO Bei kleinen Ergebnismengen ist dies schneller bei grösseren das untere
//                //else
//                //{
//                //    if (!String.IsNullOrEmpty(whereexpression))
//                //        whereexpression = whereexpression + " AND (";
//                //    else
//                //        whereexpression = whereexpression + "(";

//                //    string categoryliststring = "";
//                //    foreach (var categoryid in categorylist)
//                //    {
//                //        categoryliststring = categoryliststring + "data @> '{ \"AccoCategoryId\": \"" + categoryid + "\"}' OR ";
//                //    }
//                //    categoryliststring = categoryliststring.Remove(categoryliststring.Length - 4);

//                //    whereexpression = whereexpression + categoryliststring + ")";
//                //}
//                //Konstante laufzeit hot des.... net so stork noch ergebnismenge....
//                else
//                {
//                    if (!String.IsNullOrEmpty(whereexpression))
//                        whereexpression = whereexpression + " AND (";
//                    else
//                        whereexpression = whereexpression + "(";

//                    string categoryliststring = "";
//                    foreach (var categoryid in categorylist)
//                    {
//                        categoryliststring = categoryliststring + "'\"" + categoryid + "\"', ";
//                    }
//                    categoryliststring = categoryliststring.Remove(categoryliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'AccoCategoryId' in (" + categoryliststring + "))x";
//                }
//            }

//            //AccoType Info schaugn ob des geat!!! umgekearter foll
//            if (accotypelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (accotypelist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"AccoTypeId\": \"" + accotypelist.FirstOrDefault() + "\" }'";
//                }
//                else
//                {

//                    string accotypeliststring = "";
//                    foreach (var accotypeid in accotypelist)
//                    {
//                        accotypeliststring = accotypeliststring + "'\"" + accotypeid + "\"', ";
//                    }
//                    accotypeliststring = accotypeliststring.Remove(accotypeliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'AccoTypeId' in (" + accotypeliststring + ")";
//                }
//            }

//            //Theme Info
//            if (themelist.Where(x => x.Value == true).Count() > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string themeliststring = "";
//                foreach (var themeid in themelist.Where(x => x.Value == true))
//                {
//                    themeliststring = themeliststring + "data @> '{ \"ThemeIds\": [\"" + themeid.Key + "\"]}' AND ";
//                }
//                themeliststring = themeliststring.Remove(themeliststring.Length - 4);

//                whereexpression = whereexpression + themeliststring + ")";
//            }

//            //Theme Info
//            if (featurelist.Where(x => x.Value == true).Count() > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string featureliststring = "";
//                foreach (var featureid in featurelist.Where(x => x.Value == true))
//                {
//                    featureliststring = featureliststring + "data @> '{ \"SpecialFeaturesIds\": [\"" + featureid.Key + "\"]}' AND ";
//                }
//                featureliststring = featureliststring.Remove(featureliststring.Length - 4);

//                whereexpression = whereexpression + featureliststring + ")";
//            }

//            //OLLE DE NO TESTEN

//            //Apartment
//            if (apartmentfilter)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"HasApartment\" : " + apartmentfilter.ToString().ToLower() + "}'";
//            }

//            //Active
//            if (activefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Active\" : " + activefilter.ToString().ToLower() + "}'";
//            }

//            //SmgActive
//            if (smgactivefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"SmgActive\" : " + smgactivefilter.ToString().ToLower() + "}'";
//            }
//            //SmgTags Info
//            if (smgtaglist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string smgtagliststring = "";
//                foreach (var smgtag in smgtaglist)
//                {
//                    smgtagliststring = smgtagliststring + "data @> '{ \"SmgTags\": [\"" + smgtag.ToLower() + "\"]}' OR ";
//                }
//                smgtagliststring = smgtagliststring.Remove(smgtagliststring.Length - 4);

//                whereexpression = whereexpression + smgtagliststring + ")";
//            }

//            //Bookable
//            if (bookable != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"IsBookable\" : " + bookable.ToString().ToLower() + " }'";
//            }

//            if (altitude)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";
//                whereexpression = whereexpression + "(data ->>'Altitude')::numeric >= " + altitudemin + " AND (data->> 'Altitude')::numeric <= " + altitudemax;
//            }


//            //    //Sonderfall Apartment
//            //                .WhereIf(apartmentfilter, x => x.HasApartment == true)  
//            //    //BAdge geschichten   
//            //                .WhereIf(mysmgtaglist.Count > 0, x => x.SmgTags.In(mysmgtaglist))
//            //                .WhereIf(smgactivefilter != null, x => x.SmgActive == smgactivefilter)
//            //                .WhereIf(activefilter != null, x => x.Active == activefilter)


//            return whereexpression;
//        }

//        public static string CreateIdListWhereExpression(List<string> idlist)
//        {
//            //Do muassimer die where zommbaschgeln

//            string whereexpression = "";
//            //IDLIST
//            if (idlist.Count > 0)
//            {
//                //Tuning force to use GIN Index
//                if (idlist.Count == 1)
//                {
//                    //whereexpression = whereexpression + "data @> '{\"Id\" : \"" + idlist.FirstOrDefault() + "\" }'";
//                    whereexpression = whereexpression + "Id = '" + idlist.FirstOrDefault() + "'";
//                }
//                else
//                {
//                    string idliststring = "";
//                    foreach (var accoid in idlist)
//                    {
//                        idliststring = idliststring + "'" + accoid + "', ";
//                        //idliststring = idliststring + "'\"" + accoid.ToUpper() + "\"', ";
//                    }
//                    idliststring = idliststring.Remove(idliststring.Length - 2);

//                    whereexpression = whereexpression + "Id in (" + idliststring + ")";
//                }
//            }


//            return whereexpression;
//        }

//        public static string CreateIdListWhereExpression(List<string> idlist, bool insertdummyonemptyidlist)
//        {
//            //Do muassimer die where zommbaschgeln

//            string whereexpression = "";
//            //IDLIST
//            if (idlist.Count > 0)
//            {
//                //Tuning force to use GIN Index
//                if (idlist.Count == 1)
//                {
//                    //whereexpression = whereexpression + "data @> '{\"Id\" : \"" + idlist.FirstOrDefault() + "\" }'";
//                    whereexpression = whereexpression + "Id = '" + idlist.FirstOrDefault() + "'";
//                }
//                else
//                {
//                    string idliststring = "";
//                    foreach (var accoid in idlist)
//                    {
//                        idliststring = idliststring + "'" + accoid + "', ";
//                        //idliststring = idliststring + "'\"" + accoid.ToUpper() + "\"', ";
//                    }
//                    idliststring = idliststring.Remove(idliststring.Length - 2);

//                    whereexpression = whereexpression + "Id in (" + idliststring + ")";
//                }
//            }
//            else if (idlist.Count == 0)
//            {
//                if (insertdummyonemptyidlist)
//                    whereexpression = "Id = '00000000'";
//            }


//            return whereexpression;
//        }


//        public static string CreateActivityWhereExpression(List<string> idlist, List<string> activitytypelist, List<string> subtypelist, List<string> difficultylist, List<string> smgtaglist, List<string> districtlist, List<string> municipalitylist, List<string> tourismvereinlist, List<string> regionlist, List<string> arealist, bool distance, int distancemin, int distancemax, bool duration, int durationmin, int durationmax, bool altitude, int altitudemin, int altitudemax, bool? highlight, bool? activefilter, bool? smgactivefilter)
//        {
//            //Do muassimer die where zommbaschgeln

//            string whereexpression = "";
//            //IDLIST
//            if (idlist.Count > 0)
//            {
//                //Tuning force to use GIN Index
//                if (idlist.Count == 1)
//                {
//                    //whereexpression = whereexpression + "data @> '{\"Id\" : \"" + idlist.FirstOrDefault() + "\" }'";
//                    whereexpression = whereexpression + "Id = '" + idlist.FirstOrDefault() + "'";
//                }
//                else
//                {
//                    string idliststring = "";
//                    foreach (var accoid in idlist)
//                    {
//                        idliststring = idliststring + "'" + accoid + "', ";
//                        //idliststring = idliststring + "'\"" + accoid.ToUpper() + "\"', ";
//                    }
//                    idliststring = idliststring.Remove(idliststring.Length - 2);

//                    whereexpression = whereexpression + "Id in (" + idliststring + ")";
//                }
//            }
//            //DISTRICT
//            if (districtlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (districtlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"DistrictId\" : \"" + districtlist.FirstOrDefault() + "\" }'";
//                }
//                else
//                {
//                    string districtliststring = "";
//                    foreach (var distid in districtlist)
//                    {
//                        districtliststring = districtliststring + "'\"" + distid.ToUpper() + "\"', ";
//                    }
//                    districtliststring = districtliststring.Remove(districtliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'DistrictId' in (" + districtliststring + ")";
//                }


//            }
//            //MUNICIPALITY
//            if (municipalitylist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (municipalitylist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"MunicipalityInfo\": { \"Id\": \"" + municipalitylist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string municipalityliststring = "";
//                    foreach (var munid in municipalitylist)
//                    {
//                        municipalityliststring = municipalityliststring + "'\"" + munid.ToUpper() + "\"', ";
//                    }
//                    municipalityliststring = municipalityliststring.Remove(municipalityliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'MunicipalityInfo' -> 'Id' in (" + municipalityliststring + ")";
//                }
//            }
//            //TOURISMVEREIN
//            if (tourismvereinlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (tourismvereinlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"TvInfo\": { \"Id\": \"" + tourismvereinlist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string tvliststring = "";
//                    foreach (var tvid in tourismvereinlist)
//                    {
//                        tvliststring = tvliststring + "'\"" + tvid.ToUpper() + "\"', ";
//                    }
//                    tvliststring = tvliststring.Remove(tvliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'TvInfo' -> 'Id' in (" + tvliststring + ")";
//                }
//            }
//            //REGION
//            if (regionlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (regionlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"RegionInfo\": { \"Id\": \"" + regionlist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string regionliststring = "";
//                    foreach (var tvid in regionlist)
//                    {
//                        regionliststring = regionliststring + "'\"" + tvid.ToUpper() + "\"', ";
//                    }
//                    regionliststring = regionliststring.Remove(regionliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'RegionInfo' -> 'Id' in (" + regionliststring + ")";
//                }
//            }

//            //AREA
//            if (arealist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string arealiststring = "";
//                foreach (var area in arealist)
//                {
//                    arealiststring = arealiststring + "data @> '{ \"AreaId\": [\"" + area + "\"]}' OR ";
//                }
//                arealiststring = arealiststring.Remove(arealiststring.Length - 4);

//                whereexpression = whereexpression + arealiststring + ")";
//            }

//            //Difficulty
//            if (difficultylist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string difficultystring = "";
//                foreach (var difficultyId in difficultylist)
//                {
//                    difficultystring = difficultystring + "data @> '{ \"Difficulty\": \"" + difficultyId + "\"}' OR ";
//                }
//                difficultystring = difficultystring.Remove(difficultystring.Length - 4);

//                whereexpression = whereexpression + difficultystring + ")";
//            }

//            //Activity Type
//            if (activitytypelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string activitytypestring = "";
//                foreach (var activitytypeId in activitytypelist)
//                {
//                    activitytypestring = activitytypestring + "data @> '{ \"Type\": \"" + activitytypeId + "\"}' OR ";
//                }
//                activitytypestring = activitytypestring.Remove(activitytypestring.Length - 4);

//                whereexpression = whereexpression + activitytypestring + ")";
//            }

//            //Activity Sub Type            
//            if (subtypelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string smgtagliststring = "";
//                foreach (var smgtag in subtypelist)
//                {
//                    smgtagliststring = smgtagliststring + "data @> '{ \"SmgTags\": [\"" + smgtag.ToLower() + "\"]}' OR ";
//                }
//                smgtagliststring = smgtagliststring.Remove(smgtagliststring.Length - 4);

//                whereexpression = whereexpression + smgtagliststring + ")";
//            }

//            //SmgTags Info
//            if (smgtaglist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string smgtagliststring = "";
//                foreach (var smgtag in smgtaglist)
//                {
//                    smgtagliststring = smgtagliststring + "data @> '{ \"SmgTags\": [\"" + smgtag.ToLower() + "\"]}' OR ";
//                }
//                smgtagliststring = smgtagliststring.Remove(smgtagliststring.Length - 4);

//                whereexpression = whereexpression + smgtagliststring + ")";
//            }

//            //DURATION
//            if (duration)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "(data ->> 'DistanceDuration')::numeric > " + durationmin + " AND (data ->> 'DistanceDuration')::numeric < " + durationmax;

//            }
//            //ALTITUDE
//            if (altitude)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "(data ->> 'AltitudeDifference')::numeric > " + altitudemin + " AND (data ->> 'AltitudeDifference')::numeric < " + altitudemax;
//            }
//            //DISTANCE
//            if (distance)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "(data ->> 'DistanceLength')::numeric > " + distancemin + " AND (data ->> 'DistanceLength')::numeric < " + distancemax;
//            }

//            //Highlight
//            if (highlight != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Highlight\" : " + highlight.ToString().ToLower() + "}'";
//            }

//            //Active
//            if (activefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Active\" : " + activefilter.ToString().ToLower() + "}'";
//            }

//            //SmgActive
//            if (smgactivefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"SmgActive\" : " + smgactivefilter.ToString().ToLower() + "}'";
//            }




//            return whereexpression;
//        }

//        public static string CreatePoiWhereExpression(List<string> idlist, List<string> poitypelist, List<string> subtypelist, List<string> smgtaglist, List<string> districtlist, List<string> municipalitylist, List<string> tourismvereinlist, List<string> regionlist, List<string> arealist, bool? highlight, bool? activefilter, bool? smgactivefilter)
//        {
//            //Do muassimer die where zommbaschgeln

//            string whereexpression = "";
//            //IDLIST
//            if (idlist.Count > 0)
//            {
//                //Tuning force to use GIN Index
//                if (idlist.Count == 1)
//                {
//                    //whereexpression = whereexpression + "data @> '{\"Id\" : \"" + idlist.FirstOrDefault() + "\" }'";
//                    whereexpression = whereexpression + "Id = '" + idlist.FirstOrDefault() + "'";
//                }
//                else
//                {
//                    string idliststring = "";
//                    foreach (var accoid in idlist)
//                    {
//                        idliststring = idliststring + "'" + accoid + "', ";

//                        //idliststring = idliststring + "'\"" + accoid.ToUpper() + "\"', ";
//                    }
//                    idliststring = idliststring.Remove(idliststring.Length - 2);

//                    whereexpression = whereexpression + "Id in (" + idliststring + ")";
//                }
//            }
//            //DISTRICT
//            if (districtlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (districtlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"DistrictId\" : \"" + districtlist.FirstOrDefault() + "\" }'";
//                }
//                else
//                {
//                    string districtliststring = "";
//                    foreach (var distid in districtlist)
//                    {
//                        districtliststring = districtliststring + "'\"" + distid.ToUpper() + "\"', ";
//                    }
//                    districtliststring = districtliststring.Remove(districtliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'DistrictId' in (" + districtliststring + ")";
//                }


//            }
//            //MUNICIPALITY
//            if (municipalitylist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (municipalitylist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"MunicipalityInfo\": { \"Id\": \"" + municipalitylist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string municipalityliststring = "";
//                    foreach (var munid in municipalitylist)
//                    {
//                        municipalityliststring = municipalityliststring + "'\"" + munid.ToUpper() + "\"', ";
//                    }
//                    municipalityliststring = municipalityliststring.Remove(municipalityliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'MunicipalityInfo' -> 'Id' in (" + municipalityliststring + ")";
//                }
//            }
//            //TOURISMVEREIN
//            if (tourismvereinlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (tourismvereinlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"TvInfo\": { \"Id\": \"" + tourismvereinlist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string tvliststring = "";
//                    foreach (var tvid in tourismvereinlist)
//                    {
//                        tvliststring = tvliststring + "'\"" + tvid.ToUpper() + "\"', ";
//                    }
//                    tvliststring = tvliststring.Remove(tvliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'TvInfo' -> 'Id' in (" + tvliststring + ")";
//                }
//            }
//            //REGION
//            if (regionlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (regionlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"RegionInfo\": { \"Id\": \"" + regionlist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string regionliststring = "";
//                    foreach (var tvid in regionlist)
//                    {
//                        regionliststring = regionliststring + "'\"" + tvid.ToUpper() + "\"', ";
//                    }
//                    regionliststring = regionliststring.Remove(regionliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'RegionInfo' -> 'Id' in (" + regionliststring + ")";
//                }
//            }

//            //AREA
//            if (arealist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string arealiststring = "";
//                foreach (var area in arealist)
//                {
//                    arealiststring = arealiststring + "data @> '{ \"AreaId\": [\"" + area + "\"]}' OR ";
//                }
//                arealiststring = arealiststring.Remove(arealiststring.Length - 4);

//                whereexpression = whereexpression + arealiststring + ")";
//            }

//            //Activity Type
//            if (poitypelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string poitypeliststring = "";
//                foreach (var poitype in poitypelist)
//                {
//                    poitypeliststring = poitypeliststring + "data @> '{ \"SmgTags\": [\"" + poitype.ToLower() + "\"]}' OR ";
//                }
//                poitypeliststring = poitypeliststring.Remove(poitypeliststring.Length - 4);

//                whereexpression = whereexpression + poitypeliststring + ")";
//            }

//            //Activity Sub Type            
//            if (subtypelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string smgtagliststring = "";
//                foreach (var smgtag in subtypelist)
//                {
//                    smgtagliststring = smgtagliststring + "data @> '{ \"SmgTags\": [\"" + smgtag.ToLower() + "\"]}' OR ";
//                }
//                smgtagliststring = smgtagliststring.Remove(smgtagliststring.Length - 4);

//                whereexpression = whereexpression + smgtagliststring + ")";
//            }

//            //SmgTags Info
//            if (smgtaglist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string smgtagliststring = "";
//                foreach (var smgtag in smgtaglist)
//                {
//                    smgtagliststring = smgtagliststring + "data @> '{ \"SmgTags\": [\"" + smgtag.ToLower() + "\"]}' OR ";
//                }
//                smgtagliststring = smgtagliststring.Remove(smgtagliststring.Length - 4);

//                whereexpression = whereexpression + smgtagliststring + ")";
//            }



//            //Highlight
//            if (highlight != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Highlight\" : " + highlight.ToString().ToLower() + "}'";
//            }

//            //Active
//            if (activefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Active\" : " + activefilter.ToString().ToLower() + "}'";
//            }

//            //SmgActive
//            if (smgactivefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"SmgActive\" : " + smgactivefilter.ToString().ToLower() + "}'";
//            }




//            return whereexpression;
//        }

//        public static string CreateEventWhereExpression(List<string> idlist, List<string> orgidlist, List<string> rancidlist, List<string> typeidlist, List<string> topicrids, List<string> smgtaglist, List<string> districtlist, List<string> municipalitylist, List<string> tourismvereinlist, List<string> regionlist, Nullable<DateTime> begin, Nullable<DateTime> end, bool? activefilter, bool? smgactivefilter)
//        {
//            //Do muassimer die where zommbaschgeln

//            string whereexpression = "";
//            //IDLIST
//            if (idlist.Count > 0)
//            {
//                //Tuning force to use GIN Index
//                if (idlist.Count == 1)
//                {
//                    //whereexpression = whereexpression + "data @> '{\"Id\" : \"" + idlist.FirstOrDefault() + "\" }'";
//                    whereexpression = whereexpression + "Id = '" + idlist.FirstOrDefault() + "'";
//                }
//                else
//                {
//                    string idliststring = "";
//                    foreach (var accoid in idlist)
//                    {
//                        //idliststring = idliststring + "'\"" + accoid.ToUpper() + "\"', ";
//                        idliststring = idliststring + "'" + accoid.ToUpper() + "', ";
//                    }
//                    idliststring = idliststring.Remove(idliststring.Length - 2);

//                    //whereexpression = whereexpression + "data->'Id' in (" + idliststring + ")";
//                    whereexpression = whereexpression + "Id in (" + idliststring + ")";
//                }
//            }
//            //DISTRICT
//            if (districtlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (districtlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"DistrictId\" : \"" + districtlist.FirstOrDefault() + "\" }'";
//                }
//                else
//                {
//                    string districtliststring = "";
//                    foreach (var distid in districtlist)
//                    {
//                        districtliststring = districtliststring + "'\"" + distid.ToUpper() + "\"', ";
//                    }
//                    districtliststring = districtliststring.Remove(districtliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'DistrictId' in (" + districtliststring + ")";
//                }


//            }
//            //MUNICIPALITY
//            if (municipalitylist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (municipalitylist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"MunicipalityInfo\": { \"Id\": \"" + municipalitylist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string municipalityliststring = "";
//                    foreach (var munid in municipalitylist)
//                    {
//                        municipalityliststring = municipalityliststring + "'\"" + munid.ToUpper() + "\"', ";
//                    }
//                    municipalityliststring = municipalityliststring.Remove(municipalityliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'MunicipalityInfo' -> 'Id' in (" + municipalityliststring + ")";
//                }
//            }
//            //TOURISMVEREIN
//            if (tourismvereinlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (tourismvereinlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"TvInfo\": { \"Id\": \"" + tourismvereinlist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string tvliststring = "";
//                    foreach (var tvid in tourismvereinlist)
//                    {
//                        tvliststring = tvliststring + "'\"" + tvid.ToUpper() + "\"', ";
//                    }
//                    tvliststring = tvliststring.Remove(tvliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'TvInfo' -> 'Id' in (" + tvliststring + ")";
//                }
//            }
//            //REGION
//            if (regionlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (regionlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"RegionInfo\": { \"Id\": \"" + regionlist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string regionliststring = "";
//                    foreach (var tvid in regionlist)
//                    {
//                        regionliststring = regionliststring + "'\"" + tvid.ToUpper() + "\"', ";
//                    }
//                    regionliststring = regionliststring.Remove(regionliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'RegionInfo' -> 'Id' in (" + regionliststring + ")";
//                }
//            }


//            //Topic Rids
//            if (topicrids.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string topicliststring = "";
//                foreach (var topic in topicrids)
//                {
//                    topicliststring = topicliststring + "data @> '{ \"TopicRIDs\": [\"" + topic + "\"]}' OR ";
//                }
//                topicliststring = topicliststring.Remove(topicliststring.Length - 4);

//                whereexpression = whereexpression + topicliststring + ")";
//            }

//            //OrgIdList
//            if (orgidlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string orgidliststring = "";
//                foreach (var orgid in orgidlist)
//                {
//                    orgidliststring = orgidliststring + "data @> '{ \"OrgRID\": [\"" + orgid + "\"]}' OR ";
//                }
//                orgidliststring = orgidliststring.Remove(orgidliststring.Length - 4);

//                whereexpression = whereexpression + orgidliststring + ")";
//            }

//            //OrgIdList
//            if (rancidlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string rancidliststring = "";
//                foreach (var rancid in rancidlist)
//                {
//                    rancidliststring = rancidliststring + "data @> '{ \"Ranc\": [\"" + rancid + "\"]}' OR ";
//                }
//                rancidliststring = rancidliststring.Remove(rancidliststring.Length - 4);

//                whereexpression = whereexpression + rancidliststring + ")";
//            }

//            //OrgIdList
//            if (typeidlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string typeidliststring = "";
//                foreach (var typeid in typeidlist)
//                {
//                    typeidliststring = typeidliststring + "data @> '{ \"Type\": [\"" + typeid + "\"]}' OR ";
//                }
//                typeidliststring = typeidliststring.Remove(typeidliststring.Length - 4);

//                whereexpression = whereexpression + typeidliststring + ")";
//            }



//            //SmgTags Info
//            if (smgtaglist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string smgtagliststring = "";
//                foreach (var smgtag in smgtaglist)
//                {
//                    smgtagliststring = smgtagliststring + "data @> '{ \"SmgTags\": [\"" + smgtag.ToLower() + "\"]}' OR ";
//                }
//                smgtagliststring = smgtagliststring.Remove(smgtagliststring.Length - 4);

//                whereexpression = whereexpression + smgtagliststring + ")";
//            }



//            //Begin & End
//            if (begin != null && end != null)
//            {
//                //Beide nicht null
//                if (begin != DateTime.MinValue && end != DateTime.MaxValue)
//                {
//                    if (!String.IsNullOrEmpty(whereexpression))
//                        whereexpression = whereexpression + " AND ";

//                    whereexpression = whereexpression + "(((to_date(data ->> 'DateBegin', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "') AND (to_date(data ->> 'DateBegin', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')) OR ((to_date(data->> 'DateEnd', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "') AND(to_date(data->> 'DateEnd', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')))";
//                }
//                //Begin ist DateTime Min
//                if (begin == DateTime.MinValue && end != DateTime.MaxValue)
//                {
//                    if (!String.IsNullOrEmpty(whereexpression))
//                        whereexpression = whereexpression + " AND ";

//                    whereexpression = whereexpression + "(((to_date(data ->> 'DateBegin', 'YYYY-MM-DD') > '" + String.Format("{0:yyyy-MM-dd}", begin) + "') AND (to_date(data ->> 'DateBegin', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')) OR ((to_date(data->> 'DateEnd', 'YYYY-MM-DD') > '" + String.Format("{0:yyyy-MM-dd}", begin) + "') AND(to_date(data->> 'DateEnd', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')))";
//                }
//                //End ist DateTime Max
//                if (begin != DateTime.MinValue && end == DateTime.MaxValue)
//                {
//                    if (!String.IsNullOrEmpty(whereexpression))
//                        whereexpression = whereexpression + " AND ";

//                    whereexpression = whereexpression + "(((to_date(data ->> 'DateBegin', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "') AND (to_date(data ->> 'DateBegin', 'YYYY-MM-DD') < '" + String.Format("{0:yyyy-MM-dd}", end) + "')) OR ((to_date(data->> 'DateEnd', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "') AND(to_date(data->> 'DateEnd', 'YYYY-MM-DD') < '" + String.Format("{0:yyyy-MM-dd}", end) + "')))";
//                }
//            }


//            //Active
//            if (activefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Active\" : " + activefilter.ToString().ToLower() + "}'";
//            }

//            //SmgActive
//            if (smgactivefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"SmgActive\" : " + smgactivefilter.ToString().ToLower() + "}'";
//            }




//            return whereexpression;
//        }


//        // TO TEST
//        public static string CreateEventShortWhereExpression(DateTime start, DateTime end, string source, string eventlocation, string activefilter, List<string> eventidlist)
//        {
//            //Do muassimer die where zommbaschgeln

//            string whereexpression = "";
//            //IDLIST
//            if (eventidlist.Count > 0)
//            {
//                //Tuning force to use GIN Index
//                if (eventidlist.Count == 1)
//                {
//                    //whereexpression = whereexpression + "data @> '{\"Id\" : \"" + idlist.FirstOrDefault() + "\" }'";
//                    whereexpression = whereexpression + "Id = '" + eventidlist.FirstOrDefault() + "'";
//                }
//                else
//                {
//                    string idliststring = "";
//                    foreach (var accoid in eventidlist)
//                    {
//                        //idliststring = idliststring + "'\"" + accoid.ToUpper() + "\"', ";
//                        idliststring = idliststring + "'" + accoid.ToUpper() + "', ";
//                    }
//                    idliststring = idliststring.Remove(idliststring.Length - 2);

//                    //whereexpression = whereexpression + "data->'Id' in (" + idliststring + ")";
//                    whereexpression = whereexpression + "Id in (" + idliststring + ")";
//                }
//            }



//            //Begin & End
//            if (start != null && end != null)
//            {
//                if (start != DateTime.MinValue && end != DateTime.MaxValue)
//                {
//                    if (!String.IsNullOrEmpty(whereexpression))
//                        whereexpression = whereexpression + " AND ";

//                    whereexpression = whereexpression + "(((to_date(data ->> 'StartDate', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", start) + "') AND (to_date(data->> 'EndDate', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')))";
//                }
//                else if (start != DateTime.MinValue && end == DateTime.MaxValue)
//                {
//                    if (!String.IsNullOrEmpty(whereexpression))
//                        whereexpression = whereexpression + " AND ";

//                    whereexpression = whereexpression + "((to_date(data ->> 'StartDate', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", start) + "'))";
//                }
//                else if (start == DateTime.MinValue && end != DateTime.MaxValue)
//                {
//                    if (!String.IsNullOrEmpty(whereexpression))
//                        whereexpression = whereexpression + " AND ";

//                    whereexpression = whereexpression + "((to_date(data->> 'EndDate', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')))";
//                }

//            }

//            //Active
//            if (!String.IsNullOrEmpty(activefilter))
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Display1\" :  \"" + activefilter.ToString() + "\" }'";
//            }

//            //Source
//            if (!String.IsNullOrEmpty(source))
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Source\" : \"" + source.ToString() + "\" }'";
//            }

//            //EventLocation
//            if (!String.IsNullOrEmpty(eventlocation))
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"EventLocation\" : \"" + eventlocation.ToString() + "\" }'";
//            }

//            return whereexpression;
//        }

//        // TO TEST
//        public static string CreateEventShortWhereExpressionSpecial(DateTime start, DateTime end, string source, string eventlocation, string activefilter, List<string> eventidlist)
//        {
//            //Do muassimer die where zommbaschgeln

//            string whereexpression = "";
//            //IDLIST
//            if (eventidlist.Count > 0)
//            {
//                //Tuning force to use GIN Index
//                if (eventidlist.Count == 1)
//                {
//                    //whereexpression = whereexpression + "data @> '{\"Id\" : \"" + idlist.FirstOrDefault() + "\" }'";
//                    whereexpression = whereexpression + "Id = '" + eventidlist.FirstOrDefault() + "'";
//                }
//                else
//                {
//                    string idliststring = "";
//                    foreach (var accoid in eventidlist)
//                    {
//                        //idliststring = idliststring + "'\"" + accoid.ToUpper() + "\"', ";
//                        idliststring = idliststring + "'" + accoid.ToUpper() + "', ";
//                    }
//                    idliststring = idliststring.Remove(idliststring.Length - 2);

//                    //whereexpression = whereexpression + "data->'Id' in (" + idliststring + ")";
//                    whereexpression = whereexpression + "Id in (" + idliststring + ")";
//                }
//            }



//            //Begin & End
//            if (start != null && end != null)
//            {
//                if (start != DateTime.MinValue && end != DateTime.MaxValue)
//                {
//                    if (!String.IsNullOrEmpty(whereexpression))
//                        whereexpression = whereexpression + " AND ";

//                    whereexpression = whereexpression + "(((to_date(data ->> 'EndDate', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", start) + "') AND (to_date(data->> 'EndDate', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')))";
//                }
//                else if (start != DateTime.MinValue && end == DateTime.MaxValue)
//                {
//                    if (!String.IsNullOrEmpty(whereexpression))
//                        whereexpression = whereexpression + " AND ";

//                    whereexpression = whereexpression + "((to_date(data ->> 'EndDate', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", start) + "'))";
//                }
//                else if (start == DateTime.MinValue && end != DateTime.MaxValue)
//                {
//                    if (!String.IsNullOrEmpty(whereexpression))
//                        whereexpression = whereexpression + " AND ";

//                    whereexpression = whereexpression + "((to_date(data->> 'EndDate', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')))";
//                }

//            }

//            //Active
//            if (!String.IsNullOrEmpty(activefilter))
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Display1\" :  \"" + activefilter.ToString() + "\" }'";
//            }

//            //Source
//            if (!String.IsNullOrEmpty(source))
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Source\" : \"" + source.ToString() + "\" }'";
//            }

//            //EventLocation
//            if (!String.IsNullOrEmpty(eventlocation))
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"EventLocation\" : \"" + eventlocation.ToString() + "\" }'";
//            }

//            return whereexpression;
//        }


//        public static string CreateGastroWhereExpression(List<string> idlist, List<string> dishcodesids, List<string> ceremonycodesids, List<string> categorycodesids, List<string> facilitycodesids, List<string> smgtaglist, List<string> districtlist, List<string> municipalitylist, List<string> tourismvereinlist, List<string> regionlist, bool? activefilter, bool? smgactivefilter)
//        {
//            //Do muassimer die where zommbaschgeln

//            string whereexpression = "";
//            //IDLIST
//            if (idlist.Count > 0)
//            {
//                //Tuning force to use GIN Index
//                if (idlist.Count == 1)
//                {
//                    //whereexpression = whereexpression + "data @> '{\"Id\" : \"" + idlist.FirstOrDefault() + "\" }'";
//                    whereexpression = whereexpression + "Id = '" + idlist.FirstOrDefault() + "'";
//                }
//                else
//                {
//                    string idliststring = "";
//                    foreach (var accoid in idlist)
//                    {
//                        //idliststring = idliststring + "'\"" + accoid.ToUpper() + "\"', ";
//                        idliststring = idliststring + "'" + accoid + "', ";
//                    }
//                    idliststring = idliststring.Remove(idliststring.Length - 2);

//                    whereexpression = whereexpression + "Id in (" + idliststring + ")";
//                }
//            }
//            //DISTRICT
//            if (districtlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (districtlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"DistrictId\" : \"" + districtlist.FirstOrDefault() + "\" }'";
//                }
//                else
//                {
//                    string districtliststring = "";
//                    foreach (var distid in districtlist)
//                    {
//                        districtliststring = districtliststring + "'\"" + distid.ToUpper() + "\"', ";
//                    }
//                    districtliststring = districtliststring.Remove(districtliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'DistrictId' in (" + districtliststring + ")";
//                }


//            }
//            //MUNICIPALITY
//            if (municipalitylist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (municipalitylist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"MunicipalityInfo\": { \"Id\": \"" + municipalitylist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string municipalityliststring = "";
//                    foreach (var munid in municipalitylist)
//                    {
//                        municipalityliststring = municipalityliststring + "'\"" + munid.ToUpper() + "\"', ";
//                    }
//                    municipalityliststring = municipalityliststring.Remove(municipalityliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'MunicipalityInfo' -> 'Id' in (" + municipalityliststring + ")";
//                }
//            }
//            //TOURISMVEREIN
//            if (tourismvereinlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (tourismvereinlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"TvInfo\": { \"Id\": \"" + tourismvereinlist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string tvliststring = "";
//                    foreach (var tvid in tourismvereinlist)
//                    {
//                        tvliststring = tvliststring + "'\"" + tvid.ToUpper() + "\"', ";
//                    }
//                    tvliststring = tvliststring.Remove(tvliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'TvInfo' -> 'Id' in (" + tvliststring + ")";
//                }
//            }
//            //REGION
//            if (regionlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (regionlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"RegionInfo\": { \"Id\": \"" + regionlist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string regionliststring = "";
//                    foreach (var tvid in regionlist)
//                    {
//                        regionliststring = regionliststring + "'\"" + tvid.ToUpper() + "\"', ";
//                    }
//                    regionliststring = regionliststring.Remove(regionliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'RegionInfo' -> 'Id' in (" + regionliststring + ")";
//                }
//            }


//            //Dishcode Rids
//            if (dishcodesids.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";


//                //Tuning force to use GIN Index
//                if (dishcodesids.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"DishRates\": [{ \"Id\": \"" + dishcodesids.FirstOrDefault() + "\" }] }'";
//                }
//                else
//                {

//                    string dishcodeliststring = "";

//                    foreach (var dishcode in dishcodesids)
//                    {
//                        dishcodeliststring = dishcodeliststring + "(data @> '{\"DishRates\": [{ \"Id\": \"" + dishcode.ToUpper() + "\" }] }') OR ";
//                    }
//                    dishcodeliststring = dishcodeliststring.Remove(dishcodeliststring.Length - 4);

//                    whereexpression = whereexpression + "(" + dishcodeliststring + ")";
//                }
//            }

//            //Capacity Ceremony
//            if (ceremonycodesids.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";


//                //Tuning force to use GIN Index
//                if (ceremonycodesids.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"CapacityCeremony\": [{ \"Id\": \"" + ceremonycodesids.FirstOrDefault().ToUpper() + "\" }] }'";
//                }
//                else
//                {

//                    string ceremonycodeliststring = "";

//                    foreach (var ceremonycode in ceremonycodesids)
//                    {
//                        ceremonycodeliststring = ceremonycodeliststring + "(data @> '{\"CapacityCeremony\": [{ \"Id\": \"" + ceremonycode.ToUpper() + "\" }] }') OR ";
//                    }
//                    ceremonycodeliststring = ceremonycodeliststring.Remove(ceremonycodeliststring.Length - 4);

//                    whereexpression = whereexpression + "(" + ceremonycodeliststring + ")";
//                }
//            }

//            //Category Code
//            if (categorycodesids.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";


//                //Tuning force to use GIN Index
//                if (categorycodesids.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"CategoryCodes\": [{ \"Id\": \"" + categorycodesids.FirstOrDefault().ToUpper() + "\" }] }'";
//                }
//                else
//                {

//                    string categorycodeliststring = "";

//                    foreach (var categorycode in categorycodesids)
//                    {
//                        categorycodeliststring = categorycodeliststring + "(data @> '{\"CategoryCodes\": [{ \"Id\": \"" + categorycode.ToUpper() + "\" }] }') OR ";
//                    }
//                    categorycodeliststring = categorycodeliststring.Remove(categorycodeliststring.Length - 4);

//                    whereexpression = whereexpression + "(" + categorycodeliststring + ")";
//                }
//            }

//            //OrgIdList
//            if (facilitycodesids.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";


//                //Tuning force to use GIN Index
//                if (facilitycodesids.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"Facilities\": [{ \"Id\": \"" + facilitycodesids.FirstOrDefault().ToUpper() + "\" }] }'";
//                }
//                else
//                {

//                    string facilitycodeliststring = "";

//                    foreach (var facilitycode in facilitycodesids)
//                    {
//                        facilitycodeliststring = facilitycodeliststring + "(data @> '{\"Facilities\": [{ \"Id\": \"" + facilitycode.ToUpper() + "\" }] }') OR ";
//                    }
//                    facilitycodeliststring = facilitycodeliststring.Remove(facilitycodeliststring.Length - 4);

//                    whereexpression = whereexpression + "(" + facilitycodeliststring + ")";
//                }
//            }



//            //SmgTags Info
//            if (smgtaglist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string smgtagliststring = "";
//                foreach (var smgtag in smgtaglist)
//                {
//                    smgtagliststring = smgtagliststring + "data @> '{ \"SmgTags\": [\"" + smgtag.ToLower() + "\"]}' OR ";
//                }
//                smgtagliststring = smgtagliststring.Remove(smgtagliststring.Length - 4);

//                whereexpression = whereexpression + smgtagliststring + ")";
//            }

//            //Active
//            if (activefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Active\" : " + activefilter.ToString().ToLower() + "}'";
//            }

//            //SmgActive
//            if (smgactivefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"SmgActive\" : " + smgactivefilter.ToString().ToLower() + "}'";
//            }




//            return whereexpression;
//        }

//        public static string CreateArticleWhereExpression(List<string> idlist, List<string> typelist, List<string> subtypelist, List<string> smgtaglist, List<string> languagelist, bool? highlight, bool? activefilter, bool? smgactivefilter)
//        {
//            //Do muassimer die where zommbaschgeln

//            string whereexpression = "";
//            //IDLIST
//            if (idlist.Count > 0)
//            {
//                //Tuning force to use GIN Index
//                if (idlist.Count == 1)
//                {
//                    //whereexpression = whereexpression + "data @> '{\"Id\" : \"" + idlist.FirstOrDefault().ToUpper() + "\" }'";
//                    whereexpression = whereexpression + "Id = '" + idlist.FirstOrDefault() + "'";
//                }
//                else
//                {
//                    string idliststring = "";
//                    foreach (var accoid in idlist)
//                    {
//                        //idliststring = idliststring + "'\"" + accoid.ToUpper() + "\"', ";
//                        idliststring = idliststring + "'" + accoid + "', ";
//                    }
//                    idliststring = idliststring.Remove(idliststring.Length - 2);

//                    whereexpression = whereexpression + "Id in (" + idliststring + ")";
//                }
//            }

//            //Type
//            if (typelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (typelist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"Type\" : \"" + typelist.FirstOrDefault() + "\" }'";
//                }
//                else
//                {
//                    string typeliststring = "";
//                    foreach (var typeid in typelist)
//                    {
//                        typeliststring = typeliststring + "'\"" + typeid + "\"', ";
//                    }
//                    typeliststring = typeliststring.Remove(typeliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'Type' in (" + typeliststring + ")";
//                }
//            }

//            //IDLIST
//            if (subtypelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";


//                //Tuning force to use GIN Index
//                if (subtypelist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"SubType\" : \"" + subtypelist.FirstOrDefault() + "\" }'";
//                }
//                else
//                {
//                    string subtypeliststring = "";
//                    foreach (var subtypeid in subtypelist)
//                    {
//                        subtypeliststring = subtypeliststring + "'\"" + subtypeid + "\"', ";
//                    }
//                    subtypeliststring = subtypeliststring.Remove(subtypeliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'SubType' in (" + subtypeliststring + ")";
//                }
//            }





//            //SmgTags Info
//            if (smgtaglist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string smgtagliststring = "";
//                foreach (var smgtag in smgtaglist)
//                {
//                    smgtagliststring = smgtagliststring + "data @> '{ \"SmgTags\": [\"" + smgtag.ToLower() + "\"]}' OR ";
//                }
//                smgtagliststring = smgtagliststring.Remove(smgtagliststring.Length - 4);

//                whereexpression = whereexpression + smgtagliststring + ")";
//            }

//            //Language Info
//            if (languagelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string languageliststring = "";
//                foreach (var language in languagelist)
//                {
//                    languageliststring = languageliststring + "data @> '{ \"HasLanguage\": [\"" + language + "\"]}' OR ";
//                }
//                languageliststring = languageliststring.Remove(languageliststring.Length - 4);

//                whereexpression = whereexpression + languageliststring + ")";
//            }



//            //Highlight
//            if (highlight != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Highlight\" : " + highlight.ToString().ToLower() + "}'";
//            }

//            //Active
//            if (activefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Active\" : " + activefilter.ToString().ToLower() + "}'";
//            }

//            //SmgActive
//            if (smgactivefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"SmgActive\" : " + smgactivefilter.ToString().ToLower() + "}'";
//            }




//            return whereexpression;
//        }

//        public static string CreateSmgPoiWhereExpression(List<string> idlist, List<string> typelist, List<string> subtypelist, List<string> poitypelist, List<string> languagelist, List<string> smgtaglist, List<string> districtlist, List<string> municipalitylist, List<string> tourismvereinlist, List<string> regionlist, List<string> arealist, List<string> sourcelist, bool? highlight, bool? activefilter, bool? smgactivefilter)
//        {
//            //Do muassimer die where zommbaschgeln

//            string whereexpression = "";
//            //IDLIST
//            if (idlist.Count > 0)
//            {
//                //Tuning force to use GIN Index
//                if (idlist.Count == 1)
//                {
//                    //whereexpression = whereexpression + "data @> '{\"Id\" : \"" + idlist.FirstOrDefault() + "\" }'";
//                    whereexpression = whereexpression + "Id = '" + idlist.FirstOrDefault() + "'";
//                }
//                else
//                {
//                    string idliststring = "";
//                    foreach (var accoid in idlist)
//                    {
//                        idliststring = idliststring + "'" + accoid + "', ";
//                        //idliststring = idliststring + "'\"" + accoid.ToUpper() + "\"', ";
//                    }
//                    idliststring = idliststring.Remove(idliststring.Length - 2);

//                    whereexpression = whereexpression + "Id in (" + idliststring + ")";
//                }
//            }
//            //DISTRICT
//            if (districtlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (districtlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"DistrictId\" : \"" + districtlist.FirstOrDefault() + "\" }'";
//                }
//                else
//                {
//                    string districtliststring = "";
//                    foreach (var distid in districtlist)
//                    {
//                        districtliststring = districtliststring + "'\"" + distid.ToUpper() + "\"', ";
//                    }
//                    districtliststring = districtliststring.Remove(districtliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'DistrictId' in (" + districtliststring + ")";
//                }


//            }
//            //MUNICIPALITY
//            if (municipalitylist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (municipalitylist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"MunicipalityInfo\": { \"Id\": \"" + municipalitylist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string municipalityliststring = "";
//                    foreach (var munid in municipalitylist)
//                    {
//                        municipalityliststring = municipalityliststring + "'\"" + munid.ToUpper() + "\"', ";
//                    }
//                    municipalityliststring = municipalityliststring.Remove(municipalityliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'MunicipalityInfo' -> 'Id' in (" + municipalityliststring + ")";
//                }
//            }
//            //TOURISMVEREIN
//            if (tourismvereinlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (tourismvereinlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"TvInfo\": { \"Id\": \"" + tourismvereinlist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string tvliststring = "";
//                    foreach (var tvid in tourismvereinlist)
//                    {
//                        tvliststring = tvliststring + "'\"" + tvid.ToUpper() + "\"', ";
//                    }
//                    tvliststring = tvliststring.Remove(tvliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'TvInfo' -> 'Id' in (" + tvliststring + ")";
//                }
//            }
//            //REGION
//            if (regionlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (regionlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"RegionInfo\": { \"Id\": \"" + regionlist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string regionliststring = "";
//                    foreach (var tvid in regionlist)
//                    {
//                        regionliststring = regionliststring + "'\"" + tvid.ToUpper() + "\"', ";
//                    }
//                    regionliststring = regionliststring.Remove(regionliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'RegionInfo' -> 'Id' in (" + regionliststring + ")";
//                }
//            }

//            //AREA
//            if (arealist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string arealiststring = "";
//                foreach (var area in arealist)
//                {
//                    arealiststring = arealiststring + "data @> '{ \"AreaId\": [\"" + area + "\"]}' OR ";
//                }
//                arealiststring = arealiststring.Remove(arealiststring.Length - 4);

//                whereexpression = whereexpression + arealiststring + ")";
//            }

//            //Activity Type
//            if (typelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string typestring = "";
//                foreach (var typeId in typelist)
//                {
//                    typestring = typestring + "data @> '{ \"Type\": \"" + typeId + "\"}' OR ";
//                }
//                typestring = typestring.Remove(typestring.Length - 4);

//                whereexpression = whereexpression + typestring + ")";
//            }

//            //Activity Sub Type            
//            if (subtypelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string subtypeliststring = "";
//                foreach (var subtype in subtypelist)
//                {
//                    subtypeliststring = subtypeliststring + "data @> '{ \"SubType\": \"" + subtype + "\"}' OR ";
//                }
//                subtypeliststring = subtypeliststring.Remove(subtypeliststring.Length - 4);

//                whereexpression = whereexpression + subtypeliststring + ")";
//            }

//            //Activity POI Type            
//            if (poitypelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string poitypeliststring = "";
//                foreach (var poitype in poitypelist)
//                {
//                    poitypeliststring = poitypeliststring + "data @> '{ \"PoiType\": \"" + poitype + "\"}' OR ";
//                }
//                poitypeliststring = poitypeliststring.Remove(poitypeliststring.Length - 4);

//                whereexpression = whereexpression + poitypeliststring + ")";
//            }

//            //Language Info
//            if (languagelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string languageliststring = "";
//                foreach (var language in languagelist)
//                {
//                    languageliststring = languageliststring + "data @> '{ \"HasLanguage\": [\"" + language + "\"]}' OR ";
//                }
//                languageliststring = languageliststring.Remove(languageliststring.Length - 4);

//                whereexpression = whereexpression + languageliststring + ")";
//            }

//            //Source
//            if (sourcelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (sourcelist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"SyncSourceInterface\": \"" + sourcelist.FirstOrDefault() + "\" }'";
//                }
//                else
//                {

//                    string sourceliststring = "";
//                    foreach (var sourceid in sourcelist)
//                    {
//                        sourceliststring = sourceliststring + "'\"" + sourceid + "\"', ";
//                    }
//                    sourceliststring = sourceliststring.Remove(sourceliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'SyncSourceInterface' in (" + sourceliststring + ")";
//                }
//            }


//            //SmgTags Info
//            if (smgtaglist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string smgtagliststring = "";
//                foreach (var smgtag in smgtaglist)
//                {
//                    smgtagliststring = smgtagliststring + "data @> '{ \"SmgTags\": [\"" + smgtag.ToLower() + "\"]}' OR ";
//                }
//                smgtagliststring = smgtagliststring.Remove(smgtagliststring.Length - 4);

//                whereexpression = whereexpression + smgtagliststring + ")";
//            }



//            //Highlight
//            if (highlight != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Highlight\" : " + highlight.ToString().ToLower() + "}'";
//            }

//            //Active
//            if (activefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Active\" : " + activefilter.ToString().ToLower() + "}'";
//            }

//            //SmgActive
//            if (smgactivefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"SmgActive\" : " + smgactivefilter.ToString().ToLower() + "}'";
//            }




//            return whereexpression;
//        }

//        public static string CreatePackageWhereExpression(List<string> idlist, List<string> accolist, List<string> boardlist, List<string> themelist, List<string> smgtaglist, List<string> districtlist, List<string> municipalitylist, List<string> tourismvereinlist, List<string> regionlist, DateTime validfrom, DateTime validto, bool longstay, bool shortstay, bool? activefilter, bool? smgactivefilter)
//        {
//            //Do muassimer die where zommbaschgeln

//            string whereexpression = "";
//            //IDLIST
//            if (idlist.Count > 0)
//            {
//                //Tuning force to use GIN Index
//                if (idlist.Count == 1)
//                {
//                    //whereexpression = whereexpression + "data @> '{\"Id\" : \"" + idlist.FirstOrDefault().ToUpper() + "\" }'";
//                    whereexpression = whereexpression + "Id = '" + idlist.FirstOrDefault() + "'";
//                }
//                else
//                {
//                    string idliststring = "";
//                    foreach (var pacakgeid in idlist)
//                    {
//                        //idliststring = idliststring + "'\"" + pacakgeid.ToUpper() + "\"', ";
//                        idliststring = idliststring + "'" + pacakgeid + "', ";

//                    }
//                    idliststring = idliststring.Remove(idliststring.Length - 2);

//                    whereexpression = whereexpression + "Id in (" + idliststring + ")";
//                }
//            }
//            //DISTRICT
//            if (districtlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (districtlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"DistrictId\" : \"" + districtlist.FirstOrDefault() + "\" }'";
//                }
//                else
//                {
//                    string districtliststring = "";
//                    foreach (var distid in districtlist)
//                    {
//                        districtliststring = districtliststring + "'\"" + distid.ToUpper() + "\"', ";
//                    }
//                    districtliststring = districtliststring.Remove(districtliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'DistrictId' in (" + districtliststring + ")";
//                }


//            }
//            //MUNICIPALITY
//            if (municipalitylist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (municipalitylist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"MunicipalityInfo\": { \"Id\": \"" + municipalitylist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string municipalityliststring = "";
//                    foreach (var munid in municipalitylist)
//                    {
//                        municipalityliststring = municipalityliststring + "'\"" + munid.ToUpper() + "\"', ";
//                    }
//                    municipalityliststring = municipalityliststring.Remove(municipalityliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'MunicipalityInfo' -> 'Id' in (" + municipalityliststring + ")";
//                }
//            }
//            //TOURISMVEREIN
//            if (tourismvereinlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (tourismvereinlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"TvInfo\": { \"Id\": \"" + tourismvereinlist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string tvliststring = "";
//                    foreach (var tvid in tourismvereinlist)
//                    {
//                        tvliststring = tvliststring + "'\"" + tvid.ToUpper() + "\"', ";
//                    }
//                    tvliststring = tvliststring.Remove(tvliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'TvInfo' -> 'Id' in (" + tvliststring + ")";
//                }
//            }
//            //REGION
//            if (regionlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (regionlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"RegionInfo\": { \"Id\": \"" + regionlist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string regionliststring = "";
//                    foreach (var tvid in regionlist)
//                    {
//                        regionliststring = regionliststring + "'\"" + tvid.ToUpper() + "\"', ";
//                    }
//                    regionliststring = regionliststring.Remove(regionliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'RegionInfo' -> 'Id' in (" + regionliststring + ")";
//                }
//            }
//            //SmgTags Info
//            if (smgtaglist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string smgtagliststring = "";
//                foreach (var smgtag in smgtaglist)
//                {
//                    smgtagliststring = smgtagliststring + "data @> '{ \"SmgTags\": [\"" + smgtag.ToLower() + "\"]}' OR ";
//                }
//                smgtagliststring = smgtagliststring.Remove(smgtagliststring.Length - 4);

//                whereexpression = whereexpression + smgtagliststring + ")";
//            }
//            //Package Themes Info
//            if (themelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string themeliststring = "";
//                foreach (var theme in themelist)
//                {
//                    themeliststring = themeliststring + "data @> '{ \"PackageThemeList\": [\"" + theme + "\"]}' OR ";
//                }
//                themeliststring = themeliststring.Remove(themeliststring.Length - 4);

//                whereexpression = whereexpression + themeliststring + ")";
//            }
//            //Board Info schaugn ob des geat!!! umgekearter foll
//            if (boardlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string boardliststring = "";
//                foreach (var boardid in boardlist)
//                {
//                    boardliststring = boardliststring + "data @> '{ \"Services\": [\"" + boardid + "\"]}' OR ";
//                }
//                boardliststring = boardliststring.Remove(boardliststring.Length - 4);

//                whereexpression = whereexpression + boardliststring + ")";
//            }
//            //Acco List
//            if (accolist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string accoliststring = "";
//                foreach (var accoid in accolist)
//                {
//                    accoliststring = accoliststring + "data @> '{ \"HotelId\": \"" + accoid + "\"}' OR ";
//                }
//                accoliststring = accoliststring.Remove(accoliststring.Length - 4);

//                whereexpression = whereexpression + accoliststring + ")";
//            }
//            //Shortstay
//            if (shortstay)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"ShortStay\" : " + shortstay.ToString().ToLower() + "}'";
//            }
//            //Longstay
//            if (longstay)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"LongStay\" : " + longstay.ToString().ToLower() + "}'";
//            }

//            //Datum von bis valid
//            if (validfrom != DateTime.MinValue)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "to_date(data ->> 'ValidStart', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", validfrom) + "'";
//            }
//            if (validto != DateTime.MaxValue)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "to_date(data ->> 'ValidStop', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", validto) + "'";
//            }





//            //Active
//            if (activefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Active\" : " + activefilter.ToString().ToLower() + "}'";
//            }

//            //SmgActive
//            if (smgactivefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"SmgActive\" : " + smgactivefilter.ToString().ToLower() + "}'";
//            }




//            return whereexpression;
//        }

//        //MobileData

//        public static string CreateSmgPoiMobileWhereExpression(List<string> idlist, List<string> typelist, List<string> subtypelist, List<string> poitypelist, List<string> difficultylist, List<string> smgtaglist, List<string> languagelist, List<string> districtlist, List<string> municipalitylist, List<string> tourismvereinlist, List<string> regionlist, bool distance, int distancemin, int distancemax, bool duration, double durationmin, double durationmax, bool altitude, int altitudemin, int altitudemax, bool? highlight, bool? activefilter)
//        {
//            //Do muassimer die where zommbaschgeln

//            string whereexpression = "";
//            //IDLIST
//            if (idlist.Count > 0)
//            {
//                //Tuning force to use GIN Index
//                if (idlist.Count == 1)
//                {
//                    //whereexpression = whereexpression + "data @> '{\"Id\" : \"" + idlist.FirstOrDefault() + "\" }'";
//                    whereexpression = whereexpression + "Id = '" + idlist.FirstOrDefault() + "'";
//                }
//                else
//                {
//                    string idliststring = "";
//                    foreach (var accoid in idlist)
//                    {
//                        idliststring = idliststring + "'" + accoid + "', ";
//                        //idliststring = idliststring + "'\"" + accoid.ToUpper() + "\"', ";
//                    }
//                    idliststring = idliststring.Remove(idliststring.Length - 2);

//                    whereexpression = whereexpression + "Id in (" + idliststring + ")";
//                }
//            }
//            //DISTRICT
//            if (districtlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (districtlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"DistrictId\" : \"" + districtlist.FirstOrDefault() + "\" }'";
//                }
//                else
//                {
//                    string districtliststring = "";
//                    foreach (var distid in districtlist)
//                    {
//                        districtliststring = districtliststring + "'\"" + distid.ToUpper() + "\"', ";
//                    }
//                    districtliststring = districtliststring.Remove(districtliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'DistrictId' in (" + districtliststring + ")";
//                }


//            }
//            //MUNICIPALITY
//            if (municipalitylist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (municipalitylist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"MunicipalityInfo\": { \"Id\": \"" + municipalitylist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string municipalityliststring = "";
//                    foreach (var munid in municipalitylist)
//                    {
//                        municipalityliststring = municipalityliststring + "'\"" + munid.ToUpper() + "\"', ";
//                    }
//                    municipalityliststring = municipalityliststring.Remove(municipalityliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'MunicipalityInfo' -> 'Id' in (" + municipalityliststring + ")";
//                }
//            }
//            //TOURISMVEREIN
//            if (tourismvereinlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (tourismvereinlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"TvInfo\": { \"Id\": \"" + tourismvereinlist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string tvliststring = "";
//                    foreach (var tvid in tourismvereinlist)
//                    {
//                        tvliststring = tvliststring + "'\"" + tvid.ToUpper() + "\"', ";
//                    }
//                    tvliststring = tvliststring.Remove(tvliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'TvInfo' -> 'Id' in (" + tvliststring + ")";
//                }
//            }
//            //REGION
//            if (regionlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (regionlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"RegionInfo\": { \"Id\": \"" + regionlist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string regionliststring = "";
//                    foreach (var tvid in regionlist)
//                    {
//                        regionliststring = regionliststring + "'\"" + tvid.ToUpper() + "\"', ";
//                    }
//                    regionliststring = regionliststring.Remove(regionliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'RegionInfo' -> 'Id' in (" + regionliststring + ")";
//                }
//            }

//            //Difficulty
//            if (difficultylist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string difficultystring = "";
//                foreach (var difficultyId in difficultylist)
//                {
//                    difficultystring = difficultystring + "data @> '{ \"Difficulty\": \"" + difficultyId + "\"}' OR ";
//                }
//                difficultystring = difficultystring.Remove(difficultystring.Length - 4);

//                whereexpression = whereexpression + difficultystring + ")";
//            }

//            //Language Info
//            if (languagelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string languageliststring = "";
//                foreach (var language in languagelist)
//                {
//                    languageliststring = languageliststring + "data @> '{ \"HasLanguage\": [\"" + language + "\"]}' OR ";
//                }
//                languageliststring = languageliststring.Remove(languageliststring.Length - 4);

//                whereexpression = whereexpression + languageliststring + ")";
//            }


//            //Typelist Info
//            if (typelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string smgtagtypeliststring = "";
//                foreach (var smgtag in typelist)
//                {
//                    smgtagtypeliststring = smgtagtypeliststring + "data @> '{ \"SmgTags\": [\"" + smgtag.ToLower() + "\"]}' OR ";
//                }
//                smgtagtypeliststring = smgtagtypeliststring.Remove(smgtagtypeliststring.Length - 4);

//                whereexpression = whereexpression + smgtagtypeliststring + ")";
//            }

//            //SubTypelist Info
//            if (subtypelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string smgtagsubtypeliststring = "";
//                foreach (var smgtag in subtypelist)
//                {
//                    smgtagsubtypeliststring = smgtagsubtypeliststring + "data @> '{ \"SmgTags\": [\"" + smgtag.ToLower() + "\"]}' OR ";
//                }
//                smgtagsubtypeliststring = smgtagsubtypeliststring.Remove(smgtagsubtypeliststring.Length - 4);

//                whereexpression = whereexpression + smgtagsubtypeliststring + ")";
//            }

//            //PoiTypelist Info
//            if (poitypelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string smgtagpoitypeliststring = "";
//                foreach (var smgtag in poitypelist)
//                {
//                    smgtagpoitypeliststring = smgtagpoitypeliststring + "data @> '{ \"SmgTags\": [\"" + smgtag.ToLower() + "\"]}' OR ";
//                }
//                smgtagpoitypeliststring = smgtagpoitypeliststring.Remove(smgtagpoitypeliststring.Length - 4);

//                whereexpression = whereexpression + smgtagpoitypeliststring + ")";
//            }

//            //SmgTags Info
//            if (smgtaglist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string smgtagliststring = "";
//                foreach (var smgtag in smgtaglist)
//                {
//                    smgtagliststring = smgtagliststring + "data @> '{ \"SmgTags\": [\"" + smgtag.ToLower() + "\"]}' OR ";
//                }
//                smgtagliststring = smgtagliststring.Remove(smgtagliststring.Length - 4);

//                whereexpression = whereexpression + smgtagliststring + ")";
//            }

//            //DURATION
//            if (duration)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "(data ->> 'DistanceDuration')::numeric > " + durationmin + " AND (data ->> 'DistanceDuration')::numeric < " + durationmax;

//            }
//            //ALTITUDE
//            if (altitude)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "(data ->> 'AltitudeDifference')::numeric > " + altitudemin + " AND (data ->> 'AltitudeDifference')::numeric < " + altitudemax;
//            }
//            //DISTANCE
//            if (distance)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "(data ->> 'DistanceLength')::numeric > " + distancemin + " AND (data ->> 'DistanceLength')::numeric < " + distancemax;
//            }

//            //Highlight
//            if (highlight != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Highlight\" : " + highlight.ToString().ToLower() + "}'";
//            }

//            //Active
//            if (activefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Active\" : " + activefilter.ToString().ToLower() + "}'";
//            }

//            return whereexpression;
//        }

//        public static string CreateEventMobileWhereExpression(List<string> idlist, List<string> orgidlist, List<string> rancidlist, List<string> typeidlist, List<string> topicrids, List<string> smgtaglist, List<string> districtlist, List<string> municipalitylist, List<string> tourismvereinlist, List<string> regionlist, Nullable<DateTime> begin, Nullable<DateTime> end, bool fromnow, bool? activefilter, bool? smgactivefilter)
//        {
//            //Do muassimer die where zommbaschgeln

//            string whereexpression = "";
//            //IDLIST
//            if (idlist.Count > 0)
//            {
//                //Tuning force to use GIN Index
//                if (idlist.Count == 1)
//                {
//                    //whereexpression = whereexpression + "data @> '{\"Id\" : \"" + idlist.FirstOrDefault() + "\" }'";
//                    whereexpression = whereexpression + "Id = '" + idlist.FirstOrDefault() + "'";
//                }
//                else
//                {
//                    string idliststring = "";
//                    foreach (var accoid in idlist)
//                    {
//                        //idliststring = idliststring + "'\"" + accoid.ToUpper() + "\"', ";
//                        idliststring = idliststring + "'" + accoid.ToUpper() + "', ";
//                    }
//                    idliststring = idliststring.Remove(idliststring.Length - 2);

//                    //whereexpression = whereexpression + "data->'Id' in (" + idliststring + ")";
//                    whereexpression = whereexpression + "Id in (" + idliststring + ")";
//                }
//            }
//            //DISTRICT
//            if (districtlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (districtlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"DistrictId\" : \"" + districtlist.FirstOrDefault() + "\" }'";
//                }
//                else
//                {
//                    string districtliststring = "";
//                    foreach (var distid in districtlist)
//                    {
//                        districtliststring = districtliststring + "'\"" + distid.ToUpper() + "\"', ";
//                    }
//                    districtliststring = districtliststring.Remove(districtliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'DistrictId' in (" + districtliststring + ")";
//                }


//            }
//            //MUNICIPALITY
//            if (municipalitylist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (municipalitylist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"MunicipalityInfo\": { \"Id\": \"" + municipalitylist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string municipalityliststring = "";
//                    foreach (var munid in municipalitylist)
//                    {
//                        municipalityliststring = municipalityliststring + "'\"" + munid.ToUpper() + "\"', ";
//                    }
//                    municipalityliststring = municipalityliststring.Remove(municipalityliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'MunicipalityInfo' -> 'Id' in (" + municipalityliststring + ")";
//                }
//            }
//            //TOURISMVEREIN
//            if (tourismvereinlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (tourismvereinlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"TvInfo\": { \"Id\": \"" + tourismvereinlist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string tvliststring = "";
//                    foreach (var tvid in tourismvereinlist)
//                    {
//                        tvliststring = tvliststring + "'\"" + tvid.ToUpper() + "\"', ";
//                    }
//                    tvliststring = tvliststring.Remove(tvliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'TvInfo' -> 'Id' in (" + tvliststring + ")";
//                }
//            }
//            //REGION
//            if (regionlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                //Tuning force to use GIN Index
//                if (regionlist.Count == 1)
//                {
//                    whereexpression = whereexpression + "data @> '{\"LocationInfo\" : { \"RegionInfo\": { \"Id\": \"" + regionlist.FirstOrDefault() + "\" } } }'";
//                }
//                else
//                {

//                    string regionliststring = "";
//                    foreach (var tvid in regionlist)
//                    {
//                        regionliststring = regionliststring + "'\"" + tvid.ToUpper() + "\"', ";
//                    }
//                    regionliststring = regionliststring.Remove(regionliststring.Length - 2);

//                    whereexpression = whereexpression + "data->'LocationInfo'-> 'RegionInfo' -> 'Id' in (" + regionliststring + ")";
//                }
//            }


//            //Topic Rids
//            if (topicrids.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string topicliststring = "";
//                foreach (var topic in topicrids)
//                {
//                    topicliststring = topicliststring + "data @> '{ \"TopicRIDs\": [\"" + topic + "\"]}' OR ";
//                }
//                topicliststring = topicliststring.Remove(topicliststring.Length - 4);

//                whereexpression = whereexpression + topicliststring + ")";
//            }

//            //OrgIdList
//            if (orgidlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string orgidliststring = "";
//                foreach (var orgid in orgidlist)
//                {
//                    orgidliststring = orgidliststring + "data @> '{ \"OrgRID\": [\"" + orgid + "\"]}' OR ";
//                }
//                orgidliststring = orgidliststring.Remove(orgidliststring.Length - 4);

//                whereexpression = whereexpression + orgidliststring + ")";
//            }

//            //OrgIdList
//            if (rancidlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string rancidliststring = "";
//                foreach (var rancid in rancidlist)
//                {
//                    rancidliststring = rancidliststring + "data @> '{ \"Ranc\": [\"" + rancid + "\"]}' OR ";
//                }
//                rancidliststring = rancidliststring.Remove(rancidliststring.Length - 4);

//                whereexpression = whereexpression + rancidliststring + ")";
//            }

//            ////OrgIdList
//            if (typeidlist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string typeidliststring = "";
//                foreach (var typeid in typeidlist)
//                {
//                    typeidliststring = typeidliststring + "data @> '{ \"Type\": [\"" + typeid + "\"]}' OR ";
//                }
//                typeidliststring = typeidliststring.Remove(typeidliststring.Length - 4);

//                whereexpression = whereexpression + typeidliststring + ")";
//            }

//            //SmgTags Info
//            if (smgtaglist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string smgtagliststring = "";
//                foreach (var smgtag in smgtaglist)
//                {
//                    smgtagliststring = smgtagliststring + "data @> '{ \"SmgTags\": [\"" + smgtag.ToLower() + "\"]}' OR ";
//                }
//                smgtagliststring = smgtagliststring.Remove(smgtagliststring.Length - 4);

//                whereexpression = whereexpression + smgtagliststring + ")";
//            }



//            //Begin & End
//            if (begin != null && end != null)
//            {
//                if (begin != DateTime.MinValue && end != DateTime.MaxValue)
//                {
//                    if (!String.IsNullOrEmpty(whereexpression))
//                        whereexpression = whereexpression + " AND ";

//                    whereexpression = whereexpression + "(((to_date(data ->> 'DateBegin', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "') AND (to_date(data ->> 'DateBegin', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')) OR ((to_date(data->> 'DateEnd', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "') AND(to_date(data->> 'DateEnd', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')))";
//                }
//                else if (begin != DateTime.MinValue && end == DateTime.MaxValue)
//                {
//                    if (!String.IsNullOrEmpty(whereexpression))
//                        whereexpression = whereexpression + " AND ";

//                    whereexpression = whereexpression + "(((to_date(data ->> 'DateBegin', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "')))";
//                }
//            }

//            //From Now
//            if (fromnow)
//            {

//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "(((to_date(data ->> 'DateBegin', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", DateTime.Now) + "')))";

//            }

//            //Active
//            if (activefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Active\" : " + activefilter.ToString().ToLower() + "}'";
//            }

//            //SmgActive
//            if (smgactivefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"SmgActive\" : " + smgactivefilter.ToString().ToLower() + "}'";
//            }




//            return whereexpression;
//        }

//        public static string CreateTipsMobileWhereExpression(List<string> typestoexclude, List<string> subtypestoexclude, List<string> poitypestoexclude, List<string> languagelist, bool? highlight, bool? activefilter)
//        {
//            //Do muassimer die where zommbaschgeln

//            string whereexpression = "";

//            //Language Info
//            if (languagelist.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string languageliststring = "";
//                foreach (var language in languagelist)
//                {
//                    languageliststring = languageliststring + "data @> '{ \"HasLanguage\": [\"" + language + "\"]}' OR ";
//                }
//                languageliststring = languageliststring.Remove(languageliststring.Length - 4);

//                whereexpression = whereexpression + languageliststring + ")";
//            }



//            //Highlight
//            if (highlight != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Highlight\" : " + highlight.ToString().ToLower() + "}'";
//            }

//            //Active
//            if (activefilter != null)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND ";

//                whereexpression = whereexpression + "data @> '{ \"Active\" : " + activefilter.ToString().ToLower() + "}'";
//            }

//            //Activity Type
//            if (typestoexclude.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string typestring = "";
//                foreach (var typeId in typestoexclude)
//                {
//                    typestring = typestring + "NOT data @> '{ \"Type\": \"" + typeId + "\"}' AND ";
//                }
//                typestring = typestring.Remove(typestring.Length - 4);

//                whereexpression = whereexpression + typestring + ")";
//            }

//            //Activity Sub Type            
//            if (subtypestoexclude.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string subtypeliststring = "";
//                foreach (var subtype in subtypestoexclude)
//                {
//                    subtypeliststring = subtypeliststring + "NOT data @> '{ \"SubType\": \"" + subtype + "\"}' AND ";
//                }
//                subtypeliststring = subtypeliststring.Remove(subtypeliststring.Length - 4);

//                whereexpression = whereexpression + subtypeliststring + ")";
//            }

//            //Activity POI Type            
//            if (poitypestoexclude.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(whereexpression))
//                    whereexpression = whereexpression + " AND (";
//                else
//                    whereexpression = whereexpression + "(";

//                string poitypeliststring = "";
//                foreach (var poitype in poitypestoexclude)
//                {
//                    poitypeliststring = poitypeliststring + "NOT data @> '{ \"PoiType\": \"" + poitype + "\"}' AND ";
//                }
//                poitypeliststring = poitypeliststring.Remove(poitypeliststring.Length - 4);

//                whereexpression = whereexpression + poitypeliststring + ")";
//            }



//            return whereexpression;
//        }


//        #endregion

//        #region Select Expression Helpers

//        public static string CreateLocalizedAccoSelectExpression(string language)
//        {
//            string selectexpression = "Id," +
//                "data ->> 'Active' AS Active," +
//                "data ->> 'HgvId' AS HgvId," +
//                "data ->> 'Shortname' AS Shortname," +
//                "data ->> 'Units' AS Units," +
//                "data ->> 'Beds' AS Beds," +
//                "data ->> 'HasApartment' AS HasApartment," +
//                "data ->> 'HasRoom' AS HasRoom," +
//                "data ->> 'IsCamping' AS IsCamping," +
//                "data ->> 'IsGastronomy' AS IsGastronomy," +
//                "data ->> 'IsBookable' AS IsBookable," +
//                "data ->> 'IsAccommodation' AS IsAccommodation," +
//                "data ->> 'SmgActive' AS SmgActive," +
//                "data ->> 'TVMember' AS TVMember," +
//                "data ->> 'TourismVereinId' AS TourismVereinId," +
//                "data ->> 'MainLanguage' AS MainLanguage," +
//                "data ->> 'FirstImport' AS FirstImport," +
//                "data ->> 'LastChange' AS LastChange," +
//                "data ->> 'Gpstype' AS Gpstype," +
//                "data ->> 'Latitude' AS Latitude," +
//                "data ->> 'Longitude' AS Longitude," +
//                "data ->> 'Altitude' AS Altitude," +
//                "data ->> 'AltitudeUnitofMeasure' AS AltitudeUnitofMeasure," +
//                "data ->> 'AccoCategoryId' AS AccoCategoryId," +
//                "data ->> 'AccoTypeId' AS AccoTypeId," +
//                "data ->> 'DistrictId' AS DistrictId," +
//                "data -> 'BoardIds' AS BoardIds," +
//                "data -> 'MarketingGroupIds' AS MarketingGroupIds," +
//                "data -> 'Features' AS Features," +
//                "data -> 'BadgeIds' AS BadgeIds," +
//                "data -> 'ThemeIds' AS ThemeIds," +
//                "data -> 'SpecialFeaturesIds' AS SpecialFeaturesIds," +
//                "data -> 'AccoDetail' -> '" + language + "' AS AccoDetail," +
//                "data -> 'AccoBookingChannel' AS AccoBookingChannel," +
//                "data -> 'ImageGallery' AS ImageGallery," +   //TODO
//                "data -> 'LocationInfo' AS LocationInfo," +   //TODO
//                "data ->> 'GastronomyId' AS GastronomyId," +
//                "data -> 'SmgTags' AS SmgTags," +
//                "data -> 'HasLanguage' AS HasLanguage," +
//                "data -> 'MssResponseShort' AS MssResponseShort," +
//                "data ->> 'TrustYouID' AS TrustYouID," +
//                "data ->> 'TrustYouScore' AS TrustYouScore," +
//                "data ->> 'TrustYouResults' AS TrustYouResults"
//                ;

//            return selectexpression;
//        }

//        #endregion

//        #region Generic Helpers

//        public static string GetResultJson(int pagenumber, int totalpages, int totalcount, string seed, string data)
//        {
//            string resultstr = "";

//            if (data.StartsWith("["))
//                resultstr = "{" + "\"TotalResults\":" + totalcount + ",\"TotalPages\":" + totalpages + ",\"CurrentPage\":" + pagenumber + ",\"Seed\":\"" + seed + "\",\"Items\":" + data + "}";
//            else
//                resultstr = "{" + "\"TotalResults\":" + totalcount + ",\"TotalPages\":" + totalpages + ",\"CurrentPage\":" + pagenumber + ",\"Seed\":\"" + seed + "\",\"Items\":[" + data + "]}";

//            return resultstr;
//        }

//        public static string GetResultJson(int pagenumber, int totalpages, int totalcount, int onlineresults, string seed, string data)
//        {
//            string resultstr = "";

//            if (data.StartsWith("["))
//                resultstr = "{" + "\"TotalResults\":" + totalcount + ",\"TotalPages\":" + totalpages + ",\"CurrentPage\":" + pagenumber + ",\"OnlineResults\":" + onlineresults + ",\"Seed\":\"" + seed + "\",\"Items\":" + data + "}";
//            else
//                resultstr = "{" + "\"TotalResults\":" + totalcount + ",\"TotalPages\":" + totalpages + ",\"CurrentPage\":" + pagenumber + ",\"OnlineResults\":" + onlineresults + ",\"Seed\":\"" + seed + "\",\"Items\":[" + data + "]}";

//            return resultstr;
//        }

//        public static string GetResultJson(int pagenumber, int totalpages, int totalcount, int onlineresults, string resultid, string seed, string data)
//        {
//            string resultstr = "";

//            if (data.StartsWith("["))
//                resultstr = "{" + "\"TotalResults\":" + totalcount + ",\"TotalPages\":" + totalpages + ",\"CurrentPage\":" + pagenumber + ",\"OnlineResults\":" + onlineresults + ",\"ResultId\":\"" + resultid + "\",\"Seed\":\"" + seed + "\",\"Items\":" + data + "}";
//            else
//                resultstr = "{" + "\"TotalResults\":" + totalcount + ",\"TotalPages\":" + totalpages + ",\"CurrentPage\":" + pagenumber + ",\"OnlineResults\":" + onlineresults + ",\"ResultId\":\"" + resultid + "\",\"Seed\":\"" + seed + "\",\"Items\":[" + data + "]}";

//            return resultstr;
//        }

//        public static string GetResultJsonLowercase(int pagenumber, int totalpages, int totalcount, int onlineresults, string resultid, string seed, string data)
//        {
//            string resultstr = "";

//            if (data.StartsWith("["))
//                resultstr = "{" + "\"totalResults\":" + totalcount + ",\"totalPages\":" + totalpages + ",\"currentPage\":" + pagenumber + ",\"onlineResults\":" + onlineresults + ",\"resultId\":\"" + resultid + "\",\"seed\":\"" + seed + "\",\"items\":" + data + "}";
//            else
//                resultstr = "{" + "\"totalResults\":" + totalcount + ",\"totalPages\":" + totalpages + ",\"currentPage\":" + pagenumber + ",\"onlineResults\":" + onlineresults + ",\"resultId\":\"" + resultid + "\",\"seed\":\"" + seed + "\",\"items\":[" + data + "]}";

//            return resultstr;
//        }

//        //public static string GetResultJsonFromObjects<T>(int pagenumber, int totalpages, int totalcount, int onlineresults, string resultid, string seed, string data)
//        //{
//        //    string resultstr = "{" + "\"TotalResults\":" + totalcount + ",\"TotalPages\":" + totalpages + ",\"CurrentPage\":" + pagenumber + ",\"OnlineResults\":" + onlineresults + ",\"ResultId\":\"" + resultid + "\",\"Seed\":\"" + seed + "\",\"Items\":" + data + "}";

//        //    return resultstr;
//        //}

//        #endregion

//        #region Geo Helpers

//        //For Activities Pois and Smgpois

//        public static string GetGeoWhereSimple(double latitude, double longitude, int radius)
//        {
//            return "earth_distance(ll_to_earth(" + latitude.ToString(CultureInfo.InvariantCulture) + ", " + longitude.ToString(CultureInfo.InvariantCulture) + "),ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision)) < " + radius.ToString();
//        }

//        public static string GetGeoWhereSimple(string latitude, string longitude, string radius)
//        {
//            return "earth_distance(ll_to_earth(" + latitude + ", " + longitude + "),ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision)) < " + radius;
//        }

//        public static string GetGeoOrderBySimple(double latitude, double longitude)
//        {
//            return "earth_distance(ll_to_earth(" + latitude.ToString(CultureInfo.InvariantCulture) + ", " + longitude.ToString(CultureInfo.InvariantCulture) + "),ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision))";
//        }

//        public static string GetGeoOrderBySimple(string latitude, string longitude)
//        {
//            return "earth_distance(ll_to_earth(" + latitude + ", " + longitude + "),ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision))";
//        }

//        public static string GetGeoWhereExtended(double latitude, double longitude, int radius)
//        {
//            return "earth_distance(ll_to_earth(" + latitude.ToString(CultureInfo.InvariantCulture) + ", " + longitude.ToString(CultureInfo.InvariantCulture) + "),ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision)) < " + radius.ToString();
//        }

//        public static string GetGeoWhereExtended(string latitude, string longitude, string radius)
//        {
//            return "earth_distance(ll_to_earth(" + latitude + ", " + longitude + "),ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision)) < " + radius;
//        }

//        public static string GetGeoOrderByExtended(double latitude, double longitude)
//        {
//            return "earth_distance(ll_to_earth(" + latitude.ToString(CultureInfo.InvariantCulture) + ", " + longitude.ToString(CultureInfo.InvariantCulture) + "),ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision))";
//        }

//        public static string GetGeoOrderByExtended(string latitude, string longitude)
//        {
//            return "earth_distance(ll_to_earth(" + latitude + ", " + longitude + "),ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision))";
//        }

//        public static string GetGeoWhereBoundingBoxes(string latitude, string longitude, string radius)
//        {
//            return "earth_box(ll_to_earth(" + latitude + ", " + longitude + "), " + radius + ") @> ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision) and earth_distance(ll_to_earth(" + latitude + ", " + longitude + "), ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision)) < " + radius;
//        }

//        public static string GetGeoWhereBoundingBoxes(double latitude, double longitude, int radius)
//        {
//            return "earth_box(ll_to_earth(" + latitude.ToString(CultureInfo.InvariantCulture) + ", " + longitude.ToString(CultureInfo.InvariantCulture) + "), " + radius.ToString() + ") @> ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision) and earth_distance(ll_to_earth(" + latitude.ToString(CultureInfo.InvariantCulture) + ", " + longitude.ToString(CultureInfo.InvariantCulture) + "), ll_to_earth((data->>'Latitude')::double precision, (data->>'Longitude')::double precision)) < " + radius.ToString();
//        }

//        public static string GetGeoWhereBoundingBoxesExtended(string latitude, string longitude, string radius)
//        {
//            return "earth_box(ll_to_earth(" + latitude + ", " + longitude + "), " + radius + ") @> ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision) and earth_distance(ll_to_earth(" + latitude + ", " + longitude + "), ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision)) < " + radius;
//        }

//        public static string GetGeoWhereBoundingBoxesExtended(double latitude, double longitude, int radius)
//        {
//            return "earth_box(ll_to_earth(" + latitude.ToString(CultureInfo.InvariantCulture) + ", " + longitude.ToString(CultureInfo.InvariantCulture) + "), " + radius.ToString() + ") @> ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision) and earth_distance(ll_to_earth(" + latitude.ToString(CultureInfo.InvariantCulture) + ", " + longitude.ToString(CultureInfo.InvariantCulture) + "), ll_to_earth((data->'GpsPoints'->'position'->>'Latitude')::double precision, (data->'GpsPoints'->'position'->>'Longitude')::double precision)) < " + radius.ToString();
//        }

//        //For Accommodations
//        public static void ApplyGeoSearchWhereOrderbySimple(ref string where, ref string orderby, PGGeoSearchResult geosearchresult)
//        {
//            if (geosearchresult != null)
//            {
//                if (geosearchresult.geosearch)
//                {
//                    if (!String.IsNullOrEmpty(where))
//                        where = where + " AND ";

//                    where = where + PostgresSQLHelper.GetGeoWhereSimple(geosearchresult.latitude, geosearchresult.longitude, geosearchresult.radius);
//                    orderby = PostgresSQLHelper.GetGeoOrderBySimple(geosearchresult.latitude, geosearchresult.longitude);
//                }
//            }
//        }

//        //For Activities Pois and GBActivityPoi
//        public static void ApplyGeoSearchWhereOrderby(ref string where, ref string orderby, PGGeoSearchResult geosearchresult)
//        {
//            if (geosearchresult != null)
//            {
//                if (geosearchresult.geosearch)
//                {
//                    if (!String.IsNullOrEmpty(where))
//                        where = where + " AND ";

//                    where = where + PostgresSQLHelper.GetGeoWhereExtended(geosearchresult.latitude, geosearchresult.longitude, geosearchresult.radius);
//                    orderby = PostgresSQLHelper.GetGeoOrderByExtended(geosearchresult.latitude, geosearchresult.longitude);
//                }
//            }
//        }

//        #endregion
//    }
//}
