// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiImporter.Helpers
{
    public class A22ImportHelper : ImportHelper, IImportHelper
    {
        //TODO Make BaseUrl configurable in settings
        public const string serviceurl = @"";

        public A22ImportHelper(ISettings settings, QueryFactory queryfactory, string table, string importerURL) : base(settings, queryfactory, table, importerURL)
        {

        }

        public Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, List<string>? idlist = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
