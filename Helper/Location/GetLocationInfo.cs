// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Newtonsoft.Json;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class GetLocationInfo
    {        
        public static async Task<IEnumerable<District>> GetNearestDistrict(QueryFactory QueryFactory, PGGeoSearchResult geosearchresult, int limitto)
        {
            var districtquery = QueryFactory.Query("districts")
                        .Select("data")
                        .ApplyOrdering_GeneratedColumns(geosearchresult, null)
                        .Limit(limitto); 
            
            var data =
                await districtquery
                    .GetObjectListAsync<District>();

            return data;
        }

        public static async Task<District?> GetNearestDistrictbyGPS(QueryFactory QueryFactory, double latitude, double longitude, int radius = 30000)
        {
            string wheregeo = PostgresSQLHelper.GetGeoWhereSimple(latitude, longitude, radius);
            string orderbygeo = PostgresSQLHelper.GetGeoOrderBySimple(latitude, longitude);

            var query =
                     QueryFactory.Query("districts")
                         .Select("data")
                         .WhereRaw(wheregeo)
                         .OrderByRaw(orderbygeo);

            return await query.GetObjectSingleAsync<District>();
        }

        public static async Task<LocationInfoLinked> GetTheLocationInfoDistrict(QueryFactory QueryFactory, string? districtid)
        {
            if (districtid == null)
                return new LocationInfoLinked();

            LocationInfoLinked mylocinfo = new LocationInfoLinked();

            //Wenn nicht District nicht definiert ist oder Livinallongo/Arabba/Gebiet Pieve - Digonera - Pordoijoch - nicht südtirol ;)
            if (districtid != "79CBD63051C911D18F1400A02427D15E" && districtid != "53DF587C2BF74853B9DF3429089587E3" && districtid != "43C0E6789C4046718B70DAA56CF4332C" && districtid != "52B456D784854FB5A77F87C0CF4AFADF" && districtid != "C17DC9768C1A4DC5BA1592ED5C1D591B" && districtid != "79CBAAA0513331D18F1400A02427D15E")
            {
                var districtquery = QueryFactory.Query("districts")
                        .Select("data")
                        .Where("id", districtid.ToUpper());
                var district = await districtquery.GetObjectSingleAsync<District>();

                var districtnames = (from x in district?.Detail
                                     select x).ToDictionary(x => x.Key, x => x.Value.Title);

                var munquery = QueryFactory.Query("municipalities")
                        .Select("data")
                        .Where("id", district?.MunicipalityId?.ToUpper());
                var municipality = await munquery.GetObjectSingleAsync<Municipality>();
                
                var municipalitynames = (from x in municipality?.Detail
                                         select x).ToDictionary(x => x.Key, x => x.Value.Title);

                var tvquery = QueryFactory.Query("tvs")
                        .Select("data")
                        .Where("id", district?.TourismvereinId?.ToUpper());
                var tourismverein = await tvquery.GetObjectSingleAsync<Tourismverein>();
                
                var tourismvereinnames = (from x in tourismverein?.Detail
                                          select x).ToDictionary(x => x.Key, x => x.Value.Title);

                var regquery = QueryFactory.Query("regions")
                        .Select("data")
                        .Where("id", district?.RegionId?.ToUpper());
                var region = await regquery.GetObjectSingleAsync<Region>();
                var regionnames = new Dictionary<string, string?>();
                
                if(region != null)
                {
                    regionnames = (from x in region?.Detail
                                       select x).ToDictionary(x => x.Key, x => x.Value.Title);

                }

                mylocinfo.DistrictInfo = new DistrictInfoLinked() { Id = district?.Id, Name = districtnames };
                mylocinfo.MunicipalityInfo = new MunicipalityInfoLinked() { Id = municipality?.Id, Name = municipalitynames };
                mylocinfo.TvInfo = new TvInfoLinked() { Id = tourismverein?.Id, Name = tourismvereinnames };
                mylocinfo.RegionInfo = new RegionInfoLinked() { Id = region?.Id, Name = regionnames };
            }
            return mylocinfo;
        }

        public static async Task<LocationInfoLinked> GetTheLocationInfoDistrict_Siag(QueryFactory QueryFactory, string? districtid_siag)
        {
            if (districtid_siag == null)
                return new LocationInfoLinked();

            LocationInfoLinked mylocinfo = new LocationInfoLinked();
            
            var districtquery = QueryFactory.Query("districts")
                         .Select("data")
                         .WhereRaw("data->>'SiagId' = $$", districtid_siag);

            var district = await districtquery.GetObjectSingleAsync<District>();


            if (district != null)
            {
               
                var districtnames = (from x in district?.Detail
                                     select x).ToDictionary(x => x.Key, x => x.Value.Title);

                var munquery = QueryFactory.Query("municipalities")
                        .Select("data")
                        .Where("id", district?.MunicipalityId?.ToUpper());
                var municipality = await munquery.GetObjectSingleAsync<Municipality>();

                var municipalitynames = (from x in municipality?.Detail
                                         select x).ToDictionary(x => x.Key, x => x.Value.Title);

                var tvquery = QueryFactory.Query("tvs")
                        .Select("data")
                        .Where("id", district?.TourismvereinId?.ToUpper());
                var tourismverein = await tvquery.GetObjectSingleAsync<Tourismverein>();

                var tourismvereinnames = (from x in tourismverein?.Detail
                                          select x).ToDictionary(x => x.Key, x => x.Value.Title);

                var regquery = QueryFactory.Query("regions")
                        .Select("data")
                        .Where("id", district?.RegionId?.ToUpper());
                var region = await regquery.GetObjectSingleAsync<Region>();
                var regionnames = new Dictionary<string, string?>();

                if(region != null)
                {
                    regionnames = (from x in region?.Detail
                                   select x).ToDictionary(x => x.Key, x => x.Value.Title);
                }                
                
                mylocinfo.DistrictInfo = new DistrictInfoLinked() { Id = district?.Id, Name = districtnames };
                mylocinfo.MunicipalityInfo = new MunicipalityInfoLinked() { Id = municipality?.Id, Name = municipalitynames };
                mylocinfo.TvInfo = new TvInfoLinked() { Id = tourismverein?.Id, Name = tourismvereinnames };
                mylocinfo.RegionInfo = new RegionInfoLinked() { Id = region?.Id, Name = regionnames };
            }

            return mylocinfo;
        }

        public static async Task<LocationInfoLinked> GetTheLocationInfoMunicipality_Siag(QueryFactory QueryFactory, string? municipalityid_siag)
        {
            if (municipalityid_siag == null)
                return new LocationInfoLinked();

            LocationInfoLinked mylocinfo = new LocationInfoLinked();

            var municipalityquery = QueryFactory.Query("municipalities")
                         .Select("data")
                         .WhereRaw("data->>'SiagId' = $$", municipalityid_siag);

            var municipality = await municipalityquery.GetObjectSingleAsync<District>();

            if (municipality != null)
            {              
                var municipalitynames = (from x in municipality?.Detail
                                         select x).ToDictionary(x => x.Key, x => x.Value.Title);

                var tvquery = QueryFactory.Query("tvs")
                        .Select("data")
                        .Where("id", municipality?.TourismvereinId?.ToUpper());
                var tourismverein = await tvquery.GetObjectSingleAsync<Tourismverein>();

                var tourismvereinnames = (from x in tourismverein?.Detail
                                          select x).ToDictionary(x => x.Key, x => x.Value.Title);

                var regquery = QueryFactory.Query("regions")
                        .Select("data")
                        .Where("id", municipality?.RegionId?.ToUpper());
                var region = await regquery.GetObjectSingleAsync<Region>();

                var regionnames = new Dictionary<string, string?>();

                if (region != null)
                {
                    regionnames = (from x in region?.Detail
                                   select x).ToDictionary(x => x.Key, x => x.Value.Title);
                }

                //mylocinfo.DistrictInfo = new DistrictInfoLinked() { Id = district?.Id, Name = districtnames };
                mylocinfo.MunicipalityInfo = new MunicipalityInfoLinked() { Id = municipality?.Id, Name = municipalitynames };
                mylocinfo.TvInfo = new TvInfoLinked() { Id = tourismverein?.Id, Name = tourismvereinnames };
                mylocinfo.RegionInfo = new RegionInfoLinked() { Id = region?.Id, Name = regionnames };
            }

            return mylocinfo;
        }


        ////Get Complete Locationinfo for Municipality ID
        //public static LocationInfo GetTheLocationInfoMunicipality(NpgsqlConnection conn, string municipalityid)
        //{
        //    LocationInfo mylocinfo = new LocationInfo();



        //    var municipality = PostgresSQLHelper.SelectFromTableDataAsObject<Municipality>(conn, "municipalities", "*", "id LIKE '" + municipalityid.ToUpper() + "'", "", 0, null).FirstOrDefault();
        //    var municipalitynames = (from x in municipality.Detail
        //                             select x).ToDictionary(x => x.Key, x => x.Value.Title);

        //    var tourismverein = PostgresSQLHelper.SelectFromTableDataAsObject<Tourismverein>(conn, "tvs", "*", "id LIKE '" + municipality.TourismvereinId.ToUpper() + "'", "", 0, null).FirstOrDefault();
        //    var tourismvereinnames = (from x in tourismverein.Detail
        //                              select x).ToDictionary(x => x.Key, x => x.Value.Title);

        //    var region = PostgresSQLHelper.SelectFromTableDataAsObject<Region>(conn, "regions", "*", "id LIKE '" + municipality.RegionId.ToUpper() + "'", "", 0, null).FirstOrDefault();
        //    var regionnames = (from x in region.Detail
        //                       select x).ToDictionary(x => x.Key, x => x.Value.Title);

        //    mylocinfo.MunicipalityInfo = new MunicipalityInfo() { Id = municipality.Id, Name = municipalitynames };
        //    mylocinfo.TvInfo = new TvInfo() { Id = tourismverein.Id, Name = tourismvereinnames };
        //    mylocinfo.RegionInfo = new SuedtirolDB.RegionInfo() { Id = region.Id, Name = regionnames };

        //    return mylocinfo;
        //}

        //public static LocationInfo GetTheLocationInfoArea(NpgsqlConnection conn, string areaid)
        //{
        //    LocationInfo mylocinfo = new LocationInfo();
        //    //Query auf District mit include auf alle
        //    if (areaid != null)
        //    {
        //        var area = PostgresSQLHelper.SelectFromTableDataAsObject<Area>(conn, "areas", "*", "id LIKE '" + areaid + "'", "", 0, null).FirstOrDefault();

        //        if (area != null)
        //        {

        //            Dictionary<string, string> areanames = new Dictionary<string, string>();
        //            areanames.Add("de", area.Shortname);
        //            areanames.Add("it", area.Shortname);
        //            areanames.Add("en", area.Shortname);
        //            areanames.Add("nl", area.Shortname);
        //            areanames.Add("cs", area.Shortname);
        //            areanames.Add("pl", area.Shortname);
        //            areanames.Add("fr", area.Shortname);
        //            areanames.Add("ru", area.Shortname);


        //            mylocinfo.AreaInfo = new AreaInfo() { Id = area.Id, Name = areanames };

        //            //var municipality = session.Load<Municipality>(area.MunicipalityId);
        //            //var municipalitynames = (from x in municipality.Detail
        //            //                         select x).ToDictionary(x => x.Key, x => x.Value.Title);

        //            if (area.TourismvereinId != null)
        //            {
        //                var tourismverein = PostgresSQLHelper.SelectFromTableDataAsObject<Tourismverein>(conn, "tvs", "*", "id LIKE '" + area.TourismvereinId.ToUpper() + "'", "", 0, null).FirstOrDefault();
        //                var tourismvereinnames = (from x in tourismverein.Detail
        //                                          select x).ToDictionary(x => x.Key, x => x.Value.Title);

        //                mylocinfo.TvInfo = new TvInfo() { Id = tourismverein.Id, Name = tourismvereinnames };
        //            }


        //            if (area.RegionId != null)
        //            {
        //                var region = PostgresSQLHelper.SelectFromTableDataAsObject<Region>(conn, "regions", "*", "id LIKE '" + area.RegionId.ToUpper() + "'", "", 0, null).FirstOrDefault();
        //                var regionnames = (from x in region.Detail
        //                                   select x).ToDictionary(x => x.Key, x => x.Value.Title);

        //                mylocinfo.RegionInfo = new SuedtirolDB.RegionInfo() { Id = region.Id, Name = regionnames };
        //            }

        //            //mylocinfo.MunicipalityInfo = new MunicipalityInfo() { Id = municipality.Id, Name = municipalitynames };

        //        }
        //    }
        //    return mylocinfo;
        //}

        //public static LocationInfo GetTheLocationInfoTourismverein(NpgsqlConnection conn, string tvid)
        //{
        //    LocationInfo mylocinfo = new LocationInfo();
        //    //Query auf District mit include auf alle
        //    if (tvid != null)
        //    {
        //        var tv = PostgresSQLHelper.SelectFromTableDataAsObject<Tourismverein>(conn, "tvs", "*", "id LIKE '" + tvid.ToUpper() + "'", "", 0, null).FirstOrDefault();

        //        if (tv != null)
        //        {
        //            var tourismvereinnames = (from x in tv.Detail
        //                                      select x).ToDictionary(x => x.Key, x => x.Value.Title);

        //            mylocinfo.TvInfo = new TvInfo() { Id = tv.Id, Name = tourismvereinnames };



        //            if (tv.RegionId != null)
        //            {
        //                var region = PostgresSQLHelper.SelectFromTableDataAsObject<Region>(conn, "regions", "*", "id LIKE '" + tv.RegionId.ToUpper() + "'", "", 0, null).FirstOrDefault();

        //                var regionnames = (from x in region.Detail
        //                                   select x).ToDictionary(x => x.Key, x => x.Value.Title);

        //                mylocinfo.RegionInfo = new SuedtirolDB.RegionInfo() { Id = region.Id, Name = regionnames };
        //            }
        //        }
        //        //mylocinfo.MunicipalityInfo = new MunicipalityInfo() { Id = municipality.Id, Name = municipalitynames };


        //    }
        //    return mylocinfo;
        //}

        //public static LocationInfo GetTheLocationInfoRegion(NpgsqlConnection conn, string regionid)
        //{
        //    LocationInfo mylocinfo = new LocationInfo();
        //    //Query auf District mit include auf alle
        //    if (regionid != null)
        //    {

        //        var region = PostgresSQLHelper.SelectFromTableDataAsObject<Region>(conn, "regions", "*", "id LIKE '" + regionid.ToUpper() + "'", "", 0, null).FirstOrDefault();


        //        if (region != null)
        //        {

        //            var regionnames = (from x in region.Detail
        //                               select x).ToDictionary(x => x.Key, x => x.Value.Title);

        //            mylocinfo.RegionInfo = new SuedtirolDB.RegionInfo() { Id = region.Id, Name = regionnames };

        //        }

        //    }
        //    return mylocinfo;
        //}

        ////SonderFallSiag ID

        //public static LocationInfo GetTheLocationInfoDistrictSiag(NpgsqlConnection conn, string districtidsiag)
        //{
        //    LocationInfo mylocinfo = new LocationInfo();

        //    //Query auf District mit include auf alle
        //    var district = PostgresSQLHelper.SelectFromTableDataAsObject<District>(conn, "districts", "*", "data ->>'SiagId' LIKE '" + districtidsiag + "'", "", 0, null).FirstOrDefault();

        //    //session.Query<District, DistrictFilter>()
        //    //.Customize(x => x.Include<District>(o => o.MunicipalityId).Include<District>(o => o.TourismvereinId).Include<District>(o => o.RegionId))
        //    //.Where(x => x.SiagId == districtidsiag)
        //    //.FirstOrDefault();

        //    if (district != null)
        //    {
        //        var districtnames = (from x in district.Detail
        //                             select x).ToDictionary(x => x.Key, x => x.Value.Title);

        //        var municipality = PostgresSQLHelper.SelectFromTableDataAsObject<Municipality>(conn, "municipalities", "*", "id LIKE '" + district.MunicipalityId.ToUpper() + "'", "", 0, null).FirstOrDefault();
        //        var municipalitynames = (from x in municipality.Detail
        //                                 select x).ToDictionary(x => x.Key, x => x.Value.Title);

        //        var tourismverein = PostgresSQLHelper.SelectFromTableDataAsObject<Tourismverein>(conn, "tvs", "*", "id LIKE '" + district.TourismvereinId.ToUpper() + "'", "", 0, null).FirstOrDefault();
        //        var tourismvereinnames = (from x in tourismverein.Detail
        //                                  select x).ToDictionary(x => x.Key, x => x.Value.Title);

        //        var region = PostgresSQLHelper.SelectFromTableDataAsObject<Region>(conn, "regions", "*", "id LIKE '" + district.RegionId.ToUpper() + "'", "", 0, null).FirstOrDefault();
        //        var regionnames = (from x in region.Detail
        //                           select x).ToDictionary(x => x.Key, x => x.Value.Title);


        //        mylocinfo.DistrictInfo = new DistrictInfo() { Id = district.Id, Name = districtnames };
        //        mylocinfo.MunicipalityInfo = new MunicipalityInfo() { Id = municipality.Id, Name = municipalitynames };
        //        mylocinfo.TvInfo = new TvInfo() { Id = tourismverein.Id, Name = tourismvereinnames };
        //        mylocinfo.RegionInfo = new SuedtirolDB.RegionInfo() { Id = region.Id, Name = regionnames };
        //    }

        //    return mylocinfo;
        //}

        //public static LocationInfo GetTheLocationInfoMunicipalitySiag(NpgsqlConnection conn, string municipalitysiagid)
        //{
        //    LocationInfo mylocinfo = new LocationInfo();


        //    //Query auf District mit include auf alle
        //    var municipality = PostgresSQLHelper.SelectFromTableDataAsObject<Municipality>(conn, "municipalities", "*", "data ->>'SiagId' LIKE '" + municipalitysiagid + "'", "", 0, null).FirstOrDefault();

        //    //session.Query<Municipality, MunicipalitiyFilter>()
        //    //.Customize(x => x.Include<Municipality>(o => o.TourismvereinId).Include<Municipality>(o => o.RegionId))
        //    //.Where(x => x.SiagId == municipalitysiagid)
        //    //.FirstOrDefault();

        //    if (municipality != null)
        //    {
        //        var municipalitynames = (from x in municipality.Detail
        //                                 select x).ToDictionary(x => x.Key, x => x.Value.Title);

        //        var tourismverein = PostgresSQLHelper.SelectFromTableDataAsObject<Tourismverein>(conn, "tvs", "*", "id LIKE '" + municipality.TourismvereinId.ToUpper() + "'", "", 0, null).FirstOrDefault();
        //        var tourismvereinnames = (from x in tourismverein.Detail
        //                                  select x).ToDictionary(x => x.Key, x => x.Value.Title);

        //        var region = PostgresSQLHelper.SelectFromTableDataAsObject<Region>(conn, "regions", "*", "id LIKE '" + municipality.RegionId.ToUpper() + "'", "", 0, null).FirstOrDefault();
        //        var regionnames = (from x in region.Detail
        //                           select x).ToDictionary(x => x.Key, x => x.Value.Title);


        //        //mylocinfo.DistrictInfo = new DistrictInfo() { Id = district.Id, Name = districtnames };
        //        mylocinfo.MunicipalityInfo = new MunicipalityInfo() { Id = municipality.Id, Name = municipalitynames };
        //        mylocinfo.TvInfo = new TvInfo() { Id = tourismverein.Id, Name = tourismvereinnames };
        //        mylocinfo.RegionInfo = new SuedtirolDB.RegionInfo() { Id = region.Id, Name = regionnames };
        //    }

        //    return mylocinfo;
        //}

        ////Sonderfall mehrere Areas
        //public static LocationInfo GetTheLocationInfoAreas(NpgsqlConnection conn, List<string> areaids, string owner)
        //{
        //    LocationInfo mylocinfo = new LocationInfo();

        //    foreach (string areaid in areaids)
        //    {

        //        //Query auf District mit include auf alle
        //        if (areaid != null)
        //        {

        //            var area = PostgresSQLHelper.SelectFromTableDataAsObject<Area>(conn, "areas", "*", "id LIKE '" + areaid + "'", "", 0, null).FirstOrDefault();

        //            if (area != null)
        //            {
        //                if (area.RegionId == owner || area.TourismvereinId == owner)
        //                {

        //                    Dictionary<string, string> areanames = new Dictionary<string, string>();
        //                    areanames.Add("de", area.Shortname);
        //                    areanames.Add("it", area.Shortname);
        //                    areanames.Add("en", area.Shortname);
        //                    areanames.Add("nl", area.Shortname);
        //                    areanames.Add("cs", area.Shortname);
        //                    areanames.Add("pl", area.Shortname);
        //                    areanames.Add("fr", area.Shortname);
        //                    areanames.Add("ru", area.Shortname);


        //                    mylocinfo.AreaInfo = new AreaInfo() { Id = area.Id, Name = areanames };

        //                    //var municipality = session.Load<Municipality>(area.MunicipalityId);
        //                    //var municipalitynames = (from x in municipality.Detail
        //                    //                         select x).ToDictionary(x => x.Key, x => x.Value.Title);

        //                    if (area.TourismvereinId != null)
        //                    {
        //                        var tourismverein = PostgresSQLHelper.SelectFromTableDataAsObject<Tourismverein>(conn, "tvs", "*", "id LIKE '" + area.TourismvereinId.ToUpper() + "'", "", 0, null).FirstOrDefault();
        //                        var tourismvereinnames = (from x in tourismverein.Detail
        //                                                  select x).ToDictionary(x => x.Key, x => x.Value.Title);

        //                        mylocinfo.TvInfo = new TvInfo() { Id = tourismverein.Id, Name = tourismvereinnames };
        //                    }


        //                    if (area.RegionId != null)
        //                    {
        //                        var region = PostgresSQLHelper.SelectFromTableDataAsObject<Region>(conn, "regions", "*", "id LIKE '" + area.RegionId.ToUpper() + "'", "", 0, null).FirstOrDefault();
        //                        var regionnames = (from x in region.Detail
        //                                           select x).ToDictionary(x => x.Key, x => x.Value.Title);

        //                        mylocinfo.RegionInfo = new RegionInfo() { Id = region.Id, Name = regionnames };
        //                    }

        //                    //mylocinfo.MunicipalityInfo = new MunicipalityInfo() { Id = municipality.Id, Name = municipalitynames };
        //                }
        //            }
        //        }
        //    }
        //    return mylocinfo;
        //}
    }
}
