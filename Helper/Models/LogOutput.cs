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
    public struct LogOutput<T>
    {
        public string log { get; init; }
        public string type { get; init; }
        public string id { get; init; }
        public T output { get; init; }
    }    
}
