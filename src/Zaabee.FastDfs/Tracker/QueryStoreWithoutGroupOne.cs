using Zaabee.FastDfs.Common;

namespace Zaabee.FastDfs.Tracker
{
    /// <summary>
    /// query which storage server to store file
    /// 
    /// Reqeust 
    /// Cmd: TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITHOUT_GROUP_ONE 101
    /// Body: 
    /// 
    /// Response
    /// Cmd: TRACKER_PROTO_CMD_RESP
    /// Status: 0 right other wrong
    /// Body: 
    /// @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    /// @ IP_ADDRESS_SIZE - 1 bytes: storage server ip address
    /// @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server port
    /// @ 1 byte: store path index on the storage server
    /// </summary>
    internal class QueryStoreWithoutGroupOne : FdfsRequest
    {
        private QueryStoreWithoutGroupOne()
        {

        }
    }
}