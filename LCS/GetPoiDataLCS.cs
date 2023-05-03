using Helper;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;

namespace LCS
{
    public class GetPoiDataLCS
    {
        ServiceReferenceLCS.DataClient lcs;

        public GetPoiDataLCS(string user, string pswd)
        {
            //lcs = new ServiceReferenceLCS.DataClient();
            //lcs.ClientCredentials.UserName.UserName = ltsuser;
            //lcs.ClientCredentials.UserName.Password = ltspswd;

            BasicHttpsBinding basicHttpBinding = new BasicHttpsBinding();
            //basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            basicHttpBinding.Security.Mode = BasicHttpsSecurityMode.TransportWithMessageCredential;

            var integerMaxValue = int.MaxValue;
            basicHttpBinding.MaxBufferSize = integerMaxValue;
            basicHttpBinding.MaxReceivedMessageSize = integerMaxValue;
            basicHttpBinding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
            basicHttpBinding.AllowCookies = true;

            EndpointAddress endpointAddress = new EndpointAddress("https://lcs.lts.it/api/data.svc/soap");

            lcs = new ServiceReferenceLCS.DataClient(basicHttpBinding, endpointAddress);
            lcs.ClientCredentials.UserName.UserName = user;
            lcs.ClientCredentials.UserName.Password = pswd;
            var time = new TimeSpan(0, 0, 30);
            lcs.Endpoint.Binding.CloseTimeout = time;
        }

        //Poi Search
        public ServiceReferenceLCS.POISearchRS GetPoiSearch(XElement myrequest)
        {
            var poisearch = lcs.oPOISearch(myrequest.ToXmlElement());
            return poisearch;
        }

        //Poi Detail
        public ServiceReferenceLCS.POIDetailRS GetPoiDetail(XElement myrequest)
        {
            var poidetail = lcs.oPOIDetail(myrequest.ToXmlElement());
            return poidetail;
        }


        //Poi Changed Request
        public ServiceReferenceLCS.POIChangedItemsRS GetPoiChanged(XElement myrequest)
        {
            var poichanged = lcs.oPOIChangedItems(myrequest.ToXmlElement());
            return poichanged;
        }


    }
}
