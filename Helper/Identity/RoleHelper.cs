// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Identity
{
    public class RoleHelper
    {
        public static void AddRoleClaims(ClaimsPrincipal? principal, string client)
        {
            if (principal != null)
            {
                var claimsIdentity = principal.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    //Get Roles from resource_access 
                    if (claimsIdentity.Claims.Where(x => x.Type == "resource_access").FirstOrDefault() != null)
                    {
                        var resourceroles = JsonConvert.DeserializeObject<Dictionary<string, Resource_Roles>>(claimsIdentity.Claims.Where(x => x.Type == "resource_access").FirstOrDefault().Value);
                        
                        if(resourceroles != null && resourceroles.Where(x => x.Key == client).Count() > 0)
                        {
                            foreach (var resourcerole in resourceroles.Where(x => x.Key == client).FirstOrDefault().Value.roles)
                            {
                                //add only if role is not present
                                if (!principal.IsInRole(resourcerole))
                                    claimsIdentity.AddClaim(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", resourcerole));
                            }
                        }
                    }
                }
            }
        }

        public class Resource_Roles
        {
            public List<string> roles { get; set; }
        }

    }

}
