// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OdhApiCore.Controllers
{
    public class NullStringParameterActionFilterAttribute : ActionFilterAttribute
    {
        public NullStringParameterActionFilterAttribute() { }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var key in context.ActionArguments.Keys.ToArray())
            {
                var value = context.ActionArguments[key];
                if (value as string == "null")
                {
                    context.ActionArguments[key] = null!;
                }
            }
            base.OnActionExecuting(context);
        }
    }
}
