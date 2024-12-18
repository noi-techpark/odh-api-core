// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Helper.Generic
{
    public static class PathStringExtensions
    {
        public static string GetPathNextTo(
            this PathString pathString,
            string splitter,
            string nextto
        )
        {
            if (pathString != null && pathString.HasValue)
            {
                var splitted = pathString.Value.Split(splitter);
                bool next = false;

                foreach (var item in splitted)
                {
                    if (next)
                        return item;

                    if (item == nextto)
                        next = true;
                }
            }

            return "";
        }
    }
}
