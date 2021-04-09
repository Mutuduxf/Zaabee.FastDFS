using System;
using System.Net;
using Zaabee.FastDfs.Common;

namespace Zaabee.FastDfs.Storage
{
    /// <summary>
    /// delete file from storage server
    /// Reqeust
    /// Cmd: STORAGE_PROTO_CMD_DELETE_FILE 12
    /// Body:
    /// @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    /// @ filename bytes: filename
    /// Response
    /// Cmd: STORAGE_PROTO_CMD_RESP
    /// Status: 0 right other wrong
    /// Body:
    /// </summary>
    internal class DeleteFile : FdfsRequest
    {
        private DeleteFile()
        {
        }

        public static DeleteFile Instance { get; } = new();

        /// <summary>
        /// </summary>
        /// <param name="paramList">
        /// 1,IPEndPoint    IPEndPoint-->the storage IPEndPoint
        /// 2,string groupName
        /// 3,string fileName
        /// </param>
        /// <returns></returns>
        public override FdfsRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length is not 3)
                throw new FdfsException("param count is wrong");

            var endPoint = (IPEndPoint) paramList[0];

            var groupName = (string) paramList[1];
            var fileName = (string) paramList[2];

            var result = new DeleteFile {Connection = ConnectionManager.GetStorageConnection(endPoint)};

            if (groupName.Length > Consts.FdfsGroupNameMaxLen)
                throw new FdfsException("groupName is too long");

            long length = Consts.FdfsGroupNameMaxLen + fileName.Length;
            var bodyBuffer = new byte[length];
            var groupNameBuffer = Util.StringToByte(groupName);
            var fileNameBuffer = Util.StringToByte(fileName);

            Array.Copy(groupNameBuffer, 0, bodyBuffer, 0, groupNameBuffer.Length);
            Array.Copy(fileNameBuffer, 0, bodyBuffer, Consts.FdfsGroupNameMaxLen, fileNameBuffer.Length);

            result.Body = bodyBuffer;
            result.Header = new FdfsHeader(length, Consts.StorageProtoCmdDeleteFile, 0);
            return result;
        }

        public class Response
        {
            public Response(byte[] responseBody)
            {
            }
        }
    }
}