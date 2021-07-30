using DataModel;
using Helper;
using NINJA;
using NINJA.Parser;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<string> SaveEventsToPG(ICollection<NinjaData<NinjaEvent>> ninjadataarr, ICollection<NinjaData<NinjaPlaceRoom>> ninjaplaceroomarr)
        {
            try
            {
                var newimportcounter = 0;
                var updateimportcounter = 0;
                var errorimportcounter = 0;

                foreach (var ninjadata in ninjadataarr.Select(x => x.tmetadata))
                {
                    foreach (KeyValuePair<string, NinjaEvent> kvp in ninjadata)
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

                        var result = await InsertEventInPG(eventtosave, idtocheck);

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
                    }
                }

                return String.Format("Events Updated {0} New {1} Error {2}", updateimportcounter.ToString(), newimportcounter.ToString(), errorimportcounter.ToString());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
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

            if (districtlist != null)
            {
                myevent.DistrictId = districtlist.Id;
                myevent.DistrictIds = new List<string>() { districtlist.Id };

                //TODO MAYBE IN HELPER!
                var locinfo = await GetTheLocationInfoDistrict(districtlist.Id);

                LocationInfoLinked locinfolinked = new LocationInfoLinked();
                locinfolinked.DistrictInfo = new DistrictInfoLinked()
                {
                    Id = locinfo.DistrictInfo.Id,
                    Name = locinfo.DistrictInfo.Name
                };
                locinfolinked.MunicipalityInfo = new MunicipalityInfoLinked()
                {
                    Id = locinfo.MunicipalityInfo.Id,
                    Name = locinfo.MunicipalityInfo.Name
                };
                locinfolinked.TvInfo = new TvInfoLinked()
                {
                    Id = locinfo.TvInfo.Id,
                    Name = locinfo.TvInfo.Name
                };
                locinfolinked.RegionInfo = new RegionInfoLinked()
                {
                    Id = locinfo.RegionInfo.Id,
                    Name = locinfo.RegionInfo.Name
                };

                myevent.LocationInfo = locinfolinked;
            }
        }

        private async Task<Tuple<string, string>> InsertEventInPG(EventLinked eventtosave, string idtocheck)
        {
            try
            {
                idtocheck = idtocheck.ToUpper();
                eventtosave.Id = eventtosave.Id.ToUpper();

                //Set LicenseInfo
                eventtosave.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<Event>(eventtosave, Helper.LicenseHelper.GetLicenseforEvent);

                return await InsertInDB(eventtosave, idtocheck, "events");
            }
            catch (Exception ex)
            {
                return Tuple.Create("error", ex.Message);
            }
        }

        private async Task<Tuple<string, string>> InsertInDB(EventLinked eventtosave, string idtocheck, string tablename)
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
                               .InsertAsync(new JsonBData() { id = idtocheck, data = new JsonRaw(eventtosave) });

                return Tuple.Create("insert", queryresult.ToString());
            }
            else
            {
                var queryresult = await QueryFactory.Query(tablename).Where("id", idtocheck)
                                .UpdateAsync(new JsonBData() { id = idtocheck, data = new JsonRaw(eventtosave) });

                return Tuple.Create("update", queryresult.ToString());
            }
        }

        private async Task<LocationInfo> GetTheLocationInfoDistrict(string districtid)
        {
            try
            {
                LocationInfo mylocinfo = new LocationInfo();

                var district = await QueryFactory.Query("districts").Select("data").Where("id", districtid.ToUpper()).GetFirstOrDefaultAsObject<District>();
                var districtnames = (from x in district.Detail
                                     select x).ToDictionary(x => x.Key, x => x.Value.Title);

                var municipality = await QueryFactory.Query("municipalities").Select("data").Where("id", district.MunicipalityId.ToUpper()).GetFirstOrDefaultAsObject<Municipality>();
                var municipalitynames = (from x in municipality.Detail
                                         select x).ToDictionary(x => x.Key, x => x.Value.Title);

                var tourismverein = await QueryFactory.Query("tvs").Select("data").Where("id", district.TourismvereinId.ToUpper()).GetFirstOrDefaultAsObject<Tourismverein>();
                var tourismvereinnames = (from x in tourismverein.Detail
                                          select x).ToDictionary(x => x.Key, x => x.Value.Title);

                var region = await QueryFactory.Query("regions").Select("data").Where("id", district.RegionId.ToUpper()).GetFirstOrDefaultAsObject<Region>();
                var regionnames = (from x in region.Detail
                                   select x).ToDictionary(x => x.Key, x => x.Value.Title);

                //conn.Close();

                mylocinfo.DistrictInfo = new DistrictInfo() { Id = district.Id, Name = districtnames };
                mylocinfo.MunicipalityInfo = new MunicipalityInfo() { Id = municipality.Id, Name = municipalitynames };
                mylocinfo.TvInfo = new TvInfo() { Id = tourismverein.Id, Name = tourismvereinnames };
                mylocinfo.RegionInfo = new RegionInfo() { Id = region.Id, Name = regionnames };

                return mylocinfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

    }
}
