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
            webcam.Active = true;
            webcam.WebcamId = webcam.WebcamId = webcamtoparse.Attribute("panid").Value;

            //Parse the Feratel XML

            //GPS Info Camera
            GpsInfo gpsinfo = new GpsInfo() { Gpstype = "position" };
            gpsinfo.Altitude = webcamtoparse.Attribute("h") != null ? Convert.ToDouble(webcamtoparse.Attribute("h").Value) : 0;
            gpsinfo.Latitude = webcamtoparse.Attribute("x") != null ? Convert.ToDouble(webcamtoparse.Attribute("x").Value, myculture) : 0;
            gpsinfo.Longitude = webcamtoparse.Attribute("y") != null ? Convert.ToDouble(webcamtoparse.Attribute("y").Value, myculture) : 0;
            gpsinfo.AltitudeUnitofMeasure = "m";
            webcam.GpsInfo = new List<GpsInfo>() { gpsinfo };

            //ContactInfo
            ContactInfos contactinfo = new ContactInfos();
            contactinfo.Language = "de";
            contactinfo.ZipCode = linktoparse.Element("location").Attribute("zip") != null ? linktoparse.Element("location").Attribute("zip").Value : "";
            contactinfo.City = linktoparse.Element("location") != null ? linktoparse.Element("location").Value : "";
            contactinfo.Area = linktoparse.Element("village") != null ? linktoparse.Element("village").Value : "";
            contactinfo.Region = linktoparse.Element("region") != null ? linktoparse.Element("region").Value : "";
            contactinfo.CountryCode = linktoparse.Element("country").Attribute("ioc") != null ? linktoparse.Element("country").Attribute("ioc").Value : "";
            contactinfo.CountryName = linktoparse.Element("country").Value != null ? linktoparse.Element("country").Value : "";

            //Detail            
            Detail detail = new Detail();
            detail.Title = webcamtoparse.Attribute("l").Value;
            detail.Language = "de";

            if(!String.IsNullOrEmpty(webcamtoparse.Element("keywords").Value))
            {
                var keywords = linktoparse.Element("keywords").Value.Split(',');
                detail.Keywords = new List<string>();
                foreach (var keyword in keywords)
                {
                    detail.Keywords.Add(keyword.Trim());
                }
            }


             webcam.Detail.TryAddOrUpdate(detail.Language, detail);

            //WebcamProperties

            //url types (MediaPlayer Thumbnails, MediaPlayer Thumbnails 38, MediaPlayer v4, MediaPlayer v4 360, MediaPlayer Thumbnail 360, feratel.com)


            //Mapping
            webcam.Mapping.TryAddOrUpdate("feratel", new Dictionary<string, string>() { { "link_id", linktoparse.Attribute("id").Value }, { "panid", webcamtoparse.Attribute("panid").Value } });


            return webcam;
        }
    }
}
