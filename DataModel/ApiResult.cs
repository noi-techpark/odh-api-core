// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
    public class GenericResult
    {
        public string Message { get; set; }
    }

    public class GenericResultExtended : GenericResult
    {
        public string? Id { get; set; }
        //public object Data { get; set; }
    }

    public class UnauthorizedResult
    {
        public string Message { get; set; }
    }
}
