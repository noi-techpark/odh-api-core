//// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
////
//// SPDX-License-Identifier: AGPL-3.0-or-later

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using System.Collections.Generic;
//using System.Linq;

//namespace OdhApiCore.Controllers
//{
//    public class OdhAuthorizeAttribute : TypeFilterAttribute
//    {
//        public OdhAuthorizeAttribute(string roles) : base(typeof(OdhAuthorizeFilter))
//        {
//            Arguments = new object[] { roles } ;
//        }
//    }

//    public class OdhAuthorizeFilter : IAuthorizationFilter
//    {
//        readonly List<string> _roles;

//        public OdhAuthorizeFilter(string roles)
//        {
//            _roles = roles.Split(',').ToList();
//        }

//        public void OnAuthorization(AuthorizationFilterContext context)
//        {
//            bool allowed = false;

//            if (context.HttpContext.User.Identity != null && context.HttpContext.User.Identity.IsAuthenticated)
//            {
//                foreach (var role in _roles)
//                {
//                    if (context.HttpContext.User.IsInRole(role))
//                        allowed = true;
//                }

//                if (!allowed)
//                {
//                    context.Result = new ForbidResult();
//                }
//            }
//            else
//                context.Result = new UnauthorizedResult();

//            //TODO, if Token is invalid POST not workign anymore?
//        }
//    }
//}
