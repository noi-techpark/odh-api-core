// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class FCMModels
    {
        public string to { get; set; }
        public FCMNotification notification { get; set; }
        public dynamic data { get; set; }
    }    

    public class FCMNotification
    {
        public string? title { get; set; }
        public string? body { get; set; }
        //public string? sound { get; set; }
        public string? link { get; set; }
        public string? icon { get; set; }
    }

    public class FCMessageV2
    {
        public FCMessageBodyV2 message { get; set; }        
    }

    public class FCMessageBodyV2
    {
        public string topic { get; set; }
        public FCMNotification notification { get; set; }
        public dynamic data { get; set; }
    }
}
