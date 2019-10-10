using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    public class LTSTaggingHelper
    {
        public static string GetTaggingTypebyActivityType(string activitytype) =>
            activitytype switch
            {
                //Status Enumcode bringt: 391 Tagging bringt: 391
                "ALPINE" => "3A04AE3D220149B2AEA9640C9A13DB1F",

                //Status Enumcode bringt: 1361 Tagging bringt: 1361
                "HIKE" => "978F89296ACB4DB4B6BD1C269341802F",

                //Status Enumcode bringt: 434 Tagging bringt: 434
                "BIKE" => "2C1D1E0CA4E849229DA90775CBBF2312",

                //Status Enumcode bringt: 145 Tagging bringt: 145
                "RUNNING_FITNESS" => "1820951B216F46818F6FA9A3A685B6BD",

                //Status Enumcode bringt: 4  Tagging bringt: 4
                "CITYTOUR" => "33041C260F81427F8A3E5F0B72E57B26",

                //Status Enumcode bringt: 5  Tagging bringt: 5
                "EQUESTRIANISM" => "4B1F7527CCE049BD8293EE029AA54CD6",

                //Status Enumcode bringt: 750  Tagging bringt: 750
                "SLOPE" => "EB5D6F10C0CB4797A2A04818088CD6AB",

                //Status Enumcode bringt: 139  Tagging bringt: 139
                "SLIDE" => "F3B08D06569646F38462EDCA506D81D4",

                //Status Enumcode bringt: 272  Tagging bringt: 272
                "SKITRACK" => "D544A6312F8A47CF80CC4DFF8833FE50",

                //Status Enumcode bringt: 481  Tagging bringt: 481
                "LIFT" => "E23AA37B2AE3477F96D1C0782195AFDF",

                //Status Enumcode bringt:  Tagging bringt: 3982
                _ => "E924026962F74DA6B08BAD75FAFA1625",
            };

        public static string ActivityTypeListTranslator(string type) =>
            type switch
            {
                "HIKE" => "Wandern",
                "ALPINE" => "Berg",
                "BIKE" => "Radfahren",
                "RUNNING_FITNESS" => "Laufen und Fitness",
                "CITYTOUR" => "Stadtrundgang",
                "EQUESTRIANISM" => "Pferdesport",
                "SLIDE" => "Rodelbahnen",
                "SLOPE" => "Piste",
                "SKITRACK" => "Loipen",
                "LIFT" => "Aufstiegsanlagen",
                _ => "IRGENDWOS",
            };

        /// <summary>
        /// Methode gibt entsprechende Tag Rids nach alten poitypes zurück, default ist der Tag RID für POIs gesamt
        /// </summary>
        /// <param name="poitype"></param>
        /// <returns></returns>
        public static string GetTaggingTypebyPoiType(string poitype) =>
            poitype switch
            {
                //Status Enumcode bringt: 1011 Tagging bringt: 1010
                "ACTIVE" => "AC90E2F2AB3C4D4982E578B3C68086F3",

                //Status Enumcode bringt: 223 Tagging bringt: 227
                "HEALTH" => "B888A70A9A6542089F6DD10045834EC0",

                //case "SHOP":
                //    return "EEA4898A7489403CA4EE783B711A075C";

                //Status Enumcode bringt: 1028 Tagging bringt: 1022
                "SIGHTSEEN" => "3B4A6331987C4735A7CFE16AD9B095C5",

                "ARTISAN" => "516EBD57175C4D36921F1E0E8B61A87D",

                "SERVICEPROVIDER" => "F3CECD17D41E4EF8B4CAFF96FBBEF428",

                "SHOP" => "6648801452AE4A2185026113B97CD4BA",

                //Status Enumcode bringt: 58  Tagging bringt: 58
                "NIGHTLIFE" => "2D0F6A0701144431860BDDFA77EE55D5",

                //Status Enumcode bringt: 360  Tagging bringt: 403
                "SERVICE" => "93726D39D7DF4903BE2957750C4CD069",

                //Status Enumcode bringt: 673  Tagging bringt: 673
                "MOBILITY_TRAFFIC" => "56CCCEF721934382A31495C08C94B092",

                //Status Enumcode bringt:  Tagging bringt: 5737
                _ => "D8739556BA8A44DEB374FF62025D7A8D",
            };

        public static string TypeListTranslator(string type) =>
            type switch
            {
                "ACTIVE" => "Sport und Freizeit",
                "NIGHTLIFE" => "Nachtleben und Unterhaltung",
                "MOBILITY_TRAFFIC" => "Verkehr und Transport",
                "SERVICE" => "Öffentliche Einrichtungen",
                "SHOP" => "Geschäfte",
                "SERVICEPROVIDER" => "Dienstleister",
                "SIGHTSEEN" => "Kultur und Sehenswürdigkeiten",
                "HEALTH" => "Ärzte, Apotheken",
                "ARTISAN" => "Kunsthandwerker",
                _ => "IRGENDWOS",
            };

        public static string CheckActivityPoi(string type) =>
            (type.ToLower()) switch
            {
                "berg" => "ALPINE",
                "wandern" => "HIKE",
                "radfahren" => "BIKE",
                "piste" => "SLOPE",
                "rodelbahnen" => "SLIDE",
                "aufstiegsanlagen" => "LIFT",
                "laufen und Fitness" => "RUNNING_FITNESS",
                "loipen" => "SKITRACK",
                "stadtrundgang" => "CITYTOUR",
                "pferdesport" => "EQUESTRIANISM",
                "sport und freizeiteinrichtungen" => "ACTIVE",
                "sport und freizeit" => "ACTIVE",
                "kunsthandwerk und brauchtum" => "ARTISAN",
                "kunsthandwerker" => "ARTISAN",
                "gesundheit und wohlbefinden" => "HEALTH",
                "ärtze, apotheken" => "HEALTH",
                "ärzte, apotheken" => "HEALTH",
                "verkehr und transport" => "MOBILITY_TRAFFIC",
                "nachtleben und unterhaltung" => "NIGHTLIFE",
                "öffentliche einrichtungen" => "SERVICE",
                "dienstleister" => "SERVICEPROVIDER",
                "kultur und sehenswürdigkeiten" => "SIGHTSEEN",
                "geschäfte und dienstleister" => "SHOP",
                "geschäfte" => "SHOP",
                "hike" => "HIKE",
                "bike" => "BIKE",
                "running_fitness" => "RUNNING_FITNESS",
                "slide" => "SLIDE",
                "slope" => "SLOPE",
                "lift" => "LIFT",
                "alpine" => "ALPINE",
                "skitrack" => "SKITRACK",
                "equestrianism" => "EQUESTRIANISM",
                "citytour" => "CITYTOUR",
                _ => "ERROR",
            };

        public static async IAsyncEnumerable<LTSTaggingType> GetLTSTagParentsPGAsync(
            IPostGreSQLConnectionFactory connectionFactory, LTSTaggingType currenttag, IEnumerable<LTSTaggingType> ltstagparentlist,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (currenttag.Level > 0)
            {
                var where = PostgresSQLWhereBuilder.CreateIdListWhereExpression(currenttag.TypeParent);
                var parent = await PostgresSQLHelper.SelectFromTableDataAsObjectParametrizedAsync<LTSTaggingType>(
                    connectionFactory, "ltstaggingtypes", "*", where, "", 1, null, cancellationToken).FirstOrDefaultAsync();

                yield return parent;

                await foreach (var elem in GetLTSTagParentsPGAsync(connectionFactory, parent, ltstagparentlist, cancellationToken))
                    yield return elem;
            }
            foreach (var elem in ltstagparentlist)
                yield return elem;
        }


        public static async Task<IDictionary<string, string>> GetPoiTypeDescAsync(
            string? key, IAsyncEnumerable<LTSTaggingType> ltstaggingtypes)
        {
            IDictionary<string, string> maintypedict = new Dictionary<string, string>();

            var taggingtype = await ltstaggingtypes.Where(x => x.Key == key).FirstOrDefaultAsync();

            if (taggingtype != null)
                maintypedict = taggingtype.TypeNames;

            return maintypedict;
        }

        public static async Task<IDictionary<string, string>> GetActivityTypeDescAsync(
            string? key, IAsyncEnumerable<LTSTaggingType> ltstaggingtypes)
        {
            IDictionary<string, string> maintypedict = new Dictionary<string, string>();

            var taggingtype = await ltstaggingtypes.Where(x => x.Key == key).FirstOrDefaultAsync();

            if (taggingtype != null)
                maintypedict = taggingtype.TypeNames;

            return maintypedict;
        }

        public static string? LTSActivityTaggingTagTranslator(string? key) =>
            key switch
            {
                "Berg" => "Berge",
                "Stadtrundgang" => "Ortstouren",
                "Pferdesport" => "Pferde",
                "Piste" => "Pisten",
                _ => key,
            };
    }

    public class LTSAreaHelper
    {
        public static async IAsyncEnumerable<string> GetAreasNotToConsiderPGAsync(
            IPostGreSQLConnectionFactory connectionFactory, [EnumeratorCancellation] CancellationToken cancellationToken)
        {

            //var areasnottoconsider = PostgresSQLHelper.SelectFromTableDataAsId(conn, "areas", "data->'Id' as Id", "data @>'{\"RegionId\":null}' OR data @>'{\"RegionId\":\"\"}' OR data @>'{\"RegionId\":\"TOASSIGN\"}'", "",0, null);
            var areasnottoconsider = PostgresSQLHelper.SelectFromTableDataAsObjectAsync<string>(
                connectionFactory, "areas", "Id as PgId, data->'Id' as Id",
                "data @>'{\"RegionId\":null}' OR data @>'{\"RegionId\":\"\"}' OR data @>'{\"RegionId\":\"TOASSIGN\"}'",
                "", 0, null, cancellationToken);

            //session.Query<Area, AreaFilter>().Where(x => x.RegionId == null || x.RegionId == "TOASSIGN").Select(x => x.Id).ToList();

            await foreach (var area in areasnottoconsider)
                yield return area.ToUpper();
        }
    }
}
