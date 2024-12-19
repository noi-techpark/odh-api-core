// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace DataModel
{
    public class AlpineBitsInventoryBasic
    {
        public string AlpineBitsVersion { get; set; }
        public string AccommodationId { get; set; }
        public string RequestId { get; set; }

        public string Source { get; set; }
        public JRaw Message { get; set; }

        //Internal Use
        public string MessageType { get; set; }
        public DateTime RequestDate { get; set; }
        public string AccommodationLTSId { get; set; }
    }
}
