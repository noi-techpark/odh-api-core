// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataModel;
using Helper;
using SqlKata.Execution;

namespace OdhApiImporter.Helpers.LTSCDB
{
    public class LTSCDBEventImportHelper : ImportHelper, IImportHelper
    {
        public LTSCDBEventImportHelper(
            ISettings settings,
            QueryFactory queryfactory,
            string table,
            string importerURL
        )
            : base(settings, queryfactory, table, importerURL) { }

        public async Task<UpdateDetail> SaveDataToODH(
            DateTime? lastchanged = null,
            List<string>? idlist = null,
            CancellationToken cancellationToken = default
        )
        {
            //Import the List

            //Import Single Data

            //Deactivate Data

            throw new NotImplementedException();
        }
    }
}
