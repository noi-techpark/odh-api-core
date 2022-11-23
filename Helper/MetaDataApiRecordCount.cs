using DataModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
