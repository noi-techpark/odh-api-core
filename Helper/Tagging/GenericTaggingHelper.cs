﻿using DataModel;
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
                    ((ODHActivityPoiLinked)mypgdata).Tags = GenerateNewTags(((ODHActivityPoiLinked)mypgdata).SmgTags ?? new List<string>(), myalltaglist);
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
        public static async Task<List<TagLinked>> GetAllGenericTagsfromJson(string jsondir)
        {
            using (StreamReader r = new StreamReader(Path.Combine(jsondir, $"GenericTags.json")))
            {
                string json = await r.ReadToEndAsync();

                return JsonConvert.DeserializeObject<List<TagLinked>>(json) ?? new();
            }
        }

        //Translates OLD Tags with german keys to new English Tags
        public static List<Tags> GenerateNewTags(ICollection<string> currenttags, List<TagLinked> alltaglist)
        {
            var returnDict = new List<Tags>();

            foreach (var tag in currenttags)
            {
                var resultdict = TranslateMappingKey(tag, alltaglist);

                foreach (var kvp in resultdict)
                {
                    var listtoadd = new List<Tags>();

                    //if (returnDict.ContainsKey(kvp.Key))
                    //    listtoadd = returnDict[kvp.Key];

                    var tagtoadd = new Tags() { Id = kvp.Value, Source = kvp.Key };

                    if (!listtoadd.Select(x => x.Id).Contains(tagtoadd.Id))
                        listtoadd.Add(tagtoadd);

                    //returnDict.TryAddOrUpdate(kvp.Key, listtoadd);
                    returnDict.AddRange(listtoadd);
                }
            }

            return returnDict;
        }

        private static IDictionary<string, string> TranslateMappingKey(string germankey, List<TagLinked> alltaglist)
        {
            var returnDict = new Dictionary<string, string>();

            var tagen = alltaglist.Where(x => x.ODHTagIds.Any(y => y == germankey)).FirstOrDefault();

            if (tagen?.Id != null)
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

        //GETS all generic tags from json as object to avoid DB call on each Tag update
        public static async Task<List<AllowedTags>> GetAllAutoPublishTagsfromJson(string jsondir)
        {
            using (StreamReader r = new StreamReader(Path.Combine(jsondir, $"AutoPublishTags.json")))
            {
                string json = await r.ReadToEndAsync();

                return JsonConvert.DeserializeObject<List<AllowedTags>>(json) ?? new();
            }
        }



        //private static string RemoveSpecialCharsSimple(string id)
        //{
        //    var toreturn = id;

        //    //Change special chars hack
        //    toreturn = toreturn.Replace(" /", "");
        //    toreturn = toreturn.Replace("/ ", "");
        //    toreturn = toreturn.Replace("/", " ");

        //    toreturn = toreturn.Replace(" &", "");
        //    toreturn = toreturn.Replace("& ", "");
        //    toreturn = toreturn.Replace("&", "");

        //    //Exceptions where german key was not used, activity/gastronomy/tourismassociation
        //    toreturn = toreturn.Replace("aktivität", "activity");
        //    toreturn = toreturn.Replace("restaurant", "gastronomy");
        //    toreturn = toreturn.Replace("tourismusverein", "tourismassociation");


        //    return toreturn.Trim();
        //}

    }
}
