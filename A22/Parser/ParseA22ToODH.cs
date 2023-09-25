// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace A22
{
    public class ParseA22ToODH
    {
        public static CultureInfo myculture = new CultureInfo("en");

        public static WebcamInfoLinked ParseWebcamToWebcamInfo(WebcamInfoLinked? webcam, XElement webcamtoparse, string odhid)
        {
            if (webcam == null)
                webcam = new WebcamInfoLinked();

            webcam.Source = "feratel";
            webcam.Id = odhid;
            webcam.Active = true;


            //Mapping
            //webcam.Mapping.TryAddOrUpdate("a22", new Dictionary<string, string>() { { "link_id", linktoparse.Attribute("id").Value }, { "panid", webcamtoparse.Attribute("panid").Value } });

            //LicenseInfo


            return webcam;
        }
    }
}
