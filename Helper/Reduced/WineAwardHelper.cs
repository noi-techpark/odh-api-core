// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class WineAwardHelper
    {
        public static async Task<IEnumerable<ReducedWineAward>> GetReducedWithWineAwardList(QueryFactory QueryFactory)
        {
            try
            {
                List<ReducedWineAward> reducedlist = new List<ReducedWineAward>();

                string select = "data#>>'\\{Id\\}' as Id, data#>>'\\{CompanyId\\}' as CompanyId, (data#>>'\\{Detail,de,Title\\}') as Name";

                var query = QueryFactory.Query("wines")
                        .SelectRaw(select)
                        .Where("gen_active", true);

                var data =
                    await query
                        .GetAsync<ReducedWineAward>();

                return data;
            }
            catch (Exception ex)
            {                
                return new List<ReducedWineAward>();
            }
        }   
    }

    public class ReducedWineAward
    {
        public string Id { get; set; }

        public string CompanyId { get; set; }

        public string Name { get; set; }
    }
}
