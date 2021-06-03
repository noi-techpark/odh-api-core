using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Helper;

namespace STA
{
    public class ParseSTAPois
    {
        public static ODHActivityPoiLinked ParseSTAVendingPointToODHActivityPoi(STAVendingPoint vendingpoint)
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

            var mypoi = new ODHActivityPoiLinked();            

            //ID
            var id = "salespoint_sta_" + vendingpoint.STA_ID;
            mypoi.Id = id;

            //GPSData
            var commaCulture = new CultureInfo("en")
            {
                NumberFormat =
                {
                    NumberDecimalSeparator = ","
                }
            };

            double gpslat = !String.IsNullOrEmpty(vendingpoint.latitude) ? Convert.ToDouble(vendingpoint.latitude, commaCulture) : 0;
            double gpslong = !String.IsNullOrEmpty(vendingpoint.longitude) ? Convert.ToDouble(vendingpoint.longitude, commaCulture) : 0;

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

            if (!String.IsNullOrEmpty(vendingpoint.Zusatzinfo_DE))
                detailde.AdditionalText = vendingpoint.Zusatzinfo_DE;

            mypoi.Detail.TryAddOrUpdate("de", detailde);
       
            Detail detailit = new Detail();
            detailit.Language = "it";
            detailit.Title = vendingpoint.Salepoint_Name_STA_IT;

            if (!String.IsNullOrEmpty(vendingpoint.Zusatzinfo_IT))
                detailde.AdditionalText = vendingpoint.Zusatzinfo_IT;

            mypoi.Detail.TryAddOrUpdate("it", detailit);

            Detail detailen = new Detail();
            detailen.Language = "en";
            detailen.Title = vendingpoint.Salepoint_Name_STA_EN;

            if (!String.IsNullOrEmpty(vendingpoint.Zusatzinfo_EN))
                detailde.AdditionalText = vendingpoint.Zusatzinfo_EN;

            mypoi.Detail.TryAddOrUpdate("en", detailen);

            Detail detaillad = new Detail();
            detaillad.Language = "ld"; //ISO 639-3
            detaillad.Title = vendingpoint.Salepoint_Name_STA_LAD;

            if (!String.IsNullOrEmpty(vendingpoint.Zusatzinfo_LAD))
                detailde.AdditionalText = vendingpoint.Zusatzinfo_LAD;

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

            //Standard open all year? Make use of DB with Festive days and add them as closed?
            var operationschedule = ParseOperationScheduleFromCSV(vendingpoint);

            if(operationschedule != null)
            {
                mypoi.OperationSchedule = new List<OperationSchedule>();

                mypoi.OperationSchedule.Add(operationschedule);
            }

            //END Openingtimes Parsing

            //TODO
            //Categorization                        

            //ODH Tags 
            mypoi.SmgTags = new List<string>();
            mypoi.SmgTags.Add("mobilität");
            mypoi.SmgTags.Add("verkaufstellen ticket oeffentliche verkehrsmittel");

            mypoi.Type = "Mobilität";
            mypoi.SubType = "Verkaufstellen Ticket Oeffentliche Verkehrsmittel";

            //ODH Categorizations
            //TODO LOAD Categorizations 


            mypoi.SyncSourceInterface = "sta";
            mypoi.SyncUpdateMode = "Full";
            mypoi.Source = "STA";

            mypoi.Active = true;
            mypoi.SmgActive = true;

            mypoi.HasLanguage = new List<string>() { "de","it","en","ld" };

            mypoi.Shortname = mypoi.Detail["de"].Title;

            mypoi.LastChange = DateTime.Now;

            return mypoi;
        }

        private static OperationSchedule ParseOperationScheduleFromCSV(STAVendingPoint vendingpoint)
        {
            if (!String.IsNullOrEmpty(vendingpoint.Wochentags_Beginn) || !String.IsNullOrEmpty(vendingpoint.Samstag_Beginn) || !String.IsNullOrEmpty(vendingpoint.Sonntag_Beginn))
            {
                OperationSchedule myoperationschedule = new OperationSchedule();
                myoperationschedule.OperationscheduleName = new Dictionary<string, string>()
                {
                    { "de", "Öffnungszeiten" },
                    { "it", "orario d'apertura" },
                    { "en", "Opening time" }
                };
                myoperationschedule.Start = new DateTime(2021, 1, 1);
                myoperationschedule.Stop = new DateTime(2021, 12, 31);
                myoperationschedule.Type = "2";

                ////Try Parsing Columns
                TimeSpan beginweekday = default(TimeSpan);
                TimeSpan.TryParseExact(vendingpoint.Wochentags_Beginn, "HH:mm:ss", CultureInfo.InvariantCulture, out beginweekday);

                TimeSpan endweekdaynoon = default(TimeSpan);
                TimeSpan.TryParseExact(vendingpoint.Pause_Start, "HH:mm:ss", CultureInfo.InvariantCulture, out endweekdaynoon);

                TimeSpan beginweekdaynoon = default(TimeSpan);
                TimeSpan.TryParseExact(vendingpoint.Pause_Ende, "HH:mm:ss", CultureInfo.InvariantCulture, out beginweekdaynoon);

                TimeSpan endweekday = default(TimeSpan);
                TimeSpan.TryParseExact(vendingpoint.Wochentags_Ende, "HH:mm:ss", CultureInfo.InvariantCulture, out endweekday);

                //Saturday

                TimeSpan beginsaturday = default(TimeSpan);
                TimeSpan.TryParseExact(vendingpoint.Samstag_Beginn, "HH:mm:ss", CultureInfo.InvariantCulture, out beginsaturday);

                TimeSpan endsaturdaynoon = default(TimeSpan);
                TimeSpan.TryParseExact(vendingpoint.Pause_Samstag_Beginn, "HH:mm:ss", CultureInfo.InvariantCulture, out endsaturdaynoon);

                TimeSpan beginsaturdaynoon = default(TimeSpan);
                TimeSpan.TryParseExact(vendingpoint.Pause_Samstag_Ende, "HH:mm:ss", CultureInfo.InvariantCulture, out beginsaturdaynoon);

                TimeSpan endsaturday = default(TimeSpan);
                TimeSpan.TryParseExact(vendingpoint.Samstag_Ende, "HH:mm:ss", CultureInfo.InvariantCulture, out endsaturday);

                //Sunday

                TimeSpan beginsunday = default(TimeSpan);
                TimeSpan.TryParseExact(vendingpoint.Sonntag_Beginn, "HH:mm:ss", CultureInfo.InvariantCulture, out beginsunday);

                TimeSpan endsundaynoon = default(TimeSpan);
                TimeSpan.TryParseExact(vendingpoint.Pause_Sonntag_Beginn, "HH:mm:ss", CultureInfo.InvariantCulture, out endsundaynoon);

                TimeSpan beginsundaynoon = default(TimeSpan);
                TimeSpan.TryParseExact(vendingpoint.Pause_Sonntag_Ende, "HH:mm:ss", CultureInfo.InvariantCulture, out beginsundaynoon);

                TimeSpan endsunday = default(TimeSpan);
                TimeSpan.TryParseExact(vendingpoint.Sonntag_Ende, "HH:mm:ss", CultureInfo.InvariantCulture, out endsunday);

                myoperationschedule.OperationScheduleTime = new List<OperationScheduleTime>();

                //Add openingtime for Weekday, Saturday and Sunday

                if (beginweekday!= TimeSpan.Zero && endweekday != TimeSpan.Zero)
                {                
                    //If no pause
                    if (beginweekdaynoon == TimeSpan.Zero || endweekdaynoon == TimeSpan.Zero)
                    {
                        OperationScheduleTime myoptime = new OperationScheduleTime();
                        myoptime.Monday = true;
                        myoptime.Tuesday = true;
                        myoptime.Wednesday = true;
                        myoptime.Thuresday = true;
                        myoptime.Thursday = true;
                        myoptime.Friday = true;
                        myoptime.Saturday = false;
                        myoptime.Sunday = false;

                        myoptime.Timecode = 1;
                        myoptime.State = 2;

                        myoptime.Start = beginweekday;
                        myoptime.End = endweekday;

                        myoperationschedule.OperationScheduleTime.Add(myoptime);
                    }
                    else
                    {
                        OperationScheduleTime myoptime1 = new OperationScheduleTime();
                        myoptime1.Monday = true;
                        myoptime1.Tuesday = true;
                        myoptime1.Wednesday = true;
                        myoptime1.Thuresday = true;
                        myoptime1.Thursday = true;
                        myoptime1.Friday = true;
                        myoptime1.Saturday = false;
                        myoptime1.Sunday = false;

                        myoptime1.Timecode = 1;
                        myoptime1.State = 2;

                        myoptime1.Start = beginweekday;
                        myoptime1.End = endweekdaynoon;

                        myoperationschedule.OperationScheduleTime.Add(myoptime1);

                        OperationScheduleTime myoptime2 = new OperationScheduleTime();
                        myoptime2.Monday = true;
                        myoptime2.Tuesday = true;
                        myoptime2.Wednesday = true;
                        myoptime2.Thuresday = true;
                        myoptime2.Thursday = true;
                        myoptime2.Friday = true;
                        myoptime2.Saturday = false;
                        myoptime2.Sunday = false;

                        myoptime2.Timecode = 1;
                        myoptime2.State = 2;

                        myoptime2.Start = beginweekdaynoon;
                        myoptime2.End = endweekday;

                        myoperationschedule.OperationScheduleTime.Add(myoptime2);
                    }
                }
                //SATURDAY
                if (beginsaturday != TimeSpan.Zero && endsaturday != TimeSpan.Zero)
                {
                    //If no pause
                    if (beginsaturdaynoon == TimeSpan.Zero || endsaturdaynoon == TimeSpan.Zero)
                    {
                        OperationScheduleTime myoptime = new OperationScheduleTime();
                        myoptime.Monday = false;
                        myoptime.Tuesday = false;
                        myoptime.Wednesday = false;
                        myoptime.Thuresday = false;
                        myoptime.Thursday = false;
                        myoptime.Friday = false;
                        myoptime.Saturday = true;
                        myoptime.Sunday = false;

                        myoptime.Timecode = 1;
                        myoptime.State = 2;

                        myoptime.Start = beginsaturday;
                        myoptime.End = endsaturday;

                        myoperationschedule.OperationScheduleTime.Add(myoptime);
                    }
                    else
                    {
                        OperationScheduleTime myoptime1 = new OperationScheduleTime();
                        myoptime1.Monday = false;
                        myoptime1.Tuesday = false;
                        myoptime1.Wednesday = false;
                        myoptime1.Thuresday = false;
                        myoptime1.Thursday = false;
                        myoptime1.Friday = false;
                        myoptime1.Saturday = true;
                        myoptime1.Sunday = false;

                        myoptime1.Timecode = 1;
                        myoptime1.State = 2;

                        myoptime1.Start = beginsaturday;
                        myoptime1.End = endsaturdaynoon;

                        myoperationschedule.OperationScheduleTime.Add(myoptime1);

                        OperationScheduleTime myoptime2 = new OperationScheduleTime();
                        myoptime2.Monday = false;
                        myoptime2.Tuesday = false;
                        myoptime2.Wednesday = false;
                        myoptime2.Thuresday = false;
                        myoptime2.Thursday = false;
                        myoptime2.Friday = false;
                        myoptime2.Saturday = true;
                        myoptime2.Sunday = false;

                        myoptime2.Timecode = 1;
                        myoptime2.State = 2;

                        myoptime2.Start = beginsaturdaynoon;
                        myoptime2.End = endsaturday;

                        myoperationschedule.OperationScheduleTime.Add(myoptime2);
                    }
                }

                if (beginsunday != TimeSpan.Zero && endsunday != TimeSpan.Zero)
                {
                    //If no pause
                    if (beginsaturdaynoon == TimeSpan.Zero || endsaturdaynoon == TimeSpan.Zero)
                    {
                        OperationScheduleTime myoptime = new OperationScheduleTime();
                        myoptime.Monday = false;
                        myoptime.Tuesday = false;
                        myoptime.Wednesday = false;
                        myoptime.Thuresday = false;
                        myoptime.Thursday = false;
                        myoptime.Friday = false;
                        myoptime.Saturday = true;
                        myoptime.Sunday = false;

                        myoptime.Timecode = 1;
                        myoptime.State = 2;

                        myoptime.Start = beginsunday;
                        myoptime.End = endsunday;

                        myoperationschedule.OperationScheduleTime.Add(myoptime);
                    }
                    else
                    {
                        OperationScheduleTime myoptime1 = new OperationScheduleTime();
                        myoptime1.Monday = false;
                        myoptime1.Tuesday = false;
                        myoptime1.Wednesday = false;
                        myoptime1.Thuresday = false;
                        myoptime1.Thursday = false;
                        myoptime1.Friday = false;
                        myoptime1.Saturday = false;
                        myoptime1.Sunday = true;

                        myoptime1.Timecode = 1;
                        myoptime1.State = 2;

                        myoptime1.Start = beginsunday;
                        myoptime1.End = endsundaynoon;

                        myoperationschedule.OperationScheduleTime.Add(myoptime1);

                        OperationScheduleTime myoptime2 = new OperationScheduleTime();
                        myoptime2.Monday = false;
                        myoptime2.Tuesday = false;
                        myoptime2.Wednesday = false;
                        myoptime2.Thuresday = false;
                        myoptime2.Thursday = false;
                        myoptime2.Friday = false;
                        myoptime2.Saturday = false;
                        myoptime2.Sunday = true;

                        myoptime2.Timecode = 1;
                        myoptime2.State = 2;

                        myoptime2.Start = beginsundaynoon;
                        myoptime2.End = endsunday;

                        myoperationschedule.OperationScheduleTime.Add(myoptime2);
                    }
                }

                //Exceptions, Closed on a certain day?
                //How tho handle exceptions like 


                return myoperationschedule;
            }
            else
                return null;
        }
    }
}
