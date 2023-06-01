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

    public class ODHTagHelper
    {
        public static async Task<IEnumerable<SmgTags>> GetODHTagsValidforTranslations(QueryFactory QueryFactory, List<string> validforentity, List<string>? idlist = null)
        {
            try
            {
                List<SmgTags> validtags = new List<SmgTags>();

                var validtagquery = QueryFactory.Query("smgtags")
                        .Select("data")
                        .When(validforentity.Count > 0, q => q.WhereInJsonb(
                            validforentity,
                            tag => new { ValidForEntity = new[] { tag.ToLower() } }
                        ))
                        .When(idlist != null, w => w.WhereIn("id", idlist?.Select(x => x.ToLower()) ?? Enumerable.Empty<string>()))
                        .WhereRaw("data->>'DisplayAsCategory' = $$", "true");

                var validtagdata =
                    await validtagquery
                        .GetObjectListAsync<SmgTags>();

                return validtagdata;
            }
            catch (Exception)
            {
                return new List<SmgTags>();
            }
        }

        public static async Task<SmgTags?> GeODHTagByID(QueryFactory QueryFactory, string id)
        {
            try
            {
                var query =
                       QueryFactory.Query("smgtags")
                           .Select("data")
                           .Where("id", id.ToLower());

                var result = await query.GetObjectSingleAsync<SmgTags>();

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string SetMainCategorizationForODHActivityPoi(ODHActivityPoiLinked smgpoi)
        {
            //Add LTS Id as Mapping
            var maintype = "poi";

            if (!String.IsNullOrEmpty(smgpoi.SyncSourceInterface) && smgpoi.SyncSourceInterface.ToLower() == "activitydata")
                maintype = "activity";
            if (!String.IsNullOrEmpty(smgpoi.SyncSourceInterface) && smgpoi.SyncSourceInterface.ToLower() == "poidata")
                maintype = "poi";
            if (!String.IsNullOrEmpty(smgpoi.SyncSourceInterface) && smgpoi.SyncSourceInterface.ToLower() == "gastronomicdata")
                maintype = "gastronomy";
            if (!String.IsNullOrEmpty(smgpoi.SyncSourceInterface) && smgpoi.SyncSourceInterface.ToLower() == "beacondata")
                maintype = "poi";
            if (!String.IsNullOrEmpty(smgpoi.SyncSourceInterface) && smgpoi.SyncSourceInterface.ToLower() == "archapp")
                maintype = "poi";
            if (!String.IsNullOrEmpty(smgpoi.SyncSourceInterface) && smgpoi.SyncSourceInterface.ToLower() == "museumdata")
                maintype = "poi";
            if (!String.IsNullOrEmpty(smgpoi.SyncSourceInterface) && smgpoi.SyncSourceInterface.ToLower() == "suedtirolwein")
                maintype = "gastronomy";
            if (!String.IsNullOrEmpty(smgpoi.SyncSourceInterface) && smgpoi.SyncSourceInterface.ToLower() == "common")
                maintype = "activity";
            if (!String.IsNullOrEmpty(smgpoi.SyncSourceInterface) && smgpoi.SyncSourceInterface.ToLower() == "none")
                maintype = "poi";
            if (!String.IsNullOrEmpty(smgpoi.SyncSourceInterface) && smgpoi.SyncSourceInterface.ToLower() == "magnolia")
                maintype = "poi";
            if (!String.IsNullOrEmpty(smgpoi.SyncSourceInterface) && smgpoi.SyncSourceInterface.ToLower() == "sta")
                maintype = "poi";
            if (!String.IsNullOrEmpty(smgpoi.SyncSourceInterface) && smgpoi.SyncSourceInterface.ToLower() == "dssliftbase")
                maintype = "activity";
            if (!String.IsNullOrEmpty(smgpoi.SyncSourceInterface) && smgpoi.SyncSourceInterface.ToLower() == "dssslopebase")
                maintype = "activity";

            if(smgpoi.SmgTags == null)
            {
                smgpoi.SmgTags = new List<string>();
                smgpoi.SmgTags.Add(maintype);
            }
            else if(!smgpoi.SmgTags.Contains("activity") && !smgpoi.SmgTags.Contains("poi") && !smgpoi.SmgTags.Contains("gastronomy"))
            {
                //Assign to SMGTags if not there
                if (!smgpoi.SmgTags.Contains(maintype))
                    smgpoi.SmgTags.Add(maintype);
            }

            return maintype;
        }

    }
}
