using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.helper
{
    public class PermissionsHelper
    {
        public static bool CheckOpenData(IPrincipal currentuser)
        {
            List<string> roles = new List<string>() { "DataReader", "PoiReader" };

            return roles.Any(x => currentuser.IsInRole(x));
        }
    }
}
