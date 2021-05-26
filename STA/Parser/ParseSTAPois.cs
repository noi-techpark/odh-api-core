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
            List<PoiProperty> mypoipropertylist = new List<PoiProperty>();
            
            PoiProperty pPhasWebsite = new PoiProperty() { Name = "haswebsite", Value = vendingpoint.Website };
            mypoipropertylist.Add(pPhasWebsite);

            PoiProperty pPsuedtirolpassdienste = new PoiProperty() { Name = "suedtirolpass_services", Value = vendingpoint.SuedtirolPassDienste };
            mypoipropertylist.Add(pPsuedtirolpassdienste);
            
            PoiProperty pPsuedtirolpassover65 = new PoiProperty() { Name = "suedtirolpass_over65_apply", Value = vendingpoint.SuedtirolPass65PlusBeantragung };
            mypoipropertylist.Add(pPsuedtirolpassover65);
            
            PoiProperty pPduplicate = new PoiProperty() { Name = "duplicate", Value = vendingpoint.Duplikat };
            mypoipropertylist.Add(pPduplicate);
            
            PoiProperty pPHwertkarte = new PoiProperty() { Name = "chargecard", Value = vendingpoint.Wertkarte };
            mypoipropertylist.Add(pPHwertkarte);
            
            PoiProperty pPstadtfahrkartecitybus = new PoiProperty() { Name = "city_card_bus", Value = vendingpoint.StadtfahrkarteoCitybus };
            mypoipropertylist.Add(pPstadtfahrkartecitybus);
            
            PoiProperty pPmobilcard = new PoiProperty() { Name = "mobilecard", Value = vendingpoint.Mobilcard };
            mypoipropertylist.Add(pPmobilcard);
            
            PoiProperty pPbikemobilcard = new PoiProperty() { Name = "bike_mobilecard", Value = vendingpoint.bikemobilCard };
            mypoipropertylist.Add(pPbikemobilcard);

            PoiProperty pPmobilecard = new PoiProperty() { Name = "museum_mobilecard", Value = vendingpoint.MuseumobilCard }; 
            mypoipropertylist.Add(pPmobilecard);

            mypoi.PoiProperty.TryAddOrUpdate("de", mypoipropertylist);
            mypoi.PoiProperty.TryAddOrUpdate("it", mypoipropertylist);
            mypoi.PoiProperty.TryAddOrUpdate("en", mypoipropertylist);
            mypoi.PoiProperty.TryAddOrUpdate("ld", mypoipropertylist);

            //End Properties Parsing

            //DETAIL Parsing

            Detail detailde = new Detail();
            detailde.Language = "de";
            detailde.Title = vendingpoint.Salepoint_Name_STA_DE;
            mypoi.Detail.TryAddOrUpdate("de", detailde);
       
            Detail detailit = new Detail();
            detailit.Language = "it";
            detailit.Title = vendingpoint.Salepoint_Name_STA_IT;
            mypoi.Detail.TryAddOrUpdate("it", detailit);

            Detail detailen = new Detail();
            detailen.Language = "en";
            detailen.Title = vendingpoint.Salepoint_Name_STA_EN;
            mypoi.Detail.TryAddOrUpdate("en", detailen);

            Detail detaillad = new Detail();
            detaillad.Language = "ld"; //ISO 639-3
            detaillad.Title = vendingpoint.Salepoint_Name_STA_LAD;
            mypoi.Detail.TryAddOrUpdate("ld", detaillad);

            //End DETAIL Parsing

            //Address Parsing
            ContactInfos contactInfosde = new ContactInfos();
            contactInfosde.CompanyName = vendingpoint.Salepoint_Name_STA_DE;
            contactInfosde.ZipCode = vendingpoint.CAP;
            contactInfosde.Address = vendingpoint.Adresse_DE;
            contactInfosde.City = vendingpoint.Stadt;
            contactInfosde.Language = "de";
            mypoi.ContactInfos.TryAddOrUpdate("de", contactInfosde);

            ContactInfos contactInfosit = new ContactInfos();
            contactInfosit.CompanyName = vendingpoint.Salepoint_Name_STA_IT;
            contactInfosit.ZipCode = vendingpoint.CAP;
            contactInfosit.Address = vendingpoint.Adresse_IT_EN_LAD;
            contactInfosit.City = vendingpoint.cittaIT_EN_LAD;
            contactInfosit.Language = "it";
            mypoi.ContactInfos.TryAddOrUpdate("it", contactInfosit);

            ContactInfos contactInfosen = new ContactInfos();
            contactInfosen.CompanyName = vendingpoint.Salepoint_Name_STA_EN;
            contactInfosen.ZipCode = vendingpoint.CAP;
            contactInfosen.Address = vendingpoint.Adresse_IT_EN_LAD;
            contactInfosen.City = vendingpoint.cittaIT_EN_LAD;
            contactInfosen.Language = "en";
            mypoi.ContactInfos.TryAddOrUpdate("en", contactInfosen);

            ContactInfos contactInfoslad = new ContactInfos();
            contactInfoslad.CompanyName = vendingpoint.Salepoint_Name_STA_LAD;
            contactInfoslad.ZipCode = vendingpoint.CAP;
            contactInfoslad.Address = vendingpoint.Adresse_IT_EN_LAD;
            contactInfoslad.City = vendingpoint.cittaIT_EN_LAD;
            contactInfoslad.Language = "ld";
            mypoi.ContactInfos.TryAddOrUpdate("ld", contactInfoslad);

            //END Address Parsing

            //TODO
            //OpeningTimes Parsing

            //END Openingtimes Parsing

            //TODO
            //Categorization                        

            //ODH Tags 
            mypoi.SmgTags = new List<string>();
            mypoi.SmgTags.Add("mobilität");
            mypoi.SmgTags.Add("sellingpoint");

            //ODH Categorizations



            mypoi.SyncSourceInterface = "sta";
            mypoi.SyncUpdateMode = "Full";
            mypoi.Source = "STA";

            mypoi.Active = true;
            mypoi.SmgActive = true;

            return mypoi;
        }
    }
}
