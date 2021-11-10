﻿using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SIAG;
using DataModel;
using Newtonsoft.Json;
using System.Threading;
using System.Xml.Linq;
using Helper;

namespace OdhApiImporter.Helpers
{
    public class SIAGImportHelper
    {
        private readonly QueryFactory QueryFactory;
        private readonly ISettings settings;

        public SIAGImportHelper(ISettings settings, QueryFactory queryfactory)
        {
            this.QueryFactory = queryfactory;
            this.settings = settings;
        }

        #region SIAG Weather

        /// <summary>
        /// Save Weather to History Table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<UpdateDetail> SaveWeatherToHistoryTable(CancellationToken cancellationToken, string? id = null)
        {
            string? weatherresponsetaskde = "";
            string? weatherresponsetaskit = "";
            string? weatherresponsetasken = "";

            if (!String.IsNullOrEmpty(id))
            {
                weatherresponsetaskde = await SIAG.GetWeatherData.GetSiagWeatherData("de", settings.SiagConfig.Username, settings.SiagConfig.Password, true, id);
                weatherresponsetaskit = await SIAG.GetWeatherData.GetSiagWeatherData("it", settings.SiagConfig.Username, settings.SiagConfig.Password, true, id);
                weatherresponsetasken = await SIAG.GetWeatherData.GetSiagWeatherData("en", settings.SiagConfig.Username, settings.SiagConfig.Password, true, id);
            }
            else
            {
                weatherresponsetaskde = await SIAG.GetWeatherData.GetSiagWeatherData("de", settings.SiagConfig.Username, settings.SiagConfig.Password, true);
                weatherresponsetaskit = await SIAG.GetWeatherData.GetSiagWeatherData("it", settings.SiagConfig.Username, settings.SiagConfig.Password, true);
                weatherresponsetasken = await SIAG.GetWeatherData.GetSiagWeatherData("en", settings.SiagConfig.Username, settings.SiagConfig.Password, true);
            }

            if (!String.IsNullOrEmpty(weatherresponsetaskde) && !String.IsNullOrEmpty(weatherresponsetaskit) && !String.IsNullOrEmpty(weatherresponsetasken))
            {
                //Save all Responses to rawdata table

                var siagweatherde = JsonConvert.DeserializeObject<SIAG.WeatherModel.SiagWeather>(weatherresponsetaskde);
                var siagweatherit = JsonConvert.DeserializeObject<SIAG.WeatherModel.SiagWeather>(weatherresponsetaskit);
                var siagweatheren = JsonConvert.DeserializeObject<SIAG.WeatherModel.SiagWeather>(weatherresponsetasken);

                RawDataStore rawData = new RawDataStore();
                rawData.importdate = DateTime.Now;
                rawData.type = "weather";
                rawData.sourceid = siagweatherde?.id.ToString() ?? "";
                rawData.datasource = "siag";
                rawData.sourceinterface = "weatherbulletin";
                rawData.sourceurl = "http://daten.buergernetz.bz.it/services/weather/bulletin";
                rawData.raw = JsonConvert.SerializeObject(new { de = siagweatherde, it = siagweatherit, en = siagweatheren });

                var insertresultraw = await QueryFactory.Query("rawdata")
                      .InsertGetIdAsync<int>(rawData);

                //Save parsed Response to measurement history table
                var odhweatherresultde = await SIAG.GetWeatherData.ParseSiagWeatherDataToODHWeather("de", settings.XmlConfig.XmldirWeather, weatherresponsetaskde, true);
                var odhweatherresultit = await SIAG.GetWeatherData.ParseSiagWeatherDataToODHWeather("it", settings.XmlConfig.XmldirWeather, weatherresponsetaskit, true);
                var odhweatherresulten = await SIAG.GetWeatherData.ParseSiagWeatherDataToODHWeather("en", settings.XmlConfig.XmldirWeather, weatherresponsetasken, true);

                //Insert into Measuringhistorytable
                //var insertresultde = await QueryFactory.Query("weatherdatahistory")
                //      .InsertAsync(new JsonBDataRaw { id = odhweatherresultde.Id + "_de", data = new JsonRaw(odhweatherresultde), raw = weatherresponsetaskde });
                //var insertresultit = await QueryFactory.Query("weatherdatahistory")
                //      .InsertAsync(new JsonBDataRaw { id = odhweatherresultde.Id + "_it", data = new JsonRaw(odhweatherresultit), raw = weatherresponsetaskit });
                //var insertresulten = await QueryFactory.Query("weatherdatahistory")
                //      .InsertAsync(new JsonBDataRaw { id = odhweatherresultde.Id + "_en", data = new JsonRaw(odhweatherresulten), raw = weatherresponsetasken });

                var myweatherhistory = new WeatherHistory();
                myweatherhistory.Id = odhweatherresultde.Id.ToString();
                myweatherhistory.Weather.Add("de", odhweatherresultde);
                myweatherhistory.Weather.Add("it", odhweatherresultit);
                myweatherhistory.Weather.Add("en", odhweatherresulten);
                myweatherhistory.LicenseInfo = Helper.LicenseHelper.GetLicenseforWeather();
                myweatherhistory.FirstImport = DateTime.Now;
                myweatherhistory.HasLanguage = new List<string>() { "de", "it", "en" };
                myweatherhistory.LastChange = odhweatherresultde.date;
                myweatherhistory.Shortname = odhweatherresultde.evolutiontitle;

                var insertresult = await QueryFactory.Query("weatherdatahistory")
                      .InsertAsync(new JsonBDataRaw { id = odhweatherresultde.Id.ToString(), data = new JsonRaw(myweatherhistory), rawdataid = insertresultraw });

                ////Save to PG
                ////Check if data exists                    
                //var result = await QueryFactory.UpsertData<ODHActivityPoi>(odhactivitypoi!, "weatherdatahistory", insertresultraw);

                return new UpdateDetail() { created = insertresult, updated = 0, deleted = 0 };                    
            }
            else
                throw new Exception("No weatherdata received from source!");
        }

        #endregion

        #region SIAG Museumdata

        public async Task<UpdateDetail> SaveMuseumsToODH(QueryFactory QueryFactory, DateTime? lastchanged = null, CancellationToken cancellationToken = default)
        {
            var museumslist = await ImportMuseumlist(cancellationToken);
            var updateresult = await ImportMuseums(museumslist, cancellationToken);
            //SetMuseumsnotinListToInactive()

            return new UpdateDetail();
        }

        private async Task<XDocument> ImportMuseumlist(CancellationToken cancellationToken)
        {
            var myxml = await SIAG.GetMuseumFromSIAG.GetMuseumList();

            XDocument mymuseumlist = new XDocument();
            XElement mymuseums = new XElement("Museums");

            XNamespace ns = "http://service.kks.siag";
            XNamespace ax211 = "http://data.service.kks.siag/xsd";

            var mymuseumlist2 = myxml.Root.Element(ns + "return").Elements(ax211 + "museums");

            foreach (XElement idtoimport in mymuseumlist2)
            {
                XElement mymuseum = new XElement("Museum");
                mymuseum.Add(new XAttribute("ID", idtoimport.Element(ax211 + "museId").Value));
                mymuseum.Add(new XAttribute("PLZ", idtoimport.Element(ax211 + "plz").Value));

                mymuseums.Add(mymuseum);
            }

            mymuseumlist.Add(mymuseums);

            //check if there is the need to save it
            //mymuseumlist.Save(Constants.xmldir + "MuseumList.xml");

            return mymuseumlist;
        }

        private async Task<UpdateDetail> ImportMuseums(XDocument mymuseumlist, CancellationToken cancellationToken)
        {
            string museumid = "";
           
                XElement mymuseumroot = mymuseumlist.Root;

                XNamespace ns = "http://service.kks.siag";

                //.Where(x => x.Attribute("ID").Value == "963")
                int updatecounter = 0;
                int newcounter = 0;

                //Load ValidTagsfor Categories
                var validtagsforcategories = default(IEnumerable<SmgTags>);
                //For AdditionalInfos
                List<string> languagelistcategories = new List<string>() { "de", "it", "en", "nl", "cs", "pl", "fr", "ru" };
                
                //Getting valid Tags for Museums
                validtagsforcategories = await ODHTagHelper.GetODHTagsValidforTranslations(QueryFactory, new List<string>() { "Kultur Sehenswürdigkeiten" });

            foreach (XElement mymuseumelement in mymuseumroot.Elements("Museum"))
            {
                museumid = mymuseumelement.Attribute("ID").Value;
                string plz = mymuseumelement.Attribute("PLZ").Value;

                //Import Museum data from Siag
                var mymuseumdata = await SIAG.GetMuseumFromSIAG.GetMuseumDetail(museumid);
                var mymuseumxml = mymuseumdata.Root.Element(ns + "return");


                var mymuseumquery = QueryFactory.Query("smgpois")
                    .WhereRaw("data->>'CustomId' = ?", museumid.ToLower());

                var mymuseum = await mymuseumquery.GetFirstOrDefaultAsObject<ODHActivityPoiLinked>();

                mymuseum.Source = "SIAG";
                mymuseum.SyncSourceInterface = "museumdata";
                mymuseum.SyncUpdateMode = "Full";
                mymuseum.LastChange = DateTime.Now;

                if (mymuseum == null)
                {

                    //Neuen Datensatz
                    mymuseum = new ODHActivityPoiLinked();
                    mymuseum.FirstImport = DateTime.Now;

                    XNamespace ax211 = "http://data.service.kks.siag/xsd";

                    string siagid = mymuseumxml.Element(ax211 + "museId").Value;      
                    string gemeindeid = mymuseumxml.Element(ax211 + "gemeindeId").Value;

                    mymuseum.Id = "smgpoi" + siagid + "siag";
                    mymuseum.CustomId = siagid;
                    mymuseum.Active = true;
                    mymuseum.SmgActive = true;

                    mymuseum.FirstImport = DateTime.Now;

                    SIAG.Parser.ParseMuseum.ParseMuseumToPG(mymuseum, mymuseumxml, plz);

                    mymuseum.Shortname = mymuseum.Detail["de"].Title.Trim();

                    //Suedtirol Type laden
                    var mysmgmaintype = await ODHTagHelper.GeODHTagByID(QueryFactory, "Kultur Sehenswürdigkeiten");
                    var mysmgsubtype = await ODHTagHelper.GeODHTagByID(QueryFactory, "Museen"); 
                    var mysmgpoipoitype = new List<SmgTags>();

                    List<string> museumskategorien = new List<string>();
                    var mymuseumscategoriesstrings = mymuseum.PoiProperty["de"].Where(x => x.Name == "categories").Select(x => x.Value).ToList();
                    foreach (var mymuseumscategoriesstring in mymuseumscategoriesstrings)
                    {
                        var splittedlist = mymuseumscategoriesstring.Split(',').ToList();

                        foreach (var splitted in splittedlist)
                        {
                            museumskategorien.Add(splitted.Trim());

                            var mykategoriequery = await ODHTagHelper.GeODHTagByID(QueryFactory, "Museen " + splitted.Trim());
                            mysmgpoipoitype.Add(mykategoriequery);
                        }
                    }

                    mymuseum.Type = mysmgmaintype.Shortname;
                    mymuseum.SubType = mysmgsubtype.Shortname;

                    List<string> mysmgtags = mymuseum.SmgTags.ToList();

                    if (!mysmgtags.Contains(mysmgmaintype.Id))
                        mysmgtags.Add(mysmgmaintype.Id);

                    if (!mysmgtags.Contains(mysmgsubtype.Id))
                        mysmgtags.Add(mysmgsubtype.Id);

                    if (mysmgpoipoitype.Count > 0)
                    {
                        foreach (var mysmgpoipoitypel in mysmgpoipoitype)
                        {
                            if (!mysmgtags.Contains(mysmgpoipoitypel.Id))
                                mysmgtags.Add(mysmgpoipoitypel.Id);
                        }

                    }
                    mymuseum.SmgTags = mysmgtags.ToList();

                    if (mysmgpoipoitype.Count > 0)
                        mymuseum.PoiType = mysmgpoipoitype.FirstOrDefault().Shortname;
                    else
                        mymuseum.PoiType = "";

                    List<string> haslanguagelist = new List<string>();

                    haslanguagelist.Add("de");
                    haslanguagelist.Add("it");
                    haslanguagelist.Add("en");

                    mymuseum.HasLanguage = haslanguagelist.ToList();

                    foreach (var langcat in languagelistcategories)
                    {
                        AdditionalPoiInfos additional = new AdditionalPoiInfos();
                        additional.Language = langcat;
                        additional.MainType = mysmgmaintype.TagName[langcat];
                        additional.SubType = mysmgsubtype.TagName[langcat];
                        additional.PoiType = mysmgpoipoitype.Count > 0 ? mysmgpoipoitype.FirstOrDefault().TagName[langcat] : "";
                        mymuseum.AdditionalPoiInfos.TryAddOrUpdate(langcat, additional);
                    }

                    //Setting Categorization by Valid Tags
                    var currentcategories = validtagsforcategories.Where(x => mymuseum.SmgTags.Contains(x.Id));

                    foreach (var smgtagtotranslate in currentcategories)
                    {
                        foreach (var languagecategory in languagelistcategories)
                        {
                            if (mymuseum.AdditionalPoiInfos[languagecategory].Categories == null)
                                mymuseum.AdditionalPoiInfos[languagecategory].Categories = new List<string>();

                            if (smgtagtotranslate.TagName.ContainsKey(languagecategory) && !mymuseum.AdditionalPoiInfos[languagecategory].Categories.Contains(smgtagtotranslate.TagName[languagecategory].Trim()))
                                mymuseum.AdditionalPoiInfos[languagecategory].Categories.Add(smgtagtotranslate.TagName[languagecategory].Trim());
                        }
                    }

         
                    //Locationinfo aus GPS Punkte holen

                    if (mymuseum.GpsInfo != null && mymuseum.GpsInfo.Count > 0)
                    {
                        if (mymuseum.GpsInfo.FirstOrDefault().Latitude != 0 && mymuseum.GpsInfo.FirstOrDefault().Longitude != 0)
                        {
                            var district = await GetLocationInfo.GetNearestDistrictbyGPS(QueryFactory, mymuseum.GpsInfo.FirstOrDefault().Latitude, mymuseum.GpsInfo.FirstOrDefault().Longitude, 30000);

                            if(district != null)
                            {
                                var locinfo = await GetLocationInfo.GetTheLocationInfoDistrict(QueryFactory, district.Id);

                                mymuseum.LocationInfo = locinfo;
                                mymuseum.TourismorganizationId = locinfo.TvInfo.Id;
                            }
                        }                       
                    }
                    
                    //If still no locinfo assigned
                    if(mymuseum.LocationInfo == null)
                    {
                        if (gemeindeid.StartsWith("3"))
                            mymuseum.LocationInfo = await GetLocationInfo.GetTheLocationInfoMunicipality_Siag(QueryFactory, gemeindeid);
                        if (gemeindeid.StartsWith("8"))
                            mymuseum.LocationInfo = await GetLocationInfo.GetTheLocationInfoDistrict_Siag(QueryFactory, gemeindeid);

                        mymuseum.TourismorganizationId = mymuseum.LocationInfo.TvInfo.Id;
                    }                   
                }
                else
                {                 
                    //mymuseum.CustomId = siagid;
                    mymuseum.Active = true;
                    //mymuseum.SmgActive = true;                 
                    
                    SIAG.Parser.ParseMuseum.ParseMuseumToPG(mymuseum, mymuseumxml, plz);
                    
                    string subtype = "Museen";
                    if (mymuseum.SubType == "Bergwerke")
                        subtype = "Bergwerke";
                    if (mymuseum.SubType == "Naturparkhäuser")
                        subtype = "Naturparkhäuser";

                    //Suedtirol Type laden
                    var mysmgmaintype = await ODHTagHelper.GeODHTagByID(QueryFactory, "Kultur Sehenswürdigkeiten");
                    var mysmgsubtype = await ODHTagHelper.GeODHTagByID(QueryFactory, subtype);
                    var mysmgpoipoitype = new List<SmgTags>();


                    List<string> museumskategorien = new List<string>();
                    var mymuseumscategoriesstrings = mymuseum.PoiProperty["de"].Where(x => x.Name == "categories").Select(x => x.Value).ToList();
                    foreach (var mymuseumscategoriesstring in mymuseumscategoriesstrings)
                    {
                        var splittedlist = mymuseumscategoriesstring.Split(',').ToList();

                        foreach (var splitted in splittedlist)
                        {
                            museumskategorien.Add(splitted.Trim());

                            var mykategoriequery = await ODHTagHelper.GeODHTagByID(QueryFactory, "Museen " + splitted.Trim());
                            mysmgpoipoitype.Add(mykategoriequery);
                        }
                    }

                    mymuseum.Type = mysmgmaintype.Shortname;
                    mymuseum.SubType = mysmgsubtype.Shortname;

                    if (!mymuseum.SmgTags.Contains(mysmgmaintype.Id))
                        mymuseum.SmgTags.Add(mysmgmaintype.Id);
                    if (!mymuseum.SmgTags.Contains(mysmgsubtype.Id))
                        mymuseum.SmgTags.Add(mysmgsubtype.Id);
                    if (mysmgpoipoitype.Count > 0)
                    {
                        foreach (var mysmgpoitypel in mysmgpoipoitype)
                        {
                            if (!mymuseum.SmgTags.Contains(mysmgpoitypel.Id))
                                mymuseum.SmgTags.Add(mysmgpoitypel.Id);
                        }
                    }

                    if (mysmgpoipoitype.Count > 0)
                        mymuseum.PoiType = mysmgpoipoitype.FirstOrDefault().Shortname;
                    else
                        mymuseum.PoiType = "";


                    foreach (var langcat in languagelistcategories)
                    {
                        AdditionalPoiInfos additional = new AdditionalPoiInfos();
                        additional.Language = langcat;
                        additional.MainType = mysmgmaintype.TagName[langcat];
                        additional.SubType = mysmgsubtype.TagName[langcat];
                        additional.PoiType = mysmgpoipoitype.Count > 0 ? mysmgpoipoitype.FirstOrDefault().TagName[langcat] : "";
                        mymuseum.AdditionalPoiInfos.TryAddOrUpdate(langcat, additional);
                    }

                    //Setting Categorization by Valid Tags
                    var currentcategories = validtagsforcategories.Where(x => mymuseum.SmgTags.Contains(x.Id));
                    foreach (var smgtagtotranslate in currentcategories)
                    {
                        foreach (var languagecategory in languagelistcategories)
                        {
                            if (mymuseum.AdditionalPoiInfos[languagecategory].Categories == null)
                                mymuseum.AdditionalPoiInfos[languagecategory].Categories = new List<string>();

                            if (smgtagtotranslate.TagName.ContainsKey(languagecategory) && !mymuseum.AdditionalPoiInfos[languagecategory].Categories.Contains(smgtagtotranslate.TagName[languagecategory].Trim()))
                                mymuseum.AdditionalPoiInfos[languagecategory].Categories.Add(smgtagtotranslate.TagName[languagecategory].Trim());
                        }
                    }                                 
                }

                //Setting LicenseInfo
                mymuseum.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<SmgPoi>(mymuseum, Helper.LicenseHelper.GetLicenseforOdhActivityPoi);

                var result = await InsertSiagMuseumToDB(mymuseum, mymuseum.Id, new KeyValuePair<string, XElement>(museumid, mymuseumelement));
                newcounter = newcounter + result.created.Value;
                updatecounter = updatecounter + result.updated.Value;
            }

            return new UpdateDetail() { created = newcounter, updated = updatecounter, deleted = 0 };                
        }

        //public static void SetMuseumsnotinListToInactive(IDocumentStore documentStore)
        //{
        //    var mymuseumlist = XDocument.Load(Constants.xmldir + "MuseumList.xml");
        //    List<string> mymuseumroot = mymuseumlist.Root.Elements("Museum").Select(x => x.Attribute("ID").Value).ToList();

        //    Constants.tracesource.TraceEvent(TraceEventType.Information, 0, mymuseumroot.Count + " Elements on SIAG List");


        //    int todeactivatecounter = 0;

        //    var mymuseumsonraven = default(IEnumerable<string>);

        //    using (var session = documentStore.OpenSession())
        //    {
        //        mymuseumsonraven = session.Query<SmgPoi, SmgPoiMegaFilter>().Where(x => x.SyncSourceInterface == "MuseumData").Select(x => x.CustomId).Take(1024).ToList();
        //    }

        //    Constants.tracesource.TraceEvent(TraceEventType.Information, 0, mymuseumsonraven.Count() + " Elements in DB");


        //    var idstodelete = mymuseumsonraven.Where(p => !mymuseumroot.Any(p2 => p2 == p));

        //    Constants.tracesource.TraceEvent(TraceEventType.Information, 0, idstodelete.Count() + " Elements to delete");

        //    foreach (var idtodelete in idstodelete)
        //    {
        //        using (var session = documentStore.OpenSession())
        //        {
        //            var mymuseumtodeactivate = session.Query<SmgPoi, SmgPoiMegaFilter>().Where(x => x.SyncSourceInterface == "MuseumData" && x.CustomId == idtodelete).FirstOrDefault();

        //            if (mymuseumtodeactivate != null)
        //            {
        //                //session.Delete(mymuseumtodeactivate);
        //                mymuseumtodeactivate.SmgActive = false;
        //                mymuseumtodeactivate.Active = false;
        //                session.SaveChanges();
        //            }
        //        }
        //    }

        //}


        private async Task<PGCRUDResult> InsertSiagMuseumToDB(ODHActivityPoiLinked odhactivitypoi, string idtocheck, KeyValuePair<string, XElement> siagmuseumdata)
        {
            try
            {
                idtocheck = idtocheck.ToUpper();
                odhactivitypoi.Id = odhactivitypoi.Id?.ToLower();

                //Set LicenseInfo
                odhactivitypoi.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<ODHActivityPoi>(odhactivitypoi, Helper.LicenseHelper.GetLicenseforOdhActivityPoi);

                var rawdataid = await InsertInRawDataDB(siagmuseumdata);

                return await QueryFactory.UpsertData<ODHActivityPoiLinked>(odhactivitypoi, "smgpois", rawdataid);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<int> InsertInRawDataDB(KeyValuePair<string, XElement> siagmuseumdata)
        {
            return await QueryFactory.InsertInRawtableAndGetIdAsync(
                        new RawDataStore()
                        {
                            datasource = "siag",
                            importdate = DateTime.Now,
                            raw = siagmuseumdata.Value.ToString(),
                            sourceinterface = "museumdata",
                            sourceid = siagmuseumdata.Key,
                            sourceurl = "https://musport.prov.bz.it/musport/services/MuseumsService/",
                            type = "odhactivitypoi-museum"
                        });
        }

        


        #endregion
    }
}
