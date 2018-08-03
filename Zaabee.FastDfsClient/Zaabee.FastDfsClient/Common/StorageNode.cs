using System.Net;

namespace Zaabee.FastDfsClient.Common
{
    public class StorageNode
    {
        public string GroupName { get; internal set; }
        public IPEndPoint EndPoint { get; internal set; }
        public byte StorePathIndex { get; internal set; }
    }
}