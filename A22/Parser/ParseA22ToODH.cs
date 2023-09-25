// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper;
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

            webcam.Source = "a22";
            webcam.Id = odhid;
            webcam.Active = true;

            //Detail
            Detail detail = new Detail();
            detail.Language = "it";
            detail.Title = webcamtoparse.Attribute("Titolo").Value;
            detail.BaseText = webcamtoparse.Attribute("Descrizione").Value;

            webcam.Detail.TryAddOrUpdate(detail.Language, detail);

            webcam.HasLanguage = new List<string>() { "it" };

            //Imagegallery

            ImageGallery imagegallery = new ImageGallery();
            imagegallery.ImageUrl = webcamtoparse.Attribute("Immagine").Value;
            imagegallery.ImageSource = "a22";
            imagegallery.ImageName = webcamtoparse.Attribute("Titolo").Value;

            webcam.ImageGallery = new List<ImageGallery>() { imagegallery };

            //ContactInfos

            //Webcamproperties
            WebcamProperties webcamproperties = new WebcamProperties();
            webcamproperties.WebcamUrl = webcamtoparse.Attribute("Immagine").Value;

            webcam.WebCamProperties = webcamproperties;            

            //Mapping
            webcam.Mapping.TryAddOrUpdate("a22", new Dictionary<string, string>() { { "km", webcamtoparse.Attribute("KM").Value } });

            //LicenseInfo


            return webcam;
        }
    }
}
