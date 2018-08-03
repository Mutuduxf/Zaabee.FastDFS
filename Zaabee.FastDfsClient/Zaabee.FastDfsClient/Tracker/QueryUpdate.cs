using System;
using Zaabee.FastDfsClient.Common;

namespace Zaabee.FastDfsClient.Tracker
{
    /// <summary>
    ///     query which storage server to update the file
    ///     Reqeust
    ///     Cmd: TRACKER_PROTO_CMD_SERVICE_QUERY_UPDATE 103
    ///     Body:
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes:  group name
    ///     @ filename bytes: filename
    ///     Response
    ///     Cmd: TRACKER_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body:
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ IP_ADDRESS_SIZE - 1 bytes: storage server ip address
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server port
    /// </summary>
    internal class QueryUpdate : FdfsRequest
    {
        private QueryUpdate()
        {
        }

        public static QueryUpdate Instance { get; } = new QueryUpdate();

        /// <summary>
        /// </summary>
        /// <param name="paramList">
        ///     1,string groupName
        ///     2,string fileName
        /// </param>
        /// <returns></returns>
        public override FdfsRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length != 2)
                throw new FdfsException("param count is wrong");

            var result = new QueryUpdate();
            var groupName = (string) paramList[0];
            var fileName = (string) paramList[1];
            if (groupName.Length > Consts.FdfsGroupNameMaxLen)
                throw new FdfsException("GroupName is too long");

            var groupNameBuffer = Util.StringToByte(groupName);
            var fileNameBuffer = Util.StringToByte(fileName);
            var length = Consts.FdfsGroupNameMaxLen + fileNameBuffer.Length;
            var body = new byte[length];

            Array.Copy(groupNameBuffer, 0, body, 0, groupNameBuffer.Length);
            Array.Copy(fileNameBuffer, 0, body, Consts.FdfsGroupNameMaxLen, fileNameBuffer.Length);

            result.Body = body;
            result.Header = new FdfsHeader(length,
                Consts.TrackerProtoCmdServiceQueryUpdate, 0);
            return result;
        }

        public class Response
        {
            public string GroupName;
            public string IpStr;
            public int Port;

            public Response(byte[] responseByte)
            {
                var groupNameBuffer = new byte[Consts.FdfsGroupNameMaxLen];
                Array.Copy(responseByte, groupNameBuffer, Consts.FdfsGroupNameMaxLen);
                GroupName = Util.ByteToString(groupNameBuffer).TrimEnd('\0');
                var ipAddressBuffer = new byte[Consts.IpAddressSize - 1];
                Array.Copy(responseByte, Consts.FdfsGroupNameMaxLen, ipAddressBuffer, 0, Consts.IpAddressSize - 1);
                IpStr = new string(FdfsConfig.Charset.GetChars(ipAddressBuffer)).TrimEnd('\0');
                var portBuffer = new byte[Consts.FdfsProtoPkgLenSize];
                Array.Copy(responseByte, Consts.FdfsGroupNameMaxLen + Consts.IpAddressSize - 1,
                    portBuffer, 0, Consts.FdfsProtoPkgLenSize);
                Port = (int) Util.BufferToLong(portBuffer, 0);
            }
        }
    }
}