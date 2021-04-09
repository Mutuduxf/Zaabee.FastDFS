using System;

namespace Zaabee.FastDfs.Common
{
    public class FdfsException : Exception
    {
        public FdfsException(string msg) : base(msg)
        {
        }
        public int ErrorCode { get; set; }
    }
}