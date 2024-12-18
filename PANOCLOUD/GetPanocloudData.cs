// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Helper.GetData;

namespace PANOCLOUD
{
    public class GetPanocloudData
    {
        public static async Task<dynamic?> GetWebcams(string url)
        {
            GetData getdata = new GetData(url, null, null, null, GetDataAuthenticationOptions.None);

            return await getdata.GetDataAsJsonAsync();
        }
    }
}
