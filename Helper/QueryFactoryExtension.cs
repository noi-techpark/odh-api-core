using DataModel;
using Newtonsoft.Json;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    public static class QueryFactoryExtension
    {
        public static async Task<T> GetObjectSingleAsync<T>(this Query query, CancellationToken cancellationToken)
        {
            var result = await query.FirstOrDefaultAsync<JsonRaw>();
            return JsonConvert.DeserializeObject<T>(result.Value);
        }

        public static async Task<IEnumerable<T>> GetObjectListAsync<T>(this Query query, CancellationToken cancellationToken)
        {
            var result = await query.GetAsync<JsonRaw>();            
            return result.Select(x => JsonConvert.DeserializeObject<T>(x.Value));
        }

    }
}
