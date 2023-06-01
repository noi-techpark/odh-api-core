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
    public struct ImportLog
    {
        public string sourceinterface { get; init; }
        public bool success { get; init; }
        public string sourceid { get; init; }
        public string error { get; init; }
    }
}
