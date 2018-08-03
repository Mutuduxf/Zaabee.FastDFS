using System;

namespace Zaabee.FastDfsClient.Common
{
    public class FdfsException : Exception
    {
        public FdfsException(string msg) : base(msg)
        {
        }
        public int ErrorCode { get; set; }
    }
}