using System;
using System.Collections.Generic;

namespace Zaaby.DFS.FastDfsProvider.Mongo
{
    /// <summary>
    /// mongodb configer
    /// </summary>
    public class MongoDbConfiger
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hosts"></param>
        /// <param name="database"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="readPreference"></param>
        public MongoDbConfiger(List<string> hosts, string database, string userName, string password,
            MongoDbReadPreference? readPreference = null)
        {
            Hosts = hosts;
            Database = database;
            UserName = userName;
            Password = password;
            ReadPreference = readPreference;
        }

        private List<string> _hosts;

        /// <summary>
        /// Mongo cluster
        /// </summary>
        public List<string> Hosts
        {
            get => _hosts ?? (_hosts = new List<string>());
            private set => _hosts = value;
        }
        
        public string Database { get; }
        
        public string UserName { get; }
        
        public string Password { get; }
        
        public MongoDbReadPreference? ReadPreference { get; }
        
        public string GetConnectionString()
        {
            return $"mongodb://{UserName}:{Password}@{string.Join(",", Hosts)}/{Database}{GetReadPreferenceStr()}";
        }
        
        private string GetReadPreferenceStr()
        {
            switch (ReadPreference)
            {
                case MongoDbReadPreference.Primary:
                    return "/?readPreference=primary";
                case MongoDbReadPreference.PrimaryPreferred:
                    return "/?readPreference=primaryPreferred";
                case MongoDbReadPreference.Secondary:
                    return "/?readPreference=secondary";
                case MongoDbReadPreference.SecondaryPreferred:
                    return "/?readPreference=secondaryPreferred";
                case MongoDbReadPreference.Nearest:
                    return "/?readPreference=mearest";
                case null:
                    return "";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}