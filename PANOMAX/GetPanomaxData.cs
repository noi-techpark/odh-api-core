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
            string requesturl = url + "instances/lists/public";

            GetData getdata = new GetData(requesturl, null, null, null, GetDataAuthenticationOptions.None);

            return await getdata.GetDataAsJsonAsync();
        }

        public static async Task<dynamic?> GetVideos(string url)
        {
            string requesturl = url + "cams/videos/public";

            GetData getdata = new GetData(requesturl, null, null, null, GetDataAuthenticationOptions.None);

            return await getdata.GetDataAsJsonAsync();
        }

    }
}