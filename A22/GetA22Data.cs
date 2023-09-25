// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Helper.GetData;
using System.Xml.Linq;

namespace A22
{
    public class GetA22Data
    {
        public static async Task<XDocument> GetWebcams(string url, string user, string pass)
        {
            GetData getdata = new GetData(url, user, pass, null, GetDataAuthenticationOptions.Basic);

            return await getdata.GetDataAsXmlAsync();
        }

        public static async Task<XDocument> GetServiceAreas(string url, string user, string pass)
        {
            GetData getdata = new GetData(url, user, pass, null, GetDataAuthenticationOptions.Basic);

            return await getdata.GetDataAsXmlAsync();
        }

        public static async Task<XDocument> GetTollStations(string url, string user, string pass)
        {
            GetData getdata = new GetData(url, user, pass, null, GetDataAuthenticationOptions.Basic);

            return await getdata.GetDataAsXmlAsync();
        }

    }
}