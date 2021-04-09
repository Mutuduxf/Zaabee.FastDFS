using System.Text;

namespace Zaabee.FastDfs.Common
{
    public static class FdfsConfig
    {
        public const int StorageMaxConnection = 20;
        public const int TrackerMaxConnection = 10;
//        public const int ConnectionTimeout = 5; //Second
        public const int ConnectionLifeTime = 3600;
        public static readonly Encoding Charset = Encoding.UTF8;
    }
}