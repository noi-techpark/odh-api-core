using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SIAG;
using DataModel;
using Newtonsoft.Json;
using System.Threading;
using System.Xml.Linq;

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
        public async Task<UpdateDetail> SaveWeatherToHistoryTable(string? id = null, CancellationToken cancellationToken = default)
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

                return new UpdateDetail() { created = insertresult, updated = 0, deleted = 0 };                    
            }
            else
                throw new Exception("No weatherdata received from source!");
        }

        #endregion

        #region SIAG Museumdata

        public async Task<UpdateDetail> SaveMuseumsToODH(DateTime? lastchanged = null, CancellationToken cancellationToken)
        {
            var museumslist = await ImportMuseumlist(cancellationToken);
            //var updateresult = await ImportMuseums(museumslist, cancellationToken);
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

        //private async Task<UpdateDetail> ImportMuseums(XDocument mymuseumlist, CancellationToken cancellationToken)
        //{
        //    string museumid = "";

        //    try
        //    {
        //        XElement mymuseumroot = mymuseumlist.Root;

        //        XNamespace ns = "http://service.kks.siag";

        //        //.Where(x => x.Attribute("ID").Value == "963")
        //        int updatecounter = 0;
        //        int newcounter = 0;

        //        //Load ValidTagsfor Categories
        //        var validtagsforcategories = default(IEnumerable<SmgTags>);
        //        //For AdditionalInfos
        //        List<string> languagelistcategories = new List<string>() { "de", "it", "en", "nl", "cs", "pl", "fr", "ru" };
        //        using (var session = documentStore.OpenSession())
        //        {
        //            //Getting valid Tags for Gastronomies
        //            validtagsforcategories = SmgTagHelper.GetSmgTagsValidforTranslations(session, new List<string>() { "Kultur Sehenswürdigkeiten" });
        //        }

        //        foreach (XElement mymuseumelement in mymuseumroot.Elements("Museum"))
        //        {
        //            museumid = mymuseumelement.Attribute("ID").Value;
        //            string plz = mymuseumelement.Attribute("PLZ").Value;

        //            //Immportieren
        //            var mymuseumdata = await SIAG.GetMuseumFromSIAG.GetMuseumDetail(museumid);
        //            var mymuseumxml = mymuseumdata.Root.Element(ns + "return");

        //            //Zersch schaugn obs des Museum gibb sischt mochmer a nuies
        //            using (var session = documentStore.OpenSession())
        //            {
        //                var mymuseum = session.Query<SmgPoi, SmgPoiMegaFilter>()
        //                    .Where(x => x.SubType == "Museen" || x.SubType == "Bergwerke" || x.SubType == "Naturparkhäuser")
        //                    .Where(x => x.CustomId == museumid)
        //                    .FirstOrDefault();

        //                if (mymuseum == null)
        //                {

        //                    //Neuen Datensatz
        //                    mymuseum = new SmgPoi();
        //                    mymuseum.FirstImport = DateTime.Now;

        //                    XNamespace ax211 = "http://data.service.kks.siag/xsd";

        //                    string siagid = mymuseumxml.Element(ax211 + "museId").Value;

        //                    Constants.tracesource.TraceEvent(TraceEventType.Information, 0, "New Museum Siag ID:" + siagid);


        //                    mymuseum.Id = "Smgpoi" + siagid + "SIAG";
        //                    mymuseum.CustomId = siagid;
        //                    mymuseum.Active = true;
        //                    mymuseum.SmgActive = true;

        //                    mymuseum.Source = "SIAG";
        //                    mymuseum.SyncSourceInterface = "MuseumData";
        //                    mymuseum.SyncUpdateMode = "Full";
        //                    mymuseum.LastChange = DateTime.Now;
        //                    mymuseum.FirstImport = DateTime.Now;

        //                    var parsedmuseum = MuseumData.ParseMuseum.ParseMuseumToRaven(mymuseum, mymuseumxml, plz);
        //                    var museum = parsedmuseum.Item2;



        //                    museum.Shortname = museum.Detail["de"].Title.Trim();
        //                    //Suedtirol Type laden
        //                    var mysmgmaintype = session.Query<SuedtirolType, SuedtirolTypeByParent>()
        //                        .Where(x => x.Key == "Kultur Sehenswürdigkeiten")
        //                        //.Where(x => x.TypeParent == smgpoi.SubType)
        //                        .Where(x => x.Level == 0)
        //                        .FirstOrDefault();


        //                    var mysmgsubtype = session.Query<SuedtirolType, SuedtirolTypeByParent>()
        //                        .Where(x => x.Key == "Museen")
        //                        //.Where(x => x.TypeParent == smgpoi.SubType)
        //                        .Where(x => x.Level == 1)
        //                        .FirstOrDefault();

        //                    var mysmgpoipoitype = new List<SuedtirolType>();


        //                    List<string> museumskategorien = new List<string>();
        //                    var mymuseumscategoriesstrings = museum.PoiProperty["de"].Where(x => x.Name == "categories").Select(x => x.Value).ToList();
        //                    foreach (var mymuseumscategoriesstring in mymuseumscategoriesstrings)
        //                    {
        //                        var splittedlist = mymuseumscategoriesstring.Split(',').ToList();

        //                        foreach (var splitted in splittedlist)
        //                        {
        //                            museumskategorien.Add(splitted.Trim());
        //                        }
        //                    }


        //                    if (museumskategorien.Contains("Kultur"))
        //                    {
        //                        var mykategoriequery = session.Query<SuedtirolType, SuedtirolTypeByParent>()
        //                        .Where(x => x.Key == "Museen Kultur")
        //                        .Where(x => x.Level == 2)
        //                        .FirstOrDefault();

        //                        mysmgpoipoitype.Add(mykategoriequery);
        //                    }
        //                    if (museumskategorien.Contains("Natur"))
        //                    {
        //                        var mykategoriequery = session.Query<SuedtirolType, SuedtirolTypeByParent>()
        //                        .Where(x => x.Key == "Museen Natur")
        //                        .Where(x => x.Level == 2)
        //                        .FirstOrDefault();

        //                        mysmgpoipoitype.Add(mykategoriequery);
        //                    }
        //                    if (museumskategorien.Contains("Technik"))
        //                    {
        //                        var mykategoriequery = session.Query<SuedtirolType, SuedtirolTypeByParent>()
        //                        .Where(x => x.Key == "Museen Technik")
        //                        .Where(x => x.Level == 2)
        //                        .FirstOrDefault();

        //                        mysmgpoipoitype.Add(mykategoriequery);
        //                    }
        //                    if (museumskategorien.Contains("Kunst"))
        //                    {
        //                        var mykategoriequery = session.Query<SuedtirolType, SuedtirolTypeByParent>()
        //                        .Where(x => x.Key == "Museen Kunst")
        //                        .Where(x => x.Level == 2)
        //                        .FirstOrDefault();

        //                        mysmgpoipoitype.Add(mykategoriequery);
        //                    }


        //                    museum.Type = mysmgsubtype.TypeParent;
        //                    museum.SubType = mysmgsubtype.Key;

        //                    List<string> mysmgtags = museum.SmgTags.ToList();

        //                    if (!mysmgtags.Contains(mysmgmaintype.Key))
        //                        mysmgtags.Add(mysmgmaintype.Key);

        //                    if (!mysmgtags.Contains(mysmgsubtype.Key))
        //                        mysmgtags.Add(mysmgsubtype.Key);


        //                    if (mysmgpoipoitype.Count > 0)
        //                    {
        //                        foreach (var mysmgpoipoitypel in mysmgpoipoitype)
        //                        {
        //                            if (!mysmgtags.Contains(mysmgpoipoitypel.Key))
        //                                mysmgtags.Add(mysmgpoipoitypel.Key);
        //                        }

        //                    }
        //                    museum.SmgTags = mysmgtags.ToList();


        //                    if (mysmgpoipoitype.Count > 0)
        //                        museum.PoiType = mysmgpoipoitype.FirstOrDefault().Key;
        //                    else
        //                        museum.PoiType = "";

        //                    List<string> haslanguagelist = new List<string>();

        //                    haslanguagelist.Add("de");
        //                    haslanguagelist.Add("it");
        //                    haslanguagelist.Add("en");

        //                    museum.HasLanguage = haslanguagelist.ToList();

        //                    foreach (var langcat in languagelistcategories)
        //                    {
        //                        AdditionalPoiInfos additional = new AdditionalPoiInfos();
        //                        additional.Language = langcat;
        //                        additional.MainType = mysmgmaintype.TypeNames[langcat];
        //                        additional.SubType = mysmgsubtype.TypeNames[langcat];
        //                        additional.PoiType = mysmgpoipoitype.Count > 0 ? mysmgpoipoitype.FirstOrDefault().TypeNames[langcat] : "";
        //                        museum.AdditionalPoiInfos.TryAddOrUpdate(langcat, additional);
        //                    }

        //                    //Setting Categorization by Valid Tags
        //                    var currentcategories = validtagsforcategories.Where(x => x.Id.In(museum.SmgTags));
        //                    foreach (var smgtagtotranslate in currentcategories)
        //                    {
        //                        foreach (var languagecategory in languagelistcategories)
        //                        {
        //                            if (museum.AdditionalPoiInfos[languagecategory].Categories == null)
        //                                museum.AdditionalPoiInfos[languagecategory].Categories = new List<string>();

        //                            if (smgtagtotranslate.TagName.ContainsKey(languagecategory) && !museum.AdditionalPoiInfos[languagecategory].Categories.Contains(smgtagtotranslate.TagName[languagecategory].Trim()))
        //                                museum.AdditionalPoiInfos[languagecategory].Categories.Add(smgtagtotranslate.TagName[languagecategory].Trim());
        //                        }
        //                    }

        //                    //if (parsedmuseum.Item1.StartsWith("3"))
        //                    //    museum.LocationInfo = Common.GetLocationInfo.GetTheLocationInfoMunicipalitySiag(session, parsedmuseum.Item1);
        //                    //if (parsedmuseum.Item1.StartsWith("8"))
        //                    //    museum.LocationInfo = Common.GetLocationInfo.GetTheLocationInfoDistrictSiag(session, parsedmuseum.Item1);

        //                    //museum.TourismorganizationId = museum.LocationInfo.TvInfo.Id;

        //                    //Locationinfo aus GPS Punkte holen

        //                    if (museum.GpsInfo != null && museum.GpsInfo.Count > 0)
        //                    {
        //                        if (museum.GpsInfo.FirstOrDefault().Latitude != 0 && museum.GpsInfo.FirstOrDefault().Longitude != 0)
        //                        {
        //                            var districtlist = session.Query<District, DistrictFilter>()
        //                               .Customize(x => x.SortByDistance())
        //                               .Customize(x => x.WithinRadiusOf(
        //                                           fieldName: "Coordinates",
        //                                           radius: 15,
        //                                           latitude: museum.GpsInfo.FirstOrDefault().Latitude,
        //                                           longitude: museum.GpsInfo.FirstOrDefault().Longitude,
        //                                           radiusUnits: SpatialUnits.Kilometers))
        //                               .FirstOrDefault();

        //                            var locinfo = Common.GetLocationInfo.GetTheLocationInfoDistrict(session, districtlist.Id);

        //                            museum.LocationInfo = locinfo;
        //                            museum.TourismorganizationId = locinfo.TvInfo.Id;
        //                        }
        //                        else
        //                        {
        //                            if (parsedmuseum.Item1.StartsWith("3"))
        //                                museum.LocationInfo = Common.GetLocationInfo.GetTheLocationInfoMunicipalitySiag(session, parsedmuseum.Item1);
        //                            if (parsedmuseum.Item1.StartsWith("8"))
        //                                museum.LocationInfo = Common.GetLocationInfo.GetTheLocationInfoDistrictSiag(session, parsedmuseum.Item1);

        //                            museum.TourismorganizationId = museum.LocationInfo.TvInfo.Id;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (parsedmuseum.Item1.StartsWith("3"))
        //                            museum.LocationInfo = Common.GetLocationInfo.GetTheLocationInfoMunicipalitySiag(session, parsedmuseum.Item1);
        //                        if (parsedmuseum.Item1.StartsWith("8"))
        //                            museum.LocationInfo = Common.GetLocationInfo.GetTheLocationInfoDistrictSiag(session, parsedmuseum.Item1);

        //                        museum.TourismorganizationId = museum.LocationInfo.TvInfo.Id;
        //                    }

        //                    //Setting LicenseInfo
        //                    museum.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<SmgPoi>(museum, Helper.LicenseHelper.GetLicenseforOdhActivityPoi);

        //                    session.Store(museum);

        //                    session.SaveChanges();

        //                    Console.ForegroundColor = ConsoleColor.Red;
        //                    Console.WriteLine("Museum " + museumid + " added");

        //                    Constants.tracesource.TraceEvent(TraceEventType.Information, 0, "Museum NEW Import succeeded Id: " + museum.Id + " Siag ID:" + museum.CustomId);

        //                    newcounter++;

        //                    using (LogContext.PushProperty("id", museum.Id))
        //                    using (LogContext.PushProperty("origin", "console"))
        //                    using (LogContext.PushProperty("source", "siag"))
        //                    {
        //                        log.Information("museum.import.success");
        //                    }
        //                }
        //                else
        //                {
        //                    Constants.tracesource.TraceEvent(TraceEventType.Information, 0, "Updating Museum Id: " + mymuseum.Id + " Siag ID:" + mymuseum.CustomId);


        //                    //mymuseum.CustomId = siagid;
        //                    mymuseum.Active = true;
        //                    //mymuseum.SmgActive = true;
        //                    mymuseum.Source = "SIAG";
        //                    mymuseum.SyncSourceInterface = "MuseumData";
        //                    mymuseum.SyncUpdateMode = "Full";
        //                    mymuseum.LastChange = DateTime.Now;


        //                    //Vorhandenes Museum
        //                    var parsedmuseum = MuseumData.ParseMuseum.ParseMuseumToRaven(mymuseum, mymuseumxml, plz);

        //                    var museum = parsedmuseum.Item2;


        //                    string subtype = "Museen";

        //                    if (museum.SubType == "Bergwerke")
        //                        subtype = "Bergwerke";

        //                    if (museum.SubType == "Naturparkhäuser")
        //                        subtype = "Naturparkhäuser";

        //                    //Suedtirol Type laden
        //                    var mysmgmaintype = session.Query<SuedtirolType, SuedtirolTypeByParent>()
        //                        .Where(x => x.Key == "Kultur Sehenswürdigkeiten")
        //                        //.Where(x => x.TypeParent == smgpoi.SubType)
        //                        .Where(x => x.Level == 0)
        //                        .FirstOrDefault();


        //                    var mysmgsubtype = session.Query<SuedtirolType, SuedtirolTypeByParent>()
        //                        .Where(x => x.Key == subtype)
        //                        //.Where(x => x.TypeParent == smgpoi.SubType)
        //                        .Where(x => x.Level == 1)
        //                        .FirstOrDefault();

        //                    var mysmgpoipoitype = new List<SuedtirolType>();


        //                    //List<string> museumskategorien = museum.PoiProperty["de"].Where(x => x.Name == "Kategorien").Select(x => x.Value).ToList();
        //                    List<string> museumskategorien = new List<string>();
        //                    var mymuseumscategoriesstrings = museum.PoiProperty["de"].Where(x => x.Name == "categories").Select(x => x.Value).ToList();
        //                    foreach (var mymuseumscategoriesstring in mymuseumscategoriesstrings)
        //                    {
        //                        var splittedlist = mymuseumscategoriesstring.Split(',').ToList();

        //                        foreach (var splitted in splittedlist)
        //                        {
        //                            museumskategorien.Add(splitted.Trim());
        //                        }
        //                    }


        //                    if (museumskategorien.Contains("Kultur") && subtype == "Museen")
        //                    {
        //                        var mysmgpoipoitypequery = session.Query<SuedtirolType, SuedtirolTypeByParent>()
        //                        .Where(x => x.Key == "Museen Kultur")
        //                        .Where(x => x.Level == 2)
        //                        .FirstOrDefault();

        //                        mysmgpoipoitype.Add(mysmgpoipoitypequery);
        //                    }
        //                    if (museumskategorien.Contains("Natur") && subtype == "Museen")
        //                    {
        //                        var mysmgpoipoitypequery = session.Query<SuedtirolType, SuedtirolTypeByParent>()
        //                        .Where(x => x.Key == "Museen Natur")
        //                        .Where(x => x.Level == 2)
        //                        .FirstOrDefault();

        //                        mysmgpoipoitype.Add(mysmgpoipoitypequery);
        //                    }
        //                    if (museumskategorien.Contains("Technik") && subtype == "Museen")
        //                    {
        //                        var mysmgpoipoitypequery = session.Query<SuedtirolType, SuedtirolTypeByParent>()
        //                        .Where(x => x.Key == "Museen Technik")
        //                        .Where(x => x.Level == 2)
        //                        .FirstOrDefault();

        //                        mysmgpoipoitype.Add(mysmgpoipoitypequery);
        //                    }
        //                    if (museumskategorien.Contains("Kunst") && subtype == "Museen")
        //                    {
        //                        var mysmgpoipoitypequery = session.Query<SuedtirolType, SuedtirolTypeByParent>()
        //                        .Where(x => x.Key == "Museen Kunst")
        //                        .Where(x => x.Level == 2)
        //                        .FirstOrDefault();

        //                        mysmgpoipoitype.Add(mysmgpoipoitypequery);
        //                    }


        //                    museum.Type = mysmgsubtype.TypeParent;
        //                    museum.SubType = mysmgsubtype.Key;

        //                    if (!museum.SmgTags.Contains(mysmgmaintype.Key))
        //                        museum.SmgTags.Add(mysmgmaintype.Key);
        //                    if (!museum.SmgTags.Contains(mysmgsubtype.Key))
        //                        museum.SmgTags.Add(mysmgsubtype.Key);
        //                    if (mysmgpoipoitype.Count > 0)
        //                    {
        //                        foreach (var mysmgpoitypel in mysmgpoipoitype)
        //                        {
        //                            if (!museum.SmgTags.Contains(mysmgpoitypel.Key))
        //                                museum.SmgTags.Add(mysmgpoitypel.Key);
        //                        }
        //                    }


        //                    if (mysmgpoipoitype.Count > 0)
        //                        museum.PoiType = mysmgpoipoitype.FirstOrDefault().Key;
        //                    else
        //                        museum.PoiType = "";


        //                    foreach (var langcat in languagelistcategories)
        //                    {
        //                        AdditionalPoiInfos additional = new AdditionalPoiInfos();
        //                        additional.Language = langcat;
        //                        additional.MainType = mysmgmaintype.TypeNames[langcat];
        //                        additional.SubType = mysmgsubtype.TypeNames[langcat];
        //                        additional.PoiType = mysmgpoipoitype.Count > 0 ? mysmgpoipoitype.FirstOrDefault().TypeNames[langcat] : "";
        //                        museum.AdditionalPoiInfos.TryAddOrUpdate(langcat, additional);
        //                    }

        //                    //Setting Categorization by Valid Tags
        //                    var currentcategories = validtagsforcategories.Where(x => x.Id.In(museum.SmgTags));
        //                    foreach (var smgtagtotranslate in currentcategories)
        //                    {
        //                        foreach (var languagecategory in languagelistcategories)
        //                        {
        //                            if (museum.AdditionalPoiInfos[languagecategory].Categories == null)
        //                                museum.AdditionalPoiInfos[languagecategory].Categories = new List<string>();

        //                            if (smgtagtotranslate.TagName.ContainsKey(languagecategory) && !museum.AdditionalPoiInfos[languagecategory].Categories.Contains(smgtagtotranslate.TagName[languagecategory].Trim()))
        //                                museum.AdditionalPoiInfos[languagecategory].Categories.Add(smgtagtotranslate.TagName[languagecategory].Trim());
        //                        }
        //                    }

        //                    //Hmmm soll dass nochmal gemacht werden?
        //                    //if (parsedmuseum.Item1.StartsWith("3"))
        //                    //    museum.LocationInfo = Common.GetLocationInfo.GetTheLocationInfoMunicipalitySiag(session, parsedmuseum.Item1);
        //                    //if (parsedmuseum.Item1.StartsWith("8"))
        //                    //    museum.LocationInfo = Common.GetLocationInfo.GetTheLocationInfoDistrictSiag(session, parsedmuseum.Item1);


        //                    //Locationinfo aus GPS Punkte holen nicht mehr bei Update

        //                    //if (museum.GpsInfo != null && museum.GpsInfo.Count > 0)
        //                    //{
        //                    //    if (museum.GpsInfo.FirstOrDefault().Latitude != 0 && museum.GpsInfo.FirstOrDefault().Longitude != 0)
        //                    //    {
        //                    //        var districtlist = session.Query<District, DistrictFilter>()
        //                    //           .Customize(x => x.SortByDistance())
        //                    //           .Customize(x => x.WithinRadiusOf(
        //                    //                       fieldName: "Coordinates",
        //                    //                       radius: 15,
        //                    //                       latitude: museum.GpsInfo.FirstOrDefault().Latitude,
        //                    //                       longitude: museum.GpsInfo.FirstOrDefault().Longitude,
        //                    //                       radiusUnits: SpatialUnits.Kilometers))
        //                    //           .FirstOrDefault();

        //                    //        var locinfo = Common.GetLocationInfo.GetTheLocationInfoDistrict(session, districtlist.Id);

        //                    //        museum.LocationInfo = locinfo;
        //                    //        museum.TourismorganizationId = locinfo.TvInfo.Id;
        //                    //    }
        //                    //    else
        //                    //    {
        //                    //        if (parsedmuseum.Item1.StartsWith("3"))
        //                    //            museum.LocationInfo = Common.GetLocationInfo.GetTheLocationInfoMunicipalitySiag(session, parsedmuseum.Item1);
        //                    //        if (parsedmuseum.Item1.StartsWith("8"))
        //                    //            museum.LocationInfo = Common.GetLocationInfo.GetTheLocationInfoDistrictSiag(session, parsedmuseum.Item1);

        //                    //        museum.TourismorganizationId = museum.LocationInfo.TvInfo.Id;
        //                    //    }
        //                    //}
        //                    //else
        //                    //{
        //                    //    if (parsedmuseum.Item1.StartsWith("3"))
        //                    //        museum.LocationInfo = Common.GetLocationInfo.GetTheLocationInfoMunicipalitySiag(session, parsedmuseum.Item1);
        //                    //    if (parsedmuseum.Item1.StartsWith("8"))
        //                    //        museum.LocationInfo = Common.GetLocationInfo.GetTheLocationInfoDistrictSiag(session, parsedmuseum.Item1);

        //                    //    museum.TourismorganizationId = museum.LocationInfo.TvInfo.Id;
        //                    //}


        //                    //ReSetting LicenseInfo
        //                    museum.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<SmgPoi>(museum, Helper.LicenseHelper.GetLicenseforOdhActivityPoi);


        //                    session.Store(museum);
        //                    session.SaveChanges();

        //                    Console.ForegroundColor = ConsoleColor.Red;
        //                    Console.WriteLine("Museum " + museumid + " updated");

        //                    Constants.tracesource.TraceEvent(TraceEventType.Information, 0, "Museum Update succeeded Id: " + museum.Id + " Siag ID:" + museum.CustomId);
        //                    updatecounter++;

        //                    using (LogContext.PushProperty("id", museum.Id))
        //                    using (LogContext.PushProperty("origin", "console"))
        //                    using (LogContext.PushProperty("source", "siag"))
        //                    {
        //                        log.Information("museum.import.success");
        //                    }
        //                }

        //            }

        //            Thread.Sleep(100);

        //        }

        //        Constants.tracesource.TraceEvent(TraceEventType.Information, 0, "Museum Import Finished new: " + newcounter + " updated: " + updatecounter);
        //    }
        //    catch (Exception ex)
        //    {
        //        using (LogContext.PushProperty("id", museumid))
        //        using (LogContext.PushProperty("origin", "console"))
        //        using (LogContext.PushProperty("source", "siag"))
        //        {
        //            log.Error(ex.Message, "museum.import.error");
        //        }

        //        Constants.tracesource.TraceEvent(TraceEventType.Error, 0, "Museum Import Error:" + ex.Message);
        //    }
        //}

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


        #endregion
    }
}
