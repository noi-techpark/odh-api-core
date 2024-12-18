// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Helper
{
    public static class RemoteIpHelper
    {
        public static string? GetRequestIP(HttpContext context, bool tryUseXForwardHeader = true)
        {
            string? ip = null;

            // X-Forwarded-For (csv list):  Using the First entry in the list seems to work
            // for 99% of cases however it has been suggested that a better (although tedious)
            // approach might be to read each IP from right to left and use the first public IP.
            // http://stackoverflow.com/a/43554000/538763
            //
            if (tryUseXForwardHeader)
                ip = GetHeaderValueAs<string>("X-Forwarded-For", context)
                    ?.SplitCsv()
                    ?.FirstOrDefault();

            // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
            if (ip.IsNullOrWhitespace() && context?.Connection?.RemoteIpAddress != null)
                ip = context.Connection.RemoteIpAddress.ToString();

            if (ip.IsNullOrWhitespace() && context != null)
                ip = GetHeaderValueAs<string>("REMOTE_ADDR", context);

            // _httpContextAccessor.HttpContext?.Request?.Host this is the local host.

            //Tests are not running with this exception
            //if (ip.IsNullOrWhitespace())
            //    throw new Exception("Unable to determine caller's IP.");

            if (ip.IsNullOrWhitespace())
                return "";

            return ip;
        }

        public static T? GetHeaderValueAs<T>(string headerName, HttpContext context)
        {
            StringValues values = new StringValues();

            if (context?.Request?.Headers?.TryGetValue(headerName, out values) ?? false)
            {
                string rawValues = values.ToString(); // writes out as Csv when there are multiple.

                if (!rawValues.IsNullOrWhitespace())
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }
            return default(T);
        }

        private static List<string>? SplitCsv(
            this string csvList,
            bool nullOrWhitespaceInputReturnsNull = false
        )
        {
            if (string.IsNullOrWhiteSpace(csvList))
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

            return csvList
                .TrimEnd(',')
                .Split(',')
                .AsEnumerable<string>()
                .Select(s => s.Trim())
                .ToList();
        }

        private static bool IsNullOrWhitespace(this string? s)
        {
            return String.IsNullOrWhiteSpace(s);
        }
    }
}
