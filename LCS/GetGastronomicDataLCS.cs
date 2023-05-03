using Helper;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace LCS
{
    public class GetGastronomicDataLCS
    {
        ServiceReferenceLCS.DataClient lcs;

        public GetGastronomicDataLCS(string user, string pswd)
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

        //Gastronomy Search
        public ServiceReferenceLCS.GastronomicDataSearchRS GetGastronomicDataSearch(XElement myrequest)
        {
            var gastrosearch = lcs.oGastronomicDataSearch(myrequest.ToXmlElement());
            return gastrosearch;
        }

        //Gastronomy Detail
        public ServiceReferenceLCS.GastronomicDataDetailRS GetGastronomicDataDetail(XElement myrequest)
        {
            var gastrodetail = lcs.oGastronomicDataDetail(myrequest.ToXmlElement());
            return gastrodetail;
        }

        //Gastronomy Detail
        public ServiceReferenceLCS.GastronomicCodesRS GetGastronomicCodes(XElement myrequest)
        {
            var gastrocodes = lcs.oGastronomicCodes(myrequest.ToXmlElement());
            return gastrocodes;
        }

        //Gastronomy Codes
        public XElement GetGastronomicCodesXElement(XElement myrequest)
        {
            var gastrocodes = lcs.GastronomicCodes(myrequest.ToXmlElement());
            return GetXElementFromXmlElement(gastrocodes);
        }

        //Convert to Xelement
        public XElement GetXElementFromXmlElement(XmlElement xmlElement)
        {
            return XElement.Load(xmlElement.CreateNavigator().ReadSubtree());
        }              
    }
}
