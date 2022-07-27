using DataModel;
using Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helper
{
    public class GenericTaggingHelper
    {
        public static async Task AddMappingToODHActivityPoi(IIdentifiable mypgdata, string jsondir)
        {
            try
            {
                //Special get all Taglist and traduce it on import
                var myalltaglist = await GetAllGenericTagsfromJson(jsondir);
                if (myalltaglist != null && ((ODHActivityPoiLinked)mypgdata).SmgTags != null)
                    ((ODHActivityPoiLinked)mypgdata).Tagging = GenerateNewTagging(((ODHActivityPoiLinked)mypgdata).SmgTags, myalltaglist);
            }
            catch(Exception ex)
            {                
                Console.WriteLine(JsonConvert.SerializeObject(new UpdateResult
                {
                    operation = "Tagging object creation",
                    updatetype = "single",
                    otherinfo = "",
                    id = mypgdata.Id,
                    message = "Tagging conversion failed: " + ex.Message,
                    recordsmodified = 0,
                    created = 0,
                    updated = 0,
                    deleted = 0,
                    success = false
                }));
            }
        }


        //GETS all generic tags from json as object to avoid DB call on each Tag update
        public static async Task<List<ODHTagLinked>> GetAllGenericTagsfromJson(string jsondir)
        {
            using (StreamReader r = new StreamReader(Path.Combine(jsondir, $"GenericTags.json")))
            {
                string json = await r.ReadToEndAsync();

                return JsonConvert.DeserializeObject<List<ODHTagLinked>>(json);
            }
        }

        //Translates OLD Tags with german keys to new English Tags
        public static IDictionary<string, List<Tagging>> GenerateNewTagging(ICollection<string> currenttags, List<ODHTagLinked> alltaglist)
        {
            var returnDict = new Dictionary<string, List<Tagging>>();

            foreach (var tag in currenttags)
            {
                var resultdict = TranslateMappingKey(tag, alltaglist);

                foreach (var kvp in resultdict)
                {
                    var listtoadd = new List<Tagging>();

                    if (returnDict.ContainsKey(kvp.Key))
                        listtoadd = returnDict[kvp.Key];

                    listtoadd.Add(new Tagging() { Id = kvp.Value, Source = kvp.Key });

                    returnDict.TryAddOrUpdate(kvp.Key, listtoadd);
                }
            }

            return returnDict;
        }

        private static IDictionary<string, string> TranslateMappingKey(string germankey, List<ODHTagLinked> alltaglist)
        {
            var returnDict = new Dictionary<string, string>();

            var tagen = alltaglist.Where(x => RemoveSpecialCharsSimple(x.TagName["de"].ToLower()) == germankey).FirstOrDefault();

            if (tagen != null)
            {
                if (tagen.Source.Contains("ODHCategory"))
                {
                    returnDict.Add("idm", tagen.Id);
                }

                if (tagen.Source.Contains("LTSCategory"))
                {
                    returnDict.Add("lts", tagen.Id);
                }
            }

            return returnDict;
        }

        //Removes all special chars for the tag id
        private static string RemoveSpecialCharsRegex(string id)
        {
            var toreturn = id;

            //Change special chars hack
            toreturn = toreturn.Replace("é", "e");
            toreturn = toreturn.Replace("á", "a");
            toreturn = toreturn.Replace("í", "i");
            toreturn = toreturn.Replace("ó", "o");
            toreturn = toreturn.Replace("ú", "u");

            toreturn = toreturn.Replace("ä", "a");
            toreturn = toreturn.Replace("ö", "o");
            toreturn = toreturn.Replace("ü", "u");

            //Exclude all characters that does not match this pattern
            toreturn = Regex.Replace(toreturn, @"[^0-9a-zA-Z_ ]+", "");
            //Hack replace all double spaces
            toreturn = Regex.Replace(toreturn, @"  +", " ");

            return toreturn.Trim();
        }

        private static string RemoveSpecialCharsSimple(string id)
        {
            var toreturn = id;

            //Change special chars hack
            toreturn = toreturn.Replace(" /", "");
            toreturn = toreturn.Replace("/ ", "");
            toreturn = toreturn.Replace("/", "");

            toreturn = toreturn.Replace(" &", "");
            toreturn = toreturn.Replace("& ", "");
            toreturn = toreturn.Replace("&", "");

            return toreturn.Trim();
        }

    }
}
