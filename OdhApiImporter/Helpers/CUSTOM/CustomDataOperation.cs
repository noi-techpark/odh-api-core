using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdhApiImporter.Helpers.CUSTOM
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

        public void UpdateAllEventShortstonewDataModel()
        {
            //Load all data from PG and resave
            //var eventshorts = QueryFactory.
        }
    }
}
