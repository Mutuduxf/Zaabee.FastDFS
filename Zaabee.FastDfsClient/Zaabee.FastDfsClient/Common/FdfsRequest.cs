using System;

namespace Zaabee.FastDfsClient.Common
{
    public class FdfsRequest
    {
        #region 公共属性

        internal FdfsHeader Header { get; set; }

        internal byte[] Body { get; set; }

        internal Connection Connection { get; set; }

        #endregion

        #region 公共方法

        public byte[] ToByteArray()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 公共虚方法

        public virtual FdfsRequest GetRequest(params object[] paramList)
        {
            throw new NotImplementedException();
        }

        public byte[] GetResponse()
        {
            if (Connection == null)
                Connection = ConnectionManager.GetTrackerConnection();

            try
            {
                //打开
                Connection.OpenConnection();
                var stream = Connection.GetStream();
                var headerBuffer = Header.ToByte();

                stream.Write(headerBuffer, 0, headerBuffer.Length);
                stream.Write(Body, 0, Body.Length);

                var header = new FdfsHeader(stream);
                if (header.Status != 0)
                {
                    var fdfsEx = new FdfsException($"Get Response Error,Error Code:{header.Status}")
                    {
                        ErrorCode = header.Status
                    };
                    throw fdfsEx;
                }

                var length = (int) header.Length;
                var body = new byte[length];
                if (length == 0) return body;

                var offset = stream.Read(body, 0, length);
                while (offset < length)
                    offset += stream.Read(body, offset, length - offset);

                return body;
            }
            finally
            {
                //关闭
                //Connection.Close();
                Connection.ReleaseConnection();
            }
        }

        #endregion
    }
}