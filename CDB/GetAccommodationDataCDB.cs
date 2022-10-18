using System;
using System.Diagnostics;
using System.Xml.Linq;
using DataModel;
using ServiceReferenceCDBData;

namespace CDB
{
    public class GetAccommodationDataCDB
    {
        public static CDBDataSoapClient.EndpointConfiguration GetEndpointConfig()
        {            
            //TODO CHECK IF THIS WORKS
            CDBDataSoapClient.EndpointConfiguration myconfig = new CDBDataSoapClient.EndpointConfiguration();                        

            return myconfig;
        }

        //Hotel Daten S4 Methode 
        public static XDocument GetHotelDatafromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {            
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);
            
            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelBaseDataS4' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotel Daten S4 Methode mit Sprache
        public static XDocument GetHotelDatafromCDB(string A0RIDList, string language, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelBaseDataS4' LngID='" + language + "' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotel Kategorieen 
        public static XDocument GetHotelCategoryfromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelCategory' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotel Address Koord
        public static XDocument GetHotelAddressCoordfromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelAddressCoord' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotel Addresse
        public static XDocument GetHotelAddressfromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelAddress' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotel Webdata
        public static XDocument GetHotelWebdatafromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelWebData' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotel Werbetexte 
        public static XDocument GetHotelPublicityfromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelPublicity' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotel Position
        public static XDocument GetHotelPositionfromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelPosition' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotel Overview
        public static XDocument GetHotelOverviewfromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelOverview' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotel Features TINS
        public static XDocument GetHotelTinfromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelTin' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotel Foto
        public static XDocument GetHotelFotofromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelFoto' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotel Toursimverein?
        public static XDocument GetHotelVGfromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelVG' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotel POS Data
        public static XDocument GetHotelPOSfromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelPOS' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotel TV PAy
        public static XDocument GetHotelTVPayfromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelTVPay' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotel Iban
        public static XDocument GetHotelIbanfromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelIBAN' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotel BAnkcoordinaten
        public static XDocument GetHotelBankdatafromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelBankData' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //HotelAdresstype
        public static XDocument GetHotelTypeOfAddressfromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelTypeOfAddress' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotelgallery
        public static XDocument GetHotelGalleryfromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelGallery' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotel AstatDaten
        public static XDocument GetHotelAstatBaseDatafromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelAstatBaseData' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotel GroupBase Data ??
        public static XDocument GetHotelGroupBaseDatafromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetGroupBaseData' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotel GroupName ??
        public static XDocument GetHotelGroupNamefromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetGroupName' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotel Group Definition
        public static XDocument GetHotelGroupDefinitionfromCDB(string A0RIDList, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetGroupDefinition' Version='1.0' A0RID='" + A0RIDList + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Alle Districts
        public static XDocument GetDistrictfromCDB(string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetFraktion' Version='1.0' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Booking POS
        public static XDocument GetBookingPosfromCDB(string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetBookingPOS' Version='1.0' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Alle TINS
        public static XDocument GetTinfromCDB(string showall, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetTin' ShowAll='" + showall + "' Version='1.0' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Tin Gruppen?
        public static XDocument GetTinGroupfromCDB(string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetTinGroup' Version='1.0' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Geänderte Hotels
        public static XDocument GetHotelChangedfromCDB(DateTime startdate, string A0Ene, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelChanged' G0RID='9E72B78AC5B14A9DB6BED6C2592483BF' StartDate='" + String.Format("{0:yyyy-MM-dd}", startdate) + "' OnlyAccCmp='0' A0Ene='" + A0Ene + "' Version='1.0' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Hotel Categories (alle kategorien)
        public static XDocument GetHotelCategorysfromCDB(string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetHotelCategorys' Version='1.0' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Alle VGroupData
        public static XDocument GetVGroupDatafromCDB(string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetVGroupData' Version='1.0' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Alle VGroupData
        public static XDocument GetVGroupBaseDatafromCDB(string G0RIDs, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetVGroupBaseData' G0RID='" + G0RIDs + "' Version='1.0' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Alle VGroupData
        //G0RID RID of view (divided by ';') 
        //G0ID Group ID 
        //G0Typ Group-type: 0=general; 1=tourist association; 2=holiday regions; 
        //3= touristic area (Tourismusregion); 4=municipality; 5=Skiing region; 6=touristic relevant site 
        //(touristisch relevanter Ort) 
        public static XDocument GetVGroupHierarchyfromCDB(string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetVGroupHierarchy' Version='1.0' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //VGroup NAme mit G0RID
        public static XDocument GetVGroupNamefromCDB(string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetVGroupName' Version='1.0' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Group DAta
        public static XDocument GetGroupDatafromCDB(string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetGroupBaseData' Version='1.0' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }

        //Independent
        public static XDocument GetIndependentDatafromCDB(string A0RID, string user, string pswd, string serviceurl)
        {
            var proxy = new ServiceReferenceCDBData.CDBDataSoapClient(GetEndpointConfig(), serviceurl);

            proxy.ClientCredentials.UserName.UserName = user;
            proxy.ClientCredentials.UserName.Password = pswd;

            string sinput = "<SendData UserID='" + user + "' SessionID='" + pswd + "' Function='GetIndependentData' Version='1.0' A0RID='" + A0RID + "' />";
            var xresponse = proxy.SendData(sinput);

            XDocument myresponse = XDocument.Parse(xresponse);

            return myresponse;
        }



        //Create List of Accommodations using GetHotelChanged Method
        public static void GetAccommodationListLTS(DateTime mystartdate, string destinationpath, bool updatelist, string user, string pswd, string serviceurl)
        {
            try
            {
                string accolistname = "AccommodationFullList";
                if (updatelist)
                    accolistname = "AccommodationUpdateList";

                XDocument myaccoschanged = GetAccommodationDataCDB.GetHotelChangedfromCDB(mystartdate.Date, "1", user, pswd, serviceurl);

                string destionationdir = "";

                if (destinationpath != "")
                    destionationdir = destinationpath;
                
                myaccoschanged.Save(destionationdir + accolistname + ".xml");             
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on import Accommodation List: " + ex.Message);
            }
        }

        //Create TIN List
        public static void GetTinListLTS(string destinationpath, string showall, string user, string pswd, string serviceurl)
        {
            try
            {
                XDocument mytins = GetAccommodationDataCDB.GetTinfromCDB("1", user, pswd, serviceurl);

                //TODO CHECK If this resolves empty response problem
                if (mytins.Root.HasElements)
                {
                    string destionationdir = "";

                    if (destinationpath != "")
                        destionationdir = destinationpath;
                    //new DateTime(2010, 1, 1)
                    mytins.Save(destionationdir + "Features.xml");                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on import TIN List: " + ex.Message);
            }
        }        
    }
}
