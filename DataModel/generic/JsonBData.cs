// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
    public class JsonBData
    {
        public string? id { get; set; }
        public JsonRaw? data { get; set; }
    }

    public class JsonBDataDestinationData
    {
        public string? id { get; set; }
        public JsonRaw? data { get; set; }
        public JsonRaw? destinationdata { get; set; }
    }

    public class JsonBDataRaw
    {
        public string id { get; set; }
        public JsonRaw data { get; set; }
        public Int32 rawdataid { get; set; }
    }

    #region RawDataStore
    public interface IRawDataStore
    {
        string type { get; set; }
        string datasource { get; set; }
        string sourceinterface { get; set; }
        string sourceid { get; set; }
        string sourceurl { get; set; }
        DateTime importdate { get; set; }
        string license { get; set; }
        string rawformat { get; set; }
    }

    public class RawDataStore : IRawDataStore
    {
        public string type { get; set; }
        public string datasource { get; set; }
        public string sourceinterface { get; set; }
        public string sourceid { get; set; }
        public string sourceurl { get; set; }
        public DateTime importdate { get; set; }

        public string license { get; set; }
        public string rawformat { get; set; }

        public string raw { get; set; }
    }

    public class RawDataStoreWithId : RawDataStore
    {
        public int? id { get; set; }
    }

    public class RawDataStoreJson : RawDataStoreWithId
    {
        public RawDataStoreJson(RawDataStoreWithId rawdatastore)
        {
            this.id = rawdatastore.id;
            this.importdate = rawdatastore.importdate;
            this.datasource = rawdatastore.datasource;
            this.rawformat = rawdatastore.rawformat;
            this.sourceid = rawdatastore.sourceid;
            this.sourceinterface = rawdatastore.sourceinterface;
            this.sourceurl = rawdatastore.sourceurl;
            this.type = rawdatastore.type;
            this.license = rawdatastore.license;

            this.raw = new JsonRaw(rawdatastore.raw);
        }

        public new JsonRaw raw { get; set; }
    }

    public static class RawDataStoreExtensions
    {
        public static IRawDataStore UseJsonRaw(this RawDataStoreWithId rawdatastore)
        {
            if (rawdatastore.rawformat == "json")
            {
                return new RawDataStoreJson(rawdatastore);
            }
            else
                return rawdatastore;
        }
    }

    #endregion

    #region RawChangesStore

    public interface IRawChangesStore
    {
        string type { get; set; }
        string datasource { get; set; }
        string sourceid { get; set; }
        string editedby { get; set; }
        string editsource { get; set; }
        DateTime date { get; set; }
        JsonRaw changes { get; set; }
        string license { get; set; }
    }

    public class RawChangesStore : IRawChangesStore
    {
        public string type { get; set; }
        public string datasource { get; set; }
        public string sourceid { get; set; }
        public string editedby { get; set; }
        public string editsource { get; set; }
        public DateTime date { get; set; }
        public JsonRaw changes { get; set; }
        public string license { get; set; }
    }

    public class RawChangesStoreWithId : RawChangesStore
    {
        public int? id { get; set; }
    }


    #endregion
}
