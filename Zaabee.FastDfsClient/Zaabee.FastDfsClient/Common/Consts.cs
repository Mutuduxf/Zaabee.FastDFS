namespace Zaabee.FastDfsClient.Common
{
    public static class Consts
    {
        //Args
        public const byte FdfsFileExtNameMaxLen = 6; // common/fdfs_global.h
        public const byte FdfsVersionSize = 6;

        public const byte FdfsProtoPkgLenSize = 8;

        public const byte IpAddressSize = 16;
        public const byte FdfsFilePrefixMaxLen = 16; // tracker/tracker_types.h
        public const byte FdfsGroupNameMaxLen = 16;

        public const short FdfsDomainNameMaxSize = 128;

        //Command-Tracker
        public const byte TrackerProtoCmdServerListOneGroup = 90;
        public const byte TrackerProtoCmdServerListAllGroups = 91;
        public const byte TrackerProtoCmdServerListStorage = 92;
        public const byte TrackerProtoCmdServiceQueryStoreWithoutGroupOne = 101;
        public const byte TrackerProtoCmdServiceQueryFetchOne = 102;
        public const byte TrackerProtoCmdServiceQueryUpdate = 103;
        public const byte TrackerProtoCmdServiceQueryStoreWithGroupOne = 104;
        public const byte TrackerProtoCmdServiceQueryFetchAll = 105;
        public const byte TrackerProtoCmdServiceQueryStoreWithoutGroupAll = 106;
        public const byte TrackerProtoCmdServiceQueryStoreWithGroupAll = 107;

        //Command-Storage
        public const byte StorageProtoCmdUploadFile = 11;
        public const byte StorageProtoCmdDeleteFile = 12;
        public const byte StorageProtoCmdSetMetadata = 13;
        public const byte StorageProtoCmdDownloadFile = 14;
        public const byte StorageProtoCmdGetMetadata = 15;
        public const byte StorageProtoCmdUploadSlaveFile = 21;
        public const byte StorageProtoCmdQueryFileInfo = 22;
        public const byte StorageProtoCmdUploadAppenderFile = 23;
        public const byte StorageProtoCmdAppendFile = 24;
        public const byte StorageProtoCmdModifyFile = 34; //modify appender file  3.06新增特性
        public const byte StorageProtoCmdTruncateFile = 36; //truncate appender file 3.06新增特性

        //Exit
        public const byte FdfsProtoCmdQuit = 82; //未确认
    }
}