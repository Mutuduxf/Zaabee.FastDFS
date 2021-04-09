using System;
using MongoDB.Driver;
using Xunit;
using Zaabee.FastDfsProvider.Repository.Abstractions;

namespace Zaabee.FastDfsProvider.Mongo.UnitTest
{
    public class UnitTest
    {
        private readonly Repository _repository;

        public UnitTest()
        {
            _repository =
                new Repository(
                    new MongoClient(
                        "mongodb://admin:123@192.168.78.140:27017,192.168.78.141:27017,192.168.78.142/admin?authSource=admin&replicaSet=rs"),
                    "FastDFS");
        }

        [Fact]
        public void Test()
        {
            var fileDfsInfo = new FileDfsInfo
            {
                Id = Guid.NewGuid(),
                DfsFileName = "/M00/00/00/wKgBa1Vd7AyABRFaAAbVsieNsu4588_big.txt",
                FileName = "Test.txt",
                UtcCreateTime = DateTime.UtcNow
            };
            _repository.Add(fileDfsInfo);
            _repository.DeleteByDfsFileName(fileDfsInfo.DfsFileName);
        }
    }
}