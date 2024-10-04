// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

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
