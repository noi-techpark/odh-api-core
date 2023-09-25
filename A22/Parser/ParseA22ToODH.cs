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

        public static ODHActivityPoiLinked ParseServiceStationToODHActivityPoi(ODHActivityPoiLinked? poi, XElement poitoparse, string odhid)
        {
            //<WSOpenData_AreaDiServizio>
            //<ID>34</ID>
            //<Direzione>Sud</Direzione>
            //<Titolo>Brennero (ex dogana)</Titolo>
            //<Descrizione>Area esterna ad A22. Accessibille da entrame le carreggiate.</Descrizione>
            //<Gestori/>
            //<Distanza>1.300</Distanza>
            //<Offset>-1</Offset>
            //<MezziPesanti>true</MezziPesanti>
            //<ISAreaDiServizio>false</ISAreaDiServizio>
            //<ColonnineTesla>0</ColonnineTesla>
            //<ChargerMultiStandard>0</ChargerMultiStandard>
            //<ChargerAC>0</ChargerAC>
            //<ChargerUF>0</ChargerUF>
            //<Servizi>
            //<WSOpenData_ServizioAreaDiServizio>
            //<IDServizio>28</IDServizio>
            //<Titolo>Servizi igienici</Titolo>
            //<Immagine>https://example/icona-WC.png</Immagine>
            //<Categoria>a</Categoria>
            //</WSOpenData_ServizioAreaDiServizio>
            //<WSOpenData_ServizioAreaDiServizio>
            //<IDServizio>27</IDServizio>
            //<Titolo>Sosta mezzi pesanti</Titolo>
            //<Immagine>https://example/icona-Pcamion.png</Immagine>
            //<Categoria>p</Categoria>
            //</WSOpenData_ServizioAreaDiServizio>
            //</Servizi>
            //</WSOpenData_AreaDiServizio>

            if (poi == null)
                poi = new ODHActivityPoiLinked();

            poi.Source = "a22";
            poi.Id = odhid;
            poi.Active = true;

            //Detail
        
            //Imagegallery

            //ContactInfos

            //Mapping
            
            //LicenseInfo

            //GPSInfo

            return poi;
        }
    }
}
