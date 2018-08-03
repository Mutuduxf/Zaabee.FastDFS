using System;
using System.Net;
using Zaabee.FastDfsClient.Common;

namespace Zaabee.FastDfsClient.Storage
{
    /// <summary>
    ///     upload slave file to storage server
    ///     Reqeust
    ///     Cmd: STORAGE_PROTO_CMD_UPLOAD_SLAVE_FILE 21
    ///     Body:
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: master filename length
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file size
    ///     @ FDFS_FILE_PREFIX_MAX_LEN bytes: filename prefix
    ///     @ FDFS_FILE_EXT_NAME_MAX_LEN bytes: file ext name, do not include dot (.)
    ///     @ master filename bytes: master filename
    ///     @ file size bytes: file content
    ///     Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body:
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ filename bytes: filename
    /// </summary>
    internal class UploadSlaveFile : FdfsRequest
    {
        private UploadSlaveFile()
        {
        }

        public static UploadSlaveFile Instance { get; } = new UploadSlaveFile();

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
            if (paramList.Length != 6)
                throw new FdfsException("param count is wrong");

            var endPoint = (IPEndPoint) paramList[0];

            var fileSize = (int) paramList[1];
            var masterFilename = paramList[2].ToString();
            var prefixName = (string) paramList[3];
            var ext = (string) paramList[4];
            var contentBuffer = (byte[]) paramList[5];
            var masterFilenameBytes = Util.StringToByte(masterFilename);

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

            var result = new UploadSlaveFile {Connection = ConnectionManager.GetStorageConnection(endPoint)};
            if (ext.Length > Consts.FdfsFileExtNameMaxLen)
                throw new FdfsException("file ext is too long");

            var sizeBytes = new byte[2 * Consts.FdfsProtoPkgLenSize];

            long length = sizeBytes.Length + +Consts.FdfsFilePrefixMaxLen + Consts.FdfsFileExtNameMaxLen +
                          masterFilenameBytes.Length + contentBuffer.Length;

            var bodyBuffer = new byte[length];
            var hexLenBytes = Util.LongToBuffer(masterFilename.Length);
            var offset = hexLenBytes.Length;


            Array.Copy(hexLenBytes, 0, bodyBuffer, 0, hexLenBytes.Length);

            var fileSizeBuffer = Util.LongToBuffer(fileSize);
            Array.Copy(fileSizeBuffer, 0, bodyBuffer, offset, fileSizeBuffer.Length);

            offset = sizeBytes.Length;

            var prefixNameBs = new byte[Consts.FdfsFilePrefixMaxLen];
            var bs = Util.StringToByte(prefixName);
            var prefixNameLen = bs.Length;
            if (prefixNameLen > Consts.FdfsFilePrefixMaxLen)
            {
                prefixNameLen = Consts.FdfsFilePrefixMaxLen;
            }

            if (prefixNameLen > 0)
            {
                Array.Copy(bs, 0, prefixNameBs, 0, prefixNameLen);
            }

            Array.Copy(prefixNameBs, 0, bodyBuffer, offset, prefixNameBs.Length);

            offset += prefixNameBs.Length;

            Array.Copy(extBuffer, 0, bodyBuffer, offset, extBuffer.Length);

            offset += extBuffer.Length;

            Array.Copy(masterFilenameBytes, 0, bodyBuffer, offset, masterFilenameBytes.Length);
            offset += masterFilenameBytes.Length;

            Array.Copy(contentBuffer, 0, bodyBuffer, offset, contentBuffer.Length);

            result.Body = bodyBuffer;
            result.Header = new FdfsHeader(length, Consts.StorageProtoCmdUploadSlaveFile, 0);
            return result;
        }

        public class Response
        {
            public string FileName;
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