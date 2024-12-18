// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Extensions
{
    public static class StringExtensions
    {
        //TODO EXTEND THIS

        public static string AddHttpsPrefixIfNotPresent(this string value)
        {
            if (value.StartsWith("http"))
                return value;
            else
                return "https://" + value;
        }
    }
}
