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
        public static async Task<Dictionary<string, AdditionalPoiInfos>> GetAdditionalTypeInfoForPoi(QueryFactory QueryFactory, string subtype, List<string>? languages)
        {
            if(languages == null)
                languages = new List<string>() { "de","it","en"};

            Dictionary<string, AdditionalPoiInfos> myadditionalpoinfosdict = new Dictionary<string, AdditionalPoiInfos>();

            //Get SuedtirolType Subtype
            var subtypequery = QueryFactory.Query("smgpoitypes")
                        .Select("data")
                        .Where("id", subtype.ToLower());
                         //.WhereRaw("data->>'Key' LIKE ?", subtype);
            var subtypedata =
                await subtypequery
                    .GetFirstOrDefaultAsObject<SuedtirolType>();

            if (subtypedata != null)
            {
                var maintypequery = QueryFactory.Query("smgpoitypes")
                            .Select("data")
                            .Where("id", subtypedata.TypeParent.ToLower());
                //.WhereRaw("data->>'Key' LIKE ?",  subtypedata.TypeParent);
                var maintypedata =
                    await maintypequery
                        .GetFirstOrDefaultAsObject<SuedtirolType>();

                var validtags = await ODHTagHelper.GetSmgTagsValidforTranslations(QueryFactory, new List<string>(), new List<string>() { maintypedata.Key, subtypedata.Key });

                foreach (var lang in languages)
                {
                    AdditionalPoiInfos mypoiinfo = new AdditionalPoiInfos();
                    mypoiinfo.Language = lang;
                    mypoiinfo.MainType = maintypedata.TypeNames[lang];
                    mypoiinfo.SubType = subtypedata.TypeNames[lang];
                    mypoiinfo.Categories = validtags.Select(x => x.TagName[lang]).ToList();

                    myadditionalpoinfosdict.Add(lang, mypoiinfo);
                }
            }

            return myadditionalpoinfosdict;
        }        
    }

    public class ODHTagHelper
    {
        public static async Task<IEnumerable<SmgTags>> GetSmgTagsValidforTranslations(QueryFactory QueryFactory, List<string> validforentity, List<string> idlist = null)
        {
            try
            {
                List<SmgTags> validtags = new List<SmgTags>();

                var validtagquery = QueryFactory.Query("suedtiroltypes")
                        .Select("data")
                        .When(validforentity.Count > 0, q => q.WhereInJsonb(
                            validforentity,
                            tag => new { ValidForEntity = new[] { tag } }
                        ))
                        .When(idlist != null, w => w.WhereIn("id", idlist))
                        .WhereRaw("data->'DisplayAsCategory' = true");                        

                var validtagdata =
                    await validtagquery
                        .GetAllAsObject<SmgTags>();

                return validtagdata;
            }
            catch (Exception ex)
            {
                return new List<SmgTags>();
            }
        }
    }
}
