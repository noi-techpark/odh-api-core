// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helper;
using Microsoft.FSharp.Control;
using System.Diagnostics;
using MongoDB.Driver;
using OdhNotifier;

namespace OdhApiImporter.Helpers
{
    /// <summary>
    /// This class is used for different update operations on the data
    /// </summary>
    public class PushDataOperation
    {
        private readonly QueryFactory QueryFactory;
        private readonly ISettings settings;
        private IOdhPushNotifier OdhPushnotifier;

        public PushDataOperation(ISettings settings, QueryFactory queryfactory, IOdhPushNotifier odhpushnotifier)
        {
            this.QueryFactory = queryfactory;
            this.settings = settings;
            this.OdhPushnotifier = odhpushnotifier;
        }

        public async Task<IDictionary<string, IDictionary<string, NotifierResponse>>> PushAllODHActivityPoiwithTags(string datatype, List<string> idlist, List<string> taglist)
        {
            var pushresultlist = new Dictionary<string, IDictionary<string, NotifierResponse>>();

            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From(datatype)
                   .When(idlist.Count > 0, q => q.IdIlikeFilter(idlist))
                   .When(taglist.Count > 0, q => q.SmgTagFilterOr_GeneratedColumn(taglist));

            ODHTypeHelper.TranslateTypeString2Type(datatype);

            var datalist = await query.GetObjectListAsync<GenericODHData>();
           
            foreach (var data in datalist)
            {
                if (data.PublishedOn != null && data.PublishedOn.Contains("idm-marketplace"))
                {
                    var pushresults = await OdhPushnotifier.PushToPublishedOnServices(data.Id, "odhactivitypoi", "forced", false, false, "api", new List<string>() { "idm-marketplace" });
                    pushresultlist.Add(data.Id, pushresults);
                }
            }

            return pushresultlist;
        }
    }

}
