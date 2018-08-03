using System;
using System.Security.Cryptography;

namespace Zaabee.FastDfsClient.Common
{
    public static class Util
    {
        /// <summary>
        ///     Convert Long to byte[]
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
        ///     Convert byte[] to Long
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
        ///     get token for file URL
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

            return Md5(buff);
        }

        /// <summary>
        ///     md5 function
        /// </summary>
        /// <param name="source">source the input buffer </param>
        /// <returns>md5 string </returns>
        private static string Md5(byte[] source)
        {
            var pwd = "";
            using (var md5 = MD5.Create())
            {
                // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
                var s = md5.ComputeHash(source);
                // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
                for (var i = 0; i < s.Length; i++)
                    // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                    pwd = pwd + s[i].ToString("X2");
                return pwd.ToLower();
            }
        }
    }
}