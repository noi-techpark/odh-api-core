using Helper;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LCS
{
    public class GetAccommodationDataLCS
    {
        ServiceReferenceLCS.DataClient lcs;

        public GetAccommodationDataLCS(string user, string pswd)
        {
        //https://medium.com/grensesnittet/integrating-with-soap-web-services-in-net-core-adebfad173fb
        //https://github.com/dotnet/wcf/issues/8

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

        //Accommodation Search
        public ServiceReferenceLCS.AccommodationDataSearchRS GetAccommodationDataSearch(XElement myrequest)
        {
            try
            {
                var accosearch = lcs.oAccommodationDataSearch(myrequest.ToXmlElement());

                return accosearch;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //Accommodation Search
        public async Task<ServiceReferenceLCS.AccommodationDataSearchRS> GetAccommodationDataSearchAsync(XElement myrequest)
        {
            try
            {
                return await lcs.oAccommodationDataSearchAsync(myrequest.ToXmlElement());
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static XElement GetAccommodationDataSearchRequest(
         string resultRID,
         string pageNr,
         string pageSize,
         string language,
         string sortingcriterion,
         string sortingorder,
         string sortingpromotebookable,
         string request,
         string filters,
         string timespanstart,
         string timespanend,
         string checkavailabilitystatus,
         string onlybookableresults,
         List<string> mealplans,
         List<string> accommodationrids,
         List<string> tourismorg,
         List<string> districts,
         List<string> marketinggroups,
         List<LCSRoomStay> lcsroomstay,
         string requestor,
         string messagepswd
         )
        {
            XElement requestbody = new XElement("AccommodationDataSearchRQ");

            //POS Element Header             
            requestbody.Add(GetPOS(requestor, messagepswd));
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
            returnformat.Add(new XAttribute("Request", request));
            returnformat.Add(new XAttribute("Filters", filters));
            returnformat.Add(new XAttribute("RoomStayGenre", "1"));

            parameters.Add(returnformat);
            //Ende Returnformat


            //"<Sorting Criterion='1' Order='' PromoteBookable='' />"
            if (!String.IsNullOrEmpty(sortingcriterion) || !String.IsNullOrEmpty(sortingorder) || !String.IsNullOrEmpty(sortingpromotebookable))
            {
                XElement sortingquery = new XElement("Sorting");
                sortingquery.Add(new XAttribute("Criterion", sortingcriterion));
                sortingquery.Add(new XAttribute("Order", sortingorder));
                sortingquery.Add(new XAttribute("PromoteBookable", sortingpromotebookable));
                parameters.Add(sortingquery);
            }
            //Ende Member

            //District
            if (districts.Count > 0)
                parameters.Add(GetDistricts(districts));

            //Tourismorganisations
            if (tourismorg.Count > 0)
                parameters.Add(GetTourismOrganizations(tourismorg));

            //Marketinggroups
            if (marketinggroups.Count > 0)
                parameters.Add(GetMarketingGroups(marketinggroups));


            //	"<TimeSpan Start='2017-11-10' End='2017-11-11' CheckAvailabilityStatus='1' />" +
            if (!String.IsNullOrEmpty(timespanstart) && !String.IsNullOrEmpty(timespanend))
            {
                XElement timespan = new XElement("TimeSpan");
                timespan.Add(new XAttribute("Start", timespanstart));
                timespan.Add(new XAttribute("End", timespanend));
                timespan.Add(new XAttribute("CheckAvailabilityStatus", checkavailabilitystatus));
                parameters.Add(timespan);
            }

            //"<AdvanceBookingRestriction OnlyBookableResults='0' >" +

            if (!String.IsNullOrEmpty(onlybookableresults))
            {
                XElement onlybookableresultsquery = new XElement("AdvanceBookingRestriction");
                onlybookableresultsquery.Add(new XAttribute("OnlyBookableResults", onlybookableresults));
                parameters.Add(onlybookableresultsquery);
            }
            //Ende Searchtermphrase


            //Mealplans
            parameters.Add(GetMealplans(mealplans));

            //Accommodations
            parameters.Add(GetAccommodations(accommodationrids));

            //Roomstay
            var myroomstaydatalist = GetRoomstay(lcsroomstay);

            foreach (var myroomstaydata in myroomstaydatalist)
            {
                parameters.Add(myroomstaydata);
            }




            //Facilities
            //if (facilities.Count > 0)
            //    parameters.Add(GetFacilities(facilities));

            requestbody.Add(parameters);

            //filters
            //XElement xfilters = new XElement("Filters");

            //if (categoryprefilter.Count > 0)
            //    xfilters.Add(GetCategoryPreFilter(categoryprefilter));

            //if (facilityprefilter.Count > 0)
            //    xfilters.Add(GetFacilityPreFilter(facilityprefilter));

            //requestbody.Add(xfilters);

            //Ende Filters

            return requestbody;
        }

        public static XElement GetPOS(string requestor, string messagepswd)
        {
            //POS Element Header
            XElement pos = new XElement("POS");
            XElement source = new XElement("Source");
            XElement requestorid = new XElement("RequestorID");
            requestorid.Add(new XAttribute("MessagePassword", messagepswd));
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

        //Marketinggroups
        public static XElement GetMealplans(List<string> mealplancodes)
        {
            XElement mymealplans = new XElement("MealPlans");
            if (mealplancodes.Count > 0)
            {
                foreach (string mealplancode in mealplancodes)
                {
                    XElement mymealplancode = new XElement("MealPlan");
                    mymealplancode.Add(new XAttribute("MealPlanCodes", mealplancode));

                    mymealplans.Add(mymealplancode);
                }
            }
            return mymealplans;
        }

        //Marketinggroups
        public static XElement GetAccommodations(List<string> accommodationrids)
        {
            XElement myaccommodations = new XElement("Accommodations");
            if (accommodationrids.Count > 0)
            {
                foreach (string accorid in accommodationrids)
                {
                    XElement myaccorid = new XElement("Accommodation");
                    myaccorid.Add(new XAttribute("RID", accorid));

                    myaccommodations.Add(myaccorid);
                }
            }
            return myaccommodations;
        }

        //Roomstay
        public static List<XElement> GetRoomstay(List<LCSRoomStay> roomstays)
        {
            List<XElement> roomstaylist = new List<XElement>();

            if (roomstays.Count > 0)
            {
                foreach (var roomstay in roomstays)
                {
                    XElement mymroomstay = new XElement("RoomStay");

                    mymroomstay.Add(new XAttribute("Index", roomstay.Index));
                    if (roomstay.Type != "0")
                        mymroomstay.Add(new XAttribute("Type", roomstay.Type));

                    mymroomstay.Add(new XAttribute("Guests", roomstay.Guests));

                    foreach (var guestage in roomstay.Age)
                    {
                        XElement myguestage = new XElement("Guest");
                        myguestage.Add(new XAttribute("Age", guestage));
                        mymroomstay.Add(myguestage);
                    }

                    roomstaylist.Add(mymroomstay);
                }
            }
            return roomstaylist;
        }

        public static List<LCSRoomStay> RoomstayTransformer(string roominfo)
        {
            if (!String.IsNullOrEmpty(roominfo) && roominfo != "null")
            {
                //roominfo aufteilen Form 1Z-1P-18 oder 1Z-2P-18.18,1Z-1P-18                
                List<LCSRoomStay> myroominfo = new List<LCSRoomStay>();

                var zimmerinfos = roominfo.Split('|');
                int roomseq = 1;

                foreach (var zimmerinfo in zimmerinfos)
                {
                    List<string> mypersons = new List<string>();

                    var myspittetzimmerinfo = zimmerinfo.Split('-');

                    var mypersoninfo = myspittetzimmerinfo[1].Split(',');
                    foreach (string s in mypersoninfo)
                    {
                        mypersons.Add(s);
                    }

                    var myroom = new LCSRoomStay();
                    myroom.Index = roomseq.ToString();
                    myroom.Guests = mypersons.Count.ToString();
                    myroom.Type = myspittetzimmerinfo[0].Substring(0);
                    myroom.Age = mypersons;

                    //var myroom = new Tuple<string, string, List<string>>(roomseq.ToString(), myspittetzimmerinfo[0].Substring(0), mypersons);

                    myroominfo.Add(myroom);
                    roomseq++;
                }

                return myroominfo;
            }
            else
            {
                List<LCSRoomStay> myroominfostd = new List<LCSRoomStay>();
                myroominfostd.Add(new LCSRoomStay() { Index = "1", Type = "0", Guests = "2", Age = new List<string>() { "18", "18" } });

                return myroominfostd;
            }
        }
    }

    public class LCSRoomStay
    {
        public string Index { get; set; }
        public string Type { get; set; }
        public string Guests { get; set; }
        public List<string> Age { get; set; }
    }
}
