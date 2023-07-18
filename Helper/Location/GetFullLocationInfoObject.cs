// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Microsoft.AspNetCore.Mvc.Formatters;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Location
{
    public class GetFullLocationInfoObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldlocationinfo"></param>
        /// <param name="locationlist"></param>
        /// <returns></returns>
        public static LocationInfoLinked UpdateLocationInfo(LocationInfoLinked oldlocationinfo, IEnumerable<LocHelperclassDynamic> locationlist)
        {
            LocationInfoLinked locationInfoLinked = new LocationInfoLinked();

            //Updating Region
            if(oldlocationinfo.RegionInfo != null && !String.IsNullOrEmpty(oldlocationinfo.RegionInfo.Id))
            {
                locationInfoLinked.RegionInfo = oldlocationinfo.RegionInfo;
                var regioninfoname = locationlist.Where(x => x.id == oldlocationinfo.RegionInfo.Id).FirstOrDefault();
                if(regioninfoname != null)
                    locationInfoLinked.RegionInfo.Name = regioninfoname.name;
            }

            //Updating TV
            if (oldlocationinfo.TvInfo != null && !String.IsNullOrEmpty(oldlocationinfo.TvInfo.Id))
            {
                locationInfoLinked.TvInfo = oldlocationinfo.TvInfo;
                var tvinfoname = locationlist.Where(x => x.id == oldlocationinfo.TvInfo.Id).FirstOrDefault();
                if (tvinfoname != null)
                    locationInfoLinked.TvInfo.Name = tvinfoname.name;
            }

            //Updating Municipality
            if (oldlocationinfo.MunicipalityInfo != null && !String.IsNullOrEmpty(oldlocationinfo.MunicipalityInfo.Id))
            {
                locationInfoLinked.MunicipalityInfo = oldlocationinfo.MunicipalityInfo;
                var municipalityinfoname = locationlist.Where(x => x.id == oldlocationinfo.MunicipalityInfo.Id).FirstOrDefault();
                if (municipalityinfoname != null)
                    locationInfoLinked.MunicipalityInfo.Name = municipalityinfoname.name;
            }

            //Updating District
            if (oldlocationinfo.DistrictInfo != null && !String.IsNullOrEmpty(oldlocationinfo.DistrictInfo.Id))
            {
                locationInfoLinked.DistrictInfo = oldlocationinfo.DistrictInfo;
                var districtinfoname = locationlist.Where(x => x.id == oldlocationinfo.DistrictInfo.Id).FirstOrDefault();
                if (districtinfoname != null)
                    locationInfoLinked.DistrictInfo.Name = districtinfoname.name;
            }

            //Updating Area
            if (oldlocationinfo.AreaInfo != null && !String.IsNullOrEmpty(oldlocationinfo.AreaInfo.Id))
            {
                locationInfoLinked.AreaInfo = oldlocationinfo.AreaInfo;
                var areainfoname = locationlist.Where(x => x.id == oldlocationinfo.AreaInfo.Id).FirstOrDefault();
                if (areainfoname != null)
                    locationInfoLinked.AreaInfo.Name = areainfoname.name;
            }

            return locationInfoLinked;
        }

        public static async Task<LocationInfoLinked> UpdateLocationInfo(LocationInfoLinked oldlocationinfo, QueryFactory queryFactory)
        {
            LocationInfoLinked locationInfoLinked = new LocationInfoLinked();

            //Updating Region
            if (oldlocationinfo.RegionInfo != null && !String.IsNullOrEmpty(oldlocationinfo.RegionInfo.Id))
            {
                locationInfoLinked.RegionInfo = oldlocationinfo.RegionInfo;

                var regioninfo = await LocationListCreator.GetLocationFromDB<Region>(queryFactory, "regions", Tuple.Create("id", oldlocationinfo.RegionInfo.Id));

                if (regioninfo != null && regioninfo.Count() > 0)
                {
                    var regioninfoname = regioninfo.FirstOrDefault().Detail.ToDictionary(x => x.Key, x => x.Value.Title);
                    if (regioninfoname != null)
                        locationInfoLinked.RegionInfo.Name = regioninfoname;
                }
            }

            //Updating TV
            if (oldlocationinfo.TvInfo != null && !String.IsNullOrEmpty(oldlocationinfo.TvInfo.Id))
            {
                locationInfoLinked.TvInfo = oldlocationinfo.TvInfo;

                var tvinfo = await LocationListCreator.GetLocationFromDB<Tourismverein>(queryFactory, "tvs", Tuple.Create("id", oldlocationinfo.TvInfo.Id));

                if (tvinfo != null && tvinfo.Count() > 0)
                {
                    var tvinfoname = tvinfo.FirstOrDefault().Detail.ToDictionary(x => x.Key, x => x.Value.Title);
                    if (tvinfoname != null)
                        locationInfoLinked.TvInfo.Name = tvinfoname;
                }
            }

            //Updating Municipality
            if (oldlocationinfo.MunicipalityInfo != null && !String.IsNullOrEmpty(oldlocationinfo.MunicipalityInfo.Id))
            {
                locationInfoLinked.MunicipalityInfo = oldlocationinfo.MunicipalityInfo;

                var muninfo = await LocationListCreator.GetLocationFromDB<Municipality>(queryFactory, "municipalities", Tuple.Create("id", oldlocationinfo.MunicipalityInfo.Id));

                if (muninfo != null && muninfo.Count() > 0)
                {
                    var municipalityinfoname = muninfo.FirstOrDefault().Detail.ToDictionary(x => x.Key, x => x.Value.Title);
                    if (municipalityinfoname != null)
                        locationInfoLinked.MunicipalityInfo.Name = municipalityinfoname;
                }
            }

            //Updating District
            if (oldlocationinfo.DistrictInfo != null && !String.IsNullOrEmpty(oldlocationinfo.DistrictInfo.Id))
            {
                locationInfoLinked.DistrictInfo = oldlocationinfo.DistrictInfo;

                var distinfo = await LocationListCreator.GetLocationFromDB<Tourismverein>(queryFactory, "districts", Tuple.Create("id", oldlocationinfo.DistrictInfo.Id));

                if (distinfo != null && distinfo.Count() > 0)
                {
                    var districtinfoname = distinfo.FirstOrDefault().Detail.ToDictionary(x => x.Key, x => x.Value.Title);
                    if (districtinfoname != null)
                        locationInfoLinked.DistrictInfo.Name = districtinfoname;
                }
            }

            //Updating Area
            if (oldlocationinfo.AreaInfo != null && !String.IsNullOrEmpty(oldlocationinfo.AreaInfo.Id))
            {
                locationInfoLinked.AreaInfo = oldlocationinfo.AreaInfo;

                var areainfo = await LocationListCreator.GetLocationFromDB<Tourismverein>(queryFactory, "areas", Tuple.Create("id", oldlocationinfo.AreaInfo.Id));

                if (areainfo != null && areainfo.Count() > 0)
                {
                    var areainfoname = areainfo.FirstOrDefault().Detail.ToDictionary(x => x.Key, x => x.Value.Title);
                    if (areainfoname != null)
                        locationInfoLinked.AreaInfo.Name = areainfoname;
                }
            }

            return locationInfoLinked;
        }
    }
}
