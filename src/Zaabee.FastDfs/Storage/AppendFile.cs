using System;
using System.Net;
using Zaabee.FastDfs.Common;

namespace Zaabee.FastDfs.Storage
{
    /// <summary>
    /// append file to storage server
    /// Reqeust
    /// Cmd: STORAGE_PROTO_CMD_APPEND_FILE 24
    /// Body:
    /// @ FDFS_PROTO_PKG_LEN_SIZE bytes: file name length
    /// @ FDFS_PROTO_PKG_LEN_SIZE bytes: append file body length
    /// @ file name
    /// @ append body
    /// Response
    /// Cmd: STORAGE_PROTO_CMD_RESP
    /// Status: 0 right other wrong
    /// Body:
    /// </summary>
    internal class AppendFile : FdfsRequest
    {
        #region 单例

        public static AppendFile Instance { get; } = new AppendFile();

        #endregion

        private AppendFile()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="paramList">
        /// 1,IPEndPoint    IPEndPoint-->the storage IPEndPoint
        /// 2,string        FileName
        /// 3,byte[]        File bytes
        /// </param>
        /// <returns></returns>
        public override FdfsRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length is not 3)
                throw new FdfsException("param count is wrong");
            var endPoint = (IPEndPoint) paramList[0];

            var fileName = (string) paramList[1];
            var contentBuffer = (byte[]) paramList[2];

            var result = new AppendFile {Connection = ConnectionManager.GetStorageConnection(endPoint)};

            long length = Consts.FdfsProtoPkgLenSize + Consts.FdfsProtoPkgLenSize + fileName.Length +
                          contentBuffer.Length;
            var bodyBuffer = new byte[length];

            var fileNameLenBuffer = Util.LongToBuffer(fileName.Length);
            Array.Copy(fileNameLenBuffer, 0, bodyBuffer, 0, fileNameLenBuffer.Length);

            var fileSizeBuffer = Util.LongToBuffer(contentBuffer.Length);
            Array.Copy(fileSizeBuffer, 0, bodyBuffer, Consts.FdfsProtoPkgLenSize, fileSizeBuffer.Length);

            var fileNameBuffer = Util.StringToByte(fileName);
            Array.Copy(fileNameBuffer, 0, bodyBuffer, Consts.FdfsProtoPkgLenSize + Consts.FdfsProtoPkgLenSize,
                fileNameBuffer.Length);

            Array.Copy(contentBuffer, 0, bodyBuffer,
                Consts.FdfsProtoPkgLenSize + Consts.FdfsProtoPkgLenSize + fileNameBuffer.Length,
                contentBuffer.Length);

            result.Body = bodyBuffer;
            result.Header = new FdfsHeader(length, Consts.StorageProtoCmdAppendFile, 0);
            return result;
        }

        public class Response
        {
            public string FileName;
            public string GroupName;

            public Response(byte[] responseBody)
            {
            }
        }
    }
}