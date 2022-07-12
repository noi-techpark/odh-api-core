using DataModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OdhApiCore.GenericHelpers
{
    public class GenericTaggingHelper
    {
        public async Task<List<ODHTagLinked>> GetAllGenericTagsfromJson(string jsondir)
        {
            using (StreamReader r = new StreamReader(Path.Combine(jsondir, $"GenericTags.json")))
            {
                string json = await r.ReadToEndAsync();

                return JsonConvert.DeserializeObject<List<ODHTagLinked>>(json);
            }
        }
        
        //TODO: Add all logic with the new tags here
    }
}
