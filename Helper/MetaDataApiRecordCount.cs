// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class MetaDataApiRecordCount
    {
        public static async Task<int> GetApiRecordCount(string url, string filter, string bearertoken)
        {
            int recordcount = 0;
            
            using (var client = new HttpClient())
            {
                if(!String.IsNullOrEmpty(bearertoken))
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearertoken);

                var requesturl = url;
                if (!String.IsNullOrEmpty(filter))
                    requesturl = requesturl + filter + "&pagenumber=1&pagesize=1";
                else
                    requesturl = requesturl + "?pagenumber=1&pagesize=1";

                var response = await client.GetAsync(url + "?pagenumber=1&pagesize=1");
                
                var responsecontent = await response.Content.ReadAsStringAsync();

                var myresponseparsed = JsonConvert.DeserializeObject<dynamic>(responsecontent);

                recordcount = myresponseparsed != null && myresponseparsed is Newtonsoft.Json.Linq.JObject && myresponseparsed.TotalResults != null ? Convert.ToInt32(myresponseparsed.TotalResults) : 0;
            }

            return recordcount;
        }

        public static async Task<IDictionary<string,int>> GetRecordCountfromDB(ICollection<string>? filters, string odhtype, QueryFactory QueryFactory)
        {
            var result = new Dictionary<string, int> { };

            try
            {
                string table = ODHTypeHelper.TranslateTypeString2Table(odhtype);

                string source = "";
                string tag = "";

                
                if (filters != null)
                {
                    foreach (var filter in filters)
                    {
                        if (filter.StartsWith("source="))
                        {
                            source = filter.Replace("source=", "");
                        }

                        if (filter.StartsWith("tagfilter="))
                        {
                            tag = filter.Replace("tagfilter=", "");
                        }
                    }
                        
                }

                //Get Reduced
                var reducedcount = await QueryFactory.Query()
                    .From(table)
                    .Where("gen_reduced", true)
                    .Where("gen_licenseinfo_closeddata", false)
                    .When(!String.IsNullOrEmpty(source), q => q.Where("gen_source", source))
                    .When(!String.IsNullOrEmpty(tag), q => q.WhereArrayInListOr(new List<string>() { tag }, "gen_tags"))
                    .CountAsync<int>();

                //Get Closed
                var closedcount = await QueryFactory.Query()
                    .From(table)
                    .Where("gen_licenseinfo_closeddata", true)
                    .When(!String.IsNullOrEmpty(source), q => q.Where("gen_source", source))
                    .When(!String.IsNullOrEmpty(tag), q => q.WhereArrayInListOr(new List<string>() { tag }, "gen_tags"))
                    .CountAsync<int>();

                //Get Open
                var opencount = await QueryFactory.Query()
                    .From(table)
                    .Where("gen_licenseinfo_closeddata", false)
                    .Where("gen_reduced", false)
                    .When(!String.IsNullOrEmpty(source), q => q.Where("gen_source", source))
                    .When(!String.IsNullOrEmpty(tag), q => q.WhereArrayInListOr(new List<string>() { tag }, "gen_tags"))
                    .CountAsync<int>();

                result.TryAddOrUpdate("reduced", reducedcount);
                result.TryAddOrUpdate("closed", closedcount);
                result.TryAddOrUpdate("open", opencount);

                return result;
            }
        
            catch(Exception ex)
            {
                return result;
            }
        }

        public static bool PropertyExists(dynamic obj, string name)
        {
            if (obj == null) return false;
            if (obj is Newtonsoft.Json.Linq.JArray) return false;
            if (obj is Newtonsoft.Json.Linq.JObject) return ((Newtonsoft.Json.Linq.JObject)obj).ContainsKey(name);
            if (obj is IDictionary<string, object> dict)
            {
                return dict.ContainsKey(name);
            }
            return obj.GetType().GetProperty(name) != null;
        }
    }    
}


