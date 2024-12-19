// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class VenueListCreator
    {
        public static List<string> CreateVenueCategoryListfromFlag(string? venuecategoryfilter)
        {
            List<string> rids = new List<string>();

            if (!String.IsNullOrEmpty(venuecategoryfilter))
            {
                if (venuecategoryfilter != "null")
                {
                    var venuecategoryfilterint = Convert.ToInt32(venuecategoryfilter);

                    VenueCategoryFlag myflag = (VenueCategoryFlag)venuecategoryfilterint;

                    var myflags = myflag.GetFlags().GetDescriptionList();

                    foreach (var flag in myflags)
                    {
                        rids.Add(flag);
                    }
                }
            }

            return rids;
        }

        public static List<string> CreateVenueFeatureListfromFlag(string? venuefeaturefilter)
        {
            List<string> rids = new List<string>();

            if (!String.IsNullOrEmpty(venuefeaturefilter))
            {
                if (venuefeaturefilter != "null")
                {
                    var venuefeaturefilterint = Convert.ToInt32(venuefeaturefilter);

                    VenueFeatureFlag myflag = (VenueFeatureFlag)venuefeaturefilterint;

                    var myflags = myflag.GetFlags().GetDescriptionList();

                    foreach (var flag in myflags)
                    {
                        rids.Add(flag);
                    }
                }
            }

            return rids;
        }

        public static List<string> CreateVenueSeatTypeListfromFlag(string? venueseattypefilter)
        {
            List<string> rids = new List<string>();

            if (!String.IsNullOrEmpty(venueseattypefilter))
            {
                if (venueseattypefilter != "null")
                {
                    var venueseattypefilterint = Convert.ToInt32(venueseattypefilter);

                    VenueSeatTypeFlag myflag = (VenueSeatTypeFlag)venueseattypefilterint;

                    var myflags = myflag.GetFlags().GetDescriptionList();

                    foreach (var flag in myflags)
                    {
                        rids.Add(flag);
                    }
                }
            }

            return rids;
        }
    }
}
