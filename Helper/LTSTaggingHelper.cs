using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Helper
{
    public class LTSTaggingHelper
    {
        public static string GetTaggingTypebyActivityType(string activitytype)
        {
            switch (activitytype)
            {
                //Status Enumcode bringt: 391 Tagging bringt: 391
                case "ALPINE":
                    return "3A04AE3D220149B2AEA9640C9A13DB1F";

                //Status Enumcode bringt: 1361 Tagging bringt: 1361
                case "HIKE":
                    return "978F89296ACB4DB4B6BD1C269341802F";

                //Status Enumcode bringt: 434 Tagging bringt: 434
                case "BIKE":
                    return "2C1D1E0CA4E849229DA90775CBBF2312";

                //Status Enumcode bringt: 145 Tagging bringt: 145
                case "RUNNING_FITNESS":
                    return "1820951B216F46818F6FA9A3A685B6BD";

                //Status Enumcode bringt: 4  Tagging bringt: 4
                case "CITYTOUR":
                    return "33041C260F81427F8A3E5F0B72E57B26";

                //Status Enumcode bringt: 5  Tagging bringt: 5
                case "EQUESTRIANISM":
                    return "4B1F7527CCE049BD8293EE029AA54CD6";

                //Status Enumcode bringt: 750  Tagging bringt: 750
                case "SLOPE":
                    return "EB5D6F10C0CB4797A2A04818088CD6AB";

                //Status Enumcode bringt: 139  Tagging bringt: 139
                case "SLIDE":
                    return "F3B08D06569646F38462EDCA506D81D4";

                //Status Enumcode bringt: 272  Tagging bringt: 272
                case "SKITRACK":
                    return "D544A6312F8A47CF80CC4DFF8833FE50";

                //Status Enumcode bringt: 481  Tagging bringt: 481
                case "LIFT":
                    return "E23AA37B2AE3477F96D1C0782195AFDF";

                //Status Enumcode bringt:  Tagging bringt: 3982
                default:
                    return "E924026962F74DA6B08BAD75FAFA1625";
            }
        }

        public static string ActivityTypeListTranslator(string type)
        {
            switch (type)
            {
                case "HIKE":
                    return "Wandern";

                case "ALPINE":
                    return "Berg";

                case "BIKE":
                    return "Radfahren";

                case "RUNNING_FITNESS":
                    return "Laufen und Fitness";

                case "CITYTOUR":
                    return "Stadtrundgang";

                case "EQUESTRIANISM":
                    return "Pferdesport";

                case "SLIDE":
                    return "Rodelbahnen";

                case "SLOPE":
                    return "Piste";

                case "SKITRACK":
                    return "Loipen";

                case "LIFT":
                    return "Aufstiegsanlagen";

                default:
                    return "IRGENDWOS";
            }
        }

        /// <summary>
        /// Methode gibt entsprechende Tag Rids nach alten poitypes zurück, default ist der Tag RID für POIs gesamt
        /// </summary>
        /// <param name="poitype"></param>
        /// <returns></returns>
        public static string GetTaggingTypebyPoiType(string poitype)
        {
            switch (poitype)
            {
                //Status Enumcode bringt: 1011 Tagging bringt: 1010
                case "ACTIVE":
                    return "AC90E2F2AB3C4D4982E578B3C68086F3";

                //Status Enumcode bringt: 223 Tagging bringt: 227
                case "HEALTH":
                    return "B888A70A9A6542089F6DD10045834EC0";

                //case "SHOP":
                //    return "EEA4898A7489403CA4EE783B711A075C";

                //Status Enumcode bringt: 1028 Tagging bringt: 1022
                case "SIGHTSEEN":
                    return "3B4A6331987C4735A7CFE16AD9B095C5";

                case "ARTISAN":
                    return "516EBD57175C4D36921F1E0E8B61A87D";

                case "SERVICEPROVIDER":
                    return "F3CECD17D41E4EF8B4CAFF96FBBEF428";

                case "SHOP":
                    return "6648801452AE4A2185026113B97CD4BA";

                //Status Enumcode bringt: 58  Tagging bringt: 58
                case "NIGHTLIFE":
                    return "2D0F6A0701144431860BDDFA77EE55D5";

                //Status Enumcode bringt: 360  Tagging bringt: 403
                case "SERVICE":
                    return "93726D39D7DF4903BE2957750C4CD069";

                //Status Enumcode bringt: 673  Tagging bringt: 673
                case "MOBILITY_TRAFFIC":
                    return "56CCCEF721934382A31495C08C94B092";

                //Status Enumcode bringt:  Tagging bringt: 5737
                default:
                    return "D8739556BA8A44DEB374FF62025D7A8D";
            }
        }

        public static string TypeListTranslator(string type)
        {
            switch (type)
            {
                case "ACTIVE":
                    return "Sport und Freizeit";
                case "NIGHTLIFE":
                    return "Nachtleben und Unterhaltung";
                case "MOBILITY_TRAFFIC":
                    return "Verkehr und Transport";
                case "SERVICE":
                    return "Öffentliche Einrichtungen";
                case "SHOP":
                    return "Geschäfte";
                case "SERVICEPROVIDER":
                    return "Dienstleister";
                case "SIGHTSEEN":
                    return "Kultur und Sehenswürdigkeiten";
                case "HEALTH":
                    return "Ärzte, Apotheken";
                case "ARTISAN":
                    return "Kunsthandwerker";
                default:
                    return "IRGENDWOS";
            }
        }

        public static string CheckActivityPoi(string type)
        {
            switch (type.ToLower())
            {
                case "berg":
                    return "ALPINE";
                case "wandern":
                    return "HIKE";
                case "radfahren":
                    return "BIKE";
                case "piste":
                    return "SLOPE";
                case "rodelbahnen":
                    return "SLIDE";
                case "aufstiegsanlagen":
                    return "LIFT";
                case "laufen und Fitness":
                    return "RUNNING_FITNESS";
                case "loipen":
                    return "SKITRACK";
                case "stadtrundgang":
                    return "CITYTOUR";
                case "pferdesport":
                    return "EQUESTRIANISM";
                case "sport und freizeiteinrichtungen":
                    return "ACTIVE";
                case "sport und freizeit":
                    return "ACTIVE";
                case "kunsthandwerk und brauchtum":
                    return "ARTISAN";
                case "kunsthandwerker":
                    return "ARTISAN";
                case "gesundheit und wohlbefinden":
                    return "HEALTH";
                case "ärtze, apotheken":
                    return "HEALTH";
                case "ärzte, apotheken":
                    return "HEALTH";
                case "verkehr und transport":
                    return "MOBILITY_TRAFFIC";
                case "nachtleben und unterhaltung":
                    return "NIGHTLIFE";
                case "öffentliche einrichtungen":
                    return "SERVICE";
                case "dienstleister":
                    return "SERVICEPROVIDER";
                case "kultur und sehenswürdigkeiten":
                    return "SIGHTSEEN";
                case "geschäfte und dienstleister":
                    return "SHOP";
                case "geschäfte":
                    return "SHOP";
                case "hike":
                    return "HIKE";
                case "bike":
                    return "BIKE";
                case "running_fitness":
                    return "RUNNING_FITNESS";
                case "slide":
                    return "SLIDE";
                case "slope":
                    return "SLOPE";
                case "lift":
                    return "LIFT";
                case "alpine":
                    return "ALPINE";
                case "skitrack":
                    return "SKITRACK";
                case "equestrianism":
                    return "EQUESTRIANISM";
                case "citytour":
                    return "CITYTOUR";
            }

            return "ERROR";
        }

        public static List<LTSTaggingType> GetLTSTagParentsPG(NpgsqlConnection conn, LTSTaggingType currenttag, List<LTSTaggingType> ltstagparentlist)
        {
            //List<LTSTaggingType> ltstagparentlist = new List<LTSTaggingType>();

            if (currenttag.Level > 0)
            {
                var parent = PostgresSQLHelper.SelectFromTableDataAsObject<LTSTaggingType>(conn, "ltstaggingtypes", currenttag.TypeParent);


                ltstagparentlist.Add(parent);

                GetLTSTagParentsPG(conn, parent, ltstagparentlist);

                return ltstagparentlist;
            }
            else
                return ltstagparentlist;

        }

   
        public static IDictionary<string, string> GetPoiTypeDesc(string key, List<LTSTaggingType> ltstaggingtypes)
        {
            IDictionary<string, string> maintypedict = new Dictionary<string, string>();

            var taggingtype = ltstaggingtypes.Where(x => x.Key == key).FirstOrDefault();

            if (taggingtype != null)
                maintypedict = taggingtype.TypeNames;

            return maintypedict;
        }

        public static IDictionary<string, string> GetActivityTypeDesc(string key, List<LTSTaggingType> ltstaggingtypes)
        {
            IDictionary<string, string> maintypedict = new Dictionary<string, string>();

            var taggingtype = ltstaggingtypes.Where(x => x.Key == key).FirstOrDefault();

            if (taggingtype != null)
                maintypedict = taggingtype.TypeNames;

            return maintypedict;
        }

        public static string LTSActivityTaggingTagTranslator(string key)
        {
            switch (key)
            {
                case "Berg":
                    return "Berge";
                case "Stadtrundgang":
                    return "Ortstouren";
                case "Pferdesport":
                    return "Pferde";
                case "Piste":
                    return "Pisten";
                default:
                    return key;
            }
        }
    }

    public class LTSAreaHelper
    {
        public static List<string> GetAreasNotToConsiderPG(NpgsqlConnection conn)
        {

            conn.Open();
            //var areasnottoconsider = PostgresSQLHelper.SelectFromTableDataAsId(conn, "areas", "data->'Id' as Id", "data @>'{\"RegionId\":null}' OR data @>'{\"RegionId\":\"\"}' OR data @>'{\"RegionId\":\"TOASSIGN\"}'", "",0, null);
            var areasnottoconsider = PostgresSQLHelper.SelectFromTableDataAsObject<string>(conn, "areas", "Id as PgId, data->'Id' as Id", "data @>'{\"RegionId\":null}' OR data @>'{\"RegionId\":\"\"}' OR data @>'{\"RegionId\":\"TOASSIGN\"}'", "", 0, null);

            conn.Close();

            //session.Query<Area, AreaFilter>().Where(x => x.RegionId == null || x.RegionId == "TOASSIGN").Select(x => x.Id).ToList();

            return areasnottoconsider.ConvertAll(x => x.ToUpper());
        }        
    }
}
