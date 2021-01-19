using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.api
{
    public class EventShortHelper
    {
        public DateTime start;
        public DateTime end;
        public string? datetimeformat;
        public List<string> sourcelist;
        public List<string> eventlocationlist;
        public List<string> webaddresslist;
        public List<string> idlist;
        public string? activefilter;        
        public string? lastchange;
        public string? sortorder;

        public static EventShortHelper Create(
            string? startdate, string? enddate, string? datetimeformat, string? source,
            string? eventlocation, bool? onlyactive, string? eventids, string? webaddress,
            string? lastchange, string? sortorder)
        {
            return new EventShortHelper(startdate, enddate, datetimeformat, source, eventlocation, onlyactive, eventids, webaddress, lastchange, sortorder);
        }

        private EventShortHelper(
            string? startdate, string? enddate, string? datetimeformat, string? source,
            string? eventlocation, bool? onlyactive, string? eventids, string? webaddress,
            string? lastchange, string? sortorder)
        {            
            idlist = Helper.CommonListCreator.CreateIdList(eventids);
            sourcelist = Helper.CommonListCreator.CreateIdList(source);
            eventlocationlist = Helper.CommonListCreator.CreateIdList(eventlocation);
            webaddresslist = Helper.CommonListCreator.CreateIdList(webaddress);

            start = DateTime.MinValue;
            end = DateTime.MaxValue;

            if (String.IsNullOrEmpty(datetimeformat))
            {
                if (!String.IsNullOrEmpty(startdate))
                    start = Convert.ToDateTime(startdate);
                if (!String.IsNullOrEmpty(enddate))
                {
                    end = Convert.ToDateTime(enddate);
                    if (end.TimeOfDay == new TimeSpan(0, 0, 0))
                    {
                        end = end.AddDays(1);
                    }
                }
            }
            else if (datetimeformat == "uxtimestamp")
            {

                if (!String.IsNullOrEmpty(startdate))
                {
                    double startdatedb = Convert.ToDouble(startdate);
                    start = Helper.DateTimeHelper.UnixTimeStampToDateTimeMilliseconds(startdatedb);
                }

                if (!String.IsNullOrEmpty(enddate))
                {
                    double enddatedb = Convert.ToDouble(enddate);

                    end = Helper.DateTimeHelper.UnixTimeStampToDateTimeMilliseconds(enddatedb);
                    if (end.TimeOfDay == new TimeSpan(0, 0, 0))
                    {
                        end = end.AddDays(1);
                    }
                }
            }

            if (onlyactive == true)
                activefilter = "Y";
            else if (onlyactive == false)
                activefilter = "N";
            else
                activefilter = null;

            this.sortorder = sortorder;

            if(sortorder != null)
            {
                sortorder = sortorder.ToUpper();

                if (sortorder != "ASC" && sortorder != "DESC")
                    sortorder = "";
            }
            

            this.lastchange = lastchange;
        }
    }
}
