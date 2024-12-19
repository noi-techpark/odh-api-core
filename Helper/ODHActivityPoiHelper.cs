// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace Helper
{
    public class ODHActivityPoiHelper
    {
        public static void SetSustainableHikingTag(
            ODHActivityPoiLinked mysmgpoi,
            List<string> languagelist
        )
        {
            //NEW if the field "PublicTransportationInfo" is not empty, set the new ODHTag "sustainable-hiking".

            if (mysmgpoi.SmgTags != null && mysmgpoi.SmgTags.Contains("Wandern"))
            {
                bool haspublictrasportationinfo = false;
                if (mysmgpoi.Detail != null)
                {
                    foreach (var languagecategory in languagelist)
                    {
                        if (
                            mysmgpoi.Detail.ContainsKey(languagecategory)
                            && !String.IsNullOrEmpty(
                                mysmgpoi.Detail[languagecategory].PublicTransportationInfo
                            )
                        )
                        {
                            haspublictrasportationinfo = true;
                        }
                    }
                }
                if (haspublictrasportationinfo)
                {
                    if (mysmgpoi.SmgTags == null)
                        mysmgpoi.SmgTags = new List<string>();

                    if (!mysmgpoi.SmgTags.Contains("sustainable-hiking"))
                        mysmgpoi.SmgTags.Add("sustainable-hiking");
                }
                else if (!haspublictrasportationinfo)
                {
                    if (mysmgpoi.SmgTags != null)
                    {
                        if (mysmgpoi.SmgTags.Contains("sustainable-hiking"))
                            mysmgpoi.SmgTags.Remove("sustainable-hiking");
                    }
                }
            }
        }

        public static string SetMainCategorizationForODHActivityPoi(ODHActivityPoiLinked smgpoi)
        {
            //Add LTS Id as Mapping
            var maintype = "Poi";

            if (smgpoi.SyncSourceInterface.ToLower() == "activitydata")
                maintype = "activity";
            if (smgpoi.SyncSourceInterface.ToLower() == "poidata")
                maintype = "poi";
            if (smgpoi.SyncSourceInterface.ToLower() == "gastronomicdata")
                maintype = "gastronomy";
            if (smgpoi.SyncSourceInterface.ToLower() == "beacondata")
                maintype = "poi";
            if (smgpoi.SyncSourceInterface.ToLower() == "archapp")
                maintype = "poi";
            if (smgpoi.SyncSourceInterface.ToLower() == "museumdata")
                maintype = "poi";
            if (smgpoi.SyncSourceInterface.ToLower() == "suedtirolwein")
                maintype = "gastronomy";
            if (smgpoi.SyncSourceInterface.ToLower() == "common")
                maintype = "activity";
            if (smgpoi.SyncSourceInterface.ToLower() == "none")
                maintype = "poi";
            if (smgpoi.SyncSourceInterface.ToLower() == "magnolia")
                maintype = "poi";

            if (
                !smgpoi.SmgTags.Contains("activity")
                && !smgpoi.SmgTags.Contains("poi")
                && !smgpoi.SmgTags.Contains("gastronomy")
            )
            {
                //Assign to SMGTags if not there
                if (!smgpoi.SmgTags.Contains(maintype))
                    smgpoi.SmgTags.Add(maintype);
            }

            return maintype;
        }

        public static void AddLTSMappingToODHActivityPoi(ODHActivityPoiLinked smgpoi)
        {
            //Add Mapping
            var mapping = new Dictionary<string, string>() { { "rid", smgpoi.CustomId } };
            smgpoi.Mapping.TryAddOrUpdate("lts", mapping);
        }
    }
}
