// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Xml.Linq;
using Helper.GetData;

namespace A22
{
    public class GetA22Data
    {
        public static async Task<XDocument> GetWebcams(string url, string user, string pass)
        {
            GetData getdata = new GetData(
                url + "/GetWebCam",
                user,
                pass,
                null,
                GetDataAuthenticationOptions.Basic
            );

            return await getdata.GetDataAsXmlAsync();
        }

        public static async Task<XDocument> GetServiceAreas(string url, string user, string pass)
        {
            GetData getdata = new GetData(
                url + "/GetAreeDiServizio",
                user,
                pass,
                null,
                GetDataAuthenticationOptions.Basic
            );

            return await getdata.GetDataAsXmlAsync();
        }

        public static async Task<XDocument> GetTollStations(string url, string user, string pass)
        {
            GetData getdata = new GetData(
                url + "/GetCaselli",
                user,
                pass,
                null,
                GetDataAuthenticationOptions.Basic
            );

            return await getdata.GetDataAsXmlAsync();
        }

        public static async Task<XDocument> GetCoordinates(string url, string user, string pass)
        {
            GetData getdata = new GetData(
                url + "/GetCoordinate",
                user,
                pass,
                null,
                GetDataAuthenticationOptions.Basic
            );

            return await getdata.GetDataAsXmlAsync();
        }
    }
}
