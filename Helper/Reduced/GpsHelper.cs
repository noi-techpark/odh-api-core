// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using SqlKata.Execution;

namespace Helper
{
    public class GpsHelper
    {
        public static async Task<IEnumerable<ReducedWithGPSInfo>> GetReducedWithGPSInfoList(
            QueryFactory QueryFactory,
            string table
        )
        {
            try
            {
                List<ReducedWithGPSInfo> reducedlist = new List<ReducedWithGPSInfo>();

                string select =
                    "data#>>'\\{Id\\}' as Id, (data#>>'\\{Latitude\\}')::double precision as Latitude, (data#>>'\\{Longitude\\}')::double precision as Longitude, data#>>'\\{_Meta,Type\\}' as Type";

                var query = QueryFactory.Query(table).SelectRaw(select);

                var data = await query.GetAsync<ReducedWithGPSInfo>();

                return data;
            }
            catch (Exception ex)
            {
                return new List<ReducedWithGPSInfo>();
            }
        }
    }

    public class ReducedWithGPSInfo
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string Id { get; set; }

        public string Type { get; set; }
    }
}
