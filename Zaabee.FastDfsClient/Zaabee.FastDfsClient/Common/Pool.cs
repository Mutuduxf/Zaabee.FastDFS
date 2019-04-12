using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace Zaabee.FastDfsClient.Common
{
    /// <summary>
    /// 池
    /// </summary>
    internal class Pool
    {
        private readonly List<Connection> _inUse;
        private Stack<Connection> _idle;
        private readonly AutoResetEvent _autoEvent;
        private readonly int _maxConnection;

        public IPEndPoint IpEndPoint { get; }

        public Pool(IPEndPoint endPoint, int maxConnection)
        {
            _autoEvent = new AutoResetEvent(false);
            _inUse = new List<Connection>(maxConnection);
            _idle = new Stack<Connection>(maxConnection);
            _maxConnection = maxConnection;
            IpEndPoint = endPoint;
        }

        private Connection GetPooldConncetion()
        {
            Connection result = null;
            lock ((_idle as ICollection).SyncRoot)
            {
                if (_idle.Count > 0)
                    result = _idle.Pop();
                if (result != null && (int) (DateTime.Now - result.LastUseTime).TotalSeconds >
                    FdfsConfig.ConnectionLifeTime)
                {
                    foreach (var conn in _idle)
                        conn.CloseConnection();
                    _idle = new Stack<Connection>(_maxConnection);
                    result = null;
                }
            }

            lock ((_inUse as ICollection).SyncRoot)
            {
                if (_inUse.Count == _maxConnection)
                    return null;
                if (result == null)
                {
                    result = new Connection();
                    result.Connect(IpEndPoint);
                    result.Pool = this;
                }

                _inUse.Add(result);
            }

            return result;
        }

        public Connection GetConnection(int connectionTimeout)
        {
            var timeOut = connectionTimeout * 1000;

            var watch = Stopwatch.StartNew();
            while (timeOut > 0)
            {
                var result = GetPooldConncetion();

                if (result != null)
                    return result;

                if (!_autoEvent.WaitOne(timeOut, false))
                    break;

                watch.Stop();

                timeOut = timeOut - (int) watch.ElapsedMilliseconds;
            }

            throw new FdfsException("Connection Time Out");
        }

        public void ReleaseConnection(Connection conn)
        {
            if (!conn.InUse)
            {
                var header = new FdfsHeader(0, Consts.FdfsProtoCmdQuit, 0);
                var buffer = header.ToByte();
                conn.GetStream().Write(buffer, 0, buffer.Length);
                conn.GetStream().Close();
            }

            conn.Close();

            lock ((_inUse as ICollection).SyncRoot)
                _inUse.Remove(conn);
            _autoEvent.Set();
        }

        public void CloseConnection(Connection conn)
        {
            conn.InUse = false;
            lock ((_inUse as ICollection).SyncRoot)
                _inUse.Remove(conn);
            lock ((_idle as ICollection).SyncRoot)
                _idle.Push(conn);
            _autoEvent.Set();
        }
    }
}