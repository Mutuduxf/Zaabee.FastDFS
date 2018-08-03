using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Zaabee.FastDfsClient;
using Zaaby.DFS.Core;

namespace Zaaby.DFS.FastDfsProvider
{
    public class ZaabyFastDfsClient : IHandler
    {
        private readonly FastDfsClient _fastDfsClient;
        private readonly string _groupName;
        private readonly IRepository _repository;

        public ZaabyFastDfsClient(List<IPEndPoint> trackers, string groupName, IRepository repository = null)
        {
            _fastDfsClient = new FastDfsClient(trackers);
            _groupName = groupName;
            _repository = repository;
        }

        public string UploadFile(byte[] fileBytes, string fileName)
        {
            var strArr = fileName.Split('.');
            if (strArr.Length < 2)
                throw new ArgumentException(nameof(fileName));
            var fileNameBuilder = new StringBuilder();
            for (var i = 0; i < strArr.Length - 1; i++)
                fileNameBuilder.Append(strArr[i]);
            var file = fileNameBuilder.ToString();
            var fileExt = strArr.Last();
            if (string.IsNullOrWhiteSpace(file) || string.IsNullOrWhiteSpace(fileExt))
                throw new ArgumentException(nameof(fileName));

            var storageNode = _fastDfsClient.GetStorageNode(_groupName);
            var fileDfsInfo = new FileDfsInfo
            {
                Id = Guid.NewGuid(),
                DfsFileName = _fastDfsClient.UploadFile(storageNode, fileBytes, fileExt),
                FileName = fileName,
                UtcCreateTime = DateTime.Now
            };
            _repository?.Add(fileDfsInfo);
            return fileDfsInfo.DfsFileName;
        }

        public byte[] DownloadFile(string dfsFileName)
        {
            return _fastDfsClient.DownloadFile(_fastDfsClient.GetStorageNode(_groupName), dfsFileName);
        }

        public void RemoveFile(string dfsFileName)
        {
            _repository?.DeleteByDfsFileName(dfsFileName);
            _fastDfsClient.RemoveFile(_groupName, dfsFileName);
        }
    }
}