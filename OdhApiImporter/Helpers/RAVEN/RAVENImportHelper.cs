using DataModel;
using Helper;
using Microsoft.AspNetCore.Mvc;
using OdhNotifier;
using RAVEN;
using ServiceReferenceLCS;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiImporter.Helpers
{
    public class RavenImportHelper
    {
        private readonly QueryFactory QueryFactory;
        private readonly ISettings settings;
        private string importerURL;
        private IOdhPushNotifier OdhPushnotifier;


        public RavenImportHelper(ISettings settings, QueryFactory queryfactory, string importerURL, IOdhPushNotifier odhpushnotifier)
        {
            this.QueryFactory = queryfactory;
            this.settings = settings;
            this.importerURL = importerURL;
            this.OdhPushnotifier = odhpushnotifier;
        }

        #region ODHRAVEN Helpers

        //TODO Check if passed id has to be tranformed to lowercase or uppercase


        public async Task<Tuple<string, UpdateDetail>> GetFromRavenAndTransformToPGObject(string id, string datatype, CancellationToken cancellationToken)
        {
            var mydata = default(IIdentifiable);
            var mypgdata = default(IIdentifiable);

            var myupdateresult = default(UpdateDetail);
            var updateresultreduced = default(UpdateDetail);

            switch (datatype.ToLower())
            {
                case "accommodation":
                    mydata = await GetDataFromRaven.GetRavenData<AccommodationLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<AccommodationLinked, AccommodationLinked>((AccommodationLinked)mydata, TransformToPGObject.GetAccommodationPGObject);
                    else
                        throw new Exception("No data found!");

                    //Add the PublishedOn Logic
                    ((AccommodationLinked)mypgdata).CreatePublishenOnList();

                    myupdateresult = await SaveRavenObjectToPG<AccommodationLinked>((AccommodationLinked)mypgdata, "accommodations", true, true);

                    //Check if data has to be reduced and save it
                    if (ReduceDataTransformer.ReduceDataCheck<AccommodationLinked>((AccommodationLinked)mypgdata) == true)
                    {
                        var reducedobject = ReduceDataTransformer.GetReducedObject((AccommodationLinked)mypgdata, ReduceDataTransformer.CopyLTSAccommodationToReducedObject);

                        updateresultreduced = await SaveRavenObjectToPG<AccommodationLinked>((AccommodationLinkedReduced)reducedobject, "accommodations");
                    }

                    //UPDATE ACCOMMODATIONROOMS
                    var myroomdatalist = await GetDataFromRaven.GetRavenData<IEnumerable<AccommodationRoomLinked>>("accommodationroom", id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken, "AccommodationRoom?accoid=");

                    if (myroomdatalist != null)
                    {
                        Tuple<string, bool>? roomsourcecheck = null;
                        if (((AccommodationLinked)mypgdata).AccoRoomInfo.Select(x => x.Source).Distinct().Count() > 1)
                            roomsourcecheck = Tuple.Create("hgv", true);

                        foreach (var myroomdata in myroomdatalist)
                        {
                            var mypgroomdata = TransformToPGObject.GetPGObject<AccommodationRoomLinked, AccommodationRoomLinked>((AccommodationRoomLinked)myroomdata, TransformToPGObject.GetAccommodationRoomPGObject);
                            
                            //Add the PublishedOn Logic
                            ((AccommodationRoomLinked)mypgroomdata).CreatePublishenOnList(null, roomsourcecheck);

                            var accoroomresult = await SaveRavenObjectToPG<AccommodationRoomLinked>((AccommodationRoomLinked)mypgroomdata, "accommodationrooms", true, true);

                            //TO DO, delete all delted rooms


                            //Merge with updateresult
                            myupdateresult = GenericResultsHelper.MergeUpdateDetail(new List<UpdateDetail> { myupdateresult, accoroomresult });
                        }
                    }
                    else
                        throw new Exception("No data found!");

                    //Check if the Object has Changed and Push all infos to the channels
                    await CheckIfObjectChangedAndPush(myupdateresult, mypgdata.Id, datatype);


                    break;

                case "gastronomy":
                    mydata = await GetDataFromRaven.GetRavenData<GastronomyLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<GastronomyLinked, GastronomyLinked>((GastronomyLinked)mydata, TransformToPGObject.GetGastronomyPGObject);
                    else
                        throw new Exception("No data found!");

                    myupdateresult = await SaveRavenObjectToPG<GastronomyLinked>((GastronomyLinked)mypgdata, "gastronomies");

                    //No need for Publishedon, neither comparing data since this data is from a deprecated endpoint

                    //Check if data has to be reduced and save it
                    if (ReduceDataTransformer.ReduceDataCheck<GastronomyLinked>((GastronomyLinked)mypgdata) == true)
                    {
                        var reducedobject = ReduceDataTransformer.GetReducedObject((GastronomyLinked)mypgdata, ReduceDataTransformer.CopyLTSGastronomyToReducedObject);

                        updateresultreduced = await SaveRavenObjectToPG<GastronomyLinked>((GastronomyLinkedReduced)reducedobject, "gastronomies");
                    }

                    break;

                case "activity":
                    mydata = await GetDataFromRaven.GetRavenData<LTSActivityLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<LTSActivityLinked, LTSActivityLinked>((LTSActivityLinked)mydata, TransformToPGObject.GetActivityPGObject);
                    else
                        throw new Exception("No data found!");

                    myupdateresult = await SaveRavenObjectToPG<LTSActivityLinked>((LTSActivityLinked)mypgdata, "activities");

                    //No need for Publishedon, neither comparing data since this data is from a deprecated endpoint

                    //Check if data has to be reduced and save it
                    if (ReduceDataTransformer.ReduceDataCheck<LTSActivityLinked>((LTSActivityLinked)mypgdata) == true)
                    {
                        var reducedobject = ReduceDataTransformer.GetReducedObject((LTSActivityLinked)mypgdata, ReduceDataTransformer.CopyLTSActivityToReducedObject);

                        updateresultreduced = await SaveRavenObjectToPG<LTSActivityLinked>((LTSActivityLinkedReduced)reducedobject, "activities");
                    }

                    break;

                case "poi":
                    mydata = await GetDataFromRaven.GetRavenData<LTSPoiLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<LTSPoiLinked, LTSPoiLinked>((LTSPoiLinked)mydata, TransformToPGObject.GetPoiPGObject);
                    else
                        throw new Exception("No data found!");

                    myupdateresult = await SaveRavenObjectToPG<LTSPoiLinked>((LTSPoiLinked)mypgdata, "pois");

                    //No need for Publishedon, neither comparing data since this data is from a deprecated endpoint

                    //Check if data has to be reduced and save it
                    if (ReduceDataTransformer.ReduceDataCheck<LTSPoiLinked>((LTSPoiLinked)mypgdata) == true)
                    {
                        var reducedobject = ReduceDataTransformer.GetReducedObject((LTSPoiLinked)mypgdata, ReduceDataTransformer.CopyLTSPoiToReducedObject);

                        updateresultreduced = await SaveRavenObjectToPG<LTSPoiLinked>((LTSPoiLinkedReduced)reducedobject, "pois");
                    }

                    break;

                case "odhactivitypoi":
                    mydata = await GetDataFromRaven.GetRavenData<ODHActivityPoiLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<ODHActivityPoiLinked, ODHActivityPoiLinked>((ODHActivityPoiLinked)mydata, TransformToPGObject.GetODHActivityPoiPGObject);
                    else
                        throw new Exception("No data found!");
                    
                    //Special get all Taglist and traduce it on import
                    await GenericTaggingHelper.AddMappingToODHActivityPoi(mypgdata, settings.JsonConfig.Jsondir);

                    //Add the PublishedOn Logic
                    //Exception here all Tags with autopublish has to be passed
                    var autopublishtaglist = await GenericTaggingHelper.GetAllAutoPublishTagsfromJson(settings.JsonConfig.Jsondir);
                    ((ODHActivityPoiLinked)mypgdata).CreatePublishenOnList(autopublishtaglist);

                    myupdateresult = await SaveRavenObjectToPG<ODHActivityPoiLinked>((ODHActivityPoiLinked)mypgdata, "smgpois", true, true);                  

                    //Check if the Object has Changed and Push all infos to the channels
                    await CheckIfObjectChangedAndPush(myupdateresult, mypgdata.Id, datatype);

                    //Check if data has to be reduced and save it
                    if (ReduceDataTransformer.ReduceDataCheck<ODHActivityPoiLinked>((ODHActivityPoiLinked)mypgdata) == true)
                    {
                        var reducedobject = ReduceDataTransformer.GetReducedObject((ODHActivityPoiLinked)mypgdata, ReduceDataTransformer.CopyLTSODHActivtyPoiToReducedObject);

                        updateresultreduced = await SaveRavenObjectToPG<ODHActivityPoiLinked>((LTSODHActivityPoiReduced)reducedobject, "smgpois");
                    }

                    break;

                case "event":
                    mydata = await GetDataFromRaven.GetRavenData<EventLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<EventLinked, EventLinked>((EventLinked)mydata, TransformToPGObject.GetEventPGObject);
                    else
                        throw new Exception("No data found!");

                    //Add the PublishedOn Logic
                    ((EventLinked)mypgdata).CreatePublishenOnList();

                    myupdateresult = await SaveRavenObjectToPG<EventLinked>((EventLinked)mypgdata, "events", true, true);

                    //Check if the Object has Changed and Push all infos to the channels
                    await CheckIfObjectChangedAndPush(myupdateresult, mypgdata.Id, datatype);

                    //Check if data has to be reduced and save it
                    if (ReduceDataTransformer.ReduceDataCheck<EventLinked>((EventLinked)mypgdata) == true)
                    {
                        var reducedobject = ReduceDataTransformer.GetReducedObject((EventLinked)mypgdata, ReduceDataTransformer.CopyLTSEventToReducedObject);

                        updateresultreduced = await SaveRavenObjectToPG<EventLinked>((EventLinkedReduced)reducedobject, "events", true);
                    }

                    break;

                case "webcam":
                    mydata = await GetDataFromRaven.GetRavenData<WebcamInfoLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken, "WebcamInfo/");
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<WebcamInfoLinked, WebcamInfoLinked>((WebcamInfoLinked)mydata, TransformToPGObject.GetWebcamInfoPGObject);
                    else
                        throw new Exception("No data found!");

                    //Add the PublishedOn Logic
                    ((WebcamInfoLinked)mypgdata).CreatePublishenOnList();

                    myupdateresult = await SaveRavenObjectToPG<WebcamInfoLinked>((WebcamInfoLinked)mypgdata, "webcams", true);

                    //Check if the Object has Changed and Push all infos to the channels
                    await CheckIfObjectChangedAndPush(myupdateresult, mypgdata.Id, datatype);

                    //Check if data has to be reduced and save it
                    if (ReduceDataTransformer.ReduceDataCheck<WebcamInfoLinked>((WebcamInfoLinked)mypgdata) == true)
                    {
                        var reducedobject = ReduceDataTransformer.GetReducedObject((WebcamInfoLinked)mypgdata, ReduceDataTransformer.CopyLTSWebcamInfoToReducedObject);

                        updateresultreduced = await SaveRavenObjectToPG<WebcamInfoLinked>((WebcamInfoLinkedReduced)reducedobject, "webcams");
                    }

                    break;

                case "metaregion":
                    mydata = await GetDataFromRaven.GetRavenData<MetaRegionLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<MetaRegionLinked, MetaRegionLinked>((MetaRegionLinked)mydata, TransformToPGObject.GetMetaRegionPGObject);
                    else
                        throw new Exception("No data found!");

                    //Add the PublishedOn Logic
                    ((MetaRegionLinked)mypgdata).CreatePublishenOnList();

                    myupdateresult = await SaveRavenObjectToPG<MetaRegionLinked>((MetaRegionLinked)mypgdata, "metaregions", true, true);

                    //Check if the Object has Changed and Push all infos to the channels
                    await CheckIfObjectChangedAndPush(myupdateresult, mypgdata.Id, datatype);

                    break;

                case "region":
                    mydata = await GetDataFromRaven.GetRavenData<RegionLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<RegionLinked, RegionLinked>((RegionLinked)mydata, TransformToPGObject.GetRegionPGObject);
                    else
                        throw new Exception("No data found!");

                    //Add the PublishedOn Logic
                    ((RegionLinked)mypgdata).CreatePublishenOnList();

                    myupdateresult = await SaveRavenObjectToPG<RegionLinked>((RegionLinked)mypgdata, "regions", true, true);

                    //Check if the Object has Changed and Push all infos to the channels
                    await CheckIfObjectChangedAndPush(myupdateresult, mypgdata.Id, datatype);

                    break;

                case "tv":
                    mydata = await GetDataFromRaven.GetRavenData<TourismvereinLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken, "TourismAssociation/");
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<TourismvereinLinked, TourismvereinLinked>((TourismvereinLinked)mydata, TransformToPGObject.GetTourismAssociationPGObject);
                    else
                        throw new Exception("No data found!");

                    //Add the PublishedOn Logic
                    ((TourismvereinLinked)mypgdata).CreatePublishenOnList();

                    myupdateresult = await SaveRavenObjectToPG<TourismvereinLinked>((TourismvereinLinked)mypgdata, "tvs", true, true);

                    //Check if the Object has Changed and Push all infos to the channels
                    await CheckIfObjectChangedAndPush(myupdateresult, mypgdata.Id, datatype);

                    break;

                case "municipality":
                    mydata = await GetDataFromRaven.GetRavenData<MunicipalityLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<MunicipalityLinked, MunicipalityLinked>((MunicipalityLinked)mydata, TransformToPGObject.GetMunicipalityPGObject);
                    else
                        throw new Exception("No data found!");

                    //Add the PublishedOn Logic
                    ((MunicipalityLinked)mypgdata).CreatePublishenOnList();

                    myupdateresult = await SaveRavenObjectToPG<MunicipalityLinked>((MunicipalityLinked)mypgdata, "municipalities", true);

                    //Check if the Object has Changed and Push all infos to the channels
                    await CheckIfObjectChangedAndPush(myupdateresult, mypgdata.Id, datatype);

                    break;

                case "district":
                    mydata = await GetDataFromRaven.GetRavenData<DistrictLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<DistrictLinked, DistrictLinked>((DistrictLinked)mydata, TransformToPGObject.GetDistrictPGObject);
                    else
                        throw new Exception("No data found!");

                    //Add the PublishedOn Logic
                    ((DistrictLinked)mypgdata).CreatePublishenOnList();

                    myupdateresult = await SaveRavenObjectToPG<DistrictLinked>((DistrictLinked)mypgdata, "districts", true);

                    //Check if the Object has Changed and Push all infos to the channels
                    await CheckIfObjectChangedAndPush(myupdateresult, mypgdata.Id, datatype);

                    break;

                case "experiencearea":
                    mydata = await GetDataFromRaven.GetRavenData<ExperienceAreaLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<ExperienceAreaLinked, ExperienceAreaLinked>((ExperienceAreaLinked)mydata, TransformToPGObject.GetExperienceAreaPGObject);
                    else
                        throw new Exception("No data found!");

                    //Add the PublishedOn Logic
                    ((ExperienceAreaLinked)mypgdata).CreatePublishenOnList();

                    myupdateresult = await SaveRavenObjectToPG<ExperienceAreaLinked>((ExperienceAreaLinked)mypgdata, "experienceareas", true);

                    //Check if the Object has Changed and Push all infos to the channels
                    await CheckIfObjectChangedAndPush(myupdateresult, mypgdata.Id, datatype);

                    break;

                case "skiarea":
                    mydata = await GetDataFromRaven.GetRavenData<SkiAreaLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<SkiAreaLinked, SkiAreaLinked>((SkiAreaLinked)mydata, TransformToPGObject.GetSkiAreaPGObject);
                    else
                        throw new Exception("No data found!");

                    //Add the PublishedOn Logic
                    ((SkiAreaLinked)mypgdata).CreatePublishenOnList();

                    myupdateresult = await SaveRavenObjectToPG<SkiAreaLinked>((SkiAreaLinked)mypgdata, "skiareas", true, true);

                    //Check if the Object has Changed and Push all infos to the channels
                    await CheckIfObjectChangedAndPush(myupdateresult, mypgdata.Id, datatype);

                    break;

                case "skiregion":
                    mydata = await GetDataFromRaven.GetRavenData<SkiRegionLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<SkiRegionLinked, SkiRegionLinked>((SkiRegionLinked)mydata, TransformToPGObject.GetSkiRegionPGObject);
                    else
                        throw new Exception("No data found!");

                    //Add the PublishedOn Logic
                    ((SkiRegionLinked)mypgdata).CreatePublishenOnList();

                    myupdateresult = await SaveRavenObjectToPG<SkiRegionLinked>((SkiRegionLinked)mypgdata, "skiregions", true, true);

                    //Check if the Object has Changed and Push all infos to the channels
                    await CheckIfObjectChangedAndPush(myupdateresult, mypgdata.Id, datatype);

                    break;

                case "article":
                    mydata = await GetDataFromRaven.GetRavenData<ArticlesLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<ArticlesLinked, ArticlesLinked>((ArticlesLinked)mydata, TransformToPGObject.GetArticlePGObject);
                    else
                        throw new Exception("No data found!");

                    //Add the PublishedOn Logic
                    ((ArticlesLinked)mypgdata).CreatePublishenOnList();

                    myupdateresult = await SaveRavenObjectToPG<ArticlesLinked>((ArticlesLinked)mypgdata, "articles", true, true);

                    //Check if the Object has Changed and Push all infos to the channels
                    await CheckIfObjectChangedAndPush(myupdateresult, mypgdata.Id, datatype);

                    break;

                case "odhtag":
                    mydata = await GetDataFromRaven.GetRavenData<ODHTagLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<ODHTagLinked, ODHTagLinked>((ODHTagLinked)mydata, TransformToPGObject.GetODHTagPGObject);
                    else
                        throw new Exception("No data found!");

                    //TODO Add the PublishedOn Logic

                    myupdateresult = await SaveRavenObjectToPG<ODHTagLinked>((ODHTagLinked)mypgdata, "smgtags", true);

                    //Check if the Object has Changed and Push all infos to the channels
                    await CheckIfObjectChangedAndPush(myupdateresult, mypgdata.Id, datatype);

                    break;

                case "measuringpoint":
                    mydata = await GetDataFromRaven.GetRavenData<MeasuringpointLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken, "Weather/Measuringpoint/");
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<MeasuringpointLinked, MeasuringpointLinked>((MeasuringpointLinked)mydata, TransformToPGObject.GetMeasuringpointPGObject);
                    else
                        throw new Exception("No data found!");

                    //Add the PublishedOn Logic
                    ((MeasuringpointLinked)mypgdata).CreatePublishenOnList();

                    myupdateresult = await SaveRavenObjectToPG<MeasuringpointLinked>((MeasuringpointLinked)mypgdata, "measuringpoints", true);

                    //Check if the Object has Changed and Push all infos to the channels
                    await CheckIfObjectChangedAndPush(myupdateresult, mypgdata.Id, datatype);

                    //Check if data has to be reduced and save it
                    if (ReduceDataTransformer.ReduceDataCheck<MeasuringpointLinked>((MeasuringpointLinked)mypgdata) == true)
                    {
                        var reducedobject = ReduceDataTransformer.GetReducedObject((MeasuringpointLinked)mypgdata, ReduceDataTransformer.CopyLTSMeasuringpointToReducedObject);

                        updateresultreduced = await SaveRavenObjectToPG<MeasuringpointLinked>((MeasuringpointLinkedReduced)reducedobject, "measuringpoints");
                    }

                    break;

                case "venue":
                    //TODO ADD new Venue Model

                    mydata = await GetDataFromRaven.GetRavenData<DDVenue>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);

                    var myvenuedata = default(IIdentifiable);
                    
                    if (mydata != null)
                    {
                        mypgdata = TransformToPGObject.GetPGObject<DDVenue, DDVenue>((DDVenue)mydata, TransformToPGObject.GetVenuePGObject);
                        mydata = TransformToPGObject.GetPGObject<DDVenue, VenueLinked>((DDVenue)mydata, TransformToPGObject.GetVenuePGObjectV2);
                    }                        
                    else
                        throw new Exception("No data found!");


                    //Add the PublishedOn Logic
                    ((VenueLinked)mydata).CreatePublishenOnList();
                    ((DDVenue)mypgdata).odhdata.PublishedOn = ((VenueLinked)mydata).PublishedOn.ToList();

                    //TODO Compare result
                    myupdateresult = await SaveRavenDestinationdataObjectToPG<VenueLinked, DDVenue>((VenueLinked)mydata, (DDVenue)mypgdata, "venues_v2");

                    //Check if the Object has Changed and Push all infos to the channels
                    await CheckIfObjectChangedAndPush(myupdateresult, mypgdata.Id, datatype);

                    //Check if data has to be reduced and save it
                    if (ReduceDataTransformer.ReduceDataCheck<DDVenue>((DDVenue)mypgdata) == true)
                    {
                        var reducedobject = ReduceDataTransformer.GetReducedObject((VenueLinked)mypgdata, ReduceDataTransformer.CopyLTSVenueToReducedObject);
                        var reducedobjectdd = ReduceDataTransformer.GetReducedObject((DDVenue)mypgdata, ReduceDataTransformer.CopyLTSVenueToReducedObject);

                        updateresultreduced = await SaveRavenDestinationdataObjectToPG<VenueLinked, DDVenue>((VenueReduced)reducedobject, (DDVenueReduced)reducedobjectdd, "venues_v2");
                    }

                    break;

                case "wine":
                    mydata = await GetDataFromRaven.GetRavenData<WineLinked>(datatype, id, settings.RavenConfig.ServiceUrl, settings.RavenConfig.User, settings.RavenConfig.Password, cancellationToken);
                    if (mydata != null)
                        mypgdata = TransformToPGObject.GetPGObject<WineLinked, WineLinked>((WineLinked)mydata, TransformToPGObject.GetWinePGObject);
                    else
                        throw new Exception("No data found!");

                    //Add the PublishedOn Logic
                    ((WineLinked)mypgdata).CreatePublishenOnList();

                    myupdateresult = await SaveRavenObjectToPG<WineLinked>((WineLinked)mypgdata, "wines", true);

                    //Check if the Object has Changed and Push all infos to the channels
                    await CheckIfObjectChangedAndPush(myupdateresult, mypgdata.Id, datatype);


                    break;

                default:
                    throw new Exception("no match found");
            }

            return Tuple.Create<string, UpdateDetail>(mypgdata.Id, GenericResultsHelper.MergeUpdateDetail(new List<UpdateDetail> { myupdateresult, updateresultreduced }));
        }

        /// <summary>
        /// Save only, requires Object implementing IIdentifiable, IMetaData, IImportDateassigneable, ILicenseInfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datatosave"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private async Task<UpdateDetail> SaveRavenObjectToPG<T>(T datatosave, string table) where T : IIdentifiable, IImportDateassigneable, IMetaData, ILicenseInfo, new()
        {
            datatosave._Meta.LastUpdate = datatosave.LastChange;
        
            var result = await QueryFactory.UpsertData<T>(datatosave, table, "lts.push.import", "odh.importer", false, false);

            return new UpdateDetail() { created = result.created, updated = result.updated, deleted = result.deleted, error = result.error, objectchanged = 0, objectimagechanged = 0, comparedobjects = 0, pushchannels = result.pushchannels };
        }

        /// <summary>
        /// Save and Compare Object changes, requires Object implementing IIdentifiable, IMetaData, IImportDateassigneable, ILicenseInfo, IPublishedOn
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datatosave"></param>
        /// <param name="table"></param>
        /// <param name="compareresult"></param>
        /// <returns></returns>
        private async Task<UpdateDetail> SaveRavenObjectToPG<T>(T datatosave, string table, bool compareresult) where T : IIdentifiable, IImportDateassigneable, IMetaData, ILicenseInfo, IPublishedOn, new()
        {
            datatosave._Meta.LastUpdate = datatosave.LastChange;            

            var result = await QueryFactory.UpsertDataAndCompare<T>(datatosave, table, "lts.push.import", "odh.importer", false, false, compareresult);

            return new UpdateDetail() { created = result.created, updated = result.updated, deleted = result.deleted, error = result.error, objectchanged = result.objectchanged, objectimagechanged = 0, comparedobjects = result.compareobject != null && result.compareobject.Value ? 1 : 0, pushchannels = result.pushchannels };
        }

        /// <summary>
        /// Save and Compare Object and Image Changes, requires Object implementing IIdentifiable, IMetaData, IImportDateassigneable, ILicenseInfo, IPublishedOn and IImageGalleryAware
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datatosave"></param>
        /// <param name="table"></param>
        /// <param name="compareresult"></param>
        /// <param name="compareimagechange"></param>
        /// <returns></returns>
        private async Task<UpdateDetail> SaveRavenObjectToPG<T>(T datatosave, string table, bool compareresult, bool compareimagechange) where T : IIdentifiable, IImportDateassigneable, IMetaData, ILicenseInfo, IImageGalleryAware, IPublishedOn, new()
        {
            datatosave._Meta.LastUpdate = datatosave.LastChange;

            var result = await QueryFactory.UpsertDataAndFullCompare<T>(datatosave, table, "lts.push.import", "odh.importer", false, false, compareresult, compareimagechange);

            return new UpdateDetail() { created = result.created, updated = result.updated, deleted = result.deleted, error = result.error, objectchanged = result.objectchanged, objectimagechanged = result.objectimageschanged, comparedobjects = result.compareobject != null && result.compareobject.Value ? 1 : 0, pushchannels = result.pushchannels };
        }

        //For Destinationdata Venue
        private async Task<UpdateDetail> SaveRavenDestinationdataObjectToPG<T, V>(T datatosave, V destinationdatatosave, string table) 
            where T : IIdentifiable, IImportDateassigneable, IMetaData, ILicenseInfo, IPublishedOn, IImageGalleryAware, new()
            where V : IIdentifiable, IImportDateassigneable, IMetaData, ILicenseInfo
        {
            datatosave._Meta.LastUpdate = datatosave.LastChange;

            var result = await QueryFactory.UpsertDataDestinationData<T,V>(datatosave, destinationdatatosave, table, false, false, true, true);

            return new UpdateDetail() { created = result.created, updated = result.updated, deleted = result.deleted, error = result.error, comparedobjects = result.compareobject != null && result.compareobject.Value ? 1 : 0, objectchanged = result.objectchanged, objectimagechanged = result.objectimageschanged, pushchannels = result.pushchannels };
        }

        private async Task CheckIfObjectChangedAndPush(UpdateDetail myupdateresult, string id, string datatype)
        {
            //Check if data has changed and Push To all channels
            if (myupdateresult.objectchanged != null && myupdateresult.objectchanged > 0 && myupdateresult.pushchannels != null && myupdateresult.pushchannels.Count > 0)
            {                
                //Check if image has changed
                bool hasimagechanged = false;
                if (myupdateresult.objectimagechanged != null && myupdateresult.objectimagechanged.Value > 0)
                    hasimagechanged = true;

                var pushresults = await OdhPushnotifier.PushToPublishedOnServices(id, datatype.ToLower(), "lts.push", hasimagechanged, "api", myupdateresult.pushchannels.ToList());

                if (pushresults != null)
                {
                    myupdateresult.pushed = new Dictionary<string, string>();

                    foreach (var pushresult in pushresults)
                    {
                        myupdateresult.pushed.TryAddOrUpdate(pushresult.Key, pushresult.Value.HttpStatusCode + ":" + pushresult.Value.Response);
                    }
                }
            }            
        }

        #endregion
    }
}
