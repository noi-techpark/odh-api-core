// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Helper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Linq;
using ServiceReferenceLCS;

namespace LCS
{
    public class GetGastronomicDataLCS
    {
        ServiceReferenceLCS.DataClient lcs;

        public GetGastronomicDataLCS(string serviceurl, string user, string pswd)
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

            EndpointAddress endpointAddress = new EndpointAddress(serviceurl + "/soap");

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

        //Gastronomy DataChanged
        public ServiceReferenceLCS.GastronomicDataChangedItemsRS GetGastronomicDataChanged(XElement myrequest)
        {
            var gastrosearch = lcs.oGastronomicDataChangedItems(myrequest.ToXmlElement());
            return gastrosearch;
        }

        //Gastronomy Detail
        public ServiceReferenceLCS.GastronomicDataDetailRS GetGastronomicDataDetail(XElement myrequest)
        {
            var gastrodetail = lcs.oGastronomicDataDetail(myrequest.ToXmlElement());
            return gastrodetail;
        }

        //Gastronomy Codes
        public ServiceReferenceLCS.GastronomicCodesRS GetGastronomicCodes(XElement myrequest)
        {
            var gastrocodes = lcs.oGastronomicCodes(myrequest.ToXmlElement());
            return gastrocodes;
        }

        //Gastronomy Codes as Xelement
        public XElement GetGastronomicCodesXElement(XElement myrequest)
        {
            var gastrocodes = lcs.GastronomicCodes(myrequest.ToXmlElement());
            return XElement.Load(gastrocodes.CreateNavigator().ReadSubtree());
        }

        public ServiceReferenceLCS.GastronomicDataSearchRS GetGastronomyListLTS(int pagesize, string language, string ltsmsgpswd)
        {
            GastronomicDataSearchRS gastroresponse = default(GastronomicDataSearchRS);

            var mygastrorequest = GetLCSRequests.GetGastronomicDataSearchRequestAsync("", "1", pagesize.ToString(), language, "1", "0", "0", "0", "0", "0", "", "0", "0", "0", new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), "NOI", ltsmsgpswd);
            
            gastroresponse = GetGastronomicDataSearch(mygastrorequest);

            string resultrid = gastroresponse.Result.RID;
            int pages = gastroresponse.Paging.PagesQty;

            for (int i = 2; i <= pages; i++)
            {
                mygastrorequest = GetLCSRequests.GetGastronomicDataSearchRequestAsync(resultrid, i.ToString(), pagesize.ToString(), language, "1", "0", "0", "0", "0", "0", "", "0", "0", "0", new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), "NOI", ltsmsgpswd);

                var tempgastroresponse = GetGastronomicDataSearch(mygastrorequest);

                List<GastronomicData> gastronomicDatas = gastroresponse.GastronomicData.ToList();
                gastronomicDatas.AddRange(tempgastroresponse.GastronomicData.ToList());

                gastroresponse.GastronomicData = gastronomicDatas.ToArray();
            }

            return gastroresponse;
        }

    }
}
