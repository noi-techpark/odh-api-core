using DataModel;
using SqlKata.Execution;
using SuedtirolWein;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OdhApiImporter.Helpers.SuedtirolWein
{
    public class SuedtirolWeinImportHelper
    {
        private readonly QueryFactory QueryFactory;
        private readonly ISettings settings;

        public SuedtirolWeinImportHelper(ISettings settings, QueryFactory queryfactory)
        {
            this.QueryFactory = queryfactory;
            this.settings = settings;
        }

        public async Task<UpdateDetail> SaveSuedtirolWeinGastronomiesToODH(QueryFactory QueryFactory, DateTime? lastchanged = null, CancellationToken cancellationToken = default)
        {
            var winegastrolist = await GetSuedtirolWeinGastronomiesList(cancellationToken);

            foreach (var winedata in winegastrolist["de"].Root.Elements("item"))
            {

            }
            //var updateresult = await ImportMuseums(museumslist, cancellationToken);
            //SetMuseumsnotinListToInactive()
            //SetSuedtirolWineCompanyToInactive(documentStore, tracesource, log, winedatalistde);

            //return updateresult;
            return new UpdateDetail();
        }

        public async Task<IDictionary<string, XDocument>> GetSuedtirolWeinGastronomiesList(CancellationToken cancellationToken = default)
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

        public async Task ImportSuedtirolWeinGastronomy(XElement winedata)
        {
            //ImportSuedtirolWineCompanySingle(documentStore, tracesource, log, winedata, mywinedata, i);
        }

    }
}
