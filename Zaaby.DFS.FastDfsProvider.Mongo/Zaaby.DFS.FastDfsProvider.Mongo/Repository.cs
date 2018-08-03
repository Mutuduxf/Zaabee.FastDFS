using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Zaaby.DFS.Core;

namespace Zaaby.DFS.FastDfsProvider.Mongo
{
    public class Repository : IRepository
    {
        private MongoCollectionSettings _collectionSettings;

        private IMongoDatabase MongoDatabase { get; }

        private IMongoClient MongoClient { get; }

        private MongoCollectionSettings CollectionSettings
            => _collectionSettings ?? (_collectionSettings = new MongoCollectionSettings
            {
                AssignIdOnInsert = true,
                GuidRepresentation = GuidRepresentation.CSharpLegacy,
                ReadPreference = ReadPreference.Primary,
                WriteConcern = WriteConcern.WMajority,
                ReadConcern = ReadConcern.Default
            });

        public Repository(MongoDbConfiger configer)
        {
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            ConventionRegistry.Register("IgnoreExtraElements",
                new ConventionPack {new IgnoreExtraElementsConvention(true)}, type => true);
            MongoClient = new MongoPool().GetClient(configer);
            MongoDatabase = MongoClient.GetDatabase(configer.Database);
        }

        public void Add(FileDfsInfo fileDfsInfo)
        {
            MongoDatabase.GetCollection<FileDfsInfo>(typeof(FileDfsInfo).Name, CollectionSettings)
                .InsertOne(fileDfsInfo);
        }

        public void DeleteByDfsFileName(string dfsFileName)
        {
            MongoDatabase.GetCollection<FileDfsInfo>(typeof(FileDfsInfo).Name, CollectionSettings)
                .DeleteOne(p => p.DfsFileName == dfsFileName);
        }
    }
}