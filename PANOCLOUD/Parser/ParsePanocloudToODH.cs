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
            var z = webcamtoparse["@attributes"]["cameraStatus"];

            if ((string)webcamtoparse["@attributes"]["cameraStatus"] == "active")
                webcam.Active = true;
            else
                webcam.Active = false;
        
            if ((string)webcamtoparse["@attributes"]["full360"] == "yes")
                webcam.WebCamProperties.ViewAngleDegree = "360";
            else
                webcam.WebCamProperties.ViewAngleDegree = "";

            if ((string)webcamtoparse["@attributes"]["hasVR"] == "yes")
                webcam.WebCamProperties.HasVR = true;
            else
                webcam.WebCamProperties.HasVR = false;

            webcam.WebCamProperties.ViewerType = webcamtoparse["@attributes"]["viewerType"];            

            webcam.LastChange = Convert.ToDateTime(webcamtoparse["@attributes"]["lastModified"]);
            webcam.Shortname = webcamtoparse["@attributes"]["name"];

            var defaultlanguage = webcamtoparse["@attributes"]["defaultLang"];

            //Detail
            Detail detail = new Detail();
            detail.Title = webcamtoparse["@attributes"]["name"];
            detail.IntroText = webcamtoparse["@attributes"]["description"];


            //throws exception
            detail.BaseText = !String.IsNullOrEmpty(webcamtoparse["@attributes"]["longdescription"]) ? webcamtoparse["@attributes"]["longdescription"] : webcamtoparse["@attributes"]["description"];

            detail.AdditionalText = webcamtoparse["urlConfig"]["example"];

            detail.Language = defaultlanguage;

            webcam.Detail.TryAddOrUpdate(detail.Language, detail);

            //ContactInfos
            ContactInfos contactinfo = new ContactInfos();
            contactinfo.CompanyName = !String.IsNullOrEmpty(webcamtoparse["@attributes"]["pageTitle"]) ? webcamtoparse["@attributes"]["pageTitle"] : webcamtoparse["@attributes"]["name"];
            contactinfo.CountryCode = webcamtoparse["@attributes"]["addressIso"] != null ? ((string)webcamtoparse["@attributes"]["addressIso"]).ToUpper() : "";
            contactinfo.CountryName = contactinfo.CountryCode == "IT" ? "Italien" : "";
            contactinfo.ZipCode = webcamtoparse["@attributes"]["addressZip"];
            contactinfo.Address = webcamtoparse["@attributes"]["addressStreet"];
            contactinfo.City = webcamtoparse["@attributes"]["geoPlacename"];
            contactinfo.Region = webcamtoparse["@attributes"]["geoRegion"];
            contactinfo.Url = webcamtoparse["@attributes"]["url"];            
            contactinfo.Language = defaultlanguage;
            contactinfo.LogoUrl = webcamtoparse["Logos"]["0"]["@attributes"]["logoUrl"];

            webcam.ContactInfos.TryAddOrUpdate(contactinfo.Language, contactinfo);

            //GPSInfo
            GpsInfo gpsinfo = new GpsInfo();
            gpsinfo.Gpstype = "position";
            gpsinfo.Latitude = webcamtoparse["@attributes"]["geoLat"] != null ? Convert.ToDouble(webcamtoparse["@attributes"]["geoLat"]) : 0;
            gpsinfo.Longitude = webcamtoparse["@attributes"]["geoLong"] != null ? Convert.ToDouble(webcamtoparse["@attributes"]["geoLong"]) : 0;
            gpsinfo.Altitude = webcamtoparse["@attributes"]["geoAlt"] != null ? Convert.ToDouble(webcamtoparse["@attributes"]["geoAlt"]) : 0;
            gpsinfo.AltitudeUnitofMeasure = "m";
            webcam.GpsInfo = new List<GpsInfo>() { gpsinfo };

            //Images
            webcam.ImageGallery = new List<ImageGallery>();
            foreach(var imagetoparse in webcamtoparse.Images.image)
            {
                ImageGallery image = new ImageGallery();

                image.Width = (int)imagetoparse["@attributes"]["imgWidth"];
                image.Height = (int)imagetoparse["@attributes"]["imgHeight"];
                image.ImageName = (string)imagetoparse["@attributes"]["fileName"];
                image.ImageUrl = (string)imagetoparse["@attributes"]["fileUrl"];
                image.ImageSource = "panocloud";                
                image.IsInGallery = true;
                //"panorama": "yes",
                //"fileType": "big",
                //"fileTimeUnix": "1691568228",
                //"fileTime": "2023-08-09T08:03:48+00:00"
                //"mimeType": "image/jpeg",

                webcam.ImageGallery.Add(image);
            }

            //Videos
            foreach (var videotoparse in webcamtoparse.Videos)
            {

            }

            //Mapping
            webcam.Mapping.TryAddOrUpdate("panomax", new Dictionary<string, string>() { { "id", (string)webcamtoparse.id }, { "locationId", (string)webcamtoparse["@attributes"]["locationId"] } });


            return webcam;
        }
    }
}
