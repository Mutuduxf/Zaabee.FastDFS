using System;
using Zaabee.Cryptographic;

namespace Zaabee.FastDfs.Common
{
    public static class Util
    {
        /// <summary>
        /// Convert Long to byte[]
        /// </summary>
        /// <returns></returns>
        public static byte[] LongToBuffer(long l)
        {
            var buffer = new byte[8];
            buffer[0] = (byte) ((l >> 56) & 0xFF);
            buffer[1] = (byte) ((l >> 48) & 0xFF);
            buffer[2] = (byte) ((l >> 40) & 0xFF);
            buffer[3] = (byte) ((l >> 32) & 0xFF);
            buffer[4] = (byte) ((l >> 24) & 0xFF);
            buffer[5] = (byte) ((l >> 16) & 0xFF);
            buffer[6] = (byte) ((l >> 8) & 0xFF);
            buffer[7] = (byte) (l & 0xFF);

            return buffer;
        }

        /// <summary>
        /// Convert byte[] to Long
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static long BufferToLong(byte[] buffer, int offset)
        {
            return ((long) buffer[offset] << 56) |
                   ((long) buffer[offset + 1] << 48) |
                   ((long) buffer[offset + 2] << 40) |
                   ((long) buffer[offset + 3] << 32) |
                   ((long) buffer[offset + 4] << 24) |
                   ((long) buffer[offset + 5] << 16) |
                   ((long) buffer[offset + 6] << 8) |
                   buffer[offset + 7];
        }

        public static string ByteToString(byte[] input)
        {
            var chars = FdfsConfig.Charset.GetChars(input);
            var result = new string(chars, 0, input.Length);
            return result;
        }

        public static byte[] StringToByte(string input)
        {
            return FdfsConfig.Charset.GetBytes(input);
        }

        /// <summary>
        /// get token for file URL
        /// </summary>
        /// <param name="fileId">file_id the file id return by FastDFS server</param>
        /// <param name="ts">ts unix timestamp, unit: second</param>
        /// <param name="secretKey">secret_key the secret key</param>
        /// <returns>token string</returns>
        public static string GetToken(string fileId, int ts, string secretKey)
        {
            var bsFileId = StringToByte(fileId);
            var bsKey = StringToByte(secretKey);
            var bsTimestamp = StringToByte(ts.ToString());

            var buff = new byte[bsFileId.Length + bsKey.Length + bsTimestamp.Length];
            Array.Copy(bsFileId, 0, buff, 0, bsFileId.Length);
            Array.Copy(bsKey, 0, buff, bsFileId.Length, bsKey.Length);
            Array.Copy(bsTimestamp, 0, buff, bsFileId.Length + bsKey.Length, bsTimestamp.Length);

            return buff.ToMd5(false, true);
        }
    }
}