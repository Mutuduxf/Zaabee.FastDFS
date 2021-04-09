using System;
using Zaabee.FastDfs.Common;

namespace Zaabee.FastDfs.Tracker
{
    /// <summary>
    /// query which storage server to store file
    /// 
    /// Reqeust 
    /// Cmd: TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ONE 104
    /// Body: 
    /// @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    /// Response
    /// Cmd: TRACKER_PROTO_CMD_RESP
    /// Status: 0 right other wrong
    /// Body: 
    /// @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    /// @ IP_ADDRESS_SIZE - 1 bytes: storage server ip address
    /// @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server port
    /// @ 1 byte: store path index on the storage server
    /// </summary>
    internal class QueryStoreWithGroupOne : FdfsRequest
    {
        #region 单例对象

        public static QueryStoreWithGroupOne Instance { get; } = new();

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramList">
        /// 1,string groupName-->the storage groupName
        /// </param>
        /// <returns></returns>
        public override FdfsRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length == 0)
                throw new FdfsException("GroupName is null");

            var result = new QueryStoreWithGroupOne();

            var groupName = Util.StringToByte((string) paramList[0]);
            if (groupName.Length > Consts.FdfsGroupNameMaxLen)
                throw new FdfsException("GroupName is too long");

            var body = new byte[Consts.FdfsGroupNameMaxLen];
            Array.Copy(groupName, 0, body, 0, groupName.Length);
            result.Body = body;
            result.Header = new FdfsHeader(Consts.FdfsGroupNameMaxLen,
                Consts.TrackerProtoCmdServiceQueryStoreWithGroupOne, 0);

            return result;
        }

        public class Response
        {
            public string GroupName;
            public string IpStr;
            public int Port;
            public byte StorePathIndex;

            public Response(byte[] responseByte)
            {
                var groupNameBuffer = new byte[Consts.FdfsGroupNameMaxLen];

                Array.Copy(responseByte, groupNameBuffer, Consts.FdfsGroupNameMaxLen);

                GroupName = Util.ByteToString(groupNameBuffer).TrimEnd('\0');

                var ipAddressBuffer = new byte[Consts.IpAddressSize - 1];

                Array.Copy(responseByte, Consts.FdfsGroupNameMaxLen, ipAddressBuffer, 0, Consts.IpAddressSize - 1);

                IpStr = new string(FdfsConfig.Charset.GetChars(ipAddressBuffer)).TrimEnd('\0');

                var portBuffer = new byte[Consts.FdfsProtoPkgLenSize];

                Array.Copy(responseByte, Consts.FdfsGroupNameMaxLen + Consts.IpAddressSize - 1, portBuffer, 0,
                    Consts.FdfsProtoPkgLenSize);

                Port = (int) Util.BufferToLong(portBuffer, 0);

                StorePathIndex = responseByte[responseByte.Length - 1];
            }
        }
    }
}