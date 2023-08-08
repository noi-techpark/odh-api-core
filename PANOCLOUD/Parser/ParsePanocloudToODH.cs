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

namespace PANOCLOUD
{
    public class ParsePanocloudToODH
    {
        public static WebcamInfoLinked ParseWebcamToWebcamInfo(WebcamInfoLinked webcam, dynamic webcamtoparse, string odhid)
        {
            if (webcam == null)
                webcam = new WebcamInfoLinked();

            webcam.Source = "panocloud";
            webcam.Id = odhid; //no id in panocloud

            //TODO Parse the Panocloud Json

            if ((string)webcamtoparse.cameraStatus == "active")
                webcam.Active = true;
            else
                webcam.Active = false;

            if ((string)webcamtoparse.full360 == "yes")
                webcam.WebCamProperties.ViewAngleDegree = "360";
            else
                webcam.WebCamProperties.ViewAngleDegree = "";

            if ((string)webcamtoparse.hasVR == "yes")
                webcam.WebCamProperties.HasVR = true;
            else
                webcam.WebCamProperties.HasVR = false;



            return webcam;
        }
    }
}
