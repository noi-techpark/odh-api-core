// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ServiceReferenceCDBData;

namespace CDB
{
    public class GetEventDataCDB
    {
        public static CDBDataSoapClient.EndpointConfiguration GetEndpointConfig()
        {
            //TODO CHECK IF THIS WORKS
            CDBDataSoapClient.EndpointConfiguration myconfig = new CDBDataSoapClient.EndpointConfiguration();

            return myconfig;
        }

        //Event Data
        public static XDocument GetEventDatafromCDB(string EvRIDList, string WithOrganizer, string LngID, string PublRIDList, string WithCdays, string ltsuser, string ltspswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = ltsuser;
            proxy.ClientCredentials.UserName.Password = ltspswd;

            string sinput = "<SendData UserID='" + ltsuser + "' SessionID='" + ltspswd + "' Function='GetEveVData' Version='1.0' EvRID='" + EvRIDList + "' WithOrganizer='" + WithOrganizer + "' LngID='" + LngID + "' PublRID='" + PublRIDList + "' WithCDays='" + WithCdays + "' />";

            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Event Organizer Data
        public static XDocument GetEventOrganizerDatafromCDB(string OrgRIDList, string LngID, string ltsuser, string ltspswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = ltsuser;
            proxy.ClientCredentials.UserName.Password = ltspswd;

            string sinput = "<SendData UserID='" + ltsuser + "' SessionID='" + ltspswd + "' Function='GetEveVOrganizerData' Version='1.0' OrgRID='" + OrgRIDList + "' LngID='" + LngID + "' />";

            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Event Seek Engine
        public static XDocument GetEventSeekEngine(string PublRIDList, string DateFrom, string DateTo, string TopRIDList, string Ranc, string FraRIDList, string Seekword, string LngId, string PageAct, string PageSize, string OrgRIDList, string titlelist, string SeekID, string OnlyEveHead, string ShowSDat, string Zone, string WithCDays, string CompatiblityMode, string ltsuser, string ltspswd, string serviceurl)
        {            
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = ltsuser;
            proxy.ClientCredentials.UserName.Password = ltspswd;

            string sinput = "<SendData UserID='" + ltsuser + "' SessionID='" + ltspswd + "' Function='GetEveVSeekEngine' Version='1.0' PublRID='" + PublRIDList + "' DateFrom='" + DateFrom + "' DateTo='" + DateTo + "' TopRID='" + TopRIDList + "' Ranc='" + Ranc + "' FraRID='" + FraRIDList + "' SeekWord='" + Seekword + "' LngID='" + LngId + "' PageAct='" + PageAct + "' PageSize='" + PageSize + "' OrgRID='" + OrgRIDList + "' Title='" + titlelist + "' SeekID='" + SeekID + "' OnlyEveHead='" + OnlyEveHead + "' ShowSDat='" + ShowSDat + "' Zone='" + Zone + "' WithCDays='" + WithCDays + "' />";

            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Event Publisher RIDS Data
        public static XDocument GetEventAllPublisherfromCDB(string LngID, string ltsuser, string ltspswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = ltsuser;
            proxy.ClientCredentials.UserName.Password = ltspswd;

            string sinput = "<SendData UserID='" + ltsuser + "' SessionID='" + ltspswd + "' Function='GetEveVAllPublisher' Version='1.0' LngID='" + LngID + "' />";

            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Event Topic Data
        public static XDocument GetEventAllTopicsfromCDB(string TopicRIDList, string LngID, string ltsuser, string ltspswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = ltsuser;
            proxy.ClientCredentials.UserName.Password = ltspswd;

            string sinput = "<SendData UserID='" + ltsuser + "' SessionID='" + ltspswd + "' Function='GetEveVAllTopics' Version='1.0' TopRID='" + TopicRIDList + "' LngID='" + LngID + "' />";

            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Event Nation Data
        public static XDocument GetEventAllNationfromCDB(string NatRIDList, string LngID, string ltsuser, string ltspswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = ltsuser;
            proxy.ClientCredentials.UserName.Password = ltspswd;

            string sinput = "<SendData UserID='" + ltsuser + "' SessionID='" + ltspswd + "' Function='GetEveVNation' Version='1.0' NatID='" + NatRIDList + "' LngID='" + LngID + "' />";

            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Event Fraction Data
        public static XDocument GetEventAllFractionfromCDB(string FraRIDList, string LngID, string ltsuser, string ltspswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = ltsuser;
            proxy.ClientCredentials.UserName.Password = ltspswd;

            string sinput = "<SendData UserID='" + ltsuser + "' SessionID='" + ltspswd + "' Function='GetEveVFraktion' Version='1.0' NatID='" + FraRIDList + "' LngID='" + LngID + "' />";

            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Event Area Data This function returns the geo-zones to be displayed in the instance of EventViewer. 
        public static XDocument GetEventAllAreasfromCDB(string EveVInstanceIDList, string LngID, string ltsuser, string ltspswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = ltsuser;
            proxy.ClientCredentials.UserName.Password = ltspswd;

            string sinput = "<SendData UserID='" + ltsuser + "' SessionID='" + ltspswd + "' Function='GetEveVAreas' Version='1.0' EveVInstanceID='" + EveVInstanceIDList + "' LngID='" + LngID + "' />";

            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Event Area Data This function returns the geo-zones to be displayed in the instance of EventViewer. 
        public static XDocument GetEventAllInstancesfromCDB(string EveVInstanceIDList, string LngID, string ltsuser, string ltspswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = ltsuser;
            proxy.ClientCredentials.UserName.Password = ltspswd;

            string sinput = "<SendData UserID='" + ltsuser + "' SessionID='" + ltspswd + "' Function='GetEveVInstance' Version='1.0' EveVInstanceID='…'='" + EveVInstanceIDList + "' LngID='" + LngID + "' />";

            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Events Changed Data  active events (0=all; <>1=yes)
        public static XDocument GetEventsChangedfromCDB(string StartDate, string I0Act, string ltsuser, string ltspswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = ltsuser;
            proxy.ClientCredentials.UserName.Password = ltspswd;

            string sinput = "<SendData UserID='" + ltsuser + "' SessionID='" + ltspswd + "' Function='GetEveVChanged' Version='1.0' StartDate='" + StartDate + "' I0Act='" + I0Act + "' />";

            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }
    }
}
