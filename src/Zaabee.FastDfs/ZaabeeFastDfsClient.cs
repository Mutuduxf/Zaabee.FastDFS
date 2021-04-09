using System.Collections.Generic;
using System.Net;
using Zaabee.FastDfs.Common;
using Zaabee.FastDfs.Storage;
using Zaabee.FastDfs.Tracker;

namespace Zaabee.FastDfs
{
    /// <summary>
    /// FastDFSClient
    /// </summary>
    public class ZaabeeFastDfsClient
    {
        public ZaabeeFastDfsClient(List<IPEndPoint> trackers, int connectionTimeout = 30)
        {
            ConnectionManager.Initialize(trackers, connectionTimeout);
        }

        /// <summary>
        /// 获取存储节点
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns>存储节点实体类</returns>
        public StorageNode GetStorageNode(string groupName)
        {
            var trackerRequest = QueryStoreWithGroupOne.Instance.GetRequest(groupName);

            var trackerResponse = new QueryStoreWithGroupOne.Response(trackerRequest.GetResponse());

            var storeEndPoint = new IPEndPoint(IPAddress.Parse(trackerResponse.IpStr), trackerResponse.Port);

            return new StorageNode
            {
                GroupName = trackerResponse.GroupName,
                EndPoint = storeEndPoint,
                StorePathIndex = trackerResponse.StorePathIndex
            };
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="contentByte">文件内容</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <returns>文件名</returns>
        public string UploadFile(StorageNode storageNode, byte[] contentByte, string fileExt)
        {
            var storageRequest = Storage.UploadFile.Instance.GetRequest(storageNode.EndPoint,
                storageNode.StorePathIndex, contentByte.Length, fileExt, contentByte);

            var storageResponse = new UploadFile.Response(storageRequest.GetResponse());

            return storageResponse.FileName;
        }

        /// <summary>
        /// 上传从文件
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="contentByte">文件内容</param>
        /// <param name="masterFilename">主文件名</param>
        /// <param name="prefixName">从文件后缀</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <returns>文件名</returns>
        public string UploadSlaveFile(string groupName, byte[] contentByte, string masterFilename,
            string prefixName, string fileExt)
        {
            var trackerRequest = QueryUpdate.Instance.GetRequest(groupName, masterFilename);

            var trackerResponse = new QueryUpdate.Response(trackerRequest.GetResponse());

            var storeEndPoint = new IPEndPoint(IPAddress.Parse(trackerResponse.IpStr), trackerResponse.Port);

            var storageRequest = Storage.UploadSlaveFile.Instance.GetRequest(storeEndPoint,
                contentByte.Length, masterFilename, prefixName, fileExt, contentByte);

            var storageResponse = new UploadFile.Response(storageRequest.GetResponse());

            return storageResponse.FileName;
        }

        /// <summary>
        /// 上传可以Append的文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="contentByte">文件内容</param>
        /// <param name="fileExt">文件扩展名(注意:不包含".")</param>
        /// <returns>文件名</returns>
        public string UploadAppenderFile(StorageNode storageNode, byte[] contentByte, string fileExt)
        {
            var storageRequest = UploadAppendFile.Instance.GetRequest(storageNode.EndPoint, storageNode.StorePathIndex,
                contentByte.Length, fileExt, contentByte);

            var storageResponse = new UploadAppendFile.Response(storageRequest.GetResponse());

            return storageResponse.FileName;
        }

        /// <summary>
        /// 附加文件
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileName">文件名</param>
        /// <param name="contentByte">文件内容</param>
        public void AppendFile(string groupName, string fileName, byte[] contentByte)
        {
            var trackerRequest = QueryUpdate.Instance.GetRequest(groupName, fileName);

            var trackerResponse = new QueryUpdate.Response(trackerRequest.GetResponse());

            var storeEndPoint = new IPEndPoint(IPAddress.Parse(trackerResponse.IpStr), trackerResponse.Port);

            var storageRequest = Storage.AppendFile.Instance.GetRequest(storeEndPoint, fileName, contentByte);

            storageRequest.GetResponse();
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="fileName">文件名</param>
        public void RemoveFile(string groupName, string fileName)
        {
            try
            {
                var trackerRequest = QueryUpdate.Instance.GetRequest(groupName, fileName);
                var trackerResponse = new QueryUpdate.Response(trackerRequest.GetResponse());
                var storeEndPoint = new IPEndPoint(IPAddress.Parse(trackerResponse.IpStr), trackerResponse.Port);
                var storageRequest = DeleteFile.Instance.GetRequest(storeEndPoint, groupName, fileName);
                storageRequest.GetResponse();
            }
            catch (FdfsException)
            {
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileName">文件名</param>
        /// <returns>文件内容</returns>
        public byte[] DownloadFile(StorageNode storageNode, string fileName)
        {
            try
            {
                var storageRequest = Storage.DownloadFile.Instance.GetRequest(storageNode.EndPoint, 0L, 0L,
                    storageNode.GroupName, fileName);

                var storageResponse = new DownloadFile.Response(storageRequest.GetResponse());

                return storageResponse.Content;
            }
            catch (FdfsException)
            {
                return null;
            }
        }

        /// <summary>
        /// 增量下载文件
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileName">文件名</param>
        /// <param name="offset">从文件起始点的偏移量</param>
        /// <param name="length">要读取的字节数</param>
        /// <returns>文件内容</returns>
        public byte[] DownloadFile(StorageNode storageNode, string fileName, long offset, long length)
        {
            var storageRequest = Storage.DownloadFile.Instance.GetRequest(storageNode.EndPoint, offset,
                length, storageNode.GroupName, fileName);

            var storageResponse = new DownloadFile.Response(storageRequest.GetResponse());

            return storageResponse.Content;
        }

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="storageNode">GetStorageNode方法返回的存储节点</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public FdfsFileInfo GetFileInfo(StorageNode storageNode, string fileName)
        {
            var storageRequest =
                QueryFileInfo.Instance.GetRequest(storageNode.EndPoint, storageNode.GroupName, fileName);

            var result = new FdfsFileInfo(storageRequest.GetResponse());

            return result;
        }
    }
}