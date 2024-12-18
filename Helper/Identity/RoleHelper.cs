// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                    JObject obj = JObject.Parse(
                        claimsIdentity.Claims.First(c => c.Type == "resource_access").Value
                    );
                    var roleAccess = obj.GetValue(client)!.ToObject<JObject>()!.GetValue("roles");
                    foreach (JToken role in roleAccess!)
                    {
                        //add only if role is not present
                        if (!principal.IsInRole(role.ToString()))
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.ToString()));
                    }

                    ////Get Roles from resource_access
                    //if (claimsIdentity.Claims.Where(x => x.Type == "resource_access").FirstOrDefault() != null)
                    //{
                    //    var resourceroles = JsonConvert.DeserializeObject<Dictionary<string, Resource_Roles>>(claimsIdentity.Claims.Where(x => x.Type == "resource_access").FirstOrDefault().Value);

                    //    if(resourceroles != null && resourceroles.Where(x => x.Key == client).Count() > 0)
                    //    {
                    //        foreach (var resourcerole in resourceroles.Where(x => x.Key == client).FirstOrDefault().Value.roles)
                    //        {
                    //            //add only if role is not present
                    //            if (!principal.IsInRole(resourcerole))
                    //                claimsIdentity.AddClaim(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", resourcerole));
                    //        }
                    //    }
                    //}
                }
            }
        }
    }

    public class Resource_Roles
    {
        public List<string> roles { get; set; }
    }
}
