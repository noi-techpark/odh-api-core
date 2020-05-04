using System;
using System.Collections.Generic;
using System.Text;

namespace Helper.LCS
{
    public class ServiceReferenceConfig
    {
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_ISimpleService))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }

        //private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(TimeSpan timeout)
        //{
        //    var httpsBinding = new BasicHttpsBinding();
        //    httpsBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
        //    httpsBinding.Security.Mode = BasicHttpsSecurityMode.Transport;

        //    var integerMaxValue = int.MaxValue;
        //    httpsBinding.MaxBufferSize = integerMaxValue;
        //    httpsBinding.MaxReceivedMessageSize = integerMaxValue;
        //    httpsBinding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
        //    httpsBinding.AllowCookies = true;

        //    httpsBinding.ReceiveTimeout = timeout;
        //    httpsBinding.SendTimeout = timeout;
        //    httpsBinding.OpenTimeout = timeout;
        //    httpsBinding.CloseTimeout = timeout;

        //    return httpsBinding;
        //}

        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_ISimpleService))
            {
                return new System.ServiceModel.EndpointAddress("https://lcs.lts.it/api/data.svc/soap");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }

        public enum EndpointConfiguration
        {
            BasicHttpBinding_ISimpleService,
        }
    }
}
