using DataModel;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class GetAdditionalTypeInfo
    {
        public static async Task<Dictionary<string, AdditionalPoiInfos>> GetAdditionalTypeInfoForPoi(QueryFactory QueryFactory, string? subtype, List<string>? languages)
        {
            if(languages == null)
                languages = new List<string>() { "de","it","en"};

            Dictionary<string, AdditionalPoiInfos> myadditionalpoinfosdict = new Dictionary<string, AdditionalPoiInfos>();

            //Get SuedtirolType Subtype
            var subtypequery = QueryFactory.Query("smgpoitypes")
                        .Select("data")
                        .Where("id", subtype?.ToLower());
                         //.WhereRaw("data->>'Key' LIKE $$", subtype);
            var subtypedata =
                await subtypequery
                    .GetFirstOrDefaultAsObject<SmgPoiTypes>();

            if (subtypedata != null)
            {
                var maintypequery = QueryFactory.Query("smgpoitypes")
                            .Select("data")
                            .Where("id", subtypedata.Parent?.ToLower());
                //.WhereRaw("data->>'Key' LIKE $$",  subtypedata.TypeParent);
                var maintypedata =
                    await maintypequery
                        .GetFirstOrDefaultAsObject<SmgPoiTypes>();

                var validtags = await ODHTagHelper.GetODHTagsValidforTranslations(QueryFactory, new List<string>(), new List<string>() { maintypedata?.Key ?? "", subtypedata.Key });

                foreach (var lang in languages)
                {
                    AdditionalPoiInfos mypoiinfo = new AdditionalPoiInfos
                    {
                        Language = lang,
                        MainType = maintypedata?.TypeDesc?[lang],
                        SubType = subtypedata?.TypeDesc?[lang],
                        Categories = validtags.Select(x => x.TagName[lang]).ToList()
                    };

                    myadditionalpoinfosdict.Add(lang, mypoiinfo);
                }
            }

            return myadditionalpoinfosdict;
        }        
    }

}
