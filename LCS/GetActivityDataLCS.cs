using Helper;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;

namespace LCS
{
    public class GetActivityDataLCS
    {
        ServiceReferenceLCS.DataClient lcs;

        public GetActivityDataLCS(string user, string pswd)
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

        //Activity Search
        public ServiceReferenceLCS.ActivitySearchRS GetActivitySearch(XElement myrequest)
        {
            var activitysearch = lcs.oActivitySearch(myrequest.ToXmlElement());
            return activitysearch;
        }

        //Activity Detail
        public ServiceReferenceLCS.ActivityDetailRS GetActivityDetail(XElement myrequest)
        {
            var activitydetail = lcs.oActivityDetail(myrequest.ToXmlElement());
            return activitydetail;
        }

        //Activity Changed Request
        public ServiceReferenceLCS.ActivityChangedItemsRS GetActivityChanged(XElement myrequest)
        {
            var activitychanged = lcs.oActivityChangedItems(myrequest.ToXmlElement());
            return activitychanged;
        }

        //Weather Snow Observation Search
        public ServiceReferenceLCS.WeatherSnowObservationSearchRS GetWeatherSnowSearch(XElement myrequest)
        {
            var activitysearch = lcs.oWeatherSnowObservationSearch(myrequest.ToXmlElement());
            return activitysearch;
        }

        //Weather Snow Observation Detail
        public ServiceReferenceLCS.WeatherSnowObservationDetailRS GetWeatherSnowDetail(XElement myrequest)
        {
            var activitydetail = lcs.oWeatherSnowObservationDetail(myrequest.ToXmlElement());
            return activitydetail;
        }

        //Webcam Search
        public ServiceReferenceLCS.WebCamSearchRS GetWebcamSearch(XElement myrequest)
        {
            var activitysearch = lcs.oWebCamSearch(myrequest.ToXmlElement());
            return activitysearch;
        }

        //Webcam Detail
        public ServiceReferenceLCS.WebCamDetailRS GetWebcamDetail(XElement myrequest)
        {
            var activitydetail = lcs.oWebCamDetail(myrequest.ToXmlElement());
            return activitydetail;
        }

       
    }
}
