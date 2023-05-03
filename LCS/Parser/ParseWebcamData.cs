using DataModel;
using Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LCS
{
    public class ParseWebcamData
    {
        public static CultureInfo myculture = new CultureInfo("en");

        //Get the Webcam Detail Information
        public static WebcamInfo GetWebcamDetailLTS(string rid, WebcamInfo webcam, string ltsuser, string ltspswd, string ltsmsgpswd)
        {
            List<string> mywebcamlist = new List<string>();
            mywebcamlist.Add(rid);

            var mywebcamrequestde = GetLCSRequests.GetWebcamDetailRequest("de", "0", "0", "1", "1", "1", "1", "1", "1", "1", "1", "", "", "", mywebcamlist, "NOI", ltsmsgpswd);
            var mywebcamrequestit = GetLCSRequests.GetWebcamDetailRequest("it", "0", "0", "1", "1", "1", "1", "1", "1", "1", "1", "", "", "", mywebcamlist, "NOI", ltsmsgpswd);
            var mywebcamrequesten = GetLCSRequests.GetWebcamDetailRequest("en", "0", "0", "1", "1", "1", "1", "1", "1", "1", "1", "", "", "", mywebcamlist, "NOI", ltsmsgpswd);

            GetActivityDataLCS myactivitysearch = new GetActivityDataLCS(ltsuser, ltspswd);
            var myactivityresponsede = myactivitysearch.GetWebcamDetail(mywebcamrequestde);
            var myactivityresponseit = myactivitysearch.GetWebcamDetail(mywebcamrequestit);
            var myactivityresponseen = myactivitysearch.GetWebcamDetail(mywebcamrequesten);

            var thewebcamde = myactivityresponsede.WebCams.WebCam.FirstOrDefault();
            var thewebcamit = myactivityresponseit.WebCams.WebCam.FirstOrDefault();
            var thewebcamen = myactivityresponseen.WebCams.WebCam.FirstOrDefault();

            string name = "";

            if (thewebcamde.Name != null)
                name = thewebcamde.Name.FirstOrDefault().InnerText;
            else
                name = "unnamed";

            webcam.Shortname = name.Trim();
            webcam.Id = thewebcamde.RID;
            webcam.WebcamId = thewebcamde.RID;

            webcam.Active = Convert.ToBoolean(thewebcamde.News.Status.IsEnabled);
            webcam.SmgActive = webcam.SmgActive;

            //GeoData
            List<GpsInfo> mygpsinfolist = new List<GpsInfo>();

            var positionslist = thewebcamde.GeoDatas.GeoData.FirstOrDefault().Positions;

            if (positionslist != null)
            {
                var positions = positionslist.Position;

                if (positions != null)
                {
                    foreach (var position in positions)
                    {
                        GpsInfo mygpsinfo = new GpsInfo();

                        //mygpsinfo.Gpstype = position.EnumCodes.EnumCode.FirstOrDefault().Code.FirstOrDefault().Name.FirstOrDefault().InnerText;
                        mygpsinfo.AltitudeUnitofMeasure = "m";
                        mygpsinfo.Altitude = position.Altitude;
                        mygpsinfo.Longitude = Convert.ToDouble(position.Longitude, myculture);
                        mygpsinfo.Latitude = Convert.ToDouble(position.Latitude, myculture);
                        mygpsinfo.Gpstype = "position";

                        mygpsinfolist.Add(mygpsinfo);
                    }

                    webcam.GpsInfo = mygpsinfolist.FirstOrDefault();
                }
            }

            List<GpsTrack> mygpstracklist = new List<GpsTrack>();

            var gpstracklist = thewebcamde.GeoDatas.GeoData.FirstOrDefault().GPSTracks;

            if (gpstracklist != null)
            {
                var gpstracks = gpstracklist.GPSTrack;
                //TODO Language Specific GPXTrackdesc noch rausfieseln
                if (gpstracks != null)
                {
                    foreach (var gpstrack in gpstracks)
                    {
                        GpsTrack mygpstrack = new GpsTrack();

                        mygpstrack.Id = gpstrack.RID;
                        mygpstrack.GpxTrackUrl = gpstrack.File.URL.InnerText;

                        if (gpstrack.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText == "Übersicht")
                            mygpstrack.Type = "overview";
                        if (gpstrack.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText == "Datei zum herunterladen")
                            mygpstrack.Type = "detailed";


                        //EN und IT Info?

                        mygpstrack.GpxTrackDesc.TryAddOrUpdate("de", gpstrack.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText);

                        mygpstracklist.Add(mygpstrack);
                    }
                }
            }

            var gpstrackslistit = thewebcamit.GeoDatas.GeoData.FirstOrDefault().GPSTracks;

            if (gpstrackslistit != null)
            {
                var gpstracksit = gpstrackslistit.GPSTrack;
                //TODO Language Specific GPXTrackdesc noch rausfieseln
                if (gpstracksit != null)
                {
                    foreach (var gpstrack in gpstracksit)
                    {
                        GpsTrack mygpstrack = mygpstracklist.Where(x => x.Id == gpstrack.RID).FirstOrDefault();
                        mygpstrack.GpxTrackDesc.TryAddOrUpdate("it", gpstrack.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText);
                        //mygpstracklist.Add(mygpstrack);
                    }
                }
            }

            var gpstrackslisten = thewebcamen.GeoDatas.GeoData.FirstOrDefault().GPSTracks;

            if (gpstrackslisten != null)
            {
                var gpstracksen = gpstrackslisten.GPSTrack;
                //TODO Language Specific GPXTrackdesc noch rausfieseln
                if (gpstracksen != null)
                {
                    foreach (var gpstrack in gpstracksen)
                    {
                        GpsTrack mygpstrack = mygpstracklist.Where(x => x.Id == gpstrack.RID).FirstOrDefault();
                        mygpstrack.GpxTrackDesc.TryAddOrUpdate("en", gpstrack.EnumCodes.EnumCode.FirstOrDefault().Code.Where(x => x.Level == "1").FirstOrDefault().Name.FirstOrDefault().InnerText);
                        //mygpstracklist.Add(mygpstrack);
                    }
                }
            }

            //Not yet implemented
            //webcam.GpsTrack = mygpstracklist.ToList();

            //URls
            webcam.Webcamurl = thewebcamde.URL != null ? thewebcamde.URL.InnerText : "";
            webcam.Streamurl = thewebcamde.StreamURL != null ? thewebcamde.StreamURL.InnerText : "";
            webcam.Previewurl = thewebcamde.PreviewURL != null ? thewebcamde.PreviewURL.InnerText : "";

            //Name
            if (thewebcamde.Name != null)
                webcam.Webcamname.TryAddOrUpdate("de", thewebcamde.Name.FirstOrDefault().InnerText);
            if (thewebcamit.Name != null)
                webcam.Webcamname.TryAddOrUpdate("it", thewebcamit.Name.FirstOrDefault().InnerText);
            if (thewebcamen.Name != null)
                webcam.Webcamname.TryAddOrUpdate("en", thewebcamen.Name.FirstOrDefault().InnerText);

            if (webcam.FirstImport == null)
                webcam.FirstImport = DateTime.Now;

            webcam.LastChange = thewebcamde.News.Status != null ? Convert.ToDateTime(thewebcamde.News.Status.LastChange) : DateTime.MinValue;

            webcam.Source = "LTS";

            List<string> assignedareas = new List<string>();

            if (thewebcamde.Memberships != null && thewebcamde.Memberships.Membership != null)
            {
                foreach (var assignedarea in thewebcamde.Memberships.Membership)
                {
                    foreach (var assignedarearid in assignedarea.Area)
                    {
                        if (!String.IsNullOrEmpty(assignedarearid.RID))
                            assignedareas.Add(assignedarearid.RID);
                    }
                }
            }
            webcam.AreaIds = assignedareas;

            //hike.HasLanguage = availablelanguages;

            return webcam;
        }
    }
}
