using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Identity
{
    public enum PermissionAction
    {
        Read,
        Create,
        Update,
        Delete
    }

    public class AuthorizeODHAttribute : TypeFilterAttribute
    {
        public AuthorizeODHAttribute(PermissionAction action)
        : base(typeof(AuthorizeODHActionFilter))
        {
            Arguments = new object[] { action };
        }
    }

    public class AuthorizeODHActionFilter : IAuthorizationFilter
    {        
        private readonly PermissionAction _action;
        public AuthorizeODHActionFilter(PermissionAction action)
        {            
            _action = action;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool isAuthorized = CheckAccess(context.HttpContext.User, _action, context.HttpContext.Request.Path); // :)

            if (!isAuthorized)
            {
                context.Result = new ForbidResult();
            }
        }

        private bool CheckAccess(ClaimsPrincipal User, PermissionAction action, string endpoint)
        {
            var lastendpointelement = endpoint.Split('/').Last();

            return User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value.StartsWith(lastendpointelement + "_" + action.ToString()));
        }
    }    
}
