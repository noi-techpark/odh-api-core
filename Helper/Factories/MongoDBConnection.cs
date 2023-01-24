using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;

namespace Helper.Factories
{
    public class MongoDBConnection : IDisposable
    {       
        public MongoDBConnection(ISettings settings) //, ILogger<QueryFactory> logger)
        {
            if(!String.IsNullOrEmpty(settings.MongoDBConnectionString))
                mongoDBClient = new MongoClient(settings.MongoDBConnectionString);            
            //Logger = info => logger.LogDebug("SQL: {sql} {@parameters}", info.RawSql, info.NamedBindings);
        }

        public MongoClient mongoDBClient;        

        public new void Dispose()
        {            
            if (mongoDBClient != null)
            {
                mongoDBClient.Cluster.Dispose();
                mongoDBClient = null;
            }
        }
    }
}
