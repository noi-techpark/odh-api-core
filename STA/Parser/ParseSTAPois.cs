using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Helper;

namespace STA
{
    public class ParseSTAPois
    {
        public static SmgPoiLinked ParseSTAVendingPointToODHActivityPoi(STAVendingPoint vendingpoint)
        {
            //Integrate this data
            //Website
            //STA_ID
            //Salepoint Name DE
            //Salepoint Name IT
            //Salepoint Name EN
            //Salepoint Name LAD(im Moment noch gleich mit IT)
            //Adresse DE
            //Adresse IT/ EN / LAD
            //CAP
            //Stadt
            //Città
            //Öffnungszeiten(von AD bis AT)
            //Lat
            //Long
            //Angebot von Leistungen(von AW bis BD)

            var mypoi = new SmgPoiLinked();            

            //ID
            var id = "salespoint_sta_" + vendingpoint.STA_ID;
            mypoi.Id = id;

            //GPSData

            double gpslat = !String.IsNullOrEmpty(vendingpoint.latitude) ? Convert.ToDouble(vendingpoint.latitude) : 0;
            double gpslong = !String.IsNullOrEmpty(vendingpoint.longitude) ? Convert.ToDouble(vendingpoint.longitude) : 0;

            if (gpslat != 0 && gpslong != 0)
            {
                GpsInfo gpsinfo = new GpsInfo();
                gpsinfo.Gpstype = "position";
                gpsinfo.Latitude = gpslat;
                gpsinfo.Longitude = gpslong;

                mypoi.GpsPoints.TryAddOrUpdate(gpsinfo.Gpstype, gpsinfo);

                if (mypoi.GpsInfo == null)
                    mypoi.GpsInfo = new List<GpsInfo>();
                mypoi.GpsInfo.Add(gpsinfo);
            }

            //END GPsData

            //Properties Parsing
            bool haswebsite = false;
            bool.TryParse(vendingpoint.Website, out haswebsite);

            bool suedtirolpassdienste = false;
            bool.TryParse(vendingpoint.SuedtirolPassDienste, out suedtirolpassdienste);

            bool suedtirolpassplus65 = false;
            bool.TryParse(vendingpoint.SuedtirolPass65PlusBeantragung, out suedtirolpassplus65);

            bool duplikat = false;
            bool.TryParse(vendingpoint.Duplikat, out duplikat);

            bool wertkarte = false;
            bool.TryParse(vendingpoint.Wertkarte, out wertkarte);

            bool stadtfahrkarte = false;
            bool.TryParse(vendingpoint.StadtfahrkarteoCitybus, out stadtfahrkarte);

            bool mobilcard = false;
            bool.TryParse(vendingpoint.Mobilcard, out mobilcard);

            bool bikemobilCard = false;
            bool.TryParse(vendingpoint.bikemobilCard, out bikemobilCard);

            bool museummobilcard = false;
            bool.TryParse(vendingpoint.MuseumobilCard, out museummobilcard);

            //End Properties Parsing




            return new SmgPoiLinked();
        }
    }
}
