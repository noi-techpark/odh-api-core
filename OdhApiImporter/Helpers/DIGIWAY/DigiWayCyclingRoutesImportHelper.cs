// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using DataModel;
using DIGIWAY;
using Helper;
using Helper.Generic;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using SqlKata;
using SqlKata.Execution;
using SqlKata.Extensions;

namespace OdhApiImporter.Helpers
{
    public class DigiWayCyclingRoutesImportHelper : ImportHelper, IImportHelper
    {
        public List<string> idlistinterface { get; set; }

        public DigiWayCyclingRoutesImportHelper(
            ISettings settings,
            QueryFactory queryfactory,
            string table,
            string importerURL
        )
            : base(settings, queryfactory, table, importerURL)
        {
            idlistinterface = new List<string>();
        }

        public async Task<UpdateDetail> SaveDataToODH(
            DateTime? lastchanged = null,
            List<string>? idlist = null,
            CancellationToken cancellationToken = default
        )
        {
            var data = await GetData(cancellationToken);

            ////UPDATE all data
            var updateresult = await ImportData(data, cancellationToken);

            //Disable Data not in list
            var deleteresult = await SetDataNotinListToInactive(cancellationToken);

            return GenericResultsHelper.MergeUpdateDetail(
                new List<UpdateDetail>() { updateresult, deleteresult }
            );
        }

        //Get Data from Source
        private async Task<DigiWayRoutesCycleWaysResult> GetData(CancellationToken cancellationToken)
        {
            return await GetDigiwayData.GetDigiWayCyclingRouteDataAsync("", "", settings.DigiWayConfig["CyclingRoutes"].ServiceUrl);
        }

        //Import the Data
        public async Task<UpdateDetail> ImportData(
            DigiWayRoutesCycleWaysResult digiwaydatalist,
            CancellationToken cancellationToken
        )
        {
            int updatecounter = 0;
            int newcounter = 0;
            int deletecounter = 0;
            int errorcounter = 0;

            if (
                digiwaydatalist != null
                && digiwaydatalist.features != null
            )
            {
                //loop trough items
                foreach (
                    var digiwaydata in digiwaydatalist.features
                )
                {                    
                        var importresult = await ImportDataSingle(digiwaydata);

                        newcounter = newcounter + importresult.created ?? newcounter;
                        updatecounter = updatecounter + importresult.updated ?? updatecounter;
                        errorcounter = errorcounter + importresult.error ?? errorcounter;                    
                }
            }

            return new UpdateDetail()
            {
                created = newcounter,
                updated = updatecounter,
                deleted = deletecounter,
                error = errorcounter,
            };
        }

        //Parsing the Data
        public async Task<UpdateDetail> ImportDataSingle(DigiWayRoutesCycleWays digiwaydata)
        {
            int updatecounter = 0;
            int newcounter = 0;
            int deletecounter = 0;
            int errorcounter = 0;

            //id
            string returnid = "";

            try
            {
                returnid = digiwaydata.id;

                idlistinterface.Add(returnid);

                //Parse  Data
                var parsedobject = await ParseDigiWayDataToODHActivityPoi(
                    returnid, 
                    digiwaydata
                );
                if (parsedobject.Item1 == null || parsedobject.Item2 == null)
                    throw new Exception();

                var pgcrudshaperesult = await InsertDataInShapesDB(parsedobject.Item2);

                //Save parsedobject to DB + Save Rawdata to DB
                var pgcrudresult = await InsertDataToDB(
                    parsedobject.Item1,
                    new KeyValuePair<string, DigiWayRoutesCycleWays>(returnid, digiwaydata)
                );

                newcounter = newcounter + pgcrudresult.created ?? 0;
                updatecounter = updatecounter + pgcrudresult.updated ?? 0;

                WriteLog.LogToConsole(
                    returnid,
                    "dataimport",
                    "single.digiway",
                    new ImportLog()
                    {
                        sourceid = returnid,
                        sourceinterface = "digiway.cyclingroutes",
                        success = true,
                        error = "",
                    }
                );
            }
            catch (Exception ex)
            {
                WriteLog.LogToConsole(
                    returnid,
                    "dataimport",
                    "single.digiway",
                    new ImportLog()
                    {
                        sourceid = returnid,
                        sourceinterface = "digiway.cyclingroutes",
                        success = false,
                        error = "digiway cyclingroute could not be parsed",
                    }
                );

                errorcounter = errorcounter + 1;
            }

            return new UpdateDetail()
            {
                created = newcounter,
                updated = updatecounter,
                deleted = 0,
                error = errorcounter,
            };
        }

        //Inserting into DB
        private async Task<PGCRUDResult> InsertDataToDB(
            ODHActivityPoiLinked data,
            KeyValuePair<string, DigiWayRoutesCycleWays> digiwaydata
        )
        {
            var rawdataid = await InsertInRawDataDB(digiwaydata);

            data.Id = data.Id?.ToLower();

            //Set LicenseInfo
            data.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<ODHActivityPoiLinked>(
                data,
                Helper.LicenseHelper.GetLicenseforOdhActivityPoi
            );

            //PublishedOnInfo?

            var pgcrudresult = await QueryFactory.UpsertData<ODHActivityPoiLinked>(
                data,
                new DataInfo(table, Helper.Generic.CRUDOperation.CreateAndUpdate),
                new EditInfo("digiway.cyclingroutes.import", importerURL),
                new CRUDConstraints(),
                new CompareConfig(true, false),
                rawdataid
            );

            return pgcrudresult;
        }

        private async Task<PGCRUDResult> InsertDataInShapesDB(
          GeoShapeJson data
      )
        {
            try
            {
                //var geomfactory = new GeometryFactory();

                //List<Coordinate> coordinates = new List<Coordinate>();
                //coordinates.Add(new Coordinate(754907.9859, 5266143.9387));
                //coordinates.Add(new Coordinate(754907.1739, 5266138.4547));
                //coordinates.Add(new Coordinate(754905.7391, 5266131.014));
                //coordinates.Add(new Coordinate(754905.3508, 5266129.0538));
                //coordinates.Add(new Coordinate(754903.9335, 5266121.9573));
                //coordinates.Add(new Coordinate(754897.8422, 5266096.307));
                //coordinates.Add(new Coordinate(754889.3344, 5266067.5832));
                //coordinates.Add(new Coordinate(754874.9674, 5266026.0523));
                //coordinates.Add(new Coordinate(754866.9682, 5265998.65));
                //coordinates.Add(new Coordinate(754860.2605, 5265973.3955));


                //var linestring = geomfactory.WithSRID(32632).CreateLineString(coordinates.ToArray());

                ////schneller hack
                //if (data == null)
                //{
                //    data = new GeoShapeJson()
                //    {
                //        LicenseInfo = new LicenseInfo() { License = "open", Author = "", ClosedData = false, LicenseHolder = "" },
                //        Name = "test",
                //        Shape_area = 0,
                //        Shape_length = 0,
                //        Type = "Cyclingroute",
                //        Source = "digiway",
                //        Geometry = linestring
                //    };
                //}


                //Set LicenseInfo
                data.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<GeoShapeJson>(
                    data,
                    Helper.LicenseHelper.GetLicenseforGeoShape
                );

                //Set Meta
                data._Meta = MetadataHelper.GetMetadataobject<GeoShapeJson>(data);

                //Check if data is there
                var shape = await QueryFactory.Query("shapestest").Select("id").Where("id", data.Id).FirstOrDefaultAsync<int>();

                int operationid = 0;

                PGCRUDResult result = default(PGCRUDResult);
                if (shape == 0)
                {                    
                    //var geomfactory = new GeometryFactory();
                    //var linestring = geomfactory.WithSRID(32632).CreateLineString(data.Geometry.Coordinates);
                    var maxid = await QueryFactory
                    .Query("shapestest").SelectMax("id").GetAsync<int?>();

                    int idtopass = maxid != null && maxid.FirstOrDefault() != null && maxid.FirstOrDefault().HasValue ? maxid.FirstOrDefault().Value + 1 : 1;

                    var insert = await QueryFactory
                   .Query("shapestest").InsertAsync(new GeoShapeDBTest<UnsafeLiteral>()
                   {
                       id = idtopass,
                       licenseinfo = new JsonRaw(data.LicenseInfo),
                       meta = new JsonRaw(data._Meta),
                       name = data.Name,
                       shape_area = data.Shape_area != null ? data.Shape_area.Value : 0,
                       shape_leng = data.Shape_length != null ? data.Shape_length.Value : 0,
                       type = data.Type,
                       source = "digiway",
                       s7rid = "32632",
                       //geom = new PGGeometryRaw("ST_GeometryFromText('" + data.Geometry + "', 32632)"),
                       //geom = "ST_GeometryFromText('" + data.Geometry + "', 32632)",
                       //geometry = new PGLineStringRaw(linestring)
                       //geometry = "ST_GeometryFromText('" + linestring.ToString() + "', 32632)"
                       geometry = new UnsafeLiteral("ST_GeometryFromText('" + data.Geometry.ToString() + "', 32632)", false)
                   });


                }
                else
                {

                }

                return new PGCRUDResult()
                {
                    id = operationid.ToString(),
                    odhtype = data._Meta.Type,
                    created = 0,
                    updated = 0,
                    deleted = 0,
                    error = 0,
                    errorreason = null,
                    operation = "insert shape",
                    changes = null,
                    compareobject = false,
                    objectchanged = 0,
                    objectimagechanged = 0,
                    pushchannels = null,
                };
            }
            catch (Exception ex)
            {
                return new PGCRUDResult()
                {
                    id = "",
                    odhtype = data._Meta.Type,
                    created = 0,
                    updated = 0,
                    deleted = 0,
                    error = 1,
                    errorreason = ex.Message,
                    operation = "insert shape",
                    changes = null,
                    compareobject = false,
                    objectchanged = 0,
                    objectimagechanged = 0,
                    pushchannels = null,
                };
            }
        }

        private async Task<int> InsertInRawDataDB(KeyValuePair<string, DigiWayRoutesCycleWays> data)
        {
            return await QueryFactory.InsertInRawtableAndGetIdAsync(
                new RawDataStore()
                {
                    datasource = "digiway",
                    rawformat = "xml",
                    importdate = DateTime.Now,
                    license = "open",
                    sourceinterface = "cyclingroutes",
                    sourceurl = settings.DigiWayConfig["CyclingRoutes"].ServiceUrl,
                    type = "odhactivitypoi",
                    sourceid = data.Key,
                    raw = data.Value.ToString(),
                }
            );
        }

        //Parse the interface content
        public async Task<(ODHActivityPoiLinked?, GeoShapeJson?)> ParseDigiWayDataToODHActivityPoi(
            string odhid,
            DigiWayRoutesCycleWays input
        )
        {
            //Get the ODH Item
            var query = QueryFactory.Query(table).Select("data").Where("id", odhid);

            var dataindb = await query.GetObjectSingleAsync<ODHActivityPoiLinked>();

            var result = ParseCyclingRoutesToODHActivityPoi.ParseDigiWayCyclingRoutesToODHActivityPoi(dataindb, input);

            return result;
        }

        //Deactivates all data that is no more on the interface
        private async Task<UpdateDetail> SetDataNotinListToInactive(
            CancellationToken cancellationToken
        )
        {
            int updateresult = 0;
            int deleteresult = 0;
            int errorresult = 0;

            try
            {
                //Begin SetDataNotinListToInactive
                var idlistdb = await GetAllDataBySource(new List<string>() { "digiway" });

                var idstodelete = idlistdb.Where(p => !idlistinterface.Any(p2 => p2 == p));

                foreach (var idtodelete in idstodelete)
                {
                    var result = await DeleteOrDisableData<ODHActivityPoiLinked>(idtodelete, false);

                    updateresult = updateresult + result.Item1;
                    deleteresult = deleteresult + result.Item2;
                }
            }
            catch (Exception ex)
            {
                WriteLog.LogToConsole(
                    "",
                    "dataimport",
                    "deactivate.digiway",
                    new ImportLog()
                    {
                        sourceid = "",
                        sourceinterface = "digiway.cyclingroutes",
                        success = false,
                        error = ex.Message,
                    }
                );

                errorresult = errorresult + 1;
            }

            return new UpdateDetail()
            {
                created = 0,
                updated = updateresult,
                deleted = deleteresult,
                error = errorresult,
            };
        }
    }
}
