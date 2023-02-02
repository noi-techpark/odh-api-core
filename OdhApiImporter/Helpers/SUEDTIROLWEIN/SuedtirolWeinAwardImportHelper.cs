﻿using DataModel;
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
    public class SuedtirolWeinAwardImportHelper : ImportHelper, IImportHelper
    {
        public SuedtirolWeinAwardImportHelper(ISettings settings, QueryFactory queryfactory, string table, string importerURL) : base(settings, queryfactory, table, importerURL)
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
            var wineawarddatalistde = await GetSuedtirolWeinData.GetSueditrolWineAwardsAsync("de");
            var wineawarddatalistit = await GetSuedtirolWeinData.GetSueditrolWineAwardsAsync("it");
            var wineawarddatalisten = await GetSuedtirolWeinData.GetSueditrolWineAwardsAsync("en");

            IDictionary<string, XDocument> mywinedata = new Dictionary<string, XDocument>();
            mywinedata.Add("de", wineawarddatalistde);
            mywinedata.Add("it", wineawarddatalistit);
            mywinedata.Add("en", wineawarddatalisten);
            
            return mywinedata;
        }

        private async Task<UpdateDetail> ImportData(IDictionary<string, XDocument> wineddatalist, CancellationToken cancellationToken = default)
        {
            int updatecounter = 0;
            int newcounter = 0;
            int errorcounter = 0;
                                    
            foreach (var winedata in wineddatalist["de"].Root?.Elements("item") ?? Enumerable.Empty<XElement>())
            {
                var importresult = await ImportDataSingle(winedata, wineddatalist);

                newcounter = newcounter + importresult.created ?? newcounter;
                updatecounter = updatecounter + importresult.updated ?? updatecounter;
                errorcounter = errorcounter + importresult.error ?? errorcounter;
            }

            return new UpdateDetail() { created = newcounter, updated = updatecounter, deleted = 0, error = errorcounter };
        }

        public async Task<UpdateDetail> ImportDataSingle(
            XElement winedata, 
            IDictionary<string, XDocument> winedatalist            
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


                List<string> haslanguage = new List<string>() { "de" };

                if (winedatait != null)
                    haslanguage.Add("it");
                if (winedataen != null)
                    haslanguage.Add("en");
                
                //Get Wein Award
                var mysuedtirolweinquery = QueryFactory.Query("wine")
                    .Select("data")
                    .Where("id", dataid.ToUpper());

                var weinaward = await mysuedtirolweinquery.GetObjectSingleAsync<WineLinked>();

                if (weinaward == null)
                {
                    weinaward = new WineLinked();
                    weinaward.FirstImport = DateTime.Now;
                }

                weinaward = ParseAwardData.ParsetheAwardData(weinaward, winedata, winedatait, winedataen, haslanguage);

                //Add nullchecks field was removed
                if (winedata.Element("active") != null)
                {
                    weinaward.Active = Convert.ToBoolean(winedata.Element("active").Value);
                    weinaward.SmgActive = Convert.ToBoolean(winedata.Element("active").Value);
                }
         
                weinaward.HasLanguage = haslanguage;

                //Setting Common Infos
                weinaward.Source = "suedtirolwein";                
                weinaward.LastChange = DateTime.Now;
            
                //ADD MAPPING
                var suedtirolweinid = new Dictionary<string, string>() { { "id", dataid } };
                weinaward.Mapping.TryAddOrUpdate("suedtirolwein", suedtirolweinid);

                //Set Publishedon List
                weinaward.PublishedOn = PublishedOnHelper.GetPublishenOnList("wine", weinaward.OdhActive);

                var result = await InsertDataToDB(weinaward, new KeyValuePair<string, XElement>(dataid, winedata));
                newcounter = newcounter + result.created ?? 0;
                updatecounter = updatecounter + result.updated ?? 0;
                                
                WriteLog.LogToConsole(dataid, "dataimport", "single.suedtirolweinaward", new ImportLog() { sourceid = dataid, sourceinterface = "suedtirolwein.award", success = true, error = "" });
            }
            catch (Exception ex)
            {
                WriteLog.LogToConsole(dataid, "dataimport", "single.suedtirolweinaward", new ImportLog() { sourceid = dataid, sourceinterface = "suedtirolwein.award", success = false, error = ex.Message });

                errorcounter = errorcounter + 1;
            }            

            return new UpdateDetail() { created = newcounter, updated = updatecounter, deleted = 0, error = errorcounter };
        }

        private async Task<UpdateDetail> SetDataNotinListToInactive(IDictionary<string, XDocument> wineawarddatalist, CancellationToken cancellationToken)
        {
            int updateresult = 0;
            int deleteresult = 0;
            int errorresult = 0;

            try
            {               
                //The service returns always the same ids in each language
                List<string?> wineidlistonsource = wineawarddatalist["de"].Root?.Elements("item").Select(x => x.Attribute("id")?.Value.ToUpper()).ToList() ?? new();

                var myquery = QueryFactory.Query("wines")
                    .Select("id");

                var winesondb = await myquery.GetAsync<string>();

                var idstodelete = winesondb.Where(p => !wineidlistonsource.Any(p2 => p2 == p));

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

        private async Task<PGCRUDResult> InsertDataToDB(WineLinked wineaward, KeyValuePair<string, XElement> suedtirolweindata)
        {
            wineaward.Id = wineaward.Id?.ToUpper();

            //Set LicenseInfo
            wineaward.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<WineLinked>(wineaward, Helper.LicenseHelper.GetLicenseforWineAward);

            var rawdataid = await InsertInRawDataDB(suedtirolweindata);

            //TODO Add column rawdataid
            return await QueryFactory.UpsertData<WineLinked>(wineaward, "wines", rawdataid, "suedtirolwein.weinaward.import", importerURL);
        }

        private async Task<int> InsertInRawDataDB(KeyValuePair<string, XElement> suedtirolweindata)
        {
            return await QueryFactory.InsertInRawtableAndGetIdAsync(
                        new RawDataStore()
                        {
                            datasource = "suedtirolwein",
                            importdate = DateTime.Now,
                            raw = suedtirolweindata.Value.ToString(),
                            sourceinterface = "suedtirolwein-award",
                            sourceid = suedtirolweindata.Key,
                            sourceurl = "https://suedtirolwein.secure.consisto.net/awards.ashx",
                            type = "common_wineaward",
                            license = "open",
                            rawformat = "xml"
                        });
        }

    }
}
