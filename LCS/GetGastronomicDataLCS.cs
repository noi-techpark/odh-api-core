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


        //Gastronomy List Changed LTS WS!!! THEORETISCH UM den SMG STatus abzufragen (A0NE) müsste man bei HotelBaseData anfragen hmmm was machen wir hier? Hier aber TVList übergeben???
        //public static XDocument GetGastroChanged(DateTime startdate, string A0Ene, string G0Rids, string ltsuser, string ltspswd)
        //{
        //    var proxy = new Helper.ServiceReferenceCDBData.CDBDataSoapClient("CDBDataSoap");

        //    proxy.ClientCredentials.UserName.UserName = ltsuser;
        //    proxy.ClientCredentials.UserName.Password = ltspswd;

        //    string sinput = "<SendData UserID='" + ltsuser + "' SessionID='" + ltspswd + "' Function='GetRestChanged' G0RID='" + G0Rids + "' StartDate='" + String.Format("{0:yyyy-MM-dd}", startdate.Date) + "' OnlyAccCmp='0' A0Ene='" + A0Ene + "' Version='1.0' />";
        //    var xresponse = proxy.SendData(sinput);

        //    XDocument myresponse = XDocument.Parse(xresponse);

        //    return myresponse;
        //}

        public static XElement GetGastronomicDataSearchRequestAsync(
            string resultRID,
            string pageNr,
            string pageSize,
            string language,
            string onlyrootelement,
            string request,
            string contactinfos,
            string multimediadesc,
            string categorycodes,
            string filters,
            string searchterm,
            string tvmember,
            string hgvmember,
            string posrequired,
            List<string> districts,
            List<string> municipalities,
            List<string> tourismorg,
            List<string> marketing,
            List<string> catcodes,
            List<string> facilities,
            List<string> categoryprefilter,
            List<string> facilityprefilter,
            string requestor,
            string ltsmsgpswd
            )
        {
            XElement requestbody = new XElement("GastronomicDataSearchRQ");

            //POS Element Header             
            requestbody.Add(GetPOS(requestor, ltsmsgpswd));
            //Ende POS Element

            //Parameters
            XElement parameters = new XElement("Parameters");
            //Result
            XElement result = new XElement("Result");
            result.Add(new XAttribute("RID", resultRID));
            parameters.Add(result);
            //Ende Result

            //Paging
            XElement paging = new XElement("Paging");
            paging.Add(new XAttribute("PageNumber", pageNr));
            paging.Add(new XAttribute("PageSize", pageSize));
            parameters.Add(paging);
            //Ende Paging

            //Language
            XElement lang = new XElement("Language");
            lang.Add(new XAttribute("Code", language));
            parameters.Add(lang);
            //Ende Language

            //ReturnFormat
            XElement returnformat = new XElement("ReturnFormat");
            returnformat.Add(new XAttribute("OnlyRootElement", onlyrootelement));
            returnformat.Add(new XAttribute("Request", request));
            returnformat.Add(new XAttribute("ContactInfos", contactinfos));
            returnformat.Add(new XAttribute("MultimediaDescriptions", multimediadesc));
            returnformat.Add(new XAttribute("CategoryCodes", categorycodes));
            returnformat.Add(new XAttribute("Filters", filters));

            parameters.Add(returnformat);
            //Ende Returnformat

            //Searchtermphrase
            if (!String.IsNullOrEmpty(searchterm))
            {
                XElement mysearchterm = new XElement("SearchTerm");
                mysearchterm.Add(new XAttribute("Phrase", searchterm));
                parameters.Add(mysearchterm);
            }
            //Ende Searchtermphrase

            //Member HGV & TV
            if (!String.IsNullOrEmpty(hgvmember) || !String.IsNullOrEmpty(tvmember))
            {
                XElement myhgvmember = new XElement("Member");
                myhgvmember.Add(new XAttribute("HGV", hgvmember));
                myhgvmember.Add(new XAttribute("TV", tvmember));
                parameters.Add(myhgvmember);
            }
            //Ende Member

            //Position required
            if (!String.IsNullOrEmpty(posrequired))
            {
                XElement myposrequired = new XElement("Position");
                myposrequired.Add(new XAttribute("Required", posrequired));
                parameters.Add(myposrequired);
            }
            //Ende Searchtermphrase

            //District
            if (districts.Count > 0)
                parameters.Add(GetDistricts(districts));

            //Municipalities
            if (municipalities.Count > 0)
                parameters.Add(GetMunicipalities(municipalities));

            //Tourismorganisations
            if (tourismorg.Count > 0)
                parameters.Add(GetTourismOrganizations(tourismorg));

            //Marketinggroups
            if (marketing.Count > 0)
                parameters.Add(GetMarketingGroups(marketing));

            //Facilities
            if (facilities.Count > 0)
                parameters.Add(GetFacilities(facilities));

            requestbody.Add(parameters);

            //filters
            XElement xfilters = new XElement("Filters");

            if (categoryprefilter.Count > 0)
                xfilters.Add(GetCategoryPreFilter(categoryprefilter));

            if (facilityprefilter.Count > 0)
                xfilters.Add(GetFacilityPreFilter(facilityprefilter));

            requestbody.Add(xfilters);

            //Ende Filters

            return requestbody;
        }

        public static XElement GetGastronomicDataDetailRequestAsync(
            string gastronomyID,
            string language,
            string contactinfos,
            string multimediadesc,
            string categorycodes,
            string facilities,
            string operationschedules,
            string capacityceremonies,
            string dishrates,
            string newsandfeatures,
            string requestor,
            string ltsmsgpswd
            )
        {
            XElement requestbody = new XElement("GastronomicDataDetailRQ");

            //POS Element Header             
            requestbody.Add(GetPOS(requestor, ltsmsgpswd));
            //Ende POS Element

            //Parameters
            XElement parameters = new XElement("Parameters");

            //Language
            XElement lang = new XElement("Language");
            lang.Add(new XAttribute("Code", language));
            parameters.Add(lang);
            //Ende Language

            //ReturnFormat
            XElement returnformat = new XElement("ReturnFormat");
            returnformat.Add(new XAttribute("OperationSchedules", operationschedules));
            returnformat.Add(new XAttribute("Facilities", facilities));
            returnformat.Add(new XAttribute("ContactInfos", contactinfos));
            returnformat.Add(new XAttribute("MultimediaDescriptions", multimediadesc));
            returnformat.Add(new XAttribute("CategoryCodes", categorycodes));
            returnformat.Add(new XAttribute("CapacityCeremonies", capacityceremonies));
            returnformat.Add(new XAttribute("DishRates", dishrates));
            returnformat.Add(new XAttribute("NewsAndFeatures", newsandfeatures));

            parameters.Add(returnformat);
            //Ende Returnformat

            //Searchtermphrase
            if (!String.IsNullOrEmpty(gastronomyID))
            {
                XElement mygastroid = new XElement("GastronomicData");
                mygastroid.Add(new XAttribute("RID", gastronomyID));
                parameters.Add(mygastroid);
            }
            //Ende Searchtermphrase

            requestbody.Add(parameters);

            return requestbody;
        }

        public static XElement GetGastronomicCodesRequestAsync(
            string language,
            string requestor,
            string ltsmsgpswd
            )
        {
            XElement requestbody = new XElement("GastronomicCodesRQ");

            //POS Element Header             
            requestbody.Add(GetPOS(requestor, ltsmsgpswd));
            //Ende POS Element

            //Parameters
            XElement parameters = new XElement("Parameters");

            //Language
            XElement lang = new XElement("Language");
            lang.Add(new XAttribute("Code", language));
            parameters.Add(lang);
            //Ende Language            

            requestbody.Add(parameters);

            return requestbody;
        }


        public static XElement GetGastronomicDataChangedRequestAsync(
            string timespan,
            string tvrid,
            string requestor,
            string ltsmsgpswd
            )
        {
            XElement requestbody = new XElement("GastronomicDataChangedItemsRQ");

            //POS Element Header             
            requestbody.Add(GetPOS(requestor, ltsmsgpswd));
            //Ende POS Element

            //Parameters
            XElement parameters = new XElement("Parameters");

            //Language
            XElement ts = new XElement("TimeSpan");
            ts.Add(new XAttribute("Start", timespan));
            parameters.Add(ts);
            //Ende Language    

            if (!String.IsNullOrEmpty(tvrid))
            {
                XElement toursimverein = new XElement("TourismOrganizations");

                XElement toursimvereinrid = new XElement("TourismOrganization");
                toursimvereinrid.Add(new XAttribute("RID", tvrid));
                toursimverein.Add(toursimvereinrid);

                parameters.Add(toursimverein);
            }


            requestbody.Add(parameters);

            return requestbody;
        }


        public static XElement GetPOS(string requestor, string ltsmsgpswd)
        {
            //POS Element Header
            XElement pos = new XElement("POS");
            XElement source = new XElement("Source");
            XElement requestorid = new XElement("RequestorID");
            requestorid.Add(new XAttribute("MessagePassword", ltsmsgpswd));
            XElement companyname = new XElement("CompanyName", requestor);

            requestorid.Add(companyname);
            source.Add(requestorid);
            pos.Add(source);

            return pos;
        }

        //Districtlist
        public static XElement GetDistricts(List<string> districts)
        {
            XElement mydistricts = new XElement("Districts");
            if (districts.Count > 0)
            {
                foreach (string mydistrict in districts)
                {
                    XElement mydistrictcode = new XElement("District");
                    mydistrictcode.Add(new XAttribute("RID", mydistrict));

                    mydistricts.Add(mydistrictcode);
                }
            }
            return mydistricts;
        }

        //Municipalities
        public static XElement GetMunicipalities(List<string> municipalities)
        {
            XElement mymunicipalities = new XElement("Municipalities");
            if (municipalities.Count > 0)
            {
                foreach (string mymunicipality in municipalities)
                {
                    XElement mymunicipalitycode = new XElement("Municipality");
                    mymunicipalitycode.Add(new XAttribute("RID", mymunicipality));

                    mymunicipalities.Add(mymunicipalitycode);
                }
            }
            return mymunicipalities;
        }

        //Tourismorganisations
        public static XElement GetTourismOrganizations(List<string> tourismorg)
        {
            XElement mytourismorganisations = new XElement("TourismOrganizations");
            if (tourismorg.Count > 0)
            {
                foreach (string mytourismorg in tourismorg)
                {
                    XElement mytourismorgcode = new XElement("TourismOrganization");
                    mytourismorgcode.Add(new XAttribute("RID", mytourismorg));

                    mytourismorganisations.Add(mytourismorgcode);
                }
            }
            return mytourismorganisations;
        }

        //Marketinggroups
        public static XElement GetMarketingGroups(List<string> marketing)
        {
            XElement mymarketinggroups = new XElement("MarketingGroups");
            if (marketing.Count > 0)
            {
                foreach (string mymarketing in marketing)
                {
                    XElement mytourismorgcode = new XElement("MarketingGroup");
                    mytourismorgcode.Add(new XAttribute("RID", mymarketing));

                    mymarketinggroups.Add(mytourismorgcode);
                }
            }
            return mymarketinggroups;
        }

        //CategoryCodes
        public static XElement GetCategoryCodes(List<string> catcodes)
        {
            XElement mycategorycodes = new XElement("CategoryCodes");
            if (catcodes.Count > 0)
            {
                foreach (string mycatcode in catcodes)
                {
                    XElement mycategorycode = new XElement("GastronomicCategory");
                    mycategorycode.Add(new XAttribute("RID", mycatcode));

                    mycategorycodes.Add(mycategorycode);
                }
            }
            return mycategorycodes;
        }

        //Facilities
        public static XElement GetFacilities(List<string> facilities)
        {
            XElement myfacilities = new XElement("Facilities");
            myfacilities.Add(new XAttribute("FacilityGroupID", ""));
            if (facilities.Count > 0)
            {
                foreach (string myfacility in facilities)
                {
                    XElement myfacilitycode = new XElement("Facility");
                    myfacilitycode.Add(new XAttribute("RID", myfacility));

                    myfacilities.Add(myfacilitycode);
                }
            }
            return myfacilities;
        }

        //Categorycode Prefilters 
        public static XElement GetCategoryPreFilter(List<string> categoryprefilter)
        {
            XElement mycatprefilter = new XElement("CategoryCodes");
            int i = 1;

            if (categoryprefilter.Count > 0)
            {
                foreach (string mycatcode in categoryprefilter)
                {
                    XElement myfacilitycode = new XElement("Item");
                    myfacilitycode.Add(new XAttribute("OrderID", i));
                    myfacilitycode.Add(new XAttribute("Select", "1"));

                    XElement myfacilitycode2 = new XElement("ItemValue");
                    myfacilitycode2.Add(new XAttribute("RID", mycatcode));

                    myfacilitycode.Add(myfacilitycode2);

                    mycatprefilter.Add(myfacilitycode);

                    i++;
                }
            }
            return mycatprefilter;
        }

        //Facility Prefilters 
        public static XElement GetFacilityPreFilter(List<string> facilityprefilter)
        {
            XElement myfacilityprefilter = new XElement("Facilities");
            myfacilityprefilter.Add(new XAttribute("FacilityGroupID", ""));
            int i = 1;

            if (facilityprefilter.Count > 0)
            {
                foreach (string myprefiltercode in facilityprefilter)
                {
                    XElement myfacilitycode = new XElement("Item");
                    myfacilitycode.Add(new XAttribute("OrderID", i));
                    myfacilitycode.Add(new XAttribute("Select", "1"));

                    XElement myfacilitycode2 = new XElement("ItemValue");
                    myfacilitycode2.Add(new XAttribute("RID", myprefiltercode));

                    myfacilitycode.Add(myfacilitycode2);

                    myfacilityprefilter.Add(myfacilitycode);

                    i++;
                }
            }
            return myfacilityprefilter;
        }


    }
}
