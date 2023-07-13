// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Location
{
    public class GetFullLocationInfoObject
    {
        public static LocationInfoLinked GetFromList(IEnumerable<LocHelperclassDynamic> locationlist, LocationInfoLinked oldlocationinfo)
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
    }
}
