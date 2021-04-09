using System;
using System.Security.Cryptography;

namespace Zaabee.FastDfsProvider.UnitTest
{
    public static class Md5Helper
    {
        /// <summary>
        /// Get 32bit MD5 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="isUpper"></param>
        /// <param name="isIncludHyphen"></param>
        /// <returns></returns>
        public static string Get32Md5(byte[] bytes, bool isUpper = true, bool isIncludHyphen = false)
        {
            using (var provider = MD5.Create())
                bytes = provider.ComputeHash(bytes);
            var str = BitConverter.ToString(bytes);
            str = isUpper ? str.ToUpper() : str.ToLower();
            str = isIncludHyphen ? str : str.Replace("-", "");
            return str;
        }
    }
}