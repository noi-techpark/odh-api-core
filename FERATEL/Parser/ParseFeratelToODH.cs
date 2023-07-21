// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FERATEL
{
    public class ParseFeratelToODH
    {
        public static WebcamInfoLinked ParseWebcamToWebcamInfo(WebcamInfoLinked webcam, XDocument webcamtoparse)
        {
            if (webcam == null)
                webcam = new WebcamInfoLinked();

            //TODO Parse the Feratel XML

            return webcam;
        }
    }
}
