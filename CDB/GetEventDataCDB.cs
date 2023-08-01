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
    public class GetEvents
    {
        //Methode für Liste
        public static XDocument GetEventList(string lang, string searchid, int pageSize, int pagenr, string requestor,
            string publRIDList,
            string topicRIDList,
            DateTime datefrom,
            DateTime dateto,
            string ranc,
            string fraRIDList,
            string seekword,
            string orgRIDList,
            string title,
            string onlyEveHead,
            string showSDat,
            string zone,
            string withCdays,
            string compatibilitymode,
            string ltsuser,
            string ltspswd,
            string serviceurl
            )
        {
            try
            {
                XDocument myEvents = GetEventDataCDB.GetEventSeekEngine(publRIDList, 
                    String.Format("{0:yyyy-MM-dd}", datefrom), 
                    String.Format("{0:yyyy-MM-dd}", dateto), 
                    topicRIDList, ranc, fraRIDList, seekword, lang, 
                    pagenr.ToString(), pageSize.ToString(), orgRIDList, 
                    title, searchid, onlyEveHead, showSDat, zone, withCdays, 
                    compatibilitymode, 
                    ltsuser, 
                    ltspswd,
                    serviceurl);

                return myEvents;

                //var eventdata = myEvents.Root.Elements("Head");
                //var eventoverview = myEvents.Root.Element("Overview");

                //Result myeventlist = new Result();
                //myeventlist.SeekID = eventoverview.Attribute("SeekID").Value;
                //myeventlist.CurrentPage = Convert.ToInt32(eventoverview.Attribute("PageAct").Value);
                //myeventlist.TotalPages = Convert.ToInt32(eventoverview.Attribute("PagesTotal").Value);
                //myeventlist.TotalResults = Convert.ToInt32(eventoverview.Attribute("EventsTotal").Value);

                //int i = 1;

                //foreach (XElement myevent in eventdata)
                //{
                //    var myparsedevent = ParseEventResponse.ParsemyEventResponse(i, lang, myevent);
                //    i++;
                //    myeventlist.Event.Add(myparsedevent);
                //}
                //return myeventlist;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //Methode für Liste der Geänderten
        public static XDocument GetEventChangeList(DateTime startDate, string ltsuser, string ltspswd,
            string serviceurl)
        {
            try
            {
                XDocument myEventsChanged = GetEventDataCDB.GetEventsChangedfromCDB(String.Format("{0:yyyy-MM-dd}", startDate), "0", 
                    ltsuser, ltspswd,serviceurl);

                return myEventsChanged;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //Methode für Detail
        public static XElement GetEventDetail(string lang, string requestor,
            string evRIDList,
            string publRIDList,
            string withOrganizer,
            string withCdays,
            string ltsuser,
            string ltspswd,
            string serviceurl
            )
        {
            try
            {
                XDocument myEvents = GetEventDataCDB.GetEventDatafromCDB(evRIDList, withOrganizer, lang, publRIDList, withCdays, ltsuser, ltspswd, serviceurl);

                var myEventElement = myEvents.Root.Element("Root").Element("Head");

                return myEventElement;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }

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
