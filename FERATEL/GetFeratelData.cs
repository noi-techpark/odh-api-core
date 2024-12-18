// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Xml.Linq;
using Helper.GetData;

namespace FERATEL
{
    public class GetFeratelData
    {
        public static async Task<XDocument> GetWebcams(string url)
        {
            GetData getdata = new GetData(url, null, null, null, GetDataAuthenticationOptions.None);

            return await getdata.GetDataAsXmlAsync();
        }
    }
}
