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
using Helper;

namespace OdhApiImporter.Helpers
{
    public class SIAGImportHelper : ImportHelper, IImportHelper
    {
        public SIAGImportHelper(ISettings settings, QueryFactory queryfactory, string table) : base(settings, queryfactory, table)
        {

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
                rawData.license = "open";
                rawData.rawformat = "json";

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

                var myweatherhistory = new WeatherHistoryLinked();

                myweatherhistory.Id = odhweatherresultde.Id.ToString();
                myweatherhistory.Weather.Add("de", odhweatherresultde);
                myweatherhistory.Weather.Add("it", odhweatherresultit);
                myweatherhistory.Weather.Add("en", odhweatherresulten);
                myweatherhistory.LicenseInfo = Helper.LicenseHelper.GetLicenseforWeather();
                myweatherhistory.FirstImport = DateTime.Now;
                myweatherhistory.HasLanguage = new List<string>() { "de", "it", "en" };
                myweatherhistory.LastChange = odhweatherresultde.date;
                myweatherhistory.Shortname = odhweatherresultde.evolutiontitle;


                var insertresult = await QueryFactory.UpsertData<WeatherHistoryLinked>(myweatherhistory, "weatherdatahistory", insertresultraw, true);

                //var insertresult = await QueryFactory.Query("weatherdatahistory")
                //      .InsertAsync(new JsonBDataRaw { id = odhweatherresultde.Id.ToString(), data = new JsonRaw(myweatherhistory), rawdataid = insertresultraw });

                ////Save to PG
                ////Check if data exists                    
                //var result = await QueryFactory.UpsertData<ODHActivityPoi>(odhactivitypoi!, "weatherdatahistory", insertresultraw);

                return new UpdateDetail() { created = insertresult.created, updated = insertresult.updated, deleted = insertresult.deleted };                    
            }
            else
                throw new Exception("No weatherdata received from source!");
        }

        #endregion

        #region SIAG Museumdata

        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, CancellationToken cancellationToken = default)
        {
            //Import the actual museums List from SIAG
            var museumslist = await ImportList(cancellationToken);
            //Parse siag data single and import each museum
            var updateresult = await ImportData(museumslist, cancellationToken);
            //If in the DB there are museums no more listed in the siag response set this data to inactive
            var deleteresult = await SetDataNotinListToInactive(museumslist, cancellationToken);

            return GenericResultsHelper.MergeUpdateDetail(new List<UpdateDetail>() { updateresult, deleteresult });
        }

        private async Task<XDocument> ImportList(CancellationToken cancellationToken)
        {
            var myxml = await SIAG.GetMuseumFromSIAG.GetMuseumList();

            XDocument mymuseumlist = new XDocument();
            XElement mymuseums = new XElement("Museums");

            XNamespace ns = "http://service.kks.siag";
            XNamespace ax211 = "http://data.service.kks.siag/xsd";

            var mymuseumlist2 = myxml.Root?.Element(ns + "return")?.Elements(ax211 + "museums") ?? Enumerable.Empty<XElement>();

            foreach (XElement idtoimport in mymuseumlist2)
            {
                XElement mymuseum = new XElement("Museum");
                mymuseum.Add(new XAttribute("ID", idtoimport.Element(ax211 + "museId")?.Value ?? ""));
                mymuseum.Add(new XAttribute("PLZ", idtoimport.Element(ax211 + "plz")?.Value ?? ""));

                mymuseums.Add(mymuseum);
            }

            mymuseumlist.Add(mymuseums);

            WriteLog.LogToConsole("", "dataimport", "list.siagmuseum", new ImportLog() { sourceid = "", sourceinterface = "siag.museum", success = true, error = "" });

            return mymuseumlist;
        }

        private async Task<UpdateDetail> ImportData(XDocument mymuseumlist, CancellationToken cancellationToken)
        {         
            XElement? mymuseumroot = mymuseumlist.Root;
               
            int updatecounter = 0;
            int newcounter = 0;
            int errorcounter = 0;

            //Load ValidTagsfor Categories
            var validtagsforcategories = default(IEnumerable<SmgTags>);
            //For AdditionalInfos
            List<string> languagelistcategories = new List<string>() { "de", "it", "en", "nl", "cs", "pl", "fr", "ru" };

            //Getting valid Tags for Museums
            validtagsforcategories = await ODHTagHelper.GetODHTagsValidforTranslations(QueryFactory, new List<string>() { "Kultur Sehenswürdigkeiten" });

            foreach (XElement mymuseumelement in mymuseumroot?.Elements("Museum") ?? Enumerable.Empty<XElement>())
            {
                var importresult = await ImportDataSingle(mymuseumelement, languagelistcategories, validtagsforcategories);

                newcounter = newcounter + importresult.created.Value;
                updatecounter = updatecounter + importresult.updated.Value;
                errorcounter = errorcounter + importresult.error.Value;
            }

            return new UpdateDetail() { created = newcounter, updated = updatecounter, deleted = 0, error = errorcounter };
        }
        
        private async Task<UpdateDetail> ImportDataSingle(XElement mymuseumelement, List<string> languagelistcategories, IEnumerable<SmgTags> validtagsforcategories)
        {
            string museumidtoreturn = "";
            int updatecounter = 0;
            int newcounter = 0;
            int errorcounter = 0;

            try
            {
                XNamespace ns = "http://service.kks.siag";

                string museumid = mymuseumelement.Attribute("ID")?.Value ?? "";
                string plz = mymuseumelement.Attribute("PLZ")?.Value ?? "";

                //Import Museum data from Siag
                var mymuseumdata = await SIAG.GetMuseumFromSIAG.GetMuseumDetail(museumid);
                var mymuseumxml = mymuseumdata?.Root?.Element(ns + "return");

                //Improve Performance this query is very slow!!!!
                var mymuseumquery = QueryFactory.Query("smgpois")
                    .Select("data")
                    .WhereRaw("data->>'CustomId' = $$", museumid.ToLower());

                var mymuseum = await mymuseumquery.GetFirstOrDefaultAsObject<ODHActivityPoiLinked>();

                if (mymuseum == null)
                {

                    //Neuen Datensatz
                    mymuseum = new ODHActivityPoiLinked();
                    mymuseum.FirstImport = DateTime.Now;

                    XNamespace ax211 = "http://data.service.kks.siag/xsd";

                    string siagid = mymuseumxml?.Element(ax211 + "museId")?.Value ?? "";
                    string gemeindeid = mymuseumxml?.Element(ax211 + "gemeindeId")?.Value ?? "";

                    mymuseum.Id = "smgpoi" + siagid + "siag";

                    museumidtoreturn = mymuseum.Id;

                    mymuseum.CustomId = siagid;
                    mymuseum.Active = true;
                    mymuseum.SmgActive = true;

                    mymuseum.FirstImport = DateTime.Now;

                    if (mymuseumxml is { })
                        SIAG.Parser.ParseMuseum.ParseMuseumToPG(mymuseum, mymuseumxml, plz);

                    //ADD MAPPING
                    var museummuseId = new Dictionary<string, string>() { { "museId", siagid } };
                    mymuseum.Mapping.TryAddOrUpdate("siag", museummuseId);

                    mymuseum.Shortname = mymuseum.Detail["de"].Title?.Trim();

                    //Suedtirol Type laden
                    var mysmgmaintype = await ODHTagHelper.GeODHTagByID(QueryFactory, "Kultur Sehenswürdigkeiten");
                    var mysmgsubtype = await ODHTagHelper.GeODHTagByID(QueryFactory, "Museen");
                    var mysmgpoipoitype = new List<SmgTags>();

                    List<string> museumskategorien = new List<string>();
                    var mymuseumscategoriesstrings = mymuseum.PoiProperty["de"].Where(x => x.Name == "categories").Select(x => x.Value).ToList();
                    foreach (var mymuseumscategoriesstring in mymuseumscategoriesstrings)
                    {
                        var splittedlist = mymuseumscategoriesstring?.Split(',').ToList() ?? new();

                        foreach (var splitted in splittedlist)
                        {
                            if (!string.IsNullOrEmpty(splitted))
                            {
                                museumskategorien.Add(splitted.Trim());

                                var mykategoriequery = await ODHTagHelper.GeODHTagByID(QueryFactory, "Museen " + splitted.Trim());
                                if (mykategoriequery is { })
                                    mysmgpoipoitype.Add(mykategoriequery);
                            }
                        }
                    }

                    mymuseum.Type = mysmgmaintype?.Shortname;
                    mymuseum.SubType = mysmgsubtype?.Shortname;

                    List<string> mysmgtags = mymuseum.SmgTags?.ToList() ?? new();

                    if (mysmgmaintype?.Id is { } && !mysmgtags.Contains(mysmgmaintype.Id.ToLower()))
                        mysmgtags.Add(mysmgmaintype.Id.ToLower());

                    if (mysmgsubtype?.Id is { } && !mysmgtags.Contains(mysmgsubtype.Id.ToLower()))
                        mysmgtags.Add(mysmgsubtype.Id.ToLower());

                    if (mysmgpoipoitype.Count > 0)
                    {
                        foreach (var mysmgpoipoitypel in mysmgpoipoitype)
                        {
                            if (mysmgpoipoitypel.Id is { } && !mysmgtags.Contains(mysmgpoipoitypel.Id.ToLower()))
                                mysmgtags.Add(mysmgpoipoitypel.Id.ToLower());
                        }

                    }
                    mymuseum.SmgTags = mysmgtags.ToList();

                    if (mysmgpoipoitype.Count > 0)
                        mymuseum.PoiType = mysmgpoipoitype.FirstOrDefault()?.Shortname;
                    else
                        mymuseum.PoiType = "";

                    List<string> haslanguagelist = new();

                    haslanguagelist.Add("de");
                    haslanguagelist.Add("it");
                    haslanguagelist.Add("en");

                    mymuseum.HasLanguage = haslanguagelist.ToList();

                    foreach (var langcat in languagelistcategories)
                    {
                        AdditionalPoiInfos additional = new AdditionalPoiInfos();
                        additional.Language = langcat;
                        additional.MainType = mysmgmaintype?.TagName[langcat];
                        additional.SubType = mysmgsubtype?.TagName[langcat];
                        additional.PoiType = mysmgpoipoitype.Count > 0 ? mysmgpoipoitype.FirstOrDefault()?.TagName[langcat] : "";
                        mymuseum.AdditionalPoiInfos.TryAddOrUpdate(langcat, additional);
                    }

                    //Setting Categorization by Valid Tags
                    var currentcategories = validtagsforcategories.Where(x => mymuseum.SmgTags.Contains(x.Id.ToLower()));

                    foreach (var smgtagtotranslate in currentcategories)
                    {
                        foreach (string languagecategory in languagelistcategories)
                        {
                            if (mymuseum.AdditionalPoiInfos![languagecategory].Categories == null)
                                mymuseum.AdditionalPoiInfos[languagecategory].Categories = new List<string>();

                            if (smgtagtotranslate.TagName.ContainsKey(languagecategory) && (!mymuseum.AdditionalPoiInfos?[languagecategory].Categories?.Contains(smgtagtotranslate.TagName[languagecategory].Trim()) ?? false))
                                mymuseum.AdditionalPoiInfos![languagecategory]!.Categories!.Add(smgtagtotranslate.TagName[languagecategory].Trim());
                        }
                    }


                    //Get Locationinfo by given GPS Points
                    if (mymuseum.GpsInfo != null && mymuseum.GpsInfo.Count > 0)
                    {
                        if (mymuseum.GpsInfo.FirstOrDefault()?.Latitude != 0 && mymuseum.GpsInfo.FirstOrDefault()?.Longitude != 0)
                        {
                            var district = await GetLocationInfo.GetNearestDistrictbyGPS(QueryFactory, mymuseum.GpsInfo.FirstOrDefault()!.Latitude, mymuseum.GpsInfo.FirstOrDefault()!.Longitude, 30000);

                            if (district != null)
                            {
                                var locinfo = await GetLocationInfo.GetTheLocationInfoDistrict(QueryFactory, district.Id);

                                mymuseum.LocationInfo = locinfo;
                                mymuseum.TourismorganizationId = locinfo.TvInfo?.Id;
                            }
                        }
                    }

                    //If still no locinfo assigned
                    if (mymuseum.LocationInfo == null)
                    {
                        if (gemeindeid.StartsWith("3"))
                            mymuseum.LocationInfo = await GetLocationInfo.GetTheLocationInfoMunicipality_Siag(QueryFactory, gemeindeid);
                        if (gemeindeid.StartsWith("8"))
                            mymuseum.LocationInfo = await GetLocationInfo.GetTheLocationInfoDistrict_Siag(QueryFactory, gemeindeid);

                        mymuseum.TourismorganizationId = mymuseum.LocationInfo?.TvInfo?.Id;
                    }
                }
                else
                {
                    museumidtoreturn = mymuseum.Id;

                    //mymuseum.CustomId = siagid;
                    mymuseum.Active = true;
                    //mymuseum.SmgActive = true;                 

                    if (mymuseumxml is { })
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
                        var splittedlist = mymuseumscategoriesstring?.Split(',').ToList() ?? new();

                        foreach (var splitted in splittedlist)
                        {
                            if (!String.IsNullOrEmpty(splitted))
                            {
                                museumskategorien.Add(splitted.Trim());

                                var mykategoriequery = await ODHTagHelper.GeODHTagByID(QueryFactory, "Museen " + splitted.Trim());
                                if (mykategoriequery is { })
                                    mysmgpoipoitype.Add(mykategoriequery);
                            }
                        }
                    }

                    mymuseum.Type = mysmgmaintype?.Shortname;
                    mymuseum.SubType = mysmgsubtype?.Shortname;

                    mymuseum.SmgTags ??= new List<string>();
                    if (mysmgmaintype?.Id is { } && !mymuseum.SmgTags.Contains(mysmgmaintype.Id.ToLower()))
                        mymuseum.SmgTags.Add(mysmgmaintype.Id.ToLower());
                    if (mysmgsubtype?.Id is { } && !mymuseum.SmgTags.Contains(mysmgsubtype.Id.ToLower()))
                        mymuseum.SmgTags.Add(mysmgsubtype.Id.ToLower());
                    if (mysmgpoipoitype.Count > 0)
                    {
                        foreach (var mysmgpoitypel in mysmgpoipoitype)
                        {
                            if (mysmgpoitypel.Id is { } && !mymuseum.SmgTags.Contains(mysmgpoitypel.Id.ToLower()))
                                mymuseum.SmgTags.Add(mysmgpoitypel.Id.ToLower());
                        }
                    }

                    if (mysmgpoipoitype.Count > 0)
                        mymuseum.PoiType = mysmgpoipoitype.FirstOrDefault()?.Shortname;
                    else
                        mymuseum.PoiType = "";


                    foreach (var langcat in languagelistcategories)
                    {
                        AdditionalPoiInfos additional = new AdditionalPoiInfos();
                        additional.Language = langcat;
                        additional.MainType = mysmgmaintype?.TagName[langcat];
                        additional.SubType = mysmgsubtype?.TagName[langcat];
                        additional.PoiType = mysmgpoipoitype.Count > 0 ? mysmgpoipoitype.FirstOrDefault()?.TagName[langcat] : "";
                        mymuseum.AdditionalPoiInfos.TryAddOrUpdate(langcat, additional);
                    }

                    //Setting Categorization by Valid Tags
                    var currentcategories = validtagsforcategories.Where(x => mymuseum.SmgTags.Contains(x.Id.ToLower()));
                    foreach (var smgtagtotranslate in currentcategories)
                    {
                        foreach (var languagecategory in languagelistcategories)
                        {
                            if (mymuseum.AdditionalPoiInfos[languagecategory].Categories == null)
                                mymuseum.AdditionalPoiInfos[languagecategory].Categories = new List<string>();

                            if (smgtagtotranslate.TagName.ContainsKey(languagecategory) && (!mymuseum.AdditionalPoiInfos[languagecategory].Categories?.Contains(smgtagtotranslate.TagName[languagecategory].Trim()) ?? false))
                                mymuseum.AdditionalPoiInfos[languagecategory].Categories?.Add(smgtagtotranslate.TagName[languagecategory].Trim());
                        }
                    }
                }

                //Setting Common Infos
                mymuseum.Source = "SIAG";
                mymuseum.SyncSourceInterface = "museumdata";
                mymuseum.SyncUpdateMode = "Full";
                mymuseum.LastChange = DateTime.Now;

                //Setting LicenseInfo
                mymuseum.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<ODHActivityPoi>(mymuseum, Helper.LicenseHelper.GetLicenseforOdhActivityPoi);

                //Special get all Taglist and traduce it on import
                await GenericTaggingHelper.AddMappingToODHActivityPoi(mymuseum, settings.JsonConfig.Jsondir);

                if (mymuseumdata?.Root is { })
                {
                    var result = await InsertDataToDB(mymuseum, new KeyValuePair<string, XElement>(museumid, mymuseumdata.Root));
                    newcounter = newcounter + result.created ?? 0;
                    updatecounter = updatecounter + result.updated ?? 0;
                    if (mymuseum.Id is { })
                        WriteLog.LogToConsole(mymuseum.Id, "dataimport", "single.siagmuseum", new ImportLog() { sourceid = mymuseum.Id, sourceinterface = "siag.museum", success = true, error = "" });
                }
            }
            catch(Exception ex)
            {
                WriteLog.LogToConsole(museumidtoreturn, "dataimport", "single.siagmuseum", new ImportLog() { sourceid = museumidtoreturn, sourceinterface = "siag.museum", success = false, error = ex.Message });

                errorcounter = errorcounter + 1;
            }

            return new UpdateDetail() { created = newcounter, updated = updatecounter, deleted = 0, error = errorcounter };
        }

        private async Task<UpdateDetail> SetDataNotinListToInactive(XDocument mymuseumlist, CancellationToken cancellationToken)
        {
            //CHECK FOR ERRORS HERE

            List<string?> mymuseumroot = mymuseumlist.Root?.Elements("Museum").Select(x => x.Attribute("ID")?.Value).ToList() ?? new();
      
            var mymuseumquery = QueryFactory.Query("smgpois")
                .Select("data->>'CustomId'")
                .Where("gen_syncsourceinterface", "museumdata");

            var mymuseumsonraven = await mymuseumquery.GetAsync<string>();
            
            var idstodelete = mymuseumsonraven.Where(p => !mymuseumroot.Any(p2 => p2 == p));

            int updateresult = 0;
            int deleteresult = 0;

            foreach (var idtodelete in idstodelete)
            {
                var result = await DeleteOrDisableData(idtodelete, false);

                updateresult = updateresult + result.Item1;
                deleteresult = deleteresult + result.Item2;
            }
            
            return new UpdateDetail() { created = 0, updated = updateresult, deleted = deleteresult };
        }

        private async Task<PGCRUDResult> InsertDataToDB(ODHActivityPoiLinked odhactivitypoi, KeyValuePair<string, XElement> siagmuseumdata)
        {
            odhactivitypoi.Id = odhactivitypoi.Id?.ToLower();

            //Set LicenseInfo
            odhactivitypoi.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<ODHActivityPoi>(odhactivitypoi, Helper.LicenseHelper.GetLicenseforOdhActivityPoi);

            var rawdataid = await InsertInRawDataDB(siagmuseumdata);

            return await QueryFactory.UpsertData<ODHActivityPoiLinked>(odhactivitypoi, "smgpois", rawdataid);
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
                            type = "odhactivitypoi-museum",
                            license = "open",
                            rawformat = "xml"
                        });
        }
        
        #endregion
    }
}
