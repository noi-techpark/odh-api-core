// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helper;
using Microsoft.FSharp.Control;
using System.Diagnostics;
using Helper.Generic;
using Helper.Tagging;
using Helper.JsonHelpers;
using Newtonsoft.Json;
using SqlKata;
using System.Threading;
using System.Reflection;

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

        #region MetaData

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
                if (!String.IsNullOrEmpty(metadata.OdhType))
                {
                    metadata.RecordCount = await MetaDataApiRecordCount.GetRecordCountfromDB(metadata.ApiFilter, metadata.OdhType, QueryFactory);

                    //Save tp DB                 
                    var queryresult = await QueryFactory.Query("metadata").Where("id", metadata.Id)
                        .UpdateAsync(new JsonBData() { id = metadata.Id?.ToLower() ?? "", data = new JsonRaw(metadata) });

                    i++;
                }
            }

            return i;            
        }

        public async Task<int> ResaveMetaData(string host, bool correcturls)
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("metadata");

            var data = await query.GetObjectListAsync<TourismMetaData>();
            int i = 0;

            foreach (var metadata in data)
            {
                //fix swaggerurl mess
                //var swaggerurl = "swagger/index.html#" + metadata.SwaggerUrl.Split("#").LastOrDefault();

                //metadata.SwaggerUrl = swaggerurl;

                //modify domain

                //if (correcturls && !host.StartsWith("importer.tourism") && metadata.BaseUrl.StartsWith("https://api.tourism.testingmachine.eu"))
                //{
                //    metadata.BaseUrl = "https://tourism.api.opendatahub.com";
                //    if(!String.IsNullOrEmpty(metadata.SwaggerUrl))
                //        metadata.SwaggerUrl = metadata.SwaggerUrl.Replace("https://api.tourism.testingmachine.eu", "https://tourism.api.opendatahub.com");                    
                //}

                //if (correcturls && !host.StartsWith("importer.tourism") && metadata.ImageGallery != null && metadata.ImageGallery.Count() > 0)
                //{
                //    foreach (var image in metadata.ImageGallery)
                //    {
                //        if (image.ImageUrl.StartsWith("https://images.tourism.testingmachine.eu"))
                //        {
                //            image.ImageUrl = image.ImageUrl.Replace("https://images.tourism.testingmachine.eu", "https://images.opendatahub.com");
                //        }
                //    }
                //}


                //metadata.Type = metadata.OdhType;
                //metadata.LicenseInfo = new LicenseInfo() { Author = "https://noi.bz.it", ClosedData = false, License = "CC0", LicenseHolder = "https://noi.bz.it" };

                //Adding ApiType
                if (metadata.ApiUrl.Contains("tourism"))
                    metadata.ApiType = "content";
                else if (metadata.ApiUrl.Contains("mobility"))
                    metadata.ApiType = "timeseries";

                //Save tp DB                 
                var queryresult = await QueryFactory.Query("metadata").Where("id", metadata.Id)
                .UpdateAsync(new JsonBData() { id = metadata.Id?.ToLower() ?? "", data = new JsonRaw(metadata) });

                i++;
            }

            return i;
        }

        #endregion

        #region Generic

        public async Task<int> ResaveSourcesOnType<T>(string odhtype, string sourcetofilter, string sourcetochange) where T : notnull
        {
            string table = ODHTypeHelper.TranslateTypeString2Table(odhtype);
            var mytype = ODHTypeHelper.TranslateTypeString2Type(odhtype);


            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From(table)
                   .When(sourcetofilter != "null", x => x.WhereJsonb("Source", sourcetofilter))
                   .When(sourcetofilter == "null", x => x.WhereRaw("data->>'Source' is null"));

            var data = await query.GetObjectListAsync<T>();

            int i = 0;

            foreach (var tag in data)
            {

                if (tag is IIdentifiable)
                {
                    if (tag is ISource)
                        ((ISource)tag).Source = sourcetochange;

                    //Save to DB                 
                    var queryresult = await QueryFactory.Query(table).Where("id", ((IIdentifiable)tag).Id)
                        .UpdateAsync(new JsonBData() { id = ((IIdentifiable)tag).Id ?? "", data = new JsonRaw(tag) });

                    i = i + queryresult;
                }
            }

            return i;
        }

        #endregion

        #region Articles

        public async Task<int> NewsFeedUpdate()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("articles")
                   .WhereRaw("gen_articletype @> ARRAY['newsfeednoi']");

            var articles = await query.GetObjectListAsync<ArticlesLinked>();
            int i = 0;

            foreach (var article in articles)
            {
                //if (article.Active == null)
                //{
                //    article.Active = false;

                //    //Save tp DB
                //    var queryresult = await QueryFactory.Query("articles").Where("id", article.Id)
                //         .UpdateAsync(new JsonBData() { id = article.Id, data = new JsonRaw(article) });

                //    i++;
                //}
            }

            return i;
        }

        public async Task<int> FillDBWithDummyNews()
        {
            int crudcount = 0;

            for (int i = 1; i <= 120; i++)
            {
                ArticlesLinked myarticle = new ArticlesLinked();
                myarticle.Id = Guid.NewGuid().ToString().ToUpper();
                myarticle.Type = "newsfeednoi";
                myarticle.Active = true;
                myarticle.Detail.TryAddOrUpdate("de", new Detail() { Title = "TesttitleDE" + i, BaseText = "testtextDE " + i, Language = "de", AdditionalText = "additionaltextde" + i });
                myarticle.Detail.TryAddOrUpdate("it", new Detail() { Title = "TesttitleIT" + i, BaseText = "testtextIT " + i, Language = "it", AdditionalText = "additionaltextit" + i });
                myarticle.Detail.TryAddOrUpdate("en", new Detail() { Title = "TesttitleEN" + i, BaseText = "testtextEN " + i, Language = "en", AdditionalText = "additionaltexten" + i });

                myarticle.HasLanguage = new List<string>() { "de", "it", "en" };

                myarticle.LicenseInfo = new LicenseInfo() { Author = "", License = "CC0", ClosedData = false, LicenseHolder = "https://noi.bz.it" };

                myarticle.ContactInfos.TryAddOrUpdate("de", new ContactInfos() { Email = "community@noi.bz.it", LogoUrl = "https://databrowser.opendatahub.com/icons/NOI.png", Language = "de", CompanyName = "NOI Techpark" });
                myarticle.ContactInfos.TryAddOrUpdate("it", new ContactInfos() { Email = "community@noi.bz.it", LogoUrl = "https://databrowser.opendatahub.com/icons/NOI.png", Language = "it", CompanyName = "NOI Techpark" });
                myarticle.ContactInfos.TryAddOrUpdate("en", new ContactInfos() { Email = "community@noi.bz.it", LogoUrl = "https://databrowser.opendatahub.com/icons/NOI.png", Language = "en", CompanyName = "NOI Techpark" });

                myarticle.ArticleDate = DateTime.Now.Date.AddDays(i);

                if (i % 5 == 0)
                {
                    myarticle.ArticleDateTo = DateTime.Now.Date.AddMonths(i);
                }
                else
                    myarticle.ArticleDateTo = DateTime.MaxValue;

                myarticle.SmgActive = true;
                myarticle.Source = "noi";

                if (i % 3 == 0)
                {
                    myarticle.SmgTags = new List<string>() { "important" };
                }

                var pgcrudresult = await QueryFactory.UpsertData<ArticlesLinked>(myarticle, new DataInfo("articles", CRUDOperation.Update) { ErrorWhendataIsNew = false }, new EditInfo("article.modify", "importer"), new CRUDConstraints(), new CompareConfig(false, false));

                if (pgcrudresult.created != null)
                    crudcount = crudcount + pgcrudresult.created.Value;

            }

            return crudcount;
        }

        #endregion

        #region Weather

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

        #endregion

        #region Accommodation

        public async Task<int> AccommodationRoomModify()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("accommodationrooms");

            var accorooms = await query.GetObjectListAsync<AccommodationRoomLinked>();
            int i = 0;

            foreach (var accoroom in accorooms)
            {
                if (accoroom.PublishedOn != null && accoroom.PublishedOn.Count == 2 && accoroom.PublishedOn.FirstOrDefault() == "idm-marketplace")
                {
                    accoroom.PublishedOn = new List<string>()
                    {
                        "suedtirol.info",
                        "idm-marketplace"
                    };

                    //Save tp DB
                    var queryresult = await QueryFactory.Query("accommodationrooms").Where("id", accoroom.Id)
                         .UpdateAsync(new JsonBData() { id = accoroom.Id, data = new JsonRaw(accoroom) });

                    i++;
                }
            }

            return i;
        }

        public async Task<int> AccommodationModify(List<string> idlist, bool trim)
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("accommodations")
                   .WhereIn("id", idlist);

            var accos = await query.GetObjectListAsync<AccommodationLinked>();
            int i = 0;

            foreach (var acco in accos)
            {
                if (trim)
                {
                    acco.AccoDetail["de"].Name = acco.AccoDetail["de"].Name.Trim();
                }
                else
                {
                    acco.AccoDetail["de"].Name = acco.AccoDetail["de"].Name + " ";
                }

                //Save tp DB
                var queryresult = await QueryFactory.Query("accommodations").Where("id", acco.Id)
                     .UpdateAsync(new JsonBData() { id = acco.Id, data = new JsonRaw(acco) });

                i++;
            }

            return i;
        }

        #endregion

        #region ODHActivityPoi

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
                if (stapoi.Tags != null)
                {
                    //CopyClassHelper.CopyPropertyValues
                    var tags = stapoi.Tags;


                    stapoi.Tags = null;

                    var stapoiv2 = (ODHActivityPoiLinked)stapoi;

                    stapoiv2.Tags = new List<Tags>();
                    foreach (var tagdict in tags)
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

        #endregion

        #region EventShort

        public async Task<int> CleanEventShortstEventDocumentField()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("eventeuracnoi");

            var data = await query.GetObjectListAsync<EventShortLinked>();
            int i = 0;

            foreach (var eventshort in data.Where(x => x.Documents != null))
            {
                bool resave = false;

                List<string> keystoremove = new List<string>();

                foreach (var kvp in eventshort.Documents)
                {
                    if (kvp.Value == null || kvp.Value.Count == 0)
                    {
                        keystoremove.Add(kvp.Key);
                        resave = true;

                    }

                }
                foreach (string key in keystoremove)
                {
                    eventshort.Documents.Remove(key);
                }


                if (resave)
                {
                    //Save tp DB
                    //TODO CHECK IF THIS WORKS     
                    var queryresult = await QueryFactory.Query("eventeuracnoi").Where("id", eventshort.Id)
                        //.UpdateAsync(new JsonBData() { id = eventshort.Id.ToLower(), data = new JsonRaw(eventshort) });
                        .UpdateAsync(new JsonBData() { id = eventshort.Id?.ToLower() ?? "", data = new JsonRaw(eventshort) });

                    i++;
                }

            }

            return i;
        }

        public async Task<int> UpdateAllEventShortBrokenLinks()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("eventeuracnoi");

            var data = await query.GetObjectListAsync<EventShortLinked>();
            int i = 0;

            foreach (var eventshort in data)
            {
                bool resave = false;

                if(eventshort.ImageGallery != null && eventshort.ImageGallery.Count > 0)
                {
                    //ImageGallery link
                    foreach(var image in eventshort.ImageGallery)
                    {
                        //https://tourism.opendatahub.com/imageresizer/ImageHandler.ashx?src=images/eventshort/
                        
                        if (image.ImageUrl.Contains("imageresizer/ImageHandler.ashx?src=images/eventshort/"))
                        {
                            if(image.ImageUrl.StartsWith("https"))
                                image.ImageUrl = image.ImageUrl.Replace("https://tourism.opendatahub.com/imageresizer/ImageHandler.ashx?src=images/eventshort/eventshort/", "https://tourism.images.opendatahub.com/api/Image/GetImage?imageurl=");
                            else
                                image.ImageUrl = image.ImageUrl.Replace("http://tourism.opendatahub.com/imageresizer/ImageHandler.ashx?src=images/eventshort/eventshort/", "https://tourism.images.opendatahub.com/api/Image/GetImage?imageurl=");
                            
                            resave = true;
                        }                        
                    }

                }

                if (eventshort.EventDocument != null && eventshort.EventDocument.Count > 0)
                {
                    //EventDocument link
                    foreach (var doc in eventshort.EventDocument)
                    {
                        if (doc.DocumentURL.Contains("imageresizer/images/eventshort/pdf/"))
                        {
                            if (doc.DocumentURL.StartsWith("https"))
                                doc.DocumentURL = doc.DocumentURL.Replace("https://tourism.opendatahub.com/imageresizer/images/eventshort/pdf/", "https://tourism.images.opendatahub.com/api/File/GetFile/");
                            else
                                doc.DocumentURL = doc.DocumentURL.Replace("http://tourism.opendatahub.com/imageresizer/images/eventshort/pdf/", "https://tourism.images.opendatahub.com/api/File/GetFile/");
                            
                            resave = true;
                        }
                    }
                }

                if (resave)
                {
                    //Save tp DB
                    var queryresult = await QueryFactory.Query("eventeuracnoi").Where("id", eventshort.Id)                        
                        .UpdateAsync(new JsonBData() { id = eventshort.Id?.ToLower() ?? "", data = new JsonRaw(eventshort) });

                    i++;
                }
            }

            return i;
        }

        public async Task<int> UpdateAllEventShortPublisherInfo()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("eventeuracnoi");

            var data = await query.GetObjectListAsync<EventShortLinked>();
            int i = 0;

            foreach (var eventshort in data)
            {

                PublishedOnHelper.CreatePublishedOnList<EventShortLinked>(eventshort);

                //Save tp DB
                //TODO CHECK IF THIS WORKS     
                var queryresult = await QueryFactory.Query("eventeuracnoi").Where("id", eventshort.Id)
                    //.UpdateAsync(new JsonBData() { id = eventshort.Id.ToLower(), data = new JsonRaw(eventshort) });
                    .UpdateAsync(new JsonBData() { id = eventshort.Id?.ToLower() ?? "", data = new JsonRaw(eventshort) });

                i++;
            }

            return i;
        }

        public async Task<int> UpdateAllEventShortstEventDocumentField()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("eventeuracnoi");

            var data = await query.GetObjectListAsync<EventShortLinked>();
            int i = 0;

            foreach (var eventshort in data)
            {
                var save = false;

                if (eventshort.EventDocument != null && eventshort.EventDocument.Count > 0)
                {
                    var eventshortdocsde = eventshort.EventDocument.Where(x => x.Language == "de").Select(x => new Document { Language = x.Language, DocumentName = "", DocumentURL = x.DocumentURL }).ToList();
                    if (eventshortdocsde != null && eventshortdocsde.Count > 0)
                    {
                        save = true;
                        eventshort.Documents.TryAddOrUpdate("de", eventshortdocsde);
                    }


                    var eventshortdocsit = eventshort.EventDocument.Where(x => x.Language == "it").Select(x => new Document { Language = x.Language, DocumentName = "", DocumentURL = x.DocumentURL }).ToList();
                    if (eventshortdocsit != null && eventshortdocsit.Count > 0)
                    {
                        save = true;
                        eventshort.Documents.TryAddOrUpdate("it", eventshortdocsit);
                    }


                    var eventshortdocsen = eventshort.EventDocument.Where(x => x.Language == "en").Select(x => new Document { Language = x.Language, DocumentName = "", DocumentURL = x.DocumentURL }).ToList();
                    if (eventshortdocsen != null && eventshortdocsen.Count > 0)
                    {
                        save = true;
                        eventshort.Documents.TryAddOrUpdate("en", eventshortdocsen);
                    }

                }

                if (save)
                {
                    //Save tp DB
                    //TODO CHECK IF THIS WORKS     
                    var queryresult = await QueryFactory.Query("eventeuracnoi").Where("id", eventshort.Id)
                        //.UpdateAsync(new JsonBData() { id = eventshort.Id.ToLower(), data = new JsonRaw(eventshort) });
                        .UpdateAsync(new JsonBData() { id = eventshort.Id?.ToLower() ?? "", data = new JsonRaw(eventshort) });

                    i++;
                }

            }

            return i;
        }

        public async Task<int> UpdateAllEventShortstActiveTodayField()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("eventeuracnoi");

            var data = await query.GetObjectListAsync<EventShortLinked>();
            int i = 0;

            //foreach (var eventshort in data)
            //{
            //    if (eventshort.Display1 == "Y")
            //        eventshort.ActiveToday = true;
            //    if (eventshort.Display1 == "N")
            //        eventshort.ActiveToday = false;

            //    //Save tp DB
            //    //TODO CHECK IF THIS WORKS     
            //    var queryresult = await QueryFactory.Query("eventeuracnoi").Where("id", eventshort.Id)
            //        //.UpdateAsync(new JsonBData() { id = eventshort.Id.ToLower(), data = new JsonRaw(eventshort) });
            //        .UpdateAsync(new JsonBData() { id = eventshort.Id?.ToLower() ?? "", data = new JsonRaw(eventshort) });

            //    i++;
            //}

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

        public async Task<int> UpdateAllEventShortstActiveFieldToTrue()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("eventeuracnoi");

            var data = await query.GetObjectListAsync<EventShortLinked>();
            int i = 0;

            foreach (var eventshort in data)
            {
                eventshort.Active = true;

                //Save tp DB
                //TODO CHECK IF THIS WORKS     
                var queryresult = await QueryFactory.Query("eventeuracnoi").Where("id", eventshort.Id)
                    //.UpdateAsync(new JsonBData() { id = eventshort.Id.ToLower(), data = new JsonRaw(eventshort) });
                    .UpdateAsync(new JsonBData() { id = eventshort.Id?.ToLower() ?? "", data = new JsonRaw(eventshort) });

                i++;
            }

            return i;
        }

        public async Task<int> UpdateAllEventShortstHasLanguage()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("eventeuracnoi");

            var data = await query.GetObjectListAsync<EventShortLinked>();
            int i = 0;

            foreach (var eventshort in data)
            {
                eventshort.CheckMyInsertedLanguages();

                //Save tp DB
                //TODO CHECK IF THIS WORKS     
                var queryresult = await QueryFactory.Query("eventeuracnoi").Where("id", eventshort.Id)
                    //.UpdateAsync(new JsonBData() { id = eventshort.Id.ToLower(), data = new JsonRaw(eventshort) });
                    .UpdateAsync(new JsonBData() { id = eventshort.Id?.ToLower() ?? "", data = new JsonRaw(eventshort) });

                i++;
            }

            return i;
        }

        public async Task<int> FillEventShortTags()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("eventeuracnoi");

            var data = await query.GetObjectListAsync<EventShortLinked>();
            int i = 0;


            //Load all eventshortdata from PG
            var queryeventshorttypes = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("eventshorttypes");

            var eventshorttypes = await queryeventshorttypes.GetObjectListAsync<SmgPoiTypes>();




            foreach (var eventshort in data)
            {
                if((eventshort.CustomTagging != null  && eventshort.CustomTagging.Count > 0) || (eventshort.TechnologyFields != null && eventshort.TechnologyFields.Count > 0))
                {
                    if (eventshort.TagIds == null)
                        eventshort.TagIds = new List<string>();


                    //TODO TRANSFORM KEYS used in CustomTagging and TechnologyFields to IDs!


                    //Add CustomTagging + Technologyfields to Tags
                    foreach (var tag in eventshort.CustomTagging ?? new List<string>())
                    {
                        if(!String.IsNullOrEmpty(tag))
                        {
                            if(!eventshort.TagIds.Contains(tag.ToLower()))
                            {
                                //Search by KEy
                                var toadd = eventshorttypes.Where(x => x.Key == tag).FirstOrDefault();
                                if(toadd != null)
                                    eventshort.TagIds.Add(toadd.Id);
                            }
                                
                        }                        
                    }
                    foreach (var technofields in eventshort.TechnologyFields ?? new List<string>())
                    {
                        if (!String.IsNullOrEmpty(technofields))
                        {
                            if (!eventshort.TagIds.Contains(technofields.ToLower()))
                            {
                                //Search by KEy
                                var toadd = eventshorttypes.Where(x => x.Key == technofields).FirstOrDefault();
                                if (toadd != null)
                                    eventshort.TagIds.Add(toadd.Id);                                
                            }
                        }
                    }

                    //Populate Tags (Id/Source/Type)
                    await eventshort.UpdateTagsExtension(QueryFactory);

                    //Save tp DB
                    var queryresult = await QueryFactory.Query("eventeuracnoi").Where("id", eventshort.Id)
                        //.UpdateAsync(new JsonBData() { id = eventshort.Id.ToLower(), data = new JsonRaw(eventshort) });
                        .UpdateAsync(new JsonBData() { id = eventshort.Id?.ToLower() ?? "", data = new JsonRaw(eventshort) });

                    i++;
                }
                
            }

            return i;
        }

        public async Task<int> ResaveEventShortWithTags()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("eventeuracnoi");

            var data = await query.GetObjectListAsync<EventShortLinked>();
            int i = 0;

            foreach (var eventshort in data)
            {                
                if (eventshort.TagIds != null && eventshort.TagIds.Count != eventshort.TagIds.Distinct().Count())
                {
                    eventshort.TagIds = eventshort.TagIds.Distinct().ToList();
                }

                //Save tp DB
                var queryresult = await QueryFactory.Query("eventeuracnoi").Where("id", eventshort.Id)
                    //.UpdateAsync(new JsonBData() { id = eventshort.Id.ToLower(), data = new JsonRaw(eventshort) });
                    .UpdateAsync(new JsonBData() { id = eventshort.Id?.ToLower() ?? "", data = new JsonRaw(eventshort) });

                i++;
            }

            return i;
        }


        #endregion

        #region Wine

        public async Task<int> UpdateAllWineHasLanguage()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("wines");

            var data = await query.GetObjectListAsync<WineLinked>();
            int i = 0;

            foreach (var wine in data)
            {
                wine.CheckMyInsertedLanguages(new List<string>() { "de", "it", "en" });

                //Save tp DB
                //TODO CHECK IF THIS WORKS     
                var queryresult = await QueryFactory.Query("wines").Where("id", wine.Id)
                    .UpdateAsync(new JsonBData() { id = wine.Id?.ToLower() ?? "", data = new JsonRaw(wine) });

                i++;
            }

            return i;
        }

        #endregion

        #region ODHTags

        public async Task<int> UpdateAllODHTags()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("smgtags");

            var data = await query.GetObjectListAsync<ODHTagLinked>();
            int i = 0;

            foreach (var odhtag in data)
            {
                //Setting LicenseInfo
                //Adding LicenseInfo to ODHTag (not present on sinfo instance)                    
                odhtag.LicenseInfo = Helper.LicenseHelper.GetLicenseInfoobject<ODHTagLinked>(odhtag, Helper.LicenseHelper.GetLicenseforODHTag);

                //Save tp DB
                //TODO CHECK IF THIS WORKS     
                var queryresult = await QueryFactory.Query("smgtags").Where("id", odhtag.Id)
                    .UpdateAsync(new JsonBData() { id = odhtag.Id?.ToLower() ?? "", data = new JsonRaw(odhtag) });

                i++;
            }

            return i;
        }

        #endregion

        #region Tags

        public async Task<int> ResaveTags()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("tags");

            var data = await query.GetObjectListAsync<TagLinked>();
            int i = 0;

            foreach (var tag in data)
            {
                tag._Meta.Type = "tag";

                //Save to DB                 
                var queryresult = await QueryFactory.Query("tags").Where("id", tag.Id)
                    .UpdateAsync(new JsonBData() { id = tag.Id?.ToLower() ?? "", data = new JsonRaw(tag) });

                i++;
            }

            return i;
        }



      
        public async Task<int> TagSourceFix()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("tags");

            var data = await query.GetObjectListAsync<TagLinked>();
            int i = 0;

            foreach (var tag in data)
            {                
                tag.Source = tag.Types.Contains("ltscategory") ? "lts" : "idm";

                //Save to DB                 
                var queryresult = await QueryFactory.Query("tags").Where("id", tag.Id)
                    .UpdateAsync(new JsonBData() { id = tag.Id?.ToLower() ?? "", data = new JsonRaw(tag) });

                i++;
            }

            return i;
        }

        public async Task<int> TagTypesFix()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("tags");

            var data = await query.GetObjectListAsync<TagLinked>();
            int i = 0;

            foreach (var tag in data)
            {
                if(tag.Types != null && tag.Types.Count > 0)
                {
                    tag.Types = tag.Types.Select(x => x.ToLower()).ToList();

                    //Save to DB                 
                    var queryresult = await QueryFactory.Query("tags").Where("id", tag.Id)
                        .UpdateAsync(new JsonBData() { id = tag.Id?.ToLower() ?? "", data = new JsonRaw(tag) });

                    i++;
                }

            }

            return i;
        }


        public async Task<int> EventTopicsToTags()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("eventtypes");

            var data = await query.GetObjectListAsync<EventTypes>();
            int i = 0;

            foreach (var topic in data)
            {
                TagLinked tag = new TagLinked();

                tag.Id = topic.Id;
                tag.Source = "lts";
                tag.TagName = topic.TypeDesc;
                tag._Meta = new Metadata() { Id = tag.Id, LastUpdate = DateTime.Now, Reduced = false, Source = "lts", Type = "tag", UpdateInfo = new UpdateInfo() { UpdatedBy = "import", UpdateSource = "importer" } };
                tag.DisplayAsCategory = false;
                tag.ValidForEntity = new List<string>() { "event" };
                tag.MainEntity = "event";
                tag.LastChange = DateTime.Now;
                tag.LicenseInfo = new LicenseInfo() { Author = "https://lts.it", ClosedData = false, License = "CC0", LicenseHolder = "https://lts.it" };
                tag.Shortname = tag.TagName.ContainsKey("en") ? tag.TagName["en"] : tag.TagName.FirstOrDefault().Value;
                tag.FirstImport = DateTime.Now;
                tag.PublishedOn = null;
                tag.Types = new List<string>() { "eventtopic" };

                tag.PublishDataWithTagOn = null;
                tag.Mapping = null;
                tag.IDMCategoryMapping = null;
                tag.LTSTaggingInfo = null;
                tag.MappedTagIds = null;

                var pgcrudresult = await QueryFactory.UpsertData<TagLinked>(tag, new DataInfo("tags", CRUDOperation.Update) { ErrorWhendataIsNew = false }, new EditInfo("tag.modify", "importer"), new CRUDConstraints(), new CompareConfig(false, false));

                i++;
            }

            return i;
        }

        public async Task<int> EventShortTypesToTags()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("eventshorttypes");

            var data = await query.GetObjectListAsync<SmgPoiTypes>();
            int i = 0;

            foreach (var topic in data)
            {
                TagLinked tag = new TagLinked();

                tag.Id = topic.Id;
                tag.Source = "noi";
                tag.TagName = topic.TypeDesc;  
                tag._Meta = new Metadata() { Id = tag.Id, LastUpdate = DateTime.Now, Reduced = false, Source = "lts", Type = "tag", UpdateInfo = new UpdateInfo() { UpdatedBy = "import", UpdateSource = "importer" } };
                tag.DisplayAsCategory = false;
                tag.ValidForEntity = new List<string>() { "event", "eventshort" };
                tag.MainEntity = "event";
                tag.LastChange = DateTime.Now;
                tag.LicenseInfo = new LicenseInfo() { Author = "https://lts.it", ClosedData = false, License = "CC0", LicenseHolder = "https://lts.it" };
                tag.Shortname = tag.TagName.ContainsKey("en") ? tag.TagName["en"] : tag.TagName.FirstOrDefault().Value;
                tag.FirstImport = DateTime.Now;
                tag.PublishedOn = null;
                tag.Types = new List<string>() { topic.Type.ToLower() };

                tag.PublishDataWithTagOn = null;
                tag.Mapping = null;
                tag.IDMCategoryMapping = null;
                tag.LTSTaggingInfo = null;
                tag.MappedTagIds = null;

                var pgcrudresult = await QueryFactory.UpsertData<TagLinked>(tag, new DataInfo("tags", CRUDOperation.Update) { ErrorWhendataIsNew = false }, new EditInfo("tag.modify", "importer"), new CRUDConstraints(), new CompareConfig(false, false));

                i++;
            }

            return i;
        }

        public async Task<int> GastronomyTypesToTags()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("gastronomytypes");

            var data = await query.GetObjectListAsync<GastronomyTypes>();
            int i = 0;

            foreach (var topic in data)
            {
                TagLinked tag = new TagLinked();

                tag.Id = topic.Id;
                tag.Source = "lts";
                tag.TagName = topic.TypeDesc;
                tag._Meta = new Metadata() { Id = tag.Id, LastUpdate = DateTime.Now, Reduced = false, Source = "lts", Type = "tag", UpdateInfo = new UpdateInfo() { UpdatedBy = "import", UpdateSource = "importer" } };
                tag.DisplayAsCategory = false;
                tag.ValidForEntity = new List<string>() { "odhactivitypoi" };
                tag.MainEntity = "odhactivitypoi";
                tag.LastChange = DateTime.Now;
                tag.LicenseInfo = new LicenseInfo() { Author = "https://lts.it", ClosedData = false, License = "CC0", LicenseHolder = "https://lts.it" };
                tag.Shortname = tag.TagName.ContainsKey("en") ? tag.TagName["en"] : tag.TagName.FirstOrDefault().Value;
                tag.FirstImport = DateTime.Now;
                tag.PublishedOn = null;
                tag.Types = new List<string>() { topic.Type.ToLower() };

                tag.PublishDataWithTagOn = null;
                tag.Mapping = null;
                tag.IDMCategoryMapping = null;
                tag.LTSTaggingInfo = null;
                tag.MappedTagIds = null;

                var pgcrudresult = await QueryFactory.UpsertData<TagLinked>(tag, new DataInfo("tags", CRUDOperation.Update) { ErrorWhendataIsNew = false }, new EditInfo("tag.modify", "importer"), new CRUDConstraints(), new CompareConfig(false, false));

                i++;
            }

            return i;
        }

        public async Task<int> VenueTypesToTags()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("venuetypes");

            var data = await query.GetObjectListAsync<DDVenueCodes>();
            int i = 0;

            foreach (var topic in data)
            {
                TagLinked tag = new TagLinked();

                tag.Id = topic.Id;
                tag.Source = "lts";
                tag.TagName = topic.TypeDesc;
                tag._Meta = new Metadata() { Id = tag.Id, LastUpdate = DateTime.Now, Reduced = false, Source = "lts", Type = "tag", UpdateInfo = new UpdateInfo() { UpdatedBy = "import", UpdateSource = "importer" } };
                tag.DisplayAsCategory = false;
                tag.ValidForEntity = new List<string>() { "venue" };
                tag.MainEntity = "venue";
                tag.LastChange = DateTime.Now;
                tag.LicenseInfo = new LicenseInfo() { Author = "https://lts.it", ClosedData = false, License = "CC0", LicenseHolder = "https://lts.it" };
                tag.Shortname = tag.TagName.ContainsKey("en") ? tag.TagName["en"] : tag.TagName.FirstOrDefault().Value;
                tag.FirstImport = DateTime.Now;
                tag.PublishedOn = null;
                tag.Types = new List<string>() { topic.Type.ToLower() };

                tag.PublishDataWithTagOn = null;
                tag.Mapping = null;
                tag.IDMCategoryMapping = null;
                tag.LTSTaggingInfo = null;
                tag.MappedTagIds = null;

                var pgcrudresult = await QueryFactory.UpsertData<TagLinked>(tag, new DataInfo("tags", CRUDOperation.Update) { ErrorWhendataIsNew = false }, new EditInfo("tag.modify", "importer"), new CRUDConstraints(), new CompareConfig(false, false));

                i++;
            }

            return i;
        }

        public async Task<int> ArticleTypesToTags()
        {
            //Load all data from PG and resave
            var query = QueryFactory.Query()
                   .SelectRaw("data")
                   .From("articletypes");

            var data = await query.GetObjectListAsync<ArticleTypes>();
            int i = 0;

            foreach (var topic in data)
            {
                TagLinked tag = new TagLinked();

                tag.Id = topic.Id;
                tag.Source = "idm";  //TO CHECK
                tag.TagName = topic.TypeDesc;  //TO CHECK
                tag._Meta = new Metadata() { Id = tag.Id, LastUpdate = DateTime.Now, Reduced = false, Source = "lts", Type = "tag", UpdateInfo = new UpdateInfo() { UpdatedBy = "import", UpdateSource = "importer" } };
                tag.DisplayAsCategory = false;
                tag.ValidForEntity = new List<string>() { "article" };
                tag.MainEntity = "article";
                tag.LastChange = DateTime.Now;
                tag.LicenseInfo = new LicenseInfo() { Author = "https://lts.it", ClosedData = false, License = "CC0", LicenseHolder = "https://lts.it" };
                tag.Shortname = tag.TagName.ContainsKey("en") ? tag.TagName["en"] : tag.TagName.FirstOrDefault().Value;
                tag.FirstImport = DateTime.Now;
                tag.PublishedOn = null;
                tag.Types = new List<string>() { topic.Type.ToLower() };

                tag.PublishDataWithTagOn = null;
                tag.Mapping = null;
                tag.IDMCategoryMapping = null;
                tag.LTSTaggingInfo = null;
                tag.MappedTagIds = null;

                var pgcrudresult = await QueryFactory.UpsertData<TagLinked>(tag, new DataInfo("tags", CRUDOperation.Update) { ErrorWhendataIsNew = false }, new EditInfo("tag.modify", "importer"), new CRUDConstraints(), new CompareConfig(false, false));

                i++;
            }

            return i;
        }


        #endregion

    }

    public class ODHActivityPoiOld : ODHActivityPoiLinked
    {
        public new IDictionary<string, List<Tags>> Tags { get; set; }
    }
}
