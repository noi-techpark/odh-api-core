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

        //Get the Activities
        public static XElement GetActivitySearchRequestAsync(
            string resultRID,
            string pageNumber,
            string PageSize,
            string language,
            string sortcriterion,
            string order,
            string searchtermphrase,
            string onlyrootelement,
            string filter,
            string withrid,
            string withlevel,
            string enumcodes,
            string newsandfeatures,
            string multimediadesc,
            string contactinfos,
            string membership,
            string operationschedules,
            string geodatas,
            string showdisabled,
            string onlysummaries,
            string iswithlight,
            string hasrentals,
            string isprepared,
            string isopen,
            List<string> activities,
            List<string> activitydatatypes,
            List<string> activityenumcode,
            List<string> areacodes,
            List<string> ownercodes,
            List<string> activityprefilter,
            string requestor,
            string ltsmsgpswd
            )
        {

            XElement requestbody = new XElement("ActivitySearchRQ");

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
            paging.Add(new XAttribute("PageNumber", pageNumber));
            paging.Add(new XAttribute("PageSize", PageSize));
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
            returnformat.Add(new XAttribute("Filters", filter));
            returnformat.Add(new XAttribute("WithRID", withrid));
            returnformat.Add(new XAttribute("WithLevel", withlevel));
            returnformat.Add(new XAttribute("EnumCodes", enumcodes));
            returnformat.Add(new XAttribute("NewsAndFeatures", newsandfeatures));
            returnformat.Add(new XAttribute("MultimediaDescriptions", multimediadesc));
            returnformat.Add(new XAttribute("ContactInfos", contactinfos));
            returnformat.Add(new XAttribute("Memberships", membership));
            returnformat.Add(new XAttribute("OperationSchedules", operationschedules));
            returnformat.Add(new XAttribute("GeoDatas", geodatas));
            returnformat.Add(new XAttribute("ShowDisabled", showdisabled));
            returnformat.Add(new XAttribute("SummaryOnly", onlysummaries));

            parameters.Add(returnformat);
            //Ende Returnformat

            //Sorting
            if (!String.IsNullOrEmpty(sortcriterion))
            {
                XElement sortcrit = new XElement("Sorting");
                sortcrit.Add(new XAttribute("Criterion", sortcriterion), new XAttribute("Order", order));
                parameters.Add(sortcrit);
            }
            //Ende Sorting

            //Searchtermphrase
            if (!String.IsNullOrEmpty(searchtermphrase))
            {
                XElement searchterm = new XElement("SearchTerm");
                searchterm.Add(new XAttribute("Phrase", searchtermphrase));
                parameters.Add(searchterm);
            }
            //Ende Searchtermphrase

            parameters.Add(GetActivities(activities, activitydatatypes, activityenumcode));

            //Ende Activities

            if (areacodes.Count > 0)
                parameters.Add(GetAreas(areacodes));

            if (ownercodes.Count > 0)
                parameters.Add(GetOwners(ownercodes));

            requestbody.Add(parameters);
            //Ende Parameters    

            //Filters
            requestbody.Add(GetActivityPrefilters(activityprefilter));
            //Ende Filters

            return requestbody;
        }

        //Get the Activities
        public static XElement GetActivityByTagSearchRequestAsync(
            string resultRID,
            string pageNumber,
            string PageSize,
            string language,
            string sortcriterion,
            string order,
            string searchtermphrase,
            string onlyrootelement,
            string filter,
            string withrid,
            string withlevel,
            string enumcodes,
            string newsandfeatures,
            string multimediadesc,
            string contactinfos,
            string membership,
            string operationschedules,
            string geodatas,
            string showdisabled,
            string onlysummaries,
            string iswithlight,
            string hasrentals,
            string isprepared,
            string isopen,
            List<string> activities,
            List<string> taggingtypes,
            List<string> activityenumcode,
            List<string> areacodes,
            List<string> ownercodes,
            List<string> activityprefilter,
            string requestor,
            string ltsmsgpswd
            )
        {

            XElement requestbody = new XElement("ActivitySearchRQ");

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
            paging.Add(new XAttribute("PageNumber", pageNumber));
            paging.Add(new XAttribute("PageSize", PageSize));
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
            returnformat.Add(new XAttribute("Filters", filter));
            returnformat.Add(new XAttribute("WithRID", withrid));
            returnformat.Add(new XAttribute("WithLevel", withlevel));
            returnformat.Add(new XAttribute("EnumCodes", enumcodes));
            returnformat.Add(new XAttribute("NewsAndFeatures", newsandfeatures));
            returnformat.Add(new XAttribute("MultimediaDescriptions", multimediadesc));
            returnformat.Add(new XAttribute("ContactInfos", contactinfos));
            returnformat.Add(new XAttribute("Memberships", membership));
            returnformat.Add(new XAttribute("OperationSchedules", operationschedules));
            returnformat.Add(new XAttribute("GeoDatas", geodatas));
            returnformat.Add(new XAttribute("ShowDisabled", showdisabled));
            returnformat.Add(new XAttribute("SummaryOnly", onlysummaries));

            parameters.Add(returnformat);
            //Ende Returnformat

            //Sorting
            if (!String.IsNullOrEmpty(sortcriterion))
            {
                XElement sortcrit = new XElement("Sorting");
                sortcrit.Add(new XAttribute("Criterion", sortcriterion), new XAttribute("Order", order));
                parameters.Add(sortcrit);
            }
            //Ende Sorting

            //Searchtermphrase
            if (!String.IsNullOrEmpty(searchtermphrase))
            {
                XElement searchterm = new XElement("SearchTerm");
                searchterm.Add(new XAttribute("Phrase", searchtermphrase));
                parameters.Add(searchterm);
            }
            //Ende Searchtermphrase

            parameters.Add(GetActivities(activities, new List<string>(), activityenumcode, taggingtypes));

            //Ende Activities

            if (areacodes.Count > 0)
                parameters.Add(GetAreas(areacodes));

            if (ownercodes.Count > 0)
                parameters.Add(GetOwners(ownercodes));

            requestbody.Add(parameters);
            //Ende Parameters    

            //Filters
            requestbody.Add(GetActivityPrefilters(activityprefilter));
            //Ende Filters

            return requestbody;
        }

        //Activity Detail Request
        public static XElement GetActivityDetailRequest(
            string language,
            string onlyrootelement,
            string withrid,
            string withlevel,
            string enumcodes,
            string newsandfeatures,
            string operationschedules,
            string showdisabled,
            string memberships,
            string geodata,
            string multimediadesc,
            string contactinfos,
            string rating,
            string tags,
            string snowpark,
            string righttousetheway,
            List<string> activities,
            string requestor,
            string ltsmsgpswd
            )
        {
            XElement requestbody = new XElement("ActivityDetailRQ");

            //POS Element Header             
            requestbody.Add(GetPOS(requestor, ltsmsgpswd));

            //Parameters
            XElement parameters = new XElement("Parameters");

            //Language
            XElement lang = new XElement("Language");
            lang.Add(new XAttribute("Code", language));
            parameters.Add(lang);
            //Ende Language

            //ReturnFormat
            XElement returnformat = new XElement("ReturnFormat");
            returnformat.Add(new XAttribute("OnlyRootElement", onlyrootelement));
            returnformat.Add(new XAttribute("WithRID", withrid));
            returnformat.Add(new XAttribute("WithLevel", withlevel));
            returnformat.Add(new XAttribute("EnumCodes", enumcodes));
            returnformat.Add(new XAttribute("NewsAndFeatures", newsandfeatures));
            returnformat.Add(new XAttribute("MultimediaDescriptions", multimediadesc));
            returnformat.Add(new XAttribute("ContactInfos", contactinfos));
            returnformat.Add(new XAttribute("OperationSchedules", operationschedules));
            returnformat.Add(new XAttribute("GeoDatas", geodata));
            returnformat.Add(new XAttribute("ShowDisabled", showdisabled));
            returnformat.Add(new XAttribute("Memberships", memberships));
            returnformat.Add(new XAttribute("Tags", tags));
            returnformat.Add(new XAttribute("Rating", rating));
            returnformat.Add(new XAttribute("SnowPark", snowpark));
            returnformat.Add(new XAttribute("RightToUseTheWay", righttousetheway));

            parameters.Add(returnformat);

            if (activities.Count > 0)
                parameters.Add(GetActivities(activities, new List<string>(), new List<string>()));

            requestbody.Add(parameters);

            return requestbody;
        }

        //Weather Snow Observation Search
        public static XElement GetWeatherSnowSearchRequest(
            string resultRID,
            string pageNumber,
            string pageSize,
            string language,
            string onlyrootelement,
            string request,
            string withRID,
            string withLevel,
            string showdisabled,
            string sortcriterion,
            string order,
            string searchtermphrase,
            string observation,
            string newsandfeatures,
            string geodata,
            string enumcodes,
            List<string> measuringpoints,
            List<string> areas,
            List<string> owner,
            string requestor,
            string ltsmsgpswd
            )
        {
            XElement requestbody = new XElement("WeatherSnowObservationSearchRQ");

            //POS Element Header            
            requestbody.Add(GetPOS(requestor, ltsmsgpswd));

            //Parameters
            XElement parameters = new XElement("Parameters");

            //Result
            XElement result = new XElement("Result");
            result.Add(new XAttribute("RID", resultRID));
            parameters.Add(result);
            //Ende Result

            //Paging
            XElement paging = new XElement("Paging");
            paging.Add(new XAttribute("PageNumber", pageNumber));
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
            returnformat.Add(new XAttribute("WithRID", withRID));
            returnformat.Add(new XAttribute("WithLevel", withLevel));
            returnformat.Add(new XAttribute("EnumCodes", enumcodes));
            returnformat.Add(new XAttribute("Observation", observation));
            returnformat.Add(new XAttribute("ShowDisabled", showdisabled));
            returnformat.Add(new XAttribute("GeoDatas", geodata));
            returnformat.Add(new XAttribute("NewsAndFeatures", newsandfeatures));

            parameters.Add(returnformat);
            //Ende Returnformat

            //Sorting
            if (!String.IsNullOrEmpty(sortcriterion))
            {
                XElement sortcrit = new XElement("Sorting");
                sortcrit.Add(new XAttribute("Criterion", sortcriterion), new XAttribute("Order", order));
                parameters.Add(sortcrit);
            }
            //Ende Sorting

            //Searchtermphrase
            if (!String.IsNullOrEmpty(searchtermphrase))
            {
                XElement searchterm = new XElement("SearchTerm");
                searchterm.Add(new XAttribute("Phrase", searchtermphrase));
                parameters.Add(searchterm);
            }
            //Ende Searchtermphrase

            if (measuringpoints.Count > 0)
                parameters.Add(GetMeasuringpoints(measuringpoints));

            if (areas.Count > 0)
                parameters.Add(GetAreas(areas));

            if (owner.Count > 0)
                parameters.Add(GetOwners(owner));

            requestbody.Add(parameters);

            return requestbody;
        }

        //Üebr Areas Measuringpoints rausfinden, Measuringpoints zwischenspeichern
        public static XElement GetWeatherSnowDetailRequest(
            string resultRID,
            string pageNumber,
            string pageSize,
            string language,
            string onlyrootelement,
            string request,
            string withRID,
            string withLevel,
            string showdisabled,
            string memberships,
            string enumcodes,
            string observation,
            string geodata,
            string newsandfeatures,
            string sortcriterion,
            string order,
            string searchtermphrase,
            List<string> measuringpoints,
            string requestor,
            string ltsmsgpswd
            )
        {

            XElement requestbody = new XElement("WeatherSnowObservationDetailRQ");

            //POS Element Header            
            requestbody.Add(GetPOS(requestor, ltsmsgpswd));

            //Parameters
            XElement parameters = new XElement("Parameters");

            //Result
            XElement result = new XElement("Result");
            result.Add(new XAttribute("RID", resultRID));
            parameters.Add(result);
            //Ende Result

            //Paging
            XElement paging = new XElement("Paging");
            paging.Add(new XAttribute("PageNumber", pageNumber));
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
            returnformat.Add(new XAttribute("WithRID", withRID));
            returnformat.Add(new XAttribute("WithLevel", withLevel));
            returnformat.Add(new XAttribute("EnumCodes", enumcodes));
            returnformat.Add(new XAttribute("Observation", observation));
            returnformat.Add(new XAttribute("ShowDisabled", showdisabled));
            returnformat.Add(new XAttribute("Memberships", memberships));
            returnformat.Add(new XAttribute("GeoDatas", geodata));
            returnformat.Add(new XAttribute("NewsAndFeatures", newsandfeatures));

            parameters.Add(returnformat);
            //Ende Returnformat

            //Sorting
            if (!String.IsNullOrEmpty(sortcriterion))
            {
                XElement sortcrit = new XElement("Sorting");
                sortcrit.Add(new XAttribute("Criterion", sortcriterion), new XAttribute("Order", order));
                parameters.Add(sortcrit);
            }
            //Ende Sorting

            //Searchtermphrase
            if (!String.IsNullOrEmpty(searchtermphrase))
            {
                XElement searchterm = new XElement("SearchTerm");
                searchterm.Add(new XAttribute("Phrase", searchtermphrase));
                parameters.Add(searchterm);
            }

            //Measuringpoint
            if (measuringpoints.Count > 0)
                parameters.Add(GetMeasuringpoints(measuringpoints));

            requestbody.Add(parameters);

            return requestbody;
        }

        //Webcam Search
        public static XElement GetWebcamSearchRequest(
            string resultRID,
            string pageNumber,
            string pageSize,
            string language,
            string onlyrootelement,
            string request,
            string withRID,
            string withLevel,
            string showdisabled,
            string sortcriterion,
            string order,
            string searchtermphrase,
            List<string> areas,
            List<string> owner,
            string requestor,
            string ltsmsgpswd
            )
        {

            XElement requestbody = new XElement("WebCamSearchRQ");

            //POS Element Header            
            requestbody.Add(GetPOS(requestor, ltsmsgpswd));

            //Parameters
            XElement parameters = new XElement("Parameters");

            //Result
            XElement result = new XElement("Result");
            result.Add(new XAttribute("RID", resultRID));
            parameters.Add(result);
            //Ende Result

            //Paging
            XElement paging = new XElement("Paging");
            paging.Add(new XAttribute("PageNumber", pageNumber));
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
            returnformat.Add(new XAttribute("WithRID", withRID));
            returnformat.Add(new XAttribute("WithLevel", withLevel));
            returnformat.Add(new XAttribute("ShowDisabled", showdisabled));

            parameters.Add(returnformat);
            //Ende Returnformat

            //Sorting
            if (!String.IsNullOrEmpty(sortcriterion))
            {
                XElement sortcrit = new XElement("Sorting");
                sortcrit.Add(new XAttribute("Criterion", sortcriterion), new XAttribute("Order", order));
                parameters.Add(sortcrit);
            }
            //Ende Sorting

            //Searchtermphrase
            if (!String.IsNullOrEmpty(searchtermphrase))
            {
                XElement searchterm = new XElement("SearchTerm");
                searchterm.Add(new XAttribute("Phrase", searchtermphrase));
                parameters.Add(searchterm);
            }
            //Ende Searchtermphrase            
            if (areas.Count > 0)
                parameters.Add(GetAreas(areas));
            if (owner.Count > 0)
                parameters.Add(GetOwners(owner));

            requestbody.Add(parameters);

            return requestbody;
        }


        //Üebr Areas Measuringpoints rausfinden, Measuringpoints zwischenspeichern
        public static XElement GetWebcamDetailRequest(
            string language,
            string onlyrootelement,
            string request,
            string withRID,
            string withLevel,
            string showdisabled,
            string membership,
            string multimediadesc,
            string newsandfeatures,
            string geodatas,
            string enumcodes,
            string sortcriterion,
            string order,
            string searchtermphrase,
            List<string> webcams,
            string requestor,
            string ltsmsgpswd
            )
        {

            XElement requestbody = new XElement("WebCamDetailRQ");

            //POS Element Header            
            requestbody.Add(GetPOS(requestor, ltsmsgpswd));

            //Parameters
            XElement parameters = new XElement("Parameters");

            //Result
            //XElement result = new XElement("Result");
            //result.Add(new XAttribute("RID", resultRID));
            //parameters.Add(result);
            //Ende Result

            //Paging
            //XElement paging = new XElement("Paging");
            //paging.Add(new XAttribute("PageNumber", pageNumber));
            //paging.Add(new XAttribute("PageSize", pageSize));
            //parameters.Add(paging);
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
            returnformat.Add(new XAttribute("WithRID", withRID));
            returnformat.Add(new XAttribute("WithLevel", withLevel));
            returnformat.Add(new XAttribute("EnumCodes", enumcodes));
            returnformat.Add(new XAttribute("MultimediaDescriptions", multimediadesc));
            returnformat.Add(new XAttribute("ShowDisabled", showdisabled));
            returnformat.Add(new XAttribute("Memberships", membership));
            returnformat.Add(new XAttribute("GeoDatas", geodatas));
            returnformat.Add(new XAttribute("NewsAndFeatures", newsandfeatures));

            parameters.Add(returnformat);
            //Ende Returnformat


            //Sorting
            if (!String.IsNullOrEmpty(sortcriterion))
            {
                XElement sortcrit = new XElement("Sorting");
                sortcrit.Add(new XAttribute("Criterion", sortcriterion), new XAttribute("Order", order));
                parameters.Add(sortcrit);
            }
            //Ende Sorting

            //Searchtermphrase
            if (!String.IsNullOrEmpty(searchtermphrase))
            {
                XElement searchterm = new XElement("SearchTerm");
                searchterm.Add(new XAttribute("Phrase", searchtermphrase));
                parameters.Add(searchterm);
            }

            //Measuringpoint
            if (webcams.Count > 0)
            {
                parameters.Add(GetWebcams(webcams));
            }

            requestbody.Add(parameters);

            return requestbody;
        }

        //Get The Request for Activity Changed Items
        public static XElement GetActivityChangedRequest(
            string requestor,
            string dateFrom,
            string dataTypeID,
            string ltsmsgpswd)
        {
            XElement requestbody = new XElement("ActivityChangedItemsRQ");

            //POS Element Header            
            requestbody.Add(GetPOS(requestor, ltsmsgpswd));

            //Parameters
            XElement parameters = new XElement("Parameters");

            //TimeSpan            
            XElement lang = new XElement("TimeSpan");
            lang.Add(new XAttribute("Start", dateFrom));
            parameters.Add(lang);
            //Ende TimeSpan


            //Activities
            XElement activities = new XElement("Activities");
            XElement activitydatatype = new XElement("Activity");
            activitydatatype.Add(new XAttribute("DataTypeID", dataTypeID));
            activities.Add(activitydatatype);
            parameters.Add(activities);

            requestbody.Add(parameters);

            return requestbody;
        }

        //Get The Request for Activity Changed Items with Tagging
        public static XElement GetActivityByTagChangedRequest(
            string requestor,
            string dateFrom,
            string tagID,
            string ltsmsgpswd)
        {
            XElement requestbody = new XElement("ActivityChangedItemsRQ");

            //POS Element Header            
            requestbody.Add(GetPOS(requestor, ltsmsgpswd));

            //Parameters
            XElement parameters = new XElement("Parameters");

            //TimeSpan            
            XElement lang = new XElement("TimeSpan");
            lang.Add(new XAttribute("Start", dateFrom));
            parameters.Add(lang);
            //Ende TimeSpan


            //Activities
            XElement activities = new XElement("Activities");
            //XElement activitydatatype = new XElement("Activity");
            //activitydatatype.Add(new XAttribute("DataTypeID", ""));
            //activities.Add(activitydatatype);

            XElement taggingtags = new XElement("Tags");
            XElement taggingtyperid = new XElement("Tag");
            XAttribute activitytaggingID = new XAttribute("RID", tagID);
            taggingtyperid.Add(activitytaggingID);
            taggingtags.Add(taggingtyperid);
            activities.Add(taggingtags);


            parameters.Add(activities);

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

        public static XElement GetAreas(List<string> areas)
        {
            //Activity AreaCodes 
            XElement myareacodes = new XElement("Areas");
            if (areas.Count > 0)
            {
                foreach (string myarearid in areas)
                {
                    XElement myareacode = new XElement("Area");
                    myareacode.Add(new XAttribute("RID", myarearid));

                    myareacodes.Add(myareacode);
                }
            }

            return myareacodes;
        }

        public static XElement GetOwners(List<string> owner)
        {
            //Activity Ownercodes 
            XElement myownercodes = new XElement("Owners");
            if (owner.Count > 0)
            {
                foreach (string myownerrid in owner)
                {
                    XElement myowner = new XElement("Owner");
                    myowner.Add(new XAttribute("RID", myownerrid));

                    myownercodes.Add(myowner);
                }
            }

            return myownercodes;
        }

        public static XElement GetMeasuringpoints(List<string> measuringpoints)
        {
            //Activity Mesuringpoints 
            XElement mymeasuringpoints = new XElement("MeasuringPoints");
            if (measuringpoints.Count > 0)
            {
                foreach (string mymeasuringrid in measuringpoints)
                {
                    XElement measuringpoint = new XElement("MeasuringPoint");
                    measuringpoint.Add(new XAttribute("RID", mymeasuringrid));

                    mymeasuringpoints.Add(measuringpoint);
                }
            }

            return mymeasuringpoints;
        }

        public static XElement GetWebcams(List<string> webcams)
        {
            XElement mywebcams = new XElement("WebCams");
            if (webcams.Count > 0)
            {
                foreach (string mywebcamrid in webcams)
                {
                    XElement mywebcam = new XElement("WebCam");
                    mywebcam.Add(new XAttribute("RID", mywebcamrid));

                    mywebcams.Add(mywebcam);
                }
            }

            return mywebcams;
        }

        public static XElement GetActivities(List<string> activities, List<string> activitydatatypes, List<string> activityenumcodes)
        {
            //Activity Filters 
            XElement myactivities = new XElement("Activities");

            foreach (string myactivitydatatype in activitydatatypes)
            {
                XElement activitytype = new XElement("Activity");
                XAttribute activitytypeID = new XAttribute("DataTypeID", myactivitydatatype);
                activitytype.Add(activitytypeID);

                myactivities.Add(activitytype);
            }



            if (activityenumcodes.Count > 0)
            {
                XElement myactivityenumcodes = new XElement("EnumCodes");

                foreach (string myenumcoderid in activityenumcodes)
                {
                    XElement myactivityenumcode = new XElement("EnumCode");
                    XAttribute myactivityenumcoderid = new XAttribute("RID", myenumcoderid);
                    myactivityenumcode.Add(myactivityenumcoderid);
                    myactivityenumcodes.Add(myactivityenumcode);
                }
            }

            if (activities.Count > 0)
            {
                foreach (string myactivity in activities)
                {
                    XElement activity = new XElement("Activity");
                    activity.Add(new XAttribute("RID", myactivity));

                    myactivities.Add(activity);
                }
            }

            return myactivities;
        }

        public static XElement GetActivities(List<string> activities, List<string> activitydatatypes, List<string> activityenumcodes, List<string> taggingtypeRIDs)
        {
            //Activity Filters 
            XElement myactivities = new XElement("Activities");

            //Werd obsolet
            //foreach (string myactivitydatatype in activitydatatypes)
            //{
            //    XElement activitytype = new XElement("Activity");
            //    XAttribute activitytypeID = new XAttribute("DataTypeID", myactivitydatatype);
            //    activitytype.Add(activitytypeID);

            //    myactivities.Add(activitytype);
            //}

            //Des muassi mochen weils sischt net geat
            if (activitydatatypes.Count == 0)
            {
                XElement activitytype = new XElement("Activity");
                XAttribute activitytypeID = new XAttribute("DataTypeID", "");
                activitytype.Add(activitytypeID);

                myactivities.Add(activitytype);
            }

            //Tagging Geschichte
            XElement tagging = new XElement("Tagging");
            XElement taggingtags = new XElement("Tags");

            foreach (string myactivityTaggingtyperid in taggingtypeRIDs)
            {
                XElement taggingtyperid = new XElement("Tag");
                XAttribute activitytaggingID = new XAttribute("RID", myactivityTaggingtyperid);
                taggingtyperid.Add(activitytaggingID);

                taggingtags.Add(taggingtyperid);
            }

            tagging.Add(taggingtags);
            myactivities.Add(tagging);


            //ENDE Tagging

            if (activityenumcodes.Count > 0)
            {
                XElement myactivityenumcodes = new XElement("EnumCodes");

                foreach (string myenumcoderid in activityenumcodes)
                {
                    XElement myactivityenumcode = new XElement("EnumCode");
                    XAttribute myactivityenumcoderid = new XAttribute("RID", myenumcoderid);
                    myactivityenumcode.Add(myactivityenumcoderid);
                    myactivityenumcodes.Add(myactivityenumcode);
                }
            }

            if (activities.Count > 0)
            {
                foreach (string myactivity in activities)
                {
                    XElement activity = new XElement("Activity");
                    activity.Add(new XAttribute("RID", myactivity));

                    myactivities.Add(activity);
                }
            }

            return myactivities;
        }

        public static XElement GetActivityPrefilters(List<string> activityprefilter)
        {
            XElement filters = new XElement("Filters");

            XElement myprefilters = new XElement("EnumCodes");
            if (activityprefilter.Count > 0)
            {
                int i = 1;
                foreach (string myprefiltercode in activityprefilter)
                {
                    XElement myitem = new XElement("Item");
                    XAttribute myitemID = new XAttribute("OrderID", i);
                    XAttribute myitemSelect = new XAttribute("Select", "1");
                    myitem.Add(myitemID, myitemSelect);

                    XElement myitemvalue = new XElement("ItemValue");
                    XAttribute myitemvalueRID = new XAttribute("RID", myprefiltercode);
                    myitemvalue.Add(myitemvalueRID);

                    myitem.Add(myitemvalue);

                    myprefilters.Add(myitem);
                    i++;
                }
            }

            filters.Add(myprefilters);

            return filters;
        }
    }
}
