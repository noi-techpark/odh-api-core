// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Helper.GetData;
using System.Net;
using System.Xml.Linq;

namespace PANOMAX
{
    public class GetPanomaxData
    {
        public static async Task<dynamic?> GetWebcams(string url)
        {
            GetData getdata = new GetData(url, null, null, null, GetDataAuthenticationOptions.None);

            return await getdata.GetDataAsJsonAsync();
        }

        public static async Task<dynamic?> GetVideos(string url)
        {
            GetData getdata = new GetData(url, null, null, null, GetDataAuthenticationOptions.None);

            return await getdata.GetDataAsJsonAsync();
        }

    }
}