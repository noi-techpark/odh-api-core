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

            if (data.Representation != null)
            {
                if (data.Representation > 0 && data.Active)
                {
                    isopendata = true;
                    licensetype = "CC0";
                }
            }

            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
        }

        public static LicenseInfo GetLicenseforAccommodationRoom(AccoRoom data)
        {
            var isopendata = false;
            var licensetype = "Closed";
            var licenseholder = @"https://www.hgv.it";

            if (data.Source == "LTS")
            {
                isopendata = true;
                licensetype = "CC0";
                licenseholder = @"https://www.lts.it";
            }

            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
        }

        public static LicenseInfo GetLicenseforActivity(PoiBaseInfos data)
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

            if (data.Active)
            {
                isopendata = true;
                licensetype = "CC0";
            }

            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
        }

        public static LicenseInfo GetLicenseforOdhActivityPoi(SmgPoi data)
        {
            var isopendata = false;
            var licensetype = "Closed";
            var licenseholder = @"https://www.lts.it";

            List<string> allowedsources = new List<string>() { "magnolia", "none", "museumdata", "suedtirolwein", "archapp", "activitydata", "poidata", "beacondata", "gastronomicdata", "common","sta" };

            if (data.Active)
            {
                if (allowedsources.Contains(data.SyncSourceInterface.ToLower()))
                {
                    isopendata = true;
                    licensetype = "CC0";

                    licenseholder = data.Source;

                    if (data.Source == "Content" || data.Source == "Magnolia" || data.Source == "Common")
                        licenseholder = @"https://www.idm-suedtirol.com";
                    if (data.Source == "SIAG")
                        licenseholder = "http://www.provinz.bz.it/kunst-kultur/museen";
                    if (data.Source == "ArchApp")
                        licenseholder = "https://stiftung.arch.bz.it";
                    if (data.Source == "Suedtirol Wein")
                        licenseholder = "https://www.suedtirolwein.com";
                    if (data.Source == "STA")
                        licenseholder = "https://www.sta.bz.it/";

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

            if (!data.attributes.categories.Contains("lts/visi_unpublishedOnODH"))
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

            if (data.Source == "Content")
            {
                licenseholder = @"https://noi.bz.it";
                isopendata = true;
                licensetype = "CC0";
            }

            return GetLicenseInfoobject(licensetype, "", licenseholder, !isopendata);
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
    }
}
