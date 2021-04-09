using System;
using System.Net;
using Zaabee.FastDfs.Common;

namespace Zaabee.FastDfs.Storage
{
    /// <summary>
    /// download/fetch file from storage server
    /// Reqeust
    /// Cmd: STORAGE_PROTO_CMD_DOWNLOAD_FILE 14
    /// Body:
    /// @ FDFS_PROTO_PKG_LEN_SIZE bytes: file offset
    /// @ FDFS_PROTO_PKG_LEN_SIZE bytes: download file bytes
    /// @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    /// @ filename bytes: filename
    /// Response
    /// Cmd: STORAGE_PROTO_CMD_RESP
    /// Status: 0 right other wrong
    /// Body:
    /// @ file content
    /// </summary>
    internal class DownloadFile : FdfsRequest
    {
        private DownloadFile()
        {
        }

        public static DownloadFile Instance { get; } = new();

        /// <summary>
        /// </summary>
        /// <param name="paramList">
        /// 1,IPEndPoint    IPEndPoint-->the storage IPEndPoint
        /// 2,long offset-->file offset
        /// 3,long byteSize -->download file bytes
        /// 4,string groupName
        /// 5,string fileName
        /// </param>
        /// <returns></returns>
        public override FdfsRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length is not 5)
                throw new FdfsException("param count is wrong");
            var endPoint = (IPEndPoint) paramList[0];
            var offset = (long) paramList[1];
            var byteSize = (long) paramList[2];
            var groupName = (string) paramList[3];
            var fileName = (string) paramList[4];

            var result = new DownloadFile {Connection = ConnectionManager.GetStorageConnection(endPoint)};

            if (groupName.Length > Consts.FdfsGroupNameMaxLen)
                throw new FdfsException("groupName is too long");

            long length = Consts.FdfsProtoPkgLenSize +
                          Consts.FdfsProtoPkgLenSize +
                          Consts.FdfsGroupNameMaxLen +
                          fileName.Length;
            var bodyBuffer = new byte[length];
            var offsetBuffer = Util.LongToBuffer(offset);
            var byteSizeBuffer = Util.LongToBuffer(byteSize);
            var groupNameBuffer = Util.StringToByte(groupName);
            var fileNameBuffer = Util.StringToByte(fileName);
            Array.Copy(offsetBuffer, 0, bodyBuffer, 0, offsetBuffer.Length);
            Array.Copy(byteSizeBuffer, 0, bodyBuffer, Consts.FdfsProtoPkgLenSize, byteSizeBuffer.Length);
            Array.Copy(groupNameBuffer, 0, bodyBuffer, Consts.FdfsProtoPkgLenSize +
                                                       Consts.FdfsProtoPkgLenSize, groupNameBuffer.Length);
            Array.Copy(fileNameBuffer, 0, bodyBuffer, Consts.FdfsProtoPkgLenSize +
                                                      Consts.FdfsProtoPkgLenSize + Consts.FdfsGroupNameMaxLen,
                fileNameBuffer.Length);

            result.Body = bodyBuffer;
            result.Header = new FdfsHeader(length, Consts.StorageProtoCmdDownloadFile, 0);
            return result;
        }

        public class Response
        {
            public readonly byte[] Content;

            public Response(byte[] responseByte)
            {
                Content = responseByte;
            }
        }
    }
}