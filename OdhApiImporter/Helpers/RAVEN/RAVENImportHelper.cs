using DataModel;
using Microsoft.AspNetCore.Mvc;
using RAVEN;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiImporter.Helpers.RAVEN
{
    public class RAVENImportHelper
    {
        private readonly QueryFactory QueryFactory;
        private readonly ISettings settings;

        public RAVENImportHelper(ISettings settings, QueryFactory queryfactory)
        {
            this.QueryFactory = queryfactory;
            this.settings = settings;
        }


    }
}
