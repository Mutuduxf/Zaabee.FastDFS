using System;
using System.Net.Sockets;

namespace Zaabee.FastDfsClient.Common
{
    internal class Connection : TcpClient
    {
        public Connection()
        {
            InUse = false;
        }

        public Pool Pool { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastUseTime { get; set; }

        public bool InUse { get; set; }

        public void OpenConnection()
        {
            if (InUse)
            {
                throw new FdfsException("the connection is already in user");
            }

            InUse = true;
            LastUseTime = DateTime.Now;

            //如果连接处于关闭状态，则重新打开连接管道
            if (Connected == false)
            {
                Connect(Pool.IpEndPoint);
            }
        }

        public void CloseConnection()
        {
            Pool.CloseConnection(this);
        }

        public void ReleaseConnection()
        {
            Pool.ReleaseConnection(this);
            Dispose(true);
        }
    }
}