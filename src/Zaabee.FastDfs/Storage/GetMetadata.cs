using Zaabee.FastDfs.Common;

namespace Zaabee.FastDfs.Storage
{
    /// <summary>
    /// get metat data from storage server
    /// Reqeust
    /// Cmd: STORAGE_PROTO_CMD_GET_METADATA 15
    /// Body:
    /// @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    /// @ filename bytes: filename
    /// Response
    /// Cmd: STORAGE_PROTO_CMD_RESP
    /// Status: 0 right other wrong
    /// Body:
    /// @ meta data buff, each meta data seperated by \x01, name and value seperated by \x02
    /// </summary>
    internal class GetMetadata : FdfsRequest
    {
        private GetMetadata()
        {
        }
    }
}