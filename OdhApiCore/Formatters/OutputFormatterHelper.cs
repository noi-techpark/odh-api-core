// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Threading.Tasks;

namespace OdhApiCore.Formatters
{
    public class OutputFormatterHelper
    {
        public static Task BadRequest(OutputFormatterWriteContext context)
        {
            context.HttpContext.Response.StatusCode = 400;
            return context.HttpContext.Response.WriteAsync("Bad Request");
        }

        public static Task NotImplemented(OutputFormatterWriteContext context)
        {
            context.HttpContext.Response.StatusCode = 501;
            return context.HttpContext.Response.WriteAsync("Not implemented");
        }

        public static Task InternalServerError(OutputFormatterWriteContext context)
        {
            context.HttpContext.Response.StatusCode = 501;
            return context.HttpContext.Response.WriteAsync("Internal Server Error");
        }
    }
}
