// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

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
        /// <summary>
        /// Create the publishedon List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mydata"></param>
        /// <param name="allowedtags"></param>
        /// <param name="activatesourceonly"></param>
        public static void CreatePublishedOnList<T>(this T mydata, ICollection<AllowedTags>? allowedtags = null, Tuple<string,bool>? activatesourceonly = null) where T : IIdentifiable, IMetaData, ISource, IPublishedOn
        {
            //alowedsources  Dictionary<odhtype, sourcelist> TODO Export in Config
            Dictionary<string, List<string>> allowedsourcesMP = new Dictionary<string, List<string>>()
            {
                { "event", new List<string>(){ "lts","drin","trevilab" } },
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

            var typeswitcher = ODHTypeHelper.TranslateType2TypeString<T>(mydata);

            if (mydata != null)
            {

                switch (typeswitcher)
                {
                    //Accommodations smgactive (Source LTS IDMActive)
                    case "accommodation":
                        if ((mydata as AccommodationLinked).SmgActive && allowedsourcesMP[mydata._Meta.Type].Contains(mydata._Meta.Source))
                        {
                            publishedonlist.TryAddOrUpdateOnList("suedtirol.info");
                            publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                        }
                        break;

                    //Accommodation Room publishedon
                    case "accommodationroom":

                        //TO check add publishedon logic only for rooms with source hgv? for online bookable accommodations?

                        if (activatesourceonly != null && activatesourceonly.Item2 == true)
                        {
                            if (activatesourceonly.Item1 == (mydata as AccommodationRoomLinked)._Meta.Source)
                            {
                                publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                            }
                        }
                        else
                        {
                            publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                        }

                        publishedonlist.TryAddOrUpdateOnList("suedtirol.info");

                        break;
                    //Event Add all Active Events from now
                    case "event":
                        //EVENTS LTS
                        if ((mydata as EventLinked).Active && allowedsourcesMP[mydata._Meta.Type].Contains(mydata._Meta.Source))
                        {
                            if ((mydata as EventLinked).SmgActive && mydata._Meta.Source == "lts")
                                publishedonlist.TryAddOrUpdateOnList("suedtirol.info");

                            //Add only for Events for the future
                            if ((mydata as EventLinked).NextBeginDate >= new DateTime(2023, 1, 1) && mydata._Meta.Source == "lts")
                                publishedonlist.TryAddOrUpdateOnList("idm-marketplace");

                            //Events DRIN CENTROTREVI
                            if ((mydata as EventLinked).Active && (mydata._Meta.Source == "trevilab" || mydata._Meta.Source == "drin"))
                            {
                                if ((mydata as EventLinked).SmgActive)
                                {
                                    if (mydata._Meta.Source == "drin")
                                        publishedonlist.TryAddOrUpdateOnList("centro-trevi.drin");
                                    if (mydata._Meta.Source == "trevilab")
                                        publishedonlist.TryAddOrUpdateOnList("centro-trevi.trevilab");
                                }

                            }
                        }


                        break;

                    //ODHActivityPoi 
                    case "odhactivitypoi":

                        if ((mydata as ODHActivityPoiLinked).Active && allowedsourcesMP[mydata._Meta.Type].Contains(mydata._Meta.Source))
                        {
                            if ((mydata as ODHActivityPoiLinked).SmgActive)
                                publishedonlist.TryAddOrUpdateOnList("suedtirol.info");

                            //IF category is white or blacklisted find an intersection
                            var tagintersection = allowedtags.Select(x => x.Id).ToList().Intersect((mydata as ODHActivityPoiLinked).SmgTags);

                            if (tagintersection.Count() > 0)
                            {
                                var blacklistedpublisher = new List<string>();

                                List<string> publisherstoadd = new List<string>();

                                foreach (var intersectedtag in tagintersection)
                                {
                                    var myallowedtag = allowedtags.Where(x => x.Id == intersectedtag).FirstOrDefault();

                                    foreach (var publishon in myallowedtag.PublishDataWithTagOn)
                                    {
                                        //Marked as blacklist overwrites whitelist
                                        if (publishon.Value == false)
                                            blacklistedpublisher.Add(publishon.Key);

                                        if (blacklistsourcesandtagsMP[mydata._Meta.Type] != null && blacklistsourcesandtagsMP[mydata._Meta.Type].Item1 == mydata._Meta.Source && (mydata as ODHActivityPoiLinked).SmgTags.Contains(blacklistsourcesandtagsMP[mydata._Meta.Type].Item2))
                                            blacklistedpublisher.Add("idm-marketplace");

                                        if (!blacklistedpublisher.Contains(publishon.Key))
                                        {
                                            if (!publisherstoadd.Contains(publishon.Key))
                                            {
                                                publisherstoadd.Add(publishon.Key);
                                            }
                                        }

                                    }
                                }

                                foreach (var publishertoadd in publisherstoadd)
                                {
                                    publishedonlist.TryAddOrUpdateOnList(publishertoadd);
                                }
                            }
                        }

                        break;

                    //EventShort 
                    case "eventshort":

                        if ((mydata as EventShortLinked).ActiveWeb == true)
                        {
                            publishedonlist.TryAddOrUpdateOnList("noi.bz.it");
                        }
                        if ((mydata as EventShortLinked).ActiveToday == true)
                        {
                            publishedonlist.TryAddOrUpdateOnList("today.noi.bz.it");
                        }
                        if ((mydata as EventShortLinked).ActiveCommunityApp == true)
                        {
                            publishedonlist.TryAddOrUpdateOnList("noi-communityapp");
                        }

                        break;

                    case "measuringpoint":
                        if ((mydata as MeasuringpointLinked).Active)
                        {
                            publishedonlist.TryAddOrUpdateOnList("suedtirol.info");
                            publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                        }
                        break;

                    case "venue":
                        if ((mydata as VenueLinked).Active == true)
                        {
                            publishedonlist.TryAddOrUpdateOnList("suedtirol.info");
                            publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                        }
                        break;

                    //TO CHECK, import all and set it active on Marketplace?
                    case "webcam":
                        if ((mydata as WebcamInfoLinked).SmgActive == true)
                        {
                            publishedonlist.TryAddOrUpdateOnList("suedtirol.info");
                            publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                        }
                        break;

                    case "wineaward":
                        if ((mydata as WineLinked).Active == true)
                        {
                            publishedonlist.TryAddOrUpdateOnList("suedtirol.info");
                            publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                        }
                        break;

                    case "region":
                        if ((mydata as RegionLinked).Active)
                        {
                            publishedonlist.TryAddOrUpdateOnList("suedtirol.info");
                            publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                        }
                        break;

                    case "tourismassociation":
                        if ((mydata as TourismvereinLinked).Active)
                        {
                            publishedonlist.TryAddOrUpdateOnList("suedtirol.info");
                            publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                        }
                        break;

                    case "district":
                        if ((mydata as DistrictLinked).Active)
                        {
                            publishedonlist.TryAddOrUpdateOnList("suedtirol.info");
                            publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                        }
                        break;

                    case "municipality":
                        if ((mydata as MunicipalityLinked).Active)
                        {
                            publishedonlist.TryAddOrUpdateOnList("suedtirol.info");
                            publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                        }
                        break;

                    case "metaregion":
                        if ((mydata as MetaRegionLinked).Active)
                        {
                            publishedonlist.TryAddOrUpdateOnList("suedtirol.info");
                            publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                        }
                        break;

                    case "area":
                        if ((mydata as AreaLinked).Active)
                        {
                            publishedonlist.TryAddOrUpdateOnList("suedtirol.info");
                            publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                        }
                        break;

                    case "skiarea":
                        if ((mydata as SkiAreaLinked).Active)
                        {
                            publishedonlist.TryAddOrUpdateOnList("suedtirol.info");
                            publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                        }
                        break;

                    case "skiregion":
                        if ((mydata as SkiRegionLinked).Active)
                        {
                            publishedonlist.TryAddOrUpdateOnList("suedtirol.info");
                            publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                        }
                        break;

                    case "experiencearea":
                        if ((mydata as ExperienceAreaLinked).Active)
                        {
                            publishedonlist.TryAddOrUpdateOnList("suedtirol.info");
                            publishedonlist.TryAddOrUpdateOnList("idm-marketplace");
                        }
                        break;

                    case "article":
                        var article = (mydata as ArticlesLinked);

                        if (article.SmgActive && allowedtypesMP[mydata._Meta.Type].Contains(article.Type.ToLower()))
                        {
                            publishedonlist.TryAddOrUpdateOnList("suedtirol.info");
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
}
