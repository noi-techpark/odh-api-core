using DataModel;
using Helper;
using SqlKata.Execution;
using SuedtirolWein;
using SuedtirolWein.Parser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OdhApiImporter.Helpers.SuedtirolWein
{
    public class SuedtirolWeinCompanyImportHelper : ImportHelper, IImportHelper
    {
        public SuedtirolWeinCompanyImportHelper(ISettings settings, QueryFactory queryfactory, string table) : base(settings, queryfactory, table)
        {
            
        }

        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, List<string>? idlist = null, CancellationToken cancellationToken = default)
        {
            var winegastrolist = await ImportList(cancellationToken);

            var updateresult = await ImportData(winegastrolist, cancellationToken);
            
            var deleteresult = await SetDataNotinListToInactive(winegastrolist, cancellationToken);            

            return GenericResultsHelper.MergeUpdateDetail(new List<UpdateDetail>() { updateresult, deleteresult });
        }

        public async Task<IDictionary<string, XDocument>> ImportList(CancellationToken cancellationToken = default)
        {
            var winedatalistde = await GetSuedtirolWeinData.GetSueditrolWineCompaniesAsync("de");
            var winedatalistit = await GetSuedtirolWeinData.GetSueditrolWineCompaniesAsync("it");
            var winedatalisten = await GetSuedtirolWeinData.GetSueditrolWineCompaniesAsync("en");
            //New getting in jp and ru and us
            var winedatalistjp = await GetSuedtirolWeinData.GetSueditrolWineCompaniesAsync("jp");
            var winedatalistru = await GetSuedtirolWeinData.GetSueditrolWineCompaniesAsync("ru");
            var winedatalistus = await GetSuedtirolWeinData.GetSueditrolWineCompaniesAsync("us");

            IDictionary<string, XDocument> mywinedata = new Dictionary<string, XDocument>();
            mywinedata.Add("de", winedatalistde);
            mywinedata.Add("it", winedatalistit);
            mywinedata.Add("en", winedatalisten);
            mywinedata.Add("ru", winedatalistru);
            mywinedata.Add("jp", winedatalistjp);
            mywinedata.Add("us", winedatalistus);

            return mywinedata;
        }

        private async Task<UpdateDetail> ImportData(IDictionary<string, XDocument> wineddatalist, CancellationToken cancellationToken = default)
        {
            int updatecounter = 0;
            int newcounter = 0;
            int errorcounter = 0;
            
            //For AdditionalInfos
            List<string> languagelistcategories = new List<string>() { "de", "it", "en", "nl", "cs", "pl", "fr", "ru" };

            //Getting valid Tags for Weinkellereien
            var validtagsforcategories = await ODHTagHelper.GetODHTagsValidforTranslations(QueryFactory, new List<string>() { "Essen Trinken" }); //Essen Trinken ??

            //Load Type + Subtype fore each language
            var suedtiroltypemain = await ODHTagHelper.GeODHTagByID(QueryFactory, "Essen Trinken");
            var suedtiroltypesub = await ODHTagHelper.GeODHTagByID(QueryFactory, "Weinkellereien");

            //Loading District & Municipality data
            var districtreducedinfo = await GpsHelper.GetReducedWithGPSInfoList(QueryFactory, "districts");
            var municipalityreducedinfo = await GpsHelper.GetReducedWithGPSInfoList(QueryFactory, "municipalities");

            //Loading WineAwards
            var wineawardlist = await WineAwardHelper.GetReducedWithWineAwardList(QueryFactory);

            foreach (var winedata in wineddatalist["de"].Root?.Elements("item") ?? Enumerable.Empty<XElement>())
            {
                var importresult = await ImportDataSingle(winedata, 
                    wineddatalist, languagelistcategories, validtagsforcategories, 
                    suedtiroltypemain, suedtiroltypesub, wineawardlist.ToList(),
                    municipalityreducedinfo.ToList(), districtreducedinfo.ToList());

                newcounter = newcounter + importresult.created ?? newcounter;
                updatecounter = updatecounter + importresult.updated ?? updatecounter;
                errorcounter = errorcounter + importresult.error ?? errorcounter;
            }

            return new UpdateDetail() { created = newcounter, updated = updatecounter, deleted = 0, error = errorcounter };
        }

        public async Task<UpdateDetail> ImportDataSingle(
            XElement winedata, 
            IDictionary<string, XDocument> winedatalist, 
            List<string> languagelistcategories, 
            IEnumerable<SmgTags> validtagsforcategories, 
            SmgTags suedtiroltypemain, 
            SmgTags suedtiroltypesub,
            List<ReducedWineAward> wineawardreducelist,
            List<ReducedWithGPSInfo> municipalityreducedlist,
            List<ReducedWithGPSInfo> districtreducedlist
            )
        {            
            int updatecounter = 0;
            int newcounter = 0;
            int errorcounter = 0;

            string dataid = winedata.Element("id").Value;

            try
            {
                var winedatait = winedatalist["it"].Root.Elements("item").Where(x => x.Element("id").Value == dataid).FirstOrDefault();
                var winedataen = winedatalist["en"].Root.Elements("item").Where(x => x.Element("id").Value == dataid).FirstOrDefault();
                var winedataru = winedatalist["ru"].Root.Elements("item").Where(x => x.Element("id").Value == dataid).FirstOrDefault();
                var winedatajp = winedatalist["jp"].Root.Elements("item").Where(x => x.Element("id").Value == dataid).FirstOrDefault();
                var winedataus = winedatalist["us"].Root.Elements("item").Where(x => x.Element("id").Value == dataid).FirstOrDefault();


                IDictionary<string, XElement> mywinecompanies = new Dictionary<string, XElement>();
                mywinecompanies.Add("de", winedata);

                List<string> haslanguage = new List<string>();
                haslanguage.Add("de");
                bool newwinecompany = false;

                if (winedatait != null)
                {
                    haslanguage.Add("it");
                    mywinecompanies.Add("it", winedatait);
                }
                if (winedataen != null)
                {
                    haslanguage.Add("en");
                    mywinecompanies.Add("en", winedataen);
                }
                if (winedataru != null)
                {
                    haslanguage.Add("ru");
                    mywinecompanies.Add("ru", winedataru);
                }
                if (winedatajp != null)
                {
                    haslanguage.Add("jp");
                    mywinecompanies.Add("jp", winedatajp);
                }
                if (winedataus != null)
                {
                    haslanguage.Add("us");
                    mywinecompanies.Add("us", winedataus);
                }

                //Improve Performance this query is very slow!!!!
                //var mysuedtirolweinquery = QueryFactory.Query("smgpois")
                //    .Select("data")
                //    .WhereRaw("data->>'CustomId' = $$", dataid.ToLower());
                var mysuedtirolweinquery = QueryFactory.Query("smgpois")
                    .Select("data")
                    .Where("id", dataid.ToLower());


                var suedtirolweinpoi = await mysuedtirolweinquery.GetFirstOrDefaultAsObject<ODHActivityPoiLinked>();

                if (suedtirolweinpoi == null)
                {
                    suedtirolweinpoi = new ODHActivityPoiLinked();
                    suedtirolweinpoi.FirstImport = DateTime.Now;
                    newwinecompany = true;
                }

                suedtirolweinpoi = ParseCompanyData.ParsetheCompanyData(suedtirolweinpoi, mywinecompanies, haslanguage);

                //TODO Set locinfo based on GPS
                bool setinactive = false;

                if (newwinecompany)
                {
                    suedtirolweinpoi.Active = true;
                    suedtirolweinpoi.SmgActive = true;

                    if (suedtirolweinpoi.GpsInfo != null && suedtirolweinpoi.GpsInfo.Count > 0 && suedtirolweinpoi.GpsInfo.FirstOrDefault()?.Latitude != 0 && suedtirolweinpoi.GpsInfo.FirstOrDefault()?.Longitude != 0)
                    {
                        var district = await GetLocationInfo.GetNearestDistrictbyGPS(QueryFactory, suedtirolweinpoi.GpsInfo.FirstOrDefault()!.Latitude, suedtirolweinpoi.GpsInfo.FirstOrDefault()!.Longitude, 30000);

                        if (district != null)
                        {
                            var locinfo = await GetLocationInfo.GetTheLocationInfoDistrict(QueryFactory, district.Id);

                            suedtirolweinpoi.LocationInfo = locinfo;
                            suedtirolweinpoi.TourismorganizationId = locinfo.TvInfo?.Id;
                        }
                        else
                        {
                            var locinfo = new LocationInfoLinked();
                            
                            suedtirolweinpoi.LocationInfo = locinfo;

                            setinactive = true;
                        }
                    }
                }

                //Hack if Locationinfo not set
                if (suedtirolweinpoi.LocationInfo == null)
                {
                    suedtirolweinpoi.LocationInfo = new LocationInfoLinked();
                }

                //Type Informations
                
                suedtirolweinpoi.Type = suedtiroltypemain?.Shortname;
                suedtirolweinpoi.SubType = suedtiroltypesub?.Shortname;

                foreach (var langcat in languagelistcategories)
                {
                    AdditionalPoiInfos additional = new AdditionalPoiInfos();
                    additional.Language = langcat;
                    additional.MainType = suedtiroltypemain.TagName[langcat];
                    additional.SubType = suedtiroltypesub.TagName[langcat];
                    additional.PoiType = "";
                    suedtirolweinpoi.AdditionalPoiInfos.TryAddOrUpdate(langcat, additional);
                }

                //Tags
                suedtirolweinpoi.SmgTags ??= new List<string>();
                if (suedtiroltypemain?.Id is { } && !suedtirolweinpoi.SmgTags.Contains(suedtiroltypemain.Id.ToLower()))
                    suedtirolweinpoi.SmgTags.Add(suedtiroltypemain.Id.ToLower());
                if (suedtiroltypesub?.Id is { } && !suedtirolweinpoi.SmgTags.Contains(suedtiroltypesub.Id.ToLower()))
                    suedtirolweinpoi.SmgTags.Add(suedtiroltypesub.Id.ToLower());


                //Setting Categorization by Valid Tags
                var currentcategories = validtagsforcategories.Where(x => suedtirolweinpoi.SmgTags.Contains(x.Id.ToLower()));
                foreach (var smgtagtotranslate in currentcategories)
                {
                    foreach (var languagecategory in languagelistcategories)
                    {
                        if (suedtirolweinpoi.AdditionalPoiInfos[languagecategory].Categories == null)
                            suedtirolweinpoi.AdditionalPoiInfos[languagecategory].Categories = new List<string>();

                        if (smgtagtotranslate.TagName.ContainsKey(languagecategory) && (!suedtirolweinpoi.AdditionalPoiInfos[languagecategory].Categories?.Contains(smgtagtotranslate.TagName[languagecategory].Trim()) ?? false))
                            suedtirolweinpoi.AdditionalPoiInfos[languagecategory].Categories?.Add(smgtagtotranslate.TagName[languagecategory].Trim());
                    }
                }

                //RELATED CONTENT
                //Wineids als RElated Content
                if (!String.IsNullOrEmpty(winedata.Element("wineids").Value))
                {
                    List<RelatedContent> myrelatedcontentlist = new List<RelatedContent>();

                    var mywines = wineawardreducelist
                        .Where(x => x.CompanyId == dataid)
                       .ToList();

                    foreach (var mywine in mywines)
                    {
                        RelatedContent relatedcontent = new RelatedContent();
                        relatedcontent.Id = mywine.Id;
                        relatedcontent.Name = mywine.Name;
                        relatedcontent.Type = "WineAward";

                        myrelatedcontentlist.Add(relatedcontent);
                    }

                    suedtirolweinpoi.RelatedContent = myrelatedcontentlist.ToList();
                }

                if (setinactive)
                {
                    suedtirolweinpoi.Active = false;
                    suedtirolweinpoi.SmgActive = false;
                }
                else
                {
                    suedtirolweinpoi.Active = true;
                    suedtirolweinpoi.SmgActive = true;
                }

                suedtirolweinpoi.HasLanguage = haslanguage;

                //Setting Common Infos
                suedtirolweinpoi.Source = "suedtirolwein";
                suedtirolweinpoi.SyncSourceInterface = "suedtirolwein-company";
                suedtirolweinpoi.SyncUpdateMode = "Full";
                suedtirolweinpoi.LastChange = DateTime.Now;

                //Calculate GPS Distance to District and Municipality
                if (suedtirolweinpoi.LocationInfo != null)
                {
                    if (suedtirolweinpoi.LocationInfo.DistrictInfo != null)
                    {
                        var districtreduced = districtreducedlist.Where(x => x.Id == suedtirolweinpoi.LocationInfo.DistrictInfo.Id).FirstOrDefault();
                        if (districtreduced != null)
                        {
                            suedtirolweinpoi.ExtendGpsInfoToDistanceCalculationList("district", districtreduced.Latitude, districtreduced.Longitude);
                        }
                    }
                    if (suedtirolweinpoi.LocationInfo.MunicipalityInfo != null)
                    {
                        var municipalityreduced = municipalityreducedlist.Where(x => x.Id == suedtirolweinpoi.LocationInfo.MunicipalityInfo.Id).FirstOrDefault();
                        if (municipalityreduced != null)
                        {
                            suedtirolweinpoi.ExtendGpsInfoToDistanceCalculationList("municipality", municipalityreduced.Latitude, municipalityreduced.Longitude);
                        }
                    }
                }

                //Set Main Type
                ODHActivityPoiHelper.SetMainCategorizationForODHActivityPoi(suedtirolweinpoi);

                //Add Mapping
                var suedtirolweinid = new Dictionary<string, string>() { { "id", dataid } };
                suedtirolweinpoi.Mapping.TryAddOrUpdate("suedtirolwein", suedtirolweinid);
                
                //Set Tags based on OdhTags
                await GenericTaggingHelper.AddMappingToODHActivityPoi(suedtirolweinpoi, settings.JsonConfig.Jsondir);

                //Set Publishedon List
                suedtirolweinpoi.PublishedOn = PublishedOnHelper.GetPublishenOnList("odhactivitypoi", suedtirolweinpoi.OdhActive);

                var result = await InsertDataToDB(suedtirolweinpoi, new KeyValuePair<string, XElement>(dataid, winedata));
                newcounter = newcounter + result.created ?? 0;
                updatecounter = updatecounter + result.updated ?? 0;
                
                if (suedtirolweinpoi.Id is { })
                    WriteLog.LogToConsole(dataid, "dataimport", "single.suedtirolweincompany", new ImportLog() { sourceid = dataid, sourceinterface = "suedtirolwein.company", success = true, error = "" });

            }
            catch (Exception ex)
            {
                WriteLog.LogToConsole(dataid, "dataimport", "single.suedtirolweincompany", new ImportLog() { sourceid = dataid, sourceinterface = "suedtirolwein.company", success = false, error = ex.Message });

                errorcounter = errorcounter + 1;
            }            

            return new UpdateDetail() { created = newcounter, updated = updatecounter, deleted = 0, error = errorcounter };
        }

        private async Task<UpdateDetail> SetDataNotinListToInactive(IDictionary<string, XDocument> mywinecompanylist, CancellationToken cancellationToken)
        {
            int updateresult = 0;
            int deleteresult = 0;
            int errorresult = 0;

            try
            {               
                //The service returns always the same ids in each language
                List<string?> winecompaniesonsource = mywinecompanylist["de"].Root?.Elements("item").Select(x => x.Attribute("id")?.Value).ToList() ?? new();

                var myquery = QueryFactory.Query("smgpois")
                    .SelectRaw("data->>'CustomId'")
                    .Where("gen_syncsourceinterface", "suedtirolwein");

                var winecompaniesondb = await myquery.GetAsync<string>();

                var idstodelete = winecompaniesondb.Where(p => !winecompaniesonsource.Any(p2 => p2 == p));

                foreach (var idtodelete in idstodelete)
                {
                    var result = await DeleteOrDisableData(idtodelete, false);

                    updateresult = updateresult + result.Item1;
                    deleteresult = deleteresult + result.Item2;
                }
            }
            catch (Exception ex)
            {
                WriteLog.LogToConsole("", "dataimport", "deactivate.suedtirolweincompany", new ImportLog() { sourceid = "", sourceinterface = "suedtirolwein.company", success = false, error = ex.Message });

                errorresult = errorresult + 1;
            }

            return new UpdateDetail() { created = 0, updated = updateresult, deleted = deleteresult, error = errorresult };
        }

        private async Task<PGCRUDResult> InsertDataToDB(ODHActivityPoiLinked odhactivitypoi, KeyValuePair<string, XElement> suedtirolweindata)
        {
            odhactivitypoi.Id = odhactivitypoi.Id?.ToLower();

            //Set LicenseInfo
            odhactivitypoi.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<ODHActivityPoi>(odhactivitypoi, Helper.LicenseHelper.GetLicenseforOdhActivityPoi);

            var rawdataid = await InsertInRawDataDB(suedtirolweindata);

            return await QueryFactory.UpsertData<ODHActivityPoiLinked>(odhactivitypoi, "smgpois", rawdataid, "suedtirolwein.company.import", "importer");
        }

        private async Task<int> InsertInRawDataDB(KeyValuePair<string, XElement> suedtirolweindata)
        {
            return await QueryFactory.InsertInRawtableAndGetIdAsync(
                        new RawDataStore()
                        {
                            datasource = "suedtirolwein",
                            importdate = DateTime.Now,
                            raw = suedtirolweindata.Value.ToString(),
                            sourceinterface = "suedtirolwein-company",
                            sourceid = suedtirolweindata.Key,
                            sourceurl = "https://suedtirolwein.secure.consisto.net/companies.ashx",
                            type = "odhactivitypoi_winecompany",
                            license = "open",
                            rawformat = "xml"
                        });
        }

    }
}
