using System.Collections.Generic;
using System.IO;
using System.Net;
using Xunit;
using Zaabee.FastDfsClient;
using Zaabee.FastDfsClient.Common;

namespace UnitTest
{
    public class FastDfsClientUnitTest
    {
        private readonly FastDfsClient _client;

        public FastDfsClientUnitTest()
        {
            _client = new FastDfsClient(new List<IPEndPoint>
            {
                new IPEndPoint(IPAddress.Parse("192.168.78.152"), 22122)
            });
        }

        [Theory]
        [InlineData("group1")]
        public void GetStorageNode(string groupName)
        {
            var storageNode = FastDfsClient.GetStorageNode(groupName);
            var endPoint = storageNode.EndPoint;
            var nodeGroupName = storageNode.GroupName;
            Assert.Contains(endPoint.Address.ToString(),
                new List<string> {"192.168.78.153", "192.168.78.154", "192.168.78.155"});
            Assert.Equal(nodeGroupName, groupName);
        }

        [Theory]
        [InlineData("group1", "1.jpg", "jpg")]
        [InlineData("group1", "3.gif", "gif")]
        [InlineData("group1", "A000091712080DOF001.pdf", "pdf")]
        public void FastDfsOperation(string groupName, string filePath, string fileExt)
        {
            var storageNode = FastDfsClient.GetStorageNode(groupName);
            var uploadFile = File.ReadAllBytes(filePath);

            var fileName = _client.UploadFile(storageNode, uploadFile, fileExt);

            var downloadFile = _client.DownloadFile(storageNode, fileName);

            _client.RemoveFile(groupName, fileName);

            Assert.Equal(Md5Helper.Get32Md5(uploadFile), Md5Helper.Get32Md5(downloadFile));
        }

        [Theory]
        [InlineData("group1", "M00/00/00/ASDFSADFASDFSADF.gif")]
        [InlineData("group1", "M00/00/00/YUILYUILUYILUYUL.gif")]
        [InlineData("group1", "M00/00/00/REWQDSADSAFSD.gif")]
        public void DownloadNotExistFile(string groupName, string fileName)
        {
            var storageNode = FastDfsClient.GetStorageNode(groupName);
            Assert.Null(_client.DownloadFile(storageNode, fileName));
        }

        [Theory]
        [InlineData("group4")]
        public void GetNotExistNode(string groupName)
        {
            Assert.Throws<FdfsException>(() => { FastDfsClient.GetStorageNode(groupName); });
        }

        [Theory]
        [InlineData("group1", "M00/00/00/ASDFSADFASDFSADF.gif")]
        [InlineData("group1", "M00/00/00/YUILYUILUYILUYUL.gif")]
        [InlineData("group1", "M00/00/00/REWQDSADSAFSD.gif")]
        public void RemoveNotExistFile(string groupName, string fileName)
        {
            _client.RemoveFile(groupName, fileName);
        }
    }
}