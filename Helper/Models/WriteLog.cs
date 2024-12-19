// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Helper
{
    public static class WriteLog
    {
        /// <summary>
        /// Writes Log to Console
        /// </summary>
        /// <typeparam name="T">HttpRequestLog / ImportLog ...</typeparam>
        /// <param name="id"></param>
        /// <param name="type">Type of the Log (HttpRequest, ImportData etc...)</param>
        /// <param name="log">Additional Info (apiaccess, dataimport)</param>
        /// <param name="output">HttpRequestLog / ImportLog</param>
        public static void LogToConsole<T>(string id, string type, string log, T output)
        {
            LogOutput<T> logoutput = new LogOutput<T>()
            {
                id = id,
                type = type,
                log = log,
                output = output,
            };
            Console.WriteLine(JsonConvert.SerializeObject(logoutput));
        }
    }
}
