using DataModel;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helper;

namespace OdhApiImporter.Helpers
{
    public class CustomDataOperation
    {
        private readonly QueryFactory QueryFactory;
        private readonly ISettings settings;

        public CustomDataOperation(ISettings settings, QueryFactory queryfactory)
        {
            this.QueryFactory = queryfactory;
            this.settings = settings;
        }

        public async Task<int> UpdateAllEventShortstonewDataModel()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("eventeuracnoi");

            var data = await query.GetObjectListAsync<EventShort>();
            int i = 0;

            foreach(var eventshort in data)
            {
                //Save tp DB
                //TODO CHECK IF THIS WORKS     
                var queryresult = await QueryFactory.Query("eventeuracnoi").Where("id", eventshort.Id)
                    //.UpdateAsync(new JsonBData() { id = eventshort.Id.ToLower(), data = new JsonRaw(eventshort) });
                    .UpdateAsync(new JsonBDataRaw() { id = eventshort.Id?.ToLower() ?? "", data = new JsonRaw(eventshort) });

                i++;
            }

            return i;
        }
    }
}
