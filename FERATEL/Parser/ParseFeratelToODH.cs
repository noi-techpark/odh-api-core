// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FERATEL
{
    public class ParseFeratelToODH
    {
        public static CultureInfo myculture = new CultureInfo("en");

        public static WebcamInfoLinked ParseWebcamToWebcamInfo(WebcamInfoLinked? webcam, XElement webcamtoparse, XElement linktoparse, string odhid)
        {
            if (webcam == null)
                webcam = new WebcamInfoLinked();

            webcam.Source = "feratel";
            webcam.Id = odhid;

            //Parse the Feratel XML

            //GPS Info Camera
            GpsInfo gpsinfo = new GpsInfo() { Gpstype = "position" };
            gpsinfo.Altitude = webcamtoparse.Attribute("h") != null ? Convert.ToDouble(webcamtoparse.Attribute("h").Value) : 0;
            gpsinfo.Latitude = webcamtoparse.Attribute("x") != null ? Convert.ToDouble(webcamtoparse.Attribute("x").Value, myculture) : 0;
            gpsinfo.Longitude = webcamtoparse.Attribute("y") != null ? Convert.ToDouble(webcamtoparse.Attribute("y").Value, myculture) : 0;
            gpsinfo.AltitudeUnitofMeasure = "m";
            webcam.GpsInfo = new List<GpsInfo>() { gpsinfo };

            //ContactInfo


            //Detail            
            Detail detail = new Detail();
            detail.Title = webcamtoparse.Attribute("l").Value;
            detail.Language = "de";
            webcam.Detail.TryAddOrUpdate(detail.Language, detail);

            //WebcamProperties


            //Mapping
            webcam.Mapping.TryAddOrUpdate("feratel", new Dictionary<string, string>() { { "link_id", webcamtoparse.Attribute("id").Value }, { "panid", webcamtoparse.Attribute("panid").Value } });


            return webcam;
        }
    }
}
