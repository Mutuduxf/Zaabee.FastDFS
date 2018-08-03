using System;

namespace Zaabee.FastDfsClient.Common
{
    public class FdfsFileInfo
    {
        public long Crc32;
        public DateTime CreateTime;
        public long FileSize;

        public FdfsFileInfo(byte[] responseByte)
        {
            var fileSizeBuffer = new byte[Consts.FdfsProtoPkgLenSize];
            var createTimeBuffer = new byte[Consts.FdfsProtoPkgLenSize];
            var crcBuffer = new byte[Consts.FdfsProtoPkgLenSize];

            Array.Copy(responseByte, 0, fileSizeBuffer, 0, fileSizeBuffer.Length);
            Array.Copy(responseByte, Consts.FdfsProtoPkgLenSize, createTimeBuffer, 0, createTimeBuffer.Length);
            Array.Copy(responseByte, Consts.FdfsProtoPkgLenSize + Consts.FdfsProtoPkgLenSize, crcBuffer, 0,
                crcBuffer.Length);

            FileSize = Util.BufferToLong(responseByte, 0);
            CreateTime =
                new DateTime(1970, 1, 1).AddSeconds(Util.BufferToLong(responseByte, Consts.FdfsProtoPkgLenSize));

            Crc32 = Util.BufferToLong(responseByte, Consts.FdfsProtoPkgLenSize + Consts.FdfsProtoPkgLenSize);
        }
    }
}
