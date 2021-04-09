using MongoDB.Driver;
using Zaabee.FastDfsProvider.Repository.Abstractions;

namespace Zaabee.FastDfsProvider.Mongo
{
    public class Repository : IRepository
    {
        public IMongoClient MongoClient { get; }
        public IMongoDatabase MongoDatabase { get; }

        public Repository(IMongoClient mongoClient, string database)
        {
            MongoClient = mongoClient;
            MongoDatabase = MongoClient.GetDatabase(database);
        }

        public void Add(FileDfsInfo fileDfsInfo)
        {
            MongoDatabase.GetCollection<FileDfsInfo>(nameof(FileDfsInfo))
                .InsertOne(fileDfsInfo);
        }

        public void DeleteByDfsFileName(string dfsFileName)
        {
            MongoDatabase.GetCollection<FileDfsInfo>(nameof(FileDfsInfo))
                .DeleteOne(p => p.DfsFileName == dfsFileName);
        }
    }
}