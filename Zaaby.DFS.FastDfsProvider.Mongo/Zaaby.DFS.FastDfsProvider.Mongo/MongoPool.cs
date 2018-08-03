using System;
using System.Collections.Concurrent;
using System.Linq;
using MongoDB.Driver;

namespace Zaaby.DFS.FastDfsProvider.Mongo
{
    internal class MongoPool
    {
        /// <summary>
        /// Client Dictionary
        /// </summary>
        private static readonly ConcurrentDictionary<string, IMongoClient> Clients =
            new ConcurrentDictionary<string, IMongoClient>();

        private static readonly object LockObj = new object();
        
        public static IMongoClient GetClient(string connStr)
        {
            lock (LockObj)
            {
                if (Clients.ContainsKey(connStr))
                    return Clients[connStr];
            }
            lock (LockObj)
            {
                return Clients.ContainsKey(connStr)
                    ? Clients[connStr]
                    : Clients.GetOrAdd(connStr, key => new MongoClient(connStr));
            }
        }

        public IMongoClient GetClient(MongoDbConfiger configer)
        {
            var connStr = configer.GetConnectionString();
            lock (LockObj)
            {
                if (Clients.ContainsKey(connStr))
                    return Clients[connStr];
            }

            lock (LockObj)
            {
                if (Clients.ContainsKey(connStr))
                    return Clients[connStr];
                var settings = new MongoClientSettings
                {
                    Servers = configer.Hosts.Select(p =>
                    {
                        var strs = p.Split(':');
                        var ip = strs[0];
                        var port = string.IsNullOrWhiteSpace(strs[1]) ? 27017 : Convert.ToInt32(strs[1]);
                        return new MongoServerAddress(ip, port);
                    }),
                    ReadPreference = configer.ReadPreference != null
                        ? new ReadPreference(ConvertReadPreference(configer.ReadPreference.Value))
                        : new ReadPreference(ReadPreferenceMode.Primary),
                    Credential =
                        MongoCredential.CreateCredential(configer.Database, configer.UserName, configer.Password),
                    ConnectTimeout = TimeSpan.FromSeconds(300),
                    SocketTimeout = TimeSpan.FromSeconds(300),
                    MaxConnectionLifeTime = TimeSpan.FromSeconds(300),
                    MinConnectionPoolSize = 100,
                    ConnectionMode = ConnectionMode.Automatic
                };
                return Clients.GetOrAdd(connStr, key => new MongoClient(settings));
            }
        }

        private ReadPreferenceMode ConvertReadPreference(MongoDbReadPreference readPreference)
        {
            switch (readPreference)
            {
                case MongoDbReadPreference.Primary:
                    return ReadPreferenceMode.Primary;
                case MongoDbReadPreference.PrimaryPreferred:
                    return ReadPreferenceMode.PrimaryPreferred;
                case MongoDbReadPreference.Secondary:
                    return ReadPreferenceMode.Secondary;
                case MongoDbReadPreference.SecondaryPreferred:
                    return ReadPreferenceMode.SecondaryPreferred;
                case MongoDbReadPreference.Nearest:
                    return ReadPreferenceMode.Nearest;
                default:
                    throw new ArgumentOutOfRangeException(nameof(readPreference), readPreference, null);
            }
        }
    }
}