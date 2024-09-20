// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper;
using Newtonsoft.Json;
using NINJA;
using NINJA.Parser;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiImporter.Helpers
{
    public class MobilityEventsV2ImportHelper : ImportHelper, IImportHelper
    {       
        public MobilityEventsV2ImportHelper(ISettings settings, QueryFactory queryfactory, string table, string importerURL) : base(settings, queryfactory, table, importerURL)
        {

        }


        #region NINJA Helpers

        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, List<string>? idlist = null, CancellationToken cancellationToken = default)
        {
            //Import the data from Mobility Api
            var culturelist = await ImportList(cancellationToken);
            //Parse the data and save it to DB
            var result = await SaveEventsToPG(culturelist.Item1.data, culturelist.Item2.data);

            return result;
        }

        private async Task<Tuple<NinjaObject<NinjaEvent>,NinjaObject<NinjaPlaceRoom>>> ImportList(CancellationToken cancellationToken)
        {
            //TODO SPLIT THIS TO  VENUE + Event import
            //Check gps points?

            var responseevents = await GetNinjaData.GetNinjaEvent(settings.NinjaConfig.ServiceUrl);
            var responseplaces = await GetNinjaData.GetNinjaPlaces(settings.NinjaConfig.ServiceUrl);

            WriteLog.LogToConsole("", "dataimport", "list.mobilityculture.v2", new ImportLog() { sourceid = "", sourceinterface = "mobility.culture.v2", success = true, error = "" });

            return Tuple.Create(responseevents, responseplaces);
        }

        private async Task<UpdateDetail> SaveEventsToPG(ICollection<NinjaData<NinjaEvent>> ninjadataarr, ICollection<NinjaData<NinjaPlaceRoom>> ninjaplaceroomarr)
        {

            var newimportcounter = 0;
            var updateimportcounter = 0;
            var errorimportcounter = 0;
            var deleteimportcounter = 0;            

            List<string> idlistspreadsheet = new List<string>();
            List<string> sourcelist = new List<string>();
            sourcelist.Add("drin");
            sourcelist.Add("trevilab");

            List<string> idlistvenuespreadsheet = new List<string>();
            List<string> sourcelistvenue = new List<string>();
            sourcelistvenue.Add("drin");
            sourcelistvenue.Add("trevilab");

            foreach (var ninjadata in ninjadataarr.Select(x => x.tmetadata))
            {
                foreach (KeyValuePair<string, NinjaEvent> kvp in ninjadata)
                {
                    if (!String.IsNullOrEmpty(kvp.Key))
                    {
                        var place = ninjaplaceroomarr.Where(x => x.sname == kvp.Value.place).FirstOrDefault();
                        var room = ninjaplaceroomarr.Where(x => x.sname == kvp.Value.room).FirstOrDefault();

                        var venuetosave = ParseNinjaData.ParseNinjaEventPlaceToVenueV2(place.scode, place);

                        //Save only once
                        if (venuetosave != null && !idlistvenuespreadsheet.Contains(venuetosave.Id))
                        {
                            //Setting Location Info
                            //Location Info (by GPS Point)
                            //if (eventtosave.Latitude != 0 && eventtosave.Longitude != 0)
                            //{
                            //    await SetLocationInfo(eventtosave);
                            //}

                            venuetosave.Active = true;
          
                            //Insert Event
                            var result = await InsertDataToDB(venuetosave, place);

                            newimportcounter = newimportcounter + result.created ?? 0;
                            updateimportcounter = updateimportcounter + result.updated ?? 0;
                            errorimportcounter = errorimportcounter + result.error ?? 0;

                            idlistvenuespreadsheet.Add(venuetosave.Id);

                            if (!sourcelist.Contains(venuetosave.Source))
                                sourcelist.Add(venuetosave.Source);

                            WriteLog.LogToConsole(venuetosave.Id, "dataimport", "single.mobilityculture", new ImportLog() { sourceid = venuetosave.Id, sourceinterface = "mobility.culture.venue", success = true, error = "" });
                        }
                        else
                        {
                            WriteLog.LogToConsole(venuetosave.Id, "dataimport", "single.mobilityculture", new ImportLog() { sourceid = kvp.Key, sourceinterface = "mobility.culture.venue", success = false, error = "Venue could not be parsed" });
                        }

                        var roomtosave = ParseNinjaData.ParseNinjaEventRoomToVenueV2(room.scode, room, venuetosave.Id);

                        //Save only once
                        if (roomtosave != null && !idlistvenuespreadsheet.Contains(roomtosave.Id))
                        {
                            //Setting Location Info
                            //Location Info (by GPS Point)
                            //if (eventtosave.Latitude != 0 && eventtosave.Longitude != 0)
                            //{
                            //    await SetLocationInfo(eventtosave);
                            //}

                            roomtosave.Active = true;

                            //Insert Event
                            var result = await InsertDataToDB(roomtosave, place);

                            newimportcounter = newimportcounter + result.created ?? 0;
                            updateimportcounter = updateimportcounter + result.updated ?? 0;
                            errorimportcounter = errorimportcounter + result.error ?? 0;

                            idlistvenuespreadsheet.Add(roomtosave.Id);

                            if (!sourcelist.Contains(roomtosave.Source))
                                sourcelist.Add(roomtosave.Source);

                            WriteLog.LogToConsole(roomtosave.Id, "dataimport", "single.mobilityculture.venue", new ImportLog() { sourceid = roomtosave.Id, sourceinterface = "mobility.culture", success = true, error = "" });
                        }
                        else
                        {
                            WriteLog.LogToConsole(roomtosave.Id, "dataimport", "single.mobilityculture.venue", new ImportLog() { sourceid = kvp.Key, sourceinterface = "mobility.culture", success = false, error = "Venue could not be parsed" });
                        }


                        var eventtosave = ParseNinjaData.ParseNinjaEventToODHEventV2(kvp.Key, kvp.Value, roomtosave.Id);

                        if(eventtosave != null)
                        {
                            //Setting Location Info
                            //Location Info (by GPS Point)
                            //if (eventtosave.Latitude != 0 && eventtosave.Longitude != 0)
                            //{
                            //    await SetLocationInfo(eventtosave);
                            //}

                            eventtosave.Active = true;
                            
                            var idtocheck = kvp.Key;

                            if (idtocheck.Length > 50)
                                idtocheck = idtocheck.Substring(0, 50);

                            //Insert Venues


                            //Insert Event
                            var result = await InsertDataToDB(eventtosave, kvp);

                            newimportcounter = newimportcounter + result.created ?? 0;
                            updateimportcounter = updateimportcounter + result.updated ?? 0;
                            errorimportcounter = errorimportcounter + result.error ?? 0;

                            idlistspreadsheet.Add(idtocheck.ToUpper());

                            if (!sourcelist.Contains(eventtosave.Source))
                                sourcelist.Add(eventtosave.Source);

                            WriteLog.LogToConsole(idtocheck.ToUpper(), "dataimport", "single.mobilityculture.event", new ImportLog() { sourceid = idtocheck.ToUpper(), sourceinterface = "mobility.culture", success = true, error = "" });
                        }                        
                        else
                        {
                            WriteLog.LogToConsole(kvp.Key, "dataimport", "single.mobilityculture.event", new ImportLog() { sourceid = kvp.Key, sourceinterface = "mobility.culture", success = false, error = "Event could not be parsed" });
                        }
                    }
                }               
            }

            //Begin SetDataNotinListToInactive
            var idlistdb = await GetAllEventsBySource(sourcelist);

            var idstodelete = idlistdb.Where(p => !idlistspreadsheet.Any(p2 => p2 == p));

            foreach (var idtodelete in idstodelete)
            {
                var deletedisableresult = await DeleteOrDisableData(idtodelete, false);

                if(deletedisableresult.Item1 > 0)
                    WriteLog.LogToConsole(idtodelete, "dataimport", "single.mobilityculture.event.deactivate", new ImportLog() { sourceid = idtodelete, sourceinterface = "mobility.culture", success = true, error = "" });
                else if (deletedisableresult.Item2 > 0)
                    WriteLog.LogToConsole(idtodelete, "dataimport", "single.mobilityculture.event.delete", new ImportLog() { sourceid = idtodelete, sourceinterface = "mobility.culture", success = true, error = "" });


                deleteimportcounter = deleteimportcounter + deletedisableresult.Item1 + deletedisableresult.Item2;
            }

            //Begin SetDataNotinListToInactive
            var idlistvenuedb = await GetAllVenuesBySource(sourcelistvenue);

            var idstodeletevenue = idlistvenuedb.Where(p => !idlistvenuespreadsheet.Any(p2 => p2 == p));

            foreach (var idtodelete in idstodeletevenue)
            {
                var deletedisableresult = await DeleteOrDisableData(idtodelete, false);

                if (deletedisableresult.Item1 > 0)
                    WriteLog.LogToConsole(idtodelete, "dataimport", "single.mobilityculture.venue.deactivate", new ImportLog() { sourceid = idtodelete, sourceinterface = "mobility.culture", success = true, error = "" });
                else if (deletedisableresult.Item2 > 0)
                    WriteLog.LogToConsole(idtodelete, "dataimport", "single.mobilityculture.venue.delete", new ImportLog() { sourceid = idtodelete, sourceinterface = "mobility.culture", success = true, error = "" });


                deleteimportcounter = deleteimportcounter + deletedisableresult.Item1 + deletedisableresult.Item2;
            }

            return new UpdateDetail() { updated = updateimportcounter, created = newimportcounter, deleted = deleteimportcounter, error = errorimportcounter };
        }

        private async Task<PGCRUDResult> InsertDataToDB(EventV2 eventtosave, KeyValuePair<string, NinjaEvent> ninjaevent)
        {
            try
            {
                eventtosave.Id = eventtosave.Id?.ToUpper();

                //Set LicenseInfo
                eventtosave.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject(eventtosave, Helper.LicenseHelper.GetLicenseforEvent);

                //Setting MetaInfo (we need the MetaData Object in the PublishedOnList Creator)
                eventtosave._Meta = MetadataHelper.GetMetadataobject(eventtosave);

                //Set PublishedOn
                eventtosave.CreatePublishedOnList();

                var rawdataid = await InsertInRawDataDB(ninjaevent);

                return await QueryFactory.UpsertData<EventV2>(eventtosave, "eventsv2", rawdataid, "mobility.eventv2.import", importerURL);                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<PGCRUDResult> InsertDataToDB(VenueV2 venuetosave, NinjaData<NinjaPlaceRoom> ninjavenue)
        {
            try
            {
                venuetosave.Id = venuetosave.Id?.ToUpper();

                //Set LicenseInfo
                venuetosave.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject(venuetosave, Helper.LicenseHelper.GetLicenseforVenue);

                //Setting MetaInfo (we need the MetaData Object in the PublishedOnList Creator)
                venuetosave._Meta = MetadataHelper.GetMetadataobject(venuetosave);

                //Set PublishedOn
                venuetosave.CreatePublishedOnList();

                var rawdataid = await InsertInRawDataDB(ninjavenue);

                return await QueryFactory.UpsertData<VenueV2>(venuetosave, "venuesv2", rawdataid, "mobility.venuev2.import", importerURL);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        private async Task<int> InsertInRawDataDB(KeyValuePair<string, NinjaEvent> ninjaevent)
        {
            return await QueryFactory.InsertInRawtableAndGetIdAsync(
                        new RawDataStore()
                        {
                            datasource = "centrotrevi-drin",
                            importdate = DateTime.Now,
                            raw = JsonConvert.SerializeObject(ninjaevent.Value),
                            sourceinterface = "culture",
                            sourceid = ninjaevent.Key,
                            sourceurl = "https://mobility.api.opendatahub.com/v2/flat/Culture/",
                            type = "event",
                            license = "open",
                            rawformat = "json"
                        });
        }

        private async Task<int> InsertInRawDataDB(NinjaData<NinjaPlaceRoom> ninjaevent)
        {
            return await QueryFactory.InsertInRawtableAndGetIdAsync(
                        new RawDataStore()
                        {
                            datasource = "centrotrevi-drin",
                            importdate = DateTime.Now,
                            raw = JsonConvert.SerializeObject(ninjaevent),
                            sourceinterface = "culture",
                            sourceid = ninjaevent.scode,
                            sourceurl = "https://mobility.api.opendatahub.com/v2/flat/Culture/",
                            type = "venue",
                            license = "open",
                            rawformat = "json"
                        });
        }

        private async Task<Tuple<int,int>> DeleteOrDisableData(string eventid, bool delete)
        {
            var deleteresult = 0;
            var updateresult = 0;

            if (delete)
            {
                deleteresult = await QueryFactory.Query("eventsv2").Where("id", eventid)
                    .DeleteAsync();
            }
            else
            {
                var query =
               QueryFactory.Query("eventsv2")
                   .Select("data")
                   .Where("id", eventid);

                var data = await query.GetObjectSingleAsync<EventV2>();

                if (data != null)                
                {
                    if (data.Active != false)
                    {
                        data.Active = false;

                        //Publishedon? no push needed here
                     
                        updateresult = await QueryFactory.Query("events").Where("id", eventid)
                                        .UpdateAsync(new JsonBData() { id = eventid, data = new JsonRaw(data) });                        
                    }
                }
            }

            return Tuple.Create(updateresult, deleteresult);
        }

        private async Task<Tuple<int, int>> DeleteOrDisableVenueData(string venueid, bool delete)
        {
            var deleteresult = 0;
            var updateresult = 0;

            if (delete)
            {
                deleteresult = await QueryFactory.Query("venuesv2").Where("id", venueid)
                    .DeleteAsync();
            }
            else
            {
                var query =
               QueryFactory.Query("venuesv2")
                   .Select("data")
                   .Where("id", venueid);

                var data = await query.GetObjectSingleAsync<VenueV2>();

                if (data != null)
                {
                    if (data.Active != false)
                    {
                        data.Active = false;

                        //Publishedon? no push needed here

                        updateresult = await QueryFactory.Query("events").Where("id", venueid)
                                        .UpdateAsync(new JsonBData() { id = venueid, data = new JsonRaw(data) });
                    }
                }
            }

            return Tuple.Create(updateresult, deleteresult);
        }


        #endregion

        #region CUSTOM Ninja Import

        private async Task<List<string>> GetAllEventsBySource(List<string> sourcelist)
        {

            var query =
               QueryFactory.Query("eventsv2")
                   .Select("id")
                   .SourceFilter_GeneratedColumn(sourcelist);

            var eventids = await query.GetAsync<string>();

            return eventids.ToList();
        }

        private async Task<List<string>> GetAllVenuesBySource(List<string> sourcelist)
        {

            var query =
               QueryFactory.Query("venuesv2")
                   .Select("id")
                   .SourceFilter_GeneratedColumn(sourcelist);

            var venueids = await query.GetAsync<string>();

            return venueids.ToList();
        }


        private async Task SetLocationInfo(EventLinked myevent)
        {
            var district = await GetLocationInfo.GetNearestDistrictbyGPS(QueryFactory, myevent.Latitude, myevent.Longitude, 30000);

            if (district == null)
                return;

            myevent.DistrictId = district.Id;
            myevent.DistrictIds = new List<string>() { district.Id };

            var locinfo = await GetLocationInfo.GetTheLocationInfoDistrict(QueryFactory, district.Id);
            if (locinfo != null)
            {
                LocationInfoLinked locinfolinked = new LocationInfoLinked
                {
                    DistrictInfo = new DistrictInfoLinked
                    {
                        Id = locinfo.DistrictInfo?.Id,
                        Name = locinfo.DistrictInfo?.Name
                    },
                    MunicipalityInfo = new MunicipalityInfoLinked
                    {
                        Id = locinfo.MunicipalityInfo?.Id,
                        Name = locinfo.MunicipalityInfo?.Name
                    },
                    TvInfo = new TvInfoLinked
                    {
                        Id = locinfo.TvInfo?.Id,
                        Name = locinfo.TvInfo?.Name
                    },
                    RegionInfo = new RegionInfoLinked
                    {
                        Id = locinfo.RegionInfo?.Id,
                        Name = locinfo.RegionInfo?.Name
                    }
                };

                myevent.LocationInfo = locinfolinked;
            }
        }

        #endregion

    }
}
