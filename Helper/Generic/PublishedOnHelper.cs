using DataModel;
using Helper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public static class PublishedOnHelper
    {           
        public static void CreatePublishenOnList<T>(this T mydata, ICollection<AllowedTags>? allowedtags = null, Tuple<string,bool>? activatesourceonly = null) where T : IIdentifiable, IMetaData, ISource, IPublishedOn
        {
            //alowedsources  Dictionary<odhtype, sourcelist> TODO Export in Config
            Dictionary<string, List<string>> allowedsourcesMP = new Dictionary<string, List<string>>()
            {
                { "event", new List<string>(){ "lts" } },
                { "accommodation", new List<string>(){ "lts" } },
                { "odhactivitypoi", new List<string>(){ "lts","suedtirolwein", "archapp" } }
            };

            //Blacklist for exceptions Dictionary<string, Tuple<string,string> TODO Export in Config
            Dictionary<string, Tuple<string, string>> blacklistsourcesandtagsMP = new Dictionary<string, Tuple<string, string>>()
            {
               { "odhactivitypoi", Tuple.Create("lts", "weinkellereien") }
            };

            //Whitelist on Types Deprecated? TODO Export in Config
            Dictionary<string, List<string>> allowedtypesMP = new Dictionary<string, List<string>>()
            {
                { "article", new List<string>(){ "rezeptartikel" } }
            };


            List<string> publishedonlist = new List<string>();

            switch (mydata._Meta.Type)
            {
                //Accommodations smgactive (Source LTS IDMActive)
                case "accommodation":
                    if ((mydata as AccommodationLinked).SmgActive && allowedsourcesMP[mydata._Meta.Type].Contains(mydata._Meta.Source))
                    {
                        publishedonlist.TryAddOrUpdateOnList("https://www.suedtirol.info");
                        publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                    }
                    break;

                //Accommodation Room publishedon
                case "accommodationroom":
                    
                    //TO check add publishedon logic only for rooms with source hgv? for online bookable accommodations?

                    if(activatesourceonly != null && activatesourceonly.Item2 == true)
                    {
                        if(activatesourceonly.Item1 == (mydata as AccommodationRoomLinked)._Meta.Source)
                        {
                            publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                        }
                    }
                    else
                    {
                         publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                    }

                    publishedonlist.TryAddOrUpdateOnList("https://www.suedtirol.info");

                    break;
                //Event Add all Active Events from now
                case "event":
                    //EVENTS LTS
                    if ((mydata as EventLinked).Active && allowedsourcesMP[mydata._Meta.Type].Contains(mydata._Meta.Source))
                    {
                        if ((mydata as EventLinked).SmgActive)
                            publishedonlist.TryAddOrUpdateOnList("https://www.suedtirol.info");

                        //Add only for Events for the future
                        if ((mydata as EventLinked).NextBeginDate >= new DateTime(2023, 1, 1))
                            publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                    }

                    //Events DRIN CENTROTREVI
                    if ((mydata as EventLinked).Active && (mydata._Meta.Source == "trevilab" || mydata._Meta.Source == "drin"))
                    {
                        if ((mydata as EventLinked).SmgActive)
                        {
                            if(mydata._Meta.Source == "drin")
                                publishedonlist.TryAddOrUpdateOnList("drin");
                            if (mydata._Meta.Source == "trevilab")
                                publishedonlist.TryAddOrUpdateOnList("centro-trevi");
                        }
                            
                    }
                    break;

                //ODHActivityPoi 
                case "odhactivitypoi":

                    if ((mydata as ODHActivityPoiLinked).Active && allowedsourcesMP[mydata._Meta.Type].Contains(mydata._Meta.Source))
                    {
                        if ((mydata as ODHActivityPoiLinked).SmgActive)
                            publishedonlist.TryAddOrUpdateOnList("https://www.suedtirol.info");

                        //IF category is whitelisted
                        if (allowedtags.Select(x => x.Id).ToList().Intersect((mydata as ODHActivityPoiLinked).SmgTags).Count() > 0)
                        {
                            var isonblacklist = false;

                            if (blacklistsourcesandtagsMP[mydata._Meta.Type] != null && blacklistsourcesandtagsMP[mydata._Meta.Type].Item1 == mydata._Meta.Source && (mydata as ODHActivityPoiLinked).SmgTags.Contains(blacklistsourcesandtagsMP[mydata._Meta.Type].Item2))
                                isonblacklist = true;

                            if (!isonblacklist)
                                publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                        }
                    }

                    break;

                //EventShort 
                case "eventshort":

                    if ((mydata as EventShortLinked).ActiveWeb == true)
                    {
                        publishedonlist.TryAddOrUpdateOnList("https://noi.bz.it");
                    }
                    if ((mydata as EventShortLinked).ActiveToday == true)
                    {
                        publishedonlist.TryAddOrUpdateOnList("https://today.noi.bz");
                    }
                    if ((mydata as EventShortLinked).ActiveCommunityApp == true)
                    {
                        publishedonlist.TryAddOrUpdateOnList("noi-communityapp");
                    }

                    break;

                case "measuringpoint":
                    if ((mydata as MeasuringpointLinked).Active)
                    {
                        publishedonlist.TryAddOrUpdateOnList("https://www.suedtirol.info");
                        publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                    }
                    break;

                case "venue":
                    if ((mydata as VenueLinked).Active == true)
                    {
                        publishedonlist.TryAddOrUpdateOnList("https://www.suedtirol.info");
                        publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                    }
                    break;

                //TO CHECK, import all and set it active on Marketplace?
                case "webcam":
                    if ((mydata as WebcamInfoLinked).SmgActive == true)
                    {
                        publishedonlist.TryAddOrUpdateOnList("https://www.suedtirol.info");
                        publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                    }
                    break;

                case "wineaward":
                    if ((mydata as WineLinked).Active == true)
                    {
                        publishedonlist.TryAddOrUpdateOnList("https://www.suedtirol.info");
                        publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                    }
                    break;

                case "region":
                    if ((mydata as RegionLinked).Active)
                    {
                        publishedonlist.TryAddOrUpdateOnList("https://www.suedtirol.info");
                        publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                    }
                    break;

                case "tourismassociation":
                    if ((mydata as TourismvereinLinked).Active)
                    {
                        publishedonlist.TryAddOrUpdateOnList("https://www.suedtirol.info");
                        publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                    }
                    break;

                case "district":
                    if ((mydata as DistrictLinked).Active)
                    {
                        publishedonlist.TryAddOrUpdateOnList("https://www.suedtirol.info");
                        publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                    }
                    break;

                case "municipality":
                    if ((mydata as MunicipalityLinked).Active)
                    {
                        publishedonlist.TryAddOrUpdateOnList("https://www.suedtirol.info");
                        publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                    }
                    break;

                case "metaregion":
                    if ((mydata as MetaRegionLinked).Active)
                    {
                        publishedonlist.TryAddOrUpdateOnList("https://www.suedtirol.info");
                        publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                    }
                    break;

                case "area":
                    if ((mydata as AreaLinked).Active)
                    {
                        publishedonlist.TryAddOrUpdateOnList("https://www.suedtirol.info");
                        publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                    }
                    break;

                case "skiarea":
                    if ((mydata as SkiAreaLinked).Active)
                    {
                        publishedonlist.TryAddOrUpdateOnList("https://www.suedtirol.info");
                        publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                    }
                    break;

                case "skiregion":
                    if ((mydata as SkiRegionLinked).Active)
                    {
                        publishedonlist.TryAddOrUpdateOnList("https://www.suedtirol.info");
                        publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                    }
                    break;

                case "experiencearea":
                    if ((mydata as ExperienceAreaLinked).Active)
                    {
                        publishedonlist.TryAddOrUpdateOnList("https://www.suedtirol.info");
                        publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                    }
                    break;

                case "article":
                    var article = (mydata as ArticlesLinked);

                    if (article.SmgActive && allowedtypesMP[mydata._Meta.Type].Contains(article.Type.ToLower()))
                    {
                        publishedonlist.TryAddOrUpdateOnList("https://www.suedtirol.info");
                        //publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                    }
                    break;

                case "odhtag":
                    var odhtag = (mydata as ODHTagLinked);
                    if (odhtag != null && odhtag.DisplayAsCategory != null && odhtag.DisplayAsCategory.Value == true)
                        publishedonlist.TryAddOrUpdateOnList("idm-marketplace");                    
                    break;

                //obsolete do nothing

                case "ltsactivity":
                    break;

                case "ltspoi":
                    break;

                case "ltsgastronomy":
                    break;

                default:

                    break;
            }

            mydata.PublishedOn = publishedonlist;
        }

    }
}
