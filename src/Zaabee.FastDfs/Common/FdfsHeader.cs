using System;
using System.IO;

namespace Zaabee.FastDfs.Common
{
    public class FdfsHeader
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="length"></param>
        /// <param name="command"></param>
        /// <param name="status"></param>
        public FdfsHeader(long length, byte command, byte status)
        {
            Length = length;
            Command = command;
            Status = status;
        }

        public FdfsHeader(Stream stream)
        {
            var headerBuffer = new byte[Consts.FdfsProtoPkgLenSize + 2];

            var offset = stream.Read(headerBuffer, 0, headerBuffer.Length);
            while (offset < headerBuffer.Length)
                offset += stream.Read(headerBuffer, offset, headerBuffer.Length - offset);

            if (offset == 0)
                throw new FdfsException("Init Header Exeption : Cann't Read Stream");

            Length = Util.BufferToLong(headerBuffer, 0);
            Command = headerBuffer[Consts.FdfsProtoPkgLenSize];
            Status = headerBuffer[Consts.FdfsProtoPkgLenSize + 1];
        }

        /// <summary>
        ///     Pachage Length
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        ///     Command
        /// </summary>
        public byte Command { get; set; }

        /// <summary>
        ///     Status
        /// </summary>
        public byte Status { get; set; }

        public byte[] ToByte()
        {
            var result = new byte[Consts.FdfsProtoPkgLenSize + 2];
            var pkgLen = Util.LongToBuffer(Length);
            Array.Copy(pkgLen, 0, result, 0, pkgLen.Length);
            result[Consts.FdfsProtoPkgLenSize] = Command;
            result[Consts.FdfsProtoPkgLenSize + 1] = Status;
            return result;
        }
    }
}