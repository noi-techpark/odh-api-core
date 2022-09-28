using DataModel;
using Helper;
using SqlKata.Execution;
using SuedtirolWein;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OdhApiImporter.Helpers.SuedtirolWein
{
    public class SuedtirolWeinImportHelper: ImportHelper, IImportHelper
    {
        private readonly QueryFactory QueryFactory;
        private readonly ISettings settings;

        public SuedtirolWeinImportHelper(ISettings settings, QueryFactory queryfactory, string table) : base(settings, queryfactory, table)
        {
            
        }

        public async Task<UpdateDetail> SaveDataToODH(DateTime? lastchanged = null, CancellationToken cancellationToken = default)
        {
            var winegastrolist = await ImportList(cancellationToken);

            var updateresult = await ImportData(winegastrolist, cancellationToken);

            var deleteresult = await SetDataNotinListToInactive(winegastrolist["de"], cancellationToken);

            //SetMuseumsnotinListToInactive()
            //SetSuedtirolWineCompanyToInactive(documentStore, tracesource, log, winedatalistde);

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

        private async Task<UpdateDetail> ImportData(IDictionary<string, XDocument> winegastrolist, CancellationToken cancellationToken = default)
        {
            int updatecounter = 0;
            int newcounter = 0;
            int errorcounter = 0;

            //Load ValidTagsfor Categories
            var validtagsforcategories = default(IEnumerable<SmgTags>);
            //For AdditionalInfos
            List<string> languagelistcategories = new List<string>() { "de", "it", "en", "nl", "cs", "pl", "fr", "ru" };

            //Getting valid Tags for Museums
            validtagsforcategories = await ODHTagHelper.GetODHTagsValidforTranslations(QueryFactory, new List<string>() { "Weinkellereien" });


            foreach (var winedata in winegastrolist["de"].Root?.Elements("item") ?? Enumerable.Empty<XElement>())
            {
                var importresult = await ImportDataSingle(winedata, languagelistcategories, validtagsforcategories);

                newcounter = newcounter + importresult.created ?? newcounter;
                updatecounter = updatecounter + importresult.updated ?? updatecounter;
                errorcounter = errorcounter + importresult.error ?? errorcounter;
            }

            return new UpdateDetail() { created = newcounter, updated = updatecounter, deleted = 0, error = errorcounter };
        }

        public async Task<UpdateDetail> ImportDataSingle(XElement winedata, List<string> languagelistcategories, IEnumerable<SmgTags> validtagsforcategories)
        {
            string idtoreturn = "";
            int updatecounter = 0;
            int newcounter = 0;
            int errorcounter = 0;
            try
            {
                //ImportSuedtirolWineCompanySingle(documentStore, tracesource, log, winedata, mywinedata, i);
            }
            catch (Exception ex)
            {
                WriteLog.LogToConsole(idtoreturn, "dataimport", "single.suedtirolweincompany", new ImportLog() { sourceid = idtoreturn, sourceinterface = "suedtirolwein.company", success = false, error = ex.Message });

                errorcounter = errorcounter + 1;
            }            

            return new UpdateDetail() { created = newcounter, updated = updatecounter, deleted = 0, error = errorcounter };
        }

        private async Task<UpdateDetail> SetDataNotinListToInactive(XDocument mywinecompanylist, CancellationToken cancellationToken)
        {
            int updateresult = 0;
            int deleteresult = 0;
            int errorresult = 0;

            try
            {
                List<string?> mymuseumroot = mywinecompanylist.Root?.Elements("item").Select(x => x.Attribute("ID")?.Value).ToList() ?? new();

                var mymuseumquery = QueryFactory.Query("smgpois")
                    .SelectRaw("data->>'CustomId'")
                    .Where("gen_syncsourceinterface", "suedtirolwein");

                var mymuseumsonraven = await mymuseumquery.GetAsync<string>();

                var idstodelete = mymuseumsonraven.Where(p => !mymuseumroot.Any(p2 => p2 == p));


                foreach (var idtodelete in idstodelete)
                {
                    var result = await DeleteOrDisableData(idtodelete, false);

                    updateresult = updateresult + result.Item1;
                    deleteresult = deleteresult + result.Item2;
                }
            }
            catch (Exception ex)
            {
                WriteLog.LogToConsole("", "dataimport", "deactivate.siagmuseum", new ImportLog() { sourceid = "", sourceinterface = "siag.museum", success = false, error = ex.Message });

                errorresult = errorresult + 1;
            }

            return new UpdateDetail() { created = 0, updated = updateresult, deleted = deleteresult, error = errorresult };
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

    }
}
