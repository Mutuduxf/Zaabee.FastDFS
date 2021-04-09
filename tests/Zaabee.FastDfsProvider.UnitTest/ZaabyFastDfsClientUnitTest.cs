using System.Collections.Generic;
using System.IO;
using System.Net;
using Moq;
using Xunit;
using Zaabee.FastDfsProvider.Repository.Abstractions;

namespace Zaabee.FastDfsProvider.UnitTest
{
    public class ZaabyFastDfsClientUnitTest
    {
        private readonly ZaabyFastDfsClient _client;

        public ZaabyFastDfsClientUnitTest()
        {
            var mock = new Mock<IRepository>();
            mock.Setup(p => p.Add(It.IsAny<FileDfsInfo>()));
            mock.Setup(p => p.DeleteByDfsFileName(It.IsAny<string>()));
            var repository = mock.Object;

            _client = new ZaabyFastDfsClient(
                new List<IPEndPoint> {new IPEndPoint(IPAddress.Parse("192.168.78.152"), 22122)},
                "group1", repository);
        }

        [Theory]
        [InlineData("1.jpg")]
        [InlineData("3.gif")]
        [InlineData("A000091712080DOF001.pdf")]
        public void FastDfsOperation(string fileName)
        {
            var uploadFile = File.ReadAllBytes(fileName);

            var dfsfileName = _client.UploadFile(uploadFile, fileName);

            var downloadFile = _client.DownloadFile(dfsfileName);

            _client.RemoveFile(dfsfileName);

            Assert.Equal(Md5Helper.Get32Md5(uploadFile), Md5Helper.Get32Md5(downloadFile));
        }
    }
}