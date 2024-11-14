// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper.Location;
using SqlKata.Execution;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Tagging
{
    public static class FillTagsObject
    {
        /// <summary>
        /// Extension Method to update the Tags
        /// </summary>
        /// <param name="queryFactory"></param>
        /// <returns></returns>
        public static async Task UpdateTagsExtension<T>(this T data, QueryFactory queryFactory) where T : IHasTagInfo
        {
            data.Tags = await UpdateTags(data.TagIds, queryFactory);
        }

        private static async Task<ICollection<Tags>> UpdateTags(ICollection<string> tagIds, QueryFactory queryFactory)
        {
            ICollection<Tags> tags = new HashSet<Tags>();

            if(tagIds != null && tagIds.Count > 0)
            {
                //Load Tags from DB
                var query =
                 queryFactory.Query("tags")
                     .Select("data")
                     .WhereIn("id", tagIds);

                var assignedtags = await query.GetObjectListAsync<TagLinked>();

                //Create Tags object
                foreach (var tag in assignedtags)
                {
                    tags.Add(new Tags() { Id = tag.Id, Source = tag.Source, Type = GetTypeFromTagTypes(tag.Types), Name = GetTagName(tag.TagName) });
                }                
            }

            return tags;
        }

        //TODO REFINE
        public static string? GetTypeFromTagTypes(ICollection<string> tagtypes)
        {
            if(tagtypes == null || tagtypes.Count == 0)
                return null;
            else
            {
                if(tagtypes.Count == 1)
                {
                    return tagtypes.FirstOrDefault();
                }
                else
                {
                    if (tagtypes.Contains("ltscategory"))
                        return "ltscategory";
                    else
                        return tagtypes.FirstOrDefault();
                }
            }
        }

        public static string? GetTagName(IDictionary<string,string> tagnames)
        {
            if (tagnames == null || tagnames.Count == 0)
                return null;
            else
            {
                if (tagnames.ContainsKey("en"))
                {
                    return tagnames["en"];
                }
                else
                {
                    return tagnames.Values.Where(x => x != null).FirstOrDefault();
                }
            }
        }
    }
}
