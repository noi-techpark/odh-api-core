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
    public class NINJAImportHelper
    {
        private readonly QueryFactory QueryFactory;
        private readonly ISettings settings;

        public NINJAImportHelper(ISettings settings, QueryFactory queryfactory)
        {
            this.QueryFactory = queryfactory;
            this.settings = settings;
        }

        #region NINJA Helpers

        //TODO CHECK IF THIS IS WORKING AND REFACTOR

        /// <summary>
        /// Save Events to Postgres
        /// </summary>
        /// <param name="ninjadataarr"></param>
        public async Task<UpdateDetail> SaveEventsToPG(ICollection<NinjaData<NinjaEvent>> ninjadataarr, ICollection<NinjaData<NinjaPlaceRoom>> ninjaplaceroomarr)
        {

            var newimportcounter = 0;
            var updateimportcounter = 0;
            var errorimportcounter = 0;
            var deleteimportcounter = 0;

            List<string> idlistspreadsheet = new List<string>();
            List<string> sourcelist = new List<string>();

            foreach (var ninjadata in ninjadataarr.Select(x => x.tmetadata))
            {
                foreach (KeyValuePair<string, NinjaEvent> kvp in ninjadata)
                {
                    if (!String.IsNullOrEmpty(kvp.Key))
                    {
                        var place = ninjaplaceroomarr.Where(x => x.sname == kvp.Value.place).FirstOrDefault();
                        var room = ninjaplaceroomarr.Where(x => x.sname == kvp.Value.room).FirstOrDefault();

                        var eventtosave = ParseNinjaData.ParseNinjaEventToODHEvent(kvp.Key, kvp.Value, place, room);

                        //Setting Location Info
                        //Location Info (by GPS Point)
                        if (eventtosave.Latitude != 0 && eventtosave.Longitude != 0)
                        {
                            await SetLocationInfo(eventtosave);
                        }

                        eventtosave.Active = true;
                        eventtosave.SmgActive = false;

                        var idtocheck = kvp.Key;

                        if (idtocheck.Length > 50)
                            idtocheck = idtocheck.Substring(0, 50);

                        var result = await InsertEventInPG(eventtosave, idtocheck, kvp);

                        if (result.Item1 == "insert")
                        {
                            if (result.Item2 == "1")
                                newimportcounter++;
                        }
                        else if (result.Item1 == "update")
                        {
                            if (result.Item2 == "1")
                                updateimportcounter++;
                        }
                        else
                        {
                            errorimportcounter++;
                        }

                        idlistspreadsheet.Add(idtocheck.ToUpper());
                        if (!sourcelist.Contains(eventtosave.Source))
                            sourcelist.Add(eventtosave.Source);
                    }
                }

                //TODO get all IDs in DB
                var idlistdb = await GetAllEventsBySource(sourcelist);

                var idstodelete = idlistdb.Where(p => !idlistspreadsheet.Any(p2 => p2 == p));

                foreach (var idtodelete in idstodelete)
                {
                    deleteimportcounter = deleteimportcounter + await DeleteOrDisableEvents(idtodelete, false);
                }
            }

            return new UpdateDetail() { updated = updateimportcounter, created = newimportcounter, deleted = deleteimportcounter };
        }

        private async Task SetLocationInfo(EventLinked myevent)
        {
            string wheregeo = PostgresSQLHelper.GetGeoWhereSimple(myevent.Latitude, myevent.Longitude, 30000);
            string orderbygeo = PostgresSQLHelper.GetGeoOrderBySimple(myevent.Latitude, myevent.Longitude);

            var query =
                     QueryFactory.Query("districts")
                         .Select("data")
                         .WhereRaw(wheregeo)
                         .OrderByRaw(orderbygeo);

            var districtlist = await query.GetFirstOrDefaultAsObject<District>();

            if (districtlist == null)
                return;

            myevent.DistrictId = districtlist.Id;
            myevent.DistrictIds = new List<string>() { districtlist.Id };
            
            var locinfo = await GetTheLocationInfoDistrict(districtlist.Id);
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

        private async Task<Tuple<string, string>> InsertEventInPG(EventLinked eventtosave, string idtocheck, KeyValuePair<string, NinjaEvent> ninjaevent)
        {
            try
            {
                idtocheck = idtocheck.ToUpper();
                eventtosave.Id = eventtosave.Id?.ToUpper();

                //Set LicenseInfo
                eventtosave.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<Event>(eventtosave, Helper.LicenseHelper.GetLicenseforEvent);

                var rawdatid = await InsertInRawDataDB(ninjaevent);

                return await InsertInDB(eventtosave, idtocheck, "events", rawdatid);
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
                            datasource = "ninja",
                            importdate = DateTime.Now,
                            raw = JsonConvert.SerializeObject(ninjaevent.Value),
                            sourceinterface = "culture",
                            sourceid = ninjaevent.Key,
                            sourceurl = "https://mobility.api.opendatahub.bz.it/v2/flat/Culture/",
                            type = "event_centrotrevi-drin"
                        });
        }

        private async Task<Tuple<string, string>> InsertInDB(EventLinked eventtosave, string idtocheck, string tablename, int rawdataid)
        {
            //Check if data exists on PG

            var query =
               QueryFactory.Query("events")
                   .Select("data")
                   .Where("id", idtocheck);

            var eventindb = await query.GetAsync<JsonRaw>();

            if (eventindb.Count() == 0)
            {
                eventtosave.FirstImport = DateTime.Now;

                var queryresult = await QueryFactory.Query(tablename)
                               .InsertAsync(new JsonBDataRaw() { id = idtocheck, data = new JsonRaw(eventtosave), rawdataid = rawdataid });

                return Tuple.Create("insert", queryresult.ToString());
            }
            else
            {
                var queryresult = await QueryFactory.Query(tablename).Where("id", idtocheck)
                                .UpdateAsync(new JsonBDataRaw() { id = idtocheck, data = new JsonRaw(eventtosave), rawdataid = rawdataid });

                return Tuple.Create("update", queryresult.ToString());
            }
        }

        private async Task<LocationInfo?> GetTheLocationInfoDistrict(string districtid)
        {
            try
            {
                LocationInfo mylocinfo = new LocationInfo();

                var district = await QueryFactory.Query("districts").Select("data").Where("id", districtid.ToUpper()).GetFirstOrDefaultAsObject<District>();
                var districtnames = (from x in district?.Detail
                                     select x).ToDictionary(x => x.Key, x => x.Value.Title);

                var municipality = await QueryFactory.Query("municipalities").Select("data").Where("id", district?.MunicipalityId?.ToUpper()).GetFirstOrDefaultAsObject<Municipality>();
                var municipalitynames = (from x in municipality?.Detail
                                         select x).ToDictionary(x => x.Key, x => x.Value.Title);

                var tourismverein = await QueryFactory.Query("tvs").Select("data").Where("id", district?.TourismvereinId?.ToUpper()).GetFirstOrDefaultAsObject<Tourismverein>();
                var tourismvereinnames = (from x in tourismverein?.Detail
                                          select x).ToDictionary(x => x.Key, x => x.Value.Title);

                var region = await QueryFactory.Query("regions").Select("data").Where("id", district?.RegionId?.ToUpper()).GetFirstOrDefaultAsObject<Region>();
                var regionnames = (from x in region?.Detail
                                   select x).ToDictionary(x => x.Key, x => x.Value.Title);

                //conn.Close();

                mylocinfo.DistrictInfo = new DistrictInfo() { Id = district?.Id, Name = districtnames };
                mylocinfo.MunicipalityInfo = new MunicipalityInfo() { Id = municipality?.Id, Name = municipalitynames };
                mylocinfo.TvInfo = new TvInfo() { Id = tourismverein?.Id, Name = tourismvereinnames };
                mylocinfo.RegionInfo = new RegionInfo() { Id = region?.Id, Name = regionnames };

                return mylocinfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<List<string>> GetAllEventsBySource(List<string> sourcelist)
        {

            var query =
               QueryFactory.Query("events")
                   .Select("id")
                   .SourceFilter_GeneratedColumn(sourcelist);

            var eventids = await query.GetAsync<string>();

            return eventids.ToList();
        }

        private async Task<int> DeleteOrDisableEvents(string eventid, bool delete)
        {
            var result = 0;

            if (delete)
            {
                result = await QueryFactory.Query("events").Where("id", eventid)
                    .DeleteAsync();
            }
            else
            {
                var query =
               QueryFactory.Query("events")
                   .Select("data")
                   .Where("id", eventid);

                var data = await query.GetFirstOrDefaultAsObject<EventLinked>();

                if (data != null)
                {
                    if(data.Active != false || data.SmgActive != false)
                    {
                        data.Active = false;
                        data.SmgActive = false;

                        result = await QueryFactory.Query("events").Where("id", eventid)
                                        .UpdateAsync(new JsonBData() { id = eventid, data = new JsonRaw(data) });
                    }                
                }
            }

            return result;
        }   

        #endregion

    }
}
