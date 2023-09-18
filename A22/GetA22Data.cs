// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Helper.GetData;

namespace A22
{
    public class GetA22Data
    {
        public static async Task<dynamic?> GetWebcams(string url, string user, string pass)
        {
            GetData getdata = new GetData(url, user, pass, null, GetDataAuthenticationOptions.Basic);

            return await getdata.GetDataAsXmlAsync();
        }

        public static async Task<dynamic?> GetServiceAreas(string url, string user, string pass)
        {
            GetData getdata = new GetData(url, user, pass, null, GetDataAuthenticationOptions.Basic);

            return await getdata.GetDataAsXmlAsync();
        }

        public static async Task<dynamic?> GetTollStations(string url, string user, string pass)
        {
            GetData getdata = new GetData(url, user, pass, null, GetDataAuthenticationOptions.Basic);

            return await getdata.GetDataAsXmlAsync();
        }

    }
}