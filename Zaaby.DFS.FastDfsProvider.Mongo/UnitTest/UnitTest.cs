using System;
using System.Collections.Generic;
using Xunit;
using Zaaby.DFS.Core;
using Zaaby.DFS.FastDfsProvider.Mongo;

namespace UnitTest
{
    public class UnitTest
    {
        private readonly Repository _repository;

        public UnitTest()
        {
            _repository =
                new Repository(new MongoDbConfiger(new List<string> {"127.0.0.1:27017"}, "FastDfs", "admin", "pwd"));
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