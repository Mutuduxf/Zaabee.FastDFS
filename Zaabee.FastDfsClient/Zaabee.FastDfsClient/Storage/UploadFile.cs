using System;
using System.Net;
using Zaabee.FastDfsClient.Common;

namespace Zaabee.FastDfsClient.Storage
{
    /// <summary>
    ///     upload file to storage server
    ///     Reqeust
    ///     Cmd: STORAGE_PROTO_CMD_UPLOAD_FILE 11
    ///     Body:
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: filename size
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file bytes size
    ///     @ filename
    ///     @ file bytes: file content
    ///     Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body:
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ filename bytes: filename
    /// </summary>
    internal class UploadFile : FdfsRequest
    {
        private UploadFile()
        {
        }

        public static UploadFile Instance { get; } = new UploadFile();

        /// <summary>
        /// </summary>
        /// <param name="paramList">
        ///     1,IPEndPoint    IPEndPoint-->the storage IPEndPoint
        ///     2,Byte          StorePathIndex
        ///     3,long          FileSize
        ///     4,string        File Ext
        ///     5,byte[FileSize]    File Content
        /// </param>
        /// <returns></returns>
        public override FdfsRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length != 5)
                throw new FdfsException("param count is wrong");

            var endPoint = (IPEndPoint) paramList[0];

            var storePathIndex = (byte) paramList[1];
            var fileSize = (int) paramList[2];
            var ext = (string) paramList[3];
            var contentBuffer = (byte[]) paramList[4];

            #region 拷贝后缀扩展名值

            var extBuffer = new byte[Consts.FdfsFileExtNameMaxLen];
            var bse = Util.StringToByte(ext);
            var extNameLen = bse.Length;
            if (extNameLen > Consts.FdfsFileExtNameMaxLen)
            {
                extNameLen = Consts.FdfsFileExtNameMaxLen;
            }

            Array.Copy(bse, 0, extBuffer, 0, extNameLen);

            #endregion

            var result = new UploadFile
            {
                Connection = ConnectionManager.GetStorageConnection(endPoint)
            };

            if (ext.Length > Consts.FdfsFileExtNameMaxLen)
                throw new FdfsException("file ext is too long");

            long length = 1 + Consts.FdfsProtoPkgLenSize + Consts.FdfsFileExtNameMaxLen + contentBuffer.Length;
            var bodyBuffer = new byte[length];
            bodyBuffer[0] = storePathIndex;

            var fileSizeBuffer = Util.LongToBuffer(fileSize);

            Array.Copy(fileSizeBuffer, 0, bodyBuffer, 1, fileSizeBuffer.Length);

            Array.Copy(extBuffer, 0, bodyBuffer, 1 + Consts.FdfsProtoPkgLenSize, extBuffer.Length);

            Array.Copy(contentBuffer, 0, bodyBuffer,
                1 + Consts.FdfsProtoPkgLenSize + Consts.FdfsFileExtNameMaxLen, contentBuffer.Length);

            result.Body = bodyBuffer;
            result.Header = new FdfsHeader(length, Consts.StorageProtoCmdUploadFile, 0);

            return result;
        }

        public class Response
        {
            public readonly string FileName;
            public string GroupName;

            public Response(byte[] responseBody)
            {
                var groupNameBuffer = new byte[Consts.FdfsGroupNameMaxLen];
                Array.Copy(responseBody, groupNameBuffer, Consts.FdfsGroupNameMaxLen);
                GroupName = Util.ByteToString(groupNameBuffer).TrimEnd('\0');

                var fileNameBuffer = new byte[responseBody.Length - Consts.FdfsGroupNameMaxLen];
                Array.Copy(responseBody, Consts.FdfsGroupNameMaxLen, fileNameBuffer, 0, fileNameBuffer.Length);
                FileName = Util.ByteToString(fileNameBuffer).TrimEnd('\0');
            }
        }
    }
}