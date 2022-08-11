using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class LicenseHelper
    {
        public static LicenseInfo GetLicenseInfoobject(string licensetype, string author, string licenseholder, bool closeddata)
        {
            return new LicenseInfo() { Author = author, License = licensetype, LicenseHolder = licenseholder, ClosedData = closeddata };
        }

        //TODO Make a HOF and apply all the rules
        public static LicenseInfo GetLicenseInfoobject<T>(T myobject, Func<T, LicenseInfo> licensegenerator)
        {
            return licensegenerator(myobject);
        }

        public static LicenseInfo GetLicenseforAccommodation(Accommodation data)
        {
            var isopendata = false;
            var licensetype = "Closed";
            var licenseholder = @"https://www.lts.it";

            if (data.SmgActive)
            {
                isopendata = true;
                licensetype = "CC0";
            }           

            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
        }

        public static LicenseInfo GetLicenseforAccommodationRoom(AccoRoom data)
        {
            var isopendata = false;
            var licensetype = "Closed";
            var licenseholder = @"https://www.hgv.it";

            if (data.Source?.ToLower() == "lts")
            {
                isopendata = false;
                licensetype = "Closed";
                licenseholder = @"https://www.lts.it";
            }

            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
        }

        public static LicenseInfo GetLicenseforActivity(LTSActivity data)
        {
            var isopendata = false;
            var licensetype = "Closed";
            var licenseholder = @"https://www.lts.it";

            if (data.Active)
            {
                isopendata = true;
                licensetype = "CC0";
            }

            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
        }

        public static LicenseInfo GetLicenseforPoi(LTSPoi data)
        {
            var isopendata = false;
            var licensetype = "Closed";
            var licenseholder = @"https://www.lts.it";

            if (data.Active)
            {
                isopendata = true;
                licensetype = "CC0";
            }

            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
        }

        public static LicenseInfo GetLicenseforGastronomy(Gastronomy data)
        {
            var isopendata = false;
            var licensetype = "Closed";
            var licenseholder = @"https://www.lts.it";

            if (data.Active)
            {
                if (data.RepresentationRestriction > 0)
                {
                    isopendata = true;
                    licensetype = "CC0";
                }
            }

            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
        }

        public static LicenseInfo GetLicenseforEvent(Event data)
        {
            var isopendata = false;
            var licensetype = "Closed";
            var licenseholder = @"https://www.lts.it";

            if (data.Active && data.ClassificationRID == "CE212B488FA14954BE91BBCFA47C0F06")
            {
                isopendata = true;
                licensetype = "CC0";
            }

            //Source DRIN and CentroTrevi
            if(data.Source.ToLower() != "lts")
            {
                isopendata = true;
                licensetype = "CC0";

                if (data.Source.ToLower() == "trevilab")
                    licenseholder = @"https://www.provincia.bz.it/arte-cultura/cultura/centro-trevi.asp";
                if (data.Source.ToLower() == "drin")
                    licenseholder = @"https://www.provincia.bz.it/arte-cultura/giovani/drin.asp";
                else
                    licenseholder = "unknown";
            }

            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
        }

        public static LicenseInfo GetLicenseforOdhActivityPoi(SmgPoi data)
        {
            var isopendata = false;
            var licensetype = "Closed";
            var licenseholder = data.Source ?? "";

            if (data.Source == null)
                data.Source = "Content";

            if (data.Source.ToLower() == "content" || data.Source.ToLower() == "magnolia" || data.Source.ToLower() == "common")
                licenseholder = @"https://www.idm-suedtirol.com";
            if (data.Source.ToLower() == "siag")
                licenseholder = "http://www.provinz.bz.it/kunst-kultur/museen";
            if (data.Source.ToLower() == "archapp")
                licenseholder = "https://stiftung.arch.bz.it";
            if (data.Source.ToLower() == "suedtirolwein")
                licenseholder = "https://www.suedtirolwein.com";
            if (data.Source.ToLower() == "sta")
                licenseholder = "https://www.sta.bz.it/";
            if (data.Source.ToLower() == "lts")
                licenseholder = @"https://www.lts.it";
            if (data.Source.ToLower() == "dss")
                licenseholder = @"https://www.dolomitisuperski.com/";

            List<string?> allowedsources = new List<string?>() { "magnolia", "none", "museumdata", "suedtirolwein", "archapp", "activitydata", "poidata", "beacondata", "gastronomicdata", "common","sta", "dssliftbase", "dssslopebase" };

            if (data.Active)
            {
                if (allowedsources.Contains(data.SyncSourceInterface?.ToLower()))
                {
                    isopendata = true;
                    licensetype = "CC0";
                }
            }

            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
        }

        public static LicenseInfo GetLicenseforPackage(Package data)
        {
            var isopendata = false;
            var licensetype = "Closed";
            var licenseholder = @"https://www.hgv.it";

            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
        }

        public static LicenseInfo GetLicenseforMeasuringpoint(Measuringpoint data)
        {
            var isopendata = false;
            var licensetype = "Closed";
            var licenseholder = @"https://www.lts.it";

            if (data.Active)
            {
                isopendata = true;
                licensetype = "CC0";
            }

            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
        }

        public static LicenseInfo GetLicenseforWebcam(WebcamInfo data)
        {
            var isopendata = false;
            var licensetype = "Closed";
            var licenseholder = @"https://www.lts.it";

            if (data.Active == true)
            {
                isopendata = true;
                licensetype = ""; //licensetype = "CC0";
            }

            if(data.Source?.ToLower() == "content")
            {
                licenseholder = @"https://www.idm-suedtirol.com";
            }

            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
        }

        public static LicenseInfo GetLicenseforArticle(ArticleBaseInfos data)
        {
            var isopendata = false;
            var licensetype = "Closed";
            var licenseholder = @"https://www.idm-suedtirol.com";

            if (data.SmgActive == true)
            {
                isopendata = true;
                licensetype = "CC0";
            }

            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
        }

        public static LicenseInfo GetLicenseforVenue(DDVenue data)
        {
            var isopendata = false;
            var licensetype = "Closed";
            var licenseholder = @"https://www.lts.it";

            if (data.attributes.categories is { } && !data.attributes.categories.Contains("lts/visi_unpublishedOnODH") && data.attributes.categories.Contains("lts/visi_publishedOnODH"))
            {
                isopendata = true;
                licensetype = "CC0";
            }

            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
        }

        public static LicenseInfo GetLicenseforEventShort(EventShort data)
        {
            var isopendata = true;
            var licensetype = "CC0";
            var licenseholder = @"http://www.eurac.edu";
            var author = "";

            if (data.Source?.ToLower() == "content")
            {
                licenseholder = @"https://noi.bz.it";
                isopendata = true;
                licensetype = "CC0";
            }
            else if (data.Source?.ToLower() != "content" && data.Source?.ToLower() != "ebms")
            {
                licenseholder = @"https://noi.bz.it";
                isopendata = true;
                licensetype = "CC0";
                author = data.Source ?? "";
            }

            return GetLicenseInfoobject(licensetype, author, licenseholder, !isopendata);
        }

        public static LicenseInfo GetLicenseforBaseInfo(BaseInfos data)
        {
            var isopendata = false;
            var licensetype = "Closed";
            var licenseholder = @"https://www.idm-suedtirol.com/";

            if (data.SmgActive)
            {
                licenseholder = @"https://www.idm-suedtirol.com/";
                isopendata = true;
                licensetype = "CC0";
            }

            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
        }

        public static LicenseInfo GetLicenseforArea(Area data)
        {
            var isopendata = false;
            var licensetype = "Closed";
            var licenseholder = @"https://www.lts.it/";

            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
        }

        public static LicenseInfo GetLicenseforWineAward(Wine data)
        {
            var isopendata = false;
            var licensetype = "Closed";
            var licenseholder = "https://www.suedtirolwein.com";

            if (data.SmgActive)
            {
                isopendata = true;
                licensetype = "CC0";
            }

            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
        }

        public static LicenseInfo GetLicenseforWeather()
        {
            var isopendata = true;
            var licensetype = "";
            var licenseholder = "https://provinz.bz.it/wetter";

            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
        }

        public static void CheckLicenseInfoWithSource(LicenseInfo licenseinfo, string source, bool setcloseddatato)
        {
            if (source == "lts")
            {
                licenseinfo.ClosedData = setcloseddatato;
            }
        }
    }
}
