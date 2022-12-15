using DataModel;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helper;
using Microsoft.FSharp.Control;

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

        public async Task<int> UpdateMetaDataApiRecordCount()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("metadata");

            var data = await query.GetObjectListAsync<TourismMetaData>();
            int i = 0;

            foreach (var metadata in data)
            {
                metadata.RecordCount = await MetaDataApiRecordCount.GetRecordCountfromDB(metadata.ApiFilter, metadata.OdhType, QueryFactory);

                //Save tp DB                 
                var queryresult = await QueryFactory.Query("metadata").Where("id", metadata.Id)                    
                    .UpdateAsync(new JsonBData() { id = metadata.Id?.ToLower() ?? "", data = new JsonRaw(metadata) });

                i++;
            }

            return i;            
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

        public async Task<int> UpdateAllEventShortstonewDataModelV2()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("eventeuracnoi");

            var data = await query.GetObjectListAsync<EventShortLinked>();
            int i = 0;

            foreach (var eventshort in data)
            {
                if (!String.IsNullOrEmpty(eventshort.EventTextDE))
                    eventshort.EventText.TryAddOrUpdate("de", eventshort.EventTextDE);
                //Beschreibung IT
                if (!String.IsNullOrEmpty(eventshort.EventTextIT))
                    eventshort.EventText.TryAddOrUpdate("it", eventshort.EventTextIT);
                //Beschreibung EN
                if (!String.IsNullOrEmpty(eventshort.EventTextEN))
                    eventshort.EventText.TryAddOrUpdate("en", eventshort.EventTextEN);

                if (!String.IsNullOrEmpty(eventshort.EventDescriptionDE))
                    eventshort.EventTitle.TryAddOrUpdate("de", eventshort.EventDescriptionDE);
                //Beschreibung IT
                if (!String.IsNullOrEmpty(eventshort.EventDescriptionIT))
                    eventshort.EventTitle.TryAddOrUpdate("it", eventshort.EventDescriptionIT);
                //Beschreibung EN
                if (!String.IsNullOrEmpty(eventshort.EventDescriptionEN))
                    eventshort.EventTitle.TryAddOrUpdate("en", eventshort.EventDescriptionEN);


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

                var pgcrudresult = await QueryFactory.UpsertData<ArticlesLinked>(myarticle, "articles", "article.modify", "importer");

                if(pgcrudresult.created != null)
                    crudcount = crudcount + pgcrudresult.created.Value;

            }

            return crudcount;
        }        

        public async Task<int> UpdateAllWeatherHistoryWithMetainfo()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("weatherdatahistory");

            var data = await query.GetObjectListAsync<WeatherHistoryLinked>();
            int i = 0;

            foreach (var weatherhistory in data)
            {
                //Setting ID
                if (weatherhistory.Id == null)
                    weatherhistory.Id = weatherhistory.Weather["de"].Id.ToString();

                //Get MetaInfo
                weatherhistory._Meta = MetadataHelper.GetMetadataobject<WeatherHistoryLinked>(weatherhistory);

                //Setting MetaInfo
                weatherhistory._Meta.Reduced = false;
                

                //Save tp DB
                //TODO CHECK IF THIS WORKS     
                var queryresult = await QueryFactory.Query("weatherdatahistory").Where("id", weatherhistory.Id)
                    //.UpdateAsync(new JsonBData() { id = eventshort.Id.ToLower(), data = new JsonRaw(eventshort) });
                    .UpdateAsync(new JsonBData() { id = weatherhistory.Id, data = new JsonRaw(weatherhistory) });

                i++;
            }

            return i;
        }

        public async Task<int> UpdateAllODHActivityPoiOldTags(string source)
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("smgpois")
                   .Where("gen_source", source);

            var data = await query.GetObjectListAsync<ODHActivityPoiOld>();
            int i = 0;

            foreach (var stapoi in data)
            {
                if(stapoi.Tags != null)
                { 
                //CopyClassHelper.CopyPropertyValues
                var tags = stapoi.Tags;

                
                stapoi.Tags = null;

                var stapoiv2 = (ODHActivityPoiLinked)stapoi;

                stapoiv2.Tags = new List<Tags>();
                foreach(var tagdict in tags)
                {
                    stapoiv2.Tags.AddRange(tagdict.Value);
                }
                

                //Save tp DB
                //TODO CHECK IF THIS WORKS     
                var queryresult = await QueryFactory.Query("smgpois").Where("id", stapoiv2.Id)
                    .UpdateAsync(new JsonBData() { id = stapoiv2.Id?.ToLower() ?? "", data = new JsonRaw(stapoiv2) });

                i++;
                }
            }

            return i;
        }


    }

    public class ODHActivityPoiOld : ODHActivityPoiLinked
    {
        public new IDictionary<string, List<Tags>> Tags { get; set; }
    }
}
