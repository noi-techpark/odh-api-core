using DataModel;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helper;

namespace OdhApiImporter.Helpers
{
    /// <summary>
    /// This class is used for different update operations on the data
    /// </summary>
    public class CustomDataOperation
    {
        private readonly QueryFactory QueryFactory;
        private readonly ISettings settings;

        public CustomDataOperation(ISettings settings, QueryFactory queryfactory)
        {
            this.QueryFactory = queryfactory;
            this.settings = settings;
        }

        public async Task<int> UpdateAllEventShortstonewDataModel()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("eventeuracnoi");

            var data = await query.GetObjectListAsync<EventShortLinked>();
            int i = 0;

            foreach (var eventshort in data)
            {
                if (eventshort.LastChange == null)
                    eventshort.LastChange = eventshort.ChangedOn;

                //Setting MetaInfo
                eventshort._Meta = MetadataHelper.GetMetadataobject<EventShortLinked>(eventshort, MetadataHelper.GetMetadataforEventShort);
                eventshort._Meta.LastUpdate = eventshort.LastChange;

                //Save tp DB
                //TODO CHECK IF THIS WORKS     
                var queryresult = await QueryFactory.Query("eventeuracnoi").Where("id", eventshort.Id)
                    //.UpdateAsync(new JsonBData() { id = eventshort.Id.ToLower(), data = new JsonRaw(eventshort) });
                    .UpdateAsync(new JsonBData() { id = eventshort.Id?.ToLower() ?? "", data = new JsonRaw(eventshort) });

                i++;
            }

            return i;
        }

        public async Task<int> UpdateAllSTAVendingpoints()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("smgpois")
                   .Where("gen_source", "sta");

            var data = await query.GetObjectListAsync<ODHActivityPoiLinked>();
            int i = 0;

            foreach (var stapoi in data)
            {
                //Setting MetaInfo
                stapoi._Meta.Reduced = false;
                stapoi.Source = "sta";

                //Save tp DB
                //TODO CHECK IF THIS WORKS     
                var queryresult = await QueryFactory.Query("smgpois").Where("id", stapoi.Id)
                    //.UpdateAsync(new JsonBData() { id = eventshort.Id.ToLower(), data = new JsonRaw(eventshort) });
                    .UpdateAsync(new JsonBData() { id = stapoi.Id?.ToLower() ?? "", data = new JsonRaw(stapoi) });

                i++;
            }

            return i;
        }

        public async Task<int> FillDBWithDummyNews()
        {
            int crudcount = 0;

            for (int i = 1; i <= 120; i++)
            {
                ArticlesLinked myarticle  = new ArticlesLinked();
                myarticle.Id = Guid.NewGuid().ToString().ToUpper();
                myarticle.Type = "newsfeednoi";
                myarticle.Active = true;
                myarticle.Detail.TryAddOrUpdate("de", new Detail() { Title = "TesttitleDE" + i, BaseText = "testtextDE " + i, Language = "de", AdditionalText = "additionaltextde" + i });
                myarticle.Detail.TryAddOrUpdate("it", new Detail() { Title = "TesttitleIT" + i, BaseText = "testtextIT " + i, Language = "it", AdditionalText = "additionaltextit" + i });
                myarticle.Detail.TryAddOrUpdate("en", new Detail() { Title = "TesttitleEN" + i, BaseText = "testtextEN " + i, Language = "en", AdditionalText = "additionaltexten" + i });

                myarticle.HasLanguage = new List<string>() { "de", "it", "en" };

                myarticle.LicenseInfo = new LicenseInfo() { Author = "", License = "CC0", ClosedData = false, LicenseHolder= "https://noi.bz.it" };

                myarticle.ContactInfos.TryAddOrUpdate("de", new ContactInfos() { Email = "community@noi.bz.it", LogoUrl = "https://databrowser.opendatahub.bz.it/icons/NOI.png", Language = "de", CompanyName = "NOI Techpark" });
                myarticle.ContactInfos.TryAddOrUpdate("it", new ContactInfos() { Email = "community@noi.bz.it", LogoUrl = "https://databrowser.opendatahub.bz.it/icons/NOI.png", Language = "it", CompanyName = "NOI Techpark" });
                myarticle.ContactInfos.TryAddOrUpdate("en", new ContactInfos() { Email = "community@noi.bz.it", LogoUrl = "https://databrowser.opendatahub.bz.it/icons/NOI.png", Language = "en", CompanyName = "NOI Techpark" });

                myarticle.ArticleDate = DateTime.Now.Date.AddDays(i);
                
                if (i % 5 == 0)
                {
                    myarticle.ArticleDateTo = DateTime.Now.Date.AddMonths(i);
                }
                else
                    myarticle.ArticleDateTo = DateTime.MaxValue;

                myarticle.SmgActive = true;
                myarticle.Source = "noi";

                if(i % 3 == 0)
                {
                    myarticle.SmgTags = new List<string>() { "important" };
                }

                var pgcrudresult = await QueryFactory.UpsertData<ArticlesLinked>(myarticle, "articles");

                if(pgcrudresult.created != null)
                    crudcount = crudcount + pgcrudresult.created.Value;

            }

            return crudcount;
        }        
    }
}
