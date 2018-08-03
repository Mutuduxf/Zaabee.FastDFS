using System;
using System.Net;
using Zaabee.FastDfsClient.Common;

namespace Zaabee.FastDfsClient.Storage
{
    /// <summary>
    ///     query file info from storage server
    ///     Reqeust
    ///     Cmd: STORAGE_PROTO_CMD_QUERY_FILE_INFO 22
    ///     Body:
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ filename bytes: filename
    ///     Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body:
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file size
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file create timestamp
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file CRC32 signature
    /// </summary>
    internal class QueryFileInfo : FdfsRequest
    {
        private QueryFileInfo()
        {
        }

        public static QueryFileInfo Instance { get; } = new QueryFileInfo();

        /// <summary>
        /// </summary>
        /// <param name="paramList">
        ///     1,IPEndPoint    IPEndPoint-->the storage IPEndPoint
        ///     2,string fileName
        ///     3,string fileBytes
        /// </param>
        /// <returns></returns>
        public override FdfsRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length != 3)
                throw new FdfsException("param count is wrong");
            var endPoint = (IPEndPoint) paramList[0];

            var groupName = (string) paramList[1];
            var fileName = (string) paramList[2];

            var result = new QueryFileInfo {Connection = ConnectionManager.GetStorageConnection(endPoint)};

            if (groupName.Length > Consts.FdfsGroupNameMaxLen)
                throw new FdfsException("groupName is too long");

            long length = Consts.FdfsGroupNameMaxLen + fileName.Length;
            var bodyBuffer = new byte[length];
            var groupNameBuffer = Util.StringToByte(groupName);
            var fileNameBuffer = Util.StringToByte(fileName);

            Array.Copy(groupNameBuffer, 0, bodyBuffer, 0, groupNameBuffer.Length);
            Array.Copy(fileNameBuffer, 0, bodyBuffer, Consts.FdfsGroupNameMaxLen, fileNameBuffer.Length);

            result.Body = bodyBuffer;
            result.Header = new FdfsHeader(length, Consts.StorageProtoCmdQueryFileInfo, 0);
            return result;
        }
    }
}